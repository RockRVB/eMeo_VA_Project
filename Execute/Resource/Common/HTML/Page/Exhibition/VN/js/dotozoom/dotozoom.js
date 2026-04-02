/**
	<a class="zoom" href="xxx.png">click</a>
	
	$('.zoom').dotozoom();
**/

(function($){
	
	$wrapper = $('<div id="iviewer"><div class="iviewer_viewer"></div><ul class="iviewer_controls"><li class="iviewer_close"></li><li class="iviewer_zoomin"></li><li class="iviewer_zoomout"></li></ul></div>');
	
	$.fn.dotozoom = function(opt) {
		var defaults = {
      onOpen: function(){},
      onClose: function(){}
		};
		
		opt = $.extend(defaults, opt);
		
		this.each(function() {
			
			var obj = $(this);
			if(!$('#iviewer').length) {
				
				$('body').append($wrapper);
				
				var viewer = $wrapper.find('.iviewer_viewer').
					width($(window).width()).
					height($(window).height()).
					iviewer({
						ui_disabled : true,
						zoom : 'fit',
						onFinishLoad : function(ev) {
              opt.onOpen();
							$wrapper.find('.iviewer_viewer').fadeIn();
						}
					}
				);

				$wrapper.find('.iviewer_zoomin').click(function(e) {
					e.preventDefault();
					viewer.iviewer('zoom_by', 1);
				});

				$wrapper.find('.iviewer_zoomout').click(function(e) {
					e.preventDefault();
					viewer.iviewer('zoom_by', -1);
				});
				
				$wrapper.find('.iviewer_close').click(function(e) {
					e.preventDefault();
          opt.onClose();
					close();
				});
			}
			
			obj.click(function(e) {
				e.preventDefault();
				open($(this).attr('href'));
			});
		});
	}
	
	function open(src) {
		$wrapper.fadeIn().trigger('fadein');
		$wrapper.find('.iviewer_viewer').hide();

		var viewer = $wrapper.find('.iviewer_viewer')
			.iviewer('loadImage', src)
			.iviewer('set_zoom', 'fit');
	}
	
	function close() {
		$wrapper.fadeOut().trigger('fadeout');
	}
})(jQuery);