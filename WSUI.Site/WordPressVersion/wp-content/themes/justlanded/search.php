<?php get_header(); ?>
<div id="content">
<h1 class="page-title"><?php printf( __( 'Search Results for: %s', 'justlanded' ), '<span>' . get_search_query()  . '</span>' ); ?></h1>
<div class="block_680">
<?php if ( have_posts() ) : ?>
<?php get_template_part( 'nav', 'above' ); ?>
<?php while ( have_posts() ) : the_post() ?>
<?php get_template_part( 'entry' ); ?>
<?php endwhile; ?>
<?php get_template_part( 'nav', 'below' ); ?>
<?php else : ?>
<div class="entry-content">
<p><?php _e( 'Sorry, nothing matched your search. Please try again.', 'justlanded' ); ?></p>
<?php get_search_form(); ?>
</div>
<?php endif; ?>
</div>
</div>
<?php get_sidebar(); ?>
<?php get_footer(); ?>