using Microsoft.AspNetCore.Html;
using System.Text;

namespace Altex.Utils
{
    [Serializable]
    //*************************
    public struct Paging
    //*************************
    {
        public int curr_page     { get; set; }
        public int full_quantity { get; set; }
        public int numb_on_page  { get; set; }
        public int selector      { get; set; }

        public override string ToString()
        {
            return String.Format("{0}/{1}; qnt:{2}; slct:{3}", curr_page.ToString(), numb_on_page.ToString(), full_quantity.ToString(), selector.ToString());
        }

        public string ToStringDB()
        {
            return String.Format("{0};{1};{2};{3}", curr_page.ToString(), full_quantity.ToString(), numb_on_page.ToString(), selector.ToString());
        }
    }

    [Serializable]
    //*************************
    public struct Buttons
    //*************************
    {
        public int step_prev_numb { get; set; }
        public int step_prev_long { get; set; }
        public int step_next_numb { get; set; }
        public int step_next_long { get; set; }
        public int cnt_start     { get; set; }
        public int cnt_finish    { get; set; }
        public int pg_prev       { get; set; }
        public int pg_next       { get; set; }
        public int max_numb      { get; set; }

    }

    public static class Pagination
    {
        //*************************
        public static Paging get_new_paging()
        //*************************
        {
            Paging paging = new Paging();
            paging.curr_page = 1;
            paging.full_quantity = 0;
            paging.numb_on_page = SettingsUtils.get_numb_on_page(0);
            paging.selector = 0;
            return paging;
        }

        //*************************
        public static Paging get_paging(int numb_prods)
        //*************************
        {
            Paging paging = new Paging();
            paging.curr_page = 1;
            paging.full_quantity = numb_prods;
            paging.numb_on_page = SettingsUtils.get_numb_on_page(0);
            paging.selector = 1;
            return paging;
        }

        //*************************
        public static Buttons get_params(Paging paging)
        //*************************
        {
            int full_numb = paging.full_quantity / paging.numb_on_page;
            if (paging.full_quantity % paging.numb_on_page != 0) full_numb = full_numb + 1;

            Buttons buts = new Buttons();
            buts.cnt_start = 1;
            buts.cnt_finish = full_numb;
            buts.max_numb = full_numb;
            buts.step_prev_numb = -1;
            buts.step_prev_long = -1;
            buts.step_next_numb = -1;
            buts.step_next_long = -1;

            buts.pg_prev = paging.curr_page - 1;
            if (buts.pg_prev < 1) { buts.pg_prev = 1; }

            buts.pg_next = paging.curr_page + 1;
            if (buts.pg_next > full_numb) { buts.pg_next = full_numb; }

            int half_numb = paging.numb_on_page / 2 + 1;
            int bound_prev = paging.curr_page - half_numb;
            int bound_next = paging.curr_page + half_numb;

            // Устанавливаем PREV переходы страниц
            if (bound_prev <= 1)
            {
                buts.cnt_start = 1;
            }
            else if (bound_prev <= paging.numb_on_page)
            {
                buts.cnt_start = bound_prev;
                if (bound_prev >= half_numb) { buts.step_prev_numb = bound_prev / 2; }
            }
            else
            {
                buts.cnt_start = bound_prev;
                buts.step_prev_numb = bound_prev - paging.numb_on_page;
                if (buts.step_prev_numb >= half_numb) { buts.step_prev_long = buts.step_prev_numb / 2; }
            }

            // Устанавливаем NEXT переходы страниц
            int ostatok_next = full_numb - bound_next;
            if (bound_next >= full_numb)
            {
                buts.cnt_finish = full_numb;
            }
            else if (ostatok_next <= paging.numb_on_page)
            {
                buts.cnt_finish = bound_next;
                if (ostatok_next >= half_numb) { buts.step_next_numb = bound_next + ostatok_next / 2; }
            }
            else
            {
                buts.cnt_finish = bound_next;
                buts.step_next_numb = bound_next + paging.numb_on_page;
                int ost = full_numb - buts.step_next_numb;
                if (ost >= half_numb) { buts.step_next_long = buts.step_next_numb + ost / 2; }
            }
            return buts;
        }

