init()

function init() {
    initUtils()
    try {
        if (VueI18n != undefined) initI18n()
    } catch (err) {}
    initVue()
    window.onload = function () {
        countDown();
    }
}

function initVue() {
    // 若在页面中没有直接new的话，自动初始化Vue
    document.addEventListener("DOMContentLoaded", function () {
        if (!initBase) {
            window.app = new Base().$mount('#app')
        }
    }, false)

    // 基类
    var initBase = false
    window.Base = Vue.extend({
        data: function () {
            return {}
        },
        beforeCreate: function () {
            initBase = true
        },
        mounted: function () {
            let _this = this;
            var exitBtn = document.querySelector('#exitBtn');
            var onHideSign = document.querySelector('#OnHideSign');
            var onShowSign = document.querySelector('#OnShowSign');
            var headerBox = document.querySelector('#grg-header');
            var grgMainBox = document.querySelector('#grg-main');
            var footerTimeBox = document.querySelector('.footer-box .time-box')
            exitBtn && exitBtn.addEventListener('click', function () {
                if (onHideSign) {
                    _this.onHideSign();
                }
                this.$confirm({
                    width: '40%',
                    maxwidth: '600px',
                    content: 'Do you want to exit?',
                    onOk: function () {
                        goto('OnCancel')
                        // this.$message('用户选择退出'); // this为app
                    },
                    onCancel: function () {
                        if (onShowSign) {
                            _this.onShowSign();
                        }
                    }
                })
            }.bind(this))
            // 发送ecat命令
            if (!document.querySelector('#atmcinterop')) {
                var input = document.createElement('input')
                input.setAttribute('tag', 'tag')
                input.setAttribute('handle', false)
                input.setAttribute('id', 'atmcinterop')
                input.setAttribute('type', 'hidden')
                document.body.insertBefore(input, document.querySelector('#app'))
            }
            if (document.querySelector('#imgLoading')) {
                animateImg({
                    dom: '#imgLoading',
                    dir: '../../../Image/CN/Animal/loading/',
                    img_prefix: 'loading1_000',
                    format: '.png',
                    frames: 20,
                })
            }
            if (headerBox) {
                let html = '<div class="header-logo-img"> \n' +
                    '<img src="../../images/img_2.4/logo.png" alt=""> \n' +
                    '</div> \n' +
                    '<div class="header-date"> \n' +
                    '<div id="time" class="time"></div> \n' +
                    '<div class="date"><span id="date" class="date-1">2023-02-01</span>|<span id="week" class="date-2">Monday</span></div> \n' +
                    '</div> \n'+
                    '<div class="header-bottom"></div>'
                headerBox.innerHTML = html;
                headerDate('time', 'date', 'week');
            }
            if (grgMainBox) {
                let circle1 = document.createElement('img');
                let circle2 = document.createElement('img');
                let circle3 = document.createElement('img');
                circle1.setAttribute('src', '../../images/img_2.4/circle1.png');
                circle2.setAttribute('src', '../../images/img_2.4/circle2.png');
                circle3.setAttribute('src', '../../images/img_2.4/circle3.png');
                circle1.setAttribute('class', 'circle-1');
                circle2.setAttribute('class', 'circle-2');
                circle3.setAttribute('class', 'circle-3');
                grgMainBox.appendChild(circle1);
                grgMainBox.appendChild(circle2);
                grgMainBox.appendChild(circle3);
            }
            if (footerTimeBox) {
                headerDate('time-b', 'date-b', '');
            }
        }
    })
}

// 图片切换动画
function animateImg(opt) {
    var dom = document.querySelector(opt.dom)
    var dir = opt.dir
    var format = opt.format
    var num = opt.frames
    var numLen = ('' + num).length
    var file_prefix = opt.img_prefix
    var fill_zero = []
    for (var h = numLen; h > 0; h--) {
        fill_zero.push(new Array(h).join(0))
    }

    var str = ''
    for (var i = 0; i < 20; i++) {
        str += '<img style="display: none" src="' + dir + file_prefix + fill_zero[i.toString().length - 1] + i + format + '"/>'
    }
    dom.innerHTML = str
    var cur = 0
    setInterval(function () {
        if (!dom) return
        if (cur == num) {
            cur = 0
            dom.children[num - 1].style.display = 'none'
        }
        cur - 1 >= 0 ? dom.children[cur - 1].style.display = 'none' : ''
        dom.children[cur].style.display = 'inline-block'
        cur++
    }, 100)
}

// 日期时间
function headerDate(timeID, dateID, weekID) {
    let weekArr = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    var date = new Date(),
        year = date.getFullYear(),
        month = date.getMonth() + 1,
        day = date.getDate(),
        hour = date.getHours(),
        minute = date.getMinutes(),
        second = date.getSeconds();
    month = month < 10 ? ("0" + month) : month;
    day = day < 10 ? ("0" + day) : day;
    hour = hour < 10 ? ("0" + hour) : hour;
    minute = minute < 10 ? ("0" + minute) : minute;
    second = second < 10 ? ("0" + second) : second;
    let setTime = hour + ':' + minute + ':' + second;
    let setDate = year + '-' + month + '-' + day;
    let setWeek = weekArr[new Date().getDay()];
    if (timeID) {
        document.getElementById(timeID).innerHTML = setTime;
    }
    if (dateID) {
        document.getElementById(dateID).innerHTML = setDate;
    }
    if (weekID) {
        document.getElementById(weekID).innerHTML = setWeek;
    }
    setTimeout(function () {
        headerDate(timeID);
    }, 1000);
}

