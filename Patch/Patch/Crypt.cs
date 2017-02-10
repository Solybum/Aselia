namespace Aselia.Patch
{
    public class Crypt
    {
        private uint[] keys;
        private uint position;

        public Crypt()
        {
            keys = new uint[1042];
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
        public void CryptData(byte[] data, int index, int length)
        {
            int x;
            uint key;
            for (x = index; x < (index + length); x += 4)
            {
                key = GetNextKey();
                data[x + 0] ^= (byte)(key >> 0);
                data[x + 1] ^= (byte)(key >> 8);
                data[x + 2] ^= (byte)(key >> 16);
                data[x + 3] ^= (byte)(key >> 24);
            }
        }
    }
}
