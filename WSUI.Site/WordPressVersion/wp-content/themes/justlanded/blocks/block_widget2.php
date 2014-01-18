<?php global $is_landing_page; ?>
<?php if ( is_active_sidebar('landing-page-widget-2') ) : ?>
<!--Start of Widget Block 2-->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section id="landing-page-sidebar-widget-area-2">
    <div id="landing-page-widget-area-2" class="widget-area block">
        <ul class="sid">
            <?php dynamic_sidebar('landing-page-widget-2'); ?>
        </ul>
    </div>
</section>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Widget Block 2-->
<?php endif; ?>
