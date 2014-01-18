<?php global $post; if ( 'post' == $post->post_type ) : ?>
<div class="entry-footer">
<a href="<?php the_permalink(); ?>" title="<?php printf( __('Read %s', 'justlanded'), the_title_attribute('echo=0') ); ?>" rel="bookmark"><?php printf(__('Continue reading', 'justlanded')); ?> &raquo;</a>

<?php edit_post_link( __( 'Edit', 'justlanded' ), "<span class=\"meta-sep\"> | </span>\n\t\t\t\t\t\t<span class=\"edit-link\">", "</span>\n\t\t\t\t\t\n" ); ?>
</div>
<?php endif; ?>