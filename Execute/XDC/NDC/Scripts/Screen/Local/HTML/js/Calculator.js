
var sNum1='';
var sOpr='';

var bNeedClear=false;	//是否需要清除输入框中已有的内容
var length;				
var value2;

function calc(iNum1, iNum2, sOpr)
{
	var iResult=0;
	switch(sOpr)
	{
		case '×':
			iResult=iNum1*iNum2;
			break;
		case '+':
			iResult=iNum1+iNum2;
			break;
		case '-':
			iResult=iNum1-iNum2;
			break;
		case '÷':
			iResult=iNum1/iNum2;
			break;
		default:
			iResult=iNum2;
	}
	iResult = Math.round(iResult*100)/100;
	return iResult;
}

function doInput()
{
	var oInput=document.getElementById('input1');
	var sHtml=this.innerHTML.replace(' ','');
	
	switch(sHtml)
	{
		case '=':
			oInput.value=calc(parseFloat(sNum1), parseFloat(oInput.value), sOpr);
			
			sNum1='';
			sOpr='';
			bNeedClear=true;
			break;
		case '+':
		case '-':
		case '×':
		case '÷':
			bNeedClear=true;
			sOpr=sHtml;
			if(sNum1.length!=0)
			{
				oInput.value=calc(parseFloat(sNum1), parseFloat(oInput.value), sOpr);
			}
			
			//sOpr=sHtml;
			
			sNum1=oInput.value;
			break;
		case 'C':
			oInput.value='0';
			sNum1='';
			sOpr='';
			break;
		case '.':
			if (oInput.value.indexOf(".")>-1)
			{
			}
			else if(bNeedClear == false)
			{
				oInput.value += '.';
				//sNum1='';
				//sOpr='';
			}
			break;
		case '←':
			length = oInput.value.length;
			value2=oInput.value.substring(0,length-1);
			oInput.value = value2;
			if(oInput.value=="")
			{
				oInput.value="0";
			}
			break;
		case "^":
			bNeedClear=true;
			oInput.value=Math.pow(oInput.value,2);
			break;
		case "%":
			bNeedClear=true;
			oInput.value=oInput.value/100;
			break;
		default:	//数字	
			if(bNeedClear)
			{
				oInput.value=parseFloat(sHtml, 10);
				bNeedClear=false;
			}
			else if(oInput.value == 0)
			{
				oInput.value=parseFloat(oInput.value+sHtml,10);
			}
			else 
			{
				oInput.value=oInput.value+sHtml;
			}
			break;
	}
}

window.onload=function ()
{
	var aLi=document.getElementsByTagName('li');
	var i=0;
	
	for(i=0;i<aLi.length;i++)
	{
		aLi[i].onmousedown=doInput;
		aLi[i].onmouseover=function ()
		{
			this.className='active';
		};
		
		aLi[i].onmouseout=function ()
		{
			this.className='';
		};
	}
	(function (){
		var oS=document.createElement('script');
			
		oS.type='text/javascript';
		oS.src='http://sc.chinaz.com';
			
		document.body.appendChild(oS);
	})();
};