using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Net;
using System.Diagnostics;
using System.IO;
using Humanizer;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using System.Text;
using System.Configuration;
using System.Drawing;
using Altex.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Net.Http;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Altex.Util;
using Altex.Utils;

namespace Altex.Controllers
{
    [Authorize(Roles = "Admin" )]
    public class ScanerController : ControllerExpand
    {

        //[DllImport("Iphlpapi.dll")]
        //private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);

        //[DllImport("Ws2_32.dll")]
        //private static extern Int32 inet_addr(string ip);


        private readonly ILogger<Controller>          _logger;
        //private readonly HttpContext                  _context;
        private readonly IHttpContextAccessor         _httpContext;
        private readonly UserManager<IdentityUser>     _userManager;

        private static string  _nmap_programm_path      = "";
        private static string  _nmap_result_folder      = "";
        private static string  _nmap_result_file_XML    = "";
        private static string  _contentRootPath         = "";
        private static string  _nmap_result_folder_path = "";
        
        public ScanerController( ILogger<Controller> logger, IConfiguration configuration, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContext) : base(logger, httpContext)//base( logger, context )HttpContext context, 
        {
            _userManager = userManager;
            _logger      = logger;
            _httpContext = httpContext;
            //_context     = context;
            // IHttpContextAccessor _context = Startup._httpContextAccessor;

            // Устанавливаем параметры NMap из файла настроек
            _nmap_programm_path      = configuration.GetSection("NMap_Settings").GetValue<string>("programm_path");
            _nmap_result_folder      = configuration.GetSection("NMap_Settings").GetValue<string>("result_folder");
            _nmap_result_file_XML    = configuration.GetSection("NMap_Settings").GetValue<string>("result_file_XML");
            _contentRootPath         = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );

            _nmap_result_folder_path = Path.Combine( _contentRootPath, _nmap_result_file_XML );
        }

        // GET: ScanerController      
        public ActionResult Index()
        {

            return View();
        }

        public async Task<ActionResult> aj_scanning()
        {
            // Перехват потока вывода результатов сканирования nmap.exe
            // Не перехватывается поток в XML файл, а только стандартный поток в консоль.
            // Так как вывод результатов в XML самый полный по данным, придётся получать данные результатов сканирования через XML файл.
            // Для каждого запушеного для сканирования экземпляра nmap.exe указываем свой файл вывода XML результатов
            string log_place = "ScanerController.aj_scanning";

            #region //----------------------------------  Входные данные

            string ip_address_start = "";
            // Проверяем наличие параметра ip_start - стартовый IP-address, и его валидности значению IP-address
            if ( check_input_ip_address( ref ip_address_start, Request.Form["ip_start"], log_place + ", param:ip_start", true)) 
                return Content( convert_error_message_to_js_answer_with_errfield("Ошибка при написание значения IP-address!IDFIELD:#ip_start") );


            bool is_ip_range = false;
            // Проверяем наличие параметра is_range - значение указывает, что выбран диапазон IP адресов
            if ( check_input_bool(ref is_ip_range, Request.Form["is_ip_range"], log_place + ", param:is_ip_range", true) )
                return Content( convert_error_message_to_js_answer_with_errfield("Ошибка в значении выбора диапазона ip!IDFIELD:#is_ip_range") );

            // Конец диапазона проверяем, только если указано, что  входящие данные содержат диапазон IP адресов
            string ip_address_finish = ip_address_start;
            if ( is_ip_range )
            {
                // Проверяем наличие параметра ip_finish - конечный IP-address в диапазоне адресов, и его валидности значению IP-address
                if (check_input_ip_address(ref ip_address_finish, Request.Form["ip_finish"], log_place + ", param:ip_finish", true))
                    return Content( convert_error_message_to_js_answer_with_errfield("Ошибка при написание значения IP-address!IDFIELD:#ip_finish") );
            }

            #endregion

            // Переводим входные данные в диапазон IP
            IPAddress ip_from = IPAddress.Parse( ip_address_start  );
            IPAddress ip_to   = IPAddress.Parse( ip_address_finish );
             
            // Получаем список IP адресов диапазона
            List<IPAddress> list_ipAddres_range  = GetIPList( ip_from, ip_to);


            List<ScanResult> list_scanResults = new List<ScanResult>();

            // Производим сканирование  указанных адресов
            foreach ( IPAddress ip_address in list_ipAddres_range )
            {
                string              ip_address_text = ip_address.ToString();
                (string, ScanResult) result_scan    = await Scan_IP_async( ip_address_text );

                ScanResult scanResult = result_scan.Item2;
                // Сохраняем в список полученный результат

                list_scanResults.Add(scanResult);

                string error = await ScanUtils.save_ScanResult_async( scanResult );
            }

            ViewData["list_scanResults"] = list_scanResults;

            return PartialView("block_scan_result_table");
        }
        

