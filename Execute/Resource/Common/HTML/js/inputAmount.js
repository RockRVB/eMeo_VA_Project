require(['config/config','main'],function(){
	require(['jquery','checkClass','inputClass'],function($,check,ins){
		$(function(){
			ins.init();
			var bits=$(window).width()/1920;
			$("input[data-check='amount']").on("input propertychange",function(){
				var _this=$(this);
				var jsonobj=check.getjson("core_AmountCheckValue");
				var reg=check.getreg("core_AmountCheckValue");
				_this.attr("maxlength",jsonobj.MaxLength);
//				_this.val(_this.val().replace(reg,""));
				if(_this.val().trim().match(/^[a-zA-Z]+$/)){
					$(".error_tips").text("输入不正确！").show();
				}else{
					$(".error_tips").hide();
				}
				check.setdata("saveBuffer",_this.val());
			}).on("focus",function(){
				if(ins.slideShow("Execute","标准数字键盘","Move","VER",$(window).height(),$(window).height()-500*bits,0.6,$(this))){
					$(this).parent().parent().css({"transform":"translateY(-"+110*bits+"px)"});
				}
			}).on("blur",function(){
				ins.slideHide();
				$(this).parent().parent().css({"transform":"none"});
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