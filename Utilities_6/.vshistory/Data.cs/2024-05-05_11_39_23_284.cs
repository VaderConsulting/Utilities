using System.Data;
using System.Data.SqlClient;
#if !NET5_0_OR_GREATER
using System.Data.SQLite;
#endif
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;

using MySql.Data.MySqlClient;

namespace Utilities
{
    public static class Data
    {


        #region Events

        #endregion


        #region DLL Imports

        #endregion


        #region Properties

        #endregion


        #region Event Handlers

        #endregion


        #region New Methods

        //public static SqlConnection GetConnection(string Connectionstring)
        //{
        //    try
        //    {
        //        return new SqlConnection(Connectionstring);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //public static DataSet Read(SqlConnection Connection, string Query)
        //{
        //    DataSet Result = new DataSet();

        //    try
        //    {
        //        if (Connection.State == ConnectionState.Closed)
        //        {
        //            Connection.Open();
        //        }

        //        SqlDataAdapter Adapter = new SqlDataAdapter(Query, Connection);

        //        int RowCount = Adapter.Fill(Result);

        //        Connection.Close();

        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        Connection.Close();
        //    }

        //    return Result;
        //}

        //public static bool Write(SqlConnection Connection, string Query)
        //{
        //    bool Result = false;

        //    try
        //    {
        //        if (Connection.State == ConnectionState.Closed)
        //        {
        //            Connection.Open();
        //        }

        //        SqlDataAdapter Adapter = null; // new SqlDataAdapter(Query, Connection);

        //        string DummyQuery = "select * from Address where 0 = 1";

        //        Adapter = new SqlDataAdapter(DummyQuery, Connection);
        //        DataSet dataSet = new DataSet();
        //        Adapter.Fill(dataSet);

        //        var newRow = dataSet.Tables["Customers"].NewRow();
        //        newRow["CustomerID"] = 55;
        //        dataSet.Tables["Customers"].Rows.Add(newRow);

        //        new SqlCommandBuilder(Adapter);
        //        Adapter.Update(dataSet);

        //        Connection.Close();

        //        Result = true;

        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        Connection.Close();
        //    }

        //    return Result;
        //}

        #endregion

        #region Public Methods

