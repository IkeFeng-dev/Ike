using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;

namespace Ike
{
	/// <summary>
	/// 代码编译 反射相关操作
	/// </summary>
	public static class Compile
    {
		/// <summary>
		/// 将程序内部镶嵌的资源文件保存包指定路径
		/// </summary>
		/// <param name="resPath">资源文件路径</param>
		/// <param name="outputPath">资源文件保存到的路径,格式为: <see  langword="namespace"/>.source.png</param>
		/// <returns>写出后判断文件是否存在,存在则为<see langword="true"/>,反正为<see langword="false"/></returns>
		/// <exception cref="FileNotFoundException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public static bool GetResourceToFile(string resPath, string outputPath)
		{
			using Stream? resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resPath) ?? throw new FileNotFoundException(resPath);
			using (FileStream fileStream = File.Create(outputPath))
			{
				resourceStream.CopyTo(fileStream);
			}
			return File.Exists(outputPath);
		}

		/// <summary>
		/// 编译代码
		/// </summary>
		/// <param name="code">代码字符串,需要满足代码的基本结构,包含<see langword="namespace"/>,<see langword="class"/></param>
		/// <param name="assemblyName">生成程序集对象的名称</param>
		/// <returns>编译后的程序集对象</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static Assembly CompileCode(string code, string assemblyName)
		{
			SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
			CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary);
			CSharpCompilation compilation = CSharpCompilation.Create(assemblyName)
				.WithOptions(compilationOptions)
				.AddReferences(AppDomain.CurrentDomain.GetAssemblies().Select(assembly => MetadataReference.CreateFromFile(assembly.Location)))
				.AddSyntaxTrees(syntaxTree);
			using MemoryStream ms = new();
			EmitResult result = compilation.Emit(ms);
			if (!result.Success)
			{
				string errorMessage = string.Join(Environment.NewLine, result.Diagnostics.Select(diagnostic => diagnostic.GetMessage()));
				throw new InvalidOperationException($"Unable to compile code: {Environment.NewLine}{errorMessage}");
			}
			ms.Seek(0, SeekOrigin.Begin);
			return Assembly.Load(ms.ToArray());
		}

		/// <summary>
		/// 运行程序集对象中方法
		/// </summary>
		/// <param name="assembly">程序集</param>
		/// <param name="methodPath">方法路径,格式:  <see langword="namespace.class+class+class"/></param>
		/// <param name="methodName">执行的方法名称</param>
		/// <param name="methodParameters">方法所需参数</param>
		/// <returns>反射方法的执行结果,如果方法返回类型为<see langword="void"/>,则返回<see langword="null"/></returns>
		public static object? RunMethod(Assembly assembly, string methodPath, string methodName, object[] methodParameters)
		{
			Type type = assembly.GetType(methodPath) ?? throw new TypeLoadException($"Type '{methodPath}' not found in the assembly.");
			object? instance = Activator.CreateInstance(type);
			var addMethod = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"This method '{methodName}' not found in assembly");
			return addMethod.Invoke(instance, methodParameters);
		}

		/// <summary>
		/// 运行字符串代码中的指定方法
		/// </summary>
		/// <param name="code">代码字符串,需要满足代码的基本结构,包含<see langword="namespace"/>,<see langword="class"/></param>
		/// <param name="methodPath">方法路径,格式:  <see langword="namespace.class+class+class"/></param>
		/// <param name="methodName">执行的方法名称</param>
		/// <param name="methodParameters">方法所需参数,没有则为null</param>
		/// <returns>反射方法的执行结果,如果方法返回类型为<see langword="void"/>,则返回<see langword="null"/></returns>
		public static object? RunMethodInCode(string code, string methodPath, string methodName, object[] methodParameters)
		{
			return RunMethod(CompileCode(code, "DynamicAssembly"), methodPath, methodName, methodParameters);
		}
	}
}
