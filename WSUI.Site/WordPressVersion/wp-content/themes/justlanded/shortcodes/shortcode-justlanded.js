(function() {
    var myToolbarImageUrl = '';
    tinymce.create('tinymce.plugins.Justlanded', {
        init : function(ed, url) {
            myToolbarImageUrl = url;

            ed.addCommand('one_half', function() {
                ed.execCommand('mceReplaceContent',false,'[one_half last="no"]{$selection}[/one_half]');
            });

            ed.addCommand('one_third', function() {
                ed.execCommand('mceReplaceContent',false,'[one_third last="no"]{$selection}[/one_third]');
            });

            ed.addCommand('two_thirds', function() {
                ed.execCommand('mceReplaceContent',false,'[two_thirds last="no"]{$selection}[/two_thirds]');
            });

            ed.addCommand('three_fourths', function() {
                ed.execCommand('mceReplaceContent',false,'[three_fourths last="no"]{$selection}[/three_fourths]');
            });

            ed.addCommand('one_fourth', function() {
                ed.execCommand('mceReplaceContent',false,'[one_fourth last="no"]{$selection}[/one_fourth]');
            });

            ed.addCommand('landing_block_newsletter', function() {
                ed.execCommand('mceInsertContent',false,'[landing_block type="newsletter"]');
            });

            ed.addCommand('list', function(style) {
                ed.execCommand('mceInsertContent',false,'[list style="'+style+'"][/list]');
            });

            ed.addCommand('landing_block', function(block) {
                ed.execCommand('mceInsertContent',false,'[landing_block type="'+block+'"]');
            });


        },
        createControl : function(n, cm) {

            switch (n) {
                case 'justlanded':
                    var c = cm.createMenuButton('shortcodes', {
                        title : 'JustLanded',
                        image : myToolbarImageUrl + '/images/button-justlanded.png'
                    });

                    c.onRenderMenu.add(function(c, m) {

                        /* Layout Blocks */
                        sub = m.addMenu({title : 'Layout Columns'});
                        sub.add({
                            title : 'One Half',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-12.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('one_half');
                            }
                        });
                        sub.add({
                            title : 'One Third',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-13.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('one_third');
                            }
                        });
                        sub.add({
                            title : 'Two Thirds',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-23.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('two_thirds');
                            }
                        });
                        sub.add({
                            title : 'Three Fourths',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-34.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('three_fourths');
                            }
                        });
                        sub.add({
                            title : 'One Fourth',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-14.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('one_fourth');
                            }
                        });

                        /* Landing Page Blocks - experimental feature */
                        var sub;
                        sub = m.addMenu({title : 'Landing Page Blocks'});

                        sub.add({
                            title : 'Custom Block Generator',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-block.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tb_show("JustLanded: Custom Landing Page Block Shortcode Generator", myToolbarImageUrl + "/block_generator.php?height=550&width=600&show");
                                tinymce.DOM.setStyle(["TB_overlay", "TB_window", "TB_load"], "z-index", "999999");
                            }
                        });
                        sub.addSeparator();

                        sub.add({
                            title : 'Newsletter Sign-Up Form',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-block.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('landing_block', 'newsletter');
                            }
                        });
                        sub.add({
                            title : 'Pricing Table',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-block.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('landing_block', 'pricing_table');
                            }
                        });
                        sub.add({
                            title : 'Featured Testimonial',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-block.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('landing_block', 'big_testimonial');
                            }
                        });
                        sub.add({
                            title : 'Featured Testimonials Slider',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-block.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('landing_block', 'big_testimonial_slider');
                            }
                        });
                        sub.add({
                            title : 'Small Testimonials',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-block.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('landing_block', 'small_testimonials');
                            }
                        });
                        sub.add({
                            title : 'List of Features',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-block.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('landing_block', 'features');
                            }
                        });
                        sub.add({
                            title : 'Gallery',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-block.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('landing_block', 'gallery');
                            }
                        });
                        sub.add({
                            title : 'Payment Options',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button-block.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('landing_block', 'payment');
                            }
                        });

                        /* Styled Lists */
                        var sub;
                        sub = m.addMenu({title : 'List Styles'});
                        sub.add({
                            title : 'Check',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button_check.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('list', 'check');
                            }
                        });

                        sub.add({
                            title : 'Plus',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button_plus.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('list', 'plus');
                            }
                        });

                        sub.add({
                            title : 'Star',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button_star.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('list', 'star');
                            }
                        });

                        sub.add({
                            title : 'Caution',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button_caution.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('list', 'caution');
                            }
                        });

                        sub.add({
                            title : 'Info',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button_info.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('list', 'info');
                            }
                        });

                        sub.add({
                            title : 'Heart',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button_heart.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('list', 'heart');
                            }
                        });

                        sub.add({
                            title : 'Calendar',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button_calendar.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('list', 'calendar');
                            }
                        });

                        sub.add({
                            title : 'Pin',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button_pin.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('list', 'pin');
                            }
                        });

                        sub.add({
                            title : 'Arrow',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button_arrow.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('list', 'arrow');
                            }
                        });

                        sub.add({
                            title : 'Delete',
                            style: {
                                'backgroundImage'   : 'url('+myToolbarImageUrl+'/images/button_delete.png)',
                                'backgroundRepeat'  : 'no-repeat',
                                'backgroundPosition': '5px 0px'
                            },
                            onclick : function() {
                                tinyMCE.activeEditor.execCommand('list', 'delete');
                            }
                        });




                    });

                    // Return the new menu button instance
                    return c;
            }

            return null;
        },
        getInfo : function() {
			/*
			 * I intentionally left the information of
			 * Brett Terpstra, as his code was the
			 * foundation for this.
			*/
            return {
                longname : "JustLanded for WordPress by Shaping Rain",
                author : 'Shaping Rain',
                authorurl : 'http://www.shapingrain.com/',
                infourl : 'http://www.shapingrain.com/',
                version : "1.0"
            };
        }
    });
    tinymce.PluginManager.add('justlanded', tinymce.plugins.Justlanded);
})();