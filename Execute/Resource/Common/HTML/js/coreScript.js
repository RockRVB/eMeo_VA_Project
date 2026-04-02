const MessageType = {"Init":0, "Binding":1, "Submit":2, "SetValue":3, "GetValue":4, "EnumElement":5, "Navigate":6, "SetHotArea":7}
const MessageState = {"success":0, "timeout":1, "error":2}
const ElementType = {"ALL":{"Button":"","button":"","Input":"","input":""},"Button":{"Button":""},"button":{"button":""},"Input":{"Input":""},"input":{"input":""}}

function WebHost(verifycode){
    var _this = this
    var ws = null
    var enableHotArea = true
    var hotArea = []
    var msgobj = {}
    var interopMode = typeof(window.chrome.webview)=='object' && typeof(window.chrome.webview.hostObjects)=='function' && typeof(window.chrome.webview.hostObjects.eCAT)=='function'

    var wsconnect = function(){
        //var ws = new WebSocket('ws://localhost:8443/v1')
        var ws = new WebSocket('ws://10.1.144.50:8990/vtma/')
        ws.onmessage = msg=>handleMessage(msg)
        ws.onopen = function(){console.log("onopen")}
        ws.onerror = function(){console.log("onerror:"+this.readyState)}
        ws.onclose = function(){console.log("onclose") }
        return ws
    }
    
    if(typeof(window.chrome.webview) == 'object'){
        window.chrome.webview.addEventListener('message', msg=>handleMessage(msg));
    }else{
        ws = wsconnect()
    }
    
    this.onMessage = function(){
    }
    this.onpageinited = function(){
    }
    this.useHotArea = function(enable){
        enableHotArea = enable;
    }
    
    this.request = function(bind,callback,timeout=6000){
        if(!(bind instanceof Object)){
            if(typeof(callback) == 'function'){
                callback({state:MessageState.error,data:null,msg:'The first parameter is not an object'})
            }else{
                console.log('The first parameter is not an object')
            }
        }

        var msg = {type:MessageType.Binding,id:'',binding:bind}
        msg.id = new Date().getTime()
        msgobj[msg.id] =callback

        for (prop in bind){
            bind[prop] = ''
        }
        postMessage(msg)

        setTimeout(function(){ 
            if(msgobj.hasOwnProperty(msg.id)){
                delete msgobj[msg.id]
                if(typeof(callback) == 'function'){
                    callback({state:MessageState.timeout,data:null,msg:'The request timed out'})
                }else{
                    console.log('The first timed out')
                }
            }
        }, timeout)
    }
    
    this.submit = (eleid,eventname,extra) => {

        var msg = {type:MessageType.Submit,eleId:eleid,eventName:eventname,binding:{},extra:extra}

        var list = document.querySelectorAll('[content]')
        list.forEach(function(ele){
            var content = ele.getAttribute('content')
                if(content.includes('{Binding ') && content.includes('mode=2')){
                    var list = content.replace(/ +/g, ' ').replace('}','').split(' ')
                    var bindName = list[1]
                    if(!msg.binding.hasOwnProperty(bindName)){
                        msg.binding[bindName] = ele.tagName =='INPUT'? ele.value:ele.innerText
                    }
                }
        })
    
        postMessage(msg)
    }

    this.getAttribute = (eleId,attr) => {
        return getAttribute(eleId,attr)
    }
    
    this.enumElement = (enumType,enumFlag,msgId) => {
        var data = enumElement(enumType,enumFlag,msgId)
        var msg = {type:MessageType.EnumElement,extra:data};
        return JSON.stringify(msg)
    }

    var postMessage = (msg) => {
        
        if(msg == null){
            console.log('postMessage msg is null')
            return
        }

        msg.activityHashCode = window.actHash
        console.log('send:'+JSON.stringify(msg))
        msg.verify = verifycode=='randomflag'? null : verifycode

        if(typeof(window.chrome.webview) == 'object'){
            window.chrome.webview.postMessage(msg)
        }else{
            if(ws.readyState == WebSocket.OPEN){
                ws.send(msg)
            }else if(ws.readyState == WebSocket.CONNECTING){
                var interval = setInterval(function(){
                    if(ws.readyState != WebSocket.CONNECTING){
                        clearInterval(interval)
                        if(ws.readyState == WebSocket.OPEN){
                            ws.send(msg)
                        }else{
                            ws = wsconnect(this)
                            console.log("send fail,try reconnect webSocket server")
                        }
                    }
                }, 200)
            }else{
                ws = wsconnect(this)
                console.log("send fail,try reconnect webSocket server")
            }
        }
    }
    
    var handleMessage = function(evt){
        if(evt==null || typeof(evt)!='object'){
            console.log("invaild onmessage:"+evt)
            return
        }
        
        var msg = typeof(evt.data)=='object'? evt.data : eval('(' + evt.data + ')')
        console.log("onmessage:"+JSON.stringify(msg))
        if(msg != null && msg.type == MessageType.Binding && msg.id!=null){
            if(typeof(msgobj[msg.id]) == 'function'){
                msgobj[msg.id]({state:MessageState.success,data:msg.binding})
                delete msgobj[msg.id]
            }
        }
        
        parse(msg)

        if(typeof(_this.onMessage) == 'function'){
            _this.onMessage(evt)
        }

        if(msg.type == MessageType.Init && typeof(_this.onpageinited) == 'function'){
            _this.onpageinited()
        }
    }

    var parse = function(msg){
        if(msg != null && typeof(msg) == "object")
        {
            switch(msg.type)
            {
                case MessageType.Init:
                case MessageType.Binding:
                    updateBinding(msg)
                    if(msg.language != null){
                        changeLanguage(msg.language)
                    }
                    break;
                case MessageType.SetValue:
                    //if type == MessageType.SetValue, msg.extra[0] as attribute and value 
                    if(typeof(msg.eleId) == "string" && typeof(msg.extra) == "object"){
						for(let key in msg.extra){
							setAttribute(msg.eleId,key,msg.extra[key]);
						}
                        
                    }else{
                        console.log('invalid arguments');
                    }
                    break;
                case MessageType.GetValue:
                    //if type == MessageType.GetValue, msg.extra[0] as eleId and attribute 
                    if(typeof(msg.eleId) == "string" && typeof(msg.extra) == "object" && msg.extra.hasOwnProperty(msg.eleId)){
                        msg.extra[msg.eleId] = getAttribute(msg.eleId,msg.extra[msg.eleId])
                        postMessage(msg)
                    }else{
                        console.log('invalid arguments');
                    }
                    break;
                case MessageType.EnumElement:
                    if(msg.id != null && typeof(msg.eleId) == "string" && typeof(msg.eventName) == "string"){
                        msg.extra = enumElement(msg.eleId,msg.eventName)
                        postMessage(msg)
                    }else{
                        console.log('invalid arguments');
                    }
                    break;
                case MessageType.Navigate:
                    if(typeof(window.chrome.webview) == 'undefined'){
                        //just for websocket mode to navigate new url
                    }
                    break;
                default:
                    console.log('invalid msg.type:'+msg.type);
                    break;
            }
        }else{
            console.log('invalid msg:'+msg);
        }
    }

    var changeLanguage = language => {
        var list = document.querySelectorAll('link')
        list.forEach(function(ele){
            if(ele.getAttribute('replace') == '1' && ele.href.indexOf('.css') == ele.href.length-'.css'.length){
                var index = ele.href.lastIndexOf('/')+1;
                ele.href = ele.href.substring(0,index) +language +'.css'
            }
        })
    }

    var queryBinding = ()=>{
        var msg = {type:MessageType.Init,binding:{},ids:{},images:{}}
    
        var list = document.querySelectorAll('[ids]');
        list.forEach(function(ele){
            var key = ele.getAttribute('ids')
            if(!msg.ids.hasOwnProperty(key)){
                msg.ids[key] = '';
            }
        })
        
        list = document.querySelectorAll('[replace]');
        list.forEach(function(ele){
            if(ele.getAttribute('replace') == 1 && ele.tagName == 'IMG' && ele.getAttribute('srcKey')!=null){
                var srcKey = ele.getAttribute('srcKey')
                if(!msg.images.hasOwnProperty(srcKey)){
                    msg.images[srcKey] = ''
                }
            }
        })
    
        var attrs=['content','visible','enable']
        attrs.forEach(function(attr){
            list = document.querySelectorAll('['+attr+']')
            list.forEach(function(ele){
                var value = ele.getAttribute(attr)
                if(value!=null && value.includes('{Binding ')){
                    //{Binding core_CardText mode=2} => [core_CardText,2]
                    var list = value.replace(/ +/g, ' ').replace('}','').split(' ')
                    var bindName = list[1]
                    var mode = list.length>2 ? list[2].replace('mode=',''):'1';
                    if(!msg.binding.hasOwnProperty(bindName)){
                        msg.binding[bindName] = mode;
                    }
                }
            })
        })

        return msg
    }

    var updateBinding = obj => {
        if(obj == null || typeof(obj) != "object") return
        
        var data = obj.ids
        if(data != null && typeof(data) == "object"){
            const keys = Object.keys(data)
            var list = document.querySelectorAll('[ids]');
            list.forEach(function(ele){
                var value = ele.getAttribute('ids')
                if(keys.includes(value)){
                    ele.tagName =='INPUT'? ele.value = data[value]:ele.innerHTML = data[value]
                }
            })
        }
    
        data = obj.images
        if(data != null && typeof(data) == "object"){
            const keys = Object.keys(data)
            var list = document.querySelectorAll('[replace]');
            list.forEach(function(ele){
                if(ele.getAttribute('replace') == 1 && ele.tagName == 'IMG' && ele.getAttribute('srcKey')!=null){
                    var srcKey = ele.getAttribute('srcKey')
                    if(keys.includes(srcKey)){
                        ele.src = data[srcKey]
                    }
                }
            })
        }
    
        data = obj.binding
        if(data != null && typeof(data) == "object"){
            const keys = Object.keys(data)
            var attrs=['content','visible','enable']
            attrs.forEach(function(attr){
                var list = document.querySelectorAll('['+attr+']')
                list.forEach(function(ele){
                    var value = ele.getAttribute(attr)
                    if(value!=null && value.includes('{Binding ')){
                        var bindName = value.replace(/ +/g, ' ').replace('}','').split(' ')[1]
                        if(keys.includes(bindName)){
                            setAttributeByElement(ele,attr,data[bindName])
                        }
                    }
                })
            })
        }
    
    }

    var getAttribute = (eleId,attr) => {
        var value = null
        var ele = document.getElementById(eleId);
        if(ele!=null){
            if(attr=='content'){
                value = ele.tagName =='INPUT'? ele.value:ele.innerText
            }else if(attr=='visible'){
                value = ele.getAttribute('visible-bind')
            }else if(attr=='enable'){
                value = ele.getAttribute('enable-bind')
            }else{
                value = ele.getAttribute(attr)
            }
        }
        
        return value
    }

    var setAttribute = (eleId,attr,value) => {
        setAttributeByElement(document.getElementById(eleId),attr,value)
    }

    var setAttributeByElement = (ele,attr,value) => {
        if(ele!=null){
            if(attr=='content' || attr=='ids'){
                //ele.tagName =='INPUT'? ele.value = value:ele.innerText = value
				if(ele.tagName =='INPUT')
				{
					ele.value = value
					let evt = document.createEvent('HTMLEvents')
					evt.initEvent('input',true,true)
					ele.dispatchEvent(evt)
				}
				else if(ele.tagName =='IMG' && attr=='content')
				{
					if(value=='') return
					ele.src = value
				}
				else{
					ele.innerHTML = value
				}
            }else if(attr=='enable'){
                if(value=='') return
                if(value.toLowerCase() == 'false' || value == '0' || value === 0){
                    ele.setAttribute('enable-bind',false)
                }else{
                    ele.setAttribute('enable-bind',true)
                }
            }else if(attr=='visible'){
                if(value=='') return
                var condition = ele.getAttribute('visible_if')=='false'? false : true

                if(value.toLowerCase() == 'false' || value == '0' || value === 0){
                    ele.setAttribute('visible-bind',false)
                    ele.style.display = condition==true? 'none':'inline-block'
                }else{
                    ele.setAttribute('visible-bind',true)
                    ele.style.display = condition==true? 'inline-block':'none'
                }
            }else{
                ele.setAttribute(attr,value)
            }
        }
    }

    var enumElement = (enumType,enumFlag) => {
        var enumdata = {}
        
        list = document.querySelectorAll('[type]')
        list.forEach(function(ele){
            var type = ele.getAttribute('type')
            if(ele.id!=null && type!=null && ElementType.hasOwnProperty(enumType) && ElementType[enumType.toLowerCase()].hasOwnProperty(type.toLowerCase())){
                switch(enumFlag){
                    case "VisibleAndEnable":
                        if(getAttribute(ele.id,'visible')!='False' && getAttribute(ele.id,'enable')!='False'){
							let tagValue = ele.getAttribute('tag');
							if(tagValue != '' && tagValue != null && tagValue != undefined){
								enumdata[ele.id] = ele.getAttribute('tag');
							}                           
                        }
                        break;
                    case "OnlyVisible":
                        if(getAttribute(ele.id,'visible')!='False'){
                            let tagValue = ele.getAttribute('tag');
							if(tagValue != '' && tagValue != null && tagValue != undefined){
								enumdata[ele.id] = ele.getAttribute('tag');
							}        
                        }
                        break;
                    case "OnlyEnable":
                        if((getAttribute(ele.id,'enable')!='False')){
                            let tagValue = ele.getAttribute('tag');
							if(tagValue != '' && tagValue != null && tagValue != undefined){
								enumdata[ele.id] = ele.getAttribute('tag');
							}        
                        }
                        break;
                    case "VisibleOrEnable":
                        if(getAttribute(ele.id,'visible')!='False' || getAttribute(ele.id,'enable')!='False'){
                            let tagValue = ele.getAttribute('tag');
							if(tagValue != '' && tagValue != null && tagValue != undefined){
								enumdata[ele.id] = ele.getAttribute('tag');
							}        
                        }
                        break;
                    default:
                        break;
                }
            }
        })

        return enumdata
    }

    window.addEventListener('click',function(e){
        
        var ele = this.document.getElementById('atmcinterop')
        if(ele!=null && ele.getAttribute('handle')=='true'){
            ele.setAttribute('handle','false')
            _this.submit(ele.id, ele.getAttribute('tag'), null)
            return
        }

        var node = e.target
        do{
            var type = node.getAttribute('type');
            if(type!=null && type.toLowerCase() == 'button'){
                if(node.getAttribute('enable-bind') != 'false'){
                    var eventname = node.getAttribute('tag')
                    if(node.id !='' || eventname != null){
                        var list = node.querySelectorAll('[content]')
                        extra = list.length>0? {content:list[0].tagName =='INPUT'? list[0].value:list[0].innerText} : null
                        _this.submit(node.id, eventname, extra)
                        return
                    }
                }
                break;
            }else{
                node = node.parentNode
            }
        }while(node != document.body.parentNode)

        if(enableHotArea){
            //Todo：XDC屏幕支持，未实现
        }
    })

    window.addEventListener("load",function(){
        var initmsg = queryBinding()

        //webview2可使用内嵌对象初始化页面,同步返回数据更新页面
        if(interopMode){
			var json = JSON.stringify(initmsg);
			console.log('init:'+json)
            var data = window.chrome.webview.hostObjects.sync.eCAT.InitBindingData(json)
			console.log('recv:'+data)
			parse( eval('(' + data+ ')') )
        }else{
			//通过PostMessage或websocket发送初始化页面命令
            postMessage(initmsg)
        }
    })
}

