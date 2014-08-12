<?php
/*
 * ShapingRain.com JustLanded - A Landing Page That Just Works
 * (C) Copyright 2012-2013 ShapingRain
 *
 * License:		GPLv2
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 2 as published by
 * the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 *
 */
global $justlanded_original_title;
require_once('admin/index.php');

/*
 * Makes certain built-in extensions pluggable (and replaceable by child themes)
 */
if (!defined('JUSTLANDED_MAIN_DIR')) {
	define( 'JUSTLANDED_MAIN_DIR', TEMPLATEPATH . DIRECTORY_SEPARATOR );
}

if (!defined('JUSTLANDED_BLOCKS_DIR')) {
    define( 'JUSTLANDED_BLOCKS_DIR', TEMPLATEPATH . DIRECTORY_SEPARATOR . 'blocks' . DIRECTORY_SEPARATOR);
}

if (!defined('JUSTLANDED_SHORTCODES_DIR')) {
    define( 'JUSTLANDED_SHORTCODES_DIR', TEMPLATEPATH . DIRECTORY_SEPARATOR . 'shortcodes' . DIRECTORY_SEPARATOR);
}

if (!defined('JUSTLANDED_WIDGETS_DIR')) {
    define( 'JUSTLANDED_WIDGETS_DIR', TEMPLATEPATH . DIRECTORY_SEPARATOR . 'widgets' . DIRECTORY_SEPARATOR);
}

if (!defined('JUSTLANDED_METABOXES_DIR')) {
    define( 'JUSTLANDED_METABOXES_DIR', TEMPLATEPATH . DIRECTORY_SEPARATOR . 'metaboxes' . DIRECTORY_SEPARATOR);
}

add_filter('upload_mimes', 'justlanded_custom_upload_mimes');
if (!function_exists('justlanded_custom_upload_mimes')) {
function justlanded_custom_upload_mimes($existing_mimes){
    $existing_mimes['ico'] = 'image/vnd.microsoft.icon'; // for fav icons
    return $existing_mimes;
}
}

add_filter('the_content', 'justlanded_shortcode_empty_paragraph_fix');
if (!function_exists('justlanded_shortcode_empty_paragraph_fix')){
function justlanded_shortcode_empty_paragraph_fix($content)
{
    $array = array (
        '<p>[' => '[',
        ']</p>' => ']',
        ']<br />' => ']'
    );
    $content = strtr($content, $array);
    return $content;
}
}

if (!function_exists('justlanded_parse_shortcode_items')){
function justlanded_parse_shortcode_items($this_content, $defaults = array()) {
    $pattern = '\[(\[?)(item)(?![\w-])([^\]\/]*(?:\/(?!\])[^\]\/]*)*?)(?:(\/)\]|\](?:([^\[]*+(?:\[(?!\/\2\])[^\[]*+)*+)\[\/\2\])?)(\]?)';
    $matches = array();
    preg_match_all("/$pattern/s", $this_content, $matches);

    $items=array();
    if (isset($matches[0]) && is_array($matches[0])) {
        foreach ($matches[0] as $match) {
            $match = str_replace("[item", "", $match);
            $match = rtrim($match, "]");
            $items[] = shortcode_atts($defaults, shortcode_parse_atts($match));
        }
    }
    return $items;
}
}

if (!function_exists('justlanded_get_option')){
function justlanded_get_option($key, $default = "", $data = array()) {
    if (isset($data[$key])) {
        return $data[$key];
    }
    else {
        return $default;
    }
}
}

if (!function_exists('justlanded_responsive_embed')) {
function justlanded_responsive_embed( $output, $url=null, $attr=array() ) {
    /* Based on http://websitesthatdontsuck.com/2011/12/fluid-width-oembed-videos-in-wordpress/ */
    $resize = false;
    $services_allow_responsive = array(
        'vimeo',
        'youtube',
        'dailymotion',
        'viddler.com',
        'hulu.com',
        'blip.tv',
        'revision3.com',
        'funnyordie.com',
        'slideshare',
        'scribd.com',
    );

    foreach ( $services_allow_responsive as $provider ) {
        if ( strstr($url, $provider) ) {
            $resize = true;
            break;
        }
    }

    $attr_pattern = '/(width|height)="[0-9]*"/i';
    $whitespace_pattern = '/\s+/';
    $embed = preg_replace($attr_pattern, "", $output);
    $embed = preg_replace($whitespace_pattern, ' ', $embed);
    $embed = trim($embed);
    $inline_css = (isset($attr['width'])) ? ' style="max-width:'.absint($attr['width']).'px;"':'';
    $output = '<div class="jl_video_container"' . $inline_css . '>';
    $output .= '<div class="jl_video_container-inner">';
    $output .= $embed;
    $output .= "</div></div>";
    return $output;
}
}
add_filter( 'embed_oembed_html', 'justlanded_responsive_embed', 99, 3);

if (!function_exists('justlanded_shortcode_exists')){
function justlanded_shortcode_exists( $shortcode = false ) {
    global $shortcode_tags;
    if ( ! $shortcode )
        return false;
    if ( array_key_exists( $shortcode, $shortcode_tags ) )
        return true;
    return false;
}
}

if (!function_exists('justlanded_get_block')){
function justlanded_get_block($block_tpl, $options) {
    global $data;
	$options = apply_filters ( 'justlanded_block_template_options', $options);
	extract($options);

    if (isset($this_block_type) && function_exists('justlanded_block_' . $this_block_type)) {
        echo call_user_func('justlanded_block_' . $this_block_type) || "";
    } else {
        ob_start();
        include ($block_tpl);
        $out = ob_get_clean();
        return trim($out);
    }
}
}

/* This function is used to determine whether we should link the title and featured image on non-archive and non-index pages */
if (!function_exists('justlanded_is_archive_or_index')){
function justlanded_is_archive_or_index() {
 return (is_archive() || is_front_page() || is_front_page() || is_page() || is_author() || is_category() || is_home() || is_tag());
}
}

if (!function_exists('justlanded_image')){
function justlanded_image($url, $alt = "", $width = 0, $height = 0)
{
    if (trim($url)  == "") return;
    if (trim($alt) == "") $alt = $url;

    $size = '';
    if ($width + $height == 0) {
        // no width or height attribute to assign
    }
    else {
        if ($width > 0) {
            $size .= ' width="'.$width.'"';
        }
        if ($height > 0) {
            $size .= ' height="'.$height.'"';
        }
        $size = ' ' . trim($size);
    }
    echo '<img src="'.$url.'" alt="'.esc_attr($alt).'" title="'.esc_attr($alt).'"'.$size.'>';
}
}

if (!function_exists('justlanded_get_headline')){
function justlanded_get_headline($type = "h2", $headline) {
    if (trim($headline) != "") {
        return "<".$type.">".do_shortcode(stripslashes($headline))."</".$type.">\n";
    }
}
}

/*
 * Return an attachment ID/Post ID based on the url
 * http://philipnewcomer.net/2012/11/get-the-attachment-id-from-an-image-url-in-wordpress/
 */
