ChildrenFlag=0;
	function InvokeChildrenSwitch(){
			removeValueDataNNumber();
			switch (ChildrenFlag) {	
			case 0:		
				var tdValueList=$(".type-table-container tbody tr");
				var buttonListLength=tdValueList.length;
				if(buttonListLength>0)
				{
					ChildrenFlag=1;
					SetCheckboxValue();
					return 0;
				}
			case 1:
				var buttonList=$(".wrapper button");
				var buttonListLength=buttonList.length;
				if(buttonListLength>0)
				{
					ChildrenFlag=0;
					SetButtomValue();
					return 1;
				}	
			default:
				ChildrenFlag=0;
				return 0;
			} 
	}	
		function removeValueDataNNumber(){						
			var strTmp="";
			var buttonText=""
			var bottomButton = $("#FUN_OK");//document.getElementById("FUN_LOGOUT");
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num=strTmp.substr(0,1);
			if(reg.test(num))
			{
				buttonText.text(strTmp.substr(1,strTmp.length-1));
	    	}			
			bottomButton.attr("name","");
			
			var buttonList=$(".middle-inner button");
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

			var tdValueList=$(".type-table-container tbody tr");
			var buttonListLength=tdValueList.length;
			//alert(buttonListLength);
			for(var i=0;i<buttonListLength;i++)
			{
				var span = $(($(tdValueList[i]).children("td"))[4]).children("span");
				var spanText = $(span).text();
				var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
				var num=spanText.substr(0,1);
				if(reg.test(num))
				{
		      		 $(span).text(spanText.substr(1,spanText.length-1));
		    	}	
				//$(span).attr("id","");
				//$(span).attr("id","Select"+(i+1));
				//var btn = $(($(tdValueList[i]).children("td"))[4]).children("input");
				//$(btn).attr("id","");
			}
			
			
			return true;
			
		}
		function SetButtomValue(){
			var buttonList=$(".middle-inner button");
			var buttonListLength=buttonList.length;
			for(var i=0;i<buttonListLength;i++)
			{
				var span = $(buttonList[i]).children(".button-text");
				var spanText = $(span).text();
				//console.log(spanText);
				$(span).text((i+1)+spanText);
				$(buttonList[i]).attr("name","Select"+(i+1));
			}
			
			var strTmp="";
			var buttonText=""
			var bottomButton = $("#FUN_OK");//document.getElementById("FUN_LOGOUT");
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			buttonText.text("3"+strTmp);
			bottomButton.attr("name","Select3");	
			
			return true;
	}
		function SetCheckboxValue(){			
			var tdValueList=$(".type-table-container tbody tr");
			var buttonListLength=tdValueList.length;
			//alert(buttonListLength);
			for(var i=0;i<buttonListLength;i++)
			{
				var span = $(($(tdValueList[i]).children("td"))[4]).children("span");
				var spanText = $(span).text();
				//alert(spanText);
				$(span).text((i+1)+spanText);
				//$(span).attr("id","Select"+(i+1));
				//var btn = $(($(tdValueList[i]).children("td"))[4]).children("input");
				//$(btn).attr("id","Select"+(i+1));
			}
			return true;
	}
		function SetDownButtomValue(){
			removeValueDataNNumber();
			//每按向下按钮一次表格减一行，然后重新分配按钮值
			var tdValueList=$(".type-table-container tbody tr");
			var buttonListLength=tdValueList.length;
			//alert(buttonListLength-1);
			for(var i=0;i<buttonListLength;i++)
			{
				var span = $(($(tdValueList[i]).children("td"))[4]).children("span");
				var spanText = $(span).text();
				//alert(spanText);
				$(span).text((i+1)+spanText);
				//$(span).attr("id","Select"+(i+1));
				//var btn = $(($(tdValueList[i]).children("td"))[4]).children("input");
				//$(btn).attr("id","Select"+(i+1));
			}			
			
			return true;
	}
		function SetUpButtomValue(){
			removeValueDataNNumber();
			//每按向上按钮一次表格减一行，然后重新分配按钮值
			var tdValueList=$(".type-table-container tbody tr");
			var buttonListLength=tdValueList.length;
			//alert(buttonListLength);
			for(var i=0;i<buttonListLength;i++)
			{
				var span = $(($(tdValueList[i]).children("td"))[4]).children("span");
				var spanText = $(span).text();
				//alert(spanText);
				$(span).text((i+1)+spanText);
				//$(span).attr("id","Select"+(i+1));
				//var btn = $(($(tdValueList[i]).children("td"))[4]).children("input");
				//$(btn).attr("id","Select"+(i+1));
			}			
			
			return true;
	}	

function HandleKeyboardAction(keyValue) 
{ 
	switch (keyValue) {				
	   
		case "1":		
			$("button[name='Select1']").click();
			$("#Select1").click();
			if(ChildrenFlag == 1)
			{
				$("#CheckboxGroup_1_5").click();				
			}
			return true;			
		case "2":
			$("button[name='Select2']").click();
			$("#Select2").click();
			if(ChildrenFlag == 1)
			{
				$("#CheckboxGroup_2_5").click();				
			}
			return true; 
		case "3":
			$("button[name='Select3']").click();
			$("#Select3").click();
			if(ChildrenFlag == 1)
			{
				$("#CheckboxGroup_3_5").click();				
			}
			return true; 
		case "4":
			$("button[name='Select4']").click();
			$("#Select4").click();
			if(ChildrenFlag == 1)
			{
				$("#CheckboxGroup_4_5").click();				
			}
			return true; 
		case "5":$("button[name='Select5']").click();
			$("#Select5").click();
			if(ChildrenFlag == 1)
			{
				$("#CheckboxGroup_5_5").click();				
			}
			return true; 
		case "6":
			$("button[name='Select6']").click();
			$("#Select6").click();
			if(ChildrenFlag == 1)
			{
				$("#CheckboxGroup_6_5").click();				
			}
			return true;  
		case "7":
			$("button[name='Select7']").click();
			$("#Select7").click();		
			if(ChildrenFlag == 1)
			{
				$("#CheckboxGroup_7_5").click();				
			}	
			return true;  
		case "8":
			$("button[name='Select8']").click();
			$("#Select8").click();
			if(ChildrenFlag == 1)
			{
				$("#CheckboxGroup_8_5").click();				
			}
			return true;  
		case "9":
			$("button[value='Select9']").click();
			$("#Select9").click();
			if(ChildrenFlag == 1)
			{
				$("#CheckboxGroup_9_5").click();				
			}
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