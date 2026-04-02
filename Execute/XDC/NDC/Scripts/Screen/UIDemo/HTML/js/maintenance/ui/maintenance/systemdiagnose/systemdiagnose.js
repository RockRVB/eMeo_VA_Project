/******************** 
	作用:系统自动诊断
	作者:adam
	版本:V1.0
	时间:2015-05-25
********************/

(function(window) {

	function CustomUi() 
	{
	    this.InvalSeparator ='\x1B';
		this.LoadingText="";
		this.DeviceError="";
		this.UserError="";
		this.Unknow="";
		this.NoDevice="";
		this.Color="orange";
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

	//清除界面信息,用于从头开始显示信息
	CustomUi.prototype.cleanData = function() {
	};

	//界面生成后,重写预留的接口
	CustomUi.prototype.afterInitUi= function() {
		try{
			//设备名称和线条是否显示
			if($('#atm-box').attr('devNameAndLine')=="1"){
				$('.atm-device-name').removeClass('none');
				$('.atm-device-line').removeClass('none');
			}else{
				$('.atm-device-name').addClass('none');
				$('.atm-device-line').addClass('none');
			}

			//调整样式
			atmBox = $("#atm-box");
			atmBox.attr("class","atm-"+Config.TWIN_ATM);
			
			if($('#atm-box').attr('imgType')=="swf"){
				//刷新swf机型
				$('#atm-flash > embed').remove();
				atmSwf = document.getElementById('atm-flash');
				htmlSrc = '<embed class="atm-swf" wmode="opaque" id="atmSWF" src="..\\..\\..\\..\\Image\\CN\\maintenance\\atm\\'+Config.TWIN_ATM+'\\'+Config.TWIN_ATM+'.swf" > </embed>';
				atmSwf.innerHTML = htmlSrc;
				//隱藏圖片
				$('#atm-img').addClass("none");
			}else if($('#atm-box').attr('imgType')=="img"){
				//设置机型图片
				imgScr = '..\\..\\..\\..\\Image\\CN\\maintenance\\atm\\'+Config.TWIN_ATM+'\\'+Config.TWIN_ATM+'.png';
				$('#atm-img').attr("src",imgScr);
				//显示png机型
				$('#atm-pic').removeClass("none");
				$('#atm-img').removeClass("none");
				//隱藏Flash
				$('#atmSWF').addClass("none");
			}
		}catch(e){
			Config.log(e);
		}
	},

	//重写方法设置值
	CustomUi.prototype.setValue = function(data) {
		var result = true;
		try 
        {
		    if(data.id.indexOf('STATIC_CASHBOX')>=0||data.id=="forpagerowcount")
			{
				result=this.MySetBoxInfo(data);
			}
			else
            result=this.MySetValue(data);
			
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
	    var device = "";
		if (this.hasElement(data.id)) 
        {
			var widget = this.getElementByName(data.id);
			if (widget == null) {
				result = false;
			} else {
				if (widget.type != "raw") 
                {
                    //编写逻辑 
                    device = data.id.replace("STATIC_", "").toLowerCase();
                    var deviceInfo = data.value.split(this.InvalSeparator);
                    widget.element.element.parent().removeClass("none");
					
					//设置左边图示设备信息
                    var div = $(".single-device-" + device);
					var img = $(".atm-device-state", div);
					
                    if (deviceInfo.length > 1) {
                        var v = deviceInfo[1];
                        // 设置右边设备信息
                        
                        div.removeClass("none");
                        img.removeClass("none");
                        if (deviceInfo[0] == "N") {
                            widget.element.setValue(deviceInfo[1], Config.CMD_ATTRIBUTE_TEXT);
							img.removeClass("atm-device-state-warning atm-device-state-error");
                            img.addClass("atm-device-state-normal");
							widget.element.element.removeClass("atm-device-color-error atm-device-color-warning");
							widget.element.element.addClass("atm-device-color-normal");
							widget.element.changeColor("green");
                        }
                        else if (deviceInfo[0] == "W") 
                        {
							widget.element.changeColor("orange");
							widget.element.setValue(deviceInfo[1], Config.CMD_ATTRIBUTE_TEXT);
							img.removeClass("atm-device-state-normal atm-device-state-error");
                            img.addClass("atm-device-state-warning");
							
							widget.element.element.removeClass("atm-device-color-error atm-device-color-normal");
							widget.element.element.addClass("atm-device-color-warning");
                        } 
                        else //错误码
						{
							widget.element.options.prefix=deviceInfo[0];
							widget.element.changeColor("red");
							widget.element.setValue(deviceInfo[0]+" "+deviceInfo[1], Config.CMD_ATTRIBUTE_TEXT);
							img.removeClass("atm-device-state-normal atm-device-state-warning");
                            img.addClass("atm-device-state-error");
							
							widget.element.element.removeClass("atm-device-color-normal atm-device-color-warning");
							widget.element.element.addClass("atm-device-color-error");
                        }
						
                        widget.element.show();
                    } 
                    else 
                    {
						div.removeClass("none");
                        img.removeClass("none");
						img.removeClass("atm-device-state-normal atm-device-state-warning");
                        img.addClass("atm-device-state-error");
						if(this.LoadingText=="")
						this.LoadingText=this.GetMaintenanceText("Loading")
						if(data.value==this.LoadingText)
						{
							widget.element.element.removeClass("atm-device-color-error");
							widget.element.element.addClass("atm-device-color-normal");
							widget.element.changeColor("green");
						}
						else
						{
							widget.element.changeColor("red");
							widget.element.element.removeClass("atm-device-color-normal atm-device-color-warning");
							widget.element.element.addClass("atm-device-color-error");
						}
						
                        widget.element.setValue(data.value, Config.CMD_ATTRIBUTE_TEXT);
                        widget.element.show();
                    }

				} else 
                {
					if (widget.functionType == "html")
						widget.element.html(data.value);
					else
						widget.element.val(data.value);
				}
			}
		} 

        else 
		{
         
		    if(this.getChildUiInstance()!=undefined)
			{
				result=	this.getChildUiInstance().setValue(data);
			    // 往子页面找控件处理
			}
		}

		return result;
	};
	
	
	//查找后台文本资源
	CustomUi.prototype.GetMaintenanceText = function(text) {
	var result = "";
		try {
			 if (text==""||text==undefined)
			  return result;
			 if(Config.MaintenanceTextResource==null)
			 return result;
			 var key;
			$(Config.MaintenanceTextResource).find("item").each(function() {
				key=$(this).attr("key");
				if (key==text) {
					result = $(this).attr("value");
					return false;
				}
			});
		} catch (e) {
			Config.log(e);
		}
		return result;
	};
	//
	//设置钱箱信息
	CustomUi.prototype.MySetBoxInfo= function(data)
	{
		try
		{
			if(data.id=="forpagerowcount")
			{
				var result=false;
				if (this.hasElement("table_1")) 
				{
					var widget = this.getElementByName("table_1");
					if (widget == null) 
					{
						result = false;
					} 
					else 
					{
						if (widget.type != "raw") 
						{
						var count=parseInt(data.value)
						if (isNaN(count)) {
	                    Config.log("row Count " + data.value + " is error");
	                    return false;
						}

						if (widget.element.options.maxRowCount< count) //当实际行数少于最大行数时，为了避免操作按钮往上挤，实际行数以最大行数为准
						{
						// 设置行数
						widget.element.options.rowCount = parseInt(data.value);
						}
						// 设置完行数之后需要初始化
						widget.element.init();
					
						} else 
						{
							//if (widget.functionType == "html")
							//widget.element.html(data.value);
							//else
							//widget.element.val(data.value);
						}
					}

					result = true;
				}
				
				return result;
				
			}
			else
			{
				return this.SetUnitInfo(data);
			}
			
		} 
		catch(e)
		{
			Config.log(e);
		}
	};
	
	// 设置具体钱箱信息
	CustomUi.prototype.SetUnitInfo= function(data)
	{
		try
		{
			if(data.id.indexOf('STATIC_CASHBOXSTATUS')>=0)
			{
				//取当前字体颜色
				this.GetUnitColor(data);
				// 修正字体颜色
			    this.SetUnitColor(data);
			}
			
			if(data.id.indexOf('STATIC_CASHBOXTYPE')>=0)
			{
				data.value= this.GetUnitType(data.value);
			}
			
			// 设值
			var widget = this.getElementByName(data.id);
			if (widget == null) 
			{
				return false;
			} else 
			{
				if (widget.type != "raw") 
                {
					widget.element.setValue(data.value, Config.CMD_ATTRIBUTE_TEXT);
					widget.element.changeColor(this.Color);
					widget.element.show();
				}
				else
				{
					if (widget.functionType == "html")
						widget.element.html(data.value);
					else
						widget.element.val(data.value);
				}
			}
		}
		catch(e)
		{
			Config.log(e);
		}
	};
	// 根据钱箱类型码获取钱箱中文描述
	CustomUi.prototype.GetUnitType= function(code)
	{
	try{
			switch(code)
			{
				case '240': return Config.UnitConfigs.widthdrawalKey;//'只取';
				case '255' : return Config.UnitConfigs.cashInKey;//'只存';
				case '0' : return Config.UnitConfigs.recylingKey;//'循环';
				case '2': return Config.UnitConfigs.retractKey;//'回收箱';
				case '3': return Config.UnitConfigs.rejectKey;//'回收箱';
                default : return Config.UnitConfigs.unknowKey;//'未知';				
			}
		}
		catch(e)
		{
			Config.log(e);
			return "unknow";
		}
		
	
	};
	//设置钱箱信息行颜色
	CustomUi.prototype.GetUnitColor= function(data)
	{
		try
		{
			var color='orange';
			switch(data.value)
			{
				case 'OK': 
				case 'FULL': 
				case 'HIGH': color='green';break;
				case 'LOW': color='orange';break; // 低
				case 'MISSING':  //不存在
				case 'EMPTY':  // 空
				case 'INOP':  //不可用
				case 'NOVAL': color='red';break; //面额不匹配
				default: break;
			}
			// 设置全局变量颜色
			this.Color= color;
		}
		catch(e)
		{Config.log(e); return false;}
		
	};
	
	//修正钱箱状态颜色
	CustomUi.prototype.SetUnitColor= function(data)
	{
		try
		{
			var index= data.id.replace('STATIC_CASHBOXSTATUS','');
			var widget1 = this.getElementByName(data.id);
			var widget2 = this.getElementByName('STATIC_CASHBOXID'+index);
			var widget3 = this.getElementByName('STATIC_CASHBOXCOUNT'+index);
			var widget4 = this.getElementByName('STATIC_CASHBOXDOM'+index);
			var widget5 = this.getElementByName('STATIC_CASHBOXTYPE'+index);
			var widget6 = this.getElementByName('STATIC_CASHBOXCURRENCY'+index);
			
			widget1.element.changeColor(this.Color);
			widget2.element.changeColor(this.Color);
			widget3.element.changeColor(this.Color);
			widget4.element.changeColor(this.Color);
			widget5.element.changeColor(this.Color);
			widget6.element.changeColor(this.Color);
		}
		catch(e)
		{
			Config.log(e); 
			return false
		}
	}
	
	//--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);


// 实例化
$(function() {
	new A();
});