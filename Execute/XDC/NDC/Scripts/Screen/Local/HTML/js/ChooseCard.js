// JavaScript Document

function setCardType() {
    var ss = $("#hiddenAllCardType").val();
    var ssarray = new Array();
    ssarray = ss.split(";");

    for (i = 0; i < ssarray.length; i++) {
	if(ssarray[i]!="")
    {
        var ssarray1 = new Array();
        ssarray1 = ssarray[i].split(",");

        var cardtypename;
        var cardtypes;

        for (j = 0; j < ssarray1.length; j++) {

            if (j == 0) {
                cardtypename = ssarray1[j];
            }
            else {
                cardtypes = ssarray1[j];
            }
        }

        if (cardtypes > 0) {

            $("#sub_" + (i + 1)).removeAttr("style");
            $("#sub_" + (i + 1) + " > div > div").html(cardtypename);
            $("#sub_" + (i + 1) + " > div > div").attr("style", "background-image : url(../../../Image/CN/Card/icon_" + cardtypename.toLowerCase() + "_not_click.png)");
            $("#sub_" + (i + 1)).attr("tag", "On" + cardtypename);
        }
	  }		
    }
    
    $("li").hover(
        function () {
            var cardtypenamehtml = $(this).children().children().html().trim();
            $(this).children().children().css("background-image", "url(../../../Image/CN/Card/icon_" + cardtypenamehtml.toLowerCase() + "_check.png)");
            cardtypenamehtml = "";
        }, function () {
            var cardtypenamehtml = $(this).children().children().html().trim();
            $(this).children().children().css("background-image", "url(../../../Image/CN/Card/icon_" + cardtypenamehtml.toLowerCase() + "_not_click.png)");
            cardtypenamehtml = "";
        }
    );
}