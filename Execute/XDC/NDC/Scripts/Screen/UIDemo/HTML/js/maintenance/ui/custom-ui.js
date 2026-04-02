/******************** 
	作用:自定义界面类
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-13
********************/

(function(window) {

	function CustomUi() {
		CustomUi.superClass.constructor.call(this);
	}

	Config.extend(CustomUi, Interface);

	//--------------------------------
	window.CustomUi = CustomUi;
	//--------------------------------
})(window);
//实例化
$(function() {
	new CustomUi();
});