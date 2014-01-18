<?php global $is_landing_page; ?>
<?php if ( is_active_sidebar('landing-page-widgets-row-' . $widgets_row_idx) ) : ?>
    <!--Start of Widgets Row Block 1-->
    <?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
    <section id="landing-page-widgets-row-<?php echo $widgets_row_idx; ?>-area" class="block">
        <div id="landing-page-widgets-row-<?php echo $widgets_row_idx; ?>" class="widgets-row block">
            <ul class="sid sid-widgets-row">
                <?php dynamic_sidebar('landing-page-widgets-row-' . $widgets_row_idx); ?>
            </ul>
        </div>
    </section>
    <?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
    <!--End of Widgets Row Block 1-->
<?php endif; ?>
