using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using Aselia.Patch.Util;
using Libraries;

namespace Aselia.Patch
{
    public class Server
    {
        public static readonly long nonBlockingReceiveInterval = 1;
        public static readonly long motdRefreshInterval = 600;
        public static readonly long clientTimeout = 30;

        public Random rng = new Random((int)Stopwatch.GetTimestamp());
        private TcpListener tcpPatch;
        private TcpListener tcpData;
        private TcpListener tcpQuery;

        public List<Client> clients;
        public List<Update> updates;
        public Configuration cfg;
        public ClientCommands cmd;

        public long servertime;
        public long non_blocking_receive;
        public long motd_refresh;
        public int cfg_md5;
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
            Log.Write(Log.Level.None, Log.Type.Server, "Terminating process, result: {0}", result);
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
                byte[] data = File.ReadAllBytes(Global.ConfigFileName);
                try
                {
                    cfg = Utils.JsonDeserialize<Configuration>(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    Log.Write(Log.Level.Error, Log.Type.Server, "Error parsing ship configuration\n{0}", ex);
                    Exit(1);
                }

                if (!cfg.CheckConfiguration())
                {
                    Exit(1);
                }
                
                MakeCmd13();
                MakeUpdates();

                tcpPatch = new TcpListener(cfg._ipAddress, cfg.port);
                tcpData = new TcpListener(cfg._ipAddress, cfg.port + 1);
                tcpQuery = new TcpListener(cfg._ipAddress, cfg.port + 2);

                tcpPatch.Start();
                tcpData.Start();
                tcpQuery.Start();

                Console.WriteLine();
                cfg.LogConfiguration();
                Console.WriteLine();

                Log.Write(Log.Level.Info, Log.Type.Server, "Server started");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Log.Write(Log.Level.Error, Log.Type.Server, "Error starting server\n{0}", ex);
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
                servertime = Utils.Time();

                if ((servertime - motd_refresh) > motdRefreshInterval)
                {
                    RefreshMotd();
                }

                if (cfg.maxSpeed > 0 && lastSend != servertime)
                {
                    lastSend = servertime;
                    dataRemaining = cfg.maxSpeed;
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

                CheckClients((servertime - non_blocking_receive) > nonBlockingReceiveInterval);

                if ((servertime - non_blocking_receive) > nonBlockingReceiveInterval)
                {
                    non_blocking_receive = servertime;
                }

                Thread.Sleep(1);
            }
        }
        private void CheckClients(bool nonblock)
        {
            bool removeClients = false;
            for (int i1 = 0; i1 < clients.Count; i1++)
            {
                Client c = clients[i1];

                if (c.time != servertime)
                {
                    long n2 = 1 + servertime - c.time;
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

                c.ReadData(nonblock);
                c.CheckCmd();
                c.SendData();

                if ((servertime - c.time) > clientTimeout)
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
                if (clients.Count < cfg.maxClients)
                {
                    Client c = new Client(this);
                    c.tcpC = tcp;
                    c.time = servertime;
                    c.connectionTime = servertime;
                    c.patch = patch;
                    cmd.SndCmd02(c);

                    if (patch)
                    {
                        for (int i1 = 0; i1 < updates.Count; i1++)
                        {
                            c.updates.Add(new UpdateClient());
                        }
                    }

                    Log.Write(Log.Level.Info, Log.Type.Conn, "{0} connection from: {1}", 
                        patch ? "DATA" : "PATCH",
                        c.tcpC.Client.RemoteEndPoint);

                    clients.Add(c);
                }
                else
                {
                    Log.Write(Log.Level.Warning, Log.Type.Server, "Server connection limit reached");
                    tcp.Close();
                }
            }
            catch (SocketException se)
            {
                Log.Write(Log.Level.Error, Log.Type.Conn, "Could not accept connection. Error Code {0}", se.ErrorCode);
            }
            catch (Exception ex)
            {
                Log.Write(Log.Level.Error, Log.Type.Conn, "Could not accept connection\n{0}", ex);
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

            if (count >= cfg.maxConcurrentConnections)
            {
                Log.Write(Log.Level.Info, Log.Type.Conn, "{0} ({1}) disconnected, too many connections", clients[firstconn].username, clients[firstconn].GetIP());
                clients[firstconn].todc = true;
            }
        }

        public void RefreshMotd()
        {
            try
            {
                byte[] data = File.ReadAllBytes(Global.ConfigFileName);
                MD5 md5 = MD5.Create();
                byte[] md5_result = md5.ComputeHash(data);
                int md5_value = md5_result[0] + (md5_result[1] << 8) + (md5_result[2] << 16) + (md5_result[3] << 24);

                if (cfg_md5 != md5_value)
                {
                    Configuration newcfg = Utils.JsonDeserialize<Configuration>(data, 0, data.Length);
                    cfg.motd = newcfg.motd;
                    MakeCmd13();
                    cfg_md5 = md5_value;
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.Level.Warning, Log.Type.Server, "Error parsing ship configuration\n{0}", ex);
            }
            motd_refresh = servertime;
        }
        public void MakeCmd13()
        {
            cmd13 = new ByteArray(4096);
            if (string.IsNullOrWhiteSpace(cfg.motd))
            {
                Log.Write(Log.Level.Warning, Log.Type.Server, "Welcome message is empty");
            }

            cfg.motd = cfg.motd.Replace("\\tC", "\tC");
            cfg.motd = cfg.motd.Replace("$C", "\tC");
            cfg.motd = cfg.motd.Replace("\r\n", "\n");
            cfg.motd = cfg.motd.Replace("\\n", "\n");

            // Leaving space for a null terminator
            if (cfg.motd.Length > 2045)
            {
                Log.Write(Log.Level.Warning, Log.Type.Server, "Welcome message is too long {0}, truncating to 2045 characters", cfg.motd.Length);
                cfg.motd = cfg.motd.Substring(0, 2045);
            }
            cmd13.Write((ushort)0x0000);
            cmd13.Write((ushort)0x0013);
            cmd13.WriteStringW(cfg.motd, 0, cfg.motd.Length, true);
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
                fileList = Directory.GetFiles(cfg.updatesPath, "*", SearchOption.AllDirectories);
            }
            catch
            {
                Log.Write(Log.Level.Info, Log.Type.Server, "Updates directory not found, proceeding without updates");
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
                        Log.Write(Log.Level.Warning, Log.Type.Server, "File: {0}, file name is too long, skipping", u.fullName, u.size, u.checksum);
                        skip = true;
                    }
                    for (int i1 = 1; i1 < u.folders.Count; i1++)
                    {
                        if (u.folders[i1].Length > 64)
                        {
                            Log.Write(Log.Level.Warning, Log.Type.Server, "File: {0}, folder name \"{0}\" is too long, skipping", u.fullName, u.folders[i1]);
                            skip = true;
                        }
                    }

                    if (!skip)
                    {
                        updates.Add(u);
                        Log.Write(Log.Level.Info, Log.Type.Server, "File: {0}, Size: {1}, Checksum: {2:X8}", u.fileName, u.size, u.checksum);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(Log.Level.Error, Log.Type.Server, "Error reading file: {0}\nException: {1}", file, ex);
                }
            }
            if (updates.Count == 0)
            {
                Log.Write(Log.Level.Info, Log.Type.Server, "No updates found, proceeding without updates");
            }
        }
    }
}
