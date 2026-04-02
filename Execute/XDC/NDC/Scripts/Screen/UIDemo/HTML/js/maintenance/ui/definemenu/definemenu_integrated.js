/******************** 
	作用:自定义菜单
	作者:adam
	版本:V1.0
	时间:2015-08-17
********************/

(function(window) {

	function CustomUi() 
	{
	    this.InvalSeparator ='\x1B';
		this.dmenu;
		/**是否为键盘触发*/
		this.KeyboardFlag=false;
		CustomUi.superClass.constructor.call(this);
	}
	
	

	Config.extend(CustomUi, Interface);

	//预处理
	CustomUi.prototype.preInit = function() {
		this.cleanData();
		//this.SetDefaultValue();
		//console.log("AAf");
		
	};
	
	//
	//生成界面后
	CustomUi.prototype.afterInitUi = function() {
		try 
        {
			
        } catch (e) {
			Config.log(e);
		}

	};
	
	//清除界面信息,用于从头开始显示信息
	CustomUi.prototype.cleanData = function() {
		
	};

	//重写方法设置值
	CustomUi.prototype.setValue = function(data) {
		var result = false;
		try 
        {
			if (data.id=="INPUT_DEFINEMENU")
            {
			    result=this.MySetValue(data);
			} 
			else if (data.id=="TWIN_KEYBOARD")
			{
				    result = this.KeyAction(data.value);
			} 
            else {
			   result =CustomUi.superClass.setValue.call(this,data);
			}
            
			
		} catch (e) {
			result = false;
			Config.log(e);
		}

		return result;
	};
	

    CustomUi.prototype.getValue = function(data) {
		var result = false;
		try 
        {
			if (data=="INPUT_DEFINEMENU")
            {
			    result=this.MyGetValue(data);
			} 
            else {
			   result =CustomUi.superClass.getValue.call(this,data);
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
	    
        var ids = data.value.split(this.InvalSeparator);

	    if (ids.length <= 0)
	        return false;
	    var menuitem;
	    var m_this = this;
	    var id;
        //初始化
         this.dmenu=new DefineMenu(Config.TWIN_MENU);
	     this.dmenu.init();

	    $(ids).each(
	        function() {
	            id = this;
	            menuitem=m_this.dmenu.getMenuInfo(id);
	            if (menuitem.id != "" && menuitem.id != undefined) {
	                m_this.dmenu.addSelectMenuShow(menuitem);
	            }

	        }
	    );
		//this.dmenu.addSelectMenuShow

	    setTimeout(function() { m_this.dmenu.showChild(1); }, "10");

		return result;
	};
	
    // 私有方法 设值
	CustomUi.prototype.MyGetValue= function(data)
	{
	    var result="";
	    var m_this = this;
	    var item;
	    $(m_this.dmenu.selectMenu).each(
	        function() {
	            item = this;
	            result += item.id + ":" + item.name + m_this.InvalSeparator;
	        }
	    );
		
		return result;
	};
	
	//一进入界面就给底部3个按钮赋值
    CustomUi.prototype.SetDefaultValue= function()
	{	
	    var result=false;
		var strTmp="";
		var buttonText="";
		var bottomButton = $("#FUN_LOGOUT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("1"+strTmp);
		bottomButton.val("Select1");
		
		bottomButton = $("#FUN_QUIT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("2"+strTmp);
		bottomButton.val("Select2");
		
		bottomButton = $("#FUN_TABMODE");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("3"+strTmp);
		bottomButton.val("Select3");
		return true;
	};
	

	//设置自定义菜单列表数字
	CustomUi.prototype.SetButtomValue= function()
	{	
		//console.log("hghy56");	
	    var result=false;		
		//设置ul里边的值
		var buttonList=$(".choice-box ul li");
		var buttonListLength=buttonList.length;
		for(var i=0;i<buttonListLength;i++)
		{
			var span = ($(buttonList[i]).children("button").children("span"))[0];
			var spanText = $(span).text();
			//console.log(spanText);
			$(span).text((i+1)+spanText);
			$(buttonList[i]).children("button").val("Select"+(i+1));
		}
		
		//设置翻页按钮的值
		var sideButtonList=$(".g-button-zone button");
		var sideListLength=sideButtonList.length;
		//alert(sideListLength);
		for(var i=0;i<sideListLength;i++)
		{
			
			var span = ($(sideButtonList[i]).children("span"))[1];
			var spanText = $(span).text();
			console.log(spanText);
			$(span).text((i+5)+spanText);
			$(sideButtonList[i]).val("Select"+(i+5));
		}		
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
			//$(liList[i]).children("button").val("Select"+(i+1));
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
	
	//设置自定义菜单数字
	CustomUi.prototype.setSelectValue= function()
	{		
		var result=false;
		//设置ul里边的置
		var liList=$(".seleted-box ul li");
		var liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]).children("button").children("span"))[1];
			var spanText = $(span).text();
			$(span).text((i+1)+spanText);
			$(liList[i]).children("button").val("Select"+(i+1));
		}

		//设置翻页按钮值
		var buttonPre=$(".seleted-box #SELECTED_PRE_PAGE");
		var buttonNext=$(".seleted-box #SELECTED_NEXT_PAGE");
		if(buttonPre.is('.none'))
		{
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
	
	//按键响应
    CustomUi.prototype.KeyAction= function(key)
	{
	    var result=false;
		this.KeyboardFlag=true;
	    switch (key) {				
	    // case "ENTER":
		// return alert(111);
        // case "CANCEL":
		// return alert(222);
		case "1":			
			$("button[value='Select1']").click();
			$("#Select1").click();
			this.showAgain();
			return true;
			//this.keyBoardAction($("button[value='Select1']").attr('id'),key);
		case "2":
			$("button[value='Select2']").click();
			$("#Select2").click();
			this.showAgain();
			return true; 
		case "3":
			$("button[value='Select3']").click();
			$("#Select3").click();
			this.showAgain();
			return true; 
		case "4":
			$("button[value='Select4']").click();
			$("#Select4").click();
			this.showAgain();
			return true; 
		case "5":
			$("button[value='Select5']").click();
			$("#Select5").click();
			this.showAgain();
			return true; 
		case "6":
			$("button[value='Select6']").click();
			$("#Select6").click();
			this.showAgain();
			return true;  
		case "7":
			$("button[value='Select7']").click();
			$("#Select7").click();
			this.showAgain();
			return true;  
		case "8":
			$("button[value='Select8']").click();
			$("#Select8").click();
			this.showAgain();
			return true;  
		case "9":
			$("button[value='Select9']").click();
			$("#Select9").click();
			return true;  
		case "ENTER":return this.keyBoardAction("KEYBOARD_ENTER",key);
        case "CANCEL":return this.keyBoardAction("FUN_QUIT",key);
	    case "KEYBOARD_TAB":return this.SwitchMenu();
        // 后续插入快捷键等等
        //.........
	    default:
	        return this.keyBoardAction("keyboard",key);
	    }
	};
	CustomUi.prototype.showAgain= function(){
		var m_this=this;
		//alert(m_this.KeyboardFlag);
		switch(MenuFlag){
			case 2:
				m_this.RemoveValueDataNNumber();
				m_this.setSelectValue();
				break;
			case 3:
				m_this.RemoveValueDataNNumber();
				m_this.SetButtomValue();	
				break;
			default:break;	
		}
				
	};
	
	
	//清除数字
	CustomUi.prototype.RemoveValueDataNNumber= function()
	{	
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
			//$(liList[i]).children("button").val("");
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

			//清除选择菜单数据
			var liList=$(".seleted-box ul li");
			var liListLength=liList.length;
			for(var i=0;i<liListLength;i++)
			{
				var span = ($(liList[i]).children("button").children("span"))[1];
				var spanText = $(span).text();

				var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
				var num=spanText.substr(0,1);
				if(reg.test(num))
				{
					$(span).text(spanText.substr(1,spanText.length-1));
				}
				$(liList[i]).children("button").val("");
			}
			//
			var buttonPre=$(".seleted-box #SELECTED_PRE_PAGE");
			var buttonNext=$(".seleted-box #SELECTED_NEXT_PAGE");
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
	
		//清除侧边栏按钮设定的值
		var sideButtonList = $(".g-button-zone button");
		var sideLength = sideButtonList.length;
		for(var i=0;i<sideLength;i++)
		{
			//var span = $(sideButtonList[i]).children(".button-text");
			var span = ($(sideButtonList[i]).children("span"))[1];
			var spanText = $(span).text();
			var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num=spanText.substr(0,1);
			if(reg.test(num))
			{
				$(span).text(spanText.substr(1,spanText.length-1));
			}
			//$(sideButtonList[i]).attr("name","");
			$(sideButtonList[i]).val("");
		}

		//清除底部按钮设定的值
		var buttonList=$(".choice-box ul li");
		var buttonListLength=buttonList.length;
		for(var i=0;i<buttonListLength;i++)
		{
			var span = ($(buttonList[i]).children("button").children("span"))[0];
			var spanText = $(span).text();
			var reg = /^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num = spanText.substr(0, 2);
			if (reg.test(num)) {
				$(span).text(spanText.substr(1, spanText.length - 2));
			}
			$(buttonList[i]).children("button").val("");
		}
		
		//清除最底下三个按钮数字
		var strTmp="";
		var buttonText="";
		var bottomButton = $("#FUN_LOGOUT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
    	}			
		bottomButton.val("");
		
		bottomButton = $("#FUN_QUIT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		num=strTmp.substr(0,1);
		if(reg.test(num))
		{			
			buttonText.text(strTmp.substr(1,strTmp.length-1));
			
    	}	
		bottomButton.val("");
		
		bottomButton = $("#FUN_TABMODE");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
    	}	
		bottomButton.val("");	
		
		//清除顶部
			var result=false;
			//清除选择菜单数据
			var liList=$(".seleted-box ul li");
			var liListLength=liList.length;
			for(var i=0;i<liListLength;i++)
			{
				var span = ($(liList[i]).children("button").children("span"))[1];
				var spanText = $(span).text();

				var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
				var num=spanText.substr(0,1);
				if(reg.test(num))
				{
					$(span).text(spanText.substr(1,spanText.length-1));
				}
				$(liList[i]).children("button").val("");
			}
			//
			var buttonPre=$(".seleted-box #SELECTED_PRE_PAGE");
			var buttonNext=$(".seleted-box #SELECTED_NEXT_PAGE");
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
		return true;
	};	
	
	//切换功能
	CustomUi.prototype.SwitchMenu= function()
	{	
	    var result=false;
		var m_this = this;
		
		m_this.RemoveValueDataNNumber();
		switch (MenuFlag) {		
	    case 0:		
			var buttonList=$(".level-1 div ul li");
			var buttonListLength=buttonList.length;
			if(buttonListLength>0)
			{
				MenuFlag=1;
				return m_this.SetLevelOneValue();				
			}
	    case 1:		
			var liList=$(".seleted-box ul li");
			var liListLength=liList.length;
			if(liListLength>0)
			{
				MenuFlag=2;
				return m_this.setSelectValue();
				//return 1;
			}else{
				MenuFlag=3;
				return m_this.SetButtomValue();
				
			}
			
	    case 2:
			var buttonList=$(".g-button-zone button");
			var buttonListLength=buttonList.length;
			if(buttonListLength>0)
			{
				MenuFlag=3;
				return m_this.SetButtomValue();	
				
			}else{
				MenuFlag=0;
				return m_this.SetDefaultValue();
				
			}
			
		/*
	    case 3:
			var buttonList=$(".seleted-box ul li");
			var buttonListLength=buttonList.length;
			if(buttonListLength>0)
			{
				MenuFlag=4;
				return defineMenu.RemoveValueDataNNumber();				
			}	*/		
	    case 3:
				MenuFlag=0;
				return m_this.SetDefaultValue();
		
	    default:
			MenuFlag=0;
			return m_this.SetDefaultValue();	
	    }
	};	
	
	//--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
	
	function DefineMenu(menu) {

		
		//------------------属性---------------------------
        /** 菜单XML数据源对象*/
	    this.menu;
        /** 一级菜单 */
	    this.headMenu = [];
        /** 二级菜单 */
        this.child = [];
        /** 已选菜单 */
	    this.selectMenu = [];
        /** 一级菜单按钮列表 */
	    this.headIDS = [];
        //------------------附加属性---------------------------
        /** html 显示模板 */
	    this.headMenuString = "";
	    this.childString = "";
	    this.selectMenuString = "";
		/**是否为键盘触发*/
		//this.KeyboardFlag=false;
		this.MenuFlag=0;
        /** 子菜单按钮列表 */
	    this.childMenus = [];
        /** 当前子菜单显示页数 */
	    this.childPageIndex;
        /** 当前子菜单页显示控件个数 */
	    this.childPageSize;
        /** 当前子菜单页数 */
	    this.childPageCount;
        /** 当前页显示已选菜单 */
	    this.selectMenuShow = [];
        /** 当前页显示已选菜单个数 */
	    this.selectMenuShowPageSize;
        /** 当前第几页显示已选菜单 */
	    this.selectMenuShowPageIndex;

	    //---------------------初始化
	    this.menu = String2XML(menu);

	    this.headIDS = [1, 2, 3, 4, 5, 6];

        this.childPageIndex=0;

	    this.childPageSize = 4;
        /**规定每一页显示4个已选菜单*/
        this.selectMenuShowPageSize=4;

	    this.selectMenuShowPageIndex = 1;

	    this.headMenuString = '<li>'+
						'<div class="bg"></div>'+
						'<a href="#">'+
						'<div class="middle-outer">'+
							'<div class="middle-inner"> <input id="MenuID{0}" class="none" value="{0}"></input> <span>{1}</span></div>'+
						'</div>'+
						'</a></li>';

	    this.childString = '<li>'+
						'<button id="BUTTON_{0}" type="button" class="button-define" isAutoSend="0" focusGroup="row1"> <span class="tip"></span>  <span class="button-text">{1}</span> </button>'+
					'</li>';

	    this.selectMenuString = '<li>' +
	        '<button id="FUN_{0}" type="button" class="button-define" isAutoSend="0" focusGroup="define"> <span class="tip"></span> <span class="button-text">{1}</span> </button>' +
	        '</li>';

	}

	// 添加属性方法
	DefineMenu.prototype = {
		// 添加构造函数
		constructor: DefineMenu,
        /** 初始化函数 */
        init: function() {
			//this.KeyboardFlag=false;
			//this.MenuFlag=0;
            //添加父菜单
            this.addHeadMenu();
            // 父菜单初始化
            this.HeadMenuInit();
			//this.SetDefaultValue();

        },
        /** 添加父菜单 */
        addHeadMenu: function() {
            var str = "";
            var menuitem;   
            for (var i = 0;i < this.headIDS.length; i++) {
                menuitem = this.getMenuInfo(this.headIDS[i]);
                if (menuitem != undefined && menuitem.visible == "1") {
                    str += this.headMenuString.format(menuitem.id, menuitem.name);
                }
                
            }

            $(".level-1 ul").html(str);

        },

        HeadMenuInit: function() {
            var id;
            //初始化第一个按钮选中状态(这里有逻辑问题，当第一个按钮没有孩子节点时，应该寻找第二个按钮选中)
            //todo 还未实现
            $(".level-1 ul li").eq(0).addClass("selected");
            var m_this = this;
            //注册点击事件
            $(".level-1 ul li").each(
                function() {
                    id=$(this).find("input").val();
                    if (m_this.hasChild(id)) {
                        $(this).click(
                            function() {
                                $(".level-1 ul li").removeClass("selected");
                                $(this).addClass("selected");
                                id=$(this).find("input").val();
                                // 初始化孩子节点
                                m_this.addChild(id);
                                
                            }
                        );
                    } 
                    else {
                        // 这里要实现 父菜单 按钮变灰无法点击
                        //todo 还未实现
                    }
                }
            );
            // 需要初始化按钮可点击属性disabled
            
            id = $(".level-1 ul .selected").find("input").val();

            if (this.hasChild(id)) {
                this.addChild(id);
            }

        },

        addChild: function(id) {
            var menuitem;
            var m_this = this;
            //必须初始化
            m_this.childMenus = [];
            // 遍历所有的孩子节点（需要写逻辑，取出最多16个先显示，如果超过16个则需要控制翻页逻辑）
            $(this.menu).find("config").children().each(
		        function() {
		            var newid = $(this).attr("id");
			        if(newid.indexOf(id)==0&&newid!=id&&$(this).attr("visible")=="1"&&!m_this.hasChild(newid)) {
			            menuitem = m_this.getMenuInfo(newid);
			            m_this.childMenus.push(menuitem);
			        }
		        }
		    );
            //显示按钮
            m_this.showChild(1);
        },
        // 必须从第一页开始显示 ,显示定制菜单
        showChild: function(index) {

            //生成按钮
            var j = 4;
            var str="";
            this.childPageIndex = index;
            var pageCount = 0;
            if (this.childMenus.length % this.childPageSize > 0) {
                pageCount = parseInt(this.childMenus.length / this.childPageSize) + 1;
            } else {
                pageCount = parseInt(this.childMenus.length / this.childPageSize);
            }

            this.childPageCount = pageCount;

            if (index > pageCount)
                return;
            
            if (this.childMenus.length < this.childPageSize * index) {
                j = this.childMenus.length - this.childPageSize * (index- 1);
            }

            var baseindex = this.childPageSize * (index - 1);


            for (var i = 0; i < j; i++) {
                str += this.childString.format(this.childMenus[baseindex+i].id,this.childMenus[baseindex+i].name);
            }
            

            $(".choice-box ul").html(str);

            this.initChild();

            // 设置翻页按钮逻辑
            this.setPageNavBox();
        },
        /**设置子控件翻页按钮，并且注册事件 */
        setPageNavBox: function() {
			 var m_this = this;
			$("#FUN_UP").unbind("touchend");
			$("#FUN_DOWN").unbind("touchend");

            $("#FUN_DOWN").unbind("click");
            $("#FUN_UP").unbind("click");

            if (m_this.childPageIndex == 1) 
            {
                $("#FUN_UP").addClass("disabled");  
                
            } else {
                $("#FUN_UP").removeClass("disabled");
				$("#FUN_UP").on("touchend",
					    function() {
                        if (m_this.childPageIndex - 1 < 1)
                            m_this.showChild(1);
                        else {
							
                            m_this.showChild(m_this.childPageIndex - 1);
							
							//if(m_this.KeyboardFlag&&MenuFlag==3){// 
								//上一页事件
								//alert("AADF");
								/*
								alert(m_this.KeyboardFlag);
								m_this.RemoveValueDataNNumber();
								m_this.SetButtomValue();*/
								//m_this.KeyboardFlag=false;
							//}

                        }
                    }
				
				);
                //注册事件
                $("#FUN_UP").on("click",
                    function() {
                        if (m_this.childPageIndex - 1 < 1)
                            m_this.showChild(1);
                        else {
							
                            m_this.showChild(m_this.childPageIndex - 1);
							
							//if(m_this.KeyboardFlag&& MenuFlag==3 ){//
								//上一页事件
								/*
								m_this.RemoveValueDataNNumber();
								m_this.SetButtomValue();*/
								//m_this.KeyboardFlag=false;
							//}

                        }
                    }
                );
				
            }

            if (m_this.childPageCount <= m_this.childPageIndex) {
                $("#FUN_DOWN").addClass("disabled");
                 

            } else {
                $("#FUN_DOWN").removeClass("disabled");
				$("#FUN_DOWN").on("touchend",
						 function() {
                        if (m_this.childPageIndex + 1 > m_this.childPageCount) 
                        {
                            m_this.showChild(m_this.childPageCount);
                        } 
                        else 
                        {
                            m_this.showChild(m_this.childPageIndex + 1);
							
							if(m_this.KeyboardFlag && MenuFlag==3){//
							
								//上一页事件
								m_this.RemoveValueDataNNumber();
								m_this.SetButtomValue();
								m_this.KeyboardFlag=false;
							}
                        }

                    }
				
				);
				
				
                //注册事件
                $("#FUN_DOWN").on("click",
                    function() {
                        if (m_this.childPageIndex + 1 > m_this.childPageCount) 
                        {
                            m_this.showChild(m_this.childPageCount);
                        } 
                        else 
                        {
                            m_this.showChild(m_this.childPageIndex + 1);
							
							if(m_this.KeyboardFlag&&MenuFlag==3){//
								alert("AA");
								//上一页事件
								m_this.RemoveValueDataNNumber();
								m_this.SetButtomValue();
								m_this.KeyboardFlag=false;
							}
                        }

                    }
                );
				
            }

        },
		SetButtomValue:function(){
			var result=false;		
			//设置ul里边的值
			var buttonList=$(".choice-box ul li");
			var buttonListLength=buttonList.length;
			for(var i=0;i<buttonListLength;i++)
			{
				var span = ($(buttonList[i]).children("button").children("span"))[0];
				var spanText = $(span).text();
				//console.log(spanText);
				$(span).text((i+1)+spanText);
				$(buttonList[i]).children("button").val("Select"+(i+1));
			}
			//设置翻页按钮的值
			var sideButtonList=$(".g-button-zone button");
			var sideListLength=sideButtonList.length;
			//alert(sideListLength);
			for(var i=0;i<sideListLength;i++)
			{
				
				var span = ($(sideButtonList[i]).children("span"))[1];
				var spanText = $(span).text();
				console.log(spanText);
				$(span).text((i+5)+spanText);
				$(sideButtonList[i]).val("Select"+(i+5));
			}		
				return true;
		},
		
		//初始化子按钮
        initChild: function() {
            var m_this = this;
            var id;
            var menuitem;
            $(".choice-box li").each(
                function() {
                    //如果该按钮在已选菜单列表中则，则不显示它，设置透明度为0
                    id = $(this).find("button").attr("id");
                    id = id.replace("BUTTON_", "");
                    menuitem = m_this.getMenuInfo(id);
                    $(this).unbind("click");

                    if (m_this.findMenuIndex(m_this.selectMenu, menuitem) >=0) //该按钮已选则隐藏它
                    {
                        $(this).css("opacity", "0");
                        $(this).find("button").css("cursor", "default");
                        
                    } 
                    else // 注册事件
                    {
                        $(this).click(
                        function() 
                        {
                            if (m_this.selectMenu.length < 19) {

                                $(this).css("opacity", "0");
                                $(this).find("button").css("cursor", "default");
                                $(this).unbind("click");
                                //需要实现 把选中的按钮放置到已选列表中
                                //已经实现了
                                id = $(this).find("button").attr("id");
                                id = id.replace("BUTTON_", "");
                                menuitem = m_this.getMenuInfo(id);
                                m_this.addSelectMenuShow(menuitem);
                            } 
                            else {
                                //向后台发送超过可选最大数量的限制
                                // todo 已经实现 FUN_199
                                var i = {
                                    action: "click",
                                    data: {
                                        id: "FUN_199"
                                    }
                                };
                                Config.send(i);
                            }
                        }
                    );
                    }

                    


                }
            );
        },


        //-------------------------------------------------------------------------------
        //已选按钮功能模块
        //--------------------------------------------------------------------------------
        /** 添加当前显示已选按钮*/
        addSelectMenuShow: function(o) {

            try {
                if (this.selectMenu.length>=19) {
                    // 实现逻辑，向后台发送数据 提示用户超过19个菜单啦
                    // todo 已经实现 FUN_199
                    var i = {
                                    action: "click",
                                    data: {
                                        id: "FUN_199"
                                    }
                                };
                    Config.send(i);
                    return;
                }

                if (o == undefined)
                    return;

                if (this.selectMenuShow.length < this.selectMenuShowPageSize) {
                    this.selectMenuShow.push(o);
                } else {
                    //显示的页数加1
                    this.selectMenuShowPageIndex++;
                    this.selectMenuShow = [];
                    this.selectMenuShow.push(o);
                }
                //已选按钮总组
                this.selectMenu.push(o);
                
                //显示当前页的已选按钮
                this.showSelectMenuShow(this.selectMenuShowPageIndex);


            } catch (e) {                   
                Config.log(e);
            } 


		},

        /** 删除当前已选按钮*/
        delSelectMenuShow   : function(o) {

            try {
                var index = this.findMenuIndex(this.selectMenuShow, o);
                if (index < 0 || index > this.selectMenuShow - 1) {
                    return false;
                }
                // 从数组中删除一个元素
                this.selectMenuShow.splice(index,1);

                index =this.findMenuIndex( this.selectMenu,o);
                //从总组中也删除一条数据
                this.selectMenu.splice(index, 1);
                
                return true;

            } catch (e) {
                Config.log(e);
                return false;
            } 


		},
        //显示选择菜单栏
        showSelectMenuShow: function(index) {
            try {
                var str = "";
                var baseIndex = 0;
                this.selectMenuShowPageIndex = index;
                var j = this.selectMenuShowPageSize;
                var pageCount = 0;
                //修正总页数
                if (this.selectMenu.length%this.selectMenuShowPageSize>0) 
                {
                    pageCount = parseInt(this.selectMenu.length / this.selectMenuShowPageSize) + 1;
                } 
                else {
                    pageCount = parseInt(this.selectMenu.length / this.selectMenuShowPageSize);
                }

                // 显示逻辑有问题 已经修改

                if (this.selectMenu.length < this.selectMenuShowPageIndex * this.selectMenuShowPageSize) {
                    j =  this.selectMenu.length-(this.selectMenuShowPageIndex-1) * this.selectMenuShowPageSize;
                }

                baseIndex = this.selectMenuShowPageSize * (this.selectMenuShowPageIndex - 1);


                this.selectMenuShow = [];
            for (var i = 0; i < j; i++) {

                str  += this.selectMenuString.format(this.selectMenu[baseIndex+i].id,this.selectMenu[baseIndex+i].name );
                this.selectMenuShow.push(this.selectMenu[baseIndex+i]);
            }
            //当没有已选按钮时需要占位控件
            if (str == undefined || str == "") 
            {
                str='<li style="visibility: hidden;">' +
	        '<button id="FUN_0" type="button" class="button-define" isAutoSend="0" focusGroup="define"> <span class="tip"></span> <span class="button-text"></span> </button>' +
	        '</li>';

                $("#SELECTED_PRE_PAGE").addClass("none");
                $("#SELECTED_NEXT_PAGE").addClass("none");
            } else {
                $("#SELECTED_PRE_PAGE").removeClass("none");
                $("#SELECTED_NEXT_PAGE").removeClass("none");
            }
            //显示已选菜单按钮
            $(".seleted-box .nav-zone ul") .html(str);

            this.initSelectMenuShow();
            this.setSelectMenuShowNav();

            } catch (e) {
                Config.log(e);
            } 
		},
        /**初始化已选菜单*/
        initSelectMenuShow: function() {
            var m_this = this;
            var id;
            var menuitem;
            $(".seleted-box .nav-zone ul li").each(
                function() {

                    $(this).click(
                        function() {
                            // 从当前显示已选列表中移除

                            id = $(this).find("button").attr("id");
                                id = id.replace("FUN_", "");
                                menuitem = m_this.getMenuInfo(id);

                                m_this.delSelectMenuShow(menuitem);

                            if (m_this.selectMenuShow.length==0) 
                            {
                                if (m_this.selectMenuShowPageIndex>1) {
                                    //页数减1
                                    m_this.selectMenuShowPageIndex--;
                                    m_this.selectMenuShow = [];

                                    //从总表中拷贝出新的已选按钮填充
                                    for (var i = 0; i < m_this.selectMenuShowPageSize; i++) {
                                        m_this.selectMenuShow.push(m_this.selectMenu[(m_this.selectMenuShowPageIndex-1) * m_this.selectMenuShowPageSize + i]);
                                    }
                                    
                                }
                                
                            }

                            m_this.showSelectMenuShow(m_this.selectMenuShowPageIndex);

                            //显示子菜单（当在已选菜单中删除该按钮后，需要在可选子菜单中把其取消隐藏让其可选）
                            m_this.showChild(m_this.childPageIndex);
                        }
                    );


                }
            );

        },
        /**设置已选菜单翻页按钮*/
        setSelectMenuShowNav: function() {
        var m_this = this;
        var pageCount = 0;
        if (this.selectMenu.length % this.selectMenuShowPageSize > 0) {
            pageCount =parseInt( this.selectMenu.length / this.selectMenuShowPageSize )+ 1;
        } else {
            pageCount =parseInt( this.selectMenu.length / this.selectMenuShowPageSize );
        }

        $("#SELECTED_PRE_PAGE").unbind("click");
         $("#SELECTED_NEXT_PAGE").unbind("click");

        if (this.selectMenuShowPageIndex > 1) {
            
            $("#SELECTED_PRE_PAGE").removeClass("disabled");
			
            $("#SELECTED_PRE_PAGE").click(
                function() {
                    m_this.selectMenuShowPageIndex--;
                    m_this.showSelectMenuShow(m_this.selectMenuShowPageIndex);
					
					if(m_this.KeyboardFlag && MenuFlag==2){
						//左按钮翻页事件
						m_this.RemoveValueDataNNumber();
						m_this.setSelectValue();
						KeyboardFlag=false;
						
					}


                }
            );
			
        } 
        else {
            $("#SELECTED_PRE_PAGE").addClass("disabled");
            
        }

        if (this.selectMenuShowPageIndex < pageCount) {

        $("#SELECTED_NEXT_PAGE").removeClass("disabled");
			
            $("#SELECTED_NEXT_PAGE").click(
                function() {
                    m_this.selectMenuShowPageIndex++;
                    m_this.showSelectMenuShow(m_this.selectMenuShowPageIndex);
					
					if(m_this.KeyboardFlag &&MenuFlag==2){
						//左按钮翻页事件
						m_this.RemoveValueDataNNumber();
						m_this.setSelectValue();
						KeyboardFlag=false;
						}
					
                }
            );
			
        } else {
            $("#SELECTED_NEXT_PAGE").addClass("disabled");
           
        }


        },

	//设置一级菜单数据
    SetLevelOneValue:function()
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
			//$(liList[i]).children("button").val("Select"+(i+1));
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
	},	
		SetDefaultValue:function(){
			var result=false;
			var strTmp="";
			var buttonText="";
			var bottomButton = $("#FUN_LOGOUT");//document.getElementById("FUN_LOGOUT");
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			buttonText.text("1"+strTmp);
			bottomButton.val("Select1");
			
			bottomButton = $("#FUN_QUIT");//document.getElementById("FUN_LOGOUT");
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			buttonText.text("2"+strTmp);
			bottomButton.val("Select2");
			
			bottomButton = $("#FUN_TABMODE");//document.getElementById("FUN_LOGOUT");
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			buttonText.text("3"+strTmp);
			bottomButton.val("Select3");
			return true;			
		},
		//
		setSelectValue:function(){
			var result=false;
			//设置ul里边的置
			var liList=$(".seleted-box ul li");
			var liListLength=liList.length;
			for(var i=0;i<liListLength;i++)
			{
				var span = ($(liList[i]).children("button").children("span"))[1];
				var spanText = $(span).text();
				$(span).text((i+1)+spanText);
				$(liList[i]).children("button").val("Select"+(i+1));
			}

			//设置翻页按钮值
			var buttonPre=$(".seleted-box #SELECTED_PRE_PAGE");
			var buttonNext=$(".seleted-box #SELECTED_NEXT_PAGE");
			if(buttonPre.is('.none'))
			{
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
		},
		
		RemoveValueDataNNumber:function(){
			 var result=false;
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
				//$(liList[i]).children("button").val("");
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
			//清除选择菜单数据
			var liList=$(".seleted-box ul li");
			var liListLength=liList.length;
			for(var i=0;i<liListLength;i++)
			{
				var span = ($(liList[i]).children("button").children("span"))[1];
				var spanText = $(span).text();

				var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
				var num=spanText.substr(0,1);
				if(reg.test(num))
				{
					$(span).text(spanText.substr(1,spanText.length-1));
				}
				$(liList[i]).children("button").val("");
			}
			//
			var buttonPre=$(".seleted-box #SELECTED_PRE_PAGE");
			var buttonNext=$(".seleted-box #SELECTED_NEXT_PAGE");
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
					//清除侧边栏按钮设定的值
		var sideButtonList = $(".g-button-zone button");
		var sideLength = sideButtonList.length;
		for(var i=0;i<sideLength;i++)
		{
			//var span = $(sideButtonList[i]).children(".button-text");
			var span = ($(sideButtonList[i]).children("span"))[1];
			var spanText = $(span).text();
			var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num=spanText.substr(0,1);
			if(reg.test(num))
			{
				$(span).text(spanText.substr(1,spanText.length-1));
			}
			//$(sideButtonList[i]).attr("name","");
			$(sideButtonList[i]).val("");
		}

		//清除底部按钮设定的值
		var buttonList=$(".choice-box ul li");
		var buttonListLength=buttonList.length;
		for(var i=0;i<buttonListLength;i++)
		{
			var span = ($(buttonList[i]).children("button").children("span"))[0];
			var spanText = $(span).text();
			var reg = /^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num = spanText.substr(0, 2);
			if (reg.test(num)) {
				$(span).text(spanText.substr(1, spanText.length - 2));
			}
			$(buttonList[i]).children("button").val("");
		}
			
			
			
			//清除最底下三个按钮数字
			var strTmp="";
			var buttonText="";
			var bottomButton = $("#FUN_LOGOUT");//document.getElementById("FUN_LOGOUT");
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num=strTmp.substr(0,1);
			if(reg.test(num))
			{
				buttonText.text(strTmp.substr(1,strTmp.length-1));
	    	}			
			bottomButton.val("");
			
			bottomButton = $("#FUN_QUIT");//document.getElementById("FUN_LOGOUT");
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			num=strTmp.substr(0,1);
			if(reg.test(num))
			{			
				buttonText.text(strTmp.substr(1,strTmp.length-1));
				
	    	}	
			bottomButton.val("");
			
			bottomButton = $("#FUN_TABMODE");//document.getElementById("FUN_LOGOUT");
			buttonText = bottomButton.children(".button-text");
			strTmp=buttonText.text();
			num=strTmp.substr(0,1);
			if(reg.test(num))
			{
				buttonText.text(strTmp.substr(1,strTmp.length-1));
	    	}	
			bottomButton.val("");
			
			return true;		
		},
		

//--------------------------------------------------------
// 公共方法
//----------------------------------------------------------
        /** 添加已选按钮 */
        selectMenu: function(id) {
            try {

                var menuitem = this.getMenuInfo(id);
                if (menuitem == undefined)
                {return;}

                if (this.selectMenu.length >= 19) {
                    //需要发送消息到后台，或者自己弹框处理
                    // todo 已经实现
                    var i = {
                                    action: "click",
                                    data: {
                                        id: "FUN_199"
                                    }
                                };
                    Config.send(i);
                    return;
                }

                this.selectMenu.push(menuitem);


            } catch (e) {
                Config.log(e);  
            } 


		},


/** 获取菜单信息(包含菜单ID，菜单标题，菜单的显示情况) */
        getMenuInfo: function(id) {

            var result = {
						"id": id,
						"name": "",
                        "visible":"0"
					};

		    $(this.menu).find("config").children().each(
		    function()
		    {  
			    if($(this).attr("id")==id)
			    {
			        result.name=$(this).attr("name");
                    result.visible=$(this).attr("visible");
			        return false;
			    }
		    }
		);
		
		return result;
	    },
        /** 是否具有孩子菜单 */
        hasChild: function(id) {
            var result = false;
            
		    $(this.menu).find("config").children().each(
		    function()
		    {  
			    if($(this).attr("id").indexOf(id)==0&&$(this).attr("id")!=id&& $(this).attr("visible")=="1") {
			        result = true;
			        return false;
			    }
		    }
		);
            return result;
        },

        /**查找返回元素的位置*/
        findMenuIndex: function(list, o) {
            try {
            if (list==undefined) {
                return -1;
            }

            if (o == undefined)
                return -1;

                for (var i = 0; i < list.length; i++) {
                    if (list[i].id == o.id) {
                        return i;
                    }

                }

                return -1;

            } catch (e) {
                Config.log(e);
                return -1;

            }          

        }


	};

	
	//--------------------------------
	window.DefineMenu = DefineMenu;
	
	//--------------------------------	
	//var  defineMenu = new DefineMenu();
})(window);

var MenuFlag=3;
//var KeyboardFlag=false;

// 实例化
$(function() {
	new A();
	//new DefineMenu();
});