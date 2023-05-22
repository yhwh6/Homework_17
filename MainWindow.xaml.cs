using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Homework_16
{
    public partial class MainWindow : Window
    {
        SqlConnection connectionSQL;
        SqlDataAdapter daSQL;
        DataTable dtSQL;

        OleDbConnection? connectionAccess;
        OleDbDataAdapter? daAccess;
        DataTable? dtAccess;

        DataRowView? row;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void PrepareDataSources()
        {
            QueryMSSQL();
            QueryMSAccess();
        }

        /// <summary>
        /// Getting data from MSSQLLocalDB
        /// </summary>
        private void QueryMSSQL()
        {
            SqlConnectionStringBuilder strCon = new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = "Homework_16",
                IntegratedSecurity = true,
            };

            using (connectionSQL = new SqlConnection(strCon.ConnectionString))
            {
                connectionSQL.StateChange += SqlConnection_StateChange;
                try
                {
                    connectionSQL.Open();
                    dtSQL = new DataTable();
                    daSQL = new SqlDataAdapter();
                    var sql = @"SELECT * FROM Users Order By Users.Id";
                    daSQL.SelectCommand = new SqlCommand(sql, connectionSQL);

                    sql = @"INSERT INTO Users (lastName, firstName, middleName, phoneNumber, email) 
                                 VALUES (@lastName, @firstName, @middleName, @phoneNumber, @email); 
                     SET @Id = @@IDENTITY;";

                    daSQL.InsertCommand = new SqlCommand(sql, connectionSQL);
                    daSQL.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id").Direction = ParameterDirection.Output;
                    daSQL.InsertCommand.Parameters.Add("@lastName", SqlDbType.NVarChar, 50, "lastName");
                    daSQL.InsertCommand.Parameters.Add("@firstName", SqlDbType.NVarChar, 50, "firstName");
                    daSQL.InsertCommand.Parameters.Add("@middleName", SqlDbType.NVarChar, 50, "middleName");
                    daSQL.InsertCommand.Parameters.Add("@phoneNumber", SqlDbType.NVarChar, 50, "phoneNumber");
                    daSQL.InsertCommand.Parameters.Add("@email", SqlDbType.NVarChar, 50, "email");

                    sql = @"UPDATE Users SET 
                            lastName = @lastName,
                            firstName = @firstName, 
                            middleName = @middleName,
                            phoneNumber = @phoneNumber,
                            email = @email
                            WHERE Id = @Id";

                    daSQL.UpdateCommand = new SqlCommand(sql, connectionSQL);
                    daSQL.UpdateCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id").SourceVersion = DataRowVersion.Original;
                    daSQL.UpdateCommand.Parameters.Add("@lastName", SqlDbType.NVarChar, 50, "lastName");
                    daSQL.UpdateCommand.Parameters.Add("@firstName", SqlDbType.NVarChar, 50, "firstName");
                    daSQL.UpdateCommand.Parameters.Add("@middleName", SqlDbType.NVarChar, 50, "middleName");
                    daSQL.UpdateCommand.Parameters.Add("@phoneNumber", SqlDbType.NVarChar, 50, "phoneNumber");
                    daSQL.UpdateCommand.Parameters.Add("@email", SqlDbType.NVarChar, 50, "email");

                    sql = "DELETE FROM Users WHERE Id = @Id";

                    daSQL.DeleteCommand = new SqlCommand(sql, connectionSQL);
                    daSQL.DeleteCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id");

                    daSQL.Fill(dtSQL);
                    dg1.DataContext = dtSQL.DefaultView;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "PrepareMSSQL");
                }
            }
        }

        /// <summary>
        /// Getting data from AccessDB
        /// </summary>
        private void QueryMSAccess()
        {
            OleDbConnectionStringBuilder strCon = new OleDbConnectionStringBuilder()
            {
                Provider = @"Microsoft.ACE.OLEDB.12.0",
                DataSource = @"Homework_16.accdb"
            };

            using (connectionAccess = new OleDbConnection(strCon.ConnectionString))
            {
                connectionAccess.StateChange += SqlConnection_StateChange;
                try
                {
                    connectionAccess.Open();
                    dtAccess = new DataTable();
                    daAccess = new OleDbDataAdapter();
                    var sql = @"SELECT * FROM Goods WHERE email = @email Order By Goods.ID";
                    daAccess.SelectCommand = new OleDbCommand(sql, connectionAccess);
                    daAccess.SelectCommand.Parameters.Add("@email", OleDbType.Char, 50, "email");

                    if (dg1.SelectedItem is DataRowView selectedRow)
                    {
                        daAccess.SelectCommand.Parameters["@email"].Value =
                            selectedRow.Row.Field<string>("email");
                    }
                    else
                    {
                        daAccess.SelectCommand.Parameters["@email"].Value =
                            "mail@mail";
                    }

                    sql = @"INSERT INTO Goods (email, code, goodName) 
                                    VALUES (@email, @code, @goodName);
                                    SET @ID = @@IDENTITY;";

                    daAccess.InsertCommand = new OleDbCommand(sql, connectionAccess);

                    daAccess.InsertCommand.Parameters.Add("@ID", OleDbType.Integer, 4, "ID").Direction = ParameterDirection.Output;
                    daAccess.InsertCommand.Parameters.Add("@email", OleDbType.Char, 50, "email");
                    daAccess.InsertCommand.Parameters.Add("@code", OleDbType.Char, 50, "code");
                    daAccess.InsertCommand.Parameters.Add("@goodName", OleDbType.Char, 50, "goodName");

                    sql = @"UPDATE Goods SET 
                            email = @email,
                            code = @code, 
                            goodName = @goodName 
                            WHERE ID = @ID";

                    daAccess.UpdateCommand = new OleDbCommand(sql, connectionAccess);
                    daAccess.UpdateCommand.Parameters.Add("@ID", OleDbType.Integer, 4, "ID").SourceVersion = DataRowVersion.Original;
                    daAccess.UpdateCommand.Parameters.Add("@email", OleDbType.Char, 50, "email");
                    daAccess.UpdateCommand.Parameters.Add("@code", OleDbType.Char, 50, "code");
                    daAccess.UpdateCommand.Parameters.Add("@goodName", OleDbType.Char, 50, "goodName");

                    sql = "DELETE FROM Goods WHERE ID = @ID";

                    daAccess.DeleteCommand = new OleDbCommand(sql, connectionAccess);
                    daAccess.DeleteCommand.Parameters.Add("@ID", OleDbType.Integer, 4, "ID");

                    daAccess.Fill(dtAccess);
                    dg2.DataContext = dtAccess.DefaultView;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "PrepareMSAccess");
                }
            }
        }

        private void SqlConnection_StateChange(object sender, StateChangeEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                string connectionInfo = "";
                if (sender is SqlConnection sqlConnection)
                {
                    connectionInfo = $"{DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss")} {sqlConnection.Database} is in state: {sqlConnection.State}\n" +
                                     $"Connection string: {sqlConnection.ConnectionString}\n";
                }
                else if (sender is OleDbConnection oleDbConnection)
                {
                    connectionInfo = $"{DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss")} {oleDbConnection.DataSource} is in state: {oleDbConnection.State}\n" +
                                     $"Connection string: {oleDbConnection.ConnectionString}\n";
                }

                tb1.Text += connectionInfo;
            });
        }

        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            connectionSQL?.Close();
            connectionSQL?.Dispose();
            connectionAccess?.Close();
            connectionAccess?.Dispose();

            await Task.Delay(100);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PrepareDataSources();
        }

        private void dg1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (row != (DataRowView)e.Row.Item)
            {
                row = (DataRowView)e.Row.Item;
            }

            bool filled = true;
            int colCount = row.Row.Table.Columns.Count;
            for (int i = 1; i < colCount; i++)
            {
                filled &= !string.IsNullOrEmpty(row[i].ToString());
            }

            if (filled)
            {
                row.BeginEdit();
            }
        }

        private void dg1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (row != null)
            {
                bool filled = true;
                int colCount = row.Row.Table.Columns.Count;
                for (int i = 1; i < colCount; i++)
                {
                    filled &= !string.IsNullOrEmpty(row[i].ToString());
                }

                if (filled)
                {
                    row.EndEdit();
                    daSQL.Update(dtSQL);
                }
            }
        }

        private void dg1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is DataGridCell)
            {
                switch (e.Key)
                {
                    case Key.Delete:
                        if (MessageBox.Show($"Are you sure you want to delete client {((DataRowView)dg1.SelectedItem)[0]}?", "Deleting client", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            row = (DataRowView)dg1.SelectedItem;
                            row.Row.Delete();
                            daSQL.Update(dtSQL);
                            row = null;
                        }
                        break;
                }
            }
        }

        private void dg1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (daAccess != null && e.AddedItems.Count > 0 && dg1.SelectedItem != null)
            {
                if (e.AddedItems[0] != dg1.SelectedItem)
                {
                    QueryMSAccess();
                }
            }
        }

        private void MenuItem_ClearDB(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the user database?", "Clearing the database", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                dg1.SelectedItem = null;

                foreach (DataRowView drv in dg1.ItemsSource)
                {
                    drv.Delete();
                }

                daSQL.Update(dtSQL);

                dg1.SelectedIndex = 0;
            }
        }

    }
}
