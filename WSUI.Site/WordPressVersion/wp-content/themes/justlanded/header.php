<?php
global $data; //fetch user settings
global $justlanded_custom_blocks;
global $template;
if (isset($data['custom_shortcodes']) && trim($data['custom_shortcodes']) != "") do_shortcode($data['custom_shortcodes']);
include(JUSTLANDED_MAIN_DIR . 'custom.php');
?>
<?php do_action("justlanded_before_page_output"); ?>
<!DOCTYPE html>
<html <?php language_attributes(); ?>>
<head>
<meta http-equiv="content-type" content="<?php bloginfo('html_type'); ?>; charset=<?php bloginfo('charset'); ?>" />
<title><?php wp_title(' | ', true, 'right'); ?></title>
<?php if (!isset($data['disable_responsiveness']) || ( isset($data['disable_responsiveness']) && $data['disable_responsiveness'] == 0 ) ) : ?>
<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1, user-scalable=no" />
<?php endif; ?>
<link rel="stylesheet" type="text/css" href="<?php echo get_stylesheet_uri(); ?>" />
<?php if (trim(@$data['custom_favicon']) != "") {
 echo '<link rel="shortcut icon" href="'. @$data['custom_favicon'] .'" />';
}
?>
<?php wp_head(); ?>
</head>
<body <?php body_class(); ?>>
<?php do_action('justlanded_before_wrapper'); ?>
<div id="wrapper" class="hfeed<?php if ((justlanded_get_option('show_page_banner', 0, $data) == true && basename($template) != "landingpage.php" && basename($template) != "landingpage-nobanner.php") || basename($template) == "page-with-banner.php") { echo ' with-page-banner'; } ?>">
<?php do_action("justlanded_before_head"); ?>
<header>
<?php if(!function_exists('justlanded_head_above_banner')) { ?>
<?php
do_action('justlanded_before_top_menu');
global $template;
if (isset($data['hide_menu']) && $data['hide_menu'] == 1) {
    do_action('justlanded_no_top_menu');
}
else {
    if ((basename($template)) == "landingpage.php" || (basename($template)) == "landingpage-nobanner.php"  ) {
        if (@$data['landing_page_menu'] == 1)
        {
            if (!function_exists('justlanded_top_menu')) {
                include (JUSTLANDED_BLOCKS_DIR . 'block_header_menu.php');
            }
            else {
                do_action('justlanded_top_menu');
            }
        }
    } else {
        if (!function_exists('justlanded_top_menu')) {
            include (JUSTLANDED_BLOCKS_DIR . 'block_header_menu.php');
        }
        else {
            do_action('justlanded_top_menu');
        }
    }
}
do_action('justlanded_after_top_menu');
?>
<div id="landing_header" class="row">
<div id="landing_header_inner">
<?php
if (isset($data['header_freeform_page']) && $data['header_freeform_page'] != 0) {
@$p = get_page($data['header_freeform_page']);
if (isset ($p->post_content)) {
     echo justlanded_content_filters(@$p->post_content);
}
}
elseif (isset($data['header_freeform']) && trim($data['header_freeform']) != "") {
echo justlanded_content_filters($data['header_freeform']);
} else { ?>
<?php if (!function_exists('justlanded_logo')) : ?>
<div id="logo" class="two_thirds">
<?php if (isset($data['hide_page_title']) && $data['hide_page_title'] == 1) { ?>
<a href="<?php if (isset($data['custom_home_url']) && $data['custom_home_url'] != "") echo $data['custom_home_url']; else echo home_url()."/"; ?>" title="<?php bloginfo( 'name' ) ?>" rel="home"><?php justlanded_image(@$data['custom_logo'], get_bloginfo('name')); ?></a>
<?php } else { ?>
<h1><a href="<?php if (isset($data['custom_home_url']) && $data['custom_home_url'] != "") echo $data['custom_home_url']; else echo home_url()."/"; ?>" title="<?php bloginfo( 'name' ) ?>" rel="home"><?php justlanded_image(@$data['custom_logo'], get_bloginfo('name')); ?><span><?php bloginfo( 'name' ) ?></span></a></h1>
<h2 id="site-description"><?php bloginfo( 'description' ) ?></h2>
<?php } ?>
</div><!--End of Header Logo-->
<?php else: ?>
<?php do_action('justlanded_logo'); ?>
<?php endif; ?>
<?php if (!function_exists('justlanded_top_contact')) {
include JUSTLANDED_BLOCKS_DIR . 'block_header_phone.php';
}
else {
do_action('justlanded_top_contact');
} ?>
<?php } ?>
</div>
</div><!--End of Landing Header-->
</header>
<?php }
    else {
        do_action("justlanded_head_above_banner");
    }
?>
<?php do_action("justlanded_after_head"); ?>
<?php
if (justlanded_get_option('show_page_banner', 0, $data) == true && basename($template) != "landingpage.php" && basename($template) != "landingpage-nobanner.php" || basename($template) == "page-with-banner.php") {
	include(JUSTLANDED_BLOCKS_DIR . 'block_page_banner.php');
}
?>
<div id="container"<?php if (justlanded_get_option('hide_page_titles', 0, $data) == 1) { ?> class="no-title"<?php } ?>>