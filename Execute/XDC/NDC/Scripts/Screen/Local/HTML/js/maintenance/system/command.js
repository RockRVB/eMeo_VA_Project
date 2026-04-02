/******************** 
	作用:命令解析管理类，解析命令及调界面中的方法完成命令操作。
	作者:蔡俊雄
	版本:V1.0
	时间:2015-04-22
********************/

(function(window) {

	function Command() {
		this.command = null; //保存转换后的命令
	}

	Command.prototype = {
		constructor: Command,

		//解析命令中的各字段,并调用 switchAction 函数执行命令
		parseCommand: function(cmd) {
			try {
				this.command = cmd;
				if (Common.isEmpty(this.command.action)) {
					Config.log('命令格式错误');
					return;
				}
				this.command.action = this.command.action.toUpperCase();
				this.switchAction();
			} catch (e) {
				Config.log(e);
			}
		},
			
		//解析命令中的各字段,并调用 switchAction 函数执行命令
		switchAction: function() {
			try {
				var result = "";
				switch (this.command.action) {
					case Config.CMD_ACTION_PROMPT: //弹出提示框或去除提示框
						result = Config.Prompt.prompt(this.command.data);
						Config.send(result);
						break;
					case Config.CMD_ACTION_CHANGE: //切换界面
						Config.CurrentUI = this.command.data.ui;
						if( Config.CurrentUI ==Config.UI_HANDLEGUIDE)
						{
							//是否流程处理
							Config.tranData.handelGuide=true;
						}
						else
							Config.tranData.handelGuide=false;
						
						Config.Ui.changeUi(Config.CurrentUI);
						break;
					case Config.CMD_ACTION_SETVALUE: //设置文本值
						 Config.log("设置值");
						result = this.setValue(this.command.data);
						Config.send(result);
						break;
					case Config.CMD_ACTION_GETVALUE: //取文本值
						Config.log("取值");
						result = this.getValue(this.command.data);
						Config.send(result);
						// if (getValue(cmdElement, "text")) {
						// 	Config.Socket.sendData(Config.sGET); //最后一起发送GET到的所有元素值
						// 	Config.Socket.completed = true;
						// 	Config.sGET = ""; //清空Config.sGET
						// } else {
						// 	Config.Socket.sendData(Config.STATE_FAILED);
						// 	Config.Socket.completed = true;
						// }
						break;
					case Config.CMD_ACTION_SETATTRIBUTE: //设置元素属性
						Config.log("设置属性");
						result = this.setAttribute(this.command.data);
						Config.send(result);
						// //取出cmd_VALUE中属性序号中的值
						// var cmd_SELECTED: String = TwinString.SplitCmdStr(cmdValue, 1);
						// var cmd_ENABLED: String = TwinString.SplitCmdStr(cmdValue, 2);
						// var cmd_VISIBLE: String = TwinString.SplitCmdStr(cmdValue, 3);
						// //if (setValue(cmdElement, cmd_SELECTED, Config.CMD_ATTRIBUTE_SELECTED) && setValue(cmdElement, cmd_ENABLED, Config.CMD_ATTRIBUTE_ENABLED))
						// if (setValue(cmdElement, cmd_SELECTED, Config.CMD_ATTRIBUTE_SELECTED) && setValue(cmdElement, cmd_ENABLED, Config.CMD_ATTRIBUTE_ENABLED) && setValue(cmdElement, cmd_VISIBLE, Config.CMD_ATTRIBUTE_VISIBLE)) {
						// 	Config.Socket.sendData(Config.STATE_SUCCEED);
						// 	Config.Socket.completed = true;
						// } else {
						// 	Config.Socket.sendData(Config.STATE_FAILED);
						// 	Config.Socket.completed = true;
						// }
						break;
						//取元素属性值
					case Config.CMD_ACTION_GETATTRIBUTE:

						// var container: MovieClip;
						// if (Config.bMaintenance)
						// 	container = Config.ChildUiMc;
						// else
						// 	container = Config.getChildContainer();

						// var elements: Array = cmdElement.split(",");

						// var elementMc: MovieClip;

						// var isError: Boolean = false; //是否获取出错
						// var sSelected: String = "";
						// var sEnabled: String = "";
						// var sVisible: String = "";
						// var type: String = "";
						// for (var i = 0; i < elements.length - 1; i++) {
						// 	type = container.getTypeByName(elements[i]);
						// 	elementMc = container.getElementByName(elements[i]);
						// 	//由不同类型取不同属性
						// 	//文本框,获取当前点击行数
						// 	if (type == Config.TYPE_TEXTBOX) {

						// 		Config.sGET = elementMc.currentRow + Config.CMD_GETVAL_SEPARATOR;
						// 	}
						// 	//图片,获取地址
						// 	else if (type == Config.TYPE_IMAGE) {
						// 		Config.sGET = elementMc.config.src + Config.CMD_GETVAL_SEPARATOR;
						// 	} else
						// 	else if (
						// 		type == Config.TYPE_BUTTON //按钮
						// 		|| type == Config.TYPE_COMMON_BUTTON //常用按钮
						// 		|| type == Config.TYPE_OPERATE_BUTTON //操作按钮
						// 		|| type == Config.TYPE_PROMPT_BUTTON //提示按钮
						// 		|| type == Config.TYPE_QUICK_BUTTON //快捷按钮
						// 		|| type == Config.TYPE_TOGGLE_BUTTON //切换按钮
						// 		|| type == Config.TYPE_GUIDE_BUTTON //向导按钮
						// 		|| type == Config.TYPE_TEXTBOX_BUTTON //文本框按钮
						// 		|| type == Config.TYPE_TEXTBOX_BUTTON_TOP //文本框按钮(顶)
						// 		|| type == Config.TYPE_TEXTBOX_BUTTON_UP //文本框按钮(上)
						// 		|| type == Config.TYPE_TEXTBOX_BUTTON_DOWN //文本框按钮(下)
						// 		|| type == Config.TYPE_TEXTBOX_BUTTON_BOTTOM //文本框按钮(底)
						// 		|| type == Config.TYPE_PAGE_BUTTON //翻页按钮
						// 		|| type == Config.TYPE_PAGE_BUTTON_LEFT //翻页按钮(向左)
						// 		|| type == Config.TYPE_PAGE_BUTTON_RIGHT //翻页按钮(向右)
						// 		|| type == Config.TYPE_KEYBOARD_BUTTON //键盘按钮
						// 		|| type == Config.TYPE_MENU_1_BUTTON //一级菜单按钮
						// 		|| type == Config.TYPE_MENU_2_BUTTON //二级菜单按钮
						// 		|| type == Config.TYPE_MENU_3_BUTTON //三级菜单按钮
						// 		|| type == Config.TYPE_RADIO //单选按钮
						// 		|| type == Config.TYPE_RADIO_GROUP //单选按钮组
						// 		|| type == Config.TYPE_TEXT //静态文本
						// 		|| type == Config.TYPE_KEYBOARD //键盘
						// 		|| type == Config.TYPE_KEYBOARD_NUMBER //数字键盘
						// 		|| type == Config.TYPE_RADIO //单选按钮
						// 		|| type == Config.TYPE_RADIO_GROUP //单选按钮组
						// 		|| type == Config.TYPE_CHECKBOX //复选按钮
						// 		|| type == Config.TYPE_CHECKBOX_GROUP //复选按钮组
						// 		|| type == Config.TYPE_LISTBOX //下拉框
						// 		)
						// 	//container.getChildByName(elements[i]) is MaintenanceBTN || elements[i].indexOf(Config.ELEMENTYPE_LISTBOX) == 0 || elements[i].indexOf(Config.ELEMENTYPE_RADIOGROUP) == 0 || elements[i].indexOf(Config.ELEMENTYPE_CHECKBOXGROUP) == 0)
						// 	{
						// 		sSelected = "";
						// 		sEnabled = "";
						// 		sVisible = "";
						// 		//取选择状态
						// 		if (getValue(cmdElement, "selected")) {
						// 			sSelected = Config.sGET;
						// 			Config.sGET = "";
						// 		} else {
						// 			Config.Socket.sendData(Config.STATE_FAILED);
						// 			Config.Socket.completed = true;
						// 			isError = true; //是否获取出错
						// 			break;
						// 		}

						// 		//取可用状态
						// 		if (getValue(cmdElement, "enabled")) {
						// 			sEnabled = Config.sGET;
						// 			Config.sGET = "";
						// 		} else {
						// 			Config.Socket.sendData(Config.STATE_FAILED);
						// 			Config.Socket.completed = true;
						// 			isError = true; //是否获取出错
						// 			break;
						// 		}

						// 		//取可见状态
						// 		if (getValue(cmdElement, Config.CMD_ATTRIBUTE_VISIBLE)) {
						// 			sVisible = Config.sGET;
						// 			Config.sGET = "";
						// 		} else {
						// 			Config.Socket.sendData(Config.STATE_FAILED);
						// 			Config.Socket.completed = true;
						// 			isError = true; //是否获取出错
						// 			break;
						// 		}
						// 		//获取selected和enabled后再合并成1，0，1/X1C1，0，1/X1C格式发送
						// 		Config.sGET = TwinString.JoinCmdStr(sSelected, sEnabled, sVisible);
						// 		break;
						// 	}
						// }
						// //最后一起发送GET到的所有元素值
						// if (!isError) { //没出错才向服务器发送消息,防止向服务器发送两次消息的问题出现
						// 	Config.Socket.sendData(Config.sGET);
						// 	Config.Socket.completed = true;
						// }
						// //----------------edit by 蔡俊雄 2014-2-20(End)
						// //清空Config.sGET
						// Config.sGET = "";
						
						Config.log("设置属性");
						result = this.getAttribute(this.command.data);
						Config.send(result);
						break;
					default:
						break;
				}

			} catch (e) {
				Config.Socket.completed = true;
				Config.log(e);
			}
		},
		
		//设置值
		setValue: function(data) {
			var result = {
				"action": "setvalue",
				"success": true,
				"message": "",
				"data": []
			};
			try {
				var isSuccess = true;
				$.each(data, function() {
					var singleResult = {
						"id": this.id,
						"success": true
					};
					if (this.id.indexOf("TWIN_") == 0 || this.id == "STATIC_CATMODE" || this.id == "URL_IDS_DATA") {
						switch (this.id) {
							//语言标识
							case "TWIN_LANG":
								Config.Lang = this.value;
								LanguageConfig.language = Config.Lang;
								Config.Screen.startLoadLanguageResource();
								break;
								//ATM机型
							case "TWIN_ATM":
								Config.TWIN_ATM = this.value;
								break;
								//单双屏切换(TANG:触摸屏 INTANG:单屏)
							case "TWIN_MONITOR":
								Config.TWIN_MONITOR = this.value;
								break;
								//键盘值
							case "TWIN_KEYBOARD":
								//Config.TWIN_KEYBOARD = this.value;
								// if (Config.showKeyTip && Config.TWIN_KEYBOARDENBLED)
								// {
								// 	KeyManage.parseKey(Config.TWIN_KEYBOARD);
								// }
								if(this.id!=""&&this.id!=undefined)// id 为空则直接返回失败
								{
									isSuccess = Config.Screen.setValue(this);
									singleResult.success = isSuccess;
								}
								break;
								
							case "TWIN_MENU":
								Config.TWIN_MENU = this.value;
								//限制菜单
							case "TWIN_LIMITFUN":
								Config.TWIN_LIMITFUN = this.value.split(",");
								break;
							case "TWIN_CASHDEVTYPE":
								Config.TWIN_CASHDEVTYPE = this.value;
								break;
							// 当前eCAT 运行目录名
							case "TWIN_eCATDomainPath":
								Config.eCATDomainPath = this.value;
								break;
							case "TWIN_eCATCurrentWorkDirectory":
								Config.eCATCurrentWorkDirectory = this.value;
								break;
								
							
								//工作状态
							case "STATIC_CATMODE":
								/*
								   1		 加电模式
								   2		 配置模式/暂停模式
								   3		 服务模式
								   4		 离线模式
								   5		 维护模式
								   6 		 保持原状态且停止接收状态
								   7		 恢复接收新的状态
								 */
								if (this.value == "6")
									Config.CatMode = false;
								if (this.value == "7")
									Config.CatMode = true;

								if ((this.value == "2" || this.value == "3" || this.value == "4") && Config.CatMode) {
									Config.Screen.changeState(this.value);
								}
								break;
							default:
								Config[this.id] = this.value;
								break;
						}
					} 
					else 
					{
						if(this.id!=""&&this.id!=undefined)// id 为空则直接返回失败
						{
							isSuccess = Config.Screen.setValue(this);
							singleResult.success = isSuccess;
						}
					}
					result.data.push(singleResult);
				});
			} catch (e) {
				result.success = false;
				Config.log(e);
			}
			return result;
		},

        // 取值
		getValue: function(data) {
			var result = {
				"action": "getvalue",
				"success": true,
				"message": "",
				"data": []
			};
			try {
				var value = "";
				$.each(data, function() {
					var singleResult = {
						"id": this,
						"success": true,
                        "value":""
					};
					value="";
					
					if(this!=""&&this!=undefined) //id 为空则直接返回失败
					{
						value = Config.Screen.getValue(this);
					}
					
				    if (value != "" && value != undefined) 
                    {
				        singleResult.value = value;
				        singleResult.success = true;
				    } else {
				        singleResult.success = false;
				    }
				    result.data.push(singleResult);
				});
			} catch (e) 
            {
				result.success = false;
				Config.log(e);
			}
			return result;
		},
		// 设置属性
		setAttribute: function(data) {
			var result = {
				"action": "setattribute",
				"success": true,
				"message": "",
				"data": []
			};
			try {
				var isSuccess = true;
				$.each(data, function() {
					var singleResult = {
						"id": this.id,
						"success": true
					};
					
					isSuccess = Config.Screen.setAttribute(this);
					singleResult.success = isSuccess;
					result.data.push(singleResult);
				});
			} catch (e) 
			{
				result.success = false;
				Config.log(e);
			}
			return result;
		},
		
		// 取属性
		getAttribute: function(data) {
			var result = {
				"action": "getattribute",
				"success": true,
				"message": "",
				"data": []
			};
			try {
				var value = "";
				$.each(data, function() {
					var singleResult = {
						"id": this,
						"success": true,
                        "value":""
					};
					value="";
					
					if(this!=""&&this!=undefined) //id 为空则直接返回失败
					{
						value = Config.Screen.getAttribute(this);
					}
					
				    if (value != "" && value != undefined) 
                    {
				        singleResult.value = value;
				        singleResult.success = true;
				    } else {
				        singleResult.success = false;
				    }
				    result.data.push(singleResult);
				});
			} catch (e) 
            {
				result.success = false;
				Config.log(e);
			}
			return result;
		}
	}
		

		

	// this.setValue(this.command.data);
	//--------------------------------
	if (window.Command == undefined) {
		if (window.top.Command == undefined)
			window.Command = Command;
		else
			window.Command = window.top.Command;
	}
	//--------------------------------
})(window);