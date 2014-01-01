<?php if ($data['banner_position'] == "left") $last=" last"; else $last = ""; ?>
<!--Start of Banner Text Content-->
<?php if (isset($data['banner_custom_text_page']) && trim($data['banner_custom_text_page']) != 0) { ?>
    <div class="one_half<?php echo $last; ?>">
        <?php
        @$p = get_page($data['banner_custom_text_page']);
        if (isset ($p->post_content)) {
            echo justlanded_content_filters(@$p->post_content);
        }
        ?>
    </div>
<?php } elseif (isset($data['banner_custom_text']) && trim($data['banner_custom_text']) != "") { ?>
    <div class="one_half<?php echo $last; ?>">
        <?php echo justlanded_content_filters($data['banner_custom_text']); ?>
    </div>
<?php } else { ?>
    <div class="one_half<?php echo $last; ?>">
        <h1><?php echo @nl2br(do_shortcode(stripslashes(@$data['banner_title'])));?></h1>
        <p><?php echo @nl2br(do_shortcode(stripslashes(@$data['banner_text'])));?></p>

        <?php
        if (@$data['banner_bullets'] != "") {
            echo "<ul class=\"ticks\">\n";
            $bullets = explode("\n", trim(@$data['banner_bullets']));
            if (is_array($bullets)) {
                foreach (@$bullets as $bullet)
                {
                    echo "  <li>" . do_shortcode(stripslashes(trim($bullet))) . "</li>\n";
                }
            }
            echo "</ul>\n";
        }
        ?>
    </div>
<?php } ?>
<!--Start of Banner Text Content-->
