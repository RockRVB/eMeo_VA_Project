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
		CustomUi.superClass.constructor.call(this);
	}

	Config.extend(CustomUi, Interface);

	//预处理
	CustomUi.prototype.preInit = function() {
		this.cleanData();
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
	//设置底部按钮数字
	CustomUi.prototype.SetBroadsideValue= function()
	{		
	    var result=false;
		var sideButtonList=$(".g-button-zone button");
		var sideListLength=sideButtonList.length;
		for(var i=0;i<sideListLength;i++)
		{
			var span = ($(sideButtonList[i]).children("span"))[1];
			var spanText = $(span).text();
			//console.log(spanText);
			$(span).text((i+1)+spanText);
			$(sideButtonList[i]).val("Select"+(i+1));
		}	
		return true;
	};	

	//设置快捷菜单列表数字
	CustomUi.prototype.SetButtomValue= function()
	{		
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
			//var span = $(sideButtonList[i]).children(".button-text");
			var span = ($(sideButtonList[i]).children("span"))[1];
			var spanText = $(span).text();
			console.log(spanText);
			$(span).text((i+5)+spanText);
			//$(sideButtonList[i]).attr("name","Select"+(i+1));
			$(sideButtonList[i]).val("Select"+(i+5));
		}		
		return true;
	};
/*	
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
			$(buttonSpan).text(5+buttonSpanText);
			buttonPre.val("Select5");

			buttonSpan= ($(buttonNext).children("span"))[1];
			buttonSpanText =$(buttonSpan).text();
			$(buttonSpan).text(6+buttonSpanText);
			buttonNext.val("Select6");
		}
		return true;
	};*/
	//按键响应
    CustomUi.prototype.KeyAction= function(key)
	{
		if(!checkCanUseTheKey(key))//检测按键是否可用 不可用返回
			return
		
	    var result=false;
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
		case "ENTER":return this.keyBoardAction("KEYBOARD_ENTER",key);
        case "CANCEL":return this.keyBoardAction("FUN_QUIT",key);
	    case "KEYBOARD_TAB":return this.SwitchMenu();
        // 后续插入快捷键等等
        //.........
	    default:
	        return this.keyBoardAction("keyboard",key);
	    }
	};

	//清除数字
	CustomUi.prototype.RemoveValueDataNNumber= function()
	{		
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
	};	
	
	//切换功能
	CustomUi.prototype.SwitchMenu= function()
	{		
	    var result=false;
		
		this.RemoveValueDataNNumber();
		switch (MenuFlag) {		
	    case 0:		
			var buttonList=$(".bottom-button-zone button");
			var buttonListLength=buttonList.length;
			if(buttonListLength>0)
			{
				MenuFlag=1;
				return this.SetDefaultValue();				
			}		
	    case 1:
			var buttonList=$(".g-button-zone button");
			var buttonListLength=buttonList.length;
			if(buttonListLength>0)
			{
				MenuFlag=0;
				return this.SetButtomValue();				
			}
	    default:
			MenuFlag=0;
			return this.SetDefaultValue();	
	    }
	};	
	
	//--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);

var MenuFlag=0;

// 实例化
$(function() {
	new A();
});