using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private bool UnCheckedAuth;
        delegate void SetTextCallback();
        TcpClient _clientSocket;
        NetworkStream _serverStream = default(NetworkStream);
        string _readData = null;
        Thread _clientChatThread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            // Send Message            
            if (_clientSocket != null)
            {
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox2.Text + "$");
                _serverStream.Write(outStream, 0, outStream.Length);
                _serverStream.Flush();
                textBox2.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Open Authenticate Window
            var newFrm = new Authentication();
            newFrm.MyEvent += new EventHandler<EventsArguments>(ConnectOrCreateChat);
            newFrm.Show();
        }

        private void ConnectOrCreateChat(object sender, EventsArguments e)
        {
            if (e.RunServer)
            {
                Thread ctThread = new Thread(Server.StartServer);
                ctThread.Start();
            }
            
            _readData = "";
            if (_clientSocket == null) _clientSocket = new TcpClient();
            _clientSocket.Connect("127.0.0.1", 8888);
            _serverStream = _clientSocket.GetStream();
            byte[] outStream;
            if (e.TypeEvent == "signup")
            {
                outStream = System.Text.Encoding.ASCII.GetBytes(e.Password+"7d2abf2d0fa7c3a0c13236910f30bc43"+e.NickName + "$");
            }else if (e.TypeEvent == "login")
            {
                outStream = System.Text.Encoding.ASCII.GetBytes(e.Password + "d56b699830e77ba53855679cb1d252da" + e.NickName + "$");
            }
            else
            {
                outStream = System.Text.Encoding.ASCII.GetBytes(e.NickName + "$");
            }
             
            _serverStream.Write(outStream, 0, outStream.Length);
            _serverStream.Flush();

            _clientChatThread = new Thread(GetMessage);
            _clientChatThread.Start();
            label1.Content = e.NickName;
            //CheckIsAth(); 
        }

        private void CheckIsAth()
        {
            //UnCheckedAuth = true;
            byte[] outStream = Encoding.ASCII.GetBytes("ddbb706f8d37c145a044d931ab097eac"+ label1.Content+ "$");
            _serverStream.Write(outStream, 0, outStream.Length);
            _serverStream.Flush();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            // quite
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes("1a505b46f54bdd130ab920910e33428b" + "$");
            _serverStream.Write(outStream, 0, outStream.Length);
            _serverStream.Flush();
            _clientSocket = null;
            _clientChatThread.Abort();
            label1.Content = "Not authorized";
        }

        private void GetMessage()
        {
            while (true)
            {
                _serverStream = _clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[10025];
                _clientSocket.ReceiveBufferSize = 8192;
                buffSize = _clientSocket.ReceiveBufferSize;
                _serverStream.Read(inStream, 0, buffSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                _readData = "" + returndata;
                /*
                if (returndata.Contains("ddbb706f8d37c145a044d931ab097eac"))
                {

                }*/
                Msg();
            }
        }

        private void Msg()
        {
            if(_readData.Length > 0)
            {
                textBox1.Dispatcher.Invoke(new SetTextCallback(textBox1_TextChanged));
            }
        }

        private void textBox1_TextChanged()
        {
            textBox1.AppendText(Environment.NewLine + " >> " + _readData.Replace("\0", string.Empty));
        }

    }
}