webhost = new WebHost('randomflag')//randomflag 可在注入时替换成时间戳提高安全性

/*
function clickEventBinding() {
    var list = document.querySelectorAll('[type="Button"]')
    list.forEach(function(ele){
        if(!ele.hasAttribute('clickeventbind')){
            ele.setAttribute('clickeventbind',true)
            console.log(ele.id+' add clickeventbind')
            ele.addEventListener("click",function(){
                console.log("id:"+this.id+' clicked')
            });
        }
    })
}

function createobserver(){
    // Select the node that will be observed for mutations
    const targetNode = document.getElementById('body');

    // Options for the observer (which mutations to observe)
    const config = { attributes: true, childList: true, subtree: true };

    // Callback function to execute when mutations are observed
    const callback = function(mutationsList, observer) {
        // Use traditional 'for loops' for IE 11
        for(const mutation of mutationsList) {
            if (mutation.type === 'childList') {
                //console.log('A child node has been added or removed.');
                clickEventBinding()
            }
            else if (mutation.type === 'attributes') {
                //console.log('The ' + mutation.attributeName + ' attribute was modified.');
            }
        }
    };

    // Create an observer instance linked to the callback function
    const observer = new MutationObserver(callback);

    // Start observing the target node for configured mutations
    observer.observe(targetNode, config);

    // Later, you can stop observing
    //observer.disconnect();
}
*/
