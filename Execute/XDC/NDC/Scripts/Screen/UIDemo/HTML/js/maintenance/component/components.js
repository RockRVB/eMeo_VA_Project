var LoginRoleFlag = true;
(function(t) {
    t.event.special.valuechange = {
        teardown: function(e) {
            t(this).unbind(".valuechange")
        },
        handler: function(e) {
            t.event.special.valuechange.triggerChanged(t(this))
        },
        add: function(e) {
            t(this).on("keyup.valuechange cut.valuechange paste.valuechange input.valuechange propertychange.valuechange", e.selector, t.event.special.valuechange.handler)
        },
        triggerChanged: function(t) {
            var e = t[0].tagName.toUpperCase();
            var i = e === "TEXTAREA" || e === "DIV" || e === "SPAN" ? t.html() : t.val(),
            o = typeof t.data("previous") === "undefined" ? t[0].defaultValue: t.data("previous");
            if (i !== o) {
                t.trigger("valuechange", [t.data("previous")]);
                t.data("previous", i)
            }
        }
    };


    t.widget("grgbanking.Component", {
        flashId: null,
        propertyInit: null,
        propertyCurrent: null,
        flashCount: 0,
        // 选项
        options: {
            type: "COMPONENT",
            id: "",
            uiInstance: null,//
            isSelected: false,
            isEnabled: true,
            isVisible: true,
            ids: "",
            text: "",
            key: "5",
            isExist: false,
            allowFocus: true,
            hasFocus: false,
            focus: FocusType.IGNORE,
            focusGroup: "ignore",
            flashType: FlashType.VISIBLE,
            color: "#000000",
            disabledColor: "#B9B9B9",
            relation: FlashType.OPPOSITE,
            linkId: "",
            flashInterval: 300,
            flashCount: 0,
            parentUi: null,
            directParent: null,
            row: 0,
            column: 0,
            index: 0,
            extra: {}
        },
        // 控件初始化，根据HTML标签属性值初始化控件的属性值
        readConfig: function() {
            try {
                var e = "";
                var i = this;
                t.each
                (this.options,
                function(t, o) {
                    e = i.element.attr(t);
                    if (e != undefined) {
                        switch (typeof i.options[t]) {
                        case "boolean":
                            if (e == "1" || e == "true") {
                                i.options[t] = true
                            } else {
                                i.options[t] = false
                            }
                            break;
                        case "number":
                            i.options[t] = Number(e);
                            break;
                        case "object":
                            i.setConfigObject(t, e);
                            break;
                        default:
                            i.options[t] = e;
                            break
                        }
                    }
                });


                if (!this.isVisible()) this.options.isVisible = false;

                if (this.options.isVisible) {
                    this.show()
                } else {
                    this.hide()
                }
                if (this.options.uiInstance != null) {
                    this.options.uiInstance.elements.push(this)
                }
            } catch(o) {
                Config.log(o)
            }
        },

        setConfigObject: function(t, e) {},

        hasInitProperty: function(t) {
            return t != Config.NOT_INIT
        },
        // 控件基础方法判断是否可见
        isVisible: function() {
            return this.element.is(":visible")
        },
        //控件基础方法判断是否可用
        isEnabled: function() {
            return ! this.element.hasClass("disabled")
        },
        // 控件基础方法判断是否可操作
        canAction: function(t) {
            try {
                return ! t.hasClass("none") && !t.hasClass("disabled")
            } catch(e) {
                Config.log(e)
            }
        },
        // 控件基础方法设置控件选中
        select: function() {
            this.options.isSelected = true
        },
        //控件基础方法设置控件不选中
        deselect: function() {
            this.options.isSelected = false
        },

        // 控件基础方法设置控件不可用
        disable: function() {
            this.options.isEnabled = false
        },
        // 控件基础方法设置控件可用
        enable: function() {
            this.options.isEnabled = true
        },
        // 控件基础方法设置控件隐藏
        hide: function() {
            try {
                this.options.isVisible = false;
                this.element.addClass("none")
            } catch(t) {
                Config.log(t)
            }
        },
        // 控件基础方法设置控件显示
        show: function() {
            try {
                this.options.isVisible = true;
                this.element.removeClass("none")
            } catch(t) {
                Config.log(t)
            }
        },
        // 控件基础方法 获取控件Enabled 属性值
        getEnabled: function() {
            return (this.options.isEnabled ? 1 : 0) + ""
        },
        // 控件基础方法 获取控件Visible 属性值
        getVisible: function() {
            return (this.options.isVisible ? 1 : 0) + ""
        },
        // 控件基础方法 获取控件Selected 属性值
        getSelected: function() {
            return (this.options.isSelected ? 1 : 0) + ""
        },
        // 控件基础方法，在获取TEXT 值之前执行的逻辑
        beforeGetText: function() {},
        // 控件基础方法 获取控件Text
        getText: function() {
            this.beforeGetText();
            return this.options.text
        },



        //对外公开的取值方法 t:具体的属性值 enable/selected/visible/text
        getValue: function(t) {
            var e = "";
            try {
                if (Config.CMD_ATTRIBUTE_ENABLED == t) {
                    e = this.getEnabled()
                } else if (Config.CMD_ATTRIBUTE_SELECTED == t) {
                    e = this.getSelected()
                } else if (Config.CMD_ATTRIBUTE_VISIBLE == t) {
                    e = this.getVisible()
                } else if (Config.CMD_ATTRIBUTE_TEXT == t) {
                    e = this.getText()
                } else {
                    e = this.getText()
                }
            } catch(i) {
                Config.log(i)
            }
            return e
        },

        // 对外公开的设置Enabled 属性
        setEnabled: function(t) {
            try {
                if (t == "1" || t == true) this.enable();
                else this.disable()
            } catch(e) {
                Config.log(e);
                return false
            }
            return true
        },

        // 对外公开的 设置Visible 属性
        setVisible: function(t) {
            try {
                if (t == "1" || t == true) this.show();
                else this.hide()
            } catch(e) {
                Config.log(e);
                return false
            }
            return true
        },
        // 对外公开的设置Selected 属性值
        setSelected: function(t) {
            try {
                if (t == "1" || t == true) this.select();
                else this.deselect()
            } catch(e) {
                Config.log(e);
                return false
            }
            return true
        },
        // 内部方法设置Text 属性值
        setText: function(e) {
            try {
                this.options.text = e;
                if (this.options.relation == FlashType.RELY) {
                    t("#" + this.options.linkId).show()
                }
                if (this.options.directParent != null && this.options.row > 1) {
                    this.options.directParent.resetRow(this.options.row)
                }
            } catch(i) {
                Config.log(i);
                return false
            }
            return true
        },
        // 对外公开的 设置Value 属性 t 是值 e 是属性
        setValue: function(t, e) {
            var i = true;
            try {
                if (Config.CMD_ATTRIBUTE_ENABLED == e) {
                    i = this.setEnabled(t)
                } else if (Config.CMD_ATTRIBUTE_SELECTED == e) {
                    i = this.setSelected(t)
                } else if (Config.CMD_ATTRIBUTE_VISIBLE == e) {
                    i = this.setVisible(t)
                } else if (Config.CMD_ATTRIBUTE_TEXT == e) {
                    i = this.setText(t)
                } else {
                    i = this.setText(t)
                }
            } catch(o) {
                Config.log(o);
                i = false
            }
            return i
        },
        // 对外空开方法 取消获取焦点状态
        clearFocus: function() {
            this.options.hasFocus = false
        },
        // 对外公开方法 设置获取焦点状态
        setFocus: function() {
            this.options.hasFocus = true;
            return true
        },
        // 按键操作
        keyAction: function(t) {
            return true
        },
        // 对外方法隐藏自己
        hideIt: function(e) {
            try {
                if (e == undefined) this.hide();
                else t(e).addClass("none")
            } catch(i) {
                Config.log(i)
            }
        },
        // 对外方法显示自己
        showIt: function(e) {
            try {
                if (e == undefined) this.show();
                else t(e).removeClass("none")
            } catch(i) {
                Config.log(i)
            }
        },
        // 对外方法修改Color
        changeColor: function(e, i) {
            try {
                if (i == undefined) {
                    this.element.css("color", e)
                } else {
                    t(i).css("color", e)
                }
            } catch(o) {
                Config.log(o)
            }
        },
        // 开始播放Flassh
        startFlash: function() {
            try {
                if (this.options.flashType == FlashType.VISIBLE) {
                    this.propertyInit = this.options.isVisible;
                    this.propertyCurrent = this.options.isVisible
                } else if (this.options.flashType == FlashType.COLOR) {
                    this.propertyInit = this.element.css("color");
                    this.propertyCurrent = this.options.color
                }
                this.flashCount = 0;
                this.options.flashCount *= 2;
                this.flash();
                this.flashId = setInterval(this.flash, this.options.flashInterval, this)
            } catch(t) {
                Config.log(t)
            }
        },

        flash: function(e) {
            try {
                if (e == undefined) e = this;
                if (e.options.flashType == FlashType.VISIBLE) {
                    e.propertyCurrent = !e.propertyCurrent;
                    if (e.propertyCurrent) {
                        e.showIt()
                    } else {
                        e.hideIt()
                    }
                    if (e.options.linkId != "") {
                        var i = e.options.linkId.split(",");
                        t.each(i,
                        function() {
                            if (e.options.relation == FlashType.OPPOSITE) {
                                if (e.propertyCurrent) {
                                    e.hideIt("#" + this)
                                } else {
                                    e.showIt("#" + this)
                                }
                            } else {
                                if (e.propertyCurrent) {
                                    e.showIt("#" + this)
                                } else {
                                    e.hideIt("#" + this)
                                }
                            }
                        })
                    }
                } else if (e.options.flashType == FlashType.COLOR) {
                    if (e.propertyCurrent == e.options.color) e.propertyCurrent = e.options.disabledColor;
                    else e.propertyCurrent = e.options.color;
                    e.changeColor(e.propertyCurrent);
                    if (e.options.linkId != "") {
                        var i = e.options.linkId.split(",");
                        t.each(i,
                        function() {
                            if (e.options.relation == FlashType.OPPOSITE) {
                                if (e.propertyCurrent == e.options.color) e.changeColor(e.options.disabledColor, "#" + this);
                                else e.changeColor(e.options.color, "#" + this)
                            } else {
                                e.changeColor(e.propertyCurrent, "#" + this)
                            }
                        })
                    }
                }
                if (e.options.flashCount > 0) {
                    e.flashCount++;
                    if (e.flashCount >= e.options.flashCount) {
                        e.stopFlash()
                    }
                }
            } catch(o) {
                Config.log(o)
            }
        },
        stopFlash: function() {
            try {
                this.flashCount = 0;
                this.options.flashCount = 0;
                clearInterval(this.flashId);
                if (this.options.flashType == FlashType.VISIBLE) {
                    if (this.propertyInit) {
                        this.showIt()
                    } else {
                        this.hideIt()
                    }
                    if (this.options.linkId != "") {
                        var e = this.options.linkId.split(",");
                        var i = this;
                        t.each(e,
                        function() {
                            if (i.options.relation == FlashType.OPPOSITE) {
                                if (i.options.isVisible) {
                                    i.hideIt("#" + this)
                                } else {
                                    i.showIt("#" + this)
                                }
                            } else {
                                if (i.options.isVisible) {
                                    i.showIt("#" + this)
                                } else {
                                    i.hideIt("#" + this)
                                }
                            }
                        })
                    }
                } else if (this.options.flashType == FlashType.COLOR) {
                    this.propertyCurrent = this.propertyInit;
                    this.changeColor(this.propertyCurrent);
                    if (this.options.linkId != "") {
                        var e = this.options.linkId.split(",");
                        var i = this;
                        t.each(e,
                        function() {
                            if (i.options.relation == FlashType.OPPOSITE) {
                                if (i.propertyCurrent == i.options.color) i.changeColor(i.options.disabledColor, "#" + this);
                                else i.changeColor(i.options.color, "#" + this)
                            } else {
                                i.changeColor(i.propertyCurrent, "#" + this)
                            }
                        })
                    }
                }
                this.propertyInit = null;
                this.propertyCurrent = null
            } catch(o) {
                Config.log(o)
            }
        }
    })
})(jQuery); 

