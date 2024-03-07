using Spectre.Console;
using System.Data;

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
		/// <param name="enableMarkup">文本内容启用标记,可以根据<see cref="AnsiConsole"/>指定格式设定文本样式,需要注意的是启用后设置了错误的标记符号会引发异常</param>
		public static void Log(string log, Enums.LogType logType,bool enableMarkup = false)
		{ 
		    DateTime dateTime = DateTime.Now;
			string date = dateTime.ToString("yyyy-MM-dd");
			string time = dateTime.ToString("HH:mm:ss");
			if (!enableMarkup)
			{
				if (log.Contains('['))
				{
					log = log.Replace("[", "[[");
				}
				if (log.Contains(']'))
				{
					log = log.Replace("]", "]]");
				}
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
		/// <inheritdoc cref="Verbose"/>
		public static void OutVerbose(this string log)
		{
			Verbose(log);
		}

		/// <summary>
		/// 输出调试信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Debug(string log)
		{
			Log(log,Enums.LogType.Debug);
		}
		/// <inheritdoc cref="Debug"/>
		public static void OutDebug(this string log)
		{
			Debug(log);
		}

		/// <summary>
		/// 输出普通信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Information(string log)
		{
			Log(log,Enums.LogType.Information);
		}
		/// <inheritdoc cref="Information"/>
		public static void OutInfo(this string log)
		{
			Information(log);
		}

		/// <summary>
		/// 输出警告信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Warning(string log)
		{
			Log(log,Enums.LogType.Warning);
		}
		/// <inheritdoc cref="Warning"/>
		public static void OutWarning(this string log)
		{
			Warning(log);
		}

		/// <summary>
		/// 输出错误日志信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Error(string log)
		{
			Log(log,Enums.LogType.Error);
		}
		/// <inheritdoc cref="Error"/>
		public static void OutError(this string log)
		{
			Error(log);
		}

		/// <summary>
		/// 输出严重错误信息
		/// </summary>
		/// <param name="log">日志内容</param>
		public static void Critical(string log)
		{
			Log(log,Enums.LogType.Critical);
		}
		/// <inheritdoc cref="Critical"/>
		public static void OutCritical(this string log)
		{
			Critical(log);
		}

		/// <summary>
		/// 将<see cref="DataTable"/>数据输出到控制台
		/// </summary>
		/// <param name="dataTable">表</param>
		public static void ShowDataTable(DataTable dataTable)
		{
			ShowDataTable(dataTable, TableBorder.Ascii);
		}

		/// <summary>
		/// 将<see cref="DataTable"/>数据输出到控制台
		/// </summary>
		/// <param name="dataTable">表</param>
		/// <param name="border">边框样式</param>
		public static void ShowDataTable(DataTable dataTable, TableBorder border)
		{
			var table = new Table();
			table.Border(border);
			foreach (DataColumn column in dataTable.Columns)
			{
				table.AddColumn(column.ColumnName);
			}
			foreach (DataRow row in dataTable.Rows)
			{
				var values = new List<string>();
				foreach (DataColumn column in dataTable.Columns)
				{
					values.Add(row[column].ToString()??"NULL");
				}
				table.AddRow(values.ToArray());
			}
			AnsiConsole.Write(table);
		}

		/// <summary>
		/// 显示像素图像到控制台
		/// </summary>
		/// <param name="imagePath">图像路径</param>
		/// <param name="maxWidth">图像最大宽度,0则以最大宽度显示</param>
		/// <exception cref="FileNotFoundException"></exception>
		public static void ShowPixelImage(string imagePath,int maxWidth = 0)
		{
			if (!File.Exists(imagePath))
			{
				throw new FileNotFoundException(imagePath);
			}
			var image = new CanvasImage(imagePath);
			if (maxWidth > 0)
			{
				image.MaxWidth(maxWidth);
			}
			AnsiConsole.Write(image);
		}

		/// <summary>
		/// 显示数组数据到控制台
		/// </summary>
		/// <typeparam name="T">集合数据</typeparam>
		/// <param name="array">数据</param>
		/// <param name="isShowIndex">是否显示数据索引</param>
		/// <param name="name">列名称</param>
		public static void ShowArray<T>(IEnumerable<T> array,bool isShowIndex = false,string name = "Name")
		{
			var table = new Table();
			table.Border(TableBorder.Ascii);
			if (isShowIndex)
			{
				table.AddColumn("Index");
			}
			table.AddColumn(name);
			int index = 0;
			foreach (var row in array)
			{
				string? value = row?.ToString();
				if (value == null)
				{ 
				    value = "NULL";
				}
				if (isShowIndex)
				{
					table.AddRow(new string[] { index.ToString(), value });
				}
				else
				{
					table.AddRow(value);
				}
				index++;
			}
			AnsiConsole.Write(table);
		}

	}
}
