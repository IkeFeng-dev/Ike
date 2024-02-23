namespace Ike
{
	/// <summary>
	/// 常用枚举值
	/// </summary>
	public class Enums
	{
		/// <summary>
		/// 表示日志的类型
		/// </summary>
		public enum LogType
		{
			/// <summary>
			/// 详细信息,记录详细的调试信息或其他较为冗长的输出
			/// </summary>
			Verbose,
			/// <summary>
			/// 调试信息,用于诊断和调试目的
			/// </summary>
			Debug,
			/// <summary>
			/// 提供程序状态信息,用于跟踪程序的执行情况
			/// </summary>
			Information,
			/// <summary>
			/// 警告信息,表示潜在的问题或不符合预期的情况,但不会影响程序的正常执行
			/// </summary>
			Warning,
			/// <summary>
			/// 错误信息,用于指示程序出现的错误,但不会导致程序终止执行
			/// </summary>
			Error,
			/// <summary>
			/// 严重错误信息,用于指示程序出现的严重错误,可能会导致程序终止执行或无法继续正常运行
			/// </summary>
			Critical
		}

	}
}
