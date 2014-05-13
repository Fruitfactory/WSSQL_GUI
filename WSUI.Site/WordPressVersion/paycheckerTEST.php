<?php
include(getcwd().'/wp-content/themes/justlanded/PaymentSettingsTEST.php');

function ValidateMB()
{
	// Check product ID , Amount , Currency , Recivers email
	global $AppPrice, $Currency, $MBEmail, $SecretWord;

	debug_log('Validating Moneybookers order', true);

	// Status codes (see: 2.3.7 Detailed status description) :
	//-3 chargeback / -2 failed / 2 processed / 0 pending / -1 cancelled
	if ($_POST['status'] != 2)
	{
		debug_log('Status was not "2": '.$_POST['status'],false);
		return false;
	}

	// make sure we're getting the money
	if ($_POST['pay_to_email'] != $MBEmail)
	{
		debug_log('Invalid Reciver E-Mail : '.$_POST['pay_to_email'],false);
		return false;
	}

	// multiply Price * Quantity without using nasty lossy floats
	$exected_price = ($AppPrice[0] * $_POST['quantity'] + (int)(($AppPrice[1] * $_POST['quantity']) / 100)).'.'.str_pad((($AppPrice[1] * $_POST['quantity']) % 100), 2, '0', STR_PAD_LEFT);

	// validate the price
	if ($Currency != $_POST['currency'] || $exected_price != $_POST['amount'])
	{
		debug_log('Difference in price. Expected = '.$exected_price." $Currency | Paid = ".$_POST['amount'].' '.$_POST['currency'], false);
		return false;
	}

	// Validate the Moneybookers signature
	$concatFields = $_POST['merchant_id'].$_POST['transaction_id'].strtoupper(md5($SecretWord)).$_POST['mb_amount'].$_POST['mb_currency'].$_POST['status'];

	if (strtoupper(md5($concatFields)) == $_POST['md5sig'])
	{
		// Valid transaction.
		debug_log('IPN successfully verified.',true);
		return true;
	}
	else
	{
		// Invalid transaction. Check the log for details.
		debug_log('IPN validation failed.',false);
		return false;
	}
}

function ValidatePP()
{
	// Check product ID , Amount , Currency , Recivers email
	global $AppPrice, $Currency, $PayPalEmail, $PayPalSandbox;

	debug_log('Validating PayPal order '.$_POST['item_number'], true);

	// Only send a product key if the payment has completed successfully
        debug_log('PAYMENT_STATUS: '.$_POST['payment_status']);
	if ($_POST['payment_status'] != 'Completed')
	{
		debug_log('Invalid payment_status: '.$_POST['payment_status'],false);
		return false;
	}
        debug_log('RECEIVER EMAIL: '.$_POST['receiver_email']);
	if ($_POST['receiver_email'] != $PayPalEmail)
	{
		debug_log('Invalid Reciver E-Mail : '.$_POST['reciver_email'],false);
		return false;
	}

	$exected_price = ($AppPrice[0] * $_POST['quantity'] + (int)(($AppPrice[1] * $_POST['quantity']) / 100)).'.'.str_pad((($AppPrice[1] * $_POST['quantity']) % 100), 2, '0', STR_PAD_LEFT);

	if ($Currency != $_POST['mc_currency'] || $exected_price != $_POST['mc_gross'])
	{
		debug_log('Difference in price. Expected = '.$exected_price." $Currency | Paid = ".$_POST['mc_gross'].' '.$_POST['mc_currency'], false);
		return false;
	}

	// generate the post string we'll be using to validate the info with PayPal's servers
	$post_string = http_build_query($_POST).'&cmd=_notify-validate';

	if ($PayPalSandbox)
		$request = curl_init('https://www.sandbox.paypal.com/cgi-bin/webscr');
	else
		$request = curl_init('https://www.paypal.com/cgi-bin/webscr');

	curl_setopt($request, CURLOPT_HEADER, 0); // eliminate header info from response
	curl_setopt($request, CURLOPT_ENCODING, ""); // support gzip & deflate responses if available
	curl_setopt($request, CURLOPT_RETURNTRANSFER, 1); // Returns response data instead of TRUE(1)
	curl_setopt($request, CURLOPT_POSTFIELDS, $post_string); // use HTTP POST to send form data
	curl_setopt($request, CURLOPT_SSL_VERIFYPEER, FALSE); // uncomment this line if you get no gateway response.

	$ipn_response = curl_exec($request);

	curl_close($request);

	if (stristr($ipn_response, 'VERIFIED'))
	{
		// Valid transaction.
		debug_log('IPN successfully verified.',true);
		return true;
	}
	else
	{
		// Invalid transaction. Check the log for details.
		debug_log('IPN validation failed.',false);
		return false;
	}
}

// Begin checking :
debug_log('paychecker intiated by '.$_SERVER['REMOTE_ADDR'], true);
$userEmail = $_GET['userEmail'];

if ($_GET['paypal'])
{
	 //validate PayPal order
	if (!ValidatePP())
		exit;

	$quantity = $_POST['quantity'];
	$firstName = $_POST['first_name'];
	$lastName = $_POST['last_name'];
	$custEmail = $_POST['payer_email'];
}
else if ($_GET['moneybookers'])
{
	// validate Moneybookers order
	if (!ValidateMB())
		exit;

	$quantity = $_POST['quantity'];
	$firstName = $_POST['first_name'];
	$lastName = $_POST['last_name'];
	$custEmail = $_POST['pay_from_email'];
}
else
	exit;


// Client has successfully paid for the product
debug_log('Creating product Information to send.',true);

// This calls the function in PaymentSettings.php that
// creates the product keys and send them to the user
if($userEmail)
{
    UpdateLicensing($userEmail);
}
else
    SendPKeys($quantity, $custEmail, $firstName, $lastName);

debug_log('paychecker finished.',true,true);
?>