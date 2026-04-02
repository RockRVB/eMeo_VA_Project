
(function (window) {
    var PDFView = function (sID) {
        return new PDFView.prototype.init(sID);
    };
    PDFView.prototype = {
        PDFControlID: "_PDFControl_",
        signFieldHtml: "<div class='js-signature' " +
                            "data-width='{width}' " +
                            "data-height='{height}' " +
                            "data-border='1px solid #ffffff'" +
                            "data-background='#ffffff' " +
                            "data-line-color='#000000' " +
                            "data-auto-fit='true'>" +
                        "</div>",
        init: function (sID) {
            /// <summary>初始化</summary>
            this.PDFControlID = sID;
        },
        PDFControl: function () {
            /// <summary>获取对应的PDF控件对象（首先要调用appendToPage()方法，才能调用其他方法）</summary>
            //alert(privateConfig.PDFControlID);
            //alert(this.PDFControlID);
            return $("#" + this.PDFControlID)[0];
        },
        options: function (config) {
            /// <summary>配置控件属性（本方法必须在打开文档之前调用）</summary>
            var optionsProperty = this.PDFControl().Options;
            $.extend(optionsProperty, config);
        },
        appendToPage: function (elementID, optionsConfig) {
            /// <summary>把控件显示到指定的页面元素上（首先要调用appendToPage()方法，才能调用其他方法）</summary>
            /// <param name="elementID" type="String">html element id</param>
            /// <param name="optionsConfig" type="Object">需要初始化istyle ocx的属性值</param>
            if (window.ActiveXObject || "ActiveXObject" in window) {
                $("#" + elementID)[0].innerHTML = "<object classid=\"clsid:7017318C-BC50-4DAF-9E4A-10AC8364C315\" codebase=\"iStylePDF.cab#version=2,0,5,1103\" id='" + this.PDFControlID + "' height='100%'  width='100%'></object>";
            }
            else {
                var plugin = window.navigator.plugins["npistylepdf plugin dll"];
                if (plugin == undefined) {
                    $("#" + elementID)[0].innerHTML = "<a href='http://www.istylepdf.com/istylepdfupdate/x86/istylepdf-r2.0.5.1103-windows-x86.exe' class='install'>您尚未安装iStylePDF控件，点这里进行安装...</a>";
                    return;
                } else {
                    $("#" + elementID)[0].innerHTML = "<embed id='" + this.PDFControlID + "' type='application/npistyleax' width='100%' height='100%' />";
                }
            }
            var initConfig = {
                HighlightField: true,
                HistoryEnabled: false,
                DocumentsLayout: 0,
                TabBarVisible: false,
                TabCommandBarVisible: false
            };
            //用传入的配置参数修改默认的配置参数
            $.extend(initConfig, optionsConfig);
            //属性的初始化配置
            this.options(initConfig);
            return this;
        },
        openPDFFile: function (sFilePathOrUrl) {
            /// <summary>根据Url打开PDF文档</summary>
            /// <param name="sFilePathOrUrl" type="String">pdf文件的路径或者url</param>
            if (sFilePathOrUrl) {
                if (sFilePathOrUrl.indexOf("http") == 0)
                    this.PDFControl().Documents.OpenFromURL(sFilePathOrUrl);
                else
                    this.PDFControl().Documents.Open(sFilePathOrUrl);
            }
            return this;
        },
        getAllFieldData: function () {
            /// <summary>获取所有的Field数据</summary>
            var obj = {};
            var fields = this.PDFControl().Documents.ActiveDocument.Fields;
            for (var i = 0; i < fields.Count; i++) {
                var field = this.PDFControl().Documents.ActiveDocument.Fields.Item(i);
                obj[field.Name] = field.Value;
            }
            return JSON.stringify(obj);
        },
        savePDFFile: function (sSavePath, readOnly) {
            /// <summary>保存PDF文件</summary>
            /// <param name="sSavePath" type="String">文件的保存路径（完整目录）</param>
            /// <param name="readOnly" type="Boolean">是否只读，true只读保存，保存之后的文件不能再编辑；false不会控制只读，默认false</param>
            if (readOnly) {
                var fields = this.PDFControl().Documents.ActiveDocument.Fields;
                for (var i = 0; i < fields.Count; i++) {
                    this.PDFControl().Documents.ActiveDocument.Fields.Item(i).ReadOnly = true;
                }
            }
            return this.PDFControl().Documents.ActiveDocument.Save(sSavePath);
        },
        toolBarVisible: function (bShow) {
            /// <summary>是否显示工具栏</summary>
            /// <param name="bShow" type="Boolean">true：显示；false：隐藏</param>
            var Bars = this.PDFControl().CommandBars;
            var count = Bars.Count;
            for (var i = 0; i < count; i++) {
                var Bar = Bars.Item(i);
                if (Bar.Type == 0) {
                    Bar.Visible = bShow;
                }
            }
            return this;
        },
        menuBarVisible: function (bShow) {
            /// <summary>是否显示菜单栏</summary>
            /// <param name="bShow" type="Boolean">true：显示；false：隐藏</param>
            var vshow = this.PDFControl().CommandBars.Item("MenuBar").Visible;
            this.PDFControl().CommandBars.Item("MenuBar").Visible = bShow;
        },
        sign: function (sImageFilePath, sPfxFilePath, sPassword, oSignField) {
            try {
                //alert(this.CurrentSignatureField);
                oSignField = (oSignField == undefined) ? this.CurrentSignatureField : oSignField;
                if (oSignField != null) {
                    //alert("未签名的签名域");
                    var widget = oSignField.Widget;
                    //alert(sImageFilePath);
                    widget.SignatureAppearance(1, 0, 0, sImageFilePath, true);
                    sig = oSignField.AddSignature();
                    //alert(sig);
                    //参数1 签名文件保存路径，设置为空字符串即可。
                    //参数2 pfx证书路径
                    //参数3 pfx证书口令
                    sig.SaveSignatureFromPfx("", sPfxFilePath, sPassword);
                    this.PDFControl().Documents.ActiveDocument.Views.ActiveView.Refresh();
                }
            } catch (e) {
                //alert(e.message);
            }
        },
        processSignField: function (oField) {
            this.CurrentSignatureField = oField;
            var sHtml = this.signFieldHtml;
            //alert(this.CurrentSignatureField);
            //try {
            //    alert(jQuery("#FormFieldDataCacheKey").length);
            //} catch (e) {
            //    alert(e.message);
            //}
            if (oField != null) {
                if (oField.ReadyOnly)
                    return;
                if (oField.Type == 6) {
                    var $d = $(".dialog-demo-box");
                    //alert(10);
                    $d.dialog({
                        title: '请在空白区域手写签名', 				// title
                        dragable: false,
                        html: '', 						// html template
                        width: 600, 				// width
                        height: 300, 			// height
                        cannelText: '取消', 	// cannel text
                        confirmText: '确认', // confirm text
                        clearText: '清除画板', // confirm text
                        showFooter: true,
                        onClose: function () {	// colse callback
                        },
                        onOpen: false, 			// open callback
                        onConfirm: function () { //  confirm callback required
                            //alert(111);
                            var imageBase64 = $('.js-signature').jqSignature('getDataURL');
                            //alert(dataUrl);
                            imageBase64 = imageBase64.replace(/^data:image\/(png|jpg);base64,/, "")
                            //alert(imageBase64);
                            $("#signImageBase64String").val(imageBase64);
                            //alert($("#signImageBase64String").val());
                            $d.dialog().close();
                        },
                        onClear: function () {
                            $('.js-signature').empty();
                            //$('.js-signature').jqSignature('clearCanvas');
                            sHtml = sHtml.replace("{width}", "600").replace("{height}", ($(".body-content").height() - 5));
                            $d.find('.body-content').html(sHtml);
                            $('.js-signature').jqSignature();
                            $('.js-signature').on('jq.signature.changed', function () {
                                $(".confirm").css({ "background": "#651E7E" }).removeAttr("disabled");
                            });
                            //禁用确认按钮
                            $(".confirm").css({ "background": "#b2aea0" }).attr({ "disabled": "disabled" });
                        },
                        onCannel: function () {  	// Cannel callback
                        },
                        getContent: function () { 	// get Content callback
                            //var sHtml = "<div class='js-signature' " +
                            //        "data-width='600' " +
                            //        "data-height='" + ($(".body-content").height() - 5) + "' " +
                            //        "data-border='1px solid #ffffff'" +
                            //        "data-background='#ffffff' " +
                            //        "data-line-color='#000000' " +
                            //        "data-auto-fit='true'>" +
                            //    "</div>";
                            sHtml = sHtml.replace("{width}", "600").replace("{height}", ($(".body-content").height() - 5));
                            //alert(sHtml);
                            $d.find('.body-content').html(sHtml);
                        }
                    }).open();
                    //禁用确认按钮
                    $(".confirm").css({ "background": "#b2aea0" }).attr({ "disabled": "disabled" }).bind("click", function () {
                        //alert(1111);
                    });
                    //创建签名域
                    $('.js-signature').jqSignature();
                    $('.js-signature').on('jq.signature.changed', function () {
                        $(".confirm").css({ "background": "#651E7E" }).removeAttr("disabled");
                    });
                    var sig = oField.Signature;
                    //alert($("#EmptySignImageFile").val());
                    //alert($("#CertificateFilePath").val());
                    this.sign($("#EmptySignImageFile").val(), $("#CertificateFilePath").val(), $("#CertificatePassword").val(), sig);
                    //if (sig == null) {
                    //    alert("未签名的签名域");
                    //    var widget = oField.Widget;
                    //    widget.SignatureAppearance(1, 0, 0, "E:\\99.临时文件资料\\bitmap5.bmp", true);
                    //    sig = oField.AddSignature();
                    //    //参数1 签名文件保存路径，设置为空字符串即可。
                    //    //参数2 pfx证书路径
                    //    //参数3 pfx证书口令
                    //    sig.SaveSignatureFromPfx("", "E:\\99.临时文件资料\\JianLI.pfx", "123456");
                    //    iStylePDF.Documents.ActiveDocument.Views.ActiveView.Refresh();
                    //}
                };
                //alert(0);
            }
        }
    };

    //    PDFView.onReady = function (func) {
    //        /// <summary>pdf控件初始化完毕之后回调的方法</summary>
    //        /// <param name="func" type="function Object">方法对象（需要回调的js方法）</param>
    //        var oldonload = window.onload;
    //        if (typeof window.onload != 'function') {
    //            window.onload = func;
    //        } else {
    //            window.onload = function () {
    //                oldonload();
    //                func();
    //            }
    //        }
    //    };
    //    PDFView.extend = function (target, source) {
    //        /// <summary>扩展对象属性</summary>
    //        /// <param name="target" type="Object">需要被扩展的对象</param>
    //        /// <param name="source" type="Object">需要添加到扩展对象的属性值</param>
    //        for (var p in source) {
    //            if (source.hasOwnProperty(p)) {
    //                target[p] = source[p];
    //            }
    //        }
    //        return target;
    //    };
    //    PDFView.$$ = function (vParam) {
    //        /// <summary>增加$$快速获取dom元素的方法</summary>
    //        /// <param name="vParam" type="String/html element Object">传入html元素的id获取该对象</param>
    //        if (typeof (vParam) == "string") return document.getElementById(vParam);
    //        else return vParam;
    //    };
    //重置原型链
    PDFView.prototype.init.prototype = PDFView.prototype;
    window.PDFView = PDFView;
})(window);
