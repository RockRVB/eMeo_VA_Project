/******************** 
	作用:表格控件
	作者:蔡俊雄
	版本:V1.0
	时间:2015-03-06
********************/
(function ($) {
    $.widget("grgbanking.Table", $.grgbanking.Component, {

        //-------------------------------------------属性
        /** 向上翻页按钮 */
        prevButton: null,

        /** 向下翻页按钮 */
        nextButton: null,

        /** 是否有翻页 */
        hasPaging: true,

        /** 表格里的控件 */
        elements: [],

        //-------------------------------------------配置
        //配置
        options: {
            /** 控件类型 */
            type: Config.TYPE_TABLE,

            /** id */
            id: "",

            /** 控件是否隐藏,默认是隐藏(true:隐藏 false:可见) */
            isComponentHide: true,

            /** 行数是否固定,默认是固定(true:固定 false:动态变化) */
            isFixedRow: false,

            /** 预定义表格行数 */
            predefinedRowCount: Config.NOT_INIT,

            /** 表格最多显示行数 */
            maxRowCount: 8,

            /** 表格实际行数 */
            rowCount: 8,

            /** 当前起始行(从1开始) */
            rowIndex: 1,
           
            /** 每次滚动多少行 */
            rowIncrement: 1
        },
        //-------------------------------------------函数
        //读取配置
        readConfig: function () {
            try {
                this._super();
                //获取预定义表格行数
                if (!this.hasInitProperty(this.options.predefinedRowCount))
                {
                    var length = this.element.find("tbody tr").length;
                    this.options.predefinedRowCount = length;
                }
                //设置行和列
                var self = this;
                this.element.find("tbody tr").each(function (row) {
                    $(this).find("td").each(function (column) {
                        $(this).attr({
                            row: row+1,//行号(从1开始计算)
                            column: column + 1//列号(从1开始计算)
                        });
                    });
                });
                
            } catch (e) {
                Config.log(e);
            }
        },
        //初始化事件侦听器
        initListener: function () {
            try {
                //------------翻页按钮
                if (this.hasPaging) {
                    this.prevButton.options.eventData = { self: this };
                    this.nextButton.options.eventData = { self: this };

                    this.prevButton.options.afterclick = this.prevPage;
                    this.nextButton.options.afterclick = this.nextPage;
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //移除控件时清除事件
        removeListener: function () {
            try {

            } catch (e) {
                Config.log(e);
            }
        },
        //更新状态
        updateState: function () {
            try {
                //判断是否启用翻页按钮
                if (this.options.rowCount > this.options.maxRowCount) {


                } else {


                }


            } catch (e) {
                Config.log(e);
            }
        },
        //测试用
        test: function () {
            try {

            } catch (e) {
                Config.log(e);
            }
        },
        //点击"上一页"按钮
        prevPage: function (e,data) {
            try {
                var self = data.self;
                self.options.rowIndex-=self.options.rowIncrement;
                if (self.options.rowIndex < 1) {
                    self.options.rowIndex = 1;
                }
                self.showCurrentPage();
            } catch (e) {
                Config.log(e);
            }
        },
        //点击"下一页"按钮
        nextPage: function (e, data) {
            try {
                var self = data.self;
                self.options.rowIndex += self.options.rowIncrement;
                var endIndex = self.options.rowCount - self.options.maxRowCount + 1;
                if (endIndex < 1)
                    endIndex = 1;
                self.options.rowIndex = Math.min(self.options.rowIndex, endIndex);
                self.showCurrentPage();
            } catch (e) {
                Config.log(e);
            }
        },

        //-------------------------------------------重写控件方法
        //选中控件
        select: function () {
            try {
                this._super();
                this.updateState();
            } catch (e) {
                Config.log(e);
            }
        },
        //取消选中控件
        deselect: function () {
            try {
                this._super();
                this.updateState();
            } catch (e) {
                Config.log(e);
            }
        },
        //禁用控件
        disable: function () {
            try {
                this._super();
                this.updateState();
            } catch (e) {
                Config.log(e);
            }
        },
        //启用控件
        enable: function () {
            try {
                this._super();
                this.updateState();
            } catch (e) {
                Config.log(e);
            }
        },
        //获取值
        //getValue: function (attribute) {
        //    var result = this._super(attribute);
        //    return result;
        //},
        ////设置值
        //setValue: function (value, attribute) {
        //    var result = this._super(value, attribute);
        //    return result;
        //},
        //设置控件文本
        setText: function (value) {
            var result = true;
            try {
                this._super(value);
                //this.element.attr("src", value);
            }
            catch (e) {
                Config.log(e);
                result = false;
            }
            return result;
        },
        //显示当前页
        showCurrentPage: function () {
            try {
                var self = this;
                var startIndex = self.options.rowIndex - 1;//起始行
                var endIndex = self.options.rowIndex + self.options.maxRowCount - 2;//结束行
                if (this.options.rowCount > this.options.maxRowCount) {
                    //------------翻页按钮
                    if (this.hasPaging) {
                        this.element.find(".type-table-paging").removeClass("none"); //显示翻页按钮
                        if (this.options.rowIndex <= 1) {
                            this.prevButton.disable();//禁用"上一页"按钮
                        } else {
                            this.prevButton.enable();//启用"上一页"按钮
                        }
                        if ((this.options.rowIndex + this.options.maxRowCount - 1) < this.options.rowCount) {
                            this.nextButton.enable();//启用"下一页"按钮
                        } else {
                            this.nextButton.disable();//禁用"下一页"按钮
                        }
                    }
                    //------------表格行
                    this.element.find("tbody tr").removeClass("even odd last-visible-row").each(function (index) {
                        if (index < startIndex || index > endIndex) {
                            $(this).addClass("none");
                        } else {
                            $(this).removeClass("none");
                            //奇偶行
                            (index - startIndex) % 2 == 0 ? $(this).addClass("odd") : $(this).addClass("even");
                            //最后一行
                            if(index==endIndex)
                                $(this).addClass("last-visible-row");
                        }
                    });
                } else {
                    //------------翻页按钮
                    if (this.hasPaging) {
                        //隐藏翻页按钮
                        this.element.find(".type-table-paging").addClass("none");
                        //禁用翻页按钮
                        this.prevButton.disable();
                        this.nextButton.disable();
                    }
                    //------------表格行
                    if (this.options.isFixedRow) {
                        //行数固定,显示maxRowCount行
                        this.element.find("tbody tr").removeClass("even odd last-visible-row").each(function (index) {
                            if (index < startIndex || index > endIndex) {
                                $(this).addClass("none");
                            } else {
                                $(this).removeClass("none");
                                //奇偶行
                                (index - startIndex) % 2 == 0 ? $(this).addClass("odd") : $(this).addClass("even");
                                //最后一行
                                if (index == endIndex)
                                    $(this).addClass("last-visible-row");
                            }
                        });

                    } else {
                        //行数不固定,显示rowCount行
                        endIndex = self.options.rowIndex + self.options.rowCount - 2;//结束行
                        this.element.find("tbody tr").removeClass("even odd last-visible-row").each(function (index) {
                            if (index < startIndex || index > endIndex) {
                                $(this).addClass("none");
                            } else {
                                $(this).removeClass("none");
                                //奇偶行
                                (index - startIndex) % 2 == 0 ? $(this).addClass("odd") : $(this).addClass("even");
                                //最后一行
                                if (index == endIndex)
                                    $(this).addClass("last-visible-row");
                            }
                        });
                    }
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //重设显示行数
        resetRow: function (currentRow) {
            try {
                //显示当前行控件
                var length = this.elements.length;
                for (var i = 0; i < length; i++)
                {
                    if (this.elements[i].options.row == currentRow)
                    {
                        if (!this.elements[i].isVisible())
                            this.elements[i].show();
                    }
                }
                //this.element.find("tbody tr:eq(" + (currentRow-1) + ")").removeClass("none");
            } catch (e) {
                Config.log(e);
            }
        },
        //生成表格中的控件
        generateComponent: function () {
            try {
               
            } catch (e) {
                Config.log(e);
            }
        },
        //-------------------------------------------控件创建和销毁
        //初始化
        init: function () {
            try {
                this.hasPaging = this.element.find(".type-table-paging").length > 0;
                if (this.hasPaging)
                {
                    this.prevButton = this.element.find(".type-table-paging button[mark=prev]").Button({ isAutoSend: false }).Button("instance");
                    this.nextButton = this.element.find(".type-table-paging button[mark=next]").Button({ isAutoSend: false }).Button("instance");
                }
                this.showCurrentPage();//显示当前页
                this.generateComponent();//生成表格中的控件
            } catch (e) {
                Config.log(e);
            }
        },
        //创建控件
        _create: function () {
            try {
                this.readConfig(); //读取配置
                this.init(); //初始化
                this.initListener();//初始化事件侦听器
                this.updateState();//更新状态
            } catch (e) {
                Config.log(e);
            }
        },
        //销毁控件
        destroy: function () {
            try {
                this.removeListener();
                $.Widget.prototype.destroy.call(this);
                this.element.remove();
            } catch (e) {
                Config.log(e);
            }
        }
    });
})(jQuery);


$(function () { //ID匹配元素
    //$("[type=TABLE]").Table();

    //---------------事件处理
    //$("button").Button({
    //    preclick: function (event, data) {
    //        alert("点击前:" + data.id);
    //    },
    //    afterclick: function (event, data) {
    //        alert("点击后:"+data.id);
    //    }
    //})
    //.off();
    //$("#FUN_PRINT").Button("startFlash");


    //---------------设置值
    //$("button[type=button]").Button("setValue", "0", "selected");
    //$("button[type=button]").Button("setValue", "1", "selected");
    //$("button[type=button]").Button("setValue", "0", "enabled");
    //$("button[type=button]").Button("setValue", "1", "enabled");
    //$("button[type=button]").Button("setValue", "0", "visible");
    //$("button[type=button]").Button("setValue", "1", "visible");
    //$("button[type=button]").Button("setValue", "1", "text");
    //$("button[type=button]").Button("deselect");
    //$("button[type=button]").Button("disable");
    //---------------设置焦点

    //---------------响应按键
    //Config.TWIN_MONITOR = "INTANG";
    //$("button[type=button]").Button("setFocus");
    ////$("button[type=button]").Button("clearFocus");
    //$("button[type=button]").Button("keyAction", 4);
    //$("button[type=button]").Button("keyAction", 9);


});