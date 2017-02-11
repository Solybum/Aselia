using System;

namespace Aselia.Patch.Util.ByteArray
{
    /// <summary>
    /// Wrapper to ease data type conversions to and from <see cref="byte"/> arrays
    /// </summary>
    public partial class ByteArray
    {
        private byte[] _buffer;
        private int _position;

        /// <summary>
        /// Reference to the internal buffer
        /// </summary>
        public byte[] Buffer
        {
            get { return _buffer; }
        }
        /// <summary>
        /// Length of the internal buffer
        /// </summary>
        public int Length
        {
            get { return _buffer.Length; }
        }
        /// <summary>
        /// Position in the internal buffer
        /// </summary>
        public int Position
        {
            get { return _position; }
            set
            {
                if (value < 0 || value > _buffer.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _position = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="size">Size for the internal buffer</param>
        public ByteArray(int size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }
            _buffer = new byte[size];
        }

        /// <summary>
        /// Alternative constructor to act as a wrapper for a byte array
        /// </summary>
        /// <param name="byteArray">Array reference to use as backing array</param>
        public ByteArray(byte[] byteArray)
        {
            _buffer = byteArray ?? throw new ArgumentNullException(nameof(byteArray));
        }

        /// <summary>
        /// Change the size of the internal array
        /// </summary>
        /// <param name="size">New size of the internal array</param>
        public void Resize(int size)
        {
            if (size == _buffer.Length)
            {
                return;
            }
            if (size < 0 || size > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Array.Resize(ref _buffer, size);
            if (_position > size)
            {
                _position = size;
            }
        }

        /// <summary>
        /// Sets all the elements of the array to their default value
        /// </summary>
        public void Clear()
        {
            this.Clear(0, _buffer.Length);
        }
        /// <summary>
        /// Sets a range of elements from the array to their default value
        /// </summary>
        /// <param name="index">The starting index of the elements to be cleared</param>
        /// <param name="length">The number of elements to clear</param>
        public void Clear(int index, int length)
        {
            Array.Clear(_buffer, index, length);
        }

        /// <summary>
        /// Show up to 16 bytes from the current position
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            int tslen = Length - _position;
            if (tslen > 16)
            {
                tslen = 16;
            }
            return string.Format("0x{0:X8}: {1}", _position, BitConverter.ToString(_buffer, _position, tslen).Replace("-", " "));
        }

        /// <summary>
        /// Indexer to access the individual bytes as a byte array
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte this[int offset]
        {
            get { return _buffer[offset]; }
            set { _buffer[offset] = value; }
        }
    }
}
