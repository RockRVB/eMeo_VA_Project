<!-- setTimeout("switchMenu()",10); -->
function InvokeChildrenSwitch()
{
	removeValueDataNNumber();
	return switchMenu();
}

function switchMenu()
{	
	var tmp=$('.middle-inner').find('button');		
	for(var i=0;i<tmp.length;i++)
	{
		$(tmp[i]).children(".tip").text(i+1);		
		//$(tmp[i]).children(".tip").val("Select"+(i+1));	//这行设置错,点击按钮没反应	,修改成下面这行
		$(tmp[i]).val("Select"+(i+1));
	}		
	return 1;
}

function removeValueDataNNumber()
{
	var tmp=$('.middle-inner').find('button');		
	for(var i=0;i<tmp.length;i++)
	{
		var tmpText=$(tmp[i]).children(".tip").text();	
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=tmpText.substr(0,1);
		if(reg.test(num))
		{
			$(tmp[i]).children(".tip").text(tmpText.substr(1,tmpText.length-1));	
    	}			
		$(tmp[i]).children(".tip").val("");		
	}	
}


function HandleKeyboardAction(keyValue) 
{ 
	switch (keyValue) {				
	   
		case "1":		
			$("button[value='Select1']").click();
			$("#Select1").click();
			return true;			
		case "2":
			$("button[value='Select2']").click();
			$("#Select2").click();
			return true; 
		case "3":
			$("button[value='Select3']").click();
			$("#Select3").click();
			return true; 
		case "4":
			$("button[value='Select4']").click();
			$("#Select4").click();
			return true; 
		case "5":
			$("button[value='Select5']").click();
			$("#Select5").click();
			return true; 
		case "6":
			$("button[value='Select6']").click();
			$("#Select6").click();
			return true;  
		case "7":
			$("button[value='Select7']").click();
			$("#Select7").click();			
			return true;  
		case "8":
			$("button[value='Select8']").click();
			$("#Select8").click();
			return true;  
		case "9":
			$("button[value='Select9']").click();
			$("#Select9").click();
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