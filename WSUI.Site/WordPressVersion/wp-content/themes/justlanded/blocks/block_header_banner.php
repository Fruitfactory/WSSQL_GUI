<?php do_action("justlanded_before_banner"); ?>
<!--Start of Product Banner-->
<?php if (!function_exists('justlanded_banner')) { ?>
<?php
    if (isset($data['banner_position']) && $data['banner_position'] != "") {
        $banner_position_class = ' banner_media_' . $data['banner_position'];
    }  else {
        $banner_position_class = "";
    }

    if (isset($data['banner_layout_select']) && $data['banner_layout_select'] != "" && $data['banner_position'] != "full") {
        $media_type = str_replace(" ", "_", strtolower($data['banner_layout_select']));
        $banner_media_type_class = ' banner_type_' . $media_type;
    }  else {
        $banner_media_type_class = "";
    }
    $banner_classes = trim($banner_position_class . $banner_media_type_class);
    ?>
<section id="banner" role="banner" class="<?php echo $banner_classes; ?>">
    <div class="row">
        <?php
            if (@$data['banner_position'] == "right") {
                include(JUSTLANDED_BLOCKS_DIR . "block_header_banner_text.php");
                include(JUSTLANDED_BLOCKS_DIR . "block_header_banner_media.php");
            } elseif (@$data['banner_position'] == "left") {
                include(JUSTLANDED_BLOCKS_DIR . "block_header_banner_media.php");
                include(JUSTLANDED_BLOCKS_DIR . "block_header_banner_text.php");
            } elseif (@$data['banner_position'] == "full") {
                include(JUSTLANDED_BLOCKS_DIR . "block_header_banner_full.php");
            }
        ?>
        <?php if (isset($data['banner_action_buttons']) && $data['banner_action_buttons'] == 0) {
            if (function_exists('justlanded_block_cta_buttons_plain')) {
                echo call_user_func('justlanded_block_cta_buttons_plain');
            }
            else {
                include(JUSTLANDED_BLOCKS_DIR . "block_cta_buttons_plain.php");
            }
        ?>
        <?php } elseif (isset($data['banner_action_form']) && $data['banner_action_form'] == 1) { ?>
                <div class="mini_newsletter_banner">
                    <?php echo justlanded_get_block(JUSTLANDED_BLOCKS_DIR . 'block_newsletter_plain_form.php', array()); ?>
                </div>
        <?php } ?>
    </div>
</section>
<?php } else {
    do_action("justlanded_banner");
}
?>
<!--End of Product Banner-->
<?php do_action("justlanded_after_banner"); ?>