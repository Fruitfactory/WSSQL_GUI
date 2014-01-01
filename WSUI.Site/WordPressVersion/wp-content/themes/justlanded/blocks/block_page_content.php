<?php
global $is_landing_page;
$is_landing_page_o = $is_landing_page;
$is_landing_page = false;
?>
<?php if(@$data['page_content_'.$page_content_idx.'_layout'] == "fullwidth") { ?>
<?php if(@$data['page_content_'.$page_content_idx."_source1"] != 0) { ?>
<!--Start of Page Content Block <?php echo $page_content_idx; ?>-->
<?php if (isset($is_landing_page_o) && $is_landing_page_o == true) { ?><div class="row"><?php } ?>
<section id="page-content-block-<?php echo $page_content_idx; ?>" class="page-content-block block">
<?php
@$p = get_page($data['page_content_'.$page_content_idx."_source1"]);
if(@$data['page_content_'.$page_content_idx."_showtitle"] == 1) echo "<h2>".$p->post_title."</h2>";
if (isset ($p->post_content)) {
echo justlanded_content_filters(@$p->post_content, "page_content");
}
else {
    echo "A referenced page for this section  (Page Content Block ".$page_content_idx.") does not exist.";
}
?>
</section>
<?php if (isset($is_landing_page_o) && $is_landing_page_o == true) { ?></div><?php } ?>
<!--End of Page Content Block <?php echo $page_content_idx; ?>-->
<?php } else { ?>
<!--Start of Page Content Block <?php echo $page_content_idx; ?>-->
<?php if (isset($is_landing_page_o) && $is_landing_page_o == true) { ?><div class="row"><?php } ?>
<section id="page-content-block-<?php echo $page_content_idx; ?>" class="page-content-block block">
    There is no content to display. (Page Content Block <?php echo $page_content_idx; ?> does not have a source 1)
</section>
<?php if (isset($is_landing_page_o) && $is_landing_page_o == true) { ?></div><?php } ?>
<!--End of Page Content Block <?php echo $page_content_idx; ?>-->
<?php
}
?>
<?php } else { ?>
<!--Start of Page Content Block <?php echo $page_content_idx; ?>-->
 <?php if (isset($is_landing_page_o) && $is_landing_page_o == true) { ?><div class="row"><?php } ?>
<section id="page-content-block-<?php echo $page_content_idx; ?>" class="page-content-block block">
<div class="one_half">
<?php
if ($data['page_content_'.$page_content_idx."_source1"] == 0) echo '<p>There is no source 1 defined for page content block '.$page_content_idx.'. Please create a new regular page and define it as your source 1 for page content block '.$page_content_idx.'.</p>';
@$p = get_page($data['page_content_'.$page_content_idx."_source1"]);
if(@$data['page_content_'.$page_content_idx."_showtitle"] == 1) echo "<h2>".$p->post_title."</h2>";
if (isset ($p->post_content)) {
echo justlanded_content_filters(@$p->post_content, "page_content");
}
else {
echo "A referenced page for this section  (Page Content Block ".$page_content_idx.") does not exist.";
}
?>
</div>
<div class="one_half last">
<?php
if ($data['page_content_'.$page_content_idx."_source2"] == 0) echo '<p>There is no source 2 defined for page content block '.$page_content_idx.'. Please create a new regular page and define it as your source 2 for page content block '.$page_content_idx.'.</p>';
@$p = get_page($data['page_content_'.$page_content_idx."_source2"]);
if(@$data['page_content_'.$page_content_idx."_showtitle"] == 1) echo "<h2>".$p->post_title."</h2>";
if (isset ($p->post_content)) {
echo justlanded_content_filters(@$p->post_content, "page_content");
}
else {
echo "A referenced page for this section  (Page Content Block ".$page_content_idx.") does not exist.";
}
?>
</div>
</section>
<?php if (isset($is_landing_page_o) && $is_landing_page_o == true) { ?></div><?php } ?>
<!--End of Page Content Block <?php echo $page_content_idx; ?>-->
<?php } ?>
<?php $is_landing_page = $is_landing_page_o; ?>