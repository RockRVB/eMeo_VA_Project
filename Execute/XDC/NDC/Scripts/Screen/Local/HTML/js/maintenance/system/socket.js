/******************** 
	作用:用于和服务器建立socket连接,发送和接收消息,调用命令管理器解析命令
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-08
********************/

(function(window) {

	function Socket() {

	}

	//用于保存socket
	Socket.socket = null;

	//socket地址
	 Socket.url="ws://127.0.0.1:12306/MaintenanceMessagePusher";

	 //Socket.url="ws://localhost:12306/MessagePusher";
	//Socket.url = "ws://127.0.0.1:9999";

	//保存接收到的所有消息,用于异步处理,每一项都是一条命令
	Socket.queue = [];
	// 上一条消息
	Socket.lastMsg = "";
	//用于标记当前命令是否执行成功,调用命令管理器解析命令前标记为false,完成命令后标记为true
	Socket.completed = true;
	//消息的唯一标识
	Socket.Guid=null;
	//初始化socket
	Socket.init = function() {
		try {
			if(Socket.socket!=null)
			{
				Socket.socket.onclose=null;
				Socket.socket.onerror=null;
				Socket.socket.onopen=null;
				Socket.socket.onmessage=null;
				
				Socket.socket.close();
				Socket.socket=null;	
			}
			
			Config.log("socket初始化");
			Socket.socket = new WebSocket(Socket.url);
			Socket.completed=true;
		} catch (e2) {
			Config.log(e2);
		}

		// 打开事件注册
		Socket.socket.onopen = function(e) {
			try {
				Config.log("socket连接成功");
				var data = {
                    "action": "connected",
                    "success": true,
                    "message": "",
                    "data": []
                };
                Config.send(data);
			} catch (e) {
				Config.log(e);
			}

		};
		// 关闭事件注册
		Socket.socket.onclose = function(e) {
			try {
				Config.log("socket关闭");
				Socket.init();
			} catch (e) {
				Config.log(e);
			}
		};
		//收到消息事件注册
		Socket.socket.onmessage = function(e) {
			try {
				var message = JSON.parse(e.data);
				Config.log("【接收】"); //收到信息
				Config.log(e.data); //收到信息
				
				if(message.Guid!=undefined)
				{
					if(message.Guid==Socket.Guid) // 防止连续2条相同指令
					{
						return;
					}
					
					Socket.Guid=message.Guid;
				}
				if(Socket.completed)
				{
					Socket.completed=false;
					if(Socket.lastMsg!=""&&Socket.lastMsg==message)
					{
						return;
					}
					else
					{
						Socket.lastMsg=message;
						Config.Command.parseCommand(message.Data);
					}
				}
				// alert(message.name);
			} catch (e) {
				Config.log(e);
			}
		};
		// 出错事件注册
		Socket.socket.onerror = function(e) {
			try {
				Config.log("socket连接出错");
				Socket.init();
			} catch (e) {
				Config.log(e);
			}

		};
	};

	//向服务器发送消息
	Socket.send = function(data) {
		if(Socket.Guid!=null)
		data.Guid=Socket.Guid;
		else
		data.Guid="0000";
		
		data = JSON.stringify(data);
		Socket.socket.send(data);
		Socket.completed=true;
		Config.log("【发送】"); //发送消息
		Config.log(Socket.getTime() +" "+data); //发送消息
	}
	Socket.getTime = function CurentTime()
    { 
        var now = new Date();
        
        var year = now.getFullYear();       //年
        var month = now.getMonth() + 1;     //月
        var day = now.getDate();            //日
        
        var hh = now.getHours();            //时
        var mm = now.getMinutes();          //分
        var ss = now.getSeconds();           //秒
        
        var clock = year + "-";
        
        if(month < 10)
            clock += "0";
        
        clock += month + "-";
        
        if(day < 10)
            clock += "0";
            
        clock += day + " ";
        
        if(hh < 10)
            clock += "0";
            
        clock += hh + ":";
        if (mm < 10) clock += '0'; 
        clock += mm + ":"; 
         
        if (ss < 10) clock += '0'; 
        clock += ss; 
        return(clock); 
}
	//--------------------------------
	if (window.Socket == undefined) {
		if (window.top.Socket == undefined)
			window.Socket = Socket;
		else
			window.Socket = window.top.Socket;
	}
	//--------------------------------
})(window);