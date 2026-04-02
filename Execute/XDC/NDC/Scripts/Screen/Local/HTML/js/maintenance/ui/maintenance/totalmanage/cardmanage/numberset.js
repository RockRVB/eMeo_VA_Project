/******************** 
	作用:系统自动诊断
	作者:adam
	版本:V1.0
	时间:2016-09-22
********************/

(function(window) {

	function CustomUi() 
	{
	    this.InvalSeparator ='\x1B';
		this.trhelperObj;
		CustomUi.superClass.constructor.call(this);
	}

	Config.extend(CustomUi, Interface);

	//预处理
	CustomUi.prototype.preInit = function() {
		this.cleanData();
		this.isDelayInit = true;
		var self = this;
		self.init();
	};
	
	CustomUi.prototype.afterInitUi=function()
	{
		//this.InitCtol();
	};

	//清除界面信息,用于从头开始显示信息
	CustomUi.prototype.cleanData = function() {
	};

	CustomUi.prototype.InitCtol= function()
	{
		this.trhelperObj= new trhelper(this);
		this.trhelperObj.init();
		
		$("#FUN_1").unbind("click");
        $("#FUN_2").unbind("click");
		var self=this;
		$("#FUN_1").click(
			function(){
				self.trhelperObj.add();
			}
		);
		
		$("#FUN_2").click(
			function(){
				self.trhelperObj.del();
			}
		);
		
	}

	//--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);


// 实例化
$(function() {
	new A();
});