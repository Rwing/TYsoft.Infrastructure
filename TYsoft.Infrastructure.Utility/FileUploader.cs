using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Drawing;

namespace TYsoft.Infrastructure.Utility
{
	/// <summary>
	/// 先LoadFile，再Save。允许Load多个后再Save
	/// TODO: 很简易，许多功能未完成，例如限定上传类型等
	/// </summary>
	public class FileUploader
	{
		#region private fields

		private List<BaseUploadFile> _files;

		#endregion

		#region properties

		public List<BaseUploadFile> Files { get { return _files; } }

		public bool RandomFileName { get; private set; }

		#endregion

		public FileUploader()
			: this(true)
		{ }

		public FileUploader(bool randomFileName)
		{
			RandomFileName = randomFileName;
		}

		/// <summary>
		/// 加载文件，此时并未保存 需要调用save方法保存
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public BaseUploadFile LoadFile(HttpPostedFileBase file)
		{
			if (_files == null)
				_files = new List<BaseUploadFile>();
			var uploadFile = ConvertToUploadFile(file);
			_files.Add(uploadFile);
			return uploadFile;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <param name="ext">后缀名</param>
		/// <returns></returns>
		public BaseUploadFile LoadFile(byte[] file, string ext)
		{
			if (_files == null)
				_files = new List<BaseUploadFile>();
			var uploadFile = ConvertToUploadFile(file, ext);
			_files.Add(uploadFile);
			return uploadFile;
		}

		private BaseUploadFile ConvertToUploadFile(byte[] file, string ext)
		{
			BaseUploadFile UploadFile = UploadFileFactory.CreateInstance("", ext);
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(file, 0, file.Length);
			UploadFile.InputStream = memoryStream;
			UploadFile.ExtensionName = ext;
			UploadFile.FileName = Guid.NewGuid() + "." + UploadFile.ExtensionName;
			return BaseConvertToUploadFile(UploadFile);
		}

		private BaseUploadFile ConvertToUploadFile(HttpPostedFileBase file)
		{
			string ext = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);
			BaseUploadFile UploadFile = UploadFileFactory.CreateInstance(file.ContentType, ext);
			UploadFile.OriginalFile = file;
			UploadFile.InputStream = file.InputStream;
			UploadFile.ExtensionName = ext;
			UploadFile.FileName = (this.RandomFileName ? Guid.NewGuid() + "." + UploadFile.ExtensionName : file.FileName);
			UploadFile.FileLength = file.ContentLength;
			UploadFile.MIMEType = file.ContentType;
			return BaseConvertToUploadFile(UploadFile);
		}

		private BaseUploadFile BaseConvertToUploadFile(BaseUploadFile uploadFile)
		{
			string uploadPhysicalPath = AppSettings.UploadPhysicalPath;
			string uploadVirtualPath = AppSettings.UploadVirtualPath;
			if (uploadPhysicalPath.IsNullOrWhiteSpace())
				throw new NullReferenceException("config中未配置UploadPhysicalPath");
			if (uploadVirtualPath.IsNullOrWhiteSpace())
				uploadVirtualPath = uploadPhysicalPath.Replace("\\", "/");
			bool isAbsolute = uploadPhysicalPath.IndexOf(":") >= 0;//如果有:则为绝对路径例如c:\upload\
			uploadPhysicalPath = uploadPhysicalPath.EndsWith("\\") ? uploadPhysicalPath : uploadPhysicalPath + "\\";
			uploadPhysicalPath = isAbsolute ? uploadPhysicalPath : RequestInfos.RootDirectoryPhysicalPath+uploadPhysicalPath;
			isAbsolute = uploadVirtualPath.IndexOf(".") >= 0;//如果有.则为绝对网址例如images.xxx.com/
			uploadVirtualPath = uploadVirtualPath.EndsWith("/") ? uploadVirtualPath : uploadVirtualPath + "/";
			uploadVirtualPath = isAbsolute && !uploadVirtualPath.StartsWith("http://") ? "http://" + uploadVirtualPath : uploadVirtualPath;

			string subDir = uploadFile.SubDir + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
			uploadFile.SavePhysicalPath = Path.Combine(uploadPhysicalPath, subDir);
			uploadFile.SaveVirtualPath = uploadVirtualPath + subDir.Replace("\\", "/");
			return uploadFile;
		}


		public static readonly string[] ALLOW_EXTS = { "", "", "" };
		public static readonly string[] ALLOW_MIMETYPES = { "", "", "" };

