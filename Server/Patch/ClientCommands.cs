using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libraries;

// TODO file names 48 bytes
// TODO directory names 60 bytes

namespace Patch
{
    public class ClientCommands
    {
        private Server s;
        private Random rng = new Random((int)Utils.Time());
        byte[] fileData = new byte[Client.BufferSize];

        private Dictionary<int, Action<Client>> CmdList = new Dictionary<int, Action<Client>>();

        public ClientCommands(Server s)
        {
            this.s = s;

            CmdList[0x02] = RcvCmd02;
            CmdList[0x04] = RcvCmd04;
            CmdList[0x0F] = RcvCmd0F;
            CmdList[0x10] = RcvCmd10;
        }
        
        public void ProcessCommand(Client c)
        {
            Action<Client> cmd = null;
            if (CmdList.TryGetValue(c.dec[0x02], out cmd))
            {
                cmd(c);
            }
            else
            {
                Log.Write(Log.Level.Info, Log.Type.Server, "{0} sent an unknown packet {1:X2}", c.GetIP(), c.dec[2]);
                //LogClientPacket(c, F.LogF.Debug);
                c.todc = true;
            }
            
        }
        
        /// <summary>
        /// Welcome reply
        /// </summary>
        /// <param name="c"></param>
        private void RcvCmd02(Client c)
        {
            ByteArray ba = new ByteArray(4);
            ba.Write((ushort)0x04);
            ba.Write((ushort)0x04);
            c.Encrypt(ba.Buffer, 0, ba.Length);
        }
        /// <summary>
        /// Beginning of the update process
        /// </summary>
        /// <param name="c"></param>
        private void RcvCmd04(Client c)
        {
            c.username = c.dec.ReadStringA(16, 0x10);
            c.password = c.dec.ReadStringA(16, 0x20);
            
            if (c.patch == false)
            {
                c.Encrypt(s.cmd13.Buffer, 0, s.cmd13.Position);
                SndCmd14(c);
            }
            else
            {
                if (s.cfg.noUpdates == true || s.updates.Count == 0)
                {
                    SndCmd12(c);
                }
                else
                {
                    // Send the file checks in a loop instead of a single packet
                    // this way we can do more patches than it was possible before
                    // Not that it will be needed anyway
                    c.checkingFiles = true;
                    SndCmd0B(c);
                }
            }
        }
        /// <summary>
        /// File info
        /// </summary>
        /// <param name="c"></param>
        private void RcvCmd0F(Client c)
        {
            int index, size;
            uint checksum;
            c.dec.Position = 4;

            index = c.dec.ReadInt32();
            checksum = c.dec.ReadUInt32();
            size = c.dec.ReadInt32();

            if (index < c.updates.Count)
            {
                c.updates[index].checksum = checksum;
                c.updates[index].size = size;

                if (!s.cfg.noUpdates && 
                    (s.updates[index].size != c.updates[index].size ||
                    s.updates[index].checksum != c.updates[index].checksum))
                {
                    c.updates[index].send = true;
                    c.filesToSend++;
                }
            }
        }
        /// <summary>
        /// File info sent
        /// </summary>
        /// <param name="c"></param>
        private void RcvCmd10(Client c)
        {
            if (!c.packet10)
            {
                c.packet10 = true;
                if (c.filesToSend > 0)
                {
                    int size, files;
                    
                    size = 0;
                    files = 0;
                    for (int i1 = 0; i1 < s.updates.Count; i1++)
                    {
                        if (c.updates[i1].send == true)
                        {
                            size += s.updates[i1].size;
                            files++;
                        }
                    }

                    ByteArray ba = new ByteArray(12);

                    ba.Write((ushort)0x0C);
                    ba.Write((ushort)0x11);
                    ba.Write(size);
                    ba.Write(files);
                    c.Encrypt(ba.Buffer, 0, ba.Length);

                    Log.Write(Log.Level.Info, Log.Type.Server, "{0} downloading {1} files, {2} bytes", c.username, files, size);
                    c.sendingFiles = true;
                    c.filesToSend = 0;
                }
                else
                {
                    SndCmd12(c);
                }
            }
        }
        
