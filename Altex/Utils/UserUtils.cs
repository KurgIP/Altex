using Altex.Util;
using Npgsql;
using NpgsqlTypes;
using System.Text.RegularExpressions;

namespace Altex.Utils
{
    public static class UserUtils
    {
        private static readonly string pg_connectionString = Startup._configurationStatic.GetConnectionString("PostgreSqlConnection");
        #region //================================================================== Filter By Column ====================================

        /// <summary>
        /// Возвращает кортеж:
        /// Фильтр по колонке, Список скрытых колонок, Текущая разбивка по страницам.
        /// </summary>
        /// <param name="key_place"></param>
        /// <param name="fields_properties"></param>
        /// <returns></returns>
        // *********************************  
        public static async Task<(Dictionary<string, FilterByColumn> dct_filters_columns, List<string> list_columns_collaps, Paging pagination)> Get_user_settings_of_place_async(PlacesFiltersCollaps key_place, Dictionary<string, Dictionary<string, string>> fields_properties)
        // *********************************
        {
            string error = "";
            Dictionary<string, FilterByColumn> dct_filters_columns  = new Dictionary<string, FilterByColumn>();
            List<string>                       list_columns_collaps = new List<string>();
            Paging                             pagination           = Pagination.get_new_paging();

            // Возвращаем кортеж из трёх значений
            (Dictionary<string, FilterByColumn> dct_filters_columns, List<string> list_columns_collaps, Paging pagination) tuple = (dct_filters_columns, list_columns_collaps, pagination);

            // Получаеь ID юзера, что бы загрузить его настройки
            string id_user = Startup._StaticUserManager.GetUserId( Startup._httpContextAccessor.HttpContext.User );

            //user_name Startup._httpContextAccessor.HttpContext.User.Identity.

            using (NpgsqlConnection dbConn = new NpgsqlConnection(pg_connectionString))
            {
                string filter_txt     = "";
                string collaps_txt    = "";
                string pagination_txt = "";
                try
                {
                    dbConn.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM filters_collaps_select_by_id_user( @iid, @iplc) AS (filter text, collaps text, pagination text )", dbConn);
                    command.Parameters.Add(new NpgsqlParameter("@iid",  NpgsqlDbType.Text       )).Value = id_user;
                    command.Parameters.Add(new NpgsqlParameter("@iplc", NpgsqlDbType.Integer, 32)).Value = (int)key_place;


                    NpgsqlDataReader dr = await command.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        filter_txt     = dr["filter"].GetType().Name     == "DBNull" ? "" : (string)dr["filter"];
                        collaps_txt    = dr["collaps"].GetType().Name    == "DBNull" ? "" : (string)dr["collaps"];
                        pagination_txt = dr["pagination"].GetType().Name == "DBNull" ? "" : (string)dr["pagination"];
                        break;
                    }

                    // Получаем фильтры колонок
                    dct_filters_columns = UserUtils.Convert_String_to_dct_FilterByColumn(filter_txt, ref fields_properties);

                    // Получаем список скрытых колонок
                    list_columns_collaps = new List<string>();
                    if (!String.IsNullOrWhiteSpace(collaps_txt))
                    {
                        string[] columns_collaps_arr = collaps_txt.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        list_columns_collaps         = new List<string>( columns_collaps_arr );
                    }

                    // Получаем параметры пагинации для этого места
                    if (!String.IsNullOrWhiteSpace(pagination_txt))
                    {
                        pagination = Pagination.Parsing_string_to_Paging( pagination_txt );
                    }

                    // Собираем в кортеж для возврата
                    tuple.dct_filters_columns  = dct_filters_columns;
                    tuple.list_columns_collaps = list_columns_collaps;
                    tuple.pagination           = pagination;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    Startup._logerStatic.LogError(error);
                    //string msg = " key_place:" + key_place + ",  id_user:" + id_user;
                    //Loger.Error(Startup._httpContextAccessor.HttpContext.User.Identity.Name, "UserUtil.get_FilterByColumn_products_by_pid", msg, ex.Message, false);
                }
                finally
                {
                    if (dbConn != null) dbConn.Close();
                }
            }
            return tuple;
        }

