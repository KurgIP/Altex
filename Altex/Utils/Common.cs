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

namespace Altex.Util
{

    [Serializable]
    //*************************
    public struct DateRange
    //*************************
    {
        public DateTime start { get; set; }
        public DateTime finish { get; set; }

        public override string ToString()
        {
            return (String.Format("start:{0}; finish:{1}", start, finish));
        }

        public string ToStringIntoDB()
        {
            return (String.Format("{0};{1}", start, finish));
        }
    }

    public static class Commons
    {
        private static readonly CultureInfo _culture_info_en = new CultureInfo("en-US", false);
        public  static Random               _randObj          = new Random( Convert.ToInt32(DateTime.Now.ToString("ssffffff")) );

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

    }
}