		public virtual void Save()
		{
			if (_files != null)
				_files.ForEach(item => item.Save());
		}
	}

	//public enum UploadFileType
	//{
	//    Image,
	//    Document,
	//    Zipped,
	//    Unknow
	//}

	public class UploadFileFactory
	{
		public static BaseUploadFile CreateInstance(string mimeType, string ext)
		{
			//TODO:根据mimetype获取相应的文件类型
			ext = ext.ToUpper();
			if (ext.Equals("CSV", StringComparison.OrdinalIgnoreCase))
				return new CsvFile();
			else if (ext.Equals("JPG", StringComparison.OrdinalIgnoreCase) ||
					ext.Equals("JPEG", StringComparison.OrdinalIgnoreCase) ||
					ext.Equals("GIF", StringComparison.OrdinalIgnoreCase) ||
					ext.Equals("PNG", StringComparison.OrdinalIgnoreCase) ||
					ext.Equals("BMP", StringComparison.OrdinalIgnoreCase))
				return new ImageFile();
			else if (ext.Equals("RAR", StringComparison.OrdinalIgnoreCase) ||
					ext.Equals("ZIP", StringComparison.OrdinalIgnoreCase) ||
					ext.Equals("7Z", StringComparison.OrdinalIgnoreCase))
				return new ZippedFile();
			else if (ext.Equals("TXT", StringComparison.OrdinalIgnoreCase) ||
				ext.Equals("DOC", StringComparison.OrdinalIgnoreCase) ||
				ext.Equals("DOCX", StringComparison.OrdinalIgnoreCase))
				return new DocFile();
			else
				return new UnknowFile();
		}
	}

	/// <summary>
	/// 上传文件格式错误异常
	/// </summary>
	[Serializable]
	public class FileFormatInvalidException : Exception
	{
		public FileFormatInvalidException() : base("文件格式非法") { }
		public FileFormatInvalidException(string message) : base(message) { }
		public FileFormatInvalidException(string message, Exception inner) : base(message, inner) { }
		protected FileFormatInvalidException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// 上传的文件信息 for FileUploader
	/// </summary>
	public class BaseUploadFile
	{
		public BaseUploadFile()
		{

		}

		public BaseUploadFile(string subDir)
		{
			SubDir = subDir;
		}

		public string MIMEType { get; set; }
		public int FileLength { get; set; }
		/// <summary>
		/// 新文件名 含扩展名
		/// </summary>
		public string FileName { get; set; }
		public string ExtensionName { get; set; }
		public Stream InputStream { get; set; }
		/// <summary>
		/// 文件的绝对物理路径 不含文件名
		/// </summary>
		public string SavePhysicalPath { get; set; }

		/// <summary>
		/// 文件的虚拟网站路径 不含文件名
		/// </summary>
		public string SaveVirtualPath { get; set; }

		/// <summary>
		/// 原始文件
		/// </summary>
		public HttpPostedFileBase OriginalFile { get; set; }

		/// <summary>
		/// 文件保存的子文件夹
		/// </summary>
		public string SubDir { get; protected set; }

		public virtual void Save()
		{
			if (!Directory.Exists(this.SavePhysicalPath))
				Directory.CreateDirectory(this.SavePhysicalPath);
			if (OriginalFile != null)
				OriginalFile.SaveAs(this.SavePhysicalPath + this.FileName);
		}
	}

	public class UnknowFile : BaseUploadFile
	{
		public UnknowFile()
			: base("Unknow")
		{ }
	}

	public class ZippedFile : BaseUploadFile
	{
		public ZippedFile()
			: base("Zipped")
		{ }
	}

	public class CsvFile : BaseUploadFile
	{
		public CsvFile()
			: base("CSV")
		{ }
	}

	public class DocFile : BaseUploadFile
	{
		public DocFile()
			: base("DOC")
		{ }
	}

	public class ImageFile : BaseUploadFile
	{
		private Image _image;

		public ImageFile()
			: base("Image")
		{ }

		public override void Save()
		{
			try
			{
				_image = Image.FromStream(base.InputStream);
			}
			catch (Exception e)
			{
				throw e;
			}
			if (!Directory.Exists(base.SavePhysicalPath))
				Directory.CreateDirectory(base.SavePhysicalPath);
			this._image.Save(base.SavePhysicalPath + base.FileName);
		}

	}
}