        // *********************************  
        public static async Task<string> filters_columns_save(PlacesFiltersCollaps key_place, Dictionary<string, FilterByColumn> dct_filterByColumn)
        // *********************************
        {
            string error      = "";
            string id_user    = Startup._StaticUserManager.GetUserId(Startup._httpContextAccessor.HttpContext.User);
            string filter_txt = Commons.join_to_string(ref dct_filterByColumn);

            using ( NpgsqlConnection dbConn = new NpgsqlConnection(pg_connectionString) )
            {
                try
                {
                    dbConn.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM filters_collaps_save_filters_by_columns(@idusr, @iplc, @val)", dbConn);
                    command.Parameters.Add(new NpgsqlParameter("@idusr", NpgsqlDbType.Text       )).Value = id_user;
                    command.Parameters.Add(new NpgsqlParameter("@iplc",  NpgsqlDbType.Integer, 32)).Value = (int)key_place;
                    command.Parameters.Add(new NpgsqlParameter("@val",   NpgsqlDbType.Text       )).Value = filter_txt;

                    int ret = await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    Startup._logerStatic.LogError(error);
                    //error = ex.Message;
                    //string msg = " key_place:" + key_place + ",  id_user:" + id_user;
                    //Loger.Error(Startup._httpContextAccessor.HttpContext.User.Identity.Name, "UserUtil.filters_columns_save", msg, ex.Message, false);
                }
                finally
                {
                    if (dbConn != null) dbConn.Close();
                }
            }
            return error;
        }


        //******************************
        public static FilterByColumn Get_new_filter_by_column(string name_column_for_filter, ref Dictionary<string, Dictionary<string, string>> fields_properties)
        //****************************** 
        {
            FilterByColumn filter_by_column = new FilterByColumn();
            filter_by_column.column = name_column_for_filter;
            filter_by_column.type   = fields_properties[name_column_for_filter]["type"];

            switch (filter_by_column.type)
            {
                case "datetime":
                    DateRange date_range = new DateRange();
                    date_range.finish    = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 59, 59, 59);
                    date_range.start     = date_range.finish.AddDays(-3);

                    FilterDateRange filter_date_range = new FilterDateRange();
                    filter_date_range.period     = "last";
                    filter_date_range.date_range = date_range;
                    filter_by_column.order       = "order_desc";
                    filter_by_column.value       = filter_date_range;
                    break;

                default:
                    filter_by_column.order = "no_order"; //"order",  "order_desc"
                    filter_by_column.value = "";
                    break;
            }

