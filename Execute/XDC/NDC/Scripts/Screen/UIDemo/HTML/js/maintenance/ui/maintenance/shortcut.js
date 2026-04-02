/******************** 
	作用:快捷菜单
	作者:adam
	版本:V1.0
	时间:2015-05-26
********************/

(function(window) {

	function CustomUi() 
	{
	    this.InvalSeparator ='\x1B';
        /**快捷菜单列表*/
	    this.shortCutButList = [];
        /**快捷菜单页记录数*/
	    this.shortCutButPageSize = 6;
        /**快捷菜单当前显示页*/
	    this.shortCutButPageIndex = 1;
		
		/**是否为键盘触发*/
		this.KeyboardFlag=false;

	    this.shortCutButString = '<li><button id="{0}" type="button" class="button-shortcut"> <span class="tip"></span> <span class="button-text">{1}</span> </button></li>';

        /**错误菜单列表*/
        this.errorButList = [];
        /**错误菜单页记录数*/
	    this.errorButPageSize = 3;
        /**错误菜单当前显示页*/
	    this.errorButPageIndex = 1;

	    this.errorButString = '<li><button id="{0}" type="button" class="button-shortcut"> <span class="tip"></span> <span class="button-text">{1}</span> </button></li>';

		CustomUi.superClass.constructor.call(this);
	}

	Config.extend(CustomUi, Interface);

	//预处理
	CustomUi.prototype.preInit = function() {
		this.cleanData();
		this.isDelayInit = true;
		var self = this;
		self.init();		
		MenuFlag=10;
		//this.SetDefaultValue();
	};

	//清除界面信息,用于从头开始显示信息
	CustomUi.prototype.cleanData = function() {
	};
	
	//生成页面之后
	CustomUi.prototype.afterInitUi = function() {
		Config.Screen.changeState(Config.UI_SHORTCUT);
		Config.isShortcut = true;
		Config.bQueryErrorCode= false;
	};
	
	//重写方法设置值
	CustomUi.prototype.setValue = function(data) {
		var result = false;
		try 
        {
			if (data.id.indexOf("FUN_MENU") == 0||data.id.indexOf("FUN_ERROR") == 0) 
            {
			    result=this.MySetValue(data);
			}
			else if (data.id=="TWIN_KEYBOARD")
			{
				    result = this.KeyAction(data.value);
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
	    var ids = data.id.split(":");

	    if (ids.length < 2)
	        return false;
				
		if(ids[0].indexOf("FUN_ERROR") == 0)
		{
			$("#STATIC_PROMPT").addClass("none");
		    this.AddErrorBut({ "id": ids[1], "name": data.value });


		}
		else if (ids[0].indexOf("FUN_MENU") == 0) {
		    this.AddShortCutBut({ "id": ids[1], "name": data.value });
		}

		return true;
	};

    //-----------------------------------------------------------------------------------
    // 设置快捷菜单
    //--------------------------------------------------------------------------------------
    /** 初始化*/
    CustomUi.prototype.ShortCutButPerInit = function() {
        this.shortCutButList = [];
        this.shortCutButPageSize = 6;
        this.shortCutButPageIndex = 1;
    };

    /**添加快捷菜单*/
    CustomUi.prototype.AddShortCutBut = function(o) {
        this.shortCutButList.push(o);
        this.ShowShortCutBut(1);

    };
    

    /**显示快捷菜单*/
    CustomUi.prototype.ShowShortCutBut = function(index) {

        var str = "";
        var baseIndex = 0;
        this.shortCutButPageIndex = index;

        if (this.shortCutButList.length <= 0)
            return;

        var pagecout;
        if (this.shortCutButList.length % this.shortCutButPageSize > 0) {
            pagecout =parseInt( this.shortCutButList.length / this.shortCutButPageSize )+ 1;
        } 
        else {
            pagecout = parseInt(this.shortCutButList.length / this.shortCutButPageSize);
        }

        if (index > pagecout)
        {index = pagecout;}

        var j=this.shortCutButPageSize;

        if (this.shortCutButList.length < this.shortCutButPageSize * index) {
            j = this.shortCutButList.length - this.shortCutButPageSize * (index - 1);
        }

        baseIndex = this.shortCutButPageSize * (index - 1);

        for (var i = 0; i < j; i++) {
            str += this.shortCutButString.format(this.shortCutButList[baseIndex+i].id,this.shortCutButList[baseIndex+i].name);
        }

        if (str == "") {
         // 翻页按钮屏蔽
         $(".shortcut-function-button-zone .button-prev").addClass("none");
         $(".shortcut-function-button-zone .button-next").addClass("none");
        } else {
             $(".shortcut-function-button-zone .button-prev").removeClass("none");
         $(".shortcut-function-button-zone .button-next").removeClass("none");
        } 

        $(".shortcut-function-button-zone ul").html(str);
        this.ShortCutButInit();
        this.SetShortCutButNva();
    };

    /** 初始化*/
    CustomUi.prototype.ShortCutButInit = function() {
        var i;
        $(".shortcut-function-button-zone ul li").unbind("touchend");
        $(".shortcut-function-button-zone ul li").on("touchend",
            function(e) {
				e.preventDefault();
                i = {
                        action: "click",
                        data: {id: $(this).find("button").attr("id")}
                    };
                Config.send(i);
            }
        );
		
		$(".shortcut-function-button-zone ul li").unbind("click");
        $(".shortcut-function-button-zone ul li").on("click",
            function(e) {
				e.preventDefault();
                i = {
                        action: "click",
                        data: {id: $(this).find("button").attr("id")}
                    };
                Config.send(i);
            }
        );
    };

    /**设置翻页按钮*/
    CustomUi.prototype.SetShortCutButNva = function() {
		
		$(".shortcut-function-button-zone .button-prev").unbind("touchend");
        $(".shortcut-function-button-zone .button-next").unbind("touchend");
		
		$(".shortcut-function-button-zone .button-prev").unbind("click");
        $(".shortcut-function-button-zone .button-next").unbind("click");
		
        var m_this = this;
        if (this.shortCutButPageIndex > 1) 
        {
            
            $(".shortcut-function-button-zone .button-prev").removeClass("disabled");	
			$(".shortcut-function-button-zone .button-prev").on("touchend",
                function() {
                    m_this.shortCutButPageIndex--;
                    m_this.ShowShortCutBut(m_this.shortCutButPageIndex);

					if(m_this.KeyboardFlag && MenuFlag==2)
					{
						m_this.RemoveValueDataNNumber();
						m_this.SetShortcutListValue();	
						m_this.KeyboardFlag=false;
					}
                }
            );
			
			$(".shortcut-function-button-zone .button-prev").on("click",
                function() {
                    m_this.shortCutButPageIndex--;
                    m_this.ShowShortCutBut(m_this.shortCutButPageIndex);

					if(m_this.KeyboardFlag && MenuFlag==2)
					{
						m_this.RemoveValueDataNNumber();
						m_this.SetShortcutListValue();		
						m_this.KeyboardFlag=false;			
					}			
                }
            );
        } 
        else {
            $(".shortcut-function-button-zone .button-prev").addClass("disabled");
        }


        if (this.shortCutButPageIndex*this.shortCutButPageSize<this.shortCutButList.length) {

            $(".shortcut-function-button-zone .button-next").removeClass("disabled");
			$(".shortcut-function-button-zone .button-next").on("touchend",
                function(e) {
					e.preventDefault();
                    m_this.shortCutButPageIndex++;
                    m_this.ShowShortCutBut(m_this.shortCutButPageIndex);

					if(m_this.KeyboardFlag && MenuFlag==2)
					{
						m_this.RemoveValueDataNNumber();
						m_this.SetShortcutListValue();		
						m_this.KeyboardFlag=false;			
					}			
                }
            );
			
			$(".shortcut-function-button-zone .button-next").on("click",
                function(e) {
					e.preventDefault();
                    m_this.shortCutButPageIndex++;
                    m_this.ShowShortCutBut(m_this.shortCutButPageIndex);

					if(m_this.KeyboardFlag && MenuFlag==2)
					{
						m_this.RemoveValueDataNNumber();
						m_this.SetShortcutListValue();	
						m_this.KeyboardFlag=false;				
					}
                }
            );
        } else {
            $(".shortcut-function-button-zone .button-next").addClass("disabled");
        }
		


    };


    //-----------------------------------------------------------------------------------
    // 设置故障列表
    //--------------------------------------------------------------------------------------

    /**添加故障列表*/
    CustomUi.prototype.AddErrorBut = function(o) {
        this.errorButList.push(o);
        this.ShowErrorBut(1);

    };
    

    /**显示快捷菜单*/
    CustomUi.prototype.ShowErrorBut = function(index) {

        var str = "";
        var baseIndex = 0;
        this.errorButPageIndex = index;

        if (this.errorButList.length <= 0)
            return;

        var pagecout;
        if (this.errorButList.length % this.errorButPageSize > 0) {
            pagecout =parseInt( this.errorButList.length / this.errorButPageSize )+ 1;
        } 
        else {
            pagecout = parseInt(this.errorButList.length / this.errorButPageSize);
        }

        if (index > pagecout)
        {index = pagecout;}

        var j=this.errorButPageSize;

        if (this.errorButList.length < this.errorButPageSize * index) {
            j = this.errorButList.length - this.errorButPageSize * (index - 1);
        }

        baseIndex = this.errorButPageSize * (index - 1);

        for (var i = 0; i < j; i++) {
            str += this.errorButString.format(this.errorButList[baseIndex+i].id,this.errorButList[baseIndex+i].name);
        }

        if (str == "") {
         // 翻页按钮屏蔽
         $(".shortcut-abnormal-button-zone .button-prev").addClass("none");
         $(".shortcut-abnormal-button-zone .button-next").addClass("none");
        } else {
             $(".shortcut-abnormal-button-zone .button-prev").removeClass("none");
         $(".shortcut-abnormal-button-zone .button-next").removeClass("none");
        } 

        $(".shortcut-abnormal-button-zone ul").html(str);

        this.ErrorButInit();
        this.SetErrorButNva();
    };

    /** 初始化*/
    CustomUi.prototype.ErrorButInit = function() {
        var i;
        $(".shortcut-abnormal-button-zone ul li").unbind("touchend");
        $(".shortcut-abnormal-button-zone ul li").on("touchend",
            function(e) {
				e.preventDefault();
                i = {
                        action: "click",
                        data: {id: $(this).find("button").attr("id")}
                    };
                Config.send(i);
            }
        );
		
		$(".shortcut-abnormal-button-zone ul li").unbind("click");
        $(".shortcut-abnormal-button-zone ul li").on("click",
            function(e) {
				e.preventDefault();
                i = {
                        action: "click",
                        data: {id: $(this).find("button").attr("id")}
                    };
                Config.send(i);
            }
        );
    };

    /**设置翻页按钮*/
    CustomUi.prototype.SetErrorButNva = function() {

		$(".shortcut-abnormal-button-zone .button-prev").unbind("touchend");
        $(".shortcut-abnormal-button-zone .button-next").unbind("touchend");
		
		$(".shortcut-abnormal-button-zone .button-prev").unbind("click");
        $(".shortcut-abnormal-button-zone .button-next").unbind("click");
		
		
        var m_this = this;
        if (this.errorButPageIndex > 1) 
        {
            
            $(".shortcut-abnormal-button-zone .button-prev").removeClass("disabled");			
			$(".shortcut-abnormal-button-zone .button-prev").on("touchend",
                function() {
                    m_this.errorButPageIndex--;
                    m_this.ShowErrorBut(m_this.errorButPageIndex);
					
					if(m_this.KeyboardFlag && MenuFlag==1)
					{
						m_this.RemoveValueDataNNumber();
						m_this.SetErrorListValue();
						m_this.KeyboardFlag=false;
					}
                }
            );
			
			$(".shortcut-abnormal-button-zone .button-prev").on("click",
                function() {
                    m_this.errorButPageIndex--;
                    m_this.ShowErrorBut(m_this.errorButPageIndex);

					if(m_this.KeyboardFlag && MenuFlag==1)
					{
						m_this.RemoveValueDataNNumber();
						m_this.SetErrorListValue();
						m_this.KeyboardFlag=false;
					}
                }
            );
        } 
        else {
            $(".shortcut-abnormal-button-zone .button-prev").addClass("disabled");
        }


        if (this.errorButPageIndex*this.errorButPageSize<this.errorButList.length) {

            $(".shortcut-abnormal-button-zone .button-next").removeClass("disabled");
			$(".shortcut-abnormal-button-zone .button-next").on("touchend",
                function(e) {
					e.preventDefault();
                    m_this.errorButPageIndex++;
                    m_this.ShowErrorBut(m_this.errorButPageIndex);
					
					if(m_this.KeyboardFlag && MenuFlag==1)
					{
						m_this.RemoveValueDataNNumber();
						m_this.SetErrorListValue();
						m_this.KeyboardFlag=false;
					}

                }
            );
			
			$(".shortcut-abnormal-button-zone .button-next").on("click",
                function(e) {
					e.preventDefault();
                    m_this.errorButPageIndex++;
                    m_this.ShowErrorBut(m_this.errorButPageIndex);
					
					if(m_this.KeyboardFlag && MenuFlag==1)
					{
						m_this.RemoveValueDataNNumber();
						m_this.SetErrorListValue();
						m_this.KeyboardFlag=false;
					}

                }
            );
        } else {
            $(".shortcut-abnormal-button-zone .button-next").addClass("disabled");
        }
    };

	//按键响应
    CustomUi.prototype.KeyAction= function(key)
	{
		if(!checkCanUseTheKey(key))//检测按键是否可用 不可用返回
			return
		
	    var result=false;
		this.KeyboardFlag=true;
	    switch (key) {				
	    // case "ENTER":
		// return alert(111);
        // case "CANCEL":
		// return alert(222);
		case "1":			
			$("button[value='Select1']").click();
			return true;
			//this.keyBoardAction($("button[value='Select1']").attr('id'),key);
		case "2":
			$("button[value='Select2']").click();
			return true; 
		case "3":
			$("button[value='Select3']").click();
			return true; 
		case "4":
			$("button[value='Select4']").click();
			return true; 
		case "5":
			$("button[value='Select5']").click();
			return true; 
		case "6":
			$("button[value='Select6']").click();
			return true;  
		case "7":
			$("button[value='Select7']").click();
			return true;  
		case "8":
			$("button[value='Select8']").click();
			return true;  
		case "9":
			$("button[value='Select9']").click();
			return true;  
		case "ENTER":
			return this.keyBoardAction("KEYBOARD_ENTER",key);
        case "CANCEL":
			return this.keyBoardAction("FUN_QUIT",key);
	    case "KEYBOARD_TAB":
			return this.SwitchMenu();
        // 后续插入快捷键等等
        //.........
	    default:
	        return this.keyBoardAction("keyboard",key);
	    }
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
		
		bottomButton = $("#FUN_DEFINEMENU");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("4"+strTmp);
		bottomButton.val("Select4");
		
		return true;
	};
	
	//设置错误列表数字
	CustomUi.prototype.SetErrorListValue= function()
	{		
	    var result=false;
		//设置ul里边的置
		var liList=$(".shortcut-abnormal-button-zone ul li");
		var liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]).children("button").children("span"))[1];
			var spanText = $(span).text();
			$(span).text((i+1)+"、"+spanText);
			$(liList[i]).children("button").val("Select"+(i+1));
		}		
		
		//设置翻页按钮的值
		var buttonPre=$(".shortcut-abnormal-button-zone #ERROR_PRE_PAGE");
		var buttonNext=$(".shortcut-abnormal-button-zone #ERROR_NEXT_PAGE");	
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
	
	//设置快捷菜单列表数字
	CustomUi.prototype.SetShortcutListValue= function()
	{		
	    var result=false;		
		//设置ul里边的置
		var liList=$(".shortcut-function-button-zone ul li");
		var liListLength=liList.length;
		for(var i=0;i<liListLength;i++)
		{
			var span = ($(liList[i]).children("button").children("span"))[1];
			var spanText = $(span).text();
			$(span).text((i+1)+spanText);
			$(liList[i]).children("button").val("Select"+(i+1));
		}		
		
		
		//设置翻页按钮的值
		var buttonPre=$(".shortcut-function-button-zone #MENU_PRE_PAGE");
		var buttonNext=$(".shortcut-function-button-zone #MENU_NEXT_PAGE");	
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
	
	//清除value置和首字母数字
	CustomUi.prototype.RemoveValueDataNNumber= function()
	{		
	    var result=false;		
		//设置ul里边的置
		var liList=$(".shortcut-function-button-zone ul li");
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

		liList=$(".shortcut-abnormal-button-zone ul li");
		liListLength=liList.length;
		for(var j=0;j<liListLength;j++)
		{
			var span = ($(liList[j]).children("button").children("span"))[1];
			var spanText = $(span).text();
			
			var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num=spanText.substr(1,1);
			if(num=="、")
			{
				$(span).text(spanText.substr(2,spanText.length-1));
			}			
			// if(reg.test(num))
			// {
       		//	$(span).text(spanText.substr(2,spanText.length-1));
     		// }			
			
			$(liList[j]).children("button").val("");
		}			
		
		//设置翻页按钮的值
		var buttonPre=$(".shortcut-abnormal-button-zone #ERROR_PRE_PAGE");
		var buttonNext=$(".shortcut-abnormal-button-zone #ERROR_NEXT_PAGE");	
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
		
		buttonPre=$(".shortcut-function-button-zone #MENU_PRE_PAGE");
		buttonNext=$(".shortcut-function-button-zone #MENU_NEXT_PAGE");	
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
		
		bottomButton = $("#FUN_DEFINEMENU");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
    	}	
		bottomButton.val("");
		
		
		return true;
	};
	
	//切换功能
	CustomUi.prototype.SwitchMenu= function()
	{		
	    var result=false;
		this.RemoveValueDataNNumber();
		switch (MenuFlag) {		
		case 10:
			MenuFlag=0;
			return this.SetDefaultValue();	
	    case 0:		
			var liList=$(".shortcut-abnormal-button-zone ul li");
			var liListLength=liList.length;
			if(liListLength>0)
			{
				MenuFlag=1;
				return this.SetErrorListValue();				
			}
			else
			{
				MenuFlag=2;
				return this.SetShortcutListValue();				
			}		
	    case 1:
			var liList=$(".shortcut-function-button-zone ul li");
			var liListLength=liList.length;
			if(liListLength>0)
			{
				MenuFlag=2;
				return this.SetShortcutListValue();				
			}
			else
			{
				MenuFlag=0;
				return this.SetDefaultValue();				
			}		
	    case 2:
			MenuFlag=0;
			return this.SetDefaultValue();		
	    default:
			MenuFlag=0;
			return this.SetDefaultValue();	
	    }
	};	


	//--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);

var MenuFlag=10;

// 实例化
$(function() {
	new A();
});