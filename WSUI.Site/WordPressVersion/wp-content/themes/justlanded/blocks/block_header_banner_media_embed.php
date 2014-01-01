<?php if ($data['banner_layout_select'] == "Single Image") { ?>
    <!--Start Product Image-->
    <?php if(!function_exists('justlanded_banner_media_image')) { ?>
        <?php if (@$data['banner_image'] != "") { ?>
			<?php if (justlanded_get_option('banner_image_link', '', $data) != "") echo '<a href="'.justlanded_get_option('banner_image_link', '', $data).'"  target="'.justlanded_get_option('banner_image_link_target', '_self', $data).'">'; ?>
			<img src="<?php echo @$data['banner_image']; ?>" alt="" id="image_bottom" />
			<?php if (justlanded_get_option('banner_image_link', '', $data) != "") echo '</a>'; ?>
        <?php } else { ?>
            <p>Please upload an image using the Theme Options panel.</p>
        <?php } ?>
    <?php } else {
        do_action("justlanded_banner_media_image");
    } ?>
    <!--End Product Image-->
<?php } elseif (@$data['banner_layout_select'] == "Slider") { ?>
    <!--Start Product Slider-->
    <?php do_action("justlanded_before_banner_media_slider"); ?>
    <?php if(!function_exists('justlanded_banner_media_slider')) { ?>
        <div id="banner_slider" class="flexslider">
            <ul class="slides">
                <!-- Start Slider -->
                <?php
                $slides = $data['banner_slider'];
                $x=0;
                if (is_array($slides)) {
                    foreach ($slides as $slide) {
                        ?>
                        <li><?php if (@$slide['link'] != "") { ?><a href="<?php echo @$slide['link']; ?>"><?php } ?><img src="<?php echo @$slide['url']; ?>" alt="<?php echo esc_attr(@$slide['title']); ?>"><?php if (@$slide['link'] != "") { ?></a><?php } ?></li>
                    <?php }} else { echo "You need to add at least one slide in order for the slider to show."; } ?>
                <!-- End Slider -->
            </ul>
        </div>
        <!-- End outer wrapper -->
    <?php } else {
        do_action("justlanded_banner_media_slider");
    } ?>
    <!--End Product Slider-->
    <?php do_action("justlanded_after_banner_media_slider"); ?>
<?php } elseif (@$data['banner_layout_select'] == "Video") { ?>
    <!--Start Video-->
    <?php if(!function_exists('justlanded_banner_media_video')) { ?>
        <?php
        $video_code = $data['banner_video'];
        $embeds = array('<object', '<iframe', '<embed');
        $embed_found = false;
        foreach ($embeds as $embed) {
            if (substr_count(strtolower($video_code), $embed) > 0 ) {
                echo $video_code;
                $embed_found = true;
                break;
            }
        }
        if ($embed_found == false) echo @apply_filters('embed_oembed_html', wp_oembed_get(do_shortcode((stripslashes($data['banner_video'])))));
        ?>
    <?php } else {
        do_action("justlanded_banner_media_video");
    } ?>
    <!--End Video-->
<?php } elseif (@$data['banner_layout_select'] == "Free Form Content" ) { ?>
    <!--Start Free Form Media-->
    <?php if(!function_exists('justlanded_banner_freeform')) { ?>
        <?php echo @justlanded_content_filters(stripslashes($data['banner_free_media'])); ?>
    <?php } else {
        do_action("justlanded_banner_media_freeform");
    } ?>
    <!--End Free Form Media-->
<?php } ?>