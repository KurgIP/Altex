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

namespace Altex.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ScanerController : ControllerExpand
    {

        //[DllImport("Iphlpapi.dll")]
        //private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);

        //[DllImport("Ws2_32.dll")]
        //private static extern Int32 inet_addr(string ip);


        private readonly ILogger<Controller>          _logger;
        private readonly HttpContext                  _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private static string  _nmap_programm_path      = "";
        private static string  _nmap_result_folder      = "";
        private static string  _nmap_result_file_XML    = "";
        private static string  _contentRootPath         = "";
        private static string  _nmap_result_folder_path = "";
        
        public ScanerController( ILogger<Controller> logger, HttpContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager) : base( logger, context )
        {
            _userManager = userManager;
            _logger      = logger;
            _context     = context;

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
            // Перехват потока вывода результатов сканирования nmap.exe не перехватывает поток в XML файл, а только стандартный поток в консоль.
            // Так как вывод результатов в XML самый полный по данным, придётся получать данные результатов сканирования через XML файл.
            // Для каждого потока указываем свой файл вывода XML результатов


            string ip_address ="10.0.0.10"; //"108.170.227.82"; //

            try
            {
                string strHostName   = "www.contoso.com";
                 IPHostEntry hostInfo = Dns.GetHostEntry(strHostName);  //;
                IPHostEntry ipEntry  = Dns.GetHostEntry(strHostName);
                IPAddress[] addr     = ipEntry.AddressList;

                IPAddress ipadr = IPAddress.Parse(ip_address);
                IPHostEntry ipEntry3 = Dns.GetHostEntry(ipadr);

                string      strHostName_2 = Dns.GetHostName();
                IPHostEntry ipEntry2      = Dns.GetHostEntry(strHostName_2);

                IPAddress ipadr4 = IPAddress.Parse("194.187.204.194");
                IPHostEntry ipEntry4 = Dns.GetHostEntry(ipadr4);


                IPAddress ipadr5 = IPAddress.Parse("192.178.241.70");
                IPHostEntry ipEntry5 = Dns.GetHostEntry(ipadr4);

                //{ 20.236.44.162}

                //return addr[addr.Length - 1].ToString();
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }

            //(string, ScanItem) result_scan = await Scan_IP_async(ip_address);

            return View();
        }

        public async Task<ActionResult> aj_scanning()
        {
            // Перехват потока вывода результатов сканирования nmap.exe не перехватывает поток в XML файл, а только стандартный поток в консоль.
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

            // Производим сканирование  указанных

            _userManager.
            string ip_address = "10.0.0.10"; //"108.170.227.82"; //


            (string, ScanItem) result_scan = await Scan_IP_async(ip_address);

            return View();
        }
        private static async Task<(string, ScanItem)> Scan_IP_async( string ip_for_scan )
        {
            string   error    = String.Empty;
            ScanItem scanItem = new ScanItem();

            //string contentRootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //string path_nmap = "C:/Program Files (x86)/Nmap/nmap.exe";
            //string OutputPath = Path.Combine(contentRootPath, "nmap_results/nmap_result.txt");
            //OutputPath = "C:/projects/Altex/Altex/nmap_results/nmap_rst.xml";
            // C:/ projects/Altex/Altex/nmap_results/nmap_rst.txt";

            string output_xml_file = "";
            using ( var nmap_process = new Process() )
            {
                nmap_process.StartInfo.FileName        = _nmap_programm_path;
                nmap_process.StartInfo.UseShellExecute = false;

                string process_name    = nmap_process.ProcessName;
                output_xml_file        = _nmap_result_folder_path + "/" + _nmap_result_file_XML + "_" + process_name + ".xml";
                string arguments       = " -O " + ip_for_scan + " -oX " + output_xml_file;

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
                if (!System.IO.File.Exists(output_xml_file))
                {
                    // Файл не создан
                    // Это ошибка работы nmap.exe
                    // Возвращаем ошибку
                    error = "Ошибка! Не создан XML файл с результатами сканирования IP=" + ip_for_scan;
                    return (error, scanItem);
                }

                // Распарсиваем результаты сканирования из XML файла
                (string, ScanItem) result_scan = await NMap_ResultXML_Parser.ParsingFileXML_async( output_xml_file );
                error    = result_scan.Item1;
                scanItem = result_scan.Item2;
            }

            return (error, scanItem);
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
    }
}
