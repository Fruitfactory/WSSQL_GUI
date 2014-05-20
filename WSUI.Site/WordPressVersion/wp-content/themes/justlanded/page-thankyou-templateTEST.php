<?php
/**
 * Template Name: Thankyou page (TEST)
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
include_once ( dirname(__FILE__).'/PaymentSettingsTEST.php');

$isExistingEmail = $_GET["existEmail"];
$isNewCustomer = $_GET["newCustomer"];


$count = 0;
$keys = array();

$path_down = getDownloadUrlForLastVersion();

?>

<?php get_header(); ?>
<article id="content"<?php if (justlanded_get_option('show_page_banner', 0, $data) == true) { echo ' class="row"'; } ?>>
    <?php
        if($isExistingEmail){
    ?>
    <div class="row">
        <h1>Thank you for pushing our software.</h1>
         <p>You should restart Outlook to take effect.</p>
        <p>Here is the link for downloading the latest version: <a href="<?= $path_down ?>">download</a></p>
    </div>
    <?php }
        else if ($isNewCustomer){
    ?>
    <div class="row">
        <h1>Thank you for pushing our software.</h1>
        <p>Here is the link for downloading the latest version: <a href="<?= $path_down ?>">download</a></p>
    </div>
    <?php } ?>
</article>
<div id="auto_download">
    <?php 
    if($isNewCustomer){
      do_action('autostartdownloadfull');  
    }?>
</div>


<?php get_footer(); ?>