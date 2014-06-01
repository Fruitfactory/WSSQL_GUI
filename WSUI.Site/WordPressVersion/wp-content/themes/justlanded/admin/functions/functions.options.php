<?php
/*
 * JustLanded Options Panel --- Default Settings and Configuration Items
 * DO NOT MODIFY this file, it is part of the JustLanded package and by changing this file
 * you will lose the ability to update the JustLanded theme.
 */

add_action('init', 'of_options');

if (!function_exists('of_options')) {
	function of_options()
	{
		// Load WordPress categories into array
		$of_categories = array();
		$of_categories_obj = get_categories('hide_empty=0');
		foreach ($of_categories_obj as $of_cat) {
			$of_categories[$of_cat->cat_ID] = $of_cat->cat_name;
		}
		$categories_tmp = array_unshift($of_categories, "Select a category:");

		// Load WordPress pages into array
		$of_pages = array();
		$of_pages_obj = get_pages('sort_column=post_title,post_parent,menu_order&post_status=publish,private');
		foreach ($of_pages_obj as $of_page) {
			$of_pages[$of_page->ID] = $of_page->post_title . ' (ID '.$of_page->ID.')';
		}

		// landing page layout/order
		$of_options_homepage_blocks = array
		(
			"disabled" => array(
				"placebo" => "placebo", //REQUIRED!
				"block_social_header"               => "Header Social Icons",
				"block_big_testimonial_slider"      => "Testimonial Slider",
				"block_page_content1"               => "Page Content 1",
				"block_page_content2"               => "Page Content 2",
				"block_page_content3"               => "Page Content 3",
				"block_page_content4"               => "Page Content 4",
				"block_page_content5"               => "Page Content 5",
				"block_widget1"                     => "Widget 1",
				"block_widget2"                     => "Widget 2",
				"block_cta_buttons"                 => "Action Buttons",
				"block_comments"                    => "Page Comments",
				"block_posts"                       => "Blog Posts",
				"block_widgets_row1"                => "Widgets Row 1",
				"block_widgets_row2"                => "Widgets Row 2",
			),
			"enabled" => array(
				"placebo" => "placebo", //REQUIRED!
				"block_features"                    => "List of Features",
				"block_gallery"                     => "Gallery",
				//"block_video_gallery"				=> "Video Gallery",
				"block_big_testimonial"             => "Feat. Testimonial",
				"block_small_testimonials"          => "Small Testimonials",
				"block_newsletter"                  => "Newsletter Signup Form",
				"block_pricing_table"               => "Pricing Table",
				"block_payment"                     => "Payment Options",
				"block_social_side"                 => "Sidebar Social Icons",
			),
		);

		$of_options_social_blocks = array
		(
			"disabled" => array(
				"placebo"               => "placebo", //REQUIRED!
				"stumbleupon"           => "StumbleUpon",
				"youtube"               => "YouTube",
				"vimeo"                 => "Vimeo",
				"linkedin"              => "LinkedIn",
				"pinterest"             => "Pinterest",
				"digg"                  => "Digg",
				"myspace"               => "MySpace",
				"picasa"                => "Picasa",
				"flickr"                => "flickr",
				"dribbble"              => "Dribbble",
				"blogger"               => "Blogger",
				"weibo"                 => "Weibo",
				"houzz"                 => "Houzz",
				"yelp"					=> "Yelp",
				"slideshare"			=> "Slideshare",
			),
			"enabled" => array(
				"placebo" => "placebo", //REQUIRED!
				"twitter"               => "Twitter",
				"facebook"              => "Facebook",
				"googleplus"            => "Google+",
				"rss"                   => "RSS Feed",
				"feedback"              => "Feedback Form",
			),
		);



		//Background Images Reader
		$bg_images_path = STYLESHEETPATH . '/images/bg/'; // change this to where you store your bg images
		$bg_images_url = get_bloginfo('template_url') . '/images/bg/'; // change this to where you store your bg images
		$bg_images = array();

		if (is_dir($bg_images_path)) {
			if ($bg_images_dir = opendir($bg_images_path)) {
				while (($bg_images_file = readdir($bg_images_dir)) !== false) {
					if (stristr($bg_images_file, ".png") !== false || stristr($bg_images_file, ".jpg") !== false) {
						$bg_images[] = $bg_images_url . $bg_images_file;
					}
				}
			}
		}


		/*-----------------------------------------------------------------------------------*/
		/* TO DO: Add options/functions that use these */
		/*-----------------------------------------------------------------------------------*/

		//More Options
		$uploads_arr = wp_upload_dir();
		$all_uploads_path = $uploads_arr['path'];
		$all_uploads = get_option('of_uploads');
		$other_entries = array("Select a number:", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19");
		$body_repeat = array("no-repeat", "repeat-x", "repeat-y", "repeat");
		$body_pos = array("top left", "top center", "top right", "center left", "center center", "center right", "bottom left", "bottom center", "bottom right");

		// Image Alignment radio box
		$of_options_thumb_align = array("alignleft" => "Left", "alignright" => "Right", "aligncenter" => "Center");

		// Image Links to Options
		$of_options_image_link_to = array("image" => "The Image", "post" => "The Post");

		/*-----------------------------------------------------------------------------------*/
		/* The Options Array */
		/*-----------------------------------------------------------------------------------*/

// Set the Options Array
		global $of_options;
		$of_options = array();

		do_action("justlanded_options_top");

		/* --------------- The Home Screen */
		$of_options[] = array("name" => "Home",
							  "type" => "heading");

		do_action("justlanded_options_before_home");

		$of_options[] = array("name" => "Customer Support",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Get Support",
							  "desc" => "",
							  "id" => "introduction_support",
							  "std" => "<h3 style=\"margin: 0 0 10px;\">Get Free Premium Support</h3>
					<p>With all our products we provide free premium support. Get in touch with support via email: support@shapingrain.com</p>
					<p>Please note that we <strong>do not</strong> provide customer support through the themeforest comments section to
					ensure that you are receiving the fastest and best possible assistance.</p>",
							  "icon" => true,
							  "type" => "info");


		if (OF_ENABLE_UPDATES == true) {
			$of_options[] = array("name" => "License and Updates",
								  "type" => "sectionheader");

			$shapingrain_license_key_justlanded = get_option(THEMENAME . '_global_license_key');

			if (trim($shapingrain_license_key_justlanded) == "" || strlen(trim($shapingrain_license_key_justlanded)) <= 65) {
				$of_options[] = array("name" => "License",
									  "desc" => "",
									  "id" => "introduction_license",
									  "std" => 'Registered customers can automatically retrieve updates from ShapingRain.com. Updates are available only
            to license holders, and a free account with ShapingRain.com is required for authentication. Registered customers receive
            a personalized license key to enable the automatic update feature. Without a license key, you will still receive update notifications,
            but the automatic update feature will be unavailable. Your license key <strong>must be obtained from ShapingRain.com</strong>, it is <strong>not identical</strong> with your Item Purchase Code.
            <p>
            <a href="https://ssl.shapingrain.com/register" target="_blank" style="margin-right:15px;">Register free account</a>
            <a href="https://ssl.shapingrain.com/login" target="_blank">Log into your account</a>
            </p><div class="clear"></div>',
									  "icon" => true,
									  "type" => "info");
			}
			else {
				$of_options[] = array("name" => "License",
									  "desc" => "",
									  "id" => "introduction_license",
									  "std" => 'Thank you for entering your license key. Please remember that this license key is valid for one installation on one domain only. As per your license you will have to obtain additional licenses for each end product. Please keep your license key at a safe place and do not redistribute it.
                    <p><a href="https://ssl.shapingrain.com/login" target="_blank">Log into your ShapingRain.com account</a></p>',
									  "icon" => true,
									  "type" => "info");
			}

			$of_options[] = array("name" => "ShapingRain.com License Key",
								  "desc" => "For registered customers, enables auto updates.",
								  "descposition" => "heading",
								  "id" => "global_license_key",
								  "global" => true,
								  "std" => "",
								  "type" => "text");

			$of_options[] = array("name" => "Disable Update Check?",
								  "desc" => "If this option is checked, JustLanded for WordPress will not contact ShapingRain.com servers to check for software updates.",
								  "id" => "global_disable_updates",
								  "std" => 0,
								  "type" => "checkbox");
		}

		do_action("justlanded_options_after_home");


		/* ----------- Backup Options ---- */
		$of_options[] = array("name" => "Backup and Import",
							  "type" => "subheading");

		do_action("justlanded_options_before_backup");

		$of_options[] = array("name" => "Backup and Restore Profile Options",
							  "id" => "of_backup",
							  "std" => "",
							  "type" => "backup",
							  "desc" => 'You can use this option to backup your current options, and then restore them at a later time.',
		);

		$of_options[] = array("name" => "Import Demo Data",
							  "id" => "of_demo",
							  "std" => "",
							  "type" => "demo",
							  "desc" => 'You can use this option to import demo data. This will overwrite your settings for all profiles. If you would like to replicate the demo site, you will also have to import the WordPress export XML file that ships with JustLanded for WordPress. Please refer to the user manual for instructions.',
		);

		$of_options[] = array("name" => "Transfer Your Settings",
							  "id" => "of_transfer",
							  "std" => "",
							  "type" => "transfer",
							  "desc" => 'You can transfer options and contents between different installations or profiles by copying and pasting the contents of this box from one installation or profile to the equivalent box of another and clicking "Import Options".',
		);

		$of_options[] = array("name" => "Master Export File Download",
							  "id" => "of_master_export",
							  "std" => "",
							  "type" => "master_export",
							  "desc" => 'This file is a complete backup of your JustLanded settings. Support may ask you to provide it for debugging purposes as well.',
		);

		do_action("justlanded_options_after_backup");

		/* --------------- Profiles ------- */
		$of_options[] = array("name" => "Profiles",
							  "type" => "subheading");

		do_action("justlanded_options_before_profiles");

		$of_options[] = array("name" => "Site Default Profile",
							  "desc" => "This profile will be used when no specific profile is selected, and for the blog.",
							  "descposition" => "heading",
							  "id" => "site_default_profile",
							  "std" => "1",
							  "global" => true,
							  "type" => "profile");

		$of_options[] = array("name" => "Profile Allocation Cheat Sheet",
							  "desc" => "Quickly identify which profile is used where.",
							  "descposition" => "heading",
							  "id" => "profile_allocation",
							  "type" => "profile_allocation");


		do_action("justlanded_options_after_profiles");


		/* --------------- Global Settings */
		$of_options[] = array("name" => "General Settings",
							  "type" => "heading", "redirect" => true);

		do_action("justlanded_options_before_general");

		$of_options[] = array("name" => "Header and Footer",
							  "type" => "subheading");

		$of_options[] = array("name" => "Site Header and Menu",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Show site menu on landing page?",
							  "desc" => "Check this if you would like to show the menu bar on the landing page. This setting has no effect on regular blog pages.",
							  "id" => "landing_page_menu",
							  "std" => 1,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Always hide menu?",
							  "desc" => "Check this if you would like to hide the site menu from all pages and the blog.",
							  "id" => "hide_menu",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Use 'sticky' menu?",
							  "desc" => "Check this if you would like the menu bar to always be visible.",
							  "id" => "sticky_menu",
							  "std" => 0,
							  "type" => "checkbox");

		$menus = get_terms( 'nav_menu', array( 'hide_empty' => true ) );
		$of_menus = array();
		if (is_array($menus) && count($menus) > 0) {
			foreach ($menus as $menu) {
				$of_menus[$menu->slug] = $menu->name;
			}
		}

		$of_options[] = array("name" => "Custom Main Menu",
							  "desc" => "Select a custom main menu to be used for this profile, overriding the default menu",
							  "id" => "menu_custom_main",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_menus);

		$of_options[] = array("name" => "Custom Footer Menu",
							  "desc" => "Select a custom footer menu to be used for this profile, overriding the default menu",
							  "id" => "menu_custom_footer",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_menus);

		$of_options[] = array("name" => "Custom Logo",
							  "desc" => "Upload an image to replace the default logo. (Default: 35 x 40px)",
							  "id" => "custom_logo",
							  "std" => "",
							  "type" => "upload");

		$of_options[] = array("name" => "Custom Link URL for logo and/or page title",
							  "desc" => "If this is left empty, the site's home URL will be used.",
							  "id" => "custom_home_url",
							  "std" => "",
							  "type" => "text");

		/*
		$of_options[] = array("name" => "Custom Site Title",
		                      "desc" => "If this is left empty, the site's default title will be used.",
		                      "id" => "custom_home_text",
		                      "std" => "",
		                      "type" => "text");

		$of_options[] = array("name" => "Custom Site Tagline",
		                      "desc" => "If this is left empty, the site's default tagline will be used.",
		                      "id" => "custom_home_tagline",
		                      "std" => "",
		                      "type" => "text");
		*/

		$of_options[] = array("name" => "Hide main site/page title and tagline?",
							  "desc" => "Check this if you would like to hide the main site/page title and the tag line. Use this if you'd like a logo-only header.",
							  "id" => "hide_page_title",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Hide page titles?",
							  "desc" => "Check this if you would like to hide page titles on regular WordPress pages.",
							  "id" => "hide_page_titles",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Custom Favicon",
							  "desc" => "Upload a 16px x 16px PNG/GIF image that will represent your website's favicon.",
							  "id" => "custom_favicon",
							  "std" => "",
							  "type" => "upload");

		$of_options[] = array("name" => "Header Contact Information",
							  "desc" => "Enter either phone number <em>or</em> email address.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Phone/Email label",
							  "desc" => "This is the label shown next to your email address or phone number. If neither a phone number nor an email address are present, this label will be hidden as well.",
							  "id" => "header_info_label",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Phone number",
							  "desc" => "Please enter a phone number to be displayed on top, above the banner.",
							  "id" => "header_phone",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Phone number (Vanity Number, Display Number)",
							  "desc" => "Use this field to enter a display-only vanity number, e.g. 555-BUSINESS.",
							  "id" => "header_phone_vanity",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Email address",
							  "desc" => "Please enter an email address to be displayed on top, above the banner.",
							  "id" => "header_email",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Custom Header",
							  "desc" => "Replace the header section (above the banner) with own content.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Free Form Content",
							  "id" => "header_freeform",
							  "desc" => "Can contain HTML, shortcodes allowed.",
							  "descposition" => "heading",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Use Page Content",
							  "desc" => "Retrieve and display contents from this page instead. When selected, the contents from the Free Form Content text box will be ignored.",
							  "id" => "header_freeform_page",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Custom Footer",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Copyright Notice",
							  "desc" => "This will be shown underneath the footer navigation.",
							  "id" => "custom_footer_copyright",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Blog and Archive",
							  "type" => "subheading");

		$of_options[] = array("name" => "Blog and Archive",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Blog/Index Page Title",
							  "desc" => "This is the headline used for your blog's index page. This is used for the blog archive page that displays your blog posts.",
							  "id" => "blog_title",
							  "std" => "Blog",
							  "type" => "text");

		$of_options[] = array("name" => "Always show full post?",
							  "desc" => "Check this box if you would like to display full posts instead of excerpts on post index pages.",
							  "id" => "show_full_posts",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Hide Post Meta Info?",
							  "desc" => "Check this box if you would like to hide meta information from index and single post views.",
							  "id" => "hide_post_meta",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Hide Post Author Info?",
							  "desc" => "Check this box if you would like to hide post author information from index and single post views.",
							  "id" => "hide_post_author",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Hide Post Meta Footer Info on Detail Pages?",
							  "desc" => "Check this box if you would like to hide meta information (categories, tags) from single post pages.",
							  "id" => "hide_post_meta_footer",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Always Hide Featured Images?",
							  "desc" => "Check this box if you would like to hide featured images from index and single post views.",
							  "id" => "hide_post_featured_image",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Hide Featured Images on Post Detail Pages?",
							  "desc" => "Check this box if you would like to hide featured images from single post views.",
							  "id" => "hide_post_featured_image_detail",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Hide Featured Images on Post Index Pages?",
							  "desc" => "Check this box if you would like to hide featured images from index pages (archives etc.)",
							  "id" => "hide_post_featured_image_index",
							  "std" => 0,
							  "type" => "checkbox");



		$of_options[] = array("name" => "Blog Posts on Landing Pages",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Custom Query Parameters",
							  "desc" => "This instructs the theme to apply the given parameters to the landing page's Blog Posts block. One parameter per line, format: name=value, i.e. posts_per_page=5; <a href=\"http://codex.wordpress.org/Class_Reference/WP_Query\" target=\"_blank\">see reference</a> for full list of parameters.",
							  "id" => "posts_query_string",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Content before Blog Posts",
							  "desc" => "This will be displayed within the Blog Posts block, but above the actual blog content. Can contain HTML and/or shortcodes.",
							  "id" => "posts_before",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Content before Blog Posts (use page content)",
							  "desc" => "Retrieve and display contents from this page instead.",
							  "id" => "posts_before_page",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Content after Blog Posts",
							  "desc" => "This will be displayed within the Blog Posts block, right underneath the actual blog content. Can contain HTML and/or shortcodes.",
							  "id" => "posts_after",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Content after Blog Posts (use page content)",
							  "desc" => "Retrieve and display contents from this page instead.",
							  "id" => "posts_after_page",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Built-in Features",
							  "type" => "subheading");

		$of_options[] = array("name" => "Lightbox",
							  "desc" => "Controls the behavior of the built-in lightbox feature.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Activate lightbox for all links pointing to images?",
							  "desc" => "If this is checked, all links pointing to images will open those linked images in the lightbox.",
							  "id" => "lightbox_active_for_links",
							  "std" => 1,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Activate lightbox for Gallery block?",
							  "desc" => "If this is checked, images linked from within the Gallery block will open in the lightbox.",
							  "id" => "lightbox_active_for_gallery",
							  "std" => 1,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Scroll Up",
							  "desc" => "Controls the behavior of the built-in 'scroll up' link.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Activate scroll up link?",
							  "desc" => "If this is checked, a 'scroll up' link will be displayed in the bottom right corner to enable users to quickly jump to the top.",
							  "id" => "scroll_up_active",
							  "std" => 1,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Custom Codes",
							  "desc" => "",
							  "id" => "introduction_custom_code",
							  "std" => "",
							  "type" => "subheading");

		$of_options[] = array("name" => "Profile ".ACTIVEPROFILE.": Custom Styles and Scripts",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Custom CSS",
							  "desc" => "This code will be loaded after the theme's default stylesheet.",
							  "id" => "custom_css",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Custom Header Scripts",
							  "desc" => "Paste tracking codes or custom blocks of JavaScript code here. This will be injected into the HTML header.",
							  "id" => "custom_header",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Custom Footer Scripts",
							  "desc" => "Paste tracking codes or custom blocks of JavaScript code here. This will be injected just before the end of the HTML body.",
							  "id" => "custom_footer",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Execute Shortcodes (No Output)",
							  "desc" => "This feature enables you to execute WordPress shortcodes when a page is loaded, without using their output anywhere.",
							  "id" => "custom_shortcodes",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Global: Custom Styles and Scripts",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Custom CSS",
							  "desc" => "This code will be loaded after the theme's default stylesheet, and before the profile's Custom CSS.",
							  "id" => "global_custom_css",
							  "std" => "",
							  "type" => "textarea",
							  "global" => true);

		$of_options[] = array("name" => "Custom Header Scripts",
							  "desc" => "Paste tracking codes or custom blocks of JavaScript code here. This will be injected into the HTML header, on every page.",
							  "id" => "global_custom_header",
							  "std" => "",
							  "type" => "textarea",
							  "global" => true);


		$of_options[] = array("name" => "Custom Footer Scripts",
							  "desc" => "Paste tracking codes or custom blocks of JavaScript code here. This will be injected just before the end of the HTML body, on every.",
							  "id" => "global_custom_footer",
							  "std" => "",
							  "type" => "textarea",
							  "global" => true);

		do_action("justlanded_options_after_custom_codes");

		$of_options[] = array("name" => "Advanced Settings",
							  "desc" => "",
							  "id" => "introduction_general_advanced",
							  "std" => "",
							  "type" => "subheading");

		$of_options[] = array("name" => "Content Filters",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Content Filter Behavior: Header, Footer, Custom Content fields etc.",
							  "desc" => "Select what filters you would like the theme to apply.",
							  "id" => "content_filter",
							  "std" => "alternative",
							  "type" => "select",
							  "storage" => "value",
							  "options" => array(
								  "thecontent"  => "Apply standard WordPress filters (default)",
								  "alternative" => "Apply only essential filters (for some plug-ins)",
								  "shortcodes"  => "No filters, parse shortcodes only (no formatting etc.)",
								  "none"        => "Do not apply filters"
							  ));

		$of_options[] = array("name" => "Content Filter Behavior: Page Content Blocks",
							  "desc" => "Select what filters you would like the theme to apply.",
							  "id" => "content_filter_page_content",
							  "std" => "thecontent",
							  "type" => "select",
							  "storage" => "value",
							  "options" => array(
								  "thecontent"  => "Apply standard WordPress filters (default)",
								  "alternative" => "Apply only essential filters (for some plug-ins)",
								  "shortcodes"  => "No filters, parse shortcodes only (no formatting etc.)",
								  "none"        => "Do not apply filters"
							  ));

		$of_options[] = array("name" => "Responsiveness",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Disable responsiveness? (Experimental)",
							  "desc" => "If this is checked, an alternative stylesheet will be used and the theme will no longer be responsive.",
							  "id" => "disable_responsiveness",
							  "std" => 0,
							  "type" => "checkbox");


		do_action("justlanded_options_after_general");


		/* --------------- Typography and Colors */
		$of_options[] = array("name" => "Fonts and Colors",
							  "redirect" => true,
							  "type" => "heading");

		$of_options[] = array("name" => "Presets",
							  "type" => "subheading");

		$of_options[] = array("name" => "Color Presets",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Presets Intro",
							  "desc" => "",
							  "id" => "introduction_typography",
							  "std" => "Presets contain font and color selections. Importing a preset is the fastest way of customizing JustLanded to your liking. You can use any preset as-is, or as a starting point for your own creations. Keep in mind though that whenever you import a preset, customized font and color selections in the current profile will be overwritten.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_before_presets");

		$of_options[] = array("name" => "Import a preset",
							  "id" => "preset_colors",
							  "folder" => "colors",
							  "desc" => "Click on a preset to import its settings.",
							  "descposition" => "heading",
							  "type" => "preset");

		do_action("justlanded_options_after_presets");

		$of_options[] = array("name" => "Colors",
							  "id" => "colors_section",
							  "type" => "subheading");

		$of_options[] = array("name" => "General Colors",
							  "type" => "sectionheader");

		do_action("justlanded_options_before_colors_general");

		$of_options[] = array("name" => "Body Background Color",
							  "desc" => "Pick a background color for the theme.",
							  "id" => "body_background",
							  "std" => "#ffffff",
							  "type" => "color");

		$of_options[] = array("name" => "Menu Background",
							  "desc" => "This is the background color for the active menu item.",
							  "id" => "menu_background_active",
							  "std" => "#dd4531",
							  "type" => "color");

		$of_options[] = array("name" => "Link Color",
							  "desc" => "This is the color used for all links.",
							  "id" => "link_color",
							  "std" => "#d12727",
							  "type" => "color");

		$of_options[] = array("name" => "Link Hover Color",
							  "desc" => "This is the color used for the hover state of all links.",
							  "id" => "link_color_hover",
							  "std" => "#c03220",
							  "type" => "color");

		$of_options[] = array("name" => "Blog Buttons Color",
							  "desc" => "This is used for submit buttons on blog pages.",
							  "id" => "blog_button_background",
							  "std" => "#d12727",
							  "type" => "color");

		do_action("justlanded_options_after_colors_general");

		$of_options[] = array("name" => "Landing Page Banner and Page Banner",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Banner Background Options",
							  "std" => "For banner images, the gradient's starting color will be used as a fallback background color. It will also be used if a 'solid color' background type is selected.",
							  "id" => "banner_background_intro",
							  "type" => "info");


		$background_types = array(
			"solid" => "Solid Color",
			"gradient" => "Color Gradient",
			"image" => "Background Image"
		);
		do_action("justlanded_options_after_background_types_array");

		$of_options[] = array("name" => "Banner Background Type",
							  "desc" => "Select what background type you would like the theme to use.",
							  "id" => "banner_background_type",
							  "std" => "gradient",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $background_types);


		$of_options[] = array("name" => "Banner Gradient",
							  "desc" => "This is the gradient used for the banner.",
							  "id" => "banner_gradient",
							  "std" => array('start' => '#cf5833', 'end' => '#ab2210'),
							  "type" => "gradient");

		$of_options[] = array("name" => "Banner Background Image",
							  "desc" => "Upload an image to display instead of the gradient.",
							  "id" => "banner_background_image",
							  "std" => "",
							  "type" => "upload");

		$of_options[] = array("name" => "Banner Background Image Style",
							  "desc" => "Select how you would like to display the background image.",
							  "id" => "banner_background_image_style",
							  "std" => "cover",
							  "type" => "select",
							  "storage" => "value",
							  "options" => array(
								  "cover" => "full width background image, covering the banner",
								  "tile"  => "seamless, tiled background image"
							  ));

		$of_options[] = array("name" => "Landing Page Colors",
							  "type" => "sectionheader");

		do_action("justlanded_options_before_colors_landing");

		$of_options[] = array("name" => "Newsletter Background",
							  "desc" => "This is the background color for the newsletter/mailing list section.",
							  "id" => "newsletter_background",
							  "std" => "#dd4531",
							  "type" => "color");

		$of_options[] = array("name" => "Pricing Table Highlight",
							  "desc" => "This is the background color for the highlighted pricing table items.",
							  "id" => "pricing_highlight_background",
							  "std" => "#dd4632",
							  "type" => "color");

		$of_options[] = array("name" => "Pricing Table Highlight (Hover State)",
							  "desc" => "This is the background color for the highlighted pricing table items, in its hover state.",
							  "id" => "pricing_highlight_hover_background",
							  "std" => "#c03220",
							  "type" => "color");

		do_action("justlanded_options_after_colors_landing");

		/*
		 * Buttons
		 */

		$of_options[] = array("name" => "Buttons",
							  "desc" => "These settings enable customization of all action buttons.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Overwrite default button style with custom style settings?",
							  "desc" => "If this is checked, your custom style will be applied to all buttons. If deactivated, custom settings will be saved but not used by the theme.",
							  "id" => "buttons_base_color_active",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Button Base Color",
							  "desc" => "This value will be used to calculate all button colors. Only the hue value will be used, brightness and intensity will be calculated based on the default colors.",
							  "id" => "buttons_base_color",
							  "std" => "#c03220",
							  "type" => "color");

		$of_options[] = array("name" => "Button Base Text Color",
							  "desc" => "This defines the text color if no advanced text and shadow colors are defined.",
							  "id" => "buttons_base_text_color",
							  "std" => "#ffffff",
							  "type" => "color");

		$of_options[] = array("name" => "Button Base Text Shadow Color",
							  "desc" => "This defines the text shadow color if no advanced text and shadow colors are defined.",
							  "id" => "buttons_base_text_shadow_color",
							  "std" => "#333333",
							  "type" => "color");


		/*
		 * Advanced Buttons
		 */

		$of_options[] = array("name" => "Advanced Button Colors",
							  "desc" => "Check this to manually set colors for each button element. If unchecked, settings in this section will be temporarily deactivated.",
							  "id" => "buttons_advanced_colors",
							  "std" => 0,
							  "folds" => 1,
							  "type" => "checkbox");

		$of_options[] = array("name" => "First Button Gradient",
							  "desc" => "Start and end points of gradient.",
							  "id" => "buttons_gradient_1",
							  "std" => array("start" => "#fea817", "end" => "c54200"),
							  "fold" => "buttons_advanced_colors",
							  "type" => "gradient");

		$of_options[] = array("name" => "First Button Gradient (hover state)",
							  "desc" => "Start and end points of gradient.",
							  "id" => "buttons_gradient_1_hover",
							  "std" => array("start" => "#c54200", "end" => "fea817"),
							  "fold" => "buttons_advanced_colors",
							  "type" => "gradient");


		$of_options[] = array("name" => "Second Button Gradient",
							  "desc" => "Start and end points of gradient.",
							  "id" => "buttons_gradient_2",
							  "std" => array("start" => "#fec32d", "end" => "d86600"),
							  "fold" => "buttons_advanced_colors",
							  "type" => "gradient");

		$of_options[] = array("name" => "Second Button Gradient (hover state)",
							  "desc" => "Start and end points of gradient.",
							  "id" => "buttons_gradient_2_hover",
							  "std" => array("start" => "#d86600", "end" => "fec32d"),
							  "fold" => "buttons_advanced_colors",
							  "type" => "gradient");

		$of_options[] = array("name" => "Text Color",
							  "desc" => "This is the button's main text color",
							  "id" => "buttons_text",
							  "std" => "#ffffff",
							  "fold" => "buttons_advanced_colors",
							  "type" => "color");

		$of_options[] = array("name" => "Text Shadow Color",
							  "desc" => "This is the button's text shadow color, usually a darker version of the buttons's background color.",
							  "id" => "buttons_text_shadow",
							  "std" => "#af5305",
							  "fold" => "buttons_advanced_colors",
							  "type" => "color");

		$of_options[] = array("name" => "Split-Button Center Section Gradient",
							  "desc" => "Start and end points of gradient.",
							  "id" => "buttons_gradient_middle",
							  "std" => array("start" => "#d86600", "end" => "e88c13"),
							  "fold" => "buttons_advanced_colors",
							  "type" => "gradient");

		$of_options[] = array("name" => "Split-Button Center Section Text Color",
							  "desc" => "This is used for the text between the buttons, the 'or' bit.",
							  "id" => "buttons_text_secondary",
							  "std" => "#833100",
							  "fold" => "buttons_advanced_colors",
							  "type" => "color");

		$of_options[] = array("name" => "Split-Button Center Section Text Shadow Color",
							  "desc" => "This is the button's text shadow color, usually a darker version of the buttons's background color.",
							  "id" => "buttons_text_secondary_shadow",
							  "std" => "#f0c08b",
							  "fold" => "buttons_advanced_colors",
							  "type" => "color");

		$of_options[] = array("name" => "Newsletter Landing Page Block Submit Button Border",
							  "desc" => "This is the button's border.",
							  "id" => "buttons_newsletter_border1",
							  "std" => "#eb9f29",
							  "fold" => "buttons_advanced_colors",
							  "type" => "color");

		$of_options[] = array("name" => "Newsletter Widget Submit Button Border",
							  "desc" => "This is the button's border.",
							  "id" => "buttons_newsletter_border2",
							  "std" => "#eb9f29",
							  "fold" => "buttons_advanced_colors",
							  "type" => "color");

		do_action("justlanded_options_after_colors_buttons");

		/************* General Fonts ********************************************************/
		$of_options[] = array("name" => "General Fonts",
							  "id" => "fonts_section",
							  "type" => "subheading");

		$of_options[] = array("name" => "General Fonts",
							  "desc" => "Fonts used throughout the site.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "General Fonts",
		                      "id" => "introduction_general_fonts",
		                      "std" => "These fonts are used throughout the site. If 'em' is selected for the body font size, all other font sizes using 'em' will be based on the body font's size.
		                      <br />See <a href=\"https://www.google.com/fonts\" target=\"_blank\">Google Fonts</a> for font previews in all supported charsets and styles.",
		                      "icon" => true,
		                      "type" => "info");

		do_action("justlanded_options_before_fonts_general");

		$of_options[] = array("name" => "Body Font",
							  "desc" => "This font is used for general content.",
							  "descposition" => "heading",
							  "id" => "font_body",
							  "std" => array('size' => '0.8', 'unit' => 'em', 'face' => 'Verdana*', 'style' => 'regular', 'color' => '#333333'),
							  "type" => "typography");

		$of_options[] = array("name" => "Logo Font",
							  "desc" => "This font is used only for the logo.",
							  "descposition" => "heading",
							  "id" => "font_logo",
							  "std" => array('size' => '1.5', 'unit' => 'em', 'face' => 'Lato', 'style' => '900', 'color' => '#dd4531', 'subset' => array('latin' => 1)),
							  "type" => "typography");

		$of_options[] = array("name" => "Tagline Font",
							  "desc" => "This font is used only for the tagline, next to the logo.",
							  "descposition" => "heading",
							  "id" => "font_logo_tagline",
							  "std" => array('size' => '1.25', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#999999', 'subset' => array('latin' => 1)),
							  "type" => "typography");

		$of_options[] = array("name" => "Quotes",
							  "desc" => "This font is used for quotes on your blog and all Page Content blocks on the landing page.",
							  "descposition" => "heading",
							  "id" => "font_quotes",
							  "std" => array('size' => '1', 'unit' => 'em', 'face' => 'Verdana*', 'style' => 'italic', 'color' => '#000000'),
							  "type" => "typography");

		do_action("justlanded_options_after_fonts_general");

		/******************* Landing Page Fonts ***********************************************/
		$of_options[] = array("name" => "Landing Page Fonts",
							  "id" => "fonts_section_blog",
							  "type" => "subheading");

		$of_options[] = array("name" => "Landing Page Fonts",
							  "desc" => "These fonts are only used on the landing page.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Landing Page Fonts",
		                      "id" => "introduction_landing_fonts",
		                      "std" => "These fonts are used on landing pages. When a landing page block is used on a regular content page, it may be rendered differently in accordance with global styles.
		                      <br />See <a href=\"https://www.google.com/fonts\" target=\"_blank\">Google Fonts</a> for font previews in all supported charsets and styles.",
		                      "icon" => true,
		                      "type" => "info");

		do_action("justlanded_options_before_fonts_landing");

		$of_options[] = array("name" => "Banner Heading",
							  "desc" => "This is the main heading used for the banner.",
							  "descposition" => "heading",
							  "id" => "font_heading_banner",
							  "std" => array('size' => '3', 'unit' => 'em', 'face' => 'Merriweather', 'style' => '700', 'color' => '#ffffff'),
							  "type" => "typography");

		$of_options[] = array("name" => "Banner Text Font Color",
							  "desc" => "This is color is used for all text in the banner's text content section.",
							  "descposition" => "heading",
							  "id" => "font_text_banner",
							  "std" => '#ffffff',
							  "type" => "color");

		$of_options[] = array("name" => "Section Title",
							  "desc" => "This is the font used for a section's main title.",
							  "descposition" => "heading",
							  "id" => "font_section_title",
							  "std" => array('size' => '2.2', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#dd4632'),
							  "type" => "typography");

		$of_options[] = array("name" => "Section Subtitle",
							  "desc" => "This is the font used for a section's subtitle.",
							  "descposition" => "heading",
							  "id" => "font_section_subtitle",
							  "std" => array('size' => '1.45', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#999999'),
							  "type" => "typography");

		$of_options[] = array("name" => "Newsletter Sign-Up Form Title",
							  "desc" => "This is the newsletter form's caption.",
							  "descposition" => "heading",
							  "id" => "font_newsletter_title",
							  "std" => array('size' => '1.8', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#ffffff'),
							  "type" => "typography");

		$of_options[] = array("name" => "Big Testimonial",
							  "desc" => "This is the font used for the big testimonial.",
							  "descposition" => "heading",
							  "id" => "font_big_testimonial",
							  "std" => array('size' => '1.4', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#000000'),
							  "type" => "typography");

		/* pricing table */
		$of_options[] = array("name" => "Pricing Table Package Title",
							  "desc" => "This is the font used for a package's title.",
							  "descposition" => "heading",
							  "id" => "font_pricing_title",
							  "std" => array('size' => '2', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#ffffff'),
							  "type" => "typography");

		$of_options[] = array("name" => "Pricing Table Package Subtitle",
							  "desc" => "This is the font used for a package's subtitle.",
							  "descposition" => "heading",
							  "id" => "font_pricing_sub_title",
							  "std" => array('size' => '1.05', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#cccccc'),
							  "type" => "typography");

		$of_options[] = array("name" => "Pricing Table Package Subtitle (Featured)",
							  "desc" => "This is color used for a featured item's subtitle.",
							  "descposition" => "heading",
							  "id" => "font_pricing_sub_title_featured",
							  "std" => '#f7c9c9',
							  "type" => "color");

		$of_options[] = array("name" => "Pricing Table Price",
							  "desc" => "This is font used for the price and the currency symbol.",
							  "descposition" => "heading",
							  "id" => "font_pricing_price",
							  "std" => array('size' => '3.81', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#ffffff'),
							  "type" => "typography");

		$of_options[] = array("name" => "Pricing Table Currency Symbol",
							  "desc" => "This is currency symbol's size.",
							  "descposition" => "heading",
							  "id" => "font_pricing_currency",
							  "std" => array('size' => '0.5', 'unit' => 'em'),
							  "type" => "typography");

		do_action("justlanded_options_after_fonts_landing");

		/******************* Blog Fonts *******************************************************/
		$of_options[] = array("name" => "Blog Fonts",
							  "id" => "fonts_section_blog",
							  "type" => "subheading");

		$of_options[] = array("name" => "Blog Fonts",
							  "desc" => "These fonts are only used for blog posts and on WordPress pages.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Blog Fonts",
		                      "id" => "introduction_blog_fonts",
		                      "std" => "These fonts are used on blog and archive pages. General styles such as H1-H6 are also applied to regular content pages.
		                      <br />See <a href=\"https://www.google.com/fonts\" target=\"_blank\">Google Fonts</a> for font previews in all supported charsets and styles.",
		                      "icon" => true,
		                      "type" => "info");


		do_action("justlanded_options_before_fonts_blog");

		$of_options[] = array("name" => "Heading H1",
							  "desc" => "This is the H1 font.",
							  "descposition" => "heading",
							  "id" => "font_heading_h1",
							  "std" => array('size' => '2.35', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#000000'),
							  "type" => "typography");

		$of_options[] = array("name" => "Heading H2",
							  "desc" => "This is the H2 font.",
							  "descposition" => "heading",
							  "id" => "font_heading_h2",
							  "std" => array('size' => '2', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#000000'),
							  "type" => "typography");

		$of_options[] = array("name" => "Heading H3",
							  "desc" => "This is the H3 font.",
							  "descposition" => "heading",
							  "id" => "font_heading_h3",
							  "std" => array('size' => '1.6', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#000000'),
							  "type" => "typography");

		$of_options[] = array("name" => "Heading H4",
							  "desc" => "This is the H4 font.",
							  "descposition" => "heading",
							  "id" => "font_heading_h4",
							  "std" => array('size' => '1.2', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#000000'),
							  "type" => "typography");

		$of_options[] = array("name" => "Heading H5",
							  "desc" => "This is the H5 font.",
							  "descposition" => "heading",
							  "id" => "font_heading_h5",
							  "std" => array('size' => '1.125', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#000000'),
							  "type" => "typography");

		$of_options[] = array("name" => "Heading H6",
							  "desc" => "This is the H6 font.",
							  "descposition" => "heading",
							  "id" => "font_heading_h6",
							  "std" => array('size' => '1', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#000000'),
							  "type" => "typography");

		$of_options[] = array("name" => "Single Page Title",
							  "desc" => "This font is used for page titles.",
							  "descposition" => "heading",
							  "id" => "font_page_title",
							  "std" => array('size' => '1.8', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#000000'),
							  "type" => "typography");

		$of_options[] = array("name" => "Widget Title",
							  "desc" => "This font is used for widget titles throughout your blog.",
							  "descposition" => "heading",
							  "id" => "font_widget_title",
							  "std" => array('size' => '1.25', 'unit' => 'em', 'face' => 'Merriweather', 'style' => 'regular', 'color' => '#333333'),
							  "type" => "typography");

		$of_options[] = array("name" => "Post Meta",
							  "desc" => "This font is used for meta information, such as the post date and time and the list of categories.",
							  "descposition" => "heading",
							  "id" => "font_post_meta",
							  "std" => array('size' => '12', 'unit' => 'px', 'face' => 'Verdana*', 'style' => 'regular', 'color' => '#999999'),
							  "type" => "typography");

		do_action("justlanded_options_after_fonts_blog");


		/************************** Advanced Options, Export *********************************/
		$of_options[] = array("name" => "Advanced Options",
							  "id" => "advanced_section",
							  "type" => "subheading");

		do_action("justlanded_options_before_advanced");

		$of_options[] = array("name" => "Fonts and Colors Export",
							  "desc" => "Use this option to import and export font and color settings only.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Import or Export Fonts and Colors",
							  "desc" => "Copy and paste this into a profile's import dialog to import fonts and colors only.",
							  "id" => "extract_typography",
							  "fields" => "body_background,link_color,link_color_hover,menu_background_active,banner_gradient,banner_background_type,banner_background_image,banner_background_image_style,newsletter_background,pricing_highlight_background,pricing_highlight_hover_background,blog_button_background,font_body,font_logo,font_logo_tagline,font_quotes,font_heading_h1,font_heading_h2,font_heading_h3,font_heading_h4,font_heading_h5,font_heading_h6,font_page_title,font_widget_title,font_post_meta,font_heading_banner,font_section_title,font_section_subtitle,font_newsletter_title,font_big_testimonial,font_pricing_title,font_pricing_sub_title,font_pricing_sub_title_featured,font_pricing_price,font_pricing_currency,buttons_base_color_active,buttons_base_color,buttons_advanced_colors,buttons_gradient_1,buttons_gradient_1_hover,buttons_gradient_2,buttons_gradient_2_hover,buttons_text,buttons_text_shadow,buttons_gradient_middle,buttons_text_secondary,buttons_text_secondary_shadow,buttons_newsletter_border1,buttons_newsletter_border2,buttons_base_text_color,buttons_base_text_shadow_color",
							  "type" => "extractoptions");

		do_action("justlanded_options_after_advanced");

		/* --------------- The Landing Page */
		$of_options[] = array("name" => "Landing Layout",
							  "type" => "heading");

		$of_options[] = array("name" => "Landing Page Layout",
							  "desc" => "",
							  "id" => "introduction_landing",
							  "std" => "<h3 style=\"margin: 0 0 10px;\">The Landing Page</h3>
					This panel contains the layout settings for the integrated landing page. This module allows you
					create a landing page using building blocks of pre-defined elements. Some of these
					blocks can also be added to any WordPress page through shortcodes.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_before_layout");

		$of_options[] = array("name" => "Landing Page Layout",
							  "desc" => "Organize how you want the layout to appear on the homepage",
							  "id" => "landingpage_blocks",
							  "std" => $of_options_homepage_blocks,
							  "type" => "sorter");

		do_action("justlanded_options_after_layout");

		/* --------------- The Banner */
		$of_options[] = array("name" => "Banner",
							  "type" => "heading", "redirect" => true);

		$of_options[] = array("name" => "General",
							  "type" => "subheading");

		do_action("justlanded_options_before_banner");

		$of_options[] = array("name" => "Banner Layout and General Settings",
							  "type" => "sectionheader");

		$url = ADMIN_DIR . 'assets/images/';
		$banner_select_layout = array(
			'right'  => $url . 'media_right.png',
			'left' => $url . 'media_left.png',
			'full' => $url . 'media_full.png');

		do_action("justlanded_options_after_banner_layout_array");

		$of_options[] = array("name" => "Select Banner Layout",
							  "desc" => "Select where you would like to place media and text content. The full size option allows for a freestyle banner defined through the Full Width Banner tab.",
							  "id" => "banner_position",
							  "std" => "right",
							  "type" => "images",
							  "options" => $banner_select_layout
		);

		$of_options[] = array("name" => "Hide banner?",
							  "desc" => "If you check this box, the entire banner section will not be displayed.",
							  "id" => "hide_banner",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Text Content",
							  "type" => "subheading");

		$of_options[] = array("name" => "Text Content",
							  "type" => "sectionheader",
							  "desc" => "Use the basic template for your banner's text section",
							  "descposition" => "heading");

		$of_options[] = array("name" => "Banner Title",
							  "desc" => "This is the main headline.",
							  "id" => "banner_title",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Banner Text",
							  "desc" => "This is the main text above the bullet points. Use this paragraph to introduce your product. This is the first line below the headline, so every word counts.",
							  "id" => "banner_text",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Banner Bullet Points",
							  "desc" => "Enter bullet point items here, one per line. Don't make things too complicated: list the most important benefits!",
							  "id" => "banner_bullets",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Custom Text Content",
							  "type" => "sectionheader",
							  "desc" => "Use your own, custom content for the banner's text section.",
							  "descposition" => "heading");

		$of_options[] = array("name" => "Custom Content",
							  "desc" => "Enter your custom mark-up here.",
							  "id" => "banner_custom_text",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Custom Content from Page",
							  "desc" => "Select a page if you would like to display the contents of a WordPress page instead.",
							  "id" => "banner_custom_text_page",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Display Options",
							  "type" => "sectionheader",
							  "desc" => "Define how text content should be displayed.",
							  "descposition" => "heading");

		$of_options[] = array("name" => "Automatically resize banner to accommodate text exceeding fixed height?",
							  "desc" => "Check this option if you would like the banner to adapt to the text section's content. This option will fit all text into the banner, but images overlapping the banner are not supported in this mode.",
							  "id" => "banner_text_adapt",
							  "std" => 0,
							  "type" => "checkbox");



		$of_options[] = array("name" => "Call To Action",
							  "type" => "subheading");

		$of_options[] = array("name" => "Call To Action Options",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Hide action buttons in banner?",
							  "desc" => "If you check this box, the banner's action buttons will be hidden.",
							  "id" => "banner_action_buttons",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Show newsletter sign-up form when action buttons are hidden?",
							  "desc" => "If you check this box, a newsletter sign-up form using the 'Newsletter' tab settings will replace the action buttons when those are hidden.",
							  "id" => "banner_action_form",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Media Content",
							  "type" => "subheading");

		$of_options[] = array("name" => "Media Content",
							  "type" => "sectionheader");

		$banner_media_options = array('Single Image', 'Slider', 'Video', 'Free Form Content');
		do_action("justlanded_options_after_banner_media_array");

		$of_options[] = array("name" => "Select Media Content",
							  "desc" => "What element would you like to display as part of the banner's media section?",
							  "id" => "banner_layout_select",
							  "std" => "Slider",
							  "type" => "select",
							  "options" => $banner_media_options);

		$of_options[] = array("name" => "Single Image",
							  "desc" => "A simple image.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Single Image",
							  "desc" => "If you prefer a static image instead of the slider, upload the image here.",
							  "id" => "banner_image",
							  "std" => "",
							  "type" => "media");

		$of_options[] = array("name" => "Single Image Link URL",
							  "desc" => "Enter a URL here if you want the image to link to a URL.",
							  "id" => "banner_image_link",
							  "std" => "",
							  "type" => "text");

		$link_target_options = array('Current Window', 'New Window');
		$of_options[] = array("name" => "Single Image Link Target",
							  "desc" => "Choose whether this link should open in a new window.",
							  "id" => "banner_image_link_target",
							  "std" => "_self",
							  "storage" => "value",
							  "options" => array(
								  "_blank" => "Open in new window",
								  "_self"  => "Open in same window",
							  ),
							  "type" => "select");

		$of_options[] = array("name" => "Image Slider",
							  "desc" => "A simple slideshow.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Slides",
							  "desc" => "Add, remove or sort slides (images) you would like the slider to show.",
							  "id" => "banner_slider",
							  "std" => "",
							  "has_image" => true,
							  "has_description" => false,
							  "itemtype" => "Slide",
							  "type" => "slider");

		$banner_media_slider_animations = array('fade', 'slide');
		do_action("justlanded_options_after_banner_slider_animation_array");
		$of_options[] = array("name" => "Select Slider Animation Type",
							  "desc" => "Select the slider's animation type.",
							  "id" => "banner_slider_animation_type",
							  "std" => "fade",
							  "type" => "select",
							  "options" => $banner_media_slider_animations);

		$of_options[] = array("name" => "Slider Slideshow Speed",
							  "desc" => "Set the speed of the slideshow cycling, in milliseconds.",
							  "id" => "banner_slider_slideshow_speed",
							  "std" => "7000",
							  "type" => "text");

		$of_options[] = array("name" => "Slider Animation Speed",
							  "desc" => "Set the speed of animations, in milliseconds.",
							  "id" => "banner_slider_animation_speed",
							  "std" => "600",
							  "type" => "text");

		$of_options[] = array("name" => "Show slides in random order?",
							  "desc" => "Select whether you would like the slider to show slides in a random order.",
							  "id" => "banner_slider_random",
							  "std" => "false",
							  "type" => "select",
							  "options" => array("true", "false"));

		$of_options[] = array("name" => "Show slider arrow navigation?",
							  "desc" => "Select whether you would to display arrow controls to navigate through slides.",
							  "id" => "banner_slider_controls",
							  "std" => "true",
							  "type" => "select",
							  "options" => array("true", "false"));


		$of_options[] = array("name" => "Video and Free Form Media Content",
							  "desc" => "Video clip and other embedded content.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Video",
			//"desc" => "Paste your video integration code here. It may be called 'Embed code' and is usually available through a 'Share' or 'Embed' button.<br /><a href=\"http://support.google.com/youtube/bin/answer.py?hl=en&answer=171780\" target=\"_blank\">How to embed a YouTube video</a><br /><a href=\"http://vimeo.com/help/faq/embedding\" target=\"_blank\">How to embed a Vimeo video</a>",
							  "desc" => "Paste your video link here. You only need the plain link, WordPress will do the rest.",
							  "id" => "banner_video",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Free Form Media Content",
							  "desc" => "Paste any HTML here. This field will be parsed for shortcodes as well. You may also use this field for more exotic video providers not supported by oEmbed. ",
							  "id" => "banner_free_media",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Full Width Banner",
							  "type" => "subheading");

		$of_options[] = array("name" => "Full Width Banner Content",
							  "type" => "sectionheader",
							  "desc" => "Full width banner, replaces media and text sections.",
							  "descposition" => "heading",
		);

		do_action("justlanded_options_after_full_width_banner_section_header");

		$of_options[] = array("name" => "Free Form Content",
							  "id" => "banner_full",
							  "desc" => "Can contain HTML, shortcodes allowed.",
							  "descposition" => "heading",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Use Page Content",
							  "desc" => "Retrieve and display contents from this page instead. When selected, the contents from the Free Form Content text box will be ignored.",
							  "id" => "banner_full_page",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Center action buttons when Full Width Banner is used?",
							  "desc" => "If this option is checked, the action buttons will be centered.",
							  "id" => "banner_full_center_buttons",
							  "std" => 0,
							  "type" => "checkbox");

		/* experimental page banner */

		$of_options[] = array("name" => "Page Banner",
							  "type" => "subheading");


		$of_options[] = array("name" => "Page Banner",
							  "type" => "sectionheader",
							  "desc" => "Full width banner, only displayed on pages using a special page template.",
							  "descposition" => "heading",
		);

		$of_options[] = array("name" => "Action Buttons",
							  "desc" => "",
							  "id" => "introduction_buttons",
							  "std" => "In some circumstances you may want to use a banner on pages that do not use the landing page template. This
            is a plain banner that accepts HTML and supports some of the theme's shortcodes. One unique shortcode, <code>[justlanded_pagetitle]</code>, has been designed for this banner and always outputs a page's title.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_after_page_banner_section_header");

		$of_options[] = array("name" => "Page Banner Content",
							  "id" => "page_banner",
							  "desc" => "Can contain HTML, shortcodes allowed.",
							  "descposition" => "heading",
							  "std" => "<h1>[justlanded_pagetitle]</h1>",
							  "type" => "textarea");

		$of_options[] = array("name" => "Use Page Content",
							  "desc" => "Retrieve and display contents from this page instead. When selected, the contents from the text box above will be ignored.",
							  "id" => "page_banner_page",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		do_action("justlanded_options_after_page_content_textarea");

		$of_options[] = array("name" => "Always show page banner? (Experimental)",
							  "desc" => "If this option is checked, the page banner element will be displayed on all pages, except for landing pages.",
							  "id" => "show_page_banner",
							  "std" => 0,
							  "type" => "checkbox");


		do_action("justlanded_options_after_banner");

		/* --------------- Action Butttons */
		$of_options[] = array("name" => "Action Buttons",
							  "type" => "heading");

		$of_options[] = array("name" => "Action Buttons",
							  "desc" => "",
							  "id" => "introduction_buttons",
							  "std" => "<h3 style=\"margin: 0 0 10px;\">Action Buttons</h3>
					This set of buttons (or the first button only, depending on your selection) are used twice, as part of the header/banner, and optionally within a separate block
					intended to lead your prospects to a buying action. The latter is recommended for use at the bottom of the landing page.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_before_cta");

		$banner_button_options = array('Two Buttons/Split-Button', 'Single Large Button');
		do_action("justlanded_options_after_banner_button_array");

		$of_options[] = array("name" => "Button Style",
							  "desc" => "Choose between a split-button and a single large button.",
							  "id" => "action_buttons_layout",
							  "std" => "Two Buttons/Split-Button",
							  "type" => "select",
							  "options" => $banner_button_options);

		$of_options[] = array("name" => "Rounded Corners",
							  "desc" => "Select a radius and decide how round you would like the buttons' corners to be.",
							  "id" => "action_buttons_radius",
							  "std" => "30",
							  "type" => "select",
							  "storage" => "value",
							  "options" => array(
								  "30"  => "30px (default)",
								  "15"  => "15px",
								  "5"   => "5px (lightly rounded)",
								  "0"   => "0px (square)"
							  ));


		do_action("justlanded_options_after_banner_button_style");


		$of_options[] = array("name" => "First Button",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "First Button Link Text",
							  "desc" => "If you choose to display just one button, this will be the text shown on that button. When using the split-button, this will be the text shown on the first button.",
							  "id" => "action_button1_text",
							  "std" => "Buy for $10",
							  "type" => "text");

		$of_options[] = array("name" => "First Button Link URL",
							  "desc" => "If you choose to display just one button, this will be the link that this button points to. When using the split-button, this will be the link target for the first button.",
							  "id" => "action_button1_link",
							  "std" => "#",
							  "type" => "text");

		$link_target_options = array('Current Window', 'New Window');
		$of_options[] = array("name" => "First Button Link Target",
							  "desc" => "Choose whether this link should open in a new window.",
							  "id" => "action_button1_target",
							  "std" => "Current Window",
							  "options" => $link_target_options,
							  "type" => "select");

		$of_options[] = array("name" => "First Button OnClick Attribute",
							  "desc" => "This option adds an OnClick attribute to the link. This option may be useful for some tracking services.",
							  "id" => "action_button1_onclick",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "First Button Modal Window",
							  "desc" => "Check this to open a modal window with custom content if this button is clicked.",
							  "id" => "action_button1_modal",
							  "std" => 0,
							  "folds" => 1,
							  "type" => "checkbox");

		$of_options[] = array("name" => "First Button Modal Window Content",
							  "desc" => "Enter your custom mark-up here.",
							  "id" => "action_button1_modal_content",
							  "fold" => "action_button1_modal", /* the checkbox hook */
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "First Button Modal Window Page Content",
							  "desc" => "Select a page if you would like to use a WordPress page for this modal window.",
							  "id" => "action_button1_modal_page",
							  "fold" => "action_button1_modal", /* the checkbox hook */
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "First Button Modal Window Size",
							  "desc" => "Select a size for the modal window.",
							  "id" => "action_button1_modal_size",
							  "fold" => "action_button1_modal", /* the checkbox hook */
							  "std" => "large",
							  "type" => "select",
							  "storage" => "value",
							  "options" => array(
								  "tiny"   => "Tiny, 30%",
								  "small"  => "Small, 40%",
								  "medium" => "Medium, 60%",
								  "large"  => "Large, 70%",
								  "xlarge" => "Extra Large, 95%"
							  ));


		do_action("justlanded_options_after_first_action_button");


		$of_options[] = array("name" => "Second Button",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Second Button Link Text",
							  "desc" => "This text will be shown on the second button. If the one-button layout is selected, this button will be hidden.",
							  "id" => "action_button2_text",
							  "std" => "Try our demo",
							  "type" => "text");

		$of_options[] = array("name" => "Second Button Link URL",
							  "desc" => "This is where the second button will link to. Your second action button could link to a product demonstration or a contact form.",
							  "id" => "action_button2_link",
							  "std" => "#",
							  "type" => "text");

		$link_target_options = array('Current Window', 'New Window');
		$of_options[] = array("name" => "Second Button Link Target",
							  "desc" => "Choose whether this link should open in a new window.",
							  "id" => "action_button2_target",
							  "std" => "Current Window",
							  "options" => $link_target_options,
							  "type" => "select");

		$of_options[] = array("name" => "Second Button OnClick Attribute",
							  "desc" => "This option adds an OnClick attribute to the link. This option may be useful for some tracking services.",
							  "id" => "action_button2_onclick",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Second Button Modal Window",
							  "desc" => "Check this to open a modal window with custom content if this button is clicked.",
							  "id" => "action_button2_modal",
							  "std" => 0,
							  "folds" => 1,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Second Button Modal Window Content",
							  "desc" => "Enter your custom mark-up here.",
							  "id" => "action_button2_modal_content",
							  "fold" => "action_button2_modal", /* the checkbox hook */
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Second Button Modal Window Page Content",
							  "desc" => "Select a page if you would like to use a WordPress page for this modal window.",
							  "id" => "action_button2_modal_page",
							  "fold" => "action_button2_modal", /* the checkbox hook */
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Second Button Modal Window Size",
							  "desc" => "Select a size for the modal window.",
							  "id" => "action_button2_modal_size",
							  "fold" => "action_button2_modal", /* the checkbox hook */
							  "std" => "large",
							  "type" => "select",
							  "storage" => "value",
							  "options" => array(
								  "tiny"   => "Tiny, 30%",
								  "small"  => "Small, 40%",
								  "medium" => "Medium, 60%",
								  "large"  => "Large, 70%",
								  "xlarge" => "Extra Large, 95%"
							  ));

		do_action("justlanded_options_after_second_action_button");

		do_action("justlanded_options_after_cta");

		/* --------------- Social Icons */
		$of_options[] = array("name" => "Social Icons",
							  "type" => "heading");

		$of_options[] = array("name" => "Social Icons",
							  "desc" => "",
							  "id" => "introduction_social",
							  "std" => "<h3 style=\"margin: 0 0 10px;\">Social Icons</h3>
					In this section you can choose which social icons to show, and in what order. While it is a good idea to provide your visitors
					with a variety of options to contact and connect with you, be aware that less is often more. Display the most important links
					relevant to your site and leave out the rest.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_before_social");

		$of_options[] = array("name" => "Enable or Disable Social Icons",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Header Social Icons",
							  "desc" => "These icons will be shown in your header. You should only add the most important icons here as space is limited. These icons will be hidden from view for mobile devices.",
							  "id" => "social_blocks_header",
							  "std" => $of_options_social_blocks,
							  "type" => "sorter");

		$of_options[] = array("name" => "Sidebar Social Icons",
							  "desc" => "These icons will be displayed on the right, as a static social icons bar. On mobile devices, these icons will be located in the footer section.",
							  "id" => "social_blocks_sidebar",
							  "std" => $of_options_social_blocks,
							  "type" => "sorter");

		$of_options[] = array("name" => "Social Icon URLs",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Twitter URL",
							  "desc" => "This is your Twitter URL, e.g. http://www.twitter.com/myusername.",
							  "id" => "social_twitter",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Facebook URL",
							  "desc" => "This is your Facebook profile URL",
							  "id" => "social_facebook",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Google+ URL",
							  "desc" => "This is your Google+ profile URL",
							  "id" => "social_googleplus",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "StumbleUpon URL",
							  "desc" => "This is your StumbleUp profile URL.",
							  "id" => "social_stumbleupon",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "YouTube URL",
							  "desc" => "This is your YouTube profile URL.",
							  "id" => "social_youtube",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Vimeo URL",
							  "desc" => "This is your Vimeo profile URL.",
							  "id" => "social_vimeo",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "LinkedIn URL",
							  "desc" => "This is your LinkedIn profile URL.",
							  "id" => "social_linkedin",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Pinterest URL",
							  "desc" => "This is your Pinterest profile URL.",
							  "id" => "social_pinterest",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Digg URL",
							  "desc" => "This is your Digg profile URL.",
							  "id" => "social_digg",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "MySpace URL",
							  "desc" => "This is your MySpace profile URL.",
							  "id" => "social_myspace",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Picasa URL",
							  "desc" => "This is your Picasa profile URL.",
							  "id" => "social_picasa",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "flickr URL",
							  "desc" => "This is your flickr profile URL.",
							  "id" => "social_flickr",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Dribbble URL",
							  "desc" => "This is your Dribbble profile URL.",
							  "id" => "social_dribbble",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Weibo URL",
							  "desc" => "This is your Weibo profile URL.",
							  "id" => "social_weibo",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Houzz URL",
							  "desc" => "This is your Houzz profile URL.",
							  "id" => "social_houzz",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Yelp URL",
							  "desc" => "This is your Yelp profile URL.",
							  "id" => "social_yelp",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Blogger URL",
							  "desc" => "This is your Blogger profile URL.",
							  "id" => "social_blogger",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Slideshare URL",
		                      "desc" => "This is your Slideshare profile URL.",
		                      "id" => "social_slideshare",
		                      "std" => "#",
		                      "type" => "text");


		$of_options[] = array("name" => "RSS URL",
							  "desc" => "This is your RSS Feed's URL.",
							  "id" => "social_rss",
							  "std" => "#",
							  "type" => "text");

		$of_options[] = array("name" => "Contact/Feedback Form URL",
							  "desc" => "This is your Contac/Feedback Form URL.",
							  "id" => "social_feedback",
							  "std" => "#",
							  "type" => "text");

		do_action("justlanded_options_after_social");

		/* --------------- List of Features */
		$of_options[] = array("name" => "Feature List",
							  "type" => "heading");

		$of_options[] = array("name" => "Feature List",
							  "desc" => "",
							  "id" => "introduction_features",
							  "std" => "<h3 style=\"margin: 0 0 10px;\">List of Features</h3>
					This is list of product features meant to highlight what your product is all about. It works well when used used just underneath the banner.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_before_features");

		$of_options[] = array("name" => "Headline",
							  "desc" => "This is the headline placed above the list of features.",
							  "id" => "features_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Sub Headline",
							  "desc" => "This is the sub headline placed above the list of features.",
							  "id" => "features_sub_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Feature Items",
							  "desc" => "A list of product features. Each feature has a headline, an icon or other image, and a short description.",
							  "id" => "features",
							  "std" => "",
							  "itemtype" => "Feature",
							  "has_link" => true,
							  "has_image" => true,
							  "type" => "slider");

		do_action("justlanded_options_after_features");


		/* --------------- Gallery */
		$of_options[] = array("name" => "Gallery",
							  "type" => "heading");

		$of_options[] = array("name" => "Gallery",
							  "desc" => "",
							  "id" => "introduction_gallery",
							  "std" => "<h3 style=\"margin: 0 0 10px;\">Gallery</h3>
					This gallery can be used for product shots/screenshots. The built-in gallery comes with a leightweight
					and elegant lightbox to display larger versions of the uploaded images.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_before_gallery");

		$of_options[] = array("name" => "Headline",
							  "desc" => "This is the headline placed above the gallery section.",
							  "id" => "gallery_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Sub Headline",
							  "desc" => "This is the sub headline placed above the gallery section.",
							  "id" => "gallery_sub_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Gallery Items",
							  "desc" => "Pictures to be featured in a thumbnail gallery with lightbox.",
							  "id" => "gallery",
							  "std" => "",
							  "itemtype" => "Picture",
							  "has_image" => true,
							  "has_link" => true,
							  "type" => "slider");

		do_action("justlanded_options_after_gallery");


		/* --------------- Video Gallery -- Reserved for v2.0.0 */
		/*
		$of_options[] = array("name" => "Video Gallery",
							  "type" => "heading");

		do_action("justlanded_options_before_video_gallery");

		$of_options[] = array("name" => "Headline",
							  "desc" => "This is the headline placed above the video gallery section.",
							  "id" => "video_gallery_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Sub Headline",
							  "desc" => "This is the sub headline placed above the video gallery section.",
							  "id" => "video_gallery_sub_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Video Clips",
							  "desc" => "Video clips to be presented in a lightbox.",
							  "id" => "video_gallery",
							  "std" => "",
							  "itemtype" => "Video",
							  "has_link" => true,
							  "type" => "slider");

		do_action("justlanded_options_after_video_gallery");
		 */


		/* --------------- Testimonials */
		$of_options[] = array("name" => "Testimonials",
							  "redirect" => true,
							  "type" => "heading");

		do_action("justlanded_options_before_testimonials");

		$of_options[] = array("name" => "Featured Testimonial",
							  "type" => "subheading");

		$of_options[] = array("name" => "Featured Testimonial",
							  "desc" => "This is a single, big testimonial.",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Headline",
							  "desc" => "This is the headline placed above the testimonial.",
							  "id" => "featured_testimonial_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Sub Headline",
							  "desc" => "This is the sub headline placed above the testimonial.",
							  "id" => "featured_testimonial_sub_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Testimonial/Quote",
							  "desc" => "This is the quote or testimonial to display.",
							  "id" => "testimonial_quote",
							  "std" => "",
							  "type" => "textarea");

		$of_options[] = array("name" => "Name",
							  "desc" => "Enter the name of the person you are quoting.",
							  "id" => "testimonial_name",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Job Title/Company Name",
							  "desc" => "Enter the title or company name here.",
							  "id" => "testimonial_title",
							  "std" => "",
							  "type" => "text");

		do_action("justlanded_options_after_featured_testimonial");

		$of_options[] = array("name" => "Testimonial Slider",
							  "type" => "subheading");

		$of_options[] = array("name" => "Testimonial Slider",
							  "desc" => "Testimonials will be retrieved from 'Small Testimonials'",
							  "descposition" => "heading",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Headline",
							  "desc" => "This is the headline placed above the testimonials section.",
							  "id" => "testimonial_slider_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Sub Headline",
							  "desc" => "This is the sub headline placed above the testimonials section.",
							  "id" => "testimonial_slider_sub_headline",
							  "std" => "",
							  "type" => "text");

		do_action("justlanded_options_after_testimonials_slider");


		$of_options[] = array("name" => "Small Testimonials",
							  "type" => "subheading");

		$of_options[] = array("name" => "Small Testimonials and Widget Content",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Headline",
							  "desc" => "This is the headline placed above the testimonials section.",
							  "id" => "testimonials_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Sub Headline",
							  "desc" => "This is the sub headline placed above the testimonials section.",
							  "id" => "testimonials_sub_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Small Testimonials",
							  "desc" => "A list of testimonials.",
							  "id" => "testimonials",
							  "std" => "",
							  "itemtype" => "Testimonial",
							  "has_title" => true,
							  "has_title_testimonial" => true,
							  "has_link" => true,
							  "has_subtitle" => true,
							  "has_subtitle_testimonial" => true,
							  "has_image" => true,
							  "has_price" => false,
							  "type" => "slider");

		$link_target_options = array('Current Window', 'New Window');
		$of_options[] = array("name" => "Link Target",
							  "desc" => "Choose whether this link should open in a new window.",
							  "id" => "testimonials_link_target",
							  "std" => "_blank",
							  "storage" => "value",
							  "options" => array(
								  "_blank" => "Open in new window",
								  "_self"  => "Open in same window",
							  ),
							  "type" => "select");

		do_action("justlanded_options_after_testimonials");


		/* --------------- Newsletter Signup Form */
		$of_options[] = array("name" => "Newsletter",
							  "type" => "heading");

		$of_options[] = array("name" => "Newsletter Integration",
							  "desc" => "",
							  "id" => "introduction_newsletter",
							  "std" => "<h3 style=\"margin: 0 0 10px;\">Newsletter Integration</h3>
					The built-in newsletter sign-up form requires a third-party mailing list service in order to work. We support the two major providers <a href=\"http://www.aweber.com/\" target=\"_blank\">AWeber</a> and <a href=\"http://www.mailchimp.com\" target=\"_blank\">Mailchimp</a>.
					However, it is possible to use just about any service with a few changes and some technical knowledge. Please <a href='mailto:support@shapingrain.com'>contact support</a> if you require assistance.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_before_newsletter");

		$of_options[] = array("name" => "Headline",
							  "desc" => "This is the call to action headline placed within the newsletter sign-up form.",
							  "id" => "newsletter_headline_1",
							  "std" => "Join our newsletter for exclusive deals",
							  "type" => "text");

		$of_options[] = array("name" => "Subscribe Button Text",
							  "desc" => "This is the Subscribe button text.",
							  "id" => "newsletter_button",
							  "std" => "Subscribe",
							  "type" => "text");

		$of_options[] = array("name" => "Email Text Field Placeholder",
							  "desc" => "This will be shown if the email address field is empty.",
							  "id" => "newsletter_placeholder",
							  "std" => "Your email address",
							  "type" => "text");

		$url = ADMIN_DIR . 'assets/images/';
		$newsletter_service_options = array(
			'aweber' => $url . 'newsletter_logo_aweber.png',
			'mailchimp' => $url . 'newsletter_logo_mailchimp.png',
			'custom' => $url . 'newsletter_logo_custom.png');
		do_action("justlanded_options_after_newsletter_service_array");

		$of_options[] = array("name" => "Service Selection",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Select a Service",
							  "desc" => "Click on an icon to select this mailing list service for integration.",
							  "id" => "newsletter_select",
							  "std" => "aweber",
							  "type" => "images",
							  "options" => $newsletter_service_options
		);

		$of_options[] = array("name" => "Service Settings",
							  "type" => "sectionheader");

		$of_options[] = array("name" => "Show AWeber Settings",
							  "desc" => "Check this to enter your AWeber settings.",
							  "id" => "newsletter_group_aweber",
							  "std" => 0,
							  "folds" => 1,
							  "type" => "checkbox");

		$of_options[] = array("name" => "AWeber Post URL",
							  "desc" => "This is where your form will submit the data to.",
							  "id" => "newsletter_aweber_url",
							  "std" => "http://www.aweber.com/scripts/addlead.pl",
							  "fold" => "newsletter_group_aweber", /* the checkbox hook */
							  "type" => "text");

		$of_options[] = array("name" => "AWeber form field 'meta_web_form_id'",
							  "desc" => "meta_web_form_id form field.",
							  "id" => "newsletter_aweber_meta_web_form_id",
							  "std" => "",
							  "fold" => "newsletter_group_aweber", /* the checkbox hook */
							  "type" => "text");

		$of_options[] = array("name" => "AWeber form field 'meta_split_id'",
							  "desc" => "meta_split_id form field.",
							  "id" => "newsletter_aweber_meta_split_id",
							  "std" => "",
							  "fold" => "newsletter_group_aweber", /* the checkbox hook */
							  "type" => "text");

		$of_options[] = array("name" => "AWeber form field 'listname'",
							  "desc" => "listname form field.",
							  "id" => "newsletter_aweber_listname",
							  "std" => "",
							  "fold" => "newsletter_group_aweber", /* the checkbox hook */
							  "type" => "text");

		$of_options[] = array("name" => "AWeber form field 'redirect'",
							  "desc" => "redirect form field. Use this parameter in order to redirect to a customized confirmation page.",
							  "id" => "newsletter_aweber_redirect",
							  "std" => "",
							  "fold" => "newsletter_group_aweber", /* the checkbox hook */
							  "type" => "text");

		$of_options[] = array("name" => "AWeber form field 'meta_redirect_onlist'",
							  "desc" => "redirect_onlist form field. Use this parameter in order to redirect someone already on the list to a custom confirmation page.",
							  "id" => "newsletter_aweber_redirect_onlist",
							  "std" => "",
							  "fold" => "newsletter_group_aweber", /* the checkbox hook */
							  "type" => "text");

		$of_options[] = array("name" => "AWeber form field 'meta_adtracking'",
							  "desc" => "meta_adtracking form field.",
							  "id" => "newsletter_aweber_meta_adtracking",
							  "std" => "My_Web_Form",
							  "fold" => "newsletter_group_aweber", /* the checkbox hook */
							  "type" => "text");

		$of_options[] = array("name" => "AWeber form field 'meta_message'",
							  "desc" => "meta_message form field.",
							  "id" => "newsletter_aweber_meta_message",
							  "std" => "1",
							  "fold" => "newsletter_group_aweber", /* the checkbox hook */
							  "type" => "text");

		do_action("justlanded_options_newsletter_after_aweber");

		$of_options[] = array("name" => "Show MailChimp Settings",
							  "desc" => "Check this to enter your MailChimp settings.",
							  "id" => "newsletter_group_mailchimp",
							  "std" => 0,
							  "folds" => 1,
							  "type" => "checkbox");

		$of_options[] = array("name" => "MailChimp Post URL",
							  "desc" => "This is where your form will submit the data to.",
							  "id" => "newsletter_mailchimp_url",
							  "std" => "http://sushisepeti.us6.list-manage.com/subscribe/post",
							  "fold" => "newsletter_group_mailchimp", /* the checkbox hook */
							  "type" => "text");

		$of_options[] = array("name" => "MailChimp form field 'u'",
							  "desc" => "This is an encoded user ID.",
							  "id" => "newsletter_mailchimp_u",
							  "std" => "",
							  "fold" => "newsletter_group_mailchimp", /* the checkbox hook */
							  "type" => "text");

		$of_options[] = array("name" => "MailChimp form field 'id'",
							  "desc" => "This is an encoded list ID.",
							  "id" => "newsletter_mailchimp_id",
							  "std" => "",
							  "fold" => "newsletter_group_mailchimp", /* the checkbox hook */
							  "type" => "text");

		do_action("justlanded_options_after_mailchimp");

		$of_options[] = array("name" => "Show Other Service's/Custom Settings",
							  "desc" => "Check this to enter your third party service/custom integration settings.",
							  "id" => "newsletter_group_custom",
							  "std" => 0,
							  "folds" => 1,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Other Service's/Custom Integration Code",
							  "desc" => "When directed to do so by support, or if your intention is to embed an unsupported third party integration code, please paste your code into this field.",
							  "id" => "newsletter_custom",
							  "fold" => "newsletter_group_custom", /* the checkbox hook */
							  "std" => "",
							  "type" => "textarea");

		do_action("justlanded_options_after_newsletter");


		/* --------------- Pricing Table */
		$of_options[] = array("name" => "Pricing Table",
							  "type" => "heading");

		$of_options[] = array("name" => "Pricing Table",
							  "desc" => "",
							  "id" => "introduction_pricing",
							  "std" => "<h3 style=\"margin: 0 0 10px;\">Pricing Table</h3>
					Use the pricing table section if you are selling a product in packages. You can add as many packages as you need and sort them via drag &amp; drop. They will appear on the landing page in the same order.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_before_pricing_table");

		$of_options[] = array("name" => "Headline",
							  "desc" => "This is the headline placed above the pricing table.",
							  "id" => "pricing_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Sub Headline",
							  "desc" => "This is the sub headline placed above the pricing table, just underneath the main headline.",
							  "id" => "pricing_sub_headline",
							  "std" => "",
							  "type" => "text");

		$of_options[] = array("name" => "Button Text",
							  "desc" => "This is the text shown on the buy button. This should be a call to action.",
							  "id" => "pricing_button_text",
							  "std" => "Buy now",
							  "type" => "text");

		$of_options[] = array("name" => "Currency Symbol",
							  "desc" => "This is the currency symbol to be used.",
							  "id" => "pricing_currency",
							  "std" => "$",
							  "type" => "text");

		$of_options[] = array("name" => "Position of Currency Symbol",
							  "desc" => "Choose where you would like the theme to display the currency symbol.",
							  "id" => "pricing_currency_position",
							  "std" => "before",
							  "type" => "select",
							  "storage" => "value",
							  "options" => array(
								  "before" => "before the price" ,
								  "after"  => "after the price",
							  ));


		$of_options[] = array("name" => "Packages",
							  "desc" => "A list of packages with features and prices. Use the <strong><em>Description</em></strong> field to enter package features, one per line.",
							  "id" => "pricing_packages",
							  "std" => "",
							  "itemtype" => "Package",
							  "has_link" => true,
							  "has_subtitle" => true,
							  "has_image" => false,
							  "has_price" => true,
							  "has_badge" => true,
							  "has_highlight" => true,
							  "type" => "slider");

		do_action("justlanded_options_after_pricing_table");

		/* --------------- Payment Options */
		$of_options[] = array("name" => "Payment Options",
							  "type" => "heading");

		$of_options[] = array("name" => "Payment Options",
							  "desc" => "",
							  "id" => "introduction_pricing",
							  "std" => "<h3 style=\"margin: 0 0 10px;\">Payment Options</h3>
					This is usually the last block of content. If your customer has not clicked the buy button yet, this is the last opportunity to convince him.
					Buying this product is easy - make sure your customers knows this.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_before_payment");

		$of_options[] = array("name" => "Headline",
							  "desc" => "This is the headline placed above the Payment Options block.",
							  "id" => "payment_headline",
							  "std" => "Come on, it's time to buy our product...",
							  "type" => "text");

		$of_options[] = array("name" => "Sub Headline",
							  "desc" => "This is the sub headline placed above the Payment Options block.",
							  "id" => "payment_sub_headline",
							  "std" => "This is your final chance to convince a customer",
							  "type" => "text");

		$of_options_payment = array("visa" => "Visa", "americanexpress" => "American Express", "mastercard" => "MasterCard", "paypal" => "PayPal", "ideal" => "iDeal", "bitcoin" => "Bitcoin");
		do_action("justlanded_options_after_payment_array");

		$of_options[] = array("name" => "Accepted Payment Methods",
							  "desc" => "Check payment methods you accept. Their corresponding icons will be shown underneath the Payment Options block.",
							  "id" => "payment_methods",
							  "std" => array("visa", "master", "paypal"),
							  "type" => "multicheck",
							  "options" => $of_options_payment);

		do_action("justlanded_options_after_payment");

		/* --------------- Extra Page Content/Free Content */
		$of_options[] = array("name" => "Page Content",
							  "type" => "heading");

		$of_options[] = array("name" => "Page Content",
							  "desc" => "",
							  "id" => "introduction_page_content",
							  "std" => "<h3 style=\"margin: 0 0 10px;\">Page Content</h3>
					This is the easiest way to add freeform content to your landing page. Simply create a regular WordPress page,
					add your content and set the page to retrieve the content from here. Make sure that the Page Content Blocks
					are enabled as well, otherwise they will not be displayed.",
							  "icon" => true,
							  "type" => "info");

		do_action("justlanded_options_before_content");

		/* Page Content 1 */
		$of_options[] = array("name" => "Page Content 1",
							  "id" => "page_content_1_section",
							  "type" => "sectionheader");

		$url = ADMIN_DIR . 'assets/images/';
		$of_options[] = array("name" => "Page Content 1 - Section Layout",
							  "desc" => "Select whether you want a single column or two columns in this section.",
							  "id" => "page_content_1_layout",
							  "std" => "fullwidth",
							  "type" => "images",
							  "options" => array(
								  'fullwidth' => $url . 'page-layout-full.png',
								  'split' => $url . 'page-layout-split.png')
		);

		$of_options[] = array("name" => "Page Content 1 - Show Page Title",
							  "desc" => "Check this if you would like the actual page title to be displayed.",
							  "id" => "page_content_1_showtitle",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Page Content 1 - Do not wrap block in extra section",
							  "desc" => "Check this if you wish to remove the content wrapper for this block. You will likely not want to use this option",
							  "id" => "page_content_1_nowrapper",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Page Content 1 - Source 1",
							  "desc" => "If the two-column layout is selected, this will be the first column. In full-width mode, this will be the only content shown.",
							  "id" => "page_content_1_source1",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Page Content 1 - Source 2",
							  "desc" => "If the two-column layout is selected, this will be the second column. In full-width mode, this will be hidden.",
							  "id" => "page_content_1_source2",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);


		/* Page Content 2 */

		$of_options[] = array("name" => "Page Content 2",
							  "id" => "page_content_2_section",
							  "type" => "sectionheader");

		$url = ADMIN_DIR . 'assets/images/';
		$of_options[] = array("name" => "Page Content 2 - Section Layout",
							  "desc" => "Select whether you want a single column or two columns in this section.",
							  "id" => "page_content_2_layout",
							  "std" => "fullwidth",
							  "type" => "images",
							  "options" => array(
								  'fullwidth' => $url . 'page-layout-full.png',
								  'split' => $url . 'page-layout-split.png')

		);

		$of_options[] = array("name" => "Page Content 2 - Show Page Title",
							  "desc" => "Check this if you would like the actual page title to be displayed.",
							  "id" => "page_content_2_showtitle",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Page Content 2 - Do not wrap block in extra section",
		                      "desc" => "Check this if you wish to remove the content wrapper for this block. You will likely not want to use this option",
							  "id" => "page_content_2_nowrapper",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Page Content 2 - Source 1",
							  "desc" => "If the two-column layout is selected, this will be the first column. In full-width mode, this will be the only content shown.",
							  "id" => "page_content_2_source1",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Page Content 2 - Source 2",
							  "desc" => "If the two-column layout is selected, this will be the second column. In full-width mode, this will be hidden.",
							  "id" => "page_content_2_source2",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);


		/* Page Content 3 */

		$of_options[] = array("name" => "Page Content 3",
							  "id" => "page_content_3_section",
							  "type" => "sectionheader");

		$url = ADMIN_DIR . 'assets/images/';
		$of_options[] = array("name" => "Page Content 3 - Section Layout",
							  "desc" => "Select whether you want a single column or two columns in this section.",
							  "id" => "page_content_3_layout",
							  "std" => "fullwidth",
							  "type" => "images",
							  "options" => array(
								  'fullwidth' => $url . 'page-layout-full.png',
								  'split' => $url . 'page-layout-split.png')

		);

		$of_options[] = array("name" => "Page Content 3 - Show Page Title",
							  "desc" => "Check this if you would like the actual page title to be displayed.",
							  "id" => "page_content_3_showtitle",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Page Content 3 - Do not wrap block in extra section",
		                      "desc" => "Check this if you wish to remove the content wrapper for this block. You will likely not want to use this option",
							  "id" => "page_content_3_nowrapper",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Page Content 3 - Source 1",
							  "desc" => "If the two-column layout is selected, this will be the first column. In full-width mode, this will be the only content shown.",
							  "id" => "page_content_3_source1",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Page Content 3 - Source 2",
							  "desc" => "If the two-column layout is selected, this will be the second column. In full-width mode, this will be hidden.",
							  "id" => "page_content_3_source2",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		/* Page Content 4 */

		$of_options[] = array("name" => "Page Content 4",
							  "id" => "page_content_4_section",
							  "type" => "sectionheader");

		$url = ADMIN_DIR . 'assets/images/';
		$of_options[] = array("name" => "Page Content 4 - Section Layout",
							  "desc" => "Select whether you want a single column or two columns in this section.",
							  "id" => "page_content_4_layout",
							  "std" => "fullwidth",
							  "type" => "images",
							  "options" => array(
								  'fullwidth' => $url . 'page-layout-full.png',
								  'split' => $url . 'page-layout-split.png')

		);

		$of_options[] = array("name" => "Page Content 4 - Show Page Title",
							  "desc" => "Check this if you would like the actual page title to be displayed.",
							  "id" => "page_content_4_showtitle",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Page Content 4 - Do not wrap block in extra section",
		                      "desc" => "Check this if you wish to remove the content wrapper for this block. You will likely not want to use this option",
							  "id" => "page_content_4_nowrapper",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Page Content 4 - Source 1",
							  "desc" => "If the two-column layout is selected, this will be the first column. In full-width mode, this will be the only content shown.",
							  "id" => "page_content_4_source1",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Page Content 4 - Source 2",
							  "desc" => "If the two-column layout is selected, this will be the second column. In full-width mode, this will be hidden.",
							  "id" => "page_content_4_source2",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		/* Page Content 5 */

		$of_options[] = array("name" => "Page Content 5",
							  "id" => "page_content_5_section",
							  "type" => "sectionheader");

		$url = ADMIN_DIR . 'assets/images/';
		$of_options[] = array("name" => "Page Content 5 - Section Layout",
							  "desc" => "Select whether you want a single column or two columns in this section.",
							  "id" => "page_content_5_layout",
							  "std" => "fullwidth",
							  "type" => "images",
							  "options" => array(
								  'fullwidth' => $url . 'page-layout-full.png',
								  'split' => $url . 'page-layout-split.png')

		);

		$of_options[] = array("name" => "Page Content 5 - Show Page Title",
							  "desc" => "Check this if you would like the actual page title to be displayed.",
							  "id" => "page_content_5_showtitle",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Page Content 5 - Do not wrap block in extra section",
		                      "desc" => "Check this if you wish to remove the content wrapper for this block. You will likely not want to use this option",
							  "id" => "page_content_5_nowrapper",
							  "std" => 0,
							  "type" => "checkbox");

		$of_options[] = array("name" => "Page Content 5 - Source 1",
							  "desc" => "If the two-column layout is selected, this will be the first column. In full-width mode, this will be the only content shown.",
							  "id" => "page_content_5_source1",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		$of_options[] = array("name" => "Page Content 5 - Source 2",
							  "desc" => "If the two-column layout is selected, this will be the second column. In full-width mode, this will be hidden.",
							  "id" => "page_content_5_source2",
							  "std" => "",
							  "type" => "select",
							  "storage" => "value",
							  "options" => $of_pages);

		do_action("justlanded_options_after_content");

		do_action("justlanded_options_bottom");

	}
}
?>