if (!function_exists('justlanded_get_attachment_id_from_url')){
function justlanded_get_attachment_id_from_url( $attachment_url = '' ) {
    global $wpdb;
    $attachment_id = false;
    // If there is no url, return.
    if ( '' == $attachment_url )
        return;
    // Get the upload directory paths
    $upload_dir_paths = wp_upload_dir();
    // Make sure the upload path base directory exists in the attachment URL, to verify that we're working with a media library image
    if ( false !== strpos( $attachment_url, $upload_dir_paths['baseurl'] ) ) {
        // If this is the URL of an auto-generated thumbnail, get the URL of the original image
        $attachment_url = preg_replace( '/-\d+x\d+(?=\.(jpg|jpeg|png|gif)$)/i', '', $attachment_url );
        // Remove the upload path base directory from the attachment URL
        $attachment_url = str_replace( $upload_dir_paths['baseurl'] . '/', '', $attachment_url );
        // Finally, run a custom database query to get the attachment ID from the modified attachment URL
        $attachment_id = $wpdb->get_var( $wpdb->prepare( "SELECT wposts.ID FROM $wpdb->posts wposts, $wpdb->postmeta wpostmeta WHERE wposts.ID = wpostmeta.post_id AND wpostmeta.meta_key = '_wp_attached_file' AND wpostmeta.meta_value = '%s' AND wposts.post_type = 'attachment'", $attachment_url ) );
    }
    return $attachment_id;
}
}

/*
* Resize images dynamically using WP built-in functions
* originally written by Victor Teixeira
*/
if(!function_exists('justlanded_resize')){
    function justlanded_resize($attach_id = null, $img_url = null, $width, $height, $crop = false){

        if($attach_id){
            // this is an attachment, so we have the ID
            $image_src = wp_get_attachment_image_src($attach_id, 'full');
            $file_path = get_attached_file($attach_id);
        } elseif($img_url){
            // this is not an attachment, let's use the image url
            $file_path = parse_url($img_url);
            $file_path = $_SERVER['DOCUMENT_ROOT'].$file_path['path'];
            // Look for Multisite Path
            if(file_exists($file_path) === false){
                global $blog_id;
                $file_path = parse_url($img_url);
                if(preg_match('/files/', $file_path['path'])){
                    $path = explode('/', $file_path['path']);
                    foreach($path as $k => $v){
                        if($v == 'files'){
                            $path[$k-1] = 'wp-content/blogs.dir/'.$blog_id;
                        }
                    }
                    $path = implode('/', $path);
                }
                if (isset($path)) {
                    $file_path = $_SERVER['DOCUMENT_ROOT'].$path;
                }
                else {
                    $file_path = $_SERVER['DOCUMENT_ROOT'];
                }
            }
            if (file_exists($file_path) && !is_dir($file_path)) {
                @$orig_size = getimagesize($file_path);
                $image_src[0] = $img_url;
                $image_src[1] = $orig_size[0];
                $image_src[2] = $orig_size[1];
            } else {
                $image_src[0] = $img_url;
                $image_src[1] = 0;
                $image_src[2] = 0;
            }
        }
        if (isset($file_path) && $file_path != "") {
            $file_info = pathinfo($file_path);
        } else {
            return;
        }
        // check if file exists
        if (isset($file_info['extension'])) {
            $base_file = $file_info['dirname'].DIRECTORY_SEPARATOR.$file_info['filename'].'.'.$file_info['extension'];
        }
        else {
            $base_file = $file_info['dirname'].DIRECTORY_SEPARATOR.$file_info['filename'];
        }
        if(!file_exists($base_file))
            return;

        if (isset($file_info['extension'])) {
            $extension = '.'. $file_info['extension'];
        }
        else {
            $extension = '.jpg';
        }
        // the image path without the extension
        $no_ext_path = $file_info['dirname'].'/'.$file_info['filename'];
        $cropped_img_path = $no_ext_path.'-'.$width.'x'.$height.$extension;
        // checking if the file size is larger than the target size
        // if it is smaller or the same size, stop right here and return
        if($image_src[1] > $width){
            // the file is larger, check if the resized version already exists (for $crop = true but will also work for $crop = false if the sizes match)
            if(file_exists($cropped_img_path)){
                $cropped_img_url = str_replace(basename($image_src[0]), basename($cropped_img_path), $image_src[0]);
                $justlanded_image = array(
                    'url'   => $cropped_img_url,
                    'width' => $width,
                    'height'    => $height
                );
                return $justlanded_image;
            }
            // $crop = false or no height set
            if($crop == false OR !$height){
                // calculate the size proportionaly
                $proportional_size = wp_constrain_dimensions($image_src[1], $image_src[2], $width, $height);
                $resized_img_path = $no_ext_path.'-'.$proportional_size[0].'x'.$proportional_size[1].$extension;
                // checking if the file already exists
                if(file_exists($resized_img_path)){
                    $resized_img_url = str_replace(basename($image_src[0]), basename($resized_img_path), $image_src[0]);
                    $justlanded_image = array(
                        'url'   => $resized_img_url,
                        'width' => $proportional_size[0],
                        'height'    => $proportional_size[1]
                    );
                    return $justlanded_image;
                }
            }
            // check if image width is smaller than set width
            $img_size = getimagesize($file_path);
            if($img_size[0] <= $width) $width = $img_size[0];
            // Check if GD Library installed
            if(!function_exists('imagecreatetruecolor')){
                echo 'GD Library Error: imagecreatetruecolor does not exist - please contact your webhost and ask them to install the GD library';
                return;
            }
            // no cache files - let's finally resize it
            $new_img_path = image_resize($file_path, $width, $height, $crop);
            $new_img_size = getimagesize($new_img_path);
            $new_img = str_replace(basename($image_src[0]), basename($new_img_path), $image_src[0]);
            // resized output
            $justlanded_image = array(
                'url'   => $new_img,
                'width' => $new_img_size[0],
                'height'    => $new_img_size[1]
            );
            return $justlanded_image;
        }
        // default output - without resizing
        $justlanded_image = array(
            'url'   => $image_src[0],
            'width' => $width,
            'height'    => $height
        );
        return $justlanded_image;
    }
}


// dynamically load existing widgets
if ($handle = opendir(JUSTLANDED_WIDGETS_DIR)) {
    while (false !== ($entry = readdir($handle))) {
        if (substr_count($entry, 'widget-') > 0)
        {
            require_once(JUSTLANDED_WIDGETS_DIR . $entry);
        }
    }
    closedir($handle);
}

// dynamically load existing metabox plug-ins
if ($handle = opendir(JUSTLANDED_METABOXES_DIR)) {
    while (false !== ($entry = readdir($handle))) {
        if (substr_count($entry, 'metabox-') > 0)
        {
            require_once(JUSTLANDED_METABOXES_DIR . $entry);
        }
    }
    closedir($handle);
}

//load custom buttons class
require_once (TEMPLATEPATH . '/libs/class.new_tinymce_btn.php');

