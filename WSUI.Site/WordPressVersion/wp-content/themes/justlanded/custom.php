<?php
/*
 * This file contains customization functions to be called by the theme if one of these is defined using
 * a customization shortcode. These are designed to override theme functions without a child theme or
 * modifications of core theme files.
 *
 * create_function() is not used here due to its known memory and performance issues
 *
 * The same hooks can be used by plug-ins initialized before the theme is loaded,
 * or by a child theme.
 *
 */
global $justlanded_custom_blocks;

/* ********************************************************************************
 * HEADER SECTION
 ******************************************************************************** */

/*
 * Content before wrapper
 * [custom_block id="justlanded_before_wrapper"][/custom_block]
 *
 */
if (isset($justlanded_custom_blocks['justlanded_before_wrapper']) && !function_exists('justlanded_before_wrapper')) {
	function justlanded_before_wrapper() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_before_wrapper'];
	}
	add_action('justlanded_before_wrapper', 'justlanded_before_wrapper');
}


/*
 * Content before the header
 * [custom_block id="justlanded_before_head"][/custom_block]
 *
 */
if (isset($justlanded_custom_blocks['justlanded_before_head']) && !function_exists('justlanded_before_head')) {
	function justlanded_before_head() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_before_head'];
	}
	add_action('justlanded_before_head', 'justlanded_before_head');
}

/*
 * Replacement for entire header section
 * [custom_block id="justlanded_head_above_banner"][/custom_block]
 *
 */
if (isset($justlanded_custom_blocks['justlanded_head_above_banner']) && !function_exists('justlanded_head_above_banner')) {
	function justlanded_head_above_banner() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_head_above_banner'];
	}
	add_action('justlanded_head_above_banner', 'justlanded_head_above_banner');
}


/*
 * Replace contact section
 * [custom_block id="justlanded_top_contact"][/custom_block]
 *
 */
if (isset($justlanded_custom_blocks['justlanded_top_contact']) && !function_exists('justlanded_top_contact')) {
	function justlanded_top_contact() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_top_contact'];
	}
	add_action('justlanded_top_contact', 'justlanded_top_contact');
}

/*
 * Replace logo section
 * [custom_block id="justlanded_logo"][/custom_block]
 *
 */
if (isset($justlanded_custom_blocks['justlanded_logo']) && !function_exists('justlanded_logo')) {
	function justlanded_logo() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_logo'];
	}
	add_action('justlanded_logo', 'justlanded_logo');
}


/*
 * Content in place of top menu (replaces built-in menu output)
 * [custom_block id="justlanded_top_menu"][/custom_block]
 *
 */
if (isset($justlanded_custom_blocks['justlanded_top_menu']) && !function_exists('justlanded_top_menu')) {
	function justlanded_top_menu() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_top_menu'];
	}
	add_action('justlanded_top_menu', 'justlanded_top_menu');
}

/*
 * Content after top menu
 * [custom_block id="justlanded_after_top_menu"][/custom_block]
 *
 */
if (isset($justlanded_custom_blocks['justlanded_after_top_menu']) && !function_exists('justlanded_after_top_menu')) {
	function justlanded_after_top_menu() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_after_top_menu'];
	}
	add_action('justlanded_after_top_menu', 'justlanded_after_top_menu');
}

/*
 * Content below the header
 * [custom_block id="justlanded_after_head"][/custom_block]
 *
 */
if (isset($justlanded_custom_blocks['justlanded_after_head']) && !function_exists('justlanded_after_head')) {
	function justlanded_after_head() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_after_head'];
	}
	add_action('justlanded_after_head', 'justlanded_after_head');
}

/* ********************************************************************************
 * LANDING PAGE TEMPLATE: GENERAL
 ******************************************************************************** */

/*
 * Output before and after any block template is rendered
 * [custom_block id="justlanded_before_block_template"][/custom_block]
 * [custom_block id="justlanded_after_block_template"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_before_block_template']) && !function_exists('justlanded_before_block_template')) {
	function justlanded_before_block_template() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_before_block_template'];
	}
	add_action('justlanded_before_block_template', 'justlanded_before_block_template');
}

if (isset($justlanded_custom_blocks['justlanded_after_block_template']) && !function_exists('justlanded_after_block_template')) {
	function justlanded_after_block_template() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_after_block_template'];
	}
	add_action('justlanded_after_block_template', 'justlanded_after_block_template');
}

/* ********************************************************************************
 * LANDING PAGE TEMPLATE: BANNER
 ******************************************************************************** */

