// 若在页面中没有直接new的话，自动初始化Vue
document.addEventListener( "DOMContentLoaded", function() {
    if(!initBase) {
        window.app = new Base().$mount('#app')
    }
}, false)

// 基类
var initBase = false
var Base = Vue.extend({
    data: function() {
        return {}
    },
    beforeCreate: function() {
        initBase = true
    },
    mounted: function() {
        var exitBtn = document.querySelector('#exitBtn')
        exitBtn && exitBtn.addEventListener('click', function() {
            this.$confirm({ 
                width: '40%',
                maxwidth: '600px',
                content: 'Do you want to exit?',
                onOk: function() {
                    goto('exit')
                    this.$message('用户选择退出'); // this为app
                }
            })
        }.bind(this))
        // 发送ecat命令
        if(!document.querySelector('#atmcinterop')) {
            var input = document.createElement('input')
            input.setAttribute('tag', 'tag')
            input.setAttribute('handle', false)
            input.setAttribute('id', 'atmcinterop')
            input.setAttribute('type', 'hidden')
            document.body.insertBefore(input, document.querySelector('#app'))
        }        
        if(document.querySelector('#imgLoading')) {
            animateImg({
                dom: '#imgLoading',
                dir: '../../images/Animations/loading/',
                img_prefix: 'loading1_000',
                format: '.png',
                frames: 20,
            })
        }
    }
})

// 图片切换动画
function animateImg(opt) {
    var dom = document.querySelector(opt.dom)
    var dir = opt.dir
    var format = opt.format
    var num = opt.frames
    var numLen = ('' + num).length
    var file_prefix = opt.img_prefix
    var fill_zero = []
    for(var h=numLen; h > 0;h--){
        fill_zero.push(new Array(h).join(0))
    }
    
    var str = ''
    for(var i = 0; i < 20; i++) {
        str += '<img style="display: none" src="'+  dir + file_prefix + fill_zero[i.toString().length-1] + i + format + '"/>'
    }
    dom.innerHTML = str
    var cur = 0
    setInterval(function() {
        if(!dom) return 
        if(cur == num) {
            cur = 0 
            dom.children[num-1].style.display = 'none'
        }
        cur - 1 >= 0 ? dom.children[cur-1].style.display = 'none' : ''
        dom.children[cur].style.display = 'inline-block'
        cur++
    }, 100)
}

// 给ecat发送命令
function goto(tag){
    console.log(tag)
    var dom = document.querySelector('#atmcinterop')
    if(dom) {
        dom.setAttribute('tag', tag)
        dom.setAttribute('handle', true)
    }
}

/**
 * [changeValueListener 监听 js 修改 input 和 textarea 的 value。]
 * @param  {[HTMLElement]}   element  [具有 value 属性的 html 元素，如 input 和 textarea。]
 * @param  {Function} callback [回调，this 为 element，参数为当前值。]
 * @return {[HTMLElement]}            [element]
 */
// function changeValueListener2(element, callback) {
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
// function changeValueListener(element, callback) {
//     if (!Array.isArray(element.callbacks)) {
//         element.callbacks = [];
//         var valueDes = Object.getOwnPropertyDescriptor(element.constructor.prototype, "value");
//         Object.defineProperty(element, 'value', {
//             set: function(v) {
//                 // console.log('set', v, this, arguments);
//                 element.callbacks.forEach(function(callback) {
//                     callback.call(this, v);
//                 });
//                 valueDes.set.apply(this, arguments);
//             },
//             get: function() {
//                 // console.log('get', this, arguments);
//                 return valueDes.get.apply(this, arguments);
//             }
//         });
//     }
//     element.callbacks.push(callback);
//     return element;
// }



