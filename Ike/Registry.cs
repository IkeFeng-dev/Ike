using Microsoft.Win32;

namespace Ike
{
	/// <summary>
	/// 注册表操作
	/// </summary>
	public class Registry
	{
		/// <summary>
		/// 启用USB (管理员权限运行)
		/// </summary>
		/// <returns></returns>
		public static bool EnableUSB()
		{
			using RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine;
			string keyPath = @"SYSTEM\CurrentControlSet\Services\USBSTOR";
			using RegistryKey? openKey = regKey.OpenSubKey(keyPath, true);
			if (openKey != null)
			{
				openKey.SetValue("Start", 3, RegistryValueKind.DWord);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 禁用USB (管理员权限运行)
		/// </summary>
		/// <returns></returns>
		public static bool DisableUSB()
		{
			using RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine;
			string keyPath = @"SYSTEM\CurrentControlSet\Services\USBSTOR";
			using RegistryKey? openKey = regKey.OpenSubKey(keyPath, true);
			if (openKey != null)
			{
				openKey.SetValue("Start", 4, RegistryValueKind.DWord);
				return true;
			}
			return false;
		}



	}
}
