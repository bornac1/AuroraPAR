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
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AuroraPAR
{
    internal class Aurora
    {
        private TcpClient client = new();
        private NetworkStream? stream;
        private StreamReader? reader;
        private StreamWriter? writer;
        private SemaphoreSlim semaphore = new(1, 1);
        public bool Connected { get { return client.Connected; } }
        public async Task Connect()
        {
            try
            {
                await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 1130);
                stream = client.GetStream();
                reader = new(stream);
                writer = new(stream);
                await GetTrafficList();
            } catch (Exception ex)
            {
                //TODO: Exception handling
                Close();
                await Task.Delay(100);
                client = new();
                await Connect();
            }
        }
        public async Task<string[]> GetTrafficList()
        {
            if (writer != null && reader != null && client.Connected)
            {
                await semaphore.WaitAsync();
                await writer.WriteLineAsync("#TR");
                await writer.FlushAsync();
                string? message = await reader.ReadLineAsync();
                semaphore.Release();
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
                await semaphore.WaitAsync();
                await writer.WriteLineAsync($"#TRPOS;{callsign}");
                await writer.FlushAsync();
                string? message = await reader.ReadLineAsync();
                semaphore.Release();
                if (message != null && message.Contains("#TRPOS"))
                {
                    string[] data = message.Split(';');
                    if (data.Length >= 7)
                    {
                        Aircraft aircraft = new Aircraft()
                        {
                            Callsign = callsign,
                            Track = double.Parse(data[3]),
                            Altitude = double.Parse(data[4]),
                            Speed = double.Parse(data[5]),
                            Latitude = double.Parse(data[6].Replace('.', ',')),
                            Longitude = double.Parse(data[7].Replace('.', ',')),
                        };
                        return aircraft;
                    }
                }
            }
            return null;
        }
        public async Task<int> GetQNH(Runway runway)
        {
            if(reader != null && writer != null)
            {
                await semaphore.WaitAsync();
                await writer.WriteLineAsync($"#METAR;{runway.ICAO}");
                await writer.FlushAsync();
                string? message = await reader.ReadLineAsync();
                semaphore.Release();
                if(message != null)
                {
                    Regex rg = new Regex("Q\\d{4}");
                    Match match = rg.Match(message);
                    if (match.Length > 1)
                    {
                        return Convert.ToInt32(match.Value[1..]);
                    }
                }
            }
            return 0;
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
