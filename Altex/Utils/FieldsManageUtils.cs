using Altex.Util;
using Altex;
using Microsoft.AspNetCore.Authorization;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Altex.Utils
{
    //[Authorize(Roles = "SuperAdmin,Admin")]
    public static class FieldsManageUtils
        {
        #region //  Инициализация переменных
        private static readonly string _path_root           = Startup._serverRootPath;
        private static readonly string _pg_connectionString = Startup._configurationStatic.GetConnectionString("PostgreSqlConnection");
        //private static string _path_log         = Startup.StaticConfig.GetSection("Main").GetValue<string>("path_log");
        //private static string _connectionString = Startup.StaticConfig.GetSection("ConnectionStrings").GetValue<string>("PostgreSqlConnection");


        public static readonly List<string> list_field_properties_type = new List<string>() {
             "string", "text", "bool", "datetime", "integer", "list_images", "list_integer","numeric", "list_workers", "list_skills", "list_skill_levels", "fund", "chains", "dct_skills"
        };

        public static readonly List<string> list_field_html_render     = new List<string>() {
              "text_field", "text", "checkbox", "color", "list_images", "list_integer", "menu_pos","select", "select_currency", "skip", "text_area","txt_link", "list_workers", "list_skills", "list_skill_levels", "fund", "chains", "dct_skills"
        };
        #endregion

        public static Dictionary<string, string> Get_new_dct_field_properties(string place, string name_field)
        {
            Dictionary<string, string> dct_field_properties = new Dictionary<string, string>()
            {
                { "id",             "-1"        },
                { "name",           name_field  },
                { "order",          "0"         },
                { "type",           ""          },
                { "property",       ""          },
                { "description",    ""          },
                { "html",           ""          },
                { "place",          place       },
                { "skip_prm",       ""          },
                { "filter",         ""          },
                { "sub",            ""          },
                { "roles",          ""          },
                { "show_in_list",   ""          },
            };
            return dct_field_properties;
        }


        // *********************************
        public static async Task<List<string>> Get_list_all_places()
        // *********************************
        {
            string error = "";
            List<string> list_all_places = new List<string>();

            using ( NpgsqlConnection dbConn = new NpgsqlConnection(_pg_connectionString) )
            {
                dbConn.Open();

                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM fields_props_select_places() AS ( place varchar )", dbConn);
                try
                {
                    NpgsqlDataReader dr = await command.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        string place = (string)dr["place"];
                        list_all_places.Add(place);
                    }
                }
                catch (NpgsqlException ex)
                {
                    error = ex.Message;
                    Startup._logerStatic.LogError(error);

                    //string usr_name = Startup._httpContextAccessor.HttpContext.User.Identity.Name;
                    //Loger.Error(usr_name, "FieldsManageUtils.Get_list_all_name_models", "Ошибка при получения списка places из таблицы Fields!", ex.Message, false);
                    //throw new Exception("Ошибка при получения списка places из таблицы Fields!", ex);
                }
                finally
                {
                    if (dbConn != null) dbConn.Close();
                }
            }

            return list_all_places;
        }

        /// <summary>
        /// Загружаем словарь свойств полей для указанного места (place). NameColumn:string, Property:string, Value:string 
        /// </summary>
        /// <param name="place"></param>
        /// <param name="error"></param>
        /// <returns>Dictionary<NameColumn:string, Dictionary<Property:string, Value:string>>.</returns>
        /// <exception cref="Exception"></exception>
        // *********************************
        public static async Task<Dictionary<string, Dictionary<string, string>>> Get_dct_fields_properties_by_place_async(string place, IList<string> list_roles_user)
        // *********************************
        {
            string error = "";
            Dictionary<string, Dictionary<string, string>> dct_fields_properties_by_place = new Dictionary<string, Dictionary<string, string>>();

            using ( NpgsqlConnection dbConn = new NpgsqlConnection(_pg_connectionString) )
            {
                dbConn.Open();

                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM fields_props_select_fields_by_place(@plc)", dbConn);
                command.Parameters.Add(new NpgsqlParameter("@plc", NpgsqlDbType.Varchar, 32)).Value = place;
                try
                {
                    NpgsqlDataReader dr = await command.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        Dictionary<string, string> dct_field_param = new Dictionary<string, string>();

                        dct_field_param.Add("id",           dr["id"].ToString());
                        dct_field_param.Add("name",         (string)dr["name"]);
                        dct_field_param.Add("order",        dr["order"].ToString());
                        dct_field_param.Add("type",         (string)dr["type"]);
                        dct_field_param.Add("property",     dr["property"].GetType().Name     == "DBNull" ? "" : (string)dr["property"]);
                        dct_field_param.Add("description",  dr["description"].GetType().Name  == "DBNull" ? "" : (string)dr["description"]);
                        dct_field_param.Add("html",         (string)dr["html"]);
                        dct_field_param.Add("place",        dr["description"].GetType().Name  == "DBNull" ? "" : (string)dr["place"]);
                        dct_field_param.Add("skip_prm",     dr["skip_prm"].GetType().Name     == "DBNull" ? "" : (string)dr["skip_prm"]);
                        dct_field_param.Add("filter",       dr["filter"].GetType().Name       == "DBNull" ? "" : (string)dr["filter"]);
                        dct_field_param.Add("sub",          dr["sub"].GetType().Name          == "DBNull" ? "" : (string)dr["sub"]);
                        dct_field_param.Add("show_in_list", dr["show_in_list"].GetType().Name == "DBNull" ? "" : (string)dr["show_in_list"]);
                        dct_field_param.Add("roles",        dr["roles"].GetType().Name        == "DBNull" ? "" : (string)dr["roles"]);

                        // Каждая колонка в разных ролях отображается по разному
                        // Для свойства поля добавляем в словарь параметров добавляем ключи с названием ролей и типом отображения

                        if (String.IsNullOrWhiteSpace(dct_field_param["roles"]))
                        {
                            //Если нет ролей то устанавливаем тип вывода 2 - т.е. не выводить это поле
                            dct_field_param.Add( "role_type_show", "2" );
                        }
                        else
                        {
                            // Конвертируем текст с описаниями ролей и типом вывода в словарь
                            Dictionary<string, string> dct_roles_type = Commons.Parsing_string_to_dct_string_string(dct_field_param["roles"]);

                            // У юзера много ролей, устанавливаем тип вывода исходя из максимального из них
                            string curr_type_show = get_type_show_field_this_roles(ref dct_roles_type, ref list_roles_user);

                            // Создаём в словаре новое поле, указывающее как выводить его для текущего юзера с его правами
                            dct_field_param.Add("role_type_show", curr_type_show);
                        }

                        dct_fields_properties_by_place.Add(dct_field_param["name"], dct_field_param);
                    }
                }
                catch (NpgsqlException ex)
                {
                    error = ex.Message;
                    Startup._logerStatic.LogError(error);

                    //string usr_name = Startup._httpContextAccessor.HttpContext.User.Identity.Name;
                    //Loger.Error(usr_name, "FieldsManageUtils.Get_dct_fields_properties_by_place", "Ошибка при получения списка полей fields по places из таблицы Fields!", ex.Message, false);
                    //throw new Exception("Ошибка при получения списка полей fields по places из таблицы Fields!", ex);
                }
                finally
                {
                    if (dbConn != null) dbConn.Close();
                }
            }

            return dct_fields_properties_by_place;
        }


        // *********************************
        public static int fields_properties_insert_update_row(int id_field, ref Dictionary<string, string> field_props, ref string error)
        // *********************************
        {
            int id_result = -1;

            Dictionary<string, string> dct_clm_val_insert = new Dictionary<string, string>();

            #region //-----------------------------------------------------  SQL запрос --------------------------------
            dct_clm_val_insert["columns"] = "";
            dct_clm_val_insert["values"] = "";
            dct_clm_val_insert["update"] = "";
            foreach (KeyValuePair<string, string> prop in field_props)
            {
                dct_clm_val_insert["columns"] = dct_clm_val_insert["columns"] + "\"" + prop.Key + "\",";

                string next_val = "null";
                if (!String.IsNullOrEmpty((string)prop.Value))
                {
                    switch (prop.Key)
                    {
                        case "order":
                            next_val = prop.Value.ToString();
                            break;

                        default:
                            next_val = "'" + prop.Value + "'";
                            break;
                    }
                }
                dct_clm_val_insert["values"] = dct_clm_val_insert["values"] + next_val + ",";
                dct_clm_val_insert["update"] = dct_clm_val_insert["update"] + "\"" + prop.Key + "\"=" + next_val + ",";
            }

            dct_clm_val_insert["values"]  = dct_clm_val_insert["values"].Remove(dct_clm_val_insert["values"].Length - 1);
            dct_clm_val_insert["columns"] = dct_clm_val_insert["columns"].Remove(dct_clm_val_insert["columns"].Length - 1);
            dct_clm_val_insert["update"]  = dct_clm_val_insert["update"].Remove(dct_clm_val_insert["update"].Length - 1);
            #endregion

            using ( NpgsqlConnection dbConn = new NpgsqlConnection(_pg_connectionString) )
            {
                dbConn.Open();

                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM fields_props_update_insert_row(@iid, @iclm, @ival, @iupd)", dbConn);
                command.Parameters.Add(new NpgsqlParameter("@iid",  NpgsqlDbType.Integer, 32)).Value = id_field;
                command.Parameters.Add(new NpgsqlParameter("@iclm", NpgsqlDbType.Text,  4096)).Value = dct_clm_val_insert["columns"];
                command.Parameters.Add(new NpgsqlParameter("@ival", NpgsqlDbType.Text,  4096)).Value = dct_clm_val_insert["values"];
                command.Parameters.Add(new NpgsqlParameter("@iupd", NpgsqlDbType.Text,  4096)).Value = dct_clm_val_insert["update"];
                try
                {
                    object obj = command.ExecuteScalar();
                    if (obj.GetType().Name != "DBNull")
                    {
                        id_result = (int)obj;
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    Startup._logerStatic.LogError(error);

                    //string usr_name = Startup._httpContextAccessor.HttpContext.User.Identity.Name;
                    //Loger.Error(usr_name, "FieldsManageUtils.fields_properties_insert_update_row", "Ошибка при обновлении/сохранении свойст полей field в таблицы Fields!", ex.Message, false);
                    //throw new Exception("Ошибка при обновлении/сохранении свойст полей field в таблицы Fields!", ex);
                }
                finally
                {
                    if (dbConn != null) dbConn.Close();
                }
            }
            return id_result;
        }

        // *********************************
        public static void fields_properties_delete_row(int id_field, ref string error)
        // *********************************
        {
            using ( NpgsqlConnection dbConn = new NpgsqlConnection(_pg_connectionString) )
            {
                dbConn.Open();

                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM fields_props_delete_row( @iid )", dbConn);
                command.Parameters.Add(new NpgsqlParameter("@iid", NpgsqlDbType.Integer, 32)).Value = id_field;
                try
                {
                    object obj = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    Startup._logerStatic.LogError(error);

                    //string usr_name = Startup._httpContextAccessor.HttpContext.User.Identity.Name;
                    //Loger.Error(usr_name, "FieldsManageUtils.fields_properties_delete_row", "Ошибка при удалении строки id:" + id_field + " в таблицы Fields!", ex.Message, false);
                    //throw new Exception("Ошибка при удалении строки id:" + id_field + " в таблицы Fields!", ex);
                }
                finally
                {
                    if (dbConn != null) dbConn.Close();
                }
            }
        }



        #region //======================================================  Вспомогательные функции ================================================

        //******************************
        private static string get_type_show_field_this_roles(ref Dictionary<string, string> dct_roles_type, ref IList<string> list_roles_for_user)
        //******************************
        {
            int curr_type_show = 2;
            foreach (KeyValuePair<string, string> kv in dct_roles_type)
            {
                if (list_roles_for_user.Contains(kv.Key))
                {
                    int type_show = Convert.ToInt32(kv.Value);
                    if (type_show < curr_type_show)
                    {
                        curr_type_show = type_show;
                        if (curr_type_show == 0) break;
                    }
                }
            }
            return curr_type_show.ToString();
        }

        #endregion
    }
}
