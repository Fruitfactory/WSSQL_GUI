<?php
global $is_landing_page;
wp_reset_query();
?>
<!--Start Comments-->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
<?php comments_template('', true); ?>
</section>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Comments-->