//Text 控件
(function(t) {
    t.widget("grgbanking.Text", t.grgbanking.Component, {
        mouseOver: false,
        scrollId: null,
        options: {
            type: Config.TYPE_TEXT,
            color: "",
            disabledColor: "",
            canScroll: false,
            scrollLength: 1e3,
            scrollIndex: 0,
            scrollInterval: 800,
            scrollColor: "#FF0000",
            prefix: "",
            mouseControlScroll: true,
            scrollType: TextScrollType.CONTAINER,
            maxPixel: 100,
            scrollSpace: "   "
        },

        readConfig: function() {
            try {
                this.options["text"] = this.element.html();
                this._super()
            } catch(t) {
                Config.log(t)
            }
        },

        initListener: function() {
            try {
                var t = this;
                this.element.on("mouseover",
                function() {
                    if (t.options.mouseControlScroll) {
                        t.mouseOver = true
                    }
                }).on("mouseout",
                function() {
                    if (t.options.mouseControlScroll) {
                        t.mouseOver = false
                    }
                })
            } catch(e) {
                Config.log(e)
            }
        },
        removeListener: function() {
            try {
                this.stopScroll();
                this.element.off().unbind()
            } catch(t) {
                Config.log(t)
            }
        },
        // 基础方法更新控件状态
        updateState: function() {
            try {
                if (!this.options.isEnabled) {
                    this.element.addClass("disabled")
                } else {
                    this.element.removeClass("disabled")
                }
                if (this.options.isSelected) {
                    this.element.addClass("selected")
                } else {
                    this.element.removeClass("selected")
                }
                if (this.isScroll()) {
                    if (this.options.isEnabled) {
                        if (this.options.scrollColor != "") this.element.css("color", this.options.scrollColor)
                    } else {
                        if (this.options.disabledColor != "") this.element.css("color", this.options.disabledColor)
                    }
                    this.startScroll()
                } else {
                    this.stopScroll();
                    if (this.options.isEnabled) {
                        if (this.options.color != "") this.element.css("color", this.options.color)
                    } else {
                        if (this.options.disabledColor != "") this.element.css("color", this.options.disabledColor)
                    }
                }
            } catch(t) {
                Config.log(t)
            }
        },
        // 判断滚动条
        checkScroll: function() {
            try {
                if (this.isScroll()) {
                    this.startScroll()
                }
            } catch(t) {
                Config.log(t)
            }
        },

        startScroll: function() {
            try {
                this.stopScroll();
                this.element.addClass("scroll-config");
                this.options.scrollIndex = -1;
                this.mouseOver = false;
                this.scroll();
                this.scrollId = setInterval(this.scroll, this.options.scrollInterval, this)
            } catch(t) {
                Config.log(t)
            }
        },
        stopScroll: function() {
            try {
                this.element.css("color", "");
                this.element.removeClass("scroll-config");
                if (this.scrollId != null) clearInterval(this.scrollId)
            } catch(t) {
                Config.log(t)
            }
        },

        scroll: function(t) {
            try {
                if (t == undefined) t = this;
                if (t.mouseOver) return;
                t.options.scrollIndex++;
                var e = 0;
                var i = t.options.scrollSpace + t.options.text;
                if (t.options.scrollIndex >= i.length) {
                    t.options.scrollIndex = 0
                }
                var o = i.length - (t.options.scrollIndex + 1);
                var n = "";
                if (t.options.scrollType == TextScrollType.CONTAINER) {
                    n = i.substr(t.options.scrollIndex);
                    if (t.options.scrollIndex > 0) n += i.substr(0, t.options.scrollIndex)
                } else {
                    if (o < t.options.scrollLength) {
                        n = i.substr(t.options.scrollIndex);
                        n += i.substr(0, t.options.scrollLength - o - 1)
                    } else {
                        n = i.substr(t.options.scrollIndex, t.options.scrollLength)
                    }
                }
                if (t.options.prefix != "") {
                    n = t.options.prefix + " " + n
                }
                t.element.html(n.replace(/ /g, "&nbsp;"))
            } catch(s) {
                Config.log(s)
            }
        },
        test: function() {
            try {
                t(window).keyup(function(t) {})
            } catch(e) {
                Config.log(e)
            }
        },

        isScroll: function() {
            var t = false;
            try {
                if (!this.options.canScroll) {
                    return t
                }
                var e;
                if (this.options.prefix == "") {
                    e = this.options.text
                } else {
                    e = this.options.prefix + " " + this.options.text
                }
                var i = this.element.get(0);
                if (this.options.scrollType == TextScrollType.CONTAINER) {
                    if (i.scrollWidth > i.offsetWidth) {
                        t = true
                    }
                } else if (this.options.scrollType == TextScrollType.PIXEL) {
                    if (i.scrollWidth > this.options.maxPixel) {
                        t = true
                    }
                } else {
                    if (e.length > this.options.scrollLength) {
                        t = true
                    }
                }
            } catch(o) {
                Config.log(o)
            }
            return t
        },
        // 对外方法设置控件选中
        select: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        // 对外方法设置控件不选中
        deselect: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        disable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        enable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        setText: function(t) {
            var e = true;
            try {
                this._super(t);
                this.element.html(t);
                this.updateState()
            } catch(i) {
                Config.log(i);
                e = false
            }
            return e
        },
        _create: function() {
            try {
                this.readConfig();
                this.initListener();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        destroy: function() {
            try {
                this.removeListener();
                t.Widget.prototype.destroy.call(this);
                this.element.remove()
            } catch(e) {
                Config.log(e)
            }
        }
    })
})(jQuery); 

(function(t) {
    t.widget("grgbanking.Button", t.grgbanking.Component, {
        isSpecial: false,
        options: {
            type: Config.TYPE_BUTTON,
            id: "",
            eventData: null,
            isAutoSend: true,
            dataToSend: "",
            isToggle: false,
            isSingleToggle: true,
            parentSelector: "",
            toggleName: ""
        },
        readConfig: function() {
            this._super()
        },
		clickAction:function() {
                    e=this;
                    if (!e.canAction(t(this))) return;
                    e._trigger("preclick", null, e.options.eventData);
                    e.setToggleState();
                    var i = null;
                    if (e.options.isAutoSend) {
                        if (e.options.dataToSend == "") {
                            if (e.options.id != "") {
                                i = {
                                    action: "click",
                                    data: {
                                        id: e.options.id
                                    }
                                };
                                Config.send(i)
                            }
                        } else {
                            i = {
                                action: "click",
                                data: {
                                    id: e.options.dataToSend
                                }
                            };
                            Config.send(i)
                        }
                    }
                    e._trigger("afterclick", null, e.options.eventData)
                },
        initListener: function() {
            try {
                var e = this;
                /*this.element.on("touchend",function() {
                    if (!e.canAction(t(this))) return;
					Config.log("touchend happen");
                    e.clickAction();
                })*/
				
				this.element.click(function() {
                   Config.log("click happen");
                   e.clickAction();
                });
				
            } catch(i) {
                Config.log(i)
            }
        },
        removeListener: function() {
            try {
                this.element.off().unbind()
            } catch(t) {
                Config.log(t)
            }
        },
        updateState: function() {
            try {
                this.enableButton(this.element, this.options.isEnabled);
                if (this.options.isSelected) {
                    this.element.addClass("selected")
                } else {
                    this.element.removeClass("selected")
                }
            } catch(t) {
                Config.log(t)
            }
        },
        setToggleState: function() {
            try {
                if (this.options.isToggle) {
                    if (this.options.isSingleToggle) {
                        var e = this;
                        var i;
                        if (this.options.parentSelector != "") i = this.element.closest(this.options.parentSelector);
                        else i = this.element.parent();
                        i.find("button").each(function(i) {
                            var o = t(this).Button("instance");
                            if (o != e && o.options.isToggle && o.options.toggleName == e.options.toggleName) {
                                o.deselect()
                            }
                        })
                    }
                    this.select()
                }
            } catch(o) {
                Config.log(o)
            }
        },
        test: function() {
            try {
                t(window).keyup(function(t) {})
            } catch(e) {
                Config.log(e)
            }
        },
        enableButton: function(t, e) {
            try {
                if (e) {
                    t.removeAttr("disabled").removeClass("disabled")
                } else {
                    t.attr("disabled", "disabled").addClass("disabled")
                }
            } catch(i) {
                Config.log(i)
            }
        },
        showKey: function() {
            try {
                if (this.options.hasFocus && Config.showKeyTip()) {
                    this.element.find(".tip").html(this.options.key)
                } else {
                    this.element.find(".tip").html("")
                }
            } catch(t) {
                Config.log(t)
            }
        },
        select: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        deselect: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        disable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        enable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        setText: function(t) {
            var e = true;
            try {
                this._super(t);
                this.element.find(".button-text").html(t)
            } catch(i) {
                Config.log(i);
                e = false
            }
            return e
        },
        clearFocus: function() {
            try {
                this.options.hasFocus = false;
                this.element.find(".tip").html("")
            } catch(t) {
                Config.log(t);
                return false
            }
            return true
        },
        setFocus: function() {
            try {
                this.clearFocus();
                this._super();
                this.showKey()
            } catch(t) {
                Config.log(t);
                return false
            }
            return true
        },
        keyAction: function(t) {
            try {
                this.element.trigger("click");
                return true
            } catch(e) {
                Config.log(e)
            }
            return false
        },
        _create: function() {
            try {
                this.readConfig();
                this.initListener();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        destroy: function() {
            try {
                this.removeListener();
                t.Widget.prototype.destroy.call(this);
                this.element.remove()
            } catch(e) {
                Config.log(e)
            }
        }
    })
})(jQuery); 
(function(t) {
    t.widget("grgbanking.Promptbox", t.grgbanking.Component, {
        defaultOk: "",
        defaultCancel: "",
        sureButton: null,
        cancelButton: null,
        hideId: null,
        options: {
            type: Config.TYPE_PROMPT_BOX,
            id: "",
            promptType: Config.UI_OK_CANCEL,
            text: "",
            time: 0
        },
        defaults: {
            deltaX: 0,
            deltaY: 0,
            width: 400,
            height: 224,
            paddingLeft: 10,
            paddingRight: 10,
            paddingTop: 10,
            paddingBottom: 10,
            hAlign: AlignType.CENTER,
            size: 23,
            bold: false,
            leading: 0,
            letterSpacing: 0,
            indent: 0,
            blockIndent: 0,
            color: "#474747",
            font: Config.Font,
            ok: "",
            cancel: "",
            okData: "",
            cancelData: "",
            ui: "",
            isLoading: false
        },
        readConfig: function() {
            this.getDefaultConfig();
            this._super()
        },
        getDefaultConfig: function() {
            var e = t.extend(this.options, this.defaults);
            return e
        },
        getPromtConfig: function(e) {
            var i = this.getDefaultConfig();
            if (e == undefined || e == null) return i;
            try {
                i.ok = "";
                i.cancel = "";
                i.okData = "";
                i.cancelData = "";
                var o = "";
                t.each(e,
                function(t,e) {
                    o = e.t.toUpperCase();
                    switch (o) {
                    case "UI":
                        i.ui = e.e;
                        break;
                    case "ISLOADING":
                        i.isLoading = e.e == "false" || e.e == false ? false: true;
                        break;
                    case "WIDTH":
                        i.width = Number(e.e);
                        break;
                    case "HEIGHT":
                        i.height = Number(e.e);
                        break;
                    case "DELTAX":
                        i.deltaX = Number(e.e);
                        break;
                    case "DELTAY":
                        i.deltaY = Number(e.e);
                        break;
                    case "PADDING":
                        try {
                            var n = e.e.split(",");
                            i.paddingLeft = Number(n[0]);
                            i.paddingTop = Number(n[1]);
                            i.paddingRight = Number(n[2]);
                            i.paddingBottom = Number(n[3])
                        } catch(s) {}
                        break;
                    case "ALIGN":
                        i.hAlign = e.e;
                        break;
                    case "SIZE":
                        i.size = Number(e.e);
                        break;
                    case "BOLD":
                        i.bold = e.e == "false" || e.e == false ? false: true;
                        break;
                    case "LEADING":
                        i.leading = Number(e.e);
                        break;
                    case "LETTERSPACING":
                        i.letterSpacing = Number(e.e);
                        break;
                    case "INDENT":
                        i.indent = Number(e.e);
                        break;
                    case "BLOCKINDENT":
                        i.blockIndent = Number(e.e);
                        break;
                    case "COLOR":
                        i.color = e.e;
                        break;
                    case "FONT":
                        i.font = e.e;
                        break;
                    case "OK":
                        i.ok = e.e;
                        break;
                    case "CANCEL":
                        i.cancel = e.e;
                        break;
                    case "OKDATA":
                        i.okData = e.e;
                        break;
                    case "CANCELDATA":
                        i.cancelData = e.e;
                        break
                    }
                })
            } catch(n) {
                Config.log(n)
            }
            return i
        },
        addButtons: function() {
            try {
                this.sureButton = t("#FUN_OK").Button().Button("instance");
                this.cancelButton = t("#FUN_CANCEL").Button().Button("instance");
                if (this.options.ok != "") {
                    this.defaultOk = this.options.ok
                } else {
                    this.defaultOk = Config.PromptSureText
                }
                if (this.options.cancel != "") {
                    this.defaultCancel = this.options.cancel
                } else {
                    this.defaultCancel = Config.PromptCancelText
                }
            } catch(e) {
                Config.log(e)
            }
        },
        initListener: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        removeListener: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        updateState: function() {
            try 
            {
                var realcontentheight=$("#icon-and-content").outerHeight();
                var contentheight = $("#icon-and-content").height();
                var bigheight=$("#prompt-box-wrapper").height();

                var realbigheight = 0;
                realbigheight = bigheight + realcontentheight - contentheight;

                $("#prompt-box-wrapper").css("height", realbigheight + "px");

            } catch(t) {
                Config.log(t);
            }
        },
        test: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        clearChild: function() {
            try {
                t("#prompt-content-ui").empty()
            } catch(e) {
                Config.log(e)
            }
        },
        setText: function(e) {
            var i = true;
            try {
                this._super(e);
               //e=e.replace(/\r\n/g, "<br>");
               // e = e.replace(/\n/g, "<br>");
                //e = e.replace(/\s/g,"&nbsp;");  
                t("#prompt-box-text").html(e);
            } catch(o) {
                Config.log(o);
                i = false
            }
            return i
        },
        clearFocus: function() {
            try {
                this._super();
                this.sureButton.clearFocus();
                this.cancelButton.clearFocus()
            } catch(t) {
                Config.log(t);
                return false
            }
            return true
        },
        setFocus: function() {
            try {
                this.clearFocus();
                this._super();
                this.sureButton.options.key = "1";
                this.cancelButton.options.key = "2";
                this.sureButton.setFocus();
                this.cancelButton.setFocus()
            } catch(t) {
                Config.log(t);
                return false
            }
            return true
        },
        keyAction: function(t) {
            try {
                if (t == "1") {
                    this.sureButton.keyAction(t)
                } else if (t == "2") {
                    this.cancelButton.keyAction(t)
                }
                return true
            } catch(e) {
                Config.log(e)
            }
            return false
        },
        _create: function() {
            try {
                this.readConfig();
                this.addButtons();
                this.initListener();
                Config.Prompt = this
            } catch(t) {
                Config.log(t)
            }
        },
        init: function() {
            try {
                if (this.hideId != null) {
                    clearTimeout(this.hideId);
                    this.hideId = null
                }
                t("#prompt-content-ui").empty();
                // 设置偏移量
                this.element.find(".middle-outer:first").css({
                    left: this.options.deltaX + "px",
                    top: this.options.deltaY + "px"
                });

                // 设置进度动画
                if (this.options.promptType == Config.UI_PROMPT_LOADING) this.options.isLoading = true;
                if (this.options.isLoading) {
                    t("#prompt-content-ui").hide();
                    t("#prompt-box-icon").show();
                    t("#prompt-box-text").show();
                    t("#prompt-box-wrapper").addClass("Progress-prompt")
                } else {
                    t("#prompt-box-wrapper").removeClass("Progress-prompt");
                    if (this.options.ui != "") {
                        t("#prompt-content-ui").show();
                        t("#prompt-box-icon").hide();
                        t("#prompt-box-text").hide()
                    } else {
                        t("#prompt-content-ui").hide();
                        t("#prompt-box-icon").hide();
                        t("#prompt-box-text").show()
                    }
                }
                

                // 设置内容框偏移值,水平居中，垂直居中
                 t("#icon-and-content").css({
                       "padding-bottom": this.options.paddingBottom + "px",
                       "padding-top": this.options.paddingTop + "px",
                       "padding-right": this.options.paddingRight + "px",
                       "padding-left": this.options.paddingLeft + "px",
                       "text-align":this.options.hAlign
                    }
                );

                // 设置文本字体
                
                t("#prompt-box-content").css({
                       "font-size": this.options.size ,
                       "font-family":this.options.font,
                       "font-weight":this.options.bold
                    }
                );
                // 设置按钮参数值
                this.sureButton.options.dataToSend = this.options.okData;
                this.cancelButton.options.dataToSend = this.options.cancelData;
                switch (this.options.promptType) {
                case Config.UI_OK:
                    this.sureButton.show();
                    this.cancelButton.hide();
                    if (this.options.ok != "") {
                        this.sureButton.setValue(this.options.ok)
                    } else {
                        this.sureButton.setValue(this.defaultOk)
                    }
                    break;
                case Config.UI_OK_CANCEL:
                    this.sureButton.show();
                    this.cancelButton.show();
                    if (this.options.ok != "") {
                        this.sureButton.setValue(this.options.ok)
                    } else {
                        this.sureButton.setValue(this.defaultOk)
                    }
                    if (this.options.cancel != "") {
                        this.cancelButton.setValue(this.options.cancel)
                    } else {
                        this.cancelButton.setValue(this.defaultCancel)
                    }
                    break;
                case Config.UI_ALPHAPROMPT:
                    if (this.options.text.indexOf(Config.CMD_INVAL_SEPARATOR) != -1) {
                        this.options.text = this.options.text.substr(1)
                    }
                    this.sureButton.hide();
                    this.cancelButton.hide();
                    break;
                default:
                    this.sureButton.hide();
                    this.cancelButton.hide();
                    break;
                }
                //设置
                this.setValue(this.options.text);
                this.show();
                // 设置底部按钮显示属性
                t("#prompt-box-footer").removeClass("none");
                if (this.sureButton.isVisible() || this.cancelButton.isVisible()) {
                    t("#prompt-box-footer").removeClass("none");
                    t("#icon-and-content").removeClass("prompt-content-no-footer")
                } else {
                    t("#prompt-box-footer").addClass("none");
                    t("#icon-and-content").addClass("prompt-content-no-footer")
                }
                if (this.options.time > 0) {
                    this.hideId = setTimeout(this.hideMe, this.options.time * 1e3, this)
                }

                // 设置内容框宽度
                t("#prompt-box-wrapper").css({
                        width: this.options.width + "px",
                        height:this.options.height+"px"
                    }
                );

                var headHeight = t("#prompt-box-header").css("height");
                var headvalue = 0;
                if (headHeight==undefined||isNaN(headHeight.replace("px","")))
                    headvalue = 0;
                else {
                    headvalue = parseInt(headHeight.replace("px", ""));
                }


                if (t("#prompt-box-header").hasClass("none"))
                    headvalue = 0;

                var footHeight=t("#prompt-box-footer").css("height");
                var footvalue = 0;
                if (footHeight==undefined||isNaN(footHeight.replace("px","")))
                    footvalue = 0;
                else {
                    footvalue = parseInt(footHeight.replace("px", ""));
                }

                if (t("#prompt-box-footer").hasClass("none"))
                    footvalue = 0;

                t("#icon-and-content").css({
                       width: this.options.width + "px",
                       height:this.options.height-headvalue-footvalue+"px"
                    }
                );

                this.updateState();
            } catch(e) {
                Config.log(e)
            }
        },
        prompt: function(t) {
            var e = {
                action: "prompt",
                success: true,
                message: "",
                data: []
            };
            try {
                var i = Number(t.time);
                var o = t.type;
                if (isNaN(i)) i = 0;
                var n = this.getPromtConfig(t.config);
                n.text = t.value;
                n.time = i;
                n.promptType = o;
                switch (o) {
                case Config.UI_PROMPT_LOADING:
                case Config.UI_OK:
                case Config.UI_OK_CANCEL:
                    this.init();
                    break;
                case Config.UI_ALPHAPROMPT:
                    Config.TWIN_KEYBOARDENBLED = false;
                    this.init();
                    break;
                case Config.UI_UNALPHAPROMPT:
                    Config.TWIN_KEYBOARDENBLED = true;
                    this.init();
                    break;
                case Config.UI_PROMPT:
                    this.init();
                    break;
                case Config.UI_UNPROMPT:
                    this.hide();
                    break;
                default:
                    break
                }
            } catch(s) {
                e.success = false;
                e.message = s.stack;
                Config.log(s)
            }
            return e
        },
        hideMe: function(t) {
            try {
                if (t == undefined) t = this;
                t.hide()
            } catch(e) {
                Config.log(e)
            }
        },
        destroy: function() {
            try {
                this.sureButton.destroy();
                this.cancelButton.destroy();
                this.removeListener();
                t.Widget.prototype.destroy.call(this);
                this.element.remove()
            } catch(e) {
                Config.log(e)
            }
        }
    })
})(jQuery); 
(function(t) {
    t.widget("grgbanking.Textbox", t.grgbanking.Component, {
        textbox: null,
        currentRow: 0,
        rowNumberArray: [],
        buttonArray: [],
        buttonShowArray: [],
        pool: [],
        options: {
            type: Config.TYPE_TEXTBOX,
            lineHeight: 25,
            isAppend: false,//是否累加
            focusIndex: -1
        },
        readConfig: function() {
            this._super()
        },
        getTextbox: function() {
            try {
                if (this.textbox == null) {
                    //this.textbox = this.element.find("textarea")
					this.textbox = this.element.find("div[class='textbox-content']");					
                }
                return this.textbox
            } catch(t) {
                Config.log(t)
            }
        },
        appendChild: function() {
            try {
                if (Config.textboxChild == undefined) {
                    var t = "";
                    t += "          <!--按钮区域-->";
                    t += '          <div class="textbox-button-zone">';
                    t += '              <div class="middle-outer">';
                    t += '                  <div class="middle-inner center">';
                    t += '                      <button class="button-page" mark="top"> <span class="tip"></span> <span class="button-text button-scroll-top"><span class="button-scroll-border"></span><span class="button-scroll-arrow"></span></span> </button>';
                    t += '                      <button class="button-page textbox-button-vspace" mark="up"> <span class="tip"></span> <span class="button-text button-scroll-up"></span></button>';
                    t += '                      <button class="button-page textbox-button-vspace" mark="down"> <span class="tip"></span> <span class="button-text button-scroll-down"></span> </button>';
                    t += '                      <button class="button-page textbox-button-vspace" mark="bottom"> <span class="tip"></span> <span class="button-text button-scroll-bottom"><span class="button-scroll-arrow"></span><span class="button-scroll-border"></span></span> </button>';
                    t += "                  </div>";
                    t += "              </div>";
                    t += "          </div>";
                    t += "          <!--面板容器-->";
                    t += '          <div class="grg-panel"> ';
                    t += "              <!--面板容器-头部-->";
                    t += '              <div class="grg-panel-title textbox-title"> </div>';
                    t += "              <!--面板容器-内容-->";
                    t += '              <div class="grg-panel-content textbox-content-wrapper">';
                    t += '                  <div id="TextContent" class="textbox-content" style="word-break:normal; width:auto; display:block; white-space:pre-wrap;word-wrap : break-word ;overflow: hidden ;" readonly></div>';
                    t += "              </div>";
                    t += "          </div>";
                    Config.textboxChild = t
                }
                this.element.html(Config.textboxChild)
            } catch(e) {
                Config.log(e)
            }
        },
        addHandButton: function() {
            try {
                var e = this.getTextbox();
                var i = e.height();
                var o = parseInt(i / this.options.lineHeight) + 1;
                var n = '<button type="button" class="button-hand"><span class="tip"></span> <span class="button-text"></span></button>';
                var s = t(".textbox-content-wrapper");
                var r;
                for (var a = 0; a < o; a++) {
                    r = t(n).appendTo(s).css("top", a * this.options.lineHeight).addClass("none");
                    this.buttonArray.push(r)
                }
            } catch(l) {
                Config.log(l)
            }
        },
        initListener: function() {
            try {
                var e = this;
                this.getTextbox().on("valuechange",
                function(t, i) {
                    e.updateState()
                }).scroll(function() {
                    e.updateState()
                });
                this.element.find(".textbox-button-zone button").click(function() {
                    if (!e.canAction(t(this))) return;
                    var i = t(this).attr("mark").toUpperCase();
                    switch (i) {
                    case "TOP":
                        e.scrollTop();
                        break;
                    case "UP":
                        e.scrollUp();
                        break;
                    case "DOWN":
                        e.scrollDown();
                        break;
                    case "BOTTOM":
                        e.scrollBottom();
                        break;
                    default:
                        break
                    }
                    e.updateState()
                });
                t.each(this.buttonArray,
                function() {
                    t(this).click(function() {
                        if (!e.canAction(t(this))) return;
                        e.currentRow = t(this).attr("row");
                        Config.send("FUN_9999")
                    })
                })
            } catch(i) {
                Config.log(i)
            }
        },
        removeListener: function() {
            try {
                this.getTextbox().off();
                this.element.find(".textbox-button-zone button").off();
                t.each(this.buttonArray,
                function() {
                    t(this).off()
                })
            } catch(e) {
                Config.log(e)
            }
        },
        scrollTop: function() {
            try {
                this.getTextbox().get(0).scrollTop = 0
            } catch(t) {
                Config.log(t)
            }
        },
        scrollUp: function() {
            try {
                var t = this.getTextbox().get(0);
                var e = parseInt(t.scrollTop / this.options.lineHeight);
                e = Math.max(0, --e);
                var i = e * this.options.lineHeight;
                t.scrollTop = i
            } catch(o) {
                Config.log(o)
            }
        },
        scrollDown: function() {
            try {
                var t = this.getTextbox().get(0);
                var e = parseInt(t.scrollTop / this.options.lineHeight); ++e;
                var i = e * this.options.lineHeight;
                i = Math.min(i, t.scrollHeight);
                t.scrollTop = i
            } catch(o) {
                Config.log(o)
            }
        },
        scrollBottom: function() {
            try {
                var t = this.getTextbox().get(0);
                t.scrollTop = t.scrollHeight
            } catch(e) {
                Config.log(e)
            }
        },
        updateState: function() {
            try {
                var t = this.getTextbox().get(0);
                var e = this.getTextbox().height();
                var i = t.scrollTop;
                var o = t.scrollHeight;
                var n = i <= 0;
                var s = o - i - e <= 0;
                if (!s && i % this.options.lineHeight != 0) {
                    i = Math.floor(i / this.options.lineHeight) * this.options.lineHeight;
                    t.scrollTop = i
                }
                var r = this.element.find(".textbox-button-zone button[mark=top]");
                var a = this.element.find(".textbox-button-zone button[mark=up]");
                var l = this.element.find(".textbox-button-zone button[mark=down]");
                var h = this.element.find(".textbox-button-zone button[mark=bottom]");
                if (this.options.isEnabled) {
                    this.getTextbox().removeAttr("disabled").removeClass("disabled");
                    if (n) {
                        this.enableButton(r, false);
                        this.enableButton(a, false)
                    } else {
                        this.enableButton(r, true);
                        this.enableButton(a, true)
                    }
                    if (s) {
                        this.enableButton(l, false);
                        this.enableButton(h, false)
                    } else {
                        this.enableButton(l, true);
                        this.enableButton(h, true)
                    }
                } else {
                    this.getTextbox().attr("disabled", "disabled").addClass("disabled");
                    this.enableButton(r, false);
                    this.enableButton(a, false);
                    this.enableButton(l, false);
                    this.enableButton(h, false)
                }
                this.setHandButton()
            } catch(c) {
                Config.log(c)
            }
        },
        test: function() {
            try {
                var e = 100;
                var i = "";
                for (var o = 0; o < e; o++) {
                    i += o + "钞票信息查询 冠字号信息 交易类型:取款" + "\n"
                }
                this.getTextbox().val(i);
                t(window).keyup(function(t) {})
            } catch(n) {
                Config.log(n)
            }
        },
        enableButton: function(t, e) {
            try {
                if (e) {
                    t.removeAttr("disabled").removeClass("disabled")
                } else {
                    t.attr("disabled", "disabled").addClass("disabled")
                }
            } catch(i) {
                Config.log(i)
            }
        },
        setHandButton: function() {
            try {
                var t = this.rowNumberArray.length;
                if (t <= 0) {
                    return
                }
                var e;
                var i;
                var o = 0;
                var n;
                var s = this.getTextbox().get(0);
                var r = Math.floor(s.scrollTop / this.options.lineHeight);
                var a = Math.ceil(this.getTextbox().height() / this.options.lineHeight);
                this.buttonShowArray.splice(0);
                for (o = 0; o < t; o++) {
                    e = this.rowNumberArray[o];
                    if (e <= r + a - 1 && e >= r) {
                        if (o < this.buttonArray.length) {
                            n = e - r;
                            i = this.buttonArray[n];
                            i.attr("row", e);
                            if (this.buttonShowArray.length >= 10 - 4) {
                                i.addClass("none")
                            } else {
                                i.removeClass("none");
                                this.buttonShowArray.push(n);
                                i.attr("row", e)
                            }
                        }
                    }
                }
                t = this.buttonArray.length;
                for (o = 0; o < t; o++) {
                    i = this.buttonArray[o];
                    this.enableButton(i, this.options.isEnabled);
                    if (this.buttonShowArray.indexOf(o) == -1) {
                        i.addClass("none")
                    } else {
                        i.removeClass("none")
                    }
                }
                this.showHandKey()
            } catch(l) {
                Config.log(l)
            }
        },
        showHandKey: function() {
            try {
                var t;
                var e = this.buttonShowArray.length;
                var o;
                if (this.options.hasFocus && Config.showKeyTip()) {
                    for (i = 0; i < e; i++) {
                        o = 5 + i;
                        if (o == 10) o = 0;
                        t = this.buttonArray[this.buttonShowArray[i]];
                        t.find(".tip").html(o)
                    }
                } else {
                    for (i = 0; i < e; i++) {
                        t = this.buttonArray[this.buttonShowArray[i]];
                        t.find(".tip").html("")
                    }
                }
            } catch(n) {
                Config.log(n)
            }
        },
        saveRowNumber: function(t) {
            try {
                this.rowNumberArray = t.split(",");
                var e = this.rowNumberArray.length;
                for (var i = 0; i < e; i++) {
                    this.rowNumberArray[i] = parseInt(this.rowNumberArray[i])
                }
            } catch(o) {
                Config.log(o)
            }
        },
        disable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        enable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        loadTxt: function(e) {
            try {
                var i = t.ajax({
                    url: e,
                    async: false
                });
                this.setText(i.responseText)
            } catch(o) {
                Config.log(o)
            }
        },
        setText: function(t) {
            var e = true;
            try {
                if (t == undefined || t == "") {
                    this.options.text = "";
                    this.getTextbox().html(this.options.text);
                } else if (t.substr(0, 1) == Config.CMD_INVAL_SEPARATOR) {
                    var i;
                    var o = t.substr(1, t.length);
                    if ( - 1 != o.indexOf(Config.CMD_INVAL_SEPARATOR)) {
                        this.saveRowNumber(o.substr(o.indexOf(Config.CMD_INVAL_SEPARATOR) + 1, o.length));
                        i = o.substr(0, o.indexOf(Config.CMD_INVAL_SEPARATOR))
                    } else {
                        i = o.substr(0, o.length)
                    }
                    this.loadTxt(i)
                } else {
                    var n;
                    if ( - 1 != t.indexOf(Config.CMD_INVAL_SEPARATOR)) {
                        this.saveRowNumber(t.substr(t.indexOf(Config.CMD_INVAL_SEPARATOR) + 1, t.length));
                        n = t.substr(0, t.indexOf(Config.CMD_INVAL_SEPARATOR))
                    } else {
                        n = t
                    }
                    var s = n.split("\n");
                    var r = "";
                    for (var a = 0; a < s.length; a++) {
                        r += s[a]+ "<br/>";
                    }
                    t = r;
					
					t=t.replace(/ /g, "&nbsp;");
					
                    if (this.options.isAppend) {
                        this.options.text += "<br/>" + t;
                        this.getTextbox().html(this.options.text);
                        this.scrollBottom();
                        this.updateState()
                    } else {
                        this.options.text = t;
                        this.getTextbox().html(this.options.text);
                        this.scrollTop();
                        this.updateState()
                    }
                }
            } catch(l) {
                Config.log(l);
                e = false
            }
            return e
        },
        clearFocus: function() {
            try {
                this.options.hasFocus = false;
                this.element.find(".textbox-button-zone button .tip").html("");
                this.showHandKey()
            } catch(t) {
                Config.log(t);
                return false
            }
            return true
        },
        setFocus: function() {
            try {
                this.clearFocus();
                this._super();
                if (this.options.hasFocus && Config.showKeyTip()) {
                    this.element.find(".textbox-button-zone button").each(function(e) {
                        t(this).find(".tip").html(e + 1)
                    });
                    this.showHandKey()
                }
            } catch(e) {
                Config.log(e);
                return false
            }
            return true
        },
        keyAction: function(t) {
            try {
                var e = parseInt(t);
                if (isNaN(e)) return false;
                if (e == 0) e = 10;
                if (e >= 1 && e <= 4) {
                    this.element.find(".textbox-button-zone button:eq(" + (e - 1) + ")").trigger("click")
                } else if (e >= 5 && e <= 10) {
                    e -= 5;
                    this.buttonArray[this.buttonShowArray[e]].trigger("click")
                }
                return true
            } catch(i) {
                Config.log(i)
            }
            return false
        },
        _create: function() {
            try {
                this.readConfig();
                this.appendChild();
                this.addHandButton();
                this.initListener();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        destroy: function() {
            try {
                this.removeListener();
                t.Widget.prototype.destroy.call(this);
                this.element.remove()
            } catch(e) {
                Config.log(e)
            }
        }
    })
})(jQuery); 
(function(t) {
    t.widget("grgbanking.InputText", t.grgbanking.Component, {
        mouseOver: false,
        options: {
            type: Config.TYPE_INPUT,
            color: "",
            disabledColor: "",
            password: false,
            maxlength: 20,
            validchars: "",// 有效字符范围
            restrict: "",// 正则表达式（未实现）
            validMode: true,// 有效性验证方式，分为正则和直接验证
            numberPattern: "0123456789",// 是否输入数字
            numinput: false// 是否输入数字
        },
        readConfig: function() {
            try {
                this._super();
                var t = "";
                var e = "value";
                t = this.element.attr(e);
                if (t != undefined) {
                    this.options["text"] = t
                }
            } catch(i) {
                Config.log(i)
            }
        },
        initListener: function() {
            try {
                var t = this;
                this.element.on("valuechange",
                function(e, i) {
                    t.removeInvalidChar()
                }).on("click",
                function() {
                    Config.INPUT_TARGET = null;
                    t.setInputTarget();
                    t.setFocus()
                })
            } catch(e) {
                Config.log(e)
            }
        },
        setInputTarget: function() {
            try {
                if (Config.INPUT_TARGET == null) Config.INPUT_TARGET = this
            } catch(t) {
                Config.log(t)
            }
        },
        removeListener: function() {
            try {
                this.element.off().unbind()
            } catch(t) {
                Config.log(t)
            }
        },
        updateState: function() {
            try {
                if (this.options.isSelected) {
                    this.element.addClass("selected")
                } else {
                    this.element.removeClass("selected")
                }
                if (this.options.isEnabled) {
                    this.element.removeAttr("disabled").removeClass("disabled")
                } else {
                    this.element.attr("disabled", "disabled").addClass("disabled")
                }
            } catch(t) {
                Config.log(t)
            }
        },
        removeInvalidChar: function() {
            try {
                var t = this.element.val();
                if (t != "" && !this.isValidChar(t)) {
                    t = this.getValidChar(t);
                    this.element.val(t)
                }
            } catch(e) {
                Config.log(e)
            }
        },
        isValidChar: function(t) {
            try {
                for (var e = 0; e < t.length; e++) {
                    if (this.options.numinput && this.options.numberPattern.indexOf(t.charAt(e)) == -1) return false;
                    if (this.options.validMode && this.options.validchars.indexOf(t.charAt(e)) == -1) return false
                }
            } catch(i) {
                Config.log(i)
            }
            return true
        },
        getValidChar: function(t) {
            var e = "";
            try {
                if (t != "") {
                    for (var i = 0; i < t.length; i++) {
                        if (this.options.validMode && this.options.validchars.indexOf(t.charAt(i)) != -1) {
                            e += t.charAt(i)
                        }
                    }
                }
            } catch(o) {
                Config.log(o)
            }
            return e
        },
        clear: function() {
            try {
                var t = this.element.val();
                var e = "";
                this.element.val(e);
                if (t != e) this.element.trigger("valuechange", [t])
            } catch(i) {
                Config.log(i)
            }
        },
        input: function(t) {
            try {
                var e = this.element.val();
                var i = e + t;
                this.setValue(i);
                if (e != i) this.element.trigger("valuechange", [e])
            } catch(o) {
                Config.log(o)
            }
        },
        test: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        select: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        deselect: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        disable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        enable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        setText: function(t) {
            var e = true;
            try {
                this._super(t);
                if (t.length > this.options.maxlength) return;
                if (!this.isValidChar(t)) return;
                if (this.options.numinput && t.length > 0 && t.charAt(0) == "0") return;
                this.element.val(t)
            } catch(i) {
                Config.log(i);
                e = false
            }
            return e
        },
        getText: function() {
            var t = "";
            try {
                t = this.element.val();
                if (this.options.password) t = Config.CMD_PWD_SEPARATOR + "PWD" + Config.CMD_PWD_SEPARATOR + t + Config.CMD_PWD_SEPARATOR;
            } catch(e) {
                Config.log(e)
            }
            return t
        },
        getFocus: function() {
            try {
                this.element.trigger("focus")
            } catch(t) {
                Config.log(t)
            }
        },
        clearFocus: function() {
            try {
                this.options.hasFocus = false;
                this.deselect()
            } catch(t) {
                Config.log(t);
                return false
            }
            return true
        },
        setFocus: function() {
            try {
                if (Config.showKeyTip()) {
                    this.clearFocus();
                    this._super();
                    if (!this.options.isEnabled) return false;
                    this.select();
                    this.setInputTarget();
                    this.getFocus()
                }
            } catch(t) {
                Config.log(t);
                return false
            }
            return true
        },
        keyAction: function(t) {
            try {
                return true
            } catch(e) {
                Config.log(e)
            }
            return false
        },
        _create: function() {
            try {
                this.readConfig();
                this.setInputTarget();
                this.removeInvalidChar();
                this.initListener();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        destroy: function() {
            try {
                this.removeListener();
                t.Widget.prototype.destroy.call(this);
                this.element.remove()
            } catch(e) {
                Config.log(e)
            }
        }
    })
})(jQuery);
 (function(t) {
    t.widget("grgbanking.Image", t.grgbanking.Component, {
        options: {
            type: Config.TYPE_IMAGE,
            id: ""
        },
        readConfig: function() {
            this._super()
        },
        initListener: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        removeListener: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        updateState: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        test: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        select: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        deselect: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        disable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        enable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        setText: function(t) {
            var e = true;
            try {
                this._super(t);
                this.element.attr("src", t)
            } catch(i) {
                Config.log(i);
                e = false
            }
            return e
        },
        _create: function() {
            try {
                this.readConfig();
                this.initListener();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        destroy: function() {
            try {
                this.removeListener();
                t.Widget.prototype.destroy.call(this);
                this.element.remove()
            } catch(e) {
                Config.log(e)
            }
        }
    })
})(jQuery);
(function (t) {
    t.widget("grgbanking.Keyboard", t.grgbanking.Component, {
        buttonArray: [],
        pageIndex: 1,
        lastStates: [],
        enterButton: null,
        clearButton: null,
        changeButton: null,
        spaceButton: null,
        exitButton: null,
        hasInitButton: false,
        options: {
            type: Config.TYPE_KEYBOARD,
            id: "",
            row: 6,
            column: 5,
            useEnterKey: true,
            useExitKey: false,
            useClearKey: true,
            useChangeKey: true,
            enterKeyIndex: 30,
            exitKeyIndex: 17,
            clearKeyIndex: 28,
            changeKeyIndex: 29,
            separator: " ",
            pages: [["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"], ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"], ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0", ";", "+", "-", "*", "/", "=", "@", "#", "$", "%", "(", ")", "?", ",", ":", "."],["\\"]],
            firstPage: 1,
            disableKey: "",
            isAppend: true,
            focusIndex: 0,
            focusNumberCount: 10,
            isShowTip: true
        },
        readConfig: function() {
            this._super()
        },
        setConfigObject: function(t, e) {
            try {
                if (t == "pages") {
                    var i = e.split(this.options.separator);
                    this.options.pages = [];
                    for (var o = 0; o < i.length; o++) {
                        this.options.pages.push(i[o].split(""))
                    }
                }
            } catch(n) {
                Config.log(n)
            }
        },
        initListener: function() {
            try {
                var t = this
            } catch(e) {
                Config.log(e)
            }
        },
        removeListener: function() {
            try {
                this.element.off().unbind()
            } catch(t) {
                Config.log(t)
            }
        },
        updateState: function() {
            try {
                var t = this.buttonArray.length;
                var e = 0;
                if (this.options.isEnabled) {
                    this.showCurrentPage();
                    if (this.enterButton != null) this.enterButton.enable();
                    if (this.clearButton != null) this.clearButton.enable();
                    if (this.changeButton != null) this.changeButton.enable(); 
					if (this.spaceButton != null) this.spaceButton.enable();
                    if (this.exitButton != null) this.exitButton.enable()
                } else {
                    for (e = 0; e < t; e++) {
                        this.buttonArray[e].disable()
                    }
                }
            } catch(i) {
                Config.log(i)
            }
        },
        test: function() {
            try {
                var e = this;
                t(window).keyup(function(t) {
                    t.preventDefault();
                    t.stopPropagation();
                    if (t.keyCode == 13) {
                        e.setFocus()
                    } else {
                        if (48 <= t.keyCode && t.keyCode <= 57) {
                            e.keyAction(t.keyCode - 48)
                        }
                    }
                    console.log(t.keyCode)
                })
            } catch(i) {
                Config.log(i)
            }
        },
        enableButton: function(t, e) {
            try {
                if (e) {
                    t.removeAttr("disabled").removeClass("disabled")
                } else {
                    t.attr("disabled", "disabled").addClass("disabled")
                }
            } catch(i) {
                Config.log(i)
            }
        },
        generateButtons: function() {
            try {
                var e = t("<ul></ul>").prependTo(this.element);
                var i;
                var o;
                var n = 0;
                var s = this;
                var r = "<li></li>";
                var a = "<li class='no-right-margin'></li>";
                for (var l = 0; l < this.options.row; l++) {
                    for (var h = 0; h < this.options.column; h++) {
                        if (h < this.options.column -1) i = t(r);
                        else i = t(a);
                        i = i.appendTo(e).append('<button type="button" class="button-keyboard" id="keyboard-button-' + (n + 1) + '"><span class="tip"></span><span class="button-text"></span></button>');
                        o = i.find("button").Button({
                            uiInstance: this.options.uiInstance,
                            afterclick: function(t, e) {
                                try {
                                    s.clickButtonByIndex(parseInt(e.index))
                                } catch(i) {
                                    Config.log(i)
                                }
                            }
                        }).Button("instance");
                        this.buttonArray.push(o);
                        n++;
                        o.options.index = n;
                        o.options.eventData = {
                            index: n
                        }
                    }
                }
                if (this.options.firstPage < 1) {
                    this.options.firstPage = 1
                } else if (this.options.firstPage > this.options.pages.length) {
                    this.options.firstPage = 1
                }
                this.pageIndex = this.options.firstPage;
                this.showCurrentPage()
            } catch(c) {
                Config.log(c)
            }
        },
        showCurrentPage: function() {
            try {
                if (this.pageIndex > this.options.pages.length) {
                    this.pageIndex = 1
                }
                if (this.pageIndex < 1) {
                    this.pageIndex = 1
                }
                var t = [];
                var e = this.buttonArray.length;
                var i;
                var o = this.options.pages[this.pageIndex - 1];
                var n = 0;
                var s = 0;
                for (var r = 0; r < e; r++) {
                    if (r >= o.length) {
                        i = false
                    } else {
                        if (this.isDisableKey(o[r])) {
                            i = false
                        } else {
                            i = true
                        }
                    }
                    this.buttonArray[r].isSpecial = true;
                    if (this.options.useEnterKey && this.options.enterKeyIndex == r + 1) {
                        i = this.buttonArray[r].options.isEnabled;
                        this.enterButton = this.buttonArray[r];
                        s++;
                        this.buttonArray[r].options.isAutoSend = true
                    } else if (this.options.useClearKey && this.options.clearKeyIndex == r + 1) {
                        i = this.buttonArray[r].options.isEnabled;
                        this.clearButton = this.buttonArray[r];
                        s++;
                        this.buttonArray[r].options.isAutoSend = false
                    } else if (this.options.useChangeKey && this.options.changeKeyIndex == r + 1) {
                        i = this.buttonArray[r].options.isEnabled;
                        this.changeButton = this.buttonArray[r];
                        s++;
                        this.buttonArray[r].options.isAutoSend = false
                    }else if (this.options.useSpaceKey && this.options.spaceKeyIndex == r + 1) {
                        i = this.buttonArray[r].options.isEnabled;
                        this.spaceButton = this.buttonArray[r];
                        s++;
                        this.buttonArray[r].options.isAutoSend = false
                    } else if (this.options.useExitKey && this.options.exitKeyIndex == r + 1) {
                        i = this.buttonArray[r].options.isEnabled;
                        this.exitButton = this.buttonArray[r];
                        s++;
                        this.buttonArray[r].options.isAutoSend = true
                    } else {
                        this.buttonArray[r].isSpecial = false;
                        this.buttonArray[r].options.isAutoSend = false;
                        if (r < o.length) {
                            this.buttonArray[r].setValue(o[r])
                        } else {
                            this.buttonArray[r].setValue("")
                        }
                        if (!i) n++
                    }
                    if (i) {
                        this.buttonArray[r].enable()
                    } else {
                        this.buttonArray[r].disable()
                    }
                }
                if (!this.hasInitButton) {
                    this.hasInitButton = true;
                    this.initSpecialButton()
                }
                if (n >= e - s) {
                    this.pageIndex++;
                    this.showCurrentPage()
                }
            } catch(a) {
                Config.log(a)
            }
        },
        isDisableKey: function(t) {
            try {
                return this.options.disableKey.indexOf(t) != -1
            } catch(e) {
                Config.log(e)
            }
        },
        initSpecialButton: function() {
            try {
                if (this.enterButton != null) {
                    this.enterButton.setValue(Config.ComponentConfigs["enterKey"]);
                    this.enterButton.options.dataToSend = "KEYBOARD_ENTER"
                }
                if (this.clearButton != null) {
                    this.clearButton.setValue(Config.ComponentConfigs["clearKey"])
                }
                if (this.changeButton != null) {
                    this.changeButton.setValue(Config.ComponentConfigs["changeKey"])
                }
				if (this.spaceButton != null) {
                    this.spaceButton.setValue(Config.ComponentConfigs["spaceKey"])
                }
                if (this.exitButton != null) {
                    this.exitButton.setValue(Config.ComponentConfigs["exitKey"]);
                    this.exitButton.options.dataToSend = "FUN_QUIT"
                }
            } catch(t) {
                Config.log(t)
            }
        },
		
        clickButtonByIndex: function(t) {
            try {
                if (t > this.buttonArray.length) return;
                var e = this.buttonArray[t - 1];
                if (!e.isEnabled() || e.options.isAutoSend) {
                    return
                }
                if (e.isSpecial) {
                    if (e == this.enterButton) {} else if (e == this.clearButton) {
                        this.clearInputText()
                    } else if (e == this.spaceButton) {
						var fun2 = document.getElementById("FUN_1");
						if(LoginRoleFlag){
							document.getElementById("FUN_2").click();
							LoginRoleFlag = false;
						}else{
							document.getElementById("FUN_1").click();
							LoginRoleFlag = true;
						}
                        this.clearInputText();
                    } else if (e == this.changeButton) {
                        this.changeKeyboardPage();
					} else if (e == this.exitButton) { }
                } else {
                    this.inputText(e.getValue())
                }
            } catch(i) {
                Config.log(i)
            }
        },
        changeKeyboardPage: function() {
            try {
                this.pageIndex++;
                this.showCurrentPage()
            } catch(t) {
                Config.log(t)
            }
        },
        clearInputText: function() {
            try {
                if (Config.INPUT_TARGET != null) {
                    Config.INPUT_TARGET.clear();
                    Config.INPUT_TARGET.getFocus()
                }
            } catch(t) {
                Config.log(t)
            }
        },
        inputText: function(t) {
            try {
                if (Config.INPUT_TARGET != null) {
                    Config.INPUT_TARGET.input(t);
                    Config.INPUT_TARGET.getFocus()
                }
            } catch(e) {
                Config.log(e)
            }
        },
        setButtonValue: function(t, e, i) {
            var o = true;
            try {
                if (t <= this.buttonArray.length) {
                    o = this.buttonArray[t - 1].setValue(e, i)
                } else {
                    o = false
                }
            } catch(n) {
                o = false;
                Config.log(n)
            }
            return o
        },
        select: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        deselect: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        disable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        enable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        clearFocus: function() {
            try {
                this.options.hasFocus = false;
                var t = this.buttonArray.length;
                for (var e = 0; e < t; e++) {
                    this.buttonArray[e].options.key = "";
                    this.buttonArray[e].clearFocus()
                }
            } catch(i) {
                Config.log(i);
                return false
            }
            return true
        },
        setFocus: function() {
            try {
                this.clearFocus();
                this.options.focusIndex++;
                if (this.options.focusIndex <= 0) this.options.focusIndex = 1;
                else if ((this.options.focusIndex - 1) * this.options.focusNumberCount >= this.buttonArray.length) {
                    this.options.focusIndex = 1
                }
                this._super();
                var t = this.buttonArray.length;
                var e = (this.options.focusIndex - 1) * this.options.focusNumberCount;
                var i = e + this.options.focusNumberCount;
                var o;
                for (var n = e,
                s = 1; n < i; n++, s++) {
                    if (n >= t) break;
                    else {
                        o = s;
                        if (o == 10) o = 0;
                        this.buttonArray[n].options.key = o + "";
                        this.buttonArray[n].setFocus()
                    }
                }
                return true
            } catch(r) {
                Config.log(r);
                return false
            }
            return true
        },
        resetFocus: function() {
            try {
                this.clearFocus();
                this.options.focusIndex = 0
            } catch(t) {
                Config.log(t);
                return false
            }
            return true
        },
        keyAction: function(t) {
            try {
                if (t == "CLEAR") {
                    this.clearInputText();
                    return true
                }
                var e = parseInt(t);
                if (e == 0) e = 10;
                var i = (this.options.focusIndex - 1) * this.options.focusNumberCount + e;
                if (i <= this.buttonArray.length) {
                    this.buttonArray[i - 1].keyAction(t);
                    return true
                } else {
                    return false
                }
            } catch(o) {
                Config.log(o)
            }
            return false
        },
        _create: function() {
            try {
                this.readConfig();
                this.init();
                this.initListener();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        init: function() {
            try {
                if (this.options.isShowTip && Config.showKeyTip()) t("#keyboard-tip").show();
                else t("#keyboard-tip").hide();
                this.generateButtons()
            } catch(e) {
                Config.log(e)
            }
        },
        destroy: function() {
            try {
                this.removeListener();
                t.Widget.prototype.destroy.call(this);
                this.element.remove()
            } catch(e) {
                Config.log(e)
            }
        }
    })
})(jQuery); 
(function(t) {
    t.widget("grgbanking.KeyboardNumber", t.grgbanking.Keyboard, {
        options: {
            type: Config.TYPE_KEYBOARD_NUMBER,
            row: 4,
            column: 3,
            useEnterKey: true,
            useExitKey: false,
            useClearKey: true,
            useChangeKey: false,
            enterKeyIndex: 12,
            exitKeyIndex: 17,
            clearKeyIndex: 11,
            changeKeyIndex: 29,
            pages: [["1", "2", "3", "4", "5", "6", "7", "8", "9", "0"]]
        }
    })
})(jQuery); 
(function(t) {
    t.widget("grgbanking.KeyboardNumberLogin", t.grgbanking.Keyboard, {
        options: {
            type: Config.TYPE_KEYBOARD_NUMBER_LOGIN,
            row: 4,
            column: 3,
            useEnterKey: true,
            useExitKey: false,
            useClearKey: true,
            useChangeKey: true,
            useSpaceKey: true,
            enterKeyIndex: 12,
            exitKeyIndex: 17,
            clearKeyIndex: 11,
            changeKeyIndex: 29,
            spaceKeyIndex: 30,
            pages: [["1", "2", "3", "4", "5", "6", "7", "8", "9", "0"]]
        }
    })
})(jQuery); 
(function(t) {
    t.widget("grgbanking.Table", t.grgbanking.Component, {
        prevButton: null,
        nextButton: null,
        hasPaging: true,
        elements: [],
        options: {
            type: Config.TYPE_TABLE,
            id: "",
            isComponentHide: true,
            isFixedRow: false,
            predefinedRowCount: Config.NOT_INIT,
            maxRowCount: 6,
            rowCount: 8,
            rowIndex: 1,
            rowIncrement: 1
        },
        readConfig: function() {
            try {
                this._super();
                if (!this.hasInitProperty(this.options.predefinedRowCount)) {
                    var e = this.element.find("tbody tr").length;
                    this.options.predefinedRowCount = e
                }
                var i = this;
                this.element.find("tbody tr").each(function(e) {
                    t(this).find("td").each(function(i) {
                        t(this).attr({
                            row: e + 1,
                            column: i + 1
                        })
                    })
                })
            } catch(o) {
                Config.log(o)
            }
        },
        initListener: function() {
            try {
                if (this.hasPaging) {
                    this.prevButton.options.eventData = {
                        self: this
                    };
                    this.nextButton.options.eventData = {
                        self: this
                    };
                    this.prevButton.options.afterclick = this.prevPage;
                    this.nextButton.options.afterclick = this.nextPage
                }
            } catch(t) {
                Config.log(t)
            }
        },
        removeListener: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        updateState: function() {
            try {
                if (this.options.rowCount > this.options.maxRowCount) {} else {}
            } catch(t) {
                Config.log(t)
            }
        },
        test: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        prevPage: function(t, e) {
            try {
                var i = e.self;
                i.options.rowIndex -= i.options.rowIncrement;
                if (i.options.rowIndex < 1) {
                    i.options.rowIndex = 1
                }
                i.showCurrentPage()
            } catch(t) {
                Config.log(t)
            }
        },
        nextPage: function(t, e) {
            try {
                var i = e.self;
                i.options.rowIndex += i.options.rowIncrement;
                var o = i.options.rowCount - i.options.maxRowCount + 1;
                if (o < 1) o = 1;
                i.options.rowIndex = Math.min(i.options.rowIndex, o);
                i.showCurrentPage()
            } catch(t) {
                Config.log(t)
            }
        },
        select: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        deselect: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        disable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        enable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        setText: function(t) {
            var e = true;
            try {
                this._super(t)
            } catch(i) {
                Config.log(i);
                e = false
            }
            return e
        },
        showCurrentPage: function() {
            try {
                var e = this;
                var i = e.options.rowIndex - 1;
                var o = e.options.rowIndex + e.options.maxRowCount - 2;
                if (this.options.rowCount > this.options.maxRowCount) {
                    if (this.hasPaging) {
                        this.element.find(".type-table-paging").removeClass("none");
                        if (this.options.rowIndex <= 1) {
                            this.prevButton.disable()
                        } else {
                            this.prevButton.enable()
                        }
                        if (this.options.rowIndex + this.options.maxRowCount - 1 < this.options.rowCount) {
                            this.nextButton.enable()
                        } else {
                            this.nextButton.disable()
                        }
                    }
                    this.element.find("tbody tr").removeClass("even odd last-visible-row").each(function(e) {
                        if (e < i || e > o) {
                            t(this).addClass("none")
                        } else {
                            t(this).removeClass("none"); (e - i) % 2 == 0 ? t(this).addClass("odd") : t(this).addClass("even");
                            if (e == o) t(this).addClass("last-visible-row")
                        }
                    })
                } else {
                    if (this.hasPaging) {
                        this.element.find(".type-table-paging").addClass("none");
                        this.prevButton.disable();
                        this.nextButton.disable()
                    }
                    if (this.options.isFixedRow) {
                        this.element.find("tbody tr").removeClass("even odd last-visible-row").each(function(e) {
                            if (e < i || e > o) {
                                t(this).addClass("none")
                            } else {
                                t(this).removeClass("none"); (e - i) % 2 == 0 ? t(this).addClass("odd") : t(this).addClass("even");
                                if (e == o) t(this).addClass("last-visible-row")
                            }
                        })
                    } else {
                        o = e.options.rowIndex + e.options.rowCount - 2;
                        this.element.find("tbody tr").removeClass("even odd last-visible-row").each(function(e) {
                            if (e < i || e > o) {
                                t(this).addClass("none")
                            } else {
                                t(this).removeClass("none"); (e - i) % 2 == 0 ? t(this).addClass("odd") : t(this).addClass("even");
                                if (e == o) t(this).addClass("last-visible-row")
                            }
                        })
                    }
                }
            } catch(n) {
                Config.log(n)
            }
        },
        resetRow: function(t) {
            try {
                var e = this.elements.length;
                for (var i = 0; i < e; i++) {
                    if (this.elements[i].options.row == t) {
                        if (!this.elements[i].isVisible()) this.elements[i].show()
                    }
                }
            } catch(o) {
                Config.log(o)
            }
        },
        generateComponent: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        init: function() {
            try {
                this.hasPaging = this.element.find(".type-table-paging").length > 0;
                if (this.hasPaging) {
                    this.prevButton = this.element.find(".type-table-paging button[mark=prev]").Button({
                        isAutoSend: false
                    }).Button("instance");
                    this.nextButton = this.element.find(".type-table-paging button[mark=next]").Button({
                        isAutoSend: false
                    }).Button("instance")
                }
                this.showCurrentPage();
                this.generateComponent()
            } catch(t) {
                Config.log(t)
            }
        },
        _create: function() {
            try {
                this.readConfig();
                this.init();
                this.initListener();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        destroy: function() {
            try {
                this.removeListener();
                t.Widget.prototype.destroy.call(this);
                this.element.remove()
            } catch(e) {
                Config.log(e)
            }
        }
    })
})(jQuery);




(function(t) {
    t.widget("grgbanking.Audio", t.grgbanking.Component, {
        options: {
            type: "Audio",
            id: ""
        },
        readConfig: function() {
            this._super()
        },
        initListener: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        removeListener: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        updateState: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        test: function() {
            try {} catch(t) {
                Config.log(t)
            }
        },
        select: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        deselect: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        disable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        enable: function() {
            try {
                this._super();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        setText: function(t) {
            var e = true;
            try {
                this._super(t);
                this.element.attr("src", t)
            } catch(i) {
                Config.log(i);
                e = false
            }
            return e
        },
		// 获取当前播放时间
		getCurrentTime: function() {
            var e = 0;
            try {
                e= parseInt(this.element[0].currentTime)
            } catch(i) {
                Config.log(i);
                e = 0
            }
            return e
        },
		// 播放
		play: function() {
            var e = true;
            try {
                this.element[0].play();
            } catch(i) {
                Config.log(i);
                e = false;
            }
            return e
        },
		// 暂停
		pause: function() {
            var e = true;
            try {
                this.element[0].pause();
            } catch(i) {
                Config.log(i);
                e = false;
            }
            return e
        },
		// 静音
		muted: function() {
            var e = true;
            try {
                this.element[0].muted= true;
            } catch(i) {
                Config.log(i);
                e = false;
            }
            return e
        },
		// 不静音
		unMuted: function() {
            var e = true;
            try {
                this.element[0].muted= false;
            } catch(i) {
                Config.log(i);
                e = false;
            }
            return e
        },
		// 增加声音，每次加0.1
		addVolume: function(t) {
            var e = true;
            try {
                var volume = this.element[0].volume+t;
				if(volume <=0 ){
				volume = 0 ;
				}
				
				if(volume >=1 ){
				volume = 1;
				}
				this.element[0].volume = volume;
				
            } catch(i) {
                Config.log(i);
                e = false;
            }
            return e
        },
		
        _create: function() {
            try {
                this.readConfig();
                this.initListener();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
        destroy: function() {
            try {
                this.removeListener();
                t.Widget.prototype.destroy.call(this);
                this.element.remove()
            } catch(e) {
                Config.log(e)
            }
        }
    })
})(jQuery);

(function(t) {
    t.widget("grgbanking.BaseComponent", t.grgbanking.Component, {
        options: {
            type: "BaseComponent",
            id: ""
        },
        _create: function() {
            try {
                this.readConfig();
                this.initListener();
                this.updateState()
            } catch(t) {
                Config.log(t)
            }
        },
		
        destroy: function() {
            try {
                this.removeListener();
                t.Widget.prototype.destroy.call(this);
                this.element.remove()
            } catch(e) {
                Config.log(e)
            }
        }
    })
})(jQuery);