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
namespace Gestion_Articles
{
    public partial class MàJ_Article : Form
    {
        public MàJ_Article()
        {
            InitializeComponent();
        }

        SqlConnection cnx = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("conString"));
        public string buttonValue = "";
        public DataGridViewRow rowToModify;
        public string code_user="";

        private bool verifyForm()
        {
            verifyErrors();
            if (String.IsNullOrEmpty(refBox.Text) || String.IsNullOrEmpty(designBox.Text) ||
                 String.IsNullOrEmpty(priceBox.Text) || String.IsNullOrEmpty(tvaBox.Text) ||
                 String.IsNullOrEmpty(stockBox.Text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void validateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (verifyForm())
                {
                    cnx.Open();
                    string sql = "";
                    SqlCommand command = null;

                    if (buttonValue.Equals("Insert"))
                    {
                        sql = @"INSERT INTO Article(Ref_Article,Design_Article,PU,CodeFamille,TVA,StockMin,code_user)
                              VALUES (@Ref_Article,@Design_Article,@PU,@CodeFamille,@TVA,@StockMin,@code_user)";
                        command = new SqlCommand(sql, cnx);
                        command.Parameters.AddWithValue("@Ref_Article", refBox.Text);
                        command.Parameters.AddWithValue("@Design_Article", designBox.Text);
                        command.Parameters.AddWithValue("@PU", priceBox.Text);
                        command.Parameters.AddWithValue("@CodeFamille", typeBox.SelectedValue);
                        command.Parameters.AddWithValue("@TVA", tvaBox.Text);
                        command.Parameters.AddWithValue("@StockMin", stockBox.Text);
                        command.Parameters.AddWithValue("@code_user", code_user);
                        command.ExecuteNonQuery();
                        MessageBox.Show("Insertion réussie!");
                    }
                    else if (buttonValue.Equals("Modify"))
                    {
                        sql = @"UPDATE Article SET Ref_Article=@Ref_Article, Design_Article=@Design_Article, PU=@PU,
                            CodeFamille=@CodeFamille, TVA=@TVA, StockMin=@StockMin WHERE Ref_Article=@Ref_Article1";

                        command = new SqlCommand(sql, cnx);

                        command.Parameters.AddWithValue("@Ref_Article", refBox.Text);
                        command.Parameters.AddWithValue("@Design_Article", designBox.Text);
                        command.Parameters.AddWithValue("@PU", priceBox.Text);
                        command.Parameters.AddWithValue("@CodeFamille", typeBox.SelectedValue);
                        command.Parameters.AddWithValue("@TVA", tvaBox.Text);
                        command.Parameters.AddWithValue("@StockMin", stockBox.Text);
                        command.Parameters.AddWithValue("@Ref_Article1", (rowToModify.Cells["Ref_Article"].Value).ToString());

                        command.ExecuteNonQuery();

                        MessageBox.Show("Modification réussie!");
                    }
                    command.Dispose();
                    cnx.Close();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Veuillez remplir tous les champs!");
                }

                
                
                //AffichageArticles affichageArticle = new AffichageArticles();
                //affichageArticle.Show();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
            //AffichageArticles affichageArticle = new AffichageArticles();
            //affichageArticle.Show();
            
        }
        
        private void cancelButton_Click(object sender, EventArgs e)
        {   
            refBox.Clear();
            designBox.Text = string.Empty;
            priceBox.Text = string.Empty;
            tvaBox.Text = string.Empty;
            stockBox.Text = string.Empty;
        }

        private void MàJ_Article_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Application.Exit();
        }
        private void getFamilyCodeDesign()
        {
            cnx.Open();
            string sql = @"SELECT CodeFamille, Design_Famille FROM FamilleArticle";
            SqlCommand command = new SqlCommand(sql, cnx);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            typeBox.DataSource = ds.Tables[0];
            typeBox.ValueMember = "CodeFamille";
            typeBox.DisplayMember = "Design_Famille";
            //SqlDataReader reader = command.ExecuteReader();

            //while (reader.Read())
            //{
            //    typeBox.Items.Add(reader.GetValue(0));
            //}

            //reader.Close();
            
            command.Dispose();
            cnx.Close();
            
        }
        public void setRowToModify(DataGridViewRow row)
        {
            rowToModify = row;
        }
        public void setButtonValue(string s)
        {
            buttonValue = s;
        }
 
        private void MàJ_Article_Load(object sender, EventArgs e)
        {   
            
            getFamilyCodeDesign();
            
            if (buttonValue.Equals("Modify"))
            {
                refBox.Text = (rowToModify.Cells["Ref_Article"].Value).ToString();
                designBox.Text = (rowToModify.Cells["Design_Article"].Value).ToString();
                priceBox.Text = (rowToModify.Cells["PU"].Value).ToString();
                tvaBox.Text = (rowToModify.Cells["TVA"].Value).ToString();
                stockBox.Text = (rowToModify.Cells["StockMin"].Value).ToString();
                typeBox.SelectedText = (rowToModify.Cells["Design_Famille"].Value).ToString();
                
            }
            
           
        }

        private void refBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
                   && !char.IsDigit(e.KeyChar)
                    &&!(char.IsLetter(e.KeyChar)&&char.IsUpper(e.KeyChar))
                    )
            {
                e.Handled = true;
                return;
            }
            
