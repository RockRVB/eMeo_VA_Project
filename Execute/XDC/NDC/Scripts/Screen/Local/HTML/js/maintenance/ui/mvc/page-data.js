/******************** 
	作用:保存翻页数据
	作者:蔡俊雄
	版本:V1.0
	时间:2015-05-22
********************/

(function(window) {
	function PageData() {

		/** 总数 */
		this.totalCount = 0;


		/** 每页的容量 */
		this.pageSize = 6;

		/** 当前页数(从1开始计数,1表示第1页) */
		this.currentPage = 1;
	}

	PageData.prototype = {
		constructor: PageData,
		//向前一页
		prePage: function() {
			try {
				if (!this.isFirst()) {
					this.currentPage--;
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//向后一页
		nextPage: function() {
			try {
				if (!this.isLast()) {
					this.currentPage++;
				}
			} catch (e) {
				Config.log(e);
			}
		},
		//跳到指定页数
		goto: function(page) {
			try {
				this.currentPage = page;
				this.currentPage = Math.max(this.currentPage, this.pageCount());
				this.currentPage = Math.min(this.currentPage, 1);
			} catch (e) {
				Config.log(e);
			}
		},
		//总页数
		pageCount: function() {
			try {
				return Math.ceil(this.totalCount / this.pageSize);
			} catch (e) {
				Config.log(e);
			}
		},
		//当前索引(从0开始计算)
		index: function() {
			try {
				return (this.currentPage - 1) * this.pageSize;
			} catch (e) {
				Config.log(e);
			}
		},
		//以数组返回当前页的所有索引数据
		data: function() {
			var result = [];
			try {
				var index = this.index();
				for (var i = index; i < index + this.pageSize; i++) {
					if (i < this.totalCount) {
						result.push(i);
					} else {
						break;
					}
				}
			} catch (e) {
				Config.log(e);
			}
			return result;
		},
		//判断是否首页
		isFirst: function() {
			try {
				return this.currentPage <= 1;
			} catch (e) {
				Config.log(e);
			}
		},
		//判断是否末页
		isLast: function() {
			try {
				return this.currentPage >= this.pageCount();
			} catch (e) {
				Config.log(e);
			}
		},
		//判断是否为空
		isEmpty: function() {
			try {
				return this.totalCount <= 0;
			} catch (e) {
				Config.log(e);
			}
		}
	};
	//--------------------------------
	window.PageData = PageData;
	//--------------------------------
})(window);