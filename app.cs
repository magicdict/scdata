using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AppInfo
{
    public string phone_no_m { get; set; }
    public string busi_name { get; set; }
    public double flow { get; set; }
    public string month_id { get; set; }

    public static List<AppInfo> ReadCSV(string filename)
    {
        var df = new List<AppInfo>();
        var sr = new StreamReader(filename);
        sr.ReadLine();  //第一行
        while (!sr.EndOfStream)
        {
            var rawinfo = sr.ReadLine().Split(",");
            var r = new AppInfo();
            r.phone_no_m = rawinfo[0];
            r.busi_name = rawinfo[1];
            r.flow = Common.ParseDouble(rawinfo[2]);
            r.month_id = rawinfo[3];
            df.Add(r);
        }
        sr.Close();
        return df;
    }
}

public class App_Agg
{
    public string phone_no_m { get; set; }
    public int app_cnt { get; set; }
    public int bank_cnt { get; set; }
    public int flow_amount { get; set; }
    public int top20_cnt { get; set; }
    public int weixin_flow_avg { get; set; }
    public int qq_flow_avg { get; set; }

    public int map_flow_avg { get; set; }
    public int video_flow_avg { get; set; }
    public int alipay_flow_avg { get; set; }

    public static List<App_Agg> GetAgg(List<AppInfo> app)
    {
        var top = app.Select(x => x.busi_name).GroupBy(x => x).Select(x => (x.Key, x.Count())).ToList();
        top.Sort((x, y) => { return y.Item2 - x.Item2; });
        top = top.Take(20).ToList();
        foreach (var item in top)
        {
            System.Console.WriteLine(item.Key + ":" + item.Item2);
        }

        var app_grp = app.GroupBy(x => x.phone_no_m);
        return app_grp.Select(x => new App_Agg()
        {
            phone_no_m = x.Key,
            app_cnt = x.Select(app => app.busi_name).Distinct().Count(),
            top20_cnt = x.Select(app => app.busi_name).Distinct().Intersect(top.Select(x => x.Key)).Count(),
            flow_amount = (int)x.Sum(app => app.flow),
            weixin_flow_avg = x.Where(x => x.busi_name == "微信").Count() == 0 ? 0 : (int)x.Where(x => x.busi_name == "微信").Sum(x => x.flow) / x.Where(x => x.busi_name == "微信").Count(),
            qq_flow_avg = x.Where(x => x.busi_name == "QQ").Count() == 0 ? 0 : (int)x.Where(x => x.busi_name == "QQ").Sum(x => x.flow) / x.Where(x => x.busi_name == "QQ").Count(),
            map_flow_avg = x.Where(x => x.busi_name == "高德导航").Count() == 0 ? 0 : (int)x.Where(x => x.busi_name == "高德导航").Sum(x => x.flow) / x.Where(x => x.busi_name == "高德导航").Count(),
            video_flow_avg = x.Where(x => x.busi_name == "腾讯视频").Count() == 0 ? 0 : (int)x.Where(x => x.busi_name == "腾讯视频").Sum(x => x.flow) / x.Where(x => x.busi_name == "腾讯视频").Count(),
            alipay_flow_avg = x.Where(x => x.busi_name == "支付宝").Count() == 0 ? 0 : (int)x.Where(x => x.busi_name == "支付宝").Sum(x => x.flow) / x.Where(x => x.busi_name == "支付宝").Count(),
            bank_cnt = x.Select(app => app.busi_name).Distinct().Count(app => app.Contains("银行"))
        }).ToList();
    }

}