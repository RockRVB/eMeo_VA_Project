ChildrenFlag=0;
	function InvokeChildrenSwitch(){
			
			switch (ChildrenFlag) {	
			case 0:	
				ChildrenFlag=1;
				//开始年
				removeValueDataNNumber();
				setYear();
				return 0;
			case 1:
				ChildrenFlag=2;
				//开始月
				removeValueDataNNumber();
				setMonth();
				return 0;
			case 2:
				ChildrenFlag=3;
				//开始日
				removeValueDataNNumber();
				setDay();
				return 0;
			case 3:
				ChildrenFlag=4;
				//结束年
				removeValueDataNNumber();
				setHour();
				return 0;
			case 4:
				ChildrenFlag=5;
				//结束月
				removeValueDataNNumber();
				setMinute();
				return 0;
			case 5:
				ChildrenFlag=0;
				//结束日
				removeValueDataNNumber();
				setSecond();
				return 1;				
			default:
				ChildrenFlag=0;
				return 2;
			}  
	}
		function removeValueDataNNumber(){
				document.getElementById('INPUT_SYEAR').style.backgroundColor="";
				document.getElementById('INPUT_SMONTH').style.backgroundColor="";
				document.getElementById('INPUT_SDAY').style.backgroundColor="";
				document.getElementById('INPUT_EYEAR').style.backgroundColor="";
				document.getElementById('INPUT_EMONTH').style.backgroundColor="";
				document.getElementById('INPUT_EDAY').style.backgroundColor="";				
				document.getElementById('INPUT_SYEAR').blur();
				document.getElementById('INPUT_SMONTH').blur();
				document.getElementById('INPUT_SDAY').blur();
				document.getElementById('INPUT_EYEAR').blur();
				document.getElementById('INPUT_EMONTH').blur();
				document.getElementById('INPUT_EDAY').blur();
		}
		function setYear(){
				document.getElementById('INPUT_SYEAR').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_SYEAR').click();	
		
		}
		function setMonth(){
				document.getElementById('INPUT_SMONTH').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_SMONTH').click();
		}
		function setDay(){
				document.getElementById('INPUT_SDAY').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_SDAY').click();	
		
		}
		function setHour(){
				document.getElementById('INPUT_EYEAR').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_EYEAR').click();
		}
		function setMinute(){
				document.getElementById('INPUT_EMONTH').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_EMONTH').click();	
		
		}
		function setSecond(){
				document.getElementById('INPUT_EDAY').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_EDAY').click();
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