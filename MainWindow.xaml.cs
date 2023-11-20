using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Week4Student
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Connection Adapter/Connection
        public static NpgsqlConnection con; //creating object of connection adapter

        //Command Adapter/Execute Query
        public static NpgsqlCommand cmd;

        //Step 1 - Connect button to the PostGreSQL database
        //Substep: generate the connection string
        //Substep: establish the connection and check/verify

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // step 1 - Establish Connection
                establishConnect();

                //step 2 - Open the connection
                  con.Open();

                //step 3 - Generating the query
                string Query = "Insert into students values(@name, default, @email, @dept, @regYr)";

                //step 4 - Pass the query to Command adapter
                cmd = new NpgsqlCommand(Query,con);

                //step 4.2 - Add/define values for the variable
                cmd.Parameters.AddWithValue("@name", name.Text);
                cmd.Parameters.AddWithValue("@email", email.Text);
                cmd.Parameters.AddWithValue("@dept", dept.Text);
                cmd.Parameters.AddWithValue("@regYr", int.Parse(regYr.Text)); //Parse because originally the value is a string

                //step 5 - Execute the command
                cmd.ExecuteNonQuery();

                //step 6 - Send a successful message
                MessageBox.Show("Student created successfully!");

                //step 7 - Close the connection 
                con.Close();
            } catch (NpgsqlException ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void establishConnect()
        {
            try
            {
                con = new NpgsqlConnection(get_ConnectionString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private string get_ConnectionString()
        {
            //For PGSQL connectionString we need five values: host, port, dbName, userName, password

            string host = "Host=localhost;";
            string port = "Port=5432;";
            string dbName = "Database=StudentRegistration;";
            string userName = "Username=postgres;";
            string password = "Password=atuaprima;";

            string connectionString = string.Format("{0}{1}{2}{3}{4}", host, port, dbName, userName, password);
            return connectionString;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
                
            //In this button we will read all the database entries in this method. 
            
            //Step 1 - Establish connection
            establishConnect();

            //Step 2 - Open connection
            con.Open();

            //Step 3 - Create query
            string Query = "select * from students";

            //Step 4 - Create command
            cmd = new NpgsqlCommand(Query, con);

            //Step 5 - We need to create a SQL dataAdapter and a SQL datatable, read the data, execute it, the info needs to
            //go to the datatable and then push it to the dataAdapter to set it back to DataGrid

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            // Now we send the datatable information to dataGrid itemsource
 
            dataGrid.ItemsSource = dt.AsDataView();//making sure dataGrid is getting the full table data

            //Step 7 - Reinitialize our wpf controls data, for dataGrid
            DataContext = da;

            //Step 8 - Close connection
            con.Close();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                //Step 1 - Establish connection
                establishConnect();

                //Step 2 - Open connection
                con.Open();

                //Step 3 - Create query
                string Query = "select * from students where std_id=@Id";

                //Step 4 - Create command
                cmd = new NpgsqlCommand(Query, con);

                //Step 4.2 - Initialize the query variable
                cmd.Parameters.AddWithValue("@Id", int.Parse(search.Text));

                //Step 4.3 - Add a checker/boolean to see if the data is present or not
                bool noData = true;

                //Step 5 - Data Reader adapter
                NpgsqlDataReader dr = cmd.ExecuteReader(); //this line is going to read all the data matches with the query and return them

                //Step 6 - Checking all the info that was grabbed from the database, one by one
                while (dr.Read())
                {
                    noData = false;
                    name.Text = dr["std_name"].ToString();
                    stdid.Text = dr["std_id"].ToString();
                    email.Text = dr["std_email"].ToString();
                    dept.Text = dr["std_dept"].ToString();
                    regYr.Text = dr["std_year"].ToString();
                }
                if(noData)
                {
                    MessageBox.Show("The id you entered is not valid");
                }


                //Step 7 - Close the connection
                con.Close();
            }
            catch(NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
