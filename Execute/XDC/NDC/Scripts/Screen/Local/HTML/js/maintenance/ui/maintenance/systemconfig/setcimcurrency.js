/******************** 
	作用:设置存款面额
	作者:adam
	版本:V1.0
	时间:2015-09-08
********************/

(function(window) {

	function CustomUi() 
	{
	    this.InvalSeparator ='\x1B';

		
        /**快捷菜单列表*/
	    
        /**钱箱信息列表*/
        this.BoxList = [];
        /**页记录数*/
	    this.BoxPageSize = 6;
        /**当前显示页*/
	    this.BoxPageIndex = 1;

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
		var result = false;
		try 
        {
			if (data.id.indexOf("TABLE_1") == 0) 
            {
			    result=this.MySetValue(data);
			}
            else if (data.id.indexOf("CheckboxGroup") == 0) {
			    result = this.MysetAttribute(data);
			} 
            else {
                result = CustomUi.superClass.setValue.call(this, data);
            }

        } catch (e) {
			result = false;
			Config.log(e);
		}

		return result;
	};
	
	// 私有方法 设值
	CustomUi.prototype.MySetValue= function(data) {
	    var result = false;
	    if (this.hasElement(data.id)) {
	        var widget = this.getElementByName(data.id);
	        if (widget == null) {
	            result = false;
	        } else {
	            if (widget.type != "raw") {

	                if (isNaN(parseInt(data.value))) {
	                    Config.log("row Count " + data.value + " is error");
	                    return false;
	                }
	                    
                    // 设置行数
	                widget.element.options.rowCount = parseInt(data.value);

//	                if (widget.element.options.maxRowCount> widget.element.options.rowCount) //当实际列数小于最大列数时，避免显示空白行，使其最大行跟实际行数相等。
//                    {
//	                    widget.element.options.maxRowCount = widget.element.options.rowCount;
//	                }
	                // 设置完行数之后需要初始化
	                widget.element.init();

	            }
	        }
	    }

	};

    CustomUi.prototype.tableDataInit = function(count) {
        
    };

    CustomUi.prototype.getAttribute = function(data) {
		var result = false;
		try 
        {
			if (data.indexOf("CheckboxGroup") == 0) {
			    result = this.MygetAttribute(data);
			} 
            else {
                result = CustomUi.superClass.setValue.call(this, data);
            }

        } catch (e) {
			result = false;
			Config.log(e);
		}

		return result;
	};

    CustomUi.prototype.MysetAttribute   = function(data) {
	    var result = false;
	    if (this.hasElement(data.id)) {
	        var widget = this.getElementByName(data.id);
	        if (widget == null) {
	            result = false;
	        } else {
	            var vals = data.value.split(',');
	            if (vals.length < 2)
	                return false;

	            if (widget.type != "raw") {
							
							widget.element.setValue(vals[1], Config.CMD_ATTRIBUTE_VISIBLE);
							widget.element.setValue(vals[0], Config.CMD_ATTRIBUTE_SELECTED);
						} 
						else // 只设置可见性
						{
							
							if(vals[1] == "1" || vals[1] == true)
							{
								widget.element.show();
							}
							else
								widget.element.hide();
                            // 设置选中属性
						    if (widget.element.attr("type") == "checkbox") 
                            {
						        widget.element.attr("checked", (vals[0]=="1"||vals[0] == true||vals[0] == "true")?true:false);
						    }
						}
	            return true;
	        }
	    }

        return result;

    };
    // 取属性
    CustomUi.prototype.MygetAttribute=function(data) {
			var result = "";
			try {
				if (this.hasElement(data)) 
				{
						var widget=this.getElementByName(data);
						if(widget==null){
							result = "";
						}else{
							if(widget.type!="raw")// 非原生的HTML控件，而是自定义控件类型
							{
								result=widget.element.getValue(Config.CMD_ATTRIBUTE_SELECTED)+
								widget.element.getValue(Config.CMD_ATTRIBUTE_VISIBLE)+
								widget.element.getValue( Config.CMD_ATTRIBUTE_ENABLED);
							}
							else // 非自定义控件
							{
							    result = widget.element.is(":checked") ? "1" : "0";
                                //result+=(widget.element.is(":visible")||widget.element.is(":hidden"))?"1":"0";
							    //result += "1";
							}
						}
					
				} else 
				{
					if(this.getChildUiInstance()!=undefined)
					{
						this.getChildUiInstance().getAttribute(data);
						// 往子页面找控件处理
					}
				}
			} catch (e) {
				result = "";
				Config.log(e);
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