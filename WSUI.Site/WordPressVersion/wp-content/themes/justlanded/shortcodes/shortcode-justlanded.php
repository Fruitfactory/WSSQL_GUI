<?php
/*
    layout blocks
*/
global $justlanded_modals;
global $justlanded_custom_blocks;


if (!function_exists('justlanded_clean_content')) {
function justlanded_clean_content($content)
{
    $content = preg_replace( '#^<\/p>|^<br \/>|<p>$#', '', $content );
    $content = preg_replace( '#<br \/>#', '', $content );
    $content = preg_replace( '#<p>|</p>#', '', $content );
    return do_shortcode( shortcode_unautop( trim( $content ) ) );
}
}

if (!function_exists('justlanded_shortcode_content_block')) {
    function justlanded_shortcode_content_block( $attrs, $content = null ) {
        global $used_blocks;
        $type = "page_content";
        if (isset($id) && $id != null && intval($id != 0)) {
            $this_block_id = $id;
        } else {
            if (!isset($used_blocks[$type])) $used_blocks[$type] = 1; else $used_blocks[$type]++;
            $this_block_id   = $used_blocks[$type];
        }
        $out  = '<div class="row">' . "\n";
        $out .= '  <section id="page-content-block-'.$this_block_id.'" class="page-content-block block">' . "\n";
        $out .=  trim(do_shortcode($content)) . "\n";
        $out .= '  </section>' . "\n";
        $out .= '</div>' . "\n";
        return $out;
    }
}

if (!function_exists('justlanded_custom_block')) {
	function justlanded_custom_block($attrs, $content = null ) {
		global $justlanded_custom_blocks;
		extract(shortcode_atts(array(
			'id' => null
		), $attrs));

		if ($id != null) $id = strtolower(trim($id));

		$justlanded_custom_blocks[$id] = justlanded_content_filters(trim($content));
		return false;
	}
}


if (!function_exists('justlanded_shortcode_one_half')) {
function justlanded_shortcode_one_half( $attrs, $content = null ) {
    extract(shortcode_atts(array(
    'last'	=> 'no',
    ), $attrs));
    $last_class = '';
    if ($last == 'yes') $last_class = ' last';
	$out =  '<div class="one_half'.$last_class.'">'.do_shortcode($content).'</div>';
    if ($last == 'yes') $out .= '<div class="clear"></div>'. "\n";
    return $out;
}
}

if (!function_exists('justlanded_shortcode_one_third')) {
function justlanded_shortcode_one_third( $attrs, $content = null ) {
    extract(shortcode_atts(array(
        'last'	=> 'no',
    ), $attrs));
    $last_class = '';

    if ($last == 'yes') $last_class = ' last';
    $out =  '<div class="one_third'.$last_class.'">'.do_shortcode($content).'</div>';
    if ($last == 'yes') $out .= '<div class="clear"></div>'. "\n";
    return $out;
}
}

if (!function_exists('justlanded_shortcode_two_thirds')) {
function justlanded_shortcode_two_thirds( $attrs, $content = null ) {
    extract(shortcode_atts(array(
        'last'	=> 'no',
    ), $attrs));
    $last_class = '';
    if ($last == 'yes') $last_class = ' last';
    $out =  '<div class="two_thirds'.$last_class.'">'.do_shortcode($content).'</div>';
    if ($last == 'yes') $out .= '<div class="clear"></div>'. "\n";
    return $out;
}
}

if (!function_exists('justlanded_shortcode_three_fourths')) {
function justlanded_shortcode_three_fourths($attrs, $content = null ) {
    extract(shortcode_atts(array(
        'last'	=> 'no',
    ), $attrs));
    $last_class = '';
    if ($last == 'yes') $last_class = ' last';
    $out =  '<div class="three_fourths'.$last_class.'">'.do_shortcode($content).'</div>';
    if ($last == 'yes') $out .= '<div class="clear"></div>'. "\n";
    return $out;
}
}

