using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace Inspinia_MVC5.Dapper
{
    public class DapperORM
    {
        
            private static string connectionString = ConfigurationManager.ConnectionStrings["CONEXAO_TASK"].ToString();


            public static void ExecuteWithouReturn(string procedureName, DynamicParameters param = null)
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    sqlCon.Execute(procedureName, param, commandType: CommandType.StoredProcedure);

                }
            }


            public static T ExecuteReturnScalar<T>(string procedureName, DynamicParameters param = null)
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    return (T)Convert.ChangeType(sqlCon.Execute(procedureName, param, commandType: CommandType.StoredProcedure), typeof(T));

                }
            }

            public static IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null)
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    return sqlCon.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);

                }
            }
        }
  
}