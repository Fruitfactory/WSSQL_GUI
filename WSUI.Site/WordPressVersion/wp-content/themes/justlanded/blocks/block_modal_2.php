<?php
if (isset($data['action_button2_modal_size'])) $reveal_classes = ' ' . $data['action_button2_modal_size']; else $reveal_classes = '';
if (!function_exists('justlanded_action_button2_modal')) {
    if (isset($data['action_button2_modal']) && $data['action_button2_modal'] == 1) {
        echo "\n\n" . '<div id="action_button2_modal" class="reveal-modal'.$reveal_classes.'">' . "\n";
        ?>
        <?php
        if (isset($data['action_button2_modal_page']) && $data['action_button2_modal_page'] != 0) {
            @$p = get_page($data['action_button2_modal_page']);
            if (isset ($p->post_content)) {
                echo justlanded_content_filters(@$p->post_content);
            }
            else {
                echo 'No page content for this modal window.';
            }
            ?>
        <?php
        } else {
            if (isset($data['action_button2_modal_content']) && trim($data['action_button2_modal_content'] != "")) {
                echo justlanded_content_filters($data['action_button2_modal_content']);
            }
        }
        echo '    <a class="close-reveal-modal">&#215;</a>' . "\n" . '</div>' . "\n";
    }
} else {
    do_action("justlanded_action_button2_modal");
}
?>