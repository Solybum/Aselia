using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Libraries;

namespace Aselia.Patch
{
    public class Client
    {
        public const int BufferSize = 0xFFFF;

        private Server s;
        public TcpClient tcpC;
        public Crypt serverCipher;
        public Crypt clientCipher;
        
        public long time;
        public long connectionTime;

        public int sndbps;
        public int rcvbps;
        public int pcktps;

        public int expect;
        public ByteArray snd;
        public ByteArray rcv;
        public ByteArray dec;

        public List<UpdateClient> updates;
        public int filesToCheck;
        public int filesToSend;
        public int folder;
        public int fileOffset;
        public int chunk;

        public bool todc;
        public bool crypton;
        public bool patch;
        public bool checkingFiles;
        public bool sendingFiles;
        public bool packet10;

        public string username;
        public string password;

        public Client(Server s)
        {
            this.s = s;

            serverCipher = new Crypt();
            clientCipher = new Crypt();

            sndbps = 0;
            rcvbps = 0;
            pcktps = 0;
            expect = 0;
            snd = new ByteArray(BufferSize);
            rcv = new ByteArray(BufferSize);
            dec = new ByteArray(BufferSize);

            updates = new List<UpdateClient>();
        }

        public string GetIP()
        {
            return ((IPEndPoint)tcpC.Client.RemoteEndPoint).Address.ToString();
        }

        public void Encrypt(byte[] data, int offset, int length)
        {
            if ((length + 7) > (snd.Length - snd.Position))
            {
                Log.Write(Log.Level.Warning, Log.Type.Server, "Sending too much data to client {0}.", GetIP());
                todc = true;
            }
            else
            {
                snd.Write(data, offset, length, snd.Position);
                while ((length % 4) != 0)
                {
                    snd[snd.Position + length++] = 0;
                }
                if (crypton)
                {
                    serverCipher.CryptData(snd.Buffer, snd.Position, length);
                }
                snd.Position += length;
            }
        }
        public int SendData()
        {
            int ret = -1;
            int sndcpy = 0;

            sndcpy = snd.Position;
            if (sndcpy > BufferSize)
            {
                sndcpy = BufferSize;
            }

            if (sndcpy > 0)
            {
                try
                {
                    ret = tcpC.Client.Send(snd.Buffer, sndcpy, SocketFlags.None);
                    snd.Position -= ret;
                    sndbps += ret;

                    time = s.servertime;
                    if (snd.Position > 0)
                    {
                        Array.Copy(snd.Buffer, ret, snd.Buffer, 0, snd.Position);
                    }
                }
                catch (SocketException se)
                {
                    if (se.NativeErrorCode == 10035)
                    {
                        // WSAEWOULDBLOCK
                        // Ignore
                    }
                    else
                    {
                        Log.Write(Log.Level.Error, Log.Type.Server, "Could not send data to client {0}. Error code {1}.", GetIP(), se.ErrorCode);
                        todc = true;
                    }
                }
            }
            return ret;
        }
        public int ReadData(bool nonblocking)
        {
            int ret = -1;
            if (tcpC.Available > 0 || nonblocking)
            {
                tcpC.Client.Blocking = !nonblocking;
                try
                {
                    ret = tcpC.Client.Receive(rcv.Buffer, rcv.Position, rcv.Length - rcv.Position, SocketFlags.None);
                    rcv.Position += ret;
                    rcvbps += ret;

                    if (nonblocking && ret == 0)
                    {
                        todc = true;
                    }

                    time = s.servertime;
                }
                catch (SocketException se)
                {
                    if (se.NativeErrorCode != 10035)
                    {
                        Log.Write(Log.Level.Error, Log.Type.Server, "Could not read data from client {0}. Error code {1}.", GetIP(), se.ErrorCode);
                        todc = true;
                    }
                }

                tcpC.Client.Blocking = true;
            }
            return ret;
        }
        public void CheckCmd()
        {
            if (rcv.Position >= 4)
            {
                int rcvcpy;
                rcvcpy = rcv.Position >> 2;
                rcvcpy <<= 2;

                clientCipher.CryptData(rcv.Buffer, 0, rcvcpy);

                int rcvpos = 0;
                while (rcvcpy > 0)
                {
                    dec.Write(rcv[rcvpos++]);
                    if (expect == 0 && dec.Position >= 4)
                    {
                        expect = dec.ReadUInt16(0);
                        if ((expect % 4) != 0)
                        {
                            expect += 4 - (expect % 4);
                        }
                        if (expect > BufferSize)
                        {
                            Log.Write(Log.Level.Warning, Log.Type.Server, "Received too much data from client {0}.", GetIP());
                            todc = true;
                            return;
                        }
                    }

                    if (expect == dec.Position)
                    {
                        if (rcvbps > 20000 || sndbps > 480000)
                        {
                            Log.Write(Log.Level.Warning, Log.Type.Server, "Disconnected {0} because of possible DDOS. Packets/s: {1}, Send: {2} Kbps, Receive: {3} Kbps",
                                GetIP(),
                                pcktps,
                                sndbps / 1024L,
                                rcvbps / 1024L);
                            todc = true;
                            return;
                        }
                        else
                        {
                            dec.Position = 0;
                            s.cmd.ProcessCommand(this);
                        }

                        pcktps++;
                        dec.Position = 0;
                        expect = 0;
                    }
                    rcvcpy--;
                }

                rcv.Position -= rcvpos;
                if (rcv.Position > 0)
                {
                    Array.Copy(rcv.Buffer, rcvpos, rcv.Buffer, 0, rcv.Position);
                }
            }
        }
    }
}
