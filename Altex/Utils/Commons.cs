using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using NuGet.Packaging.Signing;
using Altex.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Altex.Controllers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Altex.Utils;

namespace Altex.Util
{

    public static class Commons
    {
        private static readonly CultureInfo _culture_info_en = new CultureInfo("en-US", false);
        public  static Random               _randObj         = new Random( Convert.ToInt32(DateTime.Now.ToString("ssffffff")) );

        #region //================================================================== Проверка входных данных ====================================

        static public string Sanitize(string txt)
        {
            if (txt == null) return null;

            txt = txt.Trim();
            int    length_txt = txt.Length;
            int    end_indx   = 10096;
            string m          = txt;
            if ( length_txt > end_indx )
            {
                m = txt.Substring(0, end_indx);
            }

            Regex reg_repl_singl_kov = new Regex(@"'", RegexOptions.IgnoreCase);
            m = reg_repl_singl_kov.Replace(m, "&#39;"); //&#8216;

            Regex reg_perevod_stroki = new Regex(@"(\r\n|\r|\n)", RegexOptions.IgnoreCase);
            m = reg_perevod_stroki.Replace(m, " ");

            m = HttpUtility.HtmlEncode(m);
            return m;
        }

        //******************************
        public static bool check_param(object val_param, string place, string error_type, string message, bool is_req)
        //******************************
        {
            bool is_check = false;
            if (val_param == null)
            {
                is_check = true;

                //HttpContext.Current
                string user_name = ""; // Startup._httpContextStatic.User.Identity.Name == null ? "user_undefined": Startup._httpContextStatic.User.Identity.Name;
                Startup._logerStatic.LogWarning( message + ":[{val_param.ToString()}];", new string[] { place, user_name, error_type });

                //Loger.EventInfo(Startup._httpContextAccessor.HttpContext.User.Identity.Name, place, error_type, message, is_req);
                //throw new Exception("Ошибка при получения списка полей fields по places из таблицы Fields!", ex);
            }
            return is_check;
        }

        //******************************
        public static bool check_contains(bool val, string place, string dict, string key, bool is_req)
        //******************************
        {
            if (!val)
            {
                string user_name = ""; // Startup._httpContextStatic.User.Identity.Name == null ? "user_undefined" : Startup._httpContextStatic.User.Identity.Name;
                Startup._logerStatic.LogWarning("Error! Key is wrong(Dict:" + dict + "[" + key + "]", new string[] { place, user_name });

                //string message = "Key is wrong(Dict:" + dict + "[" + key + "]";
                //Loger.EventInfo(Startup._httpContextAccessor.HttpContext.User.Identity.Name, place, "Hack", message, is_req);
            }
            return !val;
        }

        //******************************
        public static bool check_require_not_empty(ref Dictionary<string, string> dct_field_props, string name_param, string message, string place_log)
        //******************************
        {
            if (!dct_field_props.ContainsKey(name_param))
            {
                string user_name = ""; // Startup._httpContextStatic.User.Identity.Name == null ? "user_undefined" : Startup._httpContextStatic.User.Identity.Name;
                Startup._logerStatic.LogWarning(message + ":[{name_param}];", new string[] { place_log, user_name });
                return false;
            }

            if (String.IsNullOrWhiteSpace(dct_field_props[name_param]))
            {
                string user_name = ""; // Startup._httpContextStatic.User.Identity.Name == null ? "user_undefined" : Startup._httpContextStatic.User.Identity.Name;
                Startup._logerStatic.LogWarning(message + ":[{name_param}];", new string[] { place_log, user_name });
                return false;
            }

            return true;
        }

        #endregion



        #region //================================================================== Parsing ====================================

