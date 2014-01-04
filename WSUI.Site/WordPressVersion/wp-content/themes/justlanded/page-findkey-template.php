<?php
/**
 * Template Name: Find Key page
 * Description: Find Key page
 */
?>

<?php
@$active_profile = get_post_meta($post->ID, 'justlanded_meta_box_selectinstance_select', true);
do_action('justlanded_before_page_options'); // custom hook
if ($active_profile != 0) {
    $data = get_option(OPTIONSPREFIX.$active_profile);
} else {
    $data = get_option(OPTIONSPREFIX.SITE_DEFAULT_PROFILE);
}
do_action('justlanded_after_page_options'); // custom hook
?>

<?php
$fields_valid = false;
$success = false;
$submitting = false;
if (sizeof($_POST))
{
	require(dirname(__FILE__).'/LimeLM.php');

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
                                        //debug_log($pkeys, true);
				}

				$emailBody = 'Your product key'.($xml->pkeys['total'] > 1 ? 's are' : ' is').':

	'.$pkeys;

				// email the user their product keys
				$emailSent = mail($_POST['email'], 'Your product '.($xml->pkeys['total'] > 1 ? 'keys' : 'key'), $emailBody);
                                //debug_log($emailBody, true);        
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



<?php get_header(); ?>
<article id="content"<?php if (justlanded_get_option('show_page_banner', 0, $data) == true) { echo ' class="row"'; } ?>>
    
    <div class="row">
				<h1>Find product key</h1>
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
								<td><input type="submit" name="btnFindSerials" value="Find Keys"/></td>
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
			</div>
    
</article>
<?php get_footer(); ?>