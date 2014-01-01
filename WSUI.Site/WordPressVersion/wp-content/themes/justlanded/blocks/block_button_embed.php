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
    <!--Start of Button-->
    <?php include(JUSTLANDED_BLOCKS_DIR . "block_button_plain.php"); ?>
    <!--End of Button-->
<?php } ?>