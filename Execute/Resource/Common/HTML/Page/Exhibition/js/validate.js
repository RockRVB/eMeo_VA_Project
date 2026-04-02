$(function(){
	$('#newEmailValidation').hide();
	$('#verifyEmailValidation').hide();
    $('#newemail,#verifyemail').change(function(){
		var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
		
		var newemail = $('#newemail').val();
		var verifyemail = $('#verifyemail').val(); 
		
		if(emailReg.test(newemail)) {
			$('#newEmailValidation').hide();
		}
		else {
			$('#newEmailValidation').show();
		}
		
		if (newemail.toUpperCase() != verifyemail.toUpperCase()) {
			$('#verifyEmailValidation').show();
			$('#continue').attr("class", "disabled");
			/* $('#continue').addClass("disabled"); */
			$('#continue').removeAttr("tag");
		}
		else {
			$('#verifyEmailValidation').hide();
			$('#continue').attr("class", "");
			/* $('#continue').removeClass("disabled"); */
			$('#continue').attr("tag", "OnContinue");
		}
    });
});