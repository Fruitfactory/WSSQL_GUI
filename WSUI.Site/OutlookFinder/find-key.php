<?php
$fields_valid = false;
$success = false;
$submitting = false;

if (sizeof($_POST))
{
	require('LimeLM.php');

	//TODO: set your API key found in http://wyday.com/limelm/settings/
	LimeLM::SetAPIKey('1m115715277fb67cc5a51.73953699');

	//TODO: set the version id to generate & find keys for
	// the version id is found in the url. For example http://wyday.com/limelm/version/100/
	// the version id is 100.
	$version_id = '1432';

	$submitting = true;

	// if there's an email
	if (!empty($_POST['email']))
	{
		$fields_valid = true;

		try
		{
			// Find product keys using the email
			$xml = new SimpleXMLElement(LimeLM::FindPKey($version_id, $_POST['email']));

			if ($xml['stat'] == 'ok')
			{
				// list the product keys
				foreach ($xml->pkeys->pkey as $pkey)
				{
					if ($pkeys)
						$pkeys .= "\r\n";

					$pkeys .= $pkey['key'];
				}

				$emailBody = 'Your product key'.($xml->pkeys['total'] > 1 ? 's are' : ' is').':

	'.$pkeys;

				// email the user their product keys
				$emailSent = mail($_POST['email'], 'Your product '.($xml->pkeys['total'] > 1 ? 'keys' : 'key'), $emailBody);

				$success = true;
			}
			else //failure
			{
				$success = false;

				if ($xml->err['code'] == '1')
				{
					// tell the user that no product key was found for that email
					$pkey_not_found = true;
				}
				else // something else went wrong
				{
					//TODO, log the error or email yourself
					//$emailSent = mail('youremail@example.com', 'Error occurred', $xml->err['code'].' '.$xml->err['msg']);
				}
			}
		}
		catch (Exception $e)
		{
			$success = false;

			//TODO: log the error
		}
	}
	else 
	{
		$fields_valid = false;
	}

	LimeLM::CleanUp();
}
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en"> 
<head> 
	<title>Find product key</title>
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
</head>
<body>

	<h1>Find product key</h1>
	<p>A simple example to find the product keys using an email address.</p>

	<h2>Instructions</h2>
	<ol>
		<li>Set your LimeLM API key (see line 11).</li>
		<li>Set the version ID (see line 16).</li>
		<li>Customize your email message (see line 39)</li>
		<li>Set your email address or logging details for errors (see line 60)</li>
	</ol>

	<h2>Requirements</h2>
	<p>This script requires:</p>
	<ul>
		<li>PHP 5+ (we recommend php 5.3.x or greater).</li>
		<li>The curl extension (<strong>curl is<?php echo function_exists('curl_init') ? '' : ' <em>not</em>'; ?> installed on this server</strong>)</li>
		<li>The SimpleXML extension (<strong>simplexml is<?php echo extension_loaded('simplexml') ? '' : ' <em>not</em>'; ?> installed on this server</strong>)</li>
	</ul>

	<hr/>

<?php if ($success): ?>
	<h2>Product keys have been sent</h2>
	<p>We've found your product keys and sent them to your email address.</p>
<?php else: ?>

	<?php if ($submitting):?>
		<?php if (!$fields_valid): ?>
			<p><big><strong>Oops. You need to enter a valid email address.</strong></big></p>
		<?php elseif ($pkey_not_found): ?>
			<p><big><strong>No product keys were found for this email address. Use the email you used when you purchased the product.</strong></big></p>
		<?php else: ?>
			<p><big><strong>Something went wrong - we're investigating it.</strong></big></p>
		<?php endif; ?>
	<?php endif; ?>

	<form name="mailform" method="post" action="">
    <div>
		<table>
			<tr>
				<th><label for="form_email">Enter your email:</label></th>
				<td><input type="text" id="form_email" size="30"
					name="email" value="<?=isset($_POST['email']) ? $_POST['email'] : ''?>"/></td>
				<td><input type="submit" name="btnFindSerials" value="Find Serials"/></td>
			</tr>
		</table>
	</div>
	</form>

<script language="JavaScript">
<!--
document.mailform.email.focus();
//-->
</script>
<?php endif; ?>

</body>
</html>