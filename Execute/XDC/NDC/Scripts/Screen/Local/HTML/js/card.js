require(['config/config','main'],function(){
	require(['jquery','swiper','inputClass'],function($,Swiper,inputMs){
		//inputMs.init();
		$(document).ready(function(){
			var bits = $(window).width() / 1920,
			citycode = [10,20,21,22,23,24,25,27,28,29,310,311,312,313,314,315,316,317,318,319,335,349,350,351,352,353,354,355,356,357,358,359,370,371,372,373,374,375,376,377,378,379,391,392,393,394,395,396,398,410,411,412,413,414,415,416,417,418,419,421,427,429,431,432,433,434,435,436,437,438,439,451,452,453,454,455,456,457,458,459,464,467,470,471,472,473,474,475,476,477,478,479,482,483,510,511,512,513,514,515,516,517,518,519,523,527,530,531,532,533,534,535,536,537,538,539,543,546,550,551,552,553,554,555,556,557,558,559,561,562,563,564,565,566,570,571,572,573,574,575,576,577,578,579,580,591,592,593,594,595,596,597,598,599,631,632,633,634,635,660,662,663,668,691,692,701,710,711,712,713,714,715,716,717,718,719,722,724,728,730,731,732,733,734,735,736,737,738,739,743,744,745,746,750,751,752,753,754,755,756,757,758,759,760,762,763,766,768,769,770,771,772,773,774,775,776,777,778,779,790,791,792,793,794,795,796,797,798,799,812,813,816,817,818,825,826,827,830,831,832,833,834,835,836,837,838,839,851,852,853,854,855,856,857,858,859,870,871,872,873,874,875,876,877,878,879,883,886,887,888,891,892,893,894,895,896,897,898,901,902,903,906,908,909,911,912,913,914,915,916,917,919,930,931,932,933,934,935,936,937,938,939,941,943,951,952,953,954,955,970,971,972,973,974,975,976,979,990,991,993,994,995,996,997,998,999];
			$("div.card_input_con_group_select").parent().on("click touch",function(){
				$("div.job_container").hide().show();
				$("div.job_container_back").css("bottom",0-$("div.job_container_back").height()).animate({"bottom":0})
				var setheight=0-$("div.job_container_back").height()/2-$("div.job_container_lis").css("margin-top").split("px")[0];
				$("div.ibank_container").css("overflow","hidden").find(".card_input_container").animate({"top":setheight})
				new Swiper('.job_container_lis', {
			        scrollbar: '.swiper-scrollbar2',
			        direction: 'vertical',
			        slidesPerView: 'auto',
			        mousewheelControl: true,
			        preventLinksPropagation: false,
			        scrollbarDraggable : true,
					scrollbarHide: false,
			        freeMode: true
			    });
			})
			$("div.job_container").on("click",function(){
				$("div.job_container_back").animate({"bottom":0-$("div.job_container_back").height()}).parents("div.job_container").fadeOut(1000);
				$("div.ibank_container").css("overflow","hidden").find(".card_input_container").animate({"top":0})
			}).on("click",".job_container_back",function(){
				return false;
			}).on("click","td",function(){
				var _this=$(this);
				$("div.job_container_back").animate({"bottom":0-$("div.job_container_back").height()}).parents("div.job_container").fadeOut(1000);
				$("#career_type").val(_this.find("span").text());
				$("#career_type").prev().val(_this.prev().find("span").text());
				$("div.ibank_container").css("overflow","hidden").find(".card_input_container").animate({"top":0})
			})
			$("body").on('close','input',function(){
				//inputMs.close();
			})
			// $("div.card_input_con_group").on('propertychange input','#UserTelNo',function(){
				// var inputstate=true;
				// if($(this).attr("data-length")!==undefined&&$(this).val().length>$(this).attr("data-length")){
					// inputstate=true;
				// }else{
					// inputstate=false;
				// }
				// $(this).attr("data-length",$(this).val().length);
				// for(var i=0;i<citycode.length;i++){
					// if($(this).val().substr(0,$(this).val().length-1)==('0'+citycode[i])&&inputstate){
						// $(this).val($(this).val().substr(0,$(this).val().length-1)+"-"+$(this).val().substr($(this).val().length-1,1));
					// }
				// }
			// })
			/*$("div.card_input_con_group").on('focus','input',function(){
				if($(this).parents("li").position().top<170*bits){
					swiper.setWrapperTransition(1000);
					swiper.setWrapperTranslate(-200*bits);
				}
				var keytype=$(this).attr("keytype");
				var isshow=inputMs.slideShow("Execute",keytype||"拼音输入键盘","Move","VER",$(window).height(),$(window).height()-500*bits,0.6,$(this));
				if(isshow){
					$(this).parent().addClass("acitveinput");
					$("div.ibank_container").css("overflow","hidden").find(".card_input_container").animate({"top":0-300*bits});
				}
			}).on('blur','input',function(){
				$("div.ibank_container").css("overflow","hidden").find(".card_input_container").animate({"top":0})
				$(this).parent().removeClass("acitveinput");
				inputMs.slideHide();
				var _this=$(this);
				if(_this.val().trim()!=""){
					if(!new RegExp(_this.attr("pattern")).test(_this.val())){
						_this.parent().addClass("error_msg");
						$("div.error_msg_tips").text(_this.prev().text().replace(/:|：/g,"")+$("#pattern_unmatch_tip").text()).show();
					}else{
						$(this).parent().removeClass("error_msg");
					}
				}else{
					if($(this).parent().hasClass("required_input")){
						$(this).parent().addClass("error_msg");
						$("div.error_msg_tips").text($("#required_input_tip").text()).show();
					}else{
						$(this).parent().removeClass("error_msg");
					}
				}
				
				if($("div.error_msg").length==0){
					$("div.error_msg_tips").hide();
				}
			})*/
			$(".required_input").find("input").blur(this,function(){
					if(!$(this).val()){
						$(this).parent().addClass("error_msg");
						$("div.error_msg_tips").text($("#required_input_tip").text()).show();
					}
					else
					{
						$(this).parent().removeClass("error_msg");
						$("div.error_msg_tips").text($("#required_input_tip").text()).hide();
					}
					})

			$(document).on("click",".forword_button,.button-forword",function(){
				counts=0;
				$(".required_input").find("input").each(function(k,v){
					if(!$(v).val()){
						$(this).parent().addClass("error_msg");
						$("div.error_msg_tips").text($("#required_input_tip").text()).show();
						counts=counts+1;
					}
					else
					{
						$(this).parent().removeClass("error_msg");
						$("div.error_msg_tips").text($("#required_input_tip").text()).hide();
					}

					if(counts==0)
					{
						$(this).parent().removeClass("error_msg");
						$("div.error_msg_tips").text($("#required_input_tip").text()).hide();
					}
					else
					{
						//$(this).parent().addClass("error_msg");
						$("div.error_msg_tips").text($("#required_input_tip").text()).show();
					}
				})
				var reg=/\b0\d{2}\d{8}|\b0\d{2}\d{7}|\b0\d{3}\d{7}|\b0\d{3}\d{8}/;
				if($("#UserTelNo").val().trim()!=""&&!$("#UserTelNo").val().trim().match(reg)){
					$("#UserTelNo").parent().addClass("error_msg");
					$("div.error_msg_tips").text($("#UserTelNo").prev().text().replace(/:|：/g,"")+$("#pattern_unmatch_tip").text()).show();					
				}else{
					$("#UserTelNo").parent().removeClass("error_msg");
				}
				var regUserMobileNo=/^[1][3-8]\d{9}$/;
				if($("#UserMobileNo").val().trim()!="")
				{
					if(!$("#UserMobileNo").val().trim().match(regUserMobileNo)){
						$("#UserMobileNo").parent().addClass("error_msg");
						$("div.error_msg_tips").text($("#UserMobileNo").prev().text().replace(/:|：/g,"")+$("#pattern_unmatch_tip").text()).show();					
					}else{
						$("#UserMobileNo").parent().removeClass("error_msg");
					}
				}
				var regValidDate = /(([0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3})-(((0[13578]|1[02])-(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)-(0[1-9]|[12][0-9]|30))|(02-(0[1-9]|[1][0-9]|2[0-8]))))|((([0-9]{2})(0[48]|[2468][048]|[13579][26])|((0[48]|[2468][048]|[3579][26])00))-02-29)/;

				if($("#UserDateOfBirth").val().trim()!="")
				{
					var IdCardDOB=$("#UserDateOfBirth").val().trim();
					if(IdCardDOB.length==8)
					{
						IdCardDOB=IdCardDOB.substring(0,4)+'-'+IdCardDOB.substring(4,6)+'-'+IdCardDOB.substring(6,8);
					}
					if(!IdCardDOB.match(regValidDate)){
						$("#UserDateOfBirth").parent().addClass("error_msg");
						$("div.error_msg_tips").text($("#UserDateOfBirth").prev().text().replace(/:|：/g,"")+$("#pattern_unmatch_tip").text()).show();					
					}else{
						$("#UserDateOfBirth").parent().removeClass("error_msg");
					}
				}

				if($("#UserIDCardValidityPeriod").val().trim()!="")
				{
					var IdCardValidityPeriod=$("#UserIDCardValidityPeriod").val().trim();
					var IdCardValidityBegin;
					var IdCardValidityEnd;
					if(IdCardValidityPeriod.length>10)
					{
						IdCardValidityBegin= IdCardValidityPeriod.substring(0,4)+'-'+IdCardValidityPeriod.substring(4,6)+'-'+IdCardValidityPeriod.substring(6,8);

						IdCardValidityEnd= IdCardValidityPeriod.substring(9,13)+'-'+IdCardValidityPeriod.substring(13,15)+'-'+IdCardValidityPeriod.substring(15,17);
					}
					else
					{
						IdCardValidityBegin=IdCardValidityPeriod;
					}
					if(!IdCardValidityBegin.match(regValidDate)||!IdCardValidityEnd.match(regValidDate)){
						$("#UserIDCardValidityPeriod").parent().addClass("error_msg");
						$("div.error_msg_tips").text($("#UserIDCardValidityPeriod").prev().text().replace(/:|：/g,"")+$("#pattern_unmatch_tip").text()).show();					
					}else{
						$("#UserIDCardValidityPeriod").parent().removeClass("error_msg");
					}
				}

				//swiper.setWrapperTransition(1000);
				//swiper.setWrapperTranslate($(".card_input_container_group").height()-$(".swiper-wrapper").height());
			})
			if($("#career_type").prev().val()=="01")
			{
				$("#career_type").val($(".job_container_lis").find("tr").eq(0).find("td").eq(1).find("span").text()).prev().val($(".job_container_lis").find("tr").eq(0).find("td").eq(0).find("span").text());
			}
			else
			{
				careernum=$("#career_type").prev().val();
				careernum=parseInt(careernum);
				$("#career_type").val($(".job_container_lis").find("tr").eq(careernum-1).find("td").eq(1).find("span").text()).prev().val($(".job_container_lis").find("tr").eq(careernum-1).find("td").eq(0).find("span").text());
			}
		})
		var swiper = new Swiper('.card_input_container_group', {
	        scrollbar: '.swiper-scrollbar1',
	        direction: 'vertical',
	        slidesPerView: 'auto',
	        mousewheelControl: true,
			preventLinksPropagation:false,
			scrollbarDraggable : true,
			scrollbarHide: false,
	        freeMode: true,
			noSwiping : true,
			noSwipingClass : 'stop-swiping'
	  	});
	})
		
})

