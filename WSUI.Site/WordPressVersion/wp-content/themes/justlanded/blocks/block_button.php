<?php
global $button;
if (isset($options) && isset($options['this_atts'])) {
    $defaults = array(
        "caption" => "",
        "link" => "",
        "target" => "_self",
        "align" => "center"
    );
    $button = shortcode_atts($defaults, $options['this_atts']);
    ?>
    <?php  global $is_landing_page; ?>
    <?php if (!isset($is_landing_page) || isset($is_landing_page) && $is_landing_page == false) { ?><div class="row">
    <section id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block"><?php } ?>
        <!--Start of Button-->
            <?php include(JUSTLANDED_BLOCKS_DIR . "block_button_plain.php"); ?>
        <!--End of Button-->
    <?php if (!isset($is_landing_page) || isset($is_landing_page) && $is_landing_page == false) { ?></section>
    </div><?php } ?>
<?php } ?>