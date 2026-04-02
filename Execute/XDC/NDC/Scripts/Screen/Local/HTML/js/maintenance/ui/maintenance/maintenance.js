/******************** 
	作用:标准维护界面
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-22
********************/

(function(window) {

	function CustomUi() {
		/** 菜单按钮个数(分别对应一二三级菜单) */
		this.menuCounts = [6, 6, 6];

		/** 保存所有菜单按钮 */
		this.menuButtons = [];

		/** 保存所有分页按钮 */
		this.menuPageButtons = [];
        /** 保存所有流程按钮 */
	    this.guideMenus = [];
		//------------------MVC设置
		/** 菜单xml文件 */
		this.xml = null;
		/** 菜单数据来源 */
		this.model = null;
		/** 菜单视图设置 */
		this.view = null;
		/** 是否发送源(用于区别是主界面发送还是流程界面发出的切换命令) */
		this.isSendingSource = true;
		/** 是否禁用菜单,默认为false,启用菜单(true:禁用菜单 false:启用菜单) */
		this.isDisableMenu = false;
        /** 流程步骤数 */
	    this.progressItemCount = 0;
        /** 当前步骤序号*/
	    this.curProgressGuideIndex = 0;
        /** 当前步骤MenuID*/
	    this.curProgressMenu = "";
	    this.isGoAhead = true;
        this.InvalSeparator ='\x1B';
	    
		CustomUi.superClass.constructor.call(this);
	}

	Config.extend(CustomUi, Interface);

	//预处理
	CustomUi.prototype.preInit = function() {
		try {
			this.isDelayInit = true;
			if (Config.Ui != null && Config.Ui.currentModeUI == Config.UI_HANDLEGUIDE) {
				//流程处理界面
				this.isSendInit = false; //不向服务器发送初始化信息
			}
			if (Config.sQueryErrorCodePreUI == Config.UI_MALFUNCTIONGUIDE)
			{
				this.isSendInit = false;////不向服务器发送初始化信息
				Config.tranData.handelGuide=true; // 当成是特殊的流程处理，菜单加载完成后不自动发送菜单ID 到后台。
			}

			var self = this;
			
			if (Config.TWIN_MENU == "") //读取菜单
			{
				//加载错误码xml
				$.ajax({
					url: this.getMenuUrl(),
					dataType: 'xml',
					type: 'GET',
					timeout: 2000,
					error: function(xml) {
						Config.log("加载菜单XML文件出错!");
					},
					success: function(xml) {
						self.xml = xml;
						self.init(); //初始化
					}
				});
			}
			else // 
			{
				self.xml = String2XML(Config.TWIN_MENU);
				self.init(); //初始化
			}
			
			MenuFlag=10;
			//this.SetDefaultValue();
			
		} catch (e) {
			Config.log(e);
		}
	};

	
	
	//清除界面信息,用于从头开始显示信息
	CustomUi.prototype.cleanData = function() {
		Config.bQueryErrorCode=false; // 复位标志位（是否点击了错误码查询按钮）
	};
	
	//生成界面后
	CustomUi.prototype.afterInitUi = function() {
		try 
        {
			//采用MVC模式显示数据
			this.model = new MaintenanceMenu(this.xml, this);
			this.view = new MaintenanceView(this.xml, this.model, this);

            if (Config.tranData.handelGuide) { //如果是流程处理，则遮罩Menu 菜单
                $("#navigation-mask").removeClass("none");
            }
			
			Config.isShortcut = false;
			
            //注册控件事件并且修正单击事件发送的数据
            this.regEventHandel("FUN_-DWONSTEP", this.sendGuideMenuID,"FUN_BACK");
            this.regEventHandel("FUN_-UPSTEP", this.sendGuideMenuID,"FUN_BACK");
            this.regEventHandel("FUN_-BACK", this.sendGuideMenuID,"FUN_BACK");
			
            //判断是否从错误处理页跳转而来
			if (Config.sQueryErrorCodePreUI == Config.UI_MALFUNCTIONGUIDE)
			{
				if(Config.bQueryErrorCode)
				{
					 $("#navigation-mask").addClass("none"); //如果是特殊流程处理，则不遮罩Menu 菜单
					 this.view.showCascadeMenuById(Config.bQueryErrorCodeCurMenuID);// 定位显示错误查询页
					 this.view.sendMenuId(Config.bQueryErrorCodeCurMenuID);// 发送错误查询页按钮事件				 
					 Config.tranData.handelGuide=false;//流程处理标志位复位；
					 Config.sQueryErrorCodePreUI="";
					 $("#handleguide-container").addClass("none");
				}
				else
				{
					 $("#navigation-mask").addClass("none"); //如果是特殊流程处理，则不遮罩Menu 菜单
					 this.view.showCascadeMenuById("1");// 定位显示错误查询页
					 this.view.sendMenuId("1");// 发送错误查询页按钮事件				 
					 Config.tranData.handelGuide=false;//流程处理标志位复位；
					 Config.sQueryErrorCodePreUI="";
				  	 $("#handleguide-container").addClass("none");
				}
			}
			// 切换状态
			Config.Screen.changeState(Config.UI_MAINTENANCE);			
        } catch (e) {
			Config.log(e);
		}

	};

	//根据菜单层级设置界面中各元件的位置
	CustomUi.prototype.setPositionByMenuLevel = function(level, hasMenu) {

	};

	//判断是否禁用菜单和按钮
	CustomUi.prototype.adjustDisableMenu = function() {
		try {
			//禁用操作按钮
				if (this.isDisableMenu) {
					this.getElementByName("FUN_LOGOUT").element.disable();
					this.getElementByName("FUN_QUIT").element.disable();
					this.getElementByName("FUN_TABMODE").element.disable();
				}
		} catch (e) {
			Config.log(e);
		}
	};

	//当菜单ID不在维护界面中时重新设置导航显示
	CustomUi.prototype.showNavigationNotExit = function(id) {
		try {
			
		} catch (e) {
			Config.log(e);
		}
	};

    //重写方法设置值
	CustomUi.prototype.setValue = function(data) {
		var result = true;
		try 
        {
            if(data.id=="FUN_ITEM")
			{
				this.MySetValue(data);
				Config.tranData.handelGuide=false;//流程处理标志位复位；
			}
			else if (data.id=="TWIN_KEYBOARD")
			{
				    result = this.KeyAction(data.value);
			} 
			else
			{
				result =CustomUi.superClass.setValue.call(this,data);
			} 
			
		} catch (e) {
			result = false;
			Config.log(e);
		}

		return result;
	};


    // 私有方法 设值
	CustomUi.prototype.MySetValue= function(data)
	{
        
	    var result=false;		
	    var items = data.value.split(',');
	    this.guideMenus = items;

        var m_this = this;
        
	    //清理数据
        //当前遍历步骤序号
	    this.progressItemCount = 0;
        /** 当前步骤序号*/
	    this.curProgressGuideIndex = 0;
        /** 当前步骤MenuID*/
	    this.curProgressMenu = "";
	    this.isGoAhead = true;


	    this.curProgressMenu = items[0];
        Config.curGuideMenuID= items[0];


	    if (items.length < 0) {
	        return false;
	    }


	    if(items.length>1)
		{
			$("#handleguide-container").removeClass("none");
            $(".flow-button-zone").removeClass("none");
		    $(".flow-button-zone").find("button").removeClass("none");
			this.initProgressContrl(items.length);
			$.each(items, function() 
			{
				var index = parseInt(this.toString());
				//var xml =String2XML(Config.TWIN_MENU) 转换成XML 对象 (从菜单源中读取菜单对应的标题)
				//$(xml).find("config").children().eq(0).attr("id")
			    var title = m_this.getMenuTitle(index);
				if ( title!= undefined) 
				{
					$(".progress-step-description").eq(m_this.progressItemCount).parent().removeClass("none");
					$(".progress-step-description").eq(m_this.progressItemCount).text(title);
					// 当前遍历步骤序号+1
					m_this.progressItemCount++;
				}
			});

            setTimeout(this.sendGuideMenuID , 20);
		    return true;
		} 
        else  // 如果是单个流程步骤则当成是普通的菜单操作，取消遮罩。
	    {
	        this.view.showCascadeMenuById(this.curProgressMenu);
	        //$(".progress-bg").addClass("none");
            //$(".progress-container").addClass("none");
	        $("#handleguide-container").addClass("none");
            $(".flow-button-zone").addClass("none");
	        $("#navigation-mask").addClass("none");
			var curProgressMenuTmp="1";
			if(this.curProgressMenu == "")
			{
				curProgressMenuTmp = "1";
			}
			else
			{				
				curProgressMenuTmp = this.curProgressMenu;
			}
	        Config.send({
	            "action": "menu",
	            "data": {
	                "id": "MENU_" + curProgressMenuTmp
	            }
	        });

	        return true;
	    }
        
	};
	
	 CustomUi.prototype.getMenuTitle= function(index)
	{
	    var result;
		var xml =String2XML(Config.TWIN_MENU); //转换成XML 对象 (从菜单源中读取菜单对应的标题)
		$(xml).find("config").children().each(
		 function()
		 {
			if($(this).attr("id")==index)
			{
			   result=$(this).attr("name");
			   return false;
			}
		 }
		);
		
		return result;
	};
	
	
    //初始化进度条
    CustomUi.prototype.initProgressContrl= function(len)
	{
	    if (len != undefined) {
	        $(".progress-bg").removeClass("none");
	        $(".progress-fg ").css("width", "0");
            $(".progress-container").find("li").removeClass("step-current");
            $(".progress-container").find("li").addClass("none");
            $(".progress-container").find("li").css("width", 100 / len+"%");
	    }
	};

    //动态设置进度条状态
    CustomUi.prototype.SetProgressContrl= function() {

        var len = this.guideMenus.length;

	    if (len != undefined&&len>1) {
			if(uiInstance.curProgressGuideIndex<len)
			{
				$(".progress-fg").css("width", (100* (2*uiInstance.curProgressGuideIndex-1) / (2*len))+"%");
			}
			else
			{
				$(".progress-fg").css("width", (100* uiInstance.curProgressGuideIndex / len)+"%");
			}
	        //$(".progress-container").find("li").removeClass("step-current"); // 禁用取消已操作过的小圆圈的红色背景
	        $(".progress-container").find("li").eq(uiInstance.curProgressGuideIndex - 1).addClass("step-current");
	    }
		else
		{
			$("#handleguide-container").addClass("none");
		}
	};


    // 发送流程ID到后台
     CustomUi.prototype.sendGuideMenuID= function() {
         
         
         if (this.id == "FUN_-DWONSTEP")
             uiInstance.isGoAhead = true;
         else if(this.id == "FUN_-UPSTEP"){
             uiInstance.isGoAhead = false;
         }
         else
            uiInstance.isGoAhead = true;

        try 
        {
            var id;
             if (uiInstance.isGoAhead) 
             {
                 if (uiInstance.curProgressMenu != "" && uiInstance.curProgressGuideIndex <= uiInstance.guideMenus.length) {
                 
                     if (uiInstance.curProgressGuideIndex == 0) {
                         id = "MENU_" + Config.curGuideMenuID;
                     } else 
                     {
                         if (this.id == "FUN_-BACK") 
                         {
                             id = "MENU_88889"; // 当点击的是返回按钮时，固定统一发送返回ID
                             uiInstance.curProgressGuideIndex--;
                         } 
                         else 
                         {
                             id = "MENU_" + uiInstance.guideMenus[uiInstance.curProgressGuideIndex];
                            uiInstance.curProgressMenu = id;
                            
                         }
                         
                     }

                     uiInstance.curProgressGuideIndex++;
				     
                     var data = {
                        "action": "menu",
                        "data": {
                            "id": id
                        }
                    };
					
                    Config.send(data);

                    // 设置流程控件状态
                    uiInstance.SetProgressContrl();
                 }

                 //禁用下一步按钮
                 if (uiInstance.curProgressGuideIndex == uiInstance.guideMenus.length) {
                    uiInstance.setContrlEable("FUN_-DWONSTEP", false);
                 }
				 else
					uiInstance.setContrlEable("FUN_-DWONSTEP", true);
				 if (uiInstance.curProgressGuideIndex >1)
				 {
					//启用上一步按钮
					uiInstance.setContrlEable("FUN_-UPSTEP", true);
				 }
				 else
				 {
				 //启用上一步按钮
					uiInstance.setContrlEable("FUN_-UPSTEP", false);
				 }
				 

				
             } else 
             {

                 if (uiInstance.curProgressMenu != "" && uiInstance.curProgressGuideIndex >0 ) 
                 {
                     uiInstance.curProgressGuideIndex--;
                     if (uiInstance.curProgressGuideIndex == 1) {

                         //禁用上一步按钮
                         uiInstance.setContrlEable("FUN_-UPSTEP", false);

                     }
                     //启用下一步按钮
                     uiInstance.setContrlEable("FUN_-DWONSTEP", true);
					 if(uiInstance.curProgressGuideIndex==0)
					 {
						id = "MENU_" + uiInstance.guideMenus[uiInstance.curProgressGuideIndex];
					 }
					 else
					 {
						id = "MENU_" + uiInstance.guideMenus[uiInstance.curProgressGuideIndex-1];
					 }
                     uiInstance.curProgressMenu = id;

                      var data = {
                            "action": "menu",
                            "data": {
                                "id": id
                            }
                        };
                        Config.send(data);

                        // 设置流程控件状态
                        uiInstance.SetProgressContrl();
                 }
             }
			
         } catch (e) 
         {
            Config.log(e);
         }
                 
                 


     };
    
	//获取菜单路径
    CustomUi.prototype.getMenuUrl= function()
    {
        if (Config.TWIN_MENU != "") {
            return getRootPath_web() + "/HTML/config/maintenance/TwinScreenMenu.xml";
        }

    };

    // 为控件注册相应事件
    CustomUi.prototype.regEventHandel = function(id, fun,datatosend) {

        result = false;
        if (this.hasElement(id)) 
        {
            var widget = this.getElementByName(id);
            if (widget == null) {
                result = false;
            } else {
                if (widget.type != "raw") 
                {
                    widget.element.options.afterclick = fun;
                    widget.element.options.dataToSend = datatosend;
                    result = true;
                }

            }
        }
        return result;
    };


    //设置控件可见性
    CustomUi.prototype.setContrlEable = function(id,vb ) {

        if (vb == undefined)
            vb = true;

        if (uiInstance.hasElement(id)) 
        {
            var widget = uiInstance.getElementByName(id);
            if (widget == null) {
                return;
            } else {
                if (widget.type != "raw") 
                {
                    widget.element.setValue(vb, Config.CMD_ATTRIBUTE_ENABLED);
                }

            }
        }
    };


    //按键响应
    CustomUi.prototype.KeyAction= function(key)
	{

		if(!checkCanUseTheKey(key))//检测按键是否可用 不可用返回
			return
			
	    var result=false;
		//添加对按键是否被禁用的判断
		var nameOrValue = "Select" + key;
		if($("button[value = "+ nameOrValue + "]").prop("disabled") || $("button[name = "+ nameOrValue +"]").prop("disabled")){
			//判断当前的密码键盘对应的按钮是否有禁用属性，如果有禁用属性则返回，不做处理
			return;
		}
	    switch (key) {				
	    // case "ENTER":
		// return alert(111);
        // case "CANCEL":
		// return alert(222);
		case "1":			
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
			}
			$("button[value='Select1']").click();
			$("button[name='Select1']").click();
			$("#Select1").click();
			if(ChildrenFlag!=1)
			{
				this.switchDataAgain();
			}
			return true;
			//this.keyBoardAction($("button[value='Select1']").attr('id'),key);
		case "2":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
			}
			$("button[value='Select2']").click();
			$("button[name='Select2']").click();
			$("#Select2").click();
			if(ChildrenFlag!=1)
			{
				this.switchDataAgain();
			}
			return true; 
		case "3":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
			}
			$("button[value='Select3']").click();
			$("button[name='Select3']").click();
			$("#Select3").click();
			if(ChildrenFlag!=1)
			{
				this.switchDataAgain();
			}
			return true; 
		case "4":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
			}
			$("button[value='Select4']").click();
			$("button[name='Select4']").click();
			$("#Select4").click();
			if(ChildrenFlag!=1)
			{
				this.switchDataAgain();
			}
			return true; 
		case "5":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
			}
			$("button[value='Select5']").click();
			$("button[name='Select5']").click();
			$("#Select5").click();
			if(ChildrenFlag!=1)
			{
				this.switchDataAgain();
			}
			return true; 
		case "6":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
			}
			$("button[value='Select6']").click();
			$("button[name='Select6']").click();
			$("#Select6").click();
			if(ChildrenFlag!=1)
			{
				this.switchDataAgain();
			}
			return true;  
		case "7":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
			}
			$("button[value='Select7']").click();
			$("button[name='Select7']").click();
			$("#Select7").click();			
			if(ChildrenFlag!=1)
			{
				this.switchDataAgain();
			}
			return true;  
		case "8":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
			}
			$("button[value='Select8']").click();
			$("button[name='Select8']").click();
			$("#Select8").click();
			if(ChildrenFlag!=1)
			{
				this.switchDataAgain();
			}
			return true;  
		case "9":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
			}
			$("button[value='Select9']").click();
			$("button[name='Select9']").click();
			$("#Select9").click();
			return true;  
		case "0":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction("10");
			}			
			return true;  
		case ".":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(".");
			}			
			return true;  
		case "CLEAR":
			if(ChildrenFlag==1)
			{
				document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
			}			
			return true;  
		case "ENTER":
			if(ChildrenFlag==1)
			{
				try
				{
					document.getElementById("top-container").contentWindow.HandleKeyboardAction(key);
				}
				catch(e) 
				{
					return this.keyBoardAction("KEYBOARD_ENTER",key);
				}	
			}	
			return this.keyBoardAction("KEYBOARD_ENTER",key);
        case "CANCEL":return this.keyBoardAction("FUN_QUIT",key);
	    case "KEYBOARD_TAB":return this.SwitchMenu();
        // 后续插入快捷键等等
        //.........
	    default:
	        return this.keyBoardAction("keyboard",key);
	    }
	};
	
	//重置按钮
    CustomUi.prototype.switchDataAgain = function() {

        switch(MenuFlag)
			{
				case 0:
					this.RemoveValueDataNNumber();
					this.SetDefaultValue();
					break;
				case 1:break;
				case 2:break;
				case 3:
					this.RemoveValueDataNNumber();
					this.SetLevelOneValue();
					break;
				case 4:
					this.RemoveValueDataNNumber();
					this.SetLevelTwoValue();
					break;
				case 5:
					this.RemoveValueDataNNumber();
					this.SetLevelThreeValue();
					break;
				// case 6:
					// var divTmp=$(".level4-menu-button-zone");
					// if(divTmp.is('.none'))
					// {
						// this.RemoveValueDataNNumber();
						// this.SetLevelFourValue();
					// }
					// break;
				default: break;				
			}
    };
	
	//清除value置和首字母数字
	CustomUi.prototype.RemoveValueDataNNumber= function()
	{		
	    var result=false;		
		
		//公用底部操作按钮
		var strTmp="";
		var buttonText=""
		var bottomButton = $("#FUN_LOGOUT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
    	}			
		bottomButton.attr("name","");
		
		bottomButton = $("#FUN_QUIT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		num=strTmp.substr(0,1);
		if(reg.test(num))
		{			
			buttonText.text(strTmp.substr(1,strTmp.length-1));
			
    	}	
		bottomButton.attr("name","");
		
		bottomButton = $("#FUN_TABMODE");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
    	}	
		bottomButton.attr("name","");
		
		
		bottomButton = $("#FUN_-UPSTEP");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
    	}	
		bottomButton.attr("name","");
		
		bottomButton = $("#FUN_-DWONSTEP");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
    	}	
		bottomButton.attr("name","");
		
		bottomButton = $("#FUN_-BACK");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
    	}	
		bottomButton.attr("name","");
		
		//一级菜单
		var liList=$(".level-1 div ul li");
		var liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]).children("a").children("div").children("div").children("span"))[0];
			var spanText = $(span).text();
			
			var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num=spanText.substr(0,1);
			if(reg.test(num))
			{
      		 	$(span).text(spanText.substr(1,spanText.length-1));
    		}			
			$(span).attr("id","");
		}	
		var buttonPre=$(".level-1 #MENU1_PRE_PAGE");
		var buttonNext=$(".level-1 #MENU1_NEXT_PAGE");	
		if(buttonPre.is('.none'))
		{		
		}
		else
		{
			var buttonSpan=	($(buttonPre).children("span"))[1];
			$(buttonSpan).text("");
			buttonPre.val("");
		
			buttonSpan= ($(buttonNext).children("span"))[1];
			$(buttonSpan).text("");
			buttonNext.val("");
		}
		//二级菜单
		 liList=$(".level-2 div ul li");
		 liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]).children("a").children("div").children("div").children("span"))[0];
			var spanText = $(span).text();
			
			var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num=spanText.substr(0,1);
			if(reg.test(num))
			{
      		 	$(span).text(spanText.substr(1,spanText.length-1));
    		}			
			$(span).attr("id","");
		}	
		
		buttonPre=$(".level-2 #MENU2_PRE_PAGE");
		buttonNext=$(".level-2 #MENU2_NEXT_PAGE");	
		if(buttonPre.is('.none'))
		{		
			if(buttonNext.is('.none'))
			{					
			}
			else
			{
				var buttonSpan=	($(buttonPre).children("span"))[1];
				$(buttonSpan).text("");
				buttonPre.val("");
		
				buttonSpan= ($(buttonNext).children("span"))[1];
				$(buttonSpan).text("");
				buttonNext.val("");
			}	
		}
		else
		{
			var buttonSpan=	($(buttonPre).children("span"))[1];
			$(buttonSpan).text("");
			buttonPre.val("");
		
			buttonSpan= ($(buttonNext).children("span"))[1];
			$(buttonSpan).text("");
			buttonNext.val("");
		}
		//三级菜单
		var liList=$(".level-3 div ul li");
		var liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]).children("a").children("div").children("div").children("span"))[0];
			var spanText = $(span).text();
			
			//var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			//var num=spanText.substr(0,1);
			var num=spanText.substr(1,1);
			//if(reg.test(num))
			if(num == ".")
			{
      		 	$(span).text(spanText.substr(2,spanText.length-2));
    		}			
			$(span).attr("id","");
		}	
		
		buttonPre=$(".level-3 #MENU3_PRE_PAGE");
		buttonNext=$(".level-3 #MENU3_NEXT_PAGE");	
		if(buttonPre.is('.none'))
		{		
		}
		else
		{
			var buttonSpan=	($(buttonPre).children("span"))[1];
			$(buttonSpan).text("");
			buttonPre.val("");
		
			buttonSpan= ($(buttonNext).children("span"))[1];
			$(buttonSpan).text("");
			buttonNext.val("");
		}	

		//四级菜单
		var liList=$(".level4-menu-button-zone button");		
		var liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]))[0];
			var spanText = $(span).text();
			
			var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num=spanText.substr(0,1);
			if(reg.test(num))
			{
      		 	$(span).text(spanText.substr(1,spanText.length-1));
    		}	
			$(span).attr("name",""); 
		}	
		
		try
		{
			document.getElementById("top-container").contentWindow.removeValueDataNNumber();
		}
		catch(e) 
		{
			Config.log(e);
			return true;
		}		
		
		return true;
	};
	
	//一进入界面就给底部4个按钮赋值
    CustomUi.prototype.SetDefaultValue= function()
	{		
	    var result=false;
		var strTmp="";
		var buttonText=""
		var bottomButton = $("#FUN_LOGOUT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("1"+strTmp);
		bottomButton.attr("name","Select1"); 
		
		bottomButton = $("#FUN_QUIT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("2"+strTmp);
		bottomButton.attr("name","Select2"); 
		
		bottomButton = $("#FUN_TABMODE");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("3"+strTmp);
		bottomButton.attr("name","Select3"); 		
		
		return true;
	};
	
	//设置一级菜单数据
    CustomUi.prototype.SetLevelOneValue= function()
	{				
		var result=false;		
		//设置ul里边的置
		var liList=$(".level-1 div ul li");
		var liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]).children("a").children("div").children("div").children("span"))[0];
			var spanText = $(span).text();
			$(span).text((i+1)+spanText);
			$(span).attr("id","Select"+(i+1));
		}		
		
		
		//设置翻页按钮的值
		var buttonPre=$(".level-1 #MENU1_PRE_PAGE");
		var buttonNext=$(".level-1 #MENU1_NEXT_PAGE");	
		if(buttonPre.is('.none'))
		{		
			if(buttonNext.is('.none'))
			{}
			else
			{
				var buttonSpan=	($(buttonPre).children("span"))[1];
				var buttonSpanText =$(buttonSpan).text();
				$(buttonSpan).text(7+buttonSpanText);
				buttonPre.val("Select7");
		
				buttonSpan= ($(buttonNext).children("span"))[1];
				buttonSpanText =$(buttonSpan).text();
				$(buttonSpan).text(8+buttonSpanText);
				buttonNext.val("Select8");
			}
		}
		else
		{
			var buttonSpan=	($(buttonPre).children("span"))[1];
			var buttonSpanText =$(buttonSpan).text();
			$(buttonSpan).text(7+buttonSpanText);
			buttonPre.val("Select7");
		
			buttonSpan= ($(buttonNext).children("span"))[1];
			buttonSpanText =$(buttonSpan).text();
			$(buttonSpan).text(8+buttonSpanText);
			buttonNext.val("Select8");
		}	
		
		return true;
	};
	
	//设置二级菜单数据
    CustomUi.prototype.SetLevelTwoValue= function()
	{				
		var result=false;		
		//设置ul里边的置
		var liList=$(".level-2 div ul li");
		var liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]).children("a").children("div").children("div").children("span"))[0];
			var spanText = $(span).text();
			$(span).text((i+1)+spanText);
			$(span).attr("id","Select"+(i+1));
		}		
		
		
		//设置翻页按钮的值
		var buttonPre=$(".level-2 #MENU2_PRE_PAGE");
		var buttonNext=$(".level-2 #MENU2_NEXT_PAGE");	
		if(buttonPre.is('.none'))
		{		
			if(buttonNext.is('.none'))
			{}
			else
			{
				var buttonSpan=	($(buttonPre).children("span"))[1];
				var buttonSpanText =$(buttonSpan).text();
				$(buttonSpan).text(7+buttonSpanText);
				buttonPre.val("Select7");
		
				buttonSpan= ($(buttonNext).children("span"))[1];
				buttonSpanText =$(buttonSpan).text();
				$(buttonSpan).text(8+buttonSpanText);
				buttonNext.val("Select8");
			}
		}
		else
		{
			var buttonSpan=	($(buttonPre).children("span"))[1];
			var buttonSpanText =$(buttonSpan).text();
			$(buttonSpan).text(7+buttonSpanText);
			buttonPre.val("Select7");
		
			buttonSpan= ($(buttonNext).children("span"))[1];
			buttonSpanText =$(buttonSpan).text();
			$(buttonSpan).text(8+buttonSpanText);
			buttonNext.val("Select8");
		}	
		
		return true;
	};
	
	//设置三级菜单数据
    CustomUi.prototype.SetLevelThreeValue= function()
	{				
		var result=false;		
		//设置ul里边的置
		var liList=$(".level-3 div ul li");
		var liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]).children("a").children("div").children("div").children("span"))[0];
			var spanText = $(span).text();
			$(span).text((i+1)+"."+spanText);
			$(span).attr("id","Select"+(i+1));
		}		
		
		
		//设置翻页按钮的值
		var buttonPre=$(".level-3 #MENU3_PRE_PAGE");
		var buttonNext=$(".level-3 #MENU3_NEXT_PAGE");	
		if(buttonPre.is('.none'))
		{	
			if(buttonNext.is('.none'))
			{}
			else
			{
				var buttonSpan=	($(buttonPre).children("span"))[1];
				var buttonSpanText =$(buttonSpan).text();
				$(buttonSpan).text(7+buttonSpanText);
				buttonPre.val("Select7");
		
				buttonSpan= ($(buttonNext).children("span"))[1];
				buttonSpanText =$(buttonSpan).text();
				$(buttonSpan).text(8+buttonSpanText);
				buttonNext.val("Select8");
			}	
		}
		else
		{
			var buttonSpan=	($(buttonPre).children("span"))[1];
			var buttonSpanText =$(buttonSpan).text();
			$(buttonSpan).text(7+buttonSpanText);
			buttonPre.val("Select7");
		
			buttonSpan= ($(buttonNext).children("span"))[1];
			buttonSpanText =$(buttonSpan).text();
			$(buttonSpan).text(8+buttonSpanText);
			buttonNext.val("Select8");
		}	
		
		return true;
	};
	
	//设置四级菜单数据
    CustomUi.prototype.SetLevelFourValue= function()
	{				
		var result=false;		
		//设置ul里边的置
		var liList=$(".level4-menu-button-zone button");		
		var liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]))[0];
			var spanText = $(span).text();
			$(span).text((i+1)+spanText);
			$(span).attr("name","Select"+(i+1)); 
		}	
		return true;
	};
	
	//上一步 下一步 返回
    CustomUi.prototype.SetBottomButtonValue= function()
	{		
	    var result=false;
		var strTmp="";
		var buttonText=""
		var bottomButton = $("#FUN_-UPSTEP");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("1"+strTmp);
		bottomButton.attr("name","Select1");
		
		bottomButton = $("#FUN_-DWONSTEP");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("2"+strTmp);
		bottomButton.attr("name","Select2");
		
		bottomButton = $("#FUN_-BACK");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("3"+strTmp);
		bottomButton.attr("name","Select3");	
		
		return true;
	};
	
	
	//子窗口函数调用
    CustomUi.prototype.SetChildrenValue= function()
	{		
	    var result=false;
		try
		{
			//返回的数据，根据返回的数据设置MenuFlag的值
			var returnFlag=document.getElementById("top-container").contentWindow.InvokeChildrenSwitch();
			switch(returnFlag)
			{
				case 0:				
					return 6;
				case 1:
					return 7;			
				case 2:
					return 8;
					
			}
		}
		catch(e) 
		{
			Config.log(e);
			return 7;
		}	
	
		return true;
	};
	
	//切换功能
	CustomUi.prototype.SwitchMenu= function()
	{		
	    var result=false;
		ChildrenFlag=0;
		this.RemoveValueDataNNumber();
		switch (MenuFlag) {	
		case 10:
			MenuFlag=0;
			return this.SetDefaultValue();		
	    case 0:		
			var liList=$(".level-1 div ul li");
			var liListLength=liList.length;
			if(liListLength>0)
			{
				MenuFlag=3;
				return this.SetLevelOneValue();				
			}				
	    case 3:
			var liList=$(".level-2 div ul li");
			var liListLength=liList.length;
			if(liListLength>0)
			{
				MenuFlag=4;
				return this.SetLevelTwoValue();				
			}
			else
			{
				MenuFlag=0;
				return this.SetDefaultValue();		
			}	
	    case 4:		
			var divTmp=$(".level-3");
			if(divTmp.is('.none'))
			{
				MenuFlag=5;
				var returnValue=this.SetChildrenValue();
				if(returnValue==8)
				{
					MenuFlag=0;
					return this.SetDefaultValue();	
				}	
				else if(returnValue==7)
				{
					ChildrenFlag=1;
					MenuFlag=7;
					return true; 
				}					
				else
				{
					ChildrenFlag=1;
					return true;
				}
			}
			else
			{				
				MenuFlag=5;
				return this.SetLevelThreeValue();	
			}	
		case 5:
			var divTmp=$(".level4-menu-button-zone");
			//alert($(".level4-menu-button-zone").html());
			if(divTmp.is('.none'))
			{
				MenuFlag=6;
				var returnValue=this.SetChildrenValue();
				if(returnValue==8)
				{
					MenuFlag=0;
					return this.SetDefaultValue();	
				}	
				else if(returnValue==7)
				{
					ChildrenFlag=1;
					MenuFlag=8;
					return true; 
				}					
				else
				{
					ChildrenFlag=1;
					return true;
				}
			}
			else
			{				
				MenuFlag=6;
				return this.SetLevelFourValue();	
			}		
		case 6:
			var returnValue=this.SetChildrenValue();
			if(returnValue==8)
			{
				MenuFlag=0;
				return this.SetDefaultValue();	
			}
			else if(returnValue==7)
			{
				ChildrenFlag=1;
				MenuFlag=7;
				return true; 
			}	
			else
			{
				ChildrenFlag=1;
				MenuFlag=6;
				return true;
			}				
		case 7:
			var divTmp=$("#handleguide-container");
			if(divTmp.is('.none'))
			{
				MenuFlag=0;
				return this.SetDefaultValue();		
			}
			else{
				MenuFlag=8;
				return this.SetBottomButtonValue();	
			}
		case 8:
			MenuFlag=0;
			return this.SetDefaultValue();				
	    default:
			MenuFlag=10;
			return true;	
	    }
	};	
	
    //--------------------------------
	window.CustomUi = CustomUi;
	//--------------------------------
})(window);

