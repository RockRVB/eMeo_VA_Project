
<!-- setTimeout("switchMenu()",10); -->
var inputFlag=0;
function InvokeChildrenSwitch()
{
	removeValueDataNNumber();	
	return switchMenu();
}

function switchMenu()
{	
	var inputFocus=$("#INPUT_SYEAR");
	switch(inputFlag)
	{
		case 0:
			inputFocus=$("#INPUT_SYEAR");
			inputFocus.click();
			document.getElementById('INPUT_SYEAR').style.backgroundColor="#FFFACD";
			inputFlag=1;
			return 0;
		case 1:
			inputFocus=$("#INPUT_SMONTH");
			inputFocus.click();
			document.getElementById('INPUT_SMONTH').style.backgroundColor="#FFFACD";
			inputFlag=2;
			return 0;
		case 2:
			inputFocus=$("#INPUT_SDAY");
			inputFocus.click();
			document.getElementById('INPUT_SDAY').style.backgroundColor="#FFFACD";
			inputFlag=3;
			return 0;
		case 3:
			inputFocus=$("#INPUT_EYEAR");
			inputFocus.click();
			document.getElementById('INPUT_EYEAR').style.backgroundColor="#FFFACD";
			inputFlag=4;
			return 0;
		case 4:
			inputFocus=$("#INPUT_EMONTH");
			inputFocus.click();
			document.getElementById('INPUT_EMONTH').style.backgroundColor="#FFFACD";
			inputFlag=5;
			return 0;
		case 5:
			inputFocus=$("#INPUT_EDAY");
			inputFocus.click();
			document.getElementById('INPUT_EDAY').style.backgroundColor="#FFFACD";
			inputFlag=6;
			return 0;
		case 6:
			inputFocus=$("#INPUT_CARDNUM");
			inputFocus.click();
			document.getElementById('INPUT_CARDNUM').style.backgroundColor="#FFFACD";
			inputFlag=0;
			return 1;
		default:			
			inputFlag=2;
			return 0;
	}	
}

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