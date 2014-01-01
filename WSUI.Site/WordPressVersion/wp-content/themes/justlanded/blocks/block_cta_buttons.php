<?php  global $is_landing_page; ?>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
<?php include(JUSTLANDED_BLOCKS_DIR . "block_cta_buttons_plain.php"); ?>
</section>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
