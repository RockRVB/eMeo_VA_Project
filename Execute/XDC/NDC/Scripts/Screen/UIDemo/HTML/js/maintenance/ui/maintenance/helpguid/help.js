/******************** 
	作用:帮助界面
	作者:adam
	版本:V1.0
	时间:2015-09-22
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
	};
	
	//
	//生成界面后
	CustomUi.prototype.afterInitUi = function() {
		try 
        {
			
        } catch (e) {
			Config.log(e);
		}

	};
	
	//清除界面信息,用于从头开始显示信息
	CustomUi.prototype.cleanData = function() {
	    $("#FUN_SOUND").unbind("click");
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



            if (data.id=="STATIC_EXPCODE") {
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
		var xmlhttp = null;
		if(window.XMLHttpRequest){
			xmlhttp = new XMLHttpRequest();
		}else if(window.ActiveXObject){
			xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
		}
		
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


	//--------------------------------
	window.A = CustomUi;  //把CustomUi 这个对象赋值给window 公布出去
	//--------------------------------
})(window);


// 实例化
$(function() {
	new A();
});