/******************** 
	作用:表示单个菜单数据
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-22
********************/

(function(window) {
	function Menu(id, menu, visible, name, ui) {

		/** 菜单ID */
		this.id = "";


		/** 是否菜单(true:菜单 false:按钮) */
		this.isMenu = true;


		/** 菜单是否可见(true:可见 false:不可见) */
		this.visible = true;

		/** 菜单名称 */
		this.name = "";

		/** 菜单对应的界面 */
		this.ui = "";

		//---------------------额外属性---------------------

		/** 父级菜单ID */
		this.parentId = "";

		/** 菜单对应的按键 */
		this.key = "";

		/** 保存子菜单 */
		this.child = [];

		/** 父级菜单 */
		this.parent = null;

		//---------------------初始化
		this.id = (""+id).trim().toUpperCase();
		this.name = name;
		this.ui = ui;
		this.isMenu = menu == "1"; //是否菜单(true:菜单 false:按钮)
		this.visible = visible == "1"; //菜单是否可见(true:可见 false:不可见)

		//获取父级菜单ID
		var len = this.id.length - 1;
		if (len > 0) {
			this.parentId = this.id.substr(0, len);
		}
	}

	Menu.prototype = {
		constructor: Menu,
		//获取菜单级别
		level: function() {
			try {
				return this.id.length;
			} catch (e) {
				Config.log(e);
			}
		},
		//获取一级菜单ID
		topId: function() {
			try {
				return this.id.charAt(0);
			} catch (e) {
				Config.log(e);
			}
		},
		//获取菜单显示名称
		text: function() {
			try {
				return this.name;
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
		//复制菜单
		clone: function() {
			var cmenu = null;
			try {
				cmenu = new Menu("1","1","1","","");
				cmenu.id = this.id;
				cmenu.isMenu = this.isMenu;
				cmenu.visible = this.visible;
				cmenu.name = this.name;
				cmenu.ui = this.ui;
				cmenu.parentId = this.parentId;
				//cmenu.key = this.key;//不复制提示键
				cmenu.child = this.child;
				cmenu.parent = this.parent;
			} catch (e) {
				Config.log(e);
			}
			return cmenu;
		}

	};
	//--------------------------------
	window.Menu = Menu;
	//--------------------------------
})(window);