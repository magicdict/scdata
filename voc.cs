using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class VocInfo
{
    public string phone_no_m { get; set; }
    public string opposite_no_m { get; set; }
    public string calltype_id { get; set; }
    public DateTime start_datetime { get; set; }
    public int call_dur { get; set; }
    public string city_name { get; set; }
    public string county_name { get; set; }
    public string imei_m { get; set; }
    public int next_call_septiem { get; set; }

    public static List<VocInfo> ReadCSV(string filename)
    {
        var df = new List<VocInfo>();
        var sr = new StreamReader(filename);
        sr.ReadLine();  //第一行
        while (!sr.EndOfStream)
        {
            var rawinfo = sr.ReadLine().Split(",");
            var r = new VocInfo();
            r.phone_no_m = rawinfo[0];
            r.opposite_no_m = rawinfo[1];
            r.calltype_id = rawinfo[2];
            r.start_datetime = DateTime.Parse(rawinfo[3]);
            r.call_dur = int.Parse(rawinfo[4]);
            r.city_name = rawinfo[5];
            r.county_name = rawinfo[6];
            r.imei_m = rawinfo[7];
            df.Add(r);
        }
        sr.Close();
        df.Sort((x, y) =>
        {
            if (x.phone_no_m == y.phone_no_m)
            {
                return x.start_datetime.CompareTo(y.start_datetime);
            }
            else
            {
                return x.phone_no_m.CompareTo(y.phone_no_m);
            }
        });
        for (int i = 0; i < df.Count - 1; i++)
        {
            var c = df[i];
            var n = df[i + 1];
            if (c.phone_no_m == n.phone_no_m)
            {
                c.next_call_septiem = (int)n.start_datetime.Subtract(c.start_datetime).TotalSeconds - c.call_dur;
            }
            else
            {
                c.next_call_septiem = int.MaxValue;
            }
        }
        return df;
    }
}

public class Voc_Agg
{
    public string phone_no_m { get; set; }
    /// <summary>
    /// 电话次数
    /// </summary>
    /// <value></value>
    public int voc_cnt { get; set; }
    /// <summary>
    /// 对方号码数量
    /// </summary>
    /// <value></value>
    public int voc_opposite_no_m_cnt { get; set; }
    public int call_dur_avg;
    public int shortCallPercent;
    public int longCallPercent;
    public int voc_active_rate;
    public double call_dur_var;
    public double call_dur_std;
    public double call_dur_max;
    public double call_dur_min;
    public double diary_call_max_cnt;
    public double diary_call_min_cnt;
    public double diary_call_avg_cnt;
    public double diary_call_var_cnt;
    public double diary_call_std_cnt;
    public double diary_call_max_dur;
    public double diary_call_min_dur;
    public double diary_call_avg_dur;
    public double diary_call_var_dur;
    public double diary_call_std_dur;
    public double call_worktime_cnt_rate;
    public double call_workday_cnt_rate;
    public double call_small_septiem_cnt_rate;
    public double call_small_septiem_cnt;

    public static List<Voc_Agg> GetAgg(List<VocInfo> app)
    {
        var app_grp = app.GroupBy(x => x.phone_no_m);
        return app_grp.Select(x => new Voc_Agg()
        {
            phone_no_m = x.Key,
            voc_cnt = x.Count(),
            voc_opposite_no_m_cnt = x.Select(voc => voc.opposite_no_m).Distinct().Count(),
            call_dur_avg = (int)x.Average(voc => voc.call_dur),
            call_dur_var = x.Select(x => (double)x.call_dur).ToList().Variance(),
            call_dur_std = x.Select(x => (double)x.call_dur).ToList().StandardDeviation(),
            call_dur_max = x.Max(x => x.call_dur),
            call_dur_min = x.Min(x => x.call_dur),
            shortCallPercent = x.Count(voc => voc.call_dur < 60) * 100 / x.Count(),
            longCallPercent = x.Count(voc => voc.call_dur > 600) * 100 / x.Count(),
            voc_active_rate = x.Count(voc => voc.calltype_id == "1") * 100 / x.Count(),
            diary_call_max_cnt = x.GroupBy(voc => voc.start_datetime.ToString("yyyyMMdd")).Select(y => y.Count()).Max(),
            diary_call_min_cnt = x.GroupBy(voc => voc.start_datetime.ToString("yyyyMMdd")).Select(y => y.Count()).Min(),
            diary_call_avg_cnt = x.GroupBy(voc => voc.start_datetime.ToString("yyyyMMdd")).Select(y => y.Count()).Average(),
            diary_call_var_cnt = x.GroupBy(voc => voc.start_datetime.ToString("yyyyMMdd")).Select(y => (double)y.Count()).ToList().Variance(),
            diary_call_std_cnt = x.GroupBy(voc => voc.start_datetime.ToString("yyyyMMdd")).Select(y => (double)y.Count()).ToList().StandardDeviation(),
            diary_call_max_dur = x.GroupBy(voc => voc.start_datetime.ToString("yyyyMMdd")).Select(y => y.Sum(z => z.call_dur)).Max(),
            diary_call_min_dur = x.GroupBy(voc => voc.start_datetime.ToString("yyyyMMdd")).Select(y => y.Sum(z => z.call_dur)).Min(),
            diary_call_avg_dur = x.GroupBy(voc => voc.start_datetime.ToString("yyyyMMdd")).Select(y => y.Sum(z => z.call_dur)).Average(),
            diary_call_var_dur = x.GroupBy(voc => voc.start_datetime.ToString("yyyyMMdd")).Select(y => (double)y.Sum(z => z.call_dur)).ToList().Variance(),
            diary_call_std_dur = x.GroupBy(voc => voc.start_datetime.ToString("yyyyMMdd")).Select(y => (double)y.Sum(z => z.call_dur)).ToList().StandardDeviation(),
            call_worktime_cnt_rate = x.Count(voc => voc.start_datetime.Hour >= 9 && voc.start_datetime.Hour <= 18) * 100 / x.Count(),
            call_workday_cnt_rate = x.Count(voc => voc.start_datetime.DayOfWeek != DayOfWeek.Saturday && voc.start_datetime.DayOfWeek != DayOfWeek.Sunday) * 100 / x.Count(),
            call_small_septiem_cnt_rate = x.Count(voc => voc.next_call_septiem < 600) * 100 / x.Count(),
            call_small_septiem_cnt = x.Count(voc => voc.next_call_septiem < 600)
        }).ToList();
    }

}