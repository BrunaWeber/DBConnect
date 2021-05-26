using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBConnect;
using MySql.Data.MySqlClient;


namespace DBConect
{
    public partial class FrmDBConnect : Form
    {
        DBConnection db = null;

        int currentIndex = 0;

        List<CarInfo> carInfoList;//added 23/04 - defining the list

        public FrmDBConnect()
        {
            InitializeComponent();
        }

        private void FrmDBConnect_Load(object sender, EventArgs e)
        {
            ToolTip toolTip = new ToolTip(); //added 28/04

            toolTip.SetToolTip(this.textBoxVehicleRegNo, "Enter the registration number of the vehicle.");
            toolTip.SetToolTip(this.textBoxMake, "Enter the make of the vehicle.");
            toolTip.SetToolTip(this.textBoxEngineSize, "Enter the size of the engine.");

            this.carInfoList = new List<CarInfo>();
            
            MakeDBConnection();
            
            // get the record of cars from the database;
            
            LoadCars();
            
            //Display the first element in the CarInfoList to screen
            DisplayCarInfo(this.currentIndex);

            //this.textBoxNavigationStatus.Text = BuildNavigationStatus(); ///Remove or not?

        }

        private void DisplayCarInfo(int index) //abstracted this piece information, we do not know wicth index is this for now...
        {
            CarInfo carInfo = this.carInfoList[index];

            this.textBoxVehicleRegNo.Text = carInfo.VehicleRegNo;
            this.textBoxMake.Text = carInfo.Make;
            this.textBoxEngineSize.Text = carInfo.EngineSize;
            this.textBoxDateRegistered.Text = $"{carInfo.DateRegistered:yyyy-MM-dd}";
            this.textBoxRentalPerDay.Text = $"€{carInfo.RentalPerDay:n2}";
            if (carInfo.Available == 1)
            {
                this.checkBoxAvailable.Checked = true;

            }
            else
            {
                this.checkBoxAvailable.Checked = false;
            }

            this.textBoxNavigationStatus.Text = BuildNavigationStatus();
        }

        private void MakeDBConnection() //added 26/04
        {
            this.db = new DBConnection("localhost", "hire", "csharp", "password");
            this.db.Connect(); //added 30/04 pq comment otther part 
            //if (this.db.Connect() == false)30/04 comment
            //{
            //  MessageBox.Show("MySql Server is not connected.");
            //}
        }
        private void LoadCars()//added 26/04 
        {
            DBCars dbCars = new DBCars();

            MySqlDataReader reader = dbCars.GetCars(this.db.Connection);

            while (reader.Read())
            {
                //(0) (1) (2) (3) (4) (5) - are the indexes for columns in SQL - vehicleRegNo is the first colum so index(0)//
                string vehicleRegNo = reader.GetString(0);
                string make = reader.GetString(1);
                string engineSize = reader.GetString(2);
                DateTime dateRegistered = reader.GetDateTime(3);
                double rentalCost = reader.GetDouble(4);
                int available = reader.GetInt32(5);

                CarInfo carInfo = new CarInfo(vehicleRegNo, make, engineSize, dateRegistered, rentalCost, available);
                //ADD CAR TO LIST
                this.carInfoList.Add(carInfo);
            }
            reader.Close();
        }

