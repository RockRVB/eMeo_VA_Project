<!-- setTimeout("InvokeChildrenSwitch()",10); -->
	ChildrenFlag=0;
	var InputFlag=0;
	function InvokeChildrenSwitch(){
			removeValueDataNNumber();
			InputFlag=0;
			switch (ChildrenFlag) {	
			case 0:	
				ChildrenFlag=1;
				//IP输入框响应
				setATMPIp();
				return 0;
			case 1:
				ChildrenFlag=2;
				//端口号输入框响应
				setATMPPort();
				return 0;
			case 2:			
				ChildrenFlag=0;
				//按钮设值
				SetButtomValue();
				InputFlag=1;
				return 1;
			default:
				ChildrenFlag=0;
				InputFlag=0;
				return 2;
			}  
	}
		function removeValueDataNNumber(){
				document.getElementById('INPUT_IP').style.backgroundColor="";
				document.getElementById('INPUT_PORT').style.backgroundColor="";
				document.getElementById('INPUT_IP').blur();
				document.getElementById('INPUT_PORT').blur();
				
				var bottomButton=$("#FUN_0");			
				buttonText = bottomButton.children(".button-text");
				strTmp=buttonText.text();
				var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
				var num=strTmp.substr(0,1);
				if(reg.test(num))
				{
					buttonText.text(strTmp.substr(1,strTmp.length-1));
			    }			
				bottomButton.attr("name",""); 
				
				bottomButton=$("#FUN_1");			
				buttonText = bottomButton.children(".button-text");
				strTmp=buttonText.text();
				var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
				var num=strTmp.substr(0,1);
				if(reg.test(num))
				{
					buttonText.text(strTmp.substr(1,strTmp.length-1));
			    }			
				bottomButton.attr("name",""); 
				
				bottomButton=$("#FUN_2");			
				buttonText = bottomButton.children(".button-text");
				strTmp=buttonText.text();
				var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
				var num=strTmp.substr(0,1);
				if(reg.test(num))
				{
					buttonText.text(strTmp.substr(1,strTmp.length-1));
			    }			
				bottomButton.attr("name",""); 
				
		}
		function setATMPIp(){
				document.getElementById('INPUT_IP').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_IP').click();	
		
		}
		function setATMPPort(){
				document.getElementById('INPUT_PORT').style.backgroundColor="#FFFACD";
				document.getElementById('INPUT_PORT').click();
		}		
		
		function SetButtomValue(){	
			var bottomButton=$("#FUN_0");			
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			buttonText.text("1"+strTmp);
			bottomButton.attr("name","Select1"); 
			
			bottomButton=$("#FUN_1");			
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			buttonText.text("2"+strTmp);
			bottomButton.attr("name","Select2"); 
			
			bottomButton=$("#FUN_2");			
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			buttonText.text("3"+strTmp);
			bottomButton.attr("name","Select3"); 
			
			
			return true;
	}

function HandleKeyboardAction(keyValue) 
{ 
	switch (keyValue) 
	{
		//新增数字键事件响应,支持键盘操作,Start......
		case "1":			
			if(InputFlag==1)
			{
				$("button[name='Select1']").click();
			}
			else
			{
				$("#keyboard-button-1").click();
			}
			break;
		case "2":
			if(InputFlag==1)
			{
				$("button[name='Select2']").click();
			}
			else
			{
				$("#keyboard-button-2").click();
			}
			break;
		case "3":
			if(InputFlag==1)
			{
				$("button[name='Select3']").click();
			}
			else
			{
				$("#keyboard-button-3").click();
			}
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
	    case ".":
			$("#keyboard-button-26").click();
			break;
	    case "CLEAR":
			$("#keyboard-button-11").click();
			$("#keyboard-button-28").click();
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