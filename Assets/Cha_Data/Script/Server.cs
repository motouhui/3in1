using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System;
using System.Threading;
using System.IO;

public enum COMMANDS {
    JOIN = (Int32)2
}

public class ServerState
{
    public Int32 state;

    public Int32 nameLength;
    public Byte[] name;

    public Double heroHp;
    public Double heroMp;
    public Int32 heroAction;

    public Double swordHp;
    public Double swordMp;
    public Int32 swordAction;

    public Double bossHp;
    public Double bossMp;
    public Int32 bossAction;
}

public class Server
{

    TcpClient tcp = null;
    NetworkStream workStream = null;

    private enum DataMode { Text, Hex }
    private char[] HexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f' };
    private bool CharInArray(char aChar, char[] charArray)
    {
        return (Array.Exists<char>(charArray, delegate(char a) { return a == aChar; }));
    }

    /// <summary>

    /// 十六进制字符串转换字节数组

    /// </summary>

    /// <param name="s"></param>

    /// <returns></returns>

    private byte[] HexStringToByteArray(string s)
    {
        // s = s.Replace(" ", "");

        StringBuilder sb = new StringBuilder(s.Length);
        foreach (char aChar in s)
        {
            if (CharInArray(aChar, HexDigits))
                sb.Append(aChar);
        }
        s = sb.ToString();
        int bufferlength;
        if ((s.Length % 2) == 1)
            bufferlength = s.Length / 2 + 1;
        else bufferlength = s.Length / 2;
        byte[] buffer = new byte[bufferlength];
        for (int i = 0; i < bufferlength - 1; i++)
            buffer[i] = (byte)Convert.ToByte(s.Substring(2 * i, 2), 16);
        if (bufferlength > 0)
            buffer[bufferlength - 1] = (byte)Convert.ToByte(s.Substring(2 * (bufferlength - 1), (s.Length % 2 == 1 ? 1 : 2)), 16);
        return buffer;
    }

    /// <summary>

    /// 字节数组转换十六进制字符串

    /// </summary>

    /// <param name="data"></param>

    /// <returns></returns>

    private string ByteArrayToHexString(byte[] data)
    {
        StringBuilder sb = new StringBuilder(data.Length * 3);
        foreach (byte b in data)
            sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
        return sb.ToString().ToUpper();
    }

    /// <summary>

    /// 发送数据

    /// </summary>

    private void SendCommand(COMMANDS command)
    {
        new Thread(() =>
        {
            if (workStream != null)
            {
                BinaryWriter writer = new BinaryWriter(workStream);
                writer.Write((Int32)1);
                writer.Write((Int32)1);
                writer.Write((Int32)command);
                writer.Write((Int32)0);
            }
        }).Start();
        
    }

    delegate void SetTextCallback(string text);
    delegate void SetControl();
    delegate void GetData(byte[] data);

    private void SetText(string text)
    {
        Console.WriteLine(text + Environment.NewLine);
    }

    public ManualResetEvent connectDone = new ManualResetEvent(false);

    /// <summary>

    /// 异步连接的回调函数

    /// </summary>

    /// <param name="ar"></param>

    private void ConnectCallback(IAsyncResult ar)
    {
        connectDone.Set();
        TcpClient t = (TcpClient)ar.AsyncState;
        try
        {
            if (t.Connected)
            {
                SetText("连接成功");
                t.EndConnect(ar);
                SetText("连接线程完成");

                // write join
                SendCommand(COMMANDS.JOIN);
            }
            else
            {
                SetText("连接失败");
                t.EndConnect(ar);
            }

        }
        catch (SocketException se)
        {
            SetText("连接发生错误ConnCallBack.......:" + se.Message);
        }
    }

    /// <summary>
    /// 异步连接
    /// </summary>
    public void Connect()
    {
        if ((tcp == null) || (!tcp.Connected))
        {
            try
            {
                new Thread(() =>
                {
                    tcp = new TcpClient();
                    tcp.ReceiveTimeout = 10;


                    connectDone.Reset();

                    SetText("Establishing Connection to Server");

                    tcp.BeginConnect("218.244.145.129", 12345,
                        new AsyncCallback(ConnectCallback), tcp);

                    connectDone.WaitOne();

                    if ((tcp != null) && (tcp.Connected))
                    {
                        workStream = tcp.GetStream();

                        SetText("Connection established");

                        asyncread(tcp);
                    }
                }).Start();
            }
            catch (Exception se)
            {
                Console.WriteLine(se.Message + " Conn......." + Environment.NewLine);
            }
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void DisConnect()
    {
        if ((tcp != null) && (tcp.Connected))
        {
            workStream.Close();
            tcp.Close();
        }
    }

    /// <summary>

    /// 异步读TCP数据

    /// </summary>

    /// <param name="sock"></param>

    private void asyncread(TcpClient sock)
    {
        NetworkStream stream = sock.GetStream();

        if (stream.CanRead)
        {
            try
            {
                ServerState serverState = new ServerState();
                BinaryReader reader = new BinaryReader(stream);

                Int32 magic = reader.ReadInt32();
                Int32 id = reader.ReadInt32();
                Int32 command = reader.ReadInt32();
                Int32 length = reader.ReadInt32();

                serverState.state = reader.ReadInt32();
                serverState.nameLength = reader.ReadInt32();
                serverState.name = reader.ReadBytes(serverState.nameLength);

                Int32 role = 0;
                while (length > 0) {
                    role = reader.ReadInt32();
                    length -= 4;
                    if (role == 0) continue;
                    // read data of current role
                    reader.ReadBytes(16); // skip 16 bytes; maxhp & maxmp
                    length -= 16;

                    Double hp = reader.ReadDouble(); length -= 8;
                    Double mp = reader.ReadDouble(); length -= 8;
                    
                    // skip 16 bytes; hp_recovery_rate & mp_recovery_rate
                    reader.ReadBytes(16); length -= 16;

                    int action = reader.ReadInt32(); length -= 4;
                    if (role == 1)
                    {
                        serverState.heroHp = hp;
                        serverState.heroMp = mp;
                        serverState.heroAction = action;
                    }
                    else if (role == 2)
                    {
                        serverState.swordHp = hp;
                        serverState.swordMp = mp;
                        serverState.swordAction = action;
                    }
                    else if (role == 3)
                    {
                        serverState.bossHp = hp;
                        serverState.bossMp = mp;
                        serverState.bossAction = action;
                    }

                    if (role == 2)
                    {
                        reader.ReadBytes(24); // skip 24 bytes; mp_consumption & attack & damage
                        length -= 24;
                    }
                    else
                    {
                        reader.ReadBytes(32); // skip 32 bytes; mp_consumption & attack & critical_multiply & critical_rate
                        length -= 32;
                    }
                }
                Console.WriteLine("fdsafdsa");
            }
            catch (Exception e)
            {
                SetText("Network IO problem " + e.ToString());
            }
        }
    }

    
}
