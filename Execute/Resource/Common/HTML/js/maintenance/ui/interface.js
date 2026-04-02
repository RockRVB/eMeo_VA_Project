/******************** 
	作用:界面基类
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-12
	增加说明： 界面基类，没有实例化，在子类中实例化。
********************/

(function(window) {

	function Interface() {
		this.elements = []; //保存生成的所有控件
		this.innerElements = []; //保存所有在控件里的控件
		this.focusElements = []; //可以持有焦点的控件
		this.hasFocus = false; //是否拥有焦点
		this.focusIndex = 0; //当前焦点位置
		this.isFocusInChild = false; //当前是否进入子界面设置焦点
		this.isContainer = false; //是否为其它界面的容器(true:可以作为其它界面的容器 false:子界面)
		this.interfaceName = ""; //界面名称
		this.isDelayInit = false; //是否延迟初始化(true:延迟初始化 false:不延迟初始化)
		this.currentInputText = null; //当前输入框
		this.isDefaultInput = true; //是否默认输入框(true:默认输入框 false:自定义输入框)
		this.isSendInit = true; //是否向服务器发送初始化信息(默认为发送)(true:发送 false:不发送)
		this.flashObjects = {}; //用于保存闪烁对象

		//初始化uiInstance
		window.uiInstance = this;
		this.getUiName();//获取当前ui名称
		this.initFocusElement(); //初始化焦点容器
		this.preInit(); //预处理
		if (!this.isDelayInit)
			this.init(); //初始化
	}

	Interface.prototype = {
		constructor: Interface,
		
		//获取第二级 ifram
		getContainer : function(){
			return $("#top-container");
		},

		getChildUiInstance : function() {
			if($("#top-container")[0]!=undefined)
			{
				return $("#top-container")[0].contentWindow.uiInstance;
			}
		},

		//获取当前ui名称
		getUiName: function() {
			try {
				if(Config.Ui==null)
					return;
				var parameters=Config.getUrlParameters(window.location.href);
				this.interfaceName=parameters["ui"];
				this.isContainer=Config.Ui.isContainer(this.interfaceName);
				// Config.log(this.interfaceName);
			} catch (e) {
				Config.log(e);
			}
		},

		//初始化焦点容器
		initFocusElement: function() {
			try {
				var obj;
				var length = 2;
				for (var i = 0; i < length; i++) {
					obj = {}
					obj.index = -1; //当前焦点索引
					obj.step = 1; //每次增加的步长
					obj.elements = []; //保存相应的控件
					this.focusElements.push(obj);
				}
			} catch (e) {
				Config.log(e);
			}
		},

		//预处理,在初始化函数 init 之前运行,在这里可以设置 xml 文件路径,设置界面元素的显示与隐藏等
		preInit: function() {},

		//初始化,开始加载xml界面配置文件
		init: function() {
			try {
				this.initUi(); //生成界面控件
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

		//初始化控件
		initUi: function() {
			try {
				if (this.isSendInit)
					Config.sendInit(); //向服务器发送初始化信息
				WidgetMangage.init(this); //生成界面中的所有控件

				//是否默认输入框(true:默认输入框 false:自定义输入框)
				if (this.isDefaultInput) {
					var len = this.elements.length;
					var elementName = "";
					var element;
					//将焦点设置设置为第一个输入框
					for (var i = 0; i < len; i++) {
						element = this.elements[i];
						elementName = element.name;
						//判断是否输入框
						if (element.options.type == Config.TYPE_INPUT||element.options.type =="password"||element.options.type =="text") {
							if (this.currentInputText == null && element.options.isVisible && element.options.isEnabled) {
								this.currentInputText = element;
								Config.INPUT_TARGET = element;
								
							}
							
							var self = this;
							element.element.on('click', function() {
								self.changeInputFocus(element);
							});
						}
					}
				}
				this.afterInitUi(); //生成界面后
				this.showUnTouchFocus(); //当不可触摸时显示焦点
			} catch (e) {
				Config.log(e);
			}
		},

		//生成界面后
		afterInitUi: function() {},

		//当不可触摸时显示焦点
		showUnTouchFocus: function() {},

		//将点击的文本输入框时设置为当前输入框
		changeInputFocus: function(element) {
			try {
				this.changeInputFocusAction(element);
				if (Config.showKeyTip()) {
					//更新所有焦点
					if (element.options.isVisible && element.options.isEnabled) {
						this.recaculateFocusIndex(element);
					}
				}
			} catch (e) {
				Config.log(e);
			}
		},

		//将指定的文本输入框时设置为当前输入框并设置焦点
		changeInputFocusAction: function(element) {
			try {
				//先判断该输入框是否启用
				if (element.options.isVisible && element.options.isEnabled) {
					 if (this.currentInputText != null) {
						this.currentInputText.clearFocus(); //先清空旧的焦点
					}
					this.currentInputText = element;
					if (Config.showKeyTip()) {
					 	this.currentInputText.setFocus();
						//setInputFocus(currentInputText);
					 }
				}
			} catch (e) {
				Config.log(e);
			}
		},

		//重新指定文本输入框焦点序号
		recaculateFocusIndex: function(element) {
			try {
				
			} catch (e) {
				Config.log(e);
			}
		},

		//设置值
		setValue: function(data) {
			var result = true;
			try {
				if (this.hasElement(data.id)) {
					var widget = this.getElementByName(data.id);
					if (widget == null) {
						result = false;
					} else {
						if (widget.type != "raw") {
							widget.element.setValue(data.value, Config.CMD_ATTRIBUTE_TEXT);
							widget.element.show();
						} else {
							if (widget.functionType == "html")
								widget.element.html(data.value);
							else
								widget.element.val(data.value);
						}
					}
				} else 
				{
					if(this.getChildUiInstance()!=undefined)
					{
						this.getChildUiInstance().setValue(data);
					// 往子页面找控件处理
					}
				}
			} catch (e) {
				result = false;
				Config.log(e);
			}
			return result;
		},
		//取值
		getValue: function(data) {
			var result = "";
			try {
				if (this.hasElement(data)) {
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
					if(this.getChildUiInstance()!=undefined)
					{
						result = this.getChildUiInstance().getValue(data);
					}
					// 往子页面找控件处理
				}
			} catch (e) {
				result = "";
				Config.log(e);
			}
			return result;
		},
		// 设置属性
		setAttribute: function(data) {
			var result = true;
			try {
				if (this.hasElement(data.id)) {
					var widget = this.getElementByName(data.id);
					if (widget == null) {
						result = false;
					} else 
					{
						if (widget.type != "raw") {
							Config.log("开始设置控件属性");
							Config.log(data);
							if(data.attribute.enabled!=undefined)
							widget.element.setValue(data.attribute.enabled, Config.CMD_ATTRIBUTE_ENABLED);
							if(data.attribute.visible!=undefined)
							widget.element.setValue(data.attribute.visible, Config.CMD_ATTRIBUTE_VISIBLE);
							if(data.attribute.selected!=undefined)
							widget.element.setValue(data.attribute.selected, Config.CMD_ATTRIBUTE_SELECTED);
							Config.log("结束设置控件属性");
						} 
						else // 只设置可见性
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
				    if(this.getChildUiInstance()!=undefined)
					{
						this.getChildUiInstance().setAttribute(data);
						// 往子页面找控件处理
					}
				}
			} catch (e) {
				result = false;
				Config.log(e);
			}
			return result;
		},
		getAttribute: function(data) {
			var result = "";
			try {
				if (this.hasElement(data)) 
				{
						var widget=this.getElementByName(data);
						if(widget==null){
							result = "";
						}else{
							if(widget.type!="raw")// 非原生的HTML控件，而是自定义控件类型 返回格式为 选中，是否可见，是否可用
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
					if(this.getChildUiInstance()!=undefined)
					{
						result=this.getChildUiInstance().getAttribute(data);
						// 往子页面找控件处理
					}
				}
			} catch (e) {
				result = "";
				Config.log(e);
			}
			return result;
		},
		
		//判断界面是否存在该id
		hasElement: function(id) {
			var result = true;
			try {
				result = (this.getElementByName(id) != null);
			} catch (e) {
				result = false;
				Config.log(e);
			}
			return result;
		},
		//测试
		test: function() {
			try {
				$.each(this.elements, function(i) {
					Config.log(this.options.id);
				});
			} catch (e) {
				Config.log(e);
			}
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

        // 处理密码键盘按键事件
        keyBoardAction : function(id,key) {
			//先判断该按键是否可用； 当有弹窗的时候只允许enter和cencel按键；   可用才可以继续走下去，不可用返回；
			if(!checkCanUseTheKey(key))    
				return false;
            if (this.perKeyBoardAction(key))
                return true;

	        var result=false;
		    if (this.hasElement(id)) {
					    var widget = this.getElementByName(id);
					    if (widget == null) {
						    result = false;
					    } 
                        else 
                        {
						    if (widget.type != "raw") {
						        result = true;
						        setTimeout(function() {widget.element.keyAction(key);}, "100"); 
						    } 
                            else 
                            {
						        result = false;
						    }
					    }
				    } else 
				    {
					    result = false;
				    }
				
				    return result;
	    },

        // 按键处理之前，先处理弹出框
	     perKeyBoardAction:function(key)
	    {
	        if(Config.Prompt.isVisible())
			{
				Config.disabledTabButton= true;
			    if(key=="ENTER")
				{
					Config.disabledTabButton= false;
                    setTimeout(function() {Config.Prompt.keyAction("1");}, "100");
				    return true;
				}
				else if(key=="CANCEL")
				{
					Config.disabledTabButton= false;
                    setTimeout(function() {Config.Prompt.keyAction("2");}, "100");
					return true ;
				} else {
			        return false;
			    }
				
			}

	        return false;
	    }
	}

    





	//--------------------------------
	window.Interface = Interface;
	//--------------------------------
})(window);