        /// <summary>
        /// Welcome message and encryption keys
        /// </summary>
        /// <param name="c"></param>
        public void SndCmd02(Client c)
        {
            ByteArray ba = new ByteArray(76);
            ba.Write((ushort)0x004C);
            ba.Write((ushort)0x0002);
            ba.WriteStringA(Global.PatchWelcomeMessage, 0, Global.PatchWelcomeMessage.Length, false);

            ba.Write(rng.Next(), 0x44);
            ba.Write(rng.Next(), 0x48);

            c.serverCipher.CreateKeys(ba.ReadUInt32(0x44));
            c.clientCipher.CreateKeys(ba.ReadUInt32(0x48));


            c.Encrypt(ba.Buffer, 0, ba.Length);
            c.crypton = true;
        }
        /// <summary>
        /// File header
        /// </summary>
        /// <param name="c"></param>
        /// <param name="u"></param>
        /// <param name="index"></param>
        public void SndCmd06(Client c, Update u, int index)
        {
            ByteArray ba = new ByteArray(60);
            ba.Write((ushort)0x3C);
            ba.Write((ushort)0x06);
            ba.Write(index);
            ba.Write(u.size);
            ba.WriteStringA(u.fileName, 0, u.fileName.Length, false);
            c.Encrypt(ba.Buffer, 0, ba.Length);
        }
        /// <summary>
        /// File chunk
        /// </summary>
        /// <param name="c"></param>
        /// <param name="data"></param>
        /// <param name="size"></param>
        public void SndCmd07(Client c, byte[] data, int size)
        {
            uint checksum = Utils.CRC32(data, 0x10, size);
            
            // Wrap the data array, as it already has space for the command stuff
            ByteArray ba = new ByteArray(data);
            ba.Write((ushort)(size + 0x10));
            ba.Write((ushort)0x07);
            ba.Write(c.chunk);
            ba.Write(checksum);
            ba.Write(size);
            c.Encrypt(ba.Buffer, 0, size + 0x10);

            c.chunk++;
        }
        /// <summary>
        /// File done, this possibly has some flags about the file just updated
        /// </summary>
        /// <param name="c"></param>
        public void SndCmd08(Client c)
        {
            c.fileOffset = 0;
            c.filesToSend++;

            ByteArray ba = new ByteArray(8);
            ba.Write((ushort)0x08);
            ba.Write((ushort)0x08);
            c.Encrypt(ba.Buffer, 0, ba.Length);
        }
        /// <summary>
        /// Move to a directory, creates if it doesn't exist
        /// </summary>
        /// <param name="c"></param>
        /// <param name="folder"></param>
        public void SndCmd09(Client c, string folder)
        {
            ByteArray ba = new ByteArray(68);
            ba.Write((ushort)0x44);
            ba.Write((ushort)0x09);
            ba.WriteStringA(folder, 0, 64, false);
            c.Encrypt(ba.Buffer, 0, ba.Length);
        }
        /// <summary>
        /// Go up one level in the directory tree
        /// </summary>
        /// <param name="c"></param>
        public void SndCmd0A(Client c)
        {
            ByteArray ba = new ByteArray(4);
            ba.Write((ushort)0x04);
            ba.Write((ushort)0x0A);
            c.Encrypt(ba.Buffer, 0, ba.Length);
        }
        /// <summary>
        /// Begin directory operations?
        /// </summary>
        /// <param name="c"></param>
        public void SndCmd0B(Client c)
        {
            ByteArray ba = new ByteArray(4);
            ba.Write((ushort)0x04);
            ba.Write((ushort)0x0B);
            c.Encrypt(ba.Buffer, 0, ba.Length);
        }
        /// <summary>
        /// For for file info
        /// </summary>
        /// <param name="c"></param>
        /// <param name="patchIndex"></param>
        /// <param name="patchName"></param>
        public void SndCmd0C(Client c, int patchIndex, string patchName)
        {
            ByteArray ba = new ByteArray(40);
            ba.Write((ushort)0x28);
            ba.Write((ushort)0x0C);
            ba.Write(patchIndex);
            ba.WriteStringA(patchName, 0, 32, false);
            c.Encrypt(ba.Buffer, 0, ba.Length);
        }
        /// <summary>
        /// Done asking file info
        /// </summary>
        /// <param name="c"></param>
        public void SndCmd0D(Client c)
        {
            c.checkingFiles = false;
            ByteArray ba = new ByteArray(4);
            ba.Write((ushort)0x04);
            ba.Write((ushort)0x0D);
            c.Encrypt(ba.Buffer, 0, ba.Length);
        }
        /// <summary>
        /// End of update process?
        /// </summary>
        /// <param name="c"></param>
        public void SndCmd12(Client c)
        {
            ByteArray ba = new ByteArray(4);
            ba.Write((ushort)0x04);
            ba.Write((ushort)0x12);
            c.Encrypt(ba.Buffer, 0, ba.Length);
            c.todc = true;
        }
        /// <summary>
        /// Redirect
        /// </summary>
        /// <param name="c"></param>
        public void SndCmd14(Client c)
        {
            ByteArray ba = new ByteArray(12);
            ba.Write((ushort)0x000C);
            ba.Write((ushort)0x0014);
            ba.Write(s.cfg._ipRedirect.GetAddressBytes(), 0, 4);
            ba.Write((byte)((s.cfg.port + 1) >> 8));
            ba.Write((byte)((s.cfg.port + 1)));
            ba.Write((ushort)0x0000);
            c.Encrypt(ba.Buffer, 0, ba.Length);
            c.todc = true;
        }

