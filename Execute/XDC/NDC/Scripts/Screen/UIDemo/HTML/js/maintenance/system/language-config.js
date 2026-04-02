/******************** 
	作用:语言配置
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-08
********************/

(function(window) {

	var LanguageConfig={
		/** 系统使用的语言 */
		language:"EN",
		/** 错误声音所在的文件夹 */
		errorSoundFolder:"res/sound/errorSound/",

		languagePrefix:"../../../Text/",
		languageSuffix:"/Flash.xml",
		//获取语言所对应的资源配置文件路径
		getLanguageResouce:function(){		
			LanguageConfig.language = Config.Lang = Common.getLanguage();
			var result=LanguageConfig.languagePrefix+LanguageConfig.language+LanguageConfig.languageSuffix;
			return result;
		}
		
	};

	//--------------------------------
	if (window.LanguageConfig == undefined) {
		if (window.top.LanguageConfig == undefined)
			window.LanguageConfig = LanguageConfig;
		else
			window.LanguageConfig = window.top.LanguageConfig;
	}
	//--------------------------------
})(window);