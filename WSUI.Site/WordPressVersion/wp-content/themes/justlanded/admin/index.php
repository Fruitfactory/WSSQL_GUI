<?php
/* -----------------------------------------------------------------------------------
    ShapingRain Options Panel
    Based on the Slightly Modded Options Framework (SMOF) by AQUAGRAPHITE/Syamil MJ
    http://www.shapingrain.com
    If you have any questions or need help, please contact support@shapingrain.com
    with your Purchase Code (part of the license certificate you can download
    from themeforest)

    Inspired by and partly based on:
    Thematic Options Panel - http://wptheming.com/2010/11/thematic-options-panel-v2/
	KIA Thematic Options Panel - https://github.com/helgatheviking/thematic-options-KIA
	Woo Themes - http://woothemes.com/
	Option Tree - http://wordpress.org/extend/plugins/option-tree/
/*-----------------------------------------------------------------------------------*/

/**
 * Definitions
 */
$theme_version = '';
global $justlanded_admin_notice;

if( function_exists( 'wp_get_theme' ) ) { // WP 3.4+
	if( is_child_theme() ) {
		$temp_obj = wp_get_theme();
		$theme_obj = wp_get_theme( $temp_obj->get('Template') );
	} else {
		$theme_obj = wp_get_theme();    
	}

	$theme_version = $theme_obj->get('Version');
	$theme_name = $theme_obj->get('Name');
	$theme_uri = $theme_obj->get('ThemeURI');
	$author_uri = $theme_obj->get('AuthorURI');
} else { // < WP 3.4
	$theme_data = get_theme_data( TEMPLATEPATH.'/style.css' );
	$theme_version = $theme_data['Version'];
	$theme_name = $theme_data['Name'];
	$theme_uri = $theme_data['ThemeURI'];
	$author_uri = $theme_data['AuthorURI'];
}

define( 'OF_DEBUG_MODE', FALSE);

if (!defined('OF_ENABLE_UPDATES'))
{
    define( 'OF_ENABLE_UPDATES', TRUE);
}

define( 'SMOF_VERSION', '2.0.0' );
define( 'THEME_VERSION', $theme_version);
define( 'ADMIN_PATH', TEMPLATEPATH . '/admin/' );
define( 'ADMIN_DIR', get_template_directory_uri() . '/admin/' );
define( 'LAYOUT_PATH', ADMIN_PATH . '/layouts/' );
define( 'THEMENAME', $theme_name );
define( 'THEMEVERSION', $theme_version );
define( 'THEMEURI', $theme_uri );
define( 'THEMEAUTHORURI', $author_uri );

define( 'OF_HELP_ACTIVE', TRUE );

// Help Sidebar
$help_sidebar_content[]= "<p><strong>".__("More Help:",'optionsframework')."</strong></p>";
$help_sidebar_content[]= "<p><a target=\"_blank\" href=\"http://www.shapingrain.com/\">ShapingRain.com</a></p>";
$help_sidebar_content[]= "<p><a target=\"_blank\" href=\"http://www.shapingrain.com/support/\">".__("Support Home",'justlanded')."</a></p>";
define( 'OF_HELP_SIDEBAR', implode('', $help_sidebar_content));

/**
 * Implementation of profiles for ShapingRain.com options
 * */

do_action("justlanded_before_profile_settings");

if (!defined('MAXPROFILES')) define( 'MAXPROFILES', 20);
if (!defined('JUSTLANDED_OPTIONS_FONT_PREVIEW')) define( 'JUSTLANDED_OPTIONS_FONT_PREVIEW', false);

$active_profile = get_option($theme_name.'_profile_active');
if ($active_profile == 0)
{
    $active_profile = 1;
    update_option($theme_name.'_profile_active', 1);
}
define( 'ACTIVEPROFILE', $active_profile);
define( 'ACTIVEPROFILE_FIELD', $theme_name.'_profile_active' );

define('OPTIONSPREFIX', $theme_name.'_options' . '_');

