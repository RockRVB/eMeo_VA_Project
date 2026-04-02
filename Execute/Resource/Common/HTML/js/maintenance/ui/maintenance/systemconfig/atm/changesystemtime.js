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
            if (data.id == "INPUT_DATETIME") 
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
            if (data== "INPUT_DATETIME") 
            {
                result=this.MyGetValue();
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
		if (this.hasElement("INPUT_YEAR")
            &&this.hasElement("INPUT_MONTH")
            &&this.hasElement("INPUT_DAY")
            &&this.hasElement("INPUT_HOUR")
            &&this.hasElement("INPUT_MINUTE")
            &&this.hasElement("INPUT_SECOND")) 
        {
			var widget = this.getElementByName("INPUT_YEAR");
            var widget1 = this.getElementByName("INPUT_MONTH");
            var widget2 = this.getElementByName("INPUT_DAY");
            var widget3 = this.getElementByName("INPUT_HOUR");
            var widget4 = this.getElementByName("INPUT_MINUTE");
            var widget5 = this.getElementByName("INPUT_SECOND");

			if (widget == null||widget1 == null||widget2 == null||widget3 == null||widget4 == null||widget5 == null) {
				result = false;
			} 
            else 
            {
				if (widget.type != "raw"
                    &&widget1.type != "raw"
                    &&widget2.type != "raw"
                    &&widget3.type != "raw"
                    &&widget4.type != "raw"
                    &&widget5.type != "raw") 
                {
                    if (data.value != undefined&&data.value.length==14) 
                    {
                        widget.element.setValue(data.value.substr(0,4) , Config.CMD_ATTRIBUTE_TEXT);
                        widget1.element.setValue(data.value.substr(4,2) , Config.CMD_ATTRIBUTE_TEXT);
                        widget2.element.setValue(data.value.substr(6,2) , Config.CMD_ATTRIBUTE_TEXT);
                        widget3.element.setValue(data.value.substr(8,2) , Config.CMD_ATTRIBUTE_TEXT);
                        widget4.element.setValue(data.value.substr(10,2) , Config.CMD_ATTRIBUTE_TEXT);
                        widget5.element.setValue(data.value.substr(12,2) , Config.CMD_ATTRIBUTE_TEXT);
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
		if (this.hasElement("INPUT_YEAR")
            &&this.hasElement("INPUT_MONTH")
            &&this.hasElement("INPUT_DAY")
            &&this.hasElement("INPUT_HOUR")
            &&this.hasElement("INPUT_MINUTE")
            &&this.hasElement("INPUT_SECOND")) 
        {
			var widget = this.getElementByName("INPUT_YEAR");
            var widget1 = this.getElementByName("INPUT_MONTH");
            var widget2 = this.getElementByName("INPUT_DAY");
            var widget3 = this.getElementByName("INPUT_HOUR");
            var widget4 = this.getElementByName("INPUT_MINUTE");
            var widget5 = this.getElementByName("INPUT_SECOND");

			if (widget == null||widget1 == null||widget2 == null||widget3 == null||widget4 == null||widget5 == null) {
				result = false;
			} else {
				if (widget.type != "raw"
                    &&widget1.type != "raw"
                    &&widget2.type != "raw"
                    &&widget3.type != "raw"
                    &&widget4.type != "raw"
                    &&widget5.type != "raw") 
                {
					monthTmp = widget1.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
					dateTmp = widget2.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
					hourTmp = widget3.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
					minuteTmp = widget4.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
					secondTmp = widget5.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
					
					var date = new Date();					
					var month = date.getMonth() + 1;
					var strDate = date.getDate();
					if (month >= 1 && month <= 9) {
						month = "0" + month;
					}
					if (strDate >= 0 && strDate <= 9) {
						strDate = "0" + strDate;
					}					
					if(hourTmp == "")
					{
						hourTmp = "00";
						widget3.element.setValue( hourTmp , Config.CMD_ATTRIBUTE_TEXT);						
					}
					if(minuteTmp == "")
					{
						minuteTmp = "00";
						widget4.element.setValue( minuteTmp , Config.CMD_ATTRIBUTE_TEXT);						
					}
					if(secondTmp == "")
					{
						secondTmp = "00";
						widget5.element.setValue( secondTmp , Config.CMD_ATTRIBUTE_TEXT);						
					}
					
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
					if(Number(hourTmp) >= 1 && Number(hourTmp) <= 9 && hourTmp.length < 2)
					{
						hourTmp = "0" + hourTmp;
						widget3.element.setValue( hourTmp , Config.CMD_ATTRIBUTE_TEXT);
					}
					if(Number(minuteTmp) >= 1 && Number(minuteTmp) <= 9 && minuteTmp.length < 2)
					{
						minuteTmp = "0" + minuteTmp;
						widget4.element.setValue( minuteTmp , Config.CMD_ATTRIBUTE_TEXT);
					}
					if(Number(secondTmp) >= 1 && Number(secondTmp) <= 9 && secondTmp.length < 2)
					{
						secondTmp = "0" + secondTmp;
						widget5.element.setValue( secondTmp , Config.CMD_ATTRIBUTE_TEXT);
					}
				
                    result = widget.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget1.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget2.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget3.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget4.element.getValue(Config.CMD_ATTRIBUTE_TEXT) +
                        widget5.element.getValue(Config.CMD_ATTRIBUTE_TEXT);

                    if (result.length != 14) 
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