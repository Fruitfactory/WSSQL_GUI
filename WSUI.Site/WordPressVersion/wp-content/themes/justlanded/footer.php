<div class="clear"></div>
</div>

<?php global $is_landing_page; ?>
<?php if ( is_active_sidebar('footer-widgets') && $is_landing_page == false ) : ?>
<!--Start of Footer Widgets-->
<div class="row">
 <section id="footer-widgets-area">
     <div id="footer-widgets" class="footer-widgets block">
         <ul class="sid sid-footer">
            <?php dynamic_sidebar('footer-widgets'); ?>
         </ul>
     </div>
 </section>
 <div class="clear"></div>
</div>
<!--End of Footer Widgets-->
<?php endif; ?>

<div class="row">
<footer role="contentinfo">
<div id="page_footer">
<?php do_action('justlanded_before_footer'); ?>
<?php if (!function_exists('justlanded_footer')) : // default if custom footer function does not exist ?>
<?php
$this_menu = "";
if (isset($data['menu_custom_footer'])) $this_menu = $data['menu_custom_footer'];
if (!function_exists('justlanded_footer_menu')) {
	wp_nav_menu( array( 'theme_location' => 'footer-menu', 'menu_class' => 'secondary', 'fallback_cb' => '', 'depth' => 1, 'menu' => $this_menu) );
}
else {
	do_action('justlanded_footer_menu');
}
?>
<?php include JUSTLANDED_BLOCKS_DIR . 'block_footer_social.php'; // sidebar social icons, if activated ?>
<p>
<?php do_action("justlanded_before_footer_text"); ?>
<?php if ((isset($data['custom_footer_copyright']) && trim($data['custom_footer_copyright'] == "")) || !isset($data['custom_footer_copyright'])) { ?>
<?php echo sprintf( __( '%1$s %2$s %3$s. All Rights Reserved.', 'justlanded' ), '&copy;', date('Y'), esc_html(get_bloginfo('name')) ); ?>
<?php } else { ?>
<?php echo do_shortcode(stripslashes($data['custom_footer_copyright'])); ?>
<?php } ?>
<?php else: ?>
<?php do_action("justlanded_footer"); ?>
<?php endif; ?>
<?php do_action("justlanded_after_footer_text"); ?>
</p>
<?php do_action('justlanded_after_footer'); ?>
</div>
</footer>
</div>
</div>
<a href="#" class="scrollup">Scroll up</a>
<?php wp_footer(); ?>

<?php include JUSTLANDED_BLOCKS_DIR . 'block_modal_1.php'; // modal window 1 for action button 1 ?>
<?php include JUSTLANDED_BLOCKS_DIR . 'block_modal_2.php'; // modal window 2 for action button 2 ?>

<?php do_action("justlanded_after_body"); ?>
</body>
</html>