using System.Drawing;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace Ike
{
	/// <summary>
	/// 数据类型相关转换
	/// </summary>
	public static class Convert
	{
		/// <summary>
		/// 字符串转十六进制
		/// </summary>
		/// <param name="text">字符串</param>
		/// <returns></returns>
		public static string StringToHex(string text)
		{
			StringBuilder sb = new(text.Length * 2);
			byte[] byStr = Encoding.Default.GetBytes(text);
			for (int i = 0; i < byStr.Length; i++)
			{
				sb.Append(System.Convert.ToString(byStr[i], 16));
			}
			return sb.ToString().ToUpper();
		}

		/// <inheritdoc cref="StringToHex(string)"/>
		public static string ToHex(this string text)
		{
			return StringToHex(text);
		}


		/// <summary>
		/// 阿拉伯数字转中文大写,示例:
		/// <list type="table">
		/// <item><item><see cref="NumberToCh"/>,<see langword="long=812960510722"/>, 返回 [<see langword="捌仟壹佰贰拾玖,亿,陆仟零伍拾壹,万,零柒佰贰拾贰"/>]</item></item>
		/// <item><item><see cref="NumberToCh"/>,<see langword="long=-210012453"/>, 返回 [<see langword="负贰,亿,壹仟零壹,万,贰仟肆佰伍拾叁"/>]</item></item>
		/// </list>
		/// </summary>
		/// <param name="number">需要转换的长整数</param>
		/// <returns>[<see langword="零壹贰叁肆伍陆柒拾捌"/>]格式</returns>
		public static string NumberToCh(long number)
		{
			string conv = number.ToString();
			bool fu = false;
			if (number < 0)
			{
				fu = true;
				conv = conv[1..];
			}
			string[] num = ["零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖"];
			string[] digit = ["", "拾", "佰", "仟"];
			string[] units = ["", ",万,", ",亿,", ",万亿,"];
			string returnValue = "";
			int finger = 0;
			int m = conv.Length % 4;
			int k;
			if (m > 0)
				k = conv.Length / 4 + 1;
			else
				k = conv.Length / 4;
			for (int i = k; i > 0; i--)
			{
				int L = 4;
				if (i == k && m != 0)
				{
					L = m;
				}
				string four = conv.Substring(finger, L);
				int l = four.Length;
				for (int j = 0; j < l; j++)
				{
					int n = System.Convert.ToInt32(four.Substring(j, 1));
					if (n == 0)
					{
						if (j < l - 1 && System.Convert.ToInt32(four.Substring(j + 1, 1)) > 0 && !returnValue.EndsWith(num[n]))
							returnValue += num[n];
					}
					else
					{
						if (!(n == 1 && (returnValue.EndsWith(num[0]) | returnValue.Length == 0) && j == l - 2))
						{
							returnValue += num[n];
						}
						returnValue += digit[l - j - 1];
					}
				}
				finger += L;
				if (i < k)
				{
					if (System.Convert.ToInt32(four) != 0)
						returnValue += units[i - 1];
				}
				else
					returnValue += units[i - 1];
			}
			if (fu)
				returnValue = "负" + returnValue;
			return returnValue;
		}


		/// <inheritdoc cref="NumberToCh(long)"/>
		public static string ToCh(this long number)
		{
			return NumberToCh(number);
		}


		/// <summary>
		/// 文件转Binary
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		/// <exception cref="FileNotFoundException"></exception>
		public static string FileToBinary(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException(filePath);
			}
			using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);
			int fileLength = System.Convert.ToInt32(fileStream.Length);
			byte[] buffer = new byte[fileLength];
			using BinaryReader br = new(fileStream);
			for (int i = 0; i < fileLength; i++)
			{
				buffer[i] = br.ReadByte();
			}
			return System.Convert.ToBase64String(buffer);
			
		}

		/// <summary>
		/// Binary字符串转文件
		/// </summary>
		/// <param name="binaryStr">Binary字符串</param>
		/// <param name="savePath">文件保存路径</param>
		public static bool BinaryToFile(string binaryStr, string savePath)
		{
			using FileStream fileStream = new(savePath, FileMode.Create, FileAccess.Write);
			using BinaryWriter bw = new(fileStream);
			bw.Write(System.Convert.FromBase64String(binaryStr));
			return File.Exists(savePath);
		}

		/// <summary>
		/// <see langword="byte[]"/>转<see cref="Stream"/>
		/// </summary>
		/// <param name="bytes">byte数组</param>
		/// <returns></returns>
		public static Stream BytesToStream(byte[] bytes)
		{
			return new MemoryStream(bytes);
		}

		
		/// <inheritdoc cref="BytesToStream(byte[])"/>
		public static Stream ToStream(this byte[] bytes)
		{
			return BytesToStream(bytes);
		}


		/// <summary>
		/// 文件转<see  langword="byte[]"/>
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public static byte[] FileToBytes(string filePath)
		{
			using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			byte[] bytes = new byte[fileStream.Length];
			fileStream.Read(bytes, 0, bytes.Length);
			return bytes;
		}


		/// <summary>
		/// 文件转<see cref="Stream"/>
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public static Stream FileToStream(string filePath)
		{
			using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using MemoryStream memoryStream = new();
			fileStream.CopyTo(memoryStream);
			memoryStream.Position = 0;
			return memoryStream;
		}


		/// <summary>
		/// 正数转负数
		/// </summary>
		/// <param name="number">数值</param>
		/// <returns></returns>
		public static long NumberToNegative(long number)
		{
			if (number > 0)
			{
				number = Math.Abs(number) * (-1);
			}
			return number;
		}

		/// <inheritdoc cref="NumberToNegative(long)"/>
		public static long ToNegative(this long number)
		{
			return NumberToNegative(number);
		}

		/// <summary>
		/// 正双精度浮点数转负双精度浮点数
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static double NumberToNegative(double number)
		{
			if (number > 0)
			{
				number = Math.Abs(number) * (-1);
			}
			return number;
		}

		/// <inheritdoc cref="NumberToNegative(double)"/>
		public static double ToNegative(this double number)
		{
			return NumberToNegative(number);
		}

		/// <summary>
		/// <see cref="Color"/>转十六进制格式
		/// </summary>
		/// <param name="color">颜色</param>
		/// <returns>如果<see cref="Color"/>为<see langword="Empty"/>,则返回[#FFFFFF]</returns>
		public static string ColorToHexlColor(Color color)
		{
			string ret = "#FFFFFF";
			if (color.IsEmpty)
			{
				return ret;
			}
			string R = System.Convert.ToString(color.R, 16);
			if (R == "0")
			{
				R = "00";
			}
			string G = System.Convert.ToString(color.G, 16);
			if (G == "0")
			{
				G = "00";
			}
			string B = System.Convert.ToString(color.B, 16);
			if (B == "0")
			{
				B = "00";
			}
			string hexColor = "#" + R + G + B;
			return hexColor.ToUpper();
		}

		
		/// <inheritdoc cref="ColorToHexlColor(Color)"/>
		public static string ToHexlColor(this Color color)
		{
			return ColorToHexlColor(color);
		}

		/// <summary>
		/// RGB颜色转十六进制颜色
		/// </summary>
		/// <param name="R">R值</param>
		/// <param name="G">G值</param>
		/// <param name="B">B值</param>
		/// <returns></returns>
		public static string RGBColorToHexColor(byte R, byte G, byte B)
		{
			return ColorToHexlColor(Color.FromArgb(R, G, B));
		}

		/// <summary>
		/// ObjectToJson方法内部缓存对象
		/// </summary>
		private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

		/// <summary>
		/// <see cref="object"/>转JSON字符串
		/// </summary>
		/// <param name="obj">转换对象</param>
		/// <returns></returns>
		public static string ObjectToJson(object obj)
		{
			return JsonSerializer.Serialize(obj, jsonSerializerOptions);
		}

		/// <inheritdoc cref="ObjectToJson(object)"/>
		public static string ToJson(this object obj)
		{
			return ObjectToJson(obj);
		}

		/// <summary>
		/// <see  langword="Json"/>字符串转<see cref="object"/>
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="jsonString"><see  langword="Json"/>字符串转</param>
		/// <returns>对象<typeparamref name="T"/>的实例</returns>
		public static T? JsonToObject<T>(string jsonString)
		{
			T? result = JsonSerializer.Deserialize<T?>(jsonString);
			return result;
		}

		/// <summary>
		/// 字符串转<see  langword="byte[]"/>
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="encoding">编码</param>
		/// <returns></returns>
		public static byte[] StringToBytes(string str,Encoding encoding)
		{
			return encoding.GetBytes(str);
		}

		/// <inheritdoc cref="StringToBytes(string,Encoding)"/>
		public static byte[] ToBytes(this string str,Encoding encoding)
		{
			return StringToBytes(str, encoding);
		}

		/// <summary>
		/// 枚举数值转换为枚举值
		/// </summary>
		/// <typeparam name="T"><see cref="Enum"/>类型</typeparam>
		/// <param name="value">值</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static T ValueToEnum<T>(int value) where T : struct, Enum
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			if (!Enum.IsDefined(typeof(T), value))
			{
				throw new ArgumentException($"Value {value} is not defined in enum {typeof(T).Name}");
			}
			return (T)Enum.ToObject(typeof(T), value);
		}

		/// <inheritdoc cref="ValueToEnum"/>
		public static T ToEnum<T>(this int value) where T : struct, Enum
		{
			return ValueToEnum<T>(value);
		}

		/// <summary>
		/// <see cref="object"/>转YAML数据
		/// </summary>
		/// <param name="data">数据对象</param>
		/// <returns>序列号后的字符串</returns>
		public static string ObjectToYaml(object data)
		{
			var serializer = new SerializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();
			return serializer.Serialize(data);
		}

		/// <inheritdoc cref="ObjectToYaml"/>
		public static string ToYaml(this object data)
		{
			return ObjectToYaml(data);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="yaml"></param>
		/// <returns></returns>
		public static T YamlToObject<T>(string yaml)
		{
			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();
			return deserializer.Deserialize<T>(yaml);
		}

	}
}
