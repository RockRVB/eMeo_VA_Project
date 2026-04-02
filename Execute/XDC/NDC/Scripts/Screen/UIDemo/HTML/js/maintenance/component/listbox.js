/******************** 
	作用:实现Listbox
	作者:Quincy
	版本:V1.0
	时间:2019-06-04
********************/

(function(window) {

	function CustomUi() 
	{
	    this.InvalSeparator ='\x1B';
		this.LoadingText="";
		this.DeviceError="";
		this.UserError="";
		this.Unknow="";
		this.NoDevice="";
		this.Color="orange";
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
		    if(data.id.indexOf("Listbox_")>=0)
			{
				result=this.setListBox(data);
			} else {
				CustomUi.superClass.setValue.call(this, data);
			}
		} catch (e) {
			result = false;
			Config.log(e);
		}

		return result;
	};
	
	// 私有方法 设值
	CustomUi.prototype.setListBox= function(data)
	{
	    var result=false;
		try{
			if (data.value.indexOf("#")>=0 && data.value.indexOf(",")>=0 ){
	    	var listObj =$("#"+data.id);
	    	var values=data.value.split("#");
			var selectedIdx=values[0].split(",")[0];
			var unknown=values[0].split(",")[1];//这个参数作用不明确,暂且不处理.
			listObj.empty();
			for(var i=1;i<values.length;i++){
				var item = values[i];
				listObj.append("<option value='"+item+"'>"+item+"</option>");
			}
			$("#"+data.id).val(values[selectedIdx]); 
			result=true;
		}
			
		}catch(e){
			Config.log("listbox type should be select");
			Config.log(e);
		}
	    
		return result;
	};

	// 重写方法取值
	CustomUi.prototype.getValue= function(id) {
		var result = "";
		try {

			if( id.indexOf("Listbox_")>=0)
			{
				result=this.getListValue(id);
			}
			else
			{
				result=CustomUi.superClass.getValue.call(this,id);
			}

		} catch (e) {
			Config.log(e);
		}
		return result;
	};

	// 自定义方法取值
	CustomUi.prototype.getListValue= function(id)
	{
		var result=""; 
		result=$("#"+id).val();
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