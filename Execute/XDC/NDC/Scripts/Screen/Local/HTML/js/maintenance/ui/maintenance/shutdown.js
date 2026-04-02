/******************** 
	作用:设置钱箱张数
	作者:liaoyuyu
	版本:V1.0
	时间:2015-06-02
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
	/*
	CustomUi.prototype.setValue = function(data) {
		var result = true;
		try 
        {
			for(i=0;i<9;i++) {
				if (data.id == ("STATIC_CASSETID"+i)) {
					result = this.MySetValue(data);
				}
				if (data.id == ("STATIC_CURRENCY"+i)) {
					result = this.MySetValue(data);
				}
				if (data.id == ("STATIC_DENO"+i)) {
					result = this.MySetValue(data);
				}

			}
		} catch (e) {
			result = false;
			Config.log(e);
		}

		return result;
	};
	*/
	/*
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
                    widget.element.setValue(data.value, Config.CMD_ATTRIBUTE_TEXT);
                    widget.element.element.parent().parent(".nullClass").removeClass("none");
                    widget.element.show();
                } else 
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
	*/

	//--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);


// 实例化
$(function() {
	new A();
});