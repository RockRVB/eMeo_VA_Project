/**
 * main function
 * Date:2016-11-15 15:27:11
 * Ahthor：hcj
 * Version：1.0
 */
require(['config/config'],function(c){
	console.debug("\u6240\u6709\u004a\u0061\u0076\u0061\u0053\u0063\u0072\u0069\u0070\u0074\u6587\u4ef6\u52a0\u8f7d\u5b8c\u6210\uff01");
	require(['jquery','createCon','inputClass','checkClass','layer','loading'],function($,_,ins,check){		
	ins.init();
		$(document).ready(function(){
			var ishome=(check.getdata("core_homeVisible")==true||check.getdata("core_homeVisible").toString().toLocaleLowerCase()=="true")?true:false;
			var iscall=(check.getdata("core_callVisible")==true||check.getdata("core_callVisible").toString().toLocaleLowerCase()=="true")?true:false;
			var statebar=(check.getdata("core_powerAndNetworkIconVisible")==true||check.getdata("core_powerAndNetworkIconVisible").toString().toLocaleLowerCase()=="true")?true:false;
			var power=check.getdata("core_powerType"),network=check.getdata("core_networkType"),basestation=check.getdata("core_networkProvider");
			var lave=check.getelementtext("IDS_RemainCount4Card"),laveval=check.getdata("core_remainCount4Card");
			var released=check.getelementtext("IDS_DispenseCount4Card"),releasedval=check.getdata("core_dispenseCount4Card");
			var annex=check.getelementtext("IDS_RetainCount4Card"),annexval=check.getdata("core_retainCount4Card");
			var stateinfo={power:power,network:network,lave:{name:lave,val:laveval},released:{name:released,val:releasedval},annex:{name:annex,val:annexval}};
			var callstate = check.getdata("core_callState");
			$(document).createCon({
				derections:"bottom",//位置(值：top,bottom,left,right)分别对应上下左右
				margins:"10px",//容器的边距
				center:"yes",//容器是否居中(值范围：yes,no)
				color:"#fff",//扩展容器字体颜色
				fontSize:"50px",//扩展容器字体大小
				width:"200px",//容器宽度
				content:"",//扩展容器的内容
				appendCon:false,//显示扩展的容器
				home:ishome,//是否显示回到首页
				call:iscall,//是否显示在线客服
				statebar:statebar,//是否显示状态栏
				stateobj:stateinfo,//状态栏对象
				idshome:"",//回到首页后台数据绑定标志
				idscall:"",
			});
			$('input[inputMode="true"]').on("focus",function(){
				var _this=$(this);
				ins.slideShow("Execute","拼音输入键盘","Move","VER",$(window).height(),$(window).width()/1920*580,0.6,$(this));
			}).on("blur",function(){
				ins.slideHide();
			})
		})
		var timercount=null;
		window.shade=function(src,time){
			var showFlag=false,timeout=!time?33000:time+3000;
			var index=layer.open({
				type: 2,
			    title: false,
			    closeBtn: 0, //不显示关闭按钮
			    shade: [0],
			    area: ['100%', '100%'],
			    time: timeout,
			    shift: 2,
			    content: [src, 'no'], //iframe的url，no代表不显示滚动条
			});
			try{
			ins.hide();
			}
			catch(e)
			{}
			timercount=setTimeout(function(){//超时处理
				window.external.PopupEventRaised('Timeout');
			},timeout);
			if(!!index){
				showFlag=true;
			}
			return showFlag;
		}
		window.stopEvent=function(eid){
		
			if($("#"+eid).attr("isenable").toLocaleLowerCase()=="true"){
				$("#"+eid).attr("tag",$("#"+eid).attr("tag").indexOf("endtag")>=0?$("#"+eid).attr("tag"):$("#"+eid).attr("tag")+"endtag").addClass("disabled_btn");
			}else{
				$("#"+eid).attr("tag",$("#"+eid).attr("tag").replace("endtag","")).removeClass("disabled_btn");
			}
		}
		window.onunload=window.onbeforeunload=function(){
			try{
			ins.close();
			}
			catch(e){}
		}
		window.close_shade=function(){
			layer.closeAll();
			window.clearTimeout(timercount);
			$(document).setState(check.getdata("core_callState"));
			return true;
		}
	})
});
