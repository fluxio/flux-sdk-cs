using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Flux.SDK.DataTableAPI.DatatableTypes
{
    /// <summary>Provides a wrapper for System.IO.Stream which reports reading/writing progress.</summary>
    public class ProgressStream : Stream, IDisposable
    {
        private Stream sourceStream;
        private long position;
        private long streamLength;

        /// <summary>Reports progress when read/write operation on strem performed. Returns number of read/write bytes and stream length.</summary>
        public event ProgressEventHandler OnProgressChanged;

        /// <summary>Initializes a new instance of the Progress Stream with specified sourse stream and length.</summary>
        /// <param name="sourceStream">Stream to report progress for.</param>
        /// <param name="length">Stream full length.</param>
        public ProgressStream(Stream sourceStream, long length)
        {
            this.sourceStream = sourceStream;
            streamLength = length;
            position = 0;
        }

        /// <summary>Initializes a new instance of the Progress Stream with specified sourse stream and length.</summary>
        /// <param name="sourceStream">Stream to report progress for.</param>
        public ProgressStream(Stream sourceStream)
        {
            this.sourceStream = sourceStream;
            if (this.sourceStream.CanSeek)
            {
                streamLength = this.sourceStream.Length;
                position = this.sourceStream.Position;
            }
            else
            {
                streamLength = 0;
                position = 0;
            }
        }

        /// <summary>Gets a value indicating whether the current stream supports reading.</summary>
        /// <returns>true if the stream supports reading; false if the stream is closed or stream is write-only.</returns>
        public override bool CanRead
        {
            get { return sourceStream.CanRead; }
        }


        /// <summary>Gets a value indicating whether the current stream supports seeking.</summary>
        /// <returns>true if the stream supports seeking; false if the stream is closed or doesn't support seeking</returns>
        public override bool CanSeek
        {
            get { return sourceStream.CanSeek; }
        }

        /// <summary>Gets a value indicating whether the current stream supports writing.</summary>
        /// <returns>true if the stream supports writing; false if the stream is closed or stream is read-only.</returns>
        public override bool CanWrite
        {
            get { return sourceStream.CanWrite; }
        }

        /// <summary>Clears buffers for this stream and causes any buffered data to be written.</summary>
        public override void Flush()
        {
            sourceStream.Flush();
        }

        /// <summary>Gets the length in bytes of the stream.</summary>        
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        public override long Length
        {
            get { return streamLength; }
        }

        /// <summary>Gets or sets the current position of this stream.</summary>
        ///<returns>The current position of this stream.</returns>
        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                if (position != value)
                {
                    position = value;

                    if (sourceStream.CanSeek)
                        sourceStream.Position = position;

                    if (OnProgressChanged != null)
                        OnProgressChanged(this, new ProgressEventArgs(position, streamLength));
                }
            }
        }

        /// <summary>Reads a block of bytes from the stream and writes the data in a given buffer with reporting reading progress.</summary>
        /// <param name="buffer">When this method returns, contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The byte offset in array at which the read bytes will be placed.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>The maximum number of bytes to read.</returns>
        /// <exception cref="System.ArgumentException"> The sum of offset and count is larger than the buffer length.</exception>
        /// <exception cref="System.ArgumentNullException">Buffer is null.</exception>        
        /// <exception cref="System.ArgumentOutOfRangeException">Offset or count is negative.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int res = sourceStream.Read(buffer, offset, count);
            Position += res;
            return res;
        }

        /// <summary>Sets the position within the current stream.</summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support seeking.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return sourceStream.Seek(offset, origin);
        }

        /// <summary>Sets the length of the current stream.</summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support both writing and seeking.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override void SetLength(long value)
        {
            sourceStream.SetLength(value);
            streamLength = value;
        }

        /// <summary>Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream</summary>
        public override void Close()
        {
            if(sourceStream != null)
                sourceStream.Close();

            base.Close();
        }

        /// <summary>Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written and reports writing progress.</summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="System.ArgumentException"> The sum of offset and count is larger than the buffer length.</exception>
        /// <exception cref="System.ArgumentNullException">Buffer is null.</exception>        
        /// <exception cref="System.ArgumentOutOfRangeException">Offset or count is negative.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            sourceStream.Write(buffer, offset, count);
        }

        /// <summary>Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests with reporting reading progress.</summary>
        /// <param name="buffer">The buffer to write the data into.</param>
        /// <param name="offset">The byte offset in buffer at which to begin writing data from the stream.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
        /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the total number of bytes read into the buffer. The result value can be less than the number of bytes requested if the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached.</returns>
        /// <exception cref="System.ArgumentException"> The sum of offset and count is larger than the buffer length.</exception>
        /// <exception cref="System.ArgumentNullException">Buffer is null.</exception>        
        /// <exception cref="System.ArgumentOutOfRangeException">Offset or count is negative.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <exception cref="System.InvalidOperationException">The stream is currently in use by a previous read operation.</exception>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return sourceStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        /// <summary>Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests with reporting writing progress.</summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The zero-based byte offset in buffer from which to begin copying bytes to the stream.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <exception cref="System.ArgumentException"> The sum of offset and count is larger than the buffer length.</exception>
        /// <exception cref="System.ArgumentNullException">Buffer is null.</exception>        
        /// <exception cref="System.ArgumentOutOfRangeException">Offset or count is negative.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <exception cref="System.InvalidOperationException">The stream is currently in use by a previous write operation.</exception>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return sourceStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        /// <summary>Asynchronously clears all buffers for this stream, causes any buffered data to be written to source stream, and monitors cancellation requests.</summary>
        /// <param name="cancellationToken">he token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
        /// <returns>A task that represents the asynchronous flush operation.</returns>
        /// <exception cref="System.ObjectDisposedException">The stream has been disposed.</exception>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return sourceStream.FlushAsync(cancellationToken);
        }

        /// <summary>Raizes OnProgressChanged event with current position and length.</summary>
        public void ReportMaxValue()
        {
            if (OnProgressChanged != null)
                OnProgressChanged(this, new ProgressEventArgs(position, streamLength));
        }
    }

    /// <summary>Represents the method that will handle the ProgressChanged event.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">ProgressEventArgs that contains progress data.</param>
    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

    /// <summary>Represents the class that contain read/write progress data.</summary>
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>Returns current position in steam.</summary>
        public long Position { get; private set; }
        /// <summary>Returns stream length.</summary>
        public long Length { get; private set; }

        /// <summary>Creates instance of ProgressEventArgs with specified position and length.</summary>
        /// <param name="position">Current position in steam.</param>
        /// <param name="length">Stream length</param>
        public ProgressEventArgs(long position, long length)
        {
            Position = position;
            Length = length;
        }
    }
}
