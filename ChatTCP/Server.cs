using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatTCP.Server {

    public class Server {
        static TcpListener tcpListener; // Listen server
        List<Client> clients = new List<Client>(); //All connecions

        protected internal void AddConnection(Client clientObject) {
            clients.Add(clientObject);
        }

        protected internal void RemoveConnection(string id) {
            Client client = clients.FirstOrDefault(c => c.Id == id);

            if (client != null)
                clients.Remove(client);
        }

        // Listen init messages
        protected internal void Listen() {
            try {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Server start. Wait connecntions...");

                while (true) {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    Client clientObject = new Client(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // Send message to all clients
        protected internal void BroadcastMessage(string message, string id) {
            byte[] data = Encoding.Unicode.GetBytes(message);

            for (int i = 0; i < clients.Count; i++) {
                if (clients[i].Id != id) {
                    clients[i].Stream.Write(data, 0, data.Length); 
                }
            }
        }

        // Disconnect all clients
        protected internal void Disconnect() {
            tcpListener.Stop(); //Stop server

            for (int i = 0; i < clients.Count; i++) {
                clients[i].Close(); //Disconect user 
            }
            Environment.Exit(0); //Close progra.
        }
    }
}