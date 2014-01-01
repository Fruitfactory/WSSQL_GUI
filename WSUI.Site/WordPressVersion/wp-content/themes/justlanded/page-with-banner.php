<?php
/**
 * Template Name: Default Template, with Page Banner
 * Description: A regular content page, full width, with Page Banner feature
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
<?php get_header(); ?>
<article id="content" class="row">
<?php the_post(); ?>
<div id="post-<?php the_ID(); ?>" <?php post_class(); ?>>
<div class="entry-content">
<?php the_content(); ?>
<?php comments_template('', true); ?>
<?php wp_link_pages('before=<div class="page-link">' . __( 'Pages:', 'justlanded' ) . '&after=</div>') ?>
<?php edit_post_link( __( 'Edit', 'justlanded' ), '<div class="edit-link">', '</div>' ) ?>
</div>
</div>
</article>
<?php get_footer(); ?>