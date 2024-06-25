using Altex.Util;
using Microsoft.AspNetCore.Html;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace Altex.Utils
{
    public class GeneratorHTML
    {
        #region //*************************************************************************  GenerateMessageJQuery
        public static string GenerateMessageJQuery(string messg)
        {
            messg = messg.Replace("\n", "");
            // Формируем ответ пользователю
            StringBuilder strBld = new StringBuilder();
            strBld.Append("jQuery('#id_message').append(\"<p class='txt_msg'>");
            strBld.Append(messg);
            strBld.Append("</p>\");");
            strBld.Append("jQuery('#id_message').show();");
            strBld.Append("setTimeout( function(){ jQuery('#id_message').hide(); jQuery('#id_message').empty()}, 3000);");
            return strBld.ToString();
        }
        public static string GenerateMessageHTML(string messg)
        {
            if (String.IsNullOrEmpty(messg)) return "";

            // Формируем ответ пользователю
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<script type=\"text/javascript\">");
            strBld.Append(GenerateMessageJQuery(messg));
            strBld.Append("</script>");
            return strBld.ToString();
        }
        public static string GenerateMessageErrorJQuery(string messg)
        {
            messg = messg.Replace("\n", "");
            // Формируем ответ пользователю
            StringBuilder strBld = new StringBuilder();
            strBld.Append("jQuery('#id_error').append(\"<p class='txt_msg'>");
            strBld.Append(messg);
            strBld.Append("</p>\");");
            strBld.Append("jQuery('#id_error').show();");
            strBld.Append("setTimeout( function(){ jQuery('#id_error').hide(); jQuery('#id_error').empty();}, 10000);");
            return strBld.ToString();
        }
        public static string GenerateMessageErrorHTML(string messg)
        {
            if (String.IsNullOrEmpty(messg)) return "";

            // Формируем ответ пользователю
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<script type='text/javascript'>");
            strBld.Append(GenerateMessageErrorJQuery(messg));
            strBld.Append("</script>");
            return strBld.ToString();
        }
        public static string GenerateMessageErrorJQuery(string messg, string id_field)
        {
            messg = messg.Replace("\n", "");
            // Формируем ответ пользователю
            StringBuilder strBld = new StringBuilder();
            strBld.Append("jQuery('#id_error').append(\"<p class='txt_msg'>");
            strBld.Append(messg);
            strBld.Append("</p>\");");
            strBld.Append("jQuery('#id_error').show();");
            strBld.Append("jQuery('").Append(id_field).Append("').addClass('err_field');");

            strBld.Append("setTimeout( function(){ jQuery('#id_error').hide(); jQuery('#id_error').empty(); jQuery('").Append(id_field).Append("').removeClass('err_field');}, 10000);");
            return strBld.ToString();
        }
        public static string GenerateMessageErrorHTML(string messg, string id_field)
        {
            if (String.IsNullOrEmpty(messg)) return "";

            // Формируем ответ пользователю
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<script type='text/javascript'>");
            strBld.Append(GenerateMessageErrorJQuery(messg, id_field));
            strBld.Append("</script>");
            return strBld.ToString();
        }


        public static string GenerateMessageErrorJS(string messg, string id_field)
        {
            messg = messg.Replace("\n", "");
            // Формируем ответ пользователю
            StringBuilder strBld = new StringBuilder();
            /**/
            strBld.Append("var flderr1 = document.getElementById(\"").Append(id_field).Append("\");");
            strBld.Append("flderr1.className += ' err_field';");
            strBld.Append("var dv_err = document.getElementById('id_error');");
            strBld.Append("dv_err.textContent='").Append(messg).Append("';");
            strBld.Append("dv_err.innerText='").Append(messg).Append("';");
            strBld.Append("dv_err.style.display='block';");
            strBld.Append("setTimeout( function(){ ");
            strBld.Append("var flderr = document.getElementById(\"").Append(id_field).Append("\");");
            strBld.Append("flderr.className -= ' err_field';");
            strBld.Append("var dv_err = document.getElementById('id_error');");
            strBld.Append("dv_err.style.display = 'none';");
            strBld.Append("dv_err.textContent = '';");
            strBld.Append("dv_err.innerText = '';");
            strBld.Append("}, 10000);");

            return strBld.ToString();
        }
        public static string GenerateMessageGoToURL_JS_confirm(string messg, string url)
        {
            // Формируем ответ пользователю
            StringBuilder strBld = new StringBuilder();
            strBld.Append("if(confirm(\"").Append(messg).Append("\") ){");
            strBld.Append("window.location=\"").Append(url).Append("\";");
            strBld.Append("}");
            return strBld.ToString();
        }
        public static string GenerateMessageGoToUR_HTML(string url, bool insert_in_html)
        {
            // Формируем ответ пользователю
            StringBuilder strBld = new StringBuilder();
            if (insert_in_html) { strBld.Append("<script type=\"text/javascript\">"); }
            strBld.Append("window.location=\"").Append(url).Append("\";");
            //strBld.Append("return true;");
            if (insert_in_html) { strBld.Append("</script>"); }
            return strBld.ToString();
        }
        public static string GenerateMessageJS2HTML(string messg)
        {
            if (String.IsNullOrEmpty(messg)) return "";

            // Формируем ответ пользователю
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<script type=\"text/javascript\">");
            strBld.Append(messg);
            strBld.Append("</script>");
            return strBld.ToString();
        }

        #endregion

        #region //*************************************************************************  Filter_Column
        public static HtmlString Generate_filter_column(FilterByColumn filter_column, ref Dictionary<string, Dictionary<string, string>> dct_fields_properties)
        {
            StringBuilder filter = new StringBuilder();

            switch (dct_fields_properties[filter_column.column]["type"])
            {
                case "datetime":
                    filter.Append(filter_column_datetime(ref filter_column));
                    break;

                case "bool":
                    filter.Append(filter_column_bool(ref filter_column));
                    break;

                default:
                    filter.Append(filter_column_text(ref filter_column));
                    break;
            }

            return new HtmlString(filter.ToString());
        }

        //*******************************************
        private static string filter_column_datetime(ref FilterByColumn filter_column)
        //*******************************************
        {
            FilterDateRange filter_date_range = (FilterDateRange)filter_column.value;

            StringBuilder strBld = new StringBuilder();
            strBld.Append("<table class=\"tb_cntrl_prd\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            strBld.Append("<tr>");
            strBld.Append("<td>");

            strBld.Append( Selector_filter_column_order(filter_column.order, filter_column.column));
            strBld.Append( Selector_filter_column_period(filter_date_range.period, filter_column.column));

            strBld.Append("</td>");
            strBld.Append("</tr>");

            strBld.Append("<tr>");
            strBld.Append("<td>");

            string div_show = " style=\"display:none\"";
            if ((string)filter_column.order == "range")
                div_show = " style=\"display:block\"";

            strBld.Append("<div id=\"dv_flt_period_range\"");
            strBld.Append(div_show);
            strBld.Append(">");

            strBld.Append("<input type=\"text\"");
            strBld.Append(" id=\"inp_flt_").Append(filter_column.column).Append("_start\"");
            strBld.Append(" value=\"").Append(filter_date_range.date_range.start).Append("\"");
            strBld.Append("/>");

            strBld.Append("<input type=\"text\"");
            strBld.Append(" id=\"inp_flt_").Append(filter_column.column).Append("_finish\"");
            strBld.Append(" value=\"").Append(filter_date_range.date_range.finish).Append("\"");
            strBld.Append("/>");

            strBld.Append("</div>");

            strBld.Append("</td>");
            strBld.Append("</tr>");
            strBld.Append("</table>");

            return strBld.ToString();
        }

        //*******************************************
        private static string filter_column_bool(ref FilterByColumn filter_column)
        //*******************************************
        {
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<table class=\"tb_cntrl_prd\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            strBld.Append("<tr>");
            strBld.Append("<td>");

            strBld.Append(Selector_filter_column_bool((string)filter_column.value, filter_column.column));

            strBld.Append("</td>");
            strBld.Append("<td>");

            strBld.Append(Selector_filter_column_period(filter_column.order, filter_column.column));

            strBld.Append("</td>");
            strBld.Append("</tr>");
            strBld.Append("</table>");

            return strBld.ToString();
        }

        //*******************************************
        private static string filter_column_text(ref FilterByColumn filter_column)
        //*******************************************
        {
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<table class=\"tb_cntrl_prd\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            strBld.Append("<tr>");
            strBld.Append("<td>");

            string val_txt = "";
            switch (filter_column.type)
            {
                case "on_main_pages":
                    val_txt = Commons.join_to_string((List<string>)filter_column.value, ";");
                    break;

                default:
                    val_txt = filter_column.value.ToString();
                    break;
            }

            strBld.Append("<input type=\"text\"");
            strBld.Append(" id=\"inp_flt_").Append(filter_column.column).Append("\"");
            strBld.Append(" value=\"").Append(val_txt).Append("\"");
            strBld.Append("/>");

            strBld.Append("</td>");
            strBld.Append("<td>");

            strBld.Append(Selector_filter_column_order(filter_column.order, filter_column.column));

            strBld.Append("</td>");
            strBld.Append("</tr>");
            strBld.Append("</table>");

            return strBld.ToString();
        }
        #endregion




        #region //*************************************************************************  FILTER BY COLUMN

        //*******************************************
        public static HtmlString filter_by_column_bool(ref FilterByColumn filter_by_column)
        //*******************************************
        {
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<table class=\"tb_cntrl_prd\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            strBld.Append("<tr>");
            strBld.Append("<td>");

            strBld.Append(GeneratorHTML.Render_flt_by_column_bool((string)filter_by_column.value, filter_by_column.column));

            strBld.Append("</td>");
            strBld.Append("<td>");

            strBld.Append(GeneratorHTML.Render_flt_by_column_order(filter_by_column.order, filter_by_column.column));

            strBld.Append("</td>");
            strBld.Append("</tr>");
            strBld.Append("</table>");

            return new HtmlString(strBld.ToString());
        }

        //*******************************************
        public static HtmlString filter_by_column(ref FilterByColumn filter_by_column)
        //*******************************************
        {
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<table class=\"tb_cntrl_prd\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            strBld.Append("<tr>");
            strBld.Append("<td>");

            string val_txt = "";
            switch (filter_by_column.type)
            {
                case "on_main_pages":
                    val_txt = Commons.join_to_string((List<string>)filter_by_column.value, ";");
                    break;

                default:
                    val_txt = filter_by_column.value.ToString();
                    break;
            }

            strBld.Append("<input type=\"text\"");
            strBld.Append(" id=\"inp_flt_").Append(filter_by_column.column).Append("\"");
            strBld.Append(" value=\"").Append(val_txt).Append("\"");
            strBld.Append("/>");

            strBld.Append("</td>");
            strBld.Append("<td>");

            strBld.Append(GeneratorHTML.Render_flt_by_column_order(filter_by_column.order, filter_by_column.column));

            strBld.Append("</td>");
            strBld.Append("</tr>");
            strBld.Append("</table>");

            return new HtmlString(strBld.ToString());
        }

        //*******************************************
        public static HtmlString filter_by_column_datetime(ref FilterByColumn filter_by_column)
        //*******************************************
        {
            FilterDateRange filter_date_range = (FilterDateRange)filter_by_column.value;

            StringBuilder strBld = new StringBuilder();
            strBld.Append("<table class=\"tb_cntrl_prd\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            strBld.Append("<tr>");
            strBld.Append("<td>");

            strBld.Append(GeneratorHTML.Render_flt_by_column_order(filter_by_column.order, filter_by_column.column));
            strBld.Append(GeneratorHTML.Render_flt_by_column_period(filter_date_range.period, filter_by_column.column));

            strBld.Append("</td>");
            strBld.Append("</tr>");

            strBld.Append("<tr>");
            strBld.Append("<td>");

            string div_show = " style=\"display:none\"";
            if ((string)filter_by_column.order == "range")
                div_show = " style=\"display:block\"";

            strBld.Append("<div id=\"dv_flt_period_range\"");
            strBld.Append(div_show);
            strBld.Append(">");

            strBld.Append("<input type=\"text\"");
            strBld.Append(" id=\"inp_flt_").Append(filter_by_column.column).Append("_start\"");
            strBld.Append(" value=\"").Append(filter_date_range.date_range.start).Append("\"");
            strBld.Append("/>");

            strBld.Append("<input type=\"text\"");
            strBld.Append(" id=\"inp_flt_").Append(filter_by_column.column).Append("_finish\"");
            strBld.Append(" value=\"").Append(filter_date_range.date_range.finish).Append("\"");
            strBld.Append("/>");

            strBld.Append("</div>");

            strBld.Append("</td>");
            strBld.Append("</tr>");
            strBld.Append("</table>");

            return new HtmlString(strBld.ToString());
        }


        //*******************************************
        public static string Render_flt_by_column_order(string sel_item, string name_column)
        //*******************************************
        {
            Dictionary<string, string> sel_option = new Dictionary<string, string>();
            sel_option.Add("no_order", "нет");
            sel_option.Add("order", "возрастание");
            sel_option.Add("order_desc", "убывание");

            StringBuilder strBld = new StringBuilder();
            strBld.Append("<select");
            strBld.Append(" id=\"sl_flt_order_").Append(name_column).Append("\"");
            strBld.Append(" class=\"sl_fltbyclm_ord\"");
            //strBld.Append(" name=\"sl_ordrbyclm\"");
            //strBld.Append(" onchange=\"type_manage_upd(this)\"");
            strBld.Append(">");

            foreach (string key in sel_option.Keys)
            {
                strBld.Append("<option value=\"");
                strBld.Append(key);
                strBld.Append("\"");

                if (sel_item == key)
                {
                    strBld.Append(" selected");
                }

                strBld.Append(">");
                strBld.Append(sel_option[key]);
                strBld.Append("</option>");
            }

            strBld.Append("</select>");
            return strBld.ToString();
        }

        //*******************************************
        public static string Render_flt_by_column_period(string sel_item, string name_column)
        //*******************************************
        {
            Dictionary<string, string> sel_option = new Dictionary<string, string>();
            sel_option.Add("last", "Последние");
            sel_option.Add("day", "Сегодня");
            sel_option.Add("tomorrow", "Вчера");
            sel_option.Add("week", "Эта неделя");
            sel_option.Add("mounth", "Этот месяц");
            sel_option.Add("day7", "Последние 7 дней");
            sel_option.Add("day14", "Последние 14 дней");
            sel_option.Add("all", "Все");
            sel_option.Add("range", "Выбрать диапазон");

            StringBuilder strBld = new StringBuilder();
            strBld.Append("<select");
            strBld.Append(" id=\"sl_flt_period_").Append(name_column).Append("\"");
            strBld.Append(" class=\"sl_fltbyclm_period\"");
            //strBld.Append(" name=\"sl_stat_period\"");
            strBld.Append(" onchange=\"sl_flt_period_upd(this)\"");
            strBld.Append(">");

            foreach (string key in sel_option.Keys)
            {
                strBld.Append("<option value=\"");
                strBld.Append(key);
                strBld.Append("\"");

                if (sel_item == key)
                {
                    strBld.Append(" selected");
                }

                strBld.Append(">");
                strBld.Append(sel_option[key]);
                strBld.Append("</option>");
            }

            strBld.Append("</select>");

            return strBld.ToString();
        }

        //*******************************************
        public static string Render_flt_by_column_bool(string sel_item, string name_column)
        //*******************************************
        {
            Dictionary<string, string> sel_option = new Dictionary<string, string>();
            sel_option.Add("all", "Все");
            sel_option.Add("true", "Да");
            sel_option.Add("false", "Нет");

            StringBuilder strBld = new StringBuilder();
            strBld.Append("<select");
            strBld.Append(" id=\"inp_flt_").Append(name_column).Append("\"");
            strBld.Append(" class=\"sl_fltbyclm_bool\"");
            //strBld.Append(" name=\"sl_ordrbyclm\"");
            //strBld.Append(" onchange=\"type_manage_upd(this)\"");
            strBld.Append(">");

            foreach (string key in sel_option.Keys)
            {
                strBld.Append("<option value=\"");
                strBld.Append(key);
                strBld.Append("\"");

                if (sel_item == key)
                {
                    strBld.Append(" selected");
                }

                strBld.Append(">");
                strBld.Append(sel_option[key]);
                strBld.Append("</option>");
            }

            strBld.Append("</select>");
            return strBld.ToString();
        }

        public static HtmlString Generate_filter_column(ref Dictionary<string, FilterByColumn> dct_filters_columns, ref Dictionary<string, Dictionary<string, string>> dct_fields_properties, string name_field)
        {
            HtmlString filter_html_text = new HtmlString("");

            if (dct_filters_columns.ContainsKey(name_field))
            {
                filter_html_text = GeneratorHTML.Generate_filter_column(dct_filters_columns[name_field], ref dct_fields_properties);
            }
            return filter_html_text;
        }
        #endregion


        #region //*************************************************************************  TagHTML

        //*******************************************
        public static string Selector_filter_column_order(string sel_item, string name_column)
        //*******************************************
        {
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<select");
            strBld.Append(" id=\"sl_flt_order_").Append(name_column).Append("\"");
            strBld.Append(" class=\"sl_fltbyclm_ord\"");
            //strBld.Append(" name=\"sl_ordrbyclm\"");
            //strBld.Append(" onchange=\"type_manage_upd(this)\"");
            strBld.Append(">");

            foreach (KeyValuePair<string, string> kv in SettingsUtils.dct_order)
            {
                strBld.Append("<option value=\"");
                strBld.Append(kv.Key);
                strBld.Append("\"");

                if (sel_item == kv.Key)
                {
                    strBld.Append(" selected");
                }

                strBld.Append(">");
                strBld.Append(kv.Value);
                strBld.Append("</option>");
            }

            strBld.Append("</select>");
            return strBld.ToString();
        }

        //*******************************************
        public static string Selector_filter_column_period(string sel_item, string name_column)
        //*******************************************
        {
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<select");
            strBld.Append(" id=\"sl_flt_period_").Append(name_column).Append("\"");
            strBld.Append(" class=\"sl_fltbyclm_period\"");
            //strBld.Append(" name=\"sl_stat_period\"");
            strBld.Append(" onchange=\"sl_flt_period_upd(this)\"");
            strBld.Append(">");

            foreach (KeyValuePair<string, string> kv in SettingsUtils.dct_period)
            {
                strBld.Append("<option value=\"");
                strBld.Append(kv.Key);
                strBld.Append("\"");

                if (sel_item == kv.Key)
                {
                    strBld.Append(" selected");
                }

                strBld.Append(">");
                strBld.Append(kv.Value);
                strBld.Append("</option>");
            }

            strBld.Append("</select>");

            return strBld.ToString();
        }

        //*******************************************
        public static string Selector_filter_column_bool(string sel_item, string name_column)
        //*******************************************
        {
            StringBuilder strBld = new StringBuilder();
            strBld.Append("<select");
            strBld.Append(" id=\"inp_flt_").Append(name_column).Append("\"");
            strBld.Append(" class=\"sl_fltbyclm_bool\"");
            //strBld.Append(" name=\"sl_ordrbyclm\"");
            //strBld.Append(" onchange=\"type_manage_upd(this)\"");
            strBld.Append(">");

            foreach (KeyValuePair<string, string> kv in SettingsUtils.dct_bool)
            {
                strBld.Append("<option value=\"");
                strBld.Append(kv.Key);
                strBld.Append("\"");

                if (sel_item == kv.Key)
                {
                    strBld.Append(" selected");
                }

                strBld.Append(">");
                strBld.Append(kv.Value);
                strBld.Append("</option>");
            }

            strBld.Append("</select>");
            return strBld.ToString();
        }

        #endregion

    }
}
