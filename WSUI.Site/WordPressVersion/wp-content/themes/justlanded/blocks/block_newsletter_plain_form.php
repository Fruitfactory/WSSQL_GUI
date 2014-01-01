<?php if (@$data['newsletter_select'] == "mailchimp") { ?>
    <!--Start of Newsletter (MailChimp)-->
    <form action="<?php echo @$data['newsletter_mailchimp_url']; ?>" method="post" name="mc-embedded-subscribe-form">
        <?php if(isset($data['newsletter_mailchimp_u'])) { ?><input type="hidden" name="u" value="<?php echo @$data['newsletter_mailchimp_u']; ?>"><?php } ?>
        <?php if(isset($data['newsletter_mailchimp_id'])) { ?><input type="hidden" name="id" value="<?php echo @$data['newsletter_mailchimp_id']; ?>"><?php } ?>
        <input type="text" name="EMAIL" class="email" value="<?php echo @$data['newsletter_placeholder']; ?>" onBlur="if(this.value == '') { this.value = '<?php echo @$data['newsletter_placeholder']; ?>'; }" onFocus="if(this.value == '<?php echo @$data['newsletter_placeholder']; ?>') { this.value = ''; }" />
        <input class="submit gradient" type="submit" value="<?php echo @$data['newsletter_button']; ?>" name="subscribe" />
    </form>
    <!--End of Newsletter-->
<?php } elseif (@$data['newsletter_select'] == "aweber") { ?>
    <!--Start of Newsletter (AWeber)-->
    <form method="post" class="af-form-wrapper" action="<?php echo @$data['newsletter_aweber_url']; ?>"  >
        <?php if(isset($data['newsletter_aweber_meta_web_form_id'])) { ?><input type="hidden" name="meta_web_form_id" value="<?php echo @$data['newsletter_aweber_meta_web_form_id']; ?>" /><?php } ?>
        <?php if(isset($data['newsletter_aweber_meta_split_id'])) { ?><input type="hidden" name="meta_split_id" value="<?php echo @$data['newsletter_aweber_meta_split_id']; ?>" /><?php } ?>
        <?php if(isset($data['newsletter_aweber_listname'])) { ?><input type="hidden" name="listname" value="<?php echo @$data['newsletter_aweber_listname']; ?>" /><?php } ?>
        <?php if(isset($data['newsletter_aweber_redirect'])) { ?><input type="hidden" name="redirect" value="<?php echo @$data['newsletter_aweber_redirect']; ?>" /><?php } ?>
        <?php if(isset($data['newsletter_aweber_redirect_onlist'])) { ?><input type="hidden" name="meta_redirect_onlist" value="<?php echo @$data['newsletter_aweber_redirect_onlist']; ?>" /><?php } ?>
        <?php if(isset($data['newsletter_aweber_meta_adtracking'])) { ?><input type="hidden" name="meta_adtracking" value="<?php echo @$data['newsletter_aweber_meta_adtracking']; ?>" /><?php } ?>
        <?php if(isset($data['newsletter_aweber_meta_message'])) { ?><input type="hidden" name="meta_message" value="<?php echo @$data['newsletter_aweber_meta_message']; ?>" /><?php } ?>
        <input type="hidden" name="meta_required" value="email" />
        <input type="hidden" name="meta_tooltip" value="" />
        <?php if(isset($data['newsletter_placeholder'])) { ?><input type="text" name="email" class="email" value="<?php echo addslashes(@$data['newsletter_placeholder']); ?>" onBlur="if(this.value == '') { this.value = '<?php echo addslashes(@$data['newsletter_placeholder']); ?>'; }" onFocus="if(this.value == '<?php echo addslashes(@$data['newsletter_placeholder']); ?>') { this.value = ''; }" /><?php } ?>
        <input class="submit gradient" type="submit" value="<?php echo @$data['newsletter_button']; ?>" name="submit" />
    </form>
    <!--End of Newsletter-->
<?php } else { ?>
    <!--Start of Newsletter (Custom) -->
    <?php echo @$data['newsletter_custom']; ?>
    <!--End of Newsletter-->
<?php } ?>
