<section id="banner" role="banner" class="banner_media_full page_banner">
	<div class="row">
		<div id="banner_full">
			<?php
			do_action("justlanded_before_page_banner");
			if (isset($data['page_banner_page']) && $data['page_banner_page'] != 0) {
				@$p = get_page($data['page_banner_page']);
				if (isset ($p->post_content)) {
					echo justlanded_content_filters(@$p->post_content);
				}
			} else {
				if (isset($data['page_banner']) && trim($data['page_banner']) != "") {
					echo justlanded_content_filters(stripslashes($data['page_banner']));
				} else {
					do_action("justlanded_page_banner");
				}
			}
			do_action("justlanded_after_page_banner");
			?>
		</div>
	</div>
</section>
<?php do_action("justlanded_after_page_banner_section"); ?>
