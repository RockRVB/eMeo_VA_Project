/******************** 
	作用:静态文本
	作者:蔡俊雄
	版本:V1.0
	时间:2015-01-24
********************/
(function ($) {
    var AlignType = {
        NONE: "none",//无对齐
        //-----------------水平对齐方式
        LEFT: "left",//左对齐
        CENTER: "center",//中间对齐
        RIGHT: "right",//右对齐
        //-----------------垂直对齐方式
        TOP: "top",//上对齐
        MIDDLE: "middle",//中间对齐
        BOTTOM: "bottom",//下对齐
        //-----------------排列方式
        HORIZONTAL: "0",//排列方式:水平排列,从左到右,从上到下
        VERTICAL: "1"//排列方式:垂直排列,从上到下,从左到右到右
    };
    //静态文本控件    
    $.fn.grgLabel = function (options) {
        var opts = $.extend({}, $.fn.grgLabel.defaults, options);
        return this.each(function () {
            $this = $(this);
            //var config = $.meta ? $.extend({}, opts, $this.data()) : opts;
            var config = opts;
            var parent = config.parent ? $(config.parent) : $this;//父级 
            //根据配置生成静态文本控件
            $("<div type='grgLabel'></div>").appendTo(parent).html("<div></div>")
            .css({
                //"position": config.position,
                "display": "table",
                "left": config.x,
                "top": config.y,
                "width": config.width,
                "height": config.height,
                "background-color": config.backgroundColor
            }).find("div").css({
                "display": "table-cell",
                "color": config.color,
                "background-color": "#ccc",
                "text-align": config.hAlign,
                "vertical-align": config.vAlign
            })
                //.html(config.text);
            .html("<span style='margin-left:20px;'>" + config.text + "</span>");



            //parent.hide();
            //debug(parent.html());  
            //    
            /*$this.css({     
              backgroundColor: o.background,     
              color: o.foreground     
            });     
            var markup = $this.html();     
            // call our format function     
            markup = $.fn.grgLabel.format(markup);     
            $this.html(markup);*/
        });
    };
    // 私有函数：debugging     
    function debug($obj) {
        alert($obj);
    };
    // 定义暴露format函数     
    $.fn.grgLabel.format = function (txt) {
        return '<strong>' + txt + '</strong>';
    };
    // 插件的默认配置     
    $.fn.grgLabel.defaults = {
        id: "",//文本的ID,默认为空
        parent: "",//文本的父级容器名称(默认为空,表示使用默认的父级)(如果有多层则以点号分隔)
        font: "微软雅黑",//字体名称
        color: "#000000",//文字颜色(格式:十六进制颜色 默认是#000000)
        /** 字体大小(格式:数字 默认是18) */
        size: 18,//字体大小(格式:数字 默认是18)
        autoSize: AlignType.LEFT,//控制文本字段的自动大小调整和对齐(默认为none)(none:无 left:左对齐 center:中间对齐 right:右对齐)
        hAlign: AlignType.CENTER,//水平对齐方式(默认为左对齐)(left:左对齐 center:中间对齐 right:右对齐)
        vAlign: AlignType.MIDDLE,//垂直对齐方式(默认为顶对齐)(top:顶对齐 middle:中间对齐 bottom:下对齐)
        position: "absolute",//位置
        x: 0,//x坐标(格式:数字 默认是0)
        y: 0,//y坐标(格式:数字 默认是0)
        width: "100px",//文字宽度(格式:数字 默认是100)
        height: "80px",//文字高度(格式:数字 默认是30)
        text: "微软雅黑",//要显示的文本内容(格式:字符串 默认为空)
        style: "",//样式

        disabledColor: "#B9B9B9",//文字禁用时的颜色(格式:十六进制颜色 默认是#B9B9B9)
        isEnabled: true,//控件是否启用(true:启用 false:禁用)
        isVisible: true,//控件是否可见(true:可见 false:隐藏)
        border: false,//指定文本字段是否具有边框(默认为无边框)(true:有边框 false:无边框)
        borderColor: "#000000",//文本字段边框的颜色(格式:十六进制颜色 默认是#000000)
        background: false,//文本字段是否具有背景填充(默认为没有背景填充)(true:显示背景颜色 false:不显示背景颜色)
        backgroundColor: "#FFFFFF",//文本字段背景的颜色(格式:十六进制颜色 默认是#FFFFFF)
        bold: false,//是否粗体(默认为不是粗体)(true:粗体 false:不是粗体)
        italic: false,//是否为斜体(默认为不是斜体)(true:斜体 false:不是斜体)
        multiline: true,//是否多行(默认为多行,如果滚动则为单行)(true:多行 false:单行)
        wordWrap: true,//是否自动换行(默认为自动换行,如果滚动则为不自动换行)(true:自动换行 false:不自动换行)
        leading: 0,//行间距(格式:数字 默认是0)
        letterSpacing: 0,//字符间距(格式:数字 默认是0)
        indent: 0,//首行缩进(格式:数字 默认是0)
        blockIndent: 0,//块缩进(格式:数字 默认是0)
        //----------------------自动调整字体大小设置
        isAutoAdjustSize: false,//是否自动调整字体大小使其可以在指定行内完全显示(默认为false,不自动调整字体大小)(true:自动调整字体大小 false:不自动调整字体大小)
        adjustToLines: 1,//调整文字行数,该参数当isAutoAdjustSize为true时有效(格式:数字 默认是1,表示1行)
        //----------------------滚动设置
        canScroll: true,//文本字段是否可以滚动(默认为可以滚动)(true:可以滚动 false:不滚动)
        scrollLength: 1000,//文本字段超过多少个字符时会开始滚动(格式:数字 默认是1000)
        scrollIndex: 0,//当前滚动到了哪个字符(格式:数字 默认是0)
        scrollInterval: 800,//滚动时间间隔(单位:毫秒 默认是800)
        scrollColor: "#FF0000",//滚动文字的颜色(格式:十六进制颜色 默认是#FF0000)
        prefix: "",//文字前缀(用于滚动时和text合并显示 默认为空)
        mouseControlScroll: true,//是否使用鼠标控制滚动(true:鼠标控制滚动,鼠标移上文字时停止滚动,鼠标移出文字时停止滚动 false:不使用鼠标控制滚动)(默认为 true )
        usePixelScroll: false,//文本字段是否使用像素判断滚动,默认是false(true:使用像素判断是否可以滚动 false:不使用像素判断是否可以滚动)
        maxPixel: 100,//文字最多显示多少像素,当usePixelScroll时有效(格式:数字 默认是100)
        scrollSpace: "   "//为了让滚动效果更好所添加的空格(格式:字符串 默认是3个空格)     
    };
    // 闭包结束     
})(jQuery);