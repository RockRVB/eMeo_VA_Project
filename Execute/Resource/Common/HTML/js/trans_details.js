require(['config/config','main'],function(){
	require(['jquery','swiper','mobiscroll'],function($,Swiper,mobiscroll){
		var slideDate=(function(){
			return {
				getval:function(it){
					var timestr=[];
					$(it).find("div.dw-sel").each(function(){
						timestr.push($(this).attr("data-val"));
					});
					return timestr;
				},
				setval:function(it){
					$(it).find('div.date_select_group_con').mobiscroll().date({
		                theme: 'mobiscroll-dark', 
		                mode: 'scroller',
		                display: 'inline',
		                lang: 'zh',
		                minDate: new Date(new Date().setFullYear(new Date().getFullYear() - 10)),
		                maxDate: new Date(),
		            });
				},
				open:function(){
					$(".date_select_container").hide().slideToggle();
				},
				close:function(){
					$(".date_select_container").slideToggle();
				}
			}
		})();
		$(document).ready(function(){
			$(".transaction_details_con_head").on("click","div:nth-child(3)",function(){
				slideDate.open();
			})
			$(".transaction_details_con_head").on("click","div:nth-child(2)",function(){
				var now=new Date();
				$("#beginyear,#endyear").val(now.getFullYear());
				$("#beginmonth,#endmonth").val(now.getMonth()+1);
				$("#beginday,#endday").val(now.getDate());
				$(this).addClass("traction_current").siblings().removeClass();
			})
			$(".date_button_group").on("click","div:nth-child(1)",function(){
				slideDate.setval("div.date_select_group.startdate");
				slideDate.setval("div.date_select_group.enddate");
			})
			$(".date_button_group").on("click","div:nth-child(2)",function(){
				slideDate.close();
			})
			$(".date_button_group").on("click","div:nth-child(3)",function(){
				var beginArrr=slideDate.getval("div.date_select_group.startdate");
				var endArrr=slideDate.getval("div.date_select_group.enddate");
				$("#beginyear").val(beginArrr[0]);
				$("#beginmonth").val(parseInt(beginArrr[1])+1);
				$("#beginday").val(beginArrr[2]);
				$("#endyear").val(endArrr[0]);
				$("#endmonth").val(parseInt(endArrr[1])+1);
				$("#endday").val(endArrr[2]);
				slideDate.close();
				$("div.transaction_details_con_head").find("div").eq(2).addClass("traction_current").siblings().removeClass();
			})
			slideDate.setval("div.date_select_group.startdate");
			slideDate.setval("div.date_select_group.enddate");
		})
		var swiper = new Swiper('.transac_detail_tb', {
	        scrollbar: '.swiper-scrollbar',
	        direction: 'vertical',
	        slidesPerView: 'auto',
	        mousewheelControl: true,
	        freeMode: true
	    });
	})
})