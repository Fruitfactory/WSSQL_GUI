<?php
/**
 * Admin Functions for Options Framework
  */

/**
 * Head Hook
 *
 * @since 1.0.0
 */
function of_head() { do_action( 'of_head' ); }

/**
 * Add default options upon activation else DB does not exist
 *
 * @since 1.0.0
 */
function of_option_setup()	
{
	global $of_options, $options_machine;
	$options_machine = new Options_Machine($of_options);
		
	if (!get_option(OPTIONS))
	{
		update_option(OPTIONS,$options_machine->Defaults);
	}

    wp_safe_redirect( admin_url( 'index.php?page=justlanded-about' ) );
    exit;
}

/**
 * Change activation message
 *
 * @since 1.0.0
 */
function optionsframework_admin_message() {
    remove_submenu_page( 'index.php', 'justlanded-about' );
}

function justlanded_admin_menus() {
    $about = add_dashboard_page( __('Welcome to JustLanded for WordPress', 'justlanded'),  __('Welcome to JustLanded for WordPress', 'justlanded'), 'manage_options', 'justlanded-about', 'justlanded_about_screen' );
}
add_action( 'admin_menu', 'justlanded_admin_menus' );


function justlanded_about_screen() {
    $title = __( 'Welcome to JustLanded for WordPress' , 'justlanded');
    $display_version = THEME_VERSION;
    ?>
    <div class="wrap about-wrap about-justlanded">

        <h1><?php printf( __( 'Welcome to JustLanded for WordPress %s' ), THEME_VERSION , 'justlanded' ); ?></h1>

        <div class="about-text"><?php printf( __( 'Thank you for using JustLanded for WordPress.' , 'justlanded' ), $display_version ); ?></div>

		<div class="about-badge"><img alt="JustLanded" src="<?php echo get_template_directory_uri() . "/screenshot.png"; ?>" class="image" /></div>

		<h2 class="nav-tab-wrapper">
			<a href="#" class="nav-tab">
				<?php _e( 'Updates'  , 'justlanded'); ?>
			</a>
            <a href="#features" class="nav-tab">
                <?php _e( 'Features'  , 'justlanded'); ?>
            </a>
			<a href="#manual" class="nav-tab">
                    <?php _e( 'Documentation' , 'justlanded' ); ?>
            </a>
			<a href="#support" class="nav-tab">
                <?php _e( 'Customer Support' , 'justlanded' ); ?>
            </a>
        </h2>

		<div class="changelog">

		</div>

		<div class="changelog">
			<h3 id="updates"><?php _e( 'Updates' ); ?></h3>
			<div class="feature-section col three-col about-updates">
				<div class="col-1">
					<h4>WordPress 3.8</h4>
					<p>JustLanded is compatible with WordPress 3.8. We have made some subtle changes to the interface to ensure that the interface looks best.</p>
				</div>
				<div class="col-2">
					<h4>Bugfixes</h4>
					<p>We have addressed some minor bugs that had been brought to our attention. If you encounter an issue, please always contact <a href="mailto:support@shapingrain.com">support@shapingrain.com</a> so we can follow up on that.</p>
				</div>
				<div class="col-3 last-feature">
					<h4>Customization</h4>
					<p>Additional options for developers enable fine-tuned customization of the theme with or without using child themes. A Developer's Reference introduces some of the new features.</p>
				</div>
			</div>
		</div>

		<div class="changelog">
            <h3 id="features"><?php _e( 'Powerful Features' ); ?></h3>

            <div class="feature-section images-stagger-right">
                <h4><?php _e( 'Landing Pages' , 'justlanded' ); ?></h4>
                <p><?php _e(  'While JustLanded is a regular WordPress theme that comes with the usual features to run a blog on, it also comes with a powerful set of building blocks to help you build landing pages with ease.' , 'justlanded' ); ?></p>

                <h4><?php _e( 'Profiles' , 'justlanded' ); ?></h4>
                <p><?php _e( 'JustLanded has a unique feature that enables you to create multiple landing pages on one installation of WordPress. Each page can have its unique header and footer, colors, fonts etc., even its own scripts and Custom CSS code.' , 'justlanded' ); ?></p>
                <p><?php _e( 'Learn more about this unique feature in our <em>Quick Guide: Multiple Landing Pages Using Settings Profiles</em> that ships in the \'Documentation\' folder.' , 'justlanded' ); ?></p>

                <h4><?php _e( 'Plug-ins' , 'justlanded' ); ?></h4>
                <p><?php _e( 'Some features aren\'t for everyone, so we turned these into plugins. One of these free plug-ins that ship with the theme enables integration with the popular WooCommerce shopping cart solution.' , 'justlanded' ); ?></p>
                <p><?php _e( 'You can find all free plug-ins that ship with the theme in the \'Plug-Ins\' folder.' , 'justlanded'); ?></p>

                <h4><?php _e( 'Free Lifetime Updates' , 'justlanded' ); ?></h4>
                <p><?php _e( 'With your purchase of JustLanded for WordPress you have access to free updates. The theme supports automatic updates* if you <a href="https://ssl.shapingrain.com/register" target="_blank">register a free account</a> with us and enter a ShapingRain.com license key. Alternatively you can update the theme manually by downloading the latest package from themeforest.' , 'justlanded' ); ?></p>
                <p><?php _e( '<small>* SSL downloads must be enabled on your web server in order to retrieve automatic updates.</small>' , 'justlanded' ); ?></p>
            </div>
        </div>

        <div class="changelog">
            <h3 id="manual"><?php _e( 'Documentation' , 'justlanded' ); ?></h3>

            <div class="feature-section images-stagger-right">
                <h4><?php _e( 'Read the user manual and discover the whole potential' , 'justlanded'); ?></h4>
                <p><?php _e( 'JustLanded for WordPress comes with a fully-illustrated user manual. It is contained in the "Documentation" folder of the main file you can download from themeforest. The same folder also contains additional tutorials, quick guides and the manuals for all plug-ins we ship with the theme.' , 'justlanded' ); ?></p>
                <p><?php _e( 'Please remember that the "Main File" is not identical with the installable theme file, so you will have to download that file separately from themeforest. So please head over to <a href=" http://themeforest.net/downloads" target="_blank">themeforest\'s Downloads page</a> and click on the "Download" drop-down button to retrieve the "Main File".' , 'justlanded' ); ?></p>
            </div>

            <div class="feature-section col two-col">
                <div>
                    <h4><?php _e( 'First Steps'  , 'justlanded'); ?></h4>
                    <p><?php _e( 'Even if you do not like user manuals, we recommend that you read the <em>First Steps</em> chapter at the very least, as JustLanded has some very unique features.'  , 'justlanded'); ?></p>
                </div>
                <div class="last-feature">
                    <h4><?php _e( 'Quick Start Guide' , 'justlanded' ); ?></h4>
                    <p><?php _e( 'For experienced users, the <em>Quick Start Guide</em> provides brief instructions on how to install and update the theme, and how to install demo data.' , 'justlanded' ); ?></p>
                </div>
            </div>

        </div>

        <div class="changelog">
            <h3 id="support"><?php _e( 'Customer Support' , 'justlanded' ); ?></h3>

            <div class="feature-section col two-col">
                <div>
                    <h4><?php _e( 'Free E-Mail Support'  , 'justlanded'); ?></h4>
                    <p><?php _e( 'We exclusively provide customer support via e-mail, 7 days a week. Whenever you encounter an issue while working with the theme, please drop us a quick email to <a href="mailto:support@shapingrain.com">support@shapingrain.com</a>, explain your problem and we are happy to help.'  , 'justlanded'); ?></p>
                    <p><?php _e( 'Please note that we <em>do not</em> provide any customer support through the comments section on themeforest to ensure a consistent support experience.' , 'justlanded' ); ?></p>
                </div>
                <div class="last-feature">
                    <h4><?php _e( 'Support Policy' , 'justlanded' ); ?></h4>
                    <p><?php _e( 'We strive to offer the best customer support possible, at all times. Our support policy is published as part of the user manual and online. If you are unsure whether a particular issue is covered by our free customer support service, please review the <a href="http://themeforest.net/item/justlanded-wordpress-landing-page/3804089/support" target="_blank"> support policy</a>.' , 'justlanded' ); ?></p>
                </div>
            </div>

            <div class="feature-section col two-col">
                <div class="last-feature">
                    <h4><?php _e( 'Frequently Asked Questions' , 'justlanded' ); ?></h4>
                    <p><?php _e( 'Some of the most common questions and answers are published on our <a href="http://themeforest.net/item/justlanded-wordpress-landing-page/3804089/support" target="_blank"> support page</a>.' , 'justlanded' ); ?></p>
                </div>
            </div>


        </div>

        <div class="return-to-dashboard">
            <?php if ( current_user_can( 'update_core' ) && isset( $_GET['updated'] ) ) : ?>
                <a href="<?php echo esc_url( self_admin_url( 'update-core.php' ) ); ?>"><?php
                    is_multisite() ? _e( 'Return to Updates' ) : _e( 'Return to Dashboard &rarr; Updates' );
                    ?></a> |
            <?php endif; ?>
            <a href="<?php echo esc_url( self_admin_url() ); ?>"><?php
                is_blog_admin() ? _e( 'Go to Dashboard &rarr; Home' ) : _e( 'Go to Dashboard' ); ?></a>
        </div>

    </div>

<?php
}


/**
 * Get header classes
 *
 * @since 1.0.0
 */
function of_get_header_classes_array() 
{
	global $of_options;
	
	foreach ($of_options as $value) 
	{
		if ($value['type'] == 'heading' || $value['type'] == 'subheading')
			$hooks[] = str_replace(' ','',strtolower($value['name']));	
	}
	
	return $hooks;
}


/**
 * For use in themes
 *
 * @since forever
 */

$data = get_option(OPTIONSPREFIX.SITE_DEFAULT_PROFILE);

do_action('justlanded_after_options'); // custom hook