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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }               

        private void button1_Click(object sender, EventArgs e)
        {
            using (var budgetContext = new BudgetEntities())
            {
                var user = budgetContext.Accounts.Where(acc => acc.Name.Equals(textBox1.Text) 
                                && acc.Password.Equals(textBox2.Text) && acc.Enabled).FirstOrDefault();

                if (user != null)
                {
                    var form = new PersonalBudget(user.Id, user.Name);
                    form.Show();
                    this.Hide();
                }
                else
                {
                    label3.Text = "Invalid username or password !";
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var form = new RegistrationForm();
            form.Show();
        }       

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var form = new PasswordReset();
            form.Show();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