// dynamically load existing definitions for shortcodes and tinymce buttons
if ($handle = opendir(JUSTLANDED_SHORTCODES_DIR)) {
    while (false !== ($entry = readdir($handle))) {
        if (substr_count($entry, 'shortcode-') > 0 && substr_count($entry, '.js') > 0)
        {
            //create an instance of the class
            $plain_name = strtolower(trim(str_replace(".js", "", str_replace("shortcode-", "", $entry))));
            $t = new add_new_tinymce_btn('',$plain_name,get_bloginfo('template_url').'/shortcodes/' . $entry);
        }
        if (substr_count($entry, 'shortcode-') > 0 && substr_count($entry, '.php') > 0)
        {
            $plain_name = strtolower(trim(str_replace(".js", "", str_replace("shortcode-", "", $entry))));
            require(JUSTLANDED_SHORTCODES_DIR . $entry);
        }

    }
    closedir($handle);
}

// add custom CSS
add_action('wp_enqueue_scripts', 'justlanded_enqueue_custom_stylesheet');
if(!function_exists('justlanded_enqueue_custom_stylesheet')) {
function justlanded_enqueue_custom_stylesheet() {
    if (file_exists(get_template_directory() . DIRECTORY_SEPARATOR . 'style.custom.css')) {
        wp_register_style('justlanded_custom_stylesheet', get_template_directory_uri() . '/style.custom.css', array(),THEME_VERSION, 'all');
        wp_enqueue_style('justlanded_custom_stylesheet');
    }
}
}


// add dynamic css
//add_action('post_link', 'justlanded_dynamic_css');
add_action( 'wp_print_styles', 'justlanded_dynamic_css' ); // optional alternative
if(!function_exists('justlanded_dynamic_css')){
function justlanded_dynamic_css() {
    global $data;
	if (is_ssl()) $protocol = "https"; else $protocol = "http";
    do_action("justlanded_before_dynamic_css");
    ob_start();
    include(get_template_directory() . DIRECTORY_SEPARATOR . 'dynamic_css.php');
    $css = ob_get_clean();
    echo '<style type="text/css" media="screen">';
    echo "\n".trim($css)."\n";
    echo "</style>\n";
    echo '<!--[if gte IE 9]>'."\n";
    echo '<style type="text/css">'."\n";
    echo '.gradient { filter: none!important;}'."\n";
    echo '</style>'."\n";
    echo '<![endif]-->'."\n";
    echo '<!--[if lt IE 9]>'."\n";
    echo '<script>'."\n";
    echo "    'article aside footer header nav section time'.replace(/\w+/g,function(n){document.createElement(n)})\n";
    echo '</script>'."\n";
    //echo '<script src="'.get_stylesheet_directory_uri().'/scripts/src/css3-mediaqueries.js"></script>'."\n";
    echo '<![endif]-->'."\n";
    do_action("justlanded_after_dynamic_css");
}
}

// add custom/dynamic scripts
add_action('wp_head', 'justlanded_dynamic_scripts_header', 20);
if(!function_exists('justlanded_dynamic_scripts_header')){
    function justlanded_dynamic_scripts_header() {
        global $data;
        $global_header = get_option(THEMENAME.'_global_custom_header');
        do_action("justlanded_before_dynamic_scripts_header");
        if (isset($data['custom_header'])) {
            echo $data['custom_header'];
        }
        echo $global_header;
        do_action("justlanded_after_dynamic_scripts_header");
    }
}

add_action('wp_footer', 'justlanded_dynamic_scripts_footer', 20);
if(!function_exists('justlanded_dynamic_scripts_footer')){
function justlanded_dynamic_scripts_footer() {
    global $data;
    $global_footer = get_option(THEMENAME.'_global_custom_footer');
    do_action("justlanded_before_dynamic_scripts_footer");
    if (isset($data['custom_footer'])) {
        echo $data['custom_footer'];
    }
    echo $global_footer;
    do_action("justlanded_after_dynamic_scripts_footer");
}
}

