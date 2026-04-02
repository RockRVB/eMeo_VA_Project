/******************** 
	作用:系统登入界面
	作者:adam
	版本:V1.0
	时间:2015-05-22
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
	
	//生成页面之后
	CustomUi.prototype.afterInitUi = function() {
		Config.Screen.changeState(Config.UI_LOGIN_ROLE);
	};
	
	//重写方法设置值
	CustomUi.prototype.setValue = function(data) {
		var result = true;
		try {
				if(data.id=="FUN_USERID")
				{
					var userid= data.value.split(this.InvalSeparator);
					this.MySetValue("FUN_1",userid[0]);
					this.MySetValue("FUN_2",userid[1]);
				}
                else if (data.id=="TWIN_KEYBOARD") 
				{
				    result = this.KeyAction(data.value);
				} 
                else 
                {
		            result = CustomUi.superClass.setValue.call(this, data);
		        }

		} catch (e) {
			result = false;
			Config.log(e);
		}
		return result;
	};
	// 重写方法取值
	CustomUi.prototype.getValue= function(data) {
			var result = "";
			try {
				
				if(data=="FUN_USERID")
				{
					
					var ids=new Array("FUN_1","FUN_2");
					result=this.MyGetValue(ids);
				}
				else
				{
					result=CustomUi.superClass.getValue.call(this,data);
				}
			
			} catch (e) {
				result = "";
				Config.log(e);
			}
			return result;
		};
	
	
	// 自定义方法取值
	CustomUi.prototype.MyGetValue= function(id)
	{
	    var result="";
		
		$.each(id, function() 
		{		
			if(this!=""&&this!=undefined)// id 为空则直接返回失败
			{
				if (Config.Ui.getUiInstance().hasElement(this)) 
				{
					var widget = Config.Ui.getUiInstance().getElementByName(this);
					if (widget == null) {
						result = "";
					} 
					else 
					{
						if (widget.type != "raw") 
						{
							if(widget.element.options.isSelected)
							{
								//result = widget.element.getValue(Config.CMD_ATTRIBUTE_TEXT);
								//result= result=="银行管理员"?"1":"2";
								
								//根据所选角色按钮设置角色id值， edit by xjyong
								var roleId = widget.element.options.id.split('_');
								result = roleId[1];
								return false;
							}
							
						} 
					}
					
					
				}		
			}
			
		});
		// 当前页面没有则不再去继续找直接返回	
		return result;
	};
	
	
	// 私有方法 设值
	CustomUi.prototype.MySetValue= function(id,value)
	{
	    var result=false;
		if (this.hasElement(id)) {
					var widget = this.getElementByName(id);
					if (widget == null) {
						result = false;
					} else {
						if (widget.type != "raw") {
							widget.element.setValue(value, Config.CMD_ATTRIBUTE_TEXT);
						} else {
							if (widget.functionType == "html")
								widget.element.html(value);
							else
								widget.element.val(value);
						}
					}
				} else 
				{
					result = false;
				}
				
				return result;
	};
	//按键响应
    CustomUi.prototype.KeyAction= function(key)
	{
	    var result=false;
	    switch (key) {
		//新增数字键事件响应,支持键盘操作,Start......
	    case "1":return this.keyBoardAction("keyboard-button-1",key);
	    case "2":return this.keyBoardAction("keyboard-button-2",key);
	    case "3":return this.keyBoardAction("keyboard-button-3",key);
	    case "4":return this.keyBoardAction("keyboard-button-4",key);
	    case "5":return this.keyBoardAction("keyboard-button-5",key);
	    case "6":return this.keyBoardAction("keyboard-button-6",key);
	    case "7":return this.keyBoardAction("keyboard-button-7",key);
	    case "8":return this.keyBoardAction("keyboard-button-8",key);
	    case "9":return this.keyBoardAction("keyboard-button-9",key);
	    case "0":return this.keyBoardAction("keyboard-button-10",key);
	    case "CLEAR":return this.keyBoardAction("keyboard-button-11",key);
	    case "KEYBOARD_TAB":return this.keyBoardAction("keyboard-button-12",key);
		//新增数字键事件响应,支持键盘操作,End......
	    case "ENTER":return this.keyBoardAction("KEYBOARD_ENTER",key);
        case "CANCEL":return this.keyBoardAction("FUN_QUIT",key);
        // 后续插入快捷键等等
        //.........
	    default:
	        return this.keyBoardAction("keyboard",key);
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