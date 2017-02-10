using System;
using System.Collections.Generic;

namespace Aselia.Patch
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
        private int size;
        public int Size
        {
            get { return size; }
            set { size = value; }
        }
        private uint checksum;
        public uint Checksum
        {
            get { return checksum; }
            set { checksum = value; }
        }
        private bool send;
        public bool Send
        {
            get { return send; }
            set { send = value; }
        }
        
        public UpdateClient()
        {
        }

        public override string ToString()
        {
            return String.Format("Size: {0}, Checksum: {1:X8}, Send: {2}", size, checksum, send);
        }
    }
}
