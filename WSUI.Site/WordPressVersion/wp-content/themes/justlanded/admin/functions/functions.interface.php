<?php
/**
 * Options Framework Interface
 */

/**
 * Admin Init
 *
 * @uses wp_verify_nonce()
 * @uses header()
 * @uses update_option()
 *
 * @since 1.0.0
 */
function optionsframework_admin_init() 
{
	// Rev up the Options Machine
	global $of_options, $options_machine;
	$options_machine = new Options_Machine($of_options);
}

/**
 * Create Options page
 *
 * @uses add_theme_page()
 * @uses add_action()
 *
 * @since 1.0.0
 */
function optionsframework_add_admin() {
	
    $of_page = add_theme_page( THEMENAME, 'Theme Options', 'edit_theme_options', 'optionsframework', 'optionsframework_options_page');

    if (OF_HELP_ACTIVE == true) {
        if ($of_page){add_action('load-'.$of_page, 'optionsframework_help_init');}
    }

	// Add framework functionaily to the head individually
	add_action("admin_print_scripts-$of_page", 'of_load_only');
	add_action("admin_print_styles-$of_page",'of_style_only');
	add_action("admin_print_styles-$of_page", 'optionsframework_mlu_css', 0 );
	add_action("admin_print_scripts-$of_page", 'optionsframework_mlu_js', 0 );
}

/*
 * Internal Help System
 * by ShapingRain.com
 */
function optionsframework_help_init() {
    global $wp_version;
    if (version_compare($wp_version, '3.3', '>=')) {
        $screen = get_current_screen();

        if ($handle = opendir(ADMIN_PATH . 'help')) {
            $help_sections = array();
            while (false !== ($entry = readdir($handle))) {
                if (substr_count($entry, 'help-') > 0)
                {
                    require_once(ADMIN_PATH . 'help' . DIRECTORY_SEPARATOR . $entry);
                    $help_id = str_replace(".php", "", str_replace("help-", "", $entry));
                    $help_info = call_user_func('optionsframework_help_info_' . $help_id);
                    $help_sections[$help_info['sort']] = $help_info;
                }
            }
            closedir($handle);
            ksort($help_sections);
            foreach ($help_sections as $help_section)
            {
                $screen->add_help_tab(array(
                    'title' => __($help_section['caption'],'justlanded'),
                    'id' => $help_section['id'],
                    'content' => '',
                    'callback' => 'optionsframework_help_' . $help_section['id']
                ));
            }
        }
        else {
            return false;
        }

        $screen->set_help_sidebar(OF_HELP_SIDEBAR);
    }else{
        // we do not provide in-context help for WordPress < 3.x
    }
}

/**
 * Build Options page
 *
 * @since 1.0.0
 */
function optionsframework_options_page(){

	global $options_machine;
	include_once( ADMIN_PATH . 'frontend/options.php' );
}

/**
 * Create Options page
 *
 * @uses wp_enqueue_style()
 *
 * @since 1.0.0
 */
function of_style_only(){
	wp_enqueue_style('admin-style', ADMIN_DIR . 'assets/css/admin-style.css', array(), THEME_VERSION);
	wp_enqueue_style('color-picker', ADMIN_DIR . 'assets/css/jquery.miniColors.css', array(), THEME_VERSION);
}

/**
 * Create Options page
 *
 * @uses add_action()
 * @uses wp_enqueue_script()
 *
 * @since 1.0.0
 */
function of_load_only() 
{
	add_action('admin_head', 'of_admin_head');
	
	wp_enqueue_script('jquery-ui-core');
	wp_enqueue_script('jquery-ui-sortable');
	wp_enqueue_script('jquery-input-mask', ADMIN_DIR .'assets/js/jquery.maskedinput-1.2.2.js', array( 'jquery' ), THEME_VERSION);
	wp_enqueue_script('tipsy', ADMIN_DIR .'assets/js/jquery.tipsy.js', array( 'jquery' ), THEME_VERSION);
	wp_enqueue_script('ajaxupload', ADMIN_DIR .'assets/js/ajaxupload.js', array('jquery'), THEME_VERSION);
	wp_enqueue_script('cookie', ADMIN_DIR . 'assets/js/cookie.js', 'jquery', THEME_VERSION);
    wp_enqueue_script('miniColors', ADMIN_DIR .'assets/js/jquery.miniColors.min.js', array('jquery'), THEME_VERSION);
	wp_enqueue_script('smof', ADMIN_DIR .'assets/js/framework.js', array( 'jquery' ), THEME_VERSION);
}

/**
 * Front end inline jquery scripts
 *
 * @since 1.0.0
 */