        //******************************
        public static Dictionary<string, string> Parsing_string_to_dct_string_string(string text)
        //******************************
        {
            Dictionary<string, string> dct = new Dictionary<string, string>();

            string[] arr_pair = text.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arr_pair.Length; i++)
            {
                string[] arr_key_val = arr_pair[i].Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr_key_val.Length != 2) continue;
                dct.Add(arr_key_val[0], arr_key_val[1]);
            }
            return dct;
        }

        //******************************
        public static Dictionary<string, int> Parsing_string_to_dct_string_int(string text)
        //******************************
        {
            Dictionary<string, int> dct = new Dictionary<string, int>();

            string[] arr_pair = text.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arr_pair.Length; i++)
            {
                string[] arr_key_val = arr_pair[i].Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr_key_val.Length != 2) continue;
                dct.Add(arr_key_val[0], Convert.ToInt32(arr_key_val[1]));
            }
            return dct;
        }

        public static List<int> Parsing_string_to_list_int(string text)
        {
            List<int> lst = new List<int>();

            string[] arr_txt = text.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arr_txt.Length; i++)
            {
                lst.Add(Convert.ToInt32(arr_txt[1]));
            }
            return lst;
        }
        public static List<int> Parsing_string_to_list_int(string text, string razdelitel)
        {
            List<int> lst = new List<int>();

            string[] arr_txt = text.Split(razdelitel, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arr_txt.Length; i++)
            {
                lst.Add(Convert.ToInt32(arr_txt[1]));
            }
            return lst;
        }
        public static DateRange Parsing_string_to_DateRange(string txt)
        {
            DateTime range_start = DateTime.MinValue;
            DateTime range_finish = DateTime.MaxValue;

            DateRange date_range = new DateRange();
            date_range.start = range_start;
            date_range.finish = range_finish;

            if (String.IsNullOrEmpty(txt)) return date_range;

            string[] arr_date_range = txt.Split(new Char[] { ';' });

            if (arr_date_range.Length != 2) return date_range;

            DateTime.TryParse(arr_date_range[0], out range_start);
            DateTime.TryParse(arr_date_range[1], out range_finish);

            date_range.start = range_start;
            date_range.finish = range_finish;

            return date_range;
        }
        #endregion


        //****************************
        public static string add_sub_folder_to_end_path(string source_path_file, string add_folder)
        //****************************
        {
            string new_name_file = "";
            Regex reg_last_slash = new Regex(@"(.*)(\/.*)$", RegexOptions.IgnoreCase);
            Match reg_last_slash_m = reg_last_slash.Match(source_path_file);
            if (reg_last_slash_m.Success)
            {
                string fold = reg_last_slash_m.Groups[1].ToString();
                string name = reg_last_slash_m.Groups[2].ToString();

                if (String.IsNullOrEmpty(fold))
                    new_name_file = add_folder + name;
                else
                    new_name_file = fold + "/" + add_folder + name;
            }
            else
            {
                new_name_file = add_folder + "/" + source_path_file;
            }
            new_name_file = Commons.path_clear_simbol_folder_break_slash(ref new_name_file);
            return new_name_file;
        }

        //****************************
        public static string path_clear_simbol_folder_break_slash(ref string txt)
        //****************************
        {
            string clear_path = Regex.Replace(txt, @"(\\{1,})", "/");
            clear_path = Regex.Replace(clear_path, @"(/{2,})", "/");
            return clear_path;
        }

        #region //*************************************************************************  join_to_string
        public static string join_to_string(List<string> arr_txt, string razdelit)
        {
            if (arr_txt == null) { return "null"; }
            if (arr_txt.Count == 0) { return ""; }
            string ret = string.Join(razdelit, arr_txt.ToArray());
            return ret;
        }
        public static string join_to_string(ref List<string> arr_txt, string razdelit)
        {
            if (arr_txt == null) { return "null"; }
            if (arr_txt.Count == 0) { return ""; }
            string ret = string.Join(razdelit, arr_txt.ToArray());
            return ret;
        }
        public static string join_to_string(List<object> arr_txt, string razdelit)
        {
            if (arr_txt == null) { return "null"; }
            if (arr_txt.Count == 0) { return ""; }
            StringBuilder builder = new StringBuilder();
            foreach (object txt in arr_txt)
            {
                builder.Append(txt).Append(razdelit);
            }
            string ret = builder.ToString();
            return ret.Substring(0, (ret.Length - razdelit.Length));
        }
        public static string join_to_string(List<int> arr_txt, string razdelit)
        {
            if (arr_txt == null) { return "null"; }
            if (arr_txt.Count == 0) { return ""; }
            StringBuilder builder = new StringBuilder();
            foreach (int txt in arr_txt)
            {
                builder.Append(txt).Append(razdelit);
            }
            string ret = builder.ToString();
            return ret.Substring(0, (ret.Length - razdelit.Length));
        }
        public static string join_to_string(string[] arr_txt, string razdelit)
        {
            if (arr_txt == null) { return "null"; }
            if (arr_txt.Length == 0) { return ""; }
            StringBuilder builder = new StringBuilder();
            foreach (string txt in arr_txt)
            {
                builder.Append(txt).Append(razdelit);
            }
            string ret = builder.ToString();
            return ret.Substring(0, (ret.Length - razdelit.Length));
        }
        public static string join_to_string(int[] arr_txt, string razdelit)
        {
            if (arr_txt == null) { return "null"; }
            if (arr_txt.Length == 0) { return ""; }
            StringBuilder builder = new StringBuilder();
            foreach (int txt in arr_txt)
            {
                builder.Append(txt.ToString()).Append(razdelit);
            }
            string ret = builder.ToString();
            return ret.Substring(0, (ret.Length - razdelit.Length));
        }
        public static string join_to_string(decimal[] arr_txt, string razdelit)
        {
            if (arr_txt == null) { return "null"; }
            if (arr_txt.Length == 0) { return ""; }
            //culture_info_ru.NumberFormat.CurrencyDecimalSeparator = ".";
            StringBuilder builder = new StringBuilder();
            foreach (decimal txt in arr_txt)
            {
                string txt_dec = txt.ToString(_culture_info_en);
                builder.Append(txt_dec).Append(razdelit);
            }
            string ret = builder.ToString();
            return ret.Substring(0, (ret.Length - razdelit.Length));
        }
        public static string join_to_string(ref Dictionary<string, decimal> dct, string ravno, string razdelit)
        {
            if (dct.Count == 0) return String.Empty;
            //culture_info_ru.NumberFormat.CurrencyDecimalSeparator = ".";
            StringBuilder bld = new StringBuilder();
            foreach (string key in dct.Keys)
            {
                string str_val = dct[key].ToString(_culture_info_en);
                bld.Append(key).Append(ravno).Append(str_val).Append(razdelit);
            }

            int numb_razdelit = razdelit.Length;
            bld.Remove(bld.Length - numb_razdelit, numb_razdelit);
            return bld.ToString();
        }
        public static string join_to_string(ref Dictionary<string, string> dct, string ravno, string razdelit)
        {
            if (dct.Count == 0) return String.Empty;

            StringBuilder bld = new StringBuilder();
            foreach (string key in dct.Keys)
                bld.Append(key).Append(ravno).Append(dct[key]).Append(razdelit);

            int numb_razdelit = razdelit.Length;
            bld.Remove(bld.Length - numb_razdelit, numb_razdelit);
            return bld.ToString();
        }
        public static string join_to_string(Dictionary<string, string> dct, string ravno, string razdelit)
        {
            if (dct.Count == 0) return String.Empty;

            StringBuilder bld = new StringBuilder();
            foreach (string key in dct.Keys)
                bld.Append(key).Append(ravno).Append(dct[key]).Append(razdelit);

            int numb_razdelit = razdelit.Length;
            bld.Remove(bld.Length - numb_razdelit, numb_razdelit);
            return bld.ToString();
        }
        public static string join_to_string(Dictionary<string, decimal> dct, string ravno, string razdelit)
        {
            if (dct.Count == 0) return String.Empty;

            StringBuilder bld = new StringBuilder();
            foreach (string key in dct.Keys)
                bld.Append(key).Append(ravno).Append(dct[key]).Append(razdelit);

            int numb_razdelit = razdelit.Length;
            bld.Remove(bld.Length - numb_razdelit, numb_razdelit);
            return bld.ToString();
        }
        public static string join_to_string(Dictionary<string, string>.ValueCollection value_collection, string razdelit)
        {
            if (value_collection == null) { return "null"; }
            if (value_collection.Count == 0) { return ""; }
            StringBuilder builder = new StringBuilder();
            foreach (string txt in value_collection)
            {
                builder.Append(txt.ToString()).Append(razdelit);
            }
            string ret = builder.ToString();
            return ret.Substring(0, (ret.Length - razdelit.Length));
        }
        public static string join_to_string(Dictionary<string, string>.KeyCollection key_collection, string razdelit)
        {
            if (key_collection == null) { return "null"; }
            if (key_collection.Count == 0) { return ""; }
            StringBuilder builder = new StringBuilder();
            foreach (string txt in key_collection)
            {
                builder.Append(txt.ToString()).Append(razdelit);
            }
            string ret = builder.ToString();
            return ret.Substring(0, (ret.Length - razdelit.Length));
        }
        public static string join_to_string(ref Dictionary<string, string[]> dct_sootv, string razdelit)
        {
            string ret = "";
            if (dct_sootv == null) { return ret; }
            if (dct_sootv.Count == 0) { return ret; }

            int numb_arr = dct_sootv.ElementAt(0).Value.Length;
            int last_numb = numb_arr - 1;

            StringBuilder bld = new StringBuilder();
            foreach (KeyValuePair<string, string[]> kv in dct_sootv)
            {
                for (int i = 0; i < numb_arr; i++)
                {
                    bld.Append(kv.Value[i]);
                    if (i < last_numb)
                        bld.Append(razdelit);
                    else
                        bld.Append("\n");
                }

            }
            return bld.ToString();
        }
        public static string join_to_string(ref Dictionary<string, object> dct, string ravno, string razdelit)
        {
            if (dct.Count == 0) return String.Empty;

            StringBuilder bld = new StringBuilder();
            foreach (string key in dct.Keys)
                bld.Append(key).Append(ravno).Append(dct[key]).Append(razdelit);

            int numb_razdelit = razdelit.Length;
            bld.Remove(bld.Length - numb_razdelit, numb_razdelit);
            return bld.ToString();
        }
        public static string join_to_string(ref Dictionary<int, string> dct, string ravno, string razdelit)
        {
            if (dct.Count == 0) return String.Empty;

            StringBuilder bld = new StringBuilder();
            foreach (int key in dct.Keys)
                bld.Append(key).Append(ravno).Append(dct[key]).Append(razdelit);

            int numb_razdelit = razdelit.Length;
            bld.Remove(bld.Length - numb_razdelit, numb_razdelit);
            return bld.ToString();
        }
        public static string join_to_string(ref Dictionary<string, FilterByColumn> dct_filter_by_columns)
        {
            // String.Format("<flt><clm>{0}</clm><tp>{1}</tp><vl>{2}</vl><ord>{3}</ord></flt>", column, type, value, order)
            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, FilterByColumn> kv in dct_filter_by_columns)
            {
                builder.Append(kv.Value.ToStringDB());
            }

            return builder.ToString();
        }
        #endregion

        //********************
        public static string GenerateKey()
        {
            return Guid.NewGuid().ToString().GetHashCode().ToString("x");
        }
        public static int    GenerateNumber()
        {
            int num_int     = Guid.NewGuid().GetHashCode();
            int num_positiv = Math.Abs(num_int);
            return num_positiv;
        }
        public static int    GenerateKey_Number()
        {
            return _randObj.Next();
        }


        /// <summary>
        /// Конвертирует фильтры по колонке в таблице данных в SQL тексты для задания WERE и ORDER.
        /// </summary>
        /// <param name="dct_filter_by_columns">Словарь параметров фильтров по указанным колонкам в таблице данных</param>
        /// <returns>Dictionary&lt; where:string, order:string &gt;</returns>

        //****************************
        public static Dictionary<string, string> Convert_filters_columns_to_sql( ref Dictionary<string, FilterByColumn> dct_filter_by_columns )
        //****************************
        {
            // ПАраметр фильтр устанвливает что запрос идёт от фильтров товара в магазине, а не в админке

            Regex reg_simb_vopros         = new Regex(@"\*",        RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex reg_any                 = new Regex(@"^ANY\('\{", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            Regex reg_simb_mbr            = new Regex(@"^[<|>|=]", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex reg_from_list_values    = new Regex(@";", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex reg_from_groupSelectors = new Regex(@"^\(.*\)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string sql_where = "";
            string sql_order = "";

            //=================================================================== конвертирование фильтра в sql запрос
            foreach (string name_column in dct_filter_by_columns.Keys)
            {
                FilterByColumn filter_by_column = dct_filter_by_columns[name_column];

                // ---------------------------- PARAM ORDER
                switch (filter_by_column.order)
                {
                    case "order":
                        sql_order = sql_order + "\"" + filter_by_column.column + "\", ";
                        break;
                    case "order_desc":
                        sql_order = sql_order + "\"" + filter_by_column.column + "\" DESC, ";
                        break;
                }

                if (filter_by_column.value == null || String.IsNullOrEmpty(filter_by_column.value.ToString())) continue;

                switch (filter_by_column.type)
                {
                    //==========================
                    case "text":
                    case "string":
                    case "list_images":
                    case "file_upload":
                        #region
                        // ---------------------------- PARAM  VALUE  //SELECT * FROM "Products" WHERE LOWER("brand") LIKE LOWER('%DAIK%')
                        string val_txt = filter_by_column.value.ToString();
                        val_txt        = val_txt.Replace("&#39;", "'");

                        //Это запросы фильтров по колонке из менеджера продуктов
                        // Ищем символ "любые другие символы"
                        Match m_simb_vopros = reg_simb_vopros.Match(val_txt);
                        if (m_simb_vopros.Success)
                        {
                            // Поиск с неопределёнными символами
                            val_txt = reg_simb_vopros.Replace(val_txt, "%");
                            if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                            sql_where = sql_where + "LOWER(tb.\"" + filter_by_column.column + "\") LIKE ('" + val_txt.ToLower() + "') ";
                        }
                        else
                        {
                            // Ищет по списку указанных указ
                            bool is_any = reg_any.IsMatch(val_txt);
                            if (is_any)
                            {
                                // Поиск по логической совокупности слов
                                if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                                sql_where = sql_where + "LOWER(tb.\"" + filter_by_column.column + "\")=" + val_txt.ToLower() + " ";
                            }
                            else
                            {
                                // Поиск по словам
                                Match m_reg_from_list_values = reg_from_list_values.Match(val_txt);
                                if (m_reg_from_list_values.Success)
                                {
                                    //Если много слов разделённых ;
                                    string sql_or = "";
                                    string[] arr_val_txt = val_txt.Split(new Char[] { ';' });
                                    for (int t = 0; t < arr_val_txt.Length; t++)
                                    {
                                        // Поиск по слову
                                        if (!String.IsNullOrEmpty(sql_or)) sql_or = sql_or + " OR ";
                                        sql_or = sql_or + "LOWER(tb.\"" + filter_by_column.column + "\")='" + arr_val_txt[t] + "' ";
                                    }
                                    // Поиск по слову
                                    if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                                    sql_where = sql_where + "(" + sql_or + ")";

                                }
                                else
                                {
                                    // Поиск по слову
                                    if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                                    sql_where = sql_where + "LOWER(tb.\"" + filter_by_column.column + "\")='" + val_txt.ToLower() + "' ";
                                }
                            }
                        }
                        
                        break;
                    #endregion

                    //==========================
                    case "numeric":
                    case "integer":
                        #region
                        // ---------------------------- PARAM  VALUE
                        string val_int = filter_by_column.value.ToString();

                        // Для безопасности при передачи html символов  "<" ">" их кодируют в "&lt;", "&gt;"
                        // Поэтому их надо декодировать обратно
                        val_int = val_int.Replace("&lt;", "<");
                        val_int = val_int.Replace("&gt;", ">");


                        //Это запросы фильтров по колонке из менеджера продуктов

                        // Это запрос из менеджера продуктов
                        Match m_reg_simb_mbr = reg_simb_mbr.Match(val_int);
                        if (m_reg_simb_mbr.Success)
                        {
                            // Есть символы стоящие впереди [<|>|=]
                            string simb_rng = m_reg_simb_mbr.Value;
                            if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                            sql_where = sql_where + "tb.\"" + filter_by_column.column + "\"" + val_int + " ";
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(val_int))
                            {
                                // Поле пустое. Выводим всё. Пропускаем это поле
                            }
                            else
                            {
                                // Поле со списочными данными. Выводим только по этому номеру  WHERE  "id" IN (10,359);
                                if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                                sql_where = sql_where + "tb.\"" + filter_by_column.column + "\" IN (" + val_int + ")";

                            }
                        }
                        
                        break;
                    #endregion

                    //==========================
                    case "list_integer":
                        #region
                        // ---------------------------- PARAM  VALUE
                        string val_list_int = filter_by_column.value.ToString();

                        //Это запросы фильтров по колонке из менеджера продуктов

                        // Это запрос из менеджера продуктов
                        // Перед списком не может быть ни каких символов поэтому вставляем только строчку с цифрами
                        if (String.IsNullOrEmpty(val_list_int))
                        {
                            // Поле пустое. Выводим всё. Пропускаем это поле
                        }
                        else
                        {
                            // Поле со списочными данными. Выводим только по этому номеру  WHERE  "id" IN (10,359);
                            if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                            sql_where = sql_where + "tb.\"" + filter_by_column.column + "\" && '{" + val_list_int + "}'";
                        }
                        
                        break;
                    #endregion

                    //==========================
                    case "bool":
                        #region
                        // ---------------------------- PARAM  VALUE
                        string val_bool_txt = filter_by_column.value.ToString();

                        //Если выбрано ВСЕ то пропускаем этот запрос поля
                        if ((string)filter_by_column.value != "all")
                        {
                            bool val_bool = Convert.ToBoolean(filter_by_column.value);
                            if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                            sql_where = sql_where + "tb.\"" + filter_by_column.column + "\" IS " + val_bool + " ";
                        }
                        
                        break;
                    #endregion

                    //==========================
                    case "datetime":
                        #region
                        FilterDateRange filter_date_range = (FilterDateRange)filter_by_column.value;
                        if (filter_date_range.period == "all") break;

                        DateRange date_range = Commons.get_DateRange(ref filter_by_column);

                        // ---------------------------- PARAM  VALUE
                        if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                        sql_where = sql_where + "(tb.\"" + filter_by_column.column + "\">='" + date_range.start + "' AND tb.\"" + filter_by_column.column + "\"<='" + date_range.finish + "')";
                        break;
                    #endregion

                    //==========================
                    default:
                        #region
                        // ---------------------------- PARAM  VALUE
                        string val = filter_by_column.value.ToString();
                        // Проверка это запрос из groupSelectors,чекбоксов фильтров
                        Match m_reg_from_groupSelectors_def = reg_from_groupSelectors.Match(val);
                        if (m_reg_from_groupSelectors_def.Success)
                        {
                            // Это запрос из фильтров товаров в магазине
                            if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                            sql_where = sql_where + val + " ";
                        }
                        else
                        {
                            //Это запросы фильтров по колонке из менеджера продуктов
                            if (!String.IsNullOrEmpty(sql_where)) sql_where = sql_where + " AND ";
                            sql_where = sql_where + "tb.\"" + filter_by_column.column + "\"='" + val + "' ";
                        }
                        break;
                        #endregion
                }
            }

            if (!String.IsNullOrEmpty(sql_where))
                sql_where = " WHERE " + sql_where;

            if (!String.IsNullOrEmpty(sql_order))
            {
                sql_order = sql_order.Remove(sql_order.Length - 2, 2);
                sql_order = " ORDER BY " + sql_order;
            }
            //else
            //{
            //    sql_order = " ORDER BY updated_at";
            //}

            return new Dictionary<string, string> { { "where", sql_where }, { "order", sql_order } };
        }

        /// <summary>
        /// Задаёт диапазон дат по текстовому имени диапазона в фильтре по колонке
        /// </summary>
        /// <param name="filter_by_column"></param>
        /// <returns>DateRange - диапазон дат</returns>
        //****************************
        public static DateRange get_DateRange( ref FilterByColumn filter_by_column )
        //****************************
        {
            DateRange       date_range        = new DateRange();
            FilterDateRange filter_date_range = (FilterDateRange)filter_by_column.value;

            DateTime curr_datatime = DateTime.Now;
            DateTime curr_day      = new DateTime(curr_datatime.Year, curr_datatime.Month, curr_datatime.Day, 59, 59, 59);

            switch (filter_date_range.period)
            {
                case "last":
                    date_range.start  = DateTime.Now.Subtract(TimeSpan.FromHours(24));
                    date_range.finish = curr_day;
                    break;

                case "day":
                    date_range.start  = curr_day.Subtract(TimeSpan.FromDays(1));
                    date_range.finish = curr_day;
                    break;

                case "tomorrow":
                    date_range.start  = curr_day;
                    date_range.finish = curr_day.AddDays(1);
                    break;

                case "week":
                    int day_of_week   = (int)DateTime.Now.DayOfWeek;
                    date_range.start  = curr_day.Subtract(TimeSpan.FromDays(day_of_week + 1));
                    date_range.finish = curr_day;
                    break;

                case "mounth":
                    date_range.start  = DateTime.Parse(DateTime.Now.Year + "/" + DateTime.Now.Month + "/1 0:0:0.000");
                    int day_in_mounth = DateTime.DaysInMonth(curr_day.Year, curr_day.Month);
                    date_range.finish = date_range.start.AddDays(day_in_mounth).AddHours(59).AddHours(59).AddSeconds(59);
                    break;

                case "day7":
                    date_range.start  = curr_day.Subtract(TimeSpan.FromDays(8));
                    date_range.finish = curr_day;
                    break;

                case "day14":
                    date_range.start  = curr_day.Subtract(TimeSpan.FromDays(15));
                    date_range.finish = curr_day;
                    break;

                case "all":
                    date_range.start  = DateTime.MinValue;
                    date_range.finish = DateTime.MaxValue;
                    break;

                case "range":
                    date_range.start  = ((DateRange)filter_by_column.value).start;
                    date_range.finish = ((DateRange)filter_by_column.value).finish;
                    break;

                default:
                    break;
            }
            filter_date_range.date_range = date_range;
            filter_by_column.value       = filter_date_range;

            return date_range;
        }


    }
}
