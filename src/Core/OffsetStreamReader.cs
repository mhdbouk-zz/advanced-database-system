using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class OffsetStreamReader
    {
        private const int InitialBufferSize = 4096;
        private readonly char _bom;
        private readonly byte _end;
        private readonly Encoding _encoding;
        private readonly Stream _stream;
        private readonly bool _tail;

        private byte[] _buffer;
        private int _processedInBuffer;
        private int _informationInBuffer;

        public OffsetStreamReader(Stream stream, bool tail)
        {
            _buffer = new byte[InitialBufferSize];
            _processedInBuffer = InitialBufferSize;

            if (stream == null || !stream.CanRead)
                throw new ArgumentException("stream");

            _stream = stream;
            _tail = tail;
            _encoding = Encoding.UTF8;

            _bom = '\uFEFF';
            _end = _encoding.GetBytes(new[] { '\n' })[0];
        }

        public long Offset { get; private set; }

        public string ReadLine()
        {
            if (!_stream.CanRead)
                return null;

            if (_processedInBuffer == _informationInBuffer)
            {
                if (_tail)
                {
                    _processedInBuffer = _buffer.Length;
                    _informationInBuffer = 0;
                    ReadBuffer();
                }

                return null;
            }

            var lineEnd = Search(_buffer, _end, _processedInBuffer);
            var haveEnd = true;

            if (lineEnd.HasValue == false && _informationInBuffer + _processedInBuffer < _buffer.Length)
            {
                if (_tail)
                    return null;
                else
                {
                    lineEnd = _informationInBuffer;
                    haveEnd = false;
                }
            }

            if (!lineEnd.HasValue)
            {
                ReadBuffer();
                if (_informationInBuffer != 0)
                    return ReadLine();

                return null;
            }

            var arr = new byte[lineEnd.Value - _processedInBuffer];
            Array.Copy(_buffer, _processedInBuffer, arr, 0, arr.Length);

            Offset = Offset + lineEnd.Value - _processedInBuffer + (haveEnd ? 1 : 0);
            _processedInBuffer = lineEnd.Value + (haveEnd ? 1 : 0);

            return _encoding.GetString(arr).TrimStart(_bom).TrimEnd('\r', '\n');
        }

        private void ReadBuffer()
        {
            var notProcessedPartLength = _buffer.Length - _processedInBuffer;

            if (notProcessedPartLength == _buffer.Length)
            {
                var extendedBuffer = new byte[_buffer.Length + _buffer.Length / 2];
                Array.Copy(_buffer, extendedBuffer, _buffer.Length);
                _buffer = extendedBuffer;
            }

            Array.Copy(_buffer, (long)_processedInBuffer, _buffer, 0, notProcessedPartLength);

            _informationInBuffer = notProcessedPartLength + _stream.Read(_buffer, notProcessedPartLength, _buffer.Length - notProcessedPartLength);

            _processedInBuffer = 0;
        }

        private int? Search(byte[] buffer, byte byteToSearch, int bufferOffset)
        {
            for (int i = bufferOffset; i < buffer.Length - 1; i++)
            {
                if (buffer[i] == byteToSearch)
                    return i;
            }
            return null;
        }
    }

}
