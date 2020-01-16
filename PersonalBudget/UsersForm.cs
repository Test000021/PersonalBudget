using PersonalBudget.Classes;
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
    public partial class UsersForm : Form
    {
        private List<User> usersList;       

        public UsersForm()
        {
            InitializeComponent();
        }
        
        private void UsersForm_Load(object sender, EventArgs e)
        {
            using (var budgetContext = new BudgetEntities())
            {
                var list = budgetContext.Accounts.ToList();
                usersList = (from usr in list
                            select new User
                            {
                                Id = usr.Id,
                                Name = usr.Name,
                                Password = usr.Password,
                                KeyWord = usr.KeyWord
                            }).ToList();

                dataGridUsers.DataSource = usersList;
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (this.dataGridUsers.SelectedRows.Count > 0)
            {
                using (var budgetContext = new BudgetEntities())
                {
                    foreach (DataGridViewRow item in this.dataGridUsers.SelectedRows)
                    {
                        var id = Convert.ToInt32(item.Cells[0].Value);
                        var user = budgetContext.Accounts.Where(u => u.Id == id).FirstOrDefault();
                        user.Name = Convert.ToString(item.Cells[1].Value);
                        user.Password = Convert.ToString(item.Cells[2].Value);
                        user.KeyWord = Convert.ToString(item.Cells[3].Value);

                        budgetContext.Entry(user).State = EntityState.Modified;
                    }
                    budgetContext.SaveChanges();
                    MessageBox.Show("Users have been edited !");
                }
            }
            else
            {
                MessageBox.Show("Please select one row");
            }
        }
    }
}
