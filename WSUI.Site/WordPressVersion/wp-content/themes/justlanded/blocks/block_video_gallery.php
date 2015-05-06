<?php
global $is_landing_page;
if (@$data['video_gallery'] != "")
{
	?>
	<!--Start of Screenshots Gallery-->
	<?php if (isset($is_landing_page) && $is_landing_page == true) { ?><div class="row"><?php } ?>
	<section id="section_<?php echo $this_block_type; ?>_<?php echo $this_block_id; ?>" class="section_<?php echo $this_block_type; ?> block">
		<?php if(isset($this_atts['title'])) echo justlanded_get_headline("h2", $this_atts['title']); else echo justlanded_get_headline("h2", @$data['gallery_headline']); ?>
		<?php if(isset($this_atts['subtitle'])) echo justlanded_get_headline("h3", $this_atts['subtitle']); else echo justlanded_get_headline("h3", @$data['gallery_sub_headline']); ?>
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
			$gallery = justlanded_get_option('video_gallery', array(), $data); //get items
		}

		if (count($gallery) == 0)
		{
			echo "Gallery is empty. Nothing to show.";
		}
		else {
			$x=0;
			if (is_ssl()) $prot = "https"; else $prot = "http";

			foreach ($gallery as $item) {
				if ($x == 5) {
					$classmod = ' class="last"';
					$x=-1;
				}
				else {
					$classmod = '';
				}
				if ($item['description'] == "") $item['description'] = $item['title'];

				$gallery_link_url = $item['link'];
				$gallery_link_class = "screenshot_link";

				$th_url = $prot . "://img.youtube.com/vi/".$gallery_link_url."/mqdefault.jpg";

				?>
				<a href="#" class="<?php echo $gallery_link_class; ?>" title="<?php echo $item['description']; ?>"><img src="<?php echo $th_url; ?>" alt="<?php echo $item['title']; ?>" title="<?php echo $item['title']; ?>"<?php echo $classmod; ?>/></a>
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