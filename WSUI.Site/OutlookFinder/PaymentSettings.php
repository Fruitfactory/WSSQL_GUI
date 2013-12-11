<?php
//TODO: Set the following constants. All URLs must be publicly accessible.
// See http://wyday.com/limelm/help/how-to-generate-product-keys-after-order/
// to learn how to configure the payment form and set everything up.

//==== General Config ====

//TODO: disable debug & sandbox when you're using this on your live site.

// Enable debug logging
$debug = true;
$debug_log = dirname(__FILE__).'/payment_debug.log';


// Set any of the following 4 boolean values to either
// true or false based on whether you're using that
// particular payment method. "payment.php" automatically
// shows or hides the payment method based on these boolean
// values.

// If you set a payment setting to true then you'll also
// have to configure the payment settings for that
// particular payment method below.
$UseAuthorizeNetCC = true;
$UseAuthorizeNetBank = true;
$UseMoneybookers = true;
$UsePayPal = true;


// dollars, cents
$AppPrice = array(69, 99);
$Currency = 'USD';

// The currency symbol that the end-user will see.
// If you're not using "$" then you should use
// the HTML-entity equivalent of the symbol.
// Your customers will see the symbol.

// � = &euro;
// � = &pound;
// � = &yen;
// etc.
$CurrencySign = '$';

$CompanyName = 'YourCompany';
$AppName = 'YourApp';

// api key found in http://wyday.com/limelm/settings/
$LimeLM_ApiKey = 'PASTE THE API KEY HERE';

// the version id is found in the url.
// For example http://wyday.com/limelm/version/100/   the version id is '100'.
$LimeLM_VersionID = 'PASTE THE VERSION ID';

// URL of the "paychecker.php" script.
// This is where Moneybookers and PayPal orders are confirmed and processed.
// If you're not using Moneybookers or PayPal then you don't have to set this.
$CheckScript = 'http://example.com/buy/paychecker.php';

// Where the user can buy your products
$BuyPage = 'http://example.com/buy/payment.php';

// Thank you page (once payment is made, user is sent to this page)
$ThankYouPage = 'http://example.com/buy/pay-thankyou.php';

// The logo to display on the PayPal / Moneybookers checkout
// this site must be HTTPS or it won't display.
// Maximum size (PayPal): 150px (width) by 50px (height)
// Maximum size (Moneybookers): 200px (width) by 50px (height)
$YourLogo; // = 'https://example.com/buy/toplogo.png';




//==== Authorize.Net Config ====

// the API Login ID and Transaction Key must be replaced with valid values
$AuthNetLogin = 'PASTE LOGIN HERE';
$AuthNetTansKey = 'PASTE Transaction Key HERE';
$AuthNetTest = true;




//==== PayPal Config ====

// Use PayPal Sandbox (set to false on your live server)
$PayPalSandbox = true;

// Paypal Email
$PayPalEmail = 'your@email.com';




//==== Moneybookers Config ====

// Your Moneybookers email
$MBEmail = 'your@email.com';

// Set the "Secret Word" in your Moneybookers account
// on the "Merchant tools" page. Then set it here.
$SecretWord = 'PASTE YOUR SECRET WORD HERE';




function debug_log($message, $success, $end = false)
{
	global $debug, $debug_log;

	if (!$debug || !$debug_log)
		return;

	// Timestamp
	$text = '['.date('m/d/Y g:i A').'] - '.(($success)?'SUCCESS :':'FAILURE :').$message. "\n";

	if ($end)
		$text .= "\n------------------------------------------------------------------\n\n";

	// Write to log
	$fp=fopen($debug_log, 'a');
	fwrite($fp, $text);
	fclose($fp);
}

function SendPKeys($quantity, $email, $first, $last)
{
	//Note: we put LimeLM in this directory. Change it as needed.
	require('LimeLM.php');

	global $LimeLM_VersionID, $LimeLM_ApiKey;

	$errors = false;

	// set your API key
	LimeLM::SetAPIKey($LimeLM_ApiKey);

	try
	{
		// Generate the product key - set the number of activations using the quantity
		$xml = new SimpleXMLElement(LimeLM::GeneratePKeys($LimeLM_VersionID, 1, $quantity, $email));

		if ($xml['stat'] == 'ok')
		{
			foreach ($xml->pkeys->pkey as $pkey)
			{
				// add a newline if you're generating more than one key
				if ($product_keys)
					$product_keys .= "\r\n";

				// set the product key
				$product_keys .= $pkey['key']."\r\n";
			}
		}
		else //failure
		{
			// use the error code & message
			debug_log('Failed to generate product keys: ('.$xml->err['code'].') '.$xml->err['msg'],false);
		}
	}
	catch (Exception $e)
	{
		debug_log('Failed to generate product keys, caught exception: '.$e->getMessage(),false);
	}

	if (empty($product_keys))
	{
		// the product key didn't generate, don't send e-mail to the customer yet.
		$errors = true;
	}

	// form the customer name
	$customerName = $first;

	if (!empty($last))
		// append the last name
		$customerName .= ' '.$last;

	$emailBody = $customerName.',

    Thank you for your order. Your product key is:

'.$product_keys.'


This product key is licensed for '.$quantity.( $quantity > 1 ? ' users' : ' user' ).' of '.$AppName.'.

Thank you,

'.$CompanyName;

	if (!empty($product_keys))
	{
		// Send Email to the buyer
		$emailSent = mail($email, 'Your '.$AppName.' product key', $emailBody, $headers);
	}

	// generate an array you can insert into your database
	$new_order_info = array(
		'quantity_licenses'		=> $quantity,
		'pkey'					=> $product_keys,
		'pkey_emailed'			=> $emailSent ? '1' : '0', // 0 = no, 1 = yes
		'first_name'			=> $first,
		'last_name'				=> $last,
		'email'					=> $email
	);

	//TODO: delete logging, or log to a safe location (not accessible to outside world)
	debug_log('TODO: Insert array into db: '."\r\n\r\n".print_r($new_order_info, true), true);

	if (!$emailSent)
	{
		$errors = true;
		debug_log('Error sending product Email to '.$email.'.',false);
	}

	LimeLM::CleanUp();
}
?>