// 倒计时
function countDown(time) {
    let newTime = time;
    let timer = null;
    if (document.querySelector('.count-down') && document.querySelector('.count-down-2')) {
        let timeBox = document.querySelector('.count-down-2');
        let initTime = document.querySelector('.count-down-2').innerHTML;
        timeBox.innerHTML = initTime + 's';
        let newInitTime = initTime ? parseInt(initTime) : newTime ? newTime : 60;
        clearInterval(timer);
        timer = setInterval(() => {
            initTime--;
            timeBox.innerHTML = initTime + 's';
            if (initTime == 0) {
                initTime = newInitTime + 1;
            }
        }, 1000)
    }
}

// 给ecat发送命令
function goto(tag) {
    var dom = document.querySelector('#atmcinterop')
    if (dom) {
        dom.setAttribute('tag', tag)
        dom.setAttribute('handle', true)
    }
}
// 国际化部分错误提示语
function initI18n() {
    // var messages = {
    //     'en-US': {
    //         createAccount: {
    //             name: 'username can not be empty',
    //             birthdate: 'birthdate can not be empty',
    //             gender: 'gender can not be empty',
    //             nation: 'nation can not be empty',
    //             email: 'the format of email is error',
    //             phoneNum: 'the fomat of phone is error',
    //         },
    //         faceRegist: {
    //             userid: 'Must be more than 6 digits of English or numbers'
    //         }
    //     },
    //     'zh-CN': {
    //         createAccount: {
    //             name: '用户名不能为空',
    //             birthdate: '日期不能为空',
    //             gender: '性别不能为空',
    //             nation: '国籍不能为空',
    //             email: '邮箱格式不正确',
    //             phoneNum: '号码格式不正确',
    //         },
    //         faceRegist: {
    //             userid: '至少6位的字母或数字组成'
    //         }
    //     }
    // }
    var messages = {
        'EN': window['EN'],
        'CN': window['CN'],
    }

    var i18n = new VueI18n({
        //locale: 'en-US', // 设置语言环境
        locale: localStorage.getItem('ibank-Language') ? localStorage.getItem('ibank-Language') : 'EN',
        messages: messages
    })
    window.i18n = i18n
}
/**
 * [changeValueListener 监听 js 修改 input 和 textarea 的 value。]
 * @param  {[HTMLElement]}   element  [具有 value 属性的 html 元素，如 input 和 textarea。]
 * @param  {Function} callback [回调，this 为 element，参数为当前值。]
 * @return {[HTMLElement]}            [element]
 */
// function changeValueListener(element, callback) {
//     var MutationObserver = window.MutationObserver || window.WebKitMutationObserver || window.MozMutationObserver;
//     var observer = new MutationObserver(function(mutations) {
//         // console.log('mutations',mutations)
//       mutations.forEach(function(record) {
//           if(record.attributeName == "value"){
//               // console.log('record.target',record.target)
//               callback(record.target.getAttribute('value'))
//           }
//       });
//   });
//   observer.observe(element, {
//       attributes: true,
//       childList: true,
//       characterData: true,
//       attributeOldValue :true,
//       attributeFilter:["value"]//只监听value属性,提高性能
//   });
// }
function changeValueListener(element, callback) {
    if (!Array.isArray(element.callbacks)) {
        element.callbacks = [];
        var valueDes = Object.getOwnPropertyDescriptor(element.constructor.prototype, "value");
        Object.defineProperty(element, 'value', {
            set: function (v) {
                element.callbacks.forEach(function (callback) {
                    callback.call(this, v);
                });
                valueDes.set.apply(this, arguments);
            },
            get: function () {
                // console.log('get', this, arguments);
                return valueDes.get.apply(this, arguments);
            }
        });
    }
    element.callbacks.push(callback);
    return element;
}

// 兼容Object.assing()
function initUtils() {
    if (typeof Object.assign != 'function') {
        Object.assign = function (target) {
            'use strict';
            if (target == null) {
                throw new TypeError('Cannot convert undefined or null to object');
            }

            target = Object(target);
            for (var index = 1; index < arguments.length; index++) {
                var source = arguments[index];
                if (source != null) {
                    for (var key in source) {
                        if (Object.prototype.hasOwnProperty.call(source, key)) {
                            target[key] = source[key];
                        }
                    }
                }
            }
            return target;
        };
    }
}

// 获取多语言键盘对应的键盘类型标识值
function getKeyboardLanguage() {
    let languageKeyboardMapper = {
        'SA': 'arabic',
        'CZ': 'czech',
        'CN': 'chinese',
        'EN': 'english',
        'FR': 'french',
        'DE': 'german',
        'GR': 'greek',
        'IL': 'hebrew',
        'IN': 'hindi',
        'IT': 'italian',
        'JP': 'japanese',
        'KR': 'korean',
        'NO': 'norwegian',
        'PL': 'polish',
        'RU': 'russian',
        'ES': 'spanish',
        'SE': 'swedish',
        'TH': 'thai',
        'TR': 'turkish',
        'UA': 'ukrainian',
    };

    var lang = window.external.GetData('core_currentLanguage');
    if (lang && languageKeyboardMapper[lang.toUpperCase()])
        return languageKeyboardMapper[lang.toUpperCase()];

    return '';
}

if (typeof (window.chrome) == 'object' && typeof (webhost) == 'undefined') {
    var head = document.getElementsByTagName('HEAD').item(0);
    var script = document.createElement("script");
    script.type = "text/javascript";
    script.src = "../../js/coreScript.js"
    head.appendChild(script);
}