add_action('wp_footer', 'justlanded_additional_scripts_footer', 10);
if(!function_exists('justlanded_additional_scripts_footer')){
    function justlanded_additional_scripts_footer() {
        global $data;

        if (isset($data['banner_slider_animation_type']))  $banner_slider_animation_type  = $data['banner_slider_animation_type'];          else $banner_slider_animation_type  = "fade";
        if (isset($data['banner_slider_slideshow_speed'])) $banner_slider_slideshow_speed = intval($data['banner_slider_slideshow_speed']); else $banner_slider_slideshow_speed = "7000";
        if (isset($data['banner_slider_animation_speed'])) $banner_slider_animation_speed = intval($data['banner_slider_animation_speed']); else $banner_slider_animation_speed = "600";
        if (isset($data['banner_slider_random']))          $banner_slider_random          = $data['banner_slider_random'];                  else $banner_slider_random           = "false";
        if (isset($data['banner_slider_controls']))        $banner_slider_controls        = $data['banner_slider_controls'];                else $banner_slider_controls         = "true";


        if (isset($data['lightbox_active_for_links']) && $data['lightbox_active_for_links'] == 1 || !isset($data['lightbox_active_for_links'])) {
            $script_lightbox_links = "\n
                    jQuery('a[href*=\".jpg\"], a[href*=\"jpeg\"], a[href*=\".png\"], a[href*=\".gif\"], a.screenshot').touchTouch();
            \n";
        }
        else {
            $script_lightbox_links = "";
        }

        if (isset($data['lightbox_active_for_gallery']) && $data['lightbox_active_for_gallery'] == 1 || !isset($data['lightbox_active_for_gallery'])) {
            $script_lightbox_gallery = "\n$('.section_block_gallery a').touchTouch();\n";
        }
        else {
            $script_lightbox_gallery = "";
        }

        if (isset($data['scroll_up_active']) && $data['scroll_up_active'] == 1 || !isset($data['scroll_up_active'])) {
            $script_scrollup = "\n
            // provide a link to jump to the top quickly
            // credit: http://gazpo.com/2012/02/scrolltop/
            $(window).scroll(function(){
                if ($(this).scrollTop() > 100) {
                    $('.scrollup').fadeIn();
                } else {
                    $('.scrollup').fadeOut();
                }
            });
            $('.scrollup').click(function(){
                $(\"html, body\").animate({ scrollTop: 0 }, 600);
                return false;
            });\n";
        } else {
            $script_scrollup = "";
        }


        do_action("justlanded_before_additional_scripts_footer");

        $scripts="<script type=\"text/javascript\">
        // slider initialization
        ;(function ($, window, undefined) {
            $(document).ready(function() {
                $('#banner_slider').flexslider({
                    animation: '".$banner_slider_animation_type."',
                    slideshowSpeed: ".$banner_slider_slideshow_speed.",
                    animationSpeed: ".$banner_slider_animation_speed.",
                    randomize: ".$banner_slider_random.",
                    controlNav: false,
                    directionNav: ".$banner_slider_controls."
                });
                $('.testimonial-slider-large').flexslider({
                    animation: 'fade',
                    controlNav: false,
                    directionNav: true
                });
                $('.testimonial-slider-small').flexslider({
                    animation: 'fade',
                    controlNav: false,
                    directionNav: false
                });
                ".$script_scrollup."
                ".$script_lightbox_links."
                ".$script_lightbox_gallery."
            });
        })(jQuery, this);
        </script>";
        echo $scripts;
        do_action("justlanded_after_additional_scripts_footer");
    }
}


if(!function_exists('justlanded_get_font_style')){
function justlanded_get_font_style($id) {
    global $data;

    $style = "";
    $weight = "";

    @$color      = $data[$id]['color'];

    if (isset($data[$id]['style'])) {
        $style_tmp  = $data[$id]['style'];
    }
    else {
        $style_tmp = "";
    }

    @$size       = $data[$id]['size'].$data[$id]['unit'];
    @$face       = $data[$id]['face'];

    $default_fonts = array();
    $default_fonts['Arial'] = "Arial, Helvetica, sans-serif";
    $default_fonts['Times New Roman'] = "'Times New Roman', Times, serif";
    $default_fonts['Courier New'] = "'Courier New', Courier, monospace";
    $default_fonts['Georgia'] = "Georgia, 'Times New Roman', Times, serif";
    $default_fonts['Verdana'] = "Verdana, Arial, Helvetica, sans-serif";
    $default_fonts['Geneva'] = "Geneva, Arial, Helvetica, sans-serif";

    $weight = "";
    if (intval($style_tmp) > 0) {
        $weight = intval($style_tmp);
    }
    if (substr_count($style_tmp, "bold") > 0) {
        $weight = "bold";
    }
    if (substr_count($style_tmp, "italic") > 0) {
        $style = "italic";
    }

    $is_default_font = false;
    if (substr_count($face, "*") > 0) {
        $face = str_replace("*", "", $face);
        $face = $default_fonts[$face];
        $is_default_font = true;
    }

    $font_style = "";

    if ($is_default_font == true)
    {
        $font_style .= "font-family: " . $face . ";\n";
    } else {
        $font_style .= "font-family: '" . $face . "', sans-serif;\n";
    }

    $font_style .= "font-size: " . $size . ";\n";

    if ($weight != "") {
        $font_style .= "font-weight: " . $weight . ";\n";
    }

    if ($style != "") {
        $font_style .= "font-style: " . $style . ";\n";
    }

    $font_style .= "color: " . $color . ";\n";

    return $font_style;
}
}

// additional scripts
add_action( 'wp_enqueue_scripts', 'justlanded_scripts_load_cdn' );
if(!function_exists('justlanded_scripts_load_cdn')){
function justlanded_scripts_load_cdn()
{
    wp_register_script('justlanded-slider', get_template_directory_uri() . '/scripts/src/jquery.flexslider.js', array( 'jquery', 'justlanded-site-script'), '1.1', true);
    wp_enqueue_script('justlanded-slider');

    wp_register_script('justlanded-modernizr', get_template_directory_uri() . '/scripts/src/custom.modernizr.js', array( 'jquery' ), THEME_VERSION, true);
    wp_enqueue_script('justlanded-modernizr');

    wp_register_script('justlanded-foundation', get_template_directory_uri() . '/scripts/src/f4/foundation.js', array( 'jquery', 'justlanded-modernizr' ), THEME_VERSION, true);
    wp_enqueue_script('justlanded-foundation');

    wp_register_script('justlanded-placeholder', get_template_directory_uri() . '/scripts/src/f4/foundation.placeholder.js', array( 'jquery', 'justlanded-foundation' ), THEME_VERSION, true);
    wp_enqueue_script('justlanded-placeholder');

    wp_register_script('justlanded-foundation-reveal', get_template_directory_uri() . '/scripts/src/f4/foundation.reveal.js', array( 'jquery', 'justlanded-foundation', 'justlanded-modernizr' ), THEME_VERSION, true);
    wp_enqueue_script('justlanded-foundation-reveal');

    wp_register_script('justlanded-touchtouch', get_template_directory_uri() . '/scripts/src/touchtouch.jquery.js', array( 'jquery' ), THEME_VERSION, true);
    wp_enqueue_script('justlanded-touchtouch');

    wp_register_script('justlanded-tinynav', get_template_directory_uri() . '/scripts/src/tinynav.js', array( 'jquery' ), THEME_VERSION, true);
    wp_enqueue_script('justlanded-tinynav');

    wp_register_script('justlanded-site-script', get_template_directory_uri() . '/scripts/src/site.js', array( 'jquery', 'justlanded-modernizr', 'justlanded-foundation', 'justlanded-foundation-reveal', 'justlanded-placeholder', 'justlanded-touchtouch', 'justlanded-tinynav' ), THEME_VERSION, true);
    wp_enqueue_script('justlanded-site-script');
}
}

// add class to last post
add_filter('post_class', 'justlanded_post_class');
if(!function_exists('justlanded_post_class')){
function justlanded_post_class($classes){
    global $wp_query;
    if(($wp_query->current_post+1) == $wp_query->post_count && $wp_query->post_count != 1) $classes[] = 'last';
    return $classes;
}
}

// localization
add_filter( 'locale', 'justlanded_localized' );
if(!function_exists('justlanded_localized')){
function justlanded_localized($locale) {
    if (isset($_GET['l'])) {
        return $_GET['l'];
    }
    return $locale;
}
}
// misc actions
add_action('after_setup_theme', 'justlanded_setup');
if(!function_exists('justlanded_setup')){
function justlanded_setup()
{
    load_theme_textdomain('justlanded', TEMPLATEPATH . '/languages');
    add_theme_support('automatic-feed-links');
    add_theme_support('post-thumbnails');

// add post-thumbnail functionality
    if (function_exists('add_theme_support')) {
        add_theme_support('post-thumbnails');
    }
    set_post_thumbnail_size(100, 100, true);
    add_image_size('custom-1', 680, 0, false);
    add_image_size('custom-tiny', 60, 60, true);
    set_post_thumbnail_size(150, 150, true);

    global $content_width;
    if (!isset($content_width)) $content_width = 640;
    register_nav_menus(
        array(
            'main-menu' => __('Main Menu', 'justlanded'),
            'footer-menu' => __('Footer Menu', 'justlanded'),
			'custom-menu-1' => __('Custom Menu 1', 'justlanded'),
			'custom-menu-2' => __('Custom Menu 2', 'justlanded')
        )
    );
}
}

add_action('comment_form_before', 'justlanded_enqueue_comment_reply_script');
if(!function_exists('justlanded_enqueue_comment_reply_script')){
function justlanded_enqueue_comment_reply_script()
{
    if (get_option('thread_comments')) {
        wp_enqueue_script('comment-reply');
    }
}
}

add_action('wp', 'justlanded_fetch_original_title');
if(!function_exists('justlanded_fetch_original_title')) {
	function justlanded_fetch_original_title() {
		global $justlanded_original_title;
		$justlanded_original_title = justlanded_get_the_title();
	}
}

if (!function_exists('justlanded_get_the_title')) {
	function justlanded_get_the_title($sep = '', $seplocation = '') {
		global $wpdb, $wp_locale;

		$m = get_query_var('m');
		$year = get_query_var('year');
		$monthnum = get_query_var('monthnum');
		$day = get_query_var('day');
		$search = get_query_var('s');
		$title = '';

		$t_sep = '%WP_TITILE_SEP%'; // Temporary separator, for accurate flipping, if necessary

		// If there is a post
		if ( is_single() || ( is_home() && !is_front_page() ) || ( is_page() && !is_front_page() ) ) {
			$title = single_post_title( '', false );
		}

		// If there's a category or tag
		if ( is_category() || is_tag() ) {
			$title = single_term_title( '', false );
		}

		// If there's a taxonomy
		if ( is_tax() ) {
			$term = get_queried_object();
			$tax = get_taxonomy( $term->taxonomy );
			$title = single_term_title( $tax->labels->name . $t_sep, false );
		}

		// If there's an author
		if ( is_author() ) {
			$author = get_queried_object();
			$title = $author->display_name;
		}

		// If there's a post type archive
		if ( is_post_type_archive() )
			$title = post_type_archive_title( '', false );

		// If there's a month
		if ( is_archive() && !empty($m) ) {
			$my_year = substr($m, 0, 4);
			$my_month = $wp_locale->get_month(substr($m, 4, 2));
			$my_day = intval(substr($m, 6, 2));
			$title = $my_year . ( $my_month ? $t_sep . $my_month : '' ) . ( $my_day ? $t_sep . $my_day : '' );
		}

		// If there's a year
		if ( is_archive() && !empty($year) ) {
			$title = $year;
			if ( !empty($monthnum) )
				$title .= $t_sep . $wp_locale->get_month($monthnum);
			if ( !empty($day) )
				$title .= $t_sep . zeroise($day, 2);
		}

		// If it's a search
		if ( is_search() ) {
			/* translators: 1: separator, 2: search phrase */
			$title = sprintf(__('Search Results %1$s %2$s'), $t_sep, strip_tags($search));
		}

		// If it's a 404 page
		if ( is_404() ) {
			$title = __('Page not found');
		}

		$prefix = '';
		if ( !empty($title) )
			$prefix = " $sep ";

		// Determines position of the separator and direction of the breadcrumb
		if ( 'right' == $seplocation ) { // sep on right, so reverse the order
			$title_array = explode( $t_sep, $title );
			$title_array = array_reverse( $title_array );
			$title = implode( " $sep ", $title_array ) . $prefix;
		} else {
			$title_array = explode( $t_sep, $title );
			$title = $prefix . implode( " $sep ", $title_array );
		}


		// Send it out
		return trim($title);
	}
}


add_filter('wp_title', 'justlanded_filter_wp_title', 10, 2);
if(!function_exists('justlanded_filter_wp_title')){
function justlanded_filter_wp_title($title, $sep)
{
    global $paged, $page;

    if ( is_feed() )
        return $title;

    $title .= get_bloginfo( 'name' );

    $site_description = get_bloginfo( 'description', 'display' );
    if ( $site_description && ( is_home() || is_front_page() ) )
        $title = "$title $sep $site_description";

    if ( $paged >= 2 || $page >= 2 )
        $title = "$title $sep " . sprintf( __( 'Page %s', 'justlanded' ), max( $paged, $page ) );

    return $title;
}
}

add_filter('comment_form_defaults', 'justlanded_comment_form_defaults');
if(!function_exists('justlanded_comment_form_defaults')){
function justlanded_comment_form_defaults($args)
{
    $req = get_option('require_name_email');
    $required_text = sprintf(' ' . __('Required fields are marked %s', 'justlanded'), '*');
    $args['comment_notes_before'] = '';
    $args['comment_notes_after'] = '<p class="comment-notes">' . __('Your email is kept private.', 'justlanded') . ($req ? $required_text : '') . '</p>';
    $args['title_reply'] = __('Post a Comment', 'justlanded');
    $args['title_reply_to'] = __('Post a Reply to %s', 'justlanded');
    $args['id_submit'] = 'submit_comment';
    return $args;
}
}

add_action('init', 'justlanded_add_shortcodes');
if(!function_exists('justlanded_add_shortcodes')){
function justlanded_add_shortcodes()
{
    add_filter('widget_text', 'do_shortcode');
    //add_shortcode('wp_caption', 'fixed_img_caption_shortcode');
    //add_shortcode('caption', 'fixed_img_caption_shortcode');
}
}

if(!function_exists('fixed_img_caption_shortcode')){
function fixed_img_caption_shortcode($attr, $content = null)
{
    $output = apply_filters('img_caption_shortcode', '', $attr, $content);
    if ($output != '') return $output;
    extract(shortcode_atts(array(
        'id' => '',
        'align' => 'alignnone',
        'width' => '',
        'caption' => ''), $attr));
    if (1 > (int)$width || empty($caption))
        return $content;
    if ($id) $id = 'id="' . esc_attr($id) . '" ';
    return '<div ' . $id . 'class="wp-caption ' . esc_attr($align)
        . '">'
        . do_shortcode($content) . '<p class="wp-caption-text">'
        . $caption . '</p></div>';
}
}

add_action('widgets_init', 'justlanded_widgets_init');
if(!function_exists('justlanded_widgets_init')){
function justlanded_widgets_init()
{
    global $data;

    register_sidebar(array(
        'name' => __('Sidebar Widget Area', 'justlanded'),
        'id' => 'primary-widget-area',
        'before_widget' => '<li id="%1$s" class="widget-container %2$s">',
        'after_widget' => "</li>",
        'before_title' => '<h3 class="widget-title">',
        'after_title' => '</h3>',
    ));

    register_sidebar(array(
        'name' => __('Landing Page Full Width Widget 1', 'justlanded'),
        'id' => 'landing-page-widget-1',
        'before_widget' => '',
        'after_widget' => '',
        'before_title' => '<h2 class="widget-title">',
        'after_title' => '</h2>',
    ));

    register_sidebar(array(
        'name' => __('Landing Page Full Width Widget 2', 'justlanded'),
        'id' => 'landing-page-widget-2',
        'before_widget' => '',
        'after_widget' => '',
        'before_title' => '<h2 class="widget-title">',
        'after_title' => '</h2>',
    ));

    register_sidebar(array(
        'name' => __('Landing Page Widgets Row 1', 'justlanded'),
        'id' => 'landing-page-widgets-row-1',
        'before_title' => '<h4 class="widget-title">',
        'after_title' => '</h4>',
    ));

    register_sidebar(array(
        'name' => __('Landing Page Widgets Row 2', 'justlanded'),
        'id' => 'landing-page-widgets-row-2',
        'before_title' => '<h4 class="widget-title">',
        'after_title' => '</h4>',
    ));

    register_sidebar(array(
        'name' => __('Landing Page Posts Sidebar', 'justlanded'),
        'id' => 'landing-page-posts-sidebar',
        'before_widget' => '<li id="%1$s" class="widget-container %2$s">',
        'after_widget' => "</li>",
        'before_title' => '<h4 class="widget-title">',
        'after_title' => '</h4>',
    ));


    register_sidebar(array(
        'name' => __('Footer Widgets', 'justlanded'),
        'id' => 'footer-widgets',
        'before_title' => '<h4 class="widget-title">',
        'after_title' => '</h4>',
    ));

    if (get_query_var('paged')) {
        print ' | ' . __('Page ', 'justlanded') . get_query_var('paged');
    }

}
}


add_filter('dynamic_sidebar_params', 'justlanded_add_classes_to_widgets');
if (!function_exists('justlanded_add_classes_to_widgets')) {
function justlanded_add_classes_to_widgets($params){
    global $justlanded_widgets_row_count;
    if (!is_array($justlanded_widgets_row_count)) {
        $justlanded_widgets_row_count[0] = 0; // footer widgets
        $justlanded_widgets_row_count[1] = 0; // landing page widgets row 1
        $justlanded_widgets_row_count[2] = 0; // landing page widgets row 2
    }

    if ($params[0]['id'] == "landing-page-widgets-row-1"){
        $justlanded_widgets_row_count[1]++;
        $before_widget = $params[0]['before_widget'];
        if ($justlanded_widgets_row_count[1] == 4) {
            $before_widget = str_replace('class="', 'class="widget-container one_fourth last ', $before_widget);
            $params[0]['before_widget'] = $before_widget;
            $justlanded_widgets_row_count[1] = 0;
        }
        elseif($justlanded_widgets_row_count[1] == 1) {
            $before_widget = str_replace('class="', 'class="widget-container one_fourth first ', $before_widget);
            $params[0]['before_widget'] = $before_widget;
        }
        else {
            $before_widget = str_replace('class="', 'class="widget-container one_fourth ', $before_widget);
            $params[0]['before_widget'] = $before_widget;
        }
    }

    if ($params[0]['id'] == "landing-page-widgets-row-2"){
        $justlanded_widgets_row_count[2]++;
        $before_widget = $params[0]['before_widget'];
        if ($justlanded_widgets_row_count[2] == 4) {
            $before_widget = str_replace('class="', 'class="widget-container one_fourth last ', $before_widget);
            $params[0]['before_widget'] = $before_widget;
            $justlanded_widgets_row_count[2] = 0;
        }
        elseif($justlanded_widgets_row_count[2] == 1) {
            $before_widget = str_replace('class="', 'class="widget-container one_fourth first ', $before_widget);
            $params[0]['before_widget'] = $before_widget;
        }
        else {
            $before_widget = str_replace('class="', 'class="widget-container one_fourth ', $before_widget);
            $params[0]['before_widget'] = $before_widget;
        }
    }



    if ($params[0]['id'] == "footer-widgets"){
        $justlanded_widgets_row_count[0]++;
        $before_widget = $params[0]['before_widget'];
        if ($justlanded_widgets_row_count[0] == 4) {
            $before_widget = str_replace('class="', 'class="widget-container one_fourth last ', $before_widget);
            $params[0]['before_widget'] = $before_widget;
            $justlanded_widgets_row_count[0] = 0;
        }
        elseif($justlanded_widgets_row_count[0] == 1) {
            $before_widget = str_replace('class="', 'class="widget-container one_fourth first ', $before_widget);
            $params[0]['before_widget'] = $before_widget;
        }
        else {
            $before_widget = str_replace('class="', 'class="widget-container one_fourth ', $before_widget);
            $params[0]['before_widget'] = $before_widget;
        }
    }

    return $params;
}
}

$preset_widgets = array(
    'primary-aside' => array('search', 'pages', 'categories', 'archives'),
);

add_filter('manage_pages_columns', 'justlanded_posts_columns');
if(!function_exists('justlanded_posts_columns')) {
function justlanded_posts_columns($columns) {
    $columns['profile'] = 'Profile';
    return $columns;
}
}

add_action('manage_pages_custom_column',  'justlanded_posts_show_columns');
if(!function_exists('justlanded_posts_show_columns')) {
function justlanded_posts_show_columns($name) {
    global $post;
    switch ($name) {
        case 'profile':
            $profile = get_post_meta($post->ID, 'justlanded_meta_box_selectinstance_select', true);
            if ($profile == 0) $profile = "default";
            echo $profile;
    }
}
}

add_filter('manage_edit-page_sortable_columns', 'justlanded_posts_column_sortable' );
if(!function_exists('justlanded_posts_column_sortable')) {
function justlanded_posts_column_sortable( $columns ) {
    $columns['profile'] = 'profile';
    return $columns;
}
}

add_filter('request', 'justlanded_posts_column_sortable_orderby');
if(!function_exists('justlanded_posts_column_sortable_orderby')) {
function justlanded_posts_column_sortable_orderby($vars) {
    if (isset( $vars['orderby'] ) && $vars['orderby'] == 'profile') {
        $vars = array_merge($vars, array(
            'meta_key' => 'justlanded_meta_box_selectinstance_select',
            'orderby' => 'meta_value_num'
        ) );
    }
    return $vars;
}
}

if(!function_exists('justlanded_catz')){
function justlanded_catz($glue)
{
    $current_cat = single_cat_title('', false);
    $separator = "\n";
    $cats = explode($separator, get_the_category_list($separator));
    foreach ($cats as $i => $str) {
        if (strstr($str, ">$current_cat<")) {
            unset($cats[$i]);
            break;
        }
    }
    if (empty($cats))
        return false;
    return trim(join($glue, $cats));
}
}

if(!function_exists('justlanded_tag_it')){
function justlanded_tag_it($glue)
{
    $current_tag = single_tag_title('', '', false);
    $separator = "\n";
    $tags = explode($separator, get_the_tag_list("", "$separator", ""));
    foreach ($tags as $i => $str) {
        if (strstr($str, ">$current_tag<")) {
            unset($tags[$i]);
            break;
        }
    }
    if (empty($tags))
        return false;
    return trim(join($glue, $tags));
}
}

if(!function_exists('justlanded_commenter_link')){
function justlanded_commenter_link()
{
    $commenter = get_comment_author_link();
    echo '<span class="fn n">' . $commenter . '</span>';
}
}

if(!function_exists('justlanded_avatar')){
function justlanded_avatar()
{
    $avatar_email = get_comment_author_email();
    $avatar = str_replace("class='avatar", "class='photo avatar", get_avatar($avatar_email, 60));
    echo $avatar;
}
}

if(!function_exists('justlanded_custom_comments')){
function justlanded_custom_comments($comment, $args, $depth)
{
    $GLOBALS['comment'] = $comment;
    $GLOBALS['comment_depth'] = $depth;
    ?>
<li id="comment-<?php comment_ID() ?>" <?php comment_class() ?>>
<div class="comment-wrapper vcard">
    <div class="comment-author"><?php justlanded_avatar() ?></div>
    <div class="comment-meta"><?php justlanded_commenter_link() ?> <?php printf(__('%1$s at %2$s', 'justlanded'), get_comment_date(), get_comment_time()); ?>
        <span class="meta-sep"></span>
        <?php edit_comment_link(__('Edit', 'justlanded'), ' <span class="meta-sep"> | </span> <span class="edit-link">', '</span>'); ?>
        <?php
        if ($args['type'] == 'all' || get_comment_type() == 'comment') :
            comment_reply_link(array_merge($args, array(
                'reply_text' => __('Reply', 'justlanded'),
                'login_text' => __('Login to reply.', 'justlanded'),
                'depth' => $depth,
                'before' => '<div class="comment-reply-link">',
                'after' => '</div>'
            )));
        endif;
        ?>
    </div>
    <?php if ($comment->comment_approved == '0') {
    echo "\t\t\t\t\t<span class=\"unapproved\">";
    _e('Your comment is awaiting moderation.', 'justlanded');
    echo "</span>\n";
} ?>
    <div class="comment-content">
        <?php comment_text() ?>
    </div>
</div>
    <?php
}
}

if(!function_exists('justlanded_custom_pings')){
function justlanded_custom_pings($comment, $args, $depth)
{
    $GLOBALS['comment'] = $comment;
    ?>
<li id="comment-<?php comment_ID() ?>" <?php comment_class() ?>>
    <div class="comment-author"><?php printf(__('By %1$s on %2$s at %3$s', 'justlanded'),
        get_comment_author_link(),
        get_comment_date(),
        get_comment_time());
        edit_comment_link(__('Edit', 'justlanded'), ' <span class="meta-sep"> | </span> <span class="edit-link">', '</span>'); ?></div>
    <?php if ($comment->comment_approved == '0') {
    echo "\t\t\t\t\t<span class=\"unapproved\">";
    _e('Your trackback is awaiting moderation.', 'justlanded');
    echo "</span>\n";
} ?>
    <div class="comment-content">
        <?php comment_text() ?>
    </div>
<?php }
}

function justlanded_get_svg_gradient($start, $end) {
    $output = '<?xml version="1.0"?>';
    $output .= '<svg xmlns="http://www.w3.org/2000/svg" version="1.1" width="100%" height="100%">
        <defs>
            <linearGradient id="linear-gradient" x1="0%" y1="0%" x2="0%" y2="100%">
                <stop offset="0%" stop-color="'.$start.'" stop-opacity="1"/>
                <stop offset="100%" stop-color="'.$end.'" stop-opacity="1"/>
            </linearGradient>
        </defs>
        <rect width="100%" height="100%" fill="url(#linear-gradient)"/>
    </svg>';
    return base64_encode($output);
}

function rgb_hsl($rgb){
    $var_min = min($rgb[0],$rgb[1],$rgb[2]);
    $var_max = max($rgb[0],$rgb[1],$rgb[2]);
    $del_max = $var_max - $var_min;

    $l = ($var_max + $var_min) / 2;

    if ($del_max == 0){
        $h = 0;
        $s = 0;
    }
    else{
        if ($l < 0.5){
            $s = $del_max / ($var_max + $var_min);
        }
        else{
            $s = $del_max / (2 - $var_max - $var_min);
        }
        $del_r = ((($var_max - $rgb[0]) / 6) + ($del_max / 2)) / $del_max;
        $del_g = ((($var_max - $rgb[1]) / 6) + ($del_max / 2)) / $del_max;
        $del_b = ((($var_max - $rgb[2]) / 6) + ($del_max / 2)) / $del_max;
        if ($rgb[0] == $var_max){
            $h = $del_b - $del_g;
        }
        elseif ($rgb[1] == $var_max){
            $h = (1 / 3) + $del_r - $del_b;
        }
        elseif ($rgb[2] == $var_max){
            $h = (2 / 3) + $del_g - $del_r;
        }
        if ($h < 0){
            $h += 1;
        }
        if ($h > 1){
            $h -= 1;
        }
    }
    return "$h,$s,$l";
}

function hue_2_rgb($v1,$v2,$vh){
    if ($vh < 0){
        $vh += 1;
    }
    if ($vh > 1){
        $vh -= 1;
    }
    if ((6 * $vh) < 1){
        return ($v1 + ($v2 - $v1) * 6 * $vh);
    }
    if ((2 * $vh) < 1){
        return ($v2);
    }
    if ((3 * $vh) < 2){
        return ($v1 + ($v2 - $v1) * ((2 / 3 - $vh) * 6));
    }
    return ($v1);
}

function justlanded_button_color($setting, $rgb, $fav)
{
    global $data;
    if (isset($data['buttons_advanced_colors']) && $data['buttons_advanced_colors'] == 1) {
        if (substr_count($setting, "/") > 0) {
            $params = explode("/", $setting);
            if (isset($data[$params[0]][$params[1]])) {
                return $data[$params[0]][$params[1]];
            }
        }
        else {
            if (isset($data[$setting])) {
                return $data[$setting];
            }
        }
    }

    if(preg_match('/^#{0,1}([a-f0-9]{6,6})$/i', $rgb, $matches))
    {
        $colors = chunk_split($matches[1],2);
        $rgbHex = explode("\n", $colors);
        foreach($rgbHex as $key => $val)
        {
            $rgbDec[$key] = (hexdec($val)/256);
        }
        //// yeah
        $first_sl = rgb_hsl(array($rgbDec[0],$rgbDec[1],$rgbDec[2]));
        $first_sl = explode(",",$first_sl);
    }
    else
    {
        return(FALSE); // input not in "#xxxxxx" or "xxxxxx" format
    }
    if(preg_match('/^#{0,1}([a-f0-9]{6,6})$/i', $fav, $matches))
    {
        $colors = chunk_split($matches[1],2);
        $rgbHex = explode("\n", $colors);
        foreach($rgbHex as $key => $val)
        {
            $rgbDec[$key] = (hexdec($val)/256);
        }

        $second_hsl = rgb_hsl(array($rgbDec[0],$rgbDec[1],$rgbDec[2]));
        $final = explode(",",$second_hsl);

        if ($first_sl[1] == 0){
            $r = $first_sl[2] * 255;
            $g = $first_sl[2] * 255;
            $b = $first_sl[2] * 255;
        }
        else{
            if ($first_sl[2] < 0.5){
                $var_2 = $first_sl[2] * (1 + $first_sl[1]);
            }
            else{
                $var_2 = ($first_sl[2] + $first_sl[1]) - ($first_sl[1] * $first_sl[2]);
            }
            $var_1 = 2 * $first_sl[2] - $var_2;
            $r = 255 * hue_2_rgb($var_1,$var_2,$final[0] + (1 / 3));
            $g = 255 * hue_2_rgb($var_1,$var_2,$final[0]);
            $b = 255 * hue_2_rgb($var_1,$var_2,$final[0] - (1 / 3));
        }


        $rhex = sprintf("%02X",round($r));
        $ghex = sprintf("%02X",round($g));
        $bhex = sprintf("%02X",round($b));
        $rgbhex = $rhex.$ghex.$bhex;
        return "#" . $rgbhex;
    }
    else
    {
        return(false);
    }
}


if(!function_exists('justlanded_content_filters')){
    function justlanded_content_filters($content, $type = "")
    {
        global $data;
        if ($type == "page_content") {
            if (!isset($data['content_filter_page_content'])) {
                $filter = "thecontent";

            } else {
                $filter = $data['content_filter_page_content'];
            }
        } else {
            if (!isset($data['content_filter'])) {
                $filter = "alternative";

            } else {
                $filter = $data['content_filter'];
            }
        }


        if ($filter == "alternative") {
            $content = apply_filters('justlanded_the_content', do_shortcode($content));
        }
        elseif ($filter == "shortcodes") {
            $content = do_shortcode($content);
        }
        elseif ($filter == "none") {
            // no filter wanted -- do nothing
        }
        else { // the_content shall be applied
            $content = apply_filters('the_content', $content);
        }

        return $content;
    }
}
add_filter( 'justlanded_the_content', 'wptexturize');
add_filter( 'justlanded_the_content', 'convert_smilies');
add_filter( 'justlanded_the_content', 'convert_chars');


if(!function_exists('justlanded_get_youtube_id')) {
	function justlanded_get_youtube_id($url) {
		// http://stackoverflow.com/questions/2068344/how-to-get-thumbnail-of-youtube-video-link-using-youtube-api
		$urls = parse_url($url);

		//expect url is http://youtu.be/abcd, where abcd is video iD
		if ($urls['host'] == 'youtu.be') :
			$imgPath = ltrim($urls['path'],'/');
		//expect  url is http://www.youtube.com/embed/abcd
		elseif (strpos($urls['path'],'embed') == 1) :
			$imgPath = end(explode('/',$urls['path']));
		//expect url is abcd only
		elseif (strpos($url,'/') === false):
			$imgPath = $url;
		//expect url is http://www.youtube.com/watch?v=abcd
		else :
			parse_str($urls['query']);
			$imgPath = $v;

		endif;
	}
}

// yariki shortcode
include_once( dirname(__FILE__).'/downloadlink.php');
function auto_start_download_trial(){
    $filename = getDownloadUrlforTrial();
    return '<iframe width="0" height="0" src="'.$filename.'"></iframe>';
}
add_shortcode('auto_start_download', 'auto_start_download_trial');

function auto_start_download_full(){
    $filename = getDownloadUrlForLastVersion();
    return '<iframe width="0" height="0" src="'.$filename.'"></iframe>';
}
add_shortcode('auto_start_downlodFull', 'auto_start_download_full');

function get_download_link($attr){
    extract(shortcode_atts(array(
        'title' => '',
        'caption' => ''
    ),$attr));
    
    $filename = getDownloadUrlforTrial();
    return '<a title="'.$title.'" href="'.$filename.'">'.$caption.'</a>';
}
add_shortcode('get_download_link', 'get_download_link');

function get_download_link_full($attr){
    extract(shortcode_atts(array(
        'title' => '',
        'caption' => ''
    ),$attr));
    
    $filename = getDownloadUrlForLastVersion();
    return '<a title="'.$title.'" href="'.$filename.'">'.$caption.'</a>';
}
add_shortcode('get_download_link_full', 'get_download_link_full');

function insert_payment_table(){
    $string_data = '<input type="hidden" id="insertTable"></input>';
    return  $string_data;
}
add_shortcode('insert_payment_table', 'insert_payment_table');

include(dirname(__FILE__).'/PaymentSettings.php');
function insert_gumroad($attr){

    global $AppPriceAll,$CurrencySign;
    
    extract(shortcode_atts(array(
        'title' => '',
        'price' => '',
        'url' => '',
        'button_caption'=> ''
    ),$attr));
    $html = '<div id="pricing-panel"><div id="the-price"> <span class="currency">'.$CurrencySign.'</span><span class="price-number">'.$AppPriceAll.'</span>/yr<span class="price-text">'.$title.'</span></div><a class="button_gum" href="'.$url.'">'.$button_caption.'</a></div>';
    return $html;
}
add_shortcode('insert_gumroad', 'insert_gumroad');

// yariki filters

//add_filter('wpcf7_posted_data', 'wpcf7_generate_Lime_Key_posted_data');
//include_once(dirname(__FILE__) .'/PaymentSettings.php');
//include_once (dirname(__FILE__) . '/LimeLM.php');
//function wpcf7_generate_Lime_Key_posted_data($posted_data) {
//
//    if (!isset($posted_data["your-email"]))
//        return $posted_data;
//
//    $emailCustomer = $posted_data["your-email"];
//
//    $isOk = false;
//
//
//    global $LimeLM_VersionID, $LimeLM_ApiKey, $CompanyName, $AppName, $InstallerPage, $LicenseYear, $TrialField, $IsTrial, $UserEmail, $TimesUsed;
//
//    $date = new DateTime(date('Y-m-d H:i:s'));
//    $date->modify('+14 day');
//    $trialDate = $date->format('Y-m-d H:i:s');
//
//    $errors = false;
//
//    // set your API key
//    LimeLM::SetAPIKey($LimeLM_ApiKey);
//
//    try {
//        // Generate the product key - set the number of activations using the quantity
//		debug_log($TrialField."  ".$trialDate,true);
//		debug_log($UserEmail."  ".$emailCustomer,true);
//		debug_log($IsTrial."  1",true);
//		debug_log($TimesUsed."  0",true);
//		
//		
//		
//        $xml = new SimpleXMLElement(LimeLM::GeneratePKeys($LimeLM_VersionID, 1, 1, $emailCustomer, array($TrialField, $UserEmail, $IsTrial, $TimesUsed), array($trialDate, $emailCustomer, "1", "0")));
//        debug_log('Generating keys', true);
//        debug_log("Xml: ".$xml->asXML(),true);
//		
//		
//		
//		
//        $isOk = $xml['stat'] == 'ok';
//        if (!$isOk) { //failure
//            // use the error code & message
//            debug_log('Failed to generate product keys: (' . $xml->err['code'] . ') ' . $xml->err['msg'], false);
//        }
//    } catch (Exception $e) {
//        debug_log('Failed to generate product keys, caught exception: ' . $e->getMessage(), false);
//    }
//
//    $emailBody = $emailCustomer . ',
//    
//    Thank you for trying ' . $AppName . '
//    The'.$AppName.' is licensed for ' . 14 . ' days.
//        
//The ' . $AppName . ' team';
//    // Send Email to the buyer
////    $emailSent = mail($emailCustomer, 'Your ' . $AppName . ' trial product key', $emailBody, $headers);
////
////    if (!$emailSent) {
////        $errors = true;
////        debug_log('Error sending product Email to ' . $emailCustomer . '.', false);
////    }
//
//    LimeLM::CleanUp();
//
//    return $posted_data;
//}
