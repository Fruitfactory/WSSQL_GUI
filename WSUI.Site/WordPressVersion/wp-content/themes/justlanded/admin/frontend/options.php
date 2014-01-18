<div class="wrap" id="of_container">
	<div id="of-popup-save" class="of-save-popup">
		<div class="of-save-save">Options Updated</div>
	</div>
	
	<div id="of-popup-reset" class="of-save-popup">
		<div class="of-save-reset">Options Reset</div>
	</div>
	
	<div id="of-popup-fail" class="of-save-popup">
		<div class="of-save-fail">Error!</div>
	</div>

	<div id="of-popup-copy" class="of-save-popup">
		<div class="of-save-copy">Options Copied</div>
	</div>


	<span style="display: none;" id="hooks"><?php echo json_encode(of_get_header_classes_array()); ?></span>
	<input type="hidden" id="this_profile" value="<?php echo ACTIVEPROFILE_EDIT; ?>" />
	<input type="hidden" id="reset" value="<?php if(isset($_REQUEST['reset'])) echo $_REQUEST['reset']; ?>" />
	<input type="hidden" id="security" name="security" value="<?php echo wp_create_nonce('of_ajax_nonce'); ?>" />
	<form id="of_form" method="post" action="<?php echo esc_attr( $_SERVER['REQUEST_URI'] ) ?>" enctype="multipart/form-data" name="settings_profile_<?php echo ACTIVEPROFILE_EDIT; ?>_<?php echo mt_rand(0,999999); ?>" autocomplete="off">
		<div id="header">
			<div class="logo">
				<h2><?php echo THEMENAME; ?></h2>
				<span>v</span><span id="current_version"><?php echo (THEMEVERSION); ?></span>
                <span id="version_check"></span>
			</div>
			<div id="js-warning">Warning- This options panel will not work properly without javascript!</div>
            <img style="display:none" src="<?php echo ADMIN_DIR; ?>assets/images/loading-bottom.gif" class="ajax-loading-img-header" alt="Working..." />
			<div class="icon-option"></div>
			<div class="clear"></div>
    	</div>
		<div id="info_bar">
			<a>
				<div id="expand_options" class="expand">Expand</div>
			</a>
            <?php do_action("justlanded_options_after_expand"); ?>
            <a>
                <div id="show_hints" class="expand">Show Hints</div>
            </a>
            <?php do_action("justlanded_options_after_hints"); ?>
            <div id="select_profile">
                <select id="select_profile_value" name="select_profile_rnd_<?php echo mt_rand(0,999999); ?>">
                    <?php
                    for ($this_profile_count = 1; $this_profile_count <= MAXPROFILES; $this_profile_count++) {
                    ?>
                        <option value="<?php echo $this_profile_count; ?>" <?php selected(ACTIVEPROFILE_EDIT, $this_profile_count); ?>>Profile <?php echo $this_profile_count; ?><?php if (SITE_DEFAULT_PROFILE == $this_profile_count) echo " (default)"; ?></option>
                    <?php
                    }
                    ?>
                </select>
            </div>
            <?php do_action("justlanded_options_after_select_active_profile"); ?>
			<img style="display:none" src="<?php echo ADMIN_DIR; ?>assets/images/loading-bottom.gif" class="ajax-loading-img ajax-loading-img-bottom" alt="Working..." />
            <?php do_action("justlanded_options_before_save_button_top"); ?>
			<button id="of_save_top" type="button" class="button-primary button-save">
				<?php _e('Save All Changes', 'optionsframework');?>
			</button>
            <?php do_action("justlanded_options_after_save_button_top"); ?>
		</div><!--.info_bar-->
		<div id="main">
			<div id="of-nav">
				<ul>
				  <?php echo $options_machine->Menu ?>
				</ul>
			</div>
			<div id="content">
		  		<?php echo $options_machine->Inputs /* Settings */ ?>
		  	</div>
			<div class="clear"></div>
		</div>
		<div class="save_bar">
			<img style="display:none" src="<?php echo ADMIN_DIR; ?>assets/images/loading-bottom.gif" class="ajax-loading-img ajax-loading-img-bottom" alt="Working..." />
            <?php do_action("justlanded_options_before_save_button_bottom"); ?>
            <button id ="of_save_bottom" type="button" class="button-primary  button-save"><?php _e('Save All Changes', 'optionsframework');?></button>
            <?php do_action("justlanded_options_after_save_button_bottom"); ?>
			<button id ="of_reset" type="button" class="button submit-button reset-button" ><?php _e('Options Reset', 'optionsframework');?></button>
            <?php do_action("justlanded_options_after_reset_button"); ?>
			<img style="display:none" src="<?php echo ADMIN_DIR; ?>assets/images/loading-bottom.gif" class="ajax-reset-loading-img ajax-loading-img-bottom" alt="Working..." />
		</div><!--.save_bar-->
	</form>
	<div style="clear:both;"></div>
</div><!--wrap-->