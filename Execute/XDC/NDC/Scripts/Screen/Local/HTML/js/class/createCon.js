/**
 * ibank 动态按钮插件
 * 时间：2016年11月15日15:13:50
 * 作者：hcj
 * 版本：1.2
 */
!(function($) {
	var defaults = {
			derections: "top", //位置(值：top,bottom,left,right)分别对应上下左右
			margins: "10px", //容器的边距
			center: "yes", //容器是否居中(值范围：yes,no)
			color: "#5c5d5f", //扩展容器字体颜色
			fontSize: "50px", //扩展容器字体大小
			width: "200px", //容器宽度
			content: "", //扩展容器的内容
			appendCon: true, //显示扩展的容器
			home: true, //是否显示回到首页
			call: true, //是否显示在线客服
			statebar: true, //是否显示状态栏
			stateobj: { power: "charge", network: "mobile_net", basestation: "中国移动", lave: { name: "剩余", val: 12 }, released: { name: "已发", val: 20 }, annex: { name: "吞卡", val: 20 } }, //状态栏对象
			idshome: "", //回到首页后台数据绑定标志
			idscall: "", //在线客服后台数据绑定标志
		},
		ms = {
			init: function(v) {
				this.statebar(v);
				this.home(v);
				this.call(v);
				this.appendcon(v);
//				this.bindEvent()
			},
			statebar: function(obj) {
				if(obj.statebar) {
					var newEle = document.createElement("div");
					newEle.setAttribute("class", "device_container");
					var statehtml = '<div class="power ' + (obj.stateobj.power || "charge") + '"></div>' +
						'<div class="network ' + (obj.stateobj.network || "mobile_net") + '">' + (obj.stateobj.basestation || "") + '</div>' +
						'<div class="card_info">' +
						'<ul class="card_record">' +
						'<li>' + (obj.stateobj.lave.name || "剩余") + '<span>' + (obj.stateobj.lave.val || 0) + '</span><em>|</em></li>' +
						'<li>' + (obj.stateobj.released.name || "已发") + '<span>' + (obj.stateobj.released.val || 0) + '</span><em>|</em></li>' +
						'<li>' + (obj.stateobj.annex.name || "吞卡") + '<span>' + (obj.stateobj.annex.val || 0) + '</span></li>' +
						'</ul>' +
						'</div>';
					newEle.innerHTML = statehtml;
					document.body.appendChild(newEle);
				}
			},
			home: function(ds) {
				if(ds.home) {
					var home = document.createElement("div");
					home.setAttribute("class", "home_btn");
					home.setAttribute("id", "homeButton");
					home.setAttribute("type", "Button");
					home.setAttribute("tag", "OnCancel");
					home.setAttribute("ids", ds.idshome);
					document.body.appendChild(home);
				}
			},
			call: function(ds) {
				if(ds.call) {
					var call = document.createElement("div");
					call.setAttribute("class", "call_btn");
					call.setAttribute("id", "callButton");
					call.setAttribute("type", "Button");
					call.setAttribute("tag", "OnHelp");
					call.setAttribute("ids", ds.idscall);
					document.body.appendChild(call);
				}
			},
			appendcon: function(ds) {
				if(ds.appendCon) {
					var appendDiv = document.createElement("div"),
						derec = ds.derections;
					appendDiv.style.position = "fixed";
					appendDiv.style.width = ds.width;
					appendDiv.style[derec] = ds.margins;
					appendDiv.style.fontSize = ds.fontSize;
					appendDiv.style.color = ds.color;
					appendDiv.style.overflow = "hidden";
					appendDiv.style.wordWrap = "break-word";
					appendDiv.innerHTML = ds.content;
					if(ds.center == "yes") {
						if(derec == "top" || derec == "bottom") {
							appendDiv.style.left = (document.body.offsetWidth - ds.width.split("px")[0]) / 2;
						} else {
							appendDiv.style.top = (document.body.offsetHeight - appendDiv.offsetHeight) / 2;
						}
					}
					document.body.appendChild(appendDiv);
				}
			},
			bindEvent: function() {
				$(".call_btn").click(function() {
					if($(this).hasClass("connected")) {
						$(this).removeClass("connected").addClass("disconnect").attr("title", "已断开连接");
					} else {
						$(this).removeClass("disconnect").addClass("connecting").attr("title", "正在连接....");
						var clickTime = setTimeout(function() {
							$(this).removeClass("connecting animated").addClass("connected").attr("title", "已连接成功");
						}.bind(this), 1e4)
					}
				})
			},
			changeState:function(state){
				switch (state){
					case 1:
						$(".call_btn").removeClass().addClass("call_btn connected").attr("title", "正在连接....");
						break;
					case 2:
						$(".call_btn").removeClass().addClass("call_btn connecting animated").attr("title", "正在连接....");
						break;
					case 3:
						$(".call_btn").removeClass().addClass("call_btn disconnect").attr("title", "正在连接....");
						break;
					default:
						$(".call_btn").removeClass().addClass("call_btn").attr("title", "呼叫客服");
						break;
				}
			}
		};
	$.fn.createCon = function(options) {
		var settings = $.extend({}, defaults, options);
		return this.each(function() {
			ms.init(settings);
		})
	}
	$.fn.setState=function(state){
		ms.changeState(state);
	}
}(jQuery))