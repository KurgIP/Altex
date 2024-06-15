using Microsoft.Extensions.Hosting;
using System;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Altex.Models
{
    [Serializable]
    //*************************
    public struct Port
    //*************************
    {
        public int    ID;                     // Номер порта
        public string State;                  // Состояние порта,
        public string Protocol;               // Протокол,
        public string Reason;                 //
        //public string reason_ttl;           //

        public string Service;                // Название сервиса
        public string Method;                 // Метод сервиса
        //public string Conf;                 // Конфигурация сервиса

        #region // Пример вывода данных о порте
        //      <port protocol = "tcp" portid="443">
        //            <state state = "open" reason="syn-ack" reason_ttl="123"/>
        //            <service name = "https" method="table" conf="3"/>
        //      </port>
        #endregion
        public override string ToString()
        {
            return (String.Format("Port[{0}; {1}; {2}; {3}; {4}];", ID, State, Service, Protocol, Method));
        }
    }


    [Serializable]
    //*************************
    public struct ScanItem
    //*************************
    {
        public string     IP;                  // IPv4 address
        public string     MAC;                 // MAC address,
        public string     Vendor;              // Имя вендора,
        public string     Host;                // Имя хоста,
        public DateTime   Start_scaning;       // Время начала сканирования,
        public string     Status_state;        // Параметр state  в тэге Status,
        public string     Status_reason;       // Параметр reason в тэге Status,
        public List<Port> Ports_list;          // Имя хоста,


        #region // Пример вывода данных о порте
        //LOCAL
        // <hosthint>
        //    <status state = "up" reason="unknown-response" reason_ttl="0"/>
        //    <address addr = "8.8.8.8" addrtype="ipv4"/>
        //    <hostnames>
        //    </hostnames>           
        //    <ports>
        //          <extraports state = "closed" count="996">
        //                <extrareasons reason = "reset" count="996" proto="tcp" ports="1,3-4,6-7,9,13,17,19-20,22-26,30,32-33,37,42-43,49,53,70,79-85,88-90,99-100,106,109-110,113,119,125,135,143-144,146,161,163,179,199,211-212,222,254-256,259,264,280,301,306,311,340,366,389,406-407,416-417,425,427,443-444,458,464-465,481,497,500,512-515,524,541,543-545,548,554-555,563,587,593,616-617,625,631,636,646,648,666-668,683,687,691,700,705,711,714,720,722,726,749,765,777,783,787,800-801,808,843,873,880,888,898,900-903,911-912,981,987,990,992-993,995,999-1002,1007,1009-1011,1021-1100,1102,1104-1108,1110-1114,1117,1119,1121-1124,1126,1130-1132,1137-1138,1141,1145,1147-1149,1151-1152,1154,1163-1166,1169,1174-1175,1183,1185-1187,1192,1198-1199,1201,1213,1216-1218,1233-1234,1236,1244,1247-1248,1259,1271-1272,1277,1287,1296,1300-1301,1309-1311,1322,1328,1334,1352,1417,1433-1434,1443,1455,1461,1494,1500-1501,1503,1521,1524,1533,1556,1580,1583,1594,1600,1641,1658,1666,1687-1688,1700,1717-1721,1723,1755,1761,1782-1783,1801,1805,1812,1839-1840,1862-1864,1875,1900,1914,1935,1947,1971-1972,1974,1984,1998-2010,2013,2020-2022,2030,2033-2035,2038,2040-2043,2045-2049,2065,2068,2099-2100,2103,2105-2107,2111,2119,2121,2126,2135,2144,2160-2161,2170,2179,2190-2191,2196,2200,2222,2251,2260,2288,2301,2323,2366,2381-2383,2393-2394,2399,2401,2492,2500,2522,2525,2557,2601-2602,2604-2605,2607-2608,2638,2701-2702,2710,2717-2718,2725,2800,2809,2811,2869,2875,2909-2910,2920,2967-2968,2998,3000-3001,3003,3005-3006,3011,3017,3030-3031,3052,3071,3077,3128,3168,3211,3221,3260-3261,3268-3269,3283,3300-3301,3306,3322-3325,3333,3351,3367,3369-3372,3389-3390,3404,3476,3493,3517,3527,3546,3551,3580,3659,3689-3690,3703,3737,3766,3784,3800-3801,3809,3814,3826-3828,3851,3869,3871,3878,3880,3889,3905,3914,3918,3920,3945,3971,3986,3995,3998,4000-4006,4045,4111,4125-4126,4129,4224,4242,4279,4321,4343,4443-4446,4449,4550,4567,4662,4848,4899-4900,4998,5000-5004,5009,5030,5033,5050-5051,5054,5060-5061,5080,5087,5100-5102,5120,5190,5200,5214,5221-5222,5225-5226,5269,5280,5298,5357,5405,5414,5431-5432,5440,5500,5510,5544,5550,5555,5560,5566,5631,5633,5666,5678-5679,5718,5730,5800-5802,5810-5811,5815,5822,5825,5850,5859,5862,5877,5900-5904,5906-5907,5910-5911,5915,5922,5925,5950,5952,5959-5963,5985-5989,5998-6007,6009,6025,6059,6100-6101,6106,6112,6123,6129,6156,6346,6389,6502,6510,6543,6547,6565-6567,6580,6646,6666-6669,6689,6692,6699,6779,6788-6789,6792,6839,6881,6901,6969,7000-7002,7004,7007,7019,7025,7070,7100,7103,7106,7200-7201,7402,7435,7443,7496,7512,7625,7627,7676,7741,7777-7778,7800,7911,7920-7921,7937-7938,7999-8002,8007-8011,8021-8022,8031,8042,8045,8080-8090,8093,8099-8100,8180-8181,8192-8194,8200,8222,8254,8290-8292,8300,8333,8383,8400,8402,8443,8500,8600,8649,8651-8652,8654,8701,8800,8873,8888,8899,8994,9000-9003,9009-9011,9040,9050,9071,9080-9081,9090-9091,9099-9103,9110-9111,9200,9207,9220,9290,9415,9418,9485,9500,9502-9503,9535,9575,9593-9595,9618,9666,9876-9878,9898,9900,9917,9929,9943-9944,9968,9998-10004,10009-10010,10012,10024-10025,10082,10180,10215,10243,10566,10616-10617,10621,10626,10628-10629,10778,11110-11111,11967,12000,12174,12265,12345,13456,13722,13782-13783,14000,14238,14441-14442,15000,15002-15004,15660,15742,16000-16001,16012,16016,16018,16080,16113,16992-16993,17877,17988,18040,18101,18988,19101,19283,19315,19350,19780,19801,19842,20000,20005,20031,20221-20222,20828,21571,22939,23502,24444,24800,25734-25735,26214,27000,27352-27353,27355-27356,27715,28201,30000,30718,30951,31038,31337,32768-32785,33354,33899,34571-34573,35500,38292,40193,40911,41511,42510,44176,44442-44443,44501,45100,48080,49152-49161,49163,49165,49167,49175-49176,49400,49999-50003,50006,50300,50389,50500,50636,50800,51103,51493,52673,52822,52848,52869,54045,54328,55055-55056,55555,55600,56737-56738,57294,57797,58080,60020,60443,61532,61900,62078,63331,64623,64680,65000,65129,65389"/>
        //          </extraports>
        //          <port protocol = "tcp" portid="21">
        //              <state state = "open" reason="syn-ack" reason_ttl="64"/>
        //              <service name = "ftp" method="table" conf="3"/>
        //          </port>
        //          <port protocol = "tcp" portid="111">
        //              <state state = "open" reason="syn-ack" reason_ttl="64"/>
        //              <service name = "rpcbind" method="table" conf="3"/>
        //          </port>
        //          <port protocol = "tcp" portid="139">
        //             <state state = "open" reason="syn-ack" reason_ttl="64"/>
        //             <service name = "netbios-ssn" method="table" conf="3"/>
        //          </port>
        //          <port protocol = "tcp" portid="445">
        //             <state state = "open" reason="syn-ack" reason_ttl="64"/>
        //             <service name = "microsoft-ds" method="table" conf="3"/>
        //          </port>
        //    </ports>

        // INTERNET

        //<host starttime = "1718188145" endtime="1718188161">
        //    <status state = "up" reason="syn-ack" reason_ttl="123"/>
        //    <address addr = "8.8.8.8" addrtype="ipv4"/>
        //    <hostnames>
        //          <hostname name = "dns.google" type="PTR"/>
        //    </hostnames>

        //  <ports>
        //      <extraports state = "filtered" count="998">
        //            <extrareasons reason = "no-response" count="998" proto="tcp" ports="1,3-4,6-7,9,13,17,19-26,30,32-33,37,42-43,49,70,79-85,88-90,99-100,106,109-111,113,119,125,135,139,143-144,146,161,163,179,199,211-212,222,254-256,259,264,280,301,306,311,340,366,389,406-407,416-417,425,427,444-445,458,464-465,481,497,500,512-515,524,541,543-545,548,554-555,563,587,593,616-617,625,631,636,646,648,666-668,683,687,691,700,705,711,714,720,722,726,749,765,777,783,787,800-801,808,843,873,880,888,898,900-903,911-912,981,987,990,992-993,995,999-1002,1007,1009-1011,1021-1100,1102,1104-1108,1110-1114,1117,1119,1121-1124,1126,1130-1132,1137-1138,1141,1145,1147-1149,1151-1152,1154,1163-1166,1169,1174-1175,1183,1185-1187,1192,1198-1199,1201,1213,1216-1218,1233-1234,1236,1244,1247-1248,1259,1271-1272,1277,1287,1296,1300-1301,1309-1311,1322,1328,1334,1352,1417,1433-1434,1443,1455,1461,1494,1500-1501,1503,1521,1524,1533,1556,1580,1583,1594,1600,1641,1658,1666,1687-1688,1700,1717-1721,1723,1755,1761,1782-1783,1801,1805,1812,1839-1840,1862-1864,1875,1900,1914,1935,1947,1971-1972,1974,1984,1998-2010,2013,2020-2022,2030,2033-2035,2038,2040-2043,2045-2049,2065,2068,2099-2100,2103,2105-2107,2111,2119,2121,2126,2135,2144,2160-2161,2170,2179,2190-2191,2196,2200,2222,2251,2260,2288,2301,2323,2366,2381-2383,2393-2394,2399,2401,2492,2500,2522,2525,2557,2601-2602,2604-2605,2607-2608,2638,2701-2702,2710,2717-2718,2725,2800,2809,2811,2869,2875,2909-2910,2920,2967-2968,2998,3000-3001,3003,3005-3006,3011,3017,3030-3031,3052,3071,3077,3128,3168,3211,3221,3260-3261,3268-3269,3283,3300-3301,3306,3322-3325,3333,3351,3367,3369-3372,3389-3390,3404,3476,3493,3517,3527,3546,3551,3580,3659,3689-3690,3703,3737,3766,3784,3800-3801,3809,3814,3826-3828,3851,3869,3871,3878,3880,3889,3905,3914,3918,3920,3945,3971,3986,3995,3998,4000-4006,4045,4111,4125-4126,4129,4224,4242,4279,4321,4343,4443-4446,4449,4550,4567,4662,4848,4899-4900,4998,5000-5004,5009,5030,5033,5050-5051,5054,5060-5061,5080,5087,5100-5102,5120,5190,5200,5214,5221-5222,5225-5226,5269,5280,5298,5357,5405,5414,5431-5432,5440,5500,5510,5544,5550,5555,5560,5566,5631,5633,5666,5678-5679,5718,5730,5800-5802,5810-5811,5815,5822,5825,5850,5859,5862,5877,5900-5904,5906-5907,5910-5911,5915,5922,5925,5950,5952,5959-5963,5985-5989,5998-6007,6009,6025,6059,6100-6101,6106,6112,6123,6129,6156,6346,6389,6502,6510,6543,6547,6565-6567,6580,6646,6666-6669,6689,6692,6699,6779,6788-6789,6792,6839,6881,6901,6969,7000-7002,7004,7007,7019,7025,7070,7100,7103,7106,7200-7201,7402,7435,7443,7496,7512,7625,7627,7676,7741,7777-7778,7800,7911,7920-7921,7937-7938,7999-8002,8007-8011,8021-8022,8031,8042,8045,8080-8090,8093,8099-8100,8180-8181,8192-8194,8200,8222,8254,8290-8292,8300,8333,8383,8400,8402,8443,8500,8600,8649,8651-8652,8654,8701,8800,8873,8888,8899,8994,9000-9003,9009-9011,9040,9050,9071,9080-9081,9090-9091,9099-9103,9110-9111,9200,9207,9220,9290,9415,9418,9485,9500,9502-9503,9535,9575,9593-9595,9618,9666,9876-9878,9898,9900,9917,9929,9943-9944,9968,9998-10004,10009-10010,10012,10024-10025,10082,10180,10215,10243,10566,10616-10617,10621,10626,10628-10629,10778,11110-11111,11967,12000,12174,12265,12345,13456,13722,13782-13783,14000,14238,14441-14442,15000,15002-15004,15660,15742,16000-16001,16012,16016,16018,16080,16113,16992-16993,17877,17988,18040,18101,18988,19101,19283,19315,19350,19780,19801,19842,20000,20005,20031,20221-20222,20828,21571,22939,23502,24444,24800,25734-25735,26214,27000,27352-27353,27355-27356,27715,28201,30000,30718,30951,31038,31337,32768-32785,33354,33899,34571-34573,35500,38292,40193,40911,41511,42510,44176,44442-44443,44501,45100,48080,49152-49161,49163,49165,49167,49175-49176,49400,49999-50003,50006,50300,50389,50500,50636,50800,51103,51493,52673,52822,52848,52869,54045,54328,55055-55056,55555,55600,56737-56738,57294,57797,58080,60020,60443,61532,61900,62078,63331,64623,64680,65000,65129,65389"/>
        //      </extraports>
        //      <port protocol = "tcp" portid="53">
        //            <state state = "open" reason="syn-ack" reason_ttl="109"/>
        //            <service name = "domain" method="table" conf="3"/>
        //      </port>
        //      <port protocol = "tcp" portid="443">
        //            <state state = "open" reason="syn-ack" reason_ttl="123"/>
        //            <service name = "https" method="table" conf="3"/>
        //      </port>
        //</ports>
        #endregion

        public override string ToString()
        {
            return (String.Format("{0}; {1}; {2}; {3};", IP, MAC, Vendor, Host));
        }
     }

    public static class NMap_ResultXML_Parser
    {

        /// <summary>
        /// Parser NMap result XML file
        /// </summary>
        /// <param name="path_file_xml">Path to XML file</param>
        /// <returns>Кортеж( string, ScanItem ) Описание ошибки, Результат сканирования в структуре ScanItem</returns>

        public static async Task<(string, ScanItem)> ParsingFileXML_async(string path_file_xml)
        {
            // Файл с данными это результат сканирования.
            // Есть варианты записи файлов результата
            // 1. Каждое сканирование выводит результат в один и тоже файл, и тогда мы должны сразу его обработать, распарсить и сохранить в базу
            // 2. Каждое Сканирование выводит результат в свой файл.
            // Проблемы с 1 вариантом: сканированиеможет быть быстрее, чем обработка данных из файла, и данные перезапишуться.
            // Проблемы с 2 вариантом: Будет сохраняться слишком много файлов.
            // Что предпочтительнее не оговорено, поэтому принимаем к исполнению 1 вариант

            string   error      = String.Empty;
            string   file_text  = "";
            ScanItem scanItem   = new ScanItem();
            scanItem.Ports_list = new List<Port>();

            #region // Проверка входных данных
            // Проверка пустого пути
            if (String.IsNullOrWhiteSpace(path_file_xml))
            {
                error = "Путь к файлу не может быть пустым";
                return (error, scanItem);
            }

            // Проверка наличия файла
            if (!File.Exists(path_file_xml))
            {
                error = "Файл по указанному пути:" + path_file_xml + " не найден.";
                return (error, scanItem);
            }

            // Считываем весь текст из файла

            // Считываем из файла всесь текст
            using (StringReader stringReader = new StringReader(path_file_xml))
            {
                file_text = await stringReader.ReadToEndAsync();
            }

            // Проверка XML формата файла
            Regex rgx_xlm = new Regex(@"^<.+?xml version.+?>", RegexOptions.None);

            if (!rgx_xlm.IsMatch(file_text))
            {
                error = "Указанный файл:" + path_file_xml + " не в XML формате.";
                return (error, scanItem);
            }
            #endregion


            #region // Парсинг данных

            // Выбираем текст блока <host... </host>
            Regex rgx_block_host   = new Regex(@"<host.+?>(.+?)</host>", RegexOptions.None);
            Match m_rgx_block_host = rgx_block_host.Match(file_text);

            string block_host_txt = "";
            if ( m_rgx_block_host.Success )
            {
                // Найден блок <host... </host>
                block_host_txt = m_rgx_block_host.Groups[1].Value;
            }

            #region // Выбираем нужные параметры из этого блока

            // Получаем параметры тэга STATUS в виде кортежа значений (state, reason )
            (string, string) status_params = Parsing_tag_status( block_host_txt );

            scanItem.Status_state  = status_params.Item1;
            scanItem.Status_reason = status_params.Item2;

            // Получаем параметры тэгов ADDRESS в виде кортежа значений (IP, MAC, vendor)
            (string, string, string) address_params = Parsing_tag_address( block_host_txt );

            scanItem.IP     = address_params.Item1;
            scanItem.MAC    = address_params.Item2;
            scanItem.Vendor = address_params.Item3;

            // Получаем список портов с параметрами
            scanItem.Ports_list = Parsing_tag_ports( block_host_txt );

            #endregion

            #endregion

            return (error, scanItem);
        }

        /// <summary>
        /// Парсер значений параметров в тэге Status
        /// </summary>
        /// <param name="block_host_txt">Текст в блоке host</param>
        /// <returns>(state, reason)</returns>
        private static (string, string) Parsing_tag_status( string block_host_txt)
        {
            #region  // Пример вывода результатов работы NMAP  в виде XML текста тэга STATUS
            // <status state="up" reason="arp-response" reason_ttl="0"/>
            #endregion

            string prm_state  = "";
            string prm_reason = "";

            // Ищем в блоке тэг Status
            Regex rgx_tag_status   = new Regex(@"<status (.+?)/>", RegexOptions.None);
            Match m_rgx_tag_status = rgx_tag_status.Match(block_host_txt);

            string tag_status_txt = "";
            if (m_rgx_tag_status.Success)
            {
                // Найден блок <status.../>
                // Получаем текст в тэге
                tag_status_txt = m_rgx_tag_status.Groups[1].Value;

                #region // Ищем значение параметра state 
                Regex rgx_prm_state   = new Regex("state=\"(.+?)\"", RegexOptions.None);
                Match m_rgx_prm_state = rgx_prm_state.Match(tag_status_txt);

                if (m_rgx_prm_state.Success)
                {
                    prm_state = m_rgx_prm_state.Groups[1].Value;
                }
                #endregion

                #region // Ищем значение параметра reason 
                Regex rgx_prm_reason   = new Regex("reason=\"(.+?)\"", RegexOptions.None);
                Match m_rgx_prm_reason = rgx_prm_reason.Match(tag_status_txt);

                if (m_rgx_prm_reason.Success)
                {
                    prm_reason = m_rgx_prm_reason.Groups[1].Value;
                }
                #endregion 

            }

            // Возвращаем кортеж параметров
            return ( prm_state, prm_reason );
        }


        /// <summary>
        /// Парсер значений параметров в тэге address
        /// </summary>
        /// <param name="block_host_txt">Текст в блоке host</param>
        /// <returns>( IP, MAC, vendor )</returns>
        private static (string, string, string) Parsing_tag_address( string block_host_txt )
        {
            // Тэгов address может быть несколько
            #region  // Пример вывода результатов работы NMAP  в виде XML текста тэга ADDRESS
            //< address addr = "10.0.0.10" addrtype = "ipv4" />
            //< address addr = "D4:3D:7E:EA:7B:CF" addrtype = "mac" vendor = "Micro-Star Int&apos;l" />
            #endregion

            string IP     = "";
            string MAC    = "";
            string vendor = "";

            // Ищем в блоке тэг Status
            Regex           rgx_tag_address    = new Regex(@"<address (.+?)/>", RegexOptions.None);
            MatchCollection mc_rgx_tag_address = rgx_tag_address.Matches(block_host_txt);

            // Перебираем все найденные тэги address.
            foreach ( Match m_rgx_tag_address in mc_rgx_tag_address )
            {
                if ( m_rgx_tag_address.Success )
                {
                    string prm_addr     = "";
                    string prm_addrtype = "";
                    
                    // Получаем текст в тэге address
                    string tag_address_txt = m_rgx_tag_address.Groups[1].Value;


                    #region // Ищем значение параметра addr
                    Regex rgx_prm_addr   = new Regex( "addr=\"(.+?)\"", RegexOptions.None );
                    Match m_rgx_prm_addr = rgx_prm_addr.Match( tag_address_txt );

                    if (m_rgx_prm_addr.Success)
                    {
                        prm_addr = m_rgx_prm_addr.Groups[1].Value;
                    }
                    #endregion


                    #region // Ищем значение параметра addrtype
                    Regex rgx_prm_addrtype   = new Regex( "addrtype=\"(.+?)\"", RegexOptions.None );
                    Match m_rgx_prm_addrtype = rgx_prm_addrtype.Match( tag_address_txt );

                    if ( m_rgx_prm_addrtype.Success )
                    {
                        prm_addrtype = m_rgx_prm_addrtype.Groups[1].Value;
                    }
                    #endregion 


                    #region // Ищем значение параметра vendor
                    Regex rgx_prm_vendor   = new Regex( "vendor=\"(.+?)\"", RegexOptions.None );
                    Match m_rgx_prm_vendor = rgx_prm_vendor.Match( tag_address_txt );

                    if (m_rgx_prm_vendor.Success)
                    {
                        vendor = m_rgx_prm_vendor.Groups[1].Value;
                    }
                    #endregion 
                
                    // Нужные параметры находятся в разных тэгах address
                    // Поэтому сохраняем их согласно типу адреса
                    switch( prm_addrtype )
                    {
                        case "ipv4":
                        case "ipv6":
                            IP = prm_addr;
                            break;

                        case "mac":
                            MAC = "";
                            break;
                    }                
                }
            }


            // Возвращаем кортеж параметров
            return ( IP, MAC, vendor );
        }


        /// <summary>
        /// Парсер значений параметров портов
        /// </summary>
        /// <param name="block_host_txt">Текст в блоке host</param>
        /// <returns>List&lt;Port&gt;</returns>
        private static List<Port> Parsing_tag_ports( string block_host_txt )
        {
            // Тэгов port может быть несколько
            #region  // Пример вывода результатов работы NMAP  в виде XML текста тэга PORTS
            //< ports >
            //      < extraports state="closed" count="996" >
            //            < extrareasons reason="reset" count="996" proto="tcp" ports="1,3-4,6-7,9,13,17,19-20,22-26,30,32-33,37,42-43,49,53,70,79-85,88-90,99-100,106,109-110,113,119,125,135,143-144,146,161,163,179,199,211-212,222,254-256,259,264,280,301,306,311,340,366,389,406-407,416-417,425,427,443-444,458,464-465,481,497,500,512-515,524,541,543-545,548,554-555,563,587,593,616-617,625,631,636,646,648,666-668,683,687,691,700,705,711,714,720,722,726,749,765,777,783,787,800-801,808,843,873,880,888,898,900-903,911-912,981,987,990,992-993,995,999-1002,1007,1009-1011,1021-1100,1102,1104-1108,1110-1114,1117,1119,1121-1124,1126,1130-1132,1137-1138,1141,1145,1147-1149,1151-1152,1154,1163-1166,1169,1174-1175,1183,1185-1187,1192,1198-1199,1201,1213,1216-1218,1233-1234,1236,1244,1247-1248,1259,1271-1272,1277,1287,1296,1300-1301,1309-1311,1322,1328,1334,1352,1417,1433-1434,1443,1455,1461,1494,1500-1501,1503,1521,1524,1533,1556,1580,1583,1594,1600,1641,1658,1666,1687-1688,1700,1717-1721,1723,1755,1761,1782-1783,1801,1805,1812,1839-1840,1862-1864,1875,1900,1914,1935,1947,1971-1972,1974,1984,1998-2010,2013,2020-2022,2030,2033-2035,2038,2040-2043,2045-2049,2065,2068,2099-2100,2103,2105-2107,2111,2119,2121,2126,2135,2144,2160-2161,2170,2179,2190-2191,2196,2200,2222,2251,2260,2288,2301,2323,2366,2381-2383,2393-2394,2399,2401,2492,2500,2522,2525,2557,2601-2602,2604-2605,2607-2608,2638,2701-2702,2710,2717-2718,2725,2800,2809,2811,2869,2875,2909-2910,2920,2967-2968,2998,3000-3001,3003,3005-3006,3011,3017,3030-3031,3052,3071,3077,3128,3168,3211,3221,3260-3261,3268-3269,3283,3300-3301,3306,3322-3325,3333,3351,3367,3369-3372,3389-3390,3404,3476,3493,3517,3527,3546,3551,3580,3659,3689-3690,3703,3737,3766,3784,3800-3801,3809,3814,3826-3828,3851,3869,3871,3878,3880,3889,3905,3914,3918,3920,3945,3971,3986,3995,3998,4000-4006,4045,4111,4125-4126,4129,4224,4242,4279,4321,4343,4443-4446,4449,4550,4567,4662,4848,4899-4900,4998,5000-5004,5009,5030,5033,5050-5051,5054,5060-5061,5080,5087,5100-5102,5120,5190,5200,5214,5221-5222,5225-5226,5269,5280,5298,5357,5405,5414,5431-5432,5440,5500,5510,5544,5550,5555,5560,5566,5631,5633,5666,5678-5679,5718,5730,5800-5802,5810-5811,5815,5822,5825,5850,5859,5862,5877,5900-5904,5906-5907,5910-5911,5915,5922,5925,5950,5952,5959-5963,5985-5989,5998-6007,6009,6025,6059,6100-6101,6106,6112,6123,6129,6156,6346,6389,6502,6510,6543,6547,6565-6567,6580,6646,6666-6669,6689,6692,6699,6779,6788-6789,6792,6839,6881,6901,6969,7000-7002,7004,7007,7019,7025,7070,7100,7103,7106,7200-7201,7402,7435,7443,7496,7512,7625,7627,7676,7741,7777-7778,7800,7911,7920-7921,7937-7938,7999-8002,8007-8011,8021-8022,8031,8042,8045,8080-8090,8093,8099-8100,8180-8181,8192-8194,8200,8222,8254,8290-8292,8300,8333,8383,8400,8402,8443,8500,8600,8649,8651-8652,8654,8701,8800,8873,8888,8899,8994,9000-9003,9009-9011,9040,9050,9071,9080-9081,9090-9091,9099-9103,9110-9111,9200,9207,9220,9290,9415,9418,9485,9500,9502-9503,9535,9575,9593-9595,9618,9666,9876-9878,9898,9900,9917,9929,9943-9944,9968,9998-10004,10009-10010,10012,10024-10025,10082,10180,10215,10243,10566,10616-10617,10621,10626,10628-10629,10778,11110-11111,11967,12000,12174,12265,12345,13456,13722,13782-13783,14000,14238,14441-14442,15000,15002-15004,15660,15742,16000-16001,16012,16016,16018,16080,16113,16992-16993,17877,17988,18040,18101,18988,19101,19283,19315,19350,19780,19801,19842,20000,20005,20031,20221-20222,20828,21571,22939,23502,24444,24800,25734-25735,26214,27000,27352-27353,27355-27356,27715,28201,30000,30718,30951,31038,31337,32768-32785,33354,33899,34571-34573,35500,38292,40193,40911,41511,42510,44176,44442-44443,44501,45100,48080,49152-49161,49163,49165,49167,49175-49176,49400,49999-50003,50006,50300,50389,50500,50636,50800,51103,51493,52673,52822,52848,52869,54045,54328,55055-55056,55555,55600,56737-56738,57294,57797,58080,60020,60443,61532,61900,62078,63331,64623,64680,65000,65129,65389" />
            //      </ extraports >
            //      <port protocol="tcp" portid="21">
            //          <state state="open" reason="syn-ack" reason_ttl="64"/>
            //          <service name="ftp" method="table" conf="3"/>
            //      </port >
            //      <port protocol="tcp" portid="111"><state state="open" reason="syn-ack" reason_ttl="64"/><service name="rpcbind" method="table" conf="3"/></port >
            //      <port protocol="tcp" portid="139"><state state="open" reason="syn-ack" reason_ttl="64"/><service name="netbios-ssn" method="table" conf="3"/></port >
            //      <port protocol="tcp" portid="445"><state state="open" reason="syn-ack" reason_ttl="64"/><service name="microsoft-ds" method="table" conf="3"/></port >
            //</ports>
            #endregion

            // Срисок распарсенных портов
            List<Port> list_ports = new List<Port>();

            string block_ports_txt = "";

            // Ищем в блоке тэг PORTS
            Regex rgx_block_ports    = new Regex( @"<ports (.+?)</ports>", RegexOptions.None );
            Match m_rgx_block_ports  = rgx_block_ports.Match( block_host_txt );
            if ( m_rgx_block_ports.Success )
            {
                block_ports_txt = m_rgx_block_ports.Groups[1].Value;
            }


            // Ищем в блоке ports тэги PORT
            Regex           rgx_block_port    = new Regex( @"<port .+?</port>", RegexOptions.None );
            MatchCollection mc_rgx_block_port = rgx_block_port.Matches( block_ports_txt );

            // Перебираем все найденные PORT.
            foreach ( Match m_rgx_block_port in mc_rgx_block_port)
            {
                if ( m_rgx_block_port.Success )
                {
                    // Весь текст с тэгом PORT и тэгами внутри его
                    string block_port_txt = m_rgx_block_port.Groups[0].Value;

                    // Значения параметров тэгов которые будут распарсиваться
                    string prm_port_protocol    = "";
                    string prm_port_portid      = "";

                    string prm_state_state      = "";
                    string prm_state_reason     = "";
                    string prm_state_reason_ttl = "";

                    string prm_service_name     = "";
                    string prm_service_method   = "";
                    string prm_service_conf     = "";

                    
                    #region  // Ищем параметры в тэге PORT
                    string tag_port_prms_txt     = "";
                    Regex  rgx_tag_port_params   = new Regex(@"<port (.+?)>", RegexOptions.None);
                    Match  m_rgx_tag_port_params = rgx_tag_port_params.Match( block_port_txt );
                    if ( m_rgx_tag_port_params.Success)
                    {
                        tag_port_prms_txt = m_rgx_tag_port_params.Groups[1].Value;

                        #region // Параметр protocol тэга PORT
                        Regex rgx_prm_protocol   = new Regex("protocol=\"(.+?)\"", RegexOptions.None);
                        Match m_rgx_prm_protocol = rgx_prm_protocol.Match( tag_port_prms_txt );

                        if (m_rgx_prm_protocol.Success)
                        {
                            prm_port_protocol = m_rgx_prm_protocol.Groups[1].Value;
                        }
                        #endregion

                        #region // Параметр portid тэга PORT
                        Regex rgx_prm_portid   = new Regex("portid=\"(.+?)\"", RegexOptions.None);
                        Match m_rgx_prm_portid = rgx_prm_portid.Match(tag_port_prms_txt);

                        if ( m_rgx_prm_portid.Success )
                        {
                            prm_port_portid = m_rgx_prm_portid.Groups[1].Value;
                        }
                        #endregion
                    }
                    #endregion


                    #region  // Ищем параметры в тэге STATE
                    Regex  rgx_tag_state_params   = new Regex( @"<state (.+?)/>", RegexOptions.None );
                    Match  m_rgx_tag_state_params = rgx_tag_state_params.Match( block_port_txt );
                    if ( m_rgx_tag_state_params.Success )
                    {
                        string tag_state_prms_txt = m_rgx_tag_state_params.Groups[1].Value;

                        #region // Параметр state тэга STATE
                        Regex rgx_prm_state   = new Regex("state=\"(.+?)\"", RegexOptions.None);
                        Match m_rgx_prm_state = rgx_prm_state.Match( tag_state_prms_txt );

                        if ( m_rgx_prm_state.Success )
                        {
                            prm_state_state = m_rgx_prm_state.Groups[1].Value;
                        }
                        #endregion

                        #region // Параметр reason тэга STATE
                        Regex rgx_prm_reason   = new Regex("reason=\"(.+?)\"", RegexOptions.None);
                        Match m_rgx_prm_reason = rgx_prm_reason.Match( tag_state_prms_txt );

                        if (m_rgx_prm_reason.Success)
                        {
                            prm_state_reason = m_rgx_prm_reason.Groups[1].Value;
                        }
                        #endregion

                        #region // Параметр reason_ttl тэга STATE
                        Regex rgx_prm_reason_ttl   = new Regex("reason_ttl=\"(.+?)\"", RegexOptions.None);
                        Match m_rgx_prm_reason_ttl = rgx_prm_reason_ttl.Match(tag_state_prms_txt);

                        if (m_rgx_prm_reason_ttl.Success)
                        {
                            prm_state_reason_ttl = m_rgx_prm_reason_ttl.Groups[1].Value;
                        }
                        #endregion
                    }
                    #endregion


                    #region  // Ищем параметры в тэге SERVICE
                    string tag_service_prms_txt     = "";
                    Regex  rgx_tag_service_params   = new Regex(@"<service (.+?)/>", RegexOptions.None);
                    Match  m_rgx_tag_service_params = rgx_tag_service_params.Match(block_port_txt);
                    if (m_rgx_tag_service_params.Success)
                    {
                        tag_service_prms_txt = m_rgx_tag_service_params.Groups[1].Value;

                        #region // Параметр name тэга SERVICE
                        Regex rgx_prm_name   = new Regex( "name=\"(.+?)\"", RegexOptions.None );
                        Match m_rgx_prm_name = rgx_prm_name.Match( tag_service_prms_txt );

                        if (m_rgx_prm_name.Success)
                        {
                            prm_service_name = m_rgx_prm_name.Groups[1].Value;
                        }
                        #endregion

                        #region // Параметр method тэга SERVICE
                        Regex rgx_prm_method   = new Regex("method=\"(.+?)\"", RegexOptions.None);
                        Match m_rgx_prm_method = rgx_prm_method.Match(tag_service_prms_txt);

                        if (m_rgx_prm_method.Success)
                        {
                            prm_service_method = m_rgx_prm_method.Groups[1].Value;
                        }
                        #endregion

                        #region // Параметр conf тэга SERVICE
                        Regex rgx_prm_conf   = new Regex( "conf=\"(.+?)\"", RegexOptions.None );
                        Match m_rgx_prm_conf = rgx_prm_conf.Match( tag_service_prms_txt );

                        if ( m_rgx_prm_conf.Success )
                        {
                            prm_service_conf = m_rgx_prm_conf.Groups[1].Value;
                        }
                        #endregion
                    }
                    #endregion


                    Port port = new Port();
                    port.ID        = Convert.ToInt32( prm_port_portid );
                    port.Protocol  = prm_port_protocol;
                    port.Method    = prm_service_method;
                    port.Service   = prm_service_name;
                    port.State     = prm_state_state;
                    port.Reason    = prm_state_reason;

                    list_ports.Add( port );
                }
            }

            // Возвращаем распарсенные параметры порта
            return list_ports;
        }



    }



}