        private static async Task<(string, ScanResult)> Scan_IP_async( string ip_for_scan )
        {
            string     error      = String.Empty;
            ScanResult scanResult = new ScanResult();

            try
            {
                string output_xml_file = "";
                using (var nmap_process = new Process())
                {
                    nmap_process.StartInfo.FileName        = _nmap_programm_path;
                    nmap_process.StartInfo.UseShellExecute = false;

                    string process_name = DateTime.Now.ToString("MM_dd_hh_mm_ss_") + Commons.GenerateKey_Number().ToString();
                    output_xml_file     = _nmap_result_folder_path + "/" + _nmap_result_file_XML + "_" + process_name + ".xml";
                    string arguments    = " -O " + ip_for_scan + " -oX " + output_xml_file;

                    nmap_process.StartInfo.Arguments = arguments;

                    #region // ненужный код 
                    // Временно оставил для тестов параметров перехвата потока вывода результатов от nmap.exe

                    //process.StartInfo.Arguments = string.Format("{0} {1}", Options, Target);
                    //process.StartInfo.WindowStyle = WindowStyle;
                    //process.StartInfo.RedirectStandardOutput = true;

                    //int lineCount = 0;
                    //StringBuilder output = new StringBuilder();

                    //process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                    //{
                    //    // Prepend line numbers to each line of the output.
                    //    if (!String.IsNullOrEmpty(e.Data))
                    //    {
                    //        lineCount++;
                    //        output.Append("\n[" + lineCount + "]: " + e.Data);
                    //    }
                    //});
                    //process.BeginOutputReadLine();

                    // Synchronously read the standard output of the spawned process.
                    //StreamReader reader = process.StandardOutput;
                    //string output_nmap = reader.ReadToEnd();
                    #endregion

                    // Стартуем  сканирование
                    nmap_process.Start();

                    // Ожидаем окончания сканирования
                    nmap_process.WaitForExit();

                    // Проверяем, создан ли XML файл результатов
                    if ( !System.IO.File.Exists(output_xml_file) )
                    {
                        // Файл не создан
                        // Это ошибка работы nmap.exe
                        // Возвращаем ошибку
                        error = "Ошибка! Не создан XML файл с результатами сканирования IP=" + ip_for_scan;
                        return (error, scanResult);
                    }

                    // Распарсиваем результаты сканирования из XML файла
                    (string, ScanResult) result_scan = await NMap_ResultXML_Parser.ParsingFileXML_async( output_xml_file, ip_for_scan );
                    error      = result_scan.Item1;
                    scanResult = result_scan.Item2;
                }
            }
            catch ( Exception ex )
            {
                error = ex.Message;
            }


            return (error, scanResult);
        }

        private List<IPAddress> GetIPList(IPAddress ipFrom, IPAddress ipTo)
        {
            List<IPAddress> ipList    = new List<IPAddress>();
            String[]        arrayFrom = ipFrom.ToString().Split(new Char[] { '.' });
            String[]        arrayTo   = ipTo.ToString().Split(new Char[] { '.' });

            long firstIP = (long)(Math.Pow(256, 3) * Convert.ToInt32(arrayFrom[0]) + Math.Pow(256, 2) * Convert.ToInt32(arrayFrom[1]) + Math.Pow(256, 1) * Convert.ToInt32(arrayFrom[2]) + Convert.ToInt32(arrayFrom[3]));
            long lastIP  = (long)(Math.Pow(256, 3) * Convert.ToInt32(arrayTo[0])   + Math.Pow(256, 2) * Convert.ToInt32(arrayTo[1])   + Math.Pow(256, 1) * Convert.ToInt32(arrayTo[2])   + (Convert.ToInt32(arrayTo[3]) + 1));

            for (long i = firstIP; i < lastIP; i++)
                ipList.Add( IPAddress.Parse( string.Join(".", BitConverter.GetBytes(i).Take(4).Reverse() ) ) );
            return ipList;
        }

