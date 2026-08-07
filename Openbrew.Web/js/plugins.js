// usage: log('inside coolFunc', this, arguments);
// paulirish.com/2009/log-a-lightweight-wrapper-for-consolelog/
window.log = function f(){ log.history = log.history || []; log.history.push(arguments); if(this.console) { var args = arguments, newarr; args.callee = args.callee.caller; newarr = [].slice.call(args); if (typeof console.log === 'object') log.apply.call(console.log, console, newarr); else console.log.apply(console, newarr);}};

// make it safe to use console.log always
(function(a){function b(){}for(var c="assert,count,debug,dir,dirxml,error,exception,group,groupCollapsed,groupEnd,info,log,markTimeline,profile,profileEnd,time,timeEnd,trace,warn".split(","),d;!!(d=c.pop());){a[d]=a[d]||b;}})
(function(){try{console.log();return window.console;}catch(a){return (window.console={});}}());

// usage: $('#container').outside('click', function(e){ $(this).remove(); });
// http: //stackoverflow.com/questions/3440022/mouse-click-somewhere-else-on-page-not-on-a-specific-div
(function ($) {
	$.fn.outside = function (ename, cb) {
		return this.each(function () {
			var $this = $(this),
              self = this;
			$(document.body).bind(ename, function tempo(e) {
				if (e.target !== self && !$.contains(self, e.target)) {
					cb.apply(self, [e]);
					if (!self.parentNode) $(document.body).unbind(ename, tempo);
				}
			});
		});
	};
} (jQuery));

// Limit the input of a textarea with a character count
(function ($) {
    $.fn.limitInput = function (limit, target, preCalc) {
        return this.each(function() {
            $(this).keypress(function(event) {
                return $(this).val().length < limit;
            });

            $(this).keyup(function (event) {
                if ($(this).val().length > limit) {
                    $(this).val($(this).val().substring(0, limit));
                }
                $(target).html((limit - $(this).val().length) + ' character(s) remaining');
            });

            if (typeof preCalc != 'undefined' && preCalc) {
                $(target).html((limit - ($(this).val() != null ? $(this).val().length : 0)) + ' character(s) remaining');
            }
        });
	};
})(jQuery);

// Ajax Form
(function ($) {
	$.fn.ajaxForm = function (settings) {

		// Create some defaults, extending them with any options that were provided
		settings = $.extend( {
		  preSubmitCallback: null,
		  successCallback: null,
		  errorCallback: null
		}, settings);

		$(this).find('input[type=submit]').after($('<img src="/img/ajax-spinner.gif" class="spinner" />'));

		$(this).submit(function () {

			var submit = $(this).find('input[type=submit]');
			var spinner = $(this).find('img.spinner');
		
			if($(this).valid()) {
				var theForm = $(this);
				spinner.show();
				submit.attr('disabled', 'disabled');
				
				// Pre-Request Callback
				if(settings.preSubmitCallback) {
					settings.preSubmitCallback($(this));
				}

				// Ajax Request
				$.ajax({
					url: $(this).attr('action'),
					async: true,
					type: $(this).attr('method'),
					data: $(this).serialize(),
				
					success: function (data) {
						if(settings.successCallback) {
							settings.successCallback(theForm, data);
						}
					},
					error: function () {
						if(settings.errorCallback) {
							settings.errorCallback(theForm);
						}
					},
					complete: function () {
						submit.removeAttr('disabled');
						spinner.hide();
					}				
				});
			}

			return false;
		});
	};
})(jQuery);
