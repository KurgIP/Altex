using Altex.Models;
using Altex.Util;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Net.NetworkInformation;

namespace Altex.Utils
{
    public class ScanUtils
    {
        private static readonly string  pg_connectionString = Startup._configurationStatic.GetConnectionString("PostgreSqlConnection");

        // *********************************
        public static async Task<string> save_ScanResult_async( ScanResult scanResult )
        // *********************************
        {
            // ID сохранённой строки
            long id_scanResult = -1;

            string error = "";

            #region // Значения параметров портов передаём в виде массивов
            int port_quantity = scanResult.Ports_list.Count;

            int[]    arr_Number   = new int[port_quantity];
            string[] arr_Method   = new string[port_quantity];
            string[] arr_Protocol = new string[port_quantity];
            string[] arr_Reason   = new string[port_quantity];
            string[] arr_Service  = new string[port_quantity];
            string[] arr_State    = new string[port_quantity];

            int count = 0;
            foreach (Port port in scanResult.Ports_list )
            {
                arr_Number[count]   = port.Number;
                arr_Method[count]   = port.Method;
                arr_Protocol[count] = port.Protocol;
                arr_Reason[count]   = port.Reason;
                arr_Service[count]  = port.Service;
                arr_State[count]    = port.State;
                count++;
            }
            #endregion

            using (NpgsqlConnection dbConn = new NpgsqlConnection(pg_connectionString))
            {
                dbConn.Open();

                try
                {
                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM scan_result_save( @ip, @mac, @host, @host_type, @vendor, @finished_elapsed, @finished_exit, @finished_time," +
                                                                                             " @response_status, @runstats_down, @runstats_up, @start_scaning, @status_reason, @status_state," +
                                                                                             " @port_arr_number, @port_arr_method, @port_arr_protocol, @port_arr_reason, @port_arr_service, @port_arr_state )", dbConn);
                    // Параметры IP
                    command.Parameters.Add(new NpgsqlParameter("@ip",               NpgsqlDbType.Text       )).Value = scanResult.IP;
                    command.Parameters.Add(new NpgsqlParameter("@mac",              NpgsqlDbType.Text       )).Value = scanResult.MAC;
                    command.Parameters.Add(new NpgsqlParameter("@host",             NpgsqlDbType.Text       )).Value = scanResult.Host;
                    command.Parameters.Add(new NpgsqlParameter("@host_type",        NpgsqlDbType.Text       )).Value = scanResult.Host_type;
                    command.Parameters.Add(new NpgsqlParameter("@vendor",           NpgsqlDbType.Text       )).Value = scanResult.Vendor;
                    command.Parameters.Add(new NpgsqlParameter("@finished_elapsed", NpgsqlDbType.Real       )).Value = scanResult.finished_elapsed;
                    command.Parameters.Add(new NpgsqlParameter("@finished_exit",    NpgsqlDbType.Text       )).Value = scanResult.finished_exit;
                    command.Parameters.Add(new NpgsqlParameter("@finished_time",    NpgsqlDbType.Timestamp  )).Value = scanResult.finished_time;
                    command.Parameters.Add(new NpgsqlParameter("@response_status",  NpgsqlDbType.Text       )).Value = ResponseStatus.Not_answer.ToString();
                    command.Parameters.Add(new NpgsqlParameter("@runstats_down",    NpgsqlDbType.Text       )).Value = scanResult.runstats_down;
                    command.Parameters.Add(new NpgsqlParameter("@runstats_up",      NpgsqlDbType.Text       )).Value = scanResult.runstats_up;
                    command.Parameters.Add(new NpgsqlParameter("@start_scaning",    NpgsqlDbType.Timestamp  )).Value = scanResult.Start_scaning;
                    command.Parameters.Add(new NpgsqlParameter("@status_reason",    NpgsqlDbType.Text       )).Value = scanResult.Status_reason;
                    command.Parameters.Add(new NpgsqlParameter("@status_state",     NpgsqlDbType.Text       )).Value = scanResult.Status_state;
                    // Параметры порта
                    command.Parameters.Add(new NpgsqlParameter("@port_arr_number",   NpgsqlDbType.Integer | NpgsqlDbType.Array )).Value = arr_Number;
                    command.Parameters.Add(new NpgsqlParameter("@port_arr_method",   NpgsqlDbType.Text    | NpgsqlDbType.Array )).Value = arr_Method;
                    command.Parameters.Add(new NpgsqlParameter("@port_arr_protocol", NpgsqlDbType.Text    | NpgsqlDbType.Array )).Value = arr_Protocol;
                    command.Parameters.Add(new NpgsqlParameter("@port_arr_reason",   NpgsqlDbType.Text    | NpgsqlDbType.Array )).Value = arr_Reason;
                    command.Parameters.Add(new NpgsqlParameter("@port_arr_service",  NpgsqlDbType.Text    | NpgsqlDbType.Array )).Value = arr_Service;
                    command.Parameters.Add(new NpgsqlParameter("@port_arr_state",    NpgsqlDbType.Text    | NpgsqlDbType.Array )).Value = arr_State;

                    // Получаем ответ - ID вставленной строки
                    object id_scanResul_obj = await command.ExecuteScalarAsync();

                    if( id_scanResul_obj.GetType().Name == "DBNull" )
                    {
                        id_scanResult = (long)id_scanResul_obj;
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                    Startup._logerStatic.LogError(error);
                }
                finally
                {
                    if (dbConn != null) dbConn.Close();
                }
            }

            return error;
        }

        /// <summary>
        /// Получает из базы результаты сканирования IP, разбитые по страницам и по заданным фильтрам по колонкам в таблице .
        /// </summary>
        /// <param name="dct_filters_columns">Словарь фильтров по колонке. Dictionary&lt; name_column:string, filter_params:FilterByColumn &gt;</param>
        /// <param name="paging">Параметры разбиения вывода данных постранично. Paging</param>
        /// <returns>Возвращает кортеж ( error:string,  Dictionary&lt; id_scan_result:int, результат сканирования:ScanResult &gt;, List&lt; id_scan_result:int &gt;)</returns>

        // *********************************
        public static async Task<(string, Dictionary<int, ScanResult>, Paging)> load_ScanResults_async(Dictionary<string, FilterByColumn> dct_filters_columns, Paging paging)
        // *********************************
        {
            string error = "";
            List<int>                   list_id_ScanResults = new List<int>();
            Dictionary<int, ScanResult> dct_ScanResults     = new Dictionary<int, ScanResult>();

            // Конвертируем фильтры по колонке в SQL. Dictionary< where:string, order:string >  
            Dictionary<string, string> dct_sql_where_order = Commons.Convert_filters_columns_to_sql(ref dct_filters_columns);

            // Задаём смещение для выборки по страницам
            int offset = (paging.curr_page - 1) * paging.numb_on_page;

            int ret = -1;

            #region // Загружаем  IP
            using (NpgsqlConnection dbConn = new NpgsqlConnection(pg_connectionString))
            {
                dbConn.Open();

                try
                {
                    // Получаем список IP по фильтру и странице
                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM scan_result_select_ips_by_columns_filters( @ilim, @iofs, @iwhr, @iord )", dbConn);
                    command.Parameters.Add(new NpgsqlParameter("@ilim", NpgsqlDbType.Integer, 16)).Value = paging.numb_on_page;
                    command.Parameters.Add(new NpgsqlParameter("@iofs", NpgsqlDbType.Integer, 16)).Value = offset;
                    command.Parameters.Add(new NpgsqlParameter("@iwhr", NpgsqlDbType.Text, 2048)).Value = dct_sql_where_order["where"];
                    command.Parameters.Add(new NpgsqlParameter("@iord", NpgsqlDbType.Text, 2048)).Value = dct_sql_where_order["order"];

                    NpgsqlDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                    {
                        ScanResult scanResult = new ScanResult();

                        scanResult.Id               = (int)dr["id"];
                        scanResult.IP               = (string)dr["ip"];
                        scanResult.MAC              = (dr["mac"].GetType().Name == "DBNull") ? "" : (string)dr["mac"];
                        scanResult.Host             = (dr["host"].GetType().Name == "DBNull") ? "" : (string)dr["host"];
                        scanResult.Host_type        = (dr["host_type"].GetType().Name == "DBNull") ? "" : (string)dr["host_type"];
                        scanResult.Vendor           = (dr["vendor"].GetType().Name == "DBNull") ? "" : (string)dr["vendor"];
                        scanResult.finished_elapsed = (dr["finished_elapsed"].GetType().Name == "DBNull") ? -1 : (float)dr["finished_elapsed"];
                        scanResult.finished_exit    = (dr["finished_exit"].GetType().Name == "DBNull") ? "" : (string)dr["finished_exit"];
                        scanResult.finished_time    = (DateTime)dr["finished_time"];
                        string Response_status_txt  = (dr["response_status"].GetType().Name == "DBNull") ? "" : (string)dr["response_status"];
                        scanResult.Response_status  = (ResponseStatus)Enum.Parse(typeof(ResponseStatus), Response_status_txt, true);
                        scanResult.runstats_down    = (dr["runstats_down"].GetType().Name == "DBNull") ? "" : (string)dr["runstats_down"];
                        scanResult.runstats_up      = (dr["runstats_up"].GetType().Name == "DBNull") ? "" : (string)dr["runstats_up"];
                        scanResult.Start_scaning    = (DateTime)dr["start_scaning"];
                        scanResult.Status_reason    = (dr["status_reason"].GetType().Name == "DBNull") ? "" : (string)dr["status_reason"];
                        scanResult.Status_state     = (dr["status_state"].GetType().Name == "DBNull") ? "" : (string)dr["status_state"];

                        dct_ScanResults.Add(scanResult.Id, scanResult);
                        list_id_ScanResults.Add(scanResult.Id);
                    }
                }
                catch (NpgsqlException e)
                {
                    error = e.Message;
                    Startup._logerStatic.LogError(error);
                }
                finally
                {
                    if (dbConn != null) dbConn.Close();
                }
            }
            #endregion
            
            #region // Получаем полное количество IP по фильтру
            if( paging.full_quantity > -100 )
            {
                int full_quantity = -1;

                // Нужно для работы пагинации
                using (NpgsqlConnection dbConn = new NpgsqlConnection(pg_connectionString))
                {
                    dbConn.Open();

                    try
                    {
                        // Получаем количество строк IP по фильтру 
                        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM scan_result_select_ips_quantity_by_columns_filters( @iwhr )", dbConn);
                        command.Parameters.Add(new NpgsqlParameter("@iwhr", NpgsqlDbType.Text, 2048)).Value = dct_sql_where_order["where"];

                        object full_quantity_obj = await command.ExecuteScalarAsync();
                        if ( full_quantity_obj.GetType().Name != "DBNull" )
                        {
                            full_quantity        = (int)full_quantity_obj;
                            paging.full_quantity = full_quantity;
                        }

                    }
                    catch (NpgsqlException e)
                    {
                        error = e.Message;
                        Startup._logerStatic.LogError(error);
                    }
                    finally
                    {
                        if (dbConn != null) dbConn.Close();
                    }
                }
            }
            #endregion


            #region // Загружаем порты связанные с IP
            if ( list_id_ScanResults.Count > 0 )
            { 
                using (NpgsqlConnection dbConn = new NpgsqlConnection(pg_connectionString))
                {
                    dbConn.Open();
                    try
                    {
                        // Получаем порты по списку  IP  
                        NpgsqlCommand command_ports = new NpgsqlCommand("SELECT * FROM scan_result_select_ports_by_arr_id_ips( @arr_id )", dbConn);
                        command_ports.Parameters.Add(new NpgsqlParameter("@arr_id", NpgsqlDbType.Integer | NpgsqlDbType.Array)).Value = list_id_ScanResults.ToArray();

                        NpgsqlDataReader dr_ports = command_ports.ExecuteReader();

                        while (dr_ports.Read())
                        {
                            Port port       = new Port();
                            int ip_id       = (int)dr_ports["ip_id"];
                            port.Id         = (int)dr_ports["id"];
                            port.Number     = (int)dr_ports["number"];
                            port.Method     = (dr_ports["method"    ].GetType().Name == "DBNull") ? "" : (string)dr_ports["method"];
                            port.Protocol   = (dr_ports["protocol"  ].GetType().Name == "DBNull") ? "" : (string)dr_ports["protocol"];
                            port.Reason     = (dr_ports["reason"    ].GetType().Name == "DBNull") ? "" : (string)dr_ports["reason"];
                            port.Service    = (dr_ports["service"   ].GetType().Name == "DBNull") ? "" : (string)dr_ports["service"];
                            port.State      = (dr_ports["state"     ].GetType().Name == "DBNull") ? "" : (string)dr_ports["state"];

                            dct_ScanResults[ip_id].Ports_list.Add(port);
                        }
                    }
                    catch (NpgsqlException e)
                    {
                        error = e.Message;
                        Startup._logerStatic.LogError(error);
                    }
                    finally
                    {
                        if (dbConn != null) dbConn.Close();
                    }
                }
            }
            #endregion

            return ( error, dct_ScanResults, paging);
        }






        //// *********************************
        //public static long Model_insert(ref ModelData model_data, long id_project, ref string error)
        //// *********************************
        //{
        //    error = String.Empty;
        //    long id_model = -1;

        //    using (NpgsqlConnection dbConn = new NpgsqlConnection(pg_connectionString))
        //    {
        //        dbConn.Open();

        //        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM model_insert( @idprj, @nm, @dscr, @priv, @rght, @durat,  @mthd, @tmp, @ownr, @crt, @updt )", dbConn);
        //        command.Parameters.Add(new NpgsqlParameter("@idprj", NpgsqlDbType.Bigint)).Value = id_project;
        //        command.Parameters.Add(new NpgsqlParameter("@nm", NpgsqlDbType.Text)).Value = model_data.name;
        //        command.Parameters.Add(new NpgsqlParameter("@ncknm", NpgsqlDbType.Text)).Value = model_data.nickname;
        //        command.Parameters.Add(new NpgsqlParameter("@dscr", NpgsqlDbType.Text)).Value = model_data.description;
        //        command.Parameters.Add(new NpgsqlParameter("@priv", NpgsqlDbType.Integer)).Value = model_data.privacy;
        //        command.Parameters.Add(new NpgsqlParameter("@rght", NpgsqlDbType.Integer)).Value = model_data.rights;
        //        command.Parameters.Add(new NpgsqlParameter("@durat", NpgsqlDbType.TimestampRange)).Value = new NpgsqlRange<DateTime>(model_data.duration.start, model_data.duration.finish);
        //        command.Parameters.Add(new NpgsqlParameter("@mthd", NpgsqlDbType.Integer)).Value = model_data.methodology;
        //        command.Parameters.Add(new NpgsqlParameter("@tmp", NpgsqlDbType.Boolean)).Value = model_data.is_temp;
        //        command.Parameters.Add(new NpgsqlParameter("@ownr", NpgsqlDbType.Char, 36)).Value = model_data.owner;
        //        command.Parameters.Add(new NpgsqlParameter("@crt", NpgsqlDbType.Timestamp)).Value = model_data.created_at;
        //        command.Parameters.Add(new NpgsqlParameter("@updt", NpgsqlDbType.Timestamp)).Value = DateTime.Now;
        //        // SELECT * FROM model_insert( 4, 'Model 1', 'dscr text', 1, 1, '[2001.07.21 20:14:22, 2001.08.21 16:32:52]'::tsrange, 1, false, '8d7964c3-b4f1-4907-9e26-6951e7e24ebd', '2001.07.21 20:14:22'::timestamp, '2001.08.23 12:44:52.0000'::timestamp )

        //        try
        //        {
        //            object id_model_obj = command.ExecuteScalar();

        //            if (id_model_obj != null)
        //            {
        //                id_model = (long)id_model_obj;
        //                model_data.ID = id_model;
        //            }
        //        }
        //        catch (NpgsqlException e)
        //        {
        //            error = e.Message;
        //            Loger.Error(Membership.GetUser().UserName, "ProjectUtil.model_insert", error, "project_id:" + id_project + ", model_name:" + model_data.name, false);
        //        }
        //        finally
        //        {
        //            if (dbConn != null) dbConn.Close();
        //        }
        //    }

        //    return id_model;
        //}

        //// *********************************
        //public static int Model_update(ref ModelData model_data, ref string error)
        //// *********************************
        //{
        //    error = String.Empty;
        //    int ret = -1;

        //    using (NpgsqlConnection dbConn = new NpgsqlConnection(pg_connectionString))
        //    {
        //        dbConn.Open();
        //        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM model_update( @idmdl, @nm, @dscr, @durat,  @mthd, @updt )", dbConn);
        //        command.Parameters.Add(new NpgsqlParameter("@idmdl", NpgsqlDbType.Bigint)).Value = model_data.ID;
        //        command.Parameters.Add(new NpgsqlParameter("@nm", NpgsqlDbType.Text)).Value = model_data.name;
        //        command.Parameters.Add(new NpgsqlParameter("@ncknm", NpgsqlDbType.Text)).Value = model_data.nickname;
        //        command.Parameters.Add(new NpgsqlParameter("@dscr", NpgsqlDbType.Text)).Value = model_data.description;
        //        command.Parameters.Add(new NpgsqlParameter("@durat", NpgsqlDbType.TimestampRange)).Value = new NpgsqlRange<DateTime>(model_data.duration.start, model_data.duration.finish);
        //        command.Parameters.Add(new NpgsqlParameter("@mthd", NpgsqlDbType.Integer)).Value = model_data.methodology;
        //        command.Parameters.Add(new NpgsqlParameter("@updt", NpgsqlDbType.Timestamp)).Value = DateTime.Now;
        //        try
        //        {
        //            ret = command.ExecuteNonQuery();
        //        }
        //        catch (NpgsqlException e)
        //        {
        //            error = e.Message;
        //            Loger.Error(Membership.GetUser().UserName, "ProjectUtil.model_update", error, "model_data.ID:" + model_data.ID + ", model_name:" + model_data.name, false);
        //        }
        //        finally
        //        {
        //            if (dbConn != null) dbConn.Close();
        //        }
        //    }

        //    return ret;
        //}

        //// *********************************
        //public static int Model_delete(long id_model, ref string error)
        //// *********************************
        //{
        //    error = String.Empty;
        //    int ret = -1;

        //    using (NpgsqlConnection dbConn = new NpgsqlConnection(pg_connectionString))
        //    {
        //        dbConn.Open();
        //        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM model_delete( @idmdl )", dbConn);
        //        command.Parameters.Add(new NpgsqlParameter("@idmdl", NpgsqlDbType.Bigint)).Value = id_model;
        //        try
        //        {
        //            ret = command.ExecuteNonQuery();
        //        }
        //        catch (NpgsqlException e)
        //        {
        //            error = e.Message;
        //            Loger.Error(Membership.GetUser().UserName, "ProjectUtil.Model_delete", error, "id_model:" + id_model, false);
        //        }
        //        finally
        //        {
        //            if (dbConn != null) dbConn.Close();
        //        }
        //    }

        //    return ret;
        //}





    }
}
