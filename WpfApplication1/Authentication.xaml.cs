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
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Authentication : Window
    {
        private EventsArguments _evArgs;
        public event EventHandler<EventsArguments> MyEvent;

        protected void OnMyEvent()
        {
            if (this.MyEvent != null)
            {
                this.MyEvent(this, _evArgs);
            }
                
        }

        public Authentication()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // signUp
            _evArgs = new EventsArguments();
            _evArgs.RunServer = (bool)radioButton1.IsChecked;
            _evArgs.TypeEvent = (bool)radioButton3.IsChecked ? "signup": "login";
            _evArgs.NickName = textBox.Text;
            _evArgs.Password = textBox1.Text;
            this.OnMyEvent();
            this.Close();
        }
    }

    public class EventsArguments : EventArgs
    {
        public bool RunServer;
        public string NickName;
        public string Password;
        public string TypeEvent;
     }
}
