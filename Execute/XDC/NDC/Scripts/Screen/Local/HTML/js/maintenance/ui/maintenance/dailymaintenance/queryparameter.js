var  MenuFlag=0;
function InvokeChildrenSwitch()
{
	
	removeValueDataNNumber();
	switch(MenuFlag)
	{
		case 0:	//设置侧边栏按钮数字
				var sideButtonList = $(".middle-inner button");
				var sideLength = sideButtonList.length;
				if(sideLength>0)
				{
					MenuFlag=1;
					SetSideNumber();
					return 0;
				}
		case 1:	//设置底部按钮的数字
				var bottomButtonList = $(".g-button-zone button");
				var listLength = bottomButtonList.length;
				if(listLength>0)
				{
					MenuFlag=0;
					SetBottomNumber();
					return 1;
				}
		default:
				MenuFlag=0;
				removeValueDataNNumber();
				return 0;		
	}
	
}
//清除按钮数字
function removeValueDataNNumber()
{	
	//清除底部按钮数字
	var bottomButtonList = $(".g-button-zone button");
	var listLength = bottomButtonList.length;
		
	for(var i=0;i<listLength;i++)
	{
		var span = ($(bottomButtonList[i]).children("span"))[1];
		var spanText = $(span).text();
		
		var reg=/^[0-9]+.?[0-9]*$/; 
		var num=spanText.substr(0,1);
		if(reg.test(num))
		{
      		$(span).text(spanText.substr(1,spanText.length-1));
    	}			
		$(span).attr("id","");
	}
	
	//清除侧边栏按钮数字
	var sideButtonList = $(".middle-inner button");
	var sideLength = sideButtonList.length;
	for(var i=0;i<sideLength;i++)
	{
		var tip = ($(sideButtonList[i])).children(".tip");
		var tipText = $(tip).text();
		var reg=/^[0-9]+.?[0-9]*$/; 
		if(reg.test(tipText))
		{
			$(tip).text("");
		}
		$(tip).attr("id","");
	}
	
	return true;
	
}

//设置底部按钮的数字
function SetBottomNumber()
{	
	var strTemp="";
	var bottomButtonList = $(".g-button-zone button");
	var listLength = bottomButtonList.length;
	for(var i=0;i<listLength;i++)
	{
		var span = ($(bottomButtonList[i]).children("span"))[1];
		var spanText = $(span).text();
		$(span).text((i+1)+" "+spanText);
		$(span).attr("id","Select"+(i+1));
	}	
	return true;
}

//设置侧边栏按钮数字
function SetSideNumber()
{
	var sideButtonList = $(".middle-inner button");
	var sideLength = sideButtonList.length;
	for(var i=0;i<sideLength;i++)
	{
		var tip = ($(sideButtonList[i])).children(".tip");
		var tipText = $(tip).text();
		$(tip).text((i+1)+tipText);
		$(tip).attr("id","Select"+(i+1));
	}
	return true;
}


function HandleKeyboardAction(keyValue) 
{ 
	switch (keyValue) {				
	   
		case "1":		
			$("button[value='Select1']").click();
			$("#Select1").click();
			return true;			
		case "2":
			$("button[value='Select2']").click();
			$("#Select2").click();
			return true; 
		case "3":
			$("button[value='Select3']").click();
			$("#Select3").click();
			return true; 
		case "4":
			$("button[value='Select4']").click();
			$("#Select4").click();
			return true; 
		case "5":$("button[value='Select5']").click();
			$("#Select5").click();
			return true; 
		case "6":
			$("button[value='Select6']").click();
			$("#Select6").click();
			return true;  
		case "7":
			$("button[value='Select7']").click();
			$("#Select7").click();			
			return true;  
		case "8":
			$("button[value='Select8']").click();
			$("#Select8").click();
			return true;  
		case "9":
			$("button[value='Select9']").click();
			$("#Select9").click();
			return true;  
		case "0":
			return true;  
		case "CLEAR":
			return true;  
		case "ENTER":
			return this.keyBoardAction("KEYBOARD_ENTER",key);
        case "CANCEL":return this.keyBoardAction("FUN_QUIT",key);
	    case "KEYBOARD_TAB":return this.SwitchMenu();
        // 后续插入快捷键等等
        //.........
	    default:
	        return this.keyBoardAction("keyboard",key);
	    }
}