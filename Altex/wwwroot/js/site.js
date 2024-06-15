var _scaner = (function () {
    // Регулярное выражение для валидации IP адреса
    var rgx_ip = new RegExp("\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}", "i");

    function scan() {
        // Получаем значение IP для сканирования
        let ip_text = jQuery("#tx_ip").val();

        // Проверка на соответствие значения поля IP адресу
        if (!is_ip(ip_text)) {
            // Выводим ошибку, что текст не IP
            _common.GenerateMessageError("Текст не соответствует шаблону IP адреса", "#tx_ip")
        }

        let data   = {};
        data["ip"] = ip_text;

        _common.ajax_query_html("/Scaner/scanning", data, "#cntnr_result_scan", "empty_append");
    }

    function is_ip(ip_text) {
        if (ip_text == undefined) return false;

        ip_text = ip_text.trim();
        if ( ip_text == "" ) return false;

        // Проверка текста из поля на значение IP
        if ( rgx_ip.test(ip_text) )
            return true;

        return false;
    }




    //====================================================================  RETURN  ==================
    return {
        scan: scan     
    }
}());

