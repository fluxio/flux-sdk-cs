using System;
using System.IO;

namespace Flux.SDK.DataTableAPI.DatatableTypes
{
    /// <summary>Represents the cell value</summary>
    public struct CellValue : IDisposable
    {
        /// <summary>Stream value of the cell</summary>
        public Stream Stream { get; set; }

        /// <summary>Length of the Stream cell value</summary>
        public long StreamLength { get; set; }

        /// <summary>Disposes CellValue instance</summary>
        public void Dispose()
        {
            if (Stream != null)
                Stream.Dispose();
        }

        /// <summary>Converts Stream value of the cell to string</summary>
        /// <returns>The cell value as string</returns>
        /// <exception cref="Exceptions.InternalSDKException">Throw InternalSDKException if data can't be read.</exception>
        public string AsString()
        {
            if (Stream == null)
                return null;

            try
            {
                var valueAsStr = StreamUtils.GetStringFromStream(Stream, StreamLength);
                return valueAsStr;
            }
            catch (Exception ex)
            {
                throw new Exceptions.InternalSDKException(ex.Message);
            }
        }

        /// <summary>Converts Stream value of the cell to Int32</summary>
        /// <returns>The cell value as Int32</returns>
        /// <exception cref="Exceptions.InternalSDKException">Throw InternalSDKException if data can't be read.</exception>
        public int AsInt32()
        {
            try
            {
                var valueAsStr = StreamUtils.GetStringFromStream(Stream, StreamLength);
                return Int32.Parse(valueAsStr);
            }
            catch (Exception ex)
            {
                throw new Exceptions.InternalSDKException(ex.Message);
            }
        }

        /// <summary>Converts Stream value of the cell to Int64</summary>
        /// <returns>The cell value as Int32</returns>
        /// <exception cref="Exceptions.InternalSDKException">Throw InternalSDKException if data can't be read.</exception>
        public long AsInt64()
        {
            try
            {
                var valueAsStr = StreamUtils.GetStringFromStream(Stream, StreamLength);
                return Int64.Parse(valueAsStr);
            }
            catch (Exception ex)
            {
                throw new Exceptions.InternalSDKException(ex.Message);
            }
        }
    }
}