$(function(){
	$('#newEmailValidation').hide();
	$('#newEmailValidation2').hide();
	$('#sameEmail').hide();
	$('#verifyEmailValidation').hide();
	$('#continue').hide();
    $('#newemail,#verifyemail').change(function(){
		var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/; 		
		var newemail = $('#newemail').val();
		var verifyemail = $('#verifyemail').val(); 
		$('#verifyemailhidden').val(verifyemail);
		if(newemail.length > 0) {
			var formatOK = false, lengthOK = false, matchOK = false, differentEmail = false;
			 			if(emailReg.test(newemail)) {
				$('#newEmailValidation').hide();
				formatOK = true;
			}
			else {
				$('#newEmailValidation').show();
				formatOK = false;
			}
			
			 			if(newemail.length <= 60) {
				$('#newEmailValidation2').hide();
				lengthOK = true;
			}
			else {
				$('#newEmailValidation2').show();
				lengthOK = false;
			}
			
			 			if(newemail.toUpperCase() != $('#hiddenCurrentEmail').text()) {
				$('#sameEmail').hide();
				differentEmail = true;
			}
			else {
				$('#sameEmail').show();
				differentEmail = false;
			}
			
			if(verifyemail.length>0)
			{
				 				if (newemail.toUpperCase() != verifyemail.toUpperCase()) {
					$('#verifyEmailValidation').show();
					matchOK = false;
				}
				else {
					$('#verifyEmailValidation').hide();
					matchOK = true;
				}
			}
			 			if (formatOK && lengthOK && matchOK && differentEmail) {
				$('#continuegreyout').hide();
				$('#continue').show();
			}
			else {
				$('#continue').hide();
				$('#continuegreyout').show();
			}
		}
    });
});