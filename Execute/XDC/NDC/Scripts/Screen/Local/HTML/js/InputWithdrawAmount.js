/**
 * 输入金额实例
 * 时间：2016年9月13日09:29:55
 * 作者：hcj
 */
require(['config/config','main'],function(){
	require(['jquery','checkClass','inputClass'],function($,check,ins){
		$(function(){
			ins.init();
			var bits=$(window).width()/1920;
			$("#InputWithdrawalAmount").attr("placeholder",check.getdata("core_WithdrawalDenoPromptText"));
			$("#InputWithdrawalAmount").on("input propertychange",function(){
				var _this=$(this);
				var jsonobj=check.getjson("core_AmountCheckValue");
				var reg=check.getreg("core_AmountCheckValue");
				_this.attr("maxlength",jsonobj.MaxLength);
				_this.val(_this.val().replace(reg,""));
				check.setdata("saveBuffer",_this.val());
			}).on("focus",function(){
				if(ins.slideShow("Execute","标准数字键盘","Move","VER",$(window).height(),$(window).height()-500*bits,0.6,$(this))){
					$(".money_panel_lis").slideUp(800).next().animate({"margin-top":"150px"});
				}
			}).on("blur",function(){
				$(".money_panel_lis").slideDown(800).next().css("margin-top","63px");
				ins.slideHide();
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