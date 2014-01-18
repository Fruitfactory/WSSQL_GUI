/**
 * ShapingRain.com: WordPress Options Panel based on SMOF
 */

/*
 * --------------------------------------------------------------------
 * jQuery-Plugin - $.download - allows for simple get/post requests for files
 * by Scott Jehl, scott@filamentgroup.com
 * http://www.filamentgroup.com
 * reference article: http://www.filamentgroup.com/lab/jquery_plugin_for_requesting_ajax_like_file_downloads/
 * Copyright (c) 2008 Filament Group, Inc
 * Dual licensed under the MIT (filamentgroup.com/examples/mit-license.txt) and GPL (filamentgroup.com/examples/gpl-license.txt) licenses.
 * --------------------------------------------------------------------
 */

jQuery.download = function(url, data, method){
    //url and data options required
    if( url && data ){
        //data can be string of parameters or array/object
        data = typeof data == 'string' ? data : jQuery.param(data);
        //split params into form inputs
        var inputs = '';
        jQuery.each(data.split('&'), function(){
            var pair = this.split('=');
            inputs+='<input type="hidden" name="'+ pair[0] +'" value="'+ pair[1] +'" />';
        });
        //send request
        jQuery('<form action="'+ url +'" method="'+ (method||'post') +'">'+inputs+'</form>')
            .appendTo('body').submit().remove();
    };
};

/**
 * Transform text into a URL slug: spaces turned into dashes, remove non alnum
 * @param string text
 * http://milesj.me/snippets/javascript/slugify
 */
function slugify(text) {
    text = text.replace(/[^-a-zA-Z0-9,&\s]+/ig, '');
    text = text.replace(/-/gi, "_");
    text = text.replace(/\s/gi, "-");
    return text;
}

/**
 * Calculating Color Contrast
 * http://24ways.org/2010/calculating-color-contrast/ with minimal changes
 */
function getContrastYIQ(hexcolor) {
    if(jQuery.browser.msie && jQuery.browser.version.substring(0, 2) == "8.") return;
    hexcolor = hexcolor.replace('#', '');
    var r = parseInt(hexcolor.substr(0, 2), 16);
    var g = parseInt(hexcolor.substr(2, 2), 16);
    var b = parseInt(hexcolor.substr(4, 2), 16);
    var yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;
    return (yiq >= 200) ? '#000000' : '#ffffff';
}

/**
 * Convert RGB into HEX color code
 */
function hexc(colorval) {
    if(jQuery.browser.msie && jQuery.browser.version.substring(0, 2) == "8.") return;
    var parts = colorval.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    delete(parts[0]);
    for (var i = 1; i <= 3; ++i) {
        parts[i] = parseInt(parts[i]).toString(16);
        if (parts[i].length == 1) parts[i] = '0' + parts[i];
    }
    var color = '#' + parts.join('');
    return color;
}

/**
 * Change background color depending on font (foreground) color
 */
function matchBackgroundColor(id) {
    var this_color = jQuery('#' + id).css('color');
    this_color = hexc(this_color);
    jQuery('#' + id).css('backgroundColor', getContrastYIQ(this_color));
}

jQuery.noConflict();

