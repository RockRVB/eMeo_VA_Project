/**
 * 输入金额实例
 * 时间：2016年9月13日09:29:55
 * 作者：hcj
 */
require(['config/config','main'],function(){
	require(['jquery','checkClass','inputClass'],function($,check,ins){
		$(function(){
			ins.init();
			$("#InputWithdrawalAmount").on("input propertychange",function(){
				var _this=$(this);
				var jsonobj=check.getjson("core_AmountCheckValue");
				var reg=check.getreg("core_AmountCheckValue");
				_this.attr("maxlength",jsonobj.MaxLength);
				_this.val(_this.val().replace(reg,""));
				check.setdata("saveBuffer",_this.val());
			}).on("focus",function(){
				if(ins.show('VK','标准数字键盘',1080,600,0.8,$(this))){
					$(".money_panel_lis").slideUp(800).next().css("margin-top","150px");
				}
			}).on("blur",function(){
				$(".money_panel_lis").slideDown(800).next().css("margin-top","63px");
				ins.hide();
			})
			window.onbeforeunload=function(){
				ins.close();
			}
			window.closeInputms=function(){
				ins.close();
			}
		})
	})
})