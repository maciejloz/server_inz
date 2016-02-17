using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using Microsoft.Win32;
using Server_Knowledge_checking.Utilities;

namespace Server_Knowledge_checking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Utilities.ChangingAppearance changeApp;
        Connection.Server server;

        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Connection.ClientHandler.InitializeDictionary();
            changeApp = new Utilities.ChangingAppearance(this);
            //TcpListener tcpListener = new TcpListener("127.0.0.1", 8080);

            //tcpListener.Start();

            //tcpListener.AcceptTcpClientAsync();           
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void getTestButton_Click(object sender, RoutedEventArgs e)
        {
            if (courseName.Text != "" && groupName.Text != "")
            {           
                UsableMethods.OpenTest(courseName.Text, groupName.Text);

                if (UsableMethods.IS_OK)
                    changeApp.ChangeLabelsVisibilityWhenRun(courseName.Text, groupName.Text);
            }
            else
                MessageBox.Show("Wprowadź poprawne dane");
        }

        private void cancelTestButton_Click(object sender, RoutedEventArgs e)
        {
            UsableMethods.CancelTest();
            changeApp.ChangeLabelsVisibilityWhenNotRun();
        }

        private void connectWithClientsButton_Click(object sender, RoutedEventArgs e)
        {
            IPAddress ip;
            int port;

            if ( portNumber.Text != "" && System.Convert.ToInt32(portNumber.Text) < 65535 )
            {
                if (ipAddress.Text != "" && IPAddress.TryParse(ipAddress.Text, out ip))
                {
                    try
                    {
                        port = System.Convert.ToInt16(portNumber.Text);
                        server = new Connection.Server(ip, port);
                        DataContext = server;
                        changeApp.ChangeLabelsVisibilityWhenConnected(ip.ToString(), port.ToString());
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex + "Nie można połączyć z portem");
                    }
                }
                else
                {
                    ipAddress.Text = "";
                    MessageBox.Show("Podaj prawidłowy adres Ip");
                }
            }
            else
            {
                portNumber.Text = "";
                MessageBox.Show("Podaj prawidłowe wartości pól");
            }

        }

        private void disconnectWithClientsButton_Click(object sender, RoutedEventArgs e)
        {
            changeApp.ChangeLabelsVisibilityWhenDisconnected();
            server.CloseConnection();
        }

        private void sendTestButton_Click(object sender, RoutedEventArgs e)
        {
            server.SendTestToAllClients();
            server.EndTestSendEvent += Server_EndTestSendEvent;
        }

        private async void waitForReportsHandler()
        {
            await server.WaitForReport();
        }

        private void Server_EndTestSendEvent()
        {
            waitForReportsHandler();
        }
    }
}
