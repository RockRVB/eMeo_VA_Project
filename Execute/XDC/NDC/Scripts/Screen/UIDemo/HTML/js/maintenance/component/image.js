/******************** 
	作用:图片
	作者:蔡俊雄
	版本:V1.0
	时间:2015-03-02
********************/
(function ($) {
    $.widget("grgbanking.Image", $.grgbanking.Component, {

        //-------------------------------------------属性


        //-------------------------------------------配置
        //配置
        options: {
            /** 控件类型 */
            type: Config.TYPE_IMAGE,

            /** id */
            id: ""
        },
        //-------------------------------------------函数
        //读取配置
        readConfig: function () {
            this._super();
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
                this.element.attr("src", value);
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
    //$("image").Image();

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