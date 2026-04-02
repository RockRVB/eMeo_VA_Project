/******************** 
	作用:提示框
	作者:蔡俊雄
	版本:V1.0
	时间:2015-03-01
********************/
(function ($) {
    $.widget("grgbanking.Promptbox", $.grgbanking.Component, {

        //-------------------------------------------属性
        /** 自定义格式与原来的值的分隔符 */
        DEFINE_SEPARATOR: "\x1DDEFINE_PROMPT\x1D",

        /** 自定义界面与原来的值的分隔符 */
        UI_SEPARATOR: "\x1DUI\x1D",

        /** 默认确认按钮文本 */
        defaultOk: "",

        /** 默认取消按钮文本 */
        defaultCancel: "",

        /** "确认"按钮 */
        sureButton: null,

        /** "取消"按钮 */
        cancelButton: null,

        /** 隐藏ID */
        hideId: null,

        //-------------------------------------------配置
        //配置
        options: {
            /** 控件类型 */
            type: Config.TYPE_PROMPT_BOX,

            /** id */
            id: "",

            /** 类型 */
            promptType: Config.UI_OK_CANCEL,

            /** 要显示的文本 */
            text: "",

            /** 显示的时间(秒) */
            time: 0
        },
        //默认配置
        defaults: {
            /** x坐标方向上的偏移量(格式:数字 默认是0) */
            deltaX: 0,

            /** y坐标方向上的偏移量(格式:数字 默认是0) */
            deltaY: 0,

            /** 提示框宽度(格式:数字 默认是400) */
            width: 400,

            /** 提示框高度(格式:数字 默认是224) */
            height: 224,

            /** 文本区域离左边距离(格式:数字 默认是10) */
            paddingLeft: 10,

            /** 文本区域离右边距离(格式:数字 默认是10) */
            paddingRight: 10,

            /** 文本区域离上边距离(格式:数字 默认是10) */
            paddingTop: 10,

            /** 文本区域离下边距离(格式:数字 默认是10) */
            paddingBottom: 10,

            /** 控制文本字段的自动大小调整和对齐(默认为none)(none:无 left:左对齐 center:中间对齐 right:右对齐) */
            //autoSize: AlignType.NONE,

            /** 水平对齐方式(默认为左对齐)(left:左对齐 center:中间对齐 right:右对齐) */
            hAlign: AlignType.CENTER,

            /** 垂直对齐方式(默认为顶对齐)(top:顶对齐 middle:中间对齐 bottom:下对齐) */
            //vAlign: AlignType.MIDDLE,

            /** 字体大小(格式:数字 默认是23) */
            size: 23,

            /** 是否粗体(默认为不是粗体)(true:粗体 false:不是粗体) */
            bold: false,

            /** 行间距(格式:数字 默认是0) */
            leading: 0,

            /** 字符间距(格式:数字 默认是0) */
            letterSpacing: 0,

            /** 首行缩进(格式:数字 默认是0) */
            indent: 0,

            /** 块缩进(格式:数字 默认是0) */
            blockIndent: 0,

            /** 文字颜色(格式:十六进制颜色 默认是0x474747) */
            color: "#474747",

            /** 字体名称 */
            font: Config.Font,

            /** 类型 */
            //promptType: Config.UI_OK_CANCEL,

            /** 确认按钮文本 */
            ok: "",

            /** 取消按钮文本 */
            cancel: "",

            /** 确认按钮发送的消息 */
            okData: "",

            /** 取消按钮发送的消息 */
            cancelData: "",

            /** 是否加载子界面(如不为空则加载子界面) */
            ui: "",

            /** 是否显示loading图标(true:显示loading图标 false:不显示loading) */
            isLoading: false

        },
        //-------------------------------------------函数
        //读取配置
        readConfig: function () {
            this.getDefaultConfig();
            this._super();
        },
        //获取默认配置
        getDefaultConfig: function () {
            var settings = $.extend(this.options, this.defaults);
            return settings;
        },
        //获取对话框配置
        getPromtConfig: function (value) {
            var cfg = this.getDefaultConfig();
            if (value == undefined || value == null)
                return cfg;
            try {
                //先清空掉OK和CANCEL的文本
                cfg.ok = "";
                cfg.cancel = "";
                cfg.okData = "";
                cfg.cancelData = "";

                var tempArray = value.split("#");
                var str = "";
                for (var i = 0; i < tempArray.length; i++) {
                    var valueArray = tempArray[i].split(":");
                    str = valueArray[0].toUpperCase();
                    switch (str) {
                        case "UI": //是否加载子界面(如不为空则加载子界面)
                            cfg.ui = valueArray[1].replace("@", ":");
                            break;
                        case "ISLOADING": //是否显示loading图标(true:显示loading图标 false:不显示loading)
                            cfg.isLoading = valueArray[1] == "false" ? false : true;
                            break;
                        case "WIDTH": //宽度
                            cfg.width = Number(valueArray[1]);
                            break;
                        case "HEIGHT": //高度
                            cfg.height = Number(valueArray[1]);
                            break;
                        case "DELTAX": //宽度
                            cfg.deltaX = Number(valueArray[1]);
                            break;
                        case "DELTAY": //高度
                            cfg.deltaY = Number(valueArray[1]);
                            break;
                        case "PADDING": //文字离背景边框的间距
                            try {
                                var paddingArray = valueArray[1].split(",");
                                cfg.paddingLeft = Number(paddingArray[0]);
                                cfg.paddingTop = Number(paddingArray[1]);
                                cfg.paddingRight = Number(paddingArray[2]);
                                cfg.paddingBottom = Number(paddingArray[3]);
                            }
                            catch (e2) {

                            }
                            break;
                        case "ALIGN": //对齐方式
                            cfg.hAlign = valueArray[1];
                            break;
                        case "SIZE": //字体大小
                            cfg.size = Number(valueArray[1]);
                            break;
                        case "BOLD": //是否粗体
                            cfg.bold = valueArray[1] == "false" ? false : true;
                            break;
                        case "LEADING": //行间距
                            cfg.leading = Number(valueArray[1]);
                            break;
                        case "LETTERSPACING": //字符间距
                            cfg.letterSpacing = Number(valueArray[1]);
                            break;
                        case "INDENT": //首行缩进
                            cfg.indent = Number(valueArray[1]);
                            break;
                        case "BLOCKINDENT": //块缩进
                            cfg.blockIndent = Number(valueArray[1]);
                            break;
                        case "COLOR": //字符颜色
                            cfg.color = valueArray[1];
                            break;
                        case "FONT": //字体
                            cfg.font = valueArray[1];
                            break;
                        case "OK": //确认按钮文本
                            cfg.ok = valueArray[1];
                            break;
                        case "CANCEL": //取消按钮文本
                            cfg.cancel = valueArray[1];
                            break;
                        case "OKDATA": //确认按钮发送的消息
                            cfg.okData = valueArray[1];
                            break;
                        case "CANCELDATA": //取消按钮发送的消息
                            cfg.cancelData = valueArray[1];
                            break;

                    }
                }
            }
            catch (e) {
                Config.log(e);
            }
            return cfg;
        },
        //添加按钮
        addButtons: function () {
            try {
                //根据类型来生成按钮
                this.sureButton = $("#FUN_OK").Button().Button("instance");
                this.cancelButton = $("#FUN_CANCEL").Button().Button("instance");

                if (this.options.ok != "") {
                    this.defaultOk = this.options.ok;
                } else {
                    this.defaultOk = Config.PromptSureText;
                }
                //this.sureButton.setValue(this.defaultOk);

                if (this.options.cancel != "") {
                    this.defaultCancel = this.options.cancel;
                } else {
                    this.defaultCancel = Config.PromptCancelText;
                }
                //this.cancelButton.setValue(this.defaultCancel);


                //this.sureButton.hide();
                //this.cancelButton.hide();
                //this.sureButton.Button("hide");
            } catch (e) {
                Config.log(e);
            }
        },
        //初始化事件侦听器
        initListener: function () {
            try {

            } catch (e) {
                Config.log(e);
            }
        },
        //移除控件时清除事件
        removeListener: function () {
            try {
                //this.element.off().unbind();
            } catch (e) {
                Config.log(e);
            }
        },
        //更新状态
        updateState: function () {
            try {

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
        //清除子界面
        clearChild: function () {
            try {
                $("#prompt-content-ui").empty();
            } catch (e) {
                Config.log(e);
            }
        },
        //-------------------------------------------重写控件方法
        //设置控件文本
        setText: function (value) {
            var result = true;
            try {
                this._super(value);
                $("#prompt-box-text").html(value);
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
                this._super();
                this.sureButton.clearFocus();
                this.cancelButton.clearFocus();
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
                this.sureButton.options.key = "1";
                this.cancelButton.options.key = "2";
                this.sureButton.setFocus();
                this.cancelButton.setFocus();
            } catch (e) {
                Config.log(e);
                return false;
            }
            return true;
        },
        //响应键盘输入
        keyAction: function (keyNum) {
            try {
                if (keyNum == "1") {
                    this.sureButton.keyAction(keyNum);
                }
                else if (keyNum == "2") {
                    this.cancelButton.keyAction(keyNum);
                }
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
                this.addButtons();//添加按钮
                this.initListener();//初始化事件侦听器
                Config.PromptBox = this;
            } catch (e) {
                Config.log(e);
            }
        },
        //初始化
        init: function () {
            try {

                //停止定时隐藏
                if (this.hideId != null) {
                    clearTimeout(this.hideId);
                    //clearInterval(this.hideId);
                    this.hideId = null;
                }
                $("#prompt-content-ui").empty();
                //设置提示框位置
                this.element.find(".middle-outer:first").css({
                    "left": this.options.deltaX,
                    "top": this.options.deltaY
                });
                //根据提示类型显示不同元素
                //this.options.isLoading = true;
                //this.options.promptType = Config.UI_OK;
                //this.options.promptType = Config.UI_ALPHAPROMPT;

                if (this.options.isLoading) {
                    //loading界面
                    $("#prompt-content-ui").hide();
                    $("#prompt-box-icon").show();
                    $("#prompt-box-text").show();
                    $("#prompt-box-wrapper").addClass("transparent");
                }
                else {
                    $("#prompt-box-wrapper").removeClass("transparent");
                    if (this.options.ui != "") {
                        //加载UI界面
                        $("#prompt-content-ui").show();
                        $("#prompt-box-icon").hide();
                        $("#prompt-box-text").hide();
                    }
                    else {
                        //提示信息
                        $("#prompt-content-ui").hide();
                        $("#prompt-box-icon").hide();
                        $("#prompt-box-text").show();
                    }
                }
                //设置按钮要发送的消息
                this.sureButton.options.dataToSend = this.options.okData;
                this.cancelButton.options.dataToSend = this.options.cancelData;
                //设置按钮位置
                switch (this.options.promptType) {
                    case Config.UI_OK:
                        //case Config.UI_OK_UI: 
                        this.sureButton.show();
                        this.cancelButton.hide();
                        //设置按钮文本
                        if (this.options.ok != "") {
                            this.sureButton.setValue(this.options.ok);
                        } else {
                            this.sureButton.setValue(this.defaultOk);
                        }
                        break;
                    case Config.UI_OK_CANCEL:
                        //case Config.UI_OK_CANCEL_UI: 
                        this.sureButton.show();
                        this.cancelButton.show();

                        //设置按钮文本
                        if (this.options.ok != "") {
                            this.sureButton.setValue(this.options.ok);
                        } else {
                            this.sureButton.setValue(this.defaultOk);
                        }
                        if (this.options.cancel != "") {
                            this.cancelButton.setValue(this.options.cancel);
                        } else {
                            this.cancelButton.setValue(this.defaultCancel);
                        }
                        break;
                    case Config.UI_ALPHAPROMPT:
                        if (this.options.text.indexOf(Config.CMD_INVAL_SEPARATOR) != -1) {
                            this.options.text = this.options.text.substr(1);
                        }
                        this.sureButton.hide();
                        this.cancelButton.hide();
                        break;
                    default:
                        this.sureButton.hide();
                        this.cancelButton.hide();
                        break;
                }
                //判断是否需要显示底部
                if (this.sureButton.isVisible() || this.cancelButton.isVisible()) {
                    $("#prompt-box-footer").removeClass("none");
                    $("#icon-and-content").removeClass("prompt-content-no-footer");
                } else {
                    $("#prompt-box-footer").addClass("none");
                    $("#icon-and-content").addClass("prompt-content-no-footer");
                }
                //根据格式设置文本框的文本显示
                this.setValue(this.options.text);
                //this.setFormat(); //设置文字格式
                this.show(); //显示提示框
                //this.options.time = 1;
                //判断是否定时隐藏
                if (this.options.time > 0) {
                    this.hideId = setTimeout(this.hideMe, this.options.time * 1000, this);
                }


                //判断是否加载新界面
                //if (Config.isPromptUi()) {
                //    Config.Ui.changeUi(config.ui);
                //}


                this.updateState();//更新状态

                //this.sureButton.keyAction();
                //this.element.find(".middle-outer:first").css({
                //    "left": 200,
                //    "top": this.options.deltaY
                //});
                //this["box_mc"].x = sw / 2 + this.options..deltaX;
                //this["box_mc"].y = sh / 2 + this.options.deltaY;
            } catch (e) {
                Config.log(e);
            }
        },
        //提示
        prompt: function (promptType, text) {
            try {
                var time = parseInt(getStrValue(promptType, ":", false));
                var type = getStrValue(promptType, ":", true);
                if (isNaN(time))
                    time = 0;
                //提示设置
                var tempArray = text.split(this.DEFINE_SEPARATOR);
                text = tempArray[0];
                var promptConfig = null;
                if (tempArray.length > 1) {
                    promptConfig = this.getPromtConfig(tempArray[1]);
                } else {
                    promptConfig = this.getPromtConfig(null);
                }
                promptConfig.text = text;
                promptConfig.time = time;
                promptConfig.promptType = type;
                switch (type) {
                    case Config.UI_PROMPT_LOADING:
                    case Config.UI_OK:
                    case Config.UI_OK_CANCEL:
                        //case Config.UI_PROMPT_UI: 
                        //case Config.UI_OK_UI: 
                        //case Config.UI_OK_CANCEL_UI:

                        this.init();
                        break;
                    case Config.UI_ALPHAPROMPT:
                        //不支持键盘，不可按
                        Config.TWIN_KEYBOARDENBLED = false;
                        this.init();
                        break;
                    case Config.UI_UNALPHAPROMPT:
                        //支持键盘，恢复可按
                        Config.TWIN_KEYBOARDENBLED = true;
                        //先清除
                        this.init();
                        break;
                    case Config.UI_PROMPT:
                        //先清除
                        this.init();
                        break;
                    case Config.UI_UNPROMPT:
                        this.hide();
                        break;
                    default:
                        break;
                }
                return true;
                //if (self == undefined)
                //    self = this;
                //self.hide();
            } catch (e) {
                Config.log(e);
            }
        },
        //隐藏提示框
        hideMe: function (self) {
            try {
                if (self == undefined)
                    self = this;
                self.hide();
            } catch (e) {
                Config.log(e);
            }
        },
        //销毁控件
        destroy: function () {
            try {
                this.sureButton.destroy();
                this.cancelButton.destroy();
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
    //$("#prompt-box").Promptbox();
    ////Config.Promptbox.prompt("OK_CANCEL:", "是否确定退出维护模式？");
    ////Config.PromptBox = null;
    ////console.log(Config.PromptBox);
    //Config.PromptBox.hide();
    //Config.PromptBox.setFocus();

    //Config.PromptBox.defaultOk = "确 定";
    //Config.PromptBox.defaultCancel = "取 消";

    //var str = "操作正在进行，请稍候...";
    //var length = 3;
    //length = 0;
    //for (var i = 0; i < length; i++) {
    //    str += str;
    //}
    //Config.PromptBox.prompt("PROMPT:0", str);
    ////Config.PromptBox.prompt("PROMPT:0", str + "\x1DDEFINE_PROMPT\x1D" + "" + "padding:5,13,6,53#bold:false#isLoading:true");


    //Config.PromptBox.prompt("OK_CANCEL:", str);
    //Config.Promptbox.hide();
});