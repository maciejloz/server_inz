using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;

namespace Connection
{
    class ClientHandler 
    {
        public delegate void ClientCommunicationError(string message, ClientHandler client);
        public event ClientCommunicationError ClientCommunicationErrorEvent;

        //public delegate void ClientCloseCommunication(Client client);
        //public event ClientCloseCommunication ClientCloseCommunicationEvent;


        public TcpClient tcpClient = null;
        public int numberOfClient;
        public string nameOfClient = "";
        public static Dictionary<string, string> TypeOfServerMessage;
        public bool isReportReceive;
        public NetworkStream networkStream;

        private IPEndPoint _ipAddressOfClient;
        
        public ClientHandler(TcpClient tcpcl, int numbofclient, IPEndPoint ipendpoint)
        {
            tcpClient = tcpcl;
            numberOfClient = numbofclient;
            isReportReceive = false;
            _ipAddressOfClient = ipendpoint;

            ipToBind = _ipAddressOfClient.ToString();
            numberToBind = numberOfClient.ToString(); ;
        }

        ~ClientHandler()
        {
            if (networkStream != null)
                networkStream.Dispose();
            if (tcpClient != null && tcpClient.Connected)
            {
                tcpClient.Close();
                //ClientCommunicationErrorEvent("Komunikacja została zamknięta", this);
                //ClientCloseCommunicationEvent(this);
            }
        }

        public string nameToBind
        {
            get;
            set;
        }
        public string ipToBind
        {
            get;
            set;
        }
        public string numberToBind
        {
            get;
            set;
        }

        /// <summary>
        /// Statyczna metoda inicjalizujaca slownik o nazwie TypeOfServerMessage, ktora laczy dana odpowiedz serwera z przesylanym tekstem do klienta
        /// Elementy slownika sa const, dlatego moze byc wywolana tylko raz(w przeciwnym razie zostanie zgloszony wyjatek)
        /// </summary>
        public static void InitializeDictionary()
        {
            if (TypeOfServerMessage == null)
            {
                TypeOfServerMessage = new Dictionary<string, string>();
                TypeOfServerMessage.Add("Response To Logging", "All OK");
                TypeOfServerMessage.Add("ResponseToGettingReport", "Report OK");
                TypeOfServerMessage.Add("SendingTest", "Test was sent");
            }
        }

        public void GetClientInfo()
        {
            byte[] myReadBuffer = new byte[1024];
            StringBuilder myCompleteMessage = new StringBuilder();
            int numberOfBytesRead = 0;

            try
            {
                networkStream = tcpClient.GetStream();

                if (networkStream.CanRead)
                {
                    do
                    {
                        numberOfBytesRead = networkStream.Read(myReadBuffer, 0, myReadBuffer.Length );// myReadBuffer.Length);
                        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                    } while (networkStream.DataAvailable);

                    nameOfClient = myCompleteMessage.ToString();
                    nameToBind = nameOfClient;

                }
                    else
                    {
                        ClientCommunicationErrorEvent("Zaistniały problemy z komunikacją z klientem nr" + numberOfClient.ToString(), this );
                    }
                SendResponseToClient(TypeOfServerMessage["Response To Logging"]);
            }
            catch(ArgumentNullException ex)
            {
                ClientCommunicationErrorEvent("Bufor wysłany przez klienta jest pusty, nr klienta:" + numberOfClient.ToString(), this);
            }
            catch(ArgumentOutOfRangeException ex)
            {
                ClientCommunicationErrorEvent("Klient wysłał za duży bufor danych, nr klienta:" + numberOfClient.ToString(), this);
            }
            catch(IOException ex)
            {
                ClientCommunicationErrorEvent("Wybrane gniazdo po stronie klienta zostało wcześniej zakmnięte", this);
            }
            catch (ObjectDisposedException ex)
            {
                ClientCommunicationErrorEvent("Problem z odczytem danych z sieci", this);
            }

        } 

        private void SendResponseToClient(string messageToClient )
        {
                try
                {
                    Byte[] sendBytes = null;
                    sendBytes = Encoding.ASCII.GetBytes(messageToClient);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                }
                catch (ArgumentNullException ex)
                {
                    ClientCommunicationErrorEvent("Bufor wysłany do klienta jest pusty, nr klienta:" + numberOfClient.ToString(), this);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    ClientCommunicationErrorEvent("Wysłano zbyt duży bufor danych do klienta, nr klienta:" + numberOfClient.ToString(), this);
                }
                catch (IOException ex)
                {
                    ClientCommunicationErrorEvent("Wybrane gniazdo po stronie klienta zostało wcześniej zakmnięte", this);
                }
                catch (ObjectDisposedException ex)
                {
                    ClientCommunicationErrorEvent("Problem z odczytem danych z sieci", this);
                }
        }

        public Task SendTestToClient()
        {
            //Byte[] sendBytes = null;
            //sendBytes = Encoding.ASCII.GetBytes("Tu leci test");
            //Server_Knowledge_checking.UsableMethods.zipPath;
            Task task = Task.Run(() =>
            {
                using (var fileIO = File.OpenRead(Server_Knowledge_checking.Utilities.UsableMethods.zipPath))
                {
                    // Send Length (Int64)
                    try
                    {
                        networkStream.Write(BitConverter.GetBytes(fileIO.Length), 0, 8);
                        //_networkStream.Write(sendBytes, 0, sendBytes.Length);
                        var buffer = new byte[1024 * 8];
                        int count;

                        while ((count = fileIO.Read(buffer, 0, buffer.Length)) > 0)
                            networkStream.Write(buffer, 0, count);

                        networkStream.Flush();
                    }
                    catch (ArgumentNullException ex)
                    {
                        ClientCommunicationErrorEvent("Bufor wysłany do klienta jest pusty, nr klienta:" + numberOfClient.ToString(), this);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        ClientCommunicationErrorEvent("Wysłano zbyt duży bufor danych do klienta, nr klienta:" + numberOfClient.ToString(), this);
                    }
                    catch (IOException ex)
                    {
                        ClientCommunicationErrorEvent("Wybrane gniazdo po stronie klienta zostało wcześniej zakmnięte", this);
                    }
                    catch (ObjectDisposedException ex)
                    {
                        ClientCommunicationErrorEvent("Problem z odczytem danych z sieci", this);
                    }
                }
            });
            return task;
        }

        internal bool GetReport()
        {
            Int64 catchedBytes = 0;
            int countOfBytes;
            var buffer = new byte[1024 * 8];
            string filePath = Server_Knowledge_checking.Utilities.UsableMethods.directoryPath + "\\" + nameOfClient + ".html";
            try
            {
                networkStream.Read(buffer, 0, 8);
                Int64 numberOfBytes = BitConverter.ToInt64(buffer, 0);

                using (var fileWithTest = File.Create(filePath))
                {
                    while (catchedBytes < numberOfBytes && (countOfBytes = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileWithTest.Write(buffer, 0, countOfBytes);
                        catchedBytes += countOfBytes;
                    }
                }
                SendResponseToClient("Report is Ok");

            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("Bufor wysłany przez serwer jest pusty");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show("Serwer wysłał za duży bufor danych");
            }
            catch (IOException ex)
            {
                MessageBox.Show("Wybrane gniazdo zostało wcześniej zakmnięte lub wystąpił problem podczas zapisywania pliku z testem na dysku twardym");
            }
            catch (ObjectDisposedException ex)
            {
                MessageBox.Show("Problem z odczytem danych z sieci");
            }
            return true;
        }

    }
}
