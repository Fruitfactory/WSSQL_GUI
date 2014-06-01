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
	<article id="content"<?php if (justlanded_get_option('show_page_banner', 0, $data) == true) { echo ' class="row"'; } ?>>
		<?php do_action('justlanded_before_page_content'); ?>
		<?php the_post(); ?>
		<div id="post-<?php the_ID(); ?>" <?php post_class(); ?>>

			<?php if (justlanded_get_option('hide_page_titles', 0, $data) == 0) : ?>
				<h1 class="page-title"><?php the_title(); ?></h1>
			<?php endif; ?>

			<div class="entry-content">
				<?php
				if ( has_post_thumbnail() ) {
					the_post_thumbnail();
				}
				?>
				<?php the_content(); ?>
				<?php comments_template('', true); ?>
				<?php wp_link_pages('before=<div class="page-link">' . __( 'Pages:', 'justlanded' ) . '&after=</div>') ?>
				<?php edit_post_link( __( 'Edit', 'justlanded' ), '<div class="edit-link">', '</div>' ) ?>
			</div>
		</div>
		<?php do_action('justlanded_after_page_content'); ?>
	</article>
<?php get_footer(); ?>