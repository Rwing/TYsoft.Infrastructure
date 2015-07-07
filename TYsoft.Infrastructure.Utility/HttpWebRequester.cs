using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TYsoft.Infrastructure.Utility
{
	/// <summary>
	/// 对HttpWebRequest及HttpWebResponse的封装
	/// 临时的小封装，随便弄的- -
	/// </summary>
	public static class HttpWebRequester
	{
		/// <summary>
		/// 未进行异常捕捉，需要自行try catch
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string GetHtml(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Proxy = null;
			request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.15 (KHTML, like Gecko) Chrome/24.0.1295.0 Safari/537.15";
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					var content = reader.ReadToEnd();
					return content;
				}
			}
		}

		/// <summary>
		/// 只是简单的进行一个post地址，获得返回数据
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string Post(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Proxy = null;
			request.Method = "POST";
			// 获得接口返回值
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					var content = reader.ReadToEnd();
					return content;
				}
			}
		}

		/// <summary>
		/// 只是简单的进行一个post地址，获得返回数据
		/// </summary>
		/// <param name="url"></param>
		/// <param name="postData">{key}={value}&{key2}={value2}的形式</param>
		/// <returns></returns>
		public static string Post(string url, string postData)
		{
			byte[] dataBytes = Encoding.ASCII.GetBytes(postData);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Proxy = null;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = dataBytes.Length;
			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(dataBytes, 0, dataBytes.Length);
			}
			// 获得接口返回值
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					var content = reader.ReadToEnd();
					return content;
				}
			}
		}

		public static string PostFile(string url, string filePath)
		{
			//WebClient client = new WebClient();
			//byte[] result = client.UploadFile(url, "POST", filePath);
			//return Encoding.ASCII.GetString(result);
			string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
			byte[] data = File.ReadAllBytes(filePath);
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Proxy = null;
			request.Method = "POST";
			request.ContentLength = data.Length;
			request.Headers.Add("FileSize", data.Length.ToString());
			request.Headers.Add("FileName", fileName);
			using (var stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
				stream.Close();
			}
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					var content = reader.ReadToEnd();
					return content;
				}
			}
		}
	}
}
