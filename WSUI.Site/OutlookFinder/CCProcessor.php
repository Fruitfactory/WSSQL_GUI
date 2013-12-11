<?php
class CCProcessor
{
	public $errors;

	public $first_name;
	public $fname_err;

	public $last_name;
	public $lname_err;

	public $email;
	public $email_err;

	public $quantity = 1;
	public $quantity_err;

	public $is_cc_charge = true;


	// Credit card fields / errors
	public $cc_num;
	public $cc_num_err;
	public $cc_num_err_str;

	public $cc_adr;
	public $cc_adr_err;

	public $cc_zip;
	public $cc_zip_err;

	public $cc_expires_m;
	public $cc_expires_y;
	public $cc_exp_err;


	// Bank transfer fields / errors
	public $bank_err_str;

	public $bank_acct_name;
	public $bank_acct_name_err;

	public $bank_acct_num;
	public $bank_acct_num_err;

	public $bank_acct_type;
	public $bank_acct_type_err;

	public $bank_name;
	public $bank_name_err;

	public $bank_route_num;
	public $bank_route_num_err;


	public function process()
	{
		// first clean all the fields
		$this->first_name = trim($_POST['first_name']);
		$this->last_name = trim($_POST['last_name']);
		$this->email = trim($_POST['email']);

		$this->quantity = (int)$_POST['quantity'];

		// validate the first name
		if (strlen($this->first_name) == 0)
		{
			$this->fname_err = true;
			$this->errors = true;
		}

		// validate the last name
		if (strlen($this->last_name) == 0)
		{
			$this->lname_err = true;
			$this->errors = true;
		}

		// validate the e-mail address
		if (strlen($this->email) == 0)
		{
			$this->email_err = true;
			$this->errors = true;
		}

		$this->is_cc_charge = $_POST['mf_cardtype'] == 'Card';

		if ($this->is_cc_charge)
		{
			$this->cc_num = preg_replace('/[\D]/', '', $_POST['cc_num']);
			$this->cc_adr = trim($_POST['cc_adr']);
			$this->cc_zip = trim($_POST['cc_zip']);
			$this->cc_expires_m = (int)$_POST['cc_expires_m'];
			$this->cc_expires_y = (int)$_POST['cc_expires_y'];

			if (strlen($this->cc_zip) == 0)
			{
				$this->cc_zip_err = true;
				$this->errors = true;
			}

			if (strlen($this->cc_adr) == 0)
			{
				$this->cc_adr_err = true;
				$this->errors = true;
			}

			if ($this->cc_expires_m < 1 || $this->cc_expires_m > 12)
			{
				// Pick a month between January through December
				$this->cc_exp_err = true;
				$this->errors = true;
			}

			$time = time();
			$year = (int)date('Y', $time);

			// check for expired ccs
			if ($this->cc_expires_y < $year || ($this->cc_expires_y == $year && $this->cc_expires_m < date('n', $time)))
			{
				// Your card has expired. Select a card that expires on a later date.
				$this->cc_exp_err = true;
				$this->errors = true;
			}


			// validate credit card for typos
			// (this doesn't guarantee the CC is real - just that it passes the Luhn algorithm)

			if (strlen($this->cc_num) == 0 || !self::IsLuhnValid($this->cc_num))
			{
				$this->cc_num_err_str = 'Enter a valid credit card number';
				$this->cc_num_err = true;
				$this->errors = true;
			}
			else if (!self::IsAcceptedCC($this->cc_num))
			{
				$this->cc_num_err_str = 'We only accept '.self::AcceptedString();
				$this->cc_num_err = true;
				$this->errors = true;
			}
		}
		else // Bank transfer
		{
			// clean & verify the bank fields
			$this->bank_acct_name = trim($_POST['bank_acct_name']);
			$this->bank_acct_type = trim($_POST['bank_acct_type']);
			$this->bank_name = trim($_POST['bank_name']);
			$this->bank_acct_num = preg_replace('/[\D]/', '', $_POST['bank_acct_num']);
			$this->bank_route_num = preg_replace('/[\D]/', '', $_POST['bank_route_num']);

			if (strlen($this->bank_acct_name) == 0)
			{
				$this->bank_acct_name_err = true;
				$this->errors = true;
			}

			switch ($this->bank_acct_type)
			{
				case 'C':
					$long_acct_type = 'CHECKING';
					break;
				case 'S':
					$long_acct_type = 'SAVINGS';
					break;
				default:
					$this->bank_acct_type_err = true;
					$this->errors = true;
					$this->bank_acct_type = null;
			}

			if (strlen($this->bank_name) == 0)
			{
				$this->bank_name_err = true;
				$this->errors = true;
			}

			if (strlen($this->bank_route_num) != 9 || !self::IsABARouteValid($this->bank_route_num))
			{
				$this->bank_route_num_err = true;
				$this->errors = true;
			}

			if (strlen($this->bank_acct_num) == 0)
			{
				$this->bank_acct_num_err = true;
				$this->errors = true;
			}
		}

		if ($this->quantity < 1)
		{
			$this->errors = true;
			$this->quantity_err = true;
			$this->quantity = 1;
		}

		if ($this->errors)
			return false;


		require 'AuthorizeNet.php';

		global $AppName, $AppPrice, $AuthNetLogin, $AuthNetTansKey, $AuthNetTest;

		// format in the XXX.YY format
		$amount = ($AppPrice[0] * $this->quantity + (int)(($AppPrice[1] * $this->quantity) / 100)).'.'.str_pad((($AppPrice[1] * $this->quantity) % 100), 2, '0', STR_PAD_LEFT);

		// set the Auth.Net details
		AuthorizeNet::SetLoginDetails($AuthNetLogin, $AuthNetTansKey, $AuthNetTest);

		// charge or transfer
		if ($this->is_cc_charge)
		{
			$ret = AuthorizeNet::Charge($this->cc_num, str_pad($this->cc_expires_m, 2, "0", STR_PAD_LEFT).'/'.$this->cc_expires_y, $amount, $this->first_name, $this->last_name, $this->cc_zip, $this->cc_adr, $AppName, $this->email);
		}
		else
		{
			$ret = AuthorizeNet::BankTransfer($this->bank_acct_num, $this->bank_route_num, $long_acct_type, $this->bank_name, $this->bank_acct_name, $amount, $this->first_name, $this->last_name, $AppName, $this->email);
		}

		if ($ret)
		{
			// show the error
			if ($this->is_cc_charge)
			{
				$this->cc_num_err_str = $ret;
				$this->cc_num_err = true;
			}
			else
			{
				$this->bank_err_str = $ret;
			}

			$this->errors = true;
			return false;
		}

		// generate the product key and email them to the user
		SendPKeys($this->quantity, $this->email, $this->first_name, $this->last_name);

		// return true if no errors have occurred
		return true;
	}

