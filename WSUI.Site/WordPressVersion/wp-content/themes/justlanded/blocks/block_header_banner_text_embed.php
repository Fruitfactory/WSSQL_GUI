<!--Start of Product Banner Description Embed -->
<h1><?php echo @nl2br(do_shortcode(stripslashes(@$data['banner_title'])));?></h1>
<p><?php echo @nl2br(do_shortcode(stripslashes(@$data['banner_text'])));?></p>

<?php
if (@$data['banner_bullets'] != "") {
    echo "<ul class=\"ticks\">\n";
    $bullets = explode("\n", trim(@$data['banner_bullets']));
    if (is_array($bullets)) {
        foreach (@$bullets as $bullet)
        {
            echo "  <li>" . do_shortcode(stripslashes(trim($bullet))) . "</li>\n";
        }
    }
    echo "</ul>\n";
}
?>
<!--End of Product Banner Description Embed -->
