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
using System.Data.Sql;

namespace BC
{
    public partial class BizContacts : Form
    {
        string connString = @"Data Source=DESKTOP-PC\SQLEXPRESS;Initial Catalog=AddressBook;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlDataAdapter dataAdapter;
        DataTable table;
        SqlCommandBuilder commandBuilder;
        SqlConnection conn;
        string selectionStatement = "select * from BizContacts";

        public BizContacts()
        {
            InitializeComponent();
        }

        private void BizContacts_Load(object sender, EventArgs e)
        {
            cboSearch.SelectedIndex = 0;
            dataGridView1.DataSource = bindingSource1;

            GetData(selectionStatement);
        }

        private void GetData(string selectCommand)
        {
            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, connString);
                table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                bindingSource1.DataSource = table;
                dataGridView1.Columns[0].ReadOnly = true;
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SqlCommand command;
            string insert = @"insert into BizContacts(Date_Added, Company, Website, Title, First_Name, Last_Name, Address, City, State, Postal_Code, Mobile, Notes) values(@Date_Added, @Company, @Website, @Title, @First_Name, @Last_Name, @Address, @City, @State, @Postal_Code, @Mobile, @Notes)";
            using (conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    command = new SqlCommand(insert, conn);
                    command.Parameters.AddWithValue(@"Date_Added", dateTimePicker1.Value.Date);
                    command.Parameters.AddWithValue(@"Company", txtCompany.Text);
                    command.Parameters.AddWithValue(@"Website", txtWebsite.Text);
                    command.Parameters.AddWithValue(@"Title", txtTitle.Text);
                    command.Parameters.AddWithValue(@"First_Name", txtFName.Text);
                    command.Parameters.AddWithValue(@"Last_Name", txtLName.Text);
                    command.Parameters.AddWithValue(@"Address", txtAddress.Text);
                    command.Parameters.AddWithValue(@"City", txtCity.Text);
                    command.Parameters.AddWithValue(@"State", txtState.Text);
                    command.Parameters.AddWithValue(@"Postal_Code", txtZip.Text);
                    command.Parameters.AddWithValue(@"Mobile", txtMobile.Text);
                    command.Parameters.AddWithValue(@"Notes", txtNotes.Text);
                    command.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            GetData(selectionStatement);
            dataGridView1.Update();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            try
            {
                bindingSource1.EndEdit();
                dataAdapter.Update(table);
                MessageBox.Show("Update Successfull");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentCell.OwningRow;
            string value = row.Cells["ID"].Value.ToString();
            string fname = row.Cells["First_Name"].Value.ToString();
            string lname = row.Cells["Last_Name"].Value.ToString();
            DialogResult result = MessageBox.Show("Do you really want to delete " + fname + " " + lname + ", record " + value,"Message" , MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            string deleteState = @"Delete from BizContacts where id= '" + value + "'";
            if (result == DialogResult.Yes)
            {
                using (conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand(deleteState, conn);
                        comm.ExecuteNonQuery();
                        GetData(selectionStatement);
                        dataGridView1.Update();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            
        }
    }
}
