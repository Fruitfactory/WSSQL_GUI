<?php
do_action("justlanded_before_dynamic_css_output");
$protocol = is_ssl() ?'https':'http';

$fonts = array();
if (isset($data) && is_array($data)) {

    foreach ($data as $option => $option_settings) {
        if (substr_count($option, "font_") > 0 && isset($option_settings['face']) && substr_count(@$option_settings['face'], "*") == 0 && @$option_settings['face'] != "" && @$option_settings['face'] != "#") {
            @$face  =   $option_settings['face'];
            @$style =   $option_settings['style'];
            if (isset($option_settings['subset'])) @$subsets = $option_settings['subset']; else $subsets = "";
            $fonts[$face]['styles'][] = $style;
            if (is_array($subsets)) {
                foreach ($subsets as $subset => $checked) {
                    $fonts[$face]['subsets'][] = $subset;
                }
            }
            if (isset($fonts[$face]['styles'])) $fonts[$face]['styles'] = array_unique ($fonts[$face]['styles']);
            if (isset($fonts[$face]['subsets'])) $fonts[$face]['subsets'] = array_unique ($fonts[$face]['subsets']);
        }
    }
    foreach ($fonts as $face => $font_settings) {
        $styles = "";
        $subsets = "";
        if (isset($font_settings['styles'])) $styles  = rtrim(trim(implode(",", $font_settings['styles'])), ",");
        if (isset($font_settings['subsets'])) $subsets = rtrim(trim(implode(",", $font_settings['subsets'])), ",");

        if ($subsets != "") {
            $subsets = '&subset=' . $subsets;
        }
        echo '@import url('.$protocol.'://fonts.googleapis.com/css?family='.str_replace(" ", "+", $face).':'.$styles.$subsets.');' . "\n";
    }
}
?>

<?php do_action("justlanded_dynamic_css_after_fonts_init"); ?>

body {
background:<?php echo @$data['body_background'];?>;
}

a, #phone a {
color: <?php echo @$data['link_color'];?>;
}

a:hover {
color: <?php echo @$data['link_color_hover'];?>;
}

.section_gallery img:hover {
border: 3px solid <?php echo @$data['link_color_hover'];?>;
}

blockquote footer div {
color: <?php echo @$data['link_color'];?>;
}

.tagcloud a:hover {
background-color: <?php echo @$data['link_color_hover'];?>;
border: 1px solid <?php echo @$data['link_color_hover'];?>;
}

<?php if (isset($data['banner_background_type']) && $data['banner_background_type'] == "solid" && isset($data['banner_gradient'])) { ?>
#banner {
background:<?php echo @$data['banner_gradient']['start'];?>;
filter: none;
}
<?php } elseif (isset($data['banner_background_type']) && ($data['banner_background_type'] == "image" || $data['banner_background_type'] == "tile") && isset($data['banner_background_image']) && $data['banner_background_image'] != "") { ?>
<?php if (isset($data['banner_background_image_style']) && $data['banner_background_image_style'] == "tile") { ?>
#banner {
background: url(<?php echo @$data['banner_background_image'];?>) repeat <?php echo @$data['banner_gradient']['start'];?>;
filter: none;
}
<?php } else { ?>
#banner {
background: url(<?php echo @$data['banner_background_image'];?>) no-repeat center top <?php echo @$data['banner_gradient']['start'];?>;
filter: none;
background-attachment: scroll;
background-size: cover;
-moz-background-size: cover;
-webkit-background-size: cover;
-o-background-size: cover;
}

@media (min-width:1025px) {
#banner {
background-attachment: fixed;
}
}

<?php }} else { ?>
#banner {
background: <?php echo @$data['banner_gradient']['start'];?>;
background: -moz-linear-gradient(top, <?php echo @$data['banner_gradient']['start'];?> 0%, <?php echo @$data['banner_gradient']['end'];?> 100%);
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,<?php echo @$data['banner_gradient']['start'];?>), color-stop(100%,<?php echo @$data['banner_gradient']['end'];?>));
background: -webkit-linear-gradient(top, <?php echo @$data['banner_gradient']['start'];?> 0%,<?php echo @$data['banner_gradient']['end'];?> 100%);
background: -o-linear-gradient(top, <?php echo @$data['banner_gradient']['start'];?> 0%,<?php echo @$data['banner_gradient']['end'];?> 100%);
background: -ms-linear-gradient(top, <?php echo @$data['banner_gradient']['start'];?> 0%,<?php echo @$data['banner_gradient']['end'];?> 100%);
background: linear-gradient(to bottom, <?php echo @$data['banner_gradient']['start'];?> 0%,<?php echo @$data['banner_gradient']['end'];?> 100%);
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='<?php echo @$data['banner_gradient']['start'];?>', endColorstr='<?php echo @$data['banner_gradient']['end'];?>',GradientType=0 );
}
<?php } ?>

