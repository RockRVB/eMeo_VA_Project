var MenuFlag=1;
function InvokeChildrenSwitch()
{
	switch(MenuFlag)
	{		
		case 1:
				var buttonList  = $('.g-button-zone button');
				var buttonListLength = buttonList.length;
				if(buttonListLength>0)
				{
					MenuFlag=1;
					SetButtonNumber();
					return 1;
				}
		default:
				MenuFlag=1;
				return 2;
	}	
}

//设置按钮数字
function SetButtonNumber()
{
	var buttonList  = $('.g-button-zone button');
	var buttonListLength = buttonList.length;
	for(var i=0;i<buttonListLength;i++)
	{
		var span = ($(buttonList[i]).children('span'))[1];
		var spanText = $(span).text();
		$(span).text((i+1)+" "+spanText);
		$(buttonList[i]).attr("name","Select"+(i+1));
	};
	return true;
}

//设置按钮数字
function SetButtonNumberAgain()
{
	removeValueDataNNumber();
	SetButtonNumber();
	return true;
}


//清除按钮数字
function removeValueDataNNumber()
{
	var buttonList  = $('.g-button-zone button');
	var buttonListLength = buttonList.length;
	for(var i=0;i<buttonListLength;i++)
	{
		var span = ($(buttonList[i]).children('span'))[1];
		var spanText = $(span).text();
		var reg=/^[0-9]+.?[0-9]*$/; 
		var num=spanText.substr(0,1);
		if(reg.test(num))
		{
      		$(span).text(spanText.substr(1,spanText.length-1));
    	}			
		$(buttonList[i]).attr("name","");
	};
	return true;
}


function HandleKeyboardAction(keyValue) 
{ 
	switch (keyValue) {				
	   
		case "1":		
			$("button[name='Select1']").click();
			$("#Select1").click();
			SetButtonNumberAgain();
			return true;			
		case "2":
			$("button[name='Select2']").click();
			$("#Select2").click();
			SetButtonNumberAgain();
			return true; 
		case "3":
			$("button[name='Select3']").click();
			$("#Select3").click();
			SetButtonNumberAgain();
			return true; 
		case "4":
			$("button[name='Select4']").click();
			$("#Select4").click();
			SetButtonNumberAgain();
			return true; 
		case "5":$("button[name='Select5']").click();
			$("#Select5").click();
			SetButtonNumberAgain();
			return true; 
		case "6":
			$("button[name='Select6']").click();
			$("#Select6").click();
			SetButtonNumberAgain();
			return true;  
		case "7":
			$("button[name='Select7']").click();
			$("#Select7").click();		
			SetButtonNumberAgain();
			return true;  
		case "8":
			$("button[name='Select8']").click();
			$("#Select8").click();
			SetButtonNumberAgain();
			return true;  
		case "9":
			$("button[name='Select9']").click();
			$("#Select9").click();
			SetButtonNumberAgain();
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
