<!-- setTimeout("switchMenu()",10); -->
<!-- alert($("#table_1").attr("rowCount")); -->
var switchFlag=0;
var inputFlag=0;
function InvokeChildrenSwitch()
{
	removeValueDataNNumber();
	return switchMenu();
}

function switchMenu()
{	
	var tmp=$('.type-table-paging');
	if(tmp.is('.none'))
	{		
		//var tableTmp=$(".type-table-container table tbody tr");
		var tableTmp=$("#table_1").attr("rowCount");
		//var tableCount=tableTmp.length;
		if(tableTmp-inputFlag>0)
		{
			var returnValue=0;
			switch(inputFlag)
			{
				case 0:
					switchTr1()
					break;
				case 1:
					switchTr2()
					break;
				case 2:
					switchTr3()
					break;
				case 3:
					switchTr4()
					break;
				case 4:
					switchTr5()
					break;
				case 5:
					switchTr6()
					break;
				case 6:
					switchTr7()
					break;
				case 7:
					switchTr8()
					break;
				case 8:
					switchTr1()
					inputFlag=0;
					returnValue=1;
					break;
				default:
					inputFlag=2;
					returnValue= 0;
					break;
			}
			if(tableTmp-inputFlag==0)
			{
				inputFlag=0;
				returnValue=1;
			}
			return returnValue;
		}
		else
		{
			inputFlag=0;
			return 1;
		}
				
	}
	else
	{
		switch(switchFlag)
		{
			case 0:
				var tmp1=$('.middle-inner').find('button');		
				for(var i=0;i<tmp1.length;i++)
				{
					$(tmp1[i]).children(".tip").text(i+1);		
					$(tmp1[i]).children(".tip").val("Select"+(i+1));	
				}
				switchFlag=1;
				return 0;
			case 1:
				var tableTmp=$(".type-table-container table tbody tr");
				var tableCount=tableTmp.length;
				if(tableCount-inputFlag>0)
				{
					var returnValue=0;
					switch(inputFlag)
					{
						case 0:
							switchTr1()
							break;
						case 1:
							switchTr2()
							break;
						case 2:
							switchTr3()
							break;
						case 3:
							switchTr4()
							break;
						case 4:
							switchTr5()
							break;
						case 5:
							switchTr6()
							break;
						case 6:
							switchTr7()
							break;
						case 7:
							switchTr8()
							break;
						case 8:
							switchTr1()
							inputFlag=0;
							returnValue=1;
							break;
						default:
							inputFlag=2;
							returnValue= 0;
							break;
					}
					switchFlag=0;
					return returnValue;
				}
				else
				{
					switchFlag=0;
					inputFlag=0;
					return 1;
				}
			default:
				switchFlag=0;
				inputFlag=0;
				return 2;
		}		
	}	
	return 2;
}

function switchTr1()
{
	inputFocus=$("#INPUT_SHEET1");
	inputFocus.click();
	document.getElementById('INPUT_SHEET1').style.backgroundColor="#FFFACD";
	inputFlag=1;
}
function switchTr2()
{
	inputFocus=$("#INPUT_SHEET2");
	inputFocus.click();
	document.getElementById('INPUT_SHEET2').style.backgroundColor="#FFFACD";
	inputFlag=2;
}
function switchTr3()
{
	inputFocus=$("#INPUT_SHEET3");
	inputFocus.click();
	document.getElementById('INPUT_SHEET3').style.backgroundColor="#FFFACD";
	inputFlag=3;
}
function switchTr4()
{
	inputFocus=$("#INPUT_SHEET4");
	inputFocus.click();
	document.getElementById('INPUT_SHEET4').style.backgroundColor="#FFFACD";
	inputFlag=4;
}
function switchTr5()
{
	inputFocus=$("#INPUT_SHEET5");
	inputFocus.click();
	document.getElementById('INPUT_SHEET5').style.backgroundColor="#FFFACD";
	inputFlag=5;
}
function switchTr6()
{
	inputFocus=$("#INPUT_SHEET6");
	inputFocus.click();
	document.getElementById('INPUT_SHEET6').style.backgroundColor="#FFFACD";
	inputFlag=6;
}
function switchTr7()
{
	inputFocus=$("#INPUT_SHEET7");
	inputFocus.click();
	document.getElementById('INPUT_SHEET7').style.backgroundColor="#FFFACD";
	inputFlag=7;
}
function switchTr8()
{
	inputFocus=$("#INPUT_SHEET8");
	inputFocus.click();
	document.getElementById('INPUT_SHEET8').style.backgroundColor="#FFFACD";
	inputFlag=8;
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
	
	document.getElementById('INPUT_SHEET1').style.backgroundColor="";
	document.getElementById('INPUT_SHEET2').style.backgroundColor="";
	document.getElementById('INPUT_SHEET3').style.backgroundColor="";
	document.getElementById('INPUT_SHEET4').style.backgroundColor="";
	document.getElementById('INPUT_SHEET5').style.backgroundColor="";
	document.getElementById('INPUT_SHEET6').style.backgroundColor="";
	document.getElementById('INPUT_SHEET7').style.backgroundColor="";
	document.getElementById('INPUT_SHEET8').style.backgroundColor="";
	document.getElementById('INPUT_SHEET1').blur();	
	document.getElementById('INPUT_SHEET2').blur();	
	document.getElementById('INPUT_SHEET3').blur();
	document.getElementById('INPUT_SHEET4').blur();	
	document.getElementById('INPUT_SHEET5').blur();	
	document.getElementById('INPUT_SHEET6').blur();
	document.getElementById('INPUT_SHEET7').blur();	
	document.getElementById('INPUT_SHEET8').blur();
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
