<?php
if ( 'comments.php' == basename($_SERVER['SCRIPT_FILENAME']) )
die ( 'Please do not load this page directly.' );
if ( post_password_required() ) :
?>
<div id="comments">
<div class="nopassword"><?php _e('This post is password protected. Enter the password to view comments.', 'justlanded') ?></div>
</div>
<?php
return;
endif;
?>
<div id="comments" class="<?php if (have_comments()) echo 'has-comments'; else echo 'no-comments' ?>">
<?php if ( have_comments() ) : ?>
<?php
$ping_count = $comment_count = 0;
foreach ( $comments as $comment )
get_comment_type() == "comment" ? ++$comment_count : ++$ping_count;
?>
<?php if ( ! empty($comments_by_type['comment']) ) : ?>
<div id="comments-list" class="comments">
<h3><?php echo ($comment_count > 1 ? '<span>'.$comment_count.'</span> '. __('Comments', 'justlanded') : '<span>'. __( 'One', 'justlanded' ) .'</span> '. __('Comment', 'justlanded') ); ?></h3>
<?php $total_pages = get_comment_pages_count(); if ( $total_pages > 1 ) : ?>
<div id="comments-nav-above" class="comments-navigation">
<div class="paginated-comments-links"><?php paginate_comments_links(); ?></div>
</div>
<?php endif; ?>
<ul>
<?php wp_list_comments('type=comment&callback=justlanded_custom_comments&avatar_size=60'); ?>
</ul>
<?php $total_pages = get_comment_pages_count(); if ( $total_pages > 1 ) : ?>
<div id="comments-nav-below" class="comments-navigation">
<div class="paginated-comments-links"><?php paginate_comments_links(); ?></div>
</div>
<?php endif; ?>
</div>
<?php 
endif; 
global $comments_by_type;
$comments_by_type = separate_comments( $comments );
if ( ! empty($comments_by_type['pings']) ) : ?>
<div id="trackbacks-list" class="comments">
<h3><?php echo($ping_count > 1 ? '<span>'.$ping_count.'</span> '.__('Trackbacks', 'justlanded') : '<span>'. __('One', 'justlanded' ) .'</span> '. __('Trackback', 'justlanded') ); ?></h3>
<ul>
<?php wp_list_comments('type=pings&callback=justlanded_custom_pings'); ?>
</ul>
</div>
<?php endif; ?>
<?php endif; ?>
<?php if ( comments_open() ) : ?>
<?php comment_form(); ?>
<?php endif; ?>
</div>