using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Aselia.Patch.Properties;
using Aselia.Patch.Utils;
using Aselia.Patch.Utils.CRC32;
using Libraries;

namespace Aselia.Patch
{
    public class Server
    {
        public static readonly long nonBlockingReceiveInterval = 1;
        public static readonly long clientTimeout = 30;

        public Random rng = new Random((int)Stopwatch.GetTimestamp());
        private TcpListener tcpPatch;
        private TcpListener tcpData;
        private TcpListener tcpQuery;

        public List<Client> clients;
        public List<Update> updates;
        public Configuration cfg;
        public ClientCommands cmd;

        public long time;
        public long nonBlockingReceive;
        public long lastSend;
        public int dataRemaining;
        public ByteArray cmd13;

        public Server()
        {
            clients = new List<Client>();
            updates = new List<Update>();

            cfg = new Configuration();
            cmd = new ClientCommands(this);
        }
        public void Exit(int result)
        {
            Log.Info(Log.Type.Server, "Terminating process, result: {0}", result);
#if DEBUG
            Console.WriteLine("Press [ENTER] key to exit.");
            Console.ReadLine();
#endif
            Environment.Exit(result);
        }
        public void Start()
        {
            try
            {
                if (!cfg.CheckConfiguration())
                {
                    Exit(1);
                }
                
                MakeCmd13();
                MakeUpdates();

                tcpPatch = new TcpListener(cfg._ipAddress, Settings.Default.Port);
                tcpData = new TcpListener(cfg._ipAddress, Settings.Default.Port + 1);
                tcpQuery = new TcpListener(cfg._ipAddress, Settings.Default.Port + 2);

                tcpPatch.Start();
                tcpData.Start();
                tcpQuery.Start();

                Console.WriteLine();
                cfg.LogConfiguration();
                Console.WriteLine();

                Log.Info(Log.Type.Server, "Server started");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Log.Error(Log.Type.Server, "Error starting server\n{0}", ex);
                Exit(1);
            }

            Loop();
        }
        public void Stop()
        {

        }
        private void Loop()
        {
            while(true)
            {
                time = Util.Time();
                ProcessLoop();
                Thread.Sleep(1);
            }
        }
        private void ProcessLoop()
        {
            if (Settings.Default.MaxSpeed > 0 && lastSend != time)
            {
                lastSend = time;
                dataRemaining = Settings.Default.MaxSpeed;
            }

            if (tcpPatch.Pending() == true)
            {
                AcceptConnection(tcpPatch, false);
            }
            if (tcpData.Pending() == true)
            {
                AcceptConnection(tcpData, true);
            }
            if (tcpQuery.Pending() == true)
            {
                AcceptConnection(tcpQuery, true, true);
            }

            ProcessClients((time - nonBlockingReceive) > nonBlockingReceiveInterval);

            if ((time - nonBlockingReceive) > nonBlockingReceiveInterval)
            {
                nonBlockingReceive = time;
            }
        }
        private void ProcessClients(bool nonBlocking)
        {
            bool removeClients = false;
            for (int i1 = 0; i1 < clients.Count; i1++)
            {
                Client c = clients[i1];

                if (c.time != time)
                {
                    long n2 = 1 + time - c.time;
                    c.pcktps /= (int)n2;
                    c.sndbps /= (int)n2;
                    c.rcvbps /= (int)n2;
                }

                // Send 0Cs
                if (c.checkingFiles == true)
                {
                    cmd.SendFileChecks(c);
                }
                // Send 06s, 07s and 08s
                if (c.sendingFiles == true)
                {
                    cmd.SendFileData(c);
                }

                c.ReadData(nonBlocking);
                c.CheckCmd();
                c.SendData();

                if ((time - c.time) > clientTimeout)
                {
                    c.todc = true;
                }
                
                if (c.todc)
                {
                    removeClients = true;
                }
            }

            if (removeClients && clients.RemoveAll(c =>
            {
                if (c.todc)
                {
                    c.tcpC.Close();
                    return true;
                }
                return false;
            }) > 0) { }
        }
        
