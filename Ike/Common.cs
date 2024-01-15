﻿using System.Text;
using System.Text.RegularExpressions;

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
		/// <returns>字符串截取结果,如果<paramref name="sourse"/>中不包含<paramref name="startstr"/>或<paramref name="endstr"/>,则返回<see langword="string.Empty"/>;</returns>
		public static string IntercepString(string sourse, string startstr, string endstr)
		{
			string result = string.Empty;
			int startindex, endindex;
			startindex = sourse.IndexOf(startstr);
			if (startindex == -1)
				return result;
			string tmpstr = sourse.Substring(startindex + startstr.Length);
			endindex = tmpstr.IndexOf(endstr);
			if (endindex == -1)
				return result;
			result = tmpstr.Remove(endindex);
			return result;
		}

		/// <summary>
		/// 格式化字节长度显示为对应大小单位,例如:
		/// <list type="table">
		/// <item><see cref="FormatBytes"/>,<see langword="long=10982"/>, 返回 [<see langword="10.72KB"/>]</item>
		/// <item><see cref="FormatBytes"/>,<see langword="long=85455482"/>, 返回 [<see langword="81.50MB"/>]</item>
		/// </list>
		/// </summary>
		/// <param name="bytes">要格式化的字节数</param>
		/// <returns>格式化后的字符串</returns>
		public static string FormatBytes(long bytes)
		{
			string[] suffixes = ["B", "KB", "MB", "GB", "TB", "PB"];
			double last = 1;
			for (int i = 0; i < suffixes.Length; i++)
			{
				var current = Math.Pow(1024, i + 1);
				var temp = bytes / current;
				if (temp < 1)
				{
					return (bytes / last).ToString("n2") + suffixes[i];
				}
				last = current;
			}
			return bytes.ToString();
		}

		/// <summary>
		/// 复制字符串指定次数
		/// </summary>
		/// <param name="text">字符串</param>
		/// <param name="copyCount">复制次数</param>
		/// <returns>复制后的字符串</returns>
		public static string CopyString(string text, int copyCount)
		{
			StringBuilder builder = new(text.Length * copyCount);
			for (int i = 0; i < copyCount; i++)
			{
				builder.Append(text);
			}
			return builder.ToString();
		}

		/// <summary>
		/// 字符串中是否包含中文
		/// 使用正则匹配 <see langword="[\u4e00-\u9fa5]"/>
		/// </summary>
		/// <param name="text">字符串</param>
		/// <returns></returns>
		public static bool ContainsChines(string text)
		{
			Regex regex = new("[\u4e00-\u9fa5]");
			return regex.IsMatch(text);
		}

		/// <summary>
		/// 字符是否为中文
		/// 使用正则匹配 <see langword="[\u4e00-\u9fa5]"/>
		/// </summary>
		/// <param name="ch">输入字符</param>
		/// <returns></returns>
		public static bool ContainsChines(char ch)
		{
			return ContainsChines(ch.ToString());
		}
	}
}
