var MenuFlag=0;
function InvokeChildrenSwitch()
{
	switch(MenuFlag)
	{
		case 0://聚焦错误码输入框
				removeValueDataNNumber();
				setFocusErrorCode();											
				MenuFlag=1;
				return 0;
				
		case 1://聚焦返回码输入框
				removeValueDataNNumber();				
				setFocusRespCode();				
				MenuFlag=0;
				return 1;
		default:
				MenuFlag=0;
				removeValueDataNNumber();
				return 0;
	}
}

//聚焦错误码输入框
function setFocusErrorCode()
{
	document.getElementById('INPUT_ERRORCODE').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_ERRORCODE').click();	
	
}
//聚焦返回码输入框
function setFocusRespCode()
{
	document.getElementById('INPUT_RESPCODE').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_RESPCODE').click();
}
//失去焦点
function removeValueDataNNumber()
{
	document.getElementById('INPUT_ERRORCODE').style.backgroundColor="";
	document.getElementById('INPUT_RESPCODE').style.backgroundColor="";
	document.getElementById('INPUT_ERRORCODE').blur();	
	document.getElementById('INPUT_RESPCODE').blur();
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

