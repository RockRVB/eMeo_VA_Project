/******************** 
	作用:清理回收箱
	作者:adam
	版本:V1.0
	时间:2015-06-22
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
            switch (data.id.trim()) {
            case "StartFlash-IMAGE_NO_AM1": 
               return  result = this.StartFlashOneByOne(data,true);
            default: return result =CustomUi.superClass.setValue.call(this,data);

            }   
		} catch (e) {
			result = false;
			Config.log(e);
		}

		return result;
	};
	
	

    // 私有方法 开始播放动画
	CustomUi.prototype.StartFlashOneByOne= function(data,stop)
	{
	    var result=true;
	    this.FlashPlay(data, stop);
	    return result;
	};


    // 私有方法 开始播放动画
	CustomUi.prototype.FlashPlay= function(data,stop)
	{
         var  str = "StartFlash-";
	    var id = data.id.trim().replace('1','').replace(str, "");
	    if (this.hasElement(id)) {
	        var widget = this.getElementByName(id);
	        if (widget == null) {
	            return  ;
	        } else {
	            if (widget.type != "raw") {
	                var src;
                    if (stop) 
                    {
                        if(id.indexOf("NO")>=0)
                        src  = getRootPath_web()+"/Image/"+Config.Lang+"/maintenance/screen/194-no-am1.png";
                        else {
                            src  = getRootPath_web()+"/Image/"+Config.Lang+"/maintenance/screen/194-am1.png";
                        }
                    } 
                    else 
                    {
                        if(id.indexOf("NO")>=0)
                        src  = getRootPath_web()+"/Image/"+Config.Lang+"/maintenance/screen/194-no-am2.png";
                        else {
                            src  = getRootPath_web()+"/Image/"+Config.Lang+"/maintenance/screen/194-am2.png";
                        }
                    }
                    widget.element.setValue(src, Config.CMD_ATTRIBUTE_TEXT);
	            }   
	        }
	    }
	    if(data.value==""||data.value==undefined)
			data.value=1000;
			
        setTimeout(function() { uiInstance.FlashPlay(data,stop?false:true); }, data.value);
	};
 
    //--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);


// 实例化
$(function() {
	new A();
});