.menu .current-menu-item a, .current-menu-parent a, .current_page_parent a {
background: <?php echo @$data['menu_background_active'];?>;
}

#navigation_elements {
border-bottom: 5px solid <?php echo @$data['menu_background_active'];?>;
}

<?php if (isset($data['hide_page_title']) && $data['hide_page_title'] == 1) { ?>
#logo h1 span {
display:none !important;
}
#logo h2#site-description {
display:none !important;
}
<?php } ?>

.newsletter_inner {
background-color: <?php echo @$data['newsletter_background'];?>;
}

.newsletter form {
border: 1px solid  <?php echo @$data['newsletter_background'];?>;
}

.highlighted .pricing_header, .highlighted .pricing_footer {
background-color: <?php echo @$data['pricing_highlight_background'];?>;
}

.highlighted.pricing_block:hover .pricing_header, .highlighted.pricing_block:hover .pricing_footer {
background-color:<?php echo @$data['pricing_highlight_hover_background'];?>;
}

input#submit_comment, input.wpcf7-submit {
background: <?php echo @$data['blog_button_background'];?>;
}

<?php if (isset($data['action_buttons_radius']) && $data['action_buttons_radius'] != 30) { ?>
.buttons,
a.button_buy,
a.button_buy_big,
a.button_try,
a.button_buy_pricing,
.pricing_footer p {
border-radius: <?php echo @$data['action_buttons_radius'];?>px;
-webkit-border-radius: <?php echo @$data['action_buttons_radius'];?>px;
-moz-border-radius: <?php echo @$data['action_buttons_radius'];?>px;
}
<?php } ?>

<?php if (isset($data['action_buttons_radius']) && $data['action_buttons_radius'] != 30 && $data['action_buttons_radius'] != 0 && isset($data['action_buttons_layout']) && $data['action_buttons_layout'] == "Two Buttons/Split-Button") { ?>
a.button_buy {
border-top-right-radius: 0;
border-bottom-right-radius: 0;
-webkit-border-top-right-radius: 0;
-webkit-border-bottom-right-radius: 0;
-moz-border-radius-topright: 0;
-moz-border-radius-bottomright: 0;}

a.button_try {
border-top-left-radius: 0;
border-bottom-left-radius: 0;
-webkit-border-top-left-radius: 0;
-webkit-border-bottom-left-radius: 0;
-moz-border-radius-topleft: 0;
-moz-border-radius-bottomleft: 0;
}

@media screen and (max-width: 460px) {
 a.button_buy, a.button_try {
    border-radius: <?php echo @$data['action_buttons_radius'];?>px;
    -webkit-border-radius: <?php echo @$data['action_buttons_radius'];?>px;
    -moz-border-radius: <?php echo @$data['action_buttons_radius'];?>px;
 }
}
<?php } ?>

<?php if (isset($data['banner_text_adapt']) && $data['banner_text_adapt'] == 1) { ?>
@media screen and (min-width: 1006px) {
#banner {
height:auto;
}
#banner .buttons, #banner .buttons_big {
margin: 0 0 -60px 0 !important;
}
}
<?php } ?>

/* Global Font Styles */
body {
<?php echo justlanded_get_font_style('font_body'); ?>
}

#logo h1, #logo a { /* Logo */
<?php echo justlanded_get_font_style('font_logo'); ?>
}

#logo h2 { /* Logo Tagline */
<?php echo justlanded_get_font_style('font_logo_tagline'); ?>
}

/* Quotes */
.entry-content blockquote {
<?php echo justlanded_get_font_style('font_quotes'); ?>
}

/* Blog Font Styles */
h1 {
<?php echo justlanded_get_font_style('font_heading_h1'); ?>
}

