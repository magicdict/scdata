using System.Collections.Generic;
using System.IO;
using System.Linq;

public class BasicInfo
{
    public string phone_no_m { get; set; }
    public string city_name { get; set; }
    public string county_name { get; set; }
    public int idcard_cnt { get; set; }
    public double arpu_avg { get; set; }
    public string label { get; set; }
}

/// <summary>
/// 对于用户基本信息的处理
/// </summary>
public class BasicInfo_Train : BasicInfo
{
    public double arpu_201908 { get; set; }
    public double arpu_201909 { get; set; }
    public double arpu_201910 { get; set; }
    public double arpu_201911 { get; set; }
    public double arpu_201912 { get; set; }
    public double arpu_202001 { get; set; }
    public double arpu_202002 { get; set; }
    public double arpu_202003 { get; set; }
    //统计值
    public double arpu_avg_c()
    {
        var f = new double[] { arpu_201908, arpu_201909, arpu_201910, arpu_201911, arpu_201912, arpu_202001, arpu_202002, arpu_202003 };
        if (f.Count(x => !double.IsNaN(x)) == 0) return 0;
        return System.Math.Round(f.Sum(x => double.IsNaN(x) ? 0 : x) / f.Count(x => !double.IsNaN(x)), 2);
    }

    public static List<BasicInfo_Train> ReadCSV(string filename)
    {
        var df = new List<BasicInfo_Train>();
        var sr = new StreamReader(filename);
        sr.ReadLine();  //第一行
        while (!sr.EndOfStream)
        {
            var rawinfo = sr.ReadLine().Split(",");
            var r = new BasicInfo_Train();
            r.phone_no_m = rawinfo[0];
            r.city_name = rawinfo[1];
            r.county_name = rawinfo[2];
            r.idcard_cnt = int.Parse(rawinfo[3]);
            r.arpu_201908 = Common.ParseDouble(rawinfo[4]);
            r.arpu_201909 = Common.ParseDouble(rawinfo[5]);
            r.arpu_201910 = Common.ParseDouble(rawinfo[6]);
            r.arpu_201911 = Common.ParseDouble(rawinfo[7]);
            r.arpu_201912 = Common.ParseDouble(rawinfo[8]);
            r.arpu_202001 = Common.ParseDouble(rawinfo[9]);
            r.arpu_202002 = Common.ParseDouble(rawinfo[10]);
            r.arpu_202003 = Common.ParseDouble(rawinfo[11]);
            r.arpu_avg = r.arpu_avg_c();
            r.label = rawinfo[12];
            df.Add(r);
        }
        sr.Close();
        return df;
    }
}

/// <summary>
/// 对于用户基本信息的处理
/// </summary>
public class BasicInfo_Test : BasicInfo
{
    public static List<BasicInfo_Test> ReadCSV(string filename)
    {
        var df = new List<BasicInfo_Test>();
        var sr = new StreamReader(filename);
        sr.ReadLine();  //第一行
        while (!sr.EndOfStream)
        {
            var rawinfo = sr.ReadLine().Split(",");
            var r = new BasicInfo_Test();
            r.phone_no_m = rawinfo[0];
            r.city_name = rawinfo[1];
            r.county_name = rawinfo[2];
            r.idcard_cnt = int.Parse(rawinfo[3]);
            r.arpu_avg = Common.ParseDouble(rawinfo[4]);
            if (double.IsNaN(r.arpu_avg))
            {
                r.arpu_avg = 0;
            }
            df.Add(r);
        }
        sr.Close();
        return df;
    }
}

public static class Common
{
    public static double ParseDouble(string s)
    {
        if (string.IsNullOrEmpty(s)) return double.NaN;
        return double.Parse(s);
    }
}

