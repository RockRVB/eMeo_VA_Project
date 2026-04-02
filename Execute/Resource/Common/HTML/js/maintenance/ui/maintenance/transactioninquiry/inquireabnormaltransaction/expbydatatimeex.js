ChildrenFlag=0;
	function InvokeChildrenSwitch(){
			
			switch (ChildrenFlag) {	
			case 0:	
				ChildrenFlag=1;
				//开始年
				removeValueDataNNumber();
				setStartYear();
				return 0;
			case 1:
				ChildrenFlag=2;
				//开始月
				removeValueDataNNumber();
				setStartMonth();
				return 0;
			case 2:
				ChildrenFlag=3;
				//开始日
				removeValueDataNNumber();
				setStartDay();
				return 0;
			case 3:
				ChildrenFlag=4;
				//开始时
				removeValueDataNNumber();
				setStartHour();
				return 0;
			case 4:
				ChildrenFlag=5;
				//开始分
				removeValueDataNNumber();
				setStartMinute();
				return 0;
			case 5:
				ChildrenFlag=6;
				//开始秒
				removeValueDataNNumber();
				setStartSecond();
				return 0;
			case 6:
				ChildrenFlag=7;
				//结束年
				removeValueDataNNumber();
				setEndYear();
				return 0;
			case 7:
				ChildrenFlag=8;
				//结束月
				removeValueDataNNumber();
				setEndMonth();
				return 0;
			case 8:
				ChildrenFlag=9;
				//结束日
				removeValueDataNNumber();
				setEndDay();
				return 0;
			case 9:
				ChildrenFlag=10;
				//结束时
				removeValueDataNNumber();
				setEndHour();
				return 0;
			case 10:
				ChildrenFlag=11;
				//结束分
				removeValueDataNNumber();
				setEndMinute();
				return 0;
			case 11:
				ChildrenFlag=0;
				//结束秒
				removeValueDataNNumber();
				setEndSecond();
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
				document.getElementById('INPUT_SHOUR').style.backgroundColor="";
				document.getElementById('INPUT_SMINUTE').style.backgroundColor="";
				document.getElementById('INPUT_SSECOND').style.backgroundColor="";
				document.getElementById('INPUT_EYEAR').style.backgroundColor="";
				document.getElementById('INPUT_EMONTH').style.backgroundColor="";
				document.getElementById('INPUT_EDAY').style.backgroundColor="";
				document.getElementById('INPUT_EHOUR').style.backgroundColor="";
				document.getElementById('INPUT_EMINUTE').style.backgroundColor="";
				document.getElementById('INPUT_ESECOND').style.backgroundColor="";
				
				document.getElementById('INPUT_SYEAR').blur();
				document.getElementById('INPUT_SMONTH').blur();
				document.getElementById('INPUT_SDAY').blur();				
				document.getElementById('INPUT_SHOUR').blur();
				document.getElementById('INPUT_SMINUTE').blur();
				document.getElementById('INPUT_SSECOND').blur();				
				document.getElementById('INPUT_EYEAR').blur();
				document.getElementById('INPUT_EMONTH').blur();
				document.getElementById('INPUT_EDAY').blur();
				document.getElementById('INPUT_EHOUR').blur();
				document.getElementById('INPUT_EMINUTE').blur();
				document.getElementById('INPUT_ESECOND').blur();
		}
		function setStartYear(){
				document.getElementById('INPUT_SYEAR').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_SYEAR').click();	
		
		}
		function setStartMonth(){
				document.getElementById('INPUT_SMONTH').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_SMONTH').click();
		}
		function setStartDay(){
				document.getElementById('INPUT_SDAY').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_SDAY').click();	
		
		}
		function setStartHour(){
				document.getElementById('INPUT_SHOUR').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_SHOUR').click();
		}
		function setStartMinute(){
				document.getElementById('INPUT_SMINUTE').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_SMINUTE').click();	
		
		}
		function setStartSecond(){
				document.getElementById('INPUT_SSECOND').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_SSECOND').click();
		}
		function setEndYear(){
				document.getElementById('INPUT_EYEAR').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_EYEAR').click();
		}
		function setEndMonth(){
				document.getElementById('INPUT_EMONTH').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_EMONTH').click();	
		
		}
		function setEndDay(){
				document.getElementById('INPUT_EDAY').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_EDAY').click();
		}		
		function setEndHour(){
				document.getElementById('INPUT_EHOUR').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_EHOUR').click();
		}
		function setEndMinute(){
				document.getElementById('INPUT_EMINUTE').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_EMINUTE').click();	
		
		}
		function setEndSecond(){
				document.getElementById('INPUT_ESECOND').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_ESECOND').click();
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