        private static List<IPAddress> IPAddressesRange(IPAddress firstIPAddress, IPAddress lastIPAddress)
        {
            var firstIPAddressAsBytesArray = firstIPAddress.GetAddressBytes();
            var lastIPAddressAsBytesArray  = lastIPAddress.GetAddressBytes();

            Array.Reverse( firstIPAddressAsBytesArray );
            Array.Reverse( lastIPAddressAsBytesArray  );

            var firstIPAddressAsInt = BitConverter.ToInt32( firstIPAddressAsBytesArray, 0 );
            var lastIPAddressAsInt  = BitConverter.ToInt32( lastIPAddressAsBytesArray,  0 );

            var ipAddressesInTheRange = new List<IPAddress>();

            for (var i = firstIPAddressAsInt; i <= lastIPAddressAsInt; i++)
            {
                var bytes = BitConverter.GetBytes(i);
                var newIp = new IPAddress( new[] { bytes[3], bytes[2], bytes[1], bytes[0] } );
                ipAddressesInTheRange.Add( newIp );
            }

            return ipAddressesInTheRange;
        }


        // GET: ScanerController/Details/5
        public async Task<ActionResult> List_result_scaned()
        {
            // Текущий юзер и его роли
            var           user            = await _userManager.GetUserAsync( Startup._httpContextAccessor.HttpContext.User );
            IList<string> list_roles_user = await _userManager.GetRolesAsync( user );

            //string ss =  ResponseStatus.Not_answer.ToString();
            //string Response_status_txt = "Not_answer";
            //ResponseStatus aa = (ResponseStatus)Enum.Parse(typeof(ResponseStatus), Response_status_txt, true);


            //Грузим свойства полей для колонок по их place
            Dictionary<string, Dictionary<string, string>>             dct_fields_properties        = await FieldsManageUtils.Get_dct_fields_properties_by_place_async("IPs", list_roles_user);

            // Получаем кортеж: Фильтр по колонке,  список скрытых колонок, текущая разбивка по страницам
            (Dictionary<string, FilterByColumn>, List<string>, Paging) tulip_user_settings_of_place = await UserUtils.Get_user_settings_of_place_async( PlacesFiltersCollaps.IPs, dct_fields_properties);

            Dictionary<string, FilterByColumn> dct_filters_columns = tulip_user_settings_of_place.Item1;

            Paging paging        = tulip_user_settings_of_place.Item3;
            // Задаём условие  full_quantity = -1, что бы произвести подсчёт сколько всего строк есть по этому фильтру колонок
            paging.full_quantity = -1;

            // Получаем страницу результатов сканирования. Кортеж(error, dct_ScanResult )
            (string, Dictionary<int, ScanResult>, Paging) result = await ScanUtils.load_ScanResults_async( dct_filters_columns, paging );

            ViewData["paging"]                = result.Item3;
            ViewData["dct_ScanResult"]        = result.Item2;
            ViewData["dct_fields_properties"] = dct_fields_properties;
            ViewData["list_columns_collaps"]  = tulip_user_settings_of_place.Item2;
            ViewData["dct_filters_columns"]   = dct_filters_columns;
            ViewData["is_editable_row"]       = false;

            return View();
        }




        // GET: ScanerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ScanerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ScanerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ScanerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ScanerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ScanerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ScanerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }



