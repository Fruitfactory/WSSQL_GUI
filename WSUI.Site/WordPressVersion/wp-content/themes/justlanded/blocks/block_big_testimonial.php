<?php
    global $is_landing_page;
    $testimonial_quote = "The featured testimonial block is empty.";
    $testimonial_name = "";
    $testimonial_title = "";

	$testimonial_maintitle = "";
	$testimonial_subtitle = "";

    if (isset($data['testimonial_quote'])) $testimonial_quote = $data['testimonial_quote'];
    if (isset($data['testimonial_name']))  $testimonial_name  = $data['testimonial_name'];
    if (isset($data['testimonial_title'])) $testimonial_title = $data['testimonial_title'];

	if (isset($data['featured_testimonial_headline']))     $testimonial_maintitle = $data['featured_testimonial_headline'];
	if (isset($data['featured_testimonial_sub_headline'])) $testimonial_subtitle  = $data['featured_testimonial_sub_headline'];

    if(isset($this_atts['quote'])) $testimonial_quote = $this_atts['quote'];
    if(isset($this_atts['name']))  $testimonial_name  = $this_atts['name'];
    if(isset($this_atts['title'])) $testimonial_title = $this_atts['title'];

	if(isset($this_atts['maintitle'])) $testimonial_maintitle = $this_atts['maintitle'];
	if(isset($this_atts['subtitle']))  $testimonial_subtitle  = $this_atts['subtitle'];


?>
<!--Start of Big Testimonial-->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
	<?php if($testimonial_maintitle != "") echo justlanded_get_headline("h2", $testimonial_maintitle); ?>
	<?php if($testimonial_subtitle != "")  echo justlanded_get_headline("h3", $testimonial_subtitle); ?>
    <blockquote class="testimonial_big">
        <q><?php echo do_shortcode(stripslashes($testimonial_quote)); ?></q>
        <footer><div><?php echo do_shortcode(stripslashes($testimonial_name)); ?></div> - <?php echo do_shortcode(stripslashes($testimonial_title)); ?></footer>
    </blockquote>
</section>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Big Testimonial-->