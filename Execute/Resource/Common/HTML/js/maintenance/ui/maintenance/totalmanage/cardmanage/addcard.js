var inputFlag = 0;
var canSelectCount = 0;
var tableNone1Flag = true;
var tableNone2Flag = true;
var tableNone3Flag = true;
var firstRun = true;
var selectType = 0;
var selectFlag = false;

function checkTabelNoneFlag() {
	tableNone1Flag = $("#STATIC_TR_S1")[0].style.display == 'none';
	tableNone2Flag = $("#STATIC_TR_S2")[0].style.display == 'none';
	tableNone3Flag = $("#STATIC_TR_S3")[0].style.display == 'none';
}


function InvokeChildrenSwitch() {
	removeValueDataNNumber();
	checkTabelNoneFlag();
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
		canSelectCount = canSelectCount * 2;
		firstRun = false;
	}
	var inputTextBox = $("#STATIC_TR_S1");
	switch (inputFlag) {
		case 0:
			inputTextBox = $("#STATIC_TR_S1");
			if (!tableNone1Flag) {
				selectType = 1;
				//如果第一个槽位可用,则先点击select框,让其选择card类型;	
				if (canSelectCount % 2 == 0) {
					selectFlag = true;
					var selectFocus = $("#SELECT_CardType_S1");
					var optionList = $("#SELECT_CardType_S1 option");
					var optionLength = optionList.length;
					for (var i = 0; i < optionLength; i++) {
						var span = $(optionList[i])[0];
						var spanText = $(span).text();
						$(span).text((i + 1) + "." + spanText);
						$(span).attr("name", "Select" + (i + 1));
					}
					selectFocus.attr("multiple", "multiple");
					inputFlag = 0;
				} else {
					selectFlag = false;
					//如果第一个槽位可用,则焦点到第一个输入框
					var inputFocus = $("#INPUT_AddCount_S1");
					inputFocus.click();
					document.getElementById('INPUT_AddCount_S1').style.backgroundColor = "#FFFACD";
					inputFlag = 1;
					//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面

				}
				var returnValue = 0;
				canSelectCount = canSelectCount - 1;
				if (canSelectCount > 0) {
					returnValue = 0;
				} else if (canSelectCount == 0) {
					returnValue = 1;
					firstRun = true;
					inputFlag = 0;
				} else {
					returnValue = 2;
					firstRun = true;
					inputFlag = 0;
				}
				return returnValue;
			} else {
				inputTextBox = $("#STATIC_TR_S2");
				var returnValue = 0;
				if (!tableNone2Flag) {
					selectType = 2;
					//如果第二个槽位可用,则先点击select框,让其选择card类型;	
					if (canSelectCount % 2 == 0) {
						selectFlag = true;
						var selectFocus = $("#SELECT_CardType_S2");
						var optionList = $("#SELECT_CardType_S2 option");
						var optionLength = optionList.length;
						for (var i = 0; i < optionLength; i++) {
							var span = $(optionList[i])[0];
							var spanText = $(span).text();
							$(span).text((i + 1) + "." + spanText);
							$(span).attr("name", "Select" + (i + 1));
						}
						selectFocus.attr("multiple", "multiple");
						inputFlag = 0;
					} else {
						selectFlag = false;
						//如果第二个槽位可用，则焦点到第二个输入框
						var inputFocus = $("#INPUT_AddCount_S2");
						inputFocus.click();
						document.getElementById('INPUT_AddCount_S2').style.backgroundColor = "#FFFACD";
						var returnValue = 0;
						inputFlag = 2;
					}
					//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
					canSelectCount = canSelectCount - 1;
					if (canSelectCount > 0) {
						returnValue = 0;
					} else if (canSelectCount == 0) {
						returnValue = 1;
						firstRun = true;
						inputFlag = 0;
					} else {
						returnValue = 2;
						firstRun = true;
						inputFlag = 0;
					}
					return returnValue;
				} else {
					inputTextBox = $("#STATIC_TR_S3");
					if (!tableNone3Flag) {
						selectType = 3;
						//如果第三个槽位可用,则先点击select框,让其选择card类型;	
						if (canSelectCount % 2 == 0) {
							selectFlag = true;
							var selectFocus = $("#SELECT_CardType_S2");
							var optionList = $("#SELECT_CardType_S2 option");
							var optionLength = optionList.length;
							for (var i = 0; i < optionLength; i++) {
								var span = $(optionList[i])[0];
								var spanText = $(span).text();
								$(span).text((i + 1) + "." + spanText);
								$(span).attr("name", "Select" + (i + 1));
							}
							selectFocus.attr("multiple", "multiple");
							inputFlag = 0;
						} else {
							selectFlag = false;
							//如果第三个槽位可用，则焦点到第三个输入框
							var inputFocus = $("#INPUT_AddCount_S3");
							inputFocus.click();
							document.getElementById('INPUT_AddCount_S3').style.backgroundColor = "#FFFACD";
							inputFlag = 3;
						}
						//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
						canSelectCount = canSelectCount - 1;
						var returnValue = 0;
						if (canSelectCount > 0) {
							returnValue = 0;
						} else if (canSelectCount == 0) {
							returnValue = 1;
							inputFlag = 0;
							firstRun = true;
						} else {
							returnValue = 2;
							inputFlag = 0;
							firstRun = true;
						}
						return returnValue;
					} else {
						inputFlag = 0;
						firstRun = true;
						return 2;
					}
				}
			}

		case 1:
			inputTextBox = $("#STATIC_TR_S2");
			var returnValue = 0;
			if (!tableNone2Flag) {
				selectType = 2;
				//如果第二个槽位可用,则先点击select框,让其选择card类型;	
				if (canSelectCount % 2 == 0) {
					selectFlag = true;
					var selectFocus = $("#SELECT_CardType_S2");
					var optionList = $("#SELECT_CardType_S2 option");
					var optionLength = optionList.length;
					for (var i = 0; i < optionLength; i++) {
						var span = $(optionList[i])[0];
						var spanText = $(span).text();
						$(span).text((i + 1) + "." + spanText);
						$(span).attr("name", "Select" + (i + 1));
					}
					selectFocus.attr("multiple", "multiple");
					inputFlag = 1;
				} else {
					selectFlag = false;
					//如果第二个槽位可用，则焦点到第二个输入框
					var inputFocus = $("#INPUT_AddCount_S2");
					inputFocus.click();
					document.getElementById('INPUT_AddCount_S2').style.backgroundColor = "#FFFACD";
					var returnValue = 0;
					inputFlag = 2;
				}
				//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
				canSelectCount = canSelectCount - 1;
				if (canSelectCount > 0) {
					returnValue = 0;
				} else if (canSelectCount == 0) {
					returnValue = 1;
					firstRun = true;
				} else {
					returnValue = 2;
					firstRun = true;
				}
				return returnValue;
			} else {
				inputTextBox = $("#STATIC_TR_S3");
				if (!tableNone3Flag) {
					selectType = 3;
					//如果第三个槽位可用,则先点击select框,让其选择card类型;	
					if (canSelectCount % 2 == 0) {
						selectFlag = true;
						var selectFocus = $("#SELECT_CardType_S3");
						var optionList = $("#SELECT_CardType_S3 option");
						var optionLength = optionList.length;
						for (var i = 0; i < optionLength; i++) {
							var span = $(optionList[i])[0];
							var spanText = $(span).text();
							$(span).text((i + 1) + "." + spanText);
							$(span).attr("name", "Select" + (i + 1));
						}
						selectFocus.attr("multiple", "multiple");
						inputFlag = 1;
					} else {
						selectFlag = false;
						//如果第三个槽位可用，则焦点到第三个输入框
						var inputFocus = $("#INPUT_AddCount_S3");
						inputFocus.click();
						document.getElementById('INPUT_AddCount_S3').style.backgroundColor = "#FFFACD";
						inputFlag = 3;
					}
					//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
					canSelectCount = canSelectCount - 1;
					var returnValue = 0;
					if (canSelectCount > 0) {
						returnValue = 0;
					} else if (canSelectCount == 0) {
						returnValue = 1;
						firstRun = true;
					} else {
						returnValue = 2;
						firstRun = true;
					}
					return returnValue;
				} else {
					inputFlag = 0;
					firstRun = true;
					return 2;
				}
			}
		case 2:
			inputTextBox = $("#STATIC_TR_S3");
			if (!tableNone3Flag) {
				selectType = 3;
				if (canSelectCount % 2 == 0) {
					selectFlag = true;
					var selectFocus = $("#SELECT_CardType_S3");
					var optionList = $("#SELECT_CardType_S3 option");
					var optionLength = optionList.length;
					for (var i = 0; i < optionLength; i++) {
						var span = $(optionList[i])[0];
						var spanText = $(span).text();
						$(span).text((i + 1) + "." + spanText);
						$(span).attr("name", "Select" + (i + 1));
					}
					selectFocus.attr("multiple", "multiple");
					inputFlag = 2;
				} else {
					selectFlag = false;
					//如果第三个槽位可用，则焦点到第三个输入框
					var inputFocus = $("#INPUT_AddCount_S3");
					inputFocus.click();
					document.getElementById('INPUT_AddCount_S3').style.backgroundColor = "#FFFACD";

					inputFlag = 0;
				}
				//如果可用槽位-目前第几个输入框>0,则表明还能继续切换,如=0,则为最后一个,如<0,则直接切回主界面
				canSelectCount = canSelectCount - 1;
				var returnValue = 0;
				if (canSelectCount > 0) {
					returnValue = 0;
				} else if (canSelectCount == 0) {
					returnValue = 1;
					firstRun = true;
				} else {
					returnValue = 2;
					firstRun = true;
				}
				return returnValue;
			} else {
				inputFlag = 0;
				firstRun = true;
				return 2;
			}
		default:
			inputFlag = 2;
			selectType = 0;
			firstRun = true;
			return 0;
	}
	return 1;
}

