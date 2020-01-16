using PersonalBudget.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersonalBudget
{
    public partial class PasswordReset : Form
    {
        public PasswordReset()
        {
            InitializeComponent();
        }
        
        private void PasswordReset_Load(object sender, EventArgs e)
        {
           
        }

        private void EmptyFields()
        {
            txtName.Text = "";
            txtKeyword.Text = "";
            txtPassword.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtPassword.Text == "" ||
              txtKeyword.Text == "")
            {
                errorProvider1.SetError(txtName, "Please enter a name");
                errorProvider2.SetError(txtPassword, "Please enter a password");
                errorProvider3.SetError(txtKeyword, "Please enter a keyword");
            }
            else
            {
                errorProvider1.Dispose();
                errorProvider2.Dispose();
                errorProvider3.Dispose();
                using (var budgetContext = new BudgetEntities())
                {
                    var user = budgetContext.Accounts.Where(acc => acc.Name.Equals(txtName.Text)
                                    && acc.KeyWord.Equals(txtKeyword.Text)).FirstOrDefault();

                    if (user != null)
                    {
                        user.Password = txtPassword.Text;
                        budgetContext.Entry(user).State = EntityState.Modified;
                        budgetContext.SaveChanges();
                        MessageBox.Show("Success!");
                    }
                    else
                    {
                        label4.Text = "Invalid keyword or name !";
                    }
                }
            }
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
