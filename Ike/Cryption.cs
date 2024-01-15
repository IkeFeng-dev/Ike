using System.Security.Cryptography;
using System.Text;

namespace Ike
{
	/// <summary>
	/// 加解密相关操作
	/// </summary>
	public static class Cryption
    {
		/// <summary>
		/// 使用伪随机数加密字符串
		/// </summary>
		/// <param name="text">需要加密的字符串</param>
		/// <returns>加密密文</returns>
		public static string EncryptText(string text)
		{
			string randStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
			byte[] btData = Encoding.Default.GetBytes(text);
			int j, k, m;
			int len = randStr.Length;
			StringBuilder sb = new(text.Length * 2);
			Random rand = new();
			for (int i = 0; i < btData.Length; i++)
			{
				j = (byte)rand.Next(6);
				btData[i] = (byte)(btData[i] ^ j);
				k = btData[i] % len;
				m = btData[i] / len;
				m = m * 8 + j;
				sb.Append(string.Concat(randStr.AsSpan(k, 1), randStr.AsSpan(m, 1)));
			}
			return sb.ToString();
		}

		/// <summary>
		/// 解密<see cref="EncryptText(string)"/>加密的密文
		/// </summary>
		/// <param name="text">经过<see cref="EncryptText(string)"/>方法加密的密文</param>
		/// <returns></returns>
		public static string DecryptText(string text)
		{
			string randStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
			int j, k, m, n = 0;
			int len = randStr.Length;
			byte[] btData = new byte[text.Length / 2];
			for (int i = 0; i < text.Length; i += 2)
			{
				k = randStr.IndexOf(text[i]);
				m = randStr.IndexOf(text[i + 1]);
				j = m / 8;
				m -= j * 8;
				btData[n] = (byte)(j * len + k);
				btData[n] = (byte)(btData[n] ^ m);
				n++;
			}
			return Encoding.Default.GetString(btData);
		}

		/// <summary>
		/// 异或数字,使用同一个方法进行加解密
		/// </summary>
		/// <param name="value">加密/解密值</param>
		/// <param name="key">加密/解密<see  langword="key"></see></param>
		/// <returns></returns>
		public static long XOREncryption(long value,long key)
		{
			return value ^ key;
		}


		/// <summary>
		/// 生成随机种子
		/// </summary>
		/// <returns></returns>
		public static int RandomSeed()
		{
			byte[] bytes = new byte[4];
			using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(bytes);
			}
			return BitConverter.ToInt32(bytes, 0);
		}


		/// <summary>
		/// AES加密Key支持的字节长度
		/// </summary>
		private static readonly int[] aesKeyByteLength = [16, 24, 32];

