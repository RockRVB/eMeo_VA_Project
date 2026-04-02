/******************** 
	作用:控件管理
	作者:蔡俊雄
	版本:V1.0
	时间:2015-04-22
********************/

(function(window) {

	function WidgetMangage() {}

	//生成界面中的所有控件
	WidgetMangage.init = function(uiInstance) {
		try {
			//获取界面中的所有ids资源
			if (Config.LanguageData != null) {
				$("[ids]").each(function(index) {
					var key = $(this).attr("ids").toUpperCase();
					var value = Config.LanguageData.get(key);
					// console.log(key + ":" + value);
					$(this).html(value);
				});
			}
			//生成控件
			var container = {
				uiInstance: uiInstance
			};
			$("[type=STATIC_TXT]").Text(container); //初始化静态文本
			$("input[type=text]").InputText(container); //初始化文本输入框
			$("input[type=password]").InputText(container); //初始化密码输入框
			$("button[type=button]").Button(container); //初始化按钮
			$("[type=TEXTBOX]").Textbox(container); //初始化滚动文本框
			$("[type=IMAGE]").Image(container); //初始化图片
			$("[type=KEYBOARD_NUMBER]").KeyboardNumber(container); //初始化数字键盘
			$("[type=KEYBOARD_NUMBER_LOGIN]").KeyboardNumberLogin(container); //初始化数字键盘
			$("[type=KEYBOARD]").Keyboard(container); //初始化键盘
			$("[type=TABLE]").Table(container); //初始化表格
			$("[type=COMPONENT]").Component(container); //初始化基类控件   add by adam 20150622
			$("[type=Audio]").Audio(container); //初始化声音播放控件控件   add by adam 20150918
			$("[type=BaseComponent]").BaseComponent(container); //初始化BaseComponent类控件   add by adam 20151010
			
		} catch (e) {
			Config.log(e);
		}
	}

	//获取父级
	WidgetMangage.getParent = function() {

	}

	//--------------------------------
	// if (window.WidgetMangage == undefined) {
	// 	if (window.top.WidgetMangage == undefined)
	// 		window.WidgetMangage = WidgetMangage;
	// 	else
	// 		window.WidgetMangage = window.top.WidgetMangage;
	// }
	window.WidgetMangage = WidgetMangage;
	//--------------------------------
	// WidgetMangage.init();
})(window);