<div class="entry-summary">
<?php the_excerpt( sprintf(__( 'continue reading %s', 'justlanded' ), '<span class="meta-nav">&raquo;</span>' )  ); ?>
<?php if(is_search()) {
wp_link_pages('before=<div class="page-link">' . __( 'Pages:', 'justlanded' ) . '&after=</div>');
}
?>
</div> 