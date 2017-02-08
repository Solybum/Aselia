using System;

namespace Patch
{
    public class Crypt
    {
        private uint[] keys;
        private uint position;

        public Crypt()
        {
            keys = new uint[1042];
        }

        public uint GetNextKey()
        {
            uint ret;
            if (position == 56)
            {
                MixKeys();
                position = 1;
            }
            ret = keys[position];
            position++;
            return ret;
        }
        public void MixKeys()
        {
            uint esi, edi, eax, ebp, edx;
            edi = 1;
            edx = 0x18;
            eax = edi;
            while (edx > 0)
            {
                esi = keys[eax + 0x1F];
                ebp = keys[eax];
                ebp = ebp - esi;
                keys[eax] = ebp;
                eax++;
                edx--;
            }
            edi = 0x19;
            edx = 0x1F;
            eax = edi;
            while (edx > 0)
            {
                esi = keys[eax - 0x18];
                ebp = keys[eax];
                ebp = ebp - esi;
                keys[eax] = ebp;
                eax++;
                edx--;
            }
        }
        public void CreateKeys(uint value)
        {
            uint esi, ebx, edi, eax, edx, var1;
            esi = 1;
            ebx = value;
            edi = 0x15;
            keys[56] = ebx;
            keys[55] = ebx;
            while (edi <= 0x46E)
            {
                eax = edi;
                var1 = eax / 55;
                edx = eax - (var1 * 55);
                ebx = ebx - esi;
                edi = edi + 0x15;
                keys[edx] = esi;
                esi = ebx;
                ebx = keys[edx];
            }
            MixKeys();
            MixKeys();
            MixKeys();
            MixKeys();
            position = 56;
        }
        public void CryptData(byte[] data, int start, int length)
        {
            int x;
            for (x = start; x < (start + length); x += 4)
            {
                Array.Copy(BitConverter.GetBytes(BitConverter.ToUInt32(data, x) ^ GetNextKey()), 0, data, x, 4);
            }
        }
    }
}
