var _scaner = (function () {
    // Регулярное выражение для валидации IP адреса
    var rgx_ip = new RegExp("^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})$", "ig");

    function scan() {
        // Получаем значение IP старта диапазона для сканирования
        let ip_start = jQuery("#ip_start").val();
        ip_start     = ip_start.trim();

        // Проверка на соответствие значения поля IP адресу
        if ( !is_ip(ip_start) ) {
            // Выводим ошибку, что текст не IP
            _common.GenerateMessageError("Текст не соответствует шаблону IP адреса", "#ip_start");
            return;
        }

        // Проверку значения конца IP диапазона производим
        // Если установлен флаг, что задан диапазон IP
        let is_ip_range = jQuery('#is_ip_range').is(':checked');
        let ip_finish   = "";
        if ( is_ip_range )
        {
            // Получаем значение IP старта диапазона для сканирования
            ip_finish = jQuery("#ip_finish").val();
            ip_finish = ip_finish.trim();

            // Проверка на соответствие значения поля IP адресу
            if ( !is_ip(ip_finish) ) {
                // Выводим ошибку, что текст не IP
                _common.GenerateMessageError("Текст не соответствует шаблону IP адреса", "#ip_finish");
                return;
            }
        }

        let data   = {};
        data["ip_start"]    = ip_start;
        data["ip_finish"]   = ip_finish;
        data["is_ip_range"] = is_ip_range;

        _common.ajax_query_html("/Scaner/aj_scanning", data, "#cntnr_result_scan", "append");
    }
    function set_ip_range() {
        // 
        if (jQuery('#is_ip_range').is(':checked')) {
            // Включаем доступность поля конца диапазона IP
            jQuery("#ip_finish").prop( 'disabled', false );
        } else {
            // Блокируем доступность поля конца диапазона IP
            jQuery("#ip_finish").prop( 'disabled', true );
        }
    }
    function is_ip(ip_text) {
        if (ip_text == undefined)
            return false;

        ip_text = ip_text.trim();
        if (ip_text == "")
            return false;


        // Если мы применяем одно и то же регулярное выражение последовательно к разным строкам,
        // это может привести к неверному результату, поскольку вызов regexp.test обновляет свойство regexp.lastIndex,
        // поэтому поиск в новой строке может начаться с ненулевой позиции.
        rgx_ip.lastIndex = 0;
        let res_ip = rgx_ip.test(ip_text);

        // Проверка текста из поля на значение IP
        if (!res_ip)
            return false;

        // Проверяем, что бы в IP адресе цифры должны быть 0-255
        let arr_ip_quarter = ip_text.split(".");

        for (let i = 0; i < 4; i++) {
            let ip_quarter = Number( arr_ip_quarter[i] );
            if ( ip_quarter > 255 ) {
                // Цифра больше 255, а это ошибка в описании IP адреса
                return false;
            }
        }

        return true;
    }


    //********************************************************** PAGING  
    function ipg(n) { jQuery(n).parent().addClass("pg_sl"); }
    function opg(n) { jQuery(n).parent().removeClass("pg_sl"); }
    function pgn(n) {
        let data = {};
        data["currpg"]  = n;
        data["numonpg"] = jQuery("#sl_paginat_nmb_pg").val();
        _common.ajax_query_html("/Scaner/get_list_by_pagination", data, "#dv_ip_scan_result", "empty_append");
    }
    function pgn_nmb_pg() {
        let data = {};
        data["currpg"]  = -111;
        data["numonpg"] = jQuery("#sl_paginat_nmb_pg").val();
        _common.ajax_query_html("/Scaner/get_list_by_pagination", data, "#dv_ip_scan_result", "empty_append");
    }


    //********************************************************** Collaps column 
    function iclps(n) { jQuery(n).addClass("clps_sl"); }
    function oclps(n) { jQuery(n).removeClass("clps_sl"); }
    function clk_clps(num, clm_nm)
    //**********************
    {
        // Скрываем все td с атрибутом clps = n
        jQuery("td[clps='" + num + "']").hide();
        // Тип данных в таблице
        let list_type = jQuery("#tb_list").attr("tp");

        let data = {};
        data["tp"] = list_type;
        data["n"]  = clm_nm;

        switch (list_type) {
            case "IPs":
                _common.ajax_query_js( "/Scaner/aj_set_collaps_column", data );
                break;

            default:
                break;
        }
    }


    function ibtn(n) { jQuery(n).addClass("bt_sl"); }
    function obtn(n) { jQuery(n).removeClass("bt_sl"); }
    function clps_pnl_show()
    //**********************
    {
        // Тип данных в таблице
        let list_type = jQuery("#tb_list").attr("tp");

        let data = {};
        data["tp"] = list_type;

        switch (list_type) {
            case "IPs":
                _common.ajax_query_html("/Scaner/aj_set_collaps_columns_panel_show", data, "body", "append");
                break;

            default:
                break;
        }
    }



    function iclpspnl(n) { jQuery(n).addClass("td_pnlclps_sl"); }
    function oclpspnl(n) { jQuery(n).removeClass("td_pnlclps_sl"); }
    function clk_clpspnl(numb, name)
    //**********************
    {
        // Проверка скрыта ли эта колонка
        if (jQuery("#td_pnlclps_" + name).hasClass("td_pnlclps_hd")) {
            jQuery("#td_pnlclps_" + name).addClass("td_pnlclps_shw").removeClass("td_pnlclps_hd");
            jQuery("td[clps='" + numb + "']").show();
        }
        else {
            jQuery("#td_pnlclps_" + name).addClass("td_pnlclps_hd").removeClass("td_pnlclps_shw");
            jQuery("td[clps='" + numb + "']").hide();
        }
    }
    function pnl_clps_close() {
        // Удаляем панель
        jQuery("#dv_pnlclps").remove();
    }
    function pnlclps_sv() {
        // Массив всех закрытых колонок. td_pnlclps_name
        let arr_clm_hd       = jQuery("#tb_pnlclps").find(".td_pnlclps_hd");
        let list_clm_name_hd = "";
        for (let i = 0; i < arr_clm_hd.length; i++) {
            let clm_id      = jQuery(arr_clm_hd[i]).attr("id");
            let clm_name     = clm_id.substring(11);
            list_clm_name_hd = list_clm_name_hd + clm_name + ",";
        }
        // Тип данных в таблице
        let list_type = jQuery("#tb_list").attr("tp");

        let data = {};
        data["tp"]    = list_type;
        data["clmhd"] = list_clm_name_hd;

        switch (list_type) {
            case "IPs":
                _common.ajax_query_js_callback("/Scaner/aj_set_collaps_columns_panel_close", data, pnl_clps_close);
                break;

            default:
                break;
        }


    }

    function ibtmrw(n) { jQuery(n).addClass("bt_sz_m_sl"); }
    function obtmrw(n) { jQuery(n).removeClass("bt_sz_m_sl"); }


    function ipnlcls(n) { jQuery(n).addClass("dv_pnlcls_exit_sl"); }
    function opnlcls(n) { jQuery(n).removeClass("dv_pnlcls_exit_sl"); }
    function pnlcls(t) {
        jQuery(t).parents("div[pnl]").remove();
    }


    //********************************************************** FILTER BY COLUMN   
    function ihdrfltr(n) { jQuery(n).addClass("clmfltr_sl"); }
    function ohdrfltr(n) { jQuery(n).removeClass("clmfltr_sl"); }
    function clk_hdrfltr(t, n) {
        let data = {};
        data["nm"] = n;

        let list_type = jQuery("#tb_list").attr("tp"); // 
        switch (list_type) {
            case "IPs":
                _common.ajax_query_html("/Scaner/aj_set_filter_by_column", data, "#dv_flt_" + n, "empty_append");
                break;

            default:
                break;
        }
    }
    function fltr_by_clm() { // Какие столбцы выбраны
        let data = _common.get_prms_flts_clms("#tr_hdr_flt");

        let list_type = jQuery("#tb_list").attr("tp"); // 
        switch (list_type) {
            case "IPs":
                _common.ajax_query_html("/Scaner/aj_get_list_by_filters_column", data, "#dv_ip_scan_result", "empty_append");
                break;

            default:
                break;
        }
    }

    
    function ip_del(n) { }

    //====================================================================  RETURN  ==================
    return {
        scan:         scan,
        set_ip_range: set_ip_range,

        ipg:        ipg,
        opg:        opg,
        pgn:        pgn,
        pgn_nmb_pg: pgn_nmb_pg,
        
        iclps:         iclps,
        oclps:         oclps,
        clk_clps:      clk_clps,
        ibtn:          ibtn,
        obtn:          obtn,
        clps_pnl_show: clps_pnl_show,

        iclpspnl:      iclpspnl,
        oclpspnl:      oclpspnl,
        clk_clpspnl:   clk_clpspnl,
        pnlclps_sv:    pnlclps_sv,
        pnlcls:        pnlcls,
        ipnlcls:       ipnlcls,
        opnlcls:       opnlcls,

        ibtmrw:      ibtmrw,
        obtmrw:      obtmrw,
        ihdrfltr:    ihdrfltr,
        ohdrfltr:    ohdrfltr,
        clk_hdrfltr: clk_hdrfltr,
        fltr_by_clm: fltr_by_clm,

        ip_del: ip_del
    }
}());

