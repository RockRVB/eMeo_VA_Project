/******************** 
	作用:清理钱箱
	作者:adam
	版本:V1.0
	时间:2015-06-22
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

    //生成控件之后
    CustomUi.prototype.afterInitUi = function() {  
    };

    
	//清除界面信息,用于从头开始显示信息
	CustomUi.prototype.cleanData = function() {
	};

	//重写方法设置值
	CustomUi.prototype.setValue = function(data) {
		var result = false;
		try 
        {
            if(data.id.indexOf("STATIC_TITLE")>=0)
                result=this.SetValue4unitsinfo(data);

            else if(data.id.indexOf("StartFlash-BORDER")>=0)
                result = this.StartFlashOneByOne(data,false);

            else if(data.id.indexOf("StopFlash-BORDER")>=0)
                 result= this.StartFlashOneByOne(data,true);

            else if (data.id == "StopAllFlash") {
                data.id = "StopFlash-BORDER_1";
                this.StartFlashOneByOne(data,true);
                data.id = "StopFlash-BORDER_2";
                this.StartFlashOneByOne(data,true);
                data.id = "StopFlash-BORDER_3";
                this.StartFlashOneByOne(data,true);
                data.id = "StopFlash-BORDER_4";
                result= this.StartFlashOneByOne(data,true);

            }
            else if (data.id == "StartFlash-STATIC_PROMPT") {
                this.StartFlashStopbyOneself(data);

            } else {
                result = CustomUi.superClass.setValue.call(this, data);
            }


        } catch (e) {
			result = false;
			Config.log(e);
		}

		return result;
	};
	
	
    // 私有方法 设值
	CustomUi.prototype.SetValue4unitsinfo= function(data)
	{
	    var result=false;
		if (this.hasElement(data.id.trim())) 
        {
			var widget = this.getElementByName(data.id.trim());
			if (widget == null) {
				result = false;
			} else {
				if (widget.type != "raw") 
                {
                    widget.element.setValue(data.value, Config.CMD_ATTRIBUTE_TEXT);
                    widget.element.show();
                    widget.element.element.siblings().children().removeClass("none");
                    this.ShowBox(widget.element);

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
        
		return result;
	};

    // 私有方法 自停式播放动画
	CustomUi.prototype.StartFlashStopbyOneself= function(data)
	{
        
	    var result=false;
	    var str = "StartFlash-";
        
	    if (data.id.indexOf(str)>=0) {
	        var id = data.id.trim().replace(str, "");
	        if (this.hasElement(id)) {
	            var widget = this.getElementByName(id);
	            if (widget == null) {
	                result = false;
	            } else {
	                if (widget.type != "raw") 
                    {
                        // flashInterval: 300,
                        //flashCount: 0,
                        var strs = data.value.split(',');
                        if (strs.length > 1) {
                            widget.element.options.flashInterval = strs[0];
                            widget.element.options.flashCount = strs[1];
                        }
                         
                        
                         setTimeout(function( ) { widget.element.startFlash();}, "10");
                        result = true;
	                } 
                    else 
                    {
	                    result = false;
	                }
	                
	            }
	        }
	    }

	    return result;
	};

    // 私有方法 开始播放动画
	CustomUi.prototype.StartFlashOneByOne= function(data,stop)
	{
        
	    var result=false;
	    var str = "";
        if(stop)
	        str = "StopFlash-";
        else {
           str = "StartFlash-";
        }
        
	    if (data.id.indexOf(str)>=0) {
	        var id = data.id.trim().replace(str, "");
	        if (this.hasElement(id)) {
	            var widget = this.getElementByName(id);
	            if (widget == null) {
	                result = false;
	            } else {
	                if (widget.type != "raw") 
                    {
                        if (stop) {
                            setTimeout(function( ) { widget.element.stopFlash();}, "10");
                             
                        } else {
                           
                             setTimeout(function( ) {  widget.element.startFlash();}, "10");
                        }
                        result = true;
	                } 
                    else 
                    {
	                    result = false;
	                }
	                
	            }
	        }
	    }

	    return result;
	};


    // 私有方法 设值
	CustomUi.prototype.ShowBox= function(o)
	{
	    if (o != undefined) {
	        o.element.parent().parent().removeClass("none");
	    }
	};
 
    //--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);


// 实例化
$(function() {
	new A();
});