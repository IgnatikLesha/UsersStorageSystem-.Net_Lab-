namespace SocketClient
{
    using DAL.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousClient
    {
        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.
        private String response = String.Empty;
        public IEnumerable<ServiceConfigurationInfo> clientsInfo;
        //public string Message { get; set; }
        //private string message;
        private List<Socket> sockets = new List<Socket>();

        public AsynchronousClient(IEnumerable<ServiceConfigurationInfo> config)
        {
            clientsInfo = config;
            // Create a TCP/IP socket.
            //Socket client = new Socket(AddressFamily.InterNetwork,
            //           SocketType.Stream, ProtocolType.Tcp);
            //sockets.Add(client);
        }
        public void StartClient()
        {
            // Connect to a remote device.
            try
            {
                foreach (var clientInfo in clientsInfo)
                {
                    // Create a TCP/IP socket.
                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sockets.Add(client);
                    // Connect to the remote endpoint.
                    client.BeginConnect(clientInfo.IpEndPoint,
                        new AsyncCallback(ConnectCallback), client);
                    connectDone.WaitOne();

                    //Send test data to the remote device.
                    //Send(client, Message);
                    //sendDone.WaitOne();

                    // Receive the response from the remote device.
                    //Receive(client);
                    //receiveDone.WaitOne();

                    //// Write the response to the console.
                    //Console.WriteLine("Response received : {0}", response);
                    //while (true)
                    //{
                    //    var quit = Console.ReadKey();
                    //    if (quit.Key == ConsoleKey.Escape)
                    //        break;
                    //}
                    // Release the socket.
                    //client.Shutdown(SocketShutdown.Both);
                }
                Console.WriteLine("Enter to continue.");

                Console.ReadKey();
                //client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private  void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();

               // Receive(client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //private static void ConnectCallback(IEnumerable<IAsyncResult> ars)
        //{
        //    foreach (var ar in ars)
        //    {
        //        ConnectCallback(ar);
        //    }
        //}
        //public void Recieve()
        //{
        //    foreach (var socket in sockets)
        //    {
        //        Receive(socket);
        //    }
        //    //receiveDone.WaitOne();
        //}

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
                Console.WriteLine("Response received : {0}", response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);



                if (bytesRead > 0)
                {
                    Array.Resize(ref state.buffer, bytesRead);
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    Array.Resize(ref state.buffer, client.ReceiveBufferSize);

                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                    Console.WriteLine("Response received : {0}", response);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(Socket client, string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            Console.WriteLine("The server holds the following operation : {0}", data);
            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
            
        }

        public void Send(string data)
        {
            foreach (var socket in sockets)
            {
                Send(socket, data);
                //Receive(socket);
            }
            //sendDone.WaitOne();
            
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


    }
}
