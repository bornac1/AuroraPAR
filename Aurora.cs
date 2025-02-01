using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data;
using System.IO;
using System.Windows;

namespace AuroraPAR
{
    internal class Aurora
    {
        private readonly TcpClient client = new();
        private NetworkStream? stream;
        private StreamReader? reader;
        private StreamWriter? writer;
        public async Task Connect()
        {
            await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 1130);
            stream = client.GetStream();
            reader = new(stream);
            writer = new(stream);
            await GetTrafficList();
        }
        public async Task<string[]> GetTrafficList()
        {
            if (writer != null && reader != null && client.Connected)
            {
                await writer.WriteLineAsync("#TR");
                await writer.FlushAsync();
                string? message = await reader.ReadLineAsync();
                if (message != null)
                {
                    string[] callsigns = message.Split(';')[1..];
                    return callsigns;
                }
            }
            return [];
        }
        public async Task<Aircraft?> GetTrafficPosition (string callsign)
        {
            if (reader != null && writer != null && client.Connected && !string.IsNullOrEmpty(callsign))
            {
                await writer.WriteLineAsync($"#TRPOS;{callsign}");
                await writer.FlushAsync();
                string? message = await reader.ReadLineAsync();
                if (message != null && message.Contains("#TRPOS"))
                {
                    string[] data = message.Split(';');
                    if (data.Length >= 7)
                    {
                        Aircraft aircraft = new Aircraft()
                        {
                            Callsign = callsign,
                            Altitude = double.Parse(data[4]),
                            Latitude = double.Parse(data[6].Replace('.', ',')),
                            Longitude = double.Parse(data[7].Replace('.', ',')),
                        };
                        return aircraft;
                    }
                }
            }
            return null;
        }
        public void Close()
        {
            writer?.Close();
            writer?.Dispose();
            reader?.Close();
            reader?.Dispose();
            stream?.Close();
            stream?.Dispose();
            client.Close();
            client.Dispose();
        }
    }
}