function of_admin_head() { ?>
		
	<script type="text/javascript" language="javascript">

	jQuery.noConflict();
	jQuery(document).ready(function($){

		// COLOR Picker
		$('.miniColors').each(function(){
            $(this).miniColors();
		}); //end color picker

	}); //end doc ready
	
	</script>
	
<?php }

/**
 * Ajax Save Options
 *
 * @uses get_option()
 * @uses update_option()
 *
 * @since 1.0.0
 */
function of_ajax_callback()
{
	@set_time_limit(90);
    global $options_machine, $of_options;

	@$nonce=$_POST['security'];

	if (! wp_verify_nonce($nonce, 'of_ajax_nonce') ) die('-1');

    //get options array from db
	$all = get_option(OPTIONS);

    //get only fonts
    //shapingrain.com
    if ($_POST['type'] == "webfonts") {
        @$web_fonts_tmp = json_decode(file_get_contents(ADMIN_PATH."defaults".DIRECTORY_SEPARATOR."google-webfonts.json"));
        $web_fonts = array();

        $web_fonts['Arial*'] = array(
            'variants' => array('regular', 'bold', 'italic', 'bold italic'),
            'subsets' => array('latin')
        );
        $web_fonts['Times New Roman*'] = array(
            'variants' => array('regular', 'bold', 'italic', 'bold italic'),
            'subsets' => array('latin')
        );
        $web_fonts['Courier New*'] = array(
            'variants' => array('regular', 'bold', 'italic', 'bold italic'),
            'subsets' => array('latin')
        );
        $web_fonts['Georgia*'] = array(
            'variants' => array('regular', 'bold', 'italic', 'bold italic'),
            'subsets' => array('latin')
        );
        $web_fonts['Verdana*'] = array(
            'variants' => array('regular', 'bold', 'italic', 'bold italic'),
            'subsets' => array('latin')
        );
        $web_fonts['Geneva*'] = array(
            'variants' => array('regular', 'bold', 'italic', 'bold italic'),
            'subsets' => array('latin')
        );


        foreach ($web_fonts_tmp->items as $web_fonts_item) {
            @$web_fonts[$web_fonts_item->family] = array(
                'variants' => $web_fonts_item->variants,
                'subsets'  => $web_fonts_item->subsets,
            );
        }
        @$web_font_variants = $web_fonts[$_POST['font']]['variants'];
        @$web_font_subsets  = $web_fonts[$_POST['font']]['subsets'];

        echo json_encode(array('variants' => $web_font_variants, 'subsets' => $web_font_subsets));

        exit(1);
    }

    if (@$_POST['type'] == "profile")
    {
        @$activeProfile = intval($_POST['profile']);
        if ($activeProfile != 0)
        {
            update_option(ACTIVEPROFILE_FIELD, intval($activeProfile));
            echo ACTIVEPROFILE.":".$activeProfile;
            exit(1);
        }
    }

	$save_type = $_POST['type'];
	
	//Uploads
	if($save_type == 'upload')
	{
        $clickedID = $_POST['data']; // Acts as the name
        $filename = $_FILES[$clickedID];
        $filename['name'] = preg_replace('/[^a-zA-Z0-9._\-]/', '', $filename['name']);

        $override['test_form'] = false;
        $override['action'] = 'wp_handle_upload';
        $uploaded_file = wp_handle_upload($filename,$override);

        $upload_tracking[] = $clickedID;

        //update $options array w/ image URL
        $upload_image = $all; //preserve current data

        $upload_image[$clickedID] = $uploaded_file['url'];

        update_option(ACTIVEPROFILE_EDIT_OPTIONS, $upload_image ) ;


        if(!empty($uploaded_file['error'])) {echo 'Upload Error: ' . $uploaded_file['error']; }
        else { echo $uploaded_file['url']; } // Is the Response
	}
	elseif($save_type == 'image_reset')
	{
			$id = $_POST['data']; // Acts as the name
			$delete_image = $all; //preserve rest of data
			$delete_image[$id] = ''; //update array key with empty value	 
			update_option(ACTIVEPROFILE_EDIT_OPTIONS, $delete_image ) ;
	}
	elseif($save_type == 'backup_options')
	{
		$backup = $all;
		$backup['backup_log'] = date('r');
		update_option(BACKUPS, $backup ) ;
		die('1');
	}
	elseif($save_type == 'restore_options')
	{
		$data = get_option(BACKUPS);
		update_option(ACTIVEPROFILE_EDIT_OPTIONS, $data);
		die('1');
	}
    elseif($save_type == 'save_demo')
    {
        @$import_file = ADMIN_PATH."defaults".DIRECTORY_SEPARATOR."demo.json";
        if (file_exists($import_file)) {
            $profiles = range(1, MAXPROFILES);
            $master=array();
            foreach ($profiles as $profile)
            {
                $data = get_option(OPTIONSPREFIX . $profile);
                if ($data == false) {
                    $master[$profile] = $options_machine->Defaults;
                }
                else {
                    $master[$profile] = $data;
                }
            }
            file_put_contents($import_file, base64_encode(json_encode($master)));
        }
        else {
            die('0');
        }

        die('1');
    }
    elseif($save_type == 'save_master_export')
    {
        $profiles = range(1, MAXPROFILES);
        $master=array();
        foreach ($profiles as $profile)
        {
            $data = get_option(OPTIONSPREFIX . $profile);
            if ($data == false) {
                $master[$profile] = $options_machine->Defaults;
            }
            else {
                $master[$profile] = $data;
            }
        }
        header("Content-type: text/plain");
        header("Content-Disposition: attachment; filename=justlanded_export.txt");
        echo base64_encode(json_encode($master));
        die();
    }
    elseif($save_type == 'save_master_import')
    {
        @$import_file = ADMIN_PATH."defaults".DIRECTORY_SEPARATOR."justlanded_export.txt";
        if (file_exists($import_file)) {
            @$master = json_decode(base64_decode(file_get_contents($import_file)), TRUE);
            $profiles = range(1, MAXPROFILES);
            foreach ($profiles as $profile)
            {
                if (isset($master[$profile])) {
                    update_option(OPTIONSPREFIX . $profile, $master[$profile]);
                }
            }
            die('1');
        }
        else {
            die('0');
        }
        die('1');
    }
    elseif($save_type == 'restore_demo')
    {
        @$import_file = ADMIN_PATH."defaults".DIRECTORY_SEPARATOR."demo.json";
        if (file_exists($import_file)) {
            @$master = json_decode(base64_decode(file_get_contents($import_file)), TRUE);
            $profiles = range(1, MAXPROFILES);
            foreach ($profiles as $profile)
            {
                if (isset($master[$profile])) {
                    update_option(OPTIONSPREFIX . $profile, $master[$profile]);
                }
            }
            die('1');
        }
        else {
            die('0');
        }
        die('1');
    }
	elseif($save_type == 'import_options'){
        $data = get_option(ACTIVEPROFILE_EDIT_OPTIONS);

		@$new_data = $_POST['data'];
		@$new_data = json_decode(base64_decode($new_data), TRUE); //100% safe - ignore theme check nag

        foreach ($new_data as $d_key => $d_value) {
            try {
                unset($data[$d_key]);
                @$data[$d_key] = $d_value;
            }
            catch (Exception $e) {
                die('0');
            }
        }
		update_option(ACTIVEPROFILE_EDIT_OPTIONS, $data);
		die('1');
	}
    elseif($save_type == 'import_preset'){
        $data = get_option(ACTIVEPROFILE_EDIT_OPTIONS);

        @$folder = $_POST['folder'];
        @$file   = $_POST['file'];
        @$import_file = ADMIN_PATH."presets".DIRECTORY_SEPARATOR.$folder.DIRECTORY_SEPARATOR.$file.".txt";

        if (!file_exists($import_file)) {
            die('0');
        }

        @$new_data_tmp = explode("\n", file_get_contents($import_file));
        @$new_data = $new_data_tmp[2];
        @$new_data = json_decode(base64_decode($new_data), TRUE); //100% safe - ignore theme check nag

        foreach ($new_data as $d_key => $d_value) {
            try {
                unset($data[$d_key]);
                $d_value = str_replace("__themedir__", get_template_directory_uri(), $d_value);
                @$data[$d_key] = $d_value;
            }
            catch (Exception $e) {
                die('0');
            }
        }
        update_option(ACTIVEPROFILE_EDIT_OPTIONS, $data);
        die('1');
    }
	elseif ($save_type == 'save')
	{
		wp_parse_str(stripslashes($_POST['data']), $data);

        if (isset($data['site_default_profile'])) {
            update_option(THEMENAME.'_site_default_profile', $data['site_default_profile']);
        }

        foreach ($data as $key => $val) {
            if (substr($key, 0, 7) == "global_") {
                update_option(THEMENAME.'_'.$key, $val);
                unset($data[$key]);
            }
        }

        unset($data['security']);
		unset($data['of_save']);
		update_option(ACTIVEPROFILE_EDIT_OPTIONS, $data);
		die('1');
	}
	elseif ($save_type == 'copy_profile')
	{
		$profile = intval($_POST['profile']);
		$target  = intval($_POST['target']);
		$data = get_option(OPTIONSPREFIX . $profile);
		update_option(OPTIONSPREFIX . $target, $data);
		die('1'); //options reset
	}
	elseif ($save_type == 'reset')
	{
    	update_option(ACTIVEPROFILE_EDIT_OPTIONS,$options_machine->Defaults);
        die('1'); //options reset
	}

  	die();
}

?>