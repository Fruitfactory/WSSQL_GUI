<?php
/**
 * Template Name: Gumroad Template (TEST)
 * Description: Gumroad functionality
 */
?>

<?php
@$active_profile = get_post_meta($post->ID, 'justlanded_meta_box_selectinstance_select', true);
do_action('justlanded_before_page_options'); // custom hook
if ($active_profile != 0) {
    $data = get_option(OPTIONSPREFIX . $active_profile);
} else {
    $data = get_option(OPTIONSPREFIX . SITE_DEFAULT_PROFILE);
}
do_action('justlanded_after_page_options'); // custom hook
?>


<?php get_header(); ?>
<article id="content"<?php if (justlanded_get_option('show_page_banner', 0, $data) == true) {
    echo ' class="row"';
} ?>>

    <div class="row">
        <script type="text/javascript" src="https://gumroad.com/js/gumroad.js"></script>
        <a href="https://gum.co/AZCD">OutlookFinder</a> 
    </div>

</article>
<?php get_footer(); ?>