	public static function IsLuhnValid($value)
	{
		$length = strlen($value);

		// make sure it falls within the accepted range
		if ($length < 13 || $length > 19)
			return false;

		$sum    = 0;
		$weight = 2;

		for ($i = $length - 2; $i >= 0; $i--)
		{
			$digit = $weight * $value[$i];
			$sum += floor($digit / 10) + $digit % 10;
			$weight = $weight % 2 + 1;
		}

		// validate it passes the Luhn checksum algorithm
		if ((10 - $sum % 10) % 10 != $value[$length - 1])
			return false;

		return true;
	}

	public static function IsAcceptedCC($cc)
	{
		switch($cc[0])
		{
			case '1': // JCB starting with 1 or 2
			case '2':

				if (preg_match('/^(?:2131|1800)\d{11}$/', $cc))
					return true;
				else
					return false;

			case '3': // American Express or JCB

				if (preg_match('/^3[47][0-9]{13}$/', $cc))
					return true; //Amex
				else if (preg_match('/^(?:35)\d{14}$/', $cc))
					return true; //JCB
				else
					return false;

			case '4': // Visa

				if (preg_match('/^4[0-9]{12}(?:[0-9]{3})?$/', $cc))
					return true;
				else
					return false;

			case '5': // MasterCard

				if (preg_match('/^5[1-5][0-9]{14}$/', $cc))
					return true;
				else
					return false;

			case '6': // Discover

				if (preg_match('/^6(?:011|5[0-9]{2})[0-9]{12}$/', $cc))
					return true;
				else
					return false;
		}

		return false;
	}

	public static function IsABARouteValid($routeNum)
	{
		return $routeNum[8] == (7 * ($routeNum[0] + $routeNum[3] + $routeNum[6]) + 3 * ($routeNum[1] + $routeNum[4] + $routeNum[7]) + 9 * ($routeNum[2] + $routeNum[5])) % 10;
	}

	public static function AcceptedString()
	{
		return 'Visa, American Express, MasterCard, Discover, and JCB';
	}
}
?>