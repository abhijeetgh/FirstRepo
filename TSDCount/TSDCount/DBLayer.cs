using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace TSDCount
{
    public class DBLayer
    {
        public List<TSDReservationDTO> GetReservationCount(string strStartDate, string strEndDate, string strBrandLocation)
        {
            MyClassName_List lstReservations = new MyClassName_List();
            string connetionString = null;
            SqlConnection connection;
            SqlCommand command;
            string sql = null;
            SqlDataReader dataReader;
            connetionString = ConfigurationManager.ConnectionStrings["DB"].ToString();
            sql = "SELECT Reservationcount, Date FROM TempTSDReservation WHERE CONVERT(DATE,[Date]) BETWEEN CONVERT(DATE," + strStartDate + ") AND CONVERT(DATE," + strEndDate + ") AND Location = " + strBrandLocation;
            connection = new SqlConnection(connetionString);
            try
            {
                
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                TSDReservationDTO oTSDReservationDTO = null;
                while (dataReader.Read())
                {
                    oTSDReservationDTO = new TSDReservationDTO();
                    oTSDReservationDTO.DayInMonth = Convert.ToDateTime(dataReader["Date"]).ToString("MM/dd/yyyy");
                    oTSDReservationDTO.ReservationCount = Convert.ToInt64(dataReader["Reservationcount"]);

                    lstReservations.Add(oTSDReservationDTO);
                    oTSDReservationDTO = null;
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            finally
            {
                connection.Close();
            }

            return lstReservations;
        }
    }
}