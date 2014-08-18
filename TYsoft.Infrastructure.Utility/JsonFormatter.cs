using System.Text;
using System.Web.Script.Serialization;

namespace TYsoft.Infrastructure.Utility
{
	public static class JsonFormatter
	{
		public static string Serialize<T>(T obj)
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			StringBuilder sb = new StringBuilder();
			serializer.Serialize(obj, sb);
			return sb.ToString();
		}

		public static T Deserialize<T>(string json)
		{
			T result = default(T);
			try
			{
				result = new JavaScriptSerializer().Deserialize<T>(json);
			}
			catch (System.ArgumentException ex)
			{

			}
			return result;
		}

		//public static string Serialize<T>(T obj)
		//{
		//    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
		//    using (MemoryStream ms = new MemoryStream())
		//    {
		//        serializer.WriteObject(ms, obj);
		//        string jsonString = Encoding.UTF8.GetString(ms.ToArray());
		//        ms.Close();
		//    }
		//    return jsonString;
		//}

		//public static T Deserialize<T>(string json)
		//{
		//    DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
		//    T obj = default(T);
		//    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
		//    {
		//        T obj = (T)ds.ReadObject(ms);
		//        ms.Close();
		//    }
		//    return obj;
		//}
	}
}
