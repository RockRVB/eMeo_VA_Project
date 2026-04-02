/******************** 
	作用:用于维护状态下菜单的显示与隐藏,导航文字的显示与隐藏等
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-22
********************/

(function(window) {
	function MaintenanceView(xml, model, control) {

		/** 用于显示文字 */
		this.xml = 0;

		/** 用于保存菜单数据 */
		this.model = 6;

		/** 控制器 */
		this.control = 1;

		/** 当前界面容器 */
		this.screen = 1;

		this.xml = xml;
		this.model = model;
		this.control = control;
		this.screen = control.screen;
		this.fourBut = "<button id='{1}' class='flow-button-level4 button-common' type='button'>{0}</button>";
		this.saveMenuButtons(); //保存一二三级菜单按钮到相应的数组中
		this.getDefaultMainMenu(); //获取默认显示第几个一级菜单
		this.showMenu(1);
	}

	MaintenanceView.prototype = {
		constructor: MaintenanceView,
		//保存一二三级菜单按钮到相应的数组中
		saveMenuButtons: function() {
			try {
				var elementMc;
				var i = 1;
				var tempArray;
				var tempPageButtonArray;
				var pageButtonMc;
				var self = this;
				//保存菜单按钮
				for (var j = 0; j < this.control.menuCounts.length; j++) {
					tempArray = [];
					tempPageButtonArray = [];
					//分页按钮
					pageButtonMc = this.control.getElementByName("MENU" + (j + 1) + "_PRE_PAGE").element;
					pageButtonMc.element.attr("level", (j + 1));
					pageButtonMc.options.isAutoSend = false;
					pageButtonMc.options.eventData = {
						self: this
					};
					pageButtonMc.options.afterclick = this.menuPrePage;
					tempPageButtonArray.push(pageButtonMc); //菜单按钮向左翻页

					pageButtonMc = this.control.getElementByName("MENU" + (j + 1) + "_NEXT_PAGE").element;
					pageButtonMc.element.attr("level", (j + 1));
					pageButtonMc.options.isAutoSend = false;
					pageButtonMc.options.eventData = {
						self: this
					};
					pageButtonMc.options.afterclick = this.menuNextPage;
					tempPageButtonArray.push(pageButtonMc); //菜单按钮向右翻页

					$(".level-" + (j + 1) + " a").each(function(index, el) {
						elementMc = $(this).attr({
							"index": index,
							"level": j + 1
						});
						tempArray.push(elementMc);
					}).on("click", this, this.clickMenu);

					this.control.menuButtons.push(tempArray);
					this.control.menuPageButtons.push(tempPageButtonArray);
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//菜单按钮向左翻页
		menuPrePage: function(event, data) {
			try {
				var level = $(this).attr("level");
				level = parseInt(level);
				var self = data.self;
				self.model.scrolls[level - 1].prePage();
				self.showMenu(level);
			} catch (e) {
				Config.log(e);
			}
		},
		//菜单按钮向左翻页
		menuPrePageAction: function(event, data) {
			try {

			} catch (e) {
				Config.log(e);
			}
		},
		//菜单按钮向右翻页
		menuNextPage: function(event, data) {
			try {
				var level = $(this).attr("level");
				level = parseInt(level);
				var self = data.self;
				self.model.scrolls[level - 1].nextPage();
				self.showMenu(level);
			} catch (e) {
				Config.log(e);
			}
		},
		//菜单按钮向右翻页
		menuNextPageAction: function(btnMc) {
			try {

			} catch (e) {
				Config.log(e);
			}
		},
		//点击菜单按钮
		clickMenu: function(event) {
			try {
				event.data.menuAction($(this).attr("level"), $(this).attr("index")); //菜单对事件作出反应
			} catch (e) {
				Config.log(e);
			}
		},
		//点击菜单按钮(支持对按键作出反应)
		clickMenuAction: function(btnMc) {
			try {

			} catch (e) {
				Config.log(e);
			}
		},
		//菜单对事件作出反应
		menuAction: function(level, index) {
			try {
				$(".level-" + level + " a:eq(" + index + ")");
				var id = $(".level-" + level + " a:eq(" + index + ")").attr("idMark"); //菜单的ID
				this.showCascadeMenuById(id); //根据菜单ID显示级联菜单
				if (this.model.currentMenu.isMenu) {
					if (this.model.currentMenu.level() == 1) {
						this.sendMenuId(this.model.currentMenu.id);
					} else {
						//计算子级按钮个数
						if (this.model.currentMenu.level() == 2) {
							if (this.model.currentMenu.child.length == 1)
								this.clickMenuAction(this.control.menuButtons[this.model.currentMenu.level()][0]);
						}
					}
				} else {
					this.sendMenuId(id);
				}
				// }
			} catch (e) {
				Config.log(e);
			}
		},
		//根据菜单ID显示级联菜单
		showCascadeMenuById: function(id) {
			try {
				this.model.currentMenu = this.model.findMenuById(id);
				Config.curMenuID=id; // 保存当前菜单ID
				if (this.model.currentMenu == null || (this.model.currentMenu.parent == null && this.model.currentMenu.level() != 1)) {
					this.control.showNavigationNotExit(id); //当菜单ID不在维护界面中时重新设置导航显示
				} else {
					try {
						//将它以下的菜单都隐藏
						var level = this.model.currentMenu.level(); //菜单级别
						//循环显示上面的菜单
						for (var i = 1; i <= level; i++) {
							this.showMenu(i);
						}
						this.control.setPositionByMenuLevel(level, this.model.currentMenu.isMenu); //根据菜单层级设置界面中各元件的位置
					} catch (err) {
						Config.log(err);
					}
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//获取默认显示第几个一级菜单
		getDefaultMainMenu: function() {
			try {
				var menu;
				var len = this.model.menus.length; //一级菜单个数
				for (var i = 0; i < len; i++) {
					menu = this.model.menus[i];
					if (menu.visible) {
						break;
					}
				}

				this.model.currentMenu = menu;
				//Config.Ui.currentModeUI == Config.UI_HANDLEGUIDE
				if (Config.tranData.handelGuide) 
				{
					//流程处理界面
					$("#handleguide-container").removeClass("none");
					//需要写代码发送第一个流程ID到后台
				} else 
				{
					$("#handleguide-container").addClass("none");
					this.sendMenuId(this.model.currentMenu.id); //发送一级菜单ID
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//显示第几层菜单(根据当前页数显示菜单)
		showMenu: function(level) {
			try {
				var buttonCount; //按钮个数
				var page; //按钮页数
				var pageData; //分页数据
				var indexArray; //当前页的索引列表
				if (level < 4) {
					pageData = this.model.scrolls[level - 1];
					buttonCount = pageData.pageSize; //按钮个数
				} else {
					//4级菜单
					buttonCount = this.model.currentMenu.child.length;
				}

				var menuArray; //子级
				var parentMenu; //父级菜单
				var parentId; //父级菜单ID
				var menu; //单个菜单数据
				var index = 0; //当前菜单索引
				var btnMc; //按钮
				var buttonStateArray = []; //保存按钮状态(显示且不是选中状态为1,其余为0)
				var i = 0;
				$(".level-" + level).removeClass("none");
				$(".level4-menu-button-zone").empty();
				$(".level4-menu-button-zone").addClass("none");
				this.control.getContainer().show();
				switch (level) {
					case 1: //显示一级菜单
						if (this.model.currentMenu.level() <= 2)
							$(".level-" + 3).addClass("none");
						menuArray = this.model.menus;

						pageData.totalCount = menuArray.length; //菜单总数
						indexArray = pageData.data(); //当前页的索引列表

						for (i = 0; i < buttonCount; i++) {
							btnMc = this.control.menuButtons[level - 1][i]; //按钮
							btnMc.parent("li").removeClass("selected");
							//先判断菜单是否存在,如果存在则显示,不存在则隐藏
							index = indexArray[i];
							if (i >= indexArray.length) {
								//后面已经没有菜单,将按钮隐藏起来
								btnMc.parent("li").addClass("none");
								buttonStateArray.push(0);
							} else {
								//显示菜单
								btnMc.parent("li").removeClass("none");
								//判断菜单visible是否启用,如果为false,则将其禁用
								menu = menuArray[index];
								if (menu.visible) {
									//启用按钮
									btnMc.parent("li").removeClass("disable");
									btnMc.attr("idMark", menu.id); //菜单的ID
									//判断是否当前菜单,显示不同的菜单背景
									parentId = this.model.currentMenu.findParentId(level);
									if (this.model.currentMenu.level() >= level && parentId == menu.id) { //当前菜单
										btnMc.parent("li").addClass("selected");
										buttonStateArray.push(0);
									} else { //其它菜单
										btnMc.parent("li").removeClass("selected");
										buttonStateArray.push(1);
									}
								} else {
									//禁用按钮
									btnMc.parent("li").addClass("disable");
									btnMc.parent("li").find("span").css("color","gray");
									buttonStateArray.push(1);
								}
								btnMc.find('span').html(menu.text());
							}
						}
						//----------------设置分页按钮状态
						this.setPageState(level); //设置对应级别菜单的分页按钮状态
						//----------------显示对应的子菜单
						if (this.model.currentMenu.level() == level && this.model.currentMenu.isMenu) {
							//先重置第2,3级菜单的当前页数
							this.model.scrolls[1].currentPage = 1; //重设回第1页
							this.model.scrolls[2].currentPage = 1; //重设回第1页
							this.showMenu(level + 1);
						}
						break;
					case 2: //显示2级菜单
						if (this.model.currentMenu.level() <= level)
							$(".level-" + 3).addClass("none");
						//获取当前菜单对应的父级菜单
						parentMenu = this.model.findMenuById(this.model.currentMenu.findParentId(level - 1)); //父级菜单
						menuArray = parentMenu.child;

						pageData.totalCount = menuArray.length; //菜单总数
						indexArray = pageData.data(); //当前页的索引列表
						for (i = 0; i < buttonCount; i++) {
							btnMc = this.control.menuButtons[level - 1][i]; //按钮
							btnMc.parent("li").removeClass("selected");
							btnMc.parent("li").removeClass("parent");
							//先判断菜单是否存在,如果存在则显示,不存在则隐藏
							index = indexArray[i];
							if (i >= indexArray.length) {
								//后面已经没有菜单,将按钮隐藏起来
								btnMc.parent("li").addClass("none");
								buttonStateArray.push(0);
							} else {
								//显示菜单
								btnMc.parent("li").removeClass("none");
								//判断菜单visible是否启用,如果为false,则将其禁用
								menu = menuArray[index];
								if (menu.visible) {
									//启用按钮
									btnMc.parent("li").removeClass("disable");
									btnMc.attr("idMark", menu.id); //菜单的ID
									//判断是否当前菜单,显示不同的菜单背景
									parentId = this.model.currentMenu.findParentId(level);
									if (this.model.currentMenu.level() >= level && parentId == menu.id) {
										if (this.model.currentMenu.level() == level && !this.model.currentMenu.isMenu) {
											//当前菜单
											btnMc.parent("li").addClass("selected");
											buttonStateArray.push(0);
										} else {
											btnMc.parent("li").removeClass("selected");
											buttonStateArray.push(1);
											btnMc.parent("li").addClass("parent");
										}
									} else { //其它菜单
										btnMc.parent("li").removeClass("selected");
										buttonStateArray.push(1);
									}
								} else {
									//禁用按钮
									btnMc.parent("li").addClass("disable");
									btnMc.parent("li").find("span").css("color","gray");
									buttonStateArray.push(1);
								}
								btnMc.find('span').html(menu.text());
							}
						}
						//----------------设置分页按钮状态
						this.setPageState(level); //设置对应级别菜单的分页按钮状态
						if (this.model.currentMenu.level() == level && this.model.currentMenu.isMenu) {
							//先重置第3级菜单的当前页数

							this.model.scrolls[2].currentPage = 1; //重设回第1页
							this.showMenu(level + 1);
						}
						break;
					case 3: //显示3级菜单
						//获取当前菜单对应的父级菜单
						parentMenu = this.model.findMenuById(this.model.currentMenu.findParentId(level - 1)); //父级菜单
						menuArray = parentMenu.child;

						pageData.totalCount = menuArray.length; //菜单总数
						indexArray = pageData.data(); //当前页的索引列表
						for (i = 0; i < buttonCount; i++) {
							btnMc = this.control.menuButtons[level - 1][i]; //按钮
							btnMc.parent("li").removeClass("selected");
							//先判断菜单是否存在,如果存在则显示,不存在则隐藏
							index = indexArray[i];
							if (i >= indexArray.length) {
								//后面已经没有菜单,将按钮隐藏起来
								btnMc.parent("li").addClass("none");
								buttonStateArray.push(0);
							} else {
								//显示菜单
								btnMc.parent("li").removeClass("none");
								//判断菜单visible是否启用,如果为false,则将其禁用
								menu = menuArray[index];
								if (menu.visible) {
									//启用按钮
									btnMc.parent("li").removeClass("disable");
									btnMc.attr("idMark", menu.id); //菜单的ID
									//判断是否当前菜单,显示不同的菜单背景
									parentId = this.model.currentMenu.findParentId(level);
									if (this.model.currentMenu.level() >= level && parentId == menu.id) {
										//当前菜单
										btnMc.parent("li").addClass("selected");
										buttonStateArray.push(0);
									} else { //其它菜单
										btnMc.parent("li").removeClass("selected");
										buttonStateArray.push(1);
									}
								} else {
									//禁用按钮
									btnMc.parent("li").addClass("disable");
									btnMc.parent("li").find("span").css("color","gray");
									buttonStateArray.push(1);
								}
								btnMc.find('span').html(menu.text());
							}
						}
						//----------------设置分页按钮状态
						this.setPageState(level); //设置对应级别菜单的分页按钮状态
						if (this.model.currentMenu.level() == level && this.model.currentMenu.isMenu) {
							this.showMenu(level + 1);
						}
						break;
					case 4: //显示4级菜单
						parentMenu = this.model.findMenuById(this.model.currentMenu.findParentId(level - 1)); //父级菜单
						menuArray = parentMenu.child;
						this.control.getContainer().hide();
						for (i = 0; i < menuArray.length; i++) {
							menu= menuArray[i];
							this.ShowFourMenu(menu);
						}
						
						break;
				}
				this.showTitle(); //显示标题
			} catch (e) {
				Config.log(e);
			}
		},
		//显示4级菜单
		ShowFourMenu: function(menu) {
			try {
				var ul=$(".level4-menu-button-zone");
				if (menu.visible) 
				{
				    ul.removeClass("none");
					var self= this;
					if($(".flow-button-level4[id="+menu.id+"]")[0]==undefined)
					{
						ul.append(this.fourBut.format(menu.text(),menu.id));
						$(".flow-button-level4").unbind("click");
						$(".flow-button-level4").click(
							function()
							{
								self.sendMenuId(this.id);
								self.control.getContainer().show();
								ul.addClass("none");
							}
						);
					}
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//点击4级菜单
		clickFourMenu: function(btnMc) {
			try {

			} catch (e) {
				Config.log(e);
			}
		},
		//设置线条的显示和隐藏状态
		setLineState: function(buttonStateArray, level) {
			try {

			} catch (e) {
				Config.log(e);
			}
		},
		//设置对应级别菜单的分页按钮状态
		setPageState: function(level) {
			try {
				var pageData = this.model.scrolls[level - 1]; //当前级别的分页数据
				var preButtonMc = this.control.menuPageButtons[level - 1][0]; //前一页
				var nextButtonMc = this.control.menuPageButtons[level - 1][1]; //后一页
				//设置翻页状态
				if (pageData.isEmpty() || pageData.totalCount <= pageData.pageSize) {
					preButtonMc.hide();
					nextButtonMc.hide();
				} else {
					//设置是否启用
					if (pageData.isFirst())
						preButtonMc.hide();
					else
						preButtonMc.show();
					if (pageData.isLast())
						nextButtonMc.hide();
					else
						nextButtonMc.show();
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//显示标题
		showTitle: function() {
			try {
				
				var menu = this.model.currentMenu;
				var currentText = menu.text(); //当前菜单标题

				var textArray = [currentText]; //保存菜单标题
				while (menu.parent != null) {
					
					textArray.unshift(menu.parent.text());
					menu = menu.parent;
				}

				$("#nav-path li:gt(0)").remove();
				var str="";
				var ul=$("#nav-path ul");
				var len=textArray.length;

				$.each(textArray,function(index,value){
					ul.append("<li><span>"+value+"</span></li>");
					if(index<len-1){
						ul.append("<li><span>&gt;</span></li>");
					}else{
						ul.find("li:last").addClass("parent");
					}
				});
			} catch (e) {
				Config.log(e);
			}
		},
		//向服务器发送菜单ID
		sendMenuId: function(id) {
			try {
				this.control.isSendingSource = true;
				var tmpId = "1";
				if(id == "")
				{
					tmpId = "1";
				}
				else
				{
					tmpId = id;
				}				
				var data = {
					"action": "menu",
					"data": {
						"id": "MENU_" + tmpId
					}
				};
				Config.send(data);
			} catch (e) {
				Config.log(e);
			}
		}
	};
	//--------------------------------
	window.MaintenanceView = MaintenanceView;
	//--------------------------------
})(window);