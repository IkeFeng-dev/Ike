using Renci.SshNet;
using System.Collections;

namespace Ike
{
	/// <summary>
	/// SFTP工具类
	/// </summary>
	public class SFTP
	{
		private readonly SftpClient sftp;
		private readonly string user;
		private readonly string password;
		private readonly int port;
		private readonly string ip;

		/// <summary>
		/// SFTP连接状态
		/// </summary>
		public bool Connected { get { return sftp.IsConnected; } }

		/// <summary>
		/// 获取SFTP对象
		/// </summary>
		public SftpClient SftpClient { get { return sftp; } }

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="sftpIp">IP</param>
		/// <param name="sftpPort">端口</param>
		/// <param name="userName">用户名</param>
		/// <param name="userPassword">密码</param>
		public SFTP(string sftpIp, int sftpPort, string userName, string userPassword)
		{
			ip = sftpIp;
			port = sftpPort;
			user = userName;
			password = userPassword;
			sftp = new SftpClient(ip, port, user, password);
		}

		/// <summary>
		/// 连接SFTP
		/// </summary>
		/// <returns></returns>
		public bool Connect()
		{
			if (!Connected)
			{
				sftp.Connect();
			}
			return sftp.IsConnected;
		}

		/// <summary>
		/// 获取服务器文件哈希值
		/// </summary>
		/// <param name="sftpFilePath">服务器中文件完整路径</param>
		/// <returns></returns>
		public string GetFileHash(string sftpFilePath)
		{
			using var stream = sftp.OpenRead(sftpFilePath);
			using var md5 = System.Security.Cryptography.MD5.Create();
			byte[] hashBytes = md5.ComputeHash(stream);
			string md5Hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
			return md5Hash;
		}

		/// <summary>
		/// 断开连接
		/// </summary> 
		public void Disconnect()
		{
			if (sftp != null && Connected)
			{
				sftp.Disconnect();
			}
		}

		/// <summary>
		/// 获取目录大小
		/// </summary>
		/// <param name="remotePath">远程目录</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public long GetDirectorySize(string remotePath)
		{
			long directorySize = 0;
			if (sftp == null) throw new ArgumentNullException(nameof(sftp));
			if (!Connected) sftp.Connect();
			var files = sftp.ListDirectory(remotePath);
			foreach (var file in files)
			{
				if (file.IsDirectory && !file.Name.Equals(".") && !file.Name.Equals(".."))
				{
					directorySize += GetDirectorySize(file.FullName);
				}
				else
				{
					directorySize += file.Length;
				}
			}
			return directorySize;
		}

		/// <summary>
		/// 检查目录是否存在,不存在则创建
		/// </summary>
		/// <param name="remotePath">SFTP目录</param>
		/// <param name="checkAllDir">是否检查路径中每一个父目录</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void CheckSftpDirPath(string remotePath, bool checkAllDir = true)
		{
			if (sftp == null) throw new ArgumentNullException(nameof(sftp));
			if (!Connected) sftp.Connect();

			if (checkAllDir)
			{
				string[] parts = remotePath.Split('/');
				string currentPath = "";
				foreach (string part in parts)
				{
					currentPath += "/" + part;
					if (!sftp.Exists(currentPath))
					{
						sftp.CreateDirectory(currentPath);
					}
				}
			}
			else
			{
				sftp.CreateDirectory(remotePath);
			}
		}
		/// <summary>
		/// 上传文件夹
		/// </summary>
		/// <param name="localFolderPath">本地路径</param>
		/// <param name="sftpFolderPath">SFTP路径(需要保证SFTP端路径存在)</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public int UploadFolderToSftp(string localFolderPath, string sftpFolderPath)
		{
			if (sftp == null) throw new ArgumentNullException(nameof(sftp));
			if (!Connected) sftp.Connect();
			int count = 0;
			CheckSftpDirPath(sftpFolderPath);
			foreach (string filePath in Directory.GetFiles(localFolderPath))
			{
				string fileName = Path.GetFileName(filePath);
				string remoteFilePath = sftpFolderPath + "/" + fileName;
				using var fileStream = new FileStream(filePath, FileMode.Open);
				sftp.UploadFile(fileStream, remoteFilePath);
				count++;
			}
			foreach (string subfolderPath in Directory.GetDirectories(localFolderPath))
			{
				string subfolderName = Path.GetFileName(subfolderPath);
				string remoteSubfolderPath = sftpFolderPath + "/" + subfolderName;
				count += UploadFolderToSftp(subfolderPath, remoteSubfolderPath);
			}
			return count;
		}

		/// <summary>
		/// 上传文件
		/// </summary>
		/// <param name="localPath">本地路径</param>
		/// <param name="remotePath">远程路径</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void Upload(string localPath, string remotePath)
		{
			using var file = File.OpenRead(localPath);
			if (sftp == null) throw new ArgumentNullException(nameof(sftp));
			if (!Connected) sftp.Connect();
			sftp.UploadFile(file, remotePath);
		}

		/// <summary>
		/// 下载文件
		/// </summary>
		/// <param name="remotePath">远程路径</param>
		/// <param name="localPath">本地路径</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void Download(string remotePath, string localPath)
		{
			if (sftp == null) throw new ArgumentNullException(nameof(sftp));
			if (!Connected) sftp.Connect();

			var byt = sftp.ReadAllBytes(remotePath);
			File.WriteAllBytes(localPath, byt);
		}

		/// <summary>
		/// 删除文件 
		/// </summary>
		/// <param name="remoteFile">远程路径</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void Delete(string remoteFile)
		{
			if (sftp == null) throw new ArgumentNullException(nameof(sftp));
			if (!Connected) sftp.Connect();

			sftp.Delete(remoteFile);
		}


		/// <summary>
		/// 获取文件列表
		/// </summary>
		/// <param name="remotePath">远程目录</param>
		/// <param name="fileSuffix">文件后缀</param>
		/// <returns></returns>
		public ArrayList GetFileList(string remotePath, string fileSuffix)
		{
			if (sftp == null) throw new ArgumentNullException(nameof(sftp));
			if (!Connected) sftp.Connect();
			var files = sftp.ListDirectory(remotePath);
			var objList = new ArrayList();
			foreach (var file in files)
			{
				string name = file.Name;
				if (name.Length > (fileSuffix.Length + 1) && fileSuffix == name.Substring(name.Length - fileSuffix.Length))
				{
					objList.Add(name);
				}
			}
			return objList;
		}


		/// <summary>
		/// 移动文件
		/// </summary>
		/// <param name="oldRemotePath">旧远程路径</param>
		/// <param name="newRemotePath">新远程路径</param>
		public void Move(string oldRemotePath, string newRemotePath)
		{
			if (sftp == null) throw new ArgumentNullException(nameof(sftp));
			if (!Connected) sftp.Connect();
			sftp.RenameFile(oldRemotePath, newRemotePath);
		}
	}
}