h2 {
<?php echo justlanded_get_font_style('font_heading_h2'); ?>
}

h3 {
<?php echo justlanded_get_font_style('font_heading_h3'); ?>
}

h4 {
<?php echo justlanded_get_font_style('font_heading_h4'); ?>
}

h5 {
<?php echo justlanded_get_font_style('font_heading_h5'); ?>
}

h6 {
<?php echo justlanded_get_font_style('font_heading_h6'); ?>
}

/* Single Page Title */
h1.page-title, .type-page h1.entry-title {
<?php echo justlanded_get_font_style('font_page_title'); ?>
}

/* Widget Title */
h3.widget-title {
<?php echo justlanded_get_font_style('font_widget_title'); ?>
}

/* Meta */

.entry-meta, .entry-meta a {
<?php echo justlanded_get_font_style('font_post_meta'); ?>
}

/* Font styles used for Landing Page */

/* Banner Heading */

#banner h1 {
<?php echo justlanded_get_font_style('font_heading_banner'); ?>
}

#banner h2, #banner h3, #banner h4, #banner h5, #banner h6, #banner h7 {
color: <?php echo $data['font_heading_banner']['color']; ?>;
}

<?php if(isset($data['font_text_banner'])) { ?>
#banner, #banner p {
color: <?php echo $data['font_text_banner']; ?>
}
<?php } ?>

/* Section Title */

.page-template-landingpage-php h2 {
<?php echo justlanded_get_font_style('font_section_title'); ?>
}

/* Section Subtitle */

.page-template-landingpage-php h3 {
<?php echo justlanded_get_font_style('font_section_subtitle'); ?>
}

/* Newsletter */

.newsletter_inner h2 {
<?php echo justlanded_get_font_style('font_newsletter_title'); ?>
}

/* Big Testimonial */

.testimonial_big q {
<?php echo justlanded_get_font_style('font_big_testimonial'); ?>
}

/* Pricing Table */

.pricing_header h4 {
<?php echo justlanded_get_font_style('font_pricing_title'); ?>
}

.pricing_header h5 {
<?php echo justlanded_get_font_style('font_pricing_sub_title'); ?>
}

.highlighted .pricing_header h5 {
color:<?php echo @$data['font_pricing_sub_title_featured']; ?>;
}

.price {
<?php echo justlanded_get_font_style('font_pricing_price'); ?>
}

.price span {
font-size: <?php echo @$data['font_pricing_currency']['size'].@$data['font_pricing_currency']['unit']."\n"; ?>
}

<?php do_action("justlanded_before_custom_css"); ?>

<?php
$global_css = get_option(THEMENAME.'_global_custom_css');
echo $global_css;
?>

/* Buttons */

