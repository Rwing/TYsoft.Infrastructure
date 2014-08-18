using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System;

namespace TYsoft.Infrastructure.Utility
{
	public static class XmlFormatter
	{
		public static string Serialize<T>(T obj)
		{
			using (StringWriter sw = new StringWriter())
			{
				XmlSerializer xs = new XmlSerializer(obj.GetType());
				xs.Serialize(sw, obj);
				return sw.ToString();
				//对于 SOAP 1.1 版编码，将 encodingStyle 参数设置为“http://schemas.xmlsoap.org/soap/encoding/”；对于 SOAP 1.2 版编码，将该参数设置为“http://www.w3.org/2001/12/soap-encoding”。
			}

			//try
			//{
			//    MemoryStream memoryStream = new MemoryStream();
			//    XmlSerializer xs = new XmlSerializer(obj.GetType());
			//    XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
			//    xs.Serialize(xmlTextWriter, obj);
			//    memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
			//    string xmlString = UTF8ByteArrayToString(memoryStream.ToArray());
			//    return xmlString;
			//}
			//catch (Exception e) { return null; }
		}

		private static string UTF8ByteArrayToString(byte[] characters)
		{
			UTF8Encoding encoding = new UTF8Encoding();
			String constructedString = encoding.GetString(characters);
			return (constructedString);
		}

		public static T Deserialize<T>(string s)
		{
			using (StringReader sr = new StringReader(s))
			{
				XmlSerializer xs = new XmlSerializer(typeof(T));
				return (T)xs.Deserialize(sr);
			}
		}
	}
}
