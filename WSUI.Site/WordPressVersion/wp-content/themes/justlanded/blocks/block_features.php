<?php
global $is_landing_page;
if (isset($data['features']) && $data['features'] != "")
{
?>
<!--Start of Features-->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
    <!--Start of Features Title-->
    <hgroup>
        <?php if(isset($this_atts['title'])) echo justlanded_get_headline("h2", $this_atts['title']); else echo justlanded_get_headline("h2", @$data['features_headline']); ?>
        <?php if(isset($this_atts['subtitle'])) echo justlanded_get_headline("h3", $this_atts['subtitle']); else echo justlanded_get_headline("h3", @$data['features_sub_headline']); ?>
    </hgroup>
    <!--End of Features Title-->

    <!--Start of Features List-->
    <ul>
        <?php
        if (isset($this_content) && $this_content != null) {
            $defaults = array(
                "title" => "",
                "subtitle" => "",
                "description" => "",
                "link" => "",
                "url" => null
            );
            $features = justlanded_parse_shortcode_items($this_content, $defaults);
        }
        else {
            $features = justlanded_get_option('features', array(), $data); //get the features from settings
        }

        $x=0;
        foreach ($features as $feature) {
        if ($x == 2) {
            $classmod = ' class="one_third last"';
            $x=-1;
        }
        else {
            $classmod = ' class="one_third"';
        }
        if (@$feature['url'] == "") {
            $feature['url'] = get_bloginfo('template_directory', false).'/images/icons/icon_tick.png';
        }
        ?>
        <li<?php echo $classmod; ?>><div class="feature_image"><?php if (isset($feature['link']) && $feature['link'] != "") echo '<a href="'.stripslashes($feature['link']).'">'; ?><img src="<?php echo @$feature['url']; ?>" title="<?php echo stripslashes(@$feature['title']); ?>" alt="<?php echo stripslashes(@$feature['title']); ?>"/><?php if (isset($feature['link']) && $feature['link'] != "") echo '</a>'; ?></div>
        <div class="feature_text">
            <h4><?php if (isset($feature['link']) && $feature['link'] != "") echo '<a href="'.stripslashes($feature['link']).'">'; ?><?php echo do_shortcode(stripslashes(@$feature['title'])); ?><?php if (isset($feature['link']) && $feature['link'] != "") echo '</a>'; ?></h4> <p><?php echo do_shortcode(stripslashes(@$feature['description'])); ?></p></div></li>
        <?php
        $x++;
        }
        ?>
    </ul>
    <!--End of Features List-->
</section>
 <?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Features-->
<?php
}
?>