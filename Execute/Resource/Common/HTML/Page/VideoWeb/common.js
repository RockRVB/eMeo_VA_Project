const MessageType = { "RequestCmd": 8001, "ResponseCmd": 8002, "PostEventCmd": 8003 }
// function GetWebSocket() {
// 	return window.localStorage.getItem("ws");
// }
var ws;
var connectionState = false; //连接状态
var myDate = new Date();

// 获取当前时间
function getNowTime() {
	let dateTime
	let yy = new Date().getFullYear()
	let mm = new Date().getMonth() + 1
	let dd = new Date().getDate()
	let hh = new Date().getHours()
	let mf = new Date().getMinutes() < 10 ? '0' + new Date().getMinutes()
		:
		new Date().getMinutes()
	let ss = new Date().getSeconds() < 10 ? '0' + new Date().getSeconds()
		:
		new Date().getSeconds()
	let ms = new Date().getMilliseconds() < 10 ? '0' + new Date().getMilliseconds()
		:
		new Date().getMilliseconds()
	dateTime = yy + '-' + mm + '-' + dd + ' ' + hh + ':' + mf + ':' + ss + "." + ms;
	console.log(dateTime)
	return dateTime
}

function WebSocketInit() {
	//ws = new WebSocket("ws://10.1.24.129:8863/cmd");
	//判断是否已经连接
	if (connectionState && ws.readyState == WebSocket.OPEN){
		
		return;
	}
	//获取ip及端口
	var ip = "localhost";
	var port = "10086";
	var url = "/VideoWebCmd";
	//判空
	if (ip == null || ip == "" || port == null || port == "" || url == null || url == "") {
		//layer.msg('请检查ip及端口及url是否为空');
		
		return;
	}
	var newurl = "ws://" + ip + ":" + port + url;
	ws = new WebSocket(newurl);
	//window.localStorage.setItem("ws", ws);
	if ("WebSocket" in window) {

		ws.onopen = function () {
			// Web Socket 已连接上，使用 send() 方法发送数据
			//setTimeout(function(){ 
			//ws.send("12");
			//}, 0);
			

			connectionState = true;
			console.log("连接成功...");
		};

		ws.onmessage = function (evt) {
			
				console.log("数据已接收..." + received_msg);
			
		};

		ws.onerror = function (evt) {
			
			console.log("onerror:" + evt.data);
		};

		ws.onclose = function () {
			
			// 关闭 websocket
			console.log("onclose")
			console.log("连接已关闭...");
		
		};

	}

	else {
		// 浏览器不支持 WebSocket
		console.log("您的浏览器不支持 WebSocket!");
	}
}




