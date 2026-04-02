require(['config/config','main'],function(){
	require(['animals'],function(){
		$(function(){
			$("#insertcard").animal({
				"drag":true,    //是否防拖拽,默认开启
				"speed":50,     //单位：毫秒
				"path":"../../../Image/CN/InserCard/",  //图片路径(相对当前页面)
				"imgperfix":"put_in_00",    //img图片前缀（去掉最大帧前面的值 如：put_in_00100.png,结果为put_in_00）
				"type":"png",   //图片类型（后缀名）
				"frames":100    //动画帧
			});
		})
	})
})