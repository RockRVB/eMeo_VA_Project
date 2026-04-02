function FixPng()
{
    var arVersion = navigator.appVersion.split("MSIE");
    var version = parseFloat(arVersion[1]);

    if ((version <= 6) && (document.body.filters))
    {
        for (var i = 0; i < document.images.length; i++)
        {
            var img = document.images[i];
            var imgName = img.src.toUpperCase();
            if (imgName.indexOf(".PNG") > 0)
            {
                var width = img.width;
                var height = img.height;
                var sizingMethod = "image";
                img.runtimeStyle.filter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + img.src + "', sizingMethod='" + sizingMethod + "')";
                img.src = "../../../Video/CN/Blank.gif";
                img.width = width;
                img.height = height;
            }
        }
    }
}

function FixSinglePng(obj)
{
    if (!obj)
    {
        return;
    }

    var arVersion = navigator.appVersion.split("MSIE");
    var version = parseFloat(arVersion[1]);

    if ((version <= 6) && (document.body.filters))
    {
        var img = obj;
        var imgName = img.src.toUpperCase();
        if (imgName.indexOf(".PNG") > 0)
        {
            var width = img.width;
            var height = img.height;
            var sizingMethod = "image";
            img.runtimeStyle.filter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + img.src + "', sizingMethod='" + sizingMethod + "')";
            img.src = "../../../Video/CN/Blank.gif";
            img.width = width;
            img.height = height;
        }
    }
}

function AttachImgButton()
{
    var imgs = document.getElementsByTagName("IMG");
    var objImg;
    var upAttr;
    var downAttr;
    for (var i = 0; i < imgs.length; i++)
    {
        objImg = imgs[i];
        upAttr = objImg.getAttribute("up");
        downAttr = objImg.getAttribute("down");
        if (!!upAttr && !!downAttr)
        {
            objImg.onpropertychange = function ()
            {
                FixSinglePng(this);
            }
        }
    }
}


var flag = false;
function HiddenBlankTable(tableId, divPage) {
    setTimeout(function () {
        if (flag == false) {
            $("#" + tableId).each(function (i) {
                if (i == 1) {
                    if ($.trim($(this).find("#trandate").text()) == "") {
                        $("#" + divPage).hide();
                        return;
                    }
                }
            });
        }
    }, 1);
    flag = false;
}
function ShowTable(div) {
    flag = true;
    $("#" + divPage).show();
    return;
}

//页面加载完成时需要初始化的功能函数
function InitEvent()
{
    FixPng();
    AttachImgButton();
    document.onselectstart = new Function("event.returnValue=false;");
}

window.attachEvent("onload", InitEvent);