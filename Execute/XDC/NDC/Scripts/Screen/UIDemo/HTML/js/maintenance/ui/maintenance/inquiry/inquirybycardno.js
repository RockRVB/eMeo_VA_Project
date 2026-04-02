var MenuFlag=0;
function InvokeChildrenSwitch()
{
	switch(MenuFlag)
	{
		case 0://聚焦起始时间年输入框
				removeValueDataNNumber();
				setFocusStartYear();											
				MenuFlag=1;
				return 0;
				
		case 1://聚焦起始时间月输入框
				removeValueDataNNumber();				
				setFocusStartMonth();				
				MenuFlag=2;
				return 0;
				
		case 2://聚焦起始时间日输入框
				removeValueDataNNumber();				
				setFocusStartDay();				
				MenuFlag=3;
				return 0;
				
		case 3://聚焦结束时间年输入框
				removeValueDataNNumber();				
				setFocusEndYear();				
				MenuFlag=4;
				return 0;		
				
		case 4://聚焦结束时间月输入框
				removeValueDataNNumber();				
				setFocusEndMonth();				
				MenuFlag=5;
				return 0;		
		
		case 5://聚焦结束时间日输入框
				removeValueDataNNumber();				
				setFocusEndDay();				
				MenuFlag=6;
				return 0;
		
		case 6://聚焦卡号后八位输入框
				removeValueDataNNumber();				
				setLastEightNumber();				
				MenuFlag=0;
				return 1;

		
		default:
				MenuFlag=0;
				removeValueDataNNumber();
				return 0;
	}
}


//清除焦点
function removeValueDataNNumber()
{
	document.getElementById('INPUT_SYEAR').style.backgroundColor="";
	document.getElementById('INPUT_SMONTH').style.backgroundColor="";
	document.getElementById('INPUT_SDAY').style.backgroundColor="";
	
	document.getElementById('INPUT_EYEAR').style.backgroundColor="";
	document.getElementById('INPUT_EMONTH').style.backgroundColor="";
	document.getElementById('INPUT_EDAY').style.backgroundColor="";
	
	document.getElementById('INPUT_CARDNUM').style.backgroundColor="";

	
	document.getElementById('INPUT_SYEAR').blur();	
	document.getElementById('INPUT_SMONTH').blur();	
	document.getElementById('INPUT_SDAY').blur();	
	document.getElementById('INPUT_EYEAR').blur();	
	document.getElementById('INPUT_EMONTH').blur();	
	document.getElementById('INPUT_EDAY').blur();	
	document.getElementById('INPUT_CARDNUM').blur();
	
}

//聚焦起始时间年输入框
function setFocusStartYear()
{
	document.getElementById('INPUT_SYEAR').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_SYEAR').click();
}

//聚焦起始时间月输入框
function setFocusStartMonth()
{
	document.getElementById('INPUT_SMONTH').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_SMONTH').click();
}

//聚焦起始时间日输入框
function setFocusStartDay()
{
	document.getElementById('INPUT_SDAY').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_SDAY').click();
}

//聚焦结束时间年输入框
function setFocusEndYear()
{
	document.getElementById('INPUT_EYEAR').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_EYEAR').click();
}

//聚焦结束时间月输入框
function setFocusEndMonth()
{
	document.getElementById('INPUT_EMONTH').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_EMONTH').click();
}

//聚焦结束时间日输入框
function setFocusEndDay()
{
	document.getElementById('INPUT_EDAY').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_EDAY').click();
}

//聚焦卡号后八位输入框
function setLastEightNumber()
{
	document.getElementById('INPUT_CARDNUM').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_CARDNUM').click();
}


function HandleKeyboardAction(keyValue) 
{ 
	switch (keyValue) 
	{
		//新增数字键事件响应,支持键盘操作,Start......
		case "1":
			$("#keyboard-button-1").click();
			break;
		case "2":
			$("#keyboard-button-2").click();
			break;
		case "3":
			$("#keyboard-button-3").click();
			break;
	    case "4":
			$("#keyboard-button-4").click();
			break;
	    case "5":
			$("#keyboard-button-5").click();
			break;
	    case "6":
			$("#keyboard-button-6").click();
			break;
	    case "7":
			$("#keyboard-button-7").click();
			break;
	    case "8":
			$("#keyboard-button-8").click();
			break;
	    case "9":
			$("#keyboard-button-9").click();
			break;
	    case "10":
			$("#keyboard-button-10").click();
			break;
	    case "CLEAR":
			$("#keyboard-button-11").click();
			$("#keyboard-button-28").click();
			break;
	    case "ENTER":
			removeValueDataNNumber();
			$("#keyboard-button-12").click();
			$("#keyboard-button-30").click();
			break;
	    default:
	        break;
	    }
}