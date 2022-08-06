// -----------------------------------------------------------------------
// Public Domain (www.unlicense.org)
// This is free and unencumbered software released into the public domain.
// Anyone is free to copy, modify, publish, use, compile, sell, or distribute this
// software, either in source code form or as a compiled binary, for any purpose,
// commercial or non-commercial, and by any means.
// In jurisdictions that recognize copyright laws, the author or authors of this
// software dedicate any and all copyright interest in the software to the public
// domain. We make this dedication for the benefit of the public at large and to
// the detriment of our heirs and successors. We intend this dedication to be an
// overt act of relinquishment in perpetuity of all present and future rights to
// this software under copyright law.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// -----------------------------------------------------------------------

using System.IO.Compression;
using System.Text;

namespace Exiled.Launcher.Features.Updater;

// Original: https://gist.github.com/ForeverZer0/a2cd292bd2f3b5e114956c00bb6e872b
public class TarGzExtractor
{
    /// <summary>
    /// Extracts a <i>.tar.gz</i> archive to the specified directory.
    /// </summary>
    /// <param name="filename">The <i>.tar.gz</i> to decompress and extract.</param>
    /// <param name="outputDir">Output directory to write the files.</param>
    public static void ExtractTarGz(string filename, string outputDir)
    {
        using var stream = File.OpenRead(filename);
        ExtractTarGz(stream, outputDir);
    }

    /// <summary>
    /// Extracts a <i>.tar.gz</i> archive stream to the specified directory.
    /// </summary>
    /// <param name="stream">The <i>.tar.gz</i> to decompress and extract.</param>
    /// <param name="outputDir">Output directory to write the files.</param>
    private static void ExtractTarGz(Stream stream, string outputDir)
    {
        // A GZipStream is not seekable, so copy it first to a MemoryStream
        using var gzip = new GZipStream(stream, CompressionMode.Decompress);
        const int chunk = 4096;
        using var memStr = new MemoryStream();
        int read;
        var buffer = new byte[chunk];

        while ((read = gzip.Read(buffer, 0, buffer.Length)) > 0)
        {
            memStr.Write(buffer, 0, read);
        }

        memStr.Seek(0, SeekOrigin.Begin);
        ExtractTar(memStr, outputDir);
    }

    /// <summary>
    /// Extractes a <c>tar</c> archive to the specified directory.
    /// </summary>
    /// <param name="stream">The <i>.tar</i> to extract.</param>
    /// <param name="outputDir">Output directory to write the files.</param>
    private static void ExtractTar(Stream stream, string outputDir)
    {
        var buffer = new byte[100];
        while (true)
        {
            stream.Read(buffer, 0, 100);
            var name = Encoding.ASCII.GetString(buffer).Trim('\0');
            if (string.IsNullOrWhiteSpace(name))
                break;
            stream.Seek(24, SeekOrigin.Current);
            stream.Read(buffer, 0, 12);
            var size = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);

            stream.Seek(376L, SeekOrigin.Current);

            var output = Path.Combine(outputDir, name);
            if (!Directory.Exists(Path.GetDirectoryName(output)))
                Directory.CreateDirectory(Path.GetDirectoryName(output)!);
            if (!name.Equals("./", StringComparison.InvariantCulture))
            {
                using var str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write);
                var buf = new byte[size];
                stream.Read(buf, 0, buf.Length);
                str.Write(buf, 0, buf.Length);
            }

            var pos = stream.Position;

            var offset = 512 - (pos  % 512);
            if (offset == 512)
                offset = 0;

            stream.Seek(offset, SeekOrigin.Current);
        }
    }
}
