using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace Bloqueador
{
    public partial class Form1 : Form
    {
        public bool verip = true;
        public bool formo = false;        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            this.Hide();
            VerificaIp();
        }
        public void VerificaIp()
        {
            while (verip)
            {
                try
                {
                    if (formo == true)
                    {
                        System.Threading.Thread.Sleep(60000);
                        Dns.GetHostEntry("192.168.0.1");
                        formo = false;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                        Dns.GetHostEntry("192.168.0.1");
                    }
                }
                catch
                {
                    AbreTela();        
                }
            }
        }
        public void AbreTela()
        {
            if (formo == true)
            {             
                         
            }
            else
            {                
                    formo = true;                   
                    KeyboardHook kb = new KeyboardHook();
                    kb.HookKeyboard();                   
                    this.Show();
                    richTextBox1.Refresh();
                    textBox1.Refresh();
                    button1.Refresh();
                    label1.Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "marcus.ultime")
            {
                string diretorio = Directory.GetCurrentDirectory();
                Process processo = new Process();
                processo.StartInfo = new ProcessStartInfo(diretorio);
                processo.Start();
                Application.Exit();
                
            }
        }
    }
}
