<?php
/*
Plugin Name: Landing Page Instance Selection Meta Box
Description: Allows the user to select an options instance of the landing page options to be associated with a page.
*/

add_action( 'add_meta_boxes', 'justlanded_meta_box_add_profile_select' );
if (!function_exists('justlanded_meta_box_add_profile_select')){
function justlanded_meta_box_add_profile_select()
{
	add_meta_box( 'justlanded-landingpage-select-instance', 'JustLanded: Landing Page Options', 'justlanded_meta_box_selectinstance', 'page', 'side', 'default' );
}
}

if (!function_exists('justlanded_meta_box_selectinstance')){
function justlanded_meta_box_selectinstance( $post )
{
	$values = get_post_custom( $post->ID );
	$selected = isset( $values['justlanded_meta_box_selectinstance_select'] ) ? esc_attr( $values['justlanded_meta_box_selectinstance_select'][0] ) : '';
    $checked  = isset( $values['justlanded_meta_box_parse_content'] ) ? esc_attr( $values['justlanded_meta_box_parse_content'][0] ) : '';
	wp_nonce_field( 'justlanded-landingpage-select_nonce', 'meta_box_nonce' );
	?>
    <h4>Select a page profile</h4>
	<p>
		This allows you to select an instance of your settings based on a custom set
		of options that should be applied to this page only.
	</p>
	
	<p>
		<select name="justlanded_meta_box_selectinstance_select" id="justlanded_meta_box_selectinstance_select">
            <option value="0" <?php selected( $selected, '0' ); ?>>Site Default</option>
            <?php
            for ($this_profile_count = 1; $this_profile_count <= MAXPROFILES; $this_profile_count++) {
            ?>
            <option value="<?php echo $this_profile_count; ?>" <?php selected($selected, $this_profile_count); ?>>Profile <?php echo $this_profile_count; ?><?php if (SITE_DEFAULT_PROFILE == $this_profile_count) echo " (current default)"; ?></option>
            <?php
            }
            ?>
		</select>
	</p>
    <h4>Parse Content</h4>
    <p>
        With this option you can instruct the theme to ignore the layout builder's settings and parse the page's content for
        shortcodes instead.
    </p>
    <p class="meta-options">
        <label for="justlanded_meta_box_parse_content" class="selectit"><input name="justlanded_meta_box_parse_content" type="checkbox" id="justlanded_meta_box_parse_content" value="1" <?php checked( $checked, '1' ); ?>> Parse and display page content</label>
     </p>
	<?php
}
}

add_action( 'save_post', 'justlanded_meta_box_save' );
if (!function_exists('justlanded_meta_box_save')){
function justlanded_meta_box_save( $post_id )
{
	// Bail if we're doing an auto save
	if( defined( 'DOING_AUTOSAVE' ) && DOING_AUTOSAVE ) return;
	
	// if our nonce isn't there, or we can't verify it, bail
	if( !isset( $_POST['meta_box_nonce'] ) || !wp_verify_nonce( $_POST['meta_box_nonce'], 'justlanded-landingpage-select_nonce' ) ) return;
	
	// if our current user can't edit this post, bail
	if( !current_user_can( 'edit_post' ) ) return;
	

	if( isset( $_POST['justlanded_meta_box_selectinstance_select'] ) ) {
		update_post_meta( $post_id, 'justlanded_meta_box_selectinstance_select', esc_attr( $_POST['justlanded_meta_box_selectinstance_select'] ) );
        update_post_meta( $post_id, 'justlanded_meta_box_parse_content', esc_attr( $_POST['justlanded_meta_box_parse_content'] ) );
    }
}
}
?>