        //*************************
        public static HtmlString Render_Pagination(Paging pagination)
        //*************************
        {
            Buttons buts = Pagination.get_params(pagination);
            List<int> list_numb_on_page = SettingsUtils.get_list_numb_on_page();

            StringBuilder strBld = new StringBuilder();

            strBld.Append("<table id=\"tb_pagin\" border=\"0\" cellpadding=\"0\" cellspacing=\"3\">");
            strBld.Append("<tr id=\"tr_pagin\">");

            // Выводим кнопки перехода только когда есть переходы
            if (pagination.numb_on_page < pagination.full_quantity)
            {
                strBld.Append("<td class=\"td_pagin_go\"><div class=\"dv_pagin_go\" onclick=\"_mng.pgn(1)\" onmouseover=\"_mng.ipg(this)\" onmouseout=\"_mng.opg(this)\">&lt;&lt;&lt;</div></td>");
                strBld.Append("<td class=\"td_pagin_prev\"><div class=\"dv_pagin_go\" onclick=\"_mng.pgn(").Append(buts.pg_prev).Append(")\" onmouseover=\"_mng.ipg(this)\" onmouseout=\"_mng.opg(this)\">&lt;</div></td>");

                for (int i = buts.cnt_start; i <= buts.cnt_finish; i++)
                {
                    string dvclass = "dv_pagin_num";
                    if (i == pagination.curr_page) { dvclass = "dv_pagin_num_sl"; }

                    strBld.Append("<td class=\"td_pagin_num\"><div id=\"pgn").Append(i).Append("\" class=\"").Append(dvclass).Append("\" onclick=\"_mng.pgn(").Append(i).Append(")\" onmouseover=\"_mng.ipg(this)\" onmouseout=\"_mng.opg(this)\">").Append(i).Append("</div></td>");
                }

                strBld.Append("<td class=\"td_pagin_next\"><div class=\"dv_pagin_go\" onclick=\"_mng.pgn(").Append(buts.pg_next).Append(")\" onmouseover=\"_mng.ipg(this)\" onmouseout=\"_mng.opg(this)\">&gt;</div></td>");
                strBld.Append("<td class=\"td_pagin_go\"><div class=\"dv_pagin_go\" onclick=\"_mng.pgn(").Append(buts.max_numb).Append(")\" onmouseover=\"_mng.ipg(this)\" onmouseout=\"_mng.opg(this)\">&gt;&gt;&gt;</div></td>");
            }

            #region =========================================================// Вставляем селектор выбора количества строк для вывода

            strBld.Append("<td class=\"td_pagin_numbpg\">");

            //Dictionary<string, string> dct_numb_row = new Dictionary<string, string>() { { "3", "3" }, { "10", "10" }, { "20", "20" }, { "50", "50" }, { "100", "100" } };

            // Если количество строк без ограничений

            strBld.Append("<select id=\"sl_paginat_nmb_pg\"");
            strBld.Append(" onchange=\"_mng.pgn_nmb_pg()\"");
            strBld.Append(">");
            foreach (int numb in list_numb_on_page)
            {
                strBld.Append("<option value=\"");
                strBld.Append(numb);
                strBld.Append("\"");
                if (numb == pagination.numb_on_page)
                {
                    strBld.Append(" selected");
                }
                strBld.Append(">");
                strBld.Append(numb);
                strBld.Append("</option>");
            }
            strBld.Append("</select>");

            strBld.Append("</td>");
            #endregion

            strBld.Append("<td><i>Всего надено:&nbsp;</i><b>").Append(pagination.full_quantity).Append("</b></td>");

            strBld.Append("</tr>");
            strBld.Append("</table>");

            return new HtmlString(strBld.ToString());
        }


