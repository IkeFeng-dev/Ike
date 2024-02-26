using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using System.Text;

namespace Ike.Codes
{
	/// <summary>
	/// MySQL数据库操作类
	/// </summary>
	public class MySQL
	{
		/// <summary>
		/// 异常记录锁对象
		/// </summary>
		private static object lockObject = new object();
		/// <summary>
		/// DataReader锁对象
		/// </summary>
		private static object lockObject_DataReader = new object();
		/// <summary>
		/// 操作对象
		/// </summary>
		private MySqlConnection mySQL;
		/// <summary>
		/// 连接的数据库名称
		/// </summary>
		private string Database { get; set; }
		/// <summary>
		/// 连接的主机地址
		/// </summary>
		private string Server { get; set; }
		/// <summary>
		/// 连接数据库登录用户
		/// </summary>
		private string User { get; set; }
		/// <summary>
		/// 连接数据库登录密码
		/// </summary>
		private string Password { get; set; }
		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		private string Spliced { get { return $"Server={Server};Database={Database};Uid={User};Pwd={Password};"; } }

		/// <summary>
		/// 获取或设置是否在执行数据库操作时先检查字段名称或表名称合法性
		/// </summary>
		public bool IsCheckFieldOrTable { get; set; } = false;
		/// <summary>
		/// 获取或设置是否在执行数据库操作时先检查字段值合法性
		/// </summary>
		public bool IsCheckFieldValue { get; set; } = false;

		/// <summary>
		/// 获取或设置当前项目名称,用于异常写入数据库
		/// </summary>
		public string ProjectName { get; set; } = "MySQLOperation";
		/// <summary>
		/// 检查字段名称或表名称是否合法
		/// </summary>
		/// <param name="fields">检查的字段名称或表名称</param>
		public static void CheckFieldOrTable(params string[] fields)
		{
			char[] chars = new char[] { ' ', '+', '=', '!', '\'', '%', '#', '(', ')', '{', '}', '[', ']', '\\', '/', '<', '>', '?', '*', '|', '&', '@', '^', '~', ',', '$', ';' };
			foreach (var field in fields)
			{
				foreach (var cht in chars)
				{
					if (field.Contains(cht))
					{
						throw new Exception($"CheckFieldOrTable: Field [{field}] Contains [{cht}].");
					}
				}
				if (fields.Contains("--"))
				{
					throw new Exception($"Field [{field}] Contains [--].");
				}
			}
		}

