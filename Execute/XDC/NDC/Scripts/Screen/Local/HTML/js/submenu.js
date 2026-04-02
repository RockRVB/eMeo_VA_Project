require(['config/config','main'],function(config,m){
	require(['jquery'],function(){
		var subcount=(!window.external.GetData("core_activityTransNumber"))?5:(window.external.GetData("core_activityTransNumber"));
		var menuarr=['submenu_one','submenu_two','submenu_three','submenu_four','submenu_five'];
		$(".submenu_lis").removeClass().addClass("submenu_lis "+menuarr[subcount-1]+" clear");
	})
})