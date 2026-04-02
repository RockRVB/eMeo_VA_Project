window.doc = document.documentElement;
doc.setAttribute("data-useragent", navigator.userAgent);

var windowWidth = $(window).width();
var windowHeight = $(window).height();
$('body').css('overflow','hidden');
function bsStyle() {
$("select").selectpicker({
	//size: 11
});
}
function collapsibleNext() {
	$(".collapsible-next a.option-toggle").on("click", function(e) {
		e.preventDefault();
		$(this).toggleClass("active");
		$(this).closest(".collapsible-next").parent().toggleClass("toggle-on");
		$(this).closest(".collapsible-next").siblings().slideToggle("fast");
		$("#debitcardoff").toggleClass("hidden");
  });
}
function formFocus() {
  $(".form-field input").on("focus", function() {
    $(this).closest(".form-field").addClass("form-focus");
  }).on("blur", function() {
    $(this).closest(".form-field").removeClass("form-focus");
  });
}
function softKeyboard() {
  $(".keyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '` 1 2 3 4 5 6 7 8 9 0 - = {bksp}',
		  'q w e r t y u i o p [ ] \\',
		  'a s d f g h j k l ; \' {accept}',
		  '{shift} z x c v b n m , . / {shift}',
		  '{space}'
        ],
		'shift': [
          '~ ! @ # $ % ^ & * ( ) _ + {bksp}',
		  'Q W E R T Y U I O P { } |',
		  'A S D F G H J K L : " {accept}',
		  '{shift} Z X C V B N M ? {shift}',
		  '{space}'
        ],
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
      maxLength: 16,
	  usePreview: false,
	  autoAccept: true,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		
		$('html, body').animate({
			scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
  
  $(".namekeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
		'default': [
		  'Q W E R T Y U I O P {bksp}',
		  'A S D F G H J K L {accept}',
		  'Z X C V B N M /',
		  '{space}',
		  '{sp:1}'
        ]
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  maxLength: 19,
	  usePreview: false,
	  autoAccept: true,
	  beforeVisible: function(e, keyboard, el) { 
	    var t=$("#name").val();
		$("#name").val('').focus().val(t); 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		
		$('html, body').animate({
			scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
  
  $(".emailkeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '` 1 2 3 4 5 6 7 8 9 0 - = {bksp}',
		  'q w e r t y u i o p [ ] \\',
		  'a s d f g h j k l ; \' {accept}',
		  '{meta1} z x c v b n m @ , . / {meta1}',
		  '{space}'
        ],
		'meta1': [
          '~ ! @ # $ % ^ & * ( ) _ + {bksp}',
		  'Q W E R T Y U I O P { } |',
		  'A S D F G H J K L : " {accept}',
		  '{meta1} Z X C V B N M ? {meta1}',
		  '{space}'
        ],
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter',
		'meta1': '#+='
      },
	  maxLength: 60,
	  usePreview: false,
	  autoAccept: true,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		});  
		$(".page-container").addClass("scrollup");
		
		$('html, body').animate({
			scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  //屏蔽软件盘中"<"和">"键，防止通过软键盘注入不安全的html标签
	  beforeInsert  : function(e, keyboard, el, textToAdd) { 
		return (textToAdd=="<"||textToAdd==">")?"":textToAdd;
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
  $(".ickeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '1 2 3 {accept} ',
		  '4 5 6',
		  '7 8 9',
		  '0 {bksp}',
		  '{sp:1}'
        ]
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  css: {
        // keyboard container
        container: 'ui-widget-content ui-widget ui-corner-all ui-helper-clearfix number-keyboard',
        // default state
        buttonDefault: 'ui-state-default ui-corner-all number-keyboard',
      },
	  maxLength: 7,
	  autoAccept: true,
	  usePreview: false,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		$('html, body').animate({
        scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
  $(".dobkeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '1 2 3 {accept} ',
		  '4 5 6',
		  '7 8 9',
		  '0 {bksp}',
		  '{sp:1}'
        ]
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  css: {
        // keyboard container
        container: 'ui-widget-content ui-widget ui-corner-all ui-helper-clearfix number-keyboard',
        // default state
        buttonDefault: 'ui-state-default ui-corner-all number-keyboard',
      },
	  maxLength: 8,
	  autoAccept: true,
	  usePreview: false,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		$('html, body').animate({
        scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
  $(".sgphonekeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '1 2 3 {accept} ',
		  '4 5 6',
		  '7 8 9',
		  '0 {bksp}',
		  '{sp:1}'
        ]
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  css: {
        // keyboard container
        container: 'ui-widget-content ui-widget ui-corner-all ui-helper-clearfix number-keyboard',
        // default state
        buttonDefault: 'ui-state-default ui-corner-all number-keyboard',
      },
	  maxLength: 8,
	  autoAccept: true,
	  usePreview: false,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		$('html, body').animate({
        scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });

  $(".phonekeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '1 2 3 {accept} ',
		  '4 5 6',
		  '7 8 9',
		  '0 {bksp}',
		  '{sp:1}'
        ]
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  css: {
        // keyboard container
        container: 'ui-widget-content ui-widget ui-corner-all ui-helper-clearfix number-keyboard',
        // default state
        buttonDefault: 'ui-state-default ui-corner-all number-keyboard',
      },
	  maxLength: 16,
	  autoAccept: true,
	  usePreview: false,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		$('html, body').animate({
        scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
  
   $(".postcodekeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '1 2 3 {accept} ',
		  '4 5 6',
		  '7 8 9',
		  '0 {bksp}',
		  '{sp:1}'
        ]
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  css: {
        // keyboard container
        container: 'ui-widget-content ui-widget ui-corner-all ui-helper-clearfix number-keyboard',
        // default state
        buttonDefault: 'ui-state-default ui-corner-all number-keyboard',
      },
	  maxLength: 6,
	  autoAccept: true,
	  usePreview: false,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		$('html, body').animate({
        scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
  $(".blockkeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '` 1 2 3 4 5 6 7 8 9 0 - = {bksp}',
		  'q w e r t y u i o p [ ] \\',
		  'a s d f g h j k l ; \' {accept}',
		  '{shift} z x c v b n m , . / {shift}',
		  '{space}'
        ],
		'shift': [
          '~ ! @ # $ % ^ & * ( ) _ + {bksp}',
		  'Q W E R T Y U I O P { } |',
		  'A S D F G H J K L : " {accept}',
		  '{shift} Z X C V B N M ? {shift}',
		  '{space}'
        ],
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  maxLength: 4,
	  usePreview: false,
	  autoAccept: true,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		
		$('html, body').animate({
			scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  //屏蔽软件盘中"<"和">"键，防止通过软键盘注入不安全的html标签
	  beforeInsert  : function(e, keyboard, el, textToAdd) { 
		return (textToAdd=="<"||textToAdd==">")?"":textToAdd;
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
   $(".floorkeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '1 2 3 {accept} ',
		  '4 5 6',
		  '7 8 9',
		  '0 {bksp}',
		  '{sp:1}'
        ]
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  css: {
        // keyboard container
        container: 'ui-widget-content ui-widget ui-corner-all ui-helper-clearfix number-keyboard',
        // default state
        buttonDefault: 'ui-state-default ui-corner-all number-keyboard',
      },
	  maxLength: 3,
	  autoAccept: true,
	  usePreview: false,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		$('html, body').animate({
        scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
 
  $(".unitkeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '` 1 2 3 4 5 6 7 8 9 0 - = {bksp}',
		  'q w e r t y u i o p [ ] \\',
		  'a s d f g h j k l ; \' {accept}',
		  '{shift} z x c v b n m , . / {shift}',
		  '{space}'
        ],
		'shift': [
          '~ ! @ # $ % ^ & * ( ) _ + {bksp}',
		  'Q W E R T Y U I O P { } |',
		  'A S D F G H J K L : " {accept}',
		  '{shift} Z X C V B N M ? {shift}',
		  '{space}'
        ],
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  maxLength: 7,
	  usePreview: false,
	  autoAccept: true,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		
		$('html, body').animate({
			scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  //屏蔽软件盘中"<"和">"键，防止通过软键盘注入不安全的html标签
	  beforeInsert  : function(e, keyboard, el, textToAdd) { 
		return (textToAdd=="<"||textToAdd==">")?"":textToAdd;
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
  
   $(".streetkeyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '` 1 2 3 4 5 6 7 8 9 0 - = {bksp}',
		  'q w e r t y u i o p [ ] \\',
		  'a s d f g h j k l ; \' {accept}',
		  '{shift} z x c v b n m , . / {shift}',
		  '{space}'
        ],
		'shift': [
          '~ ! @ # $ % ^ & * ( ) _ + {bksp}',
		  'Q W E R T Y U I O P { } |',
		  'A S D F G H J K L : " {accept}',
		  '{shift} Z X C V B N M ? {shift}',
		  '{space}'
        ],
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  maxLength: 35,
	  usePreview: false,
	  autoAccept: true,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		
		$('html, body').animate({
			scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  //屏蔽软件盘中"<"和">"键，防止通过软键盘注入不安全的html标签
	  beforeInsert  : function(e, keyboard, el, textToAdd) { 
		return (textToAdd=="<"||textToAdd==">")?"":textToAdd;
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
  $(".street1keyboard").keyboard({
	  layout: 'custom',
      customLayout: {
        'default': [
          '` 1 2 3 4 5 6 7 8 9 0 - = {bksp}',
		  'q w e r t y u i o p [ ] \\',
		  'a s d f g h j k l ; \' {accept}',
		  '{shift} z x c v b n m , . / {shift}',
		  '{space}'
        ],
		'shift': [
          '~ ! @ # $ % ^ & * ( ) _ + {bksp}',
		  'Q W E R T Y U I O P { } |',
		  'A S D F G H J K L : " {accept}',
		  '{shift} Z X C V B N M ? {shift}',
		  '{space}'
        ],
      },
	  display: {
        'bksp': '&#9003:Backspace',
		'accept': 'Enter:Enter'
      },
	  maxLength: 25,
	  usePreview: false,
	  autoAccept: true,
	  beforeVisible: function(e, keyboard, el) { 
	  	$('.ui-keyboard-button').click( function() {
            $('#hiddenResetTime').text("true");
		}); 
		$(".page-container").addClass("scrollup");
		
		$('html, body').animate({
			scrollTop: $(this).offset().top - 200
		}, 300);
	  },
	  //屏蔽软件盘中"<"和">"键，防止通过软键盘注入不安全的html标签
	  beforeInsert  : function(e, keyboard, el, textToAdd) { 
		return (textToAdd=="<"||textToAdd==">")?"":textToAdd;
	  },
	  hidden: function(e, keyboard, el) { $(".page-container").removeClass("scrollup"); }
  });
}

$(function() {
  $("body").append("<div type='textblock' class='hidden' id='hiddenResetTime' content='{Binding ResetUICount mode=2}'/>");
  //Morton：2016/10/27 禁止拖动图片
$('img,a').attr({'ondragstart':'return false;','unselectable':'on','onselectstart':'return false;'});
  $('body').click( function() {  
               $('#hiddenResetTime').text("true");

            });  
  setTimeout(bsStyle, 10);
  
  collapsibleNext();
  softKeyboard();
  //@Shirley：2016/10/28 将光标定位到输入框末尾
  $('input').focus(function(){
  	var pos = this.selectionEnd;
  	this.setSelectionRange(pos,pos);
	});
});
