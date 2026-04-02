/******************** 
	作用:错误处理页面
	作者:adam
	版本:V1.0
	时间:2015-09-16
********************/

(function(window) {

	function CustomUi() 
	{
	    this.InvalSeparator ='\x1B';
        // 错误帮助对象
		this.EHandle;
        // 错误帮助处理之前的提示图片
	    this.SwichImage = "";
        // 错误帮助配置文件路径
	    this.ErrorHandleXmlPath = "";
        // 是否关闭声音
	    this.IsMuted = false;

		CustomUi.superClass.constructor.call(this);
	}

	Config.extend(CustomUi, Interface);

	//预处理
	CustomUi.prototype.preInit = function() {
		this.cleanData();

	    var self = this;
        // 注册关闭声音按钮事件
	    $("#FUN_SOUND").on("click",function() {
	            var value = "";
            if (self.IsMuted) {
                value= Config.LanguageData.get("IDS_MuteMalfunctionGuide");
                self.IsMuted = false;
            } 
            else {
                value= Config.LanguageData.get("IDS_UnMuteMalfunctionGuide");
                self.IsMuted = true;
            }
			
			var span =($("#FUN_SOUND").children('span'))[1];	
			var spanTmp = $(span).text();
			var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
			var num=spanTmp.substr(0,1);
			if(!reg.test(num))
			{
				num="";
			}			
			var spanText = $(span).text(num + value);
	       // $(this).html(value);
            self.audioMute(self.IsMuted);
	    }
	    );
		
		$("#FUN_BACK").on("click", function(){
			if(Config.bQueryErrorCode && !Config.isShortcut)
			{
				// 直接页面切换
				Config.Ui.changeUi("Maintenance");// 注意：维护主界面UI 名称跟UIMapping.xml 中一致；
			}
			else
			{
				var value= $(this).attr("dataToSend");
				if(value==""|| value== undefined)
					value="FUN_BACK";
					
				var data = {
					"action": "click",
					"data": {
						"id": value
					}
				};
				Config.send(data);
			}
		}
		
		);
	};
	
	//
	//生成界面后
	CustomUi.prototype.afterInitUi = function() {
		try 
        {
			Config.sQueryErrorCodePreUI=Config.UI_MALFUNCTIONGUIDE;
			//如果单纯的错误查询 屏蔽复位按钮
			if(Config.bQueryErrorCode)
			{
				 $("#FUN_RESET").addClass("none");
			}
			else
			{
				$("#FUN_RESET").removeClass("none");
			}
			
        } catch (e) {
			Config.log(e);
		}

	};
	
	//清除界面信息,用于从头开始显示信息
	CustomUi.prototype.cleanData = function() {
	    $("#FUN_SOUND").unbind("click");
		$("#FUN_BACK").unbind("click");
	};

	//重写方法设置值
	CustomUi.prototype.setValue = function(data) {
		var result = false;
		try 
        {
			if (data.id=="SWITCH_IMAGE") {
			    this.SwichImage = data.value;
			    return true;
			}
			
			if (data.id=="TWIN_KEYBOARD")
			{
				    result = this.KeyAction(data.value);
			} 


            if (data.id=="STATIC_ERRORCODE") {
                setTimeout(
                
                function() {
                     this.uiInstance.InitErrorHandle(data.value);
                }, 10
                );
            } 

           result = CustomUi.superClass.setValue.call(this, data);


        } catch (e) {
			result = false;
			Config.log(e);
		}

		return result;
	};
	


	// 私有方法 设值
	CustomUi.prototype.InitErrorHandle= function(code)
	{
	    if (code != undefined && code != "") {

	        this.ErrorHandleXmlPath = this.getXmlConfigPath(code);
	        this.EHandle = new ErrorHandle(code);
	        this.EHandle.SetXmlConfigPath(this.ErrorHandleXmlPath);
	        this.EHandle.init();

	        this.initPage();
	    }
	};
	
	CustomUi.prototype.CheckUrl=  function CheckUrl(urlPath)
	{
		//var urlPath = "http://localhost/cmt/Release.zip";
		var xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
		xmlhttp.open("GET", urlPath, false);
		xmlhttp.send();
		if(xmlhttp.readyState == 4) 
		{
			if(xmlhttp.status == 200) 
			{
				return true;
			} //url存在
			else if(xmlhttp.status == 404) 
			{
				return false;
			} //url不存在
			else {
				return false;
			}//其他状态
		}
	}

     CustomUi.prototype.initPage = function()
    {
        if (this.EHandle.SwitchImage!=undefined) 
        {
            this.EHandle.CurIndex = 0;
            this.setSwitchImageAndTex();
        }

        if (this.EHandle.MainMsg != undefined && this.EHandle.MainMsg != "") {
            this.setValue({ "id": "STATIC_ERRORMSG", "value": this.EHandle.MainMsg });
        }
		//this.SetDefaultValue();
        this.setErrorHandleData();


    };
	// 设置初始化故障信息动画
    CustomUi.prototype.setSwitchImageAndTex = function()
    {
        try {
            CustomUi.superClass.setValue.call(this, {"id":"help-image","value":getRootPath_web() + "/Image/error/"+this.EHandle.SwitchImage[this.SwichImage].ImageUrl.replace("swf","gif")});
            this.setValue({ "id": "help-text", "value": "" }); // 清理帮助文字
        } catch (e) {
                Config.log(e);
        } 
        

    };
    //设置错误帮助指引图片文字
    CustomUi.prototype.setErrorHandleData = function()
    {
        try {
                if (this.EHandle.CurIndex > 0) 
                {
                    if (this.EHandle.xmlobject.length>0) 
					{
						var isUrlExit=this.CheckUrl(getRootPath_web() + "/Image/error/"+this.EHandle.xmlobject[this.EHandle.CurIndex-1].ImageUrl);
						var imgTmp = $('#help-image');
						var img1Tmp=$('#imgBlock');
						if(isUrlExit)
						{
							this.setValue({ "id": "help-image", "value": getRootPath_web() + "/Image/error/"+this.EHandle.xmlobject[this.EHandle.CurIndex-1].ImageUrl });
							imgTmp.height( '250px');
							img1Tmp.hide();
						}							
						else
						{
							this.setValue({ "id": "help-image", "value": "" });
							imgTmp.attr('src','');
							imgTmp.height( '100px');
							img1Tmp.show();
						}		
						this.setValue({ "id": "help-text", "value": this.EHandle.xmlobject[this.EHandle.CurIndex-1].Text });
                        //播放声音
                        if (this.EHandle.xmlobject[this.EHandle.CurIndex - 1].Voice != undefined && this.EHandle.xmlobject[this.EHandle.CurIndex - 1].Voice != "") 
						{
                            
                            var VoiceUrl = "";
                            if (Config.Lang == "CN")
                                VoiceUrl = getRootPath_web() + "/Voice/errorSound/" + this.EHandle.xmlobject[this.EHandle.CurIndex - 1].Voice;
                            else if (Config.Lang == "EN") {
                                VoiceUrl = getRootPath_web() + "/Voice/errorSound_EN/" + this.EHandle.xmlobject[this.EHandle.CurIndex - 1].Voice;
                            } else {
                                VoiceUrl = getRootPath_web() + "/Voice/errorSound/" + this.EHandle.xmlobject[this.EHandle.CurIndex - 1].Voice;
                            }

                            this.audioPlay(VoiceUrl);
                        }
                    }
                }

                this.initButNav();

        } catch (e) {
            Config.log(e);
        } 
        

    };
    // 初始化翻页
    CustomUi.prototype.initButNav = function() {

        var self = this;
        $("#FUN_UPSTEP").unbind("click");

        $("#FUN_DWONSTEP").unbind("click");

        if (this.EHandle.hasPer()) 
        {
            this.setAttribute({ "id": "FUN_UPSTEP", "attribute": { "enabled": "1", "visible": "1", "selected": "0" } });
            $("#FUN_UPSTEP").on("click",
                function() 
                {
                    if (self.EHandle.CurIndex > 1) {
                        self.EHandle.CurIndex--;
                        self.setErrorHandleData();
                    }
                }
            );  
        } 
        else {
             this.setAttribute({ "id": "FUN_UPSTEP", "attribute": { "enabled": "0", "visible": "1", "selected": "0" } });
            
        }

        if (this.EHandle.hasNext()) {
            this.setAttribute({ "id": "FUN_DWONSTEP", "attribute": { "enabled": "1", "visible": "1", "selected": "0" } });
            $("#FUN_DWONSTEP").on("click",
                function() 
                {
                    self.EHandle.CurIndex++;
                    self.setErrorHandleData();
                }
            );

        } else {
            
            this.setAttribute({ "id": "FUN_DWONSTEP", "attribute": { "enabled": "0", "visible": "1", "selected": "0" } });
        }
		
		
		

    };
        
     //银屏播放
    CustomUi.prototype.audioPlay= function(src)
    {
         try {
				if (this.hasElement("audioPlayer")) 
				{
						var widget=this.getElementByName("audioPlayer");
						if(widget==null){
							result = "";
						}else{
							if(widget.type!="raw")// 非原生的HTML控件，而是自定义控件类型
							{
                                widget.element.setValue(src, Config.CMD_ATTRIBUTE_TEXT);
							    widget.element.play();
							}
							
						}
					
				} else 
				{
					if(this.getChildUiInstance()!=undefined)
					{
						result=this.getChildUiInstance().getAttribute(data);
						// 往子页面找控件处理
					}
				}
			} catch (e) {
				result = "";
				Config.log(e);
			}

    };
    // 关闭声音和打开声音
     CustomUi.prototype.audioMute= function(istrue)
    {
         try {
				if (this.hasElement("audioPlayer")) 
				{
						var widget=this.getElementByName("audioPlayer");
						if(widget==null){
							result = "";
						}else{
							if(widget.type!="raw")// 非原生的HTML控件，而是自定义控件类型
							{
                                if(istrue)
							    widget.element.muted();
                                else {
                                    widget.element.unMuted();
                                }
							}
							
						}
					
				} else 
				{
					if(this.getChildUiInstance()!=undefined)
					{
						result=this.getChildUiInstance().getAttribute(data);
						// 往子页面找控件处理
					}
				}
			} catch (e) {
				result = "";
				Config.log(e);
			}

    };


     //获取错误帮助配置文件路径
    CustomUi.prototype.getXmlConfigPath = function(code)
    {
         return getRootPath_web() + "/HTML/error/ErrorXml/"+code+".xml";

    };
	
	//设置按钮数字
	CustomUi.prototype.SetButtonNumberAgain= function()
	{
		this.RemoveValueDataNNumber();
		this.SetDefaultValue();
		return true;
	}
	
	//清除按钮数字
	CustomUi.prototype.RemoveValueDataNNumber= function()
	{
		var strTmp="";
		var buttonText=""
		var bottomButton = $("#FUN_RESET");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
  	}			
		bottomButton.val("");
		
		bottomButton = $("#FUN_SOUND");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
  	}			
		bottomButton.val("");
		
		bottomButton = $("#FUN_UPSTEP");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
  	}		
		bottomButton.val("");
		
		bottomButton = $("#FUN_DWONSTEP");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
  	}		
		bottomButton.val("");
		
		bottomButton = $("#FUN_BACK");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
  	}		
		bottomButton.val("");
		
		bottomButton = $("#FUN_LOGOUT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
  	}		
		bottomButton.val("");
		
		bottomButton = $("#FUN_QUIT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
  	}		
		bottomButton.val("");

		bottomButton = $("#FUN_TABMODE");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		var reg=/^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
		var num=strTmp.substr(0,1);
		if(reg.test(num))
		{
			buttonText.text(strTmp.substr(1,strTmp.length-1));
  	}		
		bottomButton.val("");
		return true;
	}
	
	//按键响应
    CustomUi.prototype.KeyAction= function(key)
	{
	    var result=false;
	    switch (key) {				
	    // case "ENTER":
		// return alert(111);
        // case "CANCEL":
		// return alert(222);
		case "1":						
			$("button[value='Select1']").click();
			$("#Select1").click();
			this.SetButtonNumberAgain();
			return true;
			//this.keyBoardAction($("button[value='Select1']").attr('id'),key);
		case "2":
			$("button[value='Select2']").click();
			$("#Select2").click();
			this.SetButtonNumberAgain();
			return true; 
		case "3":
			$("button[value='Select3']").click();
			$("#Select3").click();
			this.SetButtonNumberAgain();
			return true; 
		case "4":
			$("button[value='Select4']").click();
			$("#Select4").click();
			this.SetButtonNumberAgain();
			return true; 
		case "5":
			$("button[value='Select5']").click();
			$("#Select5").click();
			this.SetButtonNumberAgain();
			return true; 
		case "6":
			$("button[value='Select6']").click();
			$("#Select6").click();
			this.SetButtonNumberAgain();
			return true;  
		case "7":
			$("button[value='Select7']").click();
			$("#Select7").click();	
			this.SetButtonNumberAgain();
			return true;  
		case "8":
			$("button[value='Select8']").click();
			$("#Select8").click();
			SetButtonNumberAgain();
			return true;  
		case "9":
			$("button[value='Select9']").click();
			$("#Select9").click();
			this.SetButtonNumberAgain();
			return true;  
		case "0":
			return true;  
		case "CLEAR":
			return true;  
		case "ENTER":
			return this.keyBoardAction("KEYBOARD_ENTER",key);
        case "CANCEL":return this.keyBoardAction("FUN_QUIT",key);
	    case "KEYBOARD_TAB":return this.SwitchMenu();
        // 后续插入快捷键等等
        //.........
	    default:
	        return this.keyBoardAction("keyboard",key);
	    }
	};

	//一进入界面就给底部4个按钮赋值
    CustomUi.prototype.SetDefaultValue= function()
	{		
	    var result=false;
		var strTmp="";
		var buttonText=""
		var bottomButton = $("#FUN_RESET");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("1"+strTmp);
		bottomButton.val("Select1");
		
		bottomButton = $("#FUN_SOUND");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("2"+strTmp);
		bottomButton.val("Select2");
		
		bottomButton = $("#FUN_UPSTEP");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("3"+strTmp);
		bottomButton.val("Select3");
		
		bottomButton = $("#FUN_DWONSTEP");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("4"+strTmp);
		bottomButton.val("Select4");
		
		bottomButton = $("#FUN_BACK");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("5"+strTmp);
		bottomButton.val("Select5");
		
		bottomButton = $("#FUN_LOGOUT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("6"+strTmp);
		bottomButton.val("Select6");
		
		bottomButton = $("#FUN_QUIT");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("7"+strTmp);
		bottomButton.val("Select7");	

		bottomButton = $("#FUN_TABMODE");//document.getElementById("FUN_LOGOUT");
		buttonText = bottomButton.children(".button-text");
		strTmp=buttonText.text();
		buttonText.text("8"+strTmp);
		bottomButton.val("Select8");			
		return true;
	};
	

	//切换功能
	CustomUi.prototype.SwitchMenu= function()
	{		
	    var result=false;
		this.RemoveValueDataNNumber();
		switch (MenuFlag) {	
		case 0:
			MenuFlag=1;
			return this.SetDefaultValue();	
		case 1:
			MenuFlag=0;
			//return this.SetDefaultValue();				
		}
		
	};	
	
	//--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);

MenuFlag=0;

// 实例化
$(function() {
	new A();
});