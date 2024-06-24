using Altex.Util;
using Altex;

namespace Altex.Utils
{
    [Serializable]
    //*************************
    public struct DateRange
    //*************************
    {
        public DateTime start { get; set; }
        public DateTime finish { get; set; }

        // Override the ToString method:
        public override string ToString()
        {
            return (String.Format("start:{0}; finish:{1}", start, finish));
        }

        public string ToStringIntoDB()
        {
            return (String.Format("{0};{1}", start, finish));
        }
    }

    [Serializable]
    //*************************
    public struct FilterDateRange
    //*************************
    {
        public DateRange date_range { get; set; }
        public string period { get; set; }

        public override string ToString()
        {
            return (String.Format("start={0}; finish={1}; period={2};", date_range.start, date_range.finish, period));
        }

        public string ToStringIntoDB()
        {
            return (String.Format("{0};{1};{2}", date_range.start, date_range.finish, period));
        }
    }

    [Serializable]
    //*************************
    public struct FilterByColumn
    //*************************
    {
        public string column { get; set; }
        public object value  { get; set; }
        public string type   { get; set; }
        public string order  { get; set; }

        public FilterByColumn(string _field, object _value, string _type, string _order)
        {
            column = _field;
            value  = _value;
            type   = _type;
            order  = _order;
        }

        public static FilterByColumn Duplicate(ref FilterByColumn filter)
        {
            FilterByColumn filter_dupl = new FilterByColumn();
            filter_dupl.column = filter.column;
            filter_dupl.order  = filter.order;
            filter_dupl.type   = filter.type;
            filter_dupl.value  = filter.value;
            return filter_dupl;
        }

        public override string ToString()
        {
            return (String.Format("clm={0}; val={1}; type={2}; order={3};", column, value, type, order));
        }

        public string ToStringDB()
        {
            if (type == "datetime")
                return (String.Format("<flt><clm>{0}</clm><tp>{1}</tp><vl>{2}</vl><ord>{3}</ord></flt>", column, type, ((FilterDateRange)value).ToStringIntoDB(), order));
            else
                return (String.Format("<flt><clm>{0}</clm><tp>{1}</tp><vl>{2}</vl><ord>{3}</ord></flt>", column, type, value, order));
        }
    }

    public enum PlacesFiltersCollaps
    {
        IPs = 0   // Указывает на таблицу IPs
    };

    public static class SettingsUtils
    {

        #region //========================================= Dictionary ======================================
        public static Dictionary<string, string> dct_order = new Dictionary<string, string>(){
                                                                { "no_order",   "нет"         },
                                                                { "order",      "возрастание" },
                                                                { "order_desc", "убывание"    }
                                                            };

        public static Dictionary<string, string> dct_period = new Dictionary<string, string>(){
                                                                {"last",        "Последние"         },
                                                                {"day",         "Сегодня"           },
                                                                {"tomorrow",    "Вчера"             },
                                                                {"week",        "Эта неделя"        },
                                                                {"mounth",      "Этот месяц"        },
                                                                {"day7",        "Последние 7 дней"  },
                                                                {"day14",       "Последние 14 дней" },
                                                                {"all",         "Все"               },
                                                                {"range",       "Выбрать диапазон"  }
                                                            };

        public static Dictionary<string, string> dct_bool = new Dictionary<string, string>(){
                                                                {"all",   "Все" },
                                                                {"true",  "Да"  },
                                                                {"false", "Нет" }
                                                          };

        #endregion

        public static int get_numb_on_page(int index_pos)
        {
            // Временные данные
            return 5;
        }

        public static List<int> get_list_numb_on_page()
        {
            // Временные данные
            return new List<int>() { 5, 10, 20, 50 };
        }

    }
}
