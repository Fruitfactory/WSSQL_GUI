<?php global $is_landing_page; ?>
<?php if(@$data['page_content_'.$page_content_idx.'_layout'] == "fullwidth") { ?>
<?php if(@$data['page_content_'.$page_content_idx."_source1"] != 0) { ?>
<!--Start of Page Content Block <?php echo $page_content_idx; ?>-->
<div id="page-content-block-<?php echo $page_content_idx; ?>" class="page-content-block-nowrapper block-nowrapper">
<?php
@$p = get_page($data['page_content_'.$page_content_idx."_source1"]);
if (isset ($p->post_content)) {
echo justlanded_content_filters(@$p->post_content);
}
else {
    echo "A referenced page for this section  (Page Content Block ".$page_content_idx.") does not exist.";
}
?>
</div>
<!--End of Page Content Block <?php echo $page_content_idx; ?>-->
<?php } else { ?>
<!--Start of Page Content Block <?php echo $page_content_idx; ?>-->
<div id="page-content-block-<?php echo $page_content_idx; ?>" class="page-content-block-nowrapper block-nowrapper">
    There is no content to display. (Page Content Block <?php echo $page_content_idx; ?> does not have a source 1)
</div>
<!--End of Page Content Block <?php echo $page_content_idx; ?>-->
<?php
}
?>
<?php } else { ?>
<!--Start of Page Content Block <?php echo $page_content_idx; ?>-->
<div id="page-content-block-<?php echo $page_content_idx; ?>" class="page-content-block-nowrapper block-nowrapper">
<div class="one_half">
<?php
if ($data['page_content_'.$page_content_idx."_source1"] == 0) echo '<p>There is no source 1 defined for page content block '.$page_content_idx.'. Please create a new regular page and define it as your source 1 for page content block '.$page_content_idx.'.</p>';
@$p = get_page($data['page_content_'.$page_content_idx."_source1"]);
if (isset ($p->post_content)) {
echo justlanded_content_filters(@$p->post_content);
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
if (isset ($p->post_content)) {
echo justlanded_content_filters(@$p->post_content);
}
else {
echo "A referenced page for this section  (Page Content Block ".$page_content_idx.") does not exist.";
}
?>
</div>
</div>
<!--End of Page Content Block <?php echo $page_content_idx; ?>-->
<?php } ?>