if (isset($_POST) && isset($_POST['security']) && isset ($_POST['profile'])) {
    $nonce        =  $_POST['security'];
    $edit_profile =  intval($_POST['profile']);
    if (wp_verify_nonce($nonce, 'of_ajax_nonce')) {
        define('OPTIONS', $theme_name.'_options' . '_' . $edit_profile);
		if (!defined('ACTIVEPROFILE_EDIT')) define('ACTIVEPROFILE_EDIT', $edit_profile);
    };
}

if (!defined('ACTIVEPROFILE_EDIT')) define('ACTIVEPROFILE_EDIT', ACTIVEPROFILE);
if (!defined('ACTIVEPROFILE_EDIT_OPTIONS')) define('ACTIVEPROFILE_EDIT_OPTIONS', $theme_name.'_options' . '_' . ACTIVEPROFILE_EDIT );


if(!defined('OPTIONS')) {
    define('OPTIONS', $theme_name.'_options' . '_' . ACTIVEPROFILE );
}

define('BACKUPS',$theme_name.'_backups' . ACTIVEPROFILE_EDIT );

$site_default_profile = get_option($theme_name.'_site_default_profile');
if ($site_default_profile == 0)
{
    $site_default_profile = 1;
    update_option($theme_name.'_site_default_profile', 1);
}
define('SITE_DEFAULT_PROFILE', $site_default_profile);

/**
 * Required action filters
 */
if (is_admin() && isset($_GET['activated'] ) && isset($pagenow) && $pagenow == "themes.php" ) add_action('admin_init','of_option_setup');
add_action('admin_head', 'optionsframework_admin_message');
add_action('admin_init','optionsframework_admin_init');
add_action('admin_menu', 'optionsframework_add_admin');
add_action( 'init', 'optionsframework_mlu_init');

/*
 * Add menu for JustLanded to Admin Bar (Toolbar as of WP 3.3)
 */
function justlanded_custom_adminbar_menu( $meta = TRUE ) {
    global $wp_admin_bar;
    if ( !is_user_logged_in() ) { return; }
    if ( !is_super_admin() || !is_admin_bar_showing() ) { return; }
    $wp_admin_bar->add_menu( array(
        'id' => 'justlanded_menu',
        'href' => admin_url('themes.php?page=optionsframework'),
        'title' => __( 'Theme Options', 'optionsframework' ) )
    );

    $wp_admin_bar->add_menu( array(
            'parent' => 'justlanded_menu',
            'id'     => 'custom_links',
            'title' => __( 'JustLanded Options', 'optionsframework'),
            'href' => admin_url('themes.php?page=optionsframework'),
            'meta'  => array())
    );

    do_action("justlanded_admin_menu_after_options");

    $wp_admin_bar->add_menu( array(
            'parent' => 'justlanded-external',
            'id'     => 'justlanded_about',
            'title' => __( 'About JustLanded', 'optionsframework'),
            'href' => admin_url('index.php?page=justlanded-about'),
            'meta'  => array())
    );

    $wp_admin_bar->add_menu( array(
            'parent' => 'justlanded-external',
            'id'     => 'shapingrain_link',
            'title' => __( 'ShapingRain.com', 'optionsframework'),
            'href' => 'http://www.shapingrain.com/',
            'meta'  => array( 'target' => '_blank' ) )
    );
    $wp_admin_bar->add_menu( array(
            'parent' => 'justlanded-external',
            'id'     => 'shapingrain_support',
            'title' => __( 'Support', 'optionsframework'),
            'href' => 'http://www.shapingrain.com/support/',
            'meta'  => array( 'target' => '_blank' ) )
    );

    do_action("justlanded_admin_menu_after_support");

    $wp_admin_bar->add_group( array(
        'parent' => 'justlanded_menu',
        'id'     => 'justlanded-external',
        'meta'   => array(
            'class' => 'ab-sub-secondary',
        ),
    ) );
}
add_action( 'admin_bar_menu', 'justlanded_custom_adminbar_menu', 15 );

