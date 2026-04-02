/**
 * 输入卡号实例
 * 时间：2016年9月13日09:29:55
 * 作者：hcj
 */
require(['config/config','main'],function(){
	require(['jquery','checkClass','inputClass'],function($,check,ins){
		function getTxtCursorPosition(){
            var oTxt1 = document.getElementById("InputTransferAccount");
            var cursurPosition=-1;
            if(oTxt1.selectionStart){//非IE浏览器
                cursurPosition= oTxt1.selectionStart;
            }else{//IE
                var range = document.selection.createRange();
                range.moveStart("character",-oTxt1.value.length);
                cursurPosition=range.text.length;
            }
            return cursurPosition;
        }
        function setTxtCursorPosition(i){
            var oTxt1 = document.getElementById("InputTransferAccount");
            var cursurPosition=-1;
            if(oTxt1.selectionStart){//非IE浏览器
                oTxt1.selectionStart=i;
                oTxt1.selectionEnd=i;
            }else{//IE
                var range = oTxt1.createTextRange();
                range.moveStart("character",i);
                range.moveEnd("character",i);
                range.move("character",i);
//              range.select();
            }
        }
		$(function(){
			ins.init();
			var bits=$(window).width()/1920;
			var jsonobj=check.getjson("core_AccountCheckValue");
			var reg=check.getreg("core_AccountCheckValue");
			var finalval='',isback=0;
			$("#InputTransferAccount").on("input propertychange",function(){
				var currentpos=getTxtCursorPosition();
				var _this=$(this);
				//匹配非法字符
				_this.val(_this.val().replace(reg,""));
				var absval=function(){
					var arr=_this.val().split(" "),finl='';
					for(var i in arr){
						finl+=arr[i];
					}
					return finl;
				};
				_this.attr("data-val",absval());//设置给后台使用的值
				
				/**格式化输入*/
				var formatinput=function(){
					var abformat='';
					if(arguments[0]){
						for(var i in arguments[0]){
							console.log(typeof i)
							if((parseInt(i)+1)%4==0){
								abformat+=(arguments[0].charAt(i)+" ");
							}else{
								abformat+=arguments[0].charAt(i);
							}
						}
						console.log("格式化值"+abformat)
					}else{
						for(var i in absval()){
							if(parseInt(i)%4==0&&i!=0){
								abformat+=(" "+absval().charAt(i));
							}else{
								abformat+=absval().charAt(i);
							}
						}
					}
					_this.val(abformat);
				}
				if(absval().length==jsonobj.MaxLength){
					console.log(_this.val());
					finalval=_this.attr("data-val");//设置显示给用户最终的值
				}else if(absval().length>jsonobj.MaxLength){
					_this.attr("data-val",finalval);
					formatinput(finalval);
					return;
				}else if((absval().length)%4==0&&isback<absval().length){
					var str='';
					str+=(_this.val()+" ");
					_this.val(str);
				}
				/**是否达到标准格式 */
				var tarval=_this.val().trim().split(" "),booltar=true;
				for (var i=0;i<tarval.length-1;i++) {
					if(tarval[i].length!=4){
						booltar=false;
						break;
					}
				}
				formatinput();
				if(!booltar){
					//防中间插入
					setTxtCursorPosition(currentpos+1);
				}
				isback=absval().length;//是否删除退格标示
				console.log("jsval:"+absval())
				check.setdata("saveBuffer",absval());
			}).on("focus",function(){
				if(ins.slideShow("Execute","标准数字键盘","Move","VER",$(window).height(),$(window).height()-500*bits,0.6,$(this))){
					var offbottom=(function(){
						if($(this).attr("data-offtop")!=undefined){
							return $(this).attr("data-offtop");
						}else{
							$(this).attr("data-offtop",$(window).height()-$(this).parent().parent().height()-$(this).parent().parent().offset().top);
							return $(window).height()-$(this).parent().parent().height()-$(this).parent().parent().offset().top;
						}
					}.bind(this)());
					if(offbottom<(500*bits)){
						$(this).parent().parent().css({"transform":"translateY("+(offbottom-500*bits)+"px)"});
					}
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