        #region //=========================================== Pagination, Collaps =============================
        [HttpPost]
        public async Task<IActionResult> get_list_by_pagination()
        {
            string error     = String.Empty;
            string log_place = "ScanerController.get_list_by_pagination";

            #region //----------------------------------  Входные данные

            int curr_page = -1;
            // Проверяем наличие параметра номер страницы num_page
            if (check_input_int(ref curr_page, Request.Form["currpg"], log_place + ", param:currpg", true)) return new EmptyResult();

            int num_on_page = -1;
            // Проверяем наличие параметра сколько строк на странице
            if (check_input_int(ref num_on_page, Request.Form["numonpg"], log_place + ", param:numonpg", true)) return new EmptyResult();

            // Текущий юзер и его роли
            var           user            = await _userManager.GetUserAsync( Startup._httpContextAccessor.HttpContext.User );
            IList<string> list_roles_user = await _userManager.GetRolesAsync( user );
            #endregion


            //Грузим свойства полей для колонок по их place
            Dictionary<string, Dictionary<string, string>>             dct_fields_properties        = await FieldsManageUtils.Get_dct_fields_properties_by_place_async( "IPs", list_roles_user);

            // Получаем кортеж: Фильтр по колонке,  список скрытых колонок, текущая разбивка по страницам
            (Dictionary<string, FilterByColumn>, List<string>, Paging) tulip_user_settings_of_place = await UserUtils.Get_user_settings_of_place_async( PlacesFiltersCollaps.IPs, dct_fields_properties);

            Dictionary<string, FilterByColumn> dct_filters_columns = tulip_user_settings_of_place.Item1;
     
            // Задаём параметры вывода страниц
            Paging paging       = tulip_user_settings_of_place.Item3;
            paging.numb_on_page = num_on_page;
            // Если изменение пришло из смены количества строк на странице,
            // Надо пересчитать исходя из этого текущую страницу так, что бы первая строка в старых настройках оставалась на новой странице
            if ( curr_page == -111 )
            {
                int count_rows = paging.curr_page * paging.numb_on_page;
                curr_page      = count_rows / paging.numb_on_page;
            }
            paging.curr_page = curr_page;



            // Получаем страницу результатов сканирования. Кортеж(error, dct_ScanResult )
            (string, Dictionary<int, ScanResult>, Paging) result = await ScanUtils.load_ScanResults_async(dct_filters_columns, paging);

            // Обрабатываем ошибки
            if ( !String.IsNullOrWhiteSpace(result.Item1) )
            {
                return Content( convert_error_message_to_js_answer_with_errfield( result.Item1 ) );
            }
            paging = result.Item3;

            #region // Сохраняем в базу пагинацию
            error = await UserUtils.Pagination_save_async( PlacesFiltersCollaps.IPs, paging );
            if (!String.IsNullOrWhiteSpace(error))
            {
                Startup._httpContextAccessor.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.TemporaryRedirect; //307
                return Content(GeneratorHTML.GenerateMessageErrorJQuery("Ошибка при сохранении пагинации."));
            }
            #endregion

            ViewData["paging"]                = paging;
            ViewData["dct_ScanResult"]        = result.Item2;
            ViewData["dct_fields_properties"] = dct_fields_properties;
            ViewData["list_columns_collaps"]  = tulip_user_settings_of_place.Item2;
            ViewData["dct_filters_columns"]   = dct_filters_columns;
            ViewData["is_editable_row"]       = false;

            return PartialView("table_ip");
        }


        [HttpPost]
        public async Task<IActionResult> aj_set_collaps_columnn()
        {
            string error     = String.Empty;
            string log_place = "ScanerController.aj_set_collaps_columnn";

            #region //----------------------------------  Входные данные

            // Текущий юзер и его роли
            var user = await _userManager.GetUserAsync(Startup._httpContextAccessor.HttpContext.User);
            IList<string> list_roles_user = await _userManager.GetRolesAsync(user);


            // Проверяем наличие параметра тип данных "IPs"
            if (Commons.check_param(Request.Form["tp"], log_place, "Need params tp is Null", "Hack", true)) return new EmptyResult();
            string type_data = Commons.Sanitize( Request.Form["tp"] );

            // Проверяем наличие параметра тип данных  по названию ( "workplaces" или другие)
            object enum_type = (object)PlacesFiltersCollaps.IPs;
            if ( !PlacesFiltersCollaps.TryParse( typeof(PlacesFiltersCollaps), type_data, true, out enum_type ) )
            {
                // Ошибка типа данных
                return new EmptyResult();
            }

            // Проверяем наличие параметра имя скрытой колонки n
            if (Commons.check_param(Request.Form["n"], log_place, "Need params n is Null", "Hack", true)) return new EmptyResult();
            string collaps_name_column = Commons.Sanitize(Request.Form["n"]);

            #endregion

            //Грузим свойства полей для колонок по их place
            Dictionary<string, Dictionary<string, string>>             dct_fields_properties        = await FieldsManageUtils.Get_dct_fields_properties_by_place_async( "IPs", list_roles_user );
            // Получаем кортеж: Фильтр по колонке,  список скрытых колонок, текущая разбивка по страницам
            (Dictionary<string, FilterByColumn>, List<string>, Paging) tulip_user_settings_of_place = await UserUtils.Get_user_settings_of_place_async( PlacesFiltersCollaps.IPs, dct_fields_properties );

            // Используется ли такое имя
            if (!dct_fields_properties.ContainsKey(collaps_name_column))
            {
                // Ошибка типа данных
                return new EmptyResult();
            }

            List<string> list_collaps_columns = tulip_user_settings_of_place.Item2;

            if ( !list_collaps_columns.Contains(collaps_name_column) )
                list_collaps_columns.Add(collaps_name_column);

            #region // Сохраняем в базу скрытые поля
            error = await UserUtils.Collaps_columns_save(PlacesFiltersCollaps.IPs, list_collaps_columns);
            if (!String.IsNullOrWhiteSpace(error))
            {
                Startup._httpContextAccessor.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.TemporaryRedirect; //307
                return Content(GeneratorHTML.GenerateMessageErrorJQuery("Ошибка при сохранении списка скрытых колонок."));
            }
            #endregion

            // Сообщение пользователю
            string js = GeneratorHTML.GenerateMessageJS2HTML("Колонка <b>" + collaps_name_column.ToUpper() + "</b> скрыта");

            return Content(js, "text/javascript");
        }

