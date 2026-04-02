$(function(){
	$('#sgPhoneValidation').hide();
	$('#otherPhoneValidation').hide();
	$('#sameNumber').hide();
	$('#continue').hide();
	$('#sgPhoneStartValidation').hide();
	
    $('#mobile').change(function(){
		var lengthOK = false, differentNumber = false;
		// Check phone length
		if($('#mobilecountry_').val() == "0065") //SG Phone
		{
			$('#otherPhoneValidation').hide();
			if($('#mobile').val().length != 8)
			{
				$('#sgPhoneValidation').show();
				$('#sgPhoneStartValidation').hide();
				lengthOK = false;
			}
			else
			{	if($('#copChangePhoneType').text()=="M" && !/^(8|9)/.test($('#mobile').val())){
						$('#sgPhoneValidation').hide();
						$('#sgPhoneStartValidation').show();
						lengthOK = true;
				}
				else if($('#copChangePhoneType').text()=="H" && !/^(3|6)/.test($('#mobile').val())){
						$('#sgPhoneValidation').hide();
						$('#sgPhoneStartValidation').show();
						lengthOK = true;
				}else{
					$('#sgPhoneStartValidation').hide();
					$('#sgPhoneValidation').hide();
					lengthOK = true;					
					//Check if number repeated
					if ($('#mobilecountry_').val() == $('#hiddenCurrentCountry').text() &&
					$('#mobile').val() == $('#hiddenCurrentContact').text())
					{
						$('#sameNumber').show();
						differentNumber = false;
					}
					else
					{
						$('#sameNumber').hide();
						differentNumber = true;
					}
				}
			}
		}
		else //Non-SG Phone
		{
			$('#sgPhoneValidation').hide();
			if($('#mobile').val().length == 0 || $('#mobile').val().length > 16)
			{
				if ($('#mobile').val().length > 16)
				{	
					$('#otherPhoneValidation').show();
				}
				lengthOK = false;
			}
			else
			{
				$('#otherPhoneValidation').hide();
				lengthOK = true;
				
				//Check if number repeated
				if ($('#mobilecountry_').val() == $('#hiddenCurrentCountry').text() &&
				$('#mobile').val() == $('#hiddenCurrentContact').text())
				{
					$('#sameNumber').show();
					differentNumber = false;
				}
				else
				{
					$('#sameNumber').hide();
					differentNumber = true;
				}
			}
		}
		if (lengthOK && differentNumber)
		{
			$('#continuegreyout').hide();
			$('#continue').show();
		}
		else
		{
			$('#continue').hide();
			$('#continuegreyout').show();
		}
    });
});