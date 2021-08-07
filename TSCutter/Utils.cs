using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSCutter
{
    class Utils
    {
        /// <summary>
        /// 从 hh:mm:ss 解析TimeSpan
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns></returns>
        public static TimeSpan ParseDur(string timeStr)
        {
            var arr = timeStr.Replace("：", ":").Split(':');
            var days = -1;
            var hours = -1;
            var mins = -1;
            var secs = -1;
            arr.Reverse().Select(i => Convert.ToInt32(i)).ToList().ForEach(item =>
            {
                if (secs == -1) secs = item;
                else if (mins == -1) mins = item;
                else if (hours == -1) hours = item;
                else if (days == -1) days = item;
            });

            if (days == -1) days = 0;
            if (hours == -1) hours = 0;
            if (mins == -1) mins = 0;
            if (secs == -1) secs = 0;

            return new TimeSpan(days, hours, mins, secs);
        }

        /// <summary>
        /// 将秒数转换为 hh.mm.ss
        /// </summary>
        /// <param name="secs"></param>
        /// <returns></returns>
        public static string ConvertSeconds(int secs)
        {
            var ts = new TimeSpan(0, 0, 0, secs);
            return ts.ToString(@"hh\:mm\:ss").Replace(":", ".");
        }

        public static string GetValidFileName(string input, string re = ".")
        {
            string title = input;
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                title = title.Replace(invalidChar.ToString(), re);
            }
            return title;
        }
    }
}
