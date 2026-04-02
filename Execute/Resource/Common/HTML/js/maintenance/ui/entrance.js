/******************** 
	作用:程序入口
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-08
********************/

(function(window) {
	function Entrance() {
		this.twinLangCount = 0; //计算发送语言次数,防止2次加载初始化界面
		this.elements = []; //保存所有控件
	}

	Entrance.prototype = {
		constructor: Entrance,
		//初始化
		init: function() {
			try {
				//构建命令解析器和socket连接
				Config.Ui=UiManage;
				Config.Command = new Command();
				Config.Socket = Socket;
				Config.Socket.init();
				
				document.oncontextmenu = function (e) {
					return false;
				};
				
				document.onselectstart=function (e) {
					return false;
				};
				
				document.ondragstart = function (e) {
					return false;
				};
				
				//$("input").attr("disabled","disabled")
				
			} catch (e) {
				Config.log(e);
			}
		},
		//初始化相应的语言资源
		initLanguageData: function() {
			try {
				Config.PromptSureText = LanguageData.get("IDS_Ok");
				Config.PromptCancelText = LanguageData.get("IDS_Cancel");
				Config.PromptAlphaText = LanguageData.get("IDS_Operating");
				Config.ComponentConfigs.enterKey= LanguageData.get("IDS_EnterKey");
				Config.ComponentConfigs.clearKey= LanguageData.get("IDS_ClearKey");
				Config.ComponentConfigs.changeKey= LanguageData.get("IDS_AlterKey");
				Config.ComponentConfigs.exitKey= LanguageData.get("IDS_ExitKey");
				//add by xjyong
				Config.UnitConfigs.widthdrawalKey = LanguageData.get("IDS_With");
				Config.UnitConfigs.cashInKey = LanguageData.get("IDS_Dep");
				Config.UnitConfigs.recylingKey = LanguageData.get("IDS_Rcyc");
				Config.UnitConfigs.rejectKey = LanguageData.get("IDS_Rejectwith");
				Config.UnitConfigs.retractKey = LanguageData.get("IDS_RejectDep");
				Config.UnitConfigs.unknowKey = LanguageData.get("IDS_Unknow");
			} catch (e) {
				Config.log(e);
			}
		},
		//初始化提示框
		initPromptbox: function() {
			try {
				$("#prompt-box").Promptbox();
			} catch (e) {
				Config.log(e);
			}
		},
		
		//初始化调试窗口
		initdebugbox: function () { 
		
			$("#ShowDebugInfo").removeClass("none");
			
			$(".Debugbox").mousedown(function (e) { 
				iDiffX = e.pageX - $(this).offset().left; 
				iDiffY = e.pageY - $(this).offset().top; 
				
				$(document).mousemove(function (e) { 
					$(".Debugbox").css({ "left": (e.pageX - iDiffX), "top": (e.pageY - iDiffY) }); 
				}); 
			}); 
			$(".Debugbox").mouseup(function () { 
				$(document).unbind("mousemove"); 
			}); 
		},
		
		//加载语言文件（收到后端设置语言事件后开始执行这里）
		startLoadLanguageResource: function() {
			try {
				this.twinLangCount++;
				if (this.twinLangCount > 1) {
					return;
				}
				LanguageData.init();
			} catch (e) {
				Config.log(e);
			}
		},
		
		//生成界面（加载了语言资源之后才呈现界面）
		createInterface: function() {
			try {
				//控件管理器初始化
				WidgetMangage.init(this);
				this.initLanguageData();//初始化相应的语言资源
				this.initPromptbox(); //初始化提示框
				//if(Config.IsDebug)
					//this.initdebugbox();
				this.showTime(); //显示时间
				setInterval(this.showTime, 1000);

				this.startLoadInitInterface();//开始加载初始界面
			} catch (e) {
				Config.log(e);
			}
		},
		//开始加载初始界面
		startLoadInitInterface: function() {
			try {
				Config.Ui.changeUi(Config.UI_LOADING);
			} catch (e) {
				Config.log(e);
			}
		},
		//设置值
		setValue: function(data) {
			var result = false;
			try {
				if (this.hasElement(data.id)) {
					if (data.id == "STATIC_NETSTATE") //设置网络状态
					{
						var cls = data.value == true || data.value == "1" ? "net-state-online" : "net-state-offline";
						$(".net-state-image").removeClass('net-state-online net-state-offline').addClass(cls);
						$("#STATIC_NETSTATE").show();
					} else 
					{
						var widget=this.getElementByName(data.id);
						if(widget==null){
							result = false;
						}else{
							if(widget.type!="raw")// 非原生的HTML控件，而是自定义控件类型
							{
								widget.element.setValue(data.value,Config.CMD_ATTRIBUTE_TEXT);
							}
							else{
								if(widget.functionType=="html")
									widget.element.html(data.value);
								else
									widget.element.val(data.value);
								var linkId=widget.element.attr("linkId");
								if(linkId!=undefined){
									$("#"+linkId).removeClass('none');
									// $("#"+linkId).attr({
									// 	"class": ""
									// });
								}
							}
						}
					}
				} else 
				{
					if(Config.Ui.getUiInstance()!=undefined)
					result = Config.Ui.getUiInstance().setValue(data); // 分发指令 获取子页面的UiInstance（interface）
				}
			} catch (e) {
				result = false;
				Config.log(e);
			}
			return result;
		},
		
		//设置值
		getValue: function(data) {
			var result = "";
			try {
				if (this.hasElement(data)) 
				{
					var widget = this.getElementByName(data);
					if (widget == null) {
						result = "";
					} else {
						if (widget.type != "raw") {
							result = widget.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
						} else {
							if (widget.functionType == "html")
								result = widget.element.html();
							else
								result = widget.element.val();
						}
					}
				} else 
				{
					if(Config.Ui.getUiInstance()!=undefined)
					result = Config.Ui.getUiInstance().getValue(data); // 分发指令 获取子页面的UiInstance（interface）
				}
			} catch (e) {
				result = "";
				Config.log(e);
			}
			return result;
		},
		
		//设置值
		setAttribute: function(data) {
			var result = false;
			try {
				if (this.hasElement(data.id)) 
				{
						var widget=this.getElementByName(data.id);
						if(widget==null){
							result = false;
						}else{
							if(widget.type!="raw")// 非原生的HTML控件，而是自定义控件类型
							{
								if(data.attribute.enabled!=undefined)
								widget.element.setValue(data.attribute.enabled, Config.CMD_ATTRIBUTE_ENABLED);
								if(data.attribute.visible!=undefined)
								widget.element.setValue(data.attribute.visible, Config.CMD_ATTRIBUTE_VISIBLE);
								if(data.attribute.selected!=undefined)
								widget.element.setValue(data.attribute.selected, Config.CMD_ATTRIBUTE_SELECTED);
							}
							else // 非自定义控件则只设置显示和隐藏
							{
									if(data.attribute.visible!=undefined)
									{
										if(data.attribute.visible == "1" || data.attribute.visible == true)
										{
											widget.element.show();
										}
										else
											widget.element.hide();
									}	
							}
						}
					
				} else 
				{
					if(Config.Ui.getUiInstance()!=undefined)
					result = Config.Ui.getUiInstance().setAttribute(data); // 分发指令 获取子页面的UiInstance（interface）
				}
			} catch (e) {
				result = false;
				Config.log(e);
			}
			return result;
		},
		// 取属性
		getAttribute: function(data) {
			var result = "";
			try {
				if (this.hasElement(data)) 
				{
						var widget=this.getElementByName(data);
						if(widget==null){
							result = "";
						}else{
							if(widget.type!="raw")// 非原生的HTML控件，而是自定义控件类型
							{
								result=widget.element.getValue(Config.CMD_ATTRIBUTE_SELECTED)+","+
								widget.element.getValue(Config.CMD_ATTRIBUTE_VISIBLE)+","+
								widget.element.getValue( Config.CMD_ATTRIBUTE_ENABLED);
							}
							else // 非自定义控件则只取隐藏属性
							{
								result=	(widget.element.is(":visible")||widget.element.is(":hidden"))?"0,1,1":"0,0,1";
							}
						}
					
				} else 
				{
					if(Config.Ui.getUiInstance()!=undefined)
					result = Config.Ui.getUiInstance().getAttribute(data); // 分发指令 获取子页面的UiInstance（interface）
				}
			} catch (e) {
				result = "";
				Config.log(e);
			}
			return result;
		},
		//通过id获取相应的控件,如果返回为null则表示该控件不存在
		getElementByName: function(id) {
			var result = null;
			try {
				$.each(this.elements, function(i) {
					if (this.options.id == id) {
						result = {
							element: this,
							type: this.options.type
						};
						return false;
					}
				});
				if (result == null) {
					if ($("#" + id).length > 0) {
						result = {
							element: $("#" + id),
							functionType: "html",
							type: "raw"
						};
					}
				}
			} catch (e) {
				Config.log(e);
			}
			return result;
		},
		//判断界面是否存在该id
		hasElement: function(id) {
			var result = true;
			try {
				result=(this.getElementByName(id)!=null);
			} catch (e) {
				result = false;
				Config.log(e);
			}
			return result;
		},
		//切换状态
		changeState: function(uiName) {
			try {
				var state="";
				
				switch (uiName)
				{
					case Config.UI_LOADING: 
					case Config.UI_INIT: 
						state="init";
						this.hideUser();
						break;
					case Config.UI_NORMAL: 
					case Config.UI_INSERVICE: 
						state="normal";
						this.hideUser();
						break;
					case Config.UI_LOGIN_ID: 
					case Config.UI_LOGIN_ROLE: 
						state="login";
						this.hideUser();
						break;
					case Config.UI_MAINTENANCE: 
					case Config.UI_SHORTCUT: 
					case Config.UI_HANDLEGUIDE: 
					case Config.UI_MALFUNCTIONGUIDE: 
						state="maintenance";
						break;
					case Config.UI_OUTOFSERVICE: 
						state="outofservice";
						this.hideUser();
						break;
					case Config.UI_OFFLINE: 
						this.hideUser();
						state="offline";
						break;
				}
				if(state!=""){
					state="state-"+state;
					$(".state-image").attr({"class":"state-image "+state});
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//
		hideUser: function() {
			try {
				var data={"id":"STATIC_USERID","value":""};
				this.setValue(data);
				$("#VSTATIC_USERID").addClass("none");
			} catch (e) {
				Config.log(e);
			}
		},
		//显示时间
		showTime: function() {
			try {
				var date = new Date();

				var year = date.getFullYear() + "";
				var month = date.getMonth() + 1 + "";
				var day = date.getDate() + "";
				var hour = date.getHours() + "";
				var minute = date.getMinutes() + "";
				var second = date.getSeconds() + "";

				year = year.padLeft("0", 2);
				month = month.padLeft("0", 2);
				day = day.padLeft("0", 2);
				hour = hour.padLeft("0", 2);
				minute = minute.padLeft("0", 2);
				second = second.padLeft("0", 2);

				var timeStr = year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;
				$("#STATIC_CURRENT_TIME").html(timeStr);
			} catch (e) {
				Config.log(e);
			}
		}
	};
	//--------------------------------
	if (window.Entrance == undefined) {
		if (window.top.Entrance == undefined)
			window.Entrance = Entrance;
		else
			window.Entrance = window.top.Entrance;
	}
	//--------------------------------
})(window);

//程序入口相当于Main 函数入口，初始化主界面
$(function() { //ID匹配元素
	Config.Screen = new Entrance();
	Config.Screen.init(); //初始化
});