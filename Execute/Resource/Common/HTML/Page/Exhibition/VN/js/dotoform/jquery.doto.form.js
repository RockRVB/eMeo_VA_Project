/*jQuery Doto form  2.1.1*/
(function($){

	// Support IE8+ Firefox Chrome Safari
	
	var form_id = 1;
	var list_checkbox = new Array();
	
	$.fn.dotoform = function(opt)
	{
		var defaults = {
			
		};
    
    if(typeof opt == 'object'){
      opt = $.extend(defaults, opt);
    }
		
		this.each(function(){
			var obj = $(this);
			var wrapper  = obj.parent();
			
      if(typeof opt == 'string'){
        if(opt == 'refresh'){
          // clear dotoform
          obj.next('.dotoform-radio').remove();
          
          obj.unwrap('.dotoform-hide');
          obj.next('.dotoform-checkbox').remove();
          
          obj.unwrap('.dotoform-select-wrapper');
          obj.css('width','');
          
          obj.removeClass('dotoform-bulid');
        }
      }
      
			// one time only
			if(!obj.hasClass('dotoform-bulid'))
			{
				// if input radio
				if(obj.is('input') && obj.attr('type').toLowerCase() == 'radio')
				{
					obj.hide();
					var obj_name = obj.attr('name') ? obj.attr('name').replace(/\[|\]/g, "_") : '';					
					var element = '<span class="dotoform-radio ' + obj.attr('class') + (obj.attr('name') ? ' ' + obj_name : '') + (obj.is(':checked') ? ' active' : '') + (obj.attr('disabled') ? ' disabled' : '') + '" tabindex="' + (obj.attr('tabindex') ? obj.attr('tabindex') : '0') + '"></span>';
					obj.after(element);
					var element_radio = obj.next();
					if(!element_radio.hasClass('disabled'))
					{
						element_radio.click(function(){
						
							obj.prop('checked',true);
							obj.trigger('change');
							$('.dotoform-radio.' + obj_name).removeClass('active');
							element_radio.addClass('active');
						});
						
						element_radio.keydown(function(e){
							if(e.which == 32)
							{
								element_radio.click();
							}
						});
						
						$('label[for="' + obj.attr('id') + '"]').click(function(e){
							element_radio.click();
							return false;
						});
					}
					
					// feature 
						// set tabindex
						// set disabled
						// press spacebar checked unchecked element
				}
				// if input checkbox
				else if(obj.is('input') && obj.attr('type').toLowerCase() == 'checkbox')
				{
					if(!obj.attr('id'))
					{
						obj.attr('id','doto_form' + form_id++);
					}
					var element = '<label for="' + obj.attr('id') + '" class="dotoform-checkbox ' + (obj.attr('class') ? obj.attr('class') : '') + (obj.is(':checked') ? ' active' : '') + (obj.attr('disabled') ? ' disabled' : '') + '" tabindex="' + (obj.attr('tabindex') ? obj.attr('tabindex') : '0') + '"></label>';
					obj.after(element);
					obj.wrap('<div class="dotoform-hide"></div>');
					var element_checkbox = obj.parent().next();
					
					if(!element_checkbox.hasClass('disabled'))
					{
						obj.change(function(){
							for(var i in list_checkbox)
							{
								var temp = list_checkbox[i];
								if(temp.is(':checked'))
								{
									temp.parent().next().addClass('active');
								}
								else
								{
									temp.parent().next().removeClass('active');
								}
							}
						});
						
						element_checkbox.keydown(function(e){
							if(e.which == 32)
							{
								element_checkbox.click();
							}
						});
						
						list_checkbox.push(obj);
					}
					
					// feature 
						// set tabindex
						// set disabled
						// press spacebar checked unchecked element
						// native input
				}
				// if input select
				else if(obj.is('select'))
				{
					obj.wrap('<div class="dotoform-select-wrapper" style="position:relative"></div>');
					var width = obj.width();
					if(width <= 0)
					{
						$_obj = obj.clone();
						$('body').append($_obj);
						width = $_obj.width();
						$_obj.remove();
					}
					width += 10;
					obj.parent().css('width',width + 'px');
					obj.width(width + 15);
						
					// feature 
						// native select
				}
				
				obj.addClass('dotoform-bulid');
			}
		});	
	}
	
	//$.dotoform.show();
	$.dotoform = {
		'show' : function(){
			alert('show');
		}
	}
	
})(jQuery);