$(function () {
    $('#continue').hide();
	$('#continuegreyout').hide();
    $('#findaddress').hide();
	setTimeout("ValidateAddress()", 10);

    $("#postalcode,#blocknum,#floor,#unit,#streetname1,#streetname2,#streetname3,input:checkbox[name='accounts']").change(function () {
        ValidateAddress();
    });
});

function ValidateAddress() {
    var postcodeOK = false, blkOK = false, floorOK = false, unitOK = false, streetname1OK = false, streetname2OK = false, streetname3OK = false;
    var streetOK = false;
    if ($('#postalcode').val().length != 6)
	{
        $('#findaddress').hide();
        $('#findaddressgreyout').show();
        postcodeOK = false;
    }
    else
	{
        $('#findaddressgreyout').hide();
        $('#findaddress').show();
        postcodeOK = true;
    }
    var changedata = checkdata();
    if ($('#blocknum').val().length > 4 || $('#blocknum').val().trim().length == 0)
	{
        blkOK = false;
    }
    else
	{
        blkOK = true;
    }
    if ($('#floor').val().length > 3 || $('#floor').val().trim().length == 0)
	{
        floorOK = false;
    }
    else
	{
        floorOK = true;
    }
    if ($('#unit').val().length > 7 || $('#unit').val().trim().length == 0) 
	{
        unitOK = false;
    }
    else
	{
        unitOK = true;
    }
    if ($('#streetname1').val().length > 25)
	{
        streetname1OK = false;
    }
    else
	{
        streetname1OK = true;
    }
    if ($('#streetname2').val().length > 35) 
	{
        streetname2OK = false;
    }
    else 
	{
        streetname2OK = true;
    }
    if ($('#streetname3').val().length > 35) 
	{
        streetname3OK = false;
    }
    else 
	{
        streetname3OK = true;
    }
    if ($('#streetname3').val().trim().length == 0 && $('#streetname1').val().trim().length == 0 && $('#streetname2').val().trim().length == 0) 
	{
        streetOK = false;
    }
    else
	{
		streetOK = true;
	}

    if ($("#country").val() == "SINGAPORE") {
        if (postcodeOK && streetname1OK && streetname2OK && streetname3OK && streetOK && changedata && $("input:checkbox[name='accounts']:checked").length > 0) {
            $('#continuegreyout').hide();
            $('#continue').show();
        }
        else {
            $('#continue').hide();
            $('#continuegreyout').show();
        }
    }
    else {
        if (streetname1OK && streetname2OK && streetname3OK && streetOK && $("input:checkbox[name='accounts']:checked").length > 0) {
            $('#continuegreyout').hide();
            $('#continue').show();
        }
        else {
            $('#continue').hide();
            $('#continuegreyout').show();
        }
    }
}

function checkdata() {
    var check = false;
    if (($('#streetname1').val().trim().length > 0 && $("#streetname1").val() == $("#hiddenCOPNewStreet1Original").text()) &&
	($('#streetname2').val().trim().length > 0 && $("#streetname2").val() == $("#hiddenCOPNewStreet2Original").text()) && 
	($('#streetname3').val().trim().length > 0 && $("#streetname3").val() == $("#hiddenCOPNewStreet3Original").text()) && 
	($('#blocknum').val().trim().length > 0 && $("#blocknum").val() == $("#hiddenCOPNewBlocknumOriginal").text()))
        check = false;
	else 
		check = true;
    
    return check;
}


