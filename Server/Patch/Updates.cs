using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patch
{
    public class Update
    {
        public int size;
        public uint checksum;
        public string fullName; // relative full name
        public string fileName; // file name
        public string folder; // relative folder name
        public List<string> folders; // folder tree, null separated folder names

        public Update()
        {
            folders = new List<string>();
        }

        public override string ToString()
        {
            return String.Format("File: {0}, Size: {1}, Checksum: {2:X8}", fullName, size, checksum);
        }
    }

    public class UpdateClient
    {
        public int size;
        public uint checksum;
        public bool send;

        public UpdateClient()
        {
        }

        public override string ToString()
        {
            return String.Format("Size: {0}, Checksum: {1:X8}, Send: {2}", size, checksum, send);
        }
    }
}
