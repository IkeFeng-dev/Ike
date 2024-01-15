using System.Runtime.InteropServices;

namespace Ike
{
	/// <summary>
	/// Windows API方法,非托管动态链接库方法调用
	/// </summary>
	public class WinAPI
	{
		/// <summary>
		/// KnownFolder对应路径的<see cref="Guid"/>值
		/// </summary>
		public class KnownGuidValue
		{
			/// <summary>
			/// KnownFolder - 公共下载文件目录<see cref="Guid"/>
			/// </summary>
			public static Guid PublicDownload { get; } = new Guid("3D644C9B-1FB8-4F30-9B45-F670235F79C0");
			/// <summary>
			/// KnownFolder - 下载文件目录<see cref="Guid"/>
			/// </summary>
			public static Guid Download { get; } = new Guid("374DE290-123F-4565-9164-39C4925E467B");
			/// <summary>
			/// KnownFolder - 桌面目录<see cref="Guid"/>
			/// </summary>
			public static Guid Desktop { get; } = new Guid("B4BFCC3A-DB2C-424C-B029-7FE99A87C641");
			/// <summary>
			/// KnownFolder - 文档目录<see cref="Guid"/>
			/// </summary>
			public static Guid Documents { get; } = new Guid("FDD39AD0-238F-46AF-ADB4-6C85480369C7");
			/// <summary>
			/// KnownFolder - Music目录<see cref="Guid"/>
			/// </summary>
			public static Guid Music { get; } = new Guid("4BD8D571-6D19-48D3-BE97-422220080E43");
			/// <summary>
			/// KnownFolder - 图片目录<see cref="Guid"/>
			/// </summary>
			public static Guid Pictures { get; } = new Guid("33E28130-4E1E-4676-835A-98395C3BC3BB");
			/// <summary>
			/// KnownFolder - 视频目录<see cref="Guid"/>
			/// </summary>
			public static Guid Videos { get; } = new Guid("18989B1D-99B5-455B-841C-AB7C74E4DDFC");
			/// <summary>
			/// KnownFolder - 收藏夹目录<see cref="Guid"/>
			/// </summary>
			public static Guid Favorites { get; } = new Guid("1777F761-68AD-4D8A-87BD-30B759FA33DD");
			/// <summary>
			/// KnownFolder - 应用程序数据目录<see cref="Guid"/>
			/// </summary>
			public static Guid AppData { get; } = new Guid("3EB685DB-65F9-4CF6-A03A-E3EF65729F3D");
			/// <summary>
			/// KnownFolder - 本地应用程序数据目录<see cref="Guid"/>
			/// </summary>
			public static Guid LocalAppData { get; } = new Guid("F1B32785-6FBA-4FCF-9D55-7B8E7F157091");
			/// <summary>
			/// KnownFolder - 应用程序快捷菜单目录<see cref="Guid"/>
			/// </summary>
			public static Guid ProgramsMenu { get; } = new Guid("A77F5D77-2E2B-44C3-A6A2-ABA601054A51");
			/// <summary>
			/// KnownFolder - 公共应用程序快捷菜单目录<see cref="Guid"/>
			/// </summary>
			public static Guid CommonProgramsMenu { get; } = new Guid("0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8");
			/// <summary>
			/// KnownFolder - 开机自启动程序目录<see cref="Guid"/>
			/// </summary>
			public static Guid Startup { get; } = new Guid("B97D20BB-F46A-4C97-BA10-5E3608430854");
			/// <summary>
			/// KnownFolder - 公共开机自启动程序目录<see cref="Guid"/>
			/// </summary>
			public static Guid CommonStartup { get; } = new Guid("82A5EA35-D9CD-47C5-9629-E15D2F714E6E");
			/// <summary>
			/// KnownFolder - 发送到目录<see cref="Guid"/>
			/// </summary>
			public static Guid SendTo { get; } = new Guid("8983036C-27C0-404B-8F08-102D10DCFD74");
		}
		/// <summary>
		/// 钩子类型枚举 用于 <see cref="SetWindowsHookEx(HookType,HookProc,IntPtr,int)"/> 方法的第一个参数
		/// </summary>
		public enum HookType : int
		{
			/// <summary>
			/// 消息过滤
			/// </summary>
			WH_MSGFILTER = -1,
			/// <summary>
			/// 记录事件消息
			/// </summary>
			WH_JOURNALRECORD = 0,
			/// <summary>
			/// 播放事件消息
			/// </summary>
			WH_JOURNALPLAYBACK = 1,
			/// <summary>
			/// 键盘事件
			/// </summary>
			WH_KEYBOARD = 2,
			/// <summary>
			/// 获取消息
			/// </summary>
			WH_GETMESSAGE = 3,
			/// <summary>
			/// 调用窗口过程
			/// </summary>
			WH_CALLWNDPROC = 4,
			/// <summary>
			/// CBT 钩子
			/// </summary>
			WH_CBT = 5,
			/// <summary>
			/// 系统消息过滤
			/// </summary>
			WH_SYSMSGFILTER = 6,
			/// <summary>
			/// 鼠标事件
			/// </summary>
			WH_MOUSE = 7,
			/// <summary>
			/// 硬件事件
			/// </summary>
			WH_HARDWARE = 8,
			/// <summary>
			/// 调试事件
			/// </summary>
			WH_DEBUG = 9,
			/// <summary>
			/// Shell 事件
			/// </summary>
			WH_SHELL = 10,
			/// <summary>
			/// 前台空闲
			/// </summary>
			WH_FOREGROUNDIDLE = 11,
			/// <summary>
			/// 调用窗口过程返回值
			/// </summary>
			WH_CALLWNDPROCRET = 12,
			/// <summary>
			/// 低级键盘事件
			/// </summary>
			WH_KEYBOARD_LL = 13,
			/// <summary>
			/// 低级鼠标事件
			/// </summary>
			WH_MOUSE_LL = 14
		}

