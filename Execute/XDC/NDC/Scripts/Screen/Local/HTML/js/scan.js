/**
 * 高拍仪实例
 * 时间：2016年10月20日08:27:20
 * 作者：hcj
 */
require(['config/config','main'],function(){
	require(['jquery','swiper','imgview'],function(){
		var myswiper=function(){
			var spaceBetween=$(window).width()/1920*20;
			new Swiper('.photo_scan_area_list', {
		        pagination: false,
		        slidesPerView: 6,
		        spaceBetween: spaceBetween,
		        freeMode: false
		    })
		};
		myswiper();
		var photoarr=[];
		$("div.photo_scan_area_wrapper").on("click","div.photo_show",function(){
			var _this=$(this);
			if(_this.html()!=""){
				window.external.PopupEventRaised('OpenZoomPage');
				$(this).imgview({src:_this.find("img").attr("src"),picarr:photoarr});
			}
		})
	    window.takephoto=function(photolist){
	    	if(photolist!=""||photolist.length!=0){
	    		$(".enter_button").removeClass("disabled_btn");
	    		photolist=photolist.split(",");
		    	photoarr=photolist;
				$("div.photo_scan_area_wrapper").find("div.photo_show").eq(0).html('<img src="'+photolist[0]+'" />');
//		    	for (var i=0;i<photolist.length;i++) {
//		    		if(i>5){
//		    			$("div.photo_scan_area_wrapper").append('<div class="photo_show swiper-slide"><img src="'+photolist[i]+'" /></div>');
//		    			myswiper();
//		    		}else{
//		    			$("div.photo_scan_area_wrapper").find("div.photo_show").eq(i).html('<img src="'+photolist[i]+'" />');
//		    		}
//		    	}
		    	$("div.photo_scan_area_wrapper").find("div.photo_show").each(function(k,v){
		    		if(k+1>photolist.length){
		    			$(this).html("");
		    		}
		    	})
		    	$("div.photo_scan_area_tit").find("span").text(photolist.length);
	    	}else{
	    		$("div.photo_scan_area_wrapper").find("div.photo_show").each(function(k,v){
		    		$(this).html("");
		    	})
	    		$("div.photo_scan_area_tit").find("span").text(photolist.length);
	    	}
	    }
	    $(".cancel_button,.button-cancel").click(function(){
	    	$(".photo_show").html("");
	    	if($(this).hasClass("cancel_button")){
	    		$(".enter_button,.button-enter").addClass("disabled_btn");
	    	}else{
	    		$(".enter_button,.button-enter").addClass("button-disabled");
	    	}
	    })
	})
})