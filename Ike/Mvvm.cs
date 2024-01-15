using System.ComponentModel;
using System.Windows.Input;

namespace Ike
{
	/// <summary>
	///  WPF中MVVM模式的属性通知以及事件绑定
	/// </summary>
	public class Mvvm
    {
		/// <summary>
		/// 通知更新,在实际 <b>ViewModel</b> 类中继承此类,以执行属性通知方法,通常在属性<see langword="set"/> 中调用更新,此类继承接口<see cref="INotifyPropertyChanged"/>
		/// </summary>
		public abstract class NotifyPropertyChanged : INotifyPropertyChanged
		{
			/// <summary>
			/// 属性更新事件
			/// </summary>
			public event PropertyChangedEventHandler? PropertyChanged;

			/// <summary>
			/// 通知属性更新
			/// </summary>
			/// <param name="propertyName">属性名称</param>
			protected virtual void OnPropertyChanged(string propertyName)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}

			/// <summary>
			/// 设置属性更新方法,更新后通知属性更新
			/// </summary>
			/// <typeparam name="T">属性的数据类型</typeparam>
			/// <param name="privateField">属性的私有字段</param>
			/// <param name="value">新的属性值</param>
			/// <param name="propertyName">属性的名称</param>
			protected virtual void SetPropertyValue<T>(ref T privateField, T value, string propertyName)
			{
				privateField = value;
				OnPropertyChanged(propertyName);
			}

			/// <summary>
			/// 设置属性更新方法，更新前需要判断属性值与旧值是否相同,相同则不更新
			/// </summary>
			/// <typeparam name="T">属性的数据类型</typeparam>
			/// <param name="privateField">属性的私有字段</param>
			/// <param name="value">新的属性值</param>
			/// <param name="propertyName">属性的名称</param>
			/// <returns>如果属性值发生更改，则为 true；否则为 false</returns>
			protected virtual bool SetPropertyCheck<T>(ref T privateField, T value, string propertyName)
			{
				if (!EqualityComparer<T>.Default.Equals(privateField, value))
				{
					privateField = value;
					OnPropertyChanged(propertyName);
					return true;
				}
				return false;
			}
		}


		/// <summary>
		/// 实现了 <see cref="ICommand"/> 接口的可绑定命令
		/// </summary>
		public class RelayCommand : ICommand
		{
			/// <summary>
			/// 命令执行的委托
			/// </summary>
			private readonly Action<object?> execute;
			/// <summary>
			/// 判断命令是否可执行的委托
			/// </summary>
			private readonly Func<object?, bool>? canExecute;

			/// <summary>
			/// 当命令的可执行状态发生变化时触发的事件
			/// </summary>
			public event EventHandler? CanExecuteChanged;

			/// <summary>
			/// 创建一个新的 <see cref="RelayCommand"/> 实例
			/// </summary>
			/// <param name="execute">要执行的命令操作</param>
			public RelayCommand(Action<object?> execute)
			{
				this.execute = execute; 
			}

			/// <summary>
			/// 创建一个新的 <see cref="RelayCommand"/> 实例,可执行的条件由传入的参数决定
			/// </summary>
			/// <param name="execute">要执行的命令操作</param>
			/// <param name="canExecute">判断是否可执行的条件</param>
			public RelayCommand(Action<object?> execute, Func<object?, bool> canExecute)
			{
				this.execute = execute;
				this.canExecute = canExecute;
			}

			/// <summary>
			/// 判断命令是否可执行
			/// </summary>
			/// <param name="parameter">传递给命令的参数</param>
			/// <returns>如果命令可执行则返回 <see langword="true"/> ,否则返回 <see langword="false"/></returns>
			public bool CanExecute(object? parameter)
			{
				return canExecute == null || canExecute(parameter);
			}

			/// <summary>
			/// 执行命令操作
			/// </summary>
			/// <param name="parameter">传递给命令的参数</param>
			public void Execute(object? parameter)
			{
				execute?.Invoke(parameter);
			}

			/// <summary>
			/// 手动触发 <see cref="CanExecuteChanged"/> 事件,用于通知界面更新命令的可执行状态
			/// </summary>
			public void RaiseCanExecuteChanged()
			{
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
