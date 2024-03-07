using Microsoft.Win32;
using System.Reflection.Metadata;

namespace Ike
{
	/// <summary>
	/// 注册表操作
	/// </summary>
	public class Registry
	{

		/// <summary>
		/// 注册表项常量
		/// </summary>
		public class RegistryPath
		{
			/// <summary>
			/// 开启自启动项
			/// </summary>
			public const string AutoRun = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run";
			/// <summary>
			/// 文件右键
			/// </summary>
			public const string FileRightKey = @"HKEY_CLASSES_ROOT\*";
			/// <summary>
			/// 文件右键Shell项
			/// </summary>
			public const string FileRightKeyShell = @"HKEY_CLASSES_ROOT\*\shell";
			/// <summary>
			/// 文件夹右键
			/// </summary>
			public const string DirectoryRightKey = @"HKEY_CLASSES_ROOT\Directory";
			/// <summary>
			/// 文件夹右键Shell项
			/// </summary>
			public const string DirectoryRightKeyShell = @"HKEY_CLASSES_ROOT\Directory\shell";
			/// <summary>
			/// 空白处右键
			/// </summary>
			public const string EmptyRightKey = @"HKEY_CLASSES_ROOT\Directory\Background";
			/// <summary>
			/// 空白处右键Shell项
			/// </summary>
			public const string EmptyRightKeyShell = @"HKEY_CLASSES_ROOT\Directory\Background\shell";
			/// <summary>
			/// 使用的USB设备
			/// </summary>
			public const string USBRecord = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceClasses\{a5dcbf10-6530-11d2-901f-00c04fb951ed}";
			/// <summary>
			/// 近期打开的文件记录
			/// </summary>
			public const string RecentlyOpenedFile = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU\*";
			/// <summary>
			/// 网卡信息
			/// </summary>
			public const string NetworkCard = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";
		}

		/// <summary>
		/// 获取注册表路径的根对象,符号使用
		/// </summary>
		/// <param name="keyPath">路径,需要使用完整路径,找不到根键则会抛出异常</param>
		/// <param name="subKey">返回子项路径,除根目录外的字符</param>
		/// <returns>路径的根对象</returns>
		/// <exception cref="KeyNotFoundException"></exception>
		public static RegistryKey GetRootKey(string keyPath, out string subKey)
		{
			string[] keys = new string[] { "HKEY_LOCAL_MACHINE", "HKEY_USERS", "HKEY_CURRENT_CONFIG", "HKEY_CLASSES_ROOT", "HKEY_CURRENT_USER" };
			string key = string.Empty;
			for (int i = 0; i < keys.Length; i++)
			{
				if (keyPath.Contains(keys[i], StringComparison.OrdinalIgnoreCase))
				{
					key = keys[i];
					break;
				}
			}
			if (key == string.Empty)
			{
				throw new KeyNotFoundException("键路径未找到根键");
			}
			if (keyPath.StartsWith(key, StringComparison.OrdinalIgnoreCase))
			{
				subKey = keyPath.Replace(key + "\\", string.Empty, StringComparison.OrdinalIgnoreCase);
			}
			else
			{
				int index = keyPath.ToUpper().IndexOf(key);
				subKey = keyPath.Substring(index + key.Length + 1);
			}
			switch (key)
			{
				case "HKEY_LOCAL_MACHINE": return Microsoft.Win32.Registry.LocalMachine;
				case "HKEY_USERS": return Microsoft.Win32.Registry.CurrentUser;
				case "HKEY_CURRENT_CONFIG": return Microsoft.Win32.Registry.CurrentConfig;
				case "HKEY_CLASSES_ROOT": return Microsoft.Win32.Registry.ClassesRoot;
				case "HKEY_CURRENT_USER": return Microsoft.Win32.Registry.CurrentUser;
				default: throw new KeyNotFoundException(key);
			}
		}

		/// <summary>
		/// 打开注册表指定路径
		/// </summary>
		/// <param name="keyPath">注册表路径</param>
		/// <returns><paramref name="keyPath"/>的操作对象</returns>
		public static RegistryKey OpenRegistryKey(string keyPath)
		{
			RegistryKey registryKey = GetRootKey(keyPath, out string sunKey);
			RegistryKey? open = registryKey.OpenSubKey(sunKey);
			if (open == null)
			{
				throw new Exception($"找不到'{keyPath}'");
			}
			return open;
		}


		/// <summary>
		/// 获取指定项的所有子项名字
		/// </summary>
		/// <param name="keyPath">键路径</param>
		/// <returns></returns>
		public static string[] GetSubitemNames(string keyPath)
		{
			using (RegistryKey registryKey = OpenRegistryKey(keyPath))
			{
				return registryKey.GetSubKeyNames();
			}
		}

		/// <summary>
		/// 获取指定项的所有值名称
		/// </summary>
		/// <param name="keyPath">键路径</param>
		/// <returns></returns>
		public static string[] GetGetValueNames(string keyPath)
		{
			using (RegistryKey registryKey = OpenRegistryKey(keyPath))
			{
				return registryKey.GetValueNames();
			}
		}

