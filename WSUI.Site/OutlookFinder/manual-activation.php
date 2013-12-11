<?php
$fields_valid = false;
$success = false;
$submitting = false;

if (sizeof($_POST))
{
	if (!empty($_POST['xmlresp']))
	{
		// download the file and bail
		header('Content-Description: File Transfer');
		header('Content-Type: application/xml');
		header('Content-Disposition: attachment; filename=ActivationResponse.xml');
		header('Content-Transfer-Encoding: binary');
		header('Expires: 0');
		header('Cache-Control: must-revalidate, post-check=0, pre-check=0');
		header('Pragma: public');
		header('Content-Length: '.strlen($_POST['xmlresp']));
		ob_clean();
		flush();

		// output the file
		echo $_POST['xmlresp'];

		exit;
	}

	require('LimeLM.php');

	//TODO: set your API key found in http://wyday.com/limelm/settings/
	LimeLM::SetAPIKey('PASTE THE API KEY HERE');

	$submitting = true;
	$fields_valid = true;

	// get the activation request
	$act_req = $_POST['act_req'];

	if (strlen($act_req) == 0)
	{
		if ($_FILES['act_req_file']['tmp_name'])
			$act_req = file_get_contents($_FILES['act_req_file']['tmp_name']);
		else
		{
			$fields_valid = false;
		}
	}

	// activation request data is present, try to activate
	if ($fields_valid)
	{
		try
		{
			// try to manually activate
			$xml = new SimpleXMLElement(LimeLM::ManualActivation($act_req));

			if ($xml['stat'] == 'ok')
			{
				$response = htmlspecialchars($xml->act_resp_xml['data'], ENT_QUOTES, 'UTF-8');

				$success = true;
			}
			else //failure
			{
				$success = false;

				if ($xml->err['code'] == '105')
					$revoked = true;
				else if ($xml->err['code'] == '106')
					$max_used = true;
				else // something else went wrong
				{
					//TODO, log the error
					//$xml->err['code']
					//$xml->err['msg']
				}
			}
		}
		catch (Exception $e)
		{
			$success = false;

			//TODO: log the error
		}
	}

	LimeLM::CleanUp();
}
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en"> 
<head> 
	<title>Manual product activation</title>
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
</head>
<body>

	<h1>Manual product activation</h1>
	<p>A simple example showing how to do manual activation.</p>

	<h2>Instructions</h2>
	<ol>
		<li>Set your LimeLM API key (see line 31).</li>
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
	<h2>Activated successfully!</h2>

	<form name="download" accept-charset="utf-8" action="" method="post">
		<input type="hidden" value="<?=$response?>" name="xmlresp"/>
		<p><strong><a href="javascript: submitform()">Download the Activation response XML file</a></strong></p>
	</form>
	<script type="text/javascript">function submitform() { document.download.submit(); }</script>

	<p>Or, copy and paste the activation response:</p>
	<textarea rows="12" cols="80" readonly="readonly"><?=$response?></textarea>

<?php else: ?>

	<?php if ($submitting):?>
		<?php if (!$fields_valid): ?>
			<p><big><strong>Oops. You must submit an activation request file.</strong></big></p>
		<?php elseif ($revoked): ?>
			<p><big><strong>This product key has been revoked. Contact us for more information.</strong></big></p>
		<?php elseif ($max_used): ?>
			<p><big><strong>This product key has been activated the maximum number of times. Buy more licenses.</strong></big></p>
		<?php else: ?>
			<p><big><strong>Something went wrong - we're investigating it.</strong></big></p>
		<?php endif; ?>
	<?php endif; ?>

	<form enctype="multipart/form-data" accept-charset="utf-8" method="post" action="">
		<h2>Upload the activation request file</h2>
		<input name="act_req_file" type="file"/><br/>

		<div>
			<h2><label for="form_act_req">Or, paste the activation request:</label></h2>
			<textarea id="form_act_req" name="act_req" rows="9" cols="75"><?=htmlspecialchars($act_req, ENT_QUOTES, 'UTF-8') ?></textarea>
		</div>

		<div class="submit"><input type="submit" value="Activate"/></div>
	</form>

<?php endif; ?>

</body>
</html>