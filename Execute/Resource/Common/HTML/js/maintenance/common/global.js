/******************** 
	作用:全局函数
	作者:蔡俊雄
	版本:V1.0
	时间:2015-01-20
********************/

//获取以分隔符分开的前或后子字符串
// function getStrValue(str, separator, front) {
//     var index = str.indexOf(separator);
//     if (front) {
//         return str.substr(0, index);
//     } else {
//         return str.substr(index + 1, str.length);
//     }
// }
function String2XML(xmlString) {
    // for IE 
    if (window.ActiveXObject) {
        var xmlobject = new ActiveXObject("Microsoft.XMLDOM");
        xmlobject.async = "false";
        xmlobject.loadXML(xmlString);
        return xmlobject;
    }
    // for other browsers 
    else {
        var parser = new DOMParser();
        var xmlobject = parser.parseFromString(xmlString, "text/xml");
        return xmlobject;
    }
}

function getRootPath_web() {
    //获取当前网址，如： http://localhost:8083/uimcardprj/share/meun.jsp
    var curWwwPath = window.document.location.href;
    //获取主机地址之后的目录，如： uimcardprj/share/meun.jsp
    var pathName = window.document.location.pathname;
    var pos = curWwwPath.indexOf(pathName);
    //获取主机地址，如： http://localhost:8083
    var localhostPaht = curWwwPath.substring(0, pos);
    return localhostPaht;
}

//取文件全名名称
function GetFileName(filepath) {
    if (filepath != "") {
        var names = filepath.split("\\");
        return names[names.length - 1];
    }
}

function getParentId(id) {
    var arr = new Array();
    $.each(json, function (i, item) {
        if (item.Id == id && item.ParentId != 0) {
            arr.push(item.ParentId);
            arr = arr.concat(getParentId(item.ParentId));
        }
    });
    return arr;
}

//-------------------------------------------配置
/******************** 
	作用:对齐方式
	作者:蔡俊雄
	版本:V1.0
	时间:2015-03-01
********************/
var AlignType = {
    /** 无对齐 */
    NONE: "none",

    //-----------------水平对齐方式
    /** 左对齐 */
    LEFT: "left",

    /** 中间对齐 */
    CENTER: "center",

    /** 右对齐 */
    RIGHT: "right",

    //-----------------垂直对齐方式
    /** 上对齐 */
    TOP: "top",

    /** 中间对齐 */
    MIDDLE: "middle",

    /** 下对齐 */
    BOTTOM: "bottom",

    //-----------------排列方式

    /** 排列方式:水平排列,从左到右,从上到下 */
    HORIZONTAL: "0",

    /** 排列方式:垂直排列,从上到下,从左到右 */
    VERTICAL: "1"
};
/******************** 
	作用:闪烁类型
	作者:蔡俊雄
	版本:V1.0
	时间:2015-02-11
********************/
var FlashType = {
    /** 依赖 */
    RELY: "rely",

    /** 显隐 */
    VISIBLE: "visible",

    /** 变换颜色 */
    COLOR: "color",

    /** 类 */
    CLASS: "class",

    /** 相反的 */
    OPPOSITE: "opposite",

    /** 相同的 */
    SAME: "same"
};

/******************** 
	作用:文本滚动类型
	作者:蔡俊雄
	版本:V1.0
	时间:2015-02-11
********************/
var TextScrollType = {
    /** 文字宽度超出容器宽度时滚动 */
    CONTAINER: "container",

    /** 文字宽度超出最大像素时滚动 */
    PIXEL: "pixel",

    /** 文字个数超出最大个数时会开始滚动 */
    TEXT: "text"
};

/******************** 
	作用:焦点类型
	作者:蔡俊雄
	版本:V1.0
	时间:2015-02-09
********************/
var FocusType = {
    /** 不能设置焦点 */
    IGNORE: "ignore",

    /** 一次只能设置一个焦点 */
    SINGLE: "single",

    /** 可以成组设置焦点 */
    GROUP: "group",

    /** 键盘类型 */
    KEYBOARD: "keyboard"
};