		/// <summary>
		/// 获取注册表指定值
		/// </summary>
		/// <param name="keyPath">注册表完整路径</param>
		/// <param name="name">值名称</param>
		/// <param name="defaultValue">如果未找到<paramref name="name"/>,则返回这个默认值</param>
		/// <returns></returns>
		public static object GetValue(string keyPath,string name,string defaultValue) 
		{
			using (RegistryKey key = OpenRegistryKey(keyPath))
			{ 
			   return key.GetValue(name, defaultValue);
			}
		}


		/// <summary>
		/// 启用USB <b>(管理员权限运行)</b>
		/// </summary>
		/// <returns></returns>
		public static bool EnableUSB()
		{
			using (RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine)
			{
				string keyPath = @"SYSTEM\CurrentControlSet\Services\USBSTOR";
				using (RegistryKey? openKey = regKey.OpenSubKey(keyPath, true))
				{
					if (openKey != null)
					{
						openKey.SetValue("Start", 3, RegistryValueKind.DWord);
						return true;
					}
					return false;
				}
			}
		}

		/// <summary>
		/// 禁用USB <b>(管理员权限运行)</b>
		/// </summary>
		/// <returns></returns>
		public static bool DisableUSB()
		{
			using (RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine)
			{
				string keyPath = @"SYSTEM\CurrentControlSet\Services\USBSTOR";
				using (RegistryKey? openKey = regKey.OpenSubKey(keyPath, true))
				{
					if (openKey != null)
					{
						openKey.SetValue("Start", 4, RegistryValueKind.DWord);
						return true;
					}
					return false;
				}
			}
		}



		/// <summary>
		/// 根据网卡名称更改对应物理地址
		/// </summary>
		/// <param name="networkCardName">网卡名称</param>
		/// <param name="physicalAddress">二进制物理地址(格式:C025A54C83A9)</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="Exception"></exception>
		internal static bool SetNetworkCardPhysicalAddress(string networkCardName, string physicalAddress)
		{
			if (physicalAddress.Length != 12) throw new ArgumentException("物理地址格式不正确");
			List<string> networkCards = Information.GetNetworkInterfaceInstanceNames();
			if (networkCards.Count == 0)
			{
				throw new Exception("当前设备未检测到网卡");
			}
			string[] subitem = GetSubitemNames(RegistryPath.NetworkCard);
			for (int i = 0; i < subitem.Length; i++)
			{
				if (subitem[i].StartsWith("0"))
				{
					using (RegistryKey key = OpenRegistryKey(RegistryPath.NetworkCard + "\\" + subitem[i]))
					{
						if (key.GetValue("DriverDesc", string.Empty).ToString() == networkCardName)
						{
							key.SetValue("NetworkAddress", physicalAddress);
							key.Close();
							return true;
						}
					}
				}
			}
			return false;
		}


