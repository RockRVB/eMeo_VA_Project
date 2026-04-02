ChildrenFlag=0;
	function InvokeChildrenSwitch(){
			
			switch (ChildrenFlag) {	
			case 0:	
				ChildrenFlag=1;
				//年
				removeValueDataNNumber();
				setYear();
				return 0;
			case 1:
				ChildrenFlag=2;
				//月
				removeValueDataNNumber();
				setMonth();
				return 0;
			case 2:
				ChildrenFlag=3;
				//日
				removeValueDataNNumber();
				setDay();
				return 0;
			case 3:
				ChildrenFlag=4;
				//时
				removeValueDataNNumber();
				setHour();
				return 0;
			case 4:
				ChildrenFlag=5;
				//分
				removeValueDataNNumber();
				setMinute();
				return 0;
			case 5:
				ChildrenFlag=0;
				//秒
				removeValueDataNNumber();
				setSecond();
				return 1;				
			default:
				ChildrenFlag=0;
				return 2;
			}  
	}
		function removeValueDataNNumber(){
				document.getElementById('INPUT_YEAR').style.backgroundColor="";
				document.getElementById('INPUT_MONTH').style.backgroundColor="";
				document.getElementById('INPUT_DAY').style.backgroundColor="";
				document.getElementById('INPUT_HOUR').style.backgroundColor="";
				document.getElementById('INPUT_MINUTE').style.backgroundColor="";
				document.getElementById('INPUT_SECOND').style.backgroundColor="";				
				document.getElementById('INPUT_YEAR').blur();
				document.getElementById('INPUT_MONTH').blur();
				document.getElementById('INPUT_DAY').blur();
				document.getElementById('INPUT_HOUR').blur();
				document.getElementById('INPUT_MINUTE').blur();
				document.getElementById('INPUT_SECOND').blur();
		}
		function setYear(){
				document.getElementById('INPUT_YEAR').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_YEAR').click();	
		
		}
		function setMonth(){
				document.getElementById('INPUT_MONTH').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_MONTH').click();
		}
		function setDay(){
				document.getElementById('INPUT_DAY').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_DAY').click();	
		
		}
		function setHour(){
				document.getElementById('INPUT_HOUR').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_HOUR').click();
		}
		function setMinute(){
				document.getElementById('INPUT_MINUTE').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_MINUTE').click();	
		
		}
		function setSecond(){
				document.getElementById('INPUT_SECOND').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_SECOND').click();
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