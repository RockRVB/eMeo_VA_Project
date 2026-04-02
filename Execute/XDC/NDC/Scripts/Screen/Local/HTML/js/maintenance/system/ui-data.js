/******************** 
	作用:用于存储界面页面资源路径映射关系
	作者:adam
	版本:V1.0
	时间:2015-05-22
********************/

(function(window) {

	function UIPageMappingData() {}

	//按key保存值,用于加快查找速度
	UIPageMappingData.valueObject = {};

	//是否已加载xml文件(true:已加载 false:未加载)
	UIPageMappingData.loaded = false;

	//初始化xml文件
	UIPageMappingData.init = function() {
		try {
			Config.UIPageMappingData=UIPageMappingData;
			
			$.ajax({
				url: UIPageConfig.getUIMappingResouce(),
				dataType: 'xml',
				crossDomain: true,
				async:false,
				type: 'GET',
				timeout: 2000,
				error: function(xml) {
					Config.log("加载UIMapping XML文件出错!");
				},
				success: function(xml) 
				{
					var urlprefix="";
					var host="";
					var port="";
					var htmlpath ="";
					var rootitem=$(xml).find("config:eq(0)");
					urlprefix=rootitem.attr("urlprefix");
					host=rootitem.attr("host");
					port=rootitem.attr("port");
					htmlpath=rootitem.attr("htmlpath");
					
					var httphead=urlprefix+host+":"+port+"/"+htmlpath;
					
					//保存对应的中英文资源
					$(xml).find("item").each(function(i) 
					{
						var singleResult = 
						{
							"url": "",
							"iscontainer": true,
							"comment":""
						};
						var key=$(this).attr("ui").toUpperCase();
						singleResult.url =httphead+"/"+ $(this).attr("url");
						singleResult.iscontainer = $(this).attr("iscontainer")==1?true:false;
						singleResult.comment = $(this).attr("comment");
						console.log("add uimapping dic"+key+":"+singleResult);
						UIPageMappingData.valueObject[key] = singleResult;
					});
					UIPageMappingData.loaded = true;
				}
			});
		} catch (e) {
			Config.log(e);
		}

	}

	//获取相应的资源
	UIPageMappingData.get = function(key) {
		key=key.toUpperCase();
		return (UIPageMappingData.valueObject[key] == undefined ? UIPageMappingData.valueObject["DEFAULT"] : UIPageMappingData.valueObject[key]);
	}

	//--------------------------------
	if (window.UIPageMappingData == undefined) {
		if (window.top.UIPageMappingData == undefined)
			window.UIPageMappingData = UIPageMappingData;
		else
			window.UIPageMappingData = window.top.UIPageMappingData;
	}
	//--------------------------------
})(window);