        private void AcceptConnection(TcpListener tcpL, bool patch, bool discard = false)
        {
            try
            {
                TcpClient tcp = tcpL.AcceptTcpClient();
                // Discard this connection, used if the connection is to check if the server is alive
                if (discard)
                {
                    tcp.Close();
                    return;
                }

                CheckClientConnections(tcp);
                if (clients.Count < Settings.Default.MaxClients)
                {
                    Client c = new Client(this);
                    c.tcpC = tcp;
                    c.time = time;
                    c.connectionTime = time;
                    c.patch = patch;
                    cmd.SndCmd02(c);

                    if (patch)
                    {
                        for (int i1 = 0; i1 < updates.Count; i1++)
                        {
                            c.updates.Add(new UpdateClient());
                        }
                    }

                    Log.Info(Log.Type.Conn, "{0} connection from: {1}", 
                        patch ? "DATA" : "PATCH",
                        c.tcpC.Client.RemoteEndPoint);

                    clients.Add(c);
                }
                else
                {
                    Log.Warning(Log.Type.Server, "Server connection limit reached");
                    tcp.Close();
                }
            }
            catch (SocketException se)
            {
                Log.Error(Log.Type.Server, "Could not accept connection. Error Code {0}", se.ErrorCode);
            }
            catch (Exception ex)
            {
                Log.Error(Log.Type.Server, "Could not accept connection\n{0}", ex);
            }
        }
        private void CheckClientConnections(TcpClient tcpC)
        {
            int count, firstconn;
            long time;
            IPEndPoint ip1, ip2;

            count = 0;
            firstconn = 0;
            time = int.MaxValue;
            for (int n1 = 0; n1 < clients.Count; n1++)
            {
                if (!clients[n1].todc)
                {
                    ip1 = (IPEndPoint)tcpC.Client.RemoteEndPoint;
                    ip2 = (IPEndPoint)clients[n1].tcpC.Client.RemoteEndPoint;
                    if (ip1.Address.Equals(ip2.Address))
                    {
                        count++;
                        if (clients[n1].connectionTime < time)
                        {
                            time = clients[n1].connectionTime;
                            firstconn = n1;
                        }
                    }
                }
            }

            if (count >= Settings.Default.MaxConcurrentConnections)
            {
                Log.Info(Log.Type.Server, "{0} ({1}) disconnected, too many connections", clients[firstconn].username, clients[firstconn].GetIP());
                clients[firstconn].todc = true;
            }
        }
        
        public void MakeCmd13()
        {
            string motd = Settings.Default.MOTD;
            cmd13 = new ByteArray(4096);
            if (string.IsNullOrWhiteSpace(motd))
            {
                Log.Warning(Log.Type.Server, "Welcome message is empty");
            }

            motd = motd.Replace("\\tC", "\tC");
            motd = motd.Replace("$C", "\tC");
            motd = motd.Replace("\r\n", "\n");
            motd = motd.Replace("\\n", "\n");

            // Leaving space for a null terminator
            if (motd.Length > 2045)
            {
                Log.Warning(Log.Type.Server, "Welcome message is too long {0}, truncating to 2045 characters", motd.Length);
                motd = motd.Substring(0, 2045);
            }
            cmd13.Write((ushort)0x0000);
            cmd13.Write((ushort)0x0013);
            cmd13.WriteStringW(motd, 0, motd.Length, true);
            cmd13.Write((ushort)0x0000);
            while((cmd13.Position % 4) != 0)
            {
                cmd13.Write((byte)0);
            }
            cmd13.Write((ushort)cmd13.Position, 0);
        }
        public void MakeUpdates()
        {
            string file;
            string[] fileList;
            byte[] data;

            file = string.Empty;
            fileList = null;
            try
            {
                fileList = Directory.GetFiles(Settings.Default.UpdatesPath, "*", SearchOption.AllDirectories);
            }
            catch
            {
                Log.Info(Log.Type.Server, "Updates directory not found, proceeding without updates");
                return;
            }

            for (int n1 = 0; n1 < fileList.Length; n1++)
            {
                try
                {
                    bool skip = false;
                    Update u = new Update();
                    file = fileList[n1];
                    data = File.ReadAllBytes(file);
                    u.folder = Path.GetDirectoryName(file);
                    u.fileName = Path.GetFileName(file);
                    u.fullName = file;
                    u.folders = u.folder.Split(Path.DirectorySeparatorChar).ToList();
                    u.size = data.Length;
                    u.checksum = CRC32.Hash(data, 0, data.Length);

                    if (u.fileName.Length > 48)
                    {
                        Log.Warning(Log.Type.Server, "File: {0}, file name is too long, skipping", u.fullName, u.size, u.checksum);
                        skip = true;
                    }
                    for (int i1 = 1; i1 < u.folders.Count; i1++)
                    {
                        if (u.folders[i1].Length > 64)
                        {
                            Log.Warning(Log.Type.Server, "File: {0}, folder name \"{0}\" is too long, skipping", u.fullName, u.folders[i1]);
                            skip = true;
                        }
                    }

                    if (!skip)
                    {
                        updates.Add(u);
                        Log.Info(Log.Type.Server, "File: {0}, Size: {1}, Checksum: {2:X8}", u.fileName, u.size, u.checksum);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(Log.Type.Server, "Error reading file: {0}\nException: {1}", file, ex);
                }
            }
            if (updates.Count == 0)
            {
                Log.Info(Log.Type.Server, "No updates found, proceeding without updates");
            }
        }
    }
}
