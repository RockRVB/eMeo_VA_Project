/******************** 
	作用:输入文本框
	作者:蔡俊雄
	版本:V1.0
	时间:2015-03-02
********************/
(function ($) {
    $.widget("grgbanking.InputText", $.grgbanking.Component, {

        //-------------------------------------------属性
        /** 是否鼠标移到文字上(true:鼠标移到文字上 false:鼠标没移到文字上) */
        mouseOver: false,


        //-------------------------------------------配置
        //配置
        options: {
            /** 控件类型 */
            type: Config.TYPE_INPUT,

            /** 文字颜色(格式:十六进制颜色 默认是空) */
            color: "",

            /** 文字禁用时的颜色(格式:十六进制颜色 默认是0xB9B9B9) */
            disabledColor: "",

            /**
		     * 是否密码输入框(默认为 false,普通输入框)
		     * <li><strong>true:</strong>密码输入框</li>
		     * <li><strong>false:</strong>普通输入框</li>
		     */
            password: false,

            /** 最多可输入字符个数(格式:数字 默认是20) */
            maxlength: 20,

            /** 有效字符(格式:字符串 默认为空) */
            validchars: "",

            /**
             * 限制字符(默认为 null,可输入任何字符)
             * <li><strong>null:</strong>可输入任何字符</li>
             * <li><strong>空字符串:</strong>不能输入任何字符</li>
             * <li><strong>一串字符:</strong>只能输入该字符串中的字符</li>
             * <li><strong>使用连字符:</strong>可以使用连字符 (-) 指定一个范围</li>
             * <li><strong>字符串以尖号 (^) 开头:</strong>如果字符串以尖号 (^) 开头，则先接受所有字符，然后从接受字符集中排除字符串中 ^ 之后的字符</li>
             * <li><strong>字符串不以尖号 (^) 开头:</strong>如果字符串不以尖号 (^) 开头，则最初不接受任何字符，然后将字符串中的字符包括在接受字符集中。</li>
             * 
             * @example 具体例子
             * <listing version="1.0">
             * ------------------------------------
             * 下例仅允许在文本字段中输入大写字符、空格和数字：
             *  my_txt.restrict = "A-Z 0-9",
             *  下例包含除小写字母之外的所有字符：
             *  my_txt.restrict = "^a-z",
             *  可以使用反斜杠输入 ^ 或 - 的本义。 认可的反斜杠序列为 \-、\^ 或 \\。 反斜杠在字符串中必须是一个本义字符，因此在 ActionScript 中指定时必须使用两个反斜杠。 例如，下面的代码只包含短划线 (-) 和尖号 (^)：
             *  my_txt.restrict = "\\-\\^",
             *  可在字符串中的任何地方使用 ^，以在包含字符与排除字符之间进行切换。 下面的代码只包含除大写字母 Q 之外的大写字母：
             *  my_txt.restrict = "A-Z^Q",
             *  可以使用 u 转义序列构造 restrict 字符串。 下面的代码只包含从 ASCII 32（空格）到 ASCII 126（代字号）之间的字符。
             *  my_txt.restrict = " -~",
             * ------------------------------------
             * </listing> 
             */
            restrict: "",

            /**
             * 检查模式(默认为 true,检查有效字符)
             * <li><strong>true:</strong>检查有效字符</li>
             * <li><strong>false:</strong>检查限制字符</li>
             */
            validMode: true,

            /** 数字模式,用于字符是否为数字 */
            numberPattern: "0123456789",

            /**
             * 是否只支持数字输入(默认为 false,可输入非数字)
             * <li><strong>true:</strong>只支持数字输入</li>
             * <li><strong>false:</strong>可输入非数字</li>
             */
            numinput: false
        },
        //-------------------------------------------函数
        //读取配置
        readConfig: function () {
            try {
                this._super();
                //获取初始值
                var attribute = "";
                var name = "value";
                attribute = this.element.attr(name);
                if (attribute != undefined) {
                    this.options["text"] = attribute;
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //初始化事件侦听器
        initListener: function () {
            try {
                var self = this;
                this.element.on('valuechange', function (e, previous) {
                    self.removeInvalidChar();//删除不符合要求的字符串
                }).on('click', function () {
                    Config.INPUT_TARGET = null;
                    self.setInputTarget();//设置输入框为输入目标
                    self.setFocus();
                });
                //this.element.on('mouseover', function () {
                //    if (self.options.mouseControlScroll) {
                //        self.mouseOver = true;
                //    }
                //}).on('mouseout', function () {
                //    if (self.options.mouseControlScroll) {
                //        self.mouseOver = false;
                //    }
                //})
            } catch (e) {
                Config.log(e);
            }
        },
        //设置输入框为输入目标
        setInputTarget: function () {
            try {
                if (Config.INPUT_TARGET == null)
                    Config.INPUT_TARGET = this;//获取焦点,切换输入目标
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
                //设置选中状态
                if (this.options.isSelected) {
                    this.element.addClass("selected");
                } else {
                    this.element.removeClass("selected");
                }
                //设置启用状态
                if (this.options.isEnabled) {
                    this.element.removeAttr("disabled").removeClass("disabled");
                } else {
                    this.element.attr("disabled", "disabled").addClass("disabled");
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //删除不符合要求的字符串
        removeInvalidChar: function () {
            try {
                var str = this.element.val();
                //如果不是有效字符则设置文本的值
                if (str != "" && !this.isValidChar(str)) {
                    str = this.getValidChar(str);
                    this.element.val(str);
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //检测字符串是否有效
        isValidChar: function (str) {
            try {
                for (var i = 0; i < str.length; i++) {
                    if (this.options.numinput && this.options.numberPattern.indexOf(str.charAt(i)) == -1)
                        return false;
                    if (this.options.validMode && this.options.validchars.indexOf(str.charAt(i)) == -1)
                        return false;
                }
            } catch (e) {
                Config.log(e);
            }
            return true;
        },
        //获取有效字符,将字符串去除无效字符,剩下的就是有效的
        getValidChar: function (str) {
            var result = "";
            try {
                if (str != "") {
                    for (var i = 0; i < str.length; i++) {
                        if (this.options.validMode && this.options.validchars.indexOf(str.charAt(i)) != -1) {
                            result += str.charAt(i);
                        }
                    }
                }
            } catch (e) {
                Config.log(e);
            }
            return result;
        },
        //清空文本输入框
        clear: function () {
            try {
                var oldValue = this.element.val();
                var newValue = "";
                this.element.val(newValue);
                if (oldValue != newValue)
                    this.element.trigger('valuechange', [oldValue]);
            } catch (e) {
                Config.log(e);
            }
        },
        //输入内容
        input: function (value) {
            try {
                var oldValue = this.element.val();
                var newValue = oldValue + value;
                this.setValue(newValue);
                if (oldValue != newValue)
                    this.element.trigger('valuechange', [oldValue]);
            } catch (e) {
                Config.log(e);
            }
        },
        //测试用
        test: function () {
            try {
                //按键监听
                //$(window).keyup(function (event) {

                //    //alert(event.keyCode);
                //});
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
        //设置控件文本
        setText: function (value) {
            var result = true;
            try {
                this._super(value);

                //如果长度超过最大长度则不允许输入
                if (value.length > this.options.maxlength)
                    return;
                //如果不是有效字符则不能设置文本的值
                if (!this.isValidChar(value))
                    return;
                //如果是数字且长度大于0并且第一个数字是0时不允许输入
                if (this.options.numinput && value.length > 0 && value.charAt(0) == "0")
                    return;
                //if (this.options.hasFocus)
                //this.getFocus(); //尝试获取焦点
                //config.text = value;
                //textField.text = value;
                //textField.setSelection(textField.text.length, textField.text.length); //选中文本


                this.element.val(value);

                //this.updateState();
            }
            catch (e) {
                Config.log(e);
                result = false;
            }
            return result;
        },
        //获取控件文本
        getText: function () {
            var result = "";
            try {
                result = this.element.val();
                if (this.options.password)
                    result = Config.CMD_PWD_SEPARATOR + "PWD" + Config.CMD_PWD_SEPARATOR + result + Config.CMD_PWD_SEPARATOR;
            } catch (e) {
                Config.log(e);
            }
            return result;
        },
        //尝试获取焦点
        getFocus: function () {
            try {
                this.element.trigger("focus");
            } catch (e) {
                Config.log(e);
            }
        },
        //清除键盘焦点
        clearFocus: function () {
            try {
                this.options.hasFocus = false;
                this.deselect();
            } catch (e) {
                Config.log(e);
                return false;
            }
            return true;
        },
        //设置键盘焦点
        setFocus: function () {
            try {
                if (Config.showKeyTip()) {
                    this.clearFocus();
                    this._super();
                    //被禁用时不可设置焦点
                    if (!this.options.isEnabled)
                        return false;
                    this.select();
                    this.setInputTarget();//设置输入框为输入目标
                    this.getFocus(); //尝试获取焦点
                }
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

                //if (key == 0)
                //    key = 10;
                //if (key >= 1 && key <= 4) {
                //    this.element.find(".textbox-button-zone button:eq(" + (key - 1) + ")").trigger("click");
                //}
                //else if (key >= 5 && key <= 10) {
                //    key -= 5;
                //    this.buttonArray[this.buttonShowArray[key]].trigger("click");
                //}
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
                this.setInputTarget();//设置输入框为输入目标
                this.removeInvalidChar();//删除不符合要求的字符串
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
    //Config.TWIN_MONITOR = "INTANG";
    //$("#INPUT_PASSWORD").InputText();
    //$("#INPUT_PASSWORD").InputText("test");
    //$("input[type=password]").InputText();


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
    //$("#INPUT_PASSWORD").InputText("setValue", "12345", "text");
    //$("#INPUT_PASSWORD").InputText("setValue", "23", "text");

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