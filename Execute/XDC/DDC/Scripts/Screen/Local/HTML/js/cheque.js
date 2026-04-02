// blink processings
function ClearDIV(DIVid)
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

function InsertDIV(index, buttonText)
{
	var divTemp = "<div id=\"ckey\" style=\"background-image: url(../screenkeyR.png);\"></div>";
	
	var divText = "<div id=\"ButtonKeyTextD\">&nbsp;&nbsp;&nbsp;Proceed</div>";
	
	if(index == 1)
	{
		 divText = "<div id=\"D_FDK_A\" align=\"center\"><input name=\"ButtonA\" type=\"submit\" class=\"FDK_A\" onMouseDown=\" this.className='Down_FDK_A'\" onMouseUp=\"this.className='FDK_A'\" value=\"";
		 divText += buttonText;
		 divText += "\" /></div>"
		 
	}
	else if(index == 2)
	{
		 divText = "<div id=\"D_FDK_B\" align=\"center\"><input name=\"ButtonB\" type=\"submit\" class=\"FDK_B\" onMouseDown=\" this.className='Down_FDK_B'\" onMouseUp=\"this.className='FDK_B'\" value=\"";
		 divText += buttonText;
		 divText += "\" /></div>"
	}
	else if(index == 3)
	{
		 divText = "<div id=\"D_FDK_C\" align=\"right\"><input name=\"ButtonC\" type=\"submit\" class=\"FDK_C\" onMouseDown=\" this.className='Down_FDK_C'\" onMouseUp=\"this.className='FDK_C'\" value=\"";
		 divText += buttonText;
		 divText += "\" /></div>"
	}
	else if(index == 4)
	{
		 divText = "<div id=\"D_FDK_D\" align=\"center\"><input name=\"ButtonD\" type=\"submit\" class=\"FDK_D\" onMouseDown=\" this.className='Down_FDK_D'\" onMouseUp=\"this.className='FDK_D'\" value=\"";
		 divText += buttonText;
		 divText += "\" /></div>"
	}	
	else if(index == 5)
	{
		 divText = "<div id=\"D_FDK_F\" align=\"center\"><input name=\"ButtonF\" type=\"submit\" class=\"FDK_F\" onMouseDown=\" this.className='Down_FDK_F'\" onMouseUp=\"this.className='FDK_F'\" value=\"";
		 divText += buttonText;
		 divText += "\" /></div>"
	}
	else if(index == 6)
	{
		 divText = "<div id=\"D_FDK_G\" align=\"left\"><input name=\"ButtonG\" type=\"submit\" class=\"FDK_G\" onMouseDown=\" this.className='Down_FDK_G'\" onMouseUp=\"this.className='FDK_G'\" value=\"";
		 divText += buttonText;
		 divText += "\" /></div>"
	}
	else if(index == 7)
	{
		 divText = "<div id=\"D_FDK_H\" align=\"center\"><input name=\"ButtonH\" type=\"submit\" class=\"FDK_H\" onMouseDown=\" this.className='Down_FDK_H'\" onMouseUp=\"this.className='FDK_H'\" value=\"";
		 divText += buttonText;
		 divText += "\" /></div>"
	}
	else if(index == 8)
	{
		 divText = "<div id=\"D_FDK_I\" align=\"center\"><input name=\"ButtonI\" type=\"submit\" class=\"FDK_I\" onMouseDown=\" this.className='Down_FDK_I'\" onMouseUp=\"this.className='FDK_I'\" value=\"";
		 divText += buttonText;
		 divText += "\" /></div>"
	}
	
	else if(index == 9)
	{
		 divText = "<div id=\"D_FDK_B\" align=\"center\"><input name=\"ButtonB\" type=\"submit\" class=\"FDK_X\" onMouseDown=\" this.className='Down_FDK_X'\" onMouseUp=\"this.className='FDK_X'\" value=\"";
		 divText += buttonText;
		 divText += "\" /></div>"
	}
	
	
	document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divText);
}

function InsertPicture(PicturePath, PictureX, PictureY, PictureWidth, PictureHeight)
{
	
	var divTemp = "<DIV ID=\"ID_Picture\"STYLE=\"position:absolute; z-index:-1; left:"+PictureX+"px; top:"+PictureY+"px; width:"+PictureWidth+"%; height:"+PictureHeight+"%; background-image: url("+PicturePath+");\"></DIV>";
	
	//alert(divTemp);
	
	document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divTemp);
}
function InsertAmount(AmountValue,text)
{	
	var divTemp = "<div class='from'><div class='login' id='EditCheckDetail'>" + text || '**' + "</div> <div class='password'><input id=\"AMTENTRYText\" text-align =\"center\" value=\"";
	divTemp +=AmountValue;
	divTemp +="\" type=\"text\" name=\"info_entry\" maxlength=\"8\" id=\"AMTENTRYText\" /></div></div>";
	
	
	document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divTemp);

}

