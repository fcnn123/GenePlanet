using GP_Dal.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP_Dal
{
    public class Dal
    {
        public enum AggMode
        {
            all,
            min,
            max
        }

        public AggData GetAggregateData(AggMode mode, string username)
        {
            AggData aggData = null;
            using (SqlConnection conn = new SqlConnection(GP_Dal.Properties.Settings.Default.GPconnString))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandText = @"SELECT [id]
                                                  ,[agg_username]
                                                  ,[agg_min]
                                                  ,[agg_max]
                                                  ,[last_sync_date]
                                              FROM [MetricSync]
                                            WHERE [agg_username] = @agg_username";
                    comm.Parameters.AddWithValue("@agg_username", username);
                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            aggData = new AggData
                            {
                                UserName = reader["agg_username"].ToString()
                            };
                            if (DateTime.TryParse(reader["last_sync_date"].ToString(), out DateTime d))
                                aggData.LastSync = d;
                            switch (mode)
                            {
                                case AggMode.min:
                                    float.TryParse(reader["agg_min"].ToString(), out float fMin);
                                    aggData.Min = fMin;
                                    break;
                                case AggMode.max:
                                    float.TryParse(reader["agg_max"].ToString(), out float fMax);
                                    aggData.Max = fMax;
                                    break;
                                default:
                                    float.TryParse(reader["agg_max"].ToString(), out float fM);
                                    aggData.Max = fM;
                                    float.TryParse(reader["agg_min"].ToString(), out float fm);
                                    aggData.Min = fm;
                                    break;
                            }
                        }
                    }
                }
            }

            return aggData;
        }

        public void SaveAggregateData(AggData aggData)
        {
            using (SqlConnection conn = new SqlConnection(GP_Dal.Properties.Settings.Default.GPconnString))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandText = "select id from MetricSync where agg_username = @agg_username";
                    comm.Parameters.AddWithValue("@agg_username", aggData.UserName);
                    var idTemp = comm.ExecuteScalar();

                    comm.Parameters.Clear();
                    if (idTemp != null)
                    {
                        comm.CommandText = @"UPDATE [dbo].[MetricSync]
                                               SET [agg_username] = @agg_username
                                                  ,[agg_min] = @agg_min
                                                  ,[agg_max] = @agg_max
                                                  ,[last_sync_date] = @last_sync_date
                                             WHERE id = @id";
                        comm.Parameters.AddWithValue("@id", Convert.ToInt32(idTemp));
                        comm.Parameters.AddWithValue("@agg_username", aggData.UserName);
                        comm.Parameters.AddWithValue("@agg_min", aggData.Min.HasValue ? (Object)aggData.Min : DBNull.Value);
                        comm.Parameters.AddWithValue("@agg_max", aggData.Max.HasValue ? (Object)aggData.Max : DBNull.Value);
                        comm.Parameters.AddWithValue("@last_sync_date", aggData.LastSync);
                        comm.ExecuteNonQuery();
                    }
                    else
                    {
                        comm.CommandText = @"INSERT INTO [dbo].[MetricSync]
                                                   ([agg_username]
                                                   ,[agg_min]
                                                   ,[agg_max]
                                                   ,[last_sync_date])
                                             VALUES
                                                   (@agg_username
                                                   ,@agg_min
                                                   ,@agg_max
                                                   ,@last_sync_date)";
                        comm.Parameters.AddWithValue("@agg_username", aggData.UserName);
                        comm.Parameters.AddWithValue("@agg_min", aggData.Min.HasValue ? (Object)aggData.Min : DBNull.Value);
                        comm.Parameters.AddWithValue("@agg_max", aggData.Max.HasValue ? (Object)aggData.Max : DBNull.Value);
                        comm.Parameters.AddWithValue("@last_sync_date", aggData.LastSync);
                        comm.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
