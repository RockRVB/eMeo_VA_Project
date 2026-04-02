/**
 * 帧动画插件
 * 时间：2016年9月13日13:33:30
 * 作者：hcj
 * 版本：1.0
 */
!(function(){
	var defaults={
		"drag":true, //是否防拖拽
		"speed":200, //速度，值越大越慢，单位：毫秒
		"path":"../../../Image/CN/InserCard/", //图片路径(相对当前页面)
		"imgperfix":"put_in_00",//img图片前缀（去掉最大帧前面的值）
		"type":"png",//图片类型（后缀名）
		"frames":100, //动画帧 
		"startFrame":0
	},animal={
		init:function(obj,s){
			this.loadimg(obj,s);
			this.animal(s);
		},
		loadimg:function(obj,sets){
			var str='',imglist='<div>',pwidth=$(obj).width(),pheight=$(obj).height(),appendarr=[''];
			for(var h=0;h<sets.frames.toString().length;h++){
				str+="0";
				appendarr.push(str);
			}
			for(var i=sets.startFrame;i<sets.frames+sets.startFrame;i++){
				imglist+='<img src="'+sets.path+sets.imgperfix+(appendarr[(sets.frames.toString().length-i.toString().length)]+i)+'.'+sets.type+'" class="imgarr'+i+'" alt="" width="'+pwidth+'" height="'+pheight+'"/>';
			}
			$(obj).css({"background":"none"}).html(imglist+"</div>");
			if(sets.drag){
				$(obj).append($('<div style="position:absolute;width:100%;height:100%;top:0;"></div>'));
			}
		},
		animal:function(sets){
			var isbegin=sets.startFrame;//从头开始
			setInterval(function(){
				for(var i=sets.startFrame;i<isbegin+sets.startFrame;i++){
					$("img.imgarr"+i).hide().next().show();
					if(isbegin==sets.frames){
						isbegin=sets.startFrame;
					}
				}
				isbegin++;
			},sets.speed)
		}
	}
	$.fn.animal=function(options){
		var settings=$.extend({}, defaults, options);
		return this.each(function(){
			animal.init(this,settings);
		})
	}
}(jQuery))
