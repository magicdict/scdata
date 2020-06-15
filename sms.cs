using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SMSInfo
{
    public string phone_no_m { get; set; }
    public string opposite_no_m { get; set; }
    public string calltype_id { get; set; }
    public DateTime request_datetime { get; set; }
    public static List<SMSInfo> ReadCSV(string filename)
    {
        var df = new List<SMSInfo>();
        var sr = new StreamReader(filename);
        sr.ReadLine();  //第一行
        while (!sr.EndOfStream)
        {
            var rawinfo = sr.ReadLine().Split(",");
            var r = new SMSInfo();
            r.phone_no_m = rawinfo[0];
            r.opposite_no_m = rawinfo[1];
            r.calltype_id = rawinfo[2];
            r.request_datetime = DateTime.Parse(rawinfo[3]);
            df.Add(r);
        }
        sr.Close();
        return df;
    }
}

public class SMS_Agg
{
    public string phone_no_m { get; set; }
    /// <summary>
    /// 电话次数
    /// </summary>
    /// <value></value>
    public int sms_cnt { get; set; }
    /// <summary>
    /// 对方号码数量
    /// </summary>
    /// <value></value>
    public int sms_opposite_no_m_cnt { get; set; }
    public int msm_active_rate;
    public double diary_msm_max_cnt;
    public static List<SMS_Agg> GetAgg(List<SMSInfo> sms)
    {
        var app_grp = sms.GroupBy(x => x.phone_no_m);
        return app_grp.Select(x => new SMS_Agg()
        {
            phone_no_m = x.Key,
            sms_cnt = x.Count(),
            sms_opposite_no_m_cnt = x.Select(app => app.opposite_no_m).Distinct().Count(),
            msm_active_rate = x.Count(app => app.calltype_id == "1") * 100 / x.Count(),
            diary_msm_max_cnt = x.GroupBy(app => app.request_datetime.ToString("yyyyMMdd")).Select(y => y.Count()).Max(),
        }).ToList();
    }

}