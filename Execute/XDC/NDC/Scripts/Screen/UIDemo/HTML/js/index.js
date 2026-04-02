/**
 * index.js
 * Date:2016-11-15 15:27:11
 * Ahthor：hcj
 * Version：1.0
 */
$(document).ready(function(){
	var timercount=null;
	window.shade=function(src,time){
	var showFlag=false,timeout=0;
		/*var index=layer.open({
			type: 2,
			title: false,
			closeBtn: 0, //不显示关闭按钮
			shade: [0],
			area: ['30%', '30%'],
			time: timeout,
			shift: 2,
			content: [src, 'no'], //iframe的url，no代表不显示滚动条
		});	*/
		var index=layer.open({
			type: 0,
			title: false,
			skin:'layui-layer-lan',
			closeBtn: 0, //不显示关闭按钮
			shade: [0],
			area: ['25%', '35%'],
			time: timeout,
			shift: 2,
			content: '<span>Transaction Timeout,Do you want to exit?</span>', 
			btn:['Yes','No'],
			yes:function(){
				if(window.chrome){
					window.chrome.webview.hostObjects.sync.eCAT.PopupEventRaised('Confirm');	
				}else{
					window.external.PopupEventRaised('Confirm');
				}
			},
			btn2:function(){
				if(window.chrome){
					window.chrome.webview.hostObjects.sync.eCAT.PopupEventRaised('Cancel');	
				}else{
					window.external.PopupEventRaised('Cancel');
				}

				//显示签名面板（如有）
				if($('#OnShowSign')) {
					$('#OnShowSign').click();
				}
				return false;
			},
			success:function (){
				//隐藏签名面板（如有）
				if($('#OnHideSign')) {
					$('#OnHideSign').click();
				}
			}
		});
		
		if(!!index){
			showFlag=true;
		}
		return showFlag;
	}
	
	window.close_shade=function(){
		try{
			layer.closeAll();
			//window.clearTimeout(timercount);
			//$(document).setState(getdata("core_callState"));
		}
		catch(e)
		{
			
		}
		return true;
	}
})
