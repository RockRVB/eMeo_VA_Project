/******************** 
	作用:按钮
	作者:蔡俊雄
	版本:V1.0
	时间:2015-02-10
********************/
(function ($) {
    $.widget("grgbanking.Button", $.grgbanking.Component, {

        //-------------------------------------------属性


        /** 是否特殊按键(默认为普通按键)(true:特殊按键 false:普通按键) */
        isSpecial: false,

        //-------------------------------------------配置
        //配置
        options: {
            /** 控件类型 */
            type: Config.TYPE_BUTTON,

            /** id */
            id: "",

            /** 数据 */
            eventData: null,

            /** 点击按钮时是否自动向服务器发送自己的按钮ID(默认是true)(true:自动发送按钮ID false:不发送按钮ID) */
            isAutoSend: true,

            /** 在isAutoSend为true的情况下,如果dataToSend不为空则点击时会将dataToSend发送到服务器,否则发送按钮id(格式:字符串 要发送到服务器的数据) */
            dataToSend: "",

            /** 按钮是否可改变选中状态(默认是 false)(true:可改变选中状态 false:不可改变选中状态) */
            isToggle: false,

            /** 按钮是否单选(默认是 true)(true:单选 false:可多选) */
            isSingleToggle: true,

            /** 父级的选择符,用于一组按钮处于不同的父容器的情况下 */
            parentSelector: "",

            /** 按钮组的名称,用于将组内的其它按钮置为不选中状态(格式:字符串 按钮组的名称) */
            toggleName: ""

        },
        //-------------------------------------------函数
        //读取配置
        readConfig: function () {
            this._super();
        },
        //初始化事件侦听器
        initListener: function () {
            try {
                var self = this;
                this.element.click(function () {
                    if (!self.canAction($(this)))
                        return;
                    self._trigger("preclick", null, self.options.eventData);
                    self.setToggleState(); //如果是切换状态按钮则根据当前状态设置其它按钮的状态
                    //判断是否向服务器发送消息
                    if (self.options.isAutoSend) {
                        //向服务器发送消息
                        if (self.options.dataToSend == "") {
                            if (self.options.id != "")
                                Config.send(self.options.id);
                        }
                        else {
                            Config.send(self.options.dataToSend);
                        }
                    }
                    self._trigger("afterclick", null, self.options.eventData);
                });
            } catch (e) {
                Config.log(e);
            }
        },
        //移除控件时清除事件
        removeListener: function () {
            try {
                this.element.off().unbind();
            } catch (e) {
                Config.log(e);
            }
        },
        //更新状态
        updateState: function () {
            try {
                this.enableButton(this.element, this.options.isEnabled);
                //设置选中状态
                if (this.options.isSelected) {
                    this.element.addClass("selected");
                } else {
                    this.element.removeClass("selected");
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //如果是切换状态按钮根据当前状态设置其它按钮的状态
        setToggleState: function () {
            try {
                if (this.options.isToggle) {
                    if (this.options.isSingleToggle) {
                        var self = this;
                        var p;
                        if (this.options.parentSelector != "")
                            p = this.element.closest(this.options.parentSelector);
                        else
                            p = this.element.parent();
                        //当前按钮是选中状态,将组内的其它按钮置为不选中状态
                        p.find("button").each(function (index) {
                            //this.element.siblings("button").each(function (index) {
                            var btn = $(this).Button("instance");
                            //if (btn.options.id != self.options.id && btn.options.isToggle && btn.options.toggleName == self.options.toggleName)
                            if (btn != self && btn.options.isToggle && btn.options.toggleName == self.options.toggleName) {
                                btn.deselect();
                            }
                        });
                    }
                    this.select();
                }
            }
            catch (e) {
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
        //设置是否启用按钮(true为启用 false为禁用)
        enableButton: function (btn, isEnabled) {
            try {
                if (isEnabled) {
                    btn.removeAttr("disabled").removeClass("disabled");
                } else {
                    btn.attr("disabled", "disabled").addClass("disabled");
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //显示按钮的按键提示
        showKey: function () {
            try {
                if (this.options.hasFocus && Config.showKeyTip()) {
                    this.element.find(".tip").html(this.options.key);
                }
                else {
                    this.element.find(".tip").html("");
                }
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
                this.element.find(".button-text").html(value);
                //console.log(this.options.text);
            }
            catch (e) {
                Config.log(e);
                result = false;
            }
            return result;
        },
        //清除键盘焦点
        clearFocus: function () {
            try {
                this.options.hasFocus = false;
                this.element.find(".tip").html("");
            } catch (e) {
                Config.log(e);
                return false;
            }
            return true;
        },
        //设置键盘焦点
        setFocus: function () {
            try {
                this.clearFocus();
                this._super();
                this.showKey();
            } catch (e) {
                Config.log(e);
                return false;
            }
            return true;
        },
        //响应键盘输入
        keyAction: function (keyNum) {
            try {
                //var key = parseInt(keyNum);
                //if (isNaN(key))
                //    return false;
                this.element.trigger("click");
                //this.element.click();
                return true;
            }
            catch (e) {
                Config.log(e);
            }
            return false;
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
    //$("button[type=button]").Button();

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
    //$(":hidden").each(function (index) {
    //    //console.log(this.innerHTML);
    //    console.log(this.id+":"+this.className);
    //});


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