MenuFlag=10;
ChildrenFlag=0;
/*
0: 底部主要按钮
1：便捷模式错误菜单列表
2：便捷模式便捷菜单列表
3：1级功能菜单
4：2级功能菜单
5：3级功能菜单
6: 4级功能菜单
7: 子窗口模式
8: 子窗口没有需要设置的元素子窗口没有需要设置的元素
9: 单独的按钮
*/

$(function() {
	new CustomUi();
});
//以下代码解决  后维护关闭或者重启系统或者VDM时，没有隐藏注销或者退出按钮  start
//当页面从快捷模式点进来的时候，先检测二级菜单对应的（关闭机器，重启机器，厂商模式）是否被选中，如果有选中则屏蔽下方两个按钮
function firstCome(){
	var tabString = $(".level-2 div ul li.selected").find("span").text();
	if(tabString.indexOf("关闭机器") >= 0 ||
		tabString.indexOf("重启机器") >= 0 ||
		  tabString.indexOf("厂商模式") >= 0 ||
			tabString.indexOf("SHUTDOWN ATM") >= 0 ||
			  tabString.indexOf("REBOOT ATM") >= 0 ||
				tabString.indexOf("VENDOR DEPENDENT MODE") >= 0
	){
		$(".bottom .bottom-button-zone .float-left").hide();
		$("#FUN_LOGOUT").attr("disabled", "disabled");
		$("#FUN_QUIT").attr("disabled", "disabled");
	}else{
		$(".bottom .bottom-button-zone .float-left").show();
		$("#FUN_LOGOUT").removeAttr("disabled");
		$("#FUN_QUIT").removeAttr("disabled");
	}
} 

