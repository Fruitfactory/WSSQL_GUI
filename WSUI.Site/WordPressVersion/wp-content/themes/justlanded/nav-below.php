<?php global $wp_query; $total_pages = $wp_query->max_num_pages; if ( $total_pages > 1 ) { ?>
<div id="nav-below" class="navigation">
<div class="nav-previous"><?php next_posts_link(sprintf(__( 'older articles %s', 'justlanded' ),'<span class="meta-nav">&raquo;</span>')) ?></div>
<div class="nav-next"><?php previous_posts_link(sprintf(__( '%s newer articles', 'justlanded' ),'<span class="meta-nav">&laquo;</span>')) ?></div>
</div>
<?php } ?> 