		/// <summary>
		/// 鼠标事件标识
		/// </summary>
		[Flags]
		public enum MouseFlags : int
		{
			/// <summary>
			/// 坐标映射到整个桌面
			/// </summary>
			MOUSEEVENTF_ABSOLUTE = 0x8000,
			/// <summary>
			/// 水平滚轮按钮被旋转
			/// </summary>
			MOUSEEVENTF_HWHEEL = 0x01000,
			/// <summary>
			/// 发生了移动
			/// </summary>
			MOUSEEVENTF_MOVE = 0x0001,
			/// <summary>
			/// 移动数据未合并
			/// </summary>
			MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000,
			/// <summary>
			/// 按下了左鼠标按钮
			/// </summary>
			MOUSEEVENTF_LEFTDOWN = 0x0002,
			/// <summary>
			/// 释放了左鼠标按钮
			/// </summary>
			MOUSEEVENTF_LEFTUP = 0x0004,
			/// <summary>
			/// 按下了右鼠标按钮
			/// </summary>
			MOUSEEVENTF_RIGHTDOWN = 0x0008,
			/// <summary>
			/// 释放了右鼠标按钮
			/// </summary>
			MOUSEEVENTF_RIGHTUP = 0x0010,
			/// <summary>
			/// 按下了中间鼠标按钮
			/// </summary>
			MOUSEEVENTF_MIDDLEDOWN = 0x0020,
			/// <summary>
			/// 释放了中间鼠标按钮
			/// </summary>
			MOUSEEVENTF_MIDDLEUP = 0x0040,
			/// <summary>
			/// 将坐标映射到整个虚拟桌面
			/// </summary>
			MOUSEEVENTF_VIRTUALDESK = 0x4000,
			/// <summary>
			/// 如果鼠标上有滚轮，滚轮被移动
			/// </summary>
			MOUSEEVENTF_WHEEL = 0x0800,
			/// <summary>
			/// 按下了 X 按钮
			/// </summary>
			MOUSEEVENTF_XDOWN = 0x0080,
			/// <summary>
			/// 释放了 X 按钮
			/// </summary>
			MOUSEEVENTF_XUP = 0x0100,
		}

