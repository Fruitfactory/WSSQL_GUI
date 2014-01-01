<?php
/**
 * Template Name: Landing Page
 * Description: The actual landing page
*/
@$active_profile = get_post_meta($post->ID, 'justlanded_meta_box_selectinstance_select', true);
@$parse_content  = intval(get_post_meta($post->ID, 'justlanded_meta_box_parse_content', true));
do_action('justlanded_before_landing_options'); // custom hook
if ($active_profile != 0) {
    $data = get_option(OPTIONSPREFIX.$active_profile);
} else {
    $data = get_option(OPTIONSPREFIX.SITE_DEFAULT_PROFILE);
}
do_action('justlanded_after_landing_options'); // custom hook
?>
<?php get_header(); ?>
<?php if(isset($data['hide_banner']) && $data['hide_banner'] == 0 || !isset($data['hide_banner'])) include JUSTLANDED_BLOCKS_DIR . 'block_header_banner.php'; // banner and action buttons ?>
    <!--Start of Main Content-->
        <article role="main">
            <?php
            global $used_blocks, $is_landing_page;
            $is_landing_page = true;
            $used_blocks = array();

            if ($parse_content == 0) {
                $this_block_type = "";
                $this_block_id   = 0;
                $layout = $data['landingpage_blocks']['enabled'];
                if ($layout):
                    $block_count = 0;
                    foreach ($layout as $key=>$value) {
                        if ($key != "placebo") // if not an empty entry
                        {
                            if (!isset($used_blocks[$key])) $used_blocks[$key] = 1; else $used_blocks[$key]++;
                            $this_block_type = str_replace("block_", "", $key);
                            $this_block_id   = $used_blocks[$key];
                            $block_tpl = JUSTLANDED_BLOCKS_DIR . $key . '.php'; // if block template exists, otherwise do nothing
							$block_tpl = apply_filters ( 'justlanded_block_template_file', $block_tpl );
                            if (file_exists($block_tpl) || function_exists('justlanded_block_' . $this_block_type)) {
                                $options['this_block_type'] = $this_block_type;
                                $options['this_block_id'] = $this_block_id;
                                do_action('justlanded_before_block_template');
                                do_action('justlanded_before_block_' . $this_block_type);
                                if (function_exists('justlanded_block_' . $this_block_type)) {
                                    echo call_user_func('justlanded_block_' . $this_block_type) || "";
                                }
                                else {
                                    echo justlanded_get_block($block_tpl, $options);
                                }
                                do_action('justlanded_after_block_' . $this_block_type);
                                do_action('justlanded_after_block_template');
                            }
                            else {
                                // do nothing
                            }
                        }
                    }
                endif;
            }
            else {
                if (!justlanded_shortcode_exists("justlanded_landing_block")) add_shortcode('landing_block', 'justlanded_landing_block');
                if (!justlanded_shortcode_exists("landing_block_custom")) add_shortcode('landing_block_custom', 'justlanded_landing_block');
                echo do_shortcode($post->post_content);
            }
            ?>
        </article>
    <!--End of Main Content-->
<?php get_footer(); ?>