// blink processings
function EraseKey(DIVid)
{
	var divObject  = document.getElementById(DIVid);
	if(divObject)
	{
		var _parentElement = divObject.parentNode;
    if(_parentElement)
    {
       _parentElement.removeChild(divObject); 
		}
	}
}

function ShowKeys()
{
  if ($.trim(document.getElementById("FDKtable").rows[0].cells[0].innerHTML).length != 0)
  {
    divText = "<div id=\"D_FDK_I\" class=\"FDK_I\"></div>"
    document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divText);
  }

  if ($.trim(document.getElementById("FDKtable").rows[0].cells[1].innerHTML).length != 0)
  {
    divText = "<div id=\"D_FDK_A\" class=\"FDK_A\"></div>"
    document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divText);
  }

  if ($.trim(document.getElementById("FDKtable").rows[1].cells[0].innerHTML).length != 0)
  {
    divText = "<div id=\"D_FDK_H\" class=\"FDK_H\"></div>"
    document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divText);
  }

  if ($.trim(document.getElementById("FDKtable").rows[1].cells[1].innerHTML).length != 0)
  {
    divText = "<div id=\"D_FDK_B\" class=\"FDK_B\"></div>"
    document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divText);
  }

  if ($.trim(document.getElementById("FDKtable").rows[2].cells[0].innerHTML).length != 0)
  {
    divText = "<div id=\"D_FDK_G\" class=\"FDK_G\"></div>"
    document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divText);
  }

  if ($.trim(document.getElementById("FDKtable").rows[2].cells[1].innerHTML).length != 0)
  {
    divText = "<div id=\"D_FDK_C\" class=\"FDK_C\"></div>"
    document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divText);
  }

  if ($.trim(document.getElementById("FDKtable").rows[3].cells[0].innerHTML).length != 0)
  {
    divText = "<div id=\"D_FDK_F\" class=\"FDK_F\"></div>"
    document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divText);
  }

  if ($.trim(document.getElementById("FDKtable").rows[3].cells[1].innerHTML).length != 0)
  {
    divText = "<div id=\"D_FDK_D\" class=\"FDK_D\"></div>"
    document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divText);
  }
}



function IsLetter(dataText)
{
  if(dataText.length <= 0)
  {
      return -1;
  }
  
  for(var i=0; i<dataText.length; ++i)
  {
    var letterText = dataText.charAt(i);
    if((letterText>='a'&&letterText<='z')||(letterText>='A'&&letterText<='Z'))
    {
       return i;
    }
  }
  
  return -1;
}

function ConvertLower(dataText)
{ 
  var indexDataText = IsLetter(dataText);
  if(indexDataText < 0)
  {
     return dataText;
  }
  var subHead = dataText.substring(0, indexDataText+1);
  var subText = dataText.substring(indexDataText+1);
  subText = subText.toLocaleLowerCase();
  dataText = subHead + subText
  return dataText;
}


// this function is used to combine one or two strings for transaction selection screen buttons
// pass this function two strings (assumes two lines of text on button) and one combined string will be returned 
// this function will do several things
// 1. Removes line drawing characters, inlcuding - + < > _
// 2. Replaces strings of space characters with a single space character
// 3. Converts all upper case strings (NDC screens are always upper case) to Title Case
// 4. Preserves acronyms such as PIN if they are in the strings, also handles when PIN is part of Password/PIN
function CombineTwoLabels(s1, s2)
{
  var a_key_label = ""
  var a_key_label1 = s1;
  var a_key_label2 = s2;
  var hyphen = " ";
  
//  test to see the first string ends with a hyphen, if it does, then set the string to null
//  if the first line ends with hyphen we do not want to put a space between the two lines when concatenating
  if (a_key_label1.substring(a_key_label1.length-1) == "-")
  {
      hyphen = "";
  }

//  replace line draw characters with nothing, they are removed
  a_key_label1 = a_key_label1.replace(/[-_=<>]/g, "");
  a_key_label2 = a_key_label2.replace(/[-_=<>]/g, "");

// replace consecutive space characters with a single space character  
  a_key_label1 = a_key_label1.replace(/  +/g, ' ')
  a_key_label2 = a_key_label2.replace(/  +/g, ' ')

// first, combine the two strings together and check the overll length, if it is less than or equal to 20
// then they are just left as combined with a space inserted between them and assumed to fit on one line in the key button graphic
// if the combined length is greater than 20, the two strings are combined with a <br> instead of a space, which causes
//  two separate lines to be displayed by the HTML code
  if ((a_key_label1.length + a_key_label2.length) <= 20) 
  {
      a_key_label = a_key_label1 + hyphen + a_key_label2;
  }
  else
  {
    a_key_label = a_key_label1 + "<br> " + a_key_label2;
  }
  
// THis code searches for a single '/' character and replaces it with ' / ' (adds a space on each side)
// this is done to facilitate looking for separate words that might be combined by /, such as PASSWORD/PIN
  a_key_label = a_key_label.replace(/\//g, ' / ')
  
// This code converts words that are not in the listLowerCase array to Capitalized (WITHDRAW = Withdraw)
// It also leaves words that are in the listUpperCase array as all capitals (PIN = PIN)
// Additional words can be added to each array to affect more words, just add following the existing template
  a_key_label = a_key_label.replace(/\w\S*/g, function(txt)
  {
    var listLowerCase = ["from", "to", "for"];
    var listUpperCase = ["PIN", "US"];
    var bFind = -1;
    for(var i=0; i<listLowerCase.length; i++)
    {
      if(txt.toLowerCase() == listLowerCase[i])
      {bFind = 1;break;}
    }
    
    for(var ii = 0; ii < listUpperCase.length; ii++)
    {
      if(txt.toUpperCase() == listUpperCase[ii])
      {bFind = 2;break;}
    }
    
    if (bFind == 1 )
      return txt.toLowerCase();
    else if(bFind == 2)
      return txt.toUpperCase();
    else
      return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
  }
  );
  
// This capitalizes the first word of the string, no matter what it is. It fixes when a minor lower case word is the first word
  a_key_label = a_key_label.charAt(0).toUpperCase() + a_key_label.substr(1);

// THis coverts the ' / ' back to '/'. It restores concatenated words like PASSWORD/PIN after they are "fixed"
  a_key_label = a_key_label.replace(/ \/ /g, '/');
  
// the return value is one single string that has a <br> to break the line if it is too long for one line
  return a_key_label;
}

 var GrgApp = parent.GetWebCtrl("GrgApp");
 GrgApp.DisableTouchFDKKey(true);
function OnSendFDKKey(KeyData){
  if(KeyData){
     GrgApp.SendKeyPress(KeyData);
  }
  
}
function buttonTouch(){
  var button = document.querySelectorAll("#btn_container li,.b_btn");
 
  for(var i = 0,len = button.length;i<len;i++){
      button[i].onmousedown = function(){
        this.classList.add("active");
      }
      button[i].onmouseup = function(){
        var self = this;
        setTimeout(function(){
          self.classList.remove("active");
        },260)
      }
  }
  
}
window.onload = function(){
  buttonTouch();
}