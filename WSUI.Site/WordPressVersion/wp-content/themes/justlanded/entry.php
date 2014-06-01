<?php global $data; ?>
<?php if (justlanded_is_archive_or_index()) { $headline_tag = "h2"; } else { $headline_tag = "h1";} ?>
<div id="post-<?php the_ID(); ?>" <?php post_class(); ?>>
<?php if (justlanded_is_archive_or_index()) { ?><a href="<?php the_permalink(); ?>" title="<?php printf( __('Read %s', 'justlanded'), the_title_attribute('echo=0') ); ?>" rel="bookmark"><?php } ?>
<?php
if  ((isset($data['hide_post_featured_image']) && $data['hide_post_featured_image'] == 1) ||
    (justlanded_is_archive_or_index() && isset($data['hide_post_featured_image_index']) && $data['hide_post_featured_image_index'] == 1) ||
    (!justlanded_is_archive_or_index() && isset($data['hide_post_featured_image_detail']) && $data['hide_post_featured_image_detail'] == 1))
{
    // do nothing, featured image is disabled for this instance
}
else {
    the_post_thumbnail('custom-1');
}
?>
<?php if (justlanded_is_archive_or_index()) { ?></a><?php } ?>
<?php if (justlanded_get_option('hide_page_titles', 0, $data) == 1 && is_single() ) { /* do nothing, we do not want a headline here */ } else { ?>
<<?php echo $headline_tag;?> class="entry-title"><?php if(justlanded_is_archive_or_index()) {?><a href="<?php the_permalink(); ?>" title="<?php printf( __('Read %s', 'justlanded'), the_title_attribute('echo=0') ); ?>" rel="bookmark"><?php } ?><?php the_title(); ?><?php if (justlanded_is_archive_or_index()) { ?></a><?php } ?></<?php echo $headline_tag;?>>
<?php } ?>
<?php get_template_part( 'entry', 'meta' ); ?>
<?php
if ((is_archive() || is_search() || is_home() || is_category() || is_tag())) {
    if (isset($data['show_full_posts']) && $data['show_full_posts'] == 1) {
        get_template_part('entry', 'content');
    } else {
        get_template_part('entry', 'summary');
    }
} else {
    get_template_part('entry', 'content');
}
?>
<?php
if ( is_single() ) {
get_template_part( 'entry-footer', 'single' );
} else {
get_template_part( 'entry-footer' );
}
?>
</div> 