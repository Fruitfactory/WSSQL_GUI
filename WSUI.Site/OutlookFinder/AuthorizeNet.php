<?php
class AuthorizeNet
{
	private static $login;
	private static $trans_key;
	private static $test;

	public static function SetLoginDetails($login, $trans_key, $test)
	{
		self::$login = $login;
		self::$trans_key = $trans_key;
		self::$test = $test;
	}

	public static function BankTransfer($acct_num, $routing, $acct_type, $bank_name, $name_on_acct, $amount, $first_name, $last_name, $description, $email = null)
	{
		$post_values = array(
			'x_login'			=> self::$login,
			'x_tran_key'		=> self::$trans_key,

			'x_version'			=> '3.1',
			'x_delim_data'		=> 'TRUE',
			'x_delim_char'		=> '|',
			'x_relay_response'	=> 'FALSE',

			'x_type'				=> 'AUTH_CAPTURE',
			'x_method'				=> 'ECHECK',
			'x_recurring_billing'	=> 'FALSE',

			'x_bank_aba_code'	=> $routing,
			'x_bank_acct_num'	=> $acct_num,
			'x_bank_acct_type'	=> $acct_type,
			'x_bank_name'		=> str_replace('|', '', $bank_name),
			'x_bank_acct_name'	=> str_replace('|', '', $name_on_acct),
			'x_echeck_type'		=> 'WEB',

			'x_amount'			=> $amount,
			'x_description'		=> $description,

			'x_first_name'		=> str_replace('|', '', $first_name),
			'x_last_name'		=> str_replace('|', '', $last_name)
		);

		// get ip address
		if ($_SERVER['HTTP_CLIENT_IP'] != null)
            $ip = $_SERVER['HTTP_CLIENT_IP'];
        else if ($_SERVER['HTTP_X_FORWARDED_FOR'] != null)
            $ip = $_SERVER['HTTP_X_FORWARDED_FOR'];
        else
            $ip = $_SERVER['REMOTE_ADDR'];

		$post_values['x_customer_ip'] = $ip;

		// set the email
		if ($email)
			$post_values['x_email'] = $email;

		// This line takes the response and breaks it into an array using the specified delimiting character
		$response_array = explode($post_values['x_delim_char'], self::PostRequest($post_values));

		if ($response_array[0] == 1)
			return null;

		return $response_array[3] ? $response_array[3] : 'Failed to make the bank transfer.';
	}

	public static function Charge($cc, $exp_date, $amount, $first_name, $last_name, $zip, $street_address, $description, $email = null)
	{
		$post_values = array(
			'x_login'			=> self::$login,
			'x_tran_key'		=> self::$trans_key,

			'x_version'			=> '3.1',
			'x_delim_data'		=> 'TRUE',
			'x_delim_char'		=> '|',
			'x_relay_response'	=> 'FALSE',

			'x_type'			=> 'AUTH_CAPTURE',
			'x_method'			=> 'CC',
			'x_card_num'		=> $cc,
			'x_exp_date'		=> $exp_date,

			'x_amount'			=> $amount,
			'x_description'		=> $description,

			'x_first_name'		=> str_replace('|', '', $first_name),
			'x_last_name'		=> str_replace('|', '', $last_name),
			'x_address'			=> str_replace('|', '', $street_address),
			'x_zip'				=> str_replace('|', '', $zip)
		);

		// get ip address
		if ($_SERVER['HTTP_CLIENT_IP'] != null)
            $ip = $_SERVER['HTTP_CLIENT_IP'];
        else if ($_SERVER['HTTP_X_FORWARDED_FOR'] != null)
            $ip = $_SERVER['HTTP_X_FORWARDED_FOR'];
        else
            $ip = $_SERVER['REMOTE_ADDR'];

		$post_values['x_customer_ip'] = $ip;

		// set the email
		if ($email)
			$post_values['x_email'] = $email;

		// This line takes the response and breaks it into an array using the specified delimiting character
		$response_array = explode($post_values['x_delim_char'], self::PostRequest($post_values));

		if ($response_array[0] == 1)
			return null;

		return $response_array[3] ? $response_array[3] : 'Failed to charge the credit card.';
	}

	private static function PostRequest($post_values)
	{
		// This section takes the input fields and converts them to the proper format
		// for an http post.  For example: "x_login=username&x_tran_key=a1B2c3D4"
		$post_string = '';
		foreach ($post_values as $key => $value)
		{
			$post_string .= "$key=".urlencode($value).'&';
		}

		$post_string = rtrim($post_string, '& ');

		$request = curl_init(self::$test ? 'https://test.authorize.net/gateway/transact.dll' : 'https://secure.authorize.net/gateway/transact.dll');
		curl_setopt($request, CURLOPT_HEADER, 0); // eliminate header from response
		curl_setopt($request, CURLOPT_ENCODING, ""); // support gzip & deflate responses if available
		curl_setopt($request, CURLOPT_RETURNTRANSFER, 1); // Returns response data instead of TRUE(1)
		curl_setopt($request, CURLOPT_POSTFIELDS, $post_string); // use HTTP POST to send form data
		curl_setopt($request, CURLOPT_SSL_VERIFYPEER, FALSE); // uncomment this line if you get no gateway response.

		// get response
		$post_response = curl_exec($request);

		// close curl object
		curl_close($request);

		return $post_response;
	}
}
?>