<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en"> 
<head> 
<link href="css/metro-bootstrap.css" rel="stylesheet">
    <link href="css/metro-bootstrap-responsive.css" rel="stylesheet">
    <link href="css/docs.css" rel="stylesheet">
    <link href="js/prettify/prettify.css" rel="stylesheet">

    <!-- Load JavaScript Libraries -->
    <script src="js/jquery/jquery.min.js"></script>
    <script src="js/jquery/jquery.widget.min.js"></script>
    <script src="js/jquery/jquery.mousewheel.js"></script>
    <script src="js/prettify/prettify.js"></script>

    <!-- Metro UI CSS JavaScript plugins -->
    <script src="js/metro/metro-loader.js"></script>

	<title>Thank you</title>
</head>
<body class="metro">
	<h1>Thank you</h1>
	
<?php
$count = 0;
$keys = array();
	if(sizeof($_POST))
	{
	require("PaymentSettings.php");
		require("LimeLM.php");
		
		global $LimeLM_VersionID, $LimeLM_ApiKey;
		
		LimeLM::SetAPIKey($LimeLM_ApiKey);
		if(!empty($_POST["payer_email"]))	
		{
			try
			{
				$xml = new SimpleXMLElement(LimeLM::FindPKey($LimeLM_VersionID,$_POST["payer_email"]));
				if($xml["stat"] == "ok")
				{
					foreach($xml->pkeys->pkey as $pkey)
					{
						array_push($keys,$pkey["key"]);
					}	
					$count = $xml->pkeys["total"];			
				}
			}
			catch (Exception $e)
			{	
			}
		}
		
	}	
		
?>
<?php  
if($count > 0)
{
?>
	<p>Here 
		<?php if($count > 1) 
			{ echo ' are ';}
		else  {echo ' is ';} ?> 
yours product key</p>
<table>
		<?php
		foreach($keys as $key)
		{
			echo "<tr><td>$key</td></tr>";	
		}			
		?>
</table>		
<?php }?>
	<p>Also your product key will be e-mailed to you shortly. <strong>If you don't recieve your product key in the next 10 minutes check your spam folder.</strong></p>
	
</body></html>