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

namespace Altex.Controllers
{
    public class ScanerController : Controller
    {

        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);

        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);

        //private static string contentRootPath = app.Environment.ContentRootPath;

        // GET: ScanerController
        public ActionResult Index()
        {
            //string contentRootPath = Startup._serverRootPath;  //app.Environment.ContentRootPath;

            string contentRootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string path_nmap  = "C:/Program Files (x86)/Nmap/nmap.exe";
            string OutputPath = Path.Combine( contentRootPath, "nmap_results/nmap_result.txt");
            OutputPath = "C:/projects/Altex/Altex/nmap_results/nmap_rst.xml";
            using (var process = new Process())
            {
                process.StartInfo.FileName        = path_nmap;
                process.StartInfo.UseShellExecute = false;
                // process.StartInfo.Arguments       = " 8.8.8.8 -oN " + OutputPath;
                process.StartInfo.Arguments = " -O 8.8.8.8 -oX " + OutputPath; // C:/ projects/Altex/Altex/nmap_results/nmap_rst.txt";
                process.StartInfo.Arguments = " -O 8.8.8.8 " ; // C:/ projects/Altex/Altex/nmap_results/nmap_rst.txt";
                //process.StartInfo.Arguments = string.Format("{0} {1}", Options, Target);
                //process.StartInfo.WindowStyle = WindowStyle;

                int           lineCount = 0;
                StringBuilder output    = new StringBuilder();

                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    // Prepend line numbers to each line of the output.
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        lineCount++;
                        output.Append("\n[" + lineCount + "]: " + e.Data);
                    }
                });


                process.Start();

                process.BeginOutputReadLine();

                process.WaitForExit();

                if ( !System.IO.File.Exists(OutputPath))
                {
                    
                }
            }
            //sudo nmap -O  10.0.0.10 - oN / home / kipdbn_1 / Common / Altex / nmap_rslt.txt


            //string IP = ip.Text;
            //new Process
            //{
            //    StartInfo =
            //    {
            //        UseShellExecute = false,
            //        FileName = "nmap.exe",
            //        Arguments = "--top-ports 20 " + IP
            //    }
            //}.Start();
            //string arguments = "--top-ports 20 " + IP;
            //Process.Start("nmap.exe", arguments);




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


            bool pingable = false;
            Ping pinger   = null;
            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(ip_address);
                pingable = reply.Status == IPStatus.Success;
                if(pingable)
                {

                   string mac_adress = GetClientMAC(ip_address);
                }
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return View();
        }


        private static string GetClientMAC(string strClientIP)
        {
            string mac_dest = "";
            try
            {
                Int32 ldest = inet_addr(strClientIP);
                Int32 lhost = inet_addr("");
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                string mac_src = macinfo.ToString("X");
                while (mac_src.Length < 12)
                {
                    mac_src = mac_src.Insert(0, "0");
                }
                for (int i = 0; i < 11; i++)
                {
                    if (0 == (i % 2))
                    {
                        if (i == 10)
                        {
                            mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                        else
                        {
                            mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception("L?i " + err.Message);
            }
            return mac_dest;
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
