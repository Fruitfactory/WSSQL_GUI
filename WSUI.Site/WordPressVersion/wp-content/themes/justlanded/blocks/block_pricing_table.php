<?php
global $is_landing_page;
global $justlanded_modals;
if (isset($data['pricing_packages']) && $data['pricing_packages'] != "")
{
?>
<!--Start of Pricing-->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?>">
    <hgroup>
        <?php if(isset($this_atts['title'])) echo justlanded_get_headline("h2", $this_atts['title']); else echo justlanded_get_headline("h2", @$data['pricing_headline']); ?>
        <?php if(isset($this_atts['subtitle'])) echo justlanded_get_headline("h3", $this_atts['subtitle']); else echo justlanded_get_headline("h3", @$data['pricing_sub_headline']); ?>
    </hgroup>
    <div class="pricing_table">
    <?php
    $currency = justlanded_get_option('pricing_currency', '$', $data);
    $currency_pos = justlanded_get_option('pricing_currency_position', 'before', $data);
    $button   = justlanded_get_option('pricing_button_text', 'Buy', $data);
    if (isset($this_content) && $this_content != null) {
         $defaults = array(
             "title" => "Title",
             "subtitle" => "Subtitle",
             "link" => "javascript:void(0);",
             "description" => "",
             "price" => "0",
             "badge" => "",
             "highlighted" => "no",
             "button" => null
         );
         $packages = justlanded_parse_shortcode_items($this_content, $defaults);
         if (isset($this_atts['currency'])) $currency = $this_atts['currency'];
         if (isset($this_atts['button']))   $button   = $this_atts['button'];
    }
    else {
        $packages = justlanded_get_option('pricing_packages', array(), $data); //get the features from the settings
    }


    if (isset($packages) && is_array($packages)) {
    $item_count=0;
    foreach ($packages as $package) {
        $item_count++;
		$package_modal = "";

        if (isset($justlanded_modals) && is_array($justlanded_modals)) {
            foreach ($justlanded_modals as $modal) {
                if ($modal['id'] == "pricing_" . $this_block_id . "_item_" . $item_count) {
                    $package_modal = ' data-reveal-id="modal_custom_pricing_'.$this_block_id . '_item_'. $item_count . '"';
                    $package['link'] = 'javascript:void(0);';
                } else {
                }
        }
        }

        ?>

        <!--Beginn Pricing Block-->
        <div class="pricing_block<?php if (@$package['highlighted'] == "yes") echo ' highlighted'; ?> block">
			<?php if (isset($package['badge']) && $package['badge'] != "") : ?>
			<div class="<?php if ($package['badge'] != "") echo 'badge ' . @$package['badge']; ?>"><span><?php echo @$package['badge']; ?></span></div>
			<?php endif; ?>
            <div class="pricing_header">
                <hgroup>
                    <h4><?php echo do_shortcode(stripslashes(@$package['title'])); ?></h4>
                    <h5><?php echo do_shortcode(stripslashes(@$package['subtitle'])); ?></h5>
                </hgroup>
                <p class="price currency_<?php echo $currency_pos ?>"><?php if ($currency_pos == "before") { ?><span><?php echo $currency; ?></span><?php } ?><?php echo @$package['price']; ?><?php if ($currency_pos == "after") { ?><span><?php echo $currency; ?></span><?php } ?></p>
            </div>
            <?php if (substr_count(@$package['description'], "\n") > 0)
            {
            ?>
            <ul>
            <?php
                $y=0;
                $package_features = explode("\n", @$package['description']);
                foreach ($package_features as $package_feature) {
            ?>
                <li<?php if ($y == count($package_features)-1) echo ' class="last"'; ?>><?php echo @$package_feature; ?></li>
            <?php
                $y++;
                }
            ?>
            </ul>
            <?php } else { ?>
            <p>
                <?php echo do_shortcode(stripslashes(@$package['description'])); ?>
            </p>
            <?php } ?>
            <?php if (trim($package['link']) != "") { ?><div class="pricing_footer"><p><a href="<?php echo do_shortcode(stripslashes(@$package['link'])); ?>" class="button_buy_pricing gradient"<?php echo @$package_modal; ?>><?php if (isset($package['button'])) echo stripslashes($package['button']); else echo stripslashes($button); ?></a></p></div><?php } ?>
        </div>
        <!--End Pricing Block-->

        <?php
        }
    } ?>

    </div><!--End of Pricing Table-->
</section>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Pricing-->
<?php } ?>