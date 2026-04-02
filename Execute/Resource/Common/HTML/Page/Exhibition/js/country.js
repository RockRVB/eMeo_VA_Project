
function closes(){
	layer.closeAll();
}

function opens(targetid){
	var str = $("#" + targetid).prop('outerHTML');
	
	layer.open({
	  type: 1,
	  closeBtn: 0, //不显示关闭按钮
	  /* shift: 2, */
	  title: false,
	  
	  /* time:30000,//超时关闭 */
	  shadeClose: false,
	  shade: 0.8,
	  /* offset: ['auto', 'auto'], */
	  area: ['60%', 'auto'],
	  content: str
	});
	
	selectCountry();
}

var countries = [
   "AFGHANISTAN",
   "ALBANIA",
   "ALGERIA",
   "AMERICAN SAMOA",
   "ANDORRA",
   "ANGOLA",
   "ANGUILLA",
   "ARGENTINA",
   "AUSTRALIA",
   "AUSTRIA",
   "BAHAMAS",
   "BAHRAIN",
   "BANGLADESH",
   "BARBADOS",
   "BELGIUM",
   "BERMUDA",
   "BOLIVIA",
   "BOSNIA AND HERZEGOVINA",
   "BOUVET ISLAND",
   "BRAZIL",
   "BRUNEI DARUSSALAM",
   "BULGARIA",
   "BURKINA FASO",
   "CAMBODIA",
   "CAMEROON",
   "CANADA",
   "CAYMAN ISLANDS",
   "CENTRAL AFRICAN REP",
   "CHAD",
   "CHILE",
   "COLOMBIA",
   "COMOROS",
   "CONGO",
   "COOK ISLANDS",
   "COSTA RICA",
   "COTE DIVOIRE",
   "CUBA",
   "CYPRUS",
   "DENMARK",
   "DJIBOUTI",
   "DOMINICA",
   "DOMINICAN REPUBLIC",
   "EAST TIMOR",
   "ECUADOR",
   "EGYPT",
   "EL SALVADOR",
   "FIJI",
   "FINLAND",
   "FRANCE",
   "FRENCH GUIANA",
   "GABON",
   "GAMBIA",
   "GHANA",
   "GIBRALTAR",
   "GREECE",
   "GUATEMALA",
   "GUINEA",
   "GUYANA",
   "HAITI",
   "HOLY SEE (VATICAN CITY STATE)",
   "HONDURAS",
   "HONG KONG",
   "HUNGARY",
   "ICELAND",
   "INDIA",
   "INDONESIA",
   "IRAN",
   "IRAQ",
   "IRELAND",
   "ISRAEL",
   "ITALY",
   "JAMAICA",
   "JAPAN",
   "JORDAN",
   "KENYA",
   "KUWAIT",
   "LAOS",
   "LEBANON",
   "LESOTHO",
   "LIBERIA",
   "LIBYA",
   "LIECHTENSTEIN",
   "LITHUANIA",
   "MACAU",
   "MADAGASCAR",
   "MAINLAND CHINA",
   "MALAWI",
   "MALAYSIA",
   "MALI",
   "MALTA",
   "MARTINIQUE",
   "MAURITANIA",
   "MAURITIUS",
   "MEXICO",
   "MONACO",
   "MONGOLIA",
   "MONTSERRAT",
   "MOROCCO",
   "MOZAMBIQUE",
   "MYANMAR",
   "NAURU",
   "NEPAL",
   "NETHERLANDS",
   "NETHERLANDS ANTILLES",
   "NEW CALEDONIA",
   "NEW ZEALAND",
   "NICARAGUA",
   "NIGERIA",
   "NIUE",
   "NORFOLK ISLAND",
   "NORTH KOREA",
   "NORWAY",
   "OMAN",
   "PAKISTAN",
   "PAPUA NEW GUINEA",
   "PARAGUAY",
   "PERU",
   "PHILIPPINES",
   "PITCAIRN",
   "POLAND",
   "PORTUGAL",
   "PUERTO RICO",
   "QATAR",
   "ROMANIA",
   "RWANDA",
   "SAINT VINCENT AND THE GRENADINES",
   "SAMOA",
   "SAN MARINO",
   "SAUDI ARABIA",
   "SENEGAL",
   "SIERRA LEONE",
   "SINGAPORE",
   "SLOVAKIA",
   "SOMALIA",
   "SOUTH AFRICA",
   "SOUTH KOREA",
   "SPAIN",
   "SRI LANKA",
   "ST PIERRE (MIQEULON)",
   "SURINAME",
   "SVALBARD AND JAN MAYEN ISLANDS",
   "SWAZILAND",
   "SWEDEN",
   "SWITZERLAND",
   "SYRIAN ARAB REPUBLIC",
   "TAJIKISTAN",
   "TANZANIA (UNITED REPUBLIC)",
   "THAILAND",
   "THE DEMOCRATIC REPUBLIC OF THE CONGO",
   "TONGA",
   "TUNISIA",
   "TURKEY",
   "TURKS AND CAICOS ISLANDS",
   "TUVALU",
   "UAE",
   "UGANDA",
   "UNITED KINGDOM",
   "UNITED STATES",
   "URUGUAY",
   "UZBEKISTAN",
   "VENEZUELA",
   "VIETNAM",
   "VIRGIN ISLANDS (U.S)",
   "YUGOSLAVIA",
   "ZAMBIA",
   "ZIMBABWE (RHODESIA)"
]

function filter(id,filterList){
	
	var filterCountries = countries.filter(function (item) {
		var isFilter = false;
		
		$.each(filterList, function (index, value) {
			if(item.indexOf(value) == 0)
				isFilter = true;
		});
		
		return isFilter;
	});
	
	$(".country-list").empty();
	
	$.each(filterCountries, function (index, value) {
		$(".country-list").append('<li><img src="media/country/'+value+'.png"/><a>'+value+'</a></li>');
	});
	
	$(".filter").removeClass("active");
	$("." + id).addClass("active");
	
	selectCountry();
}

function selectCountry(){
	$(".country-list li").click(function () {
		//$("#country").val($(this).text());
		//closes();
		$(".country-list li").removeClass("active");
		$(this).addClass("active");
	});
}

function acceptCountry() {

	if($(".country-list li.active a").text()){
		$("#country").val($(".country-list li.active a").text());
	}

	if($("#country").val() != "SINGAPORE") {
		$('#postcodeinput,#blkinput,#unitinput').hide();
		$('#postalcode,#blocknum,#floor,#unit').val("");
	}
	else {
		$('#postcodeinput,#blkinput,#unitinput').show();
	}
	closes();
};

	$(function () {
	    filter('filter1', ['A', 'B']);
	    setTimeout("load()", 1);
	});

function load() {
    if($("#country").val() != "SINGAPORE") {
		$('#postcodeinput,#blkinput,#unitinput').hide();
		$('#postalcode,#blocknum,#floor,#unit').val("");
	}
	else {
		$('#postcodeinput,#blkinput,#unitinput').show();
	}
}