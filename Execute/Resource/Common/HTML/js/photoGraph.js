require(['config/config','main'],function(){
	window.photoGraph=function(){
		if(arguments.length>0){
			document.querySelector("div.camera_contain").innerHTML='<img src="'+arguments[0]+'" />';
			document.querySelector("div.photo_contain").innerHTML='<img src="'+arguments[0]+'" />';
			document.querySelector(".cancel_button.disabled_btn").classList.remove("disabled_btn");
		}
	}
})

