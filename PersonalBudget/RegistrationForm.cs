using PersonalBudget.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersonalBudget
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtPassword.Text == "" ||
                txtKeyWord.Text == "")
            {
                errorProvider1.SetError(txtName, "Please enter a name");
                errorProvider2.SetError(txtPassword, "Please enter a password");
                errorProvider3.SetError(txtKeyWord, "Please enter a keyword");
            }
            else
            {
                errorProvider1.Dispose();
                errorProvider2.Dispose();
                errorProvider3.Dispose();
                using (var budgetContext = new BudgetEntities())
                {
                    var user = new Account();
                    user.Name = txtName.Text;
                    user.Password = txtPassword.Text;
                    user.KeyWord = txtKeyWord.Text;
                    user.Enabled = true;

                    budgetContext.Accounts.Add(user);
                    budgetContext.SaveChanges();
                    EmptyFields();
                    MessageBox.Show("User successful registered !");
                }
            }
        }

        private void EmptyFields()
        {
            txtKeyWord.Text = "";
            txtName.Text = "";
            txtPassword.Text = "";
        }
    }
}
