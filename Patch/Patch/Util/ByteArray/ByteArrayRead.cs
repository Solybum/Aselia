using System;

namespace Aselia.Patch.Util.ByteArray
{
    public partial class ByteArray
    {
        /// <summary>
        /// Reads a <see cref="bool"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public bool ReadBoolean()
        {
            bool result = (_buffer[_position] != 0);
            _position += 1;
            return result;
        }
        /// <summary>
        /// Reads a 1 byte <see cref="char"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public char ReadCharA()
        {
            char result = Convert.ToChar(_buffer[_position]);
            _position += 1;
            return result;
        }
        /// <summary>
        /// Reads a 2 byte <see cref="char"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public char ReadCharW()
        {
            char result = Convert.ToChar(_buffer[_position + 0] | _buffer[_position + 1] << 8);
            _position += 2;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="sbyte"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public sbyte ReadInt8()
        {
            sbyte result = (sbyte)_buffer[_position];
            _position += 1;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="byte"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public byte ReadUInt8()
        {
            byte result = _buffer[_position];
            _position += 1;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="short"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public short ReadInt16()
        {
            short result = (short)(_buffer[_position + 0] | _buffer[_position + 1] << 8);
            _position += 2;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="ushort"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public ushort ReadUInt16()
        {
            ushort result = (ushort)(_buffer[_position + 0] | _buffer[_position + 1] << 8);
            _position += 2;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="int"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            int result = (_buffer[_position + 0] | _buffer[_position + 1] << 8 | _buffer[_position + 2] << 16 | _buffer[_position + 3] << 24);
            _position += 4;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="uint"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            uint result = (uint)(_buffer[_position + 0] | _buffer[_position + 1] << 8 | _buffer[_position + 2] << 16 | _buffer[_position + 3] << 24);
            _position += 4;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="long"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            uint lo = (uint)(_buffer[_position + 0] | _buffer[_position + 1] << 8 | _buffer[_position + 2] << 16 | _buffer[_position + 3] << 24);
            uint hi = (uint)(_buffer[_position + 4] | _buffer[_position + 5] << 8 | _buffer[_position + 6] << 16 | _buffer[_position + 7] << 24);
            long result = (long)((ulong)hi) << 32 | lo;
            _position += 8;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="ulong"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            uint lo = (uint)(_buffer[_position + 0] | _buffer[_position + 1] << 8 | _buffer[_position + 2] << 16 | _buffer[_position + 3] << 24);
            uint hi = (uint)(_buffer[_position + 4] | _buffer[_position + 5] << 8 | _buffer[_position + 6] << 16 | _buffer[_position + 7] << 24);
            ulong result = ((ulong)hi) << 32 | lo;
            _position += 8;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="float"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public float ReadSingle()
        {
            float result = BitConverter.ToSingle(_buffer, _position);
            _position += 4;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="double"/> advancing the internal position
        /// </summary>
        /// <returns></returns>
        public double ReadDouble()
        {
            double result = BitConverter.ToDouble(_buffer, _position);
            _position += 8;
            return result;
        }

        /// <summary>
        /// Reads array into a <see cref="sbyte"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Read(sbyte[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadInt8();
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="byte"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Read(byte[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadUInt8();
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="short"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Read(short[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadInt16();
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="ushort"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Read(ushort[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadUInt16();
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="int"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Read(int[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadInt32();
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="uint"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Read(uint[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadUInt32();
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="long"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Read(long[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadInt64();
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="ulong"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Read(ulong[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadUInt64();
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="float"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Read(float[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadSingle();
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="double"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Read(double[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadDouble();
                index++;
            }
        }

        /// <summary>
        /// Reads an ASCII string until the target length is reached or null character is found, advancing the internal position
        /// </summary>
        /// <param name="length">Maximum length of the resulting string</param>
        /// <returns></returns>
        public string ReadStringA(int length)
        {
            string result = "";
            while (result.Length < length)
            {
                if (_buffer[_position] == 0)
                {
                    break;
                }
                result += ReadCharA();
            }

            length -= result.Length;
            _position += length;
            
            return result;
        }
        /// <summary>
        /// Reads an UNICODE string until the target length is reached or null character is found, advancing the internal position
        /// </summary>
        /// <param name="length">Maximum length of the resulting string</param>
        /// <returns></returns>
        public string ReadStringW(int length)
        {
            string result = "";
            while (result.Length < length)
            {
                if (_buffer[_position] == 0)
                {
                    break;
                }
                result += ReadCharW();
            }

            length -= result.Length;
            _position += (length * 2);

            return result;
        }
        
        /// <summary>
        /// Reads a <see cref="bool"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public bool ReadBoolean(int position)
        {
            bool result = (_buffer[position] != 0);
            // position +=  1;
            return result;
        }
        /// <summary>
        /// Reads a 1 byte <see cref="char"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public char ReadCharA(int position)
        {
            char result = Convert.ToChar(_buffer[position]);
            // position += 1;
            return result;
        }
        /// <summary>
        /// Reads a 2 byte <see cref="char"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public char ReadCharW(int position)
        {
            char result = Convert.ToChar(_buffer[position + 0] | _buffer[position + 1] << 8);
            // position += 2;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="sbyte"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public sbyte ReadInt8(int position)
        {
            sbyte result = (sbyte)_buffer[position];
            // position +=  1;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="byte"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public byte ReadUInt8(int position)
        {
            byte result = _buffer[position];
            // position +=  1;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="short"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public short ReadInt16(int position)
        {
            short result = (short)(_buffer[position + 0] | _buffer[position + 1] << 8);
            // position +=  2;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="ushort"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public ushort ReadUInt16(int position)
        {
            ushort result = (ushort)(_buffer[position + 0] | _buffer[position + 1] << 8);
            // position +=  2;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="int"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public int ReadInt32(int position)
        {
            int result = (_buffer[position + 0] | _buffer[position + 1] << 8 | _buffer[position + 2] << 16 | _buffer[position + 3] << 24);
            // position +=  4;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="uint"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public uint ReadUInt32(int position)
        {
            uint result = (uint)(_buffer[position + 0] | _buffer[position + 1] << 8 | _buffer[position + 2] << 16 | _buffer[position + 3] << 24);
            // position +=  4;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="long"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public long ReadInt64(int position)
        {
            uint lo = (uint)(_buffer[position + 0] | _buffer[position + 1] << 8 | _buffer[position + 2] << 16 | _buffer[position + 3] << 24);
            uint hi = (uint)(_buffer[position + 4] | _buffer[position + 5] << 8 | _buffer[position + 6] << 16 | _buffer[position + 7] << 24);
            long result = (long)((ulong)hi) << 32 | lo;
            // position +=  8;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="ulong"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public ulong ReadUInt64(int position)
        {
            uint lo = (uint)(_buffer[position + 0] | _buffer[position + 1] << 8 | _buffer[position + 2] << 16 | _buffer[position + 3] << 24);
            uint hi = (uint)(_buffer[position + 4] | _buffer[position + 5] << 8 | _buffer[position + 6] << 16 | _buffer[position + 7] << 24);
            ulong result = ((ulong)hi) << 32 | lo;
            // position +=  8;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="float"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public float ReadSingle(int position)
        {
            float result = BitConverter.ToSingle(_buffer, position);
            // position +=  4;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="double"/> without advancing the internal position
        /// </summary>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public double ReadDouble(int position)
        {
            double result = BitConverter.ToDouble(_buffer, position);
            // position +=  8;
            return result;
        }

        /// <summary>
        /// Reads array into a <see cref="sbyte"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Read(sbyte[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadInt8(position);
                position += 1;
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="byte"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Read(byte[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadUInt8(position);
                position += 1;
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="short"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Read(short[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadInt16(position);
                position += 2;
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="ushort"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Read(ushort[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadUInt16(position);
                position += 2;
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="int"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Read(int[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadInt32(position);
                position += 4;
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="uint"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Read(uint[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadUInt32(position);
                position += 4;
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="long"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Read(long[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadInt64(position);
                position += 8;
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="ulong"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Read(ulong[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadUInt64(position);
                position += 8;
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="float"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Read(float[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadSingle(position);
                position += 4;
                index++;
            }
        }
        /// <summary>
        /// Reads array into a <see cref="double"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Read(double[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                array[index] = ReadDouble(position);
                position += 8;
                index++;
            }
        }

        /// <summary>
        /// Reads an ASCII string until the target length is reached or null character is found, without advancing the internal position
        /// </summary>
        /// <param name="length">Maximum length of the resulting string</param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public string ReadStringA(int length, int position)
        {
            string result = "";
            while (result.Length < length)
            {
                if (_buffer[position] == 0)
                {
                    break;
                }
                result += ReadCharA(position);
                position += 1;
            }
            return result;
        }
        /// <summary>
        /// Reads an UNICODE string until the target length is reached or null character is found, without advancing the internal position
        /// </summary>
        /// <param name="length">Maximum length of the resulting string</param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public string ReadStringW(int length, int position)
        {
            string result = "";
            while (result.Length < length)
            {
                if (_buffer[position] == 0)
                {
                    break;
                }
                result += ReadCharW(position);
                position += 2;
            }
            return result;
        }
    }
}
