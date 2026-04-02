/******************** 
	作用:滚动文本框
	作者:蔡俊雄
	版本:V1.0
	时间:2015-02-06
********************/
(function ($) {
    $.widget("grgbanking.Textbox", $.grgbanking.Component, {

        //-------------------------------------------属性
        /** 文本框 */
        textbox: null,

        /** 文本框的当前点击行(格式:数字 从0开始,0表示第一个) */
        currentRow: 0,

        /** 用于保存要显示手型按钮的行数 */
        rowNumberArray: [],

        /** 用于保存手型按钮 */
        buttonArray: [],

        /** 会显示的手型按钮 */
        buttonShowArray: [],

        /** 手型按钮回收池 */
        pool: [],

        //-------------------------------------------配置
        //配置
        options: {
            /** 控件类型 */
            type: Config.TYPE_TEXTBOX,

            lineHeight: 25,//每一行的高度
            /**
             * 是否追加
             * <li><strong>true:</strong>追加</li>
             * <li><strong>false:</strong>不追加</li>
             */
            isAppend: false,

            /**
             * 文本框的焦点在哪个位置(格式:数字 默认是-1)
             * <li><strong>-1:</strong>没有焦点</li>
             * <li><strong>0:</strong>按钮</li>
             * <li><strong>1:</strong>文本区域</li>
             */
            focusIndex: -1
        },
        //-------------------------------------------函数
        //读取配置
        readConfig: function () {
            this._super();
        },
        //获取
        getTextbox: function () {
            try {
                if (this.textbox == null) {
                    this.textbox = this.element.find("textarea");
                }
                return this.textbox;
            } catch (e) {
                Config.log(e);
            }
        },
        //添加手型按钮
        addHandButton: function () {
            try {
                var t = this.getTextbox();
                var height = t.height();
                var row = parseInt(height / this.options.lineHeight) + 1;//判断有多少行
                var btnStr = '<button type="button" class="button-hand"><span class="tip"></span> <span class="button-text"></span></button>';
                var container = $(".textbox-content-wrapper");
                var btn;
                for (var i = 0; i < row; i++) {
                    //添加相应的按钮
                    //$(btnStr).appendTo(container).css("top", i * this.options.lineHeight);
                    //btn = $(btnStr).appendTo(container).css("top", i * this.options.lineHeight).addClass("none").attr("row", i);
                    btn = $(btnStr).appendTo(container).css("top", i * this.options.lineHeight).addClass("none");
                    this.buttonArray.push(btn);
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //初始化事件侦听器
        initListener: function () {
            try {
                var self = this;
                this.getTextbox().on('valuechange', function (e, previous) {
                    self.updateState();
                }).scroll(function () {
                    self.updateState();
                });
                this.element.find(".textbox-button-zone button").click(function () {
                    if (!self.canAction($(this)))
                        return;
                    var operationType = $(this).attr("mark").toUpperCase();
                    switch (operationType) {
                        case "TOP":
                            self.scrollTop();
                            break;
                        case "UP":
                            self.scrollUp();
                            break;
                        case "DOWN":
                            self.scrollDown();
                            break;
                        case "BOTTOM":
                            self.scrollBottom();
                            break;
                        default:
                            break;
                    }
                    self.updateState();
                });
                //手型按钮点击事件
                $.each(this.buttonArray, function () {
                    $(this).click(function () {
                        if (!self.canAction($(this)))
                            return;
                        self.currentRow = $(this).attr("row");
                        Config.send("FUN_9999");
                        //alert($(this).attr("row"));
                        //$(this).hide();
                    });
                });
            } catch (e) {
                Config.log(e);
            }
        },
        //移除控件时清除事件
        removeListener: function () {
            try {
                this.getTextbox().off();
                this.element.find(".textbox-button-zone button").off();
                $.each(this.buttonArray, function () {
                    $(this).off();
                });
            } catch (e) {
                Config.log(e);
            }
        },
        //滚动到顶部
        scrollTop: function () {
            try {
                this.getTextbox().get(0).scrollTop = 0;
            } catch (e) {
                Config.log(e);
            }
        },
        //向上滚动
        scrollUp: function () {
            try {
                var t = this.getTextbox().get(0);
                var row = parseInt(t.scrollTop / this.options.lineHeight);//获取当前行
                row = Math.max(0, --row);
                var rowHeight = row * this.options.lineHeight;
                t.scrollTop = rowHeight;
            } catch (e) {
                Config.log(e);
            }
        },
        //向下滚动
        scrollDown: function () {
            try {
                var t = this.getTextbox().get(0);
                var row = parseInt(t.scrollTop / this.options.lineHeight);//获取当前行
                ++row;
                var rowHeight = row * this.options.lineHeight;//下一行的高度
                rowHeight = Math.min(rowHeight, t.scrollHeight);
                t.scrollTop = rowHeight;
            } catch (e) {
                Config.log(e);
            }
        },
        //滚动到底部
        scrollBottom: function () {
            try {
                var t = this.getTextbox().get(0);
                t.scrollTop = t.scrollHeight;
            } catch (e) {
                Config.log(e);
            }
        },
        //更新滚动文本框的状态
        updateState: function () {
            try {
                var t = this.getTextbox().get(0);
                var height = this.getTextbox().height();
                var start = t.scrollTop;



                var end = t.scrollHeight;
                var isTop = start <= 0;//是否到顶
                var isBottom = (end - start - height) <= 0;//是否到底

                //先将位置置为指定的行
                if (!isBottom && (start % this.options.lineHeight != 0)) {
                    start = Math.floor(start / this.options.lineHeight) * this.options.lineHeight;
                    t.scrollTop = start;
                }
                
                //----------------设置滚动按钮是否启用
                var topButton = this.element.find(".textbox-button-zone button[mark=top]");
                var upButton = this.element.find(".textbox-button-zone button[mark=up]");
                var downButton = this.element.find(".textbox-button-zone button[mark=down]");
                var bottomButton = this.element.find(".textbox-button-zone button[mark=bottom]");
                if (this.options.isEnabled) {
                    this.getTextbox().removeAttr("disabled").removeClass("disabled");
                    //判断是否到了顶部
                    if (isTop) {
                        this.enableButton(topButton, false);
                        this.enableButton(upButton, false);
                    } else {
                        this.enableButton(topButton, true);
                        this.enableButton(upButton, true);
                    }
                    //判断是否到了底部
                    if (isBottom) {
                        this.enableButton(downButton, false);
                        this.enableButton(bottomButton, false);
                    } else {
                        this.enableButton(downButton, true);
                        this.enableButton(bottomButton, true);
                    }
                }
                else {
                    this.getTextbox().attr("disabled", "disabled").addClass("disabled");

                    this.enableButton(topButton, false);
                    this.enableButton(upButton, false);
                    this.enableButton(downButton, false);
                    this.enableButton(bottomButton, false);
                }

                //----------------设置手型按钮的显示

                this.setHandButton();//设置手型按钮
            } catch (e) {
                Config.log(e);
            }
        },
        //测试用
        test: function () {
            //alert(this.options.hasFocus);
            //Config.showKeyTip();
            //显示行号
            //$(".grg-panel-title").html(this.rowNumberArray.join(","));
            try {
                var length = 100;
                var str = "";
                for (var i = 0; i < length; i++) {
                    str += i + "钞票信息查询 冠字号信息 交易类型:取款" + "\n";
                }
                this.getTextbox().val(str);

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
        //设置手型按钮的显示
        setHandButton: function () {
            try {
                //判断是否有需要显示手型按钮的行
                var length = this.rowNumberArray.length;
                if (length <= 0) {
                    return;
                }
                var line; //行号
                var handButton;
                var i = 0;
                var index;//当前索引

                var t = this.getTextbox().get(0);
                var startRow = Math.floor(t.scrollTop / this.options.lineHeight);//获取第一行
                var totalRow = Math.ceil(this.getTextbox().height() / this.options.lineHeight);//获取总行数
                this.buttonShowArray.splice(0); //临时保存显示的按钮
                for (i = 0; i < length; i++) {
                    line = this.rowNumberArray[i];
                    if (line <= (startRow + totalRow - 1) && line >= startRow) {
                        if (i < this.buttonArray.length) {
                            index = line - startRow;
                            handButton = this.buttonArray[index]; //获取手型按钮	
                            handButton.attr("row", line);//当前行号
                            if (this.buttonShowArray.length >= (10 - 4)) { //超出可显示焦点
                                handButton.addClass("none");
                            }
                            else {
                                handButton.removeClass("none");
                                this.buttonShowArray.push(index);
                                handButton.attr("row", line);
                            }
                        }
                    }
                }

                //将其它按钮隐藏起来
                length = this.buttonArray.length;
                for (i = 0; i < length; i++) {
                    handButton = this.buttonArray[i]; //获取手型按钮	
                    //是否禁用手型按钮
                    this.enableButton(handButton, this.options.isEnabled);
                    if (this.buttonShowArray.indexOf(i) == -1) {
                        handButton.addClass("none");
                    } else {
                        handButton.removeClass("none");
                    }
                }
                //----------------------显示手型按钮的按键提示
                this.showHandKey();
            } catch (e) {
                Config.log(e);
            }
        },
        //显示手型按钮的按键提示
        showHandKey: function () {
            try {
                var handButton;
                var length = this.buttonShowArray.length;
                var key;
                if (this.options.hasFocus && Config.showKeyTip())
                {
                    for (i = 0; i < length; i++) {
                        key = 5 + i;
                        if (key == 10)
                            key = 0;
                        handButton = this.buttonArray[this.buttonShowArray[i]]; //获取手型按钮
                        handButton.find(".tip").html(key);
                    }
                } else
                {
                    for (i = 0; i < length; i++) {
                        handButton = this.buttonArray[this.buttonShowArray[i]]; //获取手型按钮
                        handButton.find(".tip").html("");
                    }
                }
            } catch (e) {
                Config.log(e);
            }
        },
        //保存行号
        saveRowNumber: function (numbers) {
            try {
                this.rowNumberArray = numbers.split(",");
                //将字符串转换为数字
                var length = this.rowNumberArray.length;
                for (var i = 0; i < length; i++) {
                    this.rowNumberArray[i] = parseInt(this.rowNumberArray[i]);
                }
                //alert(this.rowNumberArray.join(","));
            }
            catch (e) {
                Config.log(e);
            }
        },
        //-------------------------------------------重写控件方法
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
        //加载TXT文件
        loadTxt: function (filePath) {
            try {
                var htmlobj = $.ajax({ url: filePath, async: false });
                this.setText(htmlobj.responseText);
            } catch (e) {
                Config.log(e);
            }
        },
        //设置控件文本
        setText: function (value) {
            var result = true;
            try {
                if (value == undefined || value == "") {
                    this.options.text = "";
                    this.getTextbox().val(this.options.text);
                }
                    //如果第一个字符为十六进制的1B则后跟为外部文件绝对路径。
                else if (value.substr(0, 1) == Config.CMD_INVAL_SEPARATOR) {
                    //获取文件加载路径
                    var filePath; // = value.substring(1, value.length);
                    var sTxt = value.substr(1, value.length);

                    if (-1 != sTxt.indexOf(Config.CMD_INVAL_SEPARATOR)) {
                        //有显示行号
                        this.saveRowNumber(sTxt.substr(sTxt.indexOf(Config.CMD_INVAL_SEPARATOR) + 1, sTxt.length));
                        filePath = sTxt.substr(0, sTxt.indexOf(Config.CMD_INVAL_SEPARATOR));
                    }
                    else {
                        filePath = sTxt.substr(0, sTxt.length);
                    }
                    //加载TXT文件
                    this.loadTxt(filePath);
                }
                else {
                    var sAppTxt;
                    if (-1 != value.indexOf(Config.CMD_INVAL_SEPARATOR)) {
                        this.saveRowNumber(value.substr(value.indexOf(Config.CMD_INVAL_SEPARATOR) + 1, value.length));
                        sAppTxt = value.substr(0, value.indexOf(Config.CMD_INVAL_SEPARATOR))
                    }
                    else {
                        sAppTxt = value;
                    }

                    //清除\r
                    var aValue = sAppTxt.split("\r");
                    var sValue = "";
                    for (var i = 0; i < aValue.length; i++) {
                        sValue += aValue[i];
                    }
                    value = sValue;

                    if (this.options.isAppend) {
                        this.options.text += "\n" + value;
                        this.getTextbox().val(this.options.text);
                        this.scrollBottom(); //滚动到底部
                        this.updateState();//更新状态
                    }
                    else {
                        this.options.text = value;
                        this.getTextbox().val(this.options.text);
                        //滚动到顶部
                        this.scrollTop(); //滚动到底部
                        this.updateState();//更新状态
                    }
                }
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
                this.element.find(".textbox-button-zone button .tip").html("");
                this.showHandKey();
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
                if (this.options.hasFocus && Config.showKeyTip()) {
                    //设置按钮焦点
                    this.element.find(".textbox-button-zone button").each(function (index) {
                        $(this).find(".tip").html(index + 1);
                    });
                    this.showHandKey();
                }
            } catch (e) {
                Config.log(e);
                return false;
            }
            return true;
        },
        //响应键盘输入
        keyAction: function (keyNum) {
            try
            {
                var key = parseInt(keyNum);
                if (isNaN(key))
                    return false;
				
                if (key == 0)
                    key = 10;
                if (key >= 1 && key <= 4)
                {
                    this.element.find(".textbox-button-zone button:eq(" + (key - 1) + ")").trigger("click");
                }
                else if (key >= 5 && key <= 10)
                {
                    key -= 5;
                    this.buttonArray[this.buttonShowArray[key]].trigger("click");
                }
                return true;
            }
            catch (e)
            {
                Config.log(e);
            }
            return false;
        },
        //-------------------------------------------控件创建和销毁
        //创建控件
        _create: function () {
            try {
                this.readConfig(); //读取配置
                this.addHandButton();//添加手型按钮
                this.initListener();//初始化事件侦听器
                this.updateState();//更新滚动文本框的状态
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
    $(".textbox-panel").Textbox();
    //$(".textbox-panel").Textbox("test");
    //$(".textbox-panel").Textbox("disable");
    //$(".textbox-panel").Textbox("enable");
    //$(".textbox-panel").Textbox("hide");
    //$(".textbox-panel").Textbox("show");
    
    //File20150209083846812.txt
    //$(".textbox-panel").Textbox("setValue","ha ha","");
    //$(".textbox-panel").Textbox("setValue", Config.CMD_INVAL_SEPARATOR + "../File20150209083846812.txt" + Config.CMD_INVAL_SEPARATOR + "0,3,6,9,12,15,18,21,24,27,30");
    //Config.TWIN_MONITOR = "INTANG";
    //$(".textbox-panel").Textbox("setFocus");
    //$(".textbox-panel").Textbox("keyAction", 4);
    //$(".textbox-panel").Textbox("keyAction", 9);

    //$(".textbox-panel").Textbox("clearFocus");
    //$(".textbox-panel").Textbox("destroy");
    //$(".textbox-panel").Textbox({ value: 20 });
    //alert("ha");
    //$("#maintenance-container").load("../totalmanage/totallist.html");//加载清机列表页
    //$("#grg-config").show().grgLabel();
    // $("#maintenance-container").show().grgLabel();
    /*$(".level-1 .nav-left").show();
    $(".level-1 .nav-zone").addClass("left-button-show");
    
    $(".level-1 .nav-right").show();
    $(".level-1 .nav-zone").addClass("right-button-show");*/
});