        public void SendFileChecks(Client c)
        {
            Update u, pu;
            pu = null;

            while (c.filesToCheck < s.updates.Count)
            {
                u = s.updates[c.filesToCheck];
                if (c.filesToCheck > 0)
                {
                    pu = s.updates[c.filesToCheck - 1];
                }
                else
                {
                    pu = new Update();
                }

                if (pu.folder != u.folder)
                {
                    while (c.folder > 0)
                    {
                        SndCmd0A(c);
                        c.folder--;
                    }
                    for (int fc1 = 1; fc1 < u.folders.Count; fc1++)
                    {
                        SndCmd09(c, u.folders[fc1]);
                        c.folder++;
                    }
                }
                pu = u;

                SndCmd0C(c, c.filesToCheck, u.fileName);
                c.filesToCheck++;
            }
            if (c.filesToCheck == c.updates.Count)
            {
                SndCmd0D(c);
                SndCmd0B(c);
                c.folder = 0;
            }
        }
        public void SendFileData(Client c)
        {
            if (s.cfg.maxSpeed == 0 || s.dataRemaining != 0)
            {
                int size;
                Update u, pu;
                pu = null;
                while (c.filesToSend < s.updates.Count)
                {
                    u = s.updates[c.filesToSend];
                    if (c.updates[c.filesToSend].send == false)
                    {
                        c.filesToSend++;
                        continue;
                    }

                    if (c.fileOffset == 0)
                    {
                        if (c.filesToSend > 0)
                        {
                            pu = s.updates[c.filesToSend - 1];
                        }
                        else
                        {
                            pu = new Update();
                        }

                        if (pu.folder != u.folder)
                        {
                            while (c.folder > 0)
                            {
                                SndCmd0A(c);
                                c.folder--;
                            }
                            for (int fc1 = 1; fc1 < u.folders.Count; fc1++)
                            {
                                SndCmd09(c, u.folders[fc1]);
                                c.folder++;
                            }
                        }
                        pu = u;
                        SndCmd06(c, u, c.filesToSend);
                    }

                    size = u.size - c.fileOffset;
                    if (size > 0x6000)
                    {
                        size = 0x6000;
                        if (c.snd.Position + size > c.snd.Length)
                        {
                            break;
                        }
                    }
                    if (size != 0)
                    {
                        try
                        {
                            using (FileStream fs = new FileStream(u.fullName, FileMode.Open))
                            {
                                fs.Seek(c.fileOffset, SeekOrigin.Begin);
                                fs.Read(fileData, 0x10, size);

                                c.fileOffset += size;
                                SndCmd07(c, fileData, size);
                            }
                        }
                        catch
                        {
                            Log.Write(Log.Level.Warning, Log.Type.Server, "Error reading file: {0}, Skipping", u.fileName);

                            SndCmd08(c);
                            c.fileOffset = 0;
                            c.filesToSend++;
                        }
                    }
                    if (c.fileOffset == u.size)
                    {
                        SndCmd08(c);
                    }
                    break;
                }

                if (c.filesToSend == c.updates.Count)
                {
                    c.sendingFiles = false;
                    SndCmd12(c);
                }
            }
        }
    }
}
