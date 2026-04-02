/******************** 
	作用:自定义菜单按钮
	作者:adam
	版本:V1.0
	时间:2015-08-14
********************/

(function(window) {
	function DefineMenu(menu) {

		
		//------------------属性---------------------------
        /** 菜单XML数据源对象*/
	    this.menu;
        /** 一级菜单 */
	    this.headMenu = [];
        /** 二级菜单 */
        this.child = [];
        /** 已选菜单 */
	    this.selectMenu = [];
        /** 一级菜单按钮列表 */
	    this.headIDS = [];
        //------------------附加属性---------------------------
        /** html 显示模板 */
	    this.headMenuString = "";
	    this.childString = "";
	    this.selectMenuString = "";

        /** 子菜单按钮列表 */
	    this.childMenus = [];
        /** 当前子菜单显示页数 */
	    this.childPageIndex;
        /** 当前子菜单页显示控件个数 */
	    this.childPageSize;
        /** 当前子菜单页数 */
	    this.childPageCount;
        /** 当前页显示已选菜单 */
	    this.selectMenuShow = [];
        /** 当前页显示已选菜单个数 */
	    this.selectMenuShowPageSize;
        /** 当前第几页显示已选菜单 */
	    this.selectMenuShowPageIndex;

	    //---------------------初始化
	    this.menu = String2XML(menu);

	    this.headIDS = [1, 2, 3, 4, 5, 6];

        this.childPageIndex=0;

	    this.childPageSize = 4;
        /**规定每一页显示4个已选菜单*/
        this.selectMenuShowPageSize=4;

	    this.selectMenuShowPageIndex = 1;

	    this.headMenuString = '<li>'+
						'<div class="bg"></div>'+
						'<a href="#">'+
						'<div class="middle-outer">'+
							'<div class="middle-inner"> <input id="MenuID{0}" class="none" value="{0}"></input> <span>{1}</span></div>'+
						'</div>'+
						'</a></li>';

	    this.childString = '<li>'+
						'<button id="BUTTON_{0}" type="button" class="button-define" isAutoSend="0" focusGroup="row1"> <span class="tip"></span>  <span class="button-text">{1}</span> </button>'+
					'</li>';

	    this.selectMenuString = '<li>' +
	        '<button id="FUN_{0}" type="button" class="button-define" isAutoSend="0" focusGroup="define"> <span class="tip"></span> <span class="button-text">{1}</span> </button>' +
	        '</li>';
	
	}
	// 添加属性方法
	DefineMenu.prototype = {
		// 添加构造函数
		constructor: DefineMenu,
        /** 初始化函数 */
        init: function() {
            //添加父菜单
            this.addHeadMenu();
            // 父菜单初始化
            this.HeadMenuInit();

        },
        /** 添加父菜单 */
        addHeadMenu: function() {
            var str = "";
            var menuitem;   
            for (var i = 0;i < this.headIDS.length; i++) {
                menuitem = this.getMenuInfo(this.headIDS[i]);
                if (menuitem != undefined && menuitem.visible == "1") {
                    str += this.headMenuString.format(menuitem.id, menuitem.name);
                }
                
            }

            $(".level-1 ul").html(str);

        },

        HeadMenuInit: function() {
            var id;
            //初始化第一个按钮选中状态(这里有逻辑问题，当第一个按钮没有孩子节点时，应该寻找第二个按钮选中)
            //todo 还未实现
            $(".level-1 ul li").eq(0).addClass("selected");
            var m_this = this;
            //注册点击事件
            $(".level-1 ul li").each(
                function() {
                    id=$(this).find("input").val();
                    if (m_this.hasChild(id)) {
                        $(this).click(
                            function() {
                                $(".level-1 ul li").removeClass("selected");
                                $(this).addClass("selected");
                                id=$(this).find("input").val();
                                // 初始化孩子节点
                                m_this.addChild(id);
                                
                            }
                        );
                    } 
                    else {
                        // 这里要实现 父菜单 按钮变灰无法点击
                        //todo 还未实现
                    }
                }
            );
            // 需要初始化按钮可点击属性disabled
            
            id = $(".level-1 ul .selected").find("input").val();

            if (this.hasChild(id)) {
                this.addChild(id);
            }

        },

        addChild: function(id) {
            var menuitem;
            var m_this = this;
            //必须初始化
            m_this.childMenus = [];
            // 遍历所有的孩子节点（需要写逻辑，取出最多16个先显示，如果超过16个则需要控制翻页逻辑）
            $(this.menu).find("config").children().each(
		        function() {
		            var newid = $(this).attr("id");
			        if(newid.indexOf(id)==0&&newid!=id&&$(this).attr("visible")=="1"&&!m_this.hasChild(newid)) {
			            menuitem = m_this.getMenuInfo(newid);
			            m_this.childMenus.push(menuitem);
			        }
		        }
		    );
            //显示按钮
            m_this.showChild(1);
        },
        // 必须从第一页开始显示 ,显示定制菜单
        showChild: function(index) {

            //生成按钮
            var j = 4;
            var str="";
            this.childPageIndex = index;
            var pageCount = 0;
            if (this.childMenus.length % this.childPageSize > 0) {
                pageCount = parseInt(this.childMenus.length / this.childPageSize) + 1;
            } else {
                pageCount = parseInt(this.childMenus.length / this.childPageSize);
            }

            this.childPageCount = pageCount;

            if (index > pageCount)
                return;
            
            if (this.childMenus.length < this.childPageSize * index) {
                j = this.childMenus.length - this.childPageSize * (index- 1);
            }

            var baseindex = this.childPageSize * (index - 1);


            for (var i = 0; i < j; i++) {
                str += this.childString.format(this.childMenus[baseindex+i].id,this.childMenus[baseindex+i].name);
            }
            

            $(".choice-box ul").html(str);

            this.initChild();

            // 设置翻页按钮逻辑
            this.setPageNavBox();
        },
        /**设置子控件翻页按钮，并且注册事件 */
        setPageNavBox: function() {
            var m_this = this;

            $("#FUN_DOWN").unbind("click");
            $("#FUN_UP").unbind("click");

            if (m_this.childPageIndex == 1) 
            {
                $("#FUN_UP").addClass("disabled");  
                
            } else {
                $("#FUN_UP").removeClass("disabled");
                //注册事件
                $("#FUN_UP").click(
                    function() {
                        if (m_this.childPageIndex - 1 < 1)
                            m_this.showChild(1);
                        else {
                            m_this.showChild(m_this.childPageIndex - 1);
                        }
                    }
                );

            }

            if (m_this.childPageCount <= m_this.childPageIndex) {
                $("#FUN_DOWN").addClass("disabled");
                 

            } else {
                $("#FUN_DOWN").removeClass("disabled");
                //注册事件
                $("#FUN_DOWN").click(
                    function() {
                        if (m_this.childPageIndex + 1 > m_this.childPageCount) 
                        {
                            m_this.showChild(m_this.childPageCount);
                        } 
                        else 
                        {
                            m_this.showChild(m_this.childPageIndex + 1);
                        }

                    }
                );
            }

        },

		//初始化子按钮
        initChild: function() {
            var m_this = this;
            var id;
            var menuitem;
            $(".choice-box li").each(
                function() {
                    //如果该按钮在已选菜单列表中则，则不显示它，设置透明度为0
                    id = $(this).find("button").attr("id");
                    id = id.replace("BUTTON_", "");
                    menuitem = m_this.getMenuInfo(id);
                    $(this).unbind("click");

                    if (m_this.findMenuIndex(m_this.selectMenu, menuitem) >=0) //该按钮已选则隐藏它
                    {
                        $(this).css("opacity", "0");
                        $(this).find("button").css("cursor", "default");
                        
                    } 
                    else // 注册事件
                    {
                        $(this).click(
                        function() 
                        {
                            if (m_this.selectMenu.length < 19) {

                                $(this).css("opacity", "0");
                                $(this).find("button").css("cursor", "default");
                                $(this).unbind("click");
                                //需要实现 把选中的按钮放置到已选列表中
                                //已经实现了
                                id = $(this).find("button").attr("id");
                                id = id.replace("BUTTON_", "");
                                menuitem = m_this.getMenuInfo(id);
                                m_this.addSelectMenuShow(menuitem);
                            } 
                            else {
                                //向后台发送超过可选最大数量的限制
                                // todo 已经实现 FUN_199
                                var i = {
                                    action: "click",
                                    data: {
                                        id: "FUN_199"
                                    }
                                };
                                Config.send(i);
                            }
                        }
                    );
                    }

                    


                }
            );
        },


        //-------------------------------------------------------------------------------
        //已选按钮功能模块
        //--------------------------------------------------------------------------------
        /** 添加当前显示已选按钮*/
        addSelectMenuShow: function(o) {

            try {
                if (this.selectMenu.length>=19) {
                    // 实现逻辑，向后台发送数据 提示用户超过19个菜单啦
                    // todo 已经实现 FUN_199
                    var i = {
                                    action: "click",
                                    data: {
                                        id: "FUN_199"
                                    }
                                };
                    Config.send(i);
                    return;
                }

                if (o == undefined)
                    return;

                if (this.selectMenuShow.length < this.selectMenuShowPageSize) {
                    this.selectMenuShow.push(o);
                } else {
                    //显示的页数加1
                    this.selectMenuShowPageIndex++;
                    this.selectMenuShow = [];
                    this.selectMenuShow.push(o);
                }
                //已选按钮总组
                this.selectMenu.push(o);
                
                //显示当前页的已选按钮
                this.showSelectMenuShow(this.selectMenuShowPageIndex);


            } catch (e) {                   
                Config.log(e);
            } 


		},

        /** 删除当前已选按钮*/
        delSelectMenuShow   : function(o) {

            try {
                var index = this.findMenuIndex(this.selectMenuShow, o);
                if (index < 0 || index > this.selectMenuShow - 1) {
                    return false;
                }
                // 从数组中删除一个元素
                this.selectMenuShow.splice(index,1);

                index =this.findMenuIndex( this.selectMenu,o);
                //从总组中也删除一条数据
                this.selectMenu.splice(index, 1);
                
                return true;

            } catch (e) {
                Config.log(e);
                return false;
            } 


		},
        //显示选择菜单栏
        showSelectMenuShow: function(index) {
            try {
                var str = "";
                var baseIndex = 0;
                this.selectMenuShowPageIndex = index;
                var j = this.selectMenuShowPageSize;
                var pageCount = 0;
                //修正总页数
                if (this.selectMenu.length%this.selectMenuShowPageSize>0) 
                {
                    pageCount = parseInt(this.selectMenu.length / this.selectMenuShowPageSize) + 1;
                } 
                else {
                    pageCount = parseInt(this.selectMenu.length / this.selectMenuShowPageSize);
                }

                // 显示逻辑有问题 已经修改

                if (this.selectMenu.length < this.selectMenuShowPageIndex * this.selectMenuShowPageSize) {
                    j =  this.selectMenu.length-(this.selectMenuShowPageIndex-1) * this.selectMenuShowPageSize;
                }

                baseIndex = this.selectMenuShowPageSize * (this.selectMenuShowPageIndex - 1);


                this.selectMenuShow = [];
            for (var i = 0; i < j; i++) {

                str  += this.selectMenuString.format(this.selectMenu[baseIndex+i].id,this.selectMenu[baseIndex+i].name );
                this.selectMenuShow.push(this.selectMenu[baseIndex+i]);
            }
            //当没有已选按钮时需要占位控件
            if (str == undefined || str == "") 
            {
                str='<li style="visibility: hidden;">' +
	        '<button id="FUN_0" type="button" class="button-define" isAutoSend="0" focusGroup="define"> <span class="tip"></span> <span class="button-text"></span> </button>' +
	        '</li>';

                $("#SELECTED_PRE_PAGE").addClass("none");
                $("#SELECTED_NEXT_PAGE").addClass("none");
            } else {
                $("#SELECTED_PRE_PAGE").removeClass("none");
                $("#SELECTED_NEXT_PAGE").removeClass("none");
            }
            //显示已选菜单按钮
            $(".seleted-box .nav-zone ul") .html(str);

            this.initSelectMenuShow();
            this.setSelectMenuShowNav();

            } catch (e) {
                Config.log(e);
            } 
		},
        /**初始化已选菜单*/
        initSelectMenuShow: function() {
            var m_this = this;
            var id;
            var menuitem;
            $(".seleted-box .nav-zone ul li").each(
                function() {

                    $(this).click(
                        function() {
                            // 从当前显示已选列表中移除

                            id = $(this).find("button").attr("id");
                                id = id.replace("FUN_", "");
                                menuitem = m_this.getMenuInfo(id);

                                m_this.delSelectMenuShow(menuitem);

                            if (m_this.selectMenuShow.length==0) 
                            {
                                if (m_this.selectMenuShowPageIndex>1) {
                                    //页数减1
                                    m_this.selectMenuShowPageIndex--;
                                    m_this.selectMenuShow = [];

                                    //从总表中拷贝出新的已选按钮填充
                                    for (var i = 0; i < m_this.selectMenuShowPageSize; i++) {
                                        m_this.selectMenuShow.push(m_this.selectMenu[(m_this.selectMenuShowPageIndex-1) * m_this.selectMenuShowPageSize + i]);
                                    }
                                    
                                }
                                
                            }

                            m_this.showSelectMenuShow(m_this.selectMenuShowPageIndex);

                            //显示子菜单（当在已选菜单中删除该按钮后，需要在可选子菜单中把其取消隐藏让其可选）
                            m_this.showChild(m_this.childPageIndex);
                        }
                    );


                }
            );

        },
        /**设置已选菜单翻页按钮*/
        setSelectMenuShowNav: function() {
        var m_this = this;
        var pageCount = 0;
        if (this.selectMenu.length % this.selectMenuShowPageSize > 0) {
            pageCount =parseInt( this.selectMenu.length / this.selectMenuShowPageSize )+ 1;
        } else {
            pageCount =parseInt( this.selectMenu.length / this.selectMenuShowPageSize );
        }

        $("#SELECTED_PRE_PAGE").unbind("click");
         $("#SELECTED_NEXT_PAGE").unbind("click");

        if (this.selectMenuShowPageIndex > 1) {
            
            $("#SELECTED_PRE_PAGE").removeClass("disabled");
            $("#SELECTED_PRE_PAGE").click(
                function() {
                    m_this.selectMenuShowPageIndex--;
                    m_this.showSelectMenuShow(m_this.selectMenuShowPageIndex);

                  // m_this.RemoveValueDataNNumber();
                  // m_this.setSelectValue();

                }
            );
        } 
        else {
            $("#SELECTED_PRE_PAGE").addClass("disabled");
            
        }

        if (this.selectMenuShowPageIndex < pageCount) {

        $("#SELECTED_NEXT_PAGE").removeClass("disabled");
            $("#SELECTED_NEXT_PAGE").click(
                function() {
                    m_this.selectMenuShowPageIndex++;
                    m_this.showSelectMenuShow(m_this.selectMenuShowPageIndex);

                  // m_this.RemoveValueDataNNumber();
                  // m_this.setSelectValue();
                }
            );

        } else {
            $("#SELECTED_NEXT_PAGE").addClass("disabled");
           
        }


        },

		

//--------------------------------------------------------
// 公共方法
//----------------------------------------------------------

         HandleKeyboardAction:function(key)
    {
        var result=false;
        switch (key) {
            // case "ENTER":
            // return alert(111);
            // case "CANCEL":
            // return alert(222);
            case "1":
                $("button[value='Select1']").click();
                return true;
            //this.keyBoardAction($("button[value='Select1']").attr('id'),key);
            case "2":
                $("button[value='Select2']").click();
                return true;
            case "3":
                $("button[value='Select3']").click();
                return true;
            case "4":
                $("button[value='Select4']").click();
                return true;
            case "5":
                $("button[value='Select5']").click();
                return true;
            case "6":
                $("button[value='Select6']").click();
                return true;
            case "7":
                $("button[value='Select7']").click();
                return true;
            case "8":
                $("button[value='Select8']").click();
                return true;
            case "9":
                $("button[value='Select9']").click();
                return true;
            case "ENTER":return this.keyBoardAction("KEYBOARD_ENTER",key);
            case "CANCEL":return this.keyBoardAction("FUN_QUIT",key);
            case "KEYBOARD_TAB":return this.SwitchMenu();
            // 后续插入快捷键等等
            //.........
            default:
                return this.keyBoardAction("keyboard",key);
        }
    },
 

        /** 添加已选按钮 */
        selectMenu: function(id) {
            try {

                var menuitem = this.getMenuInfo(id);
                if (menuitem == undefined)
                {return;}

                if (this.selectMenu.length >= 19) {
                    //需要发送消息到后台，或者自己弹框处理
                    // todo 已经实现
                    var i = {
                                    action: "click",
                                    data: {
                                        id: "FUN_199"
                                    }
                                };
                    Config.send(i);
                    return;
                }

                this.selectMenu.push(menuitem);


            } catch (e) {
                Config.log(e);  
            } 


		},


/** 获取菜单信息(包含菜单ID，菜单标题，菜单的显示情况) */
        getMenuInfo: function(id) {

            var result = {
						"id": id,
						"name": "",
                        "visible":"0"
					};

		    $(this.menu).find("config").children().each(
		    function()
		    {  
			    if($(this).attr("id")==id)
			    {
			        result.name=$(this).attr("name");
                    result.visible=$(this).attr("visible");
			        return false;
			    }
		    }
		);
		
		return result;
	    },
        /** 是否具有孩子菜单 */
        hasChild: function(id) {
            var result = false;
            
		    $(this.menu).find("config").children().each(
		    function()
		    {  
			    if($(this).attr("id").indexOf(id)==0&&$(this).attr("id")!=id&& $(this).attr("visible")=="1") {
			        result = true;
			        return false;
			    }
		    }
		);
            return result;
        },

        /**查找返回元素的位置*/
        findMenuIndex: function(list, o) {
            try {
            if (list==undefined) {
                return -1;
            }

            if (o == undefined)
                return -1;

                for (var i = 0; i < list.length; i++) {
                    if (list[i].id == o.id) {
                        return i;
                    }

                }

                return -1;

            } catch (e) {
                Config.log(e);
                return -1;

            } 
            

            

        }


	};
	//--------------------------------
	window.DefineMenu = DefineMenu;
	//--------------------------------
})(window);