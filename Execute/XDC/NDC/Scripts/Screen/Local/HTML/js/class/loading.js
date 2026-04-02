define(['jquery','animals'],function($){
	$(function(){
		if($(".waiting_animate").length>0){
			$(".waiting_animate").animal({
				"drag":true, //是否防拖拽
				"speed":30, //速度，值越大越慢，单位：毫秒
				"path":"../../../Image/CN/Animal/loading/", //图片路径(相对当前页面)
				"imgperfix":"loading1_000",//img图片前缀（去掉最大帧前面的值）
				"type":"png",//图片类型（后缀名）
				"frames":20, //动画帧
				"startFrame":0 //起始帧
			})
		}
	})
})