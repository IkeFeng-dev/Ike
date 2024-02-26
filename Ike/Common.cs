using PuppeteerSharp;
using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Ike
{
	/// <summary>
	/// 基础常用的方法类
	/// </summary>
	public static class Common
    {
		/// <summary>
		/// 记录取消标志
		/// </summary>
		public class CancelToken(bool isCancellationRequested)
		{
			/// <summary>
			/// 是否取消
			/// </summary>
			public bool IsCancellationRequested { get; set; } = isCancellationRequested;
		}

		/// <summary>
		/// 截取两个字符串之间字符串
		/// </summary>
		/// <param name="sourse">文本内容</param>
		/// <param name="startstr">开始截取的内容</param>
		/// <param name="endstr">结束截取的内容</param>
		/// <returns>字符串截取结果,如果<paramref name="sourse"/>中不包含<paramref name="startstr"/>或<paramref name="endstr"/>,则返回<see langword="string.Empty"/>;</returns>
		public static string IntercepString(string sourse, string startstr, string endstr)
		{
			string result = string.Empty;
			int startindex, endindex;
			startindex = sourse.IndexOf(startstr);
			if (startindex == -1)
				return result;
			string tmpstr = sourse.Substring(startindex + startstr.Length);
			endindex = tmpstr.IndexOf(endstr);
			if (endindex == -1)
				return result;
			result = tmpstr.Remove(endindex);
			return result;
		}

		/// <summary>
		/// 格式化字节长度显示为对应大小单位,例如:
		/// <list type="table">
		/// <item><see cref="FormatBytes"/>,<see langword="long=10982"/>, 返回 [<see langword="10.72KB"/>]</item>
		/// <item><see cref="FormatBytes"/>,<see langword="long=85455482"/>, 返回 [<see langword="81.50MB"/>]</item>
		/// </list>
		/// </summary>
		/// <param name="bytes">要格式化的字节数</param>
		/// <returns>格式化后的字符串</returns>
		public static string FormatBytes(long bytes)
		{
			string[] suffixes = ["B", "KB", "MB", "GB", "TB", "PB"];
			double last = 1;
			for (int i = 0; i < suffixes.Length; i++)
			{
				var current = Math.Pow(1024, i + 1);
				var temp = bytes / current;
				if (temp < 1)
				{
					return (bytes / last).ToString("n2") + suffixes[i];
				}
				last = current;
			}
			return bytes.ToString();
		}

		/// <summary>
		/// 复制字符串指定次数
		/// </summary>
		/// <param name="text">字符串</param>
		/// <param name="copyCount">复制次数</param>
		/// <returns>复制后的字符串</returns>
		public static string CopyString(string text, int copyCount)
		{
			StringBuilder builder = new(text.Length * copyCount);
			for (int i = 0; i < copyCount; i++)
			{
				builder.Append(text);
			}
			return builder.ToString();
		}

		/// <summary>
		/// 字符串中是否包含中文
		/// 使用正则匹配 <see langword="[\u4e00-\u9fa5]"/>
		/// </summary>
		/// <param name="text">字符串</param>
		/// <returns></returns>
		public static bool ContainsChines(string text)
		{
			Regex regex = new("[\u4e00-\u9fa5]");
			return regex.IsMatch(text);
		}

		/// <summary>
		/// 字符是否为中文
		/// 使用正则匹配 <see langword="[\u4e00-\u9fa5]"/>
		/// </summary>
		/// <param name="ch">输入字符</param>
		/// <returns></returns>
		public static bool ContainsChines(char ch)
		{
			return ContainsChines(ch.ToString());
		}

		/// <summary>
		/// 匹配两个字符串
		/// </summary>
		/// <param name="content">匹配内容</param>
		/// <param name="pattern">匹配<paramref name="content"/>的字符串,可使用一般通配符</param>
		/// <param name="ignoreCase">匹配是否不区分大小写,<see langword="true"/>:不区分,  <see langword="false"/>:区分</param>
		/// <returns></returns>
		public static bool WildcardMatch(string content, string pattern, bool ignoreCase)
		{
			RegexOptions options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
			pattern = '^' + pattern.Replace("*", ".*").Replace('?', '.') + '$';
			return Regex.IsMatch(content, pattern, options);
		}


		/// <summary>
		/// 设置屏幕不休眠
		/// </summary>
		/// <remarks>
		/// <list type="bullet">
		/// <item>表示是否应该同时保持屏幕不关闭</item>
		/// <item>设置此线程此时开始一直将处于运行状态,此时计算机不应该进入睡眠状态,此线程退出后,设置将失效</item>
		/// <item>对于游戏 视频和演示相关的任务需要保持屏幕不关闭;而对于后台服务,下载和监控等任务则不需要</item>
		/// </list>
		/// </remarks>
		/// <returns>调用 <see cref="WinAPI.SetThreadExecutionState"/> 函数实现,并返回先前的执行状态</returns>
		public static WinAPI.ExecutionState ScreenNotSleep()
		{
			return WinAPI.SetThreadExecutionState(WinAPI.ExecutionState.Continuous | WinAPI.ExecutionState.SystemRequired | WinAPI.ExecutionState.DisplayRequired);
		}

		/// <summary>
		/// 恢复此线程的运行状态,操作系统现在可以正常进入睡眠状态和关闭屏幕
		/// </summary>
		/// <returns>调用 <see cref="WinAPI.SetThreadExecutionState"/> 函数实现,并返回先前的执行状态</returns>
		public static WinAPI.ExecutionState RestoreScreenSleep()
		{
			return WinAPI.SetThreadExecutionState(WinAPI.ExecutionState.Continuous);
		}



		/// <summary>
		/// 打开可执行程序,不等待程序结束,无法获取程序返回值
		/// </summary>
		/// <param name="processPath">可执行程序路径</param>
		/// <param name="argument">传递参数,没有则为<see langword="null"></see> </param>
		/// <exception cref="FileNotFoundException"></exception>
		public static void OpenProcess(string processPath, string? argument)
		{
			if (!File.Exists(processPath))
			{
				throw new FileNotFoundException(processPath);
			}
			using (Process process = new())
			{
				process.StartInfo = new()
				{
					FileName = processPath,
					Arguments = argument,
					WorkingDirectory = Path.GetDirectoryName(processPath),
					UseShellExecute = true,
				};
				process.Start();
			}
		}


		/// <summary>
		/// CMD命令执行类型
		/// </summary>
		public enum RunCmdType
		{
			/// <summary>
			/// 启用新窗口独立运行这个命令,返回<see cref="string.Empty"/>
			/// </summary>
			IndependentOperation,
			/// <summary>
			/// 启用新窗口独立运行这个命令,显示这个窗口,按任意键关闭它,返回<see cref="string.Empty"/>
			/// </summary>
			ShowWindow,
			/// <summary>
			/// 获取输出文本
			/// </summary>
			GetOutputText,
			/// <summary>
			/// 结果直接输出到调用程序控制台中,返回<see cref="string.Empty"/>
			/// </summary>
			Association,
			/// <summary>
			/// 后台执行,不输出结果,不显示窗体,返回<see cref="string.Empty"/>
			/// </summary>
			BackgroundExecution,
		}

		/// <summary>
		/// 运行命令行指令
		/// </summary>
		/// <param name="command">指令</param>
		/// <param name="workingDirectory">设置工作目录</param>
		/// <param name="cmdType">执行类型</param>
		/// <param name="waitForExit">是否等待进程结束,默认值是<see langword="false"/></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static string RunCmd(string command, string? workingDirectory, RunCmdType cmdType, bool waitForExit = false)
		{
			if (string.IsNullOrWhiteSpace(command))
			{
				throw new ArgumentNullException(nameof(command));
			}
			string result = string.Empty;
			bool isRead = false;
			ProcessStartInfo info = new ProcessStartInfo()
			{
				FileName = "cmd.exe",
				Arguments = "/c " + command
			};
			if (!string.IsNullOrWhiteSpace(workingDirectory))
			{
				info.WorkingDirectory = workingDirectory;
			}
			switch (cmdType)
			{
				case RunCmdType.IndependentOperation:
					info.UseShellExecute = true;
					break;
				case RunCmdType.ShowWindow:
					info.UseShellExecute = true;
					info.Arguments = "/c " + command + " & pause>nul";
					break;
				case RunCmdType.Association:
					info.UseShellExecute = false;
					break;
				case RunCmdType.BackgroundExecution:
					info.UseShellExecute = false;
					info.CreateNoWindow = true;
					break;
				case RunCmdType.GetOutputText:
					info.UseShellExecute = false;
					info.RedirectStandardOutput = true;
					info.StandardOutputEncoding = Encoding.UTF8;
					waitForExit = true;
					isRead = true;
					break;
			}
			using (Process process = new Process())
			{
				process.StartInfo = info;
				process.Start();
				if (isRead)
				{
					result = process.StandardOutput.ReadToEnd();
				}
				if (waitForExit)
				{
					process.WaitForExit();
				}
			}
			return result;
		}

		/// <summary>
		/// 异步运行cmd命令,将内容输出到 <paramref name="outputHandler"/>委托对象
		/// </summary>
		/// <param name="cmdCommand">cmd命令</param>
		/// <param name="outputHandler">输出到委托对象</param>
		/// <param name="workingDirectory">工作目录</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task RunCmdAsync(string cmdCommand, Action<string?> outputHandler, string? workingDirectory)
		{
			if (string.IsNullOrWhiteSpace(cmdCommand))
			{
				throw new ArgumentNullException(nameof(cmdCommand));
			}
			ArgumentNullException.ThrowIfNull(outputHandler);
			using Process process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = "/c " + cmdCommand, 
				UseShellExecute = false,
				RedirectStandardOutput = true
			};
			if (!string.IsNullOrWhiteSpace(workingDirectory))
			{
				process.StartInfo.WorkingDirectory = workingDirectory;
			}
			process.StartInfo = startInfo;
			process.OutputDataReceived += (sender, e) =>
			{
				if (e.Data != null)
				{
					outputHandler.Invoke(e.Data);
				}
			};
			process.Start();
			process.BeginOutputReadLine();
			await process.WaitForExitAsync();
		}



		/// <summary>
		/// CMD命令执行类型
		/// </summary>
		public enum RunBatType
		{
			/// <summary>
			/// 启用新窗口独立运行这个命令,返回<see cref="string.Empty"/>
			/// </summary>
			IndependentOperation,
			/// <summary>
			/// 获取输出文本
			/// </summary>
			GetOutputText,
			/// <summary>
			/// 结果直接输出到调用程序控制台中,返回<see cref="string.Empty"/>
			/// </summary>
			Association,
			/// <summary>
			/// 后台执行,不输出结果,不显示窗体,返回<see cref="string.Empty"/>
			/// </summary>
			BackgroundExecution,
		}

		/// <summary>
		/// 运行bat文件
		/// </summary>
		/// <param name="batFilePath">bat文件路径</param>
		/// <param name="batType">执行类型</param>
		/// <param name="arguments">参数</param>
		/// <param name="workingDirectory">设置工作目录</param>
		/// <param name="waitForExit">是否等待进程结束,默认值是<see langword="false"/></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public static string RunBat(string batFilePath, RunBatType batType, string? arguments, string? workingDirectory, bool waitForExit = false)
		{
			if (string.IsNullOrWhiteSpace(batFilePath))
			{
				throw new ArgumentNullException(nameof(batFilePath));
			}
			if (!File.Exists(batFilePath))
			{
				throw new FileNotFoundException(batFilePath);
			}
			string result = string.Empty;
			bool isRead = false;
			ProcessStartInfo info = new ProcessStartInfo()
			{
				FileName = batFilePath,
				Arguments = arguments
			};
			if (!string.IsNullOrWhiteSpace(workingDirectory))
			{
				info.WorkingDirectory = workingDirectory;
			}
			switch (batType)
			{
				case RunBatType.IndependentOperation:
					info.UseShellExecute = true;
					break;
				case RunBatType.Association:
					info.UseShellExecute = false;
					break;
				case RunBatType.BackgroundExecution:
					info.UseShellExecute = false;
					info.CreateNoWindow = true;
					break;
				case RunBatType.GetOutputText:
					info.UseShellExecute = false;
					info.RedirectStandardOutput = true;
					info.StandardOutputEncoding = Encoding.UTF8;
					waitForExit = true;
					isRead = true;
					break;
			}
			using (Process process = new Process())
			{
				process.StartInfo = info;
				process.Start();
				if (isRead)
				{
					result = process.StandardOutput.ReadToEnd();
				}
				if (waitForExit)
				{
					process.WaitForExit();
				}
			}
			return result;
		}

		/// <summary>
		/// 异步运行bat文件,将内容输出到 <paramref name="outputHandler"/>委托对象
		/// </summary>
		/// <param name="batFilePath">bat文件路径</param>
		/// <param name="outputHandler">输出到委托对象</param>
		/// <param name="workingDirectory">工作目录</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public static async Task RunBatAsync(string batFilePath, Action<string?> outputHandler, string? workingDirectory)
		{
			if (string.IsNullOrWhiteSpace(batFilePath))
			{
				throw new ArgumentNullException(nameof(batFilePath));
			}
			if (!File.Exists(batFilePath))
			{
				throw new FileNotFoundException(batFilePath);
			}
			ArgumentNullException.ThrowIfNull(outputHandler);
			using Process process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = batFilePath,
				UseShellExecute = false,
				RedirectStandardOutput = true
			};
			if (!string.IsNullOrWhiteSpace(workingDirectory))
			{
				process.StartInfo.WorkingDirectory = workingDirectory;
			}
			process.StartInfo = startInfo;
			process.OutputDataReceived += (sender, e) =>
			{
				if (e.Data != null)
				{
					outputHandler.Invoke(e.Data);
				}
			};
			process.Start();
			process.BeginOutputReadLine();
			await process.WaitForExitAsync();
		}


		/// <summary>
		/// 查找字符串中的所有链接,链接结尾是以空格判断结束
		/// </summary>
		/// <param name="input">字符串内容</param>
		/// <returns></returns>
		public static string[] FindUrls(string input)
		{
			Regex regex = new Regex(@"https?://\S+");
			List<string> urls = new List<string>();
			foreach (Match match in regex.Matches(input).Cast<Match>())
			{
				urls.Add(match.Value);
			}
			return urls.ToArray();
		}

		/// <summary>
		/// 查找字符串中所有整数
		/// </summary>
		/// <param name="input">字符串内容</param>
		/// <returns></returns>
		public static string[] FindIntegers(string input)
		{
			string pattern = @"-?\b\d+\b";
			Regex regex = new Regex(pattern);
			List<string> integers = new List<string>();
			foreach (Match match in regex.Matches(input))
			{
				integers.Add(match.Value);
			}
			return integers.ToArray();
		}

		/// <summary>
		/// 查找字符串中所有的小数
		/// </summary>
		/// <param name="input">字符串内容</param>
		/// <returns></returns>
		public static string[] FindDecimals(string input)
		{
			string pattern = @"-?\b\d+\.\d+\b";
			Regex regex = new Regex(pattern);
			List<string> decimals = new List<string>();
			foreach (Match match in regex.Matches(input))
			{
				decimals.Add(match.Value);
			}
			return decimals.ToArray();
		}

		/// <summary>
		/// 查找字符串中所有的数值
		/// </summary>
		/// <param name="input">字符串内容</param>
		/// <returns></returns>
		public static string[] FindNumbers(string input)
		{
			Regex regex = new Regex(@"-?\d+(\.\d+)?");
			List<string> numbers = new List<string>();
			foreach (Match match in regex.Matches(input))
			{
				numbers.Add(match.Value);
			}
			return numbers.ToArray();
		}

		/// <summary>
		/// 在自定义分隔符中查找值
		/// </summary>
		/// <param name="input">字符串内容</param>
		/// <param name="startDelimiter">起始分隔符</param>
		/// <param name="endDelimiter">接收分隔符</param>
		/// <returns></returns>
		public static string[] FindValuesInCustomDelimiters(string input, string startDelimiter, string endDelimiter)
		{
			string pattern = Regex.Escape(startDelimiter) + "(.*?)" + Regex.Escape(endDelimiter);
			Regex regex = new Regex(pattern);
			List<string> valuesInCustomDelimiters = new List<string>();
			foreach (Match match in regex.Matches(input))
			{
				valuesInCustomDelimiters.Add(match.Groups[1].Value);
			}
			return valuesInCustomDelimiters.ToArray();
		}

		/// <summary>
		/// 保存Html页面到图像文件
		/// </summary>
		/// <param name="html">Html代码</param>
		/// <param name="width">图像宽</param>
		/// <param name="height">图像高</param>
		/// <param name="type">图像类型</param>
		/// <param name="outputFile">输出路径</param>
		/// <returns></returns>
		public static async Task SaveHtmlToImage(string html, ScreenshotType type, int width, int height, string outputFile)
		{
			await new BrowserFetcher().DownloadAsync();
			var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
			using (var page = await browser.NewPageAsync())
			{
				await page.SetViewportAsync(new ViewPortOptions
				{
					Width = width,
					Height = height
				});
				await page.SetContentAsync(html);
				ScreenshotOptions screenshotOptions = new ScreenshotOptions()
				{
					FullPage = true,
					Type = type,
					OmitBackground = true
				};
				if (type != ScreenshotType.Png)
				{
					screenshotOptions.Quality = 100;
				}
				await page.ScreenshotAsync(outputFile, screenshotOptions);
			}
		}

		/// <summary>
		/// 保存Html页面到图像文件
		/// </summary>
		/// <param name="url">Html链接地址</param>
		/// <param name="width">图像宽</param>
		/// <param name="height">图像高</param>
		/// <param name="type">图像类型</param>
		/// <param name="outputFile">输出路径</param>
		/// <param name="timeout">获取Html的超时时间(ms),0表示无期限等待</param>
		/// <returns></returns>
		public static async Task SaveHtmlToImage(Uri url, ScreenshotType type, int width, int height, string outputFile,int timeout = 30000)
		{
			using var browserFetcher = new BrowserFetcher();
			await browserFetcher.DownloadAsync();
			var browser = await Puppeteer.LaunchAsync(new LaunchOptions
			{
				Headless = true
			});
			using (var page = await browser.NewPageAsync())
			{
				await page.SetViewportAsync(new ViewPortOptions
				{
					Width = width,
					Height = height
				});
				await page.GoToAsync(url.AbsoluteUri,timeout: timeout);
				ScreenshotOptions screenshotOptions = new ScreenshotOptions()
				{
					FullPage = true,
					Type = type,
					OmitBackground = true,
				};
				await page.ScreenshotAsync(outputFile, screenshotOptions);
			}
		}

		/// <summary>
		/// 保存Html页面到PDF文件
		/// </summary>
		/// <param name="html">Html代码</param>
		/// <param name="outputFile">输出路径</param>
		/// <returns></returns>
		public static async Task SaveHtmlToPDF(string html, string outputFile)
		{
			await new BrowserFetcher().DownloadAsync();
			var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
			using (var page = await browser.NewPageAsync())
			{
				await page.SetContentAsync(html);
				await page.PdfAsync(outputFile);
			}
		}

		/// <summary>
		/// 保存Html页面到PDF文件
		/// </summary>
		/// <param name="url">Html链接地址</param>
		/// <param name="outputFile">输出路径</param>
		/// <param name="timeout">获取Html的超时时间(ms),0表示无期限等待</param>
		/// <returns></returns>
		public static async Task SaveHtmlToPDF(Uri url, string outputFile, int timeout = 30000)
		{
			using var browserFetcher = new BrowserFetcher();
			await browserFetcher.DownloadAsync();
			var browser = await Puppeteer.LaunchAsync(new LaunchOptions
			{
				Headless = true
			});
			var page = await browser.NewPageAsync();
			await page.GoToAsync(url.AbsoluteUri, timeout: timeout);
			await page.PdfAsync(outputFile);
		}




	}
}