        private bool ValidateVehicleRegNo(string vehicleRegNo)//Added 28/04
        {
            if (vehicleRegNo.Length > 0 && vehicleRegNo.Length <= 10)
            {
                return true;
            }
            MessageBox.Show("Vehicle Registration number must be greater than 0 characters, but less than 10", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }
        private bool ValidateMake(string make)
        {
            if (make.Length > 0 && make.Length <= 50)
            {
                return true;
            }
            MessageBox.Show("Make must be greater than 0 characters, but less than 50", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return false;
        }
        private bool ValidateEngineSize(string engineSize)
        {
            if (engineSize.Length > 0 && engineSize.Length <= 10)
            {
                return true;
            }
            MessageBox.Show("Engine Size must be greater than 0 characters, but less than 10", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private CarInfo CreateCarInfoFromForm() //added 27/04
        {
            string vehicleRegNo = this.textBoxVehicleRegNo.Text;
            if (ValidateVehicleRegNo(vehicleRegNo) == false)
            {
                return null;
            }

            string make = this.textBoxMake.Text;
            if (ValidateMake(make) == false)
            {
                return null;
            }
            string engineSize = this.textBoxEngineSize.Text;
            if (ValidateEngineSize(engineSize) == false)
            {
                return null;
            }

            DateTime dateRegisterd;

            if (DateTime.TryParse(this.textBoxDateRegistered.Text, out dateRegisterd) == false)
            {
                MessageBox.Show("Please enter a date in the format YYYY-MM-DD", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            //string rentalPerDaySubstring=this.textBoxRentalPerDay.Text.Substring(1); //Code added to take out £ (currency)
            //double rentalPerDay = Double.Parse(rentalPerDaySubstring);
            string rentalPerDaySubstring = "";
            if (this.textBoxRentalPerDay.Text.Length > 0)
            {
                rentalPerDaySubstring=this.textBoxRentalPerDay.Text.Substring(1);
            }

            double rentalPerDay;

             if (double.TryParse(rentalPerDaySubstring, out rentalPerDay) == false)
            {
                MessageBox.Show("Please enter a valid rental cost.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            int available = 0;
            if (this.checkBoxAvailable.Enabled)
            {
                available = 1;
            }
            CarInfo carInfo = new CarInfo(vehicleRegNo, make, engineSize, dateRegisterd, rentalPerDay, available);
            return carInfo;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            DBCars dbCars = new DBCars();

            string originalVehicleRegNo = this.carInfoList[this.currentIndex].VehicleRegNo;

            CarInfo carInfo = CreateCarInfoFromForm();
            if (carInfo == null)
            {
                return;
            }

            //updATE RECORD
            if (dbCars.Update(this.db.Connection, originalVehicleRegNo, carInfo) == true)
            {
                this.carInfoList[this.currentIndex] = carInfo;
                MessageBox.Show($"Vehicle Registration: {carInfo.VehicleRegNo} has been updated.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);//Added 28/04
            }
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            DBCars dbCars = new DBCars();

            CarInfo carInfo = CreateCarInfoFromForm();

            if (carInfo == null)
            {
                return;
            }
            //Insert Record in Database:
            if (dbCars.Add(this.db.Connection, carInfo) == true)
            {
                this.carInfoList.Add(carInfo);

                //update navigation controls: Move to the last element in list.
                //This element will be the most recent added.
                this.currentIndex = this.carInfoList.Count - 1;

                DisplayCarInfo(this.currentIndex);

                MessageBox.Show($"Vehicle Registration: {carInfo.VehicleRegNo} has been added.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);//Added 28/04
            }
        }


        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DBCars dbCars = new DBCars();

            string vehicleRegNo = this.carInfoList[this.currentIndex].VehicleRegNo;

            DialogResult result = MessageBox.Show($"You are about to delet VehicleRegNo: {vehicleRegNo}.\n\n Are you sure you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }

            if (dbCars.Delete(this.db.Connection, vehicleRegNo))
            {
                this.carInfoList.RemoveAt(this.currentIndex);

                // if we have removed the last element in the list, we need to move back one place to make sure we are still withing bounds.

                if (this.currentIndex == this.carInfoList.Count)
                {
                    this.currentIndex--;
                }
            }
            DisplayCarInfo(this.currentIndex);

        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (this.currentIndex + 1 < this.carInfoList.Count)
            {
                this.currentIndex++;
                DisplayCarInfo(currentIndex);
                //////////this.textBoxNavigationStatus.Text = BuildNavigationStatus(); //add it 
            }
            else
            {
                MessageBox.Show("No more records exist.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);//added 28/04
            }

        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            //add a check:
            if (this.currentIndex > 0)
            {
                this.currentIndex--;

                DisplayCarInfo(this.currentIndex);
            }
            else
            {
                MessageBox.Show("There are no early records.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void buttonFirst_Click(object sender, EventArgs e)
        {
            this.currentIndex = 0;

            DisplayCarInfo(this.currentIndex);

        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            this.currentIndex = this.carInfoList.Count - 1;

            DisplayCarInfo(this.currentIndex);
        }

        private string BuildNavigationStatus()
        {
            return $"{this.currentIndex + 1} of {this.carInfoList.Count}";

        }
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
            //Application.Exit(); //other way
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DisplayCarInfo(this.currentIndex);
        }

        
        private void FrmConnect_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.db != null)
            {
                this.db.Close();
            }
        }
        private string GetCarDescriptions()
        {
            string carDescriptions = "";
            foreach(CarInfo carInfo in this.carInfoList)
            {
                carDescriptions += carInfo.ToString();
                carDescriptions += "\n";

            }
            return carDescriptions;
        }
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            string carDescriptions = GetCarDescriptions();

            DocumentPrinter documentPrinter = new DocumentPrinter(carDescriptions, "Car Details");

            documentPrinter.Print();
        }
               
    }

}



    