		/// <summary>
		/// 检查字符串类型字段值是否存在特殊字符,有则转义
		/// </summary>
		/// <param name="value">字段值</param>
		/// <returns></returns>
		public static int CheckFieldValue(ref string value)
		{
			try
			{
				int result = 0;
				char[] strs = new char[] { '\'', '\\', '%', '_', '=', '(', ')', '{', '}', '[', ']', '/', ';', ',', '.', '+', '-', '<', '>', '|', '\"' };
				foreach (var item in strs)
				{
					if (value.Contains(item))
					{
						value = value.Replace(item.ToString(), "\\" + item);
						result++;
					}
				}
				if (result > 1)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < result; i++)
					{
						stringBuilder.Append('\\');
					}
					value = value.Replace(stringBuilder.ToString(), "\\");
				}
				return result;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 检查字典中字段和值的合法性
		/// </summary>
		/// <param name="data">字典</param>
		private void CheckDictionary(ref Dictionary<string, string> data)
		{
			try
			{
				if (data.Count > 0)
				{
					for (int i = 0; i < data.Count; i++)
					{
						if (IsCheckFieldOrTable)
						{
							CheckFieldOrTable(data.ElementAt(i).Key);
						}
						else if (IsCheckFieldValue)
						{
							string value = data.ElementAt(i).Value;
							if (CheckFieldValue(ref value) > 0)
							{
								data[data.ElementAt(i).Key] = value;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
		}


		/// <summary>
		/// 构造MySQL数据库操作类
		/// </summary>
		/// <param name="server">主机</param>
		/// <param name="database">数据库名称</param>
		/// <param name="user">登录用户</param>
		/// <param name="password">登录密码</param>
		public MySQL(string server, string database, string user, string password)
		{
			Server = server;
			Database = database;
			User = user;
			Password = password;
			mySQL = new MySqlConnection(Spliced);
		}


		/// <summary>
		/// 连接数据库
		/// </summary>
		/// <returns></returns>
		public bool Connect()
		{
			if (mySQL == null)
			{
				mySQL = new MySqlConnection(Spliced);
				mySQL.Open();
			}
			if (mySQL.State == ConnectionState.Open)
			{
				return true;
			}
			mySQL.Open();
			return mySQL.State == ConnectionState.Open;
		}

		/// <summary>
		/// 替换当前数据库操作对象
		/// </summary>
		/// <param name="sql">数据库对象</param>
		/// <returns></returns>
		public void ReplaceObject(MySqlConnection sql)
		{
			if (mySQL.State == ConnectionState.Open)
			{
				mySQL.Close();
				mySQL.Dispose();
			}
			ArgumentNullException.ThrowIfNull(sql);
			mySQL = sql;
		}


		/// <summary>
		/// 获取当前数据库对象
		/// </summary>
		/// <returns></returns>
		public MySqlConnection GetMySQLObject()
		{
			return mySQL;
		}


		/// <summary>
		/// 记录异常内容到数据库
		/// </summary>
		/// <param name="methodBase">方法信息,可使用 <see cref="MethodBase.GetCurrentMethod"/>方法获取 </param>
		/// <param name="exception">异常</param>
		internal async Task<bool> RecordErrorToDataBase(MethodBase methodBase, Exception exception)
		{
			return await RecordErrorToDataBase(ProjectName, methodBase, exception, Environment.MachineName);
		}

		/// <summary>
		/// 记录异常数据到数据库
		/// </summary>
		/// <param name="project">项目名称</param>
		/// <param name="methodBase">方法信息,可使用 <see cref="MethodBase.GetCurrentMethod"/>方法获取 </param>
		/// <param name="exception">异常</param>
		/// <param name="remark">备注内容</param>
		public async Task<bool> RecordErrorToDataBase(string project, MethodBase methodBase, Exception exception, string remark = "")
		{
			string fullName = string.Empty;
			if (methodBase.DeclaringType != null && methodBase.DeclaringType.FullName != null)
			{
				fullName = methodBase.DeclaringType!.FullName;
			}
			return await RecordErrorToDataBase(project, methodBase.Name, fullName, exception.Message, exception.ToString(), remark);
		}

		/// <summary>
		/// 记录异常数据到数据库
		/// </summary>
		/// <param name="project">项目名称</param>
		/// <param name="methodName">已引发异常的方法</param>
		/// <param name="methodFullName">方法的完全限定名称</param>
		/// <param name="exMessage">异常信息</param>
		/// <param name="info">异常内容</param>
		/// <param name="remark">备注信息</param>
		public async Task<bool> RecordErrorToDataBase(string project, string methodName, string methodFullName, string exMessage, string info, string remark = "")
		{
			if (methodName == "MoveNext")
			{
				string repl = Common.IntercepString(methodFullName, "<", ">");
				if (!string.IsNullOrEmpty(repl) && repl.Length > 0)
				{
					methodName = repl;
				}
			}
			if (methodName.Contains('<') && methodName.Contains('>'))
			{
				methodName = Common.IntercepString(methodName, "<", ">");
			}
			string query = "CREATE TABLE IF NOT EXISTS abnormal_records (" +
			"id INT NOT NULL AUTO_INCREMENT COMMENT 'KEY'," +
			"project VARCHAR(20) NOT NULL COMMENT 'Project Name', " +
			"method_name VARCHAR(100) NOT NULL COMMENT 'Method Name'," +
			"full_name VARCHAR(500) NOT NULL COMMENT 'Full Path'," +
			"message VARCHAR(1000) NOT NULL COMMENT 'Message'," +
			"information TEXT NOT NULL COMMENT 'Infomation'," +
			"remark TEXT COMMENT 'Other Explain'," +
			"error_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Record Time'," +
			"PRIMARY KEY (id)" +
			");" +
			"INSERT INTO abnormal_records" +
			"(project,method_name,message,information,full_name,remark) " +
			"VALUES " +
			"(@project,@method_name,@message,@information,@full_name,@remark);";
			if (!Connect())
			{
				return false;
			}
			using (MySqlCommand command = new MySqlCommand(query, mySQL))
			{
				command.Parameters.AddWithValue("@project", project);
				command.Parameters.AddWithValue("@method_name", methodName);
				command.Parameters.AddWithValue("@full_name", methodFullName);
				command.Parameters.AddWithValue("@message", exMessage);
				command.Parameters.AddWithValue("@information", info);
				command.Parameters.AddWithValue("@remark", remark);
				int row = await command.ExecuteNonQueryAsync();
				return row == 1;
			}
		}

		/// <summary>
		/// 执行无返回数据集的命令,例如 INSERT DELETE UPDATE
		/// </summary>
		/// <param name="command">执行命令</param>
		/// <returns>受影响行数</returns>
		public int ExecuteCommandToSET(string command)
		{
			return ExecuteCommandToSET(command, null);
		}

		/// <summary>
		/// 执行无返回数据集的命令,例如 INSERT DELETE UPDATE
		/// </summary>
		/// <param name="command">执行命令</param>
		/// <param name="parameterization">参数化如:{"@value",value},防止值注入,Key必须对应<paramref name="command"/>中参数化的值名称</param>
		/// <returns>受影响行数</returns>
		public int ExecuteCommandToSET(string command, Dictionary<string, string>? parameterization)
		{
			if (!Connect())
			{
				throw new Exception("Database connection status is incorrect.");
			}
			using (MySqlCommand cmd = new MySqlCommand(command, mySQL))
			{
				if (parameterization != null && parameterization.Count > 0)
				{
					foreach (var item in parameterization)
					{
						cmd.Parameters.AddWithValue(item.Key, item.Value);
					}
				}
				lock (lockObject_DataReader)
				{
					return cmd.ExecuteNonQuery();
				}
			}
		}


		/// <summary>
		/// 执行读取查询相关数据命令,返回读取查询数据
		/// </summary>
		/// <param name="command">执行命令</param>
		/// <returns></returns>
		public DataTable ExecuteCommandToGET(string command)
		{
			return ExecuteCommandToGET(command, null);
		}

		/// <summary>
		/// 执行读取查询相关数据命令
		/// </summary>
		/// <param name="command">执行命令</param>
		/// <param name="parameterization">参数化如:{"@value",value},防止值注入,Key必须对应<paramref name="command"/>中参数化的值名称,如此示例中执行命令必须包含 '@value' 值</param>
		/// <returns></returns>
		public DataTable ExecuteCommandToGET(string command, Dictionary<string, string>? parameterization)
		{
			DataTable table = new DataTable();
			if (!Connect())
			{
				return table;
			}
			using (MySqlCommand cmd = new MySqlCommand(command, mySQL))
			{
				if (parameterization != null && parameterization.Count > 0)
				{
					foreach (var item in parameterization)
					{
						cmd.Parameters.AddWithValue(item.Key, item.Value);
					}
				}
				lock (lockObject_DataReader)
				{
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.HasRows)
						{
							table.Load(reader);
							reader.Close();
						}
						return table;
					}
				}
			}
		}

		/// <summary>
		/// 将<see cref="Dictionary{TKey, TValue}"/>数据写入指定表
		/// </summary>
		/// <param name="tableName">表名称</param>
		/// <param name="data">包含字段和字段值的数据,其中Key为数据库字段名称,Value为写入值,如果与数据库中数据以及格式不匹配将会导致写入异常</param>
		/// <returns>受影响行数</returns>
		public int WriteData(string tableName, Dictionary<string, string> data)
		{
			if (!Connect())
			{
				throw new Exception("Database connection status is incorrect.");
			}
			if (data.Count > 0)
			{
				CheckDictionary(ref data);
				if (IsCheckFieldOrTable)
				{
					CheckFieldOrTable(tableName);
				}
				string query = $"INSERT INTO {tableName} ";
				string field = string.Empty;
				string fieldValue = string.Empty;
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				foreach (var item in data)
				{
					field += item.Key + ",";
					fieldValue += "@" + item.Key + ",";
					parameters.Add("@" + item.Key, item.Value);
				}
				field = field.Substring(0, field.Length - 1);
				fieldValue = fieldValue.Substring(0, fieldValue.Length - 1);
				query += "(" + field + ") VALUES (" + fieldValue + ");";
				return ExecuteCommandToSET(query, parameters);
			}
			return 0;
		}

		/// <summary>
		/// 批量写入数据到数据库
		/// </summary>
		/// <param name="tableName">表名称</param>
		/// <param name="fieldName">表中字段名称,注:字段需对应<paramref name="values"/>中值的位置</param>
		/// <param name="values">插入的数据,每一个list代表一行数据,数据需要对应<paramref name="fieldName"/>中顺序</param>
		/// <param name="isParameterization">是否需要执行参数化,确保值不存在注入风险可不使用参数化方法</param>
		/// <returns>受影响行数</returns>
		public int BatchWrite(string tableName, string[] fieldName, List<string[]> values, bool isParameterization)
		{
			if (IsCheckFieldOrTable)
			{
				CheckFieldOrTable(tableName);
				CheckFieldOrTable(fieldName.ToArray());
			}
			if (IsCheckFieldValue)
			{
				for (int i = 0; i < values.Count; i++)
				{
					for (int j = 0; j < values[i].Length; j++)
					{
						CheckFieldValue(ref values[i][j]);
					}
				}
			}
			if (!Connect())
			{
				throw new Exception("Database connection status is incorrect.");
			}
			StringBuilder mergeValues = new StringBuilder();
			int result;
			if (isParameterization)
			{
				for (int i = 0; i < values.Count; i++)
				{
					mergeValues.Append("( ");
					for (int j = 0; j < values[i].Length; j++)
					{
						mergeValues.Append("@" + i + "" + j + ", ");
					}
					mergeValues.Remove(mergeValues.Length - 2, 1);
					mergeValues.Append(" ) , ");
				}
				mergeValues.Remove(mergeValues.Length - 2, 1);

				Dictionary<string, string> keyValues = new Dictionary<string, string>();
				for (int i = 0; i < values.Count; i++)
				{
					for (int j = 0; j < values[i].Length; j++)
					{
						keyValues.Add("@" + i + "" + j, values[i][j]);
					}
				}
				string query = $"INSERT INTO {tableName} ({string.Join(",", fieldName)}) VALUES {mergeValues}";
				result = ExecuteCommandToSET(query, keyValues);
			}
			else
			{
				for (int i = 0; i < values.Count; i++)
				{
					mergeValues.Append("( ");
					for (int j = 0; j < values[i].Length; j++)
					{
						mergeValues.Append('\'' + values[i][j] + "' , ");
					}
					mergeValues.Remove(mergeValues.Length - 2, 1);
					mergeValues.Append(" ) , ");
				}
				mergeValues.Remove(mergeValues.Length - 2, 1);
				string query = $"INSERT INTO {tableName} ({string.Join(",", fieldName)}) VALUES {mergeValues}";
				result = ExecuteCommandToSET(query);
			}
			return result;
		}

		/// <summary>
		/// CSV文件写入数据库(批量插入)
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		/// <param name="fieldsName">分段名称</param>
		/// <param name="csvFilePath">csv文件路径,CSV必须是以','符号分割</param>
		/// <param name="skipTitle">是否跳过行头</param>
		/// <param name="encoding">读取csv文件编码</param>
		/// <returns>受影响行数</returns>
		public int CsvFileWrite(string tableName, string[] fieldsName, string csvFilePath, bool skipTitle, Encoding encoding)
		{
			if (!File.Exists(csvFilePath))
			{
				throw new DirectoryNotFoundException(csvFilePath);
			}
			if (IsCheckFieldOrTable)
			{
				CheckFieldOrTable(tableName);
			}
			if (!Connect())
			{
				throw new Exception("Database connection status is incorrect.");
			}
			string insertSql = $"INSERT INTO {tableName} ({string.Join(" , ", fieldsName)}) value ";
			StringBuilder builder = new();
			using (FileStream fs = new(csvFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader reder = new(fs, encoding))
				{
					if (skipTitle && !reder.EndOfStream)
					{
						reder.ReadLine();
					}
					while (!reder.EndOfStream)
					{
						string formatStr = string.Format("('{0}') , ", reder.ReadLine()!.Replace(",", "','"));
						if (formatStr != "('') , ")
						{
							builder.Append(formatStr);
						}
					}
				}
				builder.Remove(builder.Length - 2, 2);
				builder.Insert(0, insertSql);
			}
			return ExecuteCommandToSET(builder.ToString());
		}

		/// <summary>
		/// CSV字符串数据写入数据库(批量插入)
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		/// <param name="fieldsName">分段名称</param>
		/// <param name="csvStr">csv字符串</param>
		/// <param name="skipTitle">是否跳过行头</param>
		/// <returns>受影响行数</returns>
		public int CsvStringWrite(string tableName, string[] fieldsName, string csvStr, bool skipTitle)
		{
			if (string.IsNullOrEmpty(csvStr))
			{
				throw new Exception("CSV string is empty");
			}
			if (IsCheckFieldOrTable)
			{
				CheckFieldOrTable(tableName);
			}
			if (!Connect())
			{
				throw new Exception("Database connection status is incorrect.");
			}
			string insertSql = $"INSERT INTO {tableName} ({string.Join(" , ", fieldsName)}) value ";
			StringBuilder builder = new();
			byte[] byteArray = Encoding.UTF8.GetBytes(csvStr);
			using (MemoryStream memoryStream = new MemoryStream(byteArray))
			{
				using (StreamReader reder = new StreamReader(memoryStream))
				{
					if (skipTitle && !reder.EndOfStream)
					{
						reder.ReadLine();
					}
					while (!reder.EndOfStream)
					{
						string formatStr = string.Format("('{0}') , ", reder.ReadLine()!.Replace(",", "','"));
						if (formatStr != "('') , ")
						{
							builder.Append(formatStr);
						}
					}
					builder.Remove(builder.Length - 2, 2);
					builder.Insert(0, insertSql);
					return ExecuteCommandToSET(builder.ToString());
				}
			}
		}

		/// <summary>
		/// DataTable写入数据库(批量写入)
		/// </summary>
		/// <param name="table">数据表,需包含数据表名称,行头(表名称,行头,数据类型,长度必须与数据库相匹配)</param>
		/// <returns>受影响行数</returns>
		public int DataTableWrite(DataTable table)
		{
			string tableName = table.TableName;
			if (IsCheckFieldOrTable)
			{
				CheckFieldOrTable(tableName);
			}
			if (table == null || table.Columns.Count == 0 || table.Rows.Count == 0) 
			{
				throw new ArgumentNullException(nameof(table));
			}
			if (!Connect())
			{
				throw new Exception("Database connection status is incorrect.");
			}
			string[] fieldsName = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
			string insertSql = $"INSERT INTO {tableName} ({string.Join(" , ", fieldsName)}) value ";
			StringBuilder builderSql = new();
			StringBuilder builderLine = new();
			int column = table.Rows[0].ItemArray.Length;
			for (int i = 0; i < table.Rows.Count; i++)
			{
				for (int j = 0; j < column; j++)
				{
					builderLine.Append(table.Rows[i][j].ToString());
					if (j < column - 1)
					{
						builderLine.Append("','");
					}
				}
				builderSql.AppendFormat("('{0}'), ", builderLine);
				builderLine.Clear();
			}
			builderSql.Remove(builderSql.Length - 2, 2);
			builderSql.Insert(0, insertSql);
			return ExecuteCommandToSET(builderSql.ToString());
		}


		/// <summary>
		/// DataTable插入数据库(逐行写入)
		/// </summary>
		/// <param name="table">数据表(表名称对应数据库表,字段名称,类型,长度,必须对应数据库实际参数</param>
		/// <param name="parameters">参数{字段名称[字段类型,字段长度]}</param>
		/// <returns>受影响行数</returns>
		public int DataTableWrite(DataTable table, Dictionary<string, Tuple<MySqlDbType, int>> parameters)
		{
			string tableName = table.TableName;
			if (IsCheckFieldOrTable)
			{
				CheckFieldOrTable(tableName);
			}
			string fieldName = string.Empty;
			string fieldValueName = string.Empty;
			if (parameters == null || parameters.Count == 0 || table == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}
			if (!Connect())
			{
				throw new Exception("Database connection status is incorrect.");
			}
			foreach (var item in parameters)
			{
				fieldName += item.Key + ", ";
				fieldValueName += '@' + item.Key + ", ";
			}
			fieldName = fieldName.Remove(fieldName.Length - 2, 2);
			fieldValueName = fieldValueName.Remove(fieldValueName.Length - 2, 2);
			using (MySqlDataAdapter adapter = new MySqlDataAdapter())
			{
				adapter.InsertCommand = new MySqlCommand($"INSERT INTO {tableName} ({fieldName}) VALUES ({fieldValueName})", mySQL);
				foreach (var item in parameters)
				{
					adapter.InsertCommand.Parameters.Add("@" + item.Key, item.Value.Item1, item.Value.Item2, item.Key);
				}
				return adapter.Update(table);
			}
		}

		/// <summary>
		/// DataTable插入数据库,以事务提交数据,会进行参数化写入,速度较慢
		/// </summary>
		/// <param name="table">数据表(字段名称,类型,长度,必须对应数据库实际参数)</param>
		/// <param name="submissionCount">事务每次提交数量</param>
		/// <param name="parameters">参数</param>
		/// <returns>受影响行数</returns>
		public int DataTableWrite(DataTable table, int submissionCount, Dictionary<string, Tuple<MySqlDbType, int>> parameters)
		{
			string tableName = table.TableName;
			if (IsCheckFieldOrTable)
			{
				CheckFieldOrTable(tableName);
			}
			string fieldName = string.Empty;
			string fieldValueName = string.Empty;
			if (parameters == null || parameters.Count == 0 || table == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}
			if (!Connect())
			{
				throw new Exception("Database connection status is incorrect.");
			}
			List<string> fields = new List<string>();
			foreach (var item in parameters)
			{
				fieldName += item.Key + ", ";
				fieldValueName += '@' + item.Key + ", ";
				fields.Add(item.Key);
			}
			fieldName = fieldName.Remove(fieldName.Length - 2, 2);
			fieldValueName = fieldValueName.Remove(fieldValueName.Length - 2, 2);
			int result = 0;
			MySqlTransaction transaction = mySQL.BeginTransaction();
			{
				using (MySqlCommand command = new MySqlCommand($"INSERT INTO {tableName} ({fieldName}) VALUES ({fieldValueName})", mySQL))
				{
					foreach (var item in parameters)
					{
						command.Parameters.Add("@" + item.Key, item.Value.Item1, item.Value.Item2, item.Key);
					}
					try
					{
						for (int i = 0; i < table.Rows.Count; i++)
						{
							DataRow row = table.Rows[i];
							for (int j = 0; j < fields.Count; j++)
							{
								command.Parameters["@" + fields[j]].Value = row[fields[j]];
							}
							result += command.ExecuteNonQuery();
							if (i % submissionCount == 0 || i == table.Rows.Count - 1 && i != 1)
							{
								transaction.Commit();
								transaction.Dispose();
								transaction = mySQL.BeginTransaction();
							}
						}
					}
					catch
					{
						transaction.Rollback();
						throw;
					}
				}
			}
			return result;
		}



	}
}
