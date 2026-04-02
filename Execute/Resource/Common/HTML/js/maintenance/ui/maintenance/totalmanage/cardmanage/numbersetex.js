<!-- setTimeout("switchMenu()",10); -->
var inputFlag = 0;
var inputAtNum = 0;  //当前正在操作的input框

function InvokeChildrenSwitch() {
    removeValueDataNNumber();
    return switchMenu();
}


function switchMenu() {
    var inputs = $(".tbodyFlag").find("input"); //获取所有的input框

    if (inputs.length > 0 && inputAtNum < inputs.length) {
        //有input框
        inputs[inputAtNum].click();
        inputs[inputAtNum].style.backgroundColor = "#FFFACD";
        inputAtNum++;
        return 0;
    } else if (inputs.length == 0 || inputs.length < inputAtNum) {
        //当前没有输入框，或者当前输入框已被删除
        inputAtNum = 0;
        //return 0;
    }

    if (inputAtNum == inputs.length) {
        //跳转到底部按钮
        inputAtNum = 0;
        SetButtomValue();
        return 1;
    }

}


function removeValueDataNNumber() {
    $(".tbodyFlag").find("input").css("backgroundColor", "");
    $(".tbodyFlag").find("td").blur();

    //清除底部按钮设定的值
    var buttonList = $(".g-button-zone button");
    var buttonListLength = buttonList.length;
    for (var i = 0; i < buttonListLength; i++) {
        var span = $(buttonList[i]).children(".button-text");
        var spanText = $(span).text();

        var reg = /^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 ，判断正整数用/^[1-9]+[0-9]*]*$/
        var num = spanText.substr(0, 1);
        if (reg.test(num)) {
            $(span).text(spanText.substr(1, spanText.length - 1));
        }
        $(buttonList[i]).attr("name", "");
    }

}

/**
 * 设置底部按钮
 * @returns {boolean}
 * @constructor
 */
function SetButtomValue() {
    var buttonList = $(".g-button-zone button");
    var buttonListLength = buttonList.length;
    for (var i = 0; i < buttonListLength; i++) {
        var span = $(buttonList[i]).children(".button-text");
        var spanText = $(span).text();
        //console.log(spanText);
        $(span).text((i + 1) + spanText);
        $(buttonList[i]).attr("name", "Select" + (i + 1));
    }
    return true;
}


function HandleKeyboardAction(keyValue) {
    switch (keyValue) {

        case "1":
            $("button[name='Select1']").click();
            return true;
        case "2":
            $("button[name='Select2']").click();
            return true;
        case "3":
            $("button[name='Select3']").click();
            return true;
        case "4":
            $("button[name='Select4']").click();
            return true;
        case "5":
            $("button[name='Select5']").click();
            return true;
        case "6":
            $("button[name='Select6']").click();
            return true;
        case "7":
            $("button[name='Select7']").click();
            return true;
        case "8":
            $("button[name='Select8']").click();
            return true;
        case "9":
            $("button[name='Select9']").click();
            return true;
        case "0":
            return true;
        case "CLEAR":
            return true;
        case "ENTER":
            return this.keyBoardAction("KEYBOARD_ENTER", key);
        case "CANCEL":
            return this.keyBoardAction("FUN_QUIT", key);
        case "KEYBOARD_TAB":
            return this.SwitchMenu();
        // 后续插入快捷键等等
        //.........
        default:
            return this.keyBoardAction("keyboard", key);
    }
}


$(document).ready(function () {
    $("#test").click(function () {
        var line = $(".tbodyFlag").children();
        var tds = $(".tbodyFlag").find("td");
        console.log(tds.length);
    });


    $("#test2").click(function () {
        InvokeChildrenSwitch();
    });


    $("#FUN_1").click(
        function () {
            //console.log("点击了添加按钮");
            //行数
            var line = $(".tbodyFlag").children().length + 1;
            if (line > 9) return;
            console.log(line);
            var trHtml = '<tr class="">' +
                '<td><span id="STATIC_Seq_' + line + '"  type="STATIC_TXT">' + line + '</span></td>' +
                '<td><input id="INPUT_BeginNum_' + line + '" type="text" class="input-text-cardno input-sheet  "  value="" maxlength="19" validchars = "0123456789" /></td>' +
                '<td><input id="INPUT_EndNum_' + line + '" type="text" class="input-text-cardno input-sheet  "  value="" maxlength="19" validchars = "0123456789" /></td>' +
                '<td><input id="INPUT_Count_' + line + '" type="text" class="input-text-cardcount input-sheet  "  value="" maxlength="12" validchars = "0123456789" /></td>' +
                '</tr>';

            $(".tbodyFlag").append(trHtml);
        }
    );


    $("#FUN_2").click(
        function () {
            //console.log("点击了删除按钮");
            $(".tbodyFlag").children().last().remove();
        }
    );
});