        /// <summary>
        /// Execute a query, without returning a dataset
        /// </summary>
        /// <param name="ConnectionString">The Connection string to connect with</param>
        /// <param name="QueryString">The query to execute</param>
        /// <param name="Timeout">OPTIONAL Timeout.  Only override the default if you have no other option</param>
        /// <remarks></remarks>
        public static void ExecuteSQLNonQuery(string ConnectionString, string QueryString, int Timeout = 30)
        {
            SqlConnection? DataConnection;
            SqlCommand? DataCommand;

            Cursor.Current = Cursors.WaitCursor;

            DataConnection = new SqlConnection(ConnectionString);

            if (DataConnection.State == ConnectionState.Closed)
            {
                DataConnection.Open();
            }

            DataCommand = new SqlCommand(QueryString, DataConnection);

            DataCommand.CommandTimeout = Timeout;
            DataCommand.CommandText = QueryString;

            try
            {
                DataCommand.ExecuteNonQuery();
            }
            catch
            {
            }

            DataCommand.Dispose();
            DataConnection.Close();

            DataConnection.Dispose();

            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Execute a query, without returning a dataset
        /// </summary>
        /// <param name="ConnectionString">The Connection string to connect with</param>
        /// <param name="QueryString">The query to execute</param>
        /// <param name="Timeout">OPTIONAL Timeout.  Only override the default if you have no other option</param>
        /// <remarks></remarks>
        public static void ExecuteSQLiteNonQuery(string ConnectionString, string QueryString, int Timeout = 30)
        {
            SqliteConnection? Connection;
            SqliteCommand? Command;

            Cursor.Current = Cursors.WaitCursor;

            Connection = new SqliteConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            Command = Connection.CreateCommand();

            Command.CommandTimeout = Timeout;
            Command.CommandText = QueryString;

            try
            {
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception thrown: {e}");
            }

            Command.Dispose();
            Connection.Close();

            Connection.Dispose();

            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Execute a Store Procedure
        /// </summary>
        /// <param name="ConnectionString">The Connection string to connect with</param>
        /// <param name="DataCommand"></param>
        /// <param name="Timeout">OPTIONAL Timeout.  Only override the default if you have no other option</param>
        /// <remarks></remarks>
        public static void ExecuteStoredProcedure(string ConnectionString, ref SqlCommand DataCommand, int Timeout = 30)
        {
            SqlConnection? DataConnection;

            Cursor.Current = Cursors.WaitCursor;

            DataConnection = new SqlConnection(ConnectionString);

            if (DataConnection.State == ConnectionState.Closed)
            {
                DataConnection.Open();
            }

            DataCommand.Connection = DataConnection;
            DataCommand.CommandTimeout = Timeout;

            DataCommand.ExecuteScalar();

            DataConnection.Close();

            DataConnection.Dispose();

            Cursor.Current = Cursors.Default;
        }

        public static DataRow? GetDataRowFromDataset(DataSet Recordset, string TableName, int RowNumber)
        {
            DataRow? Row = null;

            if (Recordset == null || TableName == null || RowNumber < 0)
            {
                return null;
            }

            if (Recordset.Tables != null && Recordset.Tables.Count >= 0)
            {
                if (Recordset.Tables[TableName].Rows.Count >= RowNumber)
                {
                    Row = Recordset.Tables[TableName].Rows[RowNumber];
                }
            }

            return Row;
        }

        public static DataRow? GetDataRowFromDataset(DataSet Recordset, int TableNumber, int RowNumber)
        {
            DataRow? Row = null;

            try
            {
                if (Recordset.Tables.Count >= TableNumber)
                {
                    if (Recordset.Tables[TableNumber].Rows.Count >= RowNumber)
                    {
                        if (Recordset.Tables[TableNumber].Rows.Count > 0)
                        {
                            Row = Recordset.Tables[TableNumber].Rows[RowNumber];
                        }
                    }
                }
            }
            catch
            {
            }

            return Row;
        }

        /// <summary>
        /// Build a query, returning a string
        /// </summary>
        /// <param name="TableName">The table to use in the SELECT</param>
        /// <param name="FieldNames">An array of fields to select</param>
        /// <param name="OrderByFieldNames">The fields to order by</param>
        /// <param name="OutsideWhereClause">The outside where clause</param>
        /// <param name="InsideWhereClause">The inside where clause</param>
        /// <param name="Joins">The joins to apply</param>
        /// <returns>Dataset</returns>
        /// <remarks></remarks>
        public static string BuildQuery(string TableName, string[] FieldNames, string[] OrderByFieldNames, string OutsideWhereClause, string InsideWhereClause, string Joins, bool EnablePaging)
        {
            StringBuilder Builder = new StringBuilder();
            int Counter = 0;

            foreach (string f in FieldNames)
            {
                Counter++;
                Builder.Append(f);
                if (Counter < FieldNames.Length)
                {
                    Builder.Append(", ");
                }
                else
                {
                    Builder.Append(' ');
                }
            }

            if (EnablePaging)
            {
                Builder.Append(", DENSE_RANK() OVER (ORDER BY ");
            }

            Counter = 0;

            if (EnablePaging)
            {
                if (OrderByFieldNames != null)
                {
                    foreach (string o in OrderByFieldNames)
                    {
                        Counter++;
                        Builder.Append(o);
                        if (Counter < OrderByFieldNames.Length)
                        {
                            Builder.Append(", ");
                        }
                        else
                        {
                            Builder.Append(' ');
                        }
                    }
                }
            }

            if (EnablePaging)
            {
                Builder.Append(") As 'RowNumber' ");
            }

            Builder.Append(" FROM ").Append(TableName).Append(' ');
            Builder.Append(Joins);

            if (InsideWhereClause.Length > 0 || OutsideWhereClause.Length > 0)
            {
                Builder.Append("WHERE ");
            }

            if (InsideWhereClause.Length > 0)
            {
                Builder.Append('(');
                Builder.Append(InsideWhereClause);
                Builder.Append(')');
            }

            if (InsideWhereClause.Length > 0 && OutsideWhereClause.Length > 0)
            {
                Builder.Append(" AND ");
            }

            if (OutsideWhereClause.Length > 0)
            {
                Builder.Append('(');
                Builder.Append(OutsideWhereClause).Append(' ');
                Builder.Append(')');
            }

            if (OrderByFieldNames != null)
            {
                Builder.Append(" ORDER BY ");

                foreach (string o in OrderByFieldNames)
                {
                    Counter++;
                    Builder.Append(o);
                    if (Counter < OrderByFieldNames.Length)
                    {
                        Builder.Append(", ");
                    }
                }
            }

            return Builder.ToString();
        }

        public static DataSet PageSQLData(string FinalTableName, string Query, int Start, int Size, string ConnectionString, int Timeout = 30)
        {
            StringBuilder Builder = new StringBuilder("WITH " + FinalTableName + " AS (SELECT DISTINCT TOP (100) PERCENT ");

            Builder.Append(Query);
            Builder.Append(") ");

            //Console.WriteLine(String.Format("SQL PageData: Start = {0}", Start, Builder.ToString()))

            Builder.Append("SELECT * FROM ").Append(FinalTableName).Append(" WHERE RowNumber BETWEEN ").Append(Start).Append(" AND ").Append(Start).Append(Size).Append(" ORDER BY RowNumber");

            //Console.WriteLine(String.Format("SQL PageData: {0}", Builder.ToString()))

            return GetSQLData(ConnectionString, Builder.ToString(), FinalTableName, Timeout);
        }

        public static string SQLString(string InputString)
        {
            string OutputString = InputString;

            if (InputString != null)
            {
                OutputString = InputString.Replace("'", "''");
            }

            return OutputString;
        }

        public static string SQLBoolean(bool InputValue)
        {
            string OutputString = "0";

            if (InputValue)
            {
                OutputString = "1";
            }

            return OutputString;
        }

        public static string SQLInteger(int InputValue)
        {
            string OutputValue = "0";

            if (InputValue > 0)
            {
                OutputValue = InputValue.ToString();
            }

            return OutputValue;
        }

        public static string SQLFloat(float InputValue)
        {
            string OutputValue = "0.0";

            if (InputValue > 0)
            {
                OutputValue = InputValue.ToString();
            }

            return OutputValue;
        }

        public static string SQLDecimal(decimal InputValue)
        {
            string OutputValue = "0.0";

            if (InputValue > 0)
            {
                OutputValue = InputValue.ToString();
            }

            return OutputValue;
        }

        public static (string, bool, DataTable) GetCSVData(string filePath, string[] SplitCharacters, bool WithHeaders, List<string> IgnoreList, string TableName)
        {
            DataTable DataResult = new DataTable();
            int SkipLength = 0;
            TextFieldParser? Parser = null;
            List<string[]> AllColumns = new List<string[]>();
            string ResultText = "No data to display";
            bool Result = false;

            if (filePath.Trim().Length > 0 && System.IO.File.Exists(filePath))
            {
                Parser = new TextFieldParser(filePath);
            }
            else
            {
                ResultText = "File not found";
                return (ResultText, Result, DataResult);
            }

            Parser.Delimiters = SplitCharacters;
            Parser.HasFieldsEnclosedInQuotes = true;
            Parser.TextFieldType = FieldType.Delimited;
            Parser.TrimWhiteSpace = true;

            if (IgnoreList == null)
            {
                IgnoreList = new List<string>();
            }

            try
            {
                // Read all data
                while (!Parser.EndOfData)
                {
                    string[]? columns = Parser.ReadFields();

                    if (columns != null)
                    {
                        AllColumns.Add(columns);
                    }
                }
                if (Parser.ErrorLineNumber != -1)
                {
                    // Error
                    ResultText = $"Error reading data at line {Parser.ErrorLineNumber}.  The data was: {Parser.ErrorLine}";
                    Result = false;
                }
                else// No error
                {
                    // Add headers appropriately
                    if (WithHeaders)
                    {
                        SkipLength = 1; // Rows to skip

                        AllColumns[0].ToList() // Use the first row to get all columns
                                     .ForEach(column => DataResult.Columns.Add(column));
                    }
                    else
                    {
                        SkipLength = 0; // Rows to skip

                        AllColumns[0].ToList() // Use the first row to get all columns
                                     .ForEach(_ => DataResult.Columns.Add("Column " + (DataResult.Columns.Count + 1).ToString()));
                    }

                    // Remove any rows we don't want
                    List<string[]> RemainingRows = AllColumns.Skip(SkipLength).ToList();
                    IEnumerable<string[]> output = from ValidRows in RemainingRows
                                                   where !(from row in RemainingRows
                                                           from ItemToIgnore in IgnoreList
                                                           where row[0].ToLower().Trim() == ItemToIgnore.ToLower().Trim()
                                                           select row).Contains(ValidRows)
                                                   select ValidRows;

                    // Now add these rows to the output as Rows
                    output.ToList()
                          .ForEach(row => DataResult.Rows.Add(row));

                    DataResult.TableName = TableName;

                    ResultText = $"Load complete: {output.Count()} rows loaded";

                    Result = true;
                }
            }
            catch (Exception e)
            {
                ResultText = $"Error: {e.Message}";
            }

            return (ResultText, Result, DataResult);
        }

#if IsWindows
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input")]
        public static (string, bool, DataTable) GetAccessData(string filePath, string Query, string TableName)
        {
            bool Result = false;
            string ResultText = "";
            DataTable ResultData = new DataTable();
            string ConnectionString = "";

            ConnectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Persist Security Info=True;"; // "Jet OLEDB:Database Password=myPassword;";

            try
            {
                // Open OleDb Connection
                OleDbConnection myConnection = new OleDbConnection();
                myConnection.ConnectionString = ConnectionString;
                myConnection.Open();

                // Execute Queries
                OleDbCommand cmd = myConnection.CreateCommand();
                cmd.CommandText = Query; // "SELECT * FROM `myTable`";
                OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection); // close conn after complete

                // Load the result into the DataTable
                ResultData.Load(reader);
                ResultData.TableName = TableName;

                ResultText = $"Load complete: {ResultData.Rows.Count} rows loaded";

                Result = true;
            }
            catch (Exception e)
            {
                ResultText = $"Error: {e.Message}";
            }

            return (ResultText, Result, ResultData);
        }
#endif

        /// <summary>
        /// Execute a query, returning a dataset
        /// </summary>
        /// <param name="ConnectionString">The Connection string to connect with</param>
        /// <param name="QueryString">The query to execute</param>
        /// /// <param name="TableName">The name of the output table</param>
        /// <param name="CommandTimeout">OPTIONAL Timeout.  Only override the default if you have no other option</param>
        /// <returns>Dataset</returns>
        /// <remarks></remarks>
        public static (string, bool, DataTable) GetMySQLData(string ConnectionString, string Query, string TableName, int CommandTimeout = 30)
        {
            bool Result = false;
            string ResultText = "";
            DataTable ResultData = new DataTable();

            MySqlConnection Connection = new MySqlConnection(ConnectionString);
            MySqlCommand Command;

            try
            {
                Connection.Open();

                Command = Connection.CreateCommand();
                Command.CommandText = Query;
                Command.CommandTimeout = CommandTimeout;
                //command.Parameters.AddWithValue("@id, int.Parse(blah);
                MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

                // Load the result into the DataTable
                ResultData.Load(reader);
                ResultData.TableName = TableName;

                ResultText = $"Load complete: {ResultData.Rows.Count} rows loaded";

                Result = true;
            }
            catch (Exception e)
            {
                ResultText = $"Error: {e.Message}";
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }
            }

            return (ResultText, Result, ResultData);
        }

