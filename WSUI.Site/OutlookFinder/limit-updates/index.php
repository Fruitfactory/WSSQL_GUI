<?php

//TODO: set your folder containing updates (DON'T put a trailing slash)
$updates_folder = dirname(__FILE__).'/updates';

// get all update files in the update directory
if ($handle = opendir($updates_folder))
{
	while (($file = readdir($handle)) !== false)
	{
		if ($file[0] != '.')
			$files[$file] = $updates_folder.'/'.$file;
	}

	closedir($handle);
}

$update_file = $files[$_GET['update']];

if (!$update_file)
{
	// throw a 404 error
	header("HTTP/1.0 404 Not Found");
	exit;
}

$valid_license = false;

//TODO: set the correct path to the LimeLM file
require('../LimeLM.php');

//TODO: set your API key
LimeLM::SetAPIKey('PASTE THE API KEY HERE');

//TODO: set the version id to generate & find keys for
// the version id is found in the url. For example http://wyday.com/limelm/version/100/
// the version id is 100.
$version_id = 'PASTE THE VERSION ID';

try
{
	$xml = new SimpleXMLElement(LimeLM::GetPKeyID($version_id, $_GET['pkey']));

	if ($xml['stat'] == 'ok')
	{
		$pkey_id = $xml->pkey['id'];

		$xml = new SimpleXMLElement(LimeLM::GetPKeyDetails($pkey_id));

		if ($xml['stat'] == 'ok')
		{
			// if the pkey isn't revoked (and there's a features block)
			if (!$xml->pkey['revoked'] && $xml->pkey->features)
			{
				foreach ($xml->pkey->features->feature as $feature)
				{
					if ($feature['name'] == 'update_expires')
					{
						if (time() < strtotime($feature['value']))
							$valid_license = true;

						break;
					}
				}
			}
		}
		else //failure
		{
			//TODO: use the error code & message
			//$xml->err['code'];
			//$xml->err['msg'];
		}
	}
	else //failure
	{
		//TODO: use the error code & message
		//$xml->err['code'];
		//$xml->err['msg'];
	}

	LimeLM::CleanUp();
}
catch (Exception $e)
{
	//TODO: log the error
}

// forbid invalid licenses
if (!$valid_license)
{
	header('HTTP/1.1 403 Forbidden');
	exit;
}

// if the license is still valid deliver the updates to the user
header('Content-Description: File Transfer');
header('Content-Type: application/octet-stream');
header('Content-Disposition: attachment; filename='.$_GET['update'].';');
header('Content-Transfer-Encoding: binary');
header('Expires: 0');
header('Cache-Control: must-revalidate, post-check=0, pre-check=0');
header('Pragma: public');
header('Content-Length: ' . filesize($update_file));
ob_clean();
flush();
readfile($update_file);
exit;

?>