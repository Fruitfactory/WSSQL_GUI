<?php global $authordata, $data; ?>
<div class="entry-meta">
<?php if (isset($data['hide_post_author']) && $data['hide_post_author'] == 1) { } else { ?>
<span class="author vcard"><?php _e('By', 'justlanded'); ?> <a class="url fn n" href="<?php echo get_author_posts_url(get_the_author_meta('ID')); ?>" title="<?php printf( __( 'View all articles by %s', 'justlanded' ), $authordata->display_name ); ?>"><?php the_author(); ?></a></span>
<?php } ?>
<?php if (isset($data['hide_post_meta']) && $data['hide_post_meta'] == 1) { } else { ?>
<span class="entry-date"><abbr class="published" title="<?php the_time('Y-m-d\TH:i:sO') ?>"><?php the_time( get_option( 'date_format' ) ); ?></abbr></span>
<?php } ?>
<span class="comments-link"><?php comments_popup_link( __( 'Leave a comment', 'justlanded' ), __( '1 Comment', 'justlanded' ), __( '% Comments', 'justlanded' ) ); ?></span>
<?php edit_post_link( __( 'Edit', 'justlanded' ), "<span class=\"meta-sep\"> | </span>\n\t\t\t\t\t\t<span class=\"edit-link\">", "</span>\n\t\t\t\t\t" ) ?>
</div>