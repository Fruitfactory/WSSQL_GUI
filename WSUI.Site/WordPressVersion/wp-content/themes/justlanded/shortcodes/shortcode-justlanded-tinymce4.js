(function() {
	tinymce.PluginManager.add('justlanded', function ( editor ) {
		editor.addButton('justlanded', {
			type: 'menubutton',
			text: 'JustLanded',
			onselect: function(e) {
				// do nothing
			},
			icon: false,
			menu: [
				{
					text: 'Layout Columns',
					onclick: function()
					{
						// do nothing
					},
					menu: [
						{
							text: 'One Half',
							onclick: function() {
								editor.insertContent('[one_half last="no"][/one_half]');
							}
						},
						{
							text: 'One Third',
							onclick: function() {
								editor.insertContent('[one_third last="no"][/one_third]');
							}
						},
						{
							text: 'Two Thirds',
							onclick: function() {
								editor.insertContent('[two_thirds last="no"][/two_thirds]');
							}

						},
						{
							text: 'Three Fourths',
							onclick: function() {
								editor.insertContent('[three_fourths last="no"][/three_fourths]');
							}
						},
						{
							text: 'One Fourth',
							onclick: function() {
								editor.insertContent('[one_fourth last="no"][/one_fourth]');
							}
						}
					]
				},
				{
					text: 'Landing Page Blocks',
					menu: [
						{
							text: 'Custom Block Generator',
							onclick: function()
							{
								//editor.insertContent('Sub Menu item 1');
								editor.windowManager.open({
									title: "Custom Landing Page Block Generator",
									url: justlanded_theme_base_url + "/shortcodes/block_generator-tinymce4.php",
									width: 700,
									height: 600
								});

							}
						},
						{
							text: '|'
						},
						{
							text: 'Newsletter Sign-Up Form',
							onclick: function() {
								editor.insertContent('[landing_block type="newsletter"]');
							}
						},
						{
							text: 'Pricing Table',
							onclick: function() {
								editor.insertContent('[landing_block type="pricing_table"]');
							}
						},
						{
							text: 'Featured Testimonial',
							onclick: function() {
								editor.insertContent('[landing_block type="big_testimonial"]');
							}
						},
						{
							text: 'Featured Testimonials Slider',
							onclick: function() {
								editor.insertContent('[landing_block type="big_testimonial_slider"]');
							}
						},
						{
							text: 'Small Testimonials',
							onclick: function() {
								editor.insertContent('[landing_block type="small_testimonials"]');
							}
						},
						{
							text: 'List of Features',
							onclick: function() {
								editor.insertContent('[landing_block type="features"]');
							}
						},
						{
							text: 'Gallery',
							onclick: function() {
								editor.insertContent('[landing_block type="gallery"]');
							}
						},
						{
							text: 'Payment Options',
							onclick: function() {
								editor.insertContent('[landing_block type="payment"]');
							}
						},
						{
							text: 'Action Buttons (Block)',
							onclick: function() {
								editor.insertContent('[landing_block type="cta_buttons"]');
							}
						},
						{
							text: 'Action Buttons (Plain)',
							onclick: function() {
								editor.insertContent('[landing_block type="cta_buttons_plain"]');
							}
						},




					]
				},
				{
					text: 'List Styles',
					menu: [
						{
							text: 'Check',
							onclick: function() {
								editor.insertContent('[list style="check"][/list]');
							}
						},
						{
							text: 'Plus',
							onclick: function() {
								editor.insertContent('[list style="plus"][/list]');
							}
						},
						{
							text: 'Star',
							onclick: function() {
								editor.insertContent('[list style="star"][/list]');
							}
						},
						{
							text: 'Caution',
							onclick: function() {
								editor.insertContent('[list style="caution"][/list]');
							}
						},
						{
							text: 'Info',
							onclick: function() {
								editor.insertContent('[list style="info"][/list]');
							}
						},
						{
							text: 'Heart',
							onclick: function() {
								editor.insertContent('[list style="heart"][/list]');
							}
						},
						{
							text: 'Calendar',
							onclick: function() {
								editor.insertContent('[list style="calendar"][/list]');
							}
						},
						{
							text: 'Pin',
							onclick: function() {
								editor.insertContent('[list style="pin"][/list]');
							}
						},
						{
							text: 'Arrow',
							onclick: function() {
								editor.insertContent('[list style="arrow"][/list]');
							}
						},
						{
							text: 'Delete',
							onclick: function() {
								editor.insertContent('[list style="delete"][/list]');
							}
						},

					]
				}
			]
		});
	});
})();