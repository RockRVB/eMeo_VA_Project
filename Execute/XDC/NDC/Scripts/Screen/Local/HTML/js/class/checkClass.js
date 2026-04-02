/**
 * 校验类
 * 时间：2016年9月12日13:40:17
 * 作者：hcj
 * 版本：1.0
 */
(function(factory) {
	if(typeof define === "function" && define.amd){
		//amd register module
		define('checkClass',[],factory);
	}else{
		//浏览器全局
		factory();
	}
}(function(){
	return {
		getdata:function(data){
			try{
				return window.external.GetData(data);
			}catch(e){
				return 'null';
			}
		},
		getelementtext:function(data){
			try{
				return window.external.GetResourceValue(data);
			}catch(e){
				return null;
			}
		},
		setdata:function(item,val){
			try{
				window.external.SetData(item,val);
			}catch(e){
				console.log("未检测到后台setdata方法！");
			}
		},
		sethtml:function(data,ele){
			document.querySelector(ele).innerHTML=this.getdata(data);
		},
		getjson:function(code){
			try{
				var obj=window.external.GetData(code);
				obj=((typeof(obj)!="object")?JSON.parse(obj):obj);
				return obj;
			}catch(e){
				return {"Format":"####","MaxLength":19,"MinLength":0,"DecimalPointAccuracy":2,"ActivityKeys":["0","1","2","3","4","5","6","7","8","9","00"]};
			}
		},
		getreg:function(code){
			var jsonobj=this.getjson(code);
			var actkeyArr=jsonobj.ActivityKeys;
			var actkey='';
			for(var i in actkeyArr){
				actkey+=actkeyArr[i];
			}
			var reg=new RegExp("[^"+actkey+"]{1,"+jsonobj.MaxLength+"}");
			return reg;
		}
	};
}))