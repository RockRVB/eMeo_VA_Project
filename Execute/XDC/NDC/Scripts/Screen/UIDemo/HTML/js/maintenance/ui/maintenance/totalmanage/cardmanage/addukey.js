//setTimeout("InvokeChildrenSwitch()",10000); 
var inputFlag = 0;
var canSelectCount = 0;
var tableNone1Flag = $("#STATIC_TR_S1")[0].style.display == 'none';
var tableNone2Flag = $("#STATIC_TR_S2")[0].style.display == 'none';
var tableNone3Flag = $("#STATIC_TR_S3")[0].style.display == 'none';
var firstRun = true;

function InvokeChildrenSwitch() {

	removeValueDataNNumber();
	return switchMenu();
}

function switchMenu() {
	if (tableNone1Flag && tableNone2Flag && tableNone3Flag) {
		return 2;
	}
	if (firstRun) {
		if (!tableNone1Flag) {
			canSelectCount = canSelectCount + 1;
		}
		if (!tableNone2Flag) {
			canSelectCount = canSelectCount + 1;
		}
		if (!tableNone3Flag) {
			canSelectCount = canSelectCount + 1;
		}
		firstRun = false;
	}
	var inputTextBox = $("#STATIC_TR_S1");
	switch (inputFlag) {
		case 0:
			inputTextBox = $("#STATIC_TR_S1");
			if (!tableNone1Flag) {
				//如果第一个槽位可用,则焦点到第一个输入框
				var inputFocus = $("#INPUT_AddCount_S1");
				inputFocus.click();
				document.getElementById('INPUT_AddCount_S1').style.backgroundColor = "#FFFACD";
				var returnValue = 0;
				inputFlag = 1;
				//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
				canSelectCount = canSelectCount - 1;
				if (canSelectCount > 0) {
					returnValue = 0;
				} else if (canSelectCount == 0) {
					returnValue = 1;
					inputFlag = 0;
				} else {
					returnValue = 2;
					inputFlag = 0;
				}
				return returnValue;
			} else {
				inputTextBox = $("#STATIC_TR_S2");
				var returnValue = 0;
				if (!tableNone2Flag) {
					//如果第二个槽位可用，则焦点到第二个输入框
					var inputFocus = $("#INPUT_AddCount_S2");
					inputFocus.click();
					document.getElementById('INPUT_AddCount_S2').style.backgroundColor = "#FFFACD";
					var returnValue = 0;
					inputFlag = 2;
					//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
					canSelectCount = canSelectCount - 1;
					if (canSelectCount > 0) {
						returnValue = 0;
					} else if (canSelectCount == 0) {
						returnValue = 1;
						inputFlag = 0;
					} else {
						returnValue = 2;
						inputFlag = 0;
					}
					return returnValue;
				} else {
					inputTextBox = $("#STATIC_TR_S3");
					if (!tableNone3Flag) {
						//如果第三个槽位可用，则焦点到第三个输入框
						var inputFocus = $("#INPUT_AddCount_S3");
						inputFocus.click();
						document.getElementById('INPUT_AddCount_S3').style.backgroundColor = "#FFFACD";
						var returnValue = 0;
						inputFlag = 3;
						//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
						canSelectCount = canSelectCount - 1;
						if (canSelectCount > 0) {
							returnValue = 0;
						} else if (canSelectCount == 0) {
							returnValue = 1;
							inputFlag = 0;
						} else {
							returnValue = 2;
							inputFlag = 0;
						}
						return returnValue;
					} else {
						inputFlag = 0;
						return 2;
					}
				}
			}

		case 1:
			inputTextBox = $("#STATIC_TR_S2");
			var returnValue = 0;
			if (!tableNone2Flag) {
				//如果第二个槽位可用，则焦点到第二个输入框
				var inputFocus = $("#INPUT_AddCount_S2");
				inputFocus.click();
				document.getElementById('INPUT_AddCount_S2').style.backgroundColor = "#FFFACD";
				var returnValue = 0;
				inputFlag = 2;
				//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
				canSelectCount = canSelectCount - 1;
				if (canSelectCount > 0) {
					returnValue = 0;
				} else if (canSelectCount == 0) {
					returnValue = 1;
					inputFlag = 0;
				} else {
					returnValue = 2;
					inputFlag = 0;
				}
				return returnValue;
			} else {
				inputTextBox = $("#STATIC_TR_S3");
				if (!tableNone3Flag) {
					//如果第三个槽位可用，则焦点到第三个输入框
					var inputFocus = $("#INPUT_AddCount_S3");
					inputFocus.click();
					document.getElementById('INPUT_AddCount_S3').style.backgroundColor = "#FFFACD";
					var returnValue = 0;
					inputFlag = 3;
					//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
					canSelectCount = canSelectCount - 1;
					if (canSelectCount > 0) {
						returnValue = 0;
					} else if (canSelectCount == 0) {
						returnValue = 1;
						inputFlag = 0;
					} else {
						returnValue = 2;
						inputFlag = 0;
					}
					return returnValue;
				} else {
					inputFlag = 0;
					return 2;
				}
			}
		case 2:
			inputTextBox = $("#STATIC_TR_S3");
			if (!tableNone3Flag) {
				//如果第三个槽位可用，则焦点到第三个输入框
				var inputFocus = $("#INPUT_AddCount_S3");
				inputFocus.click();
				document.getElementById('INPUT_AddCount_S3').style.backgroundColor = "#FFFACD";
				var returnValue = 0;
				inputFlag = 0;
				//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
				canSelectCount = canSelectCount - 1;
				if (canSelectCount > 0) {
					returnValue = 0;
				} else if (canSelectCount == 0) {
					returnValue = 1;
					inputFlag = 0;
				} else {
					returnValue = 2;
					inputFlag = 0;
				}
				return returnValue;
			} else {
				inputFlag = 0;
				return 2;
			}
		default:
			inputFlag = 2;
			return 0;
	}
	return 1;
}

function removeValueDataNNumber() {
	document.getElementById('INPUT_AddCount_S3').style.backgroundColor = "";
	document.getElementById('INPUT_AddCount_S2').style.backgroundColor = "";
	document.getElementById('INPUT_AddCount_S1').style.backgroundColor = "";
	document.getElementById('INPUT_AddCount_S3').blur();
	document.getElementById('INPUT_AddCount_S2').blur();
	document.getElementById('INPUT_AddCount_S1').blur();
}

function HandleKeyboardAction(keyValue) {
	switch (keyValue) {
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
	SelectOption.removeAttr("multiple");

}
