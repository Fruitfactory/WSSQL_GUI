<?php
global $is_landing_page;
if (@$data['gallery'] != "")
{
?>
<!--Start of Screenshots Gallery-->
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
<section id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
    <hgroup>
        <?php if(isset($this_atts['title'])) echo justlanded_get_headline("h2", $this_atts['title']); else echo justlanded_get_headline("h2", @$data['gallery_headline']); ?>
        <?php if(isset($this_atts['subtitle'])) echo justlanded_get_headline("h3", $this_atts['subtitle']); else echo justlanded_get_headline("h3", @$data['gallery_sub_headline']); ?>
    </hgroup>
    <?php

    if (isset($this_content) && $this_content != null) {
        $defaults = array(
            "title" => "",
            "subtitle" => "",
            "description" => "",
            "link" => "",
            "url" => null
        );
        $gallery = justlanded_parse_shortcode_items($this_content, $defaults);
    }
    else {
        $gallery = justlanded_get_option('gallery', array(), $data); //get images from settings
    }

    if (count($gallery) == 1 && isset($gallery[1]) && isset($gallery[1]['url']) && trim($gallery[1]['url']) == "")
    {
        echo "Gallery is empty. Nothing to show.";
    }
    else {
        $x=0;
        foreach ($gallery as $image) {
            @$attachment_id = justlanded_get_attachment_id_from_url($image['url']);
            if ($attachment_id)
            {
                $thb = justlanded_resize($attachment_id, null, 140, 95, true);
            } else {
                $thb = justlanded_resize(null , $image['url'], 140, 95, true);
            }
            if ($x == 5) {
                $classmod = ' class="last"';
                $x=-1;
            }
            else {
                $classmod = '';
            }
            if ($image['description'] == "") $image['description'] = $image['title'];

            if (!isset($image['link']) || isset($image['link']) && $image['link'] == "") {
                $gallery_link_url = $image['url'];
                $gallery_link_class = "screenshot";
            } else {
                $gallery_link_url = $image['link'];
                $gallery_link_class = "screenshot_link";
            }

            ?>
            <a href="<?php echo $gallery_link_url; ?>" class="<?php echo $gallery_link_class; ?>" title="<?php echo $image['description']; ?>"><img src="<?php echo $thb['url']; ?>" height="95" width="140" alt="<?php echo $image['title']; ?>" title="<?php echo $image['title']; ?>"<?php echo $classmod; ?>/></a>
            <?php
            $x++;
        }
    }
    ?>
</section>
<?php if (isset($is_landing_page) && $is_landing_page == true) { ?></div><?php } ?>
<!--End of Screenshots Gallery-->
<?php
}
?>