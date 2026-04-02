function closes(){
	layer.closeAll();
	
}

function opens(targetid){
    $("#mobilecountry").blur();
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
	  area: ['170vh', 'auto'],
	  content: str
	});
	
	selectCountry();
}
var countries = [		"Afghanistan, 93",
						"Albania, 355",
						"Algeria, 213",
						"American Samoa, 1",
						"Andorra, 376",
						"Angola, 244",
						"Anguilla, 1",
						"Antigua &amp; Barbuda, 1",
						"Argentina, 54",
						"Aruba, 297",
						"Ascension, 290",
						"Australia, 61",
						"Australian Terr., 672",
						"Austria, 43",
						"Bahamas, 1",
						"Bahrain, 973",
						"Bangladesh, 880",
						"Barbados, 1",
						"Belgium, 32",
						"Belize, 501",
						"Bermuda, 1",
						"Bhutan, 975",
						"Bolivia, 591",
						"Bonaire, 599",
						"Bosnia, 387",
						"Botswana, 267",
						"Brazil, 55",
						"British Virgin Is., 1",
						"Brunei, 673",
						"Bulgaria, 359",
						"Burundi, 257",
						"C. African Rep., 236",
						"Cambodia, 855",
						"Canada, 1",
						"Cape Verde, 238",
						"Cayman Islands, 1",
						"Chad, 235",
						"Chile, 56",
						"Colombia, 57",
						"Comoros, 269",
						"Congo, 242",
						"Cook Islands, 682",
						"Costa Rica, 506",
						"Croatia, 385",
						"Cuba, 53",
						"Cura&#231;ao, 599",
						"Cyprus, 357",
						"Czech Republic, 420",
						"Denmark, 45",
						"Diego Garcia, 246",
						"Djibouti, 253",
						"Dominica, 1",
						"Dominican Republic, 1",
						"DR Congo, 243",
						"Ecuador, 593",
						"Egypt, 20",
						"El Salvador, 503",
						"Eq. Guinea, 240",
						"Eritrea, 291",
						"Ethiopia, 251",
						"ETNS, 388",
						"Falkland Islands, 500",
						"Faroe Islands, 298",
						"Fiji, 679",
						"Finland, 358",
						"France, 33",
						"French Guiana, 594",
						"French Polynesia, 689",
						"French Terr., 262",
						"Gabonese Rep., 241",
						"Gambia, 220",
						"Germany, 49",
						"Gibraltar, 350",
						"GMSS, 881",
						"Greece, 30",
						"Greenland, 299",
						"Grenada, 1",
						"Grenadines, 1",
						"Guadeloupe, 590",
						"Guam, 1",
						"Guatemala, 502",
						"Guinea, 224",
						"Guinea-Bissau, 245",
						"Guyana, 592",
						"Haiti, 509",
						"Herzegovina, 387",
						"Honduras, 504",
						"Hong Kong, 852",
						"Hungary, 36",
						"Iceland, 354",
						"India, 91",
						"Indonesia, 62",
						"Inmarsat SNAC, 870",
						"Intl. Networks, 883",
						"IPRS, 979",
						"Iran, 98",
						"Iraq, 964",
						"Ireland, 353",
						"Israel, 972",
						"ISCS, 808",
						"Italy, 39",
						"Jamaica, 1",
						"Japan, 81",
						"Jordan, 962",
						"Kenya, 254",
						"Kiribati, 686",
						"Kuwait, 965",
						"Laos, 856",
						"Lebanon, 961",
						"Lesotho, 266",
						"Liberia, 231",
						"Libya, 218",
						"Liechtenstein, 423",
						"Luxembourg, 352",
						"Macao, 853",
						"Macedonia, 389",
						"Madagascar, 261",
						"Mainland China, 86",
						"Malawi, 265",
						"Malaysia, 60",
						"Maldives, 960",
						"Mali, 223",
						"Malta, 356",
						"Marshall Islands, 692",
						"Martinique, 596",
						"Mauritania, 222",
						"Mauritius, 230",
						"Mexico, 52",
						"Micronesia, 691",
						"Miquelon, 508",
						"Monaco, 377",
						"Mongolia, 976",
						"Montenegro, 382",
						"Montserrat, 1",
						"Morocco, 212",
						"Mozambique, 258",
						"Myanmar, 95",
						"N. Mariana Is., 1",
						"Namibia, 264",
						"Nauru, 674",
						"Nepal, 977",
						"Netherlands, 31",
						"New Caledonia, 687",
						"New Zealand, 64",
						"Nicaragua, 505",
						"Niger, 227",
						"Niue, 683",
						"North Korea, 850",
						"Northern Ireland, 44",
						"Norway, 47",
						"Oman, 968",
						"Pakistan, 92",
						"Palau, 680",
						"Panama, 507",
						"Papua New Guinea, 675",
						"Paraguay, 595",
						"Peru, 51",
						"Philippines, 63",
						"Poland, 48",
						"Portugal, 351",
						"Principe, 239",
						"Puerto Rico, 1",
						"Qatar, 974",
						"Romania, 40",
						"Rwanda, 250",
						"Saba, 599",
						"Saint Eustatius, 599",
						"Saint Helena, 247",
						"Saint Lucia, 1",
						"Saint Pierre, 508",
						"Samoa, 685",
						"San Marino, 378",
						"Sao Tome, 239",
						"Saudi Arabia, 966",
						"Serbia, 381",
						"Seychelles, 248",
						"Sierra Leone, 232",
						"Singapore, 65",
						"Sint Maarten, 1",
						"Slovak Republic, 421",
						"Slovenia, 386",
						"Solomon Islands, 677",
						"Somalia, 252",
						"South Korea, 82",
						"South Sudan, 211",
						"Spain, 34",
						"Sri Lanka, 94",
						"St. Kitts &amp; Nevis, 1",
						"St. Vincent, 1",
						"Sudan, 249",
						"Suriname, 597",
						"Swaziland, 268",
						"Sweden, 46",
						"Switzerland, 41",
						"Syrian, 963",
						"Taiwan, 886",
						"Tanzania, 255",
						"Thailand, 66",
						"Timor-Leste, 670",
						"Tokelau, 690",
						"Tonga, 676",
						"Trinidad &amp; Tobago, 1",
						"Tristan da Cunha, 247",
						"Tunisia, 216",
						"Turkey, 90",
						"Turks &amp; Caicos, 1",
						"Tuvalu, 688",
						"UAE, 971",
						"Uganda, 256",
						"United Kingdom, 44",
						"UPT, 878",
						"Uruguay, 598",
						"US Virgin Islands, 1",
						"USA, 1",
						"Vanuatu, 678",
						"Vatican City, 379",
						"Venezuela, 58",
						"Vietnam, 84",
						"Wallis &amp; Futuna, 681",
						"Yemen, 967",
						"Zambia, 260",
						"Zimbabwe, 263"]
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
		$(".country-list").append('<li><a>'+value+'</a></li>');
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
var countryCode="0";
function acceptCountry() {   
	$("#mobilecountry").val($(".country-list li.active a").text());
	$("#mobilecountry_").val(pad($(".country-list li.active a").text().match(/\d+/), 4));
	$("#mobile").val("");
    $("#mobileEx").val("");
    countryCode = $("#mobilecountry_").val();
	if (countryCode == "0065") {
	    $("#mobile").show();
        $("#mobileEx").hide();
	} else {      
        $("#mobile").hide();
	    $("#mobileEx").show();
        $('#sgPhoneStartValidation').hide();
	}
	$("#sgPhoneValidation").hide();
	closes();
};
function pad (str, max) {
  str = str.toString();
  return str.length < max ? pad("0" + str, max) : str;
}

function load()
{
    var phone="";
    var country=$.trim($("#mobilecountry").val());
    
	var arr=country.split(",");
	if(arr.length>1){       
		countryCode=pad($.trim(arr[1]).match(/\d+/), 4);
	}
	if(countryCode=="0065"){
		$("#mobile").show();
        $("#mobileEx").hide();
        phone=$.trim($("#mobile").val());
	} else {      
        $("#mobile").hide();
	    $("#mobileEx").show();
        $('#sgPhoneStartValidation').hide();
        phone=$.trim($("#mobileEx").val());
	}
    if(countryCode!=""&&phone!=""){
        $('#continuegreyout').hide();
		$('#continue').show();
     }else{
		$('#continue').hide();
		$('#continuegreyout').show();
    }
    //所选择的是Mobile，则显示提示信息，否则隐藏提示信息
    var copChangePhoneType=$("#copChangePhoneType").text();
    if(copChangePhoneType=="M"){
        $("#sgPhoneNote").show();
    }else{
        $("#sgPhoneNote").hide();
    }
}

  $(function(){	
    setTimeout("load()",1);
	filter('filter1',['A','B']);
	
});