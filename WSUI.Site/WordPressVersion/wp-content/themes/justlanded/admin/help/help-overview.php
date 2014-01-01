<?php
/*
 * Help Tab: Overview
 */
function optionsframework_help_info_overview() {
    return array('id' => 'overview', 'caption' => __('Overview', 'justlanded'), 'sort' => 0);
}

function optionsframework_help_overview() {
    ?>
<p>
    <strong>JustLanded for WordPress </strong> - <em><?php _e('... a landing page that simply works.', 'justlanded'); ?></em>
</p>
<p>
    <?php _e('JustLanded is more than a WordPress theme. It comes with a fully-featured landing page builder. Using that powerful tool you can set up a fully working landing page within minutes, without shortcodes or programming. It\'s that easy!', 'justlanded'); ?>
</p>
<?php
}
