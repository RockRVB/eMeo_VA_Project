/******************** 
	作用:I58版本加钞
	作者:adam
	版本:V1.0
	时间:2015-06-23
********************/

(function(window) {

	function CustomUi() 
	{
	    this.InvalSeparator ='\x1B';
		this.isSetPageCount=false;
		this.pageCount=1;
		CustomUi.superClass.constructor.call(this);
	}

	Config.extend(CustomUi, Interface);

	//预处理
	CustomUi.prototype.preInit = function() {
		this.cleanData();
	};

    //生成控件之后
    CustomUi.prototype.afterInitUi = function() {
       $(".input-sheet").on("valuechange",this.setamount);
	   if(Config.INPUT_TARGET!=null&&Config.INPUT_TARGET!=undefined)
	   {
			//Config.INPUT_TARGET.element.click()
	   }
    };

    
	//清除界面信息,用于从头开始显示信息
	CustomUi.prototype.cleanData = function() {
        $(".input-sheet").unbind("valuechange");
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
			else if (data.id == "forpagerowcount")//forpagerowcount
			{
				result=this.Setforpagerowcount(data.value);
				this.isSetPageCount=true;
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
					widget.element.show();
					
					if(data.id.indexOf("STATIC_CASSETID") >= 0)
					{
						if(!this.isSetPageCount) // 如果没有设置行数，则动态显示行数
						{
							this.Setforpagerowcount(this.pageCount); // 设置显示行数
							this.pageCount++;
						}
						
						widget.element.element.parent().parent(".nullClass").removeClass("none");
						widget.element.element.parent().siblings().children().removeClass("none");
					}

                    if(data.id.indexOf("INPUT_SHEET") >= 0 ) {
						// 设置加钞张数
						this.setaddcashcount(data.id.replace("INPUT_SHEET",""),data.value,widget.element.getValue(Config.CMD_ATTRIBUTE_TEXT));
                        this.setamount2(widget.element);
						
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
	
	CustomUi.prototype.Setforpagerowcount= function(value)
	{
		var result=false;
		if (this.hasElement("table_1")) 
        {
			var widget = this.getElementByName("table_1");
			if (widget == null) {
				result = false;
			} else {
				if (widget.type != "raw") 
                {
					if (isNaN(parseInt(value))) {
	                    Config.log("row Count " + value + " is error");
	                    return false;
	                }
	                    
                    // 设置行数
	                widget.element.options.rowCount = parseInt(value);
	                // 设置完行数之后需要初始化
	                widget.element.init();
					
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
	}
	
    //设置单个钱箱加钞金额总入口
    CustomUi.prototype.setamount = function() {
        var i = this.id.replace("INPUT_SHEET", "");
        var deno = "STATIC_DENO" + i; //  面额
        var amount = "STATIC_MONEY" + i; // 累积加钞金额
        uiInstance.setmomeycount(deno, this.id, amount);
        uiInstance.settotleamount2(this.id);
    };

     CustomUi.prototype.setamount2 = function(obj) {
        var i = obj.options.id.replace("INPUT_SHEET", "");
        var deno = "STATIC_DENO" + i; //  面额
        var amount = "STATIC_MONEY" + i; // 累积加钞金额
        uiInstance.setmomeycount(deno, obj.options.id, amount);
        uiInstance.settotleamount2(obj.options.id);
    };

    // 设置单个钱箱加钞金额
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
                    var d= widget1.element.getValue(Config.CMD_ATTRIBUTE_TEXT);// 面额
                    var s=widget2.element.getValue(Config.CMD_ATTRIBUTE_TEXT); // 加钞张数
                    widget3.element.setValue(d * s, Config.CMD_ATTRIBUTE_TEXT); // 设置累积加钞金额
                    widget3.element.show();
                }

            }
        }
        return result;
    };

    //设置当次加钞总金额 入口
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

    //设置当次加钞总金额入口（根据输入金额框）
    CustomUi.prototype.settotleamount = function(o) {
        var os = o.element.parent().parent().siblings().find("span[id^=STATIC_MONEY]"); // 找兄弟 
        os=os.add(o.element.parent().siblings().find("span[id^=STATIC_MONEY]"));//把当前TR中的金额也加入进来 （找自己）

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

    // 设置当次加钞总金额执行
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
                    if (isNaN(d))
                        d = 0;
                    if (isNaN(s))
                        s = 0;

                    widget2.element.setValue(s+d, Config.CMD_ATTRIBUTE_TEXT);
                }

            }
        }
    };


    // 设置单个钱箱当次和累积加钞张数
    CustomUi.prototype.setaddcashcount = function(no,count,max) {
        //STATIC_CASH2  累积加钞张数
        //STATIC_CURRENT_COUNT2 当前加钞张数
        if (no == undefined)
            no = "";

        var id1 = "STATIC_CASH" + no;
        var id2 = "STATIC_CURRENT_COUNT" + no;


        if (this.hasElement(id1)&&this.hasElement(id2)) 
        {
            var widget1 = this.getElementByName(id1);
            var widget2 = this.getElementByName(id2);
            if (widget1 == null||widget2 == null) {
                return;
            } else 
            {
                if (widget1.type != "raw" && widget2.type != "raw") 
                {
					var m=parseInt(max)
					if (isNaN(m))
                        m = 0;
                    var d= parseInt(count);
                    var s=parseInt(widget1.element.getValue(Config.CMD_ATTRIBUTE_TEXT));
                    if (isNaN(d))
                        d = 0;
                    if (isNaN(s))
                        s = 0;
				
                    widget2.element.setValue(d, Config.CMD_ATTRIBUTE_TEXT);
					if(m!=0&& m>=(s+d)) //如果当前加钞张数== 累积加钞张数 则 累积加钞张数不变 
                    widget1.element.setValue(s+d, Config.CMD_ATTRIBUTE_TEXT);
                    else // 当前加钞张数清零
                    widget2.element.setValue(0, Config.CMD_ATTRIBUTE_TEXT);

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