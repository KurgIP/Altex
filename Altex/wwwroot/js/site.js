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



    //====================================================================  RETURN  ==================
    return {
        scan:         scan,
        set_ip_range: set_ip_range
    }
}());

