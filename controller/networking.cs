using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace controller
{
    /// <summary>
    /// Determines what to do next. Sets the network action that should occurr next. 
    /// </summary>
    /// <param name="state"></param>
    public delegate void NetworkAction(SocketState state);

    /// <summary>
    /// Class holds all information necessary to represent a socket connection. 
    /// All fields are public because we're using like a struct. 
    /// Simple collection of fields.
    /// </summary>
    public class SocketState
    {
        public SocketState(Socket _sock)
        {
            sock = _sock;
            bite = new byte[1000];
            bs = new StringBuilder();
        }
        public Socket sock { get; private set; }
        public byte[] bite { get; private set; }
        public StringBuilder bs { get; private set; }

        //This is how the networking library will "notify" users when a connection is made, or when data recieved. 
        public NetworkAction Call;
    }


    public static class networking
    {
        public static void MakeSocket(string hostName, out Socket socket, out IPAddress ipAddress)
        {
            ipAddress = IPAddress.None;
            socket = null;
            try
            {
                // Establish the remote endpoint for the socket.
                IPHostEntry ipHostInfo;

                // Determine if the server address is a URL or an IP
                try
                {
                    ipHostInfo = Dns.GetHostEntry(hostName);
                    bool foundIPV4 = false;
                    foreach (IPAddress addr in ipHostInfo.AddressList)
                        if (addr.AddressFamily != AddressFamily.InterNetworkV6)
                        {
                            foundIPV4 = true;
                            ipAddress = addr;
                            break;
                        }
                    // Didn't find any IPV4 addresses
                    if (!foundIPV4)
                    {
                        System.Diagnostics.Debug.WriteLine("Invalid addres: " + hostName);
                        throw new ArgumentException("Invalid address");
                    }
                }
                catch (Exception)
                {
                    // see if host name is actually an ipaddress, i.e., 155.99.123.456
                    System.Diagnostics.Debug.WriteLine("using IP");
                    ipAddress = IPAddress.Parse(hostName);
                }

                // Create a TCP/IP socket.
                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                // Disable Nagle's algorithm - can speed things up for tiny messages, 
                // such as for a game
                socket.NoDelay = true;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create socket. Error occured: " + e);
                throw new ArgumentException("Invalid address");
            }
        }


        /// <summary>
        /// Allows a hostname to resolve to a Socket, getting the IP address from a DHCP server if needed. 
        /// </summary>
        /// <param name="callMe"></param>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public static Socket ConnectToServer(NetworkAction callMe, string hostname)
        {
            Socket socket;
            IPAddress ip;

            MakeSocket("localhost", out socket, out ip);

            SocketState state = new SocketState(socket);

            state.Call = callMe;

            state.sock.BeginConnect(ip, 11000, ConnectedCallback, state);
            return state.sock;
        }

        /// <summary>
        /// This function is "called" by the operating system when the remote site acknowledges connect request
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectedCallback(IAsyncResult ar)
        {
            SocketState state = (SocketState)ar.AsyncState;

            try
            {
                // Complete the connection.
                state.sock.EndConnect(ar);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to connect to server. Error occured: " + e);
                return;
            }

            //Don't start an event loop to recieve data from the server. The client may not want to do that. 
            //Instead, invoke the client's delegate so it can do whatever it wants.
            state.Call(state);
        }

        /// <summary>
        /// Function called by the operating system when data available on the socket.
        /// </summary>
        /// <param name="ar"></param>
        public static void ReceiveCallback(IAsyncResult ar)
        {
            //method called by the OS when new data arrives.  This method should check to see how much data has arrived.  If 0, the connection
            //has been closed (presumably by the server).  On greater than zero data, this method should get the 
            //SocketState object out of the IAsyncResult (just as above in 2), and call the callback function provided in the SocketState
            //copy data to growable buffer. 

            //Get the socket state out of the Async State
            SocketState state = (SocketState)ar.AsyncState;
            try
            {
                int bytesRead = state.sock.EndReceive(ar);



                //if the socket is still open
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(state.bite, 0, bytesRead);
                    // Append to the growable buffer. We're going to grow piece by piece. May be incomplete!?
                    state.bs.Append(message);
                    //Instead, invoke the client's delegate so it can do whatever it wants.
                    state.Call(state);

                }
            }
            catch
            {
                //Do nothing to handle elegantlyt a disconnect
                Console.WriteLine();
            }
        }

        /// <summary>
        /// This is a wrapper for beginRecieve. 
        /// This is the public entry point for asking for data.
        /// Necessary so networking concerns separate from client concerns.
        /// </summary>
        /// <param name="sockMonkey"></param>
        public static void GetData(SocketState sockMonkey)
        {

            //Client code will call whenever it wants more data.
            sockMonkey.sock.BeginReceive(sockMonkey.bite, 0, sockMonkey.bite.Length, SocketFlags.None, ReceiveCallback, sockMonkey);
        }


        /// <summary>
        /// Allows the passed in data be sent to the server via the Socket's BeginSend method.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        public static bool Send(SocketState socket, String data)
        {
            try
            {
                //This function (Along with its helper 'sendCallback') will allow a program to send data over a socket.  This 
                //function needs to convert the data into bytes and then send them using Socket.BeginSend
                byte[] message = Encoding.UTF8.GetBytes(data);

                socket.sock.BeginSend(message, 0, message.Length, SocketFlags.None, SendCallback, socket);
                return true;
            }
            catch
            {
                //The socket is dead
                return false;
            }
        }

        /// <summary>
        /// Assists the send function.
        /// Extract the socket out of the IAsyncResult, and then call socket.EndSend  
        /// </summary>
        /// <param name="ar"></param>
        static void SendCallback(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;
            try
            {
                ss.sock.EndSend(ar);
            }
            catch { }
            
        }


        /// <summary>
        /// Loop to run when a server is waiting for a client to connect.
        /// Starts handshake.
        /// </summary>
        /// <param name="action"></param>
        public static void ServerAwaitingClientLoop(NetworkAction action)
        {
            //Should start a TcpListner for new Connections and pass the listener,
            //along with the CallMe delegate to BeginAcceptSocket as the state 
            Socket socket;
            IPAddress ip;
            //TODO get the IPAddress from the userinterface. Put in the below MakeSocket.

            MakeSocket("localhost", out socket, out ip);
            TcpListener listen = new TcpListener( ip, 11000);
            listen.Start();
            ConnectState cs = new ConnectState(socket);

            cs.listener = listen;
            cs.Call = action;
            listen.BeginAcceptSocket(AcceptNewClient, cs);
        }

        /// <summary>
        /// Allows the server to accept a new client!
        /// </summary>
        /// <param name="ar"></param>
        public static void AcceptNewClient(IAsyncResult ar)
        {
            ConnectState cs = (ConnectState)ar.AsyncState;
            Socket s = cs.listener.EndAcceptSocket(ar);
            SocketState ss = new SocketState(s);
            ss.Call = cs.Call;
            ss.Call(ss);
            cs.listener.BeginAcceptSocket(AcceptNewClient, cs); // And starts the loop over again so can continually accept new.
        }




        /// <summary>
        /// Like a SocketState, but relevant for servers.
        /// </summary>
        public class ConnectState
        {
            public ConnectState(Socket _sock)
            {
                sock = _sock;
                bite = new byte[1000];
                bs = new StringBuilder();
            }
            public Socket sock { get; private set; }
            public byte[] bite { get; private set; }
            public StringBuilder bs { get; private set; }

            //This is how the networking library will "notify" users when a connection is made, or when data recieved. 
            public NetworkAction Call;
            public TcpListener listener;

        }
    }
}
