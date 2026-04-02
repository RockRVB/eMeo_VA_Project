/******************** 
	作用:界面配置
	作者:adam
	版本:V1.0
	时间:2015-05-22
********************/

(function(window) {

	var UIPageConfig=
	{
		

		uiPrefix:"../../config/maintenance",
		uiSuffix:"/UIMapping.xml",
		//获取UI对应的资源配置文件路径
		getUIMappingResouce:function(){
			var result=UIPageConfig.uiPrefix+UIPageConfig.uiSuffix;
			return result;
		}
	};

	//--------------------------------
	if (window.UIPageConfig == undefined) {
		if (window.top.UIPageConfig == undefined)
			window.UIPageConfig = UIPageConfig;
		else
			window.UIPageConfig = window.top.UIPageConfig;
	}
	//--------------------------------
})(window);