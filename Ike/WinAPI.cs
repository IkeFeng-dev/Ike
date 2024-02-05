using System.Runtime.InteropServices;
using System.Text;

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
		/// <param name="lpClassName">窗口类名,可以为 <see langword="null"/></param>
		/// <param name="lpWindowName">窗口标题,可以为 <see langword="null"/></param>
		/// <remarks>
		/// 这是一个用于查找窗口句柄的 Windows API 函数,它接受窗口类名和窗口标题作为参数
		/// </remarks>
		/// <returns>如果找到匹配的窗口，则返回窗口的句柄；如果未找到匹配的窗口，则返回<see cref=" IntPtr.Zero"/></returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		/// <summary>
		/// 设置窗体状态
		/// </summary>
		/// <param name="hWnd">要操作的窗口的句柄</param>
		/// <param name="nCmdShow">指定窗口的显示方式，通常使用(<see langword="0x00"/>=隐藏,<see langword="0x01"/>=普通,<see langword="0x02"/>=最小化,<see langword="0x03"/>=最大化)
		/// </param>
		/// <remarks>
		/// 这是一个用于隐藏,显示或设置窗体状态的 Windows API 函数,它接受窗口句柄和显示状态参数作为参数
		/// </remarks>
		/// <returns>如果成功,返回 <see langword="true"/>;如果失败,返回 <see langword="false"/></returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

		/// <summary>
		/// 指针之间进行数据拷贝
		/// </summary>
		/// <param name="target">目标地址</param>
		/// <param name="source">源地址</param>
		/// <param name="length">数据长度</param>
		/// <remarks>
		/// 这是一个用于复制内存块的 Windows API 函数,它接受目标内存块的指针,源内存块的指针以及要复制的字节数作为参数
		/// </remarks>
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
		/// <remarks>
		/// 这是一个用于获取指定模块句柄的 Windows API 函数,它接受模块的名称作为参数
		/// 如果函数成功,则返回指定模块的句柄;如果函数失败,则返回 <see cref="IntPtr.Zero"/>
		/// </remarks>
		/// <returns>指定模块的句柄</returns>
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetModuleHandle(string name);


		/// <summary>
		/// 模拟鼠标事件
		/// </summary>
		/// <param name="dwFlags">指定鼠标事件的标志,如 MOUSEEVENTF_LEFTDOWN,MOUSEEVENTF_MOVE 等</param>
		/// <param name="dx">指定鼠标事件的水平坐标或移动的相对距离</param>
		/// <param name="dy">指定鼠标事件的垂直坐标或移动的相对距离</param>
		/// <param name="cButtons">指定按下或释放的鼠标按钮的数量</param>
		/// <param name="dwExtraInfo">指定与鼠标事件相关的附加信息</param>
		/// <returns>如果函数成功，则返回非零值;如果函数失败，则返回0</returns>
		/// <remarks>
		/// 这是一个用于模拟鼠标事件的 Windows API 函数,它接受鼠标事件的标志,水平坐标,垂直坐标,鼠标按钮数量和附加信息作为参数
		/// </remarks>
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

		/// <summary>
		/// 查找窗体中指定控件句柄
		/// </summary>
		/// <param name="parent">窗体句柄</param>
		/// <param name="child">子控件句柄</param>
		/// <param name="className">子控件类名</param>
		/// <param name="captionName">子控件名称</param>
		/// <returns></returns>
		[DllImport("user32.dll", EntryPoint = "FindWindowEx")]
		public extern static IntPtr FindWindowEx(IntPtr parent, IntPtr child, string className, string captionName);

		/// <summary>
		/// 向窗体发送指定信息
		/// </summary>
		/// <param name="hWnd">句柄参数,表示窗口句柄,即指向窗口的一个标识符</param>
		/// <param name="msg">消息参数,表示要发送的消息代码</param>
		/// <param name="wParam">通常用于传递附加信息</param>
		/// <param name="lParam">通常用于传递字符串信息</param>
		/// <remarks>
		/// 这是一个用于向指定窗口发送消息的 Windows API 函数 它接受窗口句柄,消息类型,附加消息信息和包含消息数据的字符串作为参数
		/// </remarks>
		/// <returns>函数返回一个整数</returns>
		[DllImport("User32.dll", EntryPoint = "SendMessage")]
		public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

		/// <summary>
		/// 向指定窗口发送消息
		/// </summary>
		/// <param name="handle">要接收消息的窗口的句柄</param>
		/// <param name="wMsg">指定消息的类型</param>
		/// <param name="wParam">指定附加的消息信息</param>
		/// <param name="lParam">指向包含消息数据的 StringBuilder 对象</param>
		/// <remarks>
		/// 这是一个用于向指定窗口发送消息的 Windows API 函数 它接受窗口句柄,消息类型,附加消息信息和指向包含消息数据的 <see cref="StringBuilder"/> 对象作为参数
		/// </remarks>
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		private static extern void SendMessage(IntPtr handle, int wMsg, int wParam, StringBuilder lParam);


		/// <summary>
		/// 设置窗口的位置和大小,以及其他窗口状态的方法
		/// </summary>
		/// <param name="hWnd">要设置位置和大小的窗口的句柄</param>
		/// <param name="hWndInsertAfter">在 Z 轴顺序中,窗口将被放置在该窗口之后,可以为特殊值,如 <seealso cref="IntPtr.Zero"/></param>
		/// <param name="x">窗口的新 X 坐标</param>
		/// <param name="y">窗口的新 Y 坐标</param>
		/// <param name="cx">窗口的新宽度</param>
		/// <param name="cy">窗口的新高度</param>
		/// <param name="uFlags">指定窗口位置的一组标志,如 SWP_NOMOVE SWP_NOSIZE 等</param>
		/// <returns>如果函数成功,则返回 <see langword="true"/>;如果函数失败,则返回 <see langword="false"/></returns>
		[DllImport("user32.dll ")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

		/// <summary>
		/// 坐标数据
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			/// <summary>
			/// 最左坐标
			/// </summary>
			public int Left;
			/// <summary>
			/// 最上坐标
			/// </summary>
			public int Top;
			/// <summary>
			/// 最右坐标
			/// </summary>
			public int Right;
			/// <summary>
			/// 最下坐标
			/// </summary>
			public int Bottom;
		}

		/// <summary>
		/// 获取指定窗口的矩形区域信息
		/// </summary>
		/// <param name="hWnd">要获取矩形区域信息的窗口的句柄</param>
		/// <param name="lpRect">指向一个 <see cref="RECT"/> 结构的引用,用于接收窗口的矩形区域信息</param>
		/// <returns>如果获取成功,则返回 <see langword="true"/>;如果函数失败,则返回 <see langword="false"/></returns>
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);


		/// <summary>
		/// 获取当前具有焦点的窗口的句柄
		/// </summary>
		/// <remarks>
		/// 这是一个用于获取当前具有焦点窗口句柄的 Windows API 函数,它不需要任何参数,直接返回当前具有焦点的窗口的句柄 如果没有窗口具有焦点,函数将返回 <see cref="IntPtr.Zero"/>
		/// </remarks>
		/// <returns>如果函数成功,则返回具有焦点的窗口的句柄;如果没有焦点的窗口,则返回 <see cref="IntPtr.Zero"/></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetForegroundWindow();


		/// <summary>
		/// 获取与指定窗口关联的线程标识和进程标识
		/// </summary>
		/// <param name="hwnd">要获取线程和进程标识的窗口的句柄</param>
		/// <param name="ID">一个输出参数,用于接收窗口关联的进程标识</param>
		/// <remarks>
		/// 这是一个用于获取与指定窗口关联的线程标识和进程标识的 Windows API 函数 它接受窗口句柄作为输入,并通过输出参数 <paramref name="ID"/> 返回与该窗口关联的进程标识
		/// </remarks>
		/// <returns>返回与窗口关联的线程标识</returns>
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);


		/// <summary>
		/// 向 INI 文件中写入指定的键和值,如果文件不存在,会创建文件;如果键已经存在,会更新键的值
		/// </summary>
		/// <param name="section">要写入的键所在的节名称</param>
		/// <param name="key">要写入的项的名称</param>
		/// <param name="val">要写入的项的新字符串</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>如果指定的 INI 文件不存在,此函数会创建文件</item>
		/// <item>如果指定的键已经存在,此函数会更新键的值</item>
		/// <item>如果 INI 文件中指定的节和键不存在,此函数会创建它们</item>
		/// </list>
		/// </remarks>
		/// <returns>如果函数成功,则返回 <seealso langword="true"/>;否则,返回 <seealso langword="false"/></returns>
		[DllImport("kernel32")]
		public static extern bool WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);


		/// <summary>
		/// 从 INI 文件中检索指定的键的值
		/// </summary>
		/// <param name="section">要检索的键所在的节名称</param>
		/// <param name="key">要检索的项的名称</param>
		/// <param name="def">如果在文件中找不到指定的键，则返回的默认值</param>
		/// <param name="retVal">用于保存返回的字符串值的缓冲区</param>
		/// <param name="size">缓冲区大小,用于保存返回的字符串</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <remarks>
		///   <list type="bullet">
		///     <item>如果找不到指定的键,则返回默认值<paramref name="def"/></item>
		///     <item>如果找到指定的键,但其值为空字符串,则返回空字符串</item>
		///     <item>如果 INI 文件或指定的节和键不存在,或者发生其他错误,函数将返回空字符串</item>
		///   </list>
		/// </remarks>
		/// <returns>从 INI 文件中检索到的字符串值</returns>
		[DllImport("kernel32")]
		public static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);

	}
}