        /// <summary>
        /// Execute a query, returning a dataset
        /// </summary>
        /// <param name="ConnectionString">The Connection string to connect with</param>
        /// <param name="QueryString">The query to execute</param>
        /// /// <param name="TableName">The name of the output table</param>
        /// <param name="CommandTimeout">OPTIONAL Timeout.  Only override the default if you have no other option</param>
        /// <returns>Dataset</returns>
        /// <remarks></remarks>
        public static (string, bool, DataTable) GetSQLiteData(string ConnectionString, string Query, string TableName, int CommandTimeout = 30)
        {
            bool Result = false;
            string ResultText = "";
            DataTable ResultData = new DataTable();

            SqliteConnection Connection = new SqliteConnection(ConnectionString);

            try
            {
                Connection.Open();
                SqliteCommand Command = Connection.CreateCommand();

                Command.CommandText = Query;
                Command.CommandTimeout = CommandTimeout;
                //command.Parameters.AddWithValue("@id, int.Parse(blah);
                SqliteDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

                // Load the result into the DataTable
                ResultData.Load(reader);
                ResultData.TableName = TableName;

                ResultText = $"Load complete: {ResultData.Rows.Count} rows loaded";

                Result = true;
            }
            catch (Exception e)
            {
                ResultText = $"Error: {e.Message}";
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }
            }

