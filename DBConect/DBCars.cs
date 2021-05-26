using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DBConnect
{
    class DBCars
    {
        public MySqlDataReader GetCars(MySqlConnection connection)
        {
            string sql = "SELECT * FROM car;";

            MySqlCommand cmd = new MySqlCommand(sql, connection);

            MySqlDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Exception found: " + ex.Message);
            }

            return reader;
        }

        //public void Update(MySqlConnection connection, string vehicleRegNo, string make, string engineSize, DateTime dateRegistered, double rentalCost, int available)
        
        
        public bool Update(MySqlConnection connection, string originalVehicleRegNo, CarInfo carInfo)
        {
            string sql = "UPDATE car SET VehicleRegNo = @vehicleRegNo, Make = @make, EngineSize = @engineSize, DateRegistered = @dateRegistered, RentalPerDay = @rentalPerDay, Available = @available WHERE VehicleRegNo = @originalVehicleRegNo;";

            MySqlCommand cmd = new MySqlCommand(sql, connection);

            
            cmd.Parameters.AddWithValue("@originalVehicleRegNo", originalVehicleRegNo);
            cmd.Parameters.AddWithValue("@vehicleRegNo", carInfo.VehicleRegNo);
            cmd.Parameters.AddWithValue("@make", carInfo.Make);
            cmd.Parameters.AddWithValue("@engineSize", carInfo.EngineSize);
            cmd.Parameters.AddWithValue("@dateRegistered", carInfo.DateRegistered);
            cmd.Parameters.AddWithValue("@rentalPerDay", carInfo.RentalPerDay);
            cmd.Parameters.AddWithValue("@available", carInfo.Available);

            try
            {
                cmd.Prepare();

                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Exception found: " + ex.Message);
                return false;
            }
            return true;

        }   

        public bool Add(MySqlConnection connection,  CarInfo carInfo)
        {
            string sql = "INSERT INTO car VALUES (@vehicleRegNo, @make, @engineSize, @dateRegistered, @rentalPerDay, @available);";
            MySqlCommand cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@vehicleRegNo", carInfo.VehicleRegNo);
            cmd.Parameters.AddWithValue("@make", carInfo.Make);
            cmd.Parameters.AddWithValue("@engineSize", carInfo.EngineSize);
            cmd.Parameters.AddWithValue("@dateRegistered", carInfo.DateRegistered);
            cmd.Parameters.AddWithValue("@rentalPerDay", carInfo.RentalPerDay);
            cmd.Parameters.AddWithValue("@available", carInfo.Available);

            try
            {
                cmd.Prepare();

                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Exception found: " + ex.Message);
                return false;
            }
            return true;
        }

        

        public bool Delete(MySqlConnection connection, string vehicleRegNo)
        {                       
            string sql = "DELETE FROM car WHERE vehicleregno = @vehicleregno;";
            
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@vehicleRegNo", vehicleRegNo);

            try
            {
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Exception found: " + ex.Message);
                return false;
            }
            return true;
        }
    }
}




