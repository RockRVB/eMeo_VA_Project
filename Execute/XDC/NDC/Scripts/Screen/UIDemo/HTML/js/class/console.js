!(function(g) {
	var instance = null;

	function Constructor() {
		this.div = document.createElement("console");
		this.div.id = "console";
		this.div.style
		this.div.style.cssText = "filter:alpha(opacity=80);position:absolute;top:0px;right:0px;width:60%;border:1px solid #ccc;background:rgba(10,10,10,.3);color:#fff;border-bottom-left-radius:5px;padding:5px";
		document.body.appendChild(this.div);
	}
	Constructor.prototype = {
		log: function(args,type) {
			var methods={assert:'', clear:'', count:'', debug:'<span style="color:blue;">debug:</span>', dir:'', dirxml:'', exception:'', error:'<span style="color:red;">error:</span>', group:'', groupCollapsed:'', groupEnd:'', info:'', log:'<span style="color:green;">log:</span>', profile:'', profileEnd:'', table:'', time:'', timeEnd:'', timeStamp:'', trace:'', warn:'<span style="color:yellow;">warn:</span>'};
			var p = document.createElement("p"),str=methods[type]||methods.log;
			for(var i = 0; i < args.length; i++) {
				str+=args[i];
			}
			p.innerHTML = str;
			this.div.appendChild(p);
		},
		assert: function() {
			this.log(arguments,'dir');
		},
		warn: function() {
			this.log(arguments,'warn');
		},
		debug: function() {
			this.log(arguments,'debug');
		},
		clear: function() {
			this.div.innerHTML = "";
		}
	}
	function getInstance() {
		if(instance == null) {
			instance = new Constructor();
		}
		return instance;
	}
	g.console=getInstance();
}(window))