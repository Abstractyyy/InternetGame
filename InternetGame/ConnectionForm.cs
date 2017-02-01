using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InternetGame
{
    public partial class ConnectionForm : Form
    {
        public ConnectionForm()
        {
            InitializeComponent();
        }
        public static string connection;

        private void button1_Click(object sender, EventArgs e)
        {
            connection = textBox1.Text;
            NetworkConnection _connection = new NetworkConnection();
            Game1 tetris = new Game1();
        }
    }
}