function justlanded_custom_menu_css() {
    $custom_menu_css = '<style type="text/css">
        #wp-admin-bar-justlanded_menu > .ab-item .ab-icon {
        width: 20px;
        height: 20px;
        margin-top: 4px;
        background-image: url('.get_template_directory_uri().'/shortcodes/images/button-justlanded.png);
        background-position: 0 0;
        background-repeat: no-repeat;
        }

        #wp-admin-bar-justlanded_menu > .ab-item:before {
        content: "\f111";
        top: 2px;
        }

		.about-badge {
			position: absolute;
			top:0;
			right:0;
		}
		.about-badge .image {
			max-width:200px;
			-webkit-box-shadow: 0 1px 3px rgba(0,0,0,.2);
			box-shadow: 0 1px 3px rgba(0,0,0,.2);
		}

</style>';
    echo $custom_menu_css;
}

if ( is_user_logged_in() && ( is_super_admin() || !is_admin_bar_showing()) ) {
    add_action( 'wp_head', 'justlanded_custom_menu_css' );
}
add_action( 'admin_head', 'justlanded_custom_menu_css' );


/**
 * Required Files
 */
require_once ( ADMIN_PATH . 'functions/functions.load.php' );
require_once ( ADMIN_PATH . 'classes/class.options_machine.php' );
require_once ( ADMIN_PATH . 'classes/theme-update-checker.php' );

/*
 * Update Check
 */

if (!function_exists('justlanded_update_check')) {
    function justlanded_update_check() {
        $domain_hash = sha1(home_url());
        $license_key = get_option(THEMENAME.'_global_license_key');
        if (!isset($license_key) || $license_key == "") $license_key = "null";

        $justlanded_update_checker = new ThemeUpdateChecker(
            'justlanded',
            'https://ssl.shapingrain.com/update/version/justlandedwp/'.$license_key.'/'.$domain_hash.'/' //URL of the metadata file.
        );
        $justlanded_update_checker->addQueryArgFilter('justlanded_update_check_query_args');
        $justlanded_update_checker->addResultFilter('justlanded_update_check_results');
        $justlanded_update_checker->addHttpRequestArgFilter('justlanded_update_check_http_args');


        // for debugging purposes = check for updates every time
		/*
        $justlanded_update_checker->checkForUpdates();
        $justlanded_update_checker->requestUpdate();
		*/

    }
}
if (!function_exists('justlanded_update_check_query_args')) {
    function justlanded_update_check_query_args($args) {

        return $args;
    }
}
if (!function_exists('justlanded_update_check_http_args')) {
    function justlanded_update_check_http_args($args) {
        $args['sslverify'] = false;
        return $args;
    }
}
if (!function_exists('justlanded_update_check_results')) {
    function justlanded_update_check_results($thupdate, $result) {
        global $justlanded_admin_notice;

        if (is_object($result)) $result = (array)$result;
        if(isset($result) && is_array($result) && isset($result['response']) && isset($result['response']['code']) && $result['response']['code'] == 404) {
            $justlanded_admin_notice = __('An error has occurred while trying to retrieve update information for JustLanded for WordPress. Please contact support@shapingrain.com if this problem reoccurs.', 'justlanded');
            if (isset($result['body'])) $justlanded_admin_notice = trim($result['body']);
            add_action( 'admin_notices', 'justlanded_update_check_admin_notice' );
        }
        return $thupdate;
    }
}

function justlanded_update_check_admin_notice() {
    global $justlanded_admin_notice;
    ?>
    <div class="error">
        <p><strong><?php _e('An error has occured while trying to retrieve update information for JustLanded for WordPress. Please check the validity of your license key (or remove the license key if you do not have one) and contact support@shapingrain.com if this problem persists', 'justlanded'); ?></strong></p>
        <p><?php _e( $justlanded_admin_notice, 'justlanded' ); ?></p>
    </div>
<?php
}


add_action('http_request_args', 'justlanded_no_ssl_http_request_args', 10, 2);
if (!function_exists('justlanded_no_ssl_http_request_args')) {
    function justlanded_no_ssl_http_request_args($args, $url) {
        $args['sslverify'] = false;
        return $args;
    }
}

if (defined('OF_ENABLE_UPDATES') && OF_ENABLE_UPDATES == true) {
    $disable_updates = get_option(THEMENAME.'_global_disable_updates');
    if ($disable_updates != true) {
        add_action( 'init', 'justlanded_update_check' );
    }
}




/**
 * AJAX Saving Options
 */
add_action('wp_ajax_of_ajax_post_action', 'of_ajax_callback');
?>