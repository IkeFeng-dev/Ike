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




	}
}