//页面加载完200毫秒后执行检测
$(function(){
	setTimeout("firstCome()", 200);

});

$(".level-1 div ul").on("click","li",function(){
	//点击了一级菜单也要显示注销用户、退出维护两个按钮，并且启用
	$(".bottom .bottom-button-zone .float-left").show();
	$("#FUN_LOGOUT").removeAttr("disabled");
	$("#FUN_QUIT").removeAttr("disabled");
});

$(".level-2 div ul").on("click","li",function(){
	var tabString = $(".level-2 div ul li.selected").find("span").text();
	//判断是否点击了对应的二级菜单指定的按钮
	if(tabString.indexOf("关闭机器") >= 0 ||
		tabString.indexOf("重启机器") >= 0 ||
		  tabString.indexOf("厂商模式") >= 0 ||
			tabString.indexOf("SHUTDOWN ATM") >= 0 ||
			  tabString.indexOf("REBOOT ATM") >= 0 ||
				tabString.indexOf("VENDOR DEPENDENT MODE") >= 0
	){
		//指定的按钮：隐藏注销用户、退出维护两个按钮，并且禁用
		$(".bottom .bottom-button-zone .float-left").hide();
		$("#FUN_LOGOUT").attr("disabled", "disabled");
		$("#FUN_QUIT").attr("disabled", "disabled");
	}else{
		//非指定按钮：显示注销用户、退出维护两个按钮，并且启用
		$(".bottom .bottom-button-zone .float-left").show();
		$("#FUN_LOGOUT").removeAttr("disabled");
		$("#FUN_QUIT").removeAttr("disabled");
	}
});	
//以上代码解决  后维护关闭或者重启系统或者VDM时，没有隐藏注销或者退出按钮   end