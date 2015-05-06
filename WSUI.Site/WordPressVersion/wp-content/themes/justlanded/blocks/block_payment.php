<?php  global $is_landing_page; ?>
<!--Start of Buy Now-->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
	<?php echo justlanded_get_headline("h2", @$data['payment_headline']); ?>
	<?php echo justlanded_get_headline("h3", @$data['payment_sub_headline']); ?>
    <?php include(JUSTLANDED_BLOCKS_DIR . "block_cta_buttons_plain.php"); ?>
    <!--Start of Payment Icons-->
    <div class="payment">
        <?php
        @$payment_methods = $data['payment_methods'];
        if (is_array($payment_methods)) {
            foreach ($payment_methods as $payment_method => $checked) {
                echo '<img src="'.get_template_directory_uri().'/images/icons/'.$payment_method.'.png" alt="'.$payment_method.'" />';
            }
        }
        ?>
    </div>
    <!--End of Payment Icons-->
</section>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Buy Now-->
