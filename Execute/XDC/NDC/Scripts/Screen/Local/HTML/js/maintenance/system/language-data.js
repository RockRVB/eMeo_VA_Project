/******************** 
	作用:用于根据不同语言获取相应的文本资源
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-08
********************/

(function(window) {

	function LanguageData() {}

	//按key保存值,用于加快查找速度
	LanguageData.valueObject = {};

	//是否已加载xml文件(true:已加载 false:未加载)
	LanguageData.loaded = false;

	//初始化xml文件
	LanguageData.init = function() {
		try {
			Config.LanguageData=LanguageData;
			$.ajax({
				url: LanguageConfig.getLanguageResouce(),
				dataType: 'xml',
				type: 'GET',
				timeout: 2000,
				async:false,
				error: function(xml) {
					Config.log("加载语言XML文件出错!");
				},
				success: function(xml) {
					//保存对应的中英文资源
					$(xml).find("item").each(function(i) {
						var key = $(this).attr("key").toUpperCase();
						var value = $(this).attr("value");
						// console.log(key+":"+value);
						LanguageData.valueObject[key] = value;
					});
					LanguageData.loaded = true;
					UIPageMappingData.init();
					// 语言资源加载成功后就需要呈现界面了
					Config.Screen.createInterface(); //生成界面
				}
			});
		} catch (e) {
			Config.log(e);
		}

	}

	//获取相应的资源
	LanguageData.get = function(key) {
		key=key.toUpperCase();
		return (LanguageData.valueObject[key] == undefined ? "" : LanguageData.valueObject[key]);
	}

	//--------------------------------
	if (window.LanguageData == undefined) {
		if (window.top.LanguageData == undefined)
			window.LanguageData = LanguageData;
		else
			window.LanguageData = window.top.LanguageData;
	}
	//--------------------------------
})(window);