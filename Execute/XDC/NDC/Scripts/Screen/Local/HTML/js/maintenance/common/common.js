/******************** 
	作用:通用类
	作者:蔡俊雄
	版本:V1.0
	时间:2015-04-22
********************/

String.prototype.padLeft = function(padChar, width) {
	var ret = this;
	while (ret.length < width) {
		if (ret.length + padChar.length < width) {
			ret = padChar + ret;
		} else {
			ret = padChar.substring(0, width - ret.length) + ret;
		}
	}
	return ret;
};
String.prototype.padRight = function(padChar, width) {
	var ret = this;
	while (ret.length < width) {
		if (ret.length + padChar.length < width) {
			ret += padChar;
		} else {
			ret += padChar.substring(0, width - ret.length);
		}
	}
	return ret;
};
String.prototype.trim = function() {
	return this.replace(/^\s+/, '').replace(/\s+$/, '');
};

String.prototype.format = function () {
    var val = this.toString();
    for (var a = 0, i = 0; a < arguments.length; a++) {
        if (arguments[a] instanceof Array) {
            for (var j = 0; j < arguments[a].length; j++) {
                val = val.replace(new RegExp("\\{" + i++ + "\\}", "g"), arguments[a][j]);
            }
        } else {
            val = val.replace(new RegExp("\\{" + i++ + "\\}", "g"), arguments[a]);
        }
    }
    return val;
};

String.prototype.trimLeft = function() {
	return this.replace(/^\s+/, '');
};
String.prototype.trimRight = function() {
	return this.replace(/\s+$/, '');
};
String.prototype.caption = function() {
	if (this) {
		return this.charAt(0).toUpperCase() + this.substr(1);
	}
	return this;
};
String.prototype.reverse = function() {
	var ret = '';
	for (var i = this.length - 1; i >= 0; i--) {
		ret += this.charAt(i);
	}
	return ret;
};
String.prototype.startWith = function(compareValue, ignoreCase) {
	if (ignoreCase) {
		return this.toLowerCase().indexOf(compareValue.toLowerCase()) == 0;
	}
	return this.indexOf(compareValue) == 0
};
String.prototype.endWith = function(compareValue, ignoreCase) {
	if (ignoreCase) {
		return this.toLowerCase().lastIndexOf(compareValue.toLowerCase()) == this.length - compareValue.length;
	}
	return this.lastIndexOf(compareValue) == this.length - compareValue.length;
};

(function(window) {

	function Common() {}

	//获取以分隔符分开的前或后子字符串
	Common.getStrValue = function(str, separator, front) {
		var index = str.indexOf(separator);
		if (front) {
			return str.substr(0, index);
		} else {
			return str.substr(index + 1, str.length);
		}
	}

	//判断字符串是否为空
	Common.isEmpty = function(str) {
		var result = false;
		if (str == null || str == undefined || str == "")
			result = true;
		return result;
	}
	
	//设置后维护语言,add by xjyong
	Common.setLanguage = function(language){
		Language.Lang = language;
		
	}
	//获取后维护语言,add by xjyong
	Common.getLanguage = function(){
		return Language.Lang;	
	}

	//--------------------------------
	if (window.Common == undefined) {
		if (window.top.Common == undefined)
			window.Common = Common;
		else
			window.Common = window.top.Common;
	}
	//--------------------------------
})(window);

//默认语言设置
var Language={
	Lang:"CN"
};