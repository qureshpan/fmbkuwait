using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Threading;
using System.Xml;


namespace FMB_Kuwait.Models
{
    public class DB
    {
        private static FixedSizedQueue<DBLogger> DBLog = new FixedSizedQueue<DBLogger>(1000);
        public DB()
        {

        }
        public static string filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "Sp_logs.txt"; //----Log file path------//
        private static string _activeDBConn = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();// CommonLogic.ConnectionString("DefaultConnection");
        private static bool IsPasswordProtected = Parser.ParseBoolean(CommonLogic.Application("IsPasswordProtected").ToString());
        private static string key = CommonLogic.Application("SymmetricKey");

        public static string GetDBConn(bool readOnly)
        {
            if (IsPasswordProtected)
            {
                System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder(_activeDBConn);
                csb.Password = CommonLogic.DecryptString(key, csb.Password);
                _activeDBConn = csb.ToString();
            }
            return readOnly ? _activeDBConn + ";ApplicationIntent=ReadOnly" : _activeDBConn;
        }







        public static SqlConnection dbConn(bool readOnly)
        {
            return new SqlConnection(DB.GetDBConn(readOnly));
        }

        public static int ExecuteStoredProc(string StoredProcName)
        {
            return ExecuteStoredProc(StoredProcName, null);
        }
        public static async Task<int> ExecuteStoredProcAsync(string StoredProcName)
        {
            return await ExecuteStoredProcAsync(StoredProcName, null);
        }

