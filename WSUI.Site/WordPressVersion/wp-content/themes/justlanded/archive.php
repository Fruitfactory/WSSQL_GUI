<?php get_header(); ?>
<div id="content" class="row">
<div class="block_680">
<?php do_action('justlanded_before_archive_content'); ?>
<?php the_post(); ?>
<?php if (!justlanded_get_option('show_page_banner', 0, $data)) : ?>
<?php if ( is_day() ) : ?>
<h1 class="page-title"><?php printf( __( 'Daily Archives: %s', 'justlanded' ), '<span>' . get_the_time(get_option('date_format')) . '</span>' ) ?></h1>
<?php elseif ( is_month() ) : ?>
<h1 class="page-title"><?php printf( __( 'Monthly Archives: %s', 'justlanded' ), '<span>' . get_the_time('F Y') . '</span>' ) ?></h1>
<?php elseif ( is_year() ) : ?>
<h1 class="page-title"><?php printf( __( 'Yearly Archives: %s', 'justlanded' ), '<span>' . get_the_time('Y') . '</span>' ) ?></h1>
<?php elseif ( isset($_GET['paged']) && !empty($_GET['paged']) ) : ?>
<h1 class="page-title"><?php _e('Blog Archives', 'justlanded' ); ?></h1>
<?php endif; ?>
<?php endif; ?>
<?php rewind_posts(); ?>
<?php while ( have_posts() ) : the_post(); ?>
<?php get_template_part( 'entry' ); ?>
<?php endwhile; ?>
<?php get_template_part( 'nav', 'below' ); ?>
<?php do_action('justlanded_after_archive_content'); ?>
</div>
<?php get_sidebar(); ?>
</div>
<?php get_footer(); ?>