if (!function_exists('justlanded_shortcode_one_fourth')) {
function justlanded_shortcode_one_fourth($attrs, $content = null ) {
    extract(shortcode_atts(array(
        'last'	=> 'no',
    ), $attrs));
    $last_class = '';
    if ($last == 'yes') $last_class = ' last';
    $out =  '<div class="one_fourth'.$last_class.'">'.do_shortcode($content).'</div>';
    if ($last == 'yes') $out .= '<div class="clear"></div>'. "\n";
    return $out;
}
}

if (!function_exists('justlanded_shortcode_clearfix')) {
function justlanded_shortcode_clearfix($attrs, $content = null ) {
    $out = '<div class="clear"></div>';
    return $out;
}
}

/*
    list styles
*/
if (!function_exists('justlanded_shortcode_list')) {
function justlanded_shortcode_list($attrs, $content = null ) {
    extract(shortcode_atts(array(
        'style'	=> 'check',
    ), $attrs));
    $out =  '<div class="list_'.$style.'">'.do_shortcode(shortcode_unautop($content)).'</div>';
    return $out;
}
}

/*
    landing page blocks
*/

if (!function_exists('justlanded_landing_block')) {
function justlanded_landing_block($attrs, $content = null ) {
    global $data, $used_blocks;
    // $this_content, $this_atts, $this_block_type, $this_block_id,
    $this_atts = $attrs;
    extract(shortcode_atts(array(
        'type'	=> null,
        'id' => null
    ), $attrs));

    $type = strtolower($type);

    if (isset($id) && $id != null && intval($id != 0)) {
        $this_block_id = $id;
    } else {
        if (!isset($used_blocks[$type])) $used_blocks[$type] = 1; else $used_blocks[$type]++;
        $this_block_id   = $used_blocks[$type];
    }
    $this_block_type = $type;
    $this_content = $content;

    $block_tpl = TEMPLATEPATH . DIRECTORY_SEPARATOR . 'blocks' . DIRECTORY_SEPARATOR . 'block_' . $type . '.php'; // if block template exists, otherwise do nothing
    if (file_exists($block_tpl)) {
        $out = justlanded_get_block($block_tpl,
            array(
                'this_content' => $this_content,
                'this_atts' => $this_atts,
                'this_block_type' => $this_block_type,
                'this_block_id' => $this_block_id
            )
        );
    }
    else {
        // do nothing
        $out = "<p>No block '".$type."' found (".$block_tpl.")</p>";
    }
    return $out;
}
}

// misc utility shortcodes
if (!function_exists('justlanded_option')) {
function justlanded_option($attrs, $content = null ) {
    global $data;
    $this_atts = $attrs;
    extract(shortcode_atts(array(
        'key' => null
    ), $attrs));

    if ($key == null || !isset($data[$key])) return "";
    return $data[$key];
}
}

if (!function_exists('justlanded_video')) {
function justlanded_video($attrs, $content = null ) {
    $this_atts = $attrs;
    extract(shortcode_atts(array(
        'url' => null
    ), $attrs));
    return apply_filters('embed_oembed_html', wp_oembed_get(do_shortcode((stripslashes($url)))));
}
}

if (!function_exists('justlanded_pagetitle')) {
    function justlanded_pagetitle($attrs, $content = null ) {
		/*
        $title = get_the_title();
        return $title;
		*/
		global $justlanded_original_title;
		return $justlanded_original_title;
    }
}

if (!function_exists('justlanded_contentdummy')) {
    function justlanded_contentdummy($attrs, $content = null ) {
        $content = apply_filters('the_content', '<!-- Content -->');
    }
}

if (!function_exists('justlanded_dynamic_sidebar')) {
	function justlanded_dynamic_sidebar($attrs, $content = null ) {
		extract(shortcode_atts(array(
			'id' => null
		), $attrs));

		if ($id != null) {

			$output = '<div id="sidebar"><div class="widget-area"><ul class="sid">';

			ob_start();
			dynamic_sidebar($id);
			$output .= ob_get_clean();
			$output .= '</ul></div></div>';
			return $output;

		}
		return;
	}
}

