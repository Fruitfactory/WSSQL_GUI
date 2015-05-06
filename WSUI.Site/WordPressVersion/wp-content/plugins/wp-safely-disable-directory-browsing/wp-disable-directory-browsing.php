<?php
	
/*
	Plugin Name: WP Safely disable directory browsing
	Plugin URI: http://www.maurisource.com/blog/wp-safely-disable-directory-browsing/
	Description: This essential .htaccess rules plugin allow you to improve security and speed of your wordpress blog. Go to <a href="options-general.php?page=wp-safe-directory.php">Settings -> Safe directory</a> for setup.
	Version: 0.1
	Author: Manesh Sonah
	Author URI: http://www.maurisource.com/blog/
	
	Copyright 2011 Manesh Sonah (serpsiteseo@gmail.com)
	
	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.
	
	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.
	
	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

	*/

/**
 * Determine the location
 */	
$wpsecurepluginpath = WP_CONTENT_URL.'/plugins/'.plugin_basename(dirname(__FILE__)).'/';
$nomesito = $_SERVER['HTTP_HOST'];
$nomesito = str_replace('http://','',$nomesito);

$wpsecure_version = "01";

// Aggiungiamo le opzioni di default

add_option('wpsecure_tre', false);

// Carichiamo le opzioni

$wpsecure_tre = get_option('wpsecure_tre');

	
/**
 * This function makes sure Sociable is able to load the different language files from
 * the i18n subfolder of the Sociable directory
 **/
function wpsecure_init_locale(){
	global $wpsecurepluginpath;
	load_plugin_textdomain('wp-safe-directory', false, 'i18n');
}
add_filter('init', 'wpsecure_init_locale');

/**
 * Add the WpSecure menu to the Settings menu
 */
function wpsecure_admin_menu() {
	add_options_page('Safe directory', 'Safe directory', 8, 'wp-safe-directory', 'wpsafe_submenu');
}
add_action('admin_menu', 'wpsecure_admin_menu');

function wpsecure_write_htaccess($tre){
	global $nomesito;
	$filename = ABSPATH.'/wp-content/.htaccess';
	/* http://zemalf.com/1076/blog-htaccess-rules/ */
	/* 1. Protect .htaccess From Outside Access - wpsecuno */

	
	/* 3. Disable Directory Browsing  - wpsafe */
	$ht3 = '# Disable directory browsing - wpsafe'."\r\n";
	$ht3 .= 'Options All -Indexes'."\r\n";
	

	
	$wpsecure_msg = '';
	if (file_exists($filename)) {
		if (is_writable($filename)) {
		
			$stringafileht = file_get_contents($filename);

			if (preg_match("/\bwpsafe\b/i", $stringafileht)) { $tre = false; }

			
			$fp = fopen($filename, 'a');
			//fwrite($fp, "# BEGIN Safe directory\r\n");

			if ($tre) fwrite($fp, $ht3."\r\n");

			//fwrite($fp, "# END Safe directory\r\n");
			fclose($fp);
			//$wpsecure_msg = "The file $filename modified correctly";
		} else { $wpsecure_msg = "The file $filename is not writable"; }
	} else { 
		// This is the case where file doesn't exist
		$fp = fopen($filename, 'w');

		if ($tre) fwrite($fp, $ht3."\r\n");

		fclose($fp);
	}
	return $wpsecure_msg;
}

function wpsafe_submenu() {
	global $wpsecurepluginpath;

		$msg = "";

		// Check form submission and update options
		if ('wpsecure_submit' == $_POST['wpsecure_submit']) {

			update_option('wpsecure_tre', $_POST['wpsecure_tre']);
			// Carico i valori

			$wpsecure_tre = get_option('wpsecure_tre');

			
			// Modifico il file .htaccess
			$msg = wpsecure_write_htaccess($wpsecure_tre);
		}
		
			// Carico i valori

			$wpsecure_tre = get_option('wpsecure_tre');
			
		
?>

<style type="text/css">
a.sm_button {
			padding:4px;
			display:block;
			padding-left:25px;
			background-repeat:no-repeat;
			background-position:5px 50%;
			text-decoration:none;
			border:none;
		}
		 
.sm-padded .inside {
	margin:12px!important;
}
.sm-padded .inside ul {
	margin:6px 0 12px 0;
}

.sm-padded .inside input {
	padding:1px;
	margin:0;
}
</style> 
            

 
<div class="wrap" id="sm_div">
    <h2>WP Safely disable directory browsing</h2> 
    by <strong>Manesh Sonah</strong>
    <p>
    &nbsp;<a target="_blank" title="WP Safely disable directory browsing" href="http://www.maurisource.com/blog/wp-safely-disable-directory-browsing/">Changelog</a> 
    | <a target="_blank" title="WP Safely disable directory browsing Support" href="http://wordpress.org/extend/plugins/wp-safely-disable-directory-browsing/">Support</a>
	</p>
<?php	if ($msg) {	?>
	<div id="message" class="error"><p><strong><?php echo $msg; ?></strong></p></div>
<?php	}	?>

    <div style="width:824px;"> 
        <div style="float:left;background-color:white;padding: 10px 10px 10px 10px;margin-right:15px;border: 1px solid #ddd;"> 
            <div style="width:350px;height:130px;"> 
            <h3>Donate</h3> 
            <em>If you like this plugin and find it useful, help keep this plugin free and actively developed by going to the <a href="http://www.maurisource.com/blog/" target="_blank"><strong>donate</strong></a> page on my website.</em> 
            <p><em>Also, don't forget to follow me on <a href="http://twitter.com/maurisource_web/" target="_blank"><strong>Twitter</strong></a>.</em></p> 
            </div> 
        </div> 
         
        <div style="float:left;background-color:white;padding: 10px 10px 10px 10px;border: 1px solid #ddd;"> 
            <div style="width:415px;height:130px;"> 
                <h3>Credits</h3> 
                <p><em>For any doubt refer to this document <a href="http://zemalf.com/1076/blog-htaccess-rules/">here</a>.</em></p>
        <p><em>Plugin by Manesh Sonah at l'<a href="http://www.maurisource.com">Agence web Montréal</a>.</em> </p>
            </div> 
        </div> 
    </div>
    <div style="clear:both";></div> 
</div>



<div id="wpbody-content"> 

<div class="wrap" id="sm_div">

<div id="poststuff" class="metabox-holder has-right-sidebar"> 





<div class="has-sidebar sm-padded" > 
					
<div id="post-body-content" class="has-sidebar-content"> 

<div class="meta-box-sortabless"> 
                                
<div id="sm_rebuild" class="postbox">
	<h3 class="hndle"><span>Safely disable directory browsing</span></h3>
    
    <div class="inside">
    
		<form name="form1" method="post" action="<?php echo $_SERVER["REQUEST_URI"]; ?>&amp;updated=true">
			<input type="hidden" name="wpsecure_submit" value="wpsecure_submit" />
            <ul>

				<li>
                <label for="wpsecure_tre">
                    <input name="wpsecure_tre" type="checkbox" id="wpsecure_tre" value="1" <?php echo $wpsecure_tre?'checked="checked"':''; ?> />
                    Disable Directory Browsing of the directory: wp-content/ (Recommended)
                </label>
                </li>
				                </ul>
            <p>To remove the plugin's modification you need to browse to these directories and empty the .htaccess manually</p>
           <p class="submit"> <input type="submit" value="Save &amp; Write" /></p>
		</form>
        
        
    </div>
    </div>
    </div>
</div>
</div> 
</div>
<?php
	}
?>
