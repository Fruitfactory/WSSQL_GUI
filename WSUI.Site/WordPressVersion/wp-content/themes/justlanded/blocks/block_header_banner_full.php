<!--Start of Full Banner-->
<div id="banner_full">
<?php if (!function_exists ('justlanded_banner_full')) : ?>
    <?php
    do_action("justlanded_before_banner_full");
    if (isset($data['banner_full_page']) && $data['banner_full_page'] != 0) {
        @$p = get_page($data['banner_full_page']);
        if (isset ($p->post_content)) {
            echo trim(justlanded_content_filters(@$p->post_content));
        }
    } else {
        if (isset($data['banner_full']) && trim($data['banner_full']) != "") {
            echo trim(justlanded_content_filters((stripslashes($data['banner_full']))));
        } else {
            do_action("justlanded_banner_full");
        }
    }
    do_action("justlanded_after_banner_full");
    ?>
<?php else: ?>
<?php do_action("justlanded_banner_full"); ?>
<?php endif; ?>
</div>
<!--End of Full Banner-->
