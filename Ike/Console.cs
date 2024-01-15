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
	}
}
