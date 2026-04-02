/******************** 
	作用:用于解析维护状态下的菜单
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-22
********************/

(function(window) {
	function MaintenanceMenu(xml, control) {

		/** 合法的菜单名称1-9 A-Z */
		this.legalNames = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"];


		/** 采用树形方式保存菜单 */
		this.menus = [];


		/** 采用树形方式保存所有菜单 */
		this.allMenus = [];

		/** 采用线性方式保存菜单 */
		this.list = [];



		/** 按ID保存菜单,用于加快查找速度 */
		this.menuObject = {};

		/** 当前菜单 */
		this.currentMenu = null;

		/** 控制器 */
		this.control = null;

		/** 滚动分页数据 */
		this.scrolls = [];


		//---------------------初始化
		this.control = control;
		this.initPageData(); //初始化分页数据
		//生成菜单数据
		var menu;
		var self = this;
		//保存对应的中英文资源
		$(xml).find("config:eq(0)").children().each(function() {
			var id = $(this).attr("id");
			var isMenu = $(this).attr("menu");
			var visible = $(this).attr("visible");
			var name = $(this).attr("name");
			var ui = $(this).attr("ui");
			menu = new Menu(id, isMenu, visible, name, ui);
			if (menu.visible || menu.parentId == "")
			{
				self.list.push(menu);
				self.menuObject[menu.id] = menu;
			}
		});
		//-----------菜单排序
		this.list.sort(this.by("id"));
		//将菜单数据保存在menus数组里,采用树形方式保存
		//-----------设置父级和子级之间的链接关系
		var len = this.list.length;
		var parentMenu; //父级菜单
		for (var i = 0; i < len; i++) {
			menu = this.list[i];
			// console.log(menu.id);
			// continue;
			//一级菜单
			if (menu.parentId == "") {
				this.menus.push(menu); //一级菜单	
			} else {
				parentMenu = this.findParentMenu(menu); //找到父级菜单
				if (parentMenu != null) {
					parentMenu.child.push(menu);
					menu.parent = parentMenu;
				}
			}
		}
	}

	MaintenanceMenu.prototype = {
		constructor: MaintenanceMenu,
		by: function(name) {
			return function(o, p) {
				var a, b;
				if (typeof o === "object" && typeof p === "object" && o && p) {
					a = o[name];
					b = p[name];
					if (a === b) {
						return 0;
					}
					if (typeof a === typeof b) {
						return a < b ? -1 : 1;
					}
					return typeof a < typeof b ? -1 : 1;
				} else {
					throw ("error");
				}
			}
		},
		//按id排序
		sortOnId: function(a, b) {
			var result = -1;
			try {
				var al = a.id.length;
				var bl = b.id.length;
				if (al < bl) {
					result = -1;
				} else if (al == bl) {
					if (a.id < b.id) {
						result = -1;
					} else if (a.id == b.id) {
						result = 0;
					} else {
						result = 1;
					}
				} else {
					result = 1;
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//初始化分页数据
		initPageData: function() {
			try {
				var length = this.control.menuCounts.length;
				var pageData;
				for (var i = 0; i < length; i++) {
					pageData = new PageData();
					pageData.pageSize = this.control.menuCounts[i];
					this.scrolls.push(pageData);
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//清除所有菜单的按键提示
		clearAllKey: function() {
			try {
				var menu = null;
				var len = list.length;
				for (var i = 0; i < len; i++) {
					menu = this.list[i];
					menu.key = "";
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//找到父级菜单
		findParentMenu: function(menu) {
			try {
				var parentMenu = this.findMenuById(menu.parentId);
				return parentMenu;
			} catch (e) {
				Config.log(e);
			}
		},
		//按ID查找菜单
		findMenuById: function(id) {
			try {
				var menu = null;
				if (undefined != this.menuObject[id]) {
					menu = this.menuObject[id];
				}
				return menu;
			} catch (e) {
				Config.log(e);
			}
		},
		//根据级别获取父级菜单ID
		findParentId: function(level) {
			try {
				return this.id.substr(0, level);
			} catch (e) {
				Config.log(e);
			}
		},
		//获取菜单ID的排序位置
		getSortIndex: function(id) {
			var index = -1;
			try {
				var len = this.legalNames.length;
				for (var i = 0; i < len; i++) {
					if (this.legalNames[i] == id) {
						index = i;
						break;
					}
				}

			} catch (e) {
				Config.log(e);
			}
			return index;
		}

	};
	//--------------------------------
	window.MaintenanceMenu = MaintenanceMenu;
	//--------------------------------
})(window);