		/// <summary>
		/// 创建文件右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		/// <param name="iconPath">图标路径,ico或可执行程序路径</param>
		/// <param name="command">右键打开文件菜单时打开的程序文件路径,可执行文件需要完整路径</param>
		public static void CreateFileRightMenu(string name, string iconPath, string command)
		{
			if (iconPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
			{
				iconPath = "\"" + iconPath + "\",0";
			}
			if (command.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && File.Exists(command))
			{
				command = "\"" + command + "\"" + " %L";
			}
			using (RegistryKey registryCreate = OpenRegistryKey(RegistryPath.FileRightKeyShell))
			{
				registryCreate.CreateSubKey(name, true);
				registryCreate.Close();
				using (RegistryKey registry = OpenRegistryKey(RegistryPath.FileRightKeyShell + "\\" + name))
				{
					registry.SetValue("icon", iconPath);
					registry.CreateSubKey("command", true).SetValue("", command);
					registry.Close();
				}
			}
		}


		/// <summary>
		/// 删除文件右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		public static void DeleteFileRightMenu(string name)
		{
			using (RegistryKey registry = OpenRegistryKey(RegistryPath.FileRightKeyShell + "\\" + name))
			{
				registry.DeleteSubKey("command", false);
				registry.Close();
				using (RegistryKey key = OpenRegistryKey(RegistryPath.FileRightKeyShell))
				{
					key.DeleteSubKey(name, false);
					key.Close();
				}
			}
		}

		/// <summary>
		/// 创建空白处右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		/// <param name="iconPath">图标路径,ico或可执行程序路径</param>
		/// <param name="command">启动菜单时打开的程序传递右键时目录路径,可执行文件需要完整路径</param>
		public static void CreateEmptyRightMenu(string name, string iconPath, string command)
		{
			if (iconPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
			{
				iconPath = "\"" + iconPath + "\",0";
			}
			if (command.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && File.Exists(command))
			{
				command = "\"" + command + "\"" + " -path " + "%V";
			}
			using (RegistryKey registry = OpenRegistryKey(RegistryPath.EmptyRightKeyShell))
			{
				registry.CreateSubKey(name, true);
				registry.Close();
				using (RegistryKey key = OpenRegistryKey(RegistryPath.EmptyRightKeyShell + "\\" + name))
				{
					registry.SetValue("icon", iconPath);
					registry.CreateSubKey("command", true).SetValue("", command);
					registry.Close();
				}
			}
		}



		/// <summary>
		/// 删除空白处右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		public static void DeleteEmptyRightMenu(string name)
		{
			using (RegistryKey registry = OpenRegistryKey(RegistryPath.EmptyRightKeyShell + "\\" + name))
			{
				if (registry != null)
				{
					registry.DeleteSubKey("command", false);
					registry.Close();
					using (RegistryKey key = OpenRegistryKey(RegistryPath.EmptyRightKeyShell))
					{
						registry.DeleteSubKey(name, false);
						registry.Close();
					}
				}
			}
		}



		/// <summary>
		/// 创建文件夹右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		/// <param name="iconPath">图标路径,ico或可执行程序路径</param>
		/// <param name="command">启动菜单时打开的程序传递右键时目录路径,可执行文件需要完整路径</param>
		public static void CreateDirectoryRightMenu(string name, string iconPath, string command)
		{
			if (iconPath.ToLower().EndsWith(".exe")) iconPath = "\"" + iconPath + "\",0";
			if (command.ToLower().EndsWith(".exe") && File.Exists(command)) command = "\"" + command + "\"" + " %L";
			using (RegistryKey registry = OpenRegistryKey(RegistryPath.DirectoryRightKeyShell))
			{
				registry.CreateSubKey(name, true);
			}
			using (RegistryKey registry = OpenRegistryKey(RegistryPath.DirectoryRightKeyShell + "\\" + name))
			{
				registry.SetValue("icon", iconPath);
				registry.CreateSubKey("command", true).SetValue("", command);
			}
		}



		/// <summary>
		/// 删除文件夹右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		public static void DeleteDirectoryRightMenu(string name)
		{
			using (RegistryKey registry = OpenRegistryKey(RegistryPath.DirectoryRightKeyShell + "\\" + name))
			{
				registry.DeleteSubKey("command", false);
			}
			using (RegistryKey registry = OpenRegistryKey(RegistryPath.DirectoryRightKeyShell))
			{
				registry.DeleteSubKey(name, false);
			}
		}


		/// <summary>
		/// 绑定文件打开方式(使用指定程序打开指定后缀文件,通过RegistryOperation.GetStartPath()获取启动文件的完整路径)
		/// </summary>
		/// <param name="suffixName">文件后缀</param>
		/// <param name="keyNamy">注册表项名称</param>
		/// <param name="operationSoftPath">打开此后缀文件的程序(完整路径)</param>
		public static void BindFileOpenMethod(string suffixName, string keyNamy, string operationSoftPath)
		{
			if (!suffixName.StartsWith(".")) suffixName = "." + suffixName;
			//创建后缀注册表
			RegistryKey? registry = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(suffixName);
			registry.SetValue("", keyNamy);
			registry.Close();
			//创建DefaultIcon和shell
			registry = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(keyNamy);
			registry.CreateSubKey("shell");
			registry.CreateSubKey("DefaultIcon");
			registry.Close();
			//设置图标路径
			registry = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(keyNamy + "\\DefaultIcon", true);
			registry.SetValue("", operationSoftPath + ",0");
			registry.Close();
			//创建open项
			registry = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(keyNamy + "\\shell", true);
			registry.CreateSubKey("open");
			//创建command及绑定打开后缀的程序
			registry = registry.OpenSubKey("open", true);
			registry = registry.CreateSubKey("command");
			registry.SetValue("", operationSoftPath + " \\F %1");
			registry.Close();
		}


		/// <summary>
		/// 取消绑定文件打开方式(使用指定程序打开指定后缀文件)
		/// </summary>
		/// <param name="suffixName">文件后缀</param>
		public static void DeleteBindFileOpenMethod(string suffixName)
		{
			if (!suffixName.StartsWith(".")) suffixName = "." + suffixName;
			//删除后缀项
			RegistryKey? registry = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(suffixName, true);
			string keyName = registry.GetValue("") as string;
			RegistryKey key = Microsoft.Win32.Registry.ClassesRoot;
			key.DeleteSubKey(suffixName, false);
			//删除主项中子项
			RegistryKey? mainKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(keyName, true);
			RegistryKey? shell = mainKey.OpenSubKey("shell", true);
			RegistryKey? open = shell.OpenSubKey("open", true);
			open.DeleteSubKey("command", false);
			open.Close();
			shell.DeleteSubKey("open", false);
			shell.Close();
			mainKey.DeleteSubKey("shell", false);
			mainKey.DeleteSubKey("DefaultIcon", false);
			mainKey.Close();
			//删除主项
			key.DeleteSubKey(keyName, false);
			registry.Close();
			key.Close();
		}


	}
}