function removeValueDataNNumber() {

	var selectFocus = $("#SELECT_CardType_S1");
	var optionList = $("#SELECT_CardType_S1 option");
	var optionLength = optionList.length;
	for (var i = 0; i < optionLength; i++) {
		var span = $(optionList[i])[0];
		var spanText = $(span).text();
		var strTmp = spanText.substr(1, 1);
		if (strTmp == ".") {
			$(span).text(spanText.substr(2, spanText.length - 2));
		}
		$(span).attr("name", "");
	}
	selectFocus.removeAttr("multiple");

	selectFocus = $("#SELECT_CardType_S2");
	optionList = $("#SELECT_CardType_S2 option");
	optionLength = optionList.length;
	for (var i = 0; i < optionLength; i++) {
		var span = $(optionList[i])[0];
		var spanText = $(span).text();
		var strTmp = spanText.substr(1, 1);
		if (strTmp == ".") {
			$(span).text(spanText.substr(2, spanText.length - 2));
		}
		$(span).attr("name", "");
	}
	selectFocus.removeAttr("multiple");

	selectFocus = $("#SELECT_CardType_S3");
	optionList = $("#SELECT_CardType_S3 option");
	optionLength = optionList.length;
	for (var i = 0; i < optionLength; i++) {
		var span = $(optionList[i])[0];
		var spanText = $(span).text();
		var strTmp = spanText.substr(1, 1);
		if (strTmp == ".") {
			$(span).text(spanText.substr(2, spanText.length - 2));
		}
		$(span).attr("name", "");
	}
	selectFocus.removeAttr("multiple");
	document.getElementById('INPUT_AddCount_S3').style.backgroundColor = "";
	document.getElementById('INPUT_AddCount_S2').style.backgroundColor = "";
	document.getElementById('INPUT_AddCount_S1').style.backgroundColor = "";
	document.getElementById('INPUT_AddCount_S3').blur();
	document.getElementById('INPUT_AddCount_S2').blur();
	document.getElementById('INPUT_AddCount_S1').blur();
}

