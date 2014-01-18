<?php
global $is_landing_page;
?>
<!--Start Posts-->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
    <?php
    if (isset($data['posts_before']) && trim($data['posts_before']) != "" || isset($data['posts_before_page']) && $data['posts_before_page'] != 0) {
    echo '<div class="block_posts_before block clear">' . "\n";
        if (isset($data['posts_before_page']) && $data['posts_before_page'] != 0) {
            @$p = get_page($data['posts_before_page']);
            if (isset ($p->post_content)) {
                echo justlanded_content_filters(@$p->post_content);
            }
        }
        else {
        ?>
            <?php  ?>
            <?php echo justlanded_content_filters(stripslashes($data['posts_before'])); ?>
        <?php
        }
        ?>
    <?php
    echo '</div>'."\n";
    }
    ?>
    <div class="block_680">
    <?php
    $args = array(
        'post_type' => 'post'
    );
    if (isset($data['posts_query_string']) && $data['posts_query_string'] != "") {
        $tmp_args = explode("\n", $data['posts_query_string']);
        foreach ($tmp_args as $tmp_args_row) {
            $tmp_args_row = trim ($tmp_args_row);
            if (substr_count($tmp_args_row, "=") && strlen($tmp_args_row) > 2)  {
                $arg = explode("=", $tmp_args_row);
                $args[trim($arg[0])] = trim($arg[1]);
            }
        }
    }
    query_posts($args);
    ?>
    <?php while ( have_posts() ) : the_post() ?>
        <?php get_template_part( 'entry' ); ?>
    <?php endwhile; ?>
    </div>
    <aside id="sidebar" class="three columns">
        <?php if ( is_active_sidebar('landing-page-posts-sidebar') ) : ?>
            <div id="primary" class="widget-area">
                <ul class="sid">
                    <?php dynamic_sidebar('landing-page-posts-sidebar'); ?>
                </ul>
            </div>
        <?php endif; ?>
    </aside>
    <?php
    if (isset($data['posts_after']) && trim($data['posts_after']) != "" || isset($data['posts_after_page']) && $data['posts_after_page'] != 0) {
        echo '<div class="block_posts_after block clear">' . "\n";
        if (isset($data['posts_after_page']) && $data['posts_after_page'] != 0) {
            @$p = get_page($data['posts_after_page']);
            if (isset ($p->post_content)) {
                echo justlanded_content_filters(@$p->post_content);
            }
        }
        else {
            ?>
            <?php  ?>
            <?php echo justlanded_content_filters(stripslashes($data['posts_after'])); ?>
        <?php
        }
        ?>
        <?php
        echo '</div>'."\n";
    }
    ?>
    <div class="clear"></div>
</section>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Posts-->