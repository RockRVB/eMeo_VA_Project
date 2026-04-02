/******************** 
	作用:查询钱箱信息
	作者:adam
	版本:V1.0
	时间:2015-10-03
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
			if(data.id.indexOf("forpagerowcount")==0)
			{
				result=this.MySetValue(data);
			}
			else
			{
				result =CustomUi.superClass.setValue.call(this,data);
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
		if (this.hasElement("Table_1")) 
        {
			var widget = this.getElementByName("Table_1");
			if (widget == null) {
				result = false;
			} else {
				if (widget.type != "raw") 
                {
                    //widget.element.setValue(data.value, Config.CMD_ATTRIBUTE_TEXT);
                    //widget.element.element.parent().parent(".nullClass").removeClass("none");
                    //widget.element.show();
					var count=parseInt(data.value)
					if (isNaN(count)) {
	                    Config.log("row Count " + data.value + " is error");
	                    return false;
	                }

	                if (widget.element.options.maxRowCount< count) //当实际行数少于最大行数时，为了避免操作按钮往上挤，实际行数以最大行数为准
                    {
	                    //widget.element.options.maxRowCount = widget.element.options.rowCount;
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