<?php if (isset($data['buttons_base_color_active']) && $data['buttons_base_color_active'] == 1) { ?>
.buttons span {
background: <?php echo justlanded_button_color("buttons_gradient_middle/start", "#d86600",$data['buttons_base_color']); ?>; /* Old browsers */
background: url(data:image/svg+xml;base64,<?php echo justlanded_get_svg_gradient(justlanded_button_color("buttons_gradient_middle/start", "#d86600",$data['buttons_base_color']), justlanded_button_color("buttons_gradient_middle/end", "#e88c13",$data['buttons_base_color'])); ?>);
background: -moz-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_middle/start", "#d86600",$data['buttons_base_color']); ?> 1%, <?php echo justlanded_button_color("buttons_gradient_middle/end", "#e88c13",$data['buttons_base_color']); ?> 100%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(1%, <?php echo justlanded_button_color("buttons_gradient_middle/start", "#d86600",$data['buttons_base_color']); ?>), color-stop(100%, <?php echo justlanded_button_color("buttons_gradient_middle/end", "#e88c13",$data['buttons_base_color']); ?>)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_middle/start", "#d86600",$data['buttons_base_color']); ?> 1%, <?php echo justlanded_button_color("buttons_gradient_middle/end", "#e88c13",$data['buttons_base_color']); ?> 100%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_middle/start", "#d86600",$data['buttons_base_color']); ?> 1%, <?php echo justlanded_button_color("buttons_gradient_middle/end", "#e88c13",$data['buttons_base_color']); ?> 100%); /* Opera 11.10+ */
background: -ms-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_middle/start", "#d86600",$data['buttons_base_color']); ?> 1%, <?php echo justlanded_button_color("buttons_gradient_middle/end", "#e88c13",$data['buttons_base_color']); ?> 100%); /* IE10+ */
background: linear-gradient(to bottom, <?php echo justlanded_button_color("buttons_gradient_middle/start", "#d86600",$data['buttons_base_color']); ?> 1%, <?php echo justlanded_button_color("buttons_gradient_middle/end", "#e88c13",$data['buttons_base_color']); ?> 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='<?php echo justlanded_button_color("buttons_gradient_middle/start", "#d86600",$data['buttons_base_color']); ?>', endColorstr='<?php echo justlanded_button_color("buttons_gradient_middle/end", "#e88c13",$data['buttons_base_color']); ?>', GradientType=0); /* IE6-8 */
text-shadow: 0 1px 0 <?php echo justlanded_button_color("buttons_text_secondary_shadow", "#f0c08b",$data['buttons_base_color']); ?>;
color: <?php echo justlanded_button_color("buttons_text_secondary", "#833100",$data['buttons_base_color']); ?>;
}

a.button_buy, a.button_buy_big, a.button_buy_pricing, input.submit {
background: <?php echo justlanded_button_color("buttons_gradient_1/start", "#fea817",$data['buttons_base_color']); ?>; /* Old browsers */
background: url(data:image/svg+xml;base64,<?php echo justlanded_get_svg_gradient(justlanded_button_color("buttons_gradient_1/start", "#fea817",$data['buttons_base_color']), justlanded_button_color("buttons_gradient_1/end", "#c54200",$data['buttons_base_color'])); ?>);
background: -moz-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_1/start", "#fea817",$data['buttons_base_color']); ?> 1%, <?php echo justlanded_button_color("buttons_gradient_1/end", "#c54200",$data['buttons_base_color']); ?> 100%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(1%, <?php echo justlanded_button_color("buttons_gradient_1/start", "#fea817",$data['buttons_base_color']); ?>), color-stop(100%, <?php echo justlanded_button_color("buttons_gradient_1/end", "#c54200",$data['buttons_base_color']); ?>)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_1/start", "#fea817",$data['buttons_base_color']); ?> 1%, <?php echo justlanded_button_color("buttons_gradient_1/end", "#c54200",$data['buttons_base_color']); ?> 100%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_1/start", "#fea817",$data['buttons_base_color']); ?> 1%, <?php echo justlanded_button_color("buttons_gradient_1/end", "#c54200",$data['buttons_base_color']); ?> 100%); /* Opera 11.10+ */
background: -ms-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_1/start", "#fea817",$data['buttons_base_color']); ?> 1%, <?php echo justlanded_button_color("buttons_gradient_1/end", "#c54200",$data['buttons_base_color']); ?> 100%); /* IE10+ */
background: linear-gradient(to bottom, <?php echo justlanded_button_color("buttons_gradient_1/start", "#fea817",$data['buttons_base_color']); ?> 1%, <?php echo justlanded_button_color("buttons_gradient_1/end", "#c54200",$data['buttons_base_color']); ?> 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='<?php echo justlanded_button_color("buttons_gradient_1/start", "#fea817",$data['buttons_base_color']); ?>', endColorstr='<?php echo justlanded_button_color("buttons_gradient_1/end", "#c54200",$data['buttons_base_color']); ?>', GradientType=0); /* IE6-8 */
color: <?php echo justlanded_button_color("buttons_text", "#ffffff",$data['buttons_base_color']); ?> !important;
text-shadow: 0 -1px 0 <?php echo justlanded_button_color("buttons_text_shadow", "#af5305",$data['buttons_base_color']); ?>;
}

a.button_buy:hover, a.button_buy_pricing:hover, a.button_buy_big:hover, input.submit:hover {
background: <?php echo justlanded_button_color("buttons_gradient_1_hover/start", "#c54200",$data['buttons_base_color']); ?>; /* Old browsers */
background: url(data:image/svg+xml;base64,<?php echo justlanded_get_svg_gradient(justlanded_button_color("buttons_gradient_1_hover/start", "#c54200",$data['buttons_base_color']), justlanded_button_color("buttons_gradient_1_hover/end", "#fea817",$data['buttons_base_color'])); ?>);
background: -moz-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_1_hover/start", "#c54200",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_1_hover/end", "#fea817",$data['buttons_base_color']); ?> 100%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, <?php echo justlanded_button_color("buttons_gradient_1_hover/start", "#c54200",$data['buttons_base_color']); ?>), color-stop(100%, <?php echo justlanded_button_color("buttons_gradient_1_hover/end", "#fea817",$data['buttons_base_color']); ?>)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_1_hover/start", "#c54200",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_1_hover/end", "#fea817",$data['buttons_base_color']); ?> 100%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_1_hover/start", "#c54200",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_1_hover/end", "#fea817",$data['buttons_base_color']); ?> 100%); /* Opera 11.10+ */
background: -ms-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_1_hover/start", "#c54200",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_1_hover/end", "#fea817",$data['buttons_base_color']); ?> 100%); /* IE10+ */
background: linear-gradient(to bottom, <?php echo justlanded_button_color("buttons_gradient_1_hover/start", "#c54200",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_1_hover/end", "#fea817",$data['buttons_base_color']); ?> 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='<?php echo justlanded_button_color("buttons_gradient_1_hover/start", "#c54200",$data['buttons_base_color']); ?>', endColorstr='<?php echo justlanded_button_color("buttons_gradient_1_hover/end", "#fea817",$data['buttons_base_color']); ?>', GradientType=0); /* IE6-8 */
color: <?php echo justlanded_button_color("buttons_text", "#ffffff",$data['buttons_base_color']); ?> !important;
text-shadow: 0 -1px 0 <?php echo justlanded_button_color("buttons_text_shadow", "#af5305",$data['buttons_base_color']); ?>;
}

a.button_try {
background: <?php echo justlanded_button_color("buttons_gradient_2/start", "#fec32d",$data['buttons_base_color']); ?>; /* Old browsers */
background: url(data:image/svg+xml;base64,<?php echo justlanded_get_svg_gradient(justlanded_button_color("buttons_gradient_2/start", "#fec32d",$data['buttons_base_color']), justlanded_button_color("buttons_gradient_2/end", "#d86600",$data['buttons_base_color'])); ?>);
background: -moz-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_2/start", "#fec32d",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_2/end", "#d86600",$data['buttons_base_color']); ?> 100%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, <?php echo justlanded_button_color("", "#fec32d",$data['buttons_base_color']); ?>), color-stop(100%, <?php echo justlanded_button_color("buttons_gradient_2/end", "#d86600",$data['buttons_base_color']); ?>)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_2/start", "#fec32d",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_2/end", "#d86600",$data['buttons_base_color']); ?> 100%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_2/start", "#fec32d",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_2/end", "#d86600",$data['buttons_base_color']); ?> 100%); /* Opera 11.10+ */
background: -ms-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_2/start", "#fec32d",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_2/end", "#d86600",$data['buttons_base_color']); ?> 100%); /* IE10+ */
background: linear-gradient(to bottom, <?php echo justlanded_button_color("buttons_gradient_2/start", "#fec32d",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_2/end", "#d86600",$data['buttons_base_color']); ?> 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='<?php echo justlanded_button_color("buttons_gradient_2/start", "#fec32d",$data['buttons_base_color']); ?>', endColorstr='<?php echo justlanded_button_color("buttons_gradient_2/end", "#d86600",$data['buttons_base_color']); ?>', GradientType=0); /* IE6-8 */
color: <?php echo justlanded_button_color("buttons_text", "#ffffff",$data['buttons_base_color']); ?> !important;
text-shadow: 0 -1px 0 <?php echo justlanded_button_color("buttons_text_shadow", "#af5305",$data['buttons_base_color']); ?>;
}

a.button_try:hover {
background: <?php echo justlanded_button_color("buttons_gradient_2_hover/start", "#d86600",$data['buttons_base_color']); ?>; /* Old browsers */
background: url(data:image/svg+xml;base64,<?php echo justlanded_get_svg_gradient(justlanded_button_color("buttons_gradient_2_hover/start", "#d86600",$data['buttons_base_color']), justlanded_button_color("buttons_gradient_2_hover/end", "#fec32d",$data['buttons_base_color'])); ?>);
background: -moz-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_2_hover/start", "#d86600",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_2_hover/end", "#fec32d",$data['buttons_base_color']); ?> 99%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, <?php echo justlanded_button_color("buttons_gradient_2_hover/start", "#d86600",$data['buttons_base_color']); ?>), color-stop(99%, <?php echo justlanded_button_color("buttons_gradient_2_hover/end", "#fec32d",$data['buttons_base_color']); ?>)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_2_hover/start", "#d86600",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_2_hover/end", "#fec32d",$data['buttons_base_color']); ?> 99%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_2_hover/start", "#d86600",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_2_hover/end", "#fec32d",$data['buttons_base_color']); ?> 99%); /* Opera 11.10+ */
background: -ms-linear-gradient(top, <?php echo justlanded_button_color("buttons_gradient_2_hover/start", "#d86600",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_2_hover/end", "#fec32d",$data['buttons_base_color']); ?> 99%); /* IE10+ */
background: linear-gradient(to bottom, <?php echo justlanded_button_color("buttons_gradient_2_hover/start", "#d86600",$data['buttons_base_color']); ?> 0%, <?php echo justlanded_button_color("buttons_gradient_2_hover/end", "#fec32d",$data['buttons_base_color']); ?> 99%); /* W3C */
color: <?php echo justlanded_button_color("buttons_text", "#ffffff",$data['buttons_base_color']); ?> !important;
filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='<?php echo justlanded_button_color("", "#d86600",$data['buttons_base_color']); ?>', endColorstr='<?php echo justlanded_button_color("", "#fec32d",$data['buttons_base_color']); ?>', GradientType=0); /* IE6-8 */
}

input.submit {
border: 1px solid <?php echo justlanded_button_color("buttons_newsletter_border1", "#eb9f29",$data['buttons_base_color']); ?>;
color: <?php echo justlanded_button_color("buttons_text", "#ffffff",$data['buttons_base_color']); ?> !important;
}

.newsletter_widget_form input.submit {
border: 1px solid <?php echo justlanded_button_color("buttons_newsletter_border2", "#c14903",$data['buttons_base_color']); ?>;
color: <?php echo justlanded_button_color("buttons_text", "#ffffff",$data['buttons_base_color']); ?> !important;
}

.newsletter input[type="text"] {
border-right: 1px solid <?php echo justlanded_button_color("buttons_newsletter_border2", "#eb9f29",$data['buttons_base_color']); ?>;
}

.mini_newsletter_banner .email {
border-right: 1px solid <?php echo justlanded_button_color("buttons_newsletter_border2", "#c14903",$data['buttons_base_color']); ?>;
}



@media screen and (max-width: 460px) {
 .newsletter input.submit {
  border: 1px solid  <?php echo justlanded_button_color("buttons_newsletter_border2", "#eb9f29",$data['buttons_base_color']); ?>;
 }
}

<?php if (isset($data['buttons_base_text_color']) && strtolower($data['buttons_base_text_color']) != "#ffffff") {
if ((isset($data['buttons_advanced_colors']) && $data['buttons_advanced_colors'] == 0) || !isset($data['buttons_advanced_colors'])) { ?>
a.button_buy, a.button_try, a.button_buy_big, a.button_buy_pricing, input.submit,
a.button_buy:hover, a.button_try:hover, a.button_buy_big:hover, a.button_buy_pricing:hover, input.submit:hover
{
color: <?php echo $data['buttons_base_text_color']; ?> !important;
<?php if (isset($data['buttons_base_text_shadow_color'])) { ?>text-shadow: 0 -1px 0 <?php echo $data['buttons_base_text_shadow_color']; ?>;<?php } ?>
<?php } ?>
}
}
<?php } ?>
<?php } ?>



<?php if (isset($data['banner_full_center_buttons']) && $data['banner_full_center_buttons'] == 1) { ?>
#banner.banner_media_full .buttons, #banner.banner_media_full .buttons_big {
    margin: 0 auto -55px auto;
    float:none !important;
}
@media screen and (max-width: 1005px) and (min-width: 761px) {
    #banner .buttons, #banner .buttons_big {
        margin: 0 auto -50px auto !important;
        float:none !important;
        clear: both;
    }
}
@media screen and (max-width: 760px) {
    #banner .buttons, #banner .buttons_big {
        margin: 35px auto -60px auto !important;
        float:none !important;
        clear: both;
    }
}
.banner_media_full .mini_newsletter_banner {
    margin: 20px auto -55px auto;
    float: none;
}
@media screen and (max-width: 460px) {
    #banner .buttons, #banner .buttons_big {
        margin: 20px 0 0 20px !important;
    }
}
<?php } ?>


