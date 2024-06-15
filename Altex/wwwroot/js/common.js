// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
var _common = (function () {
    //====================================================================  AJAX  ==================
    function ajax_query_html(url, out_data, targ_elm, chng) {
        do_ajax(1);
        jQuery.ajax({
            type: "POST",
            url: url,
            global: false,
            data: out_data,
            dataType: "html",
            error: function (msg, textStatus, errorThrown) {
                if (msg.status == 307) {
                    eval(msg.responseText);
                }
                do_ajax(0);
            },
            success: function (data, textStatus) {
                insert_html(data, targ_elm, chng);
                do_ajax(0);
            }
        });
    }
    function insert_html(data, targ_elm, chng) {
        switch (chng) {
            case "replaceWith":
                jQuery(targ_elm).replaceWith(data);
                break;
            case "append":
                jQuery(targ_elm).append(data);
                break;
            case "appendto":
                jQuery(targ_elm).appendTo(data);
                break;
            case "prepend":
                jQuery(targ_elm).prepend(data);
                break;
            case "after":
                jQuery(targ_elm).after(data);
                break;
            case "before":
                jQuery(targ_elm).before(data);
                break;
            case "remove":
                jQuery(targ_elm).remove();
                break;
            case "message":
            case "error":
                jQuery(targ_elm).append(data);
                break;
            case "empty_append":
            default:
                jQuery(targ_elm).empty().append(data);
                break;
        }
    }
    function ajax_query_js(url, out_data) {
        do_ajax(1);
        jQuery.ajax({
            type: "POST",
            url: url,
            global: false,
            data: (out_data),
            dataType: "script",
            error: function (msg, textStatus, errorThrown) { do_ajax(0); },
            success: function (datas, textStatus) { do_ajax(0); }
        });
    }
    function ajax_query_html_callback(url, out_data, targ_elm, chng, calback_sucsess) {
        do_ajax(1);
        jQuery.ajax({
            type: "POST",
            url: url,
            global: false,
            data: out_data,
            dataType: "html",
            error: function (msg, textStatus, errorThrown, responseText) {
                if (msg.status == 307) {
                    eval(msg.responseText);
                }
                do_ajax(0);
            },
            success: function (data, Status, todo) {
                insert_html(data, targ_elm, chng);
                if (calback_sucsess != undefined) {
                    calback_sucsess();
                }
                do_ajax(0);
            }
        });
    }
    function ajax_query_js_callback(url, out_data, calback_sucsess) {
        do_ajax(1);
        jQuery.ajax({
            type: "POST",
            url: url,
            global: false,
            data: (out_data),
            dataType: "script",
            error: function (msg, textStatus, errorThrown) {
                if (msg.status == 307) {
                    eval(msg.responseText);
                }
                do_ajax(0);
            },
            success: function (datas, textStatus) {
                if (calback_sucsess != undefined) {
                    calback_sucsess();
                }
                do_ajax(0);
            }
        });
    }
    function get_dct_id_val(n) {
        do_ajax(1);
        var root_elm = jQuery(n);
        var data = {};
        // находим все текстовые поля
        var arr_txt = root_elm.find("input:text");
        // переводим в словарь значений arr_txt
        data = convert_to_dict(data, arr_txt);

        // находим все скрытые поля
        var arr_txt = root_elm.find("input:hidden");
        // переводим в словарь значений  arr_txt
        data = convert_to_dict(data, arr_txt);

        // находим селекты
        var arr_select = root_elm.find("select");
        // переводим в словарь значений arr_txt
        data = convert_to_dict_select(data, arr_select);

        // находим checkbox
        var arr_checkbox = root_elm.find("input:checkbox");
        // переводим в словарь значений arr_txt
        data = convert_to_dict_checkbox(data, arr_checkbox);

        // находим тексты
        var arr_textarea = root_elm.find("textarea");

        // переводим в словарь значений arr_txt
        data = convert_to_dict(data, arr_textarea);

        // находим специально помеченный div, что там данные
        var arr_div = root_elm.find("div[isvl]");
        // переводим в словарь значений arr_txt
        data = convert_to_dict_div_isvl(data, arr_div);

        return data;
    }
    function convert_to_dict(hsh, arr) {
        // переводим в словарь значений  arr_txt
        var len = arr.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr[i]).val();
            var id = arr[i].id;
            id = id.replace(/^in_/i, '');
            // оставляем только ключ колонки
            hsh[id] = vl;
        }
        return hsh;
    }
    function convert_to_dict_select(hsh, arr) {
        // переводим в словарь значений  arr_txt
        var len = arr.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr[i]).val();
            var id = arr[i].id;
            id = id.replace(/^sl_/i, '');
            // оставляем только ключ колонки
            hsh[id] = vl;
        }
        return hsh;
    }
    function convert_to_dict_checkbox(hsh, arr) {
        // переводим в словарь значений arr_txt
        var len = arr.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr[i]).is(':checked');
            var id = arr[i].id;
            // оставляем только ключ колонки
            hsh[id] = vl;
        }
        return hsh;
    }
    function convert_to_dict_div_isvl(hsh, arr) {
        // переводим в словарь значений  arr_txt
        var len = arr.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr[i]).text();
            var id = arr[i].id;
            // оставляем только ключ колонки
            hsh[id] = vl;
        }
        return hsh;
    }
    function do_ajax(n) {
        if (n == 1) { jQuery("#do_ajax").show(); }
        else { jQuery("#do_ajax").hide(); }
    }
    function ajax_query_json(url, out_data, targ_elm, chng) {
        do_ajax(1);
        jQuery.ajax({
            type: "POST",
            url: url,
            global: false,
            data: out_data,
            dataType: "json",
            error: function (msg, textStatus, errorThrown) { alert(msg.responseText); do_ajax(0); },
            success: function (answ, textStatus) {
                if (answ.message == "error") {
                    insert_html(answ.data, "body", answ.message);
                }
                else {
                    insert_html(answ.data, targ_elm, chng);
                }
                do_ajax(0);
            }
        });
    }
    function get_tr_prm(n, grp) {
        // находим все текстовые импуты
        var arr_tx = jQuery("#tr_" + n + ">td").find("input:text");
        // переводим в словарь значений text field
        var sel_tx = {};
        sel_tx["key"] = n;
        var len = arr_tx.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr_tx[i]).val();
            var id = arr_tx[i].id;
            // оставляем только имя колонки
            id = id.replace(/^in_/i, '');
            id = id.replace("_" + n, "");
            if (grp != undefined) { id = grp + "[" + id + "]" };
            sel_tx[id] = vl;
        }

        // переводим в словарь значений textarea
        var arr_txarea = jQuery("#tr_" + n + ">td").find("textarea");
        len = arr_txarea.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr_txarea[i]).val();
            var id = arr_txarea[i].id;
            // оставляем только имя колонки
            id = id.replace(/^ta_/i, '');
            id = id.replace("_" + n, "");
            if (grp != undefined) { id = grp + "[" + id + "]" };
            sel_tx[id] = vl;
        }

        // переводим в словарь значений checkbox
        var arr_chkbx = jQuery("#tr_" + n + ">td").find("input:checkbox");
        len = arr_chkbx.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr_chkbx[i]).is(':checked');
            var id = arr_chkbx[i].id;
            // оставляем только имя колонки
            id = id.replace(/^chbx_/i, '');
            id = id.replace("_" + n, "");
            if (grp != undefined) { id = grp + "[" + id + "]" };
            sel_tx[id] = vl;
        }

        // переводим в словарь значений select
        var arr_slct = jQuery("#tr_" + n + ">td").find("select");
        len = arr_slct.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr_slct[i]).val();
            var id = arr_slct[i].id;
            // оставляем только имя колонки
            id = id.replace(/^sl_/i, '');
            id = id.replace("_" + n, "");
            if (grp != undefined) { id = grp + "[" + id + "]" };
            sel_tx[id] = vl;
        }

        return sel_tx;
    }
    function get_prms_flts_clms(n) {
        var sel_tx = {};
        sel_tx["key"] = n;

        // находим все текстовые импуты
        var arr_tx = jQuery(n + ">td").find("input:text");
        var len = arr_tx.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr_tx[i]).val();
            var id = arr_tx[i].id;
            // оставляем только имя колонки
            id = id.replace("inp_flt_", "");
            sel_tx["fltclm_" + id] = vl;
        }

        // переводим в словарь значений textarea
        var arr_txarea = jQuery(n + ">td").find("textarea");
        len = arr_txarea.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr_txarea[i]).val();
            var id = arr_txarea[i].id;
            // оставляем только имя колонки
            id = id.replace("inp_flt_", "");
            sel_tx["fltclm_" + id] = vl;
        }

        // переводим в словарь значений checkbox
        var arr_chkbx = jQuery(n + ">td").find("input:checkbox");
        len = arr_chkbx.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr_chkbx[i]).is(':checked');
            var id = arr_chkbx[i].id;
            // оставляем только имя колонки
            id = id.replace("inp_flt_", "");
            sel_tx["fltclm_" + id] = vl;
        }

        // переводим в словарь значений select
        var arr_slct = jQuery(n + ">td").find("select");
        len = arr_slct.length;
        for (var i = 0; i < len; i++) {
            var vl = jQuery(arr_slct[i]).val();
            var id = arr_slct[i].id;
            // оставляем только имя колонки  sl_flt_order_
            id = id.replace("sl_flt_", "");
            sel_tx["fltclm_" + id] = vl;
        }

        return sel_tx;
    }

    function GenerateMessage(message) {
        // Выводим сообщение
        jQuery('#id_message').append("<p class='txt_msg'>" + message + "</p>").show();
        // Закрываем сообщение и очищаем от него контейнер
        setTimeout(function () { jQuery('#id_message').hide().empty(); }, 10000);
    }
    function GenerateMessageError(message) {
        // Выводим сообщение
        jQuery('#id_error').append("<p class='txt_msg'>" + message + "</p>").show();
        // Закрываем сообщение и очищаем от него контейнер
        setTimeout(function () { jQuery('#id_error').hide().empty(); }, 10000);
    }
    function GenerateMessageError(message, id_field) {
        // Выводим сообщение
        jQuery('#id_error').append("<p class='txt_msg'>" + message + "</p>").show();

        // Выделяем поле с ошибкой
        jQuery(id_field).addClass('err_field');

        // Закрываем сообщение и очищаем от него контейнер
        setTimeout(function () { jQuery('#id_error').hide().empty(); jQuery(id_field).removeClass('err_field'); }, 10000);
    }
  
    //====================================================================  RETURN  ==================
    return {
        ajax_query_html:          ajax_query_html,
        ajax_query_js:            ajax_query_js,
        get_dct_id_val:           get_dct_id_val,
        get_tr_prm:               get_tr_prm,
        ajax_query_json:          ajax_query_json,
        get_prms_flts_clms:       get_prms_flts_clms,
        ajax_query_html_callback: ajax_query_html_callback,
        ajax_query_js_callback:   ajax_query_js_callback,

        GenerateMessage:          GenerateMessage,
        GenerateMessageError:     GenerateMessageError
    }

}());


jQuery.expr[":"].exact = jQuery.expr.createPseudo(function (arg) {
    return function (element) {
        return jQuery(element).text().trim() === arg.trim();
    };
});

jQuery.fn.exists = function () {
    return $(this).length;
}
