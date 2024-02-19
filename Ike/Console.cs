using Spectre.Console;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Ike
{
	/// <summary>
	/// 控制台相关操作
	/// </summary>
	public static class Console
    {
		/// <summary>
		/// 以密码格式读取控制台输入密码,按下回车结束输入
		/// </summary>
		/// <param name="prompt">提示文本</param>
		/// <param name="maskChar">掩码字符,输出后代替明文密码的<see langword="char"/>字符</param>
		/// <param name="maskColor">掩码字符的字体颜色</param>
		/// <returns>键入的 <see langword="password"/> 字符串</returns>
		public static string InputPassword(string prompt, char maskChar = '*', ConsoleColor maskColor = ConsoleColor.Green)
		{
			ConsoleColor defaultForegroundColor = System.Console.ForegroundColor;
			System.Console.Write(prompt);
			System.Console.ForegroundColor = maskColor;
			string password = "";
			while (true)
			{
				ConsoleKeyInfo key = System.Console.ReadKey(true);
				if (key.Key == ConsoleKey.Enter)
				{
					System.Console.WriteLine();
					break;
				}
				if (key.Key == ConsoleKey.Backspace)
				{
					if (password.Length > 0)
					{
						password = password.Remove(password.Length - 1);
						System.Console.Write("\b \b");
					}
				}
				else
				{
					password += key.KeyChar;
					System.Console.Write(maskChar);
				}
			}
			System.Console.ForegroundColor = defaultForegroundColor;
			return password;
		}

		/// <summary>  
		/// 隐藏当前程序控制台  
		/// </summary>  
		public static void HideConsole()
		{
			IntPtr hWnd = WinAPI.FindWindow("ConsoleWindowClass", System.Console.Title);
			if (hWnd != IntPtr.Zero)
			{
				WinAPI.ShowWindow(hWnd, 0);
			}
		}

		/// <summary>  
		/// 显示当前程序控制台  
		/// </summary>  
		public static void ShowConsole()
		{
			IntPtr hWnd = WinAPI.FindWindow("ConsoleWindowClass", System.Console.Title);
			if (hWnd != IntPtr.Zero)
			{
				WinAPI.ShowWindow(hWnd, 1);
			}
		}


		/// <summary>
		/// 控制台输出日志
		/// </summary>
		/// <param name="log">日志内容</param>
		/// <param name="logType">日志类型</param>
		public static void Log(string log, Enums.LogType logType)
		{ 
		    DateTime dateTime = DateTime.Now;
			string date = dateTime.ToString("yyyy-MM-dd");
			string time = dateTime.ToString("HH:mm:ss");
			if (log.Contains('['))
			{
				log = log.Replace("[","[[");
			}
			if (log.Contains(']'))
			{
				log = log.Replace("]","]]");
			}
			string color = "#ffffff";
			switch (logType)
			{
				case Enums.LogType.Verbose:
					color = "#808080";
					break;
				case Enums.LogType.Debug:
					color = "#0087ff";
					break;
				case Enums.LogType.Information:
					color = "#ffffff";
					break;
				case Enums.LogType.Warning:
					color = "#afaf00";
					break;
				case Enums.LogType.Error:
					color = "#d70087";
					break;
				case Enums.LogType.Critical:
					color = "#d70000";
					break;
			}
			AnsiConsole.MarkupLine(string.Format("[#5f5fd7]{0}[/] [#0087d7]{1}[/] [#e4e4e4]|[/] [{2}]{3}[/] [#e4e4e4]|[/]  {4}", date, time, color, logType.ToString().ToUpper()[..3], log));
		}

		/// <summary>
		/// 输出详细信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Verbose(string log)
		{
			Log(log,Enums.LogType.Verbose);
		}
		/// <summary>
		/// 输出调试信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Debug(string log)
		{
			Log(log,Enums.LogType.Debug);
		}
		/// <summary>
		/// 输出普通信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Information(string log)
		{
			Log(log,Enums.LogType.Information);
		}
		/// <summary>
		/// 输出警告信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Warning(string log)
		{
			Log(log,Enums.LogType.Warning);
		}
		/// <summary>
		/// 输出错误日志信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Error(string log)
		{
			Log(log,Enums.LogType.Error);
		}
		/// <summary>
		/// 输出严重错误信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Critical(string log)
		{
			Log(log,Enums.LogType.Critical);
		}
	}
}
