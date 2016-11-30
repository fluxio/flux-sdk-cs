using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flux.SDK
{
    /// <summary>Contains useful methods for Stream</summary>
    public static class StreamUtils
    {
        private const int CHUNK_SIZE = 1024;

        /// <summary>
        /// Decompresses response stream
        /// </summary>
        /// <param name="response">Response stream to decompress</param>
        /// <returns>Decompressed response stream</returns>
        public static Stream GetDecompressedResponseStream(HttpWebResponse response)
        {
            Stream decompressedStream = null;
            var responseStream = response.GetResponseStream();

            if (responseStream != null)
            {
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                    decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                    decompressedStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                else
                    return responseStream;
            }

            return decompressedStream;
        }

        /// <summary>
        /// Generates stream from the specified string
        /// </summary>
        /// <param name="s">The string to generate stream from</param>
        /// <returns>Generated stream</returns>
        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Generates string from the specified stream asynchronously
        /// </summary>
        /// <param name="stream">The stream to generate string from</param>
        /// <param name="length">The stream length</param>
        /// <returns>Generated string</returns>
        public static async Task<string> GetStringFromStreamAsync(Stream stream, long length = -1)
        {
            var sb = new StringBuilder();
            if (stream != null)
            {
                var cancellationToken = new CancellationToken();
                var buffer = new byte[CHUNK_SIZE];

                long streamLength = length > 0 ? length : stream.Length;

                int read = 0;
                for (int i = 0; i < streamLength; i += read)
                {
                    read = await stream.ReadAsync(buffer, 0, CHUNK_SIZE, cancellationToken);
                    if (read == 0)
                    {
                        await Task.Yield();
                        break;
                    }

                    string text = Encoding.UTF8.GetString(buffer, 0, read);
                    sb.Append(text);

                    if (stream.CanSeek)
                        stream.Position += read;
                }

                stream.Close();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates string from the specified stream
        /// </summary>
        /// <param name="stream">The stream to generate string from</param>
        /// <param name="length">The stream length</param>
        /// <returns>Generated string</returns>
        public static string GetStringFromStream(Stream stream, long length = -1)
        {
            var sb = new StringBuilder();
            if (stream != null)
            {
                var buffer = new byte[CHUNK_SIZE];

                if (stream.CanSeek)
                {
                    long streamLength = length > 0 ? length : stream.Length;

                    int read = 0;
                    for (int i = 0; i < streamLength; i += read)
                    {
                        read = stream.Read(buffer, 0, CHUNK_SIZE);
                        if (read == 0)
                            break;

                        string text = Encoding.UTF8.GetString(buffer, 0, read);
                        sb.Append(text);

                        if (stream.CanSeek)
                            stream.Position += read;
                    }
                }
                else
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var streamStr = reader.ReadToEnd();
                        sb.Append(streamStr);
                    }
                }

                stream.Close();
            }

            return sb.ToString();
        }
    }
}
