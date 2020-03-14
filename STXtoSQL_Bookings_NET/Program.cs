using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STXtoSQL.DataAccess;
using STXtoSQL.Models;
using STXtoSQL.Log;

namespace STXtoSQL_Bookings_NET
{
    class Program
    {
        static void Main(string[] args)
        {          
            // Args will change based on STXtoSQL program goal
            string date1 = "";
            string date2 = "";
            int odbcCnt = 0;
            int insertCnt = 0;
            int importCnt = 0;
        
            try
            {
                if (args.Length > 0)
                {
                    /*
                     * Must be in format mm/dd/yyyy.  No time part
                     */
                    date1 = args[0].ToString();
                    date2 = args[1].ToString();
                }
                else
                {
                    // No args = current month to yesterday
                    DateTime dtToday = DateTime.Today;

                    DateTime dtFirst = new DateTime(dtToday.Year, dtToday.Month, 1);

                    /*
                     * Need one date part of datetime.
                     * Time and date are separated by a space, so split the string
                     * and only use the 1st element.
                     */
                    string[] date1Split = dtFirst.ToString().Split(' ');
                    string[] date2Split = dtToday.AddDays(-1).ToString().Split(' ');

                    date1 = date1Split[0];
                    date2 = date2Split[0];
                }
            }
            catch (Exception ex)
            {               
                Logger.LogWrite("EXC", ex);
                Logger.LogWrite("MSG", "Return");
                return;
                //Console.WriteLine(ex.Message.ToString());
            }

            #region FromSTRATIX
            ODBCData objODBC = new ODBCData();

            List<Bookings> lstBookings = new List<Bookings>();

            try
            {
                lstBookings = objODBC.Get_Bookings(date1, date2);
            }
            catch (Exception ex)
            {
                Logger.LogWrite("EXC", ex);
                Logger.LogWrite("MSG", "Return");
                return;
                //Console.WriteLine(ex.Message.ToString());
            }
            #endregion

            #region ToSQL
            SQLData objSQL = new SQLData();

            // Only work in SQL database, if records were retreived from Stratix
            if (lstBookings.Count != 0)
            {
                odbcCnt = lstBookings.Count;

                // Put lstBookings into IMPORT Bookings table
                try
                {
                    importCnt = objSQL.Write_Bookings_IMPORT(lstBookings);
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    return;
                    //Console.WriteLine(ex.Message.ToString());
                }

                // Call SP to put IMPORT Bookings into Bookings table
                try
                {
                    insertCnt = objSQL.Write_IMPORT_to_Bookings(date1, date2);
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    return;
                    //Console.WriteLine(ex.Message.ToString());
                }

                Logger.LogWrite("MSG", "Range=" + date1 + ":" + date2 + " ODBC/IMPORT/INSERT=" + odbcCnt.ToString() + ":" + importCnt.ToString() + ":" + insertCnt.ToString());
            }
            else
                Logger.LogWrite("MSG", "No data");

            #endregion

            // Testing
            //Console.WriteLine("Press key to exit");
            //Console.ReadKey();
        }
    }
}
