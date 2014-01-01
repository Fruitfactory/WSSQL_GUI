<?php get_header(); ?>
<article id="content"<?php if (justlanded_get_option('show_page_banner', 0, $data) == true) { echo ' class="row"'; } ?>>
<div class="block_680">
<?php if ( have_posts() ) : while ( have_posts() ) : the_post(); ?>
<?php get_template_part( 'entry' ); ?>
<?php comments_template('', true); ?>
<?php endwhile; endif; ?>
<?php get_template_part( 'nav', 'below-single' ); ?>
</div>
</article>
<?php get_sidebar(); ?>
<?php get_footer(); ?>