/*
 * PAGE BANNER
 */

/*
 * Output before and after the page banner block is rendered
 * [custom_block id="justlanded_before_page_banner"][/custom_block]
 * [custom_block id="justlanded_after_page_banner"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_before_page_banner']) && !function_exists('justlanded_before_page_banner')) {
	function justlanded_before_page_banner() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_before_page_banner'];
	}
	add_action('justlanded_before_page_banner', 'justlanded_before_page_banner');
}

if (isset($justlanded_custom_blocks['justlanded_after_page_banner']) && !function_exists('justlanded_after_page_banner')) {
	function justlanded_after_page_banner() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_after_page_banner'];
	}
	add_action('justlanded_after_page_banner', 'justlanded_after_page_banner');
}

/*
 * LANDING PAGE BANNER
 */

/*
 * Replace entire banner section
 * [custom_block id="justlanded_banner"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_banner']) && !function_exists('justlanded_banner')) {
	function justlanded_banner() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_banner'];
	}
	add_action('justlanded_banner', 'justlanded_banner');
}

/*
 * Before/after banner
 * [custom_block id="justlanded_before_banner"][/custom_block]
 * [custom_block id="justlanded_after_banner"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_before_banner']) && !function_exists('justlanded_before_banner')) {
	function justlanded_before_banner() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_before_banner'];
	}
	add_action('justlanded_before_banner', 'justlanded_before_banner');
}

if (isset($justlanded_custom_blocks['justlanded_after_banner']) && !function_exists('justlanded_after_banner')) {
	function justlanded_after_banner() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_after_banner'];
	}
	add_action('justlanded_after_banner', 'justlanded_after_banner');
}

/*
 * Replace 'full width' banner
 * [custom_block id="justlanded_banner_full"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_banner_full']) && !function_exists('justlanded_banner_full')) {
	function justlanded_banner_full() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_banner_full'];
	}
	add_action('justlanded_banner_full', 'justlanded_banner_full');
}

/*
 * Replace 'full width' banner
 * [custom_block id="justlanded_banner_full"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_banner_full']) && !function_exists('justlanded_banner_full')) {
	function justlanded_banner_full() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_banner_full'];
	}
	add_action('justlanded_banner_full', 'justlanded_banner_full');
}

/*
 * Banner media section
 * [custom_block id="justlanded_banner_media_image"][/custom_block]
 * [custom_block id="justlanded_banner_media_slider"][/custom_block]
 * [custom_block id="justlanded_banner_media_video"][/custom_block]
 * [custom_block id="justlanded_banner_media_freeform"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_banner_media_image']) && !function_exists('justlanded_banner_media_image')) {
	function justlanded_banner_media_image() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_banner_media_image'];
	}
	add_action('justlanded_banner_media_image', 'justlanded_banner_media_image');
}

if (isset($justlanded_custom_blocks['justlanded_banner_media_slider']) && !function_exists('justlanded_banner_media_slider')) {
	function justlanded_banner_media_slider() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_banner_media_slider'];
	}
	add_action('justlanded_banner_media_slider', 'justlanded_banner_media_slider');
}

if (isset($justlanded_custom_blocks['justlanded_banner_media_video']) && !function_exists('justlanded_banner_media_video')) {
	function justlanded_banner_media_video() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_banner_media_video'];
	}
	add_action('justlanded_banner_media_video', 'justlanded_banner_media_video');
}

if (isset($justlanded_custom_blocks['justlanded_banner_media_freeform']) && !function_exists('justlanded_banner_media_freeform')) {
	function justlanded_banner_media_freeform() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_banner_media_freeform'];
	}
	add_action('justlanded_banner_media_freeform', 'justlanded_banner_media_freeform');
}


/*
 * Output to replace the page banner
 * [custom_block id="justlanded_page_banner"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_page_banner']) && !function_exists('justlanded_page_banner')) {
	function justlanded_page_banner() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_page_banner'];
	}
	add_action('justlanded_page_banner', 'justlanded_page_banner');
}



/* ********************************************************************************
 * FOOTER
 ******************************************************************************** */

