<?php global $is_landing_page; ?>
<?php if ( is_active_sidebar('landing-page-widget-1') ) : ?>
<!--Start of Widget Block 1-->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section id="landing-page-sidebar-widget-area-1">
    <div id="landing-page-widget-area-1" class="widget-area block">
        <ul class="sid">
            <?php dynamic_sidebar('landing-page-widget-1'); ?>
        </ul>
    </div>
</section>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Widget Block 1-->
<?php endif; ?>
