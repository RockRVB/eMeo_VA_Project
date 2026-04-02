var isEnableOtp=0; 
$(function () {
    var $write = $('.digit1'),
	    digitCount = 0;

    $('.num-pad li').click(function () {

		if(isEnableOtp==1){
			var $this = $(this),
				character = $this.children().html();

			// Delete
			if ($this.hasClass('del')) {
				if (digitCount > 0)
					digitCount--;
				switch (digitCount) {
					case 0:
						$write = $('.digit1');
						break;
					case 1:
						$write = $('.digit2');
						break;
					case 2:
						$write = $('.digit3');
						break;
					case 3:
						$write = $('.digit4');
						break;
					case 4:
						$write = $('.digit5');
						break;
					case 5:
						$write = $('.digit6');
						break;
					default:
						$write = $('.digit7');
						break;
				}
				$write.val("");
				var htmlStr = $('#hidden').html();
				$('#hidden').html(htmlStr.substr(0, htmlStr.length - 1));
				return false;
			}

			// Add the character
			$write.val(character);
			$('#hidden').html($('#hidden').html() + character);
			if (digitCount < 6)
				digitCount++;

			switch (digitCount) {
				case 0:
					$write = $('.digit1');
					break;
				case 1:
					$write = $('.digit2');
					break;
				case 2:
					$write = $('.digit3');
					break;
				case 3:
					$write = $('.digit4');
					break;
				case 4:
					$write = $('.digit5');
					break;
				case 5:
					$write = $('.digit6');
					break;
				default:
					$write = $('.digit7');
					$("#confirm").removeClass("disabled").removeAttr("disabled").attr("tag", "OnContinue");
					break;
			}
		}
    });

    $(".del").bind('click', function () {
        $("#confirm").addClass("disabled").attr("disabled", "disabled").removeAttr("tag");
    })
	
	
});