<?php if(isset($data['gravity_forms']) && $data['gravity_forms'] == 1) { ?>
.gform_wrapper {
background: #f8f8f8;
padding: 20px !important;
margin: 0 0 20px 0;
border-radius: 5px;
-moz-border-radius: 5px;
-webkit-border-radius: 5px;
overflow: hidden;
clear: both;
}

.gform_wrapper ul li {
list-style: none;
margin:0;
}

.gform_wrapper .gsection {
border-bottom: 1px dotted #CCC;
padding: 0 0 8px 0;
margin: 16px 0;
clear: both;
}

.gform_wrapper .ginput_complex .ginput_right {
width: 49%;
float: right;
}

.gform_wrapper .ginput_complex .ginput_left {
width: 50%;
float: left;
}

.gform_wrapper .ginput_complex label, .gform_wrapper .gfield_time_hour label, .gform_wrapper .gfield_time_minute label, .gform_wrapper .gfield_date_month label, .gform_wrapper .gfield_date_day label, .gform_wrapper .gfield_date_year label, .gform_wrapper .instruction {
display: block;
margin: 3px 0;
font-size: 11px !important;
letter-spacing: 0.5pt;
}

.gform_wrapper .top_label .gfield_label {
margin: 10px 0 4px 0;
font-weight: bold;
display: -moz-inline-stack;
display: inline-block;
line-height: 1.3em;
clear: both;
}


.gform_wrapper .ginput_complex input[type="text"] {
width: 240px;
border-radius: 5px;
webkit-border-radius: 5px;
-moz-border-radius: 5px;
}

.gform_wrapper .ginput_complex .ginput_full input[type=text], .gform_wrapper .ginput_complex .ginput_full input[type=url], .gform_wrapper .ginput_complex .ginput_full input[type=email], .gform_wrapper .ginput_complex .ginput_full input[type=tel], .gform_wrapper .ginput_complex .ginput_full input[type=number], .gform_wrapper .ginput_complex .ginput_full input[type=password] {
width: 565px;
}

.gform_wrapper .ginput_complex input, .gform_wrapper .ginput_complex textarea {
margin: 5px 0 0 0;
border: 1px solid #e5e5e5;
padding: 8px;
clear: both;
}

.gform_wrapper textarea {
clear: both;
margin: 5px 0 0 0;
padding: 8px;
border: 1px solid #e5e5e5;
border-radius: 5px;
width: 99%;
}

.gform_wrapper ul li.gfield {
clear: both;
}

.gform_wrapper  .gfield.gsection {
margin-top: 25px;
}
.gform_wrapper .gsection .gfield_label, .gform_wrapper h2.gsection_title, .gform_wrapper h3.gform_title {
font-weight: bold;
font-size: 1.3em;
}

.gform_wrapper ul.gform_fields li.gfield div.ginput_complex span.ginput_left select,
.gform_wrapper ul.gform_fields li.gfield div.ginput_complex span.ginput_right select,
.gform_wrapper ul.gform_fields li.gfield select {
margin-bottom: 7px;
}

.gform_wrapper .gform_footer {
padding: 16px 0 10px 0;
margin: 16px 0 0 0;
clear: both;
}

.gform_wrapper input[type=hidden],
.gform_wrapper input.gform_hidden,
.gform_wrapper .gform_hidden,
.gform_wrapper .gf_hidden {
display: none !important;
max-height: 1px !important;
overflow: hidden;
}

.gform_wrapper .ginput_full br,
.gform_wrapper .ginput_left br,
.gform_wrapper .ginput_right br {
display: none !important;
}

.gf_clear, .gf_clear_complex {
clear:both;
}
<?php } ?>



<?php echo @$data['custom_css']; ?>

<?php do_action("justlanded_after_dynamic_css_output"); ?>
