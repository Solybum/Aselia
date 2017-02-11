using System;

namespace Aselia.Patch.Util.ByteArray
{
    public partial class ByteArray
    {
        /// <summary>
        /// Writes a <see cref="bool"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(bool value)
        {
            _buffer[_position] = (byte)(value ? 1 : 0);
            _position += 1;
        }
        /// <summary>
        /// Writes a 1 byte <see cref="char"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void WriteCharA(char value)
        {
            _buffer[_position] = Convert.ToByte(value);
            _position += 1;
        }
        /// <summary>
        /// Writes a 2 byte <see cref="char"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void WriteCharW(char value)
        {
            ushort tmp = Convert.ToUInt16(value);
            _buffer[_position + 0] = (byte)tmp;
            _buffer[_position + 1] = (byte)(tmp >> 8);
            _position += 2;
        }
        /// <summary>
        /// Writes a <see cref="sbyte"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(sbyte value)
        {
            _buffer[_position] = (byte)value;
            _position += 1;
        }
        /// <summary>
        /// Writes a <see cref="byte"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(byte value)
        {
            _buffer[_position] = value;
            _position += 1;
        }
        /// <summary>
        /// Writes a <see cref="short"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(short value)
        {
            _buffer[_position + 0] = (byte)value;
            _buffer[_position + 1] = (byte)(value >> 8);
            _position += 2;
        }
        /// <summary>
        /// Writes a <see cref="ushort"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(ushort value)
        {
            _buffer[_position + 0] = (byte)value;
            _buffer[_position + 1] = (byte)(value >> 8);
            _position += 2;
        }
        /// <summary>
        /// Writes a <see cref="int"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(int value)
        {
            _buffer[_position + 0] = (byte)value;
            _buffer[_position + 1] = (byte)(value >> 8);
            _buffer[_position + 2] = (byte)(value >> 16);
            _buffer[_position + 3] = (byte)(value >> 24);
            _position += 4;
        }
        /// <summary>
        /// Writes a <see cref="uint"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(uint value)
        {
            _buffer[_position + 0] = (byte)value;
            _buffer[_position + 1] = (byte)(value >> 8);
            _buffer[_position + 2] = (byte)(value >> 16);
            _buffer[_position + 3] = (byte)(value >> 24);
            _position += 4;
        }
        /// <summary>
        /// Writes a <see cref="long"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(long value)
        {
            _buffer[_position + 0] = (byte)value;
            _buffer[_position + 1] = (byte)(value >> 8);
            _buffer[_position + 2] = (byte)(value >> 16);
            _buffer[_position + 3] = (byte)(value >> 24);
            _buffer[_position + 4] = (byte)(value >> 32);
            _buffer[_position + 5] = (byte)(value >> 40);
            _buffer[_position + 6] = (byte)(value >> 48);
            _buffer[_position + 7] = (byte)(value >> 56);
            _position += 8;
        }
        /// <summary>
        /// Writes a <see cref="ulong"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(ulong value)
        {
            _buffer[_position + 0] = (byte)value;
            _buffer[_position + 1] = (byte)(value >> 8);
            _buffer[_position + 2] = (byte)(value >> 16);
            _buffer[_position + 3] = (byte)(value >> 24);
            _buffer[_position + 4] = (byte)(value >> 32);
            _buffer[_position + 5] = (byte)(value >> 40);
            _buffer[_position + 6] = (byte)(value >> 48);
            _buffer[_position + 7] = (byte)(value >> 56);
            _position += 8;
        }
        /// <summary>
        /// Writes a <see cref="float"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(float value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _buffer, _position, 4);
            _position += 4;
        }
        /// <summary>
        /// Writes a <see cref="double"/> advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Write(double value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _buffer, _position, 8);
            _position += 8;
        }

        /// <summary>
        /// Writes array into a <see cref="sbyte"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Write(sbyte[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                Write(array[index]);
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="byte"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Write(byte[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                Write(array[index]);
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="short"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Write(short[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                Write(array[index]);
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="ushort"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Write(ushort[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                Write(array[index]);
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="int"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Write(int[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                Write(array[index]);
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="uint"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Write(uint[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                Write(array[index]);
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="long"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Write(long[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                Write(array[index]);
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="ulong"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Write(ulong[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                Write(array[index]);
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="float"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Write(float[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                Write(array[index]);
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="double"/> array, advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        public void Write(double[] array, int index, int length)
        {
            length += index;
            while (index < length)
            {
                Write(array[index]);
                index++;
            }
        }

        /// <summary>
        /// Writes an ASCII string, advancing the internal position.
        /// If the string lenght is less than the target length, the remaining bytes will be set to zero (null)
        /// </summary>
        /// <param name="text">String to write to the byte array</param>
        /// <param name="index">Starting position of the string</param>
        /// <param name="length">Amount of characters to write</param>
        /// <param name="nullTerminated">Null terminated the string in the buffer, this null character is not counted in the length parameter</param>
        /// <returns></returns>
        public void WriteStringA(string text, int index, int length, bool nullTerminated)
        {
            length += index;
            while ((index < length) && (index < text.Length))
            {
                WriteCharA(text[index]);
                index++;
            }
            while (index < length)
            {
                WriteCharA('\0');
                index++;
            }
            if (nullTerminated)
            {
                WriteCharA('\0');
            }
        }
        /// <summary>
        /// Writes an UNICODE string, advancing the internal position.
        /// If the string lenght is less than the target length, the remaining bytes will be set to zero (null)
        /// </summary>
        /// <param name="text">String to write to the byte array</param>
        /// <param name="index">Starting position of the string</param>
        /// <param name="length">Amount of characters to write</param>
        /// <param name="nullTerminated">Null terminated the string in the buffer, this null character is not counted in the length parameter</param>
        /// <returns></returns>
        public void WriteStringW(string text, int index, int length, bool nullTerminated)
        {
            length += index;
            while ((index < length) && (index < text.Length))
            {
                WriteCharW(text[index]);
                index++;
            }
            while (index < length)
            {
                WriteCharW('\0');
                index++;
            }
            if (nullTerminated)
            {
                WriteCharW('\0');
            }
        }

        /// <summary>
        /// Writes a <see cref="bool"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(bool value, int position)
        {
            _buffer[position] = (byte)(value ? 1 : 0);
            // position += 1;
        }
        /// <summary>
        /// Writes a 1 byte <see cref="char"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void WriteCharA(char value, int position)
        {
            _buffer[position] = Convert.ToByte(value);
            // position += 1;
        }
        /// <summary>
        /// Writes a 2 byte <see cref="char"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void WriteCharW(char value, int position)
        {
            ushort tmp = Convert.ToUInt16(value);
            _buffer[position + 0] = (byte)tmp;
            _buffer[position + 1] = (byte)(tmp >> 8);
            // position += 1;
        }
        /// <summary>
        /// Writes a <see cref="sbyte"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(sbyte value, int position)
        {
            _buffer[position] = (byte)value;
            // position += 1;
        }
        /// <summary>
        /// Writes a <see cref="byte"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(byte value, int position)
        {
            _buffer[position] = value;
            // position += 1;
        }
        /// <summary>
        /// Writes a <see cref="short"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(short value, int position)
        {
            _buffer[position + 0] = (byte)value;
            _buffer[position + 1] = (byte)(value >> 8);
            // position += 2;
        }
        /// <summary>
        /// Writes a <see cref="ushort"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(ushort value, int position)
        {
            _buffer[position + 0] = (byte)value;
            _buffer[position + 1] = (byte)(value >> 8);
            // position += 2;
        }
        /// <summary>
        /// Writes a <see cref="int"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(int value, int position)
        {
            _buffer[position + 0] = (byte)value;
            _buffer[position + 1] = (byte)(value >> 8);
            _buffer[position + 2] = (byte)(value >> 16);
            _buffer[position + 3] = (byte)(value >> 24);
            // position += 4;
        }
        /// <summary>
        /// Writes a <see cref="uint"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(uint value, int position)
        {
            _buffer[position + 0] = (byte)value;
            _buffer[position + 1] = (byte)(value >> 8);
            _buffer[position + 2] = (byte)(value >> 16);
            _buffer[position + 3] = (byte)(value >> 24);
            // position += 4;
        }
        /// <summary>
        /// Writes a <see cref="long"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(long value, int position)
        {
            _buffer[position + 0] = (byte)value;
            _buffer[position + 1] = (byte)(value >> 8);
            _buffer[position + 2] = (byte)(value >> 16);
            _buffer[position + 3] = (byte)(value >> 24);
            _buffer[position + 4] = (byte)(value >> 32);
            _buffer[position + 5] = (byte)(value >> 40);
            _buffer[position + 6] = (byte)(value >> 48);
            _buffer[position + 7] = (byte)(value >> 56);
            // position += 8;
        }
        /// <summary>
        /// Writes a <see cref="ulong"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(ulong value, int position)
        {
            _buffer[position + 0] = (byte)value;
            _buffer[position + 1] = (byte)(value >> 8);
            _buffer[position + 2] = (byte)(value >> 16);
            _buffer[position + 3] = (byte)(value >> 24);
            _buffer[position + 4] = (byte)(value >> 32);
            _buffer[position + 5] = (byte)(value >> 40);
            _buffer[position + 6] = (byte)(value >> 48);
            _buffer[position + 7] = (byte)(value >> 56);
            // position += 8;
        }
        /// <summary>
        /// Writes a <see cref="float"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(float value, int position)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _buffer, position, 4);
            // position += 4;
        }
        /// <summary>
        /// Writes a <see cref="double"/> without advancing the internal position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void Write(double value, int position)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _buffer, position, 8);
            // position += 8;
        }
        
        /// <summary>
        /// Writes array into a <see cref="sbyte"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Write(sbyte[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                Write(array[index], position);
                position += 1;
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="byte"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Write(byte[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                Write(array[index], position);
                position += 1;
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="short"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Write(short[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                Write(array[index], position);
                position += 2;
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="ushort"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Write(ushort[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                Write(array[index], position);
                position += 2;
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="int"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Write(int[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                Write(array[index], position);
                position += 4;
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="uint"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Write(uint[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                Write(array[index], position);
                position += 4;
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="long"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Write(long[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                Write(array[index], position);
                position += 8;
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="ulong"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Write(ulong[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                Write(array[index], position);
                position += 8;
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="float"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Write(float[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                Write(array[index], position);
                position += 4;
                index++;
            }
        }
        /// <summary>
        /// Writes array into a <see cref="double"/> array, without advancing the internal position
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">Starting position in the array</param>
        /// <param name="length">Amount of array values to read</param>
        /// <param name="position">Starting position of the internal buffer</param>
        public void Write(double[] array, int index, int length, int position)
        {
            length += index;
            while (index < length)
            {
                Write(array[index], position);
                position += 8;
                index++;
            }
        }

        /// <summary>
        /// Writes an ASCII string, without advancing the internal position.
        /// If the string lenght is less than the target length, the remaining bytes will be set to zero (null)
        /// </summary>
        /// <param name="text">String to write to the byte array</param>
        /// <param name="index">Starting position of the string</param>
        /// <param name="length">Amount of characters to write</param>
        /// <param name="nullTerminated">Null terminated the string in the buffer, this null character is not counted in the length parameter</param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void WriteStringA(string text, int index, int length, bool nullTerminated, int position)
        {
            length += index;
            while ((index < length) && (index < text.Length))
            {
                WriteCharA(text[index], position);
                position += 1;
                index++;
            }
            while (index < length)
            {
                WriteCharA('\0', position);
                position += 1;
                index++;
            }
            if (nullTerminated)
            {
                WriteCharA('\0', position);
            }
        }
        /// <summary>
        /// Writes an UNICODE string, without advancing the internal position.
        /// If the string lenght is less than the target length, the remaining bytes will be set to zero (null)
        /// </summary>
        /// <param name="text">String to write to the byte array</param>
        /// <param name="index">Starting position of the string</param>
        /// <param name="length">Amount of characters to write</param>
        /// <param name="nullTerminated">Null terminated the string in the buffer, this null character is not counted in the length parameter</param>
        /// <param name="position">Starting position of the internal buffer</param>
        /// <returns></returns>
        public void WriteStringW(string text, int index, int length, bool nullTerminated, int position)
        {
            length += index;
            while ((index < length) && (index < text.Length))
            {
                WriteCharW(text[index], position);
                position += 2;
                index++;
            }
            while (index < length)
            {
                WriteCharW('\0', position);
                position += 2;
                index++;
            }
            if (nullTerminated)
            {
                WriteCharW('\0', position);
            }
        }
    }
}
