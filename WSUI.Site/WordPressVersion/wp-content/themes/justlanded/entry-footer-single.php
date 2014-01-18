<div class="entry-footer">
<?php
if (isset($data['hide_post_meta_footer']) && $data['hide_post_meta_footer'] == 1) {
    // do nothing, footer hidden
}
else {
printf( __( 'This article was posted in %1$s%2$s.', 'justlanded' ),
get_the_category_list(', '),
get_the_tag_list( __( ' and tagged ', 'justlanded' ), ', ', '' )
);
}
edit_post_link( __( 'Edit', 'justlanded' ), "\n\t\t\t\t\t<span class=\"edit-link\">", "</span>" );
?>
</div> 