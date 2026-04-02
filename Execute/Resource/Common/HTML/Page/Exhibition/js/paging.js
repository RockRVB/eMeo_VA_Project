$("#continue").hide();
var curPage;
$(".showPage").bind('focus',function(){
	curPage = 1;
	$(".showPage").attr("disabled",true);
	var d = $("#showPage");
	d.empty();
	d.css("float","bottom");
	d.css("position","absolute");
	d.css("width","45%");
	d.css("height","70%");
	d.css("left","27.5%");
	d.css("top","18%");
	d.css("border",1);
	d.css("background-color","#ffffff");
	d.css("z-index",999);
	var name = this.name;
	var id = this.id;
	id='"'+id+'"';
	var r = $("#"+name);
	name='"'+name+'"';
	var options= r.children("option");
	$(options).each(function(){
		if($(this).css("display")=="none"){
			$(this).empty();
		}
	})
	var optionContent = "<div id='optionContent' style='width:100%;height:89%;display:none;'></div>";
	d.append(optionContent);
	var Content = $("#optionContent");
	var title="<div style='background-color:#ffffff;padding-bottom:10px;padding-top:10px;text-align:center;'>"+this.id+"<div style='float:right;' onclick='hideDiv()'><span style='padding: 0 2vh;'>X</span></div></div>";
	Content.append(title);
	var osize = options.length;
	for(var i=0;i<=7;i++){
		if (i>=osize){
			break;
		}else{
			var div = "<div class='itemhover' onclick='checkAll(this,"+name+","+i+")' style='width:100%;height:11%;padding-top:10px;'><span style='width:100%;height:5%;padding-left:20px;cursor:pointer'>"+options[i].text+"</span></div>";
		}
		Content.append(div);
	}
	$(".itemhover").hover(function(){
		$(this).css('background-color','rgb(243,243,243)');
	},function(){
		$(this).css('background-color','#ffffff');
	})
	var pages = osize%8==0?osize/8:Math.floor(osize/8)+1;
	var pageNumDiv ="<div id ='pageNum' style='background-color:#ffffff;text-align:center;'></div>";
	d.append(pageNumDiv);
	for (var j=1;j<=pages;j++){
		if(j==1){
			var left = "<div style='float:left;'><img src='media/leftIcon.png' style='padding: 0 2vh;' id='leftIcon' name='leftIcon' onclick='lrgoPage(this)'/></div>";
			$("#pageNum").append(left);
		}
		var span ="<span onclick='goPage("+j+","+name+","+id+")'>"+j+"</span>";
		$("#pageNum").append(span);
		$("#pageNum span:first-of-type").addClass('pageNum_color');
        $("#pageNum span").click(function(event) {
                  $(this).addClass('pageNum_color').siblings('#pageNum span').removeClass('pageNum_color');
				  curPage = $(this).text();
        });
        if(j==pages){
        	var right = "<div style='float:right;'><img src='media/rightIcon.png' style='padding: 0 2vh;' id='rightIcon' name='rightIcon' onclick='lrgoPage(this)'/></div>";
        	$("#pageNum").append(right);
        }
	}	
	Content.show();
	d.show();
});
function lrgoPage(lr){
	var lrr = $(lr);
	if(lrr.attr("name")=='leftIcon'){
		if(curPage==1){
			return;
		}else{
			nextPage = curPage-1;
			$("div[id ='pageNum']").find('span').each(function(){
				if($(this).text()==nextPage){
					$(this).click();
				}
			});
		}
	}else{
		nextPage = parseInt(curPage)+1;
		$("div[id ='pageNum']").find('span').each(function(){
			if($(this).text()==nextPage){
				$(this).click();
			}
		});
	}
}

function goPage(num,sname,id){
	var d = $("#showPage");
	$("#optionContent").empty();
	var r = $("#"+sname);
	var newname=sname;
	sname='"'+sname+'"';
	var options= r.children("option");
	$(options).each(function(){
		if($(this).css("display")=="none"){
			$(this).empty();
		}
	})
	var Content = $("#optionContent");
	var title="<div style='background-color:#ffffff;padding-bottom:10px;padding-top:10px;text-align:center;'>"+id+"<div style='float:right;width:30px;height:30px;position:relative;cursor:pointer;right:5px;' onclick='hideDiv()'><span style='font-weight:bolder;font-size:28px;color:rgb(233,233,233)'>X</span></div></div>";
	Content.append(title);
	var osize = options.length-1;
	var pageStart;
	var pageEnd;
	if(num==1){
		pageStart = 0;
		pageEnd =7;
	}else{
		pageStart = (num-1)*8;
		pageEnd = num*8-1;
	}
	for(var i=pageStart;i<=pageEnd;i++){
		if (i>osize){
			break;
		}else{
			var div = "<div class='itemhover' onclick='checkAll(this,"+sname+","+i+")' style='width:100%;height:11%;padding-top:10px;'><span style='width:100%;height:5%;padding-left:20px;cursor:pointer'>"+options[i].text+"</span></div>";
		}
		Content.append(div);
	}
	$(".itemhover").hover(function(){
	$(this).css('background-color','rgb(243,243,243)');
	},function(){
		$(this).css('background-color','#ffffff');
	})
	Content.show();
	d.show();
}

function checkAll(node,sname,count){
	var newname=sname;
	$("select[name="+sname+"] option:eq("+count+")").prop("selected", true);
	$("textarea[name="+newname+"]").val($(node).text())
	$("#showPage").css('display','none');
	$(".showPage").attr("disabled",false);
	if($.isFunction(window.changeItems)){
		changeItems();
	}
	 $("textarea[name="+newname+"]").autoTextarea(); 
	/*//长度自适应
	var textLen= $(node).text().length;*/
	
	// if((textLen *15+60)>=600){
	// 	$("textarea[name="+newname+"]").css('width','400px');
	// 	var rowsNu= textLen/18;
	// 	$("textarea[name="+newname+"]").attr("rows",parseInt(rowsNu));
	// 	$(this).height(this.scrollHeight);
	// }else {		

	// 	$("textarea[name="+newname+"]").attr("rows",1);
	// 	$("textarea[name="+newname+"]").css('width',(textLen *15+110)+'px');

	// }
	//$("textarea[name="+newname+"]").val($(node).text());
	//alert($("select[name="+sname+"]").val());
	var inputs=$(".showPage");
	var flag = true;
	inputs.each(function(){
		if($(this).val()==""){
			flag=false;
			return false;
		}
	})
	if(flag){
			$("#continuegreyout").remove();
			$("#continue").show();
		
	}
}
function hideDiv(){
	$(".showPage").attr("disabled",false);
	$("#showPage").css('display','none');
}