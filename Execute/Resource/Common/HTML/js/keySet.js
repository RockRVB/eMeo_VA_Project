require(['config/config','main'],function(){
	require(['jquery'],function(){
		$("#input_area>ul").on("click","button",function(){
			var _this=$(this);
			var initval=($("div.key_input_con").attr("data-key")==undefined?"":$("div.key_input_con").attr("data-key"));
			if(initval.length<32){
				$("div.key_input_con").attr("data-key",initval+_this.attr("data-keycode"));
				if(initval.length%8==0&&initval.length!=0){
					$("div.key_input_con").text($("div.key_input_con").text()+"  "+"*");
					return;
				}
				$("div.key_input_con").text($("div.key_input_con").text()+"*");
			}
		})
		$(".enter_button.reset_button").on("click",function(){
			$("div.key_input_con").attr("data-key","");
			$("div.key_input_con").text("");
		})
	})
})