        //*************************
        public static HtmlString Render_Pagination(Paging pagination, string name_space)
        //*************************
        {
            Buttons   buts              = Pagination.get_params(pagination);
            List<int> list_numb_on_page = SettingsUtils.get_list_numb_on_page();

            StringBuilder strBld = new StringBuilder();

            strBld.Append("<table id=\"tb_pagin\" border=\"0\" cellpadding=\"0\" cellspacing=\"3\">");
            strBld.Append("<tr id=\"tr_pagin\">");

            // Выводим кнопки перехода только когда есть переходы
            if (pagination.numb_on_page < pagination.full_quantity)
            {
                strBld.Append("<td class=\"td_pagin_go\"><div class=\"dv_pagin_go\" onclick=\"").Append(name_space).Append(".pgn(1)\" onmouseover=\"").Append(name_space).Append(".ipg(this)\" onmouseout=\"").Append(name_space).Append(".opg(this)\">&lt;&lt;&lt;</div></td>");
                strBld.Append("<td class=\"td_pagin_prev\"><div class=\"dv_pagin_go\" onclick=\"").Append(name_space).Append(".pgn(").Append(buts.pg_prev).Append(")\" onmouseover=\"").Append(name_space).Append(".ipg(this)\" onmouseout=\"").Append(name_space).Append(".opg(this)\">&lt;</div></td>");

                for (int i = buts.cnt_start; i <= buts.cnt_finish; i++)
                {
                    string dvclass = "dv_pagin_num";
                    if (i == pagination.curr_page) { dvclass = "dv_pagin_num_sl"; }

                    strBld.Append("<td class=\"td_pagin_num\"><div id=\"pgn").Append(i).Append("\" class=\"").Append(dvclass).Append("\" onclick=\"").Append(name_space).Append(".pgn(").Append(i).Append(")\" onmouseover=\"").Append(name_space).Append(".ipg(this)\" onmouseout=\"").Append(name_space).Append(".opg(this)\">").Append(i).Append("</div></td>");
                }

                strBld.Append("<td class=\"td_pagin_next\"><div class=\"dv_pagin_go\" onclick=\"").Append(name_space).Append(".pgn(").Append(buts.pg_next).Append(")\" onmouseover=\"").Append(name_space).Append(".ipg(this)\" onmouseout=\"").Append(name_space).Append(".opg(this)\">&gt;</div></td>");
                strBld.Append("<td class=\"td_pagin_go\"><div class=\"dv_pagin_go\" onclick=\"").Append(name_space).Append(".pgn(").Append(buts.max_numb).Append(")\" onmouseover=\"").Append(name_space).Append(".ipg(this)\" onmouseout=\"").Append(name_space).Append(".opg(this)\">&gt;&gt;&gt;</div></td>");
            }

            #region =========================================================// Вставляем селектор выбора количества строк для вывода

            strBld.Append("<td class=\"td_pagin_numbpg\">");

            //Dictionary<string, string> dct_numb_row = new Dictionary<string, string>() { { "3", "3" }, { "10", "10" }, { "20", "20" }, { "50", "50" }, { "100", "100" } };

            // Если количество строк без ограничений

            strBld.Append("<select id=\"sl_paginat_nmb_pg\"");
            strBld.Append(" onchange=\"").Append(name_space).Append(".pgn_nmb_pg()\"");
            strBld.Append(">");
            foreach (int numb in list_numb_on_page)
            {
                strBld.Append("<option value=\"");
                strBld.Append(numb);
                strBld.Append("\"");
                if (numb == pagination.numb_on_page)
                {
                    strBld.Append(" selected");
                }
                strBld.Append(">");
                strBld.Append(numb);
                strBld.Append("</option>");
            }
            strBld.Append("</select>");

            strBld.Append("</td>");
            #endregion

            strBld.Append("<td><i>Всего надено:&nbsp;</i><b>").Append(pagination.full_quantity).Append("</b></td>");

            strBld.Append("</tr>");
            strBld.Append("</table>");

            return new HtmlString(strBld.ToString());
        }




