using System.Net.WebSockets;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Text;
using System.IO;

namespace WiniGames.Server.Core
{
    public class WebSocketHandler
    {
        public static Action<string> OnReceiveMessage { get; set; }
        
        public static Action<WebSocketState> OnConnected { get; set; }
        public static Action<WebSocketState> OnSocketClosed { get; set; }
        public static Action<string> OnConnectionFailed { get; set; }

        // static CancellationTokenSource _ct;
        static ClientWebSocket ws;
        static string socketCloseReason = "";

        public static WebSocketState State => ws.State;


        public static async Task ConnectAsync(Uri uri)
        {
            if (ws != null && (ws.State == WebSocketState.Open || ws.State == WebSocketState.Connecting))
            {
                Debug.Log($"ConnectAsync - url: {uri}, SocketHandler - Socket is already open!");
                return;
            }

            // _ct = new CancellationTokenSource();
            ws = new ClientWebSocket();
            try
            {
                await ws.ConnectAsync(uri, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error connecting, <color=red>{ex.Message}</color>");
                OnConnectionFailed?.Invoke($"{ex.Message}");
                return;
            }

            OnConnected?.Invoke(ws.State);
            
            Debug.Log("Connected to server!");
            
            try
            {
                await ReceiveData();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Receiving data interrupted, Socket closed. State: {ws.State}. Exception is <color=red>{ex.Message}</color>");
                OnSocketClosed?.Invoke(ws.State);
                ws.Dispose();
                ws = null;
            }
        }

        public static async Task DisconnectAsync()
        {
            try
            {
                if (ws == null)
                    return;

                if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.CloseSent)
                {
                    Debug.Log("DisconnectAsync - Socket already closed or closing.");
                    return;
                }

                Debug.Log("Closing socket...");
                socketCloseReason = "Quiting";
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, socketCloseReason, CancellationToken.None);
                // await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, socketCloseReason, CancellationToken.None);
                OnSocketClosed?.Invoke(ws.State);
                Debug.LogWarning("Socket closed.");
                ws.Dispose();
                ws = null;
            }
            catch (Exception ex)
            {
                ws?.Dispose();
                ws = null;
                Debug.LogError($"Error closing socket. <color=red>{ex.Message}</color>");
            }
        }


        static readonly object sendLockObject = new object();
        
        public static async Task SendData(string message)
        {
            Task t = null;
            lock (sendLockObject)
            {
                if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                {
                    var msgBytes = Encoding.UTF8.GetBytes(message);
                    try
                    {
                        t = ws.SendAsync(new ArraySegment<byte>(msgBytes, 0, msgBytes.Length), WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error sending data. <color=red>{ex.Message}</color>");
                    }
                }
            }

            if (t != null) await t;
        }

        static async Task ReceiveData()
        {
            Debug.Log($"Started receiving data...");
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);
            WebSocketReceiveResult result = null;

            while (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseSent)
            {
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            OnReceiveMessage?.Invoke(reader.ReadToEnd());
                        }
                    }
                }
            }

            Debug.LogWarning($"Socket closed. State: {ws.State}, (Out of loop) Not receiving any more data.");
            OnSocketClosed?.Invoke(ws.State);
        }

        public static void Dispose()
        {
            if (ws != null)
            {
                ws.Dispose();
                ws = null;
            }
        }
    }
}