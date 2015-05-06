<?php
/**
 * Options Framework Builder
 * Modified by ShapingRain.com, based on SMOF
 */

class Options_Machine
{

    function __construct($options)
    {
        $return = $this->optionsframework_machine($options);
        $this->Inputs = $return[0];
        $this->Menu = $return[1];
        $this->Defaults = $return[2];
    }

    /**
     * Process options data and build option fields
     *
     * @uses get_option()
     * @return array
     */
    public static function optionsframework_machine($options)
    {
        $data = get_option(OPTIONS);

        $defaults = array();
        $counter = 0;
        $parent_menu = '';
        $menu = '';
        $output = '';

        // populate web fonts array
        @$web_fonts_tmp = json_decode(file_get_contents(ADMIN_PATH . "defaults" . DIRECTORY_SEPARATOR . "google-webfonts.json"));
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
                'subsets' => $web_fonts_item->subsets,
            );
        }


        foreach ($options as $value) {
            $counter++;
            $val = '';

            //create array of defaults
            if ($value['type'] == 'multicheck') {
                if (is_array($value['std'])) {
                    foreach ($value['std'] as $i => $key) {
                        $defaults[$value['id']][$key] = true;
                    }
                } else {
                    $defaults[$value['id']][$value['std']] = true;
                }
            } else {
                if (isset($value['id']) && isset($value['std'])) $defaults[$value['id']] = $value['std'];
            }

            if (isset($value['std']) && !isset($data[$value['id']])) $data[$value['id']] = $value['std'];

            //Start Heading
            if ($value['type'] != "heading" && $value['type'] != "subheading") {
                $class = '';
                if (isset($value['class'])) {
                    $class = $value['class'];
                }

                //hide items in checkbox group
                $fold = '';
                if (array_key_exists("fold", $value)) {
                    if ($data[$value['fold']]) {
                        $fold = "f_" . $value['fold'] . " ";
                    } else {
                        $fold = "f_" . $value['fold'] . " temphide ";
                    }
                }

                if ($value['type'] != "sectionheader") {
                    $output .= '<div id="section-' . $value['id'] . '" class="' . $fold . 'section section-' . $value['type'] . ' ' . $class . '">' . "\n";
                }
                else {
                    $output .= '<div class="' . $fold . 'section section-' . $value['type'] . ' ' . $class . '">' . "\n";
                }

                //only show header if 'name' value exists
                if ($value['name']) {
                    if ($value['type'] == "section") {
                        $output .= '<h3 class="heading-section">' . $value['name'] . '</h3>' . "\n";
                    } else {
                        if (isset($value['descposition']) && @$value['descposition'] == "heading" && isset($value['desc'])) {
                            $output .= '<h3 class="heading">' . $value['name'] . ' <span class="explain">' . $value['desc'] . '</span></h3>' . "\n";
                        } else {
                            if ($value['type'] != "news") {
	                            $experimental = "";
	                            if (isset($value['experimental'])) $experimental = ' <span class="experimental">Experimental</span>';
                                $output .= '<h3 class="heading">' . $value['name'] . $experimental . '</h3>' . "\n";
                            }
                        }
                    }

                    if (isset($value['descposition']) && isset($value['desc'])) {
                        if ($value['descposition'] == "top") {
                            $output .= '<div class="explain">' . $value['desc'] . '</div>';
                        }
                    }


                }

                $temp_controls_class = '';
                if (@$value['type'] != "news" && @$value['type'] != "info" && @$value['type'] != "faq") $temp_controls_class = " can_toggle";
                $output .= '<div class="option of-option-' . $value['type'] . '">' . "\n" . '<div class="controls' . $temp_controls_class . '">' . "\n";

            }
            //End Heading

            if (isset($value['global'])) {
                $gclass = " global";
                if (isset($value['id'])) $data[$value['id']] = get_option(THEMENAME.'_'.$value['id']);
            } else {
                $gclass = "";
            }


            //switch statement to handle various options type
            switch ($value['type']) {

                //preset
                case 'preset':
                    // dynamically load presets

                    $preset_groups = array(
                        "gradient" => "Color Gradients",
                        // "color" => "Solid Colors",
                        "image" => "Images",
                        "texture" => "Textures"
                    );

                    $preset_dir = get_template_directory() . DIRECTORY_SEPARATOR . 'admin' . DIRECTORY_SEPARATOR . 'presets' . DIRECTORY_SEPARATOR . $value['folder'];
                    $presets = array();
                    if ($handle = opendir($preset_dir)) {
                        while (false !== ($entry = readdir($handle))) {
                            if (substr_count($entry, '.txt') > 0) {
                                $preset = file($preset_dir . DIRECTORY_SEPARATOR . $entry);
                                $swatch = explode(",", $preset[3]);
                                $preset_title = trim($preset[0]);
                                $preset_id = trim(strtolower(str_replace(" ", "", trim($preset_title))));
                                $preset_file = trim(str_replace(".txt", "", $entry));
                                $preset_author = trim($preset[1]);
                                $preset_data_enc = trim(base64_decode($preset[2]));
                                $preset_data = json_decode($preset_data_enc);
                                $preset_group = strtolower(trim($preset[4]));
                                $presets[] = array(
                                    'title' => $preset_title,
                                    'id' => $preset_id,
                                    'file' => $preset_file,
                                    'group' => $preset_group,
                                    'author' => $preset_author,
                                    'swatch' => $swatch,
                                );
                            }
                        }
                        closedir($handle);
                    }
                    foreach ($preset_groups as $group_short => $group_title) {
                        $output .= '<h4>' . $group_title . '</h4>' . "\n";
                        $output .= '<div class="preset_group">' . "\n";

                        foreach ($presets as $preset) {
                            if ($preset['group'] == $group_short) {
                                if ($group_short == "image" || $group_short == "texture") {
                                    $swatch_image = " swatch-image";
                                    $swatch_image_file = ' style="background:url('.get_template_directory_uri().'/admin/presets/previews/'.$preset['file'].'.jpg);")';
                                }
                                else {
                                    $swatch_image = "";
                                    $swatch_image_file = "";
                                }
                                $output .= '<div class="of-presets">
                                    <div class="preset preset-'.$preset['group'].'" id="'.$preset['file'].'" title="' . $preset['title'] . ' by ' . $preset['author'] . '">
                                        <div class="preset-title" title="' . $preset['title'] . ' by ' . $preset['author'] . '"><span>' . $preset['title'] . '</span></div>
                                        <div class="swatch swatch-1'.$swatch_image.'" title="' . $preset['swatch'][0] . '"'.$swatch_image_file.'></div>
                                        <div class="swatch swatch-2" title="' . $preset['swatch'][1] . '"></div>
                                        <div class="swatch swatch-3" title="' . $preset['swatch'][2] . '"></div>
                                        <div class="swatch swatch-4" title="' . $preset['swatch'][3] . '"></div>
                                        <div class="swatch swatch-5" title="' . $preset['swatch'][4] . '"></div>
                                    </div>
                                </div>';
                            }
                        }
                        $output .= '<div class="clear"></div></div>' . "\n";
                    }

                    break;

                //text input
                case 'text':
                    $t_value = '';
                    if (isset($data[$value['id']])) $t_value = stripslashes($data[$value['id']]); else $t_value = "";
                    $t_value = esc_attr($t_value);

                    $mini = '';
                    if (!isset($value['mod'])) $value['mod'] = '';
                    if ($value['mod'] == 'mini') {
                        $mini = 'mini';
                    }

                    $output .= '<input class="of-input ' . $mini . $gclass . '" name="' . $value['id'] . '" id="' . $value['id'] . '" type="' . $value['type'] . '" value="' . $t_value . '" />';
                    break;

                //select option
                case 'select':
                    $mini = '';
                    if (!isset($value['storage'])) $value['storage'] = 'text';

                    if (!isset($value['mod'])) $value['mod'] = '';
                    if ($value['mod'] == 'mini') {
                        $mini = 'mini';
                    }
                    $output .= '<div class="' . $mini . '">';
                    $output .= '<select class="select of-input'.$gclass.'" name="' . $value['id'] . '" id="' . $value['id'] . '">';

                    if ($value['storage'] == 'text') {
                        foreach ($value['options'] as $select_ID => $option) {
                            $output .= '<option id="' . $value['id'] . '_' . $select_ID . '" value="' . $option . '" ' . selected($data[$value['id']], $option, false) . ' />' . $option . '</option>';
                        }
                    } else {
                        if (!isset($select_ID)) $select_ID="";
                        $output .= '<option id="' . $value['id'] . '_zero' . $select_ID . '" value="0" ' . selected($data[$value['id']], 0, false) . ' />---- Please select an option ----</option>';
                        foreach ($value['options'] as $select_ID => $option) {
                            $output .= '<option id="' . $value['id'] . '_' . $select_ID . '" value="' . $select_ID . '" ' . selected($data[$value['id']], $select_ID, false) . ' />' . $option . '</option>';
                        }
                    }
                    $output .= '</select></div>';
                    break;

                //textarea option
                case 'textarea':
                    $cols = '8';
                    $ta_value = '';

                    if (isset($value['options'])) {
                        $ta_options = $value['options'];
                        if (isset($ta_options['cols'])) {
                            $cols = $ta_options['cols'];
                        }
                    }

                    if (isset($data[$value['id']])) $ta_value = $data[$value['id']]; else $ta_value = "";
                    $output .= '<textarea class="of-input'.$gclass.'" name="' . $value['id'] . '" id="' . $value['id'] . '" cols="' . $cols . '" rows="8">' . esc_textarea($ta_value) . '</textarea>';
                    break;

                //radiobox option
                case 'radio':

                    $x=0;
                    foreach ($value['options'] as $option => $name) {
                        if (!isset($value['id']) && isset($value['name'])) $value['id'] = strtolower(str_replace(" ", "", $value['name']));
                        $output .= '<input class="of-input of-radio'.$gclass.'" name="' . $value['id'] . '" id="' . $value['id'] . "_" . $x . '" type="radio" value="' . $option . '" ' . checked($data[$value['id']], $option, false) . ' /><label for="' . $value['id'] . "_" . $x . '" class="radio">' . $name . '</label><br/>';
                        $x++;
                    }
                    break;

                //checkbox option
                case 'checkbox':
                    if (!isset($data[$value['id']])) {
                        $data[$value['id']] = 0;
                    }

                    $fold = '';
                    if (array_key_exists("folds", $value)) $fold = "fld ";

                    $output .= '<input type="hidden" class="' . $fold . 'checkbox aq-input'.$gclass.'" name="' . $value['id'] . '" id="' . $value['id'] . '" value="0"/>';
                    $output .= '<input type="checkbox" class="' . $fold . 'checkbox of-input'.$gclass.'" name="' . $value['id'] . '" id="' . $value['id'] . '" value="1" ' . checked($data[$value['id']], 1, false) . ' />';
                    break;

                //multiple checkbox option
                case 'multicheck':
                    $multi_stored = $data[$value['id']];

                    foreach ($value['options'] as $key => $option) {
                        if (!isset($multi_stored[$key])) {
                            $multi_stored[$key] = '';
                        }
                        $of_key_string = $value['id'] . '_' . $key;
                        $output .= '<input type="checkbox" class="checkbox of-input'.$gclass.'" name="' . $value['id'] . '[' . $key . ']' . '" id="' . $of_key_string . '" value="1" ' . checked($multi_stored[$key], 1, false) . ' /><label class="multicheck" for="' . $of_key_string . '">' . $option . '</label><br />';
                    }
                    break;

                //ajax image upload option
                case 'upload':
                    if (!isset($value['mod'])) $value['mod'] = '';
                    $output .= Options_Machine::optionsframework_uploader_function($value['id'], $value['std'], $value['mod']);
                    break;

                // native media library uploader - @uses optionsframework_media_uploader_function()
                case 'media':
                    $_id = strip_tags(strtolower($value['id']));
                    $int = '';
                    $int = optionsframework_mlu_get_silentpost($_id);
                    if (!isset($value['mod'])) $value['mod'] = '';
                    $output .= Options_Machine::optionsframework_media_uploader_function($value['id'], $value['std'], $int, $value['mod']); // New AJAX Uploader using Media Library
                    break;

                //colorpicker option
                case 'color':
                    $output .= '<input class="miniColors of-color'.$gclass.'" name="' . $value['id'] . '" id="' . $value['id'] . '" type="text" value="' . $data[$value['id']] . '" />';
                    break;

                //gradient option
                case 'gradient':
                    $gradient_stored = isset($data[$value['id']]) ? $data[$value['id']] : $value['std'];
                    $output .= '<input class="miniColors of-gradient'.$gclass.'" name="' . $value['id'] . '[start]" id="' . $value['id'] . '_start" type="text" value="' . $gradient_stored['start'] . '" />';
                    $output .= '<input class="miniColors of-gradient'.$gclass.'" name="' . $value['id'] . '[end]" id="' . $value['id'] . '_end" type="text" value="' . $gradient_stored['end'] . '" />';
                    break;


                //typography option
                case 'typography':
                    $typography_stored = isset($data[$value['id']]) ? $data[$value['id']] : $value['std'];

                    /* Font Size */
                    if (isset($typography_stored['size'])) {
                        if (isset($typography_stored['unit'])) {
                            if ($typography_stored['unit'] == "px") {
                                $unit_values = array(8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 29, 32, 35, 36, 37, 38, 40, 42, 45, 48, 50, 52, 54, 56, 58, 60, 61, 62);
                            } elseif ($typography_stored['unit'] == "em") {
                                $unit_values = array(.5, .55, .625, .7, .75, .8, .875, .95, 1, 1.05, 1.125, 1.2, 1.25, 1.3, 1.4, 1.45, 1.5, 1.6, 1.8, 2, 2.2, 2.25, 2.3, 2.35, 2.45, 2.55, 2.75, 3, 3.13, 3.25, 3.38, 3.5, 3.63, 3.75, 3.81, 3.88);
                            }
                        }

                        $output .= '<div class="typography-size" original-title="Font size">';
                        $output .= '<select class="of-typography of-typography-size select'.$gclass.'" name="' . $value['id'] . '[size]" id="' . $value['id'] . '_size">';
                        foreach ($unit_values as $i) {
                            $test = $i;
                            $output .= '<option value="' . $i . '" ' . selected($typography_stored['size'], $test, false) . '>' . $i . '</option>';
                        }
                        $output .= '</select></div>';

                    }

                    /* Font Size Unit */
                    if (isset($typography_stored['unit'])) {
                        $output .= '<div class="typography-unit" original-title="Font size unit">';
                        $output .= '<select class="of-typography of-typography-unit select'.$gclass.'" name="' . $value['id'] . '[unit]" id="' . $value['id'] . '_unit">';
                        $output .= '<option value="px" ' . selected($typography_stored['unit'], "px", false) . '>px</option>';
                        $output .= '<option value="em" ' . selected($typography_stored['unit'], "em", false) . '>em</option>';
                        $output .= '</select></div>';
                    }

                    /* Line Height */
                    if (isset($typography_stored['height'])) {

                        $output .= '<div class="typography-height" original-title="Line height">';
                        $output .= '<select class="of-typography of-typography-height select'.$gclass.'" name="' . $value['id'] . '[height]" id="' . $value['id'] . '_height">';
                        for ($i = 20; $i < 38; $i++) {
                            $test = $i . 'px';
                            $output .= '<option value="' . $i . 'px" ' . selected($typography_stored['height'], $test, false) . '>' . $i . 'px</option>';
                        }
                        $output .= '</select></div>';
                    }

                    /* Font Face */
                    if (isset($typography_stored['face'])) {

                        $output .= '<div class="typography-face" original-title="Font family">';
                        $output .= '<select class="of-typography of-typography-face select'.$gclass.'" name="' . $value['id'] . '[face]" id="' . $value['id'] . '_face">';

                        /*
						$faces = array('arial'=>'Arial',
										'verdana'=>'Verdana, Geneva',
										'trebuchet'=>'Trebuchet',
										'georgia' =>'Georgia',
										'times'=>'Times New Roman',
										'tahoma'=>'Tahoma, Geneva',
										'palatino'=>'Palatino',
										'helvetica'=>'Helvetica' );
                        */

                        foreach ($web_fonts as $i => $face) {
                            $output .= '<option value="' . $i . '" ' . selected($typography_stored['face'], $i, false) . '>' . $i . '</option>';
                        }

                        $output .= '</select></div>';

                    }

                    /* Font Weight */
                    if (isset($typography_stored['style'])) {

                        $output .= '<div class="typography-style" original-title="Font style">';
                        $output .= '<select class="of-typography of-typography-style select'.$gclass.'" name="' . $value['id'] . '[style]" id="' . $value['id'] . '_style">';

                        if (isset($web_fonts[$typography_stored['face']]['variants'])) @$styles = $web_fonts[$typography_stored['face']]['variants']; else $styles = array();
                        if (count($styles) > 0) {
                            foreach ($styles as $i => $style) {

                                $output .= '<option value="' . $style . '" ' . selected($typography_stored['style'], $style, false) . '>' . $style . '</option>';
                            }
                        }
                        $output .= '</select></div>';

                    }

                    /* Font Color */
                    if (isset($typography_stored['color'])) {
                        $output .= '<input class="miniColors of-color of-typography of-typography-color'.$gclass.'" original-title="Font color" name="' . $value['id'] . '[color]" id="' . $value['id'] . '_color" type="text" value="' . $typography_stored['color'] . '" />';
                    }

                    if (isset($typography_stored['subset'])) {
                        //'<input type="checkbox" class="checkbox of-input" name="'+this_base_id+'[subset]['+val+']" id="'+this_base_id+'_subset_'+val+'" value="1" checked="checked"><label class="multicheck" for="'+this_base_id+'_subset_'+val+'">'+val+'</label>'
                    }

                    $subset_part = "";
                    $subsets = array();
                    if (isset($typography_stored['face']) && isset($web_fonts[$typography_stored['face']]['subsets'])) $subsets = $web_fonts[$typography_stored['face']]['subsets'];
                    if (count($subsets) > 1) {
                        foreach ($subsets as $subset) {
                            $subset_checked = '';
                            if (isset($typography_stored['subset'][$subset]) && $typography_stored['subset'][$subset] == 1) $subset_checked = ' checked="checked"';
                            $subset_part .= '<input type="checkbox" class="checkbox of-input'.$gclass.'" name="' . $value['id'] . '[subset][' . $subset . ']" id="' . $value['id'] . '_subset_' . $subset . '" value="1"' . $subset_checked . '><label class="multicheck" for="' . $value['id'] . '_subset_' . $subset . '">' . $subset . '</label> ';
                        }
                    }
                    $output .= '<div class="controls typography-subset">';
                    $output .= '<div class="of-typography of-typography-subset'.$gclass.'" id="' . $value['id'] . '_subset">' . $subset_part . '</div>';
                    $output .= '</div>';


					if (JUSTLANDED_OPTIONS_FONT_PREVIEW == true) {
						$output .= '<div class="controls">';
						$output .= '<div class="of-typography of-typography-preview'.$gclass.'" id="' . $value['id'] . '_preview">Bright vixens jump; dozy fowl quack.</div>';
						$output .= '</div>';
					}

                    break;

                //border option
                case 'border':

                    /* Border Width */
                    $border_stored = $data[$value['id']];

                    $output .= '<div class="border-width">';
                    $output .= '<select class="of-border of-border-width select'.$gclass.'" name="' . $value['id'] . '[width]" id="' . $value['id'] . '_width">';
                    for ($i = 0; $i < 21; $i++) {
                        $output .= '<option value="' . $i . '" ' . selected($border_stored['width'], $i, false) . '>' . $i . '</option>';
                    }
                    $output .= '</select></div>';

                    /* Border Style */
                    $output .= '<div class="border-style">';
                    $output .= '<select class="of-border of-border-style select'.$gclass.'" name="' . $value['id'] . '[style]" id="' . $value['id'] . '_style">';

                    $styles = array('none' => 'None',
                        'solid' => 'Solid',
                        'dashed' => 'Dashed',
                        'dotted' => 'Dotted');

                    foreach ($styles as $i => $style) {
                        $output .= '<option value="' . $i . '" ' . selected($border_stored['style'], $i, false) . '>' . $style . '</option>';
                    }

                    $output .= '</select></div>';

                    /* Border Color */
                    $output .= '<input class="miniColors of-color of-border of-border-color'.$gclass.'" name="' . $value['id'] . '[color]" id="' . $value['id'] . '_color" type="text" value="' . $border_stored['color'] . '" />';

                    break;

                //images checkbox - use image as checkboxes
                case 'images':

                    $i = 0;
                    if (isset($data[$value['id']])) $select_value = $data[$value['id']]; else $select_value = "";
                    foreach ($value['options'] as $key => $option) {
                        $i++;

                        $checked = '';
                        $selected = '';
                        if (NULL != checked($select_value, $key, false)) {
                            $checked = checked($select_value, $key, false);
                            $selected = 'of-radio-img-selected';
                        }
                        $output .= '<span>';
                        $output .= '<input type="radio" id="of-radio-img-' . $value['id'] . $i . '" class="checkbox of-radio-img-radio'.$gclass.'" value="' . $key . '" name="' . $value['id'] . '" ' . $checked . ' />';
                        $output .= '<div class="of-radio-img-label">' . $key . '</div>';
                        $output .= '<img src="' . $option . '" alt="" class="of-radio-img-img ' . $selected . '" onClick="document.getElementById(\'of-radio-img-' . $value['id'] . $i . '\').checked = true;" />';
                        $output .= '</span>';
                    }
                    break;


                //info (for small intro box etc)
                case "info":
                    $info_text = $value['std'];
                    $output .= '<div class="of-info">' . $info_text . '</div>';
                    break;

                //news (via javascript, remote query via shapingrain.com api server)
                case "news":
                    $info_text = $value['std'];
                    $output .= '<div class="of-info" id="newsblock">' . $info_text . '</div>';
                    break;

                //faq (via javascript, remote query via shapingrain.com api server)
                case "faq":
                    $info_text = $value['std'];
                    $output .= '<div class="of-info" id="faqblock">' . $info_text . '</div>';
                    break;

                //shortcode help
                case "shortcodes":
                    $shortcodes = $value['std'];
                    $shortcodes_output = "";
                    foreach ($shortcodes as $shortcode)
                    {
                        $shortcodes_output .= '<div class="shortcode_block">';
                        $shortcodes_output .= '<span>'.$shortcode['desc'].'</span>';
                        if ($shortcode['type'] == "line") {
                            $shortcodes_output .= '<input class="shortcode-input" value="'.esc_attr($shortcode['code']).'">';
                        }
                        elseif ($shortcode['type'] == "textarea") {
                            $shortcodes_output .= '<textarea rows="6" class="shortcode-input">'.esc_attr($shortcode['code']).'</textarea>';
                        }
                        $shortcodes_output .= '</div>';

                    }
                    $output .= '<div class="of-shortcode">'.$shortcodes_output.'</div>';
                    break;



                //display a single image
                case "image":
                    $src = $value['std'];
                    $output .= '<img src="' . $src . '">';
                    break;

                //tab heading
                case 'heading':
                    $redirect = '';
                    if (isset($value['redirect'])) $redirect = ' class="of-redirect"';
                    $parent_menu = str_replace(' ', '', strtolower($value['name']));
                    if ($counter >= 2) {
                        $output .= '</div>' . "\n";
                    }
                    $header_class = str_replace(' ', '', strtolower($value['name']));
                    $jquery_click_hook = str_replace(' ', '', strtolower($value['name']));
                    $jquery_click_hook = "of-option-" . $jquery_click_hook;
                    $menu .= '<li class="' . $header_class . '"><a title="' . $value['name'] . '" href="#' . $jquery_click_hook . '"' . $redirect . '>' . $value['name'] . '</a></li>';
                    $output .= '<div class="group" id="' . $jquery_click_hook . '"><h2>' . $value['name'] . '</h2>' . "\n";
                    break;

                case 'subheading':
                    if ($counter >= 2) {
                        $output .= '</div>' . "\n";
                    }
                    $header_class = str_replace(' ', '', strtolower($value['name']));
                    $jquery_click_hook = str_replace(' ', '', strtolower($value['name']));
                    $jquery_click_hook = "of-option-" . $jquery_click_hook;
                    $menu .= '<li class="' . $header_class . ' of-submenu of-parent-' . $parent_menu . ' menuhide"><a title="' . $value['name'] . '" href="#' . $jquery_click_hook . '">' . $value['name'] . '</a></li>';
                    //$info_text = $value['std'];
                    //if ($info_text != "") $output .= '<div class="of-section-header">'.$info_text.'</div>';
                    $output .= '<div class="group subgroup" id="' . $jquery_click_hook . '"><h2>' . $value['name'] . '</h2>' . "\n";
                    break;

                case 'sectionheader':
                    if (isset($value['std'])) $info_text = $value['std']; else $info_text = "";
                    if ($info_text != "") $output .= '<div class="of-section-header"><h2>' . $info_text . '</h2></div>';
                    break;


                //drag & drop slide manager and generic draggable items
                case 'slider':
                    $_id = strip_tags(strtolower($value['id']));

                    $feature_classes = "";
                    if (isset($value['has_link']) && @$value['has_link'] == true || !isset($value['has_link'])) {
                        $feature_classes .= " has_link ";
                    }
                    if (isset($value['has_description']) && @$value['has_description'] == true || !isset($value['has_description'])) {
                        $feature_classes .= " has_description ";
                    }
                    if (isset($value['has_title']) && @$value['has_title'] == true || !isset($value['has_title'])) {
                        $feature_classes .= " has_title ";
                    }
                    if (isset($value['has_title_testimonial']) && @$value['has_title_testimonial'] == true) {
                        $feature_classes .= " has_title_testimonial ";
                    }
                    if (isset($value['has_subtitle']) && @$value['has_subtitle'] == true) {
                        $feature_classes .= " has_subtitle ";
                    }
                    if (isset($value['has_subtitle_testimonial']) && @$value['has_subtitle_testimonial'] == true) {
                        $feature_classes .= " has_subtitle_testimonial ";
                    }
                    if (isset($value['has_image']) && @$value['has_image'] == true || !isset($value['has_image'])) {
                        $feature_classes .= " has_image ";
                    }
                    if (isset($value['has_price']) && @$value['has_price'] == true) {
                        $feature_classes .= " has_price ";
                    }
                    if (isset($value['has_badge']) && @$value['has_badge'] == true) {
                        $feature_classes .= " has_badge ";
                    }
                    if (isset($value['has_highlight']) && @$value['has_highlight'] == true) {
                        $feature_classes .= " has_highlight ";
                    }

                    $feature_classes = str_replace("  ", " ", $feature_classes);
                    $feature_classes = trim($feature_classes);

                    $int = '';
                    $int = optionsframework_mlu_get_silentpost($_id);
                    $output .= '<div class="slider"><ul id="' . $value['id'] . '" rel="' . $int . '" class="' . $feature_classes . '">';
                    $slides = $data[$value['id']];
                    $count = count($slides);
                    if ($count < 2) {
                        $oldorder = 1;
                        $order = 1;
                        $output .= Options_Machine::optionsframework_slider_function($value['id'], $value['std'], $oldorder, $order, $int, $value);
                    } else {
                        $i = 0;
                        foreach ($slides as $slide) {
                            $oldorder = $slide['order'];
                            $i++;
                            $order = $i;
                            $output .= Options_Machine::optionsframework_slider_function($value['id'], $value['std'], $oldorder, $order, $int, $value);
                        }
                    }
                    $output .= '</ul>';
                    $output .= '<a href="#" class="button slide_add_button">Add New ' . $value['itemtype'] . '</a></div>';
                    break;

                //drag & drop block manager
                case 'sorter':
                    if (isset($data[$value['id']])) $sortlist_isset = true; else $sortlist_isset = false;
                    $sortlists = isset($data[$value['id']]) && !empty($data[$value['id']]) ? $data[$value['id']] : $value['std'];
                    if ($sortlist_isset == true && is_array($sortlists) && isset($sortlists['enabled']) && isset($sortlists['disabled'])) {
                        // defaults for this control
                        $sortlist_defaults = array();
                        foreach ($value['std']['enabled'] as $skey => $sval) {
                            if ($skey != "placebo") $sortlist_defaults[$skey] = $sval;
                        }
                        foreach ($value['std']['disabled'] as $skey => $sval) {
                            if ($skey != "placebo") $sortlist_defaults[$skey] = $sval;
                        }
                        $sortlist_defaults = array_unique($sortlist_defaults);

                        // currently set options
                        $sortlist_active = array();
                        foreach ($sortlists['enabled'] as $skey => $sval) {
                            if ($skey != "placebo") $sortlist_active[$skey] = $sval;
                        }
                        foreach ($sortlists['disabled'] as $skey => $sval) {
                            if ($skey != "placebo") $sortlist_active[$skey] = $sval;
                        }
                        $sortlist_active = array_unique($sortlist_active);


                        // add elements to disabled block that did not exist before an update
                        foreach ($sortlist_defaults as $skey => $sval) {
                            if (!array_key_exists($skey, $sortlists['enabled']) && !array_key_exists($skey, $sortlists['disabled'])) {
                                $sortlists['disabled'][$skey] = $sval;
                            }
                        }

                        // remove elements that have been removed with an update
                        foreach ($sortlist_active as $skey => $sval) {
                            if (!array_key_exists($skey, $sortlist_defaults)) {
                                if(array_key_exists($skey, $sortlists['enabled'])) unset ($sortlists['enabled'][$skey]);
                                if(array_key_exists($skey, $sortlists['disabled'])) unset ($sortlists['disabled'][$skey]);
                            }
                        }
                    }

                    $output .= '<div id="' . $value['id'] . '" class="sorter">';

                    if ($sortlists) {
                        foreach ($sortlists as $group => $sortlist) {
                            $output .= '<ul id="' . $value['id'] . '_' . $group . '" class="sortlist_' . $value['id'] . '">';
                            $output .= '<h3>' . $group . '</h3>';

                            foreach ($sortlist as $key => $list) {

                                $output .= '<input class="sorter-placebo" type="hidden" name="' . $value['id'] . '[' . $group . '][placebo]" value="placebo">';

                                if ($key != "placebo") {

                                    $output .= '<li id="' . $key . '" class="sortee">';
                                    $output .= '<input class="position" type="hidden" name="' . $value['id'] . '[' . $group . '][' . $key . ']" value="' . $list . '">';
                                    $output .= $list;
                                    $output .= '</li>';
                                }
                            }
                            $output .= '</ul>';
                        }
                    }

                    $output .= '</div>';
                    break;

                //background images option
                case 'tiles':

                    $i = 0;
                    $select_value = isset($data[$value['id']]) && !empty($data[$value['id']]) ? $data[$value['id']] : '';

                    foreach ($value['options'] as $key => $option) {
                        $i++;

                        $checked = '';
                        $selected = '';
                        if (NULL != checked($select_value, $option, false)) {
                            $checked = checked($select_value, $option, false);
                            $selected = 'of-radio-tile-selected';
                        }
                        $output .= '<span>';
                        $output .= '<input type="radio" id="of-radio-tile-' . $value['id'] . $i . '" class="checkbox of-radio-tile-radio'.$gclass.'" value="' . $option . '" name="' . $value['id'] . '" ' . $checked . ' />';
                        $output .= '<div class="of-radio-tile-img ' . $selected . '" style="background: url(' . $option . ')" onClick="document.getElementById(\'of-radio-tile-' . $value['id'] . $i . '\').checked = true;"></div>';
                        $output .= '</span>';
                    }

                    break;

                // import demo data
                case 'demo':
                    $output .= '<div class="backup-box">';
                    $output .= '<a href="#" id="of_demo" class="button-secondary" title="Import Demo Data">Import Demo Data</a>&nbsp;';
                    if (OF_DEBUG_MODE == TRUE) {
                        $output .= '<a href="#" id="of_demo_save" class="button-secondary" title="Import Demo Data">Write Demo Data</a>';
                    }
                    $output .= '</div>';
                    break;

                case 'master_export':
                    $output .= '<div class="backup-box">';
                    $output .= '<a href="#" id="of_master_export" class="button-secondary" title="Download">Download Export File</a>&nbsp;';

                    @$import_file = ADMIN_PATH."defaults".DIRECTORY_SEPARATOR."justlanded_export.txt";
                    if (file_exists($import_file)) {
                        $output .= '<a href="#" id="of_master_import" class="button-primary" title="Import File">Import File</a>';
                    }

                    $output .= '</div>';
                    break;


                //site default settings profile
                case 'profile':

					if (isset($value['source']) && $value['source'] == 'blog_default') {
						$source_profile_default = BLOG_DEFAULT_PROFILE;
					} else {
						$source_profile_default = SITE_DEFAULT_PROFILE;
					}


                    $of_options_profiles = array();
                    for ($this_profile_count = 1; $this_profile_count <= MAXPROFILES; $this_profile_count++) {
                        $of_options_profiles[$this_profile_count] = "Profile " . $this_profile_count;
                    }

                    $x=0;
					$output .= '<select name="' . $value['id'] . '" id="' . $value['id'] . "_" . $x . '">';

					if ( isset($value['std']) && $value['std'] == 0) {
						$output .= '<option  value="0" ' . selected($source_profile_default, 0, false) . ' />Use Site Default Profile</option>';
					}

                    foreach ($of_options_profiles as $option => $name) {
                        if (!isset($value['id']) && isset($value['name'])) $value['id'] = strtolower(str_replace(" ", "", $value['name']));
                        $output .= '<option  value="' . $option . '" ' . selected($source_profile_default, $option, false) . ' />' . $name . '</option>';
                        $x++;
                    }
					$output .= '</select>';
                    break;

                //site default settings profile
                case 'profile_allocation':
                    $args = array(
                        'sort_order' => 'ASC',
                        'sort_column' => 'post_title',
                        'post_type' => 'page',
                        'post_status' => 'publish,private'
                    );
                    $tmp_pages = get_pages($args);

                    $all_pages = array();
					if (is_array($tmp_pages)) {
						foreach ($tmp_pages as $page) {
							$id = $page->ID;
							$title = $page->post_title;
							$page_meta = get_post_meta( $id );
							$set_profile = get_post_meta($id, 'justlanded_meta_box_selectinstance_select', true);
							$all_pages[$set_profile][] = Array(
								'title' => $title,
								'id' => $id,
								'profile' => $set_profile
							);
						}

						for ($this_profile_count = 1; $this_profile_count <= MAXPROFILES; $this_profile_count++) {
							if (isset($all_pages[$this_profile_count])) {
								$output .= '<p><strong>Profile '.$this_profile_count.'</strong>';
								$output .= '<ul class="profile_allocation">';
								foreach ($all_pages[$this_profile_count] as $this_page)
								{
									$output .= '<li><a href="'.get_admin_url().'post.php/?post='.$this_page['id'].'&action=edit">'.$this_page['title'].'</a> ('.$this_page['id'].')</li>';
								}
								$output .= '</ul>';
								$output .= '</p>';
							}
						}
					} else {
						$output .= '<p>There are currently no pages associated with a profile.</p>';
					}
                    break;

                //backup and restore options data
                case 'backup':

                    $instructions = $value['desc'];
                    $backup = get_option(BACKUPS);

                    if (!isset($backup['backup_log'])) {
                        $log = 'No backups yet';
                    } else {
                        $log = $backup['backup_log'];
                    }

                    $output .= '<div class="backup-box">';
                    //$output .= '<div class="instructions">'.$instructions."\n";
                    $output .= '<p><strong>' . __('Last Backup : ', 'optionsframework') . '<span class="backup-log">' . $log . '</span></strong></p>' . "\n";
                    $output .= '<a href="#" id="of_backup_button" class="button-secondary" title="Create backup">Create Backup</a>';
                    $output .= ' <a href="#" id="of_restore_button" class="button-secondary" title="Restore Options">Restore Backup</a>';
                    $output .= '</div>';

                    break;

                //export or import data between different installs
                case 'transfer':
                    $instructions = $value['desc'];

                    $output .= '<textarea class="of_export_data" id="export_data_' . $value['id'] . '" rows="8">' . base64_encode(json_encode($data)) . '</textarea>' . "\n";
                    $output .= '<a href="#" id="of_import_button_' . $value['id'] . '" class="of_import_button button-secondary" title="Restore Options">Import Options</a>';

					$of_options_profiles = array();
					for ($this_profile_count = 1; $this_profile_count <= MAXPROFILES; $this_profile_count++) {
						$of_options_profiles[$this_profile_count] = "Profile " . $this_profile_count;
					}

					$output .= '<div class="copy_box">';
					$output .= '<label for="of_copy_select_' . $value['id'] . '">Copy to: </label>';

					$output .= '<select id="of_copy_select" class="copy_select">';

					$x=0;
					foreach ($of_options_profiles as $option => $name) {
						$output .= '<option value="' . $option . '" />'.$of_options_profiles[$x+1].'</option>';
						$x++;
					}
					$output .= '</select>';

					$output .= '<a href="#" id="of_copy_button" class="of_copy_button button-secondary" title="Copy">Copy</a>';

					$output .= '</div>';


                    break;

                //partial export
                case 'extractoptions':
                    $export_for_preset = true;
                    @$e_fields = explode(",", $value['fields']);
                    $extract_options = array();
                    foreach ($e_fields as $e_field) {
                        if (isset($data[$e_field])) {
                            $export_value = $data[$e_field];
                            if ($export_for_preset == true && !is_array($export_value) && (substr_count($export_value, "http://") > 0 || substr_count($export_value, "https://")) > 0) {
                                if ($e_field == "banner_background_image") {
                                    $file = basename($export_value);
                                    if (isset($data['banner_background_image_style']) && $data['banner_background_image_style'] == "tile") {
                                        $export_value = "__themedir__/images/textures/" . $file;
                                    }
                                    else {
                                        $export_value = "__themedir__/images/backgrounds/" . $file;
                                    }
                                }
                            }
                            $extract_options[$e_field] = $export_value;
                        }
                    }
                    $instructions = $value['desc'];
                    $output .= '<textarea class="of_export_data" id="export_data_' . $value['id'] . '" rows="8">' . base64_encode(json_encode($extract_options)) /* 100% safe - ignore theme check nag */ . '</textarea>' . "\n";
                    $output .= '<a href="#" id="of_import_button_' . $value['id'] . '" class="of_import_button button-secondary" title="Restore Options">Import Options</a>';
                    break;
            }

            //description of each option
            if ($value['type'] != 'heading' && $value['type'] != "subheading") {
                $explain_value = '';
                if (isset($value['desc'])) {
                    if ((isset($value['descposition']) && @$value['descposition'] == "right") || !isset($value['descposition'])) {
                        $explain_value = '<div class="explain">' . $value['desc'] . '</div>' . "\n";
                    }
                }
                $output .= '</div>' . $explain_value . "\n";
                $output .= '<div class="clear"> </div></div></div>' . "\n";
            }
        }

        $output .= '</div>';

        return array($output, $menu, $defaults);

    }


    /**
     * Ajax image uploader - supports various types of image types
     *
     * @uses get_option()
     *
     * @access public
     * @since 1.0.0
     *
     * @return string
     */
    public static function optionsframework_uploader_function($id, $std, $mod)
    {
        $data = get_option(OPTIONS);

        $uploader = '';
        if (isset($data[$id])) $upload = $data[$id]; else $upload = "";
        $hide = '';

        if ($mod == "min") {
            $hide = 'hide';
        }

        if ($upload != "") {
            $val = $upload;
        } else {
            $val = $std;
        }

        $uploader .= '<input class="' . $hide . ' upload of-input" name="' . $id . '" id="' . $id . '_upload" value="' . $val . '" />';

        $uploader .= '<div class="upload_button_div"><span class="button-secondary image_upload_button" id="' . $id . '">' . _('Upload') . '</span>';

        if (!empty($upload)) {
            $hide = '';
        } else {
            $hide = 'hide';
        }
        $uploader .= '<span class="button-secondary image_reset_button ' . $hide . '" id="reset_' . $id . '" title="' . $id . '">Remove</span>';
        $uploader .= '</div>' . "\n";
        $uploader .= '<div class="clear"></div>' . "\n";
        if (!empty($upload)) {
            $uploader .= '<div class="screenshot">';
            $uploader .= '<a class="of-uploaded-image" href="' . $upload . '">';
            $uploader .= '<img class="of-option-image" id="image_' . $id . '" src="' . $upload . '" alt="" />';
            $uploader .= '</a>';
            $uploader .= '</div>';
        }
        $uploader .= '<div class="clear"></div>' . "\n";

        return $uploader;

    }

    /**
     * Native media library uploader
     *
     * @uses get_option()
     *
     * @access public
     * @since 1.0.0
     *
     * @return string
     */
    public static function optionsframework_media_uploader_function($id, $std, $int, $mod)
    {

        $data = get_option(OPTIONS);

        $uploader = '';
        $upload = $data[$id];
        $hide = '';

        if ($mod == "min") {
            $hide = 'hide';
        }

        if ($upload != "") {
            $val = $upload;
        } else {
            $val = $std;
        }

        $uploader .= '<input class="' . $hide . ' upload of-input" name="' . $id . '" id="' . $id . '_upload" value="' . $val . '" />';

        $uploader .= '<div class="upload_button_div"><span class="button-secondary media_upload_button" id="' . $id . '" rel="' . $int . '">Upload</span>';

        if (!empty($upload)) {
            $hide = '';
        } else {
            $hide = 'hide';
        }
        $uploader .= '<span class="button-secondary mlu_remove_button ' . $hide . '" id="reset_' . $id . '" title="' . $id . '">Remove</span>';
        $uploader .= '</div>' . "\n";
        $uploader .= '<div class="screenshot">';
        if (!empty($upload)) {
            $uploader .= '<a class="of-uploaded-image" href="' . $upload . '">';
            $uploader .= '<img class="of-option-image" id="image_' . $id . '" src="' . $upload . '" alt="" />';
            $uploader .= '</a>';
        }
        $uploader .= '</div>';
        $uploader .= '<div class="clear"></div>' . "\n";

        return $uploader;

    }

    /**
     * Drag and drop slides manager
     *
     * @uses get_option()
     *
     * @access public
     * @since 1.0.0
     *
     * @return string
     */
    public static function optionsframework_slider_function($id, $std, $oldorder, $order, $int, $settings)
    {

        $data = get_option(OPTIONS);

        $slider = '';
        $slide = array();

		if (isset($data[$id])) {
			$slide = $data[$id];
		}
		else {
			$slide = array();
		}

        if (isset($slide[$oldorder])) {
            $val = $slide[$oldorder];
        } else {
            $val = $std;
        }

        //initialize all vars
        $slidevars = array('title', 'subtitle', 'url', 'link', 'description');

        foreach ($slidevars as $slidevar) {
            if (!isset($val[$slidevar])) {
                $val[$slidevar] = '';
            }
        }

        //begin slider interface
        if (!empty($val['title'])) {
            $slider .= '<li><div class="slide_header"><strong>' . stripslashes($val['title']) . '</strong>';
        } else {
            $slider .= '<li><div class="slide_header"><strong>Item ' . $order . '</strong>';
        }

        $slider .= '<input type="hidden" class="slide of-input order" name="' . $id . '[' . $order . '][order]" id="' . $id . '_' . $order . '_slide_order" value="' . $order . '" />';

        $slider .= '<a class="slide_edit_button" href="#">Edit</a></div>';

        $slider .= '<div class="slide_body">';

        if (isset($settings['has_title']) && @$settings['has_title'] == true || !isset($settings['has_title'])) {
            if (isset($settings['has_title_testimonial']) && @$settings['has_title_testimonial'] == true) {
                $slider .= '<label>Name</label>';
            } else {
                $slider .= '<label>Title</label>';
            }
            $slider .= '<input class="slide of-input of-slider-title" name="' . $id . '[' . $order . '][title]" id="' . $id . '_' . $order . '_slide_title" value="' . stripslashes($val['title']) . '" />';
        }

        if (isset($settings['has_subtitle']) && @$settings['has_subtitle'] == true) {
            $title_extra = "";
            if (isset($settings['has_subtitle_testimonial']) && @$settings['has_subtitle_testimonial'] == true) $title_extra = '/Company/Profession';
            $slider .= '<label>Sub Title' . $title_extra . '</label>';
            $slider .= '<input class="slide of-input of-slider-subtitle" name="' . $id . '[' . $order . '][subtitle]" id="' . $id . '_' . $order . '_slide_subtitle" value="' . stripslashes($val['subtitle']) . '" />';
        }


        if (isset($settings['has_image']) && @$settings['has_image'] == true) {
            $slider .= '<label>Image URL</label>';
            $slider .= '<input class="slide of-input" name="' . $id . '[' . $order . '][url]" id="' . $id . '_' . $order . '_slide_url" value="' . $val['url'] . '" />';

            $slider .= '<div class="upload_button_div"><span class="button-secondary media_upload_button" id="' . $id . '_' . $order . '" rel="' . $int . '">Upload</span>';

            if (!empty($val['url'])) {
                $hide = '';
            } else {
                $hide = 'hide';
            }
            $slider .= '<span class="button-secondary mlu_remove_button ' . $hide . '" id="reset_' . $id . '_' . $order . '" title="' . $id . '_' . $order . '">Remove</span>';
            $slider .= '</div>' . "\n";
            $slider .= '<div class="screenshot">';
            if (!empty($val['url'])) {
                $slider .= '<a class="of-uploaded-image" href="' . $val['url'] . '">';
                $slider .= '<img class="of-option-image" id="image_' . $id . '_' . $order . '" src="' . $val['url'] . '" alt="" />';
                $slider .= '</a>';
            }
            $slider .= '</div>';
        }

        if (isset($settings['has_link']) && @$settings['has_link'] == true || !isset($settings['has_link'])) {
            $slider .= '<label>Link URL</label>';
            $slider .= '<input placeholder="http://www.example.com" class="slide of-input" name="' . $id . '[' . $order . '][link]" id="' . $id . '_' . $order . '_slide_link" value="' . $val['link'] . '" />';
        }

        if (isset($settings['has_description']) && @$settings['has_description'] == true || !isset($settings['has_description'])) {
            $slider .= '<label>Description/Text</label>';
            $slider .= '<textarea class="slide of-input" name="' . $id . '[' . $order . '][description]" id="' . $id . '_' . $order . '_slide_description" cols="8" rows="8">' . stripslashes($val['description']) . '</textarea>';
        }

        if (isset($settings['has_price']) && @$settings['has_price'] == true) {
            if (!isset($val['price'])) $val['price'] = "";
            $slider .= '<label>Price</label>';
            $slider .= '<input placeholder="e.g. 19.95" class="slide of-input" name="' . $id . '[' . $order . '][price]" id="' . $id . '_' . $order . '_slide_price" value="' . $val['price'] . '" />';
        }

        if (isset($settings['has_badge']) && @$settings['has_badge'] == true) {
            $badges = array(
                "Most Chosen" => "badge_mostchosen",
                "Favorite" => "badge_favorite",
                "Best Value" => "badge_bestvalue",
                "Bestseller" => "badge_bestseller"
            );

            $badge_options = '<option value="">None</option>';
            if (!isset($val['badge'])) $val['badge'] = "";
            foreach ($badges as $b_title => $b_file) {
                if ($val['badge'] == $b_file) {
                    $b_sel = ' selected="selected"';
                } else {
                    $b_sel = '';
                }
                ;
                $badge_options .= '<option value="' . $b_file . '"' . $b_sel . '>' . $b_title . '</option>';
            }

            $slider .= '<label>Badge</label>';
            $slider .= '<select class="slide of-input" name="' . $id . '[' . $order . '][badge]" id="' . $id . '_' . $order . '_slide_badge" value="' . $val['badge'] . '" />' . $badge_options . '</select>';
        }

        if (isset($settings['has_highlight']) && @$settings['has_highlight'] == true) {
            $highlighted = array("no", "yes");
            $highlighted_options = '';
            foreach ($highlighted as $highlighted_item) {
                if (isset($val['highlighted']) && $val['highlighted'] == $highlighted_item) {
                    $b_sel = ' selected="selected"';
                } else {
                    $b_sel = '';
                }
                ;
                $highlighted_options .= '<option value="' . $highlighted_item . '"' . $b_sel . '>' . $highlighted_item . '</option>';
            }
            if (!isset($val['highlighted'])) $val['highlighted'] = "no";
            $slider .= '<label>Highlighted/Featured</label>';
            $slider .= '<select class="slide of-input" name="' . $id . '[' . $order . '][highlighted]" id="' . $id . '_' . $order . '_slide_highlighted" value="' . $val['highlighted'] . '" />' . $highlighted_options . '</select>';
        }


        $slider .= '<a class="slide_delete_button" href="#">Delete</a>';
        $slider .= '<div class="clear"></div>' . "\n";

        $slider .= '</div>';
        $slider .= '</li>';

        return $slider;

    }

}

//end Options Machine class

?>