<?php $page_content_idx = 2; ?>
<?php
if (isset($data['page_content_'.$page_content_idx.'_nowrapper']) && $data['page_content_'.$page_content_idx.'_nowrapper'] == 1) {
    require("block_page_content_nowrapper.php");
} else {
    require("block_page_content.php");
}
?>