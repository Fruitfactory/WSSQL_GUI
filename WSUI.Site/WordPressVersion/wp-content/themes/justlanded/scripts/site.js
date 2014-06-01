jQuery.noConflict();

;(function ($, window, undefined) {
    'use strict';

    var $doc = $(document),
        Modernizr = window.Modernizr;

    $(document).ready(function() {
         $doc.foundation();

        $.fn.placeholder ? $('input, textarea').placeholder() : null;

        $("#main_nav .menu").tinyNav({
            active: 'current-menu-item'
        });

		/*
		if ($('body').hasClass('has-sticky-menu')) {
			$('#navigation_elements').waypoint('sticky', {
				offset: 30
			});
		}
		*/
    });

    $('.block-grid.two-up>li:nth-child(2n+1)').css({clear: 'both'});
    $('.block-grid.three-up>li:nth-child(3n+1)').css({clear: 'both'});
    $('.block-grid.four-up>li:nth-child(4n+1)').css({clear: 'both'});
    $('.block-grid.five-up>li:nth-child(5n+1)').css({clear: 'both'});

    // Hide address bar on mobile devices (except if #hash present, so we don't mess up deep linking).
    if (Modernizr.touch && !window.location.hash) {
        $(window).load(function () {
            setTimeout(function () {
                window.scrollTo(0, 1);
            }, 0);
        });
    }

})(jQuery, this);


