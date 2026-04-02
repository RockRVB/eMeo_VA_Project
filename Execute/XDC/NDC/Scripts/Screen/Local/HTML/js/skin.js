require(['config/config', 'main'], function() {
	require(['jquery'], function() {
		$(document).ready(function() {
			//文字颜色的统一
			//功能模块
			$("ul.menu_lis li").addClass("businessColor");
			$(".submenu_lis li").addClass("businessColor");
			$(".dialog_prompt .dialog_prompt_text").addClass("subColor");

			//优先级问题
			$(".index_top .index_top_title .language_select").addClass("subColor");
			$(".index_top .index_top_title .language_select div").addClass("stepTitleColor");
			$(".index_top .index_top_title .language_select.cn div:nth-child(1)").addClass("subColor");
			$(".index_top .index_top_title .language_select.en div:nth-child(2)").addClass("subColor");
			$(".business_container .transaction_result").addClass("subColor");
			$(".business_container .transaction_result .count_res_contain").addClass("subColor");
			$(".business_container .input_acount_contain").addClass("subColor");
			$(".card_input_container").addClass("subColor");
			$(".job_container").addClass("subColor");

			//时间
			$(".index_top .index_top_title .nowdate").addClass("subColor");
			$(".count_down_time").addClass("subColor");

			//英文
			$(".index_top .index_top_title .language_select div").addClass("stepTitleColor");

			//中文
			$(".index_top .index_top_title .language_select.cn div:nth-child(1)").addClass("subColor").removeClass("stepTitleColor");

			//主体部分
			$(".ibank_container").addClass('subColor');

			//身份验证失败页面的高亮字体
			$(".authenticationFailed_tip span").addClass("focusColor");
			//点钞结果高亮字体
			$(".transaction_result_tip span").addClass("focusColor");

			//步骤
			$(".card_container .card_container_title ul li.step_current div").addClass("focusColor");
			$(".card_container .card_container_title ul li.step_finish div").addClass("focusColor");
			$(".card_container .card_container_title ul li").addClass("stepTitleColor");
			$(".card_container .card_container_title ul li em").addClass("stepColor");

			//按钮
			$(".button_container_footer").addClass("subColor");
			$(".button_container").addClass("subColor");
			$(".button_container .cancel_button").addClass("focusColor");

			//错误提示
			$(".notmatch_tip").addClass("errorTipColor");
			$(".error_tips").addClass("errorTipColor");
			$(".business_container .machine_panel.password_panel .password_panel_tip em").addClass("errorTipColor");

			//大标题
			$(".business_title").addClass("subColor");

			//录入信息
			$(".card_input_container .card_input_container_group .card_input_con .card_input_con_group input").addClass("subColor");
			$(".card_input_container .card_input_container_group .card_input_con .card_input_con_group div").addClass("businessColor");

			//生成账户
			$(".generateaccount_info span").addClass("focusColor");

			//放入钞票
			$(".put_in_money_title span").addClass("focusColor");

			//拍照
			$(".black_bg_title").addClass("subColor");
			$(".black_bg_title span").addClass("focusColor");

			//无卡存款——确认您的存款信息
			$(".business_container .transaction_result .count_res_contain span").addClass("focusColor");

			//查询业务——当前账户资产状况
			$(".business_container .transaction_result .transaction_res_contain span").addClass("focusColor");

			//账户交易详情
			$(".transaction_details_contain .transaction_details_con_head div").addClass("moneyColor");
			$(".transaction_details_contain .transaction_details_con_head div").eq(0).addClass("focusColor").siblings().addClass("businessColor");

			$(".transaction_details_contain .transaction_details_con_body").addClass("subColor");
			$(".transaction_details_contain .transaction_details_con_body .transac_detail_th").addClass("businessColor");
			$(".transaction_details_contain .transaction_details_con_body .transac_detail_tb .transac_detail_tr table").addClass("subColor");

			$(".date_select_container .date_container_main .date_select_group .date_select_group_tit").addClass("subColor");
			$(".date_select_container .date_container_main .date_select_group .date_select_group_con").addClass("subColor");
			$(".date_select_container .date_container_main .date_button_group div").addClass("subColor");
			$(".date_select_group_tit").addClass("subColor");
			$(".date_button_group").addClass("subColor");
			//选择时间的文字颜色
			$(".date_select_container .date_container_main .date_select_group .date_select_group_con .mbsc-mobiscroll-dark .dwfl .dw-li.dw-v.dw-sel").addClass("subColor");

			//请输入或者选择取款金额
			$(".money_panel .money_panel_lis li div").addClass("moneyColor");
			$(".other_amount_countain span").addClass("subColor");
			$(".other_amount_countain input").addClass("inputColor");
		})
	})
});