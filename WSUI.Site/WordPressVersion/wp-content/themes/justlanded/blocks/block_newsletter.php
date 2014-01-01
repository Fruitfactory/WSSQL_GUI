<?php global $data, $is_landing_page; ?>
<!--Start of Newsletter -->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<aside  id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
    <div class="newsletter">
        <div class="newsletter_inner">
            <?php echo justlanded_get_headline("h2", @$data['newsletter_headline_1']); ?>
            <?php echo justlanded_get_block(JUSTLANDED_BLOCKS_DIR . 'block_newsletter_plain_form.php', array()); ?>
        </div>
    </div>
</aside>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Newsletter-->
