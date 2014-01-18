<?php get_header(); ?>
<div id="content"<?php if (justlanded_get_option('show_page_banner', 0, $data) == true) { echo ' class="row"'; } ?>>
<?php the_post(); ?>
<?php if (!justlanded_get_option('show_page_banner', 0, $data)) : ?><h1 class="page-title"><?php single_cat_title() ?></h1><?php endif; ?>
<div class="block_680">
<?php $categorydesc = category_description(); if ( !empty($categorydesc) ) echo apply_filters( 'archive_meta', '<div class="archive-meta">' . $categorydesc . '</div>' ); ?>
<?php rewind_posts(); ?>
<?php get_template_part( 'nav', 'above' ); ?>
<?php while ( have_posts() ) : the_post(); ?>
<?php get_template_part( 'entry' ); ?>
<?php endwhile; ?>
<?php get_template_part( 'nav', 'below' ); ?>
</div>
<?php get_sidebar(); ?>
</div>
<?php get_footer(); ?>