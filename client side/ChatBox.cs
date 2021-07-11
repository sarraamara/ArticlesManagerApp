using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
namespace Gestion_Articles
{
    public partial class ChatBox : Form
    {
        public ChatBox()
        {
            InitializeComponent();
        }

        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        public string code_user = "";
        bool end = true;
        Thread ctThread = null;
        SqlConnection cnx = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("conString"));
        
        private void sendButton_Click(object sender, EventArgs e)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(sendMessageBox.Text + "$");
            sendMessageBox.Clear();
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        private void ChatBox_Load(object sender, EventArgs e)
        {
            codeUserLabel.Text = "Utilisateur " + code_user;
            readData = "Connexion au serveur...";
            msg();
            clientSocket.Connect("127.0.0.1", 8888);
            serverStream = clientSocket.GetStream();

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(code_user + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            updateConnected("True");
            serverStream.Flush();

            ctThread = new Thread(getMessage);
            ctThread.Start();
        }
        private void checkListUsers()
        {
            try
            {
                cnx.Open();
                string sql = @"SELECT code_user FROM users WHERE connected='True'";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, cnx);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                listUsers.DataSource = ds.Tables[0];
                listUsers.DisplayMember = "code_user";
                cnx.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void getMessage()
        {
            while (end)
            {
                try
                {
                    checkListUsers();
                    serverStream = clientSocket.GetStream();
                    int buffSize = 0;
                    buffSize = clientSocket.ReceiveBufferSize;
                    byte[] inStream = new byte[buffSize];
                    serverStream.Read(inStream, 0, buffSize);
                    string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    readData = "" + returndata;
                    msg();
                }
                catch (ThreadAbortException ex1)
                {
                    //DO NOTHING
                }
                catch (Exception ex)
                {
                    serverStream.Close();
                    clientSocket.Close();
                    MessageBox.Show(ex.ToString());
                }
            }
        }

          private void msg() //this method displays the received messages
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
                messagesBox.Text = messagesBox.Text + Environment.NewLine + " >> " + readData;
        }
          private void updateConnected(string connected)
          {
              try
              {
                  cnx.Open();
                  string sql = "UPDATE users SET connected='"+connected+"' WHERE code_user='" + code_user + "'";
                  SqlCommand command = new SqlCommand(sql, cnx);
                  command.ExecuteNonQuery();
                  command.Dispose();
                  cnx.Close();
                  
              }
              catch (Exception ex)
              {
                  MessageBox.Show(ex.ToString());
              }
          }
          private void quitButton_Click(object sender, EventArgs e)
          {
              try
              {
                  DialogResult dialogResult = MessageBox.Show("Êtes vous sûr de quitter la conversation?", "Attention", MessageBoxButtons.YesNo);
                  if (dialogResult == DialogResult.Yes)
                  {
                      updateConnected("False");
                      end = false;
                      ctThread.Abort();
                      byte[] outStream = System.Text.Encoding.ASCII.GetBytes("END$");
                      serverStream.Write(outStream, 0, outStream.Length);
                      serverStream.Flush();
                      serverStream.Close();
                      clientSocket.Close();
                      this.Close();
                  }
              }
              catch (Exception ex)
              {
                  MessageBox.Show(ex.ToString());
              }
          } 
    }
}
