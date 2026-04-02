function OnOffServices(obj){
	$.ajax({
		type: "GET" ,
		url: "resource/OnOffFeatureConfig.xml" ,
		dataType: "xml" ,
		success: function(xml) {
		var xmlDoc = $.parseXML(xml);
		$(xml).find("Service").each(function(){
			var name = $(this).attr("name");
			var enable = $(this).attr("enable");
			var controlMode = $(this).attr("controlMode");
			var startDate = $(this).attr("startDate");
			var endDate = $(this).attr("endDate");
			var startTime = $(this).attr("startTime");
			var endTime = $(this).attr("endTime");
			for(var bean in obj){
				if(obj[bean]==name){
					if(enable==1){//on check
						var today = getNowFormatDate().replace(/\-/g, "");
						var nowtime = getNowFormatTime().replace(/\-/g, "");
						if(controlMode==0){//check both date and time
							if(startDate!=null && endDate!=null){
								startDate = startDate.replace(/\-/g, "");
								endDate = endDate.replace(/\-/g, "");
								if(today>=startDate && today<=endDate){//within the period of validity
									if(startTime!=null && endTime!=null){
										startTime = startTime.replace(/\:/g, "");
										endTime = endTime.replace(/\:/g, "");
										if(nowtime>=startTime && nowtime<=endTime){
											$("#"+name).hide();
										}
									}
								}
							}
						}else if(controlMode==1){//only check the date
							if(startDate!=null && endDate!=null){
								startDate = startDate.replace(/\-/g, "");
								endDate = endDate.replace(/\-/g, "");
								if(today>=startDate && today<=endDate){//within the period of validity
									$("#"+name).hide();
								}
							}
						}else if(controlMode==2){//only check the time
							if(startTime!=null && endTime!=null){
								startTime = startTime.replace(/\:/g, "");
								endTime = endTime.replace(/\:/g, "");
								if((nowtime>=startTime) && (nowtime<=endTime)){
									$("#"+name).hide();
								}
							}
						}
					}
				}
			}
			}); 
		}       
	});
}
	   

function getNowFormatDate() {
    var date = new Date();
    var seperator1 = "-";
    var seperator2 = ":";
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate;
    return currentdate;
} 

function getNowFormatTime() {
    var date = new Date();
    var seperator1 = "-";
    var seperator2 = "";
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currenttime = date.getHours() + seperator2 + date.getMinutes()
            + seperator2 + date.getSeconds();
    return currenttime;
} 