jQuery(document).ready(function ($) {
    // values for unit px
    var unit_values_px = [8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 29, 32, 35, 36, 37, 38, 40, 42, 45, 48, 50, 52, 54, 56, 58, 60, 61, 62];
    // values for unit em
    var unit_values_em = [.5, .55, .625, .7, .75, .8, .875, .95, 1, 1.05, 1.125, 1.2, 1.25, 1.3, 1.4, 1.45, 1.5, 1.6, 1.8, 2, 2.2, 2.25, 2.3, 2.35, 2.45, 2.55, 2.75, 3, 3.13, 3.25, 3.38, 3.5, 3.63, 3.75, 3.81, 3.88];

    $.ajaxSetup({ cache: false });

    $(".select").attr("autocomplete","off");
    $(".input").attr("autocomplete","off");

    //(un)fold options in a checkbox-group
    jQuery('.fld').click(function () {
        var $fold = '.f_' + this.id;
        $($fold).slideToggle('normal', "swing");
    });

    //delays until AjaxUpload is finished loading
    //fixes bug in Safari and Mac Chrome
    if (typeof AjaxUpload != 'function') {
        return ++counter < 6 && window.setTimeout(init, counter * 500);
    }

    //hides warning if js is enabled
    $('#js-warning').hide();

    //Tabify Options
    $('.group').hide();
    $('.of-submenu').hide();

    // Display last current tab
    if ($.cookie("of_current_opt") === null) {
        $('.group:first').fadeIn('fast');
        $('#of-nav li:first').addClass('current');
    } else {

        var hooks = $('#hooks').html();
        hooks = jQuery.parseJSON(hooks);

        $.each(hooks, function (key, value) {

            if ($.cookie("of_current_opt") == '#of-option-' + value) {
                $('.group#of-option-' + value).fadeIn();
                $('#of-nav li.' + value).addClass('current');
                $('.of-parent-' + value).show();

                var this_base_id = $('#of-nav li.' + value).attr('class');
                var classes = this_base_id.split(' ');
                $.each(classes, function () {
                    if (this.indexOf('of-parent-') > -1) {
                        var this_parent_id = this;
                        this_parent_id = this_parent_id.replace('of-parent-', '');
                        $('.' + this_parent_id).addClass('current-parent');
                        $('.of-parent-' + this_parent_id).show();
                        return false;
                    }
                });
            }

        });

    }

    //Current Menu Class
    $('#of-nav li a').click(function (evt) {
        // event.preventDefault();

        if ($(this).parent().hasClass('of-submenu') == false) {
            if ($(this).parent().hasClass('current') == false && $(this).parent().hasClass('current-parent') == false) {
                $('.of-submenu').slideUp('fast');
            }
            $('.current-parent').removeClass('current-parent');
        }

        var this_base_id = $(this).parent().attr('class');
        this_base_id = this_base_id.replace(' current', '');
        this_base_id = this_base_id.replace(' ', '');
        $('li.of-parent-' + this_base_id).slideDown('fast');

        if ($(this).parent().hasClass('of-submenu')) {
            var classes = this_base_id.split(' ');
            $.each(classes, function () {
                if (this.indexOf('of-parent-') > -1) {
                    var this_parent_id = this;
                    this_parent_id = this_parent_id.replace('of-parent-', '');
                    $('.' + this_parent_id).addClass('current-parent');
                    return false;
                }
            });
        }

        $('#of-nav li').removeClass('current');

        $(this).parent().addClass('current');

        if ($(this).hasClass('of-redirect')) {
            $('#of-nav ul').find('li.of-parent-' + this_base_id + ' a').first().click();
            return false;
        }


        var clicked_group = $(this).attr('href');

        $.cookie('of_current_opt', clicked_group, {
            expires:7,
            path:'/'
        });

        $('.group').hide();

        $(clicked_group).fadeIn('fast');
        return false;

    });

    //Expand Options
    var flip = 0;
    $('#expand_options').click(function () {
        if (flip == 0) {
            flip = 1;
            $('#of_container #of-nav').hide();
            $('#of_container #content').width(755);
            $('#of_container .group').add('#of_container .group h2').show();

            $(this).removeClass('expand');
            $(this).addClass('close');
            $(this).text('Close');
        } else {
            flip = 0;
            $('#of_container #of-nav').show();
            $('#of_container #content').width(595);
            $('#of_container .group').add('#of_container .group h2').hide();
            $('#of_container .group:first').show();
            $('#of_container #of-nav li').removeClass('current');
            $('#of_container #of-nav li:first').addClass('current');

            $(this).removeClass('close');
            $(this).addClass('expand');
            $(this).text('Expand');
        }
    });

    //Toggle Hints
    var hints_flip = 0;
    $('#show_hints').click(function () {
        if (hints_flip == 0) {
            hints_flip = 1;
            $('.explain').hide();
            $('.section-info').hide();
            $('.controls .can_toggle').width(595);

            $(this).removeClass('expand');
            $(this).addClass('close');
            $(this).text('Hide hints');
        } else {
            hints_flip = 0;
            $('.explain').show();
            $('.section-info').show();
            $('.controls .can_toggle').width(345);

            $(this).removeClass('close');
            $(this).addClass('expand');
            $(this).text('Show hints');
        }
    });


    //Update Message popup
    $.fn.center = function () {
        this.animate({
            "top":($(window).height() - this.height() - 200) / 2 + $(window).scrollTop() + "px"
        }, 100);
        this.css("left", 250);
        return this;
    }


    $('#of-popup-save').center();
    $('#of-popup-reset').center();
    $('#of-popup-fail').center();

    $(window).scroll(function () {
        $('#of-popup-save').center();
        $('#of-popup-reset').center();
        $('#of-popup-fail').center();
    });


    //ShapingRain.com Google Web Fonts Selection
    $('.of-typography-face').change(function () {
        var nonce = $('#security').val();

        var this_base_id = $(this).attr('id');
        this_base_id = this_base_id.replace('_face', '');

        var this_font = $(this).val();
        var this_style = $('#' + this_base_id + '_style');
        var this_id = $(this).attr('id');
        if (this_font.indexOf('*') === -1) {
            $.post(ajaxurl, {type:'webfonts', action:'of_ajax_post_action', security:nonce, font:$(this).val()}, function (data) {
                var data = jQuery.parseJSON(data);
                var variants = data.variants;
                var subsets = data.subsets;
                var this_font_styles = '';
                var this_font_style = '';
                this_style.empty();
                if (variants.length > 0) {
                    this_font_styles = variants.join(',');
                    $.each(variants, function (key, val) {
                        this_style.append('<option value="' + val + '">' + val + '</option>');
                    });
                    this_style.find('option[value=' + variants[0] + ']').attr('selected', 'selected');
                    var this_font_style = variants[0];
                }
                $("#" + this_base_id + "_subset").empty();
                if (subsets.length > 1) {
                    $.each(subsets, function (key, val) {
                        $("#" + this_base_id + "_subset").append('<input type="checkbox" class="checkbox of-input" name="' + this_base_id + '[subset][' + val + ']" id="' + this_base_id + '_subset_' + val + '" value="1"><label class="multicheck" for="' + this_base_id + '_subset_' + val + '">' + val + '</label>');
                    });
                }

                if ($('#' + slugify(this_font) + "_preview_style").length == 0) {
                    $("<link/>", {
                        id:slugify(this_font) + "_preview_style",
                        rel:"stylesheet",
                        type:"text/css",
                        href:"http://fonts.googleapis.com/css?family=" + this_font.replace(/\s+/g, '+') + ":regular,100,200,300,400,500,600,700,800,900,100italic,200italic,300italic,400italic,500italic,600italic,700italic,800italic,900italic"
                    }).appendTo("head");
                }
                $('#' + this_base_id + '_preview').css('fontFamily', this_font);
            });
        }
        else {
            this_style.empty();
            this_style.append('<option value="regular">regular</option>');
            this_style.append('<option value="bold">bold</option>');
            this_style.append('<option value="italic">italic</option>');
            this_style.append('<option value="bold italic">bold italic</option>');
            this_style.find('option[value=regular]').attr('selected', 'selected');

            this_font = this_font.replace("*", "");
            $('#' + this_base_id + '_preview').css('fontFamily', this_font);
        }

        $('#' + this_base_id + '_preview').css('fontWeight', 'normal');
        $('#' + this_base_id + '_preview').css('fontStyle', 'normal');


    });

    //ShapingRain.com Google Web Fonts Style (regular, bold...) Selection
    $('.of-typography-style').change(function () {
        var this_base_id = $(this).attr('id');
        this_base_id = this_base_id.replace('_style', '');
        var this_font = $('#' + this_base_id + '_face').val();
        var this_style = $(this).val();

        $('#' + this_base_id + '_preview').css('fontWeight', 'normal');
        $('#' + this_base_id + '_preview').css('fontStyle', 'normal');

        var weight = parseInt(this_style);
        if (weight == 'NaN') weight = '';

        if (this_style.indexOf('italic') != -1) {
            var style = 'italic';
        }
        else {
            var style = '';
        }

        if (this_style.indexOf('bold') != -1) {
            var weight = 'bold';
        }

        if (this_style == 'regular') {
            style = "normal";
            weight = "normal";
        }

        if (weight == "") weight = 'normal';
        $('#' + this_base_id + '_preview').css('fontWeight', weight);

        if (style == "") style = 'normal';
        $('#' + this_base_id + '_preview').css('fontStyle', style);


    });

    //ShapingRain.com Google Web Fonts Size Selection
    $('.of-typography-size').change(function () {
        var this_base_id = $(this).attr('id');
        this_base_id = this_base_id.replace('_size', '');
        var this_unit = $('#' + this_base_id + '_unit').val();
        var this_size = $(this).val();
        $('#' + this_base_id + '_preview').css('fontSize', this_size + this_unit);
    });

    //ShapingRain.com Google Web Fonts Size Unit Selection
    $('.of-typography-unit').change(function () {
        var this_base_id = $(this).attr('id');
        this_base_id = this_base_id.replace('_unit', '');
        var this_unit = $(this).val();
        var this_size = $('#' + this_base_id + '_size').val();
        var this_size_select = $('#' + this_base_id + '_size');

        if (this_unit == "em") {
            var cur_size_index = unit_values_px.indexOf(parseFloat(this_size));
            this_size_select.empty();

            var length = unit_values_em.length,
                element = null;
            for (var i = 0; i < length; i++) {
                this_size_select.append('<option value="' + unit_values_em[i] + '">' + unit_values_em[i] + '</option>');
            }
            $('#' + this_base_id + '_size').val(unit_values_em[cur_size_index]);
            $('#' + this_base_id + '_preview').css('fontSize', unit_values_em[cur_size_index] + this_unit);
        } else {
            var cur_size_index = unit_values_em.indexOf(parseFloat(this_size));
            this_size_select.empty();

            var length = unit_values_px.length,
                element = null;
            for (var i = 0; i < length; i++) {
                this_size_select.append('<option value="' + unit_values_px[i] + '">' + unit_values_px[i] + '</option>');
            }
            $('#' + this_base_id + '_size').val(unit_values_px[cur_size_index]);
            $('#' + this_base_id + '_preview').css('fontSize', unit_values_px[cur_size_index] + this_unit);
        }

    });

    //ShapingRain.com Google Web Fonts Color Selection
    $('input.of-typography-color').miniColors({
        change:function (hex, rgba) {
            $(this).change();
        }
    });

    $('.of-typography-color').change(function () {
        var this_base_id = $(this).attr('id');
        this_base_id = this_base_id.replace('_color', '');
        var this_color = $(this).val();
        var this_preview = $('#' + this_base_id + '_preview');
        this_preview.css('color', this_color);
        matchBackgroundColor(this_base_id + "_preview");
    });

    //ShapingRain.com Apply Font Previews (Initialization)
    $('.of-typography-face').each(function (index) {
        var this_base_id = $(this).attr('id');
        this_base_id = this_base_id.replace('_face', '');
        var this_font = $(this).val();
        var this_style = $('#' + this_base_id + '_style').val();
        var this_color = $('#' + this_base_id + '_color').val();
        var this_size = $('#' + this_base_id + '_size').val();
        var this_unit = $('#' + this_base_id + '_unit').val();

        if (this_font.indexOf('*') === -1) {
            if ($('#' + slugify(this_font) + "_preview_style").length == 0) {
                $("<link/>", {
                    id:slugify(this_font) + "_preview_style",
                    rel:"stylesheet",
                    type:"text/css",
                    href:"http://fonts.googleapis.com/css?family=" + this_font.replace(/\s+/g, '+') + ":regular,100,200,300,400,500,600,700,800,900,100italic,200italic,300italic,400italic,500italic,600italic,700italic,800italic,900italic"
                }).appendTo("head");
            }
        }
        else {
            this_font = this_font.replace("*", "");
        }

        $('#' + this_base_id + '_preview').css('fontFamily', this_font);
        $('#' + this_base_id + '_preview').css('color', this_color);
        $('#' + this_base_id + '_preview').css('fontSize', this_size + this_unit);


        if (this_style == undefined) {
            $('#' + this_base_id + '_preview').css('fontWeight', 'normal');
            $('#' + this_base_id + '_preview').css('fontStyle', 'normal');
        }
        else {
            $('#' + this_base_id + '_preview').css('fontWeight', 'normal');
            $('#' + this_base_id + '_preview').css('fontStyle', 'normal');

            var weight = parseInt(this_style);
            if (weight == 'NaN') weight = '';

            if (this_style.indexOf('italic') != -1) {
                var style = 'italic';
            }
            else {
                var style = '';
            }

            if (this_style.indexOf('bold') != -1) {
                var weight = 'bold';
            }

            if (this_style == 'regular') {
                style = "normal";
                weight = "normal";
            }

            if (weight == "") weight = 'normal';
            $('#' + this_base_id + '_preview').css('fontWeight', weight);

            if (style == "") style = 'normal';
            $('#' + this_base_id + '_preview').css('fontStyle', style);
        }

        matchBackgroundColor(this_base_id + "_preview");
    });

    // Select Profile
    $('#select_profile_value').live('change', function () {
        var selectedProfile = $(this).find(":selected").val();
        var nonce = $('#security').val();

        var success_popup = $('#of-popup-save');
		$('.ajax-loading-img').fadeIn();
        success_popup.find('.of-save-save').html("Switching active profile...");
        success_popup.fadeIn();

        $.ajax({
            type:'POST',
            data:{
                action:'of_ajax_post_action',
                type:'profile',
                profile:selectedProfile,
                security:nonce
            },
            url:ajaxurl,
            success:function (data, textStatus) {
				$('.ajax-loading-img').fadeOut();
				window.setTimeout(function () {
                    location.reload();
                }, 100);

            },
            error:function (xhr, textStatus, errorThrown) {
				$('.ajax-loading-img').fadeOut();
                alert("Could not change profile. Reloading page.");
				location.reload();
            }
        });


    });

    // Swatches
    $('.swatch').each(function (index) {
        var scolor = $(this).attr('title');
        $(this).css('backgroundColor', scolor);
    });

    $('.preset').live('click', function() {
        var this_title = $(this).attr('title');
        var this_id    = $(this).attr('id');
        var answer = confirm("Click OK to import preset '"+this_title+"'. Custom font and color settings of the active profile will be overwritten.");
        if (answer) {
			$('.ajax-loading-img').fadeIn();

            var clickedObject = $(this);
            var clickedID = $(this).attr('id');

            var this_base_id = $(this).attr('id');
            this_base_id = this_base_id.replace('of_import_button_', '');

            var nonce = $('#security').val();
            var profile = $('#this_profile').val();

            var data = {
                action:'of_ajax_post_action',
                type:'import_preset',
                security:nonce,
                profile:profile,
                file:this_id,
                folder:'colors'
            };

            $.post(ajaxurl, data, function (response) {
				$('.ajax-loading-img').fadeOut();
                var fail_popup = $('#of-popup-fail');
                var success_popup = $('#of-popup-save');

                //check nonce
                if (response == -1) { //failed
                    fail_popup.fadeIn();
                    window.setTimeout(function () {
                        fail_popup.fadeOut();
                    }, 2000);
                } else {
					var success_popup = $('#of-popup-save');
					success_popup.find('.of-save-save').html("Preset has been imported.");

                    success_popup.fadeIn();
                    window.setTimeout(function () {
                        location.reload();
                    }, 1000);
                }
            });
        }
        return false;



    });

    //Masked Inputs (images as radio buttons)
    $('.of-radio-img-img').click(function () {
        $(this).parent().parent().find('.of-radio-img-img').removeClass('of-radio-img-selected');
        $(this).addClass('of-radio-img-selected');
    });
    $('.of-radio-img-label').hide();
    $('.of-radio-img-img').show();
    $('.of-radio-img-radio').hide();

    //Masked Inputs (background images as radio buttons)
    $('.of-radio-tile-img').click(function () {
        $(this).parent().parent().find('.of-radio-tile-img').removeClass('of-radio-tile-selected');
        $(this).addClass('of-radio-tile-selected');
    });
    $('.of-radio-tile-label').hide();
    $('.of-radio-tile-img').show();
    $('.of-radio-tile-radio').hide();

    //AJAX Upload
    function of_image_upload() {
        $('.image_upload_button').each(function () {

            var clickedObject = $(this);
            var clickedID = $(this).attr('id');

            var nonce = $('#security').val();
            var profile = $('#this_profile').val();

            new AjaxUpload(clickedID, {
                action:ajaxurl,
                name:clickedID, // File upload name
                data:{ // Additional data to send
                    action:'of_ajax_post_action',
                    type:'upload',
                    profile:profile,
                    security:nonce,
                    data:clickedID
                },
                autoSubmit:true, // Submit file after selection
                responseType:false,
                onChange:function (file, extension) {
                },
                onSubmit:function (file, extension) {
                    clickedObject.text('Uploading'); // change button text, when user selects file
                    this.disable(); // If you want to allow uploading only 1 file at time, you can disable upload button
                    interval = window.setInterval(function () {
                        var text = clickedObject.text();
                        if (text.length < 13) {
                            clickedObject.text(text + '.');
                        } else {
                            clickedObject.text('Uploading');
                        }
                    }, 200);
                },
                onComplete:function (file, response) {
                    window.clearInterval(interval);
                    clickedObject.text('Upload Image');
                    this.enable(); // enable upload button


                    // If nonce fails
                    if (response == -1) {
                        var fail_popup = $('#of-popup-fail');
                        fail_popup.fadeIn();
                        window.setTimeout(function () {
                            fail_popup.fadeOut();
                        }, 2000);
                    }

                    // If there was an error
                    else if (response.search('Upload Error') > -1) {
                        var buildReturn = '<span class="upload-error">' + response + '</span>';
                        $(".upload-error").remove();
                        clickedObject.parent().after(buildReturn);

                    } else {
                        var buildReturn = '<img class="hide of-option-image" id="image_' + clickedID + '" src="' + response + '" alt="" />';

                        $(".upload-error").remove();
                        $("#image_" + clickedID).remove();
                        clickedObject.parent().after(buildReturn);
                        $('img#image_' + clickedID).fadeIn();
                        clickedObject.next('span').fadeIn();
                        clickedObject.parent().prev('input').val(response);
                    }
                }
            });

        });

    }

    of_image_upload();

    //AJAX Remove Image (clear option value)
    $('.image_reset_button').live('click', function () {

        var clickedObject = $(this);
        var clickedID = $(this).attr('id');
        var theID = $(this).attr('title');

        var nonce = $('#security').val();
        var profile = $('#this_profile').val();

        var data = {
            action:'of_ajax_post_action',
            type:'image_reset',
            profile:profile,
            security:nonce,
            data:theID
        };

        $.post(ajaxurl, data, function (response) {

            //check nonce
            if (response == -1) { //failed

                var fail_popup = $('#of-popup-fail');
                fail_popup.fadeIn();
                window.setTimeout(function () {
                    fail_popup.fadeOut();
                }, 2000);
            } else {

                var image_to_remove = $('#image_' + theID);
                var button_to_hide = $('#reset_' + theID);
                image_to_remove.fadeOut(500, function () {
                    $(this).remove();
                });
                button_to_hide.fadeOut();
                clickedObject.parent().prev('input').val('');
            }


        });

    });


    /* Slider */
    //Hide (Collapse) the toggle containers on load
    $(".slide_body").hide();

    //Switch the "Open" and "Close" state per click then slide up/down (depending on open/close state)
    $(".slide_edit_button").live('click', function () {
        $(this).parent().toggleClass("active").next().slideToggle("fast");
        return false; //Prevent the browser jump to the link anchor
    });

    // Update slide title upon typing
    function update_slider_title(e) {
        var element = e;
        if (this.timer) {
            clearTimeout(element.timer);
        }
        this.timer = setTimeout(function () {
            $(element).parent().prev().find('strong').text(element.value);
        }, 100);
        return true;
    }

    $('.of-slider-title').live('keyup', function () {
        update_slider_title(this);
    });


    //Remove individual slide
    $('.slide_delete_button').live('click', function () {
        // event.preventDefault();
        var agree = confirm("Are you sure you wish to delete this item?");
        if (agree) {
            var $trash = $(this).parents('li');
            //$trash.slideUp('slow', function(){ $trash.remove(); }); //chrome + confirm bug made slideUp not working...
            $trash.animate({
                opacity:0.25,
                height:0
            }, 500, function () {
                $(this).remove();
            });
            return false; //Prevent the browser jump to the link anchor
        } else {
            return false;
        }
    });

    //Add new slide
    $(".slide_add_button").live('click', function () {
        var slidesContainer = $(this).prev();
        var sliderId = slidesContainer.attr('id');
        var sliderInt = $('#' + sliderId).attr('rel');

        var numArr = $('#' + sliderId + ' li').find('.order').map(function () {
            var str = this.id;
            str = str.replace(/\D/g, '');
            str = parseFloat(str);
            return str;
        }).get();

        var maxNum = Math.max.apply(Math, numArr);
        if (maxNum < 1) {
            maxNum = 0
        }

        var newNum = maxNum + 1;

        var markupTitle = '';
        if (slidesContainer.hasClass('has_title')) {
            if (slidesContainer.hasClass('has_title_testimonial')) {
                var markupTitleCaption = 'Name';
            }
            else {
                var markupTitleCaption = 'Title';
            }

            markupTitle = '<label>' + markupTitleCaption + '</label>' +
                '<input class="slide of-input of-slider-title" name="' + sliderId + '[' + newNum + '][title]" id="' + sliderId + '_' + newNum + '_slide_title" value="">';
        }

        var markupSubtitle = '';
        if (slidesContainer.hasClass('has_subtitle')) {
            var title_extra = '';
            if (slidesContainer.hasClass('has_subtitle_testimonial')) {
                title_extra = '/Company/Profession';
            }
            markupSubtitle = '<label>Sub Title' + title_extra + '</label>' +
                '<input class="slide of-input of-slider-subtitle" name="' + sliderId + '[' + newNum + '][subtitle]" id="' + sliderId + '_' + newNum + '_slide_subtitle" value="">';
        }

        var markupLink = '';
        if (slidesContainer.hasClass('has_link')) {
            markupLink = '<label>Link URL</label>' +
                '<input placeholder="http://www.example.com" class="slide of-input" name="' + sliderId + '[' + newNum + '][link]" id="' + sliderId + '_' + newNum + '_slide_link" value="">';
        }

        var markupImage = '';
        if (slidesContainer.hasClass('has_image')) {
            markupImage = '<label>Image URL</label>' +
                '<input class="slide of-input" name="' + sliderId + '[' + newNum + '][url]" id="' + sliderId + '_' + newNum + '_slide_url" value="">' +
                '<div class="upload_button_div">' +
                '<span class="button-secondary media_upload_button" id="' + sliderId + '_' + newNum + '" rel="' + sliderInt + '">Upload</span>' +
                '<span class="button-secondary mlu_remove_button hide" id="reset_' + sliderId + '_' + newNum + '" title="' + sliderId + '_' + newNum + '">Remove</span>' +
                '</div>' +
                '<div class="screenshot"></div>';
        }

        var markupDescription = '';
        if (slidesContainer.hasClass('has_description')) {
            markupDescription = '<label>Description/Text</label>' +
                '<textarea class="slide of-input" name="' + sliderId + '[' + newNum + '][description]" id="' + sliderId + '_' + newNum + '_slide_description" cols="8" rows="8"></textarea>';
        }

        var markupPrice = '';
        if (slidesContainer.hasClass('has_price')) {
            markupPrice = '<label>Price</label>' +
                '<input placeholder="e.g. 19.95" class="slide of-input" name="' + sliderId + '[' + newNum + '][price]" id="' + sliderId + '_' + newNum + '_slide_price" value="">';
        }

        var markupBadge = '';
        if (slidesContainer.hasClass('has_badge')) {
            var badges = '<option value="">None</option><option value="badge_mostchosen">Most Chosen</option><option value="badge_favorite">Favorite</option><option value="badge_bestvalue">Best Value</option><option value="badge_bestseller">Bestseller</option>';
            markupBadge = '<label>Badge</label>' +
                '<select class="slide of-input" name="' + sliderId + '[' + newNum + '][badge]" id="' + sliderId + '_' + newNum + '_slide_badge value="">' + badges + '</select>';
        }

        var markupHighlight = '';
        if (slidesContainer.hasClass('has_highlight')) {
            var highlightoptions = '<option value="no">no</option><option value="yes">yes</option>';
            markupHighlight = '<label>Featured/Highlighted</label>' +
                '<select class="slide of-input" name="' + sliderId + '[' + newNum + '][highlighted]" id="' + sliderId + '_' + newNum + '_slide_highlighted value="">' + highlightoptions + '</select>';
        }


        var newSlide = '<li class="temphide">' +
            '<div class="slide_header">' +
            '<strong>Item ' + newNum + '</strong>' +
            '<input type="hidden" class="slide of-input order" name="' + sliderId + '[' + newNum + '][order]" id="' + sliderId + '_slide_order-' + newNum + '" value="' + newNum + '">' +
            '<a class="slide_edit_button" href="#">Edit</a>' +
            '</div>' +
            '<div class="slide_body" style="display: none; ">' +
            markupTitle +
            markupSubtitle +
            markupImage +
            markupLink +
            markupDescription +
            markupPrice +
            markupBadge +
            markupHighlight +
            '<a class="slide_delete_button" href="#">Delete</a>' +
            '<div class="clear"></div>' +
            '</div>' +
            '</li>';

        slidesContainer.append(newSlide);
        $('.temphide').fadeIn('fast', function () {
            $(this).removeClass('temphide');
        });

        of_image_upload(); // re-initialise upload image..

        return false; //prevent jumps, as always..
    });

    //Sort slides
    jQuery('.slider').find('ul').each(function () {
        var id = jQuery(this).attr('id');
        $('#' + id).sortable({
            placeholder:"placeholder",
            opacity:0.6
        });
    });


    /**    Sorter (Layout Manager) */
    jQuery('.sorter').each(function () {
        var id = jQuery(this).attr('id');
        $('#' + id).find('ul').sortable({
            items:'li',
            placeholder:"placeholder",
            connectWith:'.sortlist_' + id,
            opacity:0.6,
            update:function () {
                $(this).find('.position').each(function () {

                    var listID = $(this).parent().attr('id');
                    var parentID = $(this).parent().parent().attr('id');
                    parentID = parentID.replace(id + '_', '');
                    var optionID = $(this).parent().parent().parent().attr('id');
                    $(this).prop("name", optionID + '[' + parentID + '][' + listID + ']');

                });
            }
        });
    });


    /**    Ajax Backup & Restore MOD */
        //backup button
    $('#of_backup_button').live('click', function () {
        var answer = confirm("Click OK to back up your current options.");

        if (answer) {

            var clickedObject = $(this);
            var clickedID = $(this).attr('id');

            var nonce = $('#security').val();
            var profile = $('#this_profile').val();

            var data = {
                action:'of_ajax_post_action',
                type:'backup_options',
                profile:profile,
                security:nonce
            };

			$('.ajax-loading-img').fadeIn();

			$.post(ajaxurl, data, function (response) {
				$('.ajax-loading-img').fadeOut();

                //check nonce
                if (response == -1) { //failed

                    var fail_popup = $('#of-popup-fail');
                    fail_popup.fadeIn();
                    window.setTimeout(function () {
                        fail_popup.fadeOut();
                    }, 2000);
                } else {

                    var success_popup = $('#of-popup-save');
					success_popup.find('.of-save-save').html("Backup has been created.");

					success_popup.fadeIn();
                    window.setTimeout(function () {
                        location.reload();
                    }, 1000);
                }

            });

        }

        return false;

    });

    //restore button
    $('#of_restore_button').live('click', function () {

        var answer = confirm("Warning: All of your current options will be replaced with the data from your last backup! Proceed?");

        if (answer) {

            var clickedObject = $(this);
            var clickedID = $(this).attr('id');

            var nonce = $('#security').val();
            var profile = $('#this_profile').val();

            var data = {
                action:'of_ajax_post_action',
                type:'restore_options',
                profile:profile,
                security:nonce
            };

			$('.ajax-loading-img').fadeIn();

			$.post(ajaxurl, data, function (response) {
				$('.ajax-loading-img').fadeOut();

                //check nonce
                if (response == -1) { //failed

                    var fail_popup = $('#of-popup-fail');
                    fail_popup.fadeIn();
                    window.setTimeout(function () {
                        fail_popup.fadeOut();
                    }, 2000);
                } else {

                    var success_popup = $('#of-popup-save');
					success_popup.find('.of-save-save').html("Backup has been restored.");
                    success_popup.fadeIn();
                    window.setTimeout(function () {
                        location.reload();
                    }, 1000);
                }

            });

        }

        return false;

    });

    /**    Ajax Transfer (Import/Export) Option */
    $('.of_import_button').live('click', function () {
        var answer = confirm("Click OK to import options.");
        if (answer) {

            var clickedObject = $(this);
            var clickedID = $(this).attr('id');

            var this_base_id = $(this).attr('id');
            this_base_id = this_base_id.replace('of_import_button_', '');

            var nonce = $('#security').val();
            var profile = $('#this_profile').val();

            var import_data = $('#export_data_' + this_base_id).val();

            var data = {
                action:'of_ajax_post_action',
                type:'import_options',
                profile:profile,
                security:nonce,
                data:import_data
            };

			$('.ajax-loading-img').fadeIn();

            $.post(ajaxurl, data, function (response) {
				$('.ajax-loading-img').fadeOut();
                var fail_popup = $('#of-popup-fail');
                var success_popup = $('#of-popup-save');

                //check nonce
                if (response == -1) { //failed
                    fail_popup.fadeIn();
                    window.setTimeout(function () {
                        fail_popup.fadeOut();
                    }, 2000);
                } else {
                    success_popup.fadeIn();
					success_popup.find('.of-save-save').html("Options have been imported.");

                    window.setTimeout(function () {
                        location.reload();
                    }, 1000);
                }

            });

        }

        return false;

    });

    function saveSettings() {
        var nonce = $('#security').val();

        $('.ajax-loading-img').fadeIn();

        //get serialized data from all our option fields
        var serializedReturn = $('#of_form :input[name][name!="security"][name!="of_reset"]').serialize();

        var profile = $('#this_profile').val();

        var data = {
            type:'save',
            action:'of_ajax_post_action',
            security:nonce,
            profile:profile,
            data:serializedReturn
        };

        $.post(ajaxurl, data, function (response) {
            var success = $('#of-popup-save');
            var fail = $('#of-popup-fail');
            var loading = $('.ajax-loading-img');
            loading.fadeOut();

            if (response == 1) {
				success.find('.of-save-save').html("Options Updated");
                success.fadeIn();
            } else {
                fail.fadeIn();
            }

            window.setTimeout(function () {
                success.fadeOut();
                fail.fadeOut();
            }, 2000);
        });
    }

	/** Options Transfer/Copy Options **/
	$('#of_copy_button').click(function (e) {
		e.preventDefault();

		var profile = $('#this_profile').val();
		var target = $('#of_copy_select').find(":selected").val();

		var answer = confirm("You are about to copy the contents of the current profile (Profile "+profile+") to the selected profile (Profile "+target+"). If you proceed, all existing settings and contents in that profile will be overwritten with the contents of the source profile. This action cannot be undone.");

		if (answer) {
			var nonce = $('#security').val();

			var data = {
				type:'copy_profile',
				action:'of_ajax_post_action',
				profile:profile,
				target:target,
				security:nonce
			};

			$('.ajax-loading-img').fadeIn();

			var success_popup = $('#of-popup-save');
			success_popup.find('.of-save-save').html("Copying profile " + profile + " to " + target + "...");
			success_popup.fadeIn();

			$.post(ajaxurl, data, function (response) {
				var fail = $('#of-popup-fail');
				$('.ajax-loading-img').fadeOut();

				if (response == 1) {
					success_popup.find('.of-save-save').html("Successfully copied profile " + profile + " to " + target + ".");
					window.setTimeout(function () {
						success_popup.fadeOut();
					}, 1000);
				} else {
					success_popup.fadeOut();
					fail.fadeIn();
					window.setTimeout(function () {
						fail.fadeOut();
					}, 2000);
				}
			});
		}
		return false;
	});


	/** Master Export **/
    $('#of_master_export').click(function (e) {
        e.preventDefault();
        var nonce = $('#security').val();
        var data = {
            type:'save_master_export',
            action:'of_ajax_post_action',
            security:nonce
        };
        $.download(ajaxurl, data, 'post');
    });

    /** Master Import **/
    $('#of_master_import').click(function (e) {
        e.preventDefault();

        //confirm reset
        var answer = confirm("You are about to import a backup file. If you proceed, all existing settings and contents in all profiles will be overwritten with the contents of that file. This action cannot be undone.");

        //ajax reset
        if (answer) {
			$('.ajax-loading-img').fadeIn();

            var nonce = $('#security').val();
            var rndvalue = Math.floor(Math.random()*35000);
            $('.ajax-reset-loading-img').fadeIn();
            var data = {
                type:'save_master_import',
                action:'of_ajax_post_action',
                rand:rndvalue,
                security:nonce
            };
            $.post(ajaxurl, data, function (response) {
                var success = $('#of-popup-reset');
                var fail = $('#of-popup-fail');
                var loading = $('.ajax-reset-loading-img');
                loading.fadeOut();
                if (response == 1) {
					success.find('.of-save-save').html("Master Export File has been restored.");
                    success.fadeIn();
                    window.setTimeout(function () {
                        location.reload();
                    }, 1000);
                } else {
                    fail.fadeIn();
                    window.setTimeout(function () {
                        fail.fadeOut();
                    }, 2000);
                }
            });
        }
        return false;
    });


    /** Demo Import **/
    $('#of_demo').click(function (e) {
        e.preventDefault();

        //confirm reset
        var answer = confirm("You are about to import demo data. If you proceed, all existing settings and contents in all profiles will be overwritten.");

        //ajax reset
        if (answer) {
			$('.ajax-loading-img').fadeIn();

            var nonce = $('#security').val();
            var rndvalue = Math.floor(Math.random()*35000);
            $('.ajax-reset-loading-img').fadeIn();
            var data = {
                type:'restore_demo',
                action:'of_ajax_post_action',
                rand:rndvalue,
                security:nonce
            };
            $.post(ajaxurl, data, function (response) {
                var success = $('#of-popup-reset');
                var fail = $('#of-popup-fail');
                var loading = $('.ajax-reset-loading-img');
                loading.fadeOut();
                if (response == 1) {
					success.find('.of-save-save').html("Demo Data has been imported. Remember to also import the XML file, if required.");
                    success.fadeIn();
                    window.setTimeout(function () {
                        location.reload();
                    }, 1000);
                } else {
                    fail.fadeIn();
                    window.setTimeout(function () {
                        fail.fadeOut();
                    }, 2000);
                }
            });
        }
        return false;
    });

    $('#of_demo_save').click(function (e) {
        e.preventDefault();

        //confirm reset
        var answer = confirm("This option will fail without write access.");

        //ajax reset
        if (answer) {
			$('.ajax-loading-img').fadeIn();

            var nonce = $('#security').val();
            $('.ajax-reset-loading-img').fadeIn();
            var data = {
                type:'save_demo',
                action:'of_ajax_post_action',
                security:nonce
            };
            $.post(ajaxurl, data, function (response) {
                var success = $('#of-popup-reset');
                var fail = $('#of-popup-fail');
                var loading = $('.ajax-reset-loading-img');
                loading.fadeOut();
                if (response == 1) {
					success.find('.of-save-save').html("Demo file has been created.");
                    success.fadeIn();
                    window.setTimeout(function () {
                        location.reload();
                    }, 1000);
                } else {
                    fail.fadeIn();
                    window.setTimeout(function () {
                        fail.fadeOut();
                    }, 2000);
                }
            });
        }
        return false;
    });

    /** AJAX Save Options */
    $('#of_save_top').live('click', function () {
        saveSettings();
        return false;
    });

    $('#of_save_bottom').live('click', function () {
        saveSettings();
        return false;
    });


    /* AJAX Options Reset */
    $('#of_reset').click(function () {

        //confirm reset
        var answer = confirm("Click OK to reset. All settings will be lost and replaced with default settings!");

        //ajax reset
        if (answer) {
			$('.ajax-loading-img').fadeIn();

            var nonce = $('#security').val();
            var profile = $('#this_profile').val();

            $('.ajax-reset-loading-img').fadeIn();

            var data = {
                type:'reset',
                action:'of_ajax_post_action',
                profile:profile,
                security:nonce
            };

            $.post(ajaxurl, data, function (response) {
                var success = $('#of-popup-reset');
                var fail = $('#of-popup-fail');
                var loading = $('.ajax-reset-loading-img');
                loading.fadeOut();

                if (response == 1) {
                    success.fadeIn();
                    window.setTimeout(function () {
                        location.reload();
                    }, 1000);
                } else {
                    fail.fadeIn();
                    window.setTimeout(function () {
                        fail.fadeOut();
                    }, 2000);
                }


            });

        }

        return false;

    });


    /**    Tipsy @since v1.3 */
    if (jQuery().tipsy) {
        $('.typography-size, .typography-unit, .typography-height, .typography-face, .typography-style, .of-typography-color').tipsy({
            fade:true,
            gravity:'s',
            opacity:0.7
        });
    }

}); //end doc ready