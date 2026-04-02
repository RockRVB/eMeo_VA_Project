/******************** 
	作用:系统初始化界面
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-13
********************/

(function(window) {

	function CustomUi() {

		/** 用于标记当前显示到了第几行信息,从1开始计算,取值为1-8 */
		this.showIndex = 1;

		/** 最多有多少行信息显示在界面上 */
		this.maxItems = 18;

		/** 错误码xml文件 */
		this.errorXml = null;

		CustomUi.superClass.constructor.call(this);
	}

	Config.extend(CustomUi, Interface);

	//预处理
	CustomUi.prototype.preInit = function() {
		this.cleanData();
		var url = "../../../error/ErrorCodeRange.xml";
		var self = this;
		//加载错误码xml
		$.ajax({
			url: url,
			dataType: 'xml',
			type: 'GET',
			timeout: 2000,
			error: function(xml) {
				Config.log("加载错误码范围XML文件出错!");
			},
			success: function(xml) {
				self.isDelayInit = true;
				self.errorXml = xml;
				self.init(); //初始化 在子类中初始化，父类延迟加载了。
			}
		});
	};

	//清除界面信息,用于从头开始显示信息
	CustomUi.prototype.cleanData = function() {
		$(".initialization-information").addClass('hideme');
		$(".initialization-content").empty();
		$(".initialization-error-code").empty();
		$(".initialization-warn").empty();
		$(".initialization-icon").removeClass('init-normal init-warn init-error');
	};
		//生成页面之后
	CustomUi.prototype.afterInitUi = function() {
		Config.Screen.changeState(Config.UI_INIT);
		//异步加载后台文本资源
		this.LoadMaintenanceTextResource();
	};
	
	//设置值
	CustomUi.prototype.setValue = function(data) {
		var result = true;
		try {			
			if (this.hasElement(data.id)) {				
				var index=this.showIndex - 1;
				//部件内容
				var selector = ".initialization-information:eq(" + index + ")";
				$(selector).removeClass('hideme');
				if (data.id.indexOf("STATIC_CONTENT") != -1) {
					var devname = data.id.substring(15,(data.id.length));				
					$(selector).addClass(devname);
					selector = ".initialization-content:eq(" + index + ")";
					$(selector).html(data.value);
					this.showIndex += 1;
				}
				//错误码			  
				else if (data.id.indexOf("STATIC_ERRCODE") != -1) 
				{
					var devname2 = data.id.substring(15,(data.id.length));
					var parentItem = "." + devname2;					
					if($(parentItem).children(".initialization-content").html()!="")
					{
					    //警告
					    if (this.warnErrorCode(Number(data.value))) {						    
						    $(parentItem).children(".initialization-icon").addClass('init-warn');						 
						    $(parentItem).children(".initialization-warn").html(data.value);
					    }
					    //正常
					    else if (Number(data.value) == 0) {						    
						    $(parentItem).children(".initialization-icon").addClass('init-normal');
					    }
					    //报错
					    else {						    
						    $(parentItem).children(".initialization-icon").addClass('init-error');					    
						    $(parentItem).children(".initialization-error-code").html(data.value);
					    }
						
					}
					
				}
			} else {
				result = false;
			}
		} catch (e) {
			result = false;
			Config.log(e);
		}
		return result;
	};

	//判断界面是否存在该id
	CustomUi.prototype.hasElement = function(id) {
		var result = true;
		try {
			result = (id.indexOf("STATIC_CONTENT") != -1 || id.indexOf("STATIC_ERRCODE") != -1);
		} catch (e) {
			result = false;
			Config.log(e);
		}
		return result;
	};

	//判断码值是否警告信息,如果码值在ErrorCodeRange.xml定义的 begin 和 end 之间,则是警告信息,否则不是警告信息
	CustomUi.prototype.warnErrorCode = function(code) {
		var result = false;
		try {
			var begin;
			var end;
			$(this.errorXml).find("range").each(function() {
				begin = Number($(this).attr("begin"));
				end = Number($(this).attr("end"));
				if (code >= begin && code <= end) {
					result = true;
					return false;
				}
			});
		} catch (e) {
			result = false;
			Config.log(e);
		}
		return result;
	};
	
	//加载后台文本资源文件
	CustomUi.prototype.LoadMaintenanceTextResource = function() {
		var url = getRootPath_web() +  "/Text/" + Config.Lang + "/Maintenance.xml";
		//加载错误码xml
		$.ajax({
			url: url,
			dataType: 'xml',
			type: 'GET',
			timeout: 2000,
			error: function(xml) {
				Config.log("加载后台文本资源XML文件出错!");
			},
			success: function(xml) {
				Config.MaintenanceTextResource = xml;
			}
		});
	};

	//--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);


// 实例化
$(function() {
	new A();
});