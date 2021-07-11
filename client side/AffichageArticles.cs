using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;
namespace Gestion_Articles
{
    public partial class AffichageArticles : Form
    {
        
        public AffichageArticles(string codeUser)
        {
            this.code_user = codeUser;
            InitializeComponent();
        }

        SqlConnection cnx = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("conString"));
        public string code_user="";
        private void ConnectBtn_Click(object sender, EventArgs e)
        {

            this.Close();
                 
        }

        private void getArticlesList()
        {
            try 
            {
                cnx.Open();
                string sql = @"SELECT Article.Ref_Article,Article.Design_Article,Article.PU,
                            FamilleArticle.Design_Famille,Article.TVA,Article.StockMin FROM Article INNER JOIN FamilleArticle ON Article.CodeFamille=FamilleArticle.CodeFamille"
                           + " WHERE code_user='"+code_user+"'";
                             
                           
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, cnx);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds, "Articles_table");
                dataGridView1.DataSource = ds.Tables["Articles_table"];
                cnx.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AffichageArticles_Load(object sender, EventArgs e)
        {

            verifyAuthentification();
            userLabel.Text = "Utilisateur " + code_user;
            getArticlesList();
        }
        private void verifyAuthentification()
        {
            cnx.Open();
            string sql = @"SELECT password_tmp FROM users WHERE code_user='" + code_user + "'";
            SqlCommand command = new SqlCommand(sql, cnx);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            string password_tmp = reader["password_tmp"].ToString();
            reader.Dispose();
            command.Dispose();
            cnx.Close();
            if (password_tmp == "True")
            {
                PasswordForm passwordForm = new PasswordForm();
                passwordForm.code_user = code_user;
                passwordForm.ShowDialog();
            }

        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            //this.Hide();

            MàJ_Article majArticle = new MàJ_Article();
            majArticle.buttonValue = "Insert";
            majArticle.code_user = code_user;
            majArticle.ShowDialog();
           
        }

        private void AffichageArticles_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void AffichageArticles_Activated(object sender, EventArgs e)
        {
            getArticlesList();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            MàJ_Article majArticle = new MàJ_Article();
            majArticle.setRowToModify(dataGridView1.CurrentRow);
            majArticle.setButtonValue("Modify");
            majArticle.ShowDialog();
           
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                string ref_article = (dataGridView1.CurrentRow.Cells["Ref_Article"].Value).ToString();
                DialogResult dialogResult = MessageBox.Show("Êtes vous sûr?", "Attention", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    cnx.Open();
                    string sql = @"DELETE FROM Article WHERE Ref_Article='" + ref_article + "'";
                    SqlCommand command = new SqlCommand(sql, cnx);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    cnx.Close();
                    MessageBox.Show("Suppression réussie!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
//                    cnx.Open();
//                    string sql = @"SELECT Article.Ref_Article,Article.Design_Article,Article.PU,
//                            Article.TVA,Article.StockMin,FamilleArticle.Design_Famille FROM Article INNER JOIN FamilleArticle ON Article.CodeFamille=FamilleArticle.CodeFamille"
//                               + " WHERE code_user='" + code_user + "'";
//                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, cnx);
//                    DataSet1 ds = new DataSet1();
//                    CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument =
//                        new CrystalDecisions.CrystalReports.Engine.ReportDocument();
//                    reportDocument.Load(Application.StartupPath + "\\articles.rpt");
//                    dataAdapter.Fill(ds, "Articles_table");
//                    reportDocument.SetDataSource(ds.Tables[1]);
                    CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument =
                                               new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                    reportDocument.Load(Application.StartupPath + "\\articlesNew.rpt");
                    reportDocument.SetParameterValue("code_user", code_user);
                    CrystalReportViewer rp = new CrystalReportViewer();
                    rp.ReportSource = reportDocument;
                    cnx.Close();
                    reportDocument.PrintOptions.PrinterName = printDialog.PrinterSettings.PrinterName;
                    reportDocument.PrintToPrinter(
                        printDialog.PrinterSettings.Copies,
                        printDialog.PrinterSettings.Collate,
                        printDialog.PrinterSettings.FromPage,
                        printDialog.PrinterSettings.ToPage
                        );

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void chatButton_Click(object sender, EventArgs e)
        {
            ChatBox chatBox = new ChatBox();
            chatBox.code_user = code_user;
            chatBox.ShowDialog();
        }

    }
}