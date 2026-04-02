require(['config/config','main'],function(){
	require(['jquery','swiper','inputClass','checkClass'],function($,swiper,ins,check){
		ins.init();
		var swiper = new Swiper('.card_input_container_group', {
	        scrollbar: '.swiper-scrollbar',
	        direction: 'vertical',
	        slidesPerView: 'auto',
	        mousewheelControl: true,
	        scrollbarDraggable : true,
			scrollbarHide: false,
	        freeMode: true
	  	});
	  	$(document).ready(function(){
	  		var bits=$(window).width()/1920;
	  		if(check.getdata("core_SignatureFilePath")!='null'&&check.getdata("core_SignatureFilePath")!=''){
	  			$("div.card_input_con_group.sign_area").find("p").replaceWith('<img src="'+check.getdata("core_SignatureFilePath")+'" />');
	  		}
	  		$("div.card_input_con").on("click",".sign_enable",function(){
	  			var isshow=ins.slideShow("Execute","签名键盘","Move","VER",$(window).height(),$(window).height()-500*bits,0.6,$(this));
	  			if(isshow){
					$(this).addClass("acitveinput");
					$("div.ibank_container").css("overflow","hidden").find(".card_input_container").animate({"top":0-300*bits},600);
	  			}
	  		})
	  		$(".id_photo_contain").on("dblclick","img",function(){
	  			console.log('双击发生了');
	  		})
	  		window.signBack=function(src){
	  			if($("div.card_input_con_group.sign_area").find("p").length>0){
	  				$("div.card_input_con_group.sign_area").find("p").replaceWith('<img src="'+src+'" />');
	  			}else{
	  				$("div.card_input_con_group.sign_area").find("img").replaceWith('<img src="'+src+'" />');
	  			}
	  			$(".enter_button").removeClass("disabled_btn");
		  		$("div.ibank_container").css("overflow","hidden").find(".card_input_container").animate({"top":0},600);
				window.external.SetData("core_SignatureFilePath",src);
		  	}
	  		window.recovSign=function(){
		  		$("div.ibank_container").css("overflow","hidden").find(".card_input_container").animate({"top":0},600);
		  	}
	  	})
		  	
	})
})

