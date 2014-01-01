<?php if ($data['action_buttons_layout'] != "Single Large Button") { ?>
    <!--Start of Call To Action Buttons-->
    <div class="buttons">
        <?php if (isset($data['action_button1_modal']) && $data['action_button1_modal'] == 1) { // if modal active { ?>
            <a href="<?php echo $data['action_button1_link'] ?>" class="button_buy gradient action_button_1"<?php if(isset($data['action_button1_onclick']) && trim($data['action_button1_onclick']) != "") echo ' onclick="'.esc_attr(trim($data['action_button1_onclick'])).'"';?> data-reveal-id="action_button1_modal"><?php echo do_shortcode(stripslashes($data['action_button1_text'])) ?></a>
        <?php } else { // no modal active ?>
            <a href="<?php echo $data['action_button1_link'] ?>" class="button_buy gradient action_button_1"<?php if(isset($data['action_button1_onclick']) && trim($data['action_button1_onclick']) != "") echo ' onclick="'.esc_attr(trim($data['action_button1_onclick'])).'"';?><?php if (isset($data['action_button1_target']) && $data['action_button1_target'] == "New Window") echo ' target = "_blank"'; ?>><?php echo do_shortcode(stripslashes($data['action_button1_text'])) ?></a>
        <?php } ?>
        <span class="gradient"><?php _e('or', 'justlanded'); ?></span>
        <?php if (isset($data['action_button2_modal']) && $data['action_button2_modal'] == 1) { // if modal active { ?>
            <a href="<?php echo $data['action_button2_link'] ?>" class="button_try gradient action_button_2"<?php if(isset($data['action_button2_onclick']) && trim($data['action_button2_onclick']) != "") echo ' onclick="'.esc_attr(trim($data['action_button2_onclick'])).'"';?> data-reveal-id="action_button2_modal"><?php echo do_shortcode(stripslashes($data['action_button2_text'])) ?></a>
        <?php } else { // no modal active ?>
            <a href="<?php echo $data['action_button2_link'] ?>" class="button_try gradient action_button_2"<?php if(isset($data['action_button2_onclick']) && trim($data['action_button2_onclick']) != "") echo ' onclick="'.esc_attr(trim($data['action_button2_onclick'])).'"';?><?php if (isset($data['action_button2_target']) && $data['action_button2_target'] == "New Window") echo ' target = "_blank"'; ?>><?php echo do_shortcode(stripslashes($data['action_button2_text'])) ?></a>
        <?php } ?>
    </div>
    <!--End of Call To Action Buttons-->
<?php } else { ?>
    <!--Start of Call To Action Button-->
    <div class="buttons buttons_big">
        <?php if (isset($data['action_button1_modal']) && $data['action_button1_modal'] == 1) { // if modal active { ?>
            <a href="<?php echo $data['action_button1_link'] ?>" class="button_buy_big gradient action_button_1"<?php if(isset($data['action_button1_onclick']) && trim($data['action_button1_onclick']) != "") echo ' onclick="'.esc_attr(trim($data['action_button1_onclick'])).'"';?> data-reveal-id="action_button1_modal"><?php echo do_shortcode(stripslashes($data['action_button1_text'])) ?></a>
        <?php } else { // no modal active ?>
            <a href="<?php echo $data['action_button1_link'] ?>" class="button_buy_big gradient action_button_1"<?php if(isset($data['action_button1_onclick']) && trim($data['action_button1_onclick']) != "") echo ' onclick="'.esc_attr(trim($data['action_button1_onclick'])).'"';?><?php if (isset($data['action_button1_target']) && $data['action_button1_target'] == "New Window") echo ' target = "_blank"'; ?>><?php echo do_shortcode(stripslashes(@$data['action_button1_text'])) ?></a>
        <?php } ?>
    </div>
    <!--End of Call To Action Button-->
<?php } ?>
