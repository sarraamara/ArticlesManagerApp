using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
namespace Gestion_Articles
{
    public partial class Authentification : Form
    {
        public Authentification()
        {
            InitializeComponent();
        }
        SqlConnection cnx = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("conString"));
        MailMessage message;
        SmtpClient smtp;
        string userMail = "";
        string newPassword = "";
        private void getUsers()
        {
            try
            {
                cnx.Open();
                string sql = @"SELECT code_user, password FROM users";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, cnx);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                userBox.DataSource = ds.Tables[0];
                userBox.ValueMember = "password";
                userBox.DisplayMember = "code_user";
                cnx.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Authentification_Load(object sender, EventArgs e)
        {
            
            getUsers();
            passwordBox.PasswordChar = '*';
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            
            if (passwordBox.Text == "")
            {
                passwordProvider.SetError(passwordBox, "Veuillez saisir votre mot de passe!");
                MessageBox.Show("Veuillez saisir votre mot de passe!");
            }
            else if (userBox.SelectedValue.ToString() == passwordBox.Text)
            {
                
                AffichageArticles affArticle = new AffichageArticles(userBox.Text);
                //affArticle.code_user = userBox.Text;
                affArticle.Show();
                this.Hide();
 
            }
            else
            {
                passwordProvider.SetError(passwordBox, "Mot de passe incorrect!");
                MessageBox.Show("Mot de passe incorrect!");

            }
        }
       
        private void userBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (userBox.SelectedIndex == 0)
            {
                passwordBox.Enabled = false;
                connectButton.Enabled = false;
                passwordButton.Enabled = false;
            }
            else
            {
                passwordBox.Enabled = true;
                connectButton.Enabled = true;
                passwordButton.Enabled = true;
            }

        }

        private void passwordButton_Click(object sender, EventArgs e)
        {
            //PasswordForm passwordForm = new PasswordForm();
            //passwordForm.ShowDialog();
            sendMail();
        }
        private void sendMail()
        {
            try
            {
                    cnx.Open();
                    string sql = @"SELECT email FROM users WHERE code_user='"+userBox.Text+"'";
                    SqlCommand command = new SqlCommand(sql, cnx);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    userMail = reader["email"].ToString();
                    reader.Dispose();
                    command.Dispose();
                    cnx.Close();
       
                    message = new MailMessage();
                    message.To.Add(userMail);
                    message.Subject = "[TEST]Mot de passe";
                    message.From = new MailAddress("*******");
                    newPassword = System.Web.Security.Membership.GeneratePassword(6, 2);
                    message.Body = "Bonjour, Voici votre nouveau mot de passe temporaire: " + newPassword;
                    
                    smtp = new SmtpClient("smtp.gmail.com");
                    smtp.Port = 25;
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential("*******", "*******");
                    smtp.SendAsync(message, message.Subject);
                    smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        void smtp_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("L'envoi de l'email échoué!");
            }
            else if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                updatePassword(newPassword);
                MessageBox.Show("Un nouveau mot de passe a été envoyé à:" + userBox.Text);
                
            }
        }
        private void updatePassword(string password)
        {
            cnx.Open();
            string sql = "UPDATE users SET password='" + password + "', password_tmp='True' WHERE email='" + userMail + "'";
            SqlCommand command = new SqlCommand(sql, cnx);
            command.ExecuteNonQuery();
            command.Dispose();
            cnx.Close();
        }
        private void Authentification_Activated(object sender, EventArgs e)
        {
            getUsers();
            passwordBox.PasswordChar = '*';
        }

       


        


        
    }
}