        //*************************
        public static string Render_Pagination_with_url(string path_controller, Paging pagination )
        //*************************
        {
            Buttons buts = Pagination.get_params(pagination);
            List<int> list_numb_on_page = SettingsUtils.get_list_numb_on_page();

            StringBuilder strBld = new StringBuilder();

            string site_root_url = Startup._httpContextAccessor.HttpContext.Request.PathBase;

            //string url = "https://www." + Startup._serverRootPath + "/" + path_controller;
            string url = site_root_url + "/" + path_controller;


            strBld.Append("<table id=\"tb_pagin\" border=\"0\" cellpadding=\"0\" cellspacing=\"3\">");
            strBld.Append("<tr id=\"tr_pagin\">");

            // Выводим кнопки перехода только когда есть переходы
            if (pagination.numb_on_page < pagination.full_quantity)
            {
                //strBld.Append("<td class=\"td_pagin_go\"><div class=\"dv_pagin_go\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\"><a href=\"/").Append(url).Append("/1\">&lt;&lt;&lt;</a></div></td>");
                // Страницу с номером 1 не выводим в URL "1"
                strBld.Append("<td class=\"td_pagin_go\"><div class=\"dv_pagin_go\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\"><a href=\"").Append(url).Append("\">&lt;&lt;&lt;</a></div></td>");

                // Номер один в УРЛЕ не выводим
                string url_pg_prev = url + "/" + buts.pg_prev;

                if (buts.pg_prev == 1) url_pg_prev = url;
                strBld.Append("<td class=\"td_pagin_prev\"><div class=\"dv_pagin_go\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\"><a href=\"").Append(url_pg_prev).Append("\">&lt;</a></div></td>");

                for (int i = buts.cnt_start; i <= buts.cnt_finish; i++)
                {
                    string dvclass = "dv_pagin_num";
                    if (i == pagination.curr_page) { dvclass = "dv_pagin_num_sl"; }

                    // Номер один в УРЛЕ не выводим
                    string url_pg = url + "/" + i;
                    if (i == 1) url_pg = url;
                    strBld.Append("<td class=\"td_pagin_num\"><div id=\"pgn").Append(i).Append("\" class=\"").Append(dvclass).Append("\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\"><a href=\"").Append(url_pg).Append("\">").Append(i).Append("</a></div></td>");

                    // strBld.Append("<td class=\"td_pagin_num\"><div id=\"pgn").Append(i).Append("\" class=\"").Append(dvclass).Append("\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\"><a href=\"/").Append(url).Append("/").Append(i).Append("\">").Append(i).Append("</a></div></td>");
                }

                strBld.Append("<td class=\"td_pagin_next\"><div class=\"dv_pagin_go\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\"><a href=\"").Append(url).Append("/").Append(buts.pg_next).Append("\">&gt;</a></div></td>");
                strBld.Append("<td class=\"td_pagin_go\"><div class=\"dv_pagin_go\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\"><a href=\"").Append(url).Append("/").Append(buts.max_numb).Append("\">&gt;&gt;&gt;</a></div></td>");
            }

            #region =========================================================// Вставляем селектор выбора количества строк для вывода

            strBld.Append("<td class=\"td_pagin_numb_pg\">");


            strBld.Append("<select id=\"sl_paginat_nmb_pg\"");
            strBld.Append(" onchange=\"pgn_nmb_pg()\"");
            strBld.Append(">");
            foreach (int numb in list_numb_on_page)
            {
                strBld.Append("<option value=\"");
                strBld.Append(numb);
                strBld.Append("\"");
                if (numb == pagination.numb_on_page)
                {
                    strBld.Append(" selected");
                }
                strBld.Append(">");
                strBld.Append(numb);
                strBld.Append("</option>");
            }
            strBld.Append("</select>");

            strBld.Append("</td>");
            #endregion

            strBld.Append("<td class=\"td_pagin_total\">");
            strBld.Append("<i>Всего надено:</i><b>").Append(pagination.full_quantity).Append("</b>");
            strBld.Append("<input id=\"pgn_selector\" type=\"hidden\" value=\">").Append(pagination.selector).Append("\"");
            strBld.Append("</td>");

            strBld.Append("</tr>");
            strBld.Append("</table>");

            return strBld.ToString();
        }