            e.Handled = false;
            
        }

        private void designBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
                   && !char.IsLetter(e.KeyChar)
                    )
            {
                e.Handled = true;
                return;
            }

            e.Handled = false;
        }

        private void priceBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
                   && !char.IsDigit(e.KeyChar)
                   && e.KeyChar != '.'   
                )
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.')
            {
                if (priceBox.Text.IndexOf(".") >= 0 || priceBox.Text.Length == 0)
                {
                    e.Handled = true;
                    return;
                }
            }

            e.Handled = false;
        }

        private void tvaBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
                   && !char.IsDigit(e.KeyChar)
                   && e.KeyChar != '.'
                )
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.')
            {
                if (tvaBox.Text.IndexOf(".") >= 0 || tvaBox.Text.Length == 0)
                {
                    e.Handled = true;
                    return;
                }
            }

            e.Handled = false;
        }

        private void stockBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
                   && !char.IsDigit(e.KeyChar)
                   && e.KeyChar != '.'
                )
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.')
            {
                if (stockBox.Text.IndexOf(".") >= 0 || stockBox.Text.Length == 0)
                {
                    e.Handled = true;
                    return;
                }
            }

            e.Handled = false;
        }

        private void verifyErrors()
        {
            if (refBox.Text == "")
            {
                errorProvider.SetError(refBox, "Veuillez remplir ce champ!");
            }
            else
            {
                errorProvider.Clear();

            }

            if (designBox.Text == "")
            {
                errorProvider.SetError(designBox, "Veuillez remplir ce champ!");
            }
            else
            {
                errorProvider.Clear();
            }

            if (priceBox.Text == "")
            {
                errorProvider.SetError(priceBox, "Veuillez remplir ce champ!");
            }
            else
            {
                errorProvider.Clear();
            }

            if (tvaBox.Text == "")
            {
                errorProvider.SetError(tvaBox, "Veuillez remplir ce champ!");
            }
            else
            {
                errorProvider.Clear();
            }

            if (stockBox.Text == "")
            {
                errorProvider.SetError(stockBox, "Veuillez remplir ce champ!");
            }
            else
            {
                errorProvider.Clear();
            }
        }

        private void refBox_TextChanged(object sender, EventArgs e)
        {
            verifyErrors();
        }

        private void designBox_TextChanged(object sender, EventArgs e)
        {
            verifyErrors();
        }

        private void priceBox_TextChanged(object sender, EventArgs e)
        {
            verifyErrors();
        }

        private void tvaBox_TextChanged(object sender, EventArgs e)
        {
            verifyErrors();
        }

        private void stockBox_TextChanged(object sender, EventArgs e)
        {
            verifyErrors();
        }

    }
    
}
