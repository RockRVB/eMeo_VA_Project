ChildrenFlag=0;
	function InvokeChildrenSwitch(){
			
			switch (ChildrenFlag) {	
			case 0:	
				ChildrenFlag=1;
				//ATM号
				removeValueDataNNumber();
				setATMNo();
				return 0;
			case 1:
				ChildrenFlag=2;
				//银行号
				removeValueDataNNumber();
				setBankNo();
				return 0;
			case 2:
				ChildrenFlag=0;
				//网点号
				removeValueDataNNumber();
				setNetNo();
				return 1;			
			default:
				ChildrenFlag=0;
				return 2;
			}  
	}
		function removeValueDataNNumber(){
				document.getElementById('INPUT_ATMNO').style.backgroundColor="";
				document.getElementById('INPUT_BLANKNO').style.backgroundColor="";
				document.getElementById('INPUT_NETNO').style.backgroundColor="";
				document.getElementById('INPUT_ATMNO').blur();
				document.getElementById('INPUT_BLANKNO').blur();
				document.getElementById('INPUT_NETNO').blur();
		}
		function setATMNo(){
				document.getElementById('INPUT_ATMNO').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_ATMNO').click();	
		
		}
		function setBankNo(){
				document.getElementById('INPUT_BLANKNO').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_BLANKNO').click();
		}
		function setNetNo(){
				document.getElementById('INPUT_NETNO').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_NETNO').click();	
		
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
			//$("#keyboard-button-28").click();
			$("#keyboard-button-29").click();
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