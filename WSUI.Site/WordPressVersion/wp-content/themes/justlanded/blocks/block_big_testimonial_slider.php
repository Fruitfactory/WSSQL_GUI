<?php
global $is_landing_page;
$testimonial_maintitle = "";
$testimonial_subtitle = "";


if (isset($data['testimonials']) && $data['testimonials'] != "")
{
	if (isset($data['featured_testimonial_headline']))     $testimonial_maintitle = $data['testimonial_slider_headline'];
	if (isset($data['featured_testimonial_sub_headline'])) $testimonial_subtitle  = $data['testimonial_slider_sub_headline'];

	?>
    <!--Start of Testimonial Slider-->
    <?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
        <section  id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
			<?php if($testimonial_maintitle != "") echo justlanded_get_headline("h2", $testimonial_maintitle); ?>
			<?php if($testimonial_subtitle != "")  echo justlanded_get_headline("h3", $testimonial_subtitle); ?>

            <div class="testimonial-slider-large flexslider">
                <ul class="slides">
                <?php
                if (isset($this_content) && $this_content != null) {
                    $defaults = array(
                        'title' => '',
                        'subtitle' => '',
                        'url' => '',
                        'description' => ''
                    );
                    $testimonials = justlanded_parse_shortcode_items($this_content, $defaults);
                }
                else {
                    $testimonials = justlanded_get_option('testimonials', array(), $data); //get the testimonials from settings
                }
                foreach ($testimonials as $testimonial) {
                    ?>
                    <li>
                    <!--Start of Testimonial-->
                    <blockquote class="testimonial_big">
                        <q><?php echo do_shortcode(stripslashes($testimonial['description'])); ?></q>
                        <footer><div><?php if (isset($testimonial['link']) && $testimonial['link'] != "") { ?><a href="<?php echo $testimonial['link']; ?>" target="<?php echo justlanded_get_option('testimonials_link_target', '_self', $data); ?>"><?php } ?><?php echo stripslashes(@$testimonial['title']);?><?php if (isset($testimonial['link']) && $testimonial['link'] != "") echo '</a>'; ?></div> - <?php echo do_shortcode(stripslashes($testimonial['subtitle'])); ?></footer>
                    </blockquote>
                    <!--End of Testimonial-->
                    </li>
                    <?php
                }
                ?>
                </ul>
            </div>
        </section>
    <?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
    <!--End of Testimonial Slider-->
<?php
}
?>