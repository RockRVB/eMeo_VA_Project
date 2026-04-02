(function(window){
	function trhelper(control){
		//总行数
		this.rowCount=0;
		// 当前第几行
		this.rowIndex=-1;
		//保存所有的列表
		this.rows=[];
		//把intrface 对象传递过来
		this.Control=control;
	
	}
	//添加属性方法
	trhelper.prototype={
	// 添加构造函数
		constructor: trhelper,
        /** 初始化函数 */
        init: function() {
			this.rows=$("tbody").find("tr");
			this.rowCount= this.rows.length;
        },
		// 添加一行
		add:function()
		{
			if(this.rowIndex>= this.rowCount-1)
				return 
				
			this.rowIndex++;
			this.trOper(true);
			
			
		},
		// 删除一行
		del:function()
		{
			if(this.rowIndex<0)
				return 
				
			this.trOper(false);
			this.rowIndex--;
			
		},
		//处理当前行
		//operType，操作类型 true: 显示 false ：隐藏
		trOper:function(operType)
		{
			try{
				if(this.rowIndex<0 ||this.rowIndex> this.rowCount-1)
				return;
				
				var tr=this.rows[this.rowIndex];
				if(operType)
				{
					$(tr).find("span").removeClass("none")
					$(tr).find("input").removeClass("none")
					this.tableOper(this.rowIndex+1);
				}
				else
				{
					$(tr).find("span").addClass("none")
					$(tr).find("input").addClass("none")
					this.tableOper(this.rowIndex);
				}
				
				
				
			} catch (e) {                   
                Config.log(e);
            } 
		},
		
		tableOper:function(rowCount)
		{
			if (this.Control.hasElement("table_1")) 
			{
				var widget = this.Control.getElementByName("table_1");
				if (widget == null) {
					return;
			} 
			else 
			{
				if (widget.type != "raw") 
                {
                    var count=parseInt(rowCount)
					if (isNaN(count)) {
	                    Config.log("row Count " + rowCount + " is error");
	                    return ;
	                }
					
					if(count>4)
					{
						// 设置行数
						widget.element.options.rowCount = count;
					}
					else
						widget.element.options.rowCount = 4;
					
	                // 设置完行数之后需要初始化
	                widget.element.init();
					
                }
			}
		} 
        else 
		{
		    if(this.Control.getChildUiInstance()!=undefined)
			{
				result=	this.Control.getChildUiInstance().setValue(data);
			    // 往子页面找控件处理
			}
		}
		}
	};
	
	window.trhelper = trhelper;
}
)(window)