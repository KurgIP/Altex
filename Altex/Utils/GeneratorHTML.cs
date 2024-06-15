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

    }
}
