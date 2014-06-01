<?php
/**
 * Template Name: Landing Page (Gumroad)
 * Description: The actual landing page
*/
@$active_profile = get_post_meta($post->ID, 'justlanded_meta_box_selectinstance_select', true);
@$parse_content  = intval(get_post_meta($post->ID, 'justlanded_meta_box_parse_content', true));
do_action('justlanded_before_landing_options'); // custom hook
if ($active_profile != 0) {
    $data = get_option(OPTIONSPREFIX.$active_profile);
} else {
    $data = get_option(OPTIONSPREFIX.SITE_DEFAULT_PROFILE);
}
do_action('justlanded_after_landing_options'); // custom hook
?>
<script type="text/javascript" src="https://gumroad.com/js/gumroad.js"></script>
<?php get_header(); ?>
<?php if(isset($data['hide_banner']) && $data['hide_banner'] == 0 || !isset($data['hide_banner'])) include JUSTLANDED_BLOCKS_DIR . 'block_header_banner_outlookfinder.php'; // banner and action buttons ?>
    <!--Start of Main Content-->
        <article  id="content" role="main" class="row">
            <?php the_post(); ?>
            <div id="post-<?php the_ID(); ?>" <?php post_class(); ?>>
                <div class="entry-content">
                    <?php the_content(); ?>
                </div>
            </div>
        </article>
    <!--End of Main Content-->
<?php get_footer(); ?>