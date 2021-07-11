using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;

namespace Gestion_Articles
{
    public partial class PasswordForm : Form
    {
        public PasswordForm()
        {
            InitializeComponent();

        }

        SqlConnection cnx = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("conString"));
        public string code_user = "";

        private void sendButton_Click(object sender, EventArgs e)
        {
            
            if(isValidPassword())
            {
                try
                {
                    
                    cnx.Open();
                    string sql = "UPDATE users SET password='" + newPasswordBox.Text + "', password_tmp='False' WHERE code_user='" + code_user + "'";
                    SqlCommand command = new SqlCommand(sql, cnx);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    cnx.Close();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private bool isValidPassword()
        {
            if (newPasswordBox.Text == "")
            {
                passwordProvider.SetError(newPasswordBox, "Veuillez saisir votre nouveau mot de passe!");
                return false;
            }
            else if (newPasswordBox.Text.Length < 6)
            {
                passwordProvider.SetError(newPasswordBox, "Votre mot de passe doit dépasser au moins 6 caractères!");
                return false;
            }
            else
            {
                passwordProvider.Clear();
                return true;
            }
        }

        private void showPasswordButton_Click(object sender, EventArgs e)
        {
            if (newPasswordBox.PasswordChar == '*')
            {
                this.showPasswordButton.Image = global::Gestion_Articles.Properties.Resources.hide;
                newPasswordBox.PasswordChar = '\0';
            }
            else
            {
                this.showPasswordButton.Image = global::Gestion_Articles.Properties.Resources.eye;
                newPasswordBox.PasswordChar = '*';
            }
           

        }

 

      
    }
}