/*
 * Before footer output
 * [custom_block id="justlanded_before_footer"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_before_footer']) && !function_exists('justlanded_before_footer')) {
	function justlanded_before_footer() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_before_footer'];
	}
	add_action('justlanded_before_footer', 'justlanded_before_footer');
}

/*
 * After footer output
 * [custom_block id="justlanded_after_footer"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_after_footer']) && !function_exists('justlanded_after_footer')) {
	function justlanded_after_footer() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_after_footer'];
	}
	add_action('justlanded_after_footer', 'justlanded_after_footer');
}

/*
 * Before footer copyright notice
 * [custom_block id="justlanded_before_footer_text"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_before_footer_text']) && !function_exists('justlanded_before_footer_text')) {
	function justlanded_before_footer_text() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_before_footer_text'];
	}
	add_action('justlanded_before_footer_text', 'justlanded_before_footer_text');
}

/*
 * After footer copyright notice
 * [custom_block id="justlanded_before_footer_text"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_after_footer_text']) && !function_exists('justlanded_after_footer_text')) {
	function justlanded_after_footer_text() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_after_footer_text'];
	}
	add_action('justlanded_after_footer_text', 'justlanded_after_footer_text');
}

/*
 * Replace footer menu
 * [custom_block id="justlanded_footer_menu"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_footer_menu']) && !function_exists('justlanded_footer_menu')) {
	function justlanded_footer_menu() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_footer_menu'];
	}
	add_action('justlanded_footer_menu', 'justlanded_footer_menu');
}

/*
 * Replace entire footer section
 * [custom_block id="justlanded_footer"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_footer']) && !function_exists('justlanded_footer')) {
	function justlanded_footer() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_footer'];
	}
	add_action('justlanded_footer', 'justlanded_footer');
}

/*
 * OTHER TEMPLATES
 */

/*
 * Add content to blog
 * [custom_block id="justlanded_before_blog_index_content"][/custom_block]
 * [custom_block id="justlanded_after_blog_index_content"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_before_blog_index_content']) && !function_exists('justlanded_before_blog_index_content')) {
	function justlanded_before_blog_index_content() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_before_blog_index_content'];
	}
	add_action('justlanded_before_blog_index_content', 'justlanded_before_blog_index_content');
}

if (isset($justlanded_custom_blocks['justlanded_after_blog_index_content']) && !function_exists('justlanded_after_blog_index_content')) {
	function justlanded_after_blog_index_content() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_after_blog_index_content'];
	}
	add_action('justlanded_after_blog_index_content', 'justlanded_after_blog_index_content');
}


/*
 * Add content to pages
 * [custom_block id="justlanded_before_page_content"][/custom_block]
 * [custom_block id="justlanded_after_page_content"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_before_page_content']) && !function_exists('justlanded_before_page_content')) {
	function justlanded_before_page_content() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_before_page_content'];
	}
	add_action('justlanded_before_page_content', 'justlanded_before_page_content');
}

if (isset($justlanded_custom_blocks['justlanded_after_page_content']) && !function_exists('justlanded_after_page_content')) {
	function justlanded_after_page_content() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_after_page_content'];
	}
	add_action('justlanded_after_page_content', 'justlanded_after_page_content');
}

/*
 * Add content to archives
 * [custom_block id="justlanded_before_archive_content"][/custom_block]
 * [custom_block id="justlanded_after_archive_content"][/custom_block]
 *
 */

if (isset($justlanded_custom_blocks['justlanded_before_archive_content']) && !function_exists('justlanded_before_archive_content')) {
	function justlanded_before_archive_content() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_before_archive_content'];
	}
	add_action('justlanded_before_archive_content', 'justlanded_before_archive_content');
}

if (isset($justlanded_custom_blocks['justlanded_after_archive_content']) && !function_exists('justlanded_after_archive_content')) {
	function justlanded_after_archive_content() {
		global $justlanded_custom_blocks;
		echo $justlanded_custom_blocks['justlanded_after_archive_content'];
	}
	add_action('justlanded_after_archive_content', 'justlanded_after_archive_content');
}


?>
