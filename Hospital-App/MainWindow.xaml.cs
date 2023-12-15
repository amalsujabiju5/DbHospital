using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Data;



namespace Hospital_App
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            // hide tab item tbiRoomDash and tbiPhysicianDash on startup 
            tbiRoomDash.Visibility = Visibility.Hidden;
            tbiPhysicianDash.Visibility = Visibility.Hidden;
            btnLogout.Visibility = Visibility.Hidden;
            //populateTable();

            // Connect to database


        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            String username = "Demo";
            String password = "Demo";

            if (txtbUsername.Text == username && txtbPassword.Text == password)
            {
                txtbErrorMsg.Text = "Login Successful";
                tbiRoomDash.Visibility = Visibility.Visible;
                tbiPhysicianDash.Visibility = Visibility.Visible;

                // Resign the btnLogin button to the btnLogout button
                btnLogin.Visibility = Visibility.Hidden;
                btnLogout.Visibility = Visibility.Visible;
            }
            else
            {
                txtbErrorMsg.Text = "Login Failed";
            }
        }

        private void tabcntrlNav_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string connectionString = Hospital_App.Properties.Settings.Default.connection_String;
            //SqlConnection connection = new SqlConnection(connectionString);

            //// Room Dashboard
        }

        public static SqlConnection GetConnection()
        {
            string connectionString = Hospital_App.Properties.Settings.Default.connection_String;
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            return connection;
        }


        private void populatePatientTable()
        {
            SqlConnection connection = GetConnection();
            string query =
                "SELECT ROOM.*, PATIENT.PATIENT_NAME, PATIENT.PATIENT_BED_ID, PATIENT.DISCHARGE_DATE FROM ROOM LEFT JOIN PATIENT ON ROOM.PATIENT_ID = PATIENT.PATIENT_ID;";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            // Fill the datatable with the data from the reader
            dt.Load(reader);

            // Rename the columns
            dt.Columns[0].ColumnName = "Room ID";
            dt.Columns[1].ColumnName = "Bed ID";
            dt.Columns[2].ColumnName = "Patient ID";
            dt.Columns[3].ColumnName = "Room Number";
            dt.Columns[4].ColumnName = "Room Type";
            dt.Columns[5].ColumnName = "Room Status";





            RoomDash.ItemsSource = dt.DefaultView;

            connection.Close();
            reader.Close();




        }


        private void populatePhysicianTable()
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT *\r\nFROM PATIENT\r\nJOIN PATIENT_REPORT\r\nON PATIENT.PATIENT_ID = PATIENT_REPORT.PATIENT_ID;\r\n";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            // Fill the datatable with the data from the reader
            dt.Load(reader);


            //dt.Columns[0].ColumnName = "Room ID";
            //dt.Columns[1].ColumnName = "Bed ID";
            //dt.Columns[2].ColumnName = "Room Number";
            //dt.Columns[3].ColumnName = "Room Type";
            //dt.Columns[4].ColumnName = "Room Status";




            dtgPhysicianList.ItemsSource = dt.DefaultView;

            connection.Close();
            reader.Close();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            tbiRoomDash.Visibility = Visibility.Hidden;
            tbiPhysicianDash.Visibility = Visibility.Hidden;
            btnLogout.Visibility = Visibility.Hidden;
            btnLogin.Visibility = Visibility.Visible;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When a patient is selected clear everything off the screen and display the patient's information 
            // total number of appointments, personal data(name,address,medical history, etc), and previous appointments and upcoming appointments

            if (dtgPhysicianList.SelectedIndex != -1)
            {
                //open patientView window
                //patientView patientView = new patientView();
                //patientView.Show();

            }
        }


        private void dtgPhysicianList_Loaded(object sender, RoutedEventArgs e)
        {
            ;
            populatePhysicianTable();
        }


        // This is the code for the patientView window
        private void tbiRoomDash_Loaded(object sender, RoutedEventArgs e)
        {
            populatePatientTable();
            generatePatientReport();
        }

        private void dtgPhysicianList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //patientView patientView = new patientView();
            //patientView.Show();
        }

        private void generatePatientReport()
        {
            SqlConnection connection = GetConnection();

            
            string queryOne = "SELECT ROOM.ROOM_NUMBER, COUNT(*) AS OCCUPIED_BEDS FROM ROOM WHERE ROOM.OCCUPANCY_STATUS = 'Occupied' GROUP BY ROOM.ROOM_NUMBER;";
            string queryTwo = "SELECT COUNT(*) AS BEDS_DISCHARGING_TODAY FROM ROOM INNER JOIN PATIENT ON ROOM.PATIENT_ID = PATIENT.PATIENT_ID WHERE PATIENT.DISCHARGE_DATE = CURDATE();";
            string queryThree = "SELECT ROOM.ROOM_TYPE, COUNT(*) AS EMPTY_ROOMS FROM ROOM WHERE ROOM.OCCUPANCY_STATUS = 'Unoccupied' GROUP BY ROOM.ROOM_TYPE;";
            string queryFour = "SELECT COUNT(*) AS OCCUPIED_BEDS, COUNT(DISTINCT ROOM.PATIENT_ID) AS OCCUPIED_ROOMS FROM ROOM WHERE ROOM.OCCUPANCY_STATUS = 'Occupied';";
            string queryFive = "SELECT ROOM.ROOM_TYPE, COUNT(*) AS OCCUPIED_ROOMS FROM ROOM WHERE ROOM.OCCUPANCY_STATUS = 'Occupied' GROUP BY ROOM.ROOM_TYPE;";

            SqlCommand commandOne = new SqlCommand(queryOne, connection);
            SqlDataReader readerOne = commandOne.ExecuteReader();

            //while (readerOne.Read())
            //{
            //    txtbRoomOne.Text = readerOne["ROOM_NUMBER"].ToString();
            //    txtbRoomOneBeds.Text = readerOne["OCCUPIED_BEDS"].ToString();
            //}



            connection.Close();
        }

        private void btnOccByRoom_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = GetConnection();
            string query =
                "SELECT ROOM.ROOM_NUMBER, COUNT(ROOM.PATIENT_ID) AS OCCUPIED_BEDS,  COUNT(*) - COUNT(ROOM.PATIENT_ID) AS AVAILABLE_BEDS, COUNT(*) AS TOTAL_BEDS FROM ROOM GROUP BY ROOM.ROOM_NUMBER;\r\n";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            dtgReportGenerate.ItemsSource = dt.DefaultView;

            connection.Close();
            reader.Close();



        }

        private void btnOccByBeds_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = GetConnection();
            string query =
                "SELECT COUNT(*) AS TOTAL_BEDS, COUNT(CASE WHEN OCCUPANCY_STATUS='Occupied' THEN 1 END) AS OCCUPIED_BEDS, COUNT(CASE WHEN OCCUPANCY_STATUS='Vacant' THEN 1 END) AS VACANT_BEDS FROM ROOM;\r\n";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            dtgReportGenerate.ItemsSource = dt.DefaultView;

            connection.Close();
            reader.Close();
        }

        private void btnOccByRoomType_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = GetConnection();
            string query =
                "SELECT r.ROOM_TYPE, COUNT(*) AS OCCUPANCY_COUNT FROM ROOM r WHERE r.OCCUPANCY_STATUS = 'OCCUPIED' GROUP BY  r.ROOM_TYPE ORDER BY CASE r.ROOM_TYPE\r\n        WHEN 'Private' THEN 1\r\n        WHEN 'Semi-Private' THEN 2\r\n        WHEN 'Double' THEN 3\r\n        WHEN 'Single' THEN 4\r\n        ELSE 5\r\n    END;";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            dtgReportGenerate.ItemsSource = dt.DefaultView;

            connection.Close();
            reader.Close();
        }

        private void btnDischarging_Patitents_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = GetConnection();
            string query =
                "SELECT COUNT(*) AS NumBedsDischarging\r\nFROM PATIENT P\r\nJOIN ROOM R ON P.PATIENT_BED_ID = R.BED_ID\r\nWHERE CONVERT(DATE, P.DISCHARGE_DATE) = CONVERT(DATE, GETDATE())\r\n";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            dtgReportGenerate.ItemsSource = dt.DefaultView;

            connection.Close();
            reader.Close();
        }

        private void btnRoomType_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = GetConnection();
            string query =
                "SELECT r.ROOM_TYPE, COUNT(r.ROOM_TYPE) AS NUM_EMPTY_ROOMS\r\nFROM ROOM r LEFT JOIN PATIENT p\r\nON r.PATIENT_ID = p.PATIENT_ID\r\nWHERE p.PATIENT_ID IS NULL\r\nGROUP BY r.ROOM_TYPE;\r\n";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            dtgReportGenerate.ItemsSource = dt.DefaultView;

            connection.Close();
            reader.Close();
        }

        private void dtgPhysicianList_Selected(object sender, RoutedEventArgs e)
        {
            // Get the selected row from dtgPhysicianList
            DataRowView selectedRow = dtgPhysicianList.SelectedItem as DataRowView;

            if (selectedRow != null)
            {
                // Create a new datatable and add the selected row to it
                DataTable dt = new DataTable();
                foreach (DataColumn col in selectedRow.Row.Table.Columns)
                {
                    dt.Columns.Add(col.ColumnName, col.DataType);
                }
                DataRow newRow = dt.NewRow();
                newRow.ItemArray = selectedRow.Row.ItemArray;
                dt.Rows.Add(newRow);

                // Add the new datatable to dtgPatientDetails
                dtgPatientDetails.ItemsSource = dt.DefaultView;
                dtgPatientDetails.CanUserAddRows = true;
                dtgPatientDetails.CanUserDeleteRows = true;
                dtgPatientDetails.IsReadOnly = false;

                // Set IsReadOnly to false for all columns in dtgPatientDetails
                foreach (DataGridColumn column in dtgPatientDetails.Columns)
                {
                    column.IsReadOnly = false;
                }

                // Begin editing the new row in dtgPatientDetails
                dtgPatientDetails.BeginEdit();
            }
        }

        private void btnTotalApp_Click(object sender, RoutedEventArgs e)
        {
            // Generate a random number for total appointments
            Random rand = new Random();
            int totalApp = rand.Next(1, 10);

            // Based on the only row in dtgPatientDetails display a pop up with the patients name and total appointments
            DataRowView selectedRow = dtgPatientDetails.Items[0] as DataRowView;
            MessageBox.Show("Total appointments for " + selectedRow.Row.ItemArray[1] + " "  + " is " + totalApp);
            
            
        }


    

        private void btnPersonalData_Click(object sender, RoutedEventArgs e)
        {
            // Based on the only row in dtgPatientDetails display a pop up with the patients name and personal data
            DataRowView selectedRow = dtgPatientDetails.Items[0] as DataRowView;
            
            
        }

        private void btnAppRecord_Click(object sender, RoutedEventArgs e)
        {
            // Based on the only row in dtgPatientDetails display a pop up with a random appointment record of the last 5 appointments and the date of the latest appointment
            DataRowView selectedRow = dtgPatientDetails.Items[0] as DataRowView;

            // Generate a previous appoint date between 1 and 5 days ago and upcomming appoint date between 1 and 5 days from now
            Random rand = new Random();
            int prevApp = rand.Next(1, 5);
            int upApp = rand.Next(1, 5);

            // print in a pop up window the patients name, the previous appointment date, the upcoming appointment date, and the date of the latest appointment
            MessageBox.Show("Appointment record for " + selectedRow.Row.ItemArray[1] + " "  + ":\r Previous appointment date: " + DateTime.Now.AddDays(-prevApp).ToShortDateString() + "\r Upcoming appointment date: " + DateTime.Now.AddDays(upApp).ToShortDateString() + "\r Date of latest appointment: " + DateTime.Now.ToShortDateString());




        }

        private void btnAssignTreatment_Click(object sender, RoutedEventArgs e)
        {
            // Based on the only row in dtgPatientDetails display a pop up that contains a textbox for the treatment and a button to assign the treatment
            // update the treatment index in the dtgPatientDetails datatable
            //DataRowView selectedRow = dtgPatientDetails.Items[0] as DataRowView;
            AssignTreatment assignTreatment = new AssignTreatment();
            assignTreatment.Show();
            //selectedRow.Row.ItemArray[17] = assignTreatment.txtTeatment.Text;
            // write an sql query to update the treatment in the database selectedRow.Row.ItemArray[17] and set it to assignTreatment.txtTeatment.Text;
            //SqlConnection connection = GetConnection();
            //string query =
            //    "UPDATE PATIENT_REPORT\r\nSET NOTE = 'new note'\r\nWHERE REPORT_ID = 'report_id_value';\r\n";
  










        }

        private void btnAddNote_Click(object sender, RoutedEventArgs e)
        {
            AddNote addNote = new AddNote();
            addNote.Show();
        }
    }
}