            return filter_by_column;
        }

        //******************************
        public static Dictionary<string, FilterByColumn> Convert_String_to_dct_FilterByColumn(string txt_filters_by_column, ref Dictionary<string, Dictionary<string, string>> fields_properties)
        //******************************
        {
            // Шаблон записи фильтра
            // <flt><clm>{0}</clm><tp>{1}</tp><vl>{2}</vl><ord>{3}</ord></flt>
            Regex regx_filter_column = new Regex(@"<flt>(.*?)</flt>", RegexOptions.None);
            Regex regx_column_name   = new Regex(@"<clm>(.*?)</clm>", RegexOptions.None);
            Regex regx_value         = new Regex(@"<vl>(.*?)</vl>", RegexOptions.None);
            Regex regx_order         = new Regex(@"<ord>(.*?)</ord>", RegexOptions.None);
            Regex regx_type          = new Regex(@"<tp>(.*?)</tp>", RegexOptions.None);


            Dictionary<string, FilterByColumn> dct_filter_by_columns = new Dictionary<string, FilterByColumn>();

            if (String.IsNullOrWhiteSpace(txt_filters_by_column)) return dct_filter_by_columns;


            MatchCollection mc_regx_filter_column = regx_filter_column.Matches(txt_filters_by_column);
            // Перебираем все найденные фильтры.
            foreach (Match m_regx_filter_column in mc_regx_filter_column)
            {
                if (m_regx_filter_column.Success)
                {
                    string filter_text = m_regx_filter_column.Groups[1].Value;

                    #region //--------------- column_name
                    string column_name        = "";
                    Match  m_regx_column_name = regx_column_name.Match(filter_text);
                    if (m_regx_column_name.Success)
                    {
                        column_name = m_regx_column_name.Groups[1].Value;
                    }
                    #endregion

                    FilterByColumn filterByColumn = UserUtils.Get_new_filter_by_column(column_name, ref fields_properties);

                    #region //--------------- value
                    Match m_regx_value = regx_value.Match(filter_text);
                    if (m_regx_value.Success)
                    {
                        filterByColumn.value = m_regx_value.Groups[1].Value;
                    }
                    #endregion

                    #region //--------------- order
                    Match m_regx_order = regx_order.Match(filter_text);
                    if (m_regx_order.Success)
                    {
                        filterByColumn.order = m_regx_order.Groups[1].Value;
                    }
                    #endregion

                    #region //--------------- type
                    // Заполняется при инициализации нового фильтра
                    Match m_regx_type = regx_type.Match(filter_text);
                    if (m_regx_type.Success)
                    {
                        filterByColumn.type = m_regx_type.Groups[1].Value;
                    }
                    #endregion

                    dct_filter_by_columns.Add(filterByColumn.column, filterByColumn);
                }
            }

            return dct_filter_by_columns;
        }


        // *********************************  
        public static async Task<string> Pagination_save_async( PlacesFiltersCollaps key_place, Paging paging )
        // *********************************
        {
            string error      = "";
            string id_user    = Startup._StaticUserManager.GetUserId( Startup._httpContextAccessor.HttpContext.User );
            string paging_txt = paging.ToStringDB();

            using ( NpgsqlConnection dbConn = new NpgsqlConnection( pg_connectionString ) )
            {
                try
                {
                    dbConn.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM filters_collaps_save_pagination(@idusr, @iplc, @val)", dbConn);
                    command.Parameters.Add(new NpgsqlParameter("@idusr", NpgsqlDbType.Text       )).Value = id_user;
                    command.Parameters.Add(new NpgsqlParameter("@iplc",  NpgsqlDbType.Integer, 32)).Value = (int)key_place;
                    command.Parameters.Add(new NpgsqlParameter("@val",   NpgsqlDbType.Text       )).Value = paging_txt;

                    int ret = await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    Startup._logerStatic.LogError(error);
                    //error = ex.Message;
                    //string msg = " key_place:" + key_place + ",  id_user:" + id_user;
                    //Loger.Error(Startup._httpContextAccessor.HttpContext.User.Identity.Name, "UserUtil.pagination_save", msg, ex.Message, false);
                }
                finally
                {
                    if (dbConn != null) dbConn.Close();
                }
            }
            return error;
        }


        // *********************************  
        public static async Task<string> Collaps_columns_save(PlacesFiltersCollaps key_place, List<string> collaps_columns)
        // *********************************
        {
            string error               = "";
            string id_user             = Startup._StaticUserManager.GetUserId(Startup._httpContextAccessor.HttpContext.User);
            string collaps_columns_txt = Commons.join_to_string(collaps_columns, ";");

            using (NpgsqlConnection dbConn = new NpgsqlConnection(pg_connectionString))
            {
                try
                {
                    dbConn.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM filters_collaps_save_collaps(@idusr, @iplc, @val)", dbConn);
                    command.Parameters.Add(new NpgsqlParameter("@idusr", NpgsqlDbType.Text        )).Value = id_user;
                    command.Parameters.Add(new NpgsqlParameter("@iplc",  NpgsqlDbType.Integer, 32 )).Value = (int)key_place;
                    command.Parameters.Add(new NpgsqlParameter("@val",   NpgsqlDbType.Text        )).Value = collaps_columns_txt;

                    int ret = await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    Startup._logerStatic.LogError(error);
                    //error = ex.Message;
                    //string msg = " key_place:" + key_place + ",  id_user:" + id_user;
                    //Loger.Error(Startup._httpContextAccessor.HttpContext.User.Identity.Name, "UserUtil.Collaps_columns_save", msg, ex.Message, false);
                }
                finally
                {
                    if (dbConn != null) dbConn.Close();
                }
            }
            return error;
        }


        #endregion

    }
}
