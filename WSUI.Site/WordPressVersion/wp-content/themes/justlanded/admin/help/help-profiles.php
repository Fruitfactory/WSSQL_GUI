<?php
/*
 * Help Tab: Profiles
 */
function optionsframework_help_info_profiles() {
    return array('id' => 'profiles', 'caption' => __('Profiles', 'justlanded'), 'sort' => 1);
}

function optionsframework_help_profiles() {
    ?>
<p>
    <strong>Profiles</strong>
</p>
<p><?php _e('Profiles enable you to create multiple landing pages, or instances of all your settings. To edit another profile, use the profile select box.', 'justlanded'); ?></p>
<p><?php _e('In order to apply a profile to a page or landing page, edit the page in question and use the "Select a settings profile" meta box to select a profile.', 'justlanded'); ?></p>
<p><?php _e('By default, Profile 1 is the default profile, used whenever no profile is associated with a page or landing page, and for the entire blog. You can change this setting by using the Home &#9654; Profiles tab.', 'justlanded'); ?></p>
<p><?php _e('You can copy options between profiles using the export and import features.', 'justlanded'); ?></p>
<?php
}
