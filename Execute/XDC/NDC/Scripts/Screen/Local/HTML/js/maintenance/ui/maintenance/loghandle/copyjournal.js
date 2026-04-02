/******************** 
	作用:日志操作拷贝流水
	作者:adam
	版本:V1.0
	时间:2015-09-14
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
            if (data.id == "INPUT_DATE_BEGIN") 
            {
                result=this.MySetValue(data);   
            }
            else if (data.id == "INPUT_DATE_END") 
            {
                result=this.MySetValue2(data);
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
	
    //重写方法取值
	CustomUi.prototype.getValue = function(data) {
		var result = "";
		try 
        {
            if (data == "INPUT_DATE_BEGIN") 
            {
                result=this.MyGetValue(data);   
            }
            else if (data == "INPUT_DATE_END") 
            {
                result=this.MyGetValue2(data);
            } 
            else 
            {
              result= CustomUi.superClass.getValue.call(this, data);
            }
			
		} catch (e) {
			result = "";
			Config.log(e);
		}

		return result;
	};


	// 私有方法 设值
	CustomUi.prototype.MySetValue= function(data)
	{
	    var result=false;
        //     
		if (this.hasElement("INPUT_SYEAR")
            &&this.hasElement("INPUT_SMONTH")
            &&this.hasElement("INPUT_SDAY")
            ) 
        {
			var widget = this.getElementByName("INPUT_SYEAR");
            var widget1 = this.getElementByName("INPUT_SMONTH");
            var widget2 = this.getElementByName("INPUT_SDAY");
			if (widget == null||widget1 == null||widget2 == null) {
				result = false;
			} 
            else 
            {
				if (widget.type != "raw"
                    &&widget1.type != "raw"
                    &&widget2.type != "raw"
                    ) 
                {
                    if (data.value != undefined&&data.value.length==8) 
                    {
                        widget.element.setValue(data.value.substr(0,4) , Config.CMD_ATTRIBUTE_TEXT);
                        widget1.element.setValue(data.value.substr(4,2) , Config.CMD_ATTRIBUTE_TEXT);
                        widget2.element.setValue(data.value.substr(6,2) , Config.CMD_ATTRIBUTE_TEXT);
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

                    result = true;
                }
			}
		} 
		return result;
	};
	// 私有方法 取值
	CustomUi.prototype.MyGetValue= function()
	{
	    var result="";
		if (this.hasElement("INPUT_SYEAR")
            &&this.hasElement("INPUT_SMONTH")
            &&this.hasElement("INPUT_SDAY")) 
        {
			var widget = this.getElementByName("INPUT_SYEAR");
            var widget1 = this.getElementByName("INPUT_SMONTH");
            var widget2 = this.getElementByName("INPUT_SDAY");

			if (widget == null||widget1 == null||widget2 == null) {
				result = false;
			} else {
				if (widget.type != "raw"
                    &&widget1.type != "raw"
                    &&widget2.type != "raw") 
                {

                    result = widget.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget1.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget2.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
                }
			}
		} 
		return result;
	};
	
    	// 私有方法 设值
	CustomUi.prototype.MySetValue2= function(data)
	{
	    var result=false;
        //     
		if (this.hasElement("INPUT_EYEAR")
            &&this.hasElement("INPUT_EMONTH")
            &&this.hasElement("INPUT_EDAY")) 
        {
			var widget = this.getElementByName("INPUT_EYEAR");
            var widget1 = this.getElementByName("INPUT_EMONTH");
            var widget2 = this.getElementByName("INPUT_EDAY");

			if (widget == null||widget1 == null||widget2 == null) {
				result = false;
			} 
            else 
            {
				if (widget.type != "raw"
                    &&widget1.type != "raw"
                    &&widget2.type != "raw") 
                {
                    if (data.value != undefined&&data.value.length==8) 
                    {
                        widget.element.setValue(data.value.substr(0,4) , Config.CMD_ATTRIBUTE_TEXT);
                        widget1.element.setValue(data.value.substr(4,2) , Config.CMD_ATTRIBUTE_TEXT);
                        widget2.element.setValue(data.value.substr(6,2) , Config.CMD_ATTRIBUTE_TEXT);
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

                    result = true;
                }
			}
		} 
		return result;
	};
	// 私有方法 取值
	CustomUi.prototype.MyGetValue2= function()
	{
	    var result="";
		if (this.hasElement("INPUT_EYEAR")
            &&this.hasElement("INPUT_EMONTH")
            &&this.hasElement("INPUT_EDAY")) 
        {
			var widget = this.getElementByName("INPUT_EYEAR");
            var widget1 = this.getElementByName("INPUT_EMONTH");
            var widget2 = this.getElementByName("INPUT_EDAY");

			if (widget == null||widget1 == null||widget2 == null) {
				result = false;
			} else {
				if (widget.type != "raw"
                    &&widget1.type != "raw"
                    &&widget2.type != "raw") 
                {

                    result = widget.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget1.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget2.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
                }
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