        //*************************
        public static string Render_Pagination_pref(Paging pagination, string pref)
        //*************************
        {
            Buttons buts = Pagination.get_params(pagination);
            List<int> list_numb_on_page = SettingsUtils.get_list_numb_on_page();

            StringBuilder strBld = new StringBuilder();

            strBld.Append("<table id=\"tb_pagin\" border=\"0\" cellpadding=\"0\" cellspacing=\"3\">");
            strBld.Append("<tr id=\"tr_pagin\">");

            // Выводим кнопки перехода только когда есть переходы
            if (pagination.numb_on_page < pagination.full_quantity)
            {
                strBld.Append("<td class=\"td_pagin_go\"><div class=\"dv_pagin_go\" onclick=\"pgn").Append(pref).Append("(1)\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\">&lt;&lt;&lt;</div></td>");
                strBld.Append("<td class=\"td_pagin_prev\"><div class=\"dv_pagin_go\" onclick=\"pgn").Append(pref).Append("(").Append(buts.pg_prev).Append(")\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\">&lt;</div></td>");

                for (int i = buts.cnt_start; i <= buts.cnt_finish; i++)
                {
                    string dvclass = "dv_pagin_num";
                    if (i == pagination.curr_page) { dvclass = "dv_pagin_num_sl"; }

                    strBld.Append("<td class=\"td_pagin_num\"><div id=\"pgn").Append(i).Append("\" class=\"").Append(dvclass).Append("\" onclick=\"pgn").Append(pref).Append("(").Append(i).Append(")\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\">").Append(i).Append("</div></td>");
                }

                strBld.Append("<td class=\"td_pagin_next\"><div class=\"dv_pagin_go\" onclick=\"pgn").Append(pref).Append("(").Append(buts.pg_next).Append(")\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\">&gt;</div></td>");
                strBld.Append("<td class=\"td_pagin_go\"><div class=\"dv_pagin_go\" onclick=\"pgn").Append(pref).Append("(").Append(buts.max_numb).Append(")\" onmouseover=\"ipg(this)\" onmouseout=\"opg(this)\">&gt;&gt;&gt;</div></td>");
            }

            #region =========================================================// Вставляем селектор выбора количества строк для вывода

            strBld.Append("<td class=\"td_pagin_numbpg\">");

            strBld.Append("<select id=\"sl_paginat_nmb_pg\"");
            strBld.Append(" onchange=\"pgn_nmb_pg").Append(pref).Append("()\"");
            strBld.Append(">");
            foreach (int numb in list_numb_on_page)
            {
                strBld.Append("<option value=\"");
                strBld.Append(numb);
                strBld.Append("\"");
                if (numb == pagination.numb_on_page)
                {
                    strBld.Append(" selected");
                }
                strBld.Append(">");
                strBld.Append(numb);
                strBld.Append("</option>");
            }
            strBld.Append("</select>");

            strBld.Append("</td>");
            #endregion

            strBld.Append("<td><i>Всего надено:</i><b>").Append(pagination.full_quantity).Append("</b></td>");

            strBld.Append("</tr>");
            strBld.Append("</table>");

            return strBld.ToString();
        }

        //*************************
        public static Paging Parsing_string_to_Paging(string paging_text)
        //*************************
        {
            Paging paging = get_new_paging();

            if (String.IsNullOrEmpty(paging_text)) return paging;

            string[] paging_arr = paging_text.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (paging_arr.Length != 4) return paging;

            paging.curr_page     = Convert.ToInt32(paging_arr[0]);
            paging.full_quantity = Convert.ToInt32(paging_arr[1]);
            paging.numb_on_page  = Convert.ToInt32(paging_arr[2]);
            paging.selector      = Convert.ToInt32(paging_arr[3]);

            return paging;
        }


    }
}
