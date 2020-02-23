using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STXtoSQL.DataAccess;
using STXtoSQL.Models;

namespace STXtoSQL_Bookings_NET
{
    class Program
    {
        static void Main(string[] args)
        {
            // Args will change based on STXtoSQL program goal
            //List<string> lstArgs = new List<string>();

            string date1 = "";
            string date2 = "";

            try
            {
                if (args.Length > 0)
                {
                    // args = date range
                    date1 = args[0].ToString();
                    date2 = args[1].ToString();
                }
                else
                {
                    // No args = yesterday
                    // Yesterday = -1.  testing = -2 or more
                    DateTime dtYst = DateTime.Now.AddDays(-3);
                    date1 = dtYst.Month.ToString() + "/" + dtYst.Day.ToString() + "/" + dtYst.Year.ToString();
                    date2 = date1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            // Testing
            Console.WriteLine(date1 + " / " + date2);

            #region FromSTRATIX
            ODBCData objODBC = new ODBCData();

            List<Bookings> lstBookings = new List<Bookings>();

            try
            {
                lstBookings = objODBC.Get_Bookings(date1, date2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            // Testing
            Console.WriteLine("Retrieve records: " + lstBookings.Count.ToString());

            foreach (Bookings b in lstBookings)
            {
                Console.WriteLine(b.brh + " / " + b.pep + " / " + b.wgt + " / " + b.sls + " / " + b.actvy_dt + " / " + b.mn.ToString() + " / " + b.dy.ToString() + " / " + b.yr.ToString());
            }
            // Testings.  I just want to see query results and not insert into SQL
            lstBookings.Clear();

            #endregion

            #region ToSQL
            int rowCnt = 0;

            SQLData objSQL = new SQLData();

            // Only work in SQL database, if records were retreived from Stratix
            if (lstBookings.Count != 0)
            {
                // Put lstBookings into TMP Bookings table
                try
                {
                    rowCnt = objSQL.Write_Bookings_TMP(lstBookings);
                    Console.WriteLine("TMP inserted: " + rowCnt.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }

                // Call SP to put TMP Bookings into Bookings table
                try
                {
                    rowCnt = objSQL.Write_TMP_to_Bookings();
                    Console.WriteLine("Bookings inserted: " + rowCnt.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }


            #endregion

            // Testing
            Console.WriteLine("Press key to exit");
            Console.ReadKey();
        }
    }
}
