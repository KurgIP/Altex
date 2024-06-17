using Altex;
using Altex.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Altex.Data;
using Altex.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Text.RegularExpressions;

namespace Altex.Controllers
{

    public abstract class ControllerExpand : Controller
    {
        private readonly ILogger<Controller>     _logger;
        private readonly HttpContext             _httpContext;

        public ControllerExpand( ILogger<Controller> logger, IHttpContextAccessor httpContext )
        {
            _logger      = logger;
            _httpContext = httpContext.HttpContext;
        }

        #region //=========================================== convert =============================
        public string convert_error_message_to_js_answer_with_errfield( string error )
        {
            // В ответе устанавливает код ошибки, что бы при обработке на клиенте обработка ответа перенаправилась на другой код
            // Ответ с обработкой ошибки всегда должен быть javascript, так как вставляется в DOM по eval(javascript)

            // Перезначение статус кода ответа на ajax запрос, указывает, что при обработке запроса произошла ошибка.
            // И ответ должен обрабатываться на клиенте в ветке для обработки ошибок
            _httpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.TemporaryRedirect; //307

            // Сообщение с ошибкой может содержать определение поля, значение которого валидировалось. 
            // Пример текста ошибки с полем: "Error! Параметр отсутствует.IDFIELD:#id_txt_field"
            // Если поле обозначено каким-либо jQuery селектором, то к сообщению об ошибке добавляется и выделение поля с ошибкой

            string[] arr_error = error.Split(new string[] { "IDFIELD:" }, StringSplitOptions.None);

            string answer = "";
            if (arr_error.Length == 2)
                answer = GeneratorHTML.GenerateMessageErrorJQuery(arr_error[0], arr_error[1]);
            else
                answer = GeneratorHTML.GenerateMessageErrorJQuery(error);

            return answer;
        }

        public string js_response_to_html_request(string js_code)
        {
            // В ответе устанавливает код ошибки, что бы при обработке на клиенте обработка ответа перенаправилась на другой код
            // Ответ с обработкой ошибки всегда должен быть javascript, так как вставляется в DOM по eval(javascript)

            // Перезначение статус кода ответа на ajax запрос, определяет, что при обработке запроса произошла ошибка.
            // И ответ должен обрабатываться на клиенте в ветке для обработки ошибок
            _httpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.TemporaryRedirect; //307

            return js_code;
        }
        #endregion


        #region //=========================================== check_input =============================
        public bool check_input_long( ref long result_param_long, object val_param, string place, bool is_req )
        {
            if (val_param == null)
            {
                _logger.LogWarning("Input param is Null.");
                return true;
            }

            string param_long_txt = val_param.ToString().Trim();

            if (!Int64.TryParse(param_long_txt, out result_param_long))
            {
                _logger.LogWarning("Input data is not long:[{val_param.ToString()}];", new string[] { place, _httpContext.User.Identity.Name });
                return true;
            }

            return false;
        }
        public bool check_input_int(ref int result_param_int, object val_param, string place, bool is_req)
        {
            if (val_param == null)
            {
                _logger.LogWarning("Input param is Null.");
                return true;
            }

            string param_int_txt = val_param.ToString().Trim();

            if (!Int32.TryParse(param_int_txt, out result_param_int))
            {
                _logger.LogWarning("Input param is not int:[{val_param.ToString()}];", new string[] { place, _httpContext.User.Identity.Name });
                return true;
            }

            return false;
        }
        public bool check_input_short(ref short result_param_short, object val_param, string place, bool is_req)
        {
            if (val_param == null)
            {
                _logger.LogWarning("Input param is Null.");
                return true;
            }

            string param_short_txt = val_param.ToString().Trim();

            if (!Int16.TryParse(param_short_txt, out result_param_short))
            {
                _logger.LogWarning("Input param is not short:[{val_param.ToString()}];", new string[] { place, _httpContext.User.Identity.Name });
                return true;
            }

            return false;
        }
        public bool check_input_bool(ref bool result_param_bool, object val_param, string place, bool is_req)
        {
            if (val_param == null)
            {
                _logger.LogWarning("Input param is Null.");
                return true;
            }

            string param_bool_txt = val_param.ToString().Trim();

            if (!Boolean.TryParse(param_bool_txt, out result_param_bool))
            {
                _logger.LogWarning("Input param is not bool:[{val_param.ToString()}];", new string[] { place, _httpContext.User.Identity.Name });
                return true;
            }

            return false;
        }
        public bool check_input_datetime(ref DateTime result_param_datetime, object val_param, string place, bool is_req)
        {
            if (val_param == null)
            {
                _logger.LogWarning("Input param is Null.");
                return true;
            }

            string param_datetime_txt = val_param.ToString().Trim();

            if (!DateTime.TryParse(param_datetime_txt, out result_param_datetime))
            {
                _logger.LogWarning("Input param is not DateTime:[{val_param.ToString()}];", new string[] { place, _httpContext.User.Identity.Name });
                return true;
            }

            return false;
        }
        public bool check_input_ip_address(ref string result_param_ip_address, object val_param, string place, bool is_req)
        {
            if (val_param == null)
            {
                _logger.LogWarning("Input param is Null.");
                return true;
            }

            string param_ip_address = val_param.ToString().Trim();

            // Проверка значения как IP адрес
            Regex rgx_ip_address   = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$", RegexOptions.None);
            Match m_rgx_ip_address = rgx_ip_address.Match( param_ip_address );

            if ( !m_rgx_ip_address.Success )
            {
                _logger.LogWarning("Input param is not DateTime:[{val_param.ToString()}];", new string[] { place, _httpContext.User.Identity.Name });
                return true;
            }

            result_param_ip_address = m_rgx_ip_address.Groups[0].Value;

            return false;
        }
        #endregion


        // *********************************
        public static void fill_to_structure_item(object value, ref object boxed_item, string prop_name, string prop_type)
        // *********************************
        {
            switch (prop_type)
            {
                case "Int16":
                case "Int32":
                case "Int64":
                case "integer":
                    int int_val = Convert.ToInt32(value);
                    boxed_item.GetType().GetProperty(prop_name).SetValue(boxed_item, int_val, null);
                    break;

                case "double":
                    double dbl_val = Convert.ToDouble(value);
                    boxed_item.GetType().GetProperty(prop_name).SetValue(boxed_item, dbl_val, null);
                    break;

                case "numeric":
                    decimal dcm_val = Convert.ToDecimal(value);
                    boxed_item.GetType().GetProperty(prop_name).SetValue(boxed_item, dcm_val, null);
                    break;

                case "bool":
                    bool b_val = Convert.ToBoolean(value);
                    boxed_item.GetType().GetProperty(prop_name).SetValue(boxed_item, b_val, null);
                    break;

                case "datetime":
                    DateTime date_time_val = Convert.ToDateTime(value);
                    boxed_item.GetType().GetProperty(prop_name).SetValue(boxed_item, date_time_val, null);
                    break;

                default:
                    string txt_val = value.ToString();
                    boxed_item.GetType().GetProperty(prop_name).SetValue(boxed_item, txt_val, null);
                    break;
            }
        }



    }
}
