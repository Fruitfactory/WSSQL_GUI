<?php if (@$data['header_phone'] != "") {
    if (isset ($data['header_phone']))
    {
        $phone_link = stripslashes($data['header_phone']);
        $phone_link = str_replace("-", "", $phone_link);
        $phone_link = str_replace(".", "", $phone_link);
        $phone_link = str_replace(" ", "", $phone_link);
    }
    else {
        $phone_link = "";
    }

    if (isset ($data['header_phone_vanity']) && $data['header_phone_vanity'] != "") {
        $data['header_phone'] = $data['header_phone_vanity'];
    }

?>
<!--Start of Phone-->
<aside id="phone" class="one_third last"><?php echo @$data['header_info_label']; ?> <a href="tel:<?php echo $phone_link; ?>" title="<?php echo @$data['header_info_label']; ?>"><?php echo @$data['header_phone']; ?></a></aside>
<!--End of Phone-->
<?php
} elseif (@$data['header_email'] != "") {
?>
<!--Start of Email-->
<aside id="phone" class="one_third last"><?php echo @$data['header_info_label']; ?> <a href="mailto:<?php echo do_shortcode(stripslashes(@$data['header_email'])); ?>" title="<?php echo @$data['header_info_label']; ?>"><?php echo @$data['header_email']; ?></a></aside>
<!--End of Email-->
<?php
} else {
?>
<!--Placeholder for phone number or email address-->
<?php
}
?>