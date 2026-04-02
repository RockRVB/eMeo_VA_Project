// 支持返回多种值：true, false, exit, timeout, back

function CheckConditions() {
    // 优先检查特定网址
    var currentUrl = window.location.href;
    
    // 检测GRG Banking特定页面
    if (currentUrl === "https://global.grgbanking.com/en/ProductList_134.html#99") {
        console.log("检测到GRG Banking产品列表页面，返回 true");
        return "true";
    } else if (currentUrl === "https://global.grgbanking.com/en/ProductDetail_101_152.html") {
        console.log("检测到GRG Banking BR-15N产品详情页面，返回 exit");
        return "exit";
    } else if (currentUrl === "https://global.grgbanking.com/en/ProductDetail_101_685.html") {
        console.log("检测到GRG Banking BR-15-41M产品详情页面，返回 back");
        return "back";
    }

    
    // 检查页面中是否存在特定元素来决定返回值
    
    // 示例1：检查成功完成标识
    var successElement = document.getElementById("transaction-success");
    if (successElement) {
        console.log("检测到交易成功元素，返回 true");
        return "true";
    }
    
    // 示例2：检查失败标识
    var errorElement = document.getElementById("transaction-error");
    if (errorElement) {
        console.log("检测到交易失败元素，返回 false");
        return "false";
    }
    
    // 示例3：检查退出标识
    var exitElement = document.getElementById("exit-button");
    if (exitElement && exitElement.style.display !== "none") {
        console.log("检测到退出按钮，返回 exit");
        return "exit";
    }
    
    // 示例4：检查超时标识
    var timeoutElement = document.getElementById("session-timeout");
    if (timeoutElement) {
        console.log("检测到会话超时，返回 timeout");
        return "timeout";
    }
    
    // 示例5：检查返回标识
    var backElement = document.getElementById("back-button");
    if (backElement && backElement.style.display !== "none") {
        console.log("检测到返回按钮，返回 back");
        return "back";
    }
    
    // 示例6：基于URL路径判断
    var currentPath = window.location.pathname;
    if (currentPath.includes("/success")) {
        console.log("URL包含success路径，返回 true");
        return "true";
    } else if (currentPath.includes("/error")) {
        console.log("URL包含error路径，返回 false");
        return "false";
    } else if (currentPath.includes("/exit")) {
        console.log("URL包含exit路径，返回 exit");
        return "exit";
    }
    
    // 示例7：基于页面标题判断
    var pageTitle = document.title.toLowerCase();
    if (pageTitle.includes("完成") || pageTitle.includes("success")) {
        console.log("页面标题包含完成标识，返回 true");
        return "true";
    } else if (pageTitle.includes("错误") || pageTitle.includes("error")) {
        console.log("页面标题包含错误标识，返回 false");
        return "false";
    }
    
    // 示例8：基于页面内容判断
    var bodyText = document.body.innerText.toLowerCase();
    if (bodyText.includes("交易成功") || bodyText.includes("操作完成")) {
        console.log("页面内容包含成功标识，返回 true");
        return "true";
    } else if (bodyText.includes("交易失败") || bodyText.includes("操作失败")) {
        console.log("页面内容包含失败标识，返回 false");
        return "false";
    }
    
    // 示例9：基于表单状态判断
    var forms = document.forms;
    for (var i = 0; i < forms.length; i++) {
        var form = forms[i];
        if (form.classList.contains("completed")) {
            console.log("检测到已完成的表单，返回 true");
            return "true";
        } else if (form.classList.contains("error")) {
            console.log("检测到错误的表单，返回 false");
            return "false";
        }
    }
    
    // 示例10：基于时间判断（模拟超时）
    var currentTime = new Date().getTime();
    var pageLoadTime = window.performance.timing.navigationStart;
    var timeOnPage = currentTime - pageLoadTime;
    
    if (timeOnPage > 300000) { // 5分钟超时
        console.log("页面停留时间超过5分钟，返回 timeout");
        return "timeout";
    }
    
    // 默认情况：如果没有匹配任何条件，返回true继续流程
    console.log("未匹配任何特定条件，返回默认值 true");
    return "false";
}

// 兼容性：如果页面没有定义greet函数，则使用此函数
if (typeof window.CheckConditions === 'undefined') {
    window.CheckConditions = CheckConditions;
}