
<!-- setTimeout("switchMenu()",10); -->
var inputFlag=0;
function InvokeChildrenSwitch()
{
	removeValueDataNNumber();	
	return switchMenu();
}

function switchMenu()
{	
	var inputFocus=$("#INPUT_SERIALNO_BEGIN");
	switch(inputFlag)
	{
		case 0:
			inputFocus=$("#INPUT_SERIALNO_BEGIN");
			inputFocus.click();
			document.getElementById('INPUT_SERIALNO_BEGIN').style.backgroundColor="#FFFACD";
			inputFlag=1;
			return 0;
		case 1:
			inputFocus=$("#INPUT_SERIALNO_END");
			inputFocus.click();
			document.getElementById('INPUT_SERIALNO_END').style.backgroundColor="#FFFACD";
			inputFlag=2;
			return 0;
		case 2:
			inputFocus=$("#INPUT_YEAR");
			inputFocus.click();
			document.getElementById('INPUT_YEAR').style.backgroundColor="#FFFACD";
			inputFlag=3;
			return 0;
		case 3:
			inputFocus=$("#INPUT_MONTH");
			inputFocus.click();
			document.getElementById('INPUT_MONTH').style.backgroundColor="#FFFACD";
			inputFlag=4;
			return 0;
		case 4:
			inputFocus=$("#INPUT_DAY");
			inputFocus.click();
			document.getElementById('INPUT_DAY').style.backgroundColor="#FFFACD";
			inputFlag=0;
			return 1;		
		default:			
			inputFlag=2;
			return 0;
	}	
}

function removeValueDataNNumber()
{	
	document.getElementById('INPUT_SERIALNO_BEGIN').style.backgroundColor="";
	document.getElementById('INPUT_SERIALNO_END').style.backgroundColor="";
	document.getElementById('INPUT_YEAR').style.backgroundColor="";
	document.getElementById('INPUT_MONTH').style.backgroundColor="";
	document.getElementById('INPUT_DAY').style.backgroundColor="";
	document.getElementById('INPUT_SERIALNO_BEGIN').blur();	
	document.getElementById('INPUT_SERIALNO_END').blur();
	document.getElementById('INPUT_YEAR').blur();	
	document.getElementById('INPUT_MONTH').blur();	
	document.getElementById('INPUT_DAY').blur();
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