if (!function_exists('justlanded_custom_menu')) {
	function justlanded_custom_menu($attrs, $content = null ) {
		extract(shortcode_atts(array(
			'id' => null
		), $attrs));

		if ($id != null) {
			$output = "";
			ob_start();
			wp_nav_menu( array( 'theme_location' => $id ) );
			$output .= ob_get_clean();
			return $output;
		}
		return;
	}
}


if (!function_exists('justlanded_shortcode_modal_window')) {
    function justlanded_shortcode_modal_window($attrs, $content = null ) {
        global $justlanded_modals;
        extract(shortcode_atts(array(
            'size'	=> 'large',
            'id' => null
        ), $attrs));

        if ($id != null) $id = strtolower(trim($id));

        $justlanded_modals[] = array(
            'id' => $id,
            'size' => $size,
            'content' => justlanded_content_filters(trim($content))
        );
        return false;
    }
}


if (!function_exists('justlanded_shortcode_modal_link')) {
    function justlanded_shortcode_modal_link($attrs, $content = null ) {
        extract(shortcode_atts(array(
            'id' => '1'
        ), $attrs));
        $id = strtolower(trim($id));
        return '<a href="javascript:void(0);" data-reveal-id="modal_custom_'.$id.'">' . do_shortcode($content) . '</a>';
    }
}

if (!function_exists('justlanded_output_modal_windows')) {
    function justlanded_output_modal_windows() {
        global $justlanded_modals;
        if (!is_array($justlanded_modals)) return;

        $output="";
        if (count($justlanded_modals > 0)) {
            $x=1;
            foreach ($justlanded_modals as $modal) {
                if ($modal['id'] == null) {
                    $modal['id'] = "modal_custom_" . $x;
                } else {
                    $modal['id'] = "modal_custom_" . $modal['id'];
                }
                $output .= "\n\n" . '<div id="'.$modal['id'].'" class="reveal-modal '.$modal['size'].'">' . "\n";
                $output .= do_shortcode($modal['content']) . "\n";
                $output .= '<a class="close-reveal-modal">&#215;</a>' . "\n" . '</div>' . "\n";
                $x++;
            }
        }

        echo $output;
    }
}
add_action('justlanded_after_body', 'justlanded_output_modal_windows');

// landing page blocks
add_shortcode('landing_block', 'justlanded_landing_block');
add_shortcode('landing_block_custom', 'justlanded_landing_block');
add_shortcode('content_block', 'justlanded_shortcode_content_block');
add_shortcode('custom_block', 'justlanded_custom_block');

// modal windows
add_shortcode('modal_window', 'justlanded_shortcode_modal_window');
add_shortcode('modal_link', 'justlanded_shortcode_modal_link');

// layout columns
add_shortcode('one_half', 'justlanded_shortcode_one_half');
add_shortcode('one_third', 'justlanded_shortcode_one_third');
add_shortcode('two_thirds', 'justlanded_shortcode_two_thirds');
add_shortcode('three_fourths', 'justlanded_shortcode_three_fourths');
add_shortcode('one_fourth', 'justlanded_shortcode_one_fourth');

add_shortcode('clearfix', 'justlanded_shortcode_clearfix');

// misc utility shortcodes
add_shortcode('justlanded_option', 'justlanded_option');
add_shortcode('justlanded_video', 'justlanded_video');
add_shortcode('justlanded_pagetitle', 'justlanded_pagetitle');
add_shortcode('justlanded_contentdummy', 'justlanded_contentdummy');
add_shortcode('justlanded_dynamic_sidebar', 'justlanded_dynamic_sidebar');
add_shortcode('justlanded_custom_menu', 'justlanded_custom_menu');

// list styles
add_shortcode('list', 'justlanded_shortcode_list');