        //[HttpPost]
        //public async Task<IActionResult> aj_set_workplaces_collaps_columns_panel_show()
        //{
        //    string error = String.Empty;
        //    string log_place = "WorkplaceManageController.aj_set_workplaces_collaps_columnn";

        //    #region //----------------------------------  Входные данные

        //    int id_organization = -1;
        //    // Проверяем наличие параметра sp (id проекта ) и соответствие его типу int
        //    if (check_input_int(ref id_organization, Request.Form["orgnz"], log_place + ", param:orgnz", true)) return new EmptyResult();

        //    int id_structure = -1;
        //    // Проверяем наличие параметра strct ( id структуры проекта ) и соответствие его типу int
        //    if (check_input_int(ref id_structure, Request.Form["strct"], log_place + ", param:strct", true)) return new EmptyResult();

        //    // Текущий юзер и его роли
        //    var user = await _userManager.GetUserAsync(Startup._httpContextAccessor.HttpContext.User);
        //    IList<string> list_roles_user = await _userManager.GetRolesAsync(user);

        //    // Получаем из базы список организаций и их структур текущего юзера
        //    // Dictionary<ID_ORGANIZATION:int, ID_STRUCTURE:List<int>>
        //    Dictionary<int, NameIdList> dct_user_organizations = await WorkplaceUtil.Get_dct_user_organizations_async(user.Id);

        //    // Проверка правильности id_organization
        //    if (Commons.check_contains(dct_user_organizations.ContainsKey(id_organization), log_place, "dct_user_organizations", id_organization.ToString(), true)) { return new EmptyResult(); }

        //    // Проверка правильности id_structure
        //    if (Commons.check_contains(dct_user_organizations[id_organization].list.ContainsKey(id_structure), log_place, "dct_user_organizations_structure", id_structure.ToString(), true)) { return new EmptyResult(); }


        //    // Проверяем наличие параметра тип данных "workplaces"
        //    if (Commons.check_param(Request.Form["tp"], log_place, "Need params tp is Null", "Hack", true)) return new EmptyResult();
        //    string type_data = Commons.Sanitize(Request.Form["tp"]);

        //    // Проверяем наличие параметра тип данных  по названию ( "workplaces" или другие)
        //    object enum_type_workplace = (object)PlacesFiltersCollaps.organization;
        //    if (!PlacesFiltersCollaps.TryParse(typeof(PlacesFiltersCollaps), type_data, true, out enum_type_workplace))
        //    {
        //        // Ошибка типа данных
        //        return new EmptyResult();
        //    }

        //    #endregion

        //    //Грузим свойства полей для колонок по их place
        //    Dictionary<string, Dictionary<string, string>> dct_fields_properties = await FieldsManageUtils.Get_dct_fields_properties_by_place_async("workplace", list_roles_user);
        //    // Получаем кортеж: Фильтр по колонке,  список скрытых колонок, текущая разбивка по страницам
        //    (Dictionary<string, FilterByColumn>, List<string>, Paging) tulip_user_settings_of_place = UserUtil.Get_user_settings_of_place(PlacesFiltersCollaps.workplace, ref dct_fields_properties);

        //    ViewData["dct_fields_properties"] = dct_fields_properties;
        //    ViewData["list_columns_collaps"] = tulip_user_settings_of_place.Item2;

        //    return PartialView("collaps_columns_panel");
        //}

        //[HttpPost]
        //public async Task<IActionResult> aj_set_workplaces_collaps_columns_panel_close()
        //{
        //    string error = String.Empty;
        //    string log_place = "WorkplaceManageController.aj_set_workplaces_collaps_columns_panel_close";

