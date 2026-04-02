var html="<div class='card_container'><div class='card_container_title clear'><ul class='card_title_menu_five'>";
var i=0;
$(function(){
  setTimeout(startCreateNavBar,10);
});

function startCreateNavBar(){
  var navBarData=$("#allSteps").val();
  if(navBarData.length>0)
  {
    var items=navBarData.split(';');
    $.each(items,function(index,obj){
          CreateNavBar(obj,html,i);
    });
    html=html+"</ul></div></div>";
    $("header:first").append(html);
  }
}

function CreateNavBar (argument,html1,j) {
   if(argument.length>0)
   {
      var menuitem=argument.split(',');
      if(menuitem.length>=3&&menuitem[2]==-1)
      {
     i=j+1;
        var curmenu=$("#curStep").val();
        if(curmenu!=null&&curmenu!="")
        {
          curitem=parseInt(curmenu);
        }
        if(parseInt(menuitem[0])==curitem)
        {
            html=html1+"<li class='step_current'><em>"+i+"</em><div>"+menuitem[1]+"</div><span></span></li>";
        }
        else 
        {
           html=html1+"<li><em>"+i+"</em><div>"+menuitem[1]+"</div><span></span></li>";
        }
      }
   }// body...
}