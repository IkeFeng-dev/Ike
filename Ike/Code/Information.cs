using System.Net.NetworkInformation;

namespace Ike
{
	/// <summary>
	/// 获取信息类
	/// </summary>
	public static class Information
	{
		/// <summary>
		/// 获取程序所在目录
		/// </summary>
		/// <value>等同于<see cref="AppDomain.CurrentDomain"/>.<see langword="BaseDirectory"/>;</value>
		public static string ProgramDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }

		/// <summary>
		/// 获取特殊文件夹目录
		/// </summary>
		/// <param name="guid">在<see cref="WinAPI.KnownGuidValue"/>中有定义特定目录的<see cref="Guid"/>的值</param>
		/// <returns></returns>
		public static string GetKnownFolderPath(Guid guid)
		{
			_ = WinAPI.GetKnownFolderPath(guid, 0, IntPtr.Zero, out string path);
			return path;
		}

		/// <summary>
		/// 获取网卡接口名称
		/// </summary>
		/// <returns></returns>
		public static List<string> GetNetworkInterfaceInstanceNames()
		{
			List<string> instanceNames = new List<string>();
			NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in interfaces)
			{
				instanceNames.Add(networkInterface.Name);
			}
			return instanceNames;
		}

		/// <summary>
		/// 获取网卡接口描述
		/// </summary>
		/// <returns></returns>
		public static List<string> GetNetworkInterfaceInstanceDescription()
		{
			List<string> instanceNames = new List<string>();
			NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in interfaces)
			{
				instanceNames.Add(networkInterface.Description);
			}
			return instanceNames;
		}

		/// <summary>
		/// 获取本地计算机网络接口对象
		/// </summary>
		/// <returns>包含所有本地网络接口对象,如果没有则为空数组对象</returns>
		public static NetworkInterface[] GetAllNetworkInterfaces()
		{
			return NetworkInterface.GetAllNetworkInterfaces();
		}

	}
}
