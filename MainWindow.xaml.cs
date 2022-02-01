using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace SpringPractice1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isBusy = false;
        private UdpClient client;
        private IPAddress groupAddress;
        private int localPort, remotePort, ttl;

        private IPEndPoint remoteEP;
        private UnicodeEncoding encoding = new UnicodeEncoding();

        private string name, msg;

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            try
            {
                var config = ConfigurationManager.AppSettings;
                groupAddress = IPAddress.Parse(config["groupAddress"]);
                localPort = int.Parse(config["localPort"]);
                remotePort = int.Parse(config["remotePort"]);
                ttl = int.Parse(config["ttl"]);
            }
            catch (Exception e)
            {
                MessageBox.Show("Application configuration file is incorrect.\n" + e.Message, "Error");
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isBusy)
                StopListener();
        }

        private void Listener()
        {
            isBusy = true;
            try
            {
                while (isBusy)
                {
                    IPEndPoint ep = null;
                    byte[] buffer = client.Receive(ref ep);
                    msg = encoding.GetString(buffer);

                    Dispatcher.Invoke(() => DisplayReceivedMessage());
                }
            }
            catch (Exception e)
            {
                if (!isBusy) return;
                MessageBox.Show(e.Message, "Error");
            }
        }

        private void DisplayReceivedMessage()
        {
            tbChat.Text += $"{DateTime.Now.ToString("t")} - {msg}\n";
        }
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            StopListener();
        }
        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            name = tbName.Text;
            tbName.IsEnabled = false;

            try
            {
                client = new UdpClient(localPort);
                client.JoinMulticastGroup(groupAddress, ttl);
                remoteEP = new IPEndPoint(groupAddress, remotePort);

                Thread receiver = new Thread(new ThreadStart(Listener));
                receiver.IsBackground = true;
                receiver.Start();

                byte[] data = encoding.GetBytes($"{name} has joined the chat!");
                client.Send(data, data.Length, remoteEP);

                startBtn.IsEnabled = false;
                stopBtn.IsEnabled = true;
                sendBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        private void StopListener()
        {
            byte[] data = encoding.GetBytes($"{name} has left the chat!");
            client.Send(data, data.Length, remoteEP);

            client.DropMulticastGroup(groupAddress);
            client.Close();

            isBusy = false;
            
            startBtn.IsEnabled = true;
            stopBtn.IsEnabled = false;
            sendBtn.IsEnabled = false;
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] data = encoding.GetBytes($"{name}: '{tbMsg.Text}'");
                client.Send(data, data.Length, remoteEP);
                tbMsg.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