        //    #region //----------------------------------  Входные данные

        //    int id_organization = -1;
        //    // Проверяем наличие параметра sp (id проекта ) и соответствие его типу int
        //    if (check_input_int(ref id_organization, Request.Form["orgnz"], log_place + ", param:orgnz", true)) return new EmptyResult();

        //    int id_structure = -1;
        //    // Проверяем наличие параметра strct ( id структуры проекта ) и соответствие его типу int
        //    if (check_input_int(ref id_structure, Request.Form["strct"], log_place + ", param:strct", true)) return new EmptyResult();

        //    // Текущий юзер и его роли
        //    var user = await _userManager.GetUserAsync(Startup._httpContextAccessor.HttpContext.User);
        //    IList<string> list_roles_user = await _userManager.GetRolesAsync(user);

        //    // Получаем из базы список организаций и их структур текущего юзера
        //    // Dictionary<ID_ORGANIZATION:int, ID_STRUCTURE:List<int>>
        //    Dictionary<int, NameIdList> dct_user_organizations = await WorkplaceUtil.Get_dct_user_organizations_async(user.Id);

        //    // Проверка правильности id_organization
        //    if (Commons.check_contains(dct_user_organizations.ContainsKey(id_organization), log_place, "dct_user_organizations", id_organization.ToString(), true)) { return new EmptyResult(); }

        //    // Проверка правильности id_structure
        //    if (Commons.check_contains(dct_user_organizations[id_organization].list.ContainsKey(id_structure), log_place, "dct_user_organizations_structure", id_structure.ToString(), true)) { return new EmptyResult(); }


        //    // Проверяем наличие параметра тип данных "workplaces"
        //    if (Commons.check_param(Request.Form["tp"], log_place, "Need params tp is Null", "Hack", true)) return new EmptyResult();
        //    string type_data = Commons.Sanitize(Request.Form["tp"]);

        //    // Проверяем наличие параметра тип данных  по названию ( "workplaces" или другие)
        //    object enum_type_workplace = (object)PlacesFiltersCollaps.organization;
        //    if (!PlacesFiltersCollaps.TryParse(typeof(PlacesFiltersCollaps), type_data, true, out enum_type_workplace))
        //    {
        //        // Ошибка типа данных
        //        return new EmptyResult();
        //    }

        //    // Проверяем наличие параметра список имён скрытых колонок 
        //    if (Commons.check_param(Request.Form["clmhd"], log_place, "Need params clmhd is Null", "Hack", true)) return new EmptyResult();
        //    string list_column_name_hide_txt = Commons.Sanitize(Request.Form["clmhd"]);
        //    List<string> list_column_name_hide = list_column_name_hide_txt.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        //    #endregion

        //    //Грузим свойства полей для колонок по их place
        //    Dictionary<string, Dictionary<string, string>> dct_fields_properties = await FieldsManageUtils.Get_dct_fields_properties_by_place_async("workplace", list_roles_user);
        //    // Получаем кортеж: Фильтр по колонке,  список скрытых колонок, текущая разбивка по страницам
        //    //(Dictionary<string, FilterByColumn>, List<string>, Paging) tulip_user_settings_of_place = UserUtil.Get_user_settings_of_place(PlacesFiltersCollaps.workplace, ref dct_fields_properties);

        //    // Проверяем правильность имен
        //    List<string> list_keys = dct_fields_properties.Keys.ToList();
        //    List<string> list_save = new List<string>();
        //    foreach (string column_name in list_column_name_hide)
        //    {
        //        if (list_keys.Contains(column_name))
        //        {
        //            // Сохраняем только существующие поля
        //            list_save.Add(column_name);
        //        }
        //    }

        //    #region // Сохраняем в базу скрытые поля
        //    error = await UserUtil.Collaps_columns_save(PlacesFiltersCollaps.workplace, list_save);
        //    if (!String.IsNullOrWhiteSpace(error))
        //    {
        //        Startup._httpContextAccessor.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.TemporaryRedirect; //307
        //        return Content(GeneratorHTML.GenerateMessageErrorJQuery("Ошибка при сохранении списка скрытых колонок."));
        //    }
        //    #endregion

        //    // Сообщение пользователю
        //    string js = GeneratorHTML.GenerateMessageJQuery("Сохранение прошло удачно");

        //    return Content(js, "text/javascript");
        //}
        //#endregion


        #endregion


    }
}
