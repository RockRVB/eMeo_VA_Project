var MenuFlag=0;

function InvokeChildrenSwitch()
{
	switch(MenuFlag)
	{
		case 0:
				removeValueDataNNumber();
				setFocusOnHour();
				MenuFlag=1;
				return 0;
		case 1:
				removeValueDataNNumber();
				setFocusOnMinute();
				MenuFlag=2;
				return 0;
		case 2:
				removeValueDataNNumber();
				setFocusOffHour();
				MenuFlag=3;
				return 0;
		case 3:
				removeValueDataNNumber();
				setFocusOffMinute();
				MenuFlag=0;
				return 1;
		default :
				MenuFlag=0;
				removeValueDataNNumber();
				return 0;
	}
}


//聚焦开启时间小时输入框
function setFocusOnHour()
{
	document.getElementById('INPUT_ONHOUR').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_ONHOUR').click();	
}
//聚焦开启时间分钟输入框
function setFocusOnMinute()
{
	document.getElementById('INPUT_ONMINUTE').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_ONMINUTE').click();
}
//聚焦关闭时间小时输入框
function setFocusOffHour()
{
	document.getElementById('INPUT_OFFHOUR').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_OFFHOUR').click();	
}
//聚焦关闭时间分钟输入框
function setFocusOffMinute()
{
	document.getElementById('INPUT_OFFMINUTE').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_OFFMINUTE').click();
}

//失去焦点
function removeValueDataNNumber()
{
	document.getElementById('INPUT_ONHOUR').style.backgroundColor="";
	document.getElementById('INPUT_ONMINUTE').style.backgroundColor="";
	document.getElementById('INPUT_OFFHOUR').style.backgroundColor="";
	document.getElementById('INPUT_OFFMINUTE').style.backgroundColor="";
	document.getElementById('INPUT_ONHOUR').blur();	
	document.getElementById('INPUT_ONMINUTE').blur();	
	document.getElementById('INPUT_OFFHOUR').blur();
	document.getElementById('INPUT_OFFMINUTE').blur();	
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