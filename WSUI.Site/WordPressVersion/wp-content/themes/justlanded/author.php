<?php get_header(); ?>
<div id="content">
<?php the_post(); ?>
<?php if (!justlanded_get_option('show_page_banner', 0, $data)) : ?><h1 class="page-title"><?php printf( __( 'Author Archives: %s', 'justlanded' ), "<span class=\"vcard\"><a class='url fn n' href='$authordata->user_url' title='$authordata->display_name' rel='me'>$authordata->display_name</a></span>" ) ?></h1><?php endif; ?>
<div class="block_680">
<?php rewind_posts(); ?>
<?php get_template_part( 'nav', 'above' ); ?>
<?php while ( have_posts() ) : the_post(); ?>
<?php get_template_part( 'entry' ); ?>
<?php endwhile; ?>
<?php get_template_part( 'nav', 'below' ); ?>
</div>
</div>
<?php get_sidebar(); ?>
<?php get_footer(); ?>