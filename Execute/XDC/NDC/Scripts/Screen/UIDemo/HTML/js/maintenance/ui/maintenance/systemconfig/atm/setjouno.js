/******************** 
	作用:系统环境诊断
	作者:liaoyuyu
	版本:V1.0
	时间:2015-06-03
********************/

(function(window) {

	function CustomUi() 
	{
	    this.InvalSeparator ='\x1B';
		
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

	//重写方法设置值
	CustomUi.prototype.setValue = function(data) {
		var result = true;
		try 
        {
            if (data.id == "STATIC_CURRENT_SERIAL_LEN") 
            {
                result=this.MySetValue(data);
            } 
            else {
                CustomUi.superClass.setValue.call(this, data);
            }
			
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
		if (this.hasElement(data.id)) 
        {
			var widget = this.getElementByName(data.id);
			if (widget == null) {
				result = false;
			} else {
				if (widget.type != "raw") 
                {
                    if (data.value != undefined) 
                    {
                       
                        widget.element.setValue(data.value , Config.CMD_ATTRIBUTE_TEXT);
                        //设置输入框输入的最大长度
                        $("#INPUT_SERAL_NUM").attr("maxlength", data.value);
                        result = true;
                    } 
                    else 
                    {
                        return false;
                    }
                } 
                else 
                {
					if (widget.functionType == "html")
						widget.element.html(data.value);
					else
						widget.element.val(data.value);
				}

			    result = true;
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
	

	//--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);


// 实例化
$(function() {
	new A();
});