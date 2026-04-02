/******************** 
	作用:键盘
	作者:蔡俊雄
	版本:V1.0
	时间:2015-03-03
********************/
(function ($) {
    $.widget("grgbanking.Keyboard", $.grgbanking.Component, {

        //-------------------------------------------属性
        /** 保存所有键盘按钮 */
        buttonArray: [],

        /** 当前是第几页 */
        pageIndex: 1,

        /** 保存上次的状态 */
        lastStates: [],

        //-------------------特殊按键
        /** "确认"按钮 */
        enterButton: null,

        /** "更正"按钮 */
        clearButton: null,

        /** "切换"按钮 */
        changeButton: null,
		
		/** "空白"按钮 */
        spaceButton: null,

        /** "退出"按钮 */
        exitButton: null,

        /** 特殊按钮是否已经初始化(true:已经初始化 false:未初始化) */
        hasInitButton: false,

        //-------------------------------------------配置
        //配置
        options: {
            /** 控件类型 */
            type: Config.TYPE_KEYBOARD,

            /** id */
            id: "",

            /** 键盘按钮行数(格式:数字 默认是6) */
            row: 6,

            /** 键盘按钮列数(格式:数字 默认是5) */
            column: 5,

            //-----------------------------------------------特殊功能键(START)
            /**
             * 键盘是否使用确定键(默认是true)
             * <li><strong>true:</strong>使用确定键</li>
             * <li><strong>false:</strong>不使用确定键</li>
             */
            useEnterKey: true,

            /**
             * 键盘是否使用退出键(默认是false)
             * <li><strong>true:</strong>使用退出键</li>
             * <li><strong>false:</strong>不使用退出键</li>
             */
            useExitKey: false,

            /**
             * 键盘是否使用更正键(默认是true)
             * <li><strong>true:</strong>使用更正键</li>
             * <li><strong>false:</strong>不使用更正键</li>
             */
            useClearKey: true,

            /**
             * 键盘是否使用切换键(默认是true)
             * <li><strong>true:</strong>使用切换键</li>
             * <li><strong>false:</strong>不使用切换键</li>
             */
            useChangeKey: true,
			
			useSpaceKey: true,

            /** 确定键的位置(格式:数字 默认是30,表示30个键盘按钮将会是确定键) */
            enterKeyIndex: 30,

            /** 退出键的位置(格式:数字 默认是17,表示17个键盘按钮将会是退出键) */
            exitKeyIndex: 17,

            /** 更正键的位置(格式:数字 默认是28,表示28个键盘按钮将会是更正键) */
            clearKeyIndex: 28,

            /** 切换键的位置(格式:数字 默认是29,表示29个键盘按钮将会是切换键) */
            changeKeyIndex: 29,

            //-----------------------------------------------特殊功能键(END)

            /** 分隔字符 */
            separator: " ",

            /** 键盘每页的按键值 */
            pages: [
                ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"],
                ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"],
                ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0", ";", "+", "-", "*", "/", "=", "@", "#", "$", "%", "(", ")", "?", ",", ":", "."]
            ],

            /** 一开始时键盘显示第几页(格式:数字 默认是1) */
            firstPage: 1,

            /** 键盘上限制输入的按键值(格式:字符串 默认为空) */
            disableKey: "",

            /**
             * 是否追加
             * <li><strong>true:</strong>追加</li>
             * <li><strong>false:</strong>不追加</li>
             */
            isAppend: true,

            /**
             * 文本框的焦点在哪个位置(格式:数字 默认是0)
             * <li><strong>0:</strong>没有焦点</li>
             * <li><strong>1:</strong>1-9-0</li>
             * <li><strong>2:</strong>1-9-0</li>
             */
            focusIndex: 0,

            /** 当键盘获取到焦点时一次会使用多少个数字显示(格式:数字 默认是10) */
            focusNumberCount: 10,

            /**
             * 是否显示键盘提示文本(默认是true)
             * <li><strong>true:</strong>显示</li>
             * <li><strong>false:</strong>隐藏</li>
             */
            isShowTip: true

        },
        //-------------------------------------------函数
        //读取配置
        readConfig: function () {
            this._super();
            //保存页数
        },
        //设置对象类型的配置
        setConfigObject: function (name, attribute) {
            try {
                if (name == "pages") {
                    var tempArray = attribute.split(this.options.separator);
                    this.options.pages = [];
                    for (var i = 0; i < tempArray.length; i++) {
                        this.options.pages.push(tempArray[i].split(""));
                    }
                    //this.options.pages = attribute.split(this.options.separator)
                }
            } catch (e) {
                Config.log(e);
            } 
        },
        //初始化事件侦听器
        initListener: function () {
            try {
                var self = this;
                //$.each(this.buttonArray, function () {
                //console.log(this);
                //this.on('afterclick', function () {
                //this.on('afterclick', function () {
                //console.log("ha");
                ////self.updateState();
                //});

                //$(this).click(function () {
                //    if (!self.canAction($(this)))
                //        return;
                //    self.currentRow = $(this).attr("row");
                //    Config.send("FUN_9999");
                //    //alert($(this).attr("row"));
                //    //$(this).hide();
                //});
                //});
                //this.element.click(function () {
                //    if (!self.canAction($(this)))
                //        return;
                //    self._trigger("preclick", null, { id: self.options.id });
                //    self.setToggleState(); //如果是切换状态按钮则根据当前状态设置其它按钮的状态
                //    //判断是否向服务器发送消息
                //    if (self.options.isAutoSend) {
                //        //向服务器发送消息
                //        if (self.options.dataToSend == "")
                //            Config.send(self.options.id);
                //        else
                //            Config.send(self.options.dataToSend);
                //    }
                //    self._trigger("afterclick", null, { id: self.options.id });
                //});
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
                //this.enableButton(this.element, this.options.isEnabled);
                //设置选中状态
                //if (this.options.isSelected) {
                //    this.element.addClass("selected");
                //} else {
                //    this.element.removeClass("selected");
                //}
                var len = this.buttonArray.length;
                var i = 0;
                if (this.options.isEnabled) {
                    this.showCurrentPage();
                    //确认
                    if (this.enterButton != null)
                        this.enterButton.enable();
                    //更正
                    if (this.clearButton != null)
                        this.clearButton.enable();
                    //切换
                    if (this.changeButton != null)
                        this.changeButton.enable();
					//空白
                    if (this.spaceButton != null)
                        this.spaceButton.enable();
                    //退出
                    if (this.exitButton != null)
                        this.exitButton.enable();
                } else {
                    for (i = 0; i < len; i++) {
                        this.buttonArray[i].disable();
                    }
                }


            } catch (e) {
                Config.log(e);
            }
        },
        //测试用
        test: function () {
            try {
                var self = this;
                //按键监听
                $(window).keyup(function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    if (e.keyCode == 13) {
                        self.setFocus();
                    } else {
                        if (48 <= e.keyCode && e.keyCode <= 57) {
                            self.keyAction(e.keyCode - 48);
                        }
                    }
                    //self.keyAction(1);
                    //self.setFocus();
                    console.log(e.keyCode);
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
        //生成按钮
        generateButtons: function () {
            try {
                var parent = $("<ul></ul>").prependTo(this.element);
                var li;
                var btn;
                var index = 0;
                var self = this;
                var liStr = "<li></li>";
                var liNoSpace = "<li class='no-right-margin'></li>";
                for (var i = 0; i < this.options.row; i++) {
                    for (var j = 0; j < this.options.column; j++) {
                        if (j < this.options.column)
                            li = $(liStr);
                        elsesty
                            li = $(liNoSpace);
                        li = li.appendTo(parent).append('<button type="button" class="button-keyboard"><span class="tip" style="display:">testyixian</span><span class="button-text"></span></button>');
                        btn = li.find("button").Button({
                            afterclick: function (e, data) {
                                try {
                                    //console.log(data.index);
                                    self.clickButtonByIndex(parseInt(data.index));
                                } catch (err) {
                                    Config.log(err);
                                }
                            }
                        }).Button("instance");
                        this.buttonArray.push(btn);

                        index++;
                        btn.options.index = index;
                        btn.options.eventData = { index: index };
                    }
                }
                //判断页数
                if (this.options.firstPage < 1) {
                    this.options.firstPage = 1;
                }
                else if (this.options.firstPage > this.options.pages.length) {
                    this.options.firstPage = 1;
                }
                this.pageIndex = this.options.firstPage; //当前是第几页
                this.showCurrentPage(); //显示当前页
            } catch (e) {
                Config.log(e);
            }
        },
        //显示当前页
        showCurrentPage: function () {
            try {
                //判断当前页是否禁用
                if (this.pageIndex > this.options.pages.length) {
                    this.pageIndex = 1;
                }
                if (this.pageIndex < 1) {
                    this.pageIndex = 1;
                }
                //设置当前页面按钮的启用或禁用状态
                var stateArray = [];
                var len = this.buttonArray.length;
                var state; //按钮的状态
                var pageArray = this.options.pages[this.pageIndex - 1];
                var disableCount = 0; //计算界面上被禁用的普通键值按钮个数
                var specialCount = 0; //计算界面上特殊按钮个数
                for (var i = 0; i < len; i++) {
                    //this.buttonArray[i].index = i + 1;
                    //获取按键是否禁用
                    if (i >= pageArray.length) {
                        state = false;
                    }
                    else {
                        if (this.isDisableKey(pageArray[i])) {
                            state = false;
                        }
                        else {
                            state = true;
                        }
                    }
                    this.buttonArray[i].isSpecial = true;
                    if (this.options.useEnterKey && this.options.enterKeyIndex == (i + 1)) {
                        //确认
                        state = this.buttonArray[i].options.isEnabled; //判断是否被禁用
                        this.enterButton = this.buttonArray[i];
                        specialCount++; //计算界面上特殊按钮个数
                        this.buttonArray[i].options.isAutoSend = true;
                    }
                    else if (this.options.useClearKey && this.options.clearKeyIndex == (i + 1)) {
                        //更正
                        state = this.buttonArray[i].options.isEnabled; //判断是否被禁用
                        this.clearButton = this.buttonArray[i];
                        specialCount++; //计算界面上特殊按钮个数
                        this.buttonArray[i].options.isAutoSend = false;
                    }
                    else if (this.options.useChangeKey && this.options.changeKeyIndex == (i + 1)) {
                        //切换
                        state = this.buttonArray[i].options.isEnabled; //判断是否被禁用
                        this.changeButton = this.buttonArray[i];
                        specialCount++; //计算界面上特殊按钮个数
                        this.buttonArray[i].options.isAutoSend = false;
                    }
					else if (this.options.useSpaceKey && this.options.spaceKeyIndex == (i + 1)) {
                        //切换
                        state = this.buttonArray[i].options.isEnabled; //判断是否被禁用
                        this.spcaeButton = this.buttonArray[i];
                        specialCount++; //计算界面上特殊按钮个数
                        this.buttonArray[i].options.isAutoSend = false;
                    }
                    else if (this.options.useExitKey && this.options.exitKeyIndex == (i + 1)) {
                        //退出
                        state = this.buttonArray[i].options.isEnabled; //判断是否被禁用
                        this.exitButton = this.buttonArray[i];
                        specialCount++; //计算界面上特殊按钮个数
                        this.buttonArray[i].options.isAutoSend = true;
                    }
                    else {
                        this.buttonArray[i].isSpecial = false;
                        this.buttonArray[i].options.isAutoSend = false;
                        if (i < pageArray.length) {
                            this.buttonArray[i].setValue(pageArray[i]);
                        }
                        else {
                            this.buttonArray[i].setValue("");
                        }
                        if (!state)
                            disableCount++;
                        //if (!buttonArray[i].hasEventListener(MouseEvent.CLICK)) {

                        //}
                    }
                    //buttonArray[i].addEventListener(MouseEvent.CLICK, clickButton); //点击时清除当前界面上的文本框内容
                    //this.buttonArray[i].actionClickFun = clickButtonAction; //点击时清除当前界面上的文本框内容
                    if (state) {
                        this.buttonArray[i].enable();
                    }
                    else {
                        this.buttonArray[i].disable();
                    }
                }
                if (!this.hasInitButton) {
                    this.hasInitButton = true;
                    this.initSpecialButton(); //初始化特殊按钮
                }
                //判断是否需要切换到下一页
                if (disableCount >= (len - specialCount)) {
                    //当前页的普通键值按钮已经全部被禁用,开始切换到下一页
                    this.pageIndex++;
                    this.showCurrentPage(); //显示当前页
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //判断该键值是否被禁用(true:禁用 false:启用)
        isDisableKey: function (testKey) {
            try {
                return (this.options.disableKey.indexOf(testKey) != -1);
            } catch (e) {
                Config.log(e);
            }
        },
        //初始化特殊按钮
        initSpecialButton: function () {
            try {
                //console.log(Config.ComponentConfigs["enterKey"]);
                //console.log(this.enterButton);
                //确认
                if (this.enterButton != null) {
                    this.enterButton.setValue(Config.ComponentConfigs["enterKey"]);
                    this.enterButton.options.id = "KEYBOARD_ENTER";
                    //enterButton.addEventListener(MouseEvent.CLICK, clickEnter); //点击确认按钮
                    //enterButton.config.isAutoSend = true;
                }

                //更正
                if (this.clearButton != null) {
                    this.clearButton.setValue(Config.ComponentConfigs["clearKey"]);
                    //clearButton.addEventListener(MouseEvent.CLICK, clearInputText); //点击时清除当前界面上的文本框内容
                }
                //切换
                if (this.changeButton != null) {
                    this.changeButton.setValue(Config.ComponentConfigs["changeKey"]);
                    //changeButton.addEventListener(MouseEvent.CLICK, changeKeyboardPage); //点击时切换键盘显示页数
                }
				//空白
                if (this.spaceButton != null) {
                    this.spaceButton.setValue(Config.ComponentConfigs["spaceKey"]);
                    //spaceButton.addEventListener(MouseEvent.CLICK, spaceKeyboardPage); //点击时切换键盘显示页数
                }
                //退出
                if (this.exitButton != null) {
                    this.exitButton.setValue(Config.ComponentConfigs["exitKey"]);
                    this.exitButton.options.id = "FUN_QUIT";
                    //exitButton.addEventListener(MouseEvent.CLICK, exit); //点击时向服务器发送退出消息
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //点击按钮
        //clickButtonAction: function (e, data) {
        //    try {
        //        //console.log(data.index);

        //        this.clickButtonByIndex(parseInt(data.index));
        //    } catch (e) {

        //    }
        //},
        //根据位置点击按钮
        clickButtonByIndex: function (index) {
            try {
                if (index > this.buttonArray.length)
                    return;
                var btn = this.buttonArray[index - 1];
                if (!btn.isEnabled() || btn.options.isAutoSend) {
                    return;
                }

                if (btn.isSpecial) {
                    //判断是哪一种按钮
                    if (btn == this.enterButton) {
                        //this.clickEnter();
                    }
                    else if (btn == this.clearButton) {
                        this.clearInputText();
                    }
                    else if (btn == this.changeButton) {
                        this.changeKeyboardPage();
                    }
					else if (btn == this.spaceButton) {
                        this.spaceKeyboardPage();
                    }
                    else if (btn == this.exitButton) {
                        //this.exit();
                    }
                }
                else {
                    this.inputText(btn.getValue());
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //点击时切换键盘显示页数
        changeKeyboardPage: function () {
            try {
                //if (this.changeButton != null && this.changeButton.isEnabled() && this.changeButton.isVisible())
                //{
                this.pageIndex++;
                this.showCurrentPage(); //显示当前页
                //}
            } catch (e) {
                Config.log(e);
            }
        },
		//点击时清除当前文本输入框中的内容
        spaceKeyboardPage: function () {
            try {
                if (Config.INPUT_TARGET != null) {
                    Config.INPUT_TARGET.clear();
                    Config.INPUT_TARGET.getFocus();
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //清除当前文本输入框中的内容
        clearInputText: function () {
            try {
                if (Config.INPUT_TARGET != null) {
                    Config.INPUT_TARGET.clear();
                    Config.INPUT_TARGET.getFocus();
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //向当前文本输入框输入内容
        inputText: function (value) {
            try {
                //console.log(Config.INPUT_TARGET);
                if (Config.INPUT_TARGET != null) {
                    Config.INPUT_TARGET.input(value);
                    Config.INPUT_TARGET.getFocus();
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //设置按钮的值
        setButtonValue: function (buttonIndex, value, attribute) {
            var result = true;
            try {
                if (buttonIndex <= this.buttonArray.length) {
                    result = this.buttonArray[buttonIndex - 1].setValue(value, attribute);
                }
                else {
                    result = false;
                }
            } catch (e) {
                result = false;
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
        //清除键盘焦点
        clearFocus: function () {
            try {
                this.options.hasFocus = false;
                //清除所有键盘按钮的键盘提示
                var len = this.buttonArray.length;
                for (var i = 0; i < len; i++) {
                    this.buttonArray[i].options.key = "";
                    this.buttonArray[i].clearFocus();
                }
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
                this.options.focusIndex++;
                if (this.options.focusIndex <= 0)
                    this.options.focusIndex = 1;
                else if ((this.options.focusIndex - 1) * this.options.focusNumberCount >= this.buttonArray.length) {
                    //超出范围,跳到下一个控件
                    //this.options.focusIndex = 0;
                    //return false;
                    this.options.focusIndex = 1;
                    //return false;
                }
                this._super();


                var len = this.buttonArray.length;
                var startIndex = (this.options.focusIndex - 1) * this.options.focusNumberCount;
                var endIndex = startIndex + this.options.focusNumberCount;
                var key; //显示的按键
                for (var i = startIndex, j = 1; i < endIndex; i++, j++) {
                    //判断是否超出范围
                    if (i >= len)
                        break;
                    else {
                        key = j;
                        if (key == 10)
                            key = 0;
                        this.buttonArray[i].options.key = key + "";
                        this.buttonArray[i].setFocus();
                    }
                }
                return true;
                //this.showKey();
            } catch (e) {
                Config.log(e);
                return false;
            }
            return true;
        },
        //重新设置焦点索引所在的位置,使其从头开始
        resetFocus: function () {
            try {
                this.clearFocus();
                this.options.focusIndex = 0;
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
                //this.element.trigger("click");
                if (keyNum == "CLEAR") {
                    this.clearInputText();
                    return true;
                }
                var key = parseInt(keyNum); //显示的按键
                if (key == 0)
                    key = 10;
                var buttonIndex = (this.options.focusIndex - 1) * this.options.focusNumberCount + key;
                if (buttonIndex <= this.buttonArray.length) {
                    this.buttonArray[buttonIndex - 1].keyAction(keyNum);
                    return true;
                }
                else {
                    return false;
                }
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
                this.init();//初始化
                this.initListener();//初始化事件侦听器
                this.updateState();//更新状态
            } catch (e) {
                Config.log(e);
            }
        },
        //初始化
        init: function () {
            try {
                //显示或隐藏提示信息
                if (this.options.isShowTip && Config.showKeyTip())
                    $("#keyboard-tip").show();
                else
                    $("#keyboard-tip").hide();
                this.generateButtons();//生成按钮
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

/******************** 
	作用:图片
	作者:蔡俊雄
	版本:V1.0
	时间:2015-03-02
********************/
(function ($) {
    $.widget("grgbanking.KeyboardNumber", $.grgbanking.Keyboard, {
        //配置
        options: {
            /** 控件类型 */
            type: Config.TYPE_KEYBOARD_NUMBER,

            /** 键盘按钮行数(格式:数字 默认是6) */
            row: 4,

            /** 键盘按钮列数(格式:数字 默认是5) */
            column: 3,

            //-----------------------------------------------特殊功能键(START)
            /**
             * 键盘是否使用确定键(默认是true)
             * <li><strong>true:</strong>使用确定键</li>
             * <li><strong>false:</strong>不使用确定键</li>
             */
            useEnterKey: true,

            /**
             * 键盘是否使用退出键(默认是false)
             * <li><strong>true:</strong>使用退出键</li>
             * <li><strong>false:</strong>不使用退出键</li>
             */
            useExitKey: false,

            /**
             * 键盘是否使用更正键(默认是true)
             * <li><strong>true:</strong>使用更正键</li>
             * <li><strong>false:</strong>不使用更正键</li>
             */
            useClearKey: true,

            /**
             * 键盘是否使用切换键(默认是true)
             * <li><strong>true:</strong>使用切换键</li>
             * <li><strong>false:</strong>不使用切换键</li>
             */
            useChangeKey: false,

            /** 确定键的位置(格式:数字 默认是12,表示12个键盘按钮将会是确定键) */
            enterKeyIndex: 12,

            /** 退出键的位置(格式:数字 默认是17,表示17个键盘按钮将会是退出键) */
            exitKeyIndex: 17,

            /** 更正键的位置(格式:数字 默认是11,表示11个键盘按钮将会是更正键) */
            clearKeyIndex: 11,

            /** 切换键的位置(格式:数字 默认是29,表示29个键盘按钮将会是切换键) */
            changeKeyIndex: 29,

            //-----------------------------------------------特殊功能键(END)

            /** 键盘每页的按键值 */
            pages: [
                ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0"]
            ]
        },

    });
})(jQuery);

(function ($) {
    $.widget("grgbanking.KeyboardNumberLogin", $.grgbanking.Keyboard, {
        //配置
        options: {
            /** 控件类型 */
            type: Config.TYPE_KEYBOARD_NUMBER_LOGIN,

            /** 键盘按钮行数(格式:数字 默认是6) */
            row: 4,

            /** 键盘按钮列数(格式:数字 默认是5) */
            column: 3,

            //-----------------------------------------------特殊功能键(START)
            /**
             * 键盘是否使用确定键(默认是true)
             * <li><strong>true:</strong>使用确定键</li>
             * <li><strong>false:</strong>不使用确定键</li>
             */
            useEnterKey: true,

            /**
             * 键盘是否使用退出键(默认是false)
             * <li><strong>true:</strong>使用退出键</li>
             * <li><strong>false:</strong>不使用退出键</li>
             */
            useExitKey: false,

            /**
             * 键盘是否使用更正键(默认是true)
             * <li><strong>true:</strong>使用更正键</li>
             * <li><strong>false:</strong>不使用更正键</li>
             */
            useClearKey: true,

            /**
             * 键盘是否使用切换键(默认是true)
             * <li><strong>true:</strong>使用切换键</li>
             * <li><strong>false:</strong>不使用切换键</li>
             */
            useChangeKey: true,

            /** 确定键的位置(格式:数字 默认是12,表示12个键盘按钮将会是确定键) */
            enterKeyIndex: 12,

            /** 退出键的位置(格式:数字 默认是17,表示17个键盘按钮将会是退出键) */
            exitKeyIndex: 17,

            /** 更正键的位置(格式:数字 默认是11,表示11个键盘按钮将会是更正键) */
            clearKeyIndex: 11,

            /** 切换键的位置(格式:数字 默认是29,表示29个键盘按钮将会是切换键) */
            changeKeyIndex: 29,
			
			spaceKeyIndex: 30,

            //-----------------------------------------------特殊功能键(END)

            /** 键盘每页的按键值 */
            pages: [
                ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0"]
            ]
        },

    });
})(jQuery);

$(function () { //ID匹配元素
    //Config.TWIN_MONITOR = "INTANG";
    //$("#keyboard").Keyboard();
    //$("#keyboard").Keyboard("disable");
    //$("#keyboard").Keyboard("enable");
    //$("#keyboard").Keyboard("setButtonValue",1,"c","");

    //$("#keyboard").KeyboardNumber();

    //$("#keyboard").KeyboardNumber("disable");
    //$("#keyboard").KeyboardNumber("enable");
    //$("#keyboard").KeyboardNumber("setFocus");
    //$("#keyboard").KeyboardNumber("test");
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
    //$("#keyboard").Keyboard("setFocus");
    //$("#keyboard").Keyboard("test");
    //$("#keyboard").Keyboard("keyAction",1);
    //$("button[type=button]").Button("setFocus");
    ////$("button[type=button]").Button("clearFocus");
    //$("button[type=button]").Button("keyAction", 4);
    //$("button[type=button]").Button("keyAction", 9);


});