        public static int ExecuteStoredProc(string StoredProcName, SqlParameter[] paramColl, bool readOnly = false)
        {
            int ret = -1; string Exec = ""; StringBuilder sbLog = new StringBuilder();
            using (SqlConnection con = dbConn(readOnly))
            {
                using (SqlCommand dbCommand = new SqlCommand(StoredProcName, con))
                {
                    dbCommand.CommandType = CommandType.StoredProcedure;
                    if (paramColl != null)
                    {
                        sbLog.AppendLine("Parameters of " + StoredProcName + " ->");
                        if (paramColl != null)
                        {
                            foreach (SqlParameter sp in paramColl)
                            {
                                dbCommand.Parameters.Add(sp);
                                sbLog.AppendLine(sp.ParameterName + " - " + Convert.ToString(sp.Value));
                            }
                        }
                    }
                    DateTime dt1 = DateTime.Now;
                    try
                    {
                        con.Open();
                        ret = dbCommand.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        //Files.Log(LogType.SQLException, sbLog.ToString(), ex);
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                    int tt = (int)DateTime.Now.Subtract(dt1).TotalMilliseconds;
                    if (tt > 100)
                    {
                        sbLog.AppendLine("Time taken by " + StoredProcName + "-[" + tt.ToString() + "]");
                        //Files.Log(LogType.Warning, sbLog.ToString());
                    }
                    return ret;
                }
            }
        }

        public static async Task<int> ExecuteStoredProcAsync(string StoredProcName, SqlParameter[] paramColl, bool readOnly = false)
        {
            int ret = -1; StringBuilder sbLog = new StringBuilder();
            using (SqlConnection con = dbConn(readOnly))
            {
                using (SqlCommand dbCommand = new SqlCommand(StoredProcName, con))
                {
                    dbCommand.CommandType = CommandType.StoredProcedure;
                    if (paramColl != null)
                    {
                        sbLog.AppendLine("Parameters of " + StoredProcName + " ->");
                        if (paramColl != null)
                        {
                            foreach (SqlParameter sp in paramColl)
                            {
                                dbCommand.Parameters.Add(sp);
                                sbLog.AppendLine(sp.ParameterName + " - " + Convert.ToString(sp.Value));
                            }
                        }
                    }
                    DateTime dt1 = DateTime.Now;
                    try
                    {
                        await con.OpenAsync();
                        ret = await dbCommand.ExecuteNonQueryAsync();
                    }
                    catch (SqlException ex)
                    {
                        //Files.Log(LogType.SQLException, sbLog.ToString(), ex);
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                    int tt = (int)DateTime.Now.Subtract(dt1).TotalMilliseconds;
                    if (tt > 100)
                    {
                        sbLog.AppendLine("Time taken by " + StoredProcName + "-[" + tt.ToString() + "]");
                        //Files.Log(LogType.Warning, sbLog.ToString());
                    }
                    return ret;
                }
            }
        }

        public static object ExecuteStoredProcScaler(string StoredProcName, SqlParameter[] paramColl, bool readOnly = false)
        {
            object ret = null; StringBuilder sbLog = new StringBuilder();
            using (SqlConnection con = dbConn(readOnly))
            {
                using (SqlCommand dbCommand = new SqlCommand(StoredProcName, con))
                {
                    dbCommand.CommandType = CommandType.StoredProcedure;
                    if (paramColl != null)
                    {
                        sbLog.AppendLine("Parameters of " + StoredProcName + " ->");
                        foreach (SqlParameter sp in paramColl)
                        {
                            dbCommand.Parameters.Add(sp);
                            sbLog.AppendLine(sp.ParameterName + " - " + Convert.ToString(sp.Value));
                        }
                    }
                    DateTime dt1 = DateTime.Now;
                    try
                    {
                        con.Open();
                        ret = dbCommand.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {
                        //Files.Log(LogType.SQLException, sbLog.ToString(), ex);
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                    int tt = (int)DateTime.Now.Subtract(dt1).TotalMilliseconds;
                    if (tt > 100)
                    {
                        sbLog.AppendLine("Time taken by " + StoredProcName + "-[" + tt.ToString() + "]");
                        //Files.Log(LogType.Warning, sbLog.ToString());
                    }
                    return ret;
                }
            }
        }

        public static async Task<object> ExecuteStoredProcScalerAsync(string StoredProcName, SqlParameter[] paramColl, bool readOnly = false)
        {
            object ret = null; StringBuilder sbLog = new StringBuilder();
            using (SqlConnection con = dbConn(readOnly))
            {
                using (SqlCommand dbCommand = new SqlCommand(StoredProcName, con))
                {
                    dbCommand.CommandType = CommandType.StoredProcedure;
                    if (paramColl != null)
                    {
                        sbLog.AppendLine("Parameters of " + StoredProcName + " ->");
                        foreach (SqlParameter sp in paramColl)
                        {
                            dbCommand.Parameters.Add(sp);
                            sbLog.AppendLine(sp.ParameterName + " - " + Convert.ToString(sp.Value));
                        }
                    }
                    DateTime dt1 = DateTime.Now;
                    try
                    {
                        await con.OpenAsync();
                        ret = await dbCommand.ExecuteScalarAsync();
                    }
                    catch (SqlException ex)
                    {
                        //Files.Log(LogType.SQLException, sbLog.ToString(), ex);
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                    int tt = (int)DateTime.Now.Subtract(dt1).TotalMilliseconds;
                    if (tt > 100)
                    {
                        sbLog.AppendLine("Time taken by " + StoredProcName + "-[" + tt.ToString() + "]");
                        //Files.Log(LogType.Warning, sbLog.ToString());
                    }
                    return ret;
                }
            }
        }

        public static DataSet ExecuteStoredProcDataSet(string StoredProcName)
        {
            return ExecuteStoredProcDataSet(StoredProcName, null);
        }
        public static async Task<DataSet> ExecuteStoredProcDataSetAsync(string StoredProcName)
        {
            return await ExecuteStoredProcDataSetAsync(StoredProcName, null);
        }

        public static DataSet ExecuteStoredProcDataSet(string StoredProcName, SqlParameter[] paramColl, bool readOnly = false)
        {
            StringBuilder sbLog = new StringBuilder();
            DateTime dt1 = DateTime.Now;
            using (SqlConnection con = dbConn(readOnly))
            {
                using (SqlCommand dbCommand = new SqlCommand(StoredProcName, con))
                {
                    dbCommand.CommandType = CommandType.StoredProcedure;

                    if (paramColl != null)
                    {
                        sbLog.AppendLine("Parameters of " + StoredProcName + " ->");
                        foreach (SqlParameter sp in paramColl)
                        {
                            dbCommand.Parameters.Add(sp);
                            sbLog.AppendLine(sp.ParameterName + " - " + Convert.ToString(sp.Value));
                        }
                    }
                    SqlDataAdapter da = new SqlDataAdapter(dbCommand);
                    DataSet ds = new DataSet();
                    try
                    {
                        dt1 = DateTime.Now;
                        con.Open();
                        da.Fill(ds);
                    }
                    catch (SqlException ex)
                    {
                        //Files.Log(LogType.SQLException, sbLog.ToString(), ex);
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                    TimeSpan ts = DateTime.Now.Subtract(dt1);
                    if (ts.TotalMilliseconds > 100 && (!StoredProcName.Equals("tlb_ExceptionLogs_Save")))
                    {
                        sbLog.AppendLine("Time taken by " + StoredProcName + "-[" + ts.TotalMilliseconds.ToString() + "]");
                        //Files.Log(LogType.Warning, sbLog.ToString());
                    }
                    return ds;
                }

            }
        }

        public static async Task<DataSet> ExecuteStoredProcDataSetAsync(string StoredProcName, SqlParameter[] paramColl, bool readOnly = false)
        {

            StringBuilder sbLog = new StringBuilder();
            DateTime dt1 = DateTime.Now;
            using (SqlConnection con = dbConn(readOnly))
            {
                using (SqlCommand dbCommand = new SqlCommand(StoredProcName, con))
                {
                    dbCommand.CommandType = CommandType.StoredProcedure;
                    if (paramColl != null)
                    {
                        sbLog.AppendLine("Parameters of " + StoredProcName + " ->");
                        foreach (SqlParameter sp in paramColl)
                        {
                            dbCommand.Parameters.Add(sp);
                            sbLog.AppendLine(sp.ParameterName + " - " + Convert.ToString(sp.Value));
                        }
                    }
                    DataSet ds = new DataSet();
                    try
                    {
                        await con.OpenAsync();
                        ds = await dbCommand.GetDataSetAsync(CancellationToken.None);
                    }
                    catch (SqlException ex)
                    {
                        //Log.LogNowAsync(LogType.Error, ex: ex);
                        //Files.Log(LogType.SQLException, sbLog.ToString(), ex);
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                    TimeSpan ts = DateTime.Now.Subtract(dt1);
                    if (ts.TotalMilliseconds > 100)
                    {
                        sbLog.AppendLine("Time taken by " + StoredProcName + "-[" + ts.TotalMilliseconds.ToString() + "]");
                        //Files.Log(LogType.Warning, sbLog.ToString());
                    }

                    return ds;
                }
            }
        }


        public static async Task<String> ExecuteStoredProcDataSetXMLAsync(string StoredProcName, SqlParameter[] paramColl, bool readOnly = false)
        {

            StringBuilder sbLog = new StringBuilder();
            DateTime dt1 = DateTime.Now;
            string ds = "";
            using (SqlConnection con = dbConn(readOnly))
            {
                using (SqlCommand dbCommand = new SqlCommand(StoredProcName, con))
                {
                    dbCommand.CommandType = CommandType.StoredProcedure;
                    if (paramColl != null)
                    {
                        sbLog.AppendLine("Parameters of " + StoredProcName + " ->");
                        foreach (SqlParameter sp in paramColl)
                        {
                            dbCommand.Parameters.Add(sp);
                            sbLog.AppendLine(sp.ParameterName + " - " + Convert.ToString(sp.Value));
                        }
                    }

                    try
                    {
                        await con.OpenAsync();
                        ds = await dbCommand.GetDataSetXMLAsync(CancellationToken.None);
                    }
                    catch (SqlException ex)
                    {
                        //Log.LogNowAsync(LogType.Error, ex: ex);
                        //Files.Log(LogType.SQLException, sbLog.ToString(), ex);
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                    TimeSpan ts = DateTime.Now.Subtract(dt1);
                    if (ts.TotalMilliseconds > 100)
                    {
                        sbLog.AppendLine("Time taken by " + StoredProcName + "-[" + ts.TotalMilliseconds.ToString() + "]");
                        //Files.Log(LogType.Warning, sbLog.ToString());
                    }

                    return ds;
                }
            }
        }

        public static SqlParameter SetValueDecimal(SqlParameter sparam, object value)
        {
            if (value == null)
            {
                sparam.Value = DBNull.Value;
            }
            else
            {
                sparam.Value = Convert.ToDecimal(value.ToString());
            }

            return sparam;
        }

        public static SqlParameter SetValueBool(SqlParameter sparam, object value)
        {
            if (value == null)
            {
                sparam.Value = DBNull.Value;
            }
            else
            {
                sparam.Value = Convert.ToBoolean(value.ToString());
            }

            return sparam;
        }
        public static SqlParameter SetValueSmallInt(SqlParameter sparam, object value)
        {
            if (value == null)
            {
                sparam.Value = DBNull.Value;
            }
            else
            {
                sparam.Value = Convert.ToInt16(value.ToString());
            }

            return sparam;
        }
        public static SqlParameter SetValueTinyInt(SqlParameter sparam, object value)
        {
            if (value == null)
            {
                sparam.Value = DBNull.Value;
            }
            else
            {
                sparam.Value = Convert.ToByte(Convert.ToInt32(value));
            }

            return sparam;
        }
        public static SqlParameter SetValueInt(SqlParameter sparam, object value)
        {
            if (value == null)
            {
                sparam.Value = DBNull.Value;
            }
            else
            {
                sparam.Value = Convert.ToInt32(value.ToString());
            }

            return sparam;
        }
        public static SqlParameter SetValueBigInt(SqlParameter sparam, object value)
        {
            if (value == null)
            {
                sparam.Value = DBNull.Value;
            }
            else
            {
                sparam.Value = Convert.ToInt64(value.ToString());
            }

            return sparam;
        }
        public static SqlParameter SetValueDateTime(SqlParameter sparam, object value)
        {
            if (value == null)
            {
                sparam.Value = DBNull.Value;
            }
            else
            {
                sparam.Value = DateTime.Parse(value.ToString());
            }
            return sparam;
        }
        public static SqlParameter SetValueGUID(SqlParameter sparam, object value)
        {
            if (value == null)
            {
                sparam.Value = DBNull.Value;
            }
            else
            {
                sparam.Value = (Guid)value;
            }
            return sparam;
        }
        public static SqlParameter SetValue(SqlParameter sparam, object value)
        {
            if (value == null)
            {
                sparam.Value = DBNull.Value;
            }
            else
            {
                sparam.Value = value;
            }
            return sparam;
        }

        public static SqlParameter CreateSQLParameter(string ParameterName, SqlDbType ParamterType, object Value)
        {
            return CreateSQLParameter(ParameterName, ParamterType, ParameterDirection.Input, Value);
        }

        public static SqlParameter CreateSQLParameter(string ParameterName, SqlDbType ParamterType, ParameterDirection ParameterDirection, object Value, int size = 0)
        {
            SqlParameter sq = new SqlParameter(ParameterName, ParamterType);
            sq.Direction = ParameterDirection;
            if (size > 0)
                sq.Size = size;

            switch (ParamterType)
            {
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.Float:
                case SqlDbType.Real:
                case SqlDbType.SmallMoney:
                    sq = SetValueDecimal(sq, Value);
                    break;
                case SqlDbType.Int:
                    sq = SetValueInt(sq, Value);
                    break;
                case SqlDbType.BigInt:
                    sq = SetValueBigInt(sq, Value);
                    break;
                case SqlDbType.TinyInt:
                    sq = SetValueTinyInt(sq, Value);
                    break;
                case SqlDbType.NVarChar:
                case SqlDbType.NChar:
                    sq = SetValue(sq, Value);
                    break;
                case SqlDbType.VarChar:
                case SqlDbType.Char:
                    sq = SetValue(sq, Value);
                    break;
                case SqlDbType.NText:
                case SqlDbType.Text:
                    sq = SetValue(sq, Value);
                    break;
                case SqlDbType.Xml:
                    sq = SetValue(sq, Value);
                    break;
                case SqlDbType.Bit:
                    sq = SetValueBool(sq, Value);
                    break;
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                    sq = SetValueDateTime(sq, Value);
                    break;
                case SqlDbType.SmallInt:
                    sq = SetValueSmallInt(sq, Value);
                    break;
                case SqlDbType.UniqueIdentifier:
                    sq = SetValueGUID(sq, Value);
                    break;
            }
            return sq;
        }

        /*public static SqlParameter[] CreateSQLParameterArray_Obselete(SqlParameter[] spa, SqlParameter sp)
        {
            Array.Resize(ref spa, spa.Length + 1);
            spa[spa.Length - 1] = sp;

            return spa;
        }*/

        public static string GetNewGUID()
        {
            return Guid.NewGuid().ToString();
        }


        private static void log_file(string message)
        {
            /* var sw = new System.IO.StreamWriter(filename, true);
             //  sw.WriteLine(DateTime.Now.ToString() + " " + e.Message + " " + e.InnerException);
             sw.WriteLine(DateTime.Now.ToString() + "\t" + message );
             sw.Close();*/
        }


        public static SqlParameter[] CreateSQLParameterArray(SqlParameter[] spa, SqlParameter sp)
        {
            Array.Resize(ref spa, spa.Length + 1);
            spa[spa.Length - 1] = sp;

            return spa;
        }
    }
    public static class DBUtils
    {
        public static async Task<DataSet> GetDataSetAsync(this System.Data.Common.DbCommand command, CancellationToken cancellationToken)
        {
            TaskCompletionSource<DataSet> source = new TaskCompletionSource<DataSet>();
            var resultSet = new DataSet("DataSet1");

            DbDataReader dataReader = null;
            if (cancellationToken.IsCancellationRequested == true)
            {
                source.SetCanceled();
                await source.Task;
            }
            try
            {
                dataReader = await command.ExecuteReaderAsync(CommandBehavior.Default);
                int i = 0;
                bool IsNext = true;// dataReader.NextResult();

                while (IsNext)
                {
                    DataTable dt1 = new DataTable("Table" + (++i).ToString());
                    dt1.Load(dataReader);
                    if (dt1.Columns.Count == 0)
                        break;
                    resultSet.Tables.Add(dt1);
                }
                source.SetResult(resultSet);
            }
            catch (Exception ex)
            {
                //Log.LogNowAsync(LogType.Error, ex: ex);
                source.SetException(ex);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
            }
            return resultSet;
        }

        public static async Task<string> GetDataSetXMLAsync(this SqlCommand command, CancellationToken cancellationToken)
        {
            TaskCompletionSource<string> source = new TaskCompletionSource<string>();
            string myXML = "";
            //DbDataReader dataReader = null;
            XmlReader dataReader = null;
            if (cancellationToken.IsCancellationRequested == true)
            {
                source.SetCanceled();
                await source.Task;
            }
            try
            {
                using (XmlReader reader = command.ExecuteXmlReader())
                {
                    while (reader.Read())
                    {
                        myXML = reader.ReadOuterXml();
                    }
                }

                dataReader = await command.ExecuteXmlReaderAsync();
                int i = 0;
                bool IsNext = true;// dataReader.NextResult();


                source.SetResult(myXML);
            }
            catch (Exception ex)
            {
                //Log.LogNowAsync(LogType.Error, ex: ex);
                source.SetException(ex);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
            }
            return myXML;
        }
    }

    public class DBLogger
    {
        public DateTime ExecTime { get; set; }
        public string Procedure { get; set; }
        public int TimeTaken { get; set; }
        public string Exception { get; set; }
    }
    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        public int Size { get; private set; }

        public FixedSizedQueue(int size)
        {
            Size = size;
        }
        private void _Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (this)
            {
                while (base.Count > Size)
                {
                    T outObj;
                    base.TryDequeue(out outObj);
                }
            }
        }
        public new void Enqueue(T obj)
        {
            Task.Run(() => _Enqueue(obj));
        }
    }
}