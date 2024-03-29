<?php
/**
 * Template Name: Thankyou page
 * Description: Thankyou page
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
include ( dirname(__FILE__).'/downloadlink.php');
require( dirname(__FILE__).'/PaymentSettings.php');

$count = 0;
$keys = array();

foreach ($_POST as $key => $value) {
    debug_log('POST: Key = '.$key.'; Value = '.$value, true);
}
debug_log('HEADER', true);
$headers = parseRequestHeaders();

foreach ($headers as $header => $value) {
    debug_log('HEADER: Name = '.$header.'; Value = '.$value, true);
}

if(sizeof($_POST))
{

    require(dirname(__FILE__).'/LimeLM.php');

    global $LimeLM_VersionID, $LimeLM_ApiKey;

    LimeLM::SetAPIKey($LimeLM_ApiKey);
    if(!empty($_POST["payer_email"]))	
    {
            try
            {
                $xml = new SimpleXMLElement(LimeLM::FindPKey($LimeLM_VersionID,$_POST["payer_email"]));
                debug_log('Find Status: '.$xml["stat"], true);
                if($xml["stat"] == "ok")
                {
                        foreach($xml->pkeys->pkey as $pkey)
                        {
                                array_push($keys,$pkey["key"]);
                                debug_log($pkey["key"], true);
                        }	
                        $count = $xml->pkeys["total"];			
                }
            }
            catch (Exception $e)
            {	
            }
    }
	
}	


$path_down = getDownloadUrlForLastVersion();

function parseRequestHeaders() {
    $headers = array();
    foreach($_SERVER as $key => $value) {
        if (substr($key, 0, 5) <> 'HTTP_') {
            continue;
        }
        $header = str_replace(' ', '-', ucwords(str_replace('_', ' ', strtolower(substr($key, 5)))));
        $headers[$header] = $value;
    }
    return $headers;
}

?>

<?php get_header(); ?>
<article id="content"<?php if (justlanded_get_option('show_page_banner', 0, $data) == true) { echo ' class="row"'; } ?>>
    <div class="row">
        <h1>Thank you</h1>
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
                <hr/>
                <p>Also your product key will be e-mailed to you shortly. <strong>If you don't receive your product key in the next 10 minutes check your spam folder.</strong></p>
                <p><a href="<?= $path_down ?>">Download</a> last version</p>
    </div>
</article>
<div id="auto_download">
    <?php do_action('auto_start_download_full'); ?>
</div>

<?php get_footer(); ?>