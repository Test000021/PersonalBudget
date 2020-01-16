using PersonalBudget.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersonalBudget
{
    public partial class PersonalBudget : Form
    {
        #region Private members

        private List<string> currency = new List<string>{  "EUR", "USD", "RUB", "RON", "MDL",
                                                            "UAH", "GBP", "CHF", "XAU", "JPY",
                                                            "BGN", "BYN", "HUF", "AUD", "CAD",
                                                            "CZK", "DKK", "EGP", "PLN", "SEK", "TRY"  };
        private BindingSource bs = new BindingSource();
        private DataSet outgoings;
        private DataSet incomes;
        private DataSet savings;
        private string username;
        private int userId;

        #endregion

        #region Initialization 

        SqlConnection connection;

        public PersonalBudget(int _userId, string _username)
        {
            username = _username;
            userId = _userId;           

            InitializeComponent();
        }

        #endregion

        #region Used Common Controls

        private void PersonalBudget_Load(object sender, EventArgs e)
        {
            InitializeControls();
            foreach (var item in currency)
            {
                cbCurrency1.Items.Add(item);
                cbCurrency2.Items.Add(item);
                cbIncomeCurrency1.Items.Add(item);
                cbIncomeCurrency2.Items.Add(item);
                cbSavingsCurrency.Items.Add(item);
            }

            if (userId == 1 && username == "admin")
            {
                usersToolStripMenuItem.Visible = true;
            }
            else
            {
                usersToolStripMenuItem.Visible = false;
            }
         }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Personal Budget application" +
                "\n\n* Desktop application for budget evidence" +
                "\n* The database needs a sql conection" +
                "\n* The curency can be changed but it's a must to work only " +
                "\n with one currency, in order to obtain valid results" +
                "\n* Only the admin account has the option to edit the users accounts" +
                "", "Information");
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new LoginForm();
            form.Show();
            this.Hide();
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new UsersForm();
            form.Show();
        }

        #endregion

        #region Outgoings

        //ADD Outgoings by single date
        private async void button1_Click(object sender, EventArgs e)
        {
            if (tbDayOutgoing.Text == "" || dtDayOutgoing.Value == null ||
                cbCurrency1.SelectedItem == null || rbDetailsDayOutgoing.Text == "")
            {
                errorProvider1.SetError(tbDayOutgoing, "Please enter amount");
                errorProvider2.SetError(dtDayOutgoing, "Please select a date");
                errorProvider3.SetError(cbCurrency1, "Please select a currency");
                errorProvider4.SetError(rbDetailsDayOutgoing, "Please enter details");
            }
            else
            {
                errorProvider1.Dispose();
                errorProvider2.Dispose();
                errorProvider3.Dispose();
                errorProvider4.Dispose();
                using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
                {
                    try
                    {
                        connection.Open();
                        string insertString = $@"insert into Outgoings (Amount, StartDate, EndDate, Currency, Details, AccountFk) 
                                  values ('{tbDayOutgoing.Text}', '{dtDayOutgoing.Value}',
                                  '{dtDayOutgoing.Value}', '{cbCurrency1.SelectedItem}', '{rbDetailsDayOutgoing.Text}', 1)";

                        // 1. Instantiate a new command with a query and connection
                        SqlCommand cmd = new SqlCommand(insertString, connection);

                        // 2. Call ExecuteNonQuery to send command
                        await cmd.ExecuteNonQueryAsync();

                        connection.Close();

                        tbDayOutgoing.Text = "";
                        rbDetailsDayOutgoing.Text = "";
                        dtDayOutgoing.ResetText();
                        cbCurrency1.ResetText();
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }           
        }

        //ADD Outgoings by multi date
        private async void button2_Click(object sender, EventArgs e)
        {
            if (tbPeriodOutgoing.Text == "" || dtPeriodOutgoing1.Value == null || dtPeriodOutgoing2.Value == null ||
               cbCurrency2.SelectedItem == null || rbDetailsPeriodOutgoing.Text == "")
            {
                errorProvider5.SetError(tbPeriodOutgoing, "Please enter amount");
                errorProvider6.SetError(dtPeriodOutgoing1, "Please select a date");
                errorProvider7.SetError(dtPeriodOutgoing2, "Please select a date");
                errorProvider8.SetError(cbCurrency2, "Please select a currency");
                errorProvider9.SetError(rbDetailsPeriodOutgoing, "Please enter details");
            }
            else
            {
                errorProvider5.Dispose();
                errorProvider6.Dispose();
                errorProvider7.Dispose();
                errorProvider8.Dispose();
                errorProvider9.Dispose();
                using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
                {
                    connection.Open();
                    string insertString = $@"insert into Outgoings (Amount, StartDate, EndDate, Currency, Details, AccountFk) 
                                  values ('{tbPeriodOutgoing.Text}', '{dtPeriodOutgoing1.Value}',
                                  '{dtPeriodOutgoing2.Value}', '{cbCurrency2.SelectedItem}','{rbDetailsPeriodOutgoing.Text}', 1)";

                    // 1. Instantiate a new command with a query and connection
                    SqlCommand cmd = new SqlCommand(insertString, connection);

                    // 2. Call ExecuteNonQuery to send command
                    await cmd.ExecuteNonQueryAsync();

                    connection.Close();

                    tbPeriodOutgoing.Text = "";
                    rbDetailsPeriodOutgoing.Text = "";
                    dtPeriodOutgoing1.ResetText();
                    dtPeriodOutgoing2.ResetText();
                    cbCurrency2.ResetText();
                }
            }
        }

        //GET Outgoings grindview
        private void button3_Click(object sender, EventArgs e)
        {
            using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
            {
                connection.Open();
                decimal amount = 0;
                SqlCommand cmd = new SqlCommand($"select SUM(Amount) as Total from Outgoings where StartDate >= " +
                    $"'{dtOutgoings1.Value}' and EndDate <= " +
                    $"'{dtOutgoings2.Value}'", connection);
                var result = cmd.ExecuteScalar();

                if (!string.IsNullOrEmpty(result.ToString()))
                {
                    amount = Convert.ToDecimal(result);
                }
                
                cmd.Dispose();

                SqlDataAdapter daOutgoings = new SqlDataAdapter($"select Id, StartDate, EndDate, Amount, Currency, Details from Outgoings where StartDate between" +
                    $"'{dtOutgoings1.Value}' and" +
                    $" '{dtOutgoings2.Value}'", connection);

                SqlCommandBuilder cmdBldr = new SqlCommandBuilder(daOutgoings);
                outgoings = new DataSet();

                daOutgoings.Fill(outgoings);
                dgrOutgoings.DataSource = null;
                dgrOutgoings.DataSource = outgoings.Tables[0];
                totalOutgoings.Text = amount.ToString();

                connection.Close();
            }

        }       

        //UPDATE Outgoings
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
                {
                    connection.Open();
                    SqlDataAdapter daOutgoings = new SqlDataAdapter("select * from Outgoings", connection);
                    SqlCommandBuilder command = new SqlCommandBuilder(daOutgoings);
                    daOutgoings.Update(outgoings);
                    connection.Close();
                    MessageBox.Show("Updated !");
                }
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message);
            }            
        }

        //DELETE Outgoings
        private async void button10_Click(object sender, EventArgs e)
        {
            using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
            {
                connection.Open();
                string rowIds = "";
                List<int> rows = new List<int>();

                if (this.dgrOutgoings.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow item in this.dgrOutgoings.SelectedRows)
                    {
                        var id = Convert.ToInt32(item.Cells[0].Value);
                        rows.Add(id);
                        dgrOutgoings.Rows.RemoveAt(item.Index);                        
                    }

                    rowIds = string.Join(",", rows);
                    string sql = "DELETE FROM Outgoings WHERE Id  in (" + rowIds + ")";
                    SqlCommand delcmd = new SqlCommand(sql, connection);
                    delcmd.Connection = connection;
                    await delcmd.ExecuteNonQueryAsync();
                    connection.Close();
                }
                else
                {
                    MessageBox.Show("Please select one row");
                }
            }
        }

        //RESET single date
        private void button7_Click(object sender, EventArgs e)
        {
            tbDayOutgoing.Text = "";
            rbDetailsDayOutgoing.Text = "";
            dtDayOutgoing.ResetText();
            cbCurrency1.ResetText();
            errorProvider1.Dispose();
            errorProvider2.Dispose();
            errorProvider3.Dispose();
            errorProvider4.Dispose();
        }

        //RESET multi date
        private void button8_Click(object sender, EventArgs e)
        {
            tbPeriodOutgoing.Text = "";
            rbDetailsPeriodOutgoing.Text = "";
            dtPeriodOutgoing1.ResetText();
            dtPeriodOutgoing2.ResetText();
            cbCurrency2.ResetText();
            errorProvider5.Dispose();
            errorProvider6.Dispose();
            errorProvider7.Dispose();
            errorProvider8.Dispose();
            errorProvider9.Dispose();
        }

        #endregion

        #region Income

        //ADD single income
        private async void button3_Click_1(object sender, EventArgs e)
        {
            if (tbSingleIncome.Text == "" || dtSingleIncome.Value == null ||
                cbIncomeCurrency1.SelectedItem == null || rtbSingleIncome.Text == "")
            {
                errorProvider10.SetError(tbSingleIncome, "Please enter amount");
                errorProvider11.SetError(dtSingleIncome, "Please select a date");
                errorProvider12.SetError(cbIncomeCurrency1, "Please select a currency");
                errorProvider13.SetError(rtbSingleIncome, "Please enter details");
            }
            else
            {
                errorProvider10.Dispose();
                errorProvider11.Dispose();
                errorProvider12.Dispose();
                errorProvider13.Dispose();
                using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
                {
                    try
                    {
                        connection.Open();
                        string insertString = $@"insert into Incomes (Amount, StartDate, EndDate, Currency, Details, AccountFk) 
                                  values ('{tbSingleIncome.Text}', '{dtSingleIncome.Value}',
                                  '{dtSingleIncome.Value}', '{cbIncomeCurrency1.SelectedItem}', '{rtbSingleIncome.Text}', 1)";

                        // 1. Instantiate a new command with a query and connection
                        SqlCommand cmd = new SqlCommand(insertString, connection);

                        // 2. Call ExecuteNonQuery to send command
                        await cmd.ExecuteNonQueryAsync();

                        connection.Close();

                        tbSingleIncome.Text = "";
                        rtbSingleIncome.Text = "";
                        dtSingleIncome.ResetText();
                        cbIncomeCurrency1.ResetText();
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }
        }

        //ADD Multi
        private async void button2_Click_1(object sender, EventArgs e)
        {
            if (tbMultiIncome.Text == "" || dtMultiIncome2.Value == null || dtMultiIncome1.Value == null ||
               cbIncomeCurrency2.SelectedItem == null || rtbMultiIncome.Text == "")
            {
                errorProvider14.SetError(tbMultiIncome, "Please enter amount");
                errorProvider15.SetError(dtMultiIncome2, "Please select a date");
                errorProvider16.SetError(dtMultiIncome1, "Please select a date");
                errorProvider17.SetError(cbIncomeCurrency2, "Please select a currency");
                errorProvider18.SetError(rtbMultiIncome, "Please enter details");
            }
            else
            {
                errorProvider14.Dispose();
                errorProvider15.Dispose();
                errorProvider16.Dispose();
                errorProvider17.Dispose();
                errorProvider18.Dispose();
                using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
                {
                    connection.Open();
                    string insertString = $@"insert into Incomes (Amount, StartDate, EndDate, Currency, Details, AccountFk) 
                                  values ('{tbMultiIncome.Text}', '{dtMultiIncome1.Value}',
                                  '{dtMultiIncome2.Value}', '{cbIncomeCurrency2.SelectedItem}','{rtbMultiIncome.Text}', 1)";

                    // 1. Instantiate a new command with a query and connection
                    SqlCommand cmd = new SqlCommand(insertString, connection);

                    // 2. Call ExecuteNonQuery to send command
                    await cmd.ExecuteNonQueryAsync();

                    connection.Close();

                    tbMultiIncome.Text = "";
                    rtbMultiIncome.Text = "";
                    dtMultiIncome1.ResetText();
                    dtMultiIncome2.ResetText();
                    cbIncomeCurrency2.ResetText();
                }
            }
        }

        //GET income
        private void button1_Click_1(object sender, EventArgs e)
        {
            using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
            {
                connection.Open();
                decimal amount = 0;
                SqlCommand cmd = new SqlCommand($"select SUM(Amount) as Total from Incomes where StartDate >= " +
                    $"'{dtIncomeStart.Value}' and EndDate <= " +
                    $"'{dtIncomeEnd.Value}'", connection);
                var result = cmd.ExecuteScalar();

                if (!string.IsNullOrEmpty(result.ToString()))
                {
                    amount = Convert.ToDecimal(result);
                }

                cmd.Dispose();

                SqlDataAdapter daIncomes = new SqlDataAdapter($"select * from Incomes where StartDate between" +
                    $"'{dtIncomeStart.Value}' and" +
                    $" '{dtIncomeEnd.Value}'", connection);

                SqlCommandBuilder cmdBldr = new SqlCommandBuilder(daIncomes);
                incomes = new DataSet();

                daIncomes.Fill(incomes);
                dtIncomeGridView.DataSource = null;
                dtIncomeGridView.DataSource = incomes.Tables[0];
                dtIncomeGridView.Columns[6].Visible = false;
                totalIncome.Text = amount.ToString();

                connection.Close();
            }
        }

        //UPDATE
        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
                {
                    connection.Open();
                    SqlDataAdapter daIncomes = new SqlDataAdapter("select * from Incomes", connection);
                    SqlCommandBuilder command = new SqlCommandBuilder(daIncomes);
                    daIncomes.Update(incomes);
                    connection.Close();
                    MessageBox.Show("Updated !");
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        //DELETE 
        private async void button14_Click(object sender, EventArgs e)
        {
            using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
            {
                connection.Open();
                string rowIds = "";
                List<int> rows = new List<int>();

                if (this.dtIncomeGridView.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow item in this.dtIncomeGridView.SelectedRows)
                    {
                        var id = Convert.ToInt32(item.Cells[0].Value);
                        rows.Add(id);
                        dtIncomeGridView.Rows.RemoveAt(item.Index);
                    }

                    rowIds = string.Join(",", rows);
                    string sql = "DELETE FROM Incomes WHERE Id  in (" + rowIds + ")";
                    SqlCommand delcmd = new SqlCommand(sql, connection);
                    delcmd.Connection = connection;
                    await delcmd.ExecuteNonQueryAsync();
                    connection.Close();
                }
                else
                {
                    MessageBox.Show("Please select one row");
                }
            }
        }

        //RESET single
        private void button11_Click(object sender, EventArgs e)
        {
            tbSingleIncome.Text = "";
            rtbSingleIncome.Text = "";
            dtSingleIncome.ResetText();
            cbIncomeCurrency1.ResetText();
            errorProvider10.Dispose();
            errorProvider11.Dispose();
            errorProvider12.Dispose();
            errorProvider13.Dispose();
        }

        //RESET Multi
        private void button12_Click(object sender, EventArgs e)
        {
            tbMultiIncome.Text = "";
            rtbMultiIncome.Text = "";
            dtMultiIncome2.ResetText();
            dtMultiIncome1.ResetText();
            cbIncomeCurrency2.ResetText();
            errorProvider14.Dispose();
            errorProvider15.Dispose();
            errorProvider16.Dispose();
            errorProvider17.Dispose();
            errorProvider18.Dispose();
        }

        #endregion

        #region Reports
        
        private async void button6_Click(object sender, EventArgs e)
        {
            decimal outValue = 0;
            decimal inValue = 0;
            decimal profitValue = 0;
            ClearCharts();

            using (var db = new BudgetEntities())
            {                
                if (rbYears.Checked)
                {
                    var nrYears = dtProfitEnd.Value.Year - dtProfitStart.Value.Year;
                                      
                    for (int i = 0; i <= nrYears; i++)
                    {
                        decimal? outgoings = db.Outgoings.Where(o => o.StartDate.Year == dtProfitStart.Value.Year + i).Count() > 0 ?
                             db.Outgoings.Where(o => o.StartDate.Year == dtProfitStart.Value.Year + i).Sum(a => a.Amount) : 0;
                        outValue += outgoings.Value;

                        decimal? incomes = db.Incomes.Where(o => o.StartDate.Year == dtProfitStart.Value.Year + i).Count() > 0 
                            ? db.Incomes.Where(o => o.StartDate.Year == dtProfitStart.Value.Year + i).Sum(a => a.Amount) : 0;
                        inValue += incomes.Value;

                        decimal? profit = incomes - outgoings;
                        profitValue += profit.Value;

                        chart1.Series["Profit"].Points.AddXY(dtProfitStart.Value.Year + i, profit);
                        chart1.Series["Profit"].IsValueShownAsLabel = true;
                        chart2.Series["Income"].Points.AddXY(dtProfitStart.Value.Year + i, incomes);
                        chart2.Series["Income"].IsValueShownAsLabel = true;
                        chart3.Series["Outgoings"].Points.AddXY(dtProfitStart.Value.Year + i, outgoings);
                        chart3.Series["Outgoings"].IsValueShownAsLabel = true;
                    }
                }

                if (rbMonths.Checked)
                {                    
                    if (dtProfitEnd.Value.Year == dtProfitStart.Value.Year)
                    {
                        var nrMonths = dtProfitEnd.Value.Month - dtProfitStart.Value.Month;

                        for (int i = 0; i <= nrMonths; i++)
                        {
                            decimal? outgoings = db.Outgoings.Where(o => o.StartDate.Month == dtProfitStart.Value.Month + i).Count() > 0 ?
                                db.Outgoings.Where(o => o.StartDate.Month == dtProfitStart.Value.Month + i).Sum(a => a.Amount) : 0;
                            outValue += outgoings.Value;

                            decimal? incomes = db.Incomes.Where(o => o.StartDate.Month == dtProfitStart.Value.Month + i).Count() > 0
                                ? db.Incomes.Where(o => o.StartDate.Month == dtProfitStart.Value.Month + i).Sum(a => a.Amount) : 0;
                            inValue += incomes.Value;

                            decimal? profit = incomes - outgoings;
                            profitValue += profit.Value;

                            chart1.Series["Profit"].Points.AddXY(dtProfitStart.Value.Month + i, profit);
                            chart1.Series["Profit"].IsValueShownAsLabel = true;
                            chart2.Series["Income"].Points.AddXY(dtProfitStart.Value.Month + i, incomes);
                            chart2.Series["Income"].IsValueShownAsLabel = true;
                            chart3.Series["Outgoings"].Points.AddXY(dtProfitStart.Value.Month + i, outgoings);
                            chart3.Series["Outgoings"].IsValueShownAsLabel = true;
                        }
                    }                    
                }

                labelOutgoing.Text = outValue.ToString();
                labelIncome.Text = inValue.ToString();
                labelProfit.Text = profitValue.ToString();
            }
        }

        private void rbMonths_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void rbYears_CheckedChanged(object sender, EventArgs e)
        {   
        }

        #endregion

        #region Savings

        //Add savings
        private async void button5_Click(object sender, EventArgs e)
        {
            if (tbSavings.Text == "" || dtSavings1.Value == null || dtSavings2.Value == null ||
                cbSavingsCurrency.SelectedItem == null || rtbSavings.Text == "")
            {
                errorProvider19.SetError(tbSavings, "Please enter amount");
                errorProvider20.SetError(dtSavings1, "Please select a date");
                errorProvider21.SetError(dtSavings2, "Please select a date");
                errorProvider22.SetError(cbSavingsCurrency, "Please select a currency");
                errorProvider23.SetError(rtbSavings, "Please enter details");
            }
            else
            {
                errorProvider19.Dispose();
                errorProvider20.Dispose();
                errorProvider21.Dispose();
                errorProvider22.Dispose();
                errorProvider23.Dispose();
                using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
                {
                    try
                    {
                        connection.Open();
                        string insertString = $@"insert into Savings (Amount, StartDate, EndDate, Currency, Details, AccountFk) 
                                  values ('{tbSavings.Text}', '{dtSavings1.Value}',
                                  '{dtSavings2.Value}', '{cbSavingsCurrency.SelectedItem}', '{rtbSavings.Text}', 1)";

                        // 1. Instantiate a new command with a query and connection
                        SqlCommand cmd = new SqlCommand(insertString, connection);

                        // 2. Call ExecuteNonQuery to send command
                        await cmd.ExecuteNonQueryAsync();

                        connection.Close();

                        tbSavings.Text = "";
                        rtbSavings.Text = "";
                        dtSavings1.ResetText();
                        dtSavings2.ResetText();
                        cbSavingsCurrency.ResetText();
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }
        }

        //Get Savings
        private void button4_Click(object sender, EventArgs e)
        {
            using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
            {
                connection.Open();
                decimal amount = 0;
                SqlCommand cmd = new SqlCommand($"select SUM(Amount) as Total from Savings where StartDate >= " +
                    $"'{dtSavingsStart.Value}' and EndDate <= " +
                    $"'{dtSavingsEnd.Value}'", connection);
                var result = cmd.ExecuteScalar();

                if (!string.IsNullOrEmpty(result.ToString()))
                {
                    amount = Convert.ToDecimal(result);
                }

                cmd.Dispose();

                SqlDataAdapter daSavings = new SqlDataAdapter($"select * from Savings where StartDate between" +
                    $"'{dtSavingsStart.Value}' and" +
                    $" '{dtSavingsEnd.Value}'", connection);

                SqlCommandBuilder cmdBldr = new SqlCommandBuilder(daSavings);
                savings = new DataSet();

                daSavings.Fill(savings);
                dtSavingsGridView.DataSource = null;
                dtSavingsGridView.DataSource = savings.Tables[0];
                dtSavingsGridView.Columns[6].Visible = false;
                totalSavings.Text = amount.ToString();

                connection.Close();
            }
        }

        //Update savings
        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
                {
                    connection.Open();
                    SqlDataAdapter daSavings = new SqlDataAdapter("select * from Savings", connection);
                    SqlCommandBuilder command = new SqlCommandBuilder(daSavings);
                    daSavings.Update(savings);
                    connection.Close();
                    MessageBox.Show("Updated !");
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        //Delete
        private async void button16_Click(object sender, EventArgs e)
        {
            using (connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Budget;Trusted_Connection=Yes;"))
            {
                connection.Open();
                string rowIds = "";
                List<int> rows = new List<int>();

                if (this.dtSavingsGridView.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow item in this.dtSavingsGridView.SelectedRows)
                    {
                        var id = Convert.ToInt32(item.Cells[0].Value);
                        rows.Add(id);
                        dtSavingsGridView.Rows.RemoveAt(item.Index);
                    }

                    rowIds = string.Join(",", rows);
                    string sql = "DELETE FROM Savings WHERE Id  in (" + rowIds + ")";
                    SqlCommand delcmd = new SqlCommand(sql, connection);
                    delcmd.Connection = connection;
                    await delcmd.ExecuteNonQueryAsync();
                    connection.Close();
                }
                else
                {
                    MessageBox.Show("Please select one row");
                }
            }
        }

        //Reset 
        private void button17_Click(object sender, EventArgs e)
        {
            tbSavings.Text = "";
            rtbSavings.Text = "";
            dtSavings1.ResetText();
            dtSavings2.ResetText();
            cbSavingsCurrency.ResetText();
            errorProvider19.Dispose();
            errorProvider20.Dispose();
            errorProvider21.Dispose();
            errorProvider22.Dispose();
            errorProvider23.Dispose();
        }

        #endregion

        #region Heler mwthods
        
        private void ClearCharts()
        {
            foreach (var series in chart1.Series)
                series.Points.Clear();

            foreach (var series in chart2.Series)
                series.Points.Clear();

            foreach (var series in chart3.Series)
                series.Points.Clear();

            labelOutgoing.Text = "0";
            labelIncome.Text = "0";
            labelProfit.Text = "0";
        }

        private void InitializeControls()
        {
            //Outgoings
            dtOutgoings1.Format = DateTimePickerFormat.Custom;
            dtOutgoings1.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dtOutgoings2.Format = DateTimePickerFormat.Custom;
            dtOutgoings2.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dtDayOutgoing.Format = DateTimePickerFormat.Custom;
            dtDayOutgoing.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dtPeriodOutgoing1.Format = DateTimePickerFormat.Custom;
            dtPeriodOutgoing1.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dtPeriodOutgoing2.Format = DateTimePickerFormat.Custom;
            dtPeriodOutgoing2.CustomFormat = "MM/dd/yyyy hh:mm:ss";

            //Income
            dtSingleIncome.Format = DateTimePickerFormat.Custom;
            dtSingleIncome.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dtMultiIncome1.Format = DateTimePickerFormat.Custom;
            dtMultiIncome1.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dtMultiIncome2.Format = DateTimePickerFormat.Custom;
            dtMultiIncome2.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dtIncomeStart.Format = DateTimePickerFormat.Custom;
            dtIncomeStart.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dtIncomeEnd.Format = DateTimePickerFormat.Custom;
            dtIncomeEnd.CustomFormat = "MM/dd/yyyy hh:mm:ss";

            //Savings
            dtSavings1.Format = DateTimePickerFormat.Custom;
            dtSavings1.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dtSavings2.Format = DateTimePickerFormat.Custom;
            dtSavings2.CustomFormat = "MM/dd/yyyy hh:mm:ss";          
            dtSavingsStart.Format = DateTimePickerFormat.Custom;
            dtSavingsStart.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dtSavingsEnd.Format = DateTimePickerFormat.Custom;
            dtSavingsEnd.CustomFormat = "MM/dd/yyyy hh:mm:ss";

            //radio bttn
            rbYears.Checked = true;
        }

        #endregion

        #region Unused Controls
        private void dtProfitStart_ValueChanged(object sender, EventArgs e){
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e){
        }

        private void chart3_Click(object sender, EventArgs e){
        }

        private void Profit_Click(object sender, EventArgs e){
        }

        private void label1_Click(object sender, EventArgs e){
        }

        private void label5_Click(object sender, EventArgs e){
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e){
        }

        private void totalOutgoings_Click(object sender, EventArgs e){
        }

        private void label3_Click(object sender, EventArgs e){
        }

        private void tbPeriodOutgoing_TextChanged(object sender, EventArgs e){
        }

        private void label13_Click(object sender, EventArgs e){
        }

        private void label21_Click(object sender, EventArgs e){
        }

        private void dgrOutgoings_CellContentClick(object sender, DataGridViewCellEventArgs e){
        }

        private void Outgoings_Click(object sender, EventArgs e){
        }

        #endregion

    }
}
