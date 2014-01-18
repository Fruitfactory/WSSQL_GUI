<?php get_header(); ?>
<div id="content"<?php if (justlanded_get_option('show_page_banner', 0, $data) == true) { echo ' class="row"'; } ?>>
<?php if(@$data['blog_title'] != "" && !justlanded_get_option('show_page_banner', 0, $data)) { ?><h1 class="page-title"><?php echo do_shortcode(stripslashes($data['blog_title'])); ?></h1><?php } ?>
<div class="block_680">
<?php do_action('justlanded_before_blog_index_content'); ?>
<?php while ( have_posts() ) : the_post() ?>
<?php get_template_part( 'entry' ); ?>
<?php comments_template(); ?>
<?php endwhile; ?>
<?php get_template_part( 'nav', 'below' ); ?>
<?php do_action('justlanded_after_blog_index_content'); ?>
</div>
</div>
<?php get_sidebar(); ?>
<?php get_footer(); ?>