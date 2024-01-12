using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ike
{
    /// <summary>
    /// 基础常用的方法类
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// 截取两个字符串之间字符串
        /// </summary>
        /// <param name="sourse">文本内容</param>
        /// <param name="startstr">开始截取的内容</param>
        /// <param name="endstr">结束截取的内容</param>
        /// <returns></returns>
        public static string IntercepString(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            startindex = sourse.IndexOf(startstr);
            if (startindex == -1)
                return result;
            string tmpstr = sourse[(startindex + startstr.Length)..];
            endindex = tmpstr.IndexOf(endstr);
            if (endindex == -1)
                return result;
            result = tmpstr.Remove(endindex);
            return result;
        }
    }
}
