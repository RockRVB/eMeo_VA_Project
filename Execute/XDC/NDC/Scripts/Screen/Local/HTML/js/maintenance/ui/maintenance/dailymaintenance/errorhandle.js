
(function(window){
	function ErrorHandle(errorCode)
	{
		// 当前错误索引
		this.CurIndex=1;
		// 当前错误码
		this.ErrorCode=errorCode;
		//错误处理XML路径
		this.XmlConfigPath="";
		//首要提示信息
		this.MainMsg="";
		
		this.xmlobject=[];
		
		this.SwitchImage;
	};
	
	ErrorHandle.prototype={
		// 添加构造函数
		constructor: ErrorHandle,
		// 初始化
		init: function() {
		    var self = this;
		//加载错误码xml
				$.ajax({
					url: this.XmlConfigPath,
					dataType: 'xml',
					type: 'GET',
                    async:false,
					timeout: 2000,
					error: function(xml) {
						Config.log("加载菜单XML文件出错!");
					},
					success: function(xml) {
						//解析动画对象
					    var root = $(xml).find(Config.Lang + ":eq(0)");
						var SwitchItem=
						{
							"ImageUrl":"",
							"Voice":"",
							"Text":""
						};
						
						self.MainMsg=$(root).find("TXT:eq(0)").attr("value");

					    if ($(root).find("Am").length>0) {
					        SwitchItem.ImageUrl = $(root).find("Am").find("img0").attr("img");
					        SwitchItem.Voice = $(root).find("Am").find("img0").attr("voice");
					        SwitchItem.Text = $(root).find("Am").find("img0").attr("txt");

                            if(self.SwitchImage==undefined)
					        {
					            self.SwitchImage = {};
					        }
					        self.SwitchImage["AM"] = SwitchItem;
					    }

                        var SwitchItem2=
						{
							"ImageUrl":"",
							"Voice":"",
							"Text":""
						};

					    if ($(root).find("RBT") .length>0) {
					        SwitchItem2.ImageUrl = $(root).find("RBT").find("img0").attr("img");
					        SwitchItem2.Voice = $(root).find("RBT").find("img0").attr("voice");
					        SwitchItem2.Text = $(root).find("RBT").find("img0").attr("txt");

                            if(self.SwitchImage==undefined)
					        {
					            self.SwitchImage = {};
					        }
					        self.SwitchImage["RBT"] = SwitchItem2;
					    }


					    //解析帮助图片
						var singleItem;
						for(var i=1;i<100;i++)
						{
                            var SwitchItem3=
						    {
							    "ImageUrl":"",
							    "Voice":"",
							    "Text":""
						    };

							singleItem=$(root).find("IMG").find("img"+i);
							
							if(singleItem.length<1)
							{
								return;
							}
							
							SwitchItem3.ImageUrl=$(singleItem).attr("img");
							SwitchItem3.Voice=$(singleItem).attr("voice");
							SwitchItem3.Text=$(singleItem).attr("txt");
							
							self.xmlobject.push(SwitchItem3);
							
						}
						
					}
				});
		},
		
		// 是否具有上一页
		hasPer:function()
		{
			if(this.CurIndex>1)
			return true;
			else
			return false;
		
		},
		//是否具有下一页
		hasNext:function()
		{
			if(this.CurIndex<this.xmlobject.length)
			return true;
			else
			return false;
		},
		// 获取当前
		getCurErrorItem: function()
		{
			if(this.xmlobject.length>=this.CurIndex)
			{
				return this.xmlobject[this.CurIndex-1];
			}
			
		},
		
	// 设置帮助文件路径
	SetXmlConfigPath:function( xmlConfigPath){
		this.XmlConfigPath= xmlConfigPath;
	},
	
	
	
	};
	
	//------------------------------
	window.ErrorHandle=ErrorHandle;
	//-----------------------------
})(window)