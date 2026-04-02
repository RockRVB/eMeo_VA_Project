/**
 * 输入法类
 * 时间：2016年9月12日10:07:19
 * 作者：hcj
 * 版本：2.0
 */
(function(fac){
	if(typeof define === "function" && define.amd){
		define('inputClass',[],fac);//amd模式
	}else{
		//browser模式
		fac();
	}
}(function(){
	var url = "ws://127.0.0.1:18018";
    var ws = null,showState = null,currentEle = null,keybord = null;
    var bits=document.body.offsetWidth/1920;
    return {
    	init:function(){
    		try{
    			if ("WebSocket" in window) {
	                ws = new WebSocket(url);
	            }else if ("MozWebSocket" in window) {
	                ws = new MozWebSocket(url);
	            }else{
	            	console.log('浏览器不支持WebSocket');
	            }
    		}catch(e){
    			//TODO handle the exception
    			console.log("WebSocket连接出错了！");
    		}
            ws.onopen = function () {
            	console.log('连接服务器成功');
            }
            ws.onclose = function () {
            	console.log('与服务器断开连接');
            }
            ws.onerror = function () {
            	console.log('通信发生错误');
            }
            ws.onmessage = function (msg) {
                if (msg.data instanceof Blob) {
                    //服务器端发送的是BinaryMessage
            		console.log(msg.data.size);
                } else {
                    //服务器端发送的是TextMessage
            		console.log(msg.data);
            		var jsonobj=JSON.parse(msg.data);
            		if(jsonobj.MsgParams1=='OnConfirm'){
            			currentEle.blur();
            			this.slideHide();
            			if(keybord=="签名键盘"){
            				signBack(jsonobj.MsgParams2);
            			}
//          			document.querySelector(".enter_button").click();
            		}else if(jsonobj.MsgParams1=='OnClear'){
            			currentEle[0].value='';
            			currentEle[0].removeAttribute("data-val");
            			try{
							window.external.SetData("saveBuffer","");
						}catch(e){
							console.log("未检测到后台setdata方法！");
						}
            		}else if(jsonobj.MsgParams1=='OnClose'){
            			currentEle.blur();
            			this.slideHide();
            			if(keybord=="签名键盘"){
            				recovSign();
            			}
            		}
                }
            }.bind(this);
    	},
    	sendMsg:function(type,cmd,param1,param2,param3,param4,param5,param6){
    		if (!ws || ws.readyState != 1) {
		        this.init();
		    }
    		if (ws){
    			ws.send("{'MsgType':'"+type+"','MsgCMD':'"+cmd+"','MsgParams1':'"+param1+"','MsgParams2':'"+param2+"','MsgParams3':'"+param3+"','MsgParams4':'"+param4+"','MsgParams5':'"+param5+"','MsgParams6':'"+param6+"'}");
            }
    	}.bind(this),
    	show:function(mode,name,width,height,left,top){
    		if(ws){
    			this.sendMsg("Execute","Show",mode,name,width,height,left,top);
//  			showState = true;//一个页面只初始化窗口一次
    		}
    	},
    	hide:function(){
    		this.sendMsg("Execute","Hide","","","","","","");
    	},
    	slideShow:function(type,name,cmd,derection,startpos,endpos,time,it){
    		/**
    		 * startpos、endpos相对于derection的值代表x轴或y轴的坐标 
    		 **/
    		var type=type||"Execute",name=name||"拼音输入键盘",cmd=cmd||"Move",derection=derection||"VER",startpos=startpos||document.body.offsetHeight,time=time||1;
    		currentEle=it;
    		var height=bits*500;
    		if(!showState){
    			this.show("VK",name,document.body.offsetWidth,height,0,document.body.offsetHeight,time);
    			keybord=name;
    		}
    		this.sendMsg(type,cmd,derection,startpos,endpos,time,"","")
    		return true;
    	},
    	slideHide:function(time){
    		var time=time||0.6;
    		var endpos=document.body.offsetHeight;
    		this.sendMsg("Execute","Move","VER",document.body.offsetHeight-500*bits,endpos,time||0.6,"","");
    	},
    	setSize:function(setwidth,setheight){
    		this.sendMsg("SetWindowSize",setwidth,setheight);
    	},
    	sendBinary:function(){
    		if (ws) {
                //这个方法有点问题 没办法传大文件 待改善
                var fileInput = document.getElementById("file_input");
                if (fileInput.files.length > 0) {

                    var fileReader = new FileReader();

                    fileReader.onloadend = function (e) {
                        //此时的ws.binaryType="arraybuffer"
                        ws.send(this.result);//发送读取到的二进制流
                    }
                    fileReader.readAsArrayBuffer(fileInput.files[0]);
                }
            }
    	},
    	close:function(){
//  		this.slideHide(0.6);
    		this.hide();
    		ws.close();
    	}
    };
}))