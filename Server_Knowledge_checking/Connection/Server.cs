using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Connection
{
    delegate void del();

    class Server
    {
        //public static del myDelegate;
        public static IPAddress ipAdress;
        public static int portNumber;
        public TcpListener tcpListener;
        public TcpClient tcpClient;
        //private ObservableCollection<Client> clientsList = new ObservableCollection<Client>();
        private Task _listenTask;
        private bool _isConnectionCanceled;
         
        public Server(IPAddress ip, int port)
        {
            ipAdress = ip;
            portNumber = port;
            _isConnectionCanceled = false;

            clientsList = new ObservableCollection<ClientHandler>();


            //myDelegate += ListenClients;
            _listenTask = new Task(ListenClients);
            _listenTask.Start();
           // await ListenClients();
        }

        ~Server()
        {
            //this._listenTask.Dispose();
            if(tcpListener != null)
                tcpListener.Stop();
        }

        public ObservableCollection<ClientHandler> clientsList
        {
            get;
            set;
        }

        //private static ObservableCollection<Client> DeepCopy<Client>(ObservableCollection<Client> list) where Client : ICloneable
        //{
        //    ObservableCollection<Client> newList = new ObservableCollection<Client>();
        //    foreach (Client rec in list)
        //    {
        //        newList.Add((Client)rec.Clone());
        //    }
        //    return newList;
        //}

        public void ListenClients()
        {
            try
            {

                int i = 0;
                tcpListener = new TcpListener(IPAddress.Any, portNumber);
                //tcpClient = new TcpClient();
                tcpListener.Start();
                int counter = 0;
                while (!_isConnectionCanceled)
                {
                    //tcpListener.BeginAcceptSocket(new AsyncCallback(AcceptTcpClientCallback), tcpListener);
                    //pending okresla, czy jest zadanie polaczenia oczekujacego, jesli jest to wchodzi do if'a
                    if (tcpListener.Pending())
                    {
                        tcpClient = tcpListener.AcceptTcpClient();
                        //tcpListener.Server.EnableBroadcast = true;
                        counter += 1;
                        ClientHandlerCreator(tcpClient, counter);
                    }
                }
                //listenTask.Dispose();

                //tcpListener.BeginAcceptTcpClient(new AsyncCallback(myMethod), tcpListener);


                //if (tcpClient != null)
                //    tcpClient.Close();
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            //finally
            //{
            //    if(tcpClient != null && tcpClient.Connected)
            //        tcpClient.Close();
            //    tcpListener.Stop();
            //}

        }


        //public async Task ListenClients()
        //{
        //    try
        //    {

        //        int i = 0;
        //        tcpListener = new TcpListener(ipAdress, portNumber);
        //        //tcpClient = new TcpClient();
        //        tcpListener.Start();
        //        int counter = 0;
        //        //while (!isConnectionCanceled)
        //        //{
        //        //    //tcpListener.BeginAcceptSocket(new AsyncCallback(AcceptTcpClientCallback), tcpListener);
        //        //    tcpClient = tcpListener.AcceptTcpClient();
        //        //    counter += 1;
        //        //    ClientHandler(tcpClient, counter);

        //        //}

        //            tcpListener.BeginAcceptTcpClient(new AsyncCallback(myMethod), tcpListener);


        //        //if (tcpClient != null)
        //        //    tcpClient.Close();
        //    }
        //    catch(SocketException ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }

        //}

        //private void myMethod(IAsyncResult ar)
        //{
        //    TcpListener s = (TcpListener)ar.AsyncState;
        //    tcpClient = s.EndAcceptTcpClient(ar);
        //}

        private void ClientHandlerCreator(TcpClient tcpClient, int counter)
        {
            IPEndPoint IP = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            int myCounter = counter;
            Connection.ClientHandler clientObject = new Connection.ClientHandler(tcpClient, myCounter, IP);
            //clientsList.Add(clientObject);

            clientObject.ClientCommunicationErrorEvent += ClientObject_ClientCommunicationErrorEvent;
            //clientObject.ClientCloseCommunicationEvent += ClientObject_ClientCloseCommunicationEvent;

            clientObject.GetClientInfo();

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
            new Action(() => this.clientsList.Add(clientObject)));

            //clientsList.First();
            
            
            //Task clientTask = new Task()

        }

        public async void SendTestToAllClients()
        {

            //ze wzgledu na uzywanie w trakcie wysylania(w razie niepowodzenia callbackowej metody ClientObject_ClientCommunicationErrorEvent, musimy stworzyc
            //liste, do ktorej beda kopiowane wszystkie obiekty klientow z listy typu ObservableCollection
            List<ClientHandler> clientsListToSend = new List<ClientHandler>();
            foreach (var client in clientsList)
            {
                clientsListToSend.Add(client);
            }

            //po skopiowaniu metoda SendTestToClient jest wywolywana dla kazdego klienta z listy clientsListToSend, ktora jest typu List, a nie ObservableCollection
            foreach (var client in clientsListToSend)
            {
                await client.SendTestToClient();
            }
        }

        public void CloseConnection()
        {
            //ustawiamy flage dla metody obslugujacec TcpListenera na true(oznacza zakonczenie watku nasluchujacego)
            this._isConnectionCanceled = true;

            //dla kazdego obiektu klienta 
            foreach (var client in clientsList)
            {
                if(client.tcpClient != null && client.tcpClient.Connected)
                    client.tcpClient.Close();

            }
            clientsList.Clear();
        }

        /// <summary>
        /// Metoda callbackowa dla ClientObject_ClientCommunicationErrorEvent, gdy cos sie wysypie, wyrzucimy to do message boxa i zamkniemy polaczenie z danym klientem
        /// </summary>
        /// <param name="message">tresc wiadomosci i bledzie przekazanej do message boxa</param>
        /// <param name="client">wskaznik na klienta, z ktorym przerwano z jakiegos wzledu polaczenie</param>
        private void ClientObject_ClientCommunicationErrorEvent(string message, Connection.ClientHandler client)
        {
            MessageBox.Show(message);

            //metoda clientsList.Remove jest wywolywana z innego watku niz watek UI, wiec musimy wykorzystac Dispatchera do usuniecia elementu z listy, ktora
            //jest zbindowana z kontrolka stworzona w watku UI
            if (client.tcpClient != null && this.clientsList.Count > 0) //nie mozemy usunac danego elementu, jesli wczesniej zostala wyczyszczona cala lista
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() => this.clientsList.Remove(client)));

            //clientsList.Remove(client);
            //clientsList.Clear();
            if (client.tcpClient != null && client.tcpClient.Connected)
                client.tcpClient.Close();
        }
        //private void ClientObject_ClientCloseCommunicationEvent(Connection.Client client)
        //{
        //    clientsList.Remove(client);
        //}

        //private void AcceptTcpClientCallback(IAsyncResult ar)
        //{
        //    TcpListener localListener = (TcpListener)ar.AsyncState;
        //    tcpClient = localListener.EndAcceptTcpClient(ar);
        //    string client = "Client connected";
        //    localListener.Stop();
        //    tcpClient.Close();
        //}
    }
}
