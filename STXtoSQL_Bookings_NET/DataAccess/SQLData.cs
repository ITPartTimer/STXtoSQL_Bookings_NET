using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STXtoSQL;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    public class SQLData : Helpers
    {
        public int Write_Bookings_TMP(List<Bookings> lstBookings)
        {
            // Returning rows inserted into TMP
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                SqlCommand cmd = new SqlCommand();

                cmd.Transaction = trans;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                // First empty TMP table
                try
                {
                    cmd.CommandText = "DELETE from SCORE_TMP_tbl_Bookings";

                    cmd.ExecuteNonQuery();
                }
                catch(Exception)
                {
                    throw;
                }

                try
                {
                    // Change Text to Insert data into TMP
                    cmd.CommandText = "INSERT INTO SCORE_TMP_tbl_Bookings (bsb_brh,bsb_pep,wgt,sls,bsb_actvy_dt,actvy_mn,actvy_dy,actvy_yr) " +
                                        "VALUES (@brh,@pep,@wgt,@sls,@actvy_dt,@actvy_mn,@actvy_dy,@actvy_yr)";

                    cmd.Parameters.Add("@brh", SqlDbType.VarChar);
                    cmd.Parameters.Add("@pep", SqlDbType.VarChar);
                    cmd.Parameters.Add("@wgt", SqlDbType.Decimal);
                    cmd.Parameters.Add("@sls", SqlDbType.Decimal);
                    cmd.Parameters.Add("@inv_dt", SqlDbType.DateTime);
                    cmd.Parameters.Add("@actvy_mn", SqlDbType.Int);
                    cmd.Parameters.Add("@actvy_dy", SqlDbType.Int);
                    cmd.Parameters.Add("@actvy_yr", SqlDbType.Int);

                    foreach (Bookings s in lstBookings)
                    {
                        cmd.Parameters[1].Value = s.brh;
                        cmd.Parameters[2].Value = s.pep;
                        cmd.Parameters[3].Value = Convert.ToDecimal(s.wgt);
                        cmd.Parameters[4].Value = Convert.ToDecimal(s.sls);
                        cmd.Parameters[6].Value = Convert.ToDateTime(s.actvy_dt);
                        cmd.Parameters[7].Value = s.mn;
                        cmd.Parameters[8].Value = s.dy;
                        cmd.Parameters[9].Value = s.yr;

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch (Exception)
                {
                    // Shouldn't lave a Transaction hanging, so rollback
                    trans.Rollback();
                    throw;
                }
                try
                {
                    // Get count of rows inserted into TMP
                    cmd.CommandText = "SELECT COUNT(brh) from SCORE_TMP_tbl_Bookings";
                    r = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch(Exception)
                {
                    throw;
                }
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        }

        public int Write_TMP_to_Bookings()
        {
            // Returning rows inserted into TMP
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                // Call SP to copy TMP to Bookings table.  Return rows inserted.
                cmd.CommandText ="SCORE_proc_TMP_to_Bookings";

                AddParamToSQLCmd(cmd, "@rows", SqlDbType.Int, 8, ParameterDirection.Output);

                cmd.ExecuteNonQuery();

                r = Convert.ToInt32(cmd.Parameters["@rows"].Value);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        }

    }
}
