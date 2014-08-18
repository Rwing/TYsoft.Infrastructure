using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace TYsoft.Infrastructure.Utility
{
	public static class CryptographyExtensions
	{

		/// <summary>
		/// 转为16进制字符串
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToHexString(this byte[] source)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < source.Length; i++)
			{
				sb.Append(source[i].ToString("x2"));
			}
			return sb.ToString();
		}

		public static byte[] BaseHash(this string source, System.Security.Cryptography.HashAlgorithm algorithm)
		{
			return source.BaseHash(algorithm, "utf-8");
		}

		public static byte[] BaseHash(this string source, System.Security.Cryptography.HashAlgorithm algorithm, string encoding)
		{
			byte[] inputBytes = Encoding.GetEncoding(encoding).GetBytes(source);
			if (algorithm == null) return inputBytes;
			using (algorithm)
			{
				return algorithm.ComputeHash(inputBytes);
			}
		}

		/// <summary>
		/// 返回字符串的md5值
		/// </summary>
		/// <param name="source"></param>
		/// <returns>32位md5</returns>
		public static string ToMD5(this string source)
		{
			if (source.IsNullOrWhiteSpace()) return string.Empty;
			return source.BaseHash(System.Security.Cryptography.MD5.Create()).ToHexString();
		}

		/// <summary>
		/// 返回字符串的md5值 gb2312
		/// </summary>
		/// <param name="source"></param>
		/// <returns>32位md5</returns>
		public static string ToMD5gb2312(this string source)
		{
			if (source.IsNullOrWhiteSpace()) return string.Empty;
			return source.BaseHash(System.Security.Cryptography.MD5.Create(), "gb2312").ToHexString();
		}

		public static string ToHMACMD5(this string source, string key)
		{
			if (source.IsNullOrWhiteSpace()) return string.Empty;
			return source.BaseHash(new System.Security.Cryptography.HMACMD5(Encoding.UTF8.GetBytes(key))).ToHexString();
		}

		/// <summary>
		/// 返回字符串的SHA256值
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToSHA256(this string source)
		{
			if (source.IsNullOrWhiteSpace()) return string.Empty;
			return source.BaseHash(new System.Security.Cryptography.SHA256Managed()).ToHexString();
		}

		/// <summary>
		/// 返回字符串的SHA1值
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToSHA1(this string source)
		{
			if (source.IsNullOrWhiteSpace()) return string.Empty;
			return source.BaseHash(new System.Security.Cryptography.SHA1Managed()).ToHexString();
		}

		/// <summary>
		/// 返回字符串的base64编码
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToBase64(this string source)
		{
			if (source.IsNullOrWhiteSpace()) return string.Empty;
			return Convert.ToBase64String(source.BaseHash(null));
		}


		/// <summary>
		/// 返回字符串的SHA256值的base64编码
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToBase64SHA256(this string source)
		{
			if (source.IsNullOrWhiteSpace()) return string.Empty;
			return Convert.ToBase64String(source.BaseHash(new System.Security.Cryptography.SHA256Managed()));
		}

		public static string ToRijndaelDecryption(this string cipherText, string key, string IV)
		{
			return Convert.FromBase64String(cipherText).ToRijndaelDecryption(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(IV));
		}

		public static string ToRijndaelDecryption(this byte[] cipherText, byte[] key, byte[] IV)
		{
			// Check arguments.
			if (cipherText == null || cipherText.Length <= 0)
				throw new ArgumentNullException("cipherText");
			if (key == null || key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("IV");

			// Declare the RijndaelManaged object
			// used to decrypt the data.
			RijndaelManaged aesAlg = null;

			// Declare the string used to hold
			// the decrypted text.
			string plaintext = null;

			try
			{
				// Create a RijndaelManaged object
				// with the specified key and IV.
				aesAlg = new RijndaelManaged();
				aesAlg.Key = key;
				aesAlg.IV = IV;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
				// Create the streams used for decryption.
				using (MemoryStream msDecrypt = new MemoryStream(cipherText))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))

							// Read the decrypted bytes from the decrypting stream
							// and place them in a string.
							plaintext = srDecrypt.ReadToEnd();
					}
				}
			}
			finally
			{
				// Clear the RijndaelManaged object.
				if (aesAlg != null)
					aesAlg.Clear();
			}

			return plaintext;
		}

		public static string ToRijndaelEncryption(this string plainText, string key, string IV)
		{
			var result = plainText.ToRijndaelEncryption(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(IV));
			return Convert.ToBase64String(result);
		}

		public static byte[] ToRijndaelEncryption(this string plainText, byte[] key, byte[] IV)
		{
			if (plainText == null || plainText.Length <= 0)
				throw new ArgumentNullException("plainText");
			if (key == null || key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("IV");

			// Declare the stream used to encrypt to an in memory
			// array of bytes.
			MemoryStream msEncrypt = null;

			// Declare the RijndaelManaged object
			// used to encrypt the data.
			RijndaelManaged aesAlg = null;

			try
			{
				// Create a RijndaelManaged object
				// with the specified key and IV.
				aesAlg = new RijndaelManaged();
				aesAlg.Key = key;
				aesAlg.IV = IV;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for encryption.
				msEncrypt = new MemoryStream();
				using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
				{
					using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
					{

						//Write all data to the stream.
						swEncrypt.Write(plainText);
					}
				}
			}
			finally
			{
				// Clear the RijndaelManaged object.
				if (aesAlg != null)
					aesAlg.Clear();
			}

			// Return the encrypted bytes from the memory stream.
			return msEncrypt.ToArray();
		}
	}
}
