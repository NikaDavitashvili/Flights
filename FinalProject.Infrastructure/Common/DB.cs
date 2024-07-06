using FinalProject.Domain.Common;
using System.Data;
using System.Data.SqlClient;

namespace FinalProject.Infrastructure.Common;
public class DB
{
    public static DataTable Select(string query, Dictionary<string, object> Parameters, out string errorMessage)
    {
        try
        {
            errorMessage = null;

            if (string.IsNullOrEmpty(Configuration.ConnectionStrings.Flights))
                throw new Exception("Database Connection Not Found!");

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionStrings.Flights))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                    if (Parameters != null && Parameters.Count > 0)
                        foreach (var item in Parameters.Keys)
                        {
                            if (Parameters[item] == null)
                                cmd.Parameters.AddWithValue($"@{item}", Parameters[item]).Value = DBNull.Value;
                            else
                                cmd.Parameters.AddWithValue($"@{item}", Parameters[item]);
                        }

                    cmd.Connection = connection;
                    connection.Open();

                    using SqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
            }

            return dt;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return null;
        }
    }

    public static Dictionary<int, string> Run(string query, Dictionary<string, object> Parameters)
    {
        Dictionary<int, string> result = new Dictionary<int, string>();
        try
        {
            if (string.IsNullOrEmpty(Configuration.ConnectionStrings.Flights))
                throw new Exception("Database Connection Not Found!");

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionStrings.Flights))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    if (Parameters != null && Parameters.Count > 0)
                    {
                        foreach (var item in Parameters.Keys)
                        {
                            if (Parameters[item] is byte[])
                                cmd.Parameters.AddWithValue($"@{item}", Parameters[item]);
                            else
                            {
                                if (Parameters[item] == null)
                                    cmd.Parameters.AddWithValue($"@{item}", Parameters[item]?.ToString()).Value = DBNull.Value;
                                else
                                    cmd.Parameters.AddWithValue($"@{item}", Parameters[item]?.ToString());
                            }
                        }
                    }
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    connection.Open();
                    var resultCode = cmd.ExecuteNonQuery();

                    result[resultCode] = "Process completed successfully!";
                    return result;
                }
            }
        }
        catch (Exception ex)
        {
            int resultCode = -1;
            result[resultCode] = ex.Message;
            return result;
        }
    }
}
