<!-- setTimeout("switchMenu()",10); -->
function InvokeChildrenSwitch()
{
	removeValueDataNNumber();	
	return switchMenu();
}

function switchMenu()
{	
	var bottomButton = $("#FUN_10");
	var strTmp="";
	var buttonText=""
	buttonText = bottomButton.children(".button-text");	
	strTmp=buttonText.text();
	buttonText.text("1"+strTmp);
	bottomButton.val("Select1");
	return 1;
}

function removeValueDataNNumber()
{

	bottomButton = $("#FUN_10");//document.getElementById("FUN_LOGOUT");
	buttonText = bottomButton.children(".button-text");
	strTmp=buttonText.text();
	num=strTmp.substr(0,1);
	var reg=/^[0-9]+.?[0-9]*$/;
	if(reg.test(num))
	{			
		buttonText.text(strTmp.substr(1,strTmp.length-1));			
   	}	
	bottomButton.val("");
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
		case "5":$("button[value='Select5']").click();
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