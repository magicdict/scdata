using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace 诈骗电话识别
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateTrain();
            GC.Collect();
            CreateTest();
            GC.Collect();
        }
        public static void CreateTrain()
        {
            Console.WriteLine("读取用户基本数据(训练集)");
            var user_train = BasicInfo_Train.ReadCSV(@"F:\诈骗电话识别\诈骗电话号码识别-0527\train\train_user.csv");
            Console.WriteLine("件数：" + user_train.Count);
            Console.WriteLine("正样本：" + user_train.Count(x => x.label == "1"));
            Console.WriteLine("负样本：" + user_train.Count(x => x.label == "0"));

            Console.WriteLine("读取用户APP数据(训练集)");
            var app_train = AppInfo.ReadCSV(@"F:\诈骗电话识别\诈骗电话号码识别-0527\train\train_app.csv");
            Console.WriteLine("件数：" + app_train.Count);
            var app_agg = App_Agg.GetAgg(app_train);
            app_train.Clear();
            GC.Collect();

            Console.WriteLine("读取用户VOC数据(训练集)");
            var voc_train = VocInfo.ReadCSV(@"F:\诈骗电话识别\诈骗电话号码识别-0527\train\train_voc.csv");
            Console.WriteLine("件数：" + voc_train.Count);
            var voc_agg = Voc_Agg.GetAgg(voc_train);
            voc_train.Clear();
            GC.Collect();

            Console.WriteLine("读取用户SMS数据(训练集)");
            var sms_train = SMSInfo.ReadCSV(@"F:\诈骗电话识别\诈骗电话号码识别-0527\train\train_sms.csv");
            Console.WriteLine("件数：" + sms_train.Count);
            var sms_agg = SMS_Agg.GetAgg(sms_train);
            sms_train.Clear();
            GC.Collect();

            output(user_train, app_agg, voc_agg, sms_agg, @"F:\诈骗电话识别\诈骗电话号码识别-0527\train_all.csv");
        }

        public static void CreateTest()
        {

            Console.WriteLine("读取用户基本数据(测试集)");
            var user_test = BasicInfo_Test.ReadCSV(@"F:\诈骗电话识别\诈骗电话号码识别-0527\test\test_user.csv");
            Console.WriteLine("件数：" + user_test.Count);

            Console.WriteLine("读取用户APP数据(测试集)");
            var app_test = AppInfo.ReadCSV(@"F:\诈骗电话识别\诈骗电话号码识别-0527\test\test_app.csv");
            Console.WriteLine("件数：" + app_test.Count);

            Console.WriteLine("读取用户VOC数据(测试集)");
            var voc_test = VocInfo.ReadCSV(@"F:\诈骗电话识别\诈骗电话号码识别-0527\test\test_voc.csv");
            Console.WriteLine("件数：" + voc_test.Count);

            Console.WriteLine("读取用户SMS数据(测试集)");
            var sms_test = SMSInfo.ReadCSV(@"F:\诈骗电话识别\诈骗电话号码识别-0527\test\test_sms.csv");
            Console.WriteLine("件数：" + sms_test.Count);

            //app数据分组
            output(user_test, App_Agg.GetAgg(app_test), Voc_Agg.GetAgg(voc_test), SMS_Agg.GetAgg(sms_test), @"F:\诈骗电话识别\诈骗电话号码识别-0527\test_all.csv");
        }

        public static void output(IEnumerable<BasicInfo> user_train,
        List<App_Agg> app_agg, List<Voc_Agg> voc_agg, List<SMS_Agg> sms_agg, string filename)
        {
            //整合新的表格
            var sw = new StreamWriter(filename);
            var fileds = "phone_no_m,city_name,county_name,idcard_cnt,arpu_avg," +
                         "app_cnt,bank_cnt,flow_amount,top20_cnt,weixin_flow_avg,qq_flow_avg,map_flow_avg,video_flow_avg,alipay_flow_avg," +
                         "voc_cnt,voc_opposite_no_m_cnt,call_dur_avg,call_dur_var,call_dur_std,call_dur_max,call_dur_min," +
                         "diary_call_max_cnt,diary_call_min_cnt,diary_call_avg_cnt,diary_call_var_cnt,diary_call_std_cnt," +
                         "diary_call_max_dur,diary_call_min_dur,diary_call_avg_dur,diary_call_var_dur,diary_call_std_dur," +
                         "shortCallPercent,longCallPercent,voc_active_rate,call_worktime_cnt_rate,call_workday_cnt_rate,call_small_septiem_cnt_rate,call_small_septiem_cnt," +
                         "sms_cnt,sms_opposite_no_m_cnt,diary_msm_max_cnt," +
                         "label";
            sw.WriteLine(fileds);
            foreach (var user in user_train)
            {
                sw.Write(user.phone_no_m + ",");
                sw.Write(user.city_name + ",");
                sw.Write(user.county_name + ",");
                sw.Write(user.idcard_cnt + ",");
                sw.Write(user.arpu_avg + ",");
                var app = app_agg.Where(x => x.phone_no_m == user.phone_no_m).FirstOrDefault();
                sw.Write(app.app_cnt + ",");
                sw.Write(app.bank_cnt + ",");
                sw.Write(app.flow_amount + ",");
                sw.Write(app.top20_cnt + ",");
                sw.Write(app.weixin_flow_avg + ",");
                sw.Write(app.qq_flow_avg + ",");
                sw.Write(app.map_flow_avg + ",");
                sw.Write(app.video_flow_avg + ",");
                sw.Write(app.alipay_flow_avg + ",");
                var voc = voc_agg.Where(x => x.phone_no_m == user.phone_no_m).FirstOrDefault();
                if (voc == null) voc = new Voc_Agg();
                sw.Write(voc.voc_cnt + ",");
                sw.Write(voc.voc_opposite_no_m_cnt + ",");

                sw.Write(voc.call_dur_avg + ",");
                sw.Write(voc.call_dur_var + ",");
                sw.Write(voc.call_dur_std + ",");
                sw.Write(voc.call_dur_max + ",");
                sw.Write(voc.call_dur_min + ",");

                sw.Write(voc.diary_call_max_cnt + ",");
                sw.Write(voc.diary_call_min_cnt + ",");
                sw.Write(voc.diary_call_avg_cnt + ",");
                sw.Write(voc.diary_call_var_cnt + ",");
                sw.Write(voc.diary_call_std_cnt + ",");

                sw.Write(voc.diary_call_max_dur + ",");
                sw.Write(voc.diary_call_min_dur + ",");
                sw.Write(voc.diary_call_avg_dur + ",");
                sw.Write(voc.diary_call_var_dur + ",");
                sw.Write(voc.diary_call_std_dur + ",");

                sw.Write(voc.shortCallPercent + ",");
                sw.Write(voc.longCallPercent + ",");
                sw.Write(voc.voc_active_rate + ",");

                sw.Write(voc.call_worktime_cnt_rate + ",");
                sw.Write(voc.call_workday_cnt_rate + ",");
                sw.Write(voc.call_small_septiem_cnt_rate + ",");
                sw.Write(voc.call_small_septiem_cnt + ",");

                var sms = sms_agg.Where(x => x.phone_no_m == user.phone_no_m).FirstOrDefault();
                if (sms == null) sms = new SMS_Agg();
                sw.Write(sms.sms_cnt + ",");
                sw.Write(sms.sms_opposite_no_m_cnt + ",");
                sw.Write(sms.diary_msm_max_cnt + ",");
                sw.WriteLine(user.label);
            }
            sw.Close();
        }
    }
}
