var MenuFlag=1;
function InvokeChildrenSwitch()
{
	switch(MenuFlag)
	{
		case 0:	//去掉聚焦
				removeValueDataNNumber();
				MenuFlag=1;
				return 0;
		case 1:	//聚焦钞票序列号输入框
				removeValueDataNNumber();
				MenuFlag=0;
				setFocusNoteSerialNumber();
				return 1;
				
		default:
				MenuFlag=0;
				return 2;
	}	
}

//聚焦钞票序列号输入框
function setFocusNoteSerialNumber()
{
	document.getElementById('INPUT_NOTE_SERIALNO').style.backgroundColor="#FFFACD";
	document.getElementById('INPUT_NOTE_SERIALNO').click();
}

//去掉聚焦
function removeValueDataNNumber()
{
	document.getElementById('INPUT_NOTE_SERIALNO').style.backgroundColor="";
	document.getElementById('INPUT_NOTE_SERIALNO').blur();
};


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