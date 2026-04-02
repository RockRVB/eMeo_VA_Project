function buttonTouch(){
  var button = document.querySelectorAll("#btn_container li,.b_btn");
 
  for(var i = 0,len = button.length;i<len;i++){
      button[i].onmousedown = function(){
        this.classList.add("active");
      }
      button[i].onmouseup = function(){
        var self = this;
        setTimeout(function(){
          self.classList.remove("active");
        },260)
      }
  }
  
}
function showTime(id) {
  if(!document.getElementById(id)) return;
  var now = new Date();
  var year = now.getFullYear();
  var month = now.getMonth() * 1;
  var date = now.getDate();
  var day = now.getDay();
  var hour = now.getHours() * 1;
  var minutes = now.getMinutes() * 1;
  var second = now.getSeconds() * 1;
  month = month + 1;
  month < 10 ? month = '0' + month : month;
  hour < 10 ? hour = '0' + hour : hour;
  minutes < 10 ? minutes = '0' + minutes : minutes;
  second < 10 ? second = '0' + second : second;
  var now_time = '<div style="font-size:2rem">' + hour + ':' + minutes + ':' + '' + second + '</div>' + ' ' + year + '/' + month + '/' + date;
  document.getElementById(id).innerHTML = now_time;
  setTimeout("showTime('" + id + "')", 1000);
}
window.onload = function(){
  buttonTouch();
  //showTime("showTime");
}