function ShowChequeAmount(AmountValue){

    var divTemp = "<DIV ID=\"ID_ShowChequeAmt\" style=\"position:absolute;left:630px; top:220px;\"><FONT STYLE=\"background-color:''; font-size:15px; font-weight:bold; letter-spacing:0px; \" COLOR=\"#000000\" FACE=\"Arial ROunded MT Bold\">";
	divTemp += AmountValue;
	divTemp += "</FONT></DIV>";
	//alert(divTemp);
	
	document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divTemp);
}

function EnterAmountTip()
{
    var divTemp = "<DIV ID=\"ID_EnterAmount\" STYLE=\"position:absolute; left:10px; top:360px; z-index:0;\"><FONT id='in_font' STYLE=\"background-color:''; font-size:22px; line-height:100%;font-weight:bold; letter-spacing:0px; \" COLOR=\"#000000\" FACE=\"Arial\">Please enter amount for check if incorrect</FONT></DIV>";
	//alert(divTemp);
	
	document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divTemp);
}

function WaitDisplayTip()
{
    var divTemp = "<DIV ID=\"ID_WaitDisply\" STYLE=\"position:absolute; left:10px; top:360px; z-index:0;\"><FONT id='in_font' STYLE=\"background-color:''; font-size:22px; line-height:100%;font-weight:bold; letter-spacing:0px; \" COLOR=\"#000000\" FACE=\"Arial\">Identifying check amount, please wait</FONT></DIV>";
	//alert(divTemp);
	
	document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divTemp);
}

function UnResumableTip(tipData)
{
    var divTemp = "<DIV ID=\"ID_WaitDisply\" STYLE=\"position:absolute; left:20px; top:320px; z-index:0;\"><FONT id='in_font' STYLE=\"background-color:''; font-size:22px; line-height:100%;font-weight:bold; letter-spacing:0px; \" COLOR=\"#000000\" FACE=\"Arial\">" + tipData + "</FONT></DIV>";
	//alert(divTemp);
	
	document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divTemp);
}

function InsertKeyNumbertTip1()
{	
	var divTemp = "<DIV ID=\"ID_KEYNUMBER1\" STYLE=\"position:absolute; left:200px; top:95px; z-index:0;\"><FONT STYLE=\"background-color:''; font-size:24px; line-height:100%; letter-spacing:0px; \" COLOR=\"#000000\" FACE=\"Arial Rounded MT Bold\">Check 1</FONT></DIV>";
	//alert(divTemp);
	
	document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divTemp);
}

function InsertKeyNumbertTip2()
{	
	var divTemp = "<DIV ID=\"ID_KEYNUMBER2\" STYLE=\"position:absolute; left:550px; top:95px; z-index:0;\"><FONT STYLE=\"background-color:''; font-size:24px; line-height:100%; letter-spacing:0px; \" COLOR=\"#000000\" FACE=\"Arial Rounded MT Bold\">Check 2</FONT></DIV>";
	//alert(divTemp);
	
	document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divTemp);
}

function GetEmelentSize(el)
{
	for (var lx=0,ly=0; el!=null; lx+=el.offsetLeft,ly+=el.offsetTop,el=el.offsetParent);
	return {x:lx,y:ly}
}

//�����ڵ��㺯��
function CreateLayerText(el, textData)
{
		var divTemp = "<DIV ID=\"ID_TEXT_LAYER_RETURN\" STYLE=\"position:absolute; left:50px; top:190px; z-index:0;\"><FONT STYLE=\"background-color:''; font-size:42px; line-height:100%; letter-spacing:0px; \" COLOR=\"#FF0000\" FACE=\"Arial\">TO BE RETURNED</FONT></DIV>";
	//alert(divTemp);
	
	document.getElementById("DYNAMIC_INSERT_HTML_FLAG").insertAdjacentHTML("beforeBegin", divTemp);
}

function loadXML(xmlString)
{
   var xmlDoc=null;
   //�ж������������
   //֧��IE����� 
   if(!window.DOMParser && window.ActiveXObject)
   {   //window.DOMParser �ж��Ƿ��Ƿ�ie�����
       var xmlDomVersions = ['MSXML.2.DOMDocument.6.0','MSXML.2.DOMDocument.3.0','Microsoft.XMLDOM'];
       for(var i=0;i<xmlDomVersions.length;i++)
       {
           try
           {
               xmlDoc = new ActiveXObject(xmlDomVersions[i]);
               xmlDoc.async = false;
               xmlDoc.load(xmlString); //loadXML��������xml�ַ���
               //alert( xmlString);
               //alert(xmlDoc);
               return xmlDoc;
           }
           catch(e)
           {
           }
       }
   }
    
   return null;
}