		/// <summary>
		/// AES加密字符串
		/// </summary>
		/// <param name="text">加密的字符串</param>
		/// <param name="key">秘钥,只能使用<see langword="16,24,32"/>字节长度的字符串作为秘钥</param>
		/// <returns>AES加密密文</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
		public static string AesEncrypt(string text, string key)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException(nameof(text));
			}
			byte[] byteKey = Encoding.UTF8.GetBytes(key);
			if (!aesKeyByteLength.Contains(byteKey.Length))
			{
				throw new Exception("Length does not meet the requirement, the actual length is '" + byteKey.Length + "'");
			}
			using Aes aesAlg = Aes.Create();
			aesAlg.Key = byteKey;
			aesAlg.Mode = CipherMode.ECB;
			aesAlg.Padding = PaddingMode.PKCS7;
			using ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
			using var msEncrypt = new MemoryStream();
			using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
			{
				using var swEncrypt = new StreamWriter(csEncrypt);
				swEncrypt.Write(text);
			}
			return System.Convert.ToBase64String(msEncrypt.ToArray());
		}

		/// <summary>
		/// AES解密字符串
		/// </summary>
		/// <param name="ciphertext">密文</param>
		/// <param name="key">秘钥,只能使用<see langword="16,24,32"/>字节长度的字符串作为秘钥</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
		public static string AesDecrypt(string ciphertext, string key)
		{
			if (string.IsNullOrEmpty(ciphertext))
			{
				throw new ArgumentNullException(nameof(ciphertext));
			}
			byte[] byteKey = Encoding.UTF8.GetBytes(key);
			if (!aesKeyByteLength.Contains(byteKey.Length))
			{
				throw new Exception("Length does not meet the requirement, the actual length is '" + byteKey.Length + "'");
			}
			using Aes aesAlg = Aes.Create();
			aesAlg.Key = byteKey;
			aesAlg.Mode = CipherMode.ECB;
			aesAlg.Padding = PaddingMode.PKCS7;
			using ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
			using var msDecrypt = new MemoryStream(System.Convert.FromBase64String(ciphertext));
			using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
			using var srDecrypt = new StreamReader(csDecrypt);
			return srDecrypt.ReadToEnd();
		}


		/// <summary>
		/// AES加密文件
		/// </summary>
		/// <param name="sourceFile">待加密的文件的完整路径</param>
		/// <param name="outputFile">加密后的文件的完整路径</param>
		/// <param name="secretKey">秘钥,只能使用<see langword="16,24,32"/>字节长度的字符串作为秘钥</param>
		public static bool AesFileEncrypt(string sourceFile, string outputFile, string secretKey)
		{
			byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
			if (!aesKeyByteLength.Contains(keyByte.Length))
			{
				throw new ArgumentException($"The length of the parameter byte is not correct, the correct length is [16, 24,32] and the current length '{keyByte.Length}'");
			}
			using Aes aesAlg = Aes.Create();
			aesAlg.Key = keyByte;
			aesAlg.GenerateIV();
			FileDir.ReadinessFileDirectory(outputFile);
			using FileStream fsInput = new(sourceFile, FileMode.Open, FileAccess.Read);
			using FileStream fsEncrypted = new(outputFile, FileMode.Create, FileAccess.Write);
			fsEncrypted.Write(aesAlg.IV, 0, aesAlg.IV.Length);
			using ICryptoTransform aesEncryptor = aesAlg.CreateEncryptor();
			using CryptoStream cryptoStream = new(fsEncrypted, aesEncryptor, CryptoStreamMode.Write);
			byte[] bytearrayinput = new byte[fsInput.Length];
			fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
			cryptoStream.Write(bytearrayinput, 0, bytearrayinput.Length);
			return File.Exists(outputFile);
		}


		/// <summary>
		/// AES文件解密
		/// </summary>
		/// <param name="sourceFile">加密文件路径</param>
		/// <param name="outputFile">解密后储存路径</param>
		/// <param name="secretKey">秘钥,只能使用<see langword="16,24,32"/>字节长度的字符串作为秘钥</param>
		/// <returns></returns>
		public static bool AesFileDecrypt(string sourceFile, string outputFile, string secretKey)
		{
			byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
			if (!aesKeyByteLength.Contains(keyByte.Length))
			{
				throw new ArgumentException($"The length of the parameter byte is not correct, the correct length is [16, 24,32] and the current length '{keyByte.Length}'");
			}
			using Aes aesAlg = Aes.Create();
			aesAlg.Key = keyByte;
			FileDir.ReadinessFileDirectory(outputFile);
			using FileStream fsInput = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
			byte[] iv = new byte[aesAlg.IV.Length];
			fsInput.Read(iv, 0, iv.Length);
			using FileStream fsDecrypted = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
			using ICryptoTransform aesDecryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);
			using CryptoStream cryptoStream = new CryptoStream(fsInput, aesDecryptor, CryptoStreamMode.Read);
			cryptoStream.CopyTo(fsDecrypted);
			return File.Exists(outputFile);
		}

		/// <summary>
		/// RSA秘钥
		/// </summary>
		public class RSASecretKey
		{
			private readonly string publicKey;
			private readonly string privateKey;
			/// <summary>
			/// 生成一对RSA秘钥
			/// </summary>
			public RSASecretKey()
			{
				using RSA rsa = RSA.Create();
				publicKey = System.Convert.ToBase64String(rsa.ExportRSAPublicKey());
				privateKey = System.Convert.ToBase64String(rsa.ExportRSAPrivateKey());
			}
			/// <summary>
			/// RSA公钥,用于加密
			/// </summary>
			public string PublicKey { get { return publicKey; } }
			/// <summary>
			/// RSA私钥,用于解密
			/// </summary>
			public string PrivateKey { get { return privateKey; } }
		}

		/// <summary>
		/// RSA加密字符串,可使用<see cref="RSASecretKey"/>生成秘钥
		/// </summary>
		/// <param name="text">需要加密的字符串</param>
		/// <param name="publicKey"><see cref="RSASecretKey"/>生成的公钥,需要使用对应的私钥进行解密,记得保留好私钥</param>
		/// <returns>密文</returns>
		public static string RsaEncryption(string text, string publicKey)
		{
			using RSA rsa = RSA.Create();
			rsa.ImportRSAPublicKey(System.Convert.FromBase64String(publicKey), out _);
			byte[] plainData = Encoding.UTF8.GetBytes(text);
			byte[] encryptedData = rsa.Encrypt(plainData, RSAEncryptionPadding.OaepSHA256);
			return System.Convert.ToBase64String(encryptedData);
		}

		/// <summary>
		/// RSA解密字符串
		/// </summary>
		/// <param name="ciphertext">密文</param>
		/// <param name="privateKey">密文的私钥</param>
		/// <returns></returns>
		public static string RsaDecryption(string ciphertext, string privateKey)
		{
			using RSA rsa = RSA.Create();
			rsa.ImportRSAPrivateKey(System.Convert.FromBase64String(privateKey), out _);
			byte[] encryptedData = System.Convert.FromBase64String(ciphertext);
			byte[] decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
			return Encoding.UTF8.GetString(decryptedData);
		}




	}
}
