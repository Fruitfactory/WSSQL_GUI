<html>
<head>
	<script>
		function justlanded_insert_block(block) {
			top.tinymce.activeEditor.execCommand('mceInsertContent', false, block);
			top.tinymce.activeEditor.windowManager.close();
		}
	</script>
	<style type="text/css">
	#wpwrap {
		height: auto;
		min-height: 100%;
		width: 100%;
		position: relative;
		-webkit-font-smoothing: subpixel-antialiased;
	}

	#wpcontent {
		height: 100%;
	}

	#wpcontent,
	#wpfooter {
		margin-left: 180px;
	}

	.folded #wpcontent,
	.folded #wpfooter {
		margin-left: 56px;
	}

	#wpbody-content {
		padding-bottom: 65px;
		float: left;
		width: 100%;
		overflow: visible !important;
	}

	/* inner 2 column liquid layout */

	.inner-sidebar {
		float: right;
		clear: right;
		display: none;
		width: 281px;
		position: relative;
	}

	.columns-2 .inner-sidebar {
		margin-right: auto;
		width: 286px;
		display: block;
	}

	.inner-sidebar #side-sortables,
	.columns-2 .inner-sidebar #side-sortables {
		min-height: 300px;
		width: 280px;
		padding: 0;
	}

	.has-right-sidebar .inner-sidebar {
		display: block;
	}

	.has-right-sidebar #post-body {
		float: left;
		clear: left;
		width: 100%;
		margin-right: -2000px;
	}

	.has-right-sidebar #post-body-content {
		margin-right: 300px;
		float: none;
		width: auto;
	}

	/* general */
	html,
	body {
		height: 100%;
		margin: 0;
		padding: 0;
	}

	html {
		background: #f1f1f1;
	}

	body {
		color: #444;
		font-family: "Open Sans", sans-serif;
		font-size: 13px;
		line-height: 1.4em;
		min-width: 600px;
	}

	a {
		color: #0074a2;
		-webkit-transition-property: border, background, color;
		transition-property: border, background, color;
		-webkit-transition-duration: .05s;
		transition-duration: .05s;
		-webkit-transition-timing-function: ease-in-out;
		transition-timing-function: ease-in-out;
	}

	a,
	div {
		outline: 0;
	}

	a:hover,
	a:active {
		color: #2ea2cc;
	}

	a:focus {
		color: #124964;
	}

	a:focus,
	a:active {
		outline: thin dotted;
	}

	p {
		font-size: 13px;
		line-height: 1.5;
		margin: 1em 0;
	}

	h1,
	h2,
	h3,
	h4,
	h5,
	h6 {
		display: block;
		font-weight: 600;
	}

	h1 {
		font-size: 2em;
		margin: .67em 0;
	}

	h2 {
		color: #222;
		font-size: 1.5em;
		margin: .83em 0;
		font-weight: 400;
	}

	h3 {
		color: #222;
		font-size: 1.3em;
		margin: 1em 0;
	}

	h4 {
		font-size: 1em;
		margin: 1.33em 0;
	}

	h5 {
		font-size: 0.83em;
		margin: 1.67em 0;
	}

	h6 {
		font-size: 0.67em;
		margin: 2.33em 0;
	}

	ul,
	ol {
		padding: 0;
	}

	ul {
		list-style: none;
	}

	ol {
		list-style-type: decimal;
		margin-left: 2em;
	}

	ul.ul-disc {
		list-style: disc outside;
	}

	ul.ul-square {
		list-style: square outside;
	}

	ol.ol-decimal {
		list-style: decimal outside;
	}

	ul.ul-disc,
	ul.ul-square,
	ol.ol-decimal {
		margin-left: 1.8em;
	}

	ul.ul-disc > li,
	ul.ul-square > li,
	ol.ol-decimal > li {
		margin: 0 0 0.5em;
	}

	.wrap h2 {
		font-size: 23px;
		font-weight: 400;
		padding: 9px 15px 4px 0;
		line-height: 29px;
	}

	.subtitle {
		color: #777;
		font-size: 14px;
		padding-left: 25px;
	}

	.wrap .add-new-h2,
	.wrap .add-new-h2:active {
		margin-left: 4px;
		padding: 4px 8px;
		position: relative;
		top: -3px;
		text-decoration: none;
		border: none;
		-webkit-border-radius: 2px;
		border-radius: 2px;
		background: #e0e0e0;
		text-shadow: none;
		font-weight: 600;
		font-size: 13px;
	}

	.wrap .add-new-h2:hover {
		background: #2ea2cc;
		color: #fff;
	}

	.wrap h2.long-header {
		padding-right: 0;
	}

	.wp-dialog {
		background-color: #fff;
	}

	.metabox-holder {
		padding:20px;
	}
	p {
		padding:20px;
	}

	</style>
</head>
<body>
<div class="metabox-holder">
        <div id="formdiv" class="postbox">
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
	                    <li><a class="button button-large" href="javascript:void(0);" onclick="justlanded_insert_block('[landing_block_custom type=&quot;button&quot; caption=&quot;&quot; link=&quot;http://www.shapingrain.com&quot; target=&quot;_blank&quot; align=&quot;center&quot;]');">Call To Action Button</a></li>
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
</body>
</html>
