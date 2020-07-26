// -----------------------------------------------------------------------
// <copyright file="Installer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Installer
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Installs Exiled libraries on a SCP:SL game server.
    /// </summary>
    internal class Installer
    {
        private const string Url = "https://github.com/galaxy119/EXILED/releases/";

        /// <summary>
        /// Entry point of the program.
        /// </summary>
        /// <param name="args">Extra arguments, not required.</param>
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                    args = new[] { "/home/scp/scp_server" };

                Console.WriteLine("Getting latest download URL...");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                StreamReader reader = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream() ?? throw new NullReferenceException());

                string[] readArray = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                string subFolder =
                    Between(
                        readArray.FirstOrDefault(subFolderName => subFolderName.Contains("Exiled.tar.gz")),
                        "/galaxy119/EXILED/releases/download/",
                        "/Exiled.tar.gz");
                string fullPath = $"{Url}download/{subFolder}/Exiled.tar.gz";

                Console.WriteLine($"GitHub download URL found: {fullPath}, downloading...");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(fullPath, "Exiled.tar.gz");
                }

                Console.WriteLine("Latest version downloaded, extracting...");

                ExtractTarGz("Exiled.tar.gz", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

                Console.WriteLine("Extraction complete, moving files...");

                string installPath = Path.Combine(args[0], "SCPSL_Data");
                string managedPath = Path.Combine(installPath, "Managed");
                string fileName = Path.Combine(managedPath, "Assembly-CSharp.dll");

                if (!Directory.Exists(args[0]))
                    throw new ArgumentException("The provided Managed folder does not exist.");

                File.Delete(fileName);
                File.Move(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "Assembly-CSharp.dll"), fileName);

                Console.WriteLine("Installation complete.");
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static string Between(string str, string firstString, string lastString)
        {
            int firstPosition = str.IndexOf(firstString, StringComparison.Ordinal) + firstString.Length;

            return str.Substring(firstPosition, str.IndexOf(lastString, StringComparison.Ordinal) - firstPosition);
        }

        private static void ExtractTarGz(string filename, string outputDir)
        {
            using (FileStream stream = File.OpenRead(filename))
                ExtractTarGz(stream, outputDir);
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
                        Directory.CreateDirectory(Path.GetDirectoryName(output) ?? throw new NullReferenceException());

                    if (!name.Equals("./", StringComparison.InvariantCulture))
                    {
                        using (FileStream str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            byte[] buf = new byte[size];
                            stream.Read(buf, 0, buf.Length);
                            str.Write(buf, 0, buf.Length);
                        }
                    }

                    long position = stream.Position;
                    long offset = 512 - (position % 512);

                    if (offset == 512)
                        offset = 0;

                    stream.Seek(offset, SeekOrigin.Current);
                }
                catch (UnauthorizedAccessException)
                {
                    // ignored
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"An error has occurred while extracting Exiled files: {exception}");
                }
            }
        }
    }
}
