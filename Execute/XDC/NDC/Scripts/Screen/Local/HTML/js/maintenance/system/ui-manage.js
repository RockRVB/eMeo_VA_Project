/******************** 
	作用:界面切换管理
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-11
********************/

(function(window) {

	function UiManage() {}

	//主屏界面名称
	UiManage.currentModeUI = "";
	//子界面名称
	UiManage.currentChildUI = "";
	//用于维护界面清屏时是否向服务器发送"FUN_BACK"信息(1:发送 0:不发送)
	UiManage.bTempSelfDef = 0;

	//-----------------------清屏类型

	//用于清除主屏界面元素
	UiManage.TYPE_MODE = 1;
	//用于清除子界面元素
	UiManage.TYPE_CHILD = 2;
	//用于清除所有界面元素
	UiManage.TYPE_ALL = 3;
	//用于清除提示框
	UiManage.TYPE_PROMPT = 4;
	//用于清除流程子界面元素
	UiManage.TYPE_MODE = 5;
	
	UiManage.getUiObject = function (uiName)
	{
		var uiObject;
		try 
		{
			if(Config.UIPageMappingData!=undefined)
			{
				uiObject=UIPageMappingData.get(uiName);
			}
		} catch (e) {
			Config.log(e);
		}
		
		return uiObject;
	}

	//判断给定的界面名称是否为主界面
	UiManage.isContainer = function(uiName) {
		var result = false;
		try 
		{
			var uiObject =UiManage.getUiObject(uiName);
			
			if(uiObject!=undefined)
			{
				result=uiObject.iscontainer;
			}
		} catch (e) {
			Config.log(e);
		}
		return result;
	}

	//获取子界面容器（获取第一级ifram 中的 ifram ）
	UiManage.getChildContainer = function() {
		if(UiManage.getUiInstance()!=undefined)
		{
			return UiManage.getUiInstance().getContainer();
		}
		//没有子界面则 返回父业面容器
		return UiManage.getContainer();
		
	}

	//获取父界面容器(获取Index.html 页面中的Ifarm )
	UiManage.getContainer = function() {
		return $("#top-container");
	}

	//获取父界面容器对应的(子页面的界面实例interface.js）实例
	UiManage.getUiInstance = function() {
		return $("#top-container")[0].contentWindow.uiInstance;
	}
	
	
	

	// 页面切换
	UiManage.changeUi = function(ui) {
		var result = false;
		try {
			var isMode=UiManage.isContainer(ui);
			
			var url;
			var object =UiManage.getUiObject(ui);
			if(object!=undefined)
			{
				url=object.url;
				url+="?dt="+Math.random()+"&ui="+ui;
				var container;
				if(isMode)
				{
					container=UiManage.getContainer()
				}
				else
				{
					if(UiManage.getChildContainer()!=undefined)
					{
						container=UiManage.getChildContainer();
					}
					else
						container=UiManage.getContainer();
					
				}
				container.attr("src",url);
			}
			
			
		} catch (e) {
			Config.log(e);
		}
		return result;
	}

	//--------------------------------
	if (window.UiManage == undefined) {
		if (window.top.UiManage == undefined)
			window.UiManage = UiManage;
		else
			window.UiManage = window.top.UiManage;
	}
	//--------------------------------
})(window);