		/// <summary>
		/// 获取窗口句柄
		/// </summary>
		/// <param name="lpClassName">窗口类名，可以为 <see langword="null"/></param>
		/// <param name="lpWindowName">窗口标题，可以为 <see langword="null"/></param>
		/// <returns>如果找到匹配的窗口，则返回窗口的句柄；如果未找到匹配的窗口，则返回<see cref=" IntPtr.Zero"/></returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		/// <summary>
		/// 设置窗体的显示与隐藏
		/// </summary>
		/// <param name="hWnd">要操作的窗口的句柄</param>
		/// <param name="nCmdShow">指定窗口的显示方式，通常使用(<see langword="0x00"/>=隐藏,<see langword="0x01"/>=普通,<see langword="0x02"/>=最小化,<see langword="0x03"/>=最大化)
		/// </param>
		/// <returns>如果成功,返回 <see langword="true"/>;如果失败,返回 <see langword="false"/></returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

		/// <summary>
		/// 指针之间进行数据拷贝
		/// </summary>
		/// <param name="target">目标地址</param>
		/// <param name="source">源地址</param>
		/// <param name="length">数据长度</param>
		[DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Ansi)]
		public static extern void CopyMemory(IntPtr target, IntPtr source, int length);

		/// <summary>
		/// 钩子回调函数
		/// </summary>
		/// <param name="nCode">如果代码小于零，则挂钩过程必须将消息传递给<see cref="CallNextHookEx(int,int,int,IntPtr)"/>函数，而无需进一步处理，并且应返回<see cref="CallNextHookEx(int,int,int,IntPtr)"/>返回的值)</param>
		/// <param name="wParam">记录了按下的按钮</param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

		/// <summary>
		/// 安装 Windows 钩子的函数,用于监视和截取特定事件,例如键盘和鼠标事件
		/// </summary>
		/// <param name="idHook">要安装的钩子类型</param>
		/// <param name="lpfn">回调函数,用于处理钩子事件的通知</param>
		/// <param name="hInstance">调用模块的句柄,通常为<see cref="IntPtr.Zero"/></param>
		/// <param name="threadId">与要安装的钩子关联的线程标识符,通常为 0 表示当前线程</param>
		/// <returns>成功安装钩子时,返回钩子的标识符,否则返回 0</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int SetWindowsHookEx(HookType idHook, HookProc lpfn, IntPtr hInstance, int threadId);

		/// <summary>
		/// 卸载 Windows 钩子的函数,用于停止监视和截取特定事件
		/// </summary>
		/// <param name="idHook">要卸载的钩子的标识符</param>
		/// <returns>成功卸载钩子时返回 <see langword="true"/>,否则返回 <see langword="false"/></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern bool UnhookWindowsHookEx(int idHook);

		/// <summary>
		/// 传递钩子事件到下一个钩子或默认过程的函数
		/// </summary>
		/// <param name="idHook">当前钩子的标识符</param>
		/// <param name="nCode">通知代码,用于确定如何处理事件</param>
		/// <param name="wParam">事件相关的参数</param>
		/// <param name="lParam">事件相关的参数</param>
		/// <returns>返回值取决于钩子类型和事件处理过程</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

		/// <summary>
		/// 获取指定模块的句柄
		/// </summary>
		/// <param name="name">要查找的模块名称,通常是进程的可执行文件名称(例如pro.exe)</param>
		/// <returns>指定模块的句柄</returns>
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetModuleHandle(string name);


		/// <summary>
		/// 模拟鼠标操作的函数,用于发送鼠标事件
		/// </summary>
		/// <param name="dwFlags">指定鼠标事件的标志</param>
		/// <param name="dx">鼠标事件发生的横坐标</param>
		/// <param name="dy">鼠标事件发生的纵坐标</param>
		/// <param name="cButtons">鼠标事件涉及的按钮数量</param>
		/// <param name="dwExtraInfo">与鼠标事件相关的附加信息</param>
		[DllImport("user32.dll")]
		public static extern int mouse_event(MouseFlags dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		/// <summary>
		/// 检索特定已知文件夹(known folder)的路径
		/// </summary>
		/// <param name="rfid">标识要检索的已知文件夹的<see cref="Guid"/>,可使用<see cref="KnownGuidValue"/>类中<see cref="Guid"/>作为查询参数</param>
		/// <param name="dwFlags">标志,用于指定其他选项,通常为 0</param>
		/// <param name="hToken">访问权限标记,通常为 IntPtr.Zero 表示使用当前用户的访问权限</param>
		/// <param name="pszPath">接收文件夹路径的字符串</param>
		/// <returns></returns>
		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		public static extern int GetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);

	}
}
