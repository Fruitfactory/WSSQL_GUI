<?php
global $is_landing_page;
if (isset($data['testimonials']) && $data['testimonials'] != "")
{
?>
<!--Start of Testimonials-->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section  id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
    <hgroup>
        <?php if(isset($this_atts['title'])) echo justlanded_get_headline("h2", $this_atts['title']); else echo justlanded_get_headline("h2", @$data['testimonials_headline']); ?>
        <?php if(isset($this_atts['subtitle'])) echo justlanded_get_headline("h3", $this_atts['subtitle']); else echo justlanded_get_headline("h3", @$data['testimonials_sub_headline']); ?>
    </hgroup>
    <?php

    if (isset($this_content) && $this_content != null) {
        $defaults = array(
            "title" => "",
            "subtitle" => "",
            "description" => "",
			"link" => null,
            "url" => null
        );
        $testimonials = justlanded_parse_shortcode_items($this_content, $defaults);
    }
    else {
        $testimonials = justlanded_get_option('testimonials', array(), $data); //get the testimonials from settings
    }

    $x=0;
    foreach ($testimonials as $testimonial) {
    if ($x == 2) {
        $classmod = ' last';
        $x=-1;
    }
    else {
        $classmod = '';
    }
    if (@$testimonial['url'] == "") {
        $testimonial['url'] = get_bloginfo('template_directory', false).'/images/avatar.jpg';
    }
    ?>
    <!--Start of Testimonial-->
    <blockquote class="one_third<?php echo $classmod; ?>">
        <q><?php echo do_shortcode(stripslashes(@$testimonial['description']));?></q>
        <footer><?php if (isset($testimonial['link']) && $testimonial['link'] != "") { ?><a href="<?php echo $testimonial['link']; ?>" target="<?php echo justlanded_get_option('testimonials_link_target', '_self', $data); ?>"><?php } ?><img src="<?php echo @$testimonial['url']; ?>" alt="<?php echo esc_attr(do_shortcode(stripslashes(@$testimonial['title'])));?>"/><?php if (isset($testimonial['link']) && $testimonial['link'] != "") echo '</a>'; ?><div><?php if ((isset($testimonial['link']) && $testimonial['link'] != "")) { ?><a href="<?php echo $testimonial['link']; ?>" target="<?php echo justlanded_get_option('testimonials_link_target', '_self', $data); ?>"><?php } ?><?php echo stripslashes(@$testimonial['title']);?><?php if (isset($testimonial['link']) && $testimonial['link'] != "") echo '</a>'; ?></div> <?php echo do_shortcode(stripslashes(@$testimonial['subtitle']));?></footer>
    </blockquote>
    <!--End of Testimonial-->
    <?php
     $x++;
    }
    ?>
</section>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Testimonials-->
<?php
}
?>