function HandleKeyboardAction(keyValue) {
	var SelectOption = $("#SELECT_CardType_S1");
	switch (selectType) {
		case 1:
			SelectOption = $("#SELECT_CardType_S1");
			break;
		case 2:
			SelectOption = $("#SELECT_CardType_S2");
			break;
		case 3:
			SelectOption = $("#SELECT_CardType_S3");
			break;
		default:
			break;
	}
	switch (keyValue) {
		//新增数字键事件响应,支持键盘操作,Start......
		case "1":
			if (selectFlag) {
				var optionValue = $("option[name='Select1']").val();
				SelectOption.val(optionValue);
			} else
				$("#keyboard-button-1").click();
			break;
		case "2":
			if (selectFlag) {
				var optionValue = $("option[name='Select2']").val();
				SelectOption.val(optionValue);
			} else
				$("#keyboard-button-2").click();
			break;
		case "3":
			if (selectFlag) {
				var optionValue = $("option[name='Select3']").val();
				SelectOption.val(optionValue);
			} else
				$("#keyboard-button-3").click();
			break;
		case "4":
			if (selectFlag) {
				var optionValue = $("option[name='Select4']").val();
				SelectOption.val(optionValue);
			} else
				$("#keyboard-button-4").click();
			break;
		case "5":
			if (selectFlag) {
				var optionValue = $("option[name='Select5']").val();
				SelectOption.val(optionValue);
			} else
				$("#keyboard-button-5").click();
			break;
		case "6":
			if (selectFlag) {
				var optionValue = $("option[name='Select6']").val();
				SelectOption.val(optionValue);
			} else
				$("#keyboard-button-6").click();
			break;
		case "7":
			if (selectFlag) {
				var optionValue = $("option[name='Select7']").val();
				SelectOption.val(optionValue);
			} else
				$("#keyboard-button-7").click();
			break;
		case "8":
			if (selectFlag) {
				var optionValue = $("option[name='Select8']").val();
				SelectOption.val(optionValue);
			} else
				$("#keyboard-button-8").click();
			break;
		case "9":
			if (selectFlag) {
				var optionValue = $("option[name='Select9']").val();
				SelectOption.val(optionValue);
			} else
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
