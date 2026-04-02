/******************** 
	作用:核心版本加钞
	作者:adam
	版本:V1.0
	时间:2015-05-25
********************/

(function(window) {

	function CustomUi() 
	{
	    this.InvalSeparator ='\x1B';
		
		CustomUi.superClass.constructor.call(this);

         $(".input-sheet").on("valuechange",this.setamount);
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
		var result = true;
		try 
        {
            if (data.id == "STATIC_TIP") {
                // 调用父类方法设值
                result =CustomUi.superClass.setValue.call(this,data);
            } 
            else //调用私有方法设值
            {
                result=this.MySetValue(data);
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
                    widget.element.setValue(data.value, Config.CMD_ATTRIBUTE_TEXT);
                    widget.element.element.parent().parent(".nullClass").removeClass("none");
                    widget.element.element.parent().siblings().find(".input-sheet").removeClass("none");
                    widget.element.show();
                    //设置总金额
                    if (data.id.indexOf("INPUT_SHEET") >= 0) {
                        this.setamount2(widget.element);
                        this.settotleamount(widget.element);
                    } 
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
	
    //设置金额
    CustomUi.prototype.setamount = function() {
        var i = this.id.replace("INPUT_SHEET", "");
        var deno = "STATIC_DENO" + i;
        var amount = "STATIC_MONEY" + i;
        uiInstance.setmomeycount(deno, this.id, amount);
        uiInstance.settotleamount2(this.id);
    };

     //设置金额
    CustomUi.prototype.setamount2 = function(e) {
        id = e.options.id;
        var i = id.replace("INPUT_SHEET", "");
        var deno = "STATIC_DENO" + i;
        var amount = "STATIC_MONEY" + i;
        //设置钱箱金额
        uiInstance.setmomeycount(deno,id, amount);
        //设置总金额
        uiInstance.settotleamount(e);

    };
    // 设置金额
    CustomUi.prototype.setmomeycount = function(deno,sheel,amount) {

        result = false;
        if (this.hasElement(deno)&&this.hasElement(sheel)&&this.hasElement(amount)) 
        {
            var widget1 = this.getElementByName(deno);
            var widget2 = this.getElementByName(sheel);
            var widget3 = this.getElementByName(amount);


            if (widget1 == null||widget2==null||widget3==null) {
                result = false;
            } else 
            {
                if (widget1.type != "raw"&&widget2.type != "raw"&&widget3.type != "raw") 
                {
                    var d= widget1.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
                    var s=widget2.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
                    widget3.element.setValue(d * s, Config.CMD_ATTRIBUTE_TEXT);
                    widget3.element.show();
                }

            }
        }
        return result;
    };

    //设置总金额2
    CustomUi.prototype.settotleamount2 = function(id) {
        if (this.hasElement(id)) 
        {
            var widget1 = this.getElementByName(id);

            if (widget1 == null) {
                return;
            } else 
            {
                if (widget1.type != "raw") {
                    this.settotleamount(widget1.element);
                }
            }
        }

    };

    //设置总金额
    CustomUi.prototype.settotleamount = function(o) {
        var os = o.element.parent().parent().siblings().find("span[id^=STATIC_MONEY]");
        os=os.add(o.element.parent().siblings().find("span[id^=STATIC_MONEY]"));//把当前TR中的金额也加入进来

        var widget = this.getElementByName("STATIC_TOTALMONEY");
		return;//CNB 多币种钞箱打印总金额有误，干脆取消
		if (widget  ==null) {
		    return;
		}
		else 
        {
           if (widget.type != "raw") 
           {
                widget.element.setValue("0", Config.CMD_ATTRIBUTE_TEXT);// 初始化总金额
			  	$.each(os, function() {

            	uiInstance.settotlemomeycount(this.id,widget);

       		 }); 
					
		   }
        }
        

    };

    // 设置总金额
    CustomUi.prototype.settotlemomeycount = function(id,widget2) {

        if (this.hasElement(id)) 
        {
            var widget1 = this.getElementByName(id);

            if (widget1 == null) {
                return;
            } else 
            {
                if (widget1.type != "raw") 
                {
                    var d= parseInt(widget1.element.getValue(Config.CMD_ATTRIBUTE_TEXT));
                    var s=parseInt(widget2.element.getValue(Config.CMD_ATTRIBUTE_TEXT));
                    if (d == NaN)
                        d = 0;
                    if (s == NaN)
                        s = 0;

                    widget2.element.setValue(s+d, Config.CMD_ATTRIBUTE_TEXT);
                }

            }
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