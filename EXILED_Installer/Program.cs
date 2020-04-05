using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace EXILED_Installer
{
	internal class Program
	{
		private const string url = "https://github.com/galaxy119/EXILED/releases/";

		public static void Main(string[] args)
		{
			if (args.Length < 1)
				args = new[] { "/home/scp/scp_server" };

			Console.WriteLine("Getting latest download URL...");
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "latest");
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			Stream stream = response.GetResponseStream();
			StreamReader reader = new StreamReader(stream);
			string read = reader.ReadToEnd();
			string[] readArray = read.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			string thing = readArray.FirstOrDefault(s => s.Contains("EXILED.tar.gz"));
			string sub = Between(thing, "/galaxy119/EXILED/releases/download/", "/EXILED.tar.gz");
			string path = $"{url}download/{sub}/EXILED.tar.gz";

			Console.WriteLine($"GitHub download URL found: {path}, downloading...");

			using (WebClient client = new WebClient()) client.DownloadFile(path, "EXILED.tar.gz");

			Console.WriteLine("Latest version downloaded, extracting...");
			ExtractTarGz("EXILED.tar.gz", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			Console.WriteLine("Extraction complete, moving files...");

			string installPath = Path.Combine(args[0], "SCPSL_Data");
			string managedPath = Path.Combine(installPath, "Managed");
			string fileName = Path.Combine(managedPath, "Assembly-CSharp.dll");

			if (!Directory.Exists(args[0]))
				throw new ArgumentException("The provided Managed folder does not exist.");

			File.Delete(fileName);
			File.Move(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assembly-CSharp.dll"), fileName);

			Console.WriteLine("Installation complete.");
		}

		private static string Between(string str, string firstString, string lastString)
		{
			int pos1 = str.IndexOf(firstString, StringComparison.Ordinal) + firstString.Length;
			int pos2 = str.IndexOf(lastString, StringComparison.Ordinal);
			string finalString = str.Substring(pos1, pos2 - pos1);
			return finalString;
		}

		private static void ExtractTarGz(string filename, string outputDir)
		{
			using (FileStream stream = File.OpenRead(filename))
			{
				ExtractTarGz(stream, outputDir);
			}
		}

		private static void ExtractTarGz(Stream stream, string outputDir)
		{
			using (GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress))
			{
				const int chunk = 4096;
				using (MemoryStream memStr = new MemoryStream())
				{
					int read;
					byte[] buffer = new byte[chunk];

					do
					{
						read = gzip.Read(buffer, 0, chunk);
						memStr.Write(buffer, 0, read);
					}
					while (read == chunk);

					memStr.Seek(0, SeekOrigin.Begin);
					ExtractTar(memStr, outputDir);
				}
			}
		}

		private static void ExtractTar(Stream stream, string outputDir)
		{
			byte[] buffer = new byte[100];
			while (true)
			{
				try
				{
					stream.Read(buffer, 0, 100);
					string name = Encoding.ASCII.GetString(buffer).Trim('\0');
					if (string.IsNullOrWhiteSpace(name))
						break;
					stream.Seek(24, SeekOrigin.Current);
					stream.Read(buffer, 0, 12);
					long size = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);

					stream.Seek(376L, SeekOrigin.Current);

					string output = Path.Combine(outputDir, name);
					if (!Directory.Exists(Path.GetDirectoryName(output)))
						Directory.CreateDirectory(Path.GetDirectoryName(output));
					if (!name.Equals("./", StringComparison.InvariantCulture))
					{
						using (FileStream str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write))
						{
							byte[] buf = new byte[size];
							stream.Read(buf, 0, buf.Length);
							str.Write(buf, 0, buf.Length);
						}
					}

					long pos = stream.Position;

					long offset = 512 - (pos % 512);
					if (offset == 512)
						offset = 0;

					stream.Seek(offset, SeekOrigin.Current);
				}
				catch (Exception)
				{
					// ignored
				}
			}
		}
	}
}