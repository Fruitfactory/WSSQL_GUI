<script>
    function justlanded_insert_block(block) {
        tinyMCE.activeEditor.execCommand('mceInsertContent', false, block);
        tb_remove();
    }
</script>
<style type="text/css">
    ul li a.button {
        width:210px;
    }
</style>
<div class="metabox-holder" style="margin-top:20px;">
    <div id="form-sortables" class="meta-box-sortables ui-sortable">
        <div id="formdiv" class="postbox">
            <div class="handlediv" title="Click to toggle"><br></div>
            <h3 class="hndle"><span>Select a landing page block</span></h3>
            <div class="inside">
                <div>
                    <ul class="justlanded_blocks">
                        <li><a class="button button-large" href="javascript:void(0);" onclick="justlanded_insert_block('[landing_block type=&quot;newsletter&quot;]');">Newsletter Sign-Up Form</a></li>
                        <li><a class="button button-large" href="javascript:void(0);" onclick="justlanded_insert_block('[landing_block_custom type=&quot;pricing_table&quot; title=&quot;&quot; subtitle=&quot;&quot; currency=&quot;$&quot;]<br>[item title=&quot;&quot; subtitle=&quot;&quot; link=&quot;#&quot; description=&quot;&quot; price=&quot;0&quot; badge=&quot;&quot; highlighted=&quot;no&quot; button=&quot;Buy Me&quot;]<br>[/landing_block_custom]');">Pricing Table</a></li>
                        <li><a class="button button-large" href="javascript:void(0);" onclick="justlanded_insert_block('[landing_block_custom type=&quot;big_testimonial&quot; quote=&quot;&quot; name=&quot;&quot; title=&quot;&quot;]');">Featured Testimonial</a></li>
                        <li><a class="button button-large" href="javascript:void(0);" onclick="justlanded_insert_block('[landing_block_custom type=&quot;big_testimonial_slider&quot;]<br>[item title=&quot;&quot; subtitle=&quot;&quot; url=&quot;&quot; description=&quot;&quot;]<br>[/landing_block_custom]');">Featured Testimonials Slider</a></li>
                        <li><a class="button button-large" href="javascript:void(0);" onclick="justlanded_insert_block('[landing_block_custom type=&quot;small_testimonials&quot; title=&quot;&quot; subtitle=&quot;&quot;]<br>[item title=&quot;&quot; subtitle=&quot;&quot; url=&quot;&quot; description=&quot;&quot;]<br>[/landing_block_custom]');">Small Testimonials</a></li>
                        <li><a class="button button-large" href="javascript:void(0);" onclick="justlanded_insert_block('[landing_block_custom type=&quot;features&quot; title=&quot;&quot; subtitle=&quot;&quot;]<br>[item title=&quot;&quot; subtitle=&quot;&quot; link=&quot;#&quot; description=&quot;&quot; url=&quot;&quot;]<br>[/landing_block_custom]');">List of Features</a></li>
                        <li><a class="button button-large" href="javascript:void(0);" onclick="justlanded_insert_block('[landing_block_custom type=&quot;gallery&quot; title=&quot;&quot; subtitle=&quot;&quot;]<br>[item title=&quot;&quot; subtitle=&quot;&quot; description=&quot;&quot; url=&quot;&quot;]<br>[/landing_block_custom]');">Gallery</a></li>
                        <li><a class="button button-large" href="javascript:void(0);" onclick="justlanded_insert_block('[landing_block type=&quot;payment&quot;]');">Payment Options</a></li>
                    </ul>
                </div>
                <br class="clear">
            </div>
        </div>
    </div>
    <p style="font-size:11px;">
        Remember that you do not have to define your landing page blocks through shortcodes. You can always use the options panel to change the content of these blocks,
        and you can still use a <strong>[landing_block type="..."]</strong> style shortcode to insert that block into a page. If a block's content is not defined through the
        shortcode, content is pulled from the current page's active settings profile, or the site default profile.
    </p>
</div>