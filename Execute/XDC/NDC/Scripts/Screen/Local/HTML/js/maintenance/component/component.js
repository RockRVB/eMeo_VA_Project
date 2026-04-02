/******************** 
	作用:所有控件的基类
	作者:蔡俊雄
	版本:V1.0
	时间:2015-02-06
********************/
(function ($) {
    $.event.special.valuechange = {
        teardown: function (namespaces) {
            $(this).unbind('.valuechange');
        },
        handler: function (e) {
            $.event.special.valuechange.triggerChanged($(this));
        },
        add: function (obj) {
            $(this).on('keyup.valuechange cut.valuechange paste.valuechange input.valuechange propertychange.valuechange', obj.selector, $.event.special.valuechange.handler);
        },
        triggerChanged: function (element) {
            //console.log(element[0].tagName.toUpperCase());
            //var current = element[0].contentEditable === 'true' ? element.html() : element.val()
            var tagName = element[0].tagName.toUpperCase();
            var current = (tagName === 'TEXTAREA' || tagName === 'DIV' || tagName === 'SPAN') ? element.html() : element.val()
              , previous = typeof element.data('previous') === 'undefined' ? element[0].defaultValue : element.data('previous');
            if (current !== previous) {
                element.trigger('valuechange', [element.data('previous')]);
                element.data('previous', current);
            }
        }
    }
    $.widget("grgbanking.Component", {

        /** 闪烁ID */
        flashId: null,

        /** 闪烁属性初始值 */
        propertyInit: null,

        /** 闪烁属性当前值 */
        propertyCurrent: null,

        /** 当前闪烁次数 */
        flashCount: 0,

        options: {
            /** 控件类型 */
            type: "COMPONENT",

            /** id */
            id: "",

            /**
             * 是否选中(默认是true)
             * <li><strong>true:</strong>选中</li>
             * <li><strong>false:</strong>不选中</li>
             */
            isSelected: false,

            /**
             * 是否启用(默认是true)
             * <li><strong>true:</strong>启用</li>
             * <li><strong>false:</strong>禁用</li>
             */
            isEnabled: true,

            /**
             * 是否可见(默认是true)
             * <li><strong>true:</strong>可见</li>
             * <li><strong>false:</strong>隐藏</li>
             */
            isVisible: true,

            /** IDS_多语言配置 */
            ids: "",

            /** 控件的文本 */
            text: "",

            /** 控件的提示文字 */
            key: "5",

            //-------------------------焦点


            /** 界面中是否已存在,默认是true(true:界面中已存在 false:未在界面中) */
            isExist: false,



            //--------------------焦点设置
            /** 是否接收焦点,默认是true(true:允许设置焦点 false:不允许设置焦点) */
            allowFocus: true,

            /** 控件当前是否获得焦点(true:获得焦点 false:没获得焦点) */
            hasFocus: false,

            /** 焦点类型 */
            focus: FocusType.IGNORE,

            /** 焦点组名 */
            focusGroup: "ignore",

            //--------------------闪烁设置

            /** 闪烁类型(color:变换颜色 visible:显隐 class:类 默认为显隐)*/
            flashType: FlashType.VISIBLE,

            /** 文字颜色(格式:十六进制颜色 默认是空) */
            color: "#000000",

            /** 文字禁用时的颜色(格式:十六进制颜色 默认是0xB9B9B9) */
            disabledColor: "#B9B9B9",

            /** 关联类型 */
            relation: FlashType.OPPOSITE,

            /** 闪烁关联ID */
            linkId: "",

            /** 闪烁间隔时间(毫秒) */
            flashInterval: 300,

            /** 闪烁次数 */
            flashCount: 0,



            /** 控件所在的界面 */
            parentUi: null,

            /** 直接的父级 */
            directParent: null,

            //-------------------------用于获取控件的额外属性
            /** 控件所在的表格行数 */
            row: 0,

            /** 控件所在的表格列数 */
            column: 0,

            /** 当前是界面中的第几个控件 */
            index: 0,

            /** 用于保存控件的额外属性 */
            extra: {}
        },
        //读取配置
        readConfig: function () {
            try {
                var attribute = "";
                var self = this;
                $.each(this.options, function (name, value) {
                    //console.log(name+":"+typeof (self.options[name]));
                    attribute = self.element.attr(name);
                    if (attribute != undefined) {
                        switch (typeof (self.options[name])) {
                            case "boolean":
                                if (attribute == "1" || attribute == "true") {
                                    self.options[name] = true;
                                } else {
                                    self.options[name] = false;
                                }
                                break;
                            case "number":
                                self.options[name] = Number(attribute);
                                break;
                            case "object":
                                //console.log(self.element[0].nodeName + "*****" + name + ":" + typeof (self.options[name]));
                                self.setConfigObject(name, attribute);
                                break;
                            default:
                                self.options[name] = attribute;
                                break;
                        }
                    }
                });
                //判断一开始时是否有隐藏
                if (!this.isVisible())
                    this.options.isVisible = false;
                //设置可见性
                if (this.options.isVisible) {
                    this.show();
                }
                else {
                    this.hide();
                }
                //attribute = this.element.attr("isAppend");
                //alert(this.options.isVisible);
                //attribute = this.element.attr("isAppend");
            } catch (e) {
                Config.log(e);
                //console.log(e.message);
                //alert(this.options.isVisible);
            }
        },
        //设置对象类型的配置
        setConfigObject: function (name, attribute) {
            //this.options[name] = attribute;
        },
        //属性是否已经进行初始化
        hasInitProperty: function (property) {
            return property != Config.NOT_INIT;
        },
       
        //---------------------------------------获取属性
        //是否可见
        isVisible: function () {
            //return !this.element.hasClass("none");
            return this.element.is(":visible");
        },
        //是否启用
        isEnabled: function () {
            return !this.element.hasClass("disabled");
        },
        //是否可以响应事件
        canAction: function (obj) {
            try {
                return !(obj.hasClass("none")) && !(obj.hasClass("disabled"));
            }
            catch (e) {
                Config.log(e);
            }
        },
        //---------------------------------------属性操作
        //选中控件
        select: function () {
            this.options.isSelected = true;
        },
        //取消选中控件
        deselect: function () {
            this.options.isSelected = false;
        },
        //禁用控件
        disable: function () {
            this.options.isEnabled = false;
        },
        //启用控件
        enable: function () {
            this.options.isEnabled = true;
        },
        //隐藏控件
        hide: function () {
            try {
                this.options.isVisible = false;
                this.element.addClass("none");
            }
            catch (e) {
                Config.log(e);
            }
        },
        //显示控件
        show: function () {
            try {
                this.options.isVisible = true;
                this.element.removeClass("none");
            }
            catch (e) {
                Config.log(e);
            }
        },
        //---------------------------------------获取值
        //获取控件是否启用
        getEnabled: function () {
            return (this.options.isEnabled ? 1 : 0) + "";
        },
        //获取控件是否可见
        getVisible: function () {
            return (this.options.isVisible ? 1 : 0) + "";
        },
        //获取控件是否选中
        getSelected: function () {
            return (this.options.isSelected ? 1 : 0) + "";
        },
        //在获取控件文本前的操作
        beforeGetText: function () {

        },
        //获取控件文本
        getText: function () {
            this.beforeGetText();
            return this.options.text;
        },
        //获取值
        getValue: function (attribute) {
            var result = "";
            try {
                if (Config.CMD_ATTRIBUTE_ENABLED == attribute) {
                    result = this.getEnabled();
                }
                else if (Config.CMD_ATTRIBUTE_SELECTED == attribute) {
                    result = this.getSelected();
                }
                else if (Config.CMD_ATTRIBUTE_VISIBLE == attribute) {
                    result = this.getVisible();
                }
                else if (Config.CMD_ATTRIBUTE_TEXT == attribute) {
                    result = this.getText();
                }
                else {
                    result = this.getText();
                }
            } catch (e) {
                Config.log(e);
            }
            return result;
        },
        //---------------------------------------设置值
        //设置控件是否启用
        setEnabled: function (value) {
            try {
                if (value == "1" || value == true)
                    this.enable();
                else
                    this.disable();
            }
            catch (e) {
                Config.log(e);
                return false;
            }
            return true;
        },
        //设置控件是否可见
        setVisible: function (value) {
            try {
                if (value == "1" || value == true)
                    this.show();
                else
                    this.hide();
            }
            catch (e) {
                Config.log(e);
                return false;
            }
            return true;
        },
        //设置控件是否选中
        setSelected: function (value) {
            try {
                if (value == "1" || value == true)
                    this.select();
                else
                    this.deselect();
            }
            catch (e) {
                Config.log(e);
                return false;
            }
            return true;
        },
        //设置控件文本
        setText: function (value) {
            try {
                this.options.text = value;
                //判断是否在表格里,如在表格里则将该行的所有控件显示出来
                if (this.options.directParent != null && this.options.row>1)
                {
                    this.options.directParent.resetRow(this.options.row);
                } 
            }
            catch (e) {
                Config.log(e);
                return false;
            }
            return true;
        },
        //设置值
        setValue: function (value, attribute) {
            var result = true;
            try {
                if (Config.CMD_ATTRIBUTE_ENABLED == attribute) {
                    result = this.setEnabled(value);//设置控件是否启用
                }
                else if (Config.CMD_ATTRIBUTE_SELECTED == attribute) {
                    result = this.setSelected(value);//设置控件是否选中
                }
                else if (Config.CMD_ATTRIBUTE_VISIBLE == attribute) {
                    result = this.setVisible(value);//设置控件是否可见
                }
                else if (Config.CMD_ATTRIBUTE_TEXT == attribute) {
                    result = this.setText(value);//设置文本
                }
                else {
                    result = this.setText(value);//设置文本
                }
            }
            catch (e) {
                Config.log(e);
                result = false;
            }
            //if (directParent != null)
            //{
            //    try
            //    {
            //        if (row > 0 && !directParent.isResetByRow)
            //            directParent.resetRow(row);
            //    }
            //    catch (err)
            //    {

            //    }
            //}
            return result;
        },
        //清除键盘焦点
        clearFocus: function () {
            this.options.hasFocus = false;
        },
        //设置键盘焦点
        setFocus: function () {
            this.options.hasFocus = true;
            return true;
        },
        //响应键盘输入
        keyAction: function (keyNum) {
            return true;
        },
        //---------------------------------------闪烁
        //隐藏元素
        hideIt: function (selector) {
            try {
                if (selector == undefined)
                    this.hide();
                else
                    $(selector).addClass("none");
            }
            catch (e) {
                Config.log(e);
            }

            //if (selector == undefined) {
            //    this.element.css("visibility", "hidden");
            //} else {
            //    $(selector).css("visibility", "hidden");
            //}

            //if (selector == undefined)
            //    this.element.addClass("hidden");
            //else
            //    $(selector).addClass("hidden");
        },
        //显示元素
        showIt: function (selector) {
            try {
                if (selector == undefined)
                    this.show();
                else
                    $(selector).removeClass("none");
            }
            catch (e) {
                Config.log(e);
            }

            //if (selector == undefined) {
            //    this.element.css("visibility", "visible");
            //} else {
            //    $(selector).css("visibility", "visible");
            //}

            //if (selector == undefined)
            //    this.element.removeClass("hidden");
            //else
            //    $(selector).removeClass("hidden");
        },
        //改变颜色
        changeColor: function (color, selector) {
            try {
                if (selector == undefined) {
                    this.element.css("color", color);
                } else {
                    $(selector).css("color", color);
                }
            }
            catch (e) {
                Config.log(e);
            }
        },
        //开始闪烁
        startFlash: function () {
            try {
                //保存初始属性
                if (this.options.flashType == FlashType.VISIBLE) {//显隐
                    this.propertyInit = this.options.isVisible;
                    this.propertyCurrent = this.options.isVisible;
                } else if (this.options.flashType == FlashType.COLOR) {//颜色
                    this.propertyInit = this.element.css("color");
                    this.propertyCurrent = this.options.color;
                }
                //设置计数
                this.flashCount = 0;
                this.options.flashCount *= 2;
                //闪烁
                this.flash();
                this.flashId = setInterval(this.flash, this.options.flashInterval, this);
            }
            catch (e) {
                Config.log(e);
            }
        },
        //闪烁
        flash: function (self) {
            try {
                if (self == undefined)
                    self = this;
                if (self.options.flashType == FlashType.VISIBLE) {//显隐
                    self.propertyCurrent = !self.propertyCurrent;
                    if (self.propertyCurrent) {
                        self.showIt();
                    } else {
                        self.hideIt();
                    }
                    //判断是否有相关元素
                    if (self.options.linkId != "") {
                        var idArray = self.options.linkId.split(",");
                        $.each(idArray, function () {
                            if (self.options.relation == FlashType.OPPOSITE) {//相反
                                if (self.propertyCurrent) {
                                    self.hideIt("#" + this);
                                } else {
                                    self.showIt("#" + this);
                                }
                            } else//相同
                            {
                                if (self.propertyCurrent) {
                                    self.showIt("#" + this);
                                } else {
                                    self.hideIt("#" + this);
                                }
                            }
                        });
                    }

                } else if (self.options.flashType == FlashType.COLOR) {//颜色
                    if (self.propertyCurrent == self.options.color)
                        self.propertyCurrent = self.options.disabledColor;
                    else
                        self.propertyCurrent = self.options.color;

                    self.changeColor(self.propertyCurrent);
                    //判断是否有相关元素
                    if (self.options.linkId != "") {
                        var idArray = self.options.linkId.split(",");
                        $.each(idArray, function () {
                            if (self.options.relation == FlashType.OPPOSITE) {//相反
                                if (self.propertyCurrent == self.options.color)
                                    self.changeColor(self.options.disabledColor, "#" + this);
                                else
                                    self.changeColor(self.options.color, "#" + this);
                            } else//相同
                            {
                                self.changeColor(self.propertyCurrent, "#" + this);
                            }
                        });
                    }

                    //console.log(self.element.css("color"));

                }
                //判断是否可以停止闪烁
                if (self.options.flashCount > 0) {
                    self.flashCount++;
                    //console.log(self.flashCount);
                    if (self.flashCount >= self.options.flashCount) {
                        self.stopFlash();
                    }
                }
            }
            catch (e) {
                Config.log(e);
            }
        },
        //停止闪烁
        stopFlash: function () {
            try {
                this.flashCount = 0;
                this.options.flashCount = 0;
                clearInterval(this.flashId);
                //重置初始属性
                if (this.options.flashType == FlashType.VISIBLE) {//显隐
                    if (this.propertyInit) {
                        this.showIt();
                    } else {
                        this.hideIt();
                    }
                    //判断是否有相关元素
                    if (this.options.linkId != "") {
                        var idArray = this.options.linkId.split(",");
                        var self = this;
                        $.each(idArray, function () {
                            if (self.options.relation == FlashType.OPPOSITE) {//相反
                                if (self.options.isVisible) {
                                    self.hideIt("#" + this);
                                } else {
                                    self.showIt("#" + this);
                                }
                            } else//相同
                            {
                                if (self.options.isVisible) {
                                    self.showIt("#" + this);
                                } else {
                                    self.hideIt("#" + this);
                                }
                            }
                        });
                    }
                } else if (this.options.flashType == FlashType.COLOR) {//颜色
                    this.propertyCurrent = this.propertyInit;
                    this.changeColor(this.propertyCurrent);
                    //判断是否有相关元素
                    if (this.options.linkId != "") {
                        var idArray = this.options.linkId.split(",");
                        var self = this;
                        $.each(idArray, function () {
                            if (self.options.relation == FlashType.OPPOSITE) {//相反
                                if (self.propertyCurrent == self.options.color)
                                    self.changeColor(self.options.disabledColor, "#" + this);
                                else
                                    self.changeColor(self.options.color, "#" + this);
                            } else//相同
                            {
                                self.changeColor(self.propertyCurrent, "#" + this);
                            }
                        });
                    }
                }
                this.propertyInit = null;
                this.propertyCurrent = null;
            }
            catch (e) {
                Config.log(e);
            }
        }
        //--------------------------
    });
})(jQuery);