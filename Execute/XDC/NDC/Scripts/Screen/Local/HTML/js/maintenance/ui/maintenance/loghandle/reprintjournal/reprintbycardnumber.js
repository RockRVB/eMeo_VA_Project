/******************** 
	作用:按卡号补打流水
	作者:Adam
	版本:V1.0
	时间:2016-08-26
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
            if (data.id == "INPUT_DATE_BEGIN"||data.id == "INPUT_DATE_END") 
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
	
    //重写方法取值
	CustomUi.prototype.getValue = function(data) {
		var result = "";
		try 
        {
            if (data== "INPUT_DATE_BEGIN"||data== "INPUT_DATE_END") 
            {
                result=this.MyGetValue(data);
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
		var head="S";
		if(data.id=="INPUT_DATE_BEGIN")
		{
			head="S";
		}
		else
			head="E";
        //     
		if (this.hasElement("INPUT_"+head+"YEAR")
            &&this.hasElement("INPUT_"+head+"MONTH")
            &&this.hasElement("INPUT_"+head+"DAY")
            ) 
        {
			var widget = this.getElementByName("INPUT_"+head+"YEAR");
            var widget1 = this.getElementByName("INPUT_"+head+"MONTH");
            var widget2 = this.getElementByName("INPUT_"+head+"DAY");

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
	CustomUi.prototype.MyGetValue= function(data)
	{
		var head="S";
		if(data=="INPUT_DATE_BEGIN")
		{
			head="S";
		}
		else
			head="E";
			
	    var result="";
		if (this.hasElement("INPUT_"+head+"YEAR")
            &&this.hasElement("INPUT_"+head+"MONTH")
            &&this.hasElement("INPUT_"+head+"DAY")) 
        {
			var widget = this.getElementByName("INPUT_"+head+"YEAR");
            var widget1 = this.getElementByName("INPUT_"+head+"MONTH");
            var widget2 = this.getElementByName("INPUT_"+head+"DAY");

			if (widget == null||widget1 == null||widget2 == null) {
				result = false;
			} else {
				if (widget.type != "raw"
                    &&widget1.type != "raw"
                    &&widget2.type != "raw") 
                {
					
					monthTmp = widget1.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
					dateTmp = widget2.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
					
					if(Number(monthTmp) >= 1 && Number(monthTmp) <= 9 && monthTmp.length < 2)
					{
						monthTmp = "0" + monthTmp;
						widget1.element.setValue( monthTmp , Config.CMD_ATTRIBUTE_TEXT);
					}
					if(Number(dateTmp) >= 1 && Number(dateTmp) <= 9 && dateTmp.length < 2)
					{
						dateTmp = "0" + dateTmp;
						widget2.element.setValue( dateTmp , Config.CMD_ATTRIBUTE_TEXT);
					}

                    result = widget.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget1.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget2.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
                    if (result.length != 8) 
                    {
                        result = "";
                    }
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