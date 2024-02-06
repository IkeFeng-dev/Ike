using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace Ike
{
	/// <summary>
	/// 文件目录相关操作
	/// </summary>
	public class FileDir
    {
		/// <summary>
		/// 准备文件目录,传递文件路径,如果不存在则创建这个文件路目录
		/// </summary>
		/// <param name="filePath">文件路径</param>
		public static void ReadinessFileDirectory(string filePath)
		{
			if (File.Exists(filePath))
			{
				return;
			}
			string? path = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path!);
			}
		}


		/// <summary>
		/// 获取文件夹内文件,按照创建日期排序
		/// </summary>
		/// <param name="directoryPath">目标文件夹</param>
		/// <param name="search">通配符匹配文件</param>
		/// <param name="select">选择查询目录</param>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public static string[] GetFileCreationTimeOrder(string directoryPath, string search = "*", SearchOption select= SearchOption.TopDirectoryOnly)
		{
			if (!Directory.Exists(directoryPath))
			{
				throw new DirectoryNotFoundException(directoryPath);
			}
			return [.. Directory.GetFiles(directoryPath, search, select).OrderBy(File.GetCreationTime)];
		}
		/// <summary>
		/// 获取文件夹内文件,按照写入日期排序
		/// </summary>
		/// <param name="directoryPath">目标文件夹</param>
		/// <param name="search">通配符匹配文件</param>
		/// <param name="select">选择查询目录</param>
		/// <returns></returns>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public static string[] GetFileWriteTimeOrder(string directoryPath, string search = "*", SearchOption select= SearchOption.TopDirectoryOnly)
		{
			if (!Directory.Exists(directoryPath))
			{
				throw new DirectoryNotFoundException(directoryPath);
			}
			return [.. Directory.GetFiles(directoryPath, search, select).OrderBy(File.GetLastWriteTime)];
		}
		/// <summary>
		/// 获取文件夹内文件,按照最后修改文件的日期排序
		/// </summary>
		/// <param name="directoryPath">目标文件夹</param>
		/// <param name="search">通配符匹配文件</param>
		/// <param name="select">选择查询目录</param>
		/// <returns></returns>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public static string[] GetFileLastWriteTimeOrder(string directoryPath, string search = "*", SearchOption select= SearchOption.TopDirectoryOnly)
		{
			if (!Directory.Exists(directoryPath))
			{
				throw new DirectoryNotFoundException(directoryPath);
			}
			return [.. Directory.GetFiles(directoryPath, search, select).OrderBy(File.GetLastWriteTime)];
		}

		/// <summary>
		/// 获取<see langword="ini"/>文件中所有的<see langword="Section"/>
		/// </summary>
		/// <param name="filePath"><see langword="ini"/>文件路径</param>
		/// <returns></returns>
		/// <exception cref="FileNotFoundException"></exception>
		public static List<string> GetIniSectionNames(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException(filePath);
			}
			List<string> sectionNames = [];
			using (StreamReader reader = new(filePath))
			{
				string? line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith('[') && line.EndsWith(']'))
					{
						string sectionName = line[1..^1];
						sectionNames.Add(sectionName);
					}
				}
			}
			return sectionNames;
		}

		/// <summary>
		/// 获取<see langword="ini"/>文件指定<see langword="Section"/>下的所有<see langword="Key"/>和<see langword="Value"/>
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="sectionName">要获取的<see langword="Section"/></param>
		/// <returns></returns>
		/// <exception cref="FileNotFoundException"></exception>
		/// <exception cref="Exception"></exception>
		public static Dictionary<string, string> GetIniSectionKeyValues(string filePath, string sectionName)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);
			if (string.IsNullOrEmpty(sectionName)) throw new Exception("String cannot be empty.");
			Dictionary<string, string> keyValues = [];
			using (StreamReader reader = new(filePath))
			{
				string? line;
				string? currentSection = null;
				while ((line = reader.ReadLine()) != null)
				{
					line = line.Trim();
					if (line.StartsWith('[') && line.EndsWith(']'))
					{
						currentSection = line[1..^1];
					}
					else if (currentSection == sectionName && line.Contains('='))
					{
						int separatorIndex = line.IndexOf('=');
						var key = line[..separatorIndex].Trim();
						var value = line[(separatorIndex + 1)..].Trim();
						keyValues[key] = value;
					}
				}
			}
			return keyValues;
		}

		/// <summary>
		/// 获取<see langword="ini"/>文件中所有的<see langword="Key"/>和<see langword="Value"/>
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public static Dictionary<string, Dictionary<string, string>> GetIniAllKeyValues(string filePath)
		{
			Dictionary<string, Dictionary<string, string>> keyValues = [];
			foreach (string section in GetIniSectionNames(filePath))
			{
				keyValues.Add(section, GetIniSectionKeyValues(filePath, section));
			}
			return keyValues;
		}

		/// <summary>
		/// 使用<seealso cref="SHA256"/>哈希算法比较两个文件是否相同
		/// </summary>
		/// <param name="sourceFile">原文件路径</param>
		/// <param name="compareFile">比较文件路径</param>
		/// <returns>如果文件内容相同,则返回 <see langword="true"/>;反之则返回 <see langword="false"/></returns>
		public static bool CompareFileContent(string sourceFile, string compareFile)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				using (FileStream file1 = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (FileStream file2 = new FileStream(compareFile, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						return StructuralComparisons.StructuralEqualityComparer.Equals(sha256.ComputeHash(file1),sha256.ComputeHash(file2));
					}
				}
			}
		}

		/// <summary>
		/// 获取文件哈希值,使用<seealso cref="SHA256"/>算法
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public static string GetFileHashValue(string filePath)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					return BitConverter.ToString(sha256.ComputeHash(file)).Replace("-", string.Empty);
				}
			}
		}

		/// <summary>
		/// 指定编码格式读取INI内容
		/// </summary>
		/// <param name="section">要检索的键所在的节名称</param>
		/// <param name="key">要检索的项的名称</param>
		/// <param name="def">如果在文件中找不到指定的键，则返回的默认值</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <param name="encoding">指定编码读取</param>
		/// <param name="bufferSize">缓冲区大小,用于保存返回的字符串,如果缓存区小于实际内容大小,则会返回不完整的内容</param>
		/// <remarks>
		///   <list type="bullet">
		///     <item>如果找不到指定的键,则返回默认值<paramref name="def"/></item>
		///     <item>如果找到指定的键,但其值为空字符串,则返回空字符串</item>
		///     <item>如果 INI 文件或指定的节和键不存在,或者发生其他错误,函数将返回空字符串</item>
		///   </list>
		/// </remarks>
		/// <returns>从 INI 文件中检索到的字符串值,如果找不到指定的键,则返回默认值<paramref name="def"/></returns>
		public static string IniRead(string section, string key, string def, string filePath, Encoding encoding, int bufferSize = 1024)
		{
			byte[] buffer = new byte[bufferSize];
			int count = WinAPI.GetPrivateProfileString(section.ToBytes(encoding), key.ToBytes(encoding), def.ToBytes(encoding), buffer, bufferSize, filePath);
			return encoding.GetString(buffer, 0, count);
		}

		/// <summary>
		/// 指定编码格式写入INI内容
		/// </summary>
		/// <param name="section">要写入的键所在的节名称</param>
		/// <param name="key">要写入的项的名称</param>
		/// <param name="value">要写入的项的新字符串</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <param name="encoding">指定编码写入</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>如果指定的 INI 文件不存在,此函数会创建文件</item>
		/// <item>如果指定的键已经存在,此函数会更新键的值</item>
		/// <item>如果 INI 文件中指定的节和键不存在,此函数会创建它们</item>
		/// </list>
		/// </remarks>
		/// <returns>如果写入成功,则返回 <seealso langword="true"/>;否则,返回 <seealso langword="false"/></returns>
		public static bool IniWrite(string section, string key, string value, string filePath, Encoding encoding)
		{
			return WinAPI.WritePrivateProfileString(section.ToBytes(encoding), key.ToBytes(encoding), value.ToBytes(encoding), filePath);
		}





	}
}
