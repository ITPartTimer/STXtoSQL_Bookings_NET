using System;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Text;
using STXtoSQL;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    public class ODBCData : Helpers
    {
        public List<Bookings> Get_Bookings(string date1, string date2)
        {

            List<Bookings> lstBookings = new List<Bookings>();

            OdbcConnection conn = new OdbcConnection(ODBCDataConnString);         

            try
            {
                conn.Open();

                // Try to split with verbatim literal
                OdbcCommand cmd = conn.CreateCommand();

                cmd.CommandText = @"select bsb_brh,SUBSTRING(bsb_pep,1,2) as pep,bsb_s_tot_wgt+bsb_a_tot_wgt as wgt,bsb_s_tot_sls+bsb_a_tot_sls as sls,bsb_actvy_dt,
	                                DATE_PART('month',bsb_actvy_dt) as mn,DATE_PART('day',bsb_actvy_dt) as dy,DATE_PART('year',bsb_actvy_dt) as yr
	                                from orsbsb_rec 
	                                where bsb_ord_pfx = 'SO' and bsb_brh in ('SW','MS','CS','AR') and bsb_actvy_dt >= '" + date1 + "' and bsb_actvy_dt <= '" + date2 + "'";       

                OdbcDataReader rdr = cmd.ExecuteReader();

                using (rdr)
                {
                    while (rdr.Read())
                    {
                        Bookings b = new Bookings();

                        b.brh = rdr["bsb_brh"].ToString();
                        b.pep = rdr["pep"].ToString();
                        b.wgt = rdr["wgt"].ToString();
                        b.sls = rdr["sls"].ToString();
                        b.actvy_dt = rdr["bsb_actvy_dt"].ToString();
                        b.mn = Convert.ToInt32(rdr["mn"]);
                        b.dy = Convert.ToInt32(rdr["dy"]);
                        b.yr = Convert.ToInt32(rdr["yr"]);

                        lstBookings.Add(b);
                    }
                }
            }
            catch (OdbcException)
            {
                throw;
                //Console.WriteLine("MultDetail odbc ex: " + ex.Message);
            }
            catch (Exception)
            {
                throw;
                //Console.WriteLine("MultDetail other ex: " + ex.Message);
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return lstBookings;
        }
    }
}
