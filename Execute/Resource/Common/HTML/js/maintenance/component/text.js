/******************** 
	作用:静态文本
	作者:蔡俊雄
	版本:V1.0
	时间:2015-02-11
********************/
(function ($) {
    $.widget("grgbanking.Text", $.grgbanking.Component, {

        //-------------------------------------------属性
        /** 是否鼠标移到文字上(true:鼠标移到文字上 false:鼠标没移到文字上) */
        mouseOver: false,

        /** 滚动ID */
        scrollId: null,

        //-------------------------------------------配置
        //配置
        options: {
            /** 控件类型 */
            type: Config.TYPE_TEXT,

            /** 文字颜色(格式:十六进制颜色 默认是空) */
            color: "",

            /** 文字禁用时的颜色(格式:十六进制颜色 默认是0xB9B9B9) */
            disabledColor: "",

            //----------------------滚动设置
            /** 文本字段是否可以滚动(默认为可以滚动)(true:可以滚动 false:不滚动) */
            canScroll: false,

            /** 文本字段超过多少个字符时会开始滚动(格式:数字 默认是1000) */
            scrollLength: 1000,

            /** 当前滚动到了哪个字符(格式:数字 默认是0) */
            scrollIndex: 0,

            /** 滚动时间间隔(单位:毫秒 默认是800) */
            scrollInterval: 800,

            /** 滚动文字的颜色(格式:十六进制颜色 默认是0xFF0000)*/
            scrollColor: "#FF0000",

            /** 文字前缀(用于滚动时和text合并显示 默认为空)*/
            prefix: "",

            /** 是否使用鼠标控制滚动(true:鼠标控制滚动,鼠标移上文字时停止滚动,鼠标移出文字时停止滚动 false:不使用鼠标控制滚动)(默认为 true ) */
            mouseControlScroll: true,

            /** 滚动类型(格式:十六进制颜色 默认是0xFF0000)*/
            scrollType: TextScrollType.CONTAINER,

            /** 文字最多显示多少像素,当usePixelScroll时有效(格式:数字 默认是100) */
            maxPixel: 100,

            /** 为了让滚动效果更好所添加的空格(格式:字符串 默认是3个空格) */
            scrollSpace: "   "
        },
        //-------------------------------------------函数
        //读取配置
        readConfig: function () {
            try {
                this.options["text"] = this.element.html();//获取初始值
                this._super();
            } catch (e) {
                Config.log(e);
            }
        },
        //初始化事件侦听器
        initListener: function () {
            try {
                var self = this;
                this.element.on('mouseover', function () {
                    if (self.options.mouseControlScroll) {
                        self.mouseOver = true;
                    }
                }).on('mouseout', function () {
                    if (self.options.mouseControlScroll) {
                        self.mouseOver = false;
                    }
                })
            } catch (e) {
                Config.log(e);
            }
        },
        //移除控件时清除事件
        removeListener: function () {
            try {
                this.stopScroll(); //停止滚动文本
                this.element.off().unbind();
            } catch (e) {
                Config.log(e);
            }
        },
        //更新状态
        updateState: function () {
            try {
                //this.enableButton(this.element, this.options.isEnabled);
                //this.checkScroll(); //判断是否可以滚动
                //设置启用状态
                if (!this.options.isEnabled) {
                    this.element.addClass("disabled");
                } else {
                    this.element.removeClass("disabled");
                }
                //设置选中状态
                if (this.options.isSelected) {
                    this.element.addClass("selected");
                } else {
                    this.element.removeClass("selected");
                }

                //根据状态设置颜色
                if (this.isScroll()) { //判断是否可以滚动
                    //设置文字颜色
                    if (this.options.isEnabled) {
                        if (this.options.scrollColor != "")
                            this.element.css("color", this.options.scrollColor);//字符颜色
                    }
                    else {
                        if (this.options.disabledColor != "")
                            this.element.css("color", this.options.disabledColor);//字符颜色
                    }
                    //开始滚动
                    this.startScroll(); 
                } else
                {
                    //停止滚动文本
                    this.stopScroll();
                    //设置文字颜色
                    if (this.options.isEnabled) {
                        if (this.options.color != "")
                            this.element.css("color", this.options.color);//字符颜色
                    }
                    else {
                        if (this.options.disabledColor != "")
                            this.element.css("color", this.options.disabledColor);//字符颜色
                    }
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //判断是否可以滚动
        checkScroll: function () {
            try {
                if (this.isScroll()) {
                    this.startScroll(); //开始滚动
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //开始滚动
        startScroll: function () {
            try {
                this.stopScroll(); //停止滚动文本
                //添加滚动类
                this.element.addClass("scroll-config");
                //开始滚动
                this.options.scrollIndex = -1;
                this.mouseOver = false;
                //判断长度
                //alert(this.element.get(0).innerHTML);
                //alert(this.element.get(0).offsetWidth + ":" + this.element.get(0).scrollWidth);

                this.scroll();
                this.scrollId = setInterval(this.scroll, this.options.scrollInterval,this);
            } catch (e) {
                Config.log(e);
            }
        },
        //停止滚动文本
        stopScroll: function () {
            try {
                this.element.css("color", "");//字符颜色
                this.element.removeClass("scroll-config");
                if (this.scrollId != null)
                    clearInterval(this.scrollId);
            } catch (e) {
                Config.log(e);
            }
        },
        //滚动文本
        scroll: function (self) {
            try {
                if (self == undefined)
                    self = this;
                if (self.mouseOver)
                    return;
                self.options.scrollIndex++;
                var length = 0;
                //if (this.options.usePixelScroll) {
                //文本字段使用像素判断滚动
                //if (this.options.scrollIndex >= this.options.text.length)
                //{
                //this.options.scrollIndex = 0;
                //}
                //}else {
                //
                //}
                var scrollText = self.options.scrollSpace + self.options.text;//滚动文本前的空格
                if (self.options.scrollIndex >= scrollText.length)
                {
                    self.options.scrollIndex = 0;
                }
                var remain = scrollText.length - (self.options.scrollIndex + 1); //剩余个数
                var str = "";
                if (self.options.scrollType == TextScrollType.CONTAINER) {
                    //文本字段使用像素判断滚动
                    str = scrollText.substr(self.options.scrollIndex);
                    if (self.options.scrollIndex > 0)
                        str += scrollText.substr(0, self.options.scrollIndex);
                }else {
                    if (remain < self.options.scrollLength)
                    {
                        str = scrollText.substr(self.options.scrollIndex);
                        str += scrollText.substr(0, self.options.scrollLength - remain - 1);
                    }
                    else
                    {
                        str = scrollText.substr(self.options.scrollIndex, self.options.scrollLength);
                    }
                }
                if (self.options.prefix != "")
                {
                    //如有前缀则加上前缀
                    str = self.options.prefix + " " + str;
                }
                self.element.html(str.replace(/ /g, "&nbsp;"));
            } catch (e) {
                Config.log(e);
            }
        },
        //测试用
        test: function () {
            try {
                //按键监听
                $(window).keyup(function (event) {
                    //alert(event.keyCode);
                });
            } catch (e) {
                Config.log(e);
            }
        },
        //判断当前是否处于滚动状态
        isScroll: function () {
            var result = false;
            try {
                if (!this.options.canScroll) {
                    return result;
                }

                var str;
                if (this.options.prefix == "") {
                    str = this.options.text;
                }
                else {
                    str = this.options.prefix + " " + this.options.text;
                }
                var container=this.element.get(0);
                if (this.options.scrollType == TextScrollType.CONTAINER) {
                    //if (container.scrollWidth > this.options.maxPixel)
                    if (container.scrollWidth > container.offsetWidth)
                    {
                        result = true;
                    }
                }
                else if (this.options.scrollType == TextScrollType.PIXEL) {
                    if (container.scrollWidth > this.options.maxPixel)
                    {
                        result = true;
                    }
                }
                else {
                    if (str.length > this.options.scrollLength) {
                        result = true;
                    }
                }
            } catch (e) {
                Config.log(e);
            }
            return result;
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
                this.element.html(value);
                this.updateState();
            }
            catch (e) {
                Config.log(e);
                result = false;
            }
            return result;
        },


        //-------------------------------------------控件创建和销毁
        //创建控件
        _create: function () {
            try {
                this.readConfig(); //读取配置
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
    //$("#STATIC_PROMPT").Text();

    //---------------事件处理
    //$("button").Text({
    //    preclick: function (event, data) {
    //        alert("点击前:" + data.id);
    //    },
    //    afterclick: function (event, data) {
    //        alert("点击后:"+data.id);
    //    }
    //})
    //.off();
    
    //---------------设置值
    //$("button[type=button]").Text("setValue", "0", "selected");
    //$("button[type=button]").Text("setValue", "1", "selected");
    //$("button[type=button]").Text("setValue", "0", "enabled");
    //$("button[type=button]").Text("setValue", "1", "enabled");
    //$("button[type=button]").Text("setValue", "0", "visible");
    //$("button[type=button]").Text("setValue", "1", "visible");
    //$("button[type=button]").Text("setValue", "1", "text");
    //$("button[type=button]").Text("deselect");
    //$("button[type=button]").Text("disable");
    //$("#STATIC_PROMPT").Text("setValue", "事件处理打印凭条成功", "text");
    //$("#STATIC_PROMPT").Text("setValue", "当插件被调用时，它将创建一个新的插件实例，所有的函数都将在该实例的语境中被执行。当插件被调用时，它将创建一个新的插件实例，所有的函数都将在该实例的语境中被执行。", "text");
    //$("#STATIC_PROMPT").Text("startFlash");
    //---------------设置焦点

    //---------------响应按键
    //Config.TWIN_MONITOR = "INTANG";
    //$("button[type=button]").Text("setFocus");
    ////$("button[type=button]").Text("clearFocus");
    //$("button[type=button]").Text("keyAction", 4);
    //$("button[type=button]").Text("keyAction", 9);


});