(function (window) {
    /******************** 
        作用:配置
        作者:蔡俊雄
        版本:V1.0
        时间:2015-02-09
    ********************/
    var Config = {
        /** 界面宽度 */
        ScreenWidth: 768,

        /** 界面高度 */
        ScreenHeight: 1024,

        /** ATM各部件名称 */
        AtmPartNames: [],
        //------------------------------------多语言支持

        /** 系统使用的语言标识 */
        Lang: "CN",

        /** 系统文字使用的字体 */
        Font: "微软雅黑",

        /** 系统使用的语言资源 */
        LanguageXml: null,

        /** 系统使用的语言配置 */
        Languages: {},

        /** 保存多语言数据 */
        LanguageData: null,

        /** 保存页面资源数据 */
        UIPageMappingData: null,

        //------------------------------------闪烁命令
        /** 开始闪烁的命令 */
        FlashStartPrefix: "StartFlash-",
        /** 停止闪烁的命令 */
        FlashStopPrefix: "StopFlash-",
        /** 停止当前所有闪烁的命令 */
        FlashStopAllCommand: "StopAllFlash",

        //------------------------------------新手上路视频路径
        /** 新手上路视频路径 */
        HelpVideoUrl: "res/flv/help.flv",

        /** 子界面的垂直偏移位置 */
        ChildDeltaY: 78,
        //
        //---------------------------------菜单------------------------------------------
        /**
         * 默认内存中菜单
         * (加载路径为"C:\GrgBanking\eCAT\TwinScreenMenu.xml")
         */
        TWIN_MENU: "",


        //---------------------------------其它参数------------------------------------------
        /** 
         * 受限功能(数组),当在 CmdManage 类中收到"TWIN_LIMITFUN"消息时设置,
         * 单个的格式为"256_FUN_3"(第一个下划线前的界面名称,后面的是功能按钮的ID)
         */
        TWIN_LIMITFUN: [],
        /** 保存ATM机型 */
        TWIN_ATM: "",

        /**
         * 保存ATM机芯类型(机芯类型的编号和Config数据库中CashDev保持一致)
         * <li><strong>8:</strong>表示CRM9250机芯</li>
         * <li><strong>9:</strong>表示HCM机芯</li>
         */
        TWIN_CASHDEVTYPE: 9,
        /**
         * 保存当前的屏幕类型(当在 CmdManage 类中收到"TWIN_MONITOR"消息时设置)
         * <li><strong>INTANG:</strong>不可触摸</li>
         * <li><strong>TANG:</strong>可触摸</li>
         */
        TWIN_MONITOR: "TANG",
        /**
         * 屏幕不可触摸
         */
        INTANG: "INTANG",
        /**
         * 屏幕可触摸
         */
        TANG: "TANG",

        /** 键盘的输入目标 */
        INPUT_TARGET: null,
        //---------------------CmdManage 类中 cmd 命令-----------------------------
        //------------命令字段

        /** 服务器发来的命令中的字段(Action) */
        CMD_ACTION: "Action",
        /** 服务器发来的命令中的字段(UI) */
        CMD_UI: "UI",
        /** 服务器发来的命令中的字段(Element) */
        CMD_ELEMENT: "Element",
        /** 服务器发来的命令中的字段(Value) */
        CMD_VALUE: "Value",
        /** 在代码中从未使用过 */
        CMD_SELECTED: "Selected",
        /** 在代码中从未使用过 */
        CMD_ENABLED: "Enabled",

        //------------属性类型

        /** 在代码中从未使用过 */
        CMD_ATTRIBUTE: ["text", "selected", "enabled"],
        /** 属性-对应于显示的文字  */
        CMD_ATTRIBUTE_TEXT: "text",
        /** 属性-对应于是否选择  */
        CMD_ATTRIBUTE_SELECTED: "selected",
        /** 属性-对应于是否启用  */
        CMD_ATTRIBUTE_ENABLED: "enabled",
        /** 属性-对应于是否可见  */
        CMD_ATTRIBUTE_VISIBLE: "visible",

        //------------命令类型

        /** 命令类型-提示框("PROMPT") */
        CMD_ACTION_PROMPT: "PROMPT",
        /** 命令类型-切换界面("CHANGE") */
        CMD_ACTION_CHANGE: "CHANGE",
        /** 
         * 命令类型-获取显示的文字("GETVALUE")
         * <li><strong>按钮:</strong>返回 label 属性</li>
         * <li><strong>文本框:</strong>返回 text 属性</li>
         */
        CMD_ACTION_GETVALUE: "GETVALUE",
        /** 
         * 命令类型-设置显示的文字("SETVALUE")
         * <li><strong>菜单:</strong>调用 SetMenu() 函数加载菜单xml文件</li>
         * <li><strong>按钮:</strong>设置 label 属性</li>
         * <li><strong>文本框:</strong>设置 text 属性</li>
         */
        CMD_ACTION_SETVALUE: "SETVALUE",
        /** 
         * 命令类型-取属性("GETATTRIBUTE")
         * <li><strong>按钮:</strong>获取"selected"和"enabled"属性</li>
         * <li><strong>文本框ExTextBox:</strong>调用 getRow() 函数获取属性</li>
         */
        CMD_ACTION_GETATTRIBUTE: "GETATTRIBUTE",
        /** 
         * 命令类型-设置属性("SETATTRIBUTE")
         * <li><strong>按钮:</strong>获取"selected"和"enabled"属性</li>
         * <li><strong>文本框ExTextBox:</strong>调用 getRow() 函数获取属性</li>
         */
        CMD_ACTION_SETATTRIBUTE: "SETATTRIBUTE",

        /** 在代码中从未使用过 */
        CMD_ACTION_INITIALIZED: "INITIALIZED",
        /** 在代码中从未使用过 */
        CMD_ACTION_UNEXIST: "UNEXIST",

        //------------分隔符

        /** 消息包内部分隔符 */
        CMD_INVAL_SEPARATOR: "\x1B",
        /** 返回值内部分隔符 */
        CMD_GETVAL_SEPARATOR: "\x1C",
        /** 密码分隔符,用于 LOGIN_ROLE 界面 */
        CMD_PWD_SEPARATOR: "\x1A",
        /** 返回包加尾 */
        CMD_DATA_SEPARATOR: "\x03",
        /** 下拉框,单选按钮,复选按钮的值的分隔符 */
        CMD_LIST_SEPARATOR: "#",
        /** 复选按钮选择值的分隔符 */
        CMD_CHECKBOXGROUP_SEPARATOR: "@",
        /** 表示退出 */
        FUN_QUIT: "FUN_QUIT",

        //---------------------Error错误类型----------------------------------------
        /** 报错,在代码中从未使用过 */
        CMD_ERROR: "ERROR -> ?",

        //---------------------Event事件--------------------------------------------
        /** xml文件加载完成(用于 TwinEvent 类中的相应事件名称) */
        EVT_XML_COMPLETE: "XMLCOMPLETE",
        /** swf文件加载完成(用于 TwinEvent 类中的相应事件名称) */
        EVT_SWF_COMPLETE: "SWFCOMPLETE",
        //
        /** 获取元素值(用于 TwinEvent 类中的相应事件名称) */
        EVT_ACTION_GET: "ACTION_GET",

        //------------键盘按键

        /** 按键1 */
        KEYBOARD_1: "KEYBOARD_1",
        /** 按键2 */
        KEYBOARD_2: "KEYBOARD_2",
        /** 按键3 */
        KEYBOARD_3: "KEYBOARD_3",
        /** 按键4 */
        KEYBOARD_4: "KEYBOARD_4",
        /** 按键5 */
        KEYBOARD_5: "KEYBOARD_5",
        /** 按键6 */
        KEYBOARD_6: "KEYBOARD_6",
        /** 按键7 */
        KEYBOARD_7: "KEYBOARD_7",
        /** 按键8 */
        KEYBOARD_8: "KEYBOARD_8",
        /** 按键9 */
        KEYBOARD_9: "KEYBOARD_9",
        /** 按键0 */
        KEYBOARD_0: "KEYBOARD_0",
        /** 按键A */
        KEYBOARD_A: "KEYBOARD_A",
        /** 按键B */
        KEYBOARD_B: "KEYBOARD_B",
        /** 按键C */
        KEYBOARD_C: "KEYBOARD_C",
        /** 按键D */
        KEYBOARD_D: "KEYBOARD_D",
        /** 按键E */
        KEYBOARD_E: "KEYBOARD_E",
        /** 按键F */
        KEYBOARD_F: "KEYBOARD_F",

        //---------------------状态 ID ------------------------------------------------------
        /** 状态-界面已初始化 */
        STATE_INITIALIZED: "STATE_INITIALIZED",
        /** 状态-执行命令成功 */
        STATE_SUCCEED: "STATE_SUCCEED",
        /** 状态-执行命令失败 */
        STATE_FAILED: "STATE_FAILED",

        //---------------------外部文件路径 ------------------------------------------------------
        /** xml文件-错误码范围 */
        ECRXML: "res/cfg/ErrorCodeRange.xml",
        /** xml文件-初始化配置(用于 TwinScreen 类中) */
        INITXML: "res/cfg/Init.xml",
        /** xml文件-侧键文件配置(用于 TwinScreen 类中) */
        SIDEBUTTONXML: "res/cfg/SideButton.xml",
        /** xml文件-主界面上的字符显示配置(用于 TwinScreen 类中) */
        UIXML: "res/cfg/UIManage.xml",
        /** 默认xml界面文件保存目录 */
        UI_XML: "res/cfg/UIxml/",
        /** 默认xml界面文件保存目录(用于调试) */
        UI_XML_DEBUG: "../cfg/UIxml/",
        /** 自定义xml界面文件保存目录 */
        DEFINEDUI_XML: "res/cfg/DefineUIxml/",
        /** 用于保存控件xml配置的目录 */
        COMPONENT_XML: "res/cfg/CompnentXml/",
        /** 用于保存错误xml文件的目录 */
        ERROR_XML: "res/cfg/ErrorXml/",
        /** 用于保存声音文件(用于 Menu_47 类中) */
        SOUND: "res/sound/Sound.mp3",
        /** 错误声音目录-中文(用于 MalfunctionGuide 类和 Menu_6 类中) */
        ERRORSOUND: "res/sound/errorSound/",
        /** 错误声音目录-英文(用于 MalfunctionGuide 类和 Menu_6 类中) */
        ERRORSOUND_EN: "res/sound/errorSound_EN/",
        /** 保存swf文件的目录 */
        UI_SWF: "res/swf/",
        /** 保存不同机型ATM对应的swf文件的目录 */
        URL_ATM: "res/img/ATM/",
        /** 保存错误图片的目录 */
        ERROR_IMG: "res/img/error/",
        /** 保存场景模式图标的目录 */
        ICO_IMG: "res/img/ico/",
        /** 保存背景图片的目录 */
        BG_IMG: "res/img/bg/",
        //#### Hegel 2012年12月20日14:22:40 #### res/cfg/CN/flash.xml 该地址为默认地址，实际应用需要发给我；
        //  IDS_DATA_ZN                 :            "res/cfg/CN.xml",
        //  IDS_DATA_EN                 :            "res/cfg/EN.xml",
        /** 保存语言配置文件 flash.xml 文件的内容 */
        //    LanguageData_Xml              :LanguageData,

        /** 语言配置文件路径,注意:在 TwinScreen 类中 URL_IDS_DATA 的值会发生变化,所以中英两个版本的flash.xml文件要保存多一份,一共需要4个flash.xml文件 */
        URL_IDS_DATA: "../Resource/Common/Text/",
        //   URL_IDS_DATA                   :            "res/cfg/",
        /** 语言配置文件名称 */
        IDS_NAME_XML: "/flash.xml",

        //------------------------------------文件
        /** 配置文件夹 */
        XmlCfgFolder: "res/cfg/",

        /** 一开始要加载的配置文件夹 */
        ConfigFile: "res/cfg/Config.xml",

        /** 后缀-xml文件 */
        Xml: ".xml",
        /** 后缀-swf文件 */
        Swf: ".swf",
        /** 后缀-jpg文件 */
        Jpg: ".jpg",
        /** 后缀-png文件 */
        Png: ".png",

        //----------------------初始化XML常量-----------------------
        //XML值

        /** 在 Init.xml 文件中的 link 节点,保存服务器连接配置 */
        LINK: "link",
        /** 在 Init.xml 文件中的 initUI 节点,保存初始化加载哪个 swf 文件 */
        INITUI: "initUI",
        /** 在 Init.xml 文件中的 initUI 节点的 value 属性,保存初始化加载哪个 swf 文件 */
        VALUE: "value",
        /** 在 Init.xml 文件中的 link 节点的 ip 属性,保存服务器IP */
        IP: "ip",
        /** 在 Init.xml 文件中的 link 节点的 port 属性,保存服务器端口 */
        PORT: "port",
        /** 在 Init.xml 文件中的 link 节点的 relink 属性,保存重连接服务器的时间(毫秒) */
        RELINK: "relink",
        /** 在 Init.xml 文件中的 link 节点的 delay 属性,保存多长时间(毫秒)执行一次命令数据 */
        DELAY: "delay",

        //----------------------默认值-----------------------
        /** 默认服务器IP */
        DEFAULT_IP: "127.0.0.1",
        /** 默认服务器端口 */
        DEFAULT_PORT: 9001,
        /** 默认服务器重连接时间(毫秒) */
        DEFAULT_RELINK: 5000,
        /** GB2312编码 */
        GB2312: "GB2312",

        /** utf-8编码 */
        UTF8: "utf-8",

        //----------------------主屏UI界面ID号-----------------------
        /** 系统一开始调用的加载界面 */
        UI_LOADING: "Loading",
        /** 系统初始化界面,启动时显示各部件的信息,部件名称,部件状态,如果有错误或有警告则显示相应的(错误码或警告信息) */
        UI_INIT: "Initialization",
        /** 对外服务模式界面 */
        UI_NORMAL: "SystemDiagnose",
        /** 登陆界面(界面提供输入用户ID和密码的形式) */
        UI_LOGIN_ID: "LOGIN_ID",
        /** 登陆界面(用户选择用户角色登陆) */
        UI_LOGIN_ROLE: "LoginRole",
        /** 维护模式界面(标准维护) */
        UI_MAINTENANCE: "Main",
        /** 维护模式界面(快捷维护) */
        UI_SHORTCUT: "Shortcut",
        /** 提供流程向导界面 */
        UI_HANDLEGUIDE: "HANDLEGUIDE",
        /** 提供向导式故障处理界面 */
        UI_MALFUNCTIONGUIDE: "ResolveError",
        /** 提供定制日常维护菜单界面 */
        UI_DEFINEMENU: "DefineMenu",
        /** 显示错误信息的界面 */
        UI_SHOWINFORMATION: "SHOWINFORMATION",

        //----------------------状态值-----------------------
        /** 暂停服务 */
        UI_OUTOFSERVICE: "2",
        /** 离线状态 */
        UI_OFFLINE: "4",
        //加电模式
        //  UI_POWERUP          :        "1",
        /** 对外服务 */
        UI_INSERVICE: "3",
        //维护模式
        //  UI_MAINTENANCEMODE  :        "5",

        //----------------------提示类型-----------------------
        /** 有确定按钮的提示框 */
        UI_PROMPT_LOADING: "LOADING",
        /** 有确定按钮的提示框 */
        UI_OK: "OK",
        /** 有确定按钮和取消按钮的提示框 */
        UI_OK_CANCEL: "OK_CANCEL",
        /** 只显示文字的提示框 */
        UI_PROMPT: "PROMPT",

        /** 有确定按钮的提示框(加载自定义界面) */
        UI_OK_UI: "UI_OK",
        /** 有确定按钮和取消按钮的提示框(加载自定义界面) */
        UI_OK_CANCEL_UI: "UI_OK_CANCEL",
        /** 只显示文字的提示框(加载自定义界面) */
        UI_PROMPT_UI: "UI_PROMPT",


        /** 取消提示框 */
        UI_UNPROMPT: "UNPROMPT",
        /** 透明提示框 */
        UI_ALPHAPROMPT: "ALPHAPROMPT",
        /** 取消透明提示框 */
        UI_UNALPHAPROMPT: "UNALPHAPROMPT",

        //----------------------支持界面元素ID号前辍-----------------------
        /** 图片类型前缀 */
        ELEMENTYPE_IMAGE: "IMAGE_",
        /** 静态文本或翻页框前缀 */
        ELEMENTYPE_STATIC: "STATIC_",
        /** 输入框前缀 */
        ELEMENTYPE_INPUT: "INPUT_",
        /** 功能按钮前缀 */
        ELEMENTYPE_FUN: "FUN_",
        /** 菜单按钮前缀 */
        ELEMENTYPE_MENU: "MENU_",
        /** 下拉框前缀 */
        ELEMENTYPE_LISTBOX: "Listbox_",
        /** 单选按钮组前缀 */
        ELEMENTYPE_RADIOGROUP: "RadioGroup_",
        /** 复选按钮组前缀 */
        ELEMENTYPE_CHECKBOXGROUP: "CheckboxGroup_",

        /** 数字键盘前缀 */
        ELEMENTYPE_KEYBOARD_NUMBER: "KEYBOARD_NUMBER_",

        //-----------------------界面控制变量-------------------------

        KEYBOARD_CLEAR: "CLEAR",



        /** 主界面 */
        Screen: null,



        //------------------------------------控件类型
        /** 类型-控件基类 */
        TYPE_COMPONENT: "COMPONENT",
        /** 类型-输出调试信息 */
        TYPE_DEBUG: "DEBUG",
        /** 类型-按钮 */
        TYPE_BUTTON: "BUTTON",
        /** 类型-快捷按钮 */
        TYPE_QUICK_BUTTON: "QUICK_BUTTON",
        /** 类型-操作按钮 */
        TYPE_OPERATE_BUTTON: "OPERATE_BUTTON",
        /** 类型-新手上路按钮 */
        TYPE_NEW_HAND_BUTTON: "NEW_HAND_BUTTON",
        /** 类型-常用按钮 */
        TYPE_COMMON_BUTTON: "COMMON_BUTTON",
        /** 类型-常用按钮-小 */
        TYPE_SMALL_COMMON_BUTTON: "SMALL_COMMON_BUTTON",
        /** 类型-提示按钮 */
        TYPE_PROMPT_BUTTON: "PROMPT_BUTTON",
        /** 类型-切换按钮 */
        TYPE_TOGGLE_BUTTON: "TOGGLE_BUTTON",
        /** 类型-定制菜单按钮 */
        TYPE_DEFINE_BUTTON: "DEFINE_BUTTON",
        /** 类型-向导按钮 */
        TYPE_GUIDE_BUTTON: "GUIDE_BUTTON",
        /** 类型-文本框按钮 */
        TYPE_TEXTBOX_BUTTON: "TEXTBOX_BUTTON",
        /** 类型-文本框按钮(顶) */
        TYPE_TEXTBOX_BUTTON_TOP: "TEXTBOX_BUTTON_TOP",
        /** 类型-文本框按钮(上) */
        TYPE_TEXTBOX_BUTTON_UP: "TEXTBOX_BUTTON_UP",
        /** 类型-文本框按钮(下) */
        TYPE_TEXTBOX_BUTTON_DOWN: "TEXTBOX_BUTTON_DOWN",
        /** 类型-文本框按钮(底) */
        TYPE_TEXTBOX_BUTTON_BOTTOM: "TEXTBOX_BUTTON_BOTTOM",
        /** 类型-文本框手型按钮 */
        TYPE_HAND_BUTTON: "HAND_BUTTON",
        /** 类型-翻页按钮 */
        TYPE_PAGE_BUTTON: "PAGE_BUTTON",
        /** 类型-翻页按钮(向左) */
        TYPE_PAGE_BUTTON_LEFT: "PAGE_BUTTON_LEFT",
        /** 类型-翻页按钮(向右) */
        TYPE_PAGE_BUTTON_RIGHT: "PAGE_BUTTON_RIGHT",
        /** 类型-翻页按钮(向上) */
        TYPE_PAGE_BUTTON_UP: "PAGE_BUTTON_UP",
        /** 类型-翻页按钮(向下) */
        TYPE_PAGE_BUTTON_DOWN: "PAGE_BUTTON_DOWN",
        /** 类型-箭头按钮(向左) */
        TYPE_ARROW_BUTTON_LEFT: "ARROW_BUTTON_LEFT",
        /** 类型-箭头按钮(向右) */
        TYPE_ARROW_BUTTON_RIGHT: "ARROW_BUTTON_RIGHT",
        /** 类型-键盘按钮 */
        TYPE_KEYBOARD_BUTTON: "KEYBOARD_BUTTON",

        /** 类型-菜单按钮1(一级菜单按钮) */
        TYPE_MENU_1_BUTTON: "MENU_1_BUTTON",
        /** 类型-菜单按钮2(二级菜单按钮) */
        TYPE_MENU_2_BUTTON: "MENU_2_BUTTON",
        /** 类型-菜单按钮3(三级菜单按钮) */
        TYPE_MENU_3_BUTTON: "MENU_3_BUTTON",

        /** 类型-长红色按钮 */
        TYPE_LONG_RED_BUTTON: "LONG_RED_BUTTON",
        /** 类型-红色按钮 */
        TYPE_RED_BUTTON: "RED_BUTTON",
        /** 类型-登陆切换按钮 */
        TYPE_LOGIN_TOGGLE_BUTTON: "LOGIN_TOGGLE_BUTTON",

        /** 类型-显示类型 */
        TYPE_DISPLAY: "DISPLAY",
        /** 类型-图片类型 */
        TYPE_IMAGE: "IMAGE",
        /** 类型-调整元件类型 */
        TYPE_ADJUST_PROPERTY: "ADJUST_PROPERTY",
        /** 类型-静态文本 */
        TYPE_TEXT: "STATIC_TXT",
        /** 类型-文本输入框 */
        TYPE_INPUT: "INPUT_TXT",

        /** 类型-数字键盘 */
        TYPE_KEYBOARD_NUMBER: "KEYBOARD_NUMBER",

        /** 类型-提示框 */
        TYPE_PROMPT_BOX: "PROMPT_BOX",

        /** 类型-单选按钮 */
        TYPE_RADIO: "RADIO",

        /** 类型-单选按钮组 */
        TYPE_RADIO_GROUP: "RADIO_GROUP",

        /** 类型-复选按钮 */
        TYPE_CHECKBOX: "CHECKBOX",

        /** 类型-复选按钮组 */
        TYPE_CHECKBOX_GROUP: "CHECKBOX_GROUP",

        /** 类型-下拉框内容 */
        TYPE_LISTBOX_CONTENT: "LISTBOX_CONTENT",

        /** 类型-下拉框内容单项文字动画 */
        TYPE_LISTBOX_ITEM: "LISTBOX_ITEM",

        /** 类型-下拉框 */
        TYPE_LISTBOX: "LISTBOX",

        /** 类型-表格 */
        TYPE_TABLE: "TABLE",

        /** 类型-功能按钮 */
        TYPE_MAINTENANCE_BUTTON: "FUN_BTN",
        /** 类型-菜单按钮 */
        TYPE_MENU_BUTTON: "MenuBTN",
        /** 类型-比较宽的可滚动文本框 */
        TYPE_WTEXTBOX: "WTEXTBOX",
        /** 类型-扩展的可滚动文本框 */
        TYPE_EXTEXTBOX: "EXTEXTBOX",
        /** 类型-可滚动文本框 */
        TYPE_TEXTBOX: "TEXTBOX",
        /** 类型-键盘 */
        TYPE_KEYBOARD: "KEYBOARD",
        /** 类型-分页键盘 */
        TYPE_KEYBOARD_ALL: "KEYBOARDALL",
        /** 类型-分页键盘 */
        TYPE_EXKEYBOARD: "EXKEYBOARD",



        //--------------------------全局配置

        /** 保存控件对应的配置类名称 */
        ComponentConfigClasses: [],
        /** 保存控件对应的类名称 */
        ComponentClasses: [],
        /** 保存控件类型 */
        ComponentTypes: [],

        /** 保存控件类型对应的解析配置 */
        ComponentConfigs: {
            enterKey: "确认", //确认
            clearKey: "更正", //更正
            changeKey: "切换", //切换
            exitKey: "退出" //退出
        },

        /** 界面中的提示框 */
        Prompt: null,

        /** 界面中的下拉框内容显示容器 */
        // ListboxContentMc:ListboxContentBox,

        /** ui界面管理 */
        Ui: null,

        /** 键盘管理 */
        Keyboard: null,

        /** 命令管理 */
        Command: null,

        /** socket管理 */
        Socket: null,

        /** 提示框中确认按钮的文字 */
        PromptSureText: "",

        /** 提示框中取消按钮的文字 */
        PromptCancelText: "",

        /** 提示框中透明提示的文字 */
        PromptAlphaText: "",

        /** 保存对应的模式界面 */
        ModeUiArray: [],

        /** 保存对应的子界面 */
        ChildUiMc: null,

        /** 保存对应的界面容器 */
        ChildContainer: null,

        /** 保存对应的界面容器 */
        Container: null,

        /** 保存菜单xml文件管理 */
        MenuXmlManage: null,

        /** 是否启试状态(true:调试状态 false:发布状态) */
        IsDebug: true,

        /** 是否写日志到后台 */
        IsWriteLogFile: false,

        /** 在开启调试的情况下要加载的ui界面,为空则不加载 */
        DebugLoadUi: "",

        /** 调试输出信息控件 */
        Debug: null,

        /** 未初始化时的值 */
        NOT_INIT: 9999999999,


        /** 标准维护模式中当前点了那个按钮。 */
        TWIN_MAINTENANCEINDEX: 0,
        /** 当前用户(在代码中从未使用过) */
        TWIN_USER: "",
        //键盘键值



        /** 用于保存服务器发送过来的键盘键值 */
        TWIN_KEYBOARD: "",
        /** 是否支持键盘(true:支持键盘 false:不支持键盘) */
        TWIN_KEYBOARDENBLED: true,

        /** 是否进入维护状态(true:进入维护状态 false:未进入维护状态) */
        bMaintenance: false,
        /** 错误码查询界面的前一个界面 */
        sQueryErrorCodePreUI: "",
        /** 是否进入错误码查询界面(true:进入 false:未进入) */
        bQueryErrorCode: false,
        /** adam 20151009 增加全局变量保存错误查询界面的当前菜单显示ID */
        bQueryErrorCodeCurMenuID: "",
        /** adam 20151009 增加全局变量保存当前菜单显示ID（当菜单获得焦点时赋值） */
        curMenuID: "",
        /** GET值的叠加 */
        sGET: "",

        /** 自定义界面名称 */
        sDefinedUI: "",
        //wyan
        /** 菜单按钮页数 */
        TWIN_SHORTCUT_PAGE: 1,
        /** 菜单按钮总数 */
        TWIN_SHORTCUT_TOTALMENU: 0,
        //wyan

        /** 是否开启写日志功能(1:开启 0:不开启) */
        OPEN_WRITELOG: 0,

        /** Nomal界面,默认机型开关(true:显示默认机型 false:显示相应的机型) */
        OPEN_NORMAL: false,
        /** 当前界面 */
        CurrentUI: "",
        /** 前一个模式界面 */
        PreModeUi: "",

        /** 是否处于快捷菜单模式 */
        isShortcut: false,

        /** 当前所有行数(用于滚动显示) */
        CurrentRowCount: 0,

        //----------------------保存当前拥有焦点的控件
        /** 保存当前拥有焦点的控件 */
        FocusElements: [],

        /** 密码键盘上切换文本输入框和键盘的按键对应的字符,"00"键对应字符"-" */
        TabKeyboardKey: "-",
        /** 密码键盘上切换焦点的按键对应的字符,空格键对应字符"KEYBOARD_TAB" */
        TabKey: "KEYBOARD_TAB",

        /** 设置中间条件变量，解决多次发送成功 */
        HasSend: true,

        /** 是否改变服务模式状态 */
        CatMode: true,


        //实现继承
        extend: function (subClass, superClass) {
            try {
                var F = function () { };
                F.prototype = superClass.prototype;
                subClass.prototype = new F();
                subClass.superClass = superClass.prototype;
            } catch (e) {
                Config.log(e);
            }
        },

        //从请求中获取参数
        getUrlParameters: function (url) {
            var vars = {};
            try {
                var parts = url.replace(/[?&]+([^=&]+)=([^&]*)/gi,
                    function (m, key, value) {
                        vars[key] = value;
                    });
            } catch (e) {
                Config.log(e);
            }
            return vars;
        },

        //获取子界面容器
        getChildContainer: function () {
            return null;
        },

        //记录信息
        log: function (data) {

            if (data == undefined || data == null) {
                Config.ConsoleLog(data);
            }
            else if (data.stack != undefined) {
                Config.ConsoleLog(data.stack);
            }
            else {
                Config.ConsoleLog(data);
            }

        },

        ConsoleLog: function (data) {
            if (Config.IsDebug) {
                console.log(data);
            }

            if (Config.IsWriteLogFile) {
                try {
                    window.external.ScriptWriteLog(data);
                }
                catch (e) {
                }
            }
        },

        showDebugInfo: function (data) {

            var str = $("#ShowDebugInfo").html();
            $("#ShowDebugInfo").html(str + "<br/>" + data);

        },
        //向服务器发送消息
        // send: function(data) {
        //     console.log("---------向服务器消息(START)-----------");
        //     console.log(data);
        //     console.log("---------向服务器消息(END)-----------");
        //     //if(this.Socket!=null)
        //     //    this.Socket.sendData(data);
        // },

        //是否显示按键提示(true:显示按键提示 false:不显示按键提示)
        showKeyTip: function () {
            return (this.TWIN_MONITOR == "INTANG");
        },
        //向服务器发送消息
        send: function (data) {
            // console.log("---------向服务器消息(START)-----------");
            // console.log("---------向服务器消息(END)-----------");
            try {
                if (Config.Socket == null) return;
                Config.Socket.send(data);
            } catch (e) {
                Config.log(e);
            }
        },
        //向服务器发送初始化消息
        sendInit: function (data) {
            try {
                if (Config.Socket == null) return;
                var data = {
                    "action": "changeui",
                    "success": true,
                    "message": "",
                    "data": []
                };
                // Config.send(Config.STATE_INITIALIZED);
                Config.send(data);
                Config.Socket.completed = true;
            } catch (e) {
                Config.log(e);
            }
        },

		/******************** 
		作用:交易数据
		作者:Adam
		版本:V1.0
		时间:2015-05-28
		********************/
        tranData:
        {
            //是否流程处理
            "handelGuide": false,
        },

        /** 当前步骤菜单ID */
        curGuideMenuID: "0",
        /**当前eCAT程序运行目录（只包含当前目录名） */
        eCATDomainPath: "",
        eCATCurrentWorkDirectory: "",
		/******************** 
		作用:缓存后台文本资源，maintenance.xml
		初始化：在设备启动界面开始异步初始化并赋值
		作者:Adam
		版本:V1.0
		时间:2015-11-04
		********************/
        MaintenanceTextResource: null,
        /** 保存钱箱类型对应的解析配置 add by xjyong*/
        UnitConfigs: {
            widthdrawalKey: "只取",
            cashInKey: "只存",
            recylingKey: "循环",
			retractKey:"回收",
            rejectKey: "回收",
            unknowKey: "未知"
        },
    };



    //实例化Config
    //--------------------------------
    if (window.Config == undefined) {
        if (window.top.Config == undefined)
            window.Config = Config;
        else
            window.Config = window.top.Config;
    }
    //--------------------------------
})(window);


/******************** 
	作用:按键管理
	作者:蔡俊雄
	版本:V1.0
	时间:2015-02-10
********************/
var KeyManage = {
    /**
     * 当前是哪一种控件
     * <li>1:键盘之外的控件</li>
     * <li>2:键盘</li>
     */
    typeIndex: 1,

    /**
     * 按下PC键盘按键
     * @param	e 键盘事件对象
     */
    pressPcKey: function (e) {
        var key = e.keyCode;
        var currentKey; //当前键值
        if (48 <= key && key <= 57) {
            //0-9
            if (Config.FocusElements.length > 0 && Config.FocusElements[0].type != Config.TYPE_INPUT)
                currentKey = (key - 48) + "";
        } else if (96 <= key && key <= 105) {
            //0-9
            if (Config.FocusElements.length > 0 && Config.FocusElements[0].type != Config.TYPE_INPUT)
                currentKey = (key - 96) + "";
        } else if (key == 56) {
            currentKey = "CLEAR"; //更正
        } else if (key == 13 || key == 13) {
            currentKey = "ENTER"; //确认按钮
        } else if (key == e.ctrlKey) {
            currentKey = Config.TabKeyboardKey; //密码键盘上切换文本输入框和键盘的按键对应的字符
        } else if (key == 9) {
            currentKey = Config.TabKey; //密码键盘上切换焦点的按键对应的字符
        } else if (key == 190 || key == 190) {
            if (Config.FocusElements.length > 0 && Config.FocusElements[0].type != Config.TYPE_INPUT)
                currentKey = "."; //点号
        } else {
            //currentKey = key+"";//其它
        }
        if (currentKey != null)
            this.parseKey(currentKey);
    },

    /**
     * 对按收到的按键进行处理
     * @param	key 键值
     */
    parseKey: function (key) {
        try {
            key = key.toLocaleUpperCase();
            switch (key) {
                case "CANCEL":
                    //取消
                    if (!Config.Prompt.visible) {
                        keyAction(key); //对按键作出反应
                    } else {
                        var promptType = Config.Prompt.options.promptType;
                        if (promptType != Config.UI_OK && promptType != Config.UI_OK_UI)
                            Config.send("FUN_CANCEL");
                    }
                    break;
                case "CLEAR":
                    //更正
                    if (!Config.Prompt.visible) {
                        keyAction(key); //对按键作出反应
                    }
                    break;
                case "ENTER":
                    //提示框
                    if (!Config.Prompt.visible) {
                        keyAction(key); //对按键作出反应
                    } else
                        Config.send("FUN_OK");

                    break;
                case "BLANK":
                    break;
                case Config.TabKey:
                    //提示框
                    if (!Config.Prompt.visible)
                        changeFocus(Config.TabKey);
                    break;
                default:
                    //其它按键
                    if (!Config.Prompt.visible) {
                        keyAction(key); //对按键作出反应
                    }
                    break;
            }
        } catch (e) {

        }
    },

    /**
     * 对按键作出反应
     * @param	key 键值
     */
    keyAction: function (key) {
        var container;
        //container = Config.getTopContainer();
        //if (container != null)
        //    container.parseKey(key);
    },

    /**
     * 切换焦点
     * @param	key KEYBOARD_TAB按键
     */
    changeFocus: function (key) {
        try {
            var container;
            if (!Config.Prompt.visible) {
                container = Config.getTopContainer();
                if (container != null)
                    container.parseKey(key);
                else if (Config.ChildUiMc != null)
                    Config.ChildUiMc.parseKey(key);

            }
        } catch (e) {

        }
    }

};

/******************** 
	作用:类型解析
	作者:蔡俊雄
	版本:V1.0
	时间:2015-03-09
********************/
var TypeConfig = {
    /** 静态文本 */
    "STATIC_TXT": {
        "type": Config.TYPE_TEXT,
        "class": "Text",
        "focus": FocusType.IGNORE,
        "focusGroup": "ignore"
    },
    /** 输入文本框 */
    "text": {
        "type": Config.TYPE_INPUT,
        "class": "InputText",
        "focus": FocusType.SINGLE,
        "focusGroup": "single"
    },
    /** 输入文本框-密码 */
    "password": {
        "type": Config.TYPE_INPUT,
        "class": "InputText",
        "focus": FocusType.SINGLE,
        "focusGroup": "single"
    },
    /** 图片 */
    "IMAGE": {
        "type": Config.TYPE_IMAGE,
        "class": "Image",
        "focus": FocusType.IGNORE,
        "focusGroup": "ignore"
    },
    /** 按钮 */
    "button": {
        "type": Config.TYPE_BUTTON,
        "class": "Button",
        "focus": FocusType.GROUP,
        "focusGroup": "button"
    },
    /** 滚动文本框 */
    "TEXTBOX": {
        "type": Config.TYPE_TEXTBOX,
        "class": "Textbox",
        "focus": FocusType.SINGLE,
        "focusGroup": "textbox"
    },
    /** 提示框 */
    "PROMPT_BOX": {
        "type": Config.TYPE_PROMPT_BOX,
        "class": "Promptbox",
        "focus": FocusType.SINGLE,
        "focusGroup": "promptbox"
    },
    /** 键盘 */
    "KEYBOARD": {
        "type": Config.TYPE_KEYBOARD,
        "class": "Keyboard",
        "focus": FocusType.KEYBOARD,
        "focusGroup": "keyboard"
    },
    /** 数字键盘 */
    "KEYBOARD_NUMBER": {
        "type": Config.TYPE_KEYBOARD_NUMBER,
        "class": "KeyboardNumber",
        "focus": FocusType.KEYBOARD,
        "focusGroup": "keyboard"
    },
    /** 登陆数字键盘 */
    "KEYBOARD_NUMBER_LOGIN": {
        "type": Config.TYPE_KEYBOARD_NUMBER_LOGIN,
        "class": "KeyboardNumber_LOGIN",
        "focus": FocusType.KEYBOARD,
        "focusGroup": "keyboard"
    },
    /** 表格控件 */
    "TABLE": {
        "type": Config.TYPE_TABLE,
        "class": "Table",
        "focus": FocusType.IGNORE,
        "focusGroup": "ignore"
    }
};

window.onload = function () {
    var timer = null;
    if (document.addEventListener) {//chrome、firefox、IE9+
        document.addEventListener('keydown', shieldRefresh);
    } else {//IE8-
        document.attachEvent('onkeydown', shieldRefresh);
    }

    function shieldRefresh(event) {

        var event = event || window.event;
        var keycode = event.keyCode || event.which;
        var obj = event.target || event.srcElement;//获取事件源       
        var t = obj.type || obj.getAttribute('type');//获取事件源类型 
        //获取作为判断条件的事件类型   
        var vReadOnly = obj.readOnly;
        var vDisabled = obj.disabled;
        //处理undefined值情况   
        vReadOnly = (vReadOnly == undefined) ? false : vReadOnly;
        vDisabled = (vDisabled == undefined) ? true : vDisabled;
        if (keycode == 116) {
            if (event.preventDefault) {//chrome、firefox、IE9+
                event.preventDefault();
            } else {//IE8-
                event.keyCode = 0;
                event.returnValue = false;
            }
        }
        if (keycode == 9) {
            if (event.preventDefault) {
                event.preventDefault();
            } else {
                event.keyCode = 0;
                event.returnValue = false;
            }
        }
        //当敲Backspace键时，事件源类型为密码或单行、多行文本的，    
        //并且readOnly属性为true或disabled属性为true的，则退格键失效    
        var flag1 = keycode == 8 && (t == "password" || t == "text" || t == "textarea") && (vReadOnly == true || vDisabled == true);
        //当敲Backspace键时，事件源类型非密码或单行、多行文本的，则退格键失效      
        var flag2 = keycode == 8 && t != "password" && t != "text" && t != "textarea";
        //判断      
        if (flag2 || flag1) {
            if (event.preventDefault) {
                event.preventDefault();
            } else {
                event.keyCode = 0;
                event.returnValue = false;
            }
        }
    }
};



//检测按键是否可用；  不可用返回false可用返回true；  当有弹窗的时候只允许使用enter和cencel按键，禁用其他按键   
function checkCanUseTheKey(key){	
	if(Config.Prompt.isVisible()){
		switch(key){ 
			case "FUN_QUIT":
			case "KEYBOARD_ENTER":
			case "ENTER":
			case "CANCEL": 
				return true;
			default:
				return false;
		}
	}else{
		return true;
	}

}