            return (ResultText, Result, ResultData);
        }

        /// <summary>
        /// Execute a query, returning a dataset
        /// </summary>
        /// <param name="ConnectionString">The Connection string to connect with</param>
        /// <param name="QueryString">The query to execute</param>
        /// /// <param name="TableName">The name of the output table</param>
        /// <param name="CommandTimeout">OPTIONAL Timeout.  Only override the default if you have no other option</param>
        /// <returns>Dataset</returns>
        /// <remarks></remarks>
        public static DataSet GetSQLData(string ConnectionString, string QueryString, string TableName, int CommandTimeout = 30)
        {
            DataSet ReturnData = new DataSet("DataSet1");
            DataTable Table = new DataTable("Table1");
            SqlConnection? DataConnection;
            SqlDataReader? DataReader = default(SqlDataReader);
            SqlCommand? DataCommand;

            Cursor.Current = Cursors.WaitCursor;

            DataConnection = new SqlConnection(ConnectionString);

            if (DataConnection.State == ConnectionState.Closed)
            {
                try
                {
                    DataConnection.Open();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                    Cursor.Current = Cursors.Default;
                }
            }

            DataCommand = new SqlCommand(QueryString, DataConnection);

            DataCommand.CommandTimeout = CommandTimeout;
            DataCommand.CommandText = QueryString;

            try
            {
                DataReader = DataCommand.ExecuteReader();

                // Get the data and put it into a table
                if (DataReader.HasRows)
                {
                    Table.Load(DataReader);
                }
                Table.TableName = TableName;
                ReturnData.Tables.Add(Table);

                DataCommand.Dispose();

                DataReader.Close();
                DataConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            DataCommand.Dispose();
            DataReader?.Dispose();
            DataConnection.Dispose();

            Cursor.Current = Cursors.Default;

            return ReturnData;
        }

        #endregion

        #region Classes

        // By my own convention all classes should be in their own file, however sometimes it makes sense to include a class within the same file as it's parent Namespace

        public class Parameters
        {


            #region Events

            #endregion


            #region DLL Imports

            #endregion

            #region Fields

            private string? _ReadStatement;
            private string? _CreateStatement;
            private string? _UpdateStatement;
            private string? _DeleteStatement;

            #endregion

            #region Properties

            public string? ReadStatement
            {
                get => _ReadStatement;
                set => _ReadStatement = value;
            }

            public string? CreateStatement
            {
                get => _CreateStatement;
                set => _CreateStatement = value;
            }

            public string? UpdateStatement
            {
                get => _UpdateStatement;
                set => _UpdateStatement = value;
            }

            public string? DeleteStatement
            {
                get => _DeleteStatement;
                set => _DeleteStatement = value;
            }

            #endregion


            #region Event Handlers

            #endregion


            #region Public Methods

            #endregion

            #region Classes

            // By my own convention all classes should be in their own file, however sometimes it makes sense to include a class within the same file as it's parent Namespace

            #endregion

        }

        #endregion
    }
}
