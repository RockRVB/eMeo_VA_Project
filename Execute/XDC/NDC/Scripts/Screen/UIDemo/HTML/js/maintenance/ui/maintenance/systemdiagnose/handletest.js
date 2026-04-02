ChildrenFlag=0;
	function InvokeChildrenSwitch(){
			removeValueDataNNumber();
			switch (ChildrenFlag) {	
			case 0:		
				var buttonList=$(".middle-inner button");
				var buttonListLength=buttonList.length;
				if(buttonListLength>0)
				{
					ChildrenFlag=1;
					SetBroadsideValue();
					return 0;
				}	
				return 0;
			case 1:
				var buttonList=$(".g-button-zone button");
				var buttonListLength=buttonList.length;
				if(buttonListLength>0)
				{
					ChildrenFlag=0;
					SetButtomValue();
					return 1;
				}
				return 1;	
			default:
				ChildrenFlag=0;
				removeValueDataNNumber();
				return 2;
			}   
	}	
		function removeValueDataNNumber(){
			//清除侧边栏按钮设定的值
			var sideButtonList = $(".middle-inner button");
			var sideLength = sideButtonList.length;
			for(var i=0;i<sideLength;i++)
			{
				var tip = ($(sideButtonList[i])).children(".tip");
				var tipText = $(tip).text();
				var reg=/^[0-9]+.?[0-9]*$/; 
				if(reg.test(tipText))
				{
					$(tip).text("");
				}
				$(sideButtonList[i]).attr("name","");
			}
			
			//清除底部按钮设定的值
			var buttonList=$(".g-button-zone button");
			var buttonListLength=buttonList.length;
			for(var i=0;i<buttonListLength;i++)
			{
				var span = $(buttonList[i]).children(".button-text");
				var spanText = $(span).text();
				
				var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
				var num=spanText.substr(0,1);
				if(reg.test(num))
				{
	      		 	$(span).text(spanText.substr(1,spanText.length-1));
	    		}			
				$(buttonList[i]).attr("name","");
			}	
			return true;
			
		}
		function SetBroadsideValue(){
			var sideButtonList=$(".middle-inner button");
			var sideListLength=sideButtonList.length;
			for(var i=0;i<sideListLength;i++)
			{
			var span = ($(sideButtonList[i]).children(".tip"));
			var spanText = $(span).text();
			//console.log(spanText);
			$(span).text((i+1)+spanText);
			$(sideButtonList[i]).attr("name","Select"+(i+1));
			}	
			return true;
		}
		function SetButtomValue(){
			var buttonList=$(".g-button-zone button");
			var buttonListLength=buttonList.length;
			for(var i=0;i<buttonListLength;i++)
			{
			var span = $(buttonList[i]).children(".button-text");
			var spanText = $(span).text();
			//console.log(spanText);
			$(span).text((i+1)+spanText);
			$(buttonList[i]).attr("name","Select"+(i+1));
			}
			return true;
	}

	function showAgain()
	{	
			switch (ChildrenFlag) {	
			case 1:		
				removeValueDataNNumber();	
				var buttonList=$(".middle-inner button");
				var buttonListLength=buttonList.length;
				if(buttonListLength>0)
				{					
					SetBroadsideValue();					
				}
				break;				
			case 0:
				removeValueDataNNumber();	
				var buttonList=$(".g-button-zone button");
				var buttonListLength=buttonList.length;
				if(buttonListLength>0)
				{				
					SetButtomValue();
				}
				break;
			default:
				break;
				
			}
	}

function HandleKeyboardAction(keyValue) 
{ 
	switch (keyValue) {				
	   
		case "1":		
			$("button[name='Select1']").click();			
			showAgain();
			return true;			
		case "2":
			$("button[name='Select2']").click();
			showAgain();
			return true; 
		case "3":
			$("button[name='Select3']").click();
			showAgain();
			return true; 
		case "4":
			$("button[name='Select4']").click();
			showAgain();
			return true; 
		case "5":$("button[name='Select5']").click();
			return true; 
		case "6":
			$("button[name='Select6']").click();
			return true;  
		case "7":
			$("button[name='Select7']").click();
			return true;  
		case "8":
			$("button[name='Select8']").click();
			return true;  
		case "9":
			$("button[name='Select9']").click();
			return true;  
		case "0":
			return true;  
		case "CLEAR":
			return true;  
		case "ENTER":
			return this.keyBoardAction("KEYBOARD_ENTER",key);
        case "CANCEL":return this.keyBoardAction("FUN_QUIT",key);
	    case "KEYBOARD_TAB":return this.SwitchMenu();
        // 后续插入快捷键等等
        //.........
	    default:
	        return this.keyBoardAction("keyboard",key);
	    }
}