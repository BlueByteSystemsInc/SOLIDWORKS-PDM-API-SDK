using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Diagnostics
{
    /// <summary>
    /// Logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Returns the type of the logger.
        /// </summary>
        /// <returns></returns>
        LoggerType_e GetLoggerType();
        

        /// <summary>
        /// Print a value in a text file.
        /// </summary>
        /// <param name="target">Text file path.</param>
        /// <param name="value">New value.</param>
       void LogToOutput(string target, string value);
      /// <summary>
      /// Ouput location.
      /// </summary>
       string OutputLocation { get; set; }
        /// <summary>
        /// Print an exception.
        /// </summary>
        /// <param name="e">Exception</param>
        /// <returns></returns>
       string Print(Exception e);
        /// <summary>
        /// Print an error message.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
       string Print(string errorMessage);

        /// <summary>
        ///  Add a log record to the SQL server db.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="identity"></param>
        /// <param name="message"></param>
        /// <param name="tableName"></param>
        /// <param name="taskName"></param>
        /// <param name="taskInstanceGUID"></param>
        void LogToSQLServer(DateTime timeStamp, Identity identity, string message, string tableName = "Log", string taskName = "", string taskInstanceGUID = "");

        /// <summary>
        /// Starts a connection with the SQL server.
        /// </summary>
        void StartConnection();

        /// <summary>
        /// Ends the connection with the SQLServer
        /// </summary>
        void EndConnection();

        void SetConnectionString(string connectionString);
    }

    public abstract class LoggerBase 
    {

         void Verify()
        {
            var type = GetLoggerType();
            switch (type)
            {
                case LoggerType_e.Console:
                case LoggerType_e.File:
                    throw new Exception($"Chosen logger type is {type.ToString()}");
                case LoggerType_e.SQL:
                    break;
                default:
                    break;
            }

            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new Exception("ConnectionString was not set.");

        }

        public void LogToSQLServer(DateTime timeStamp, Identity identity, string message, string tableName = "Log", string taskName = "", string taskInstanceGUID = "")
        {

            Verify();
            var sql = $"INSERT into {tableName}(TimeStamp,AddInName,AddInVersion,CompanyName,TaskName,TaskInstanceGUID,Message) VALUES (@TimeStamp,@AddInName,@AddInVersion,@CompanyName,@TaskName,@TaskInstanceGUID,@Message)";

           

            var command = new SqlCommand(sql, cnn);
            command.Parameters.Add(new SqlParameter("@TimeStamp", timeStamp));
            command.Parameters.Add(new SqlParameter("@AddInName", identity.Name));
            command.Parameters.Add(new SqlParameter("@AddInVersion", identity.Version));
            command.Parameters.Add(new SqlParameter("@CompanyName", identity.CompanyName));
            command.Parameters.Add(new SqlParameter("@TaskName", taskName));
            command.Parameters.Add(new SqlParameter("@TaskInstanceGUID", taskInstanceGUID));
            command.Parameters.Add(new SqlParameter("@Message", message));

            command.ExecuteNonQuery();

           
        }


        public void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        SqlConnection cnn;

        public string ConnectionString { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public void StartConnection()
        {
            Verify();

           if (cnn == null)
            {
                cnn = new SqlConnection(ConnectionString);
                cnn.Open();
            }
           else if (cnn.State == System.Data.ConnectionState.Closed)
            {
                cnn.Open();
            }
           
            
             
        }
        public void EndConnection()
        {
            Verify();

            if (cnn.State == System.Data.ConnectionState.Open)
                cnn.Close();
        }

        

        public LoggerType_e GetLoggerType()
        {
            var typeName = this.GetType().Name.Replace("Logger","");
            LoggerType_e type;
            var rt = Enum.TryParse(typeName, out type);
            if (rt)
                return type;
            else
                throw new Exception("Could not infer type of logger. Make sure to choose the logger type.");
        }
    }

    public class SQLLogger : LoggerBase, ILogger
    {
        public string OutputLocation { get; set; }

        public void Init(string connectionString)
        {

        }


        public string Print(Exception e)
        {
            Debug.Print(e.Message);
            return e.Message;
        }

        public string Print(string errorMessage)
        {
            Debug.Print(errorMessage);
            return errorMessage;
        }

        public void LogToOutput(string fileName, string value)
        {
            throw new NotImplementedException();
        }
    }
    public class ConsoleLogger  : LoggerBase, ILogger
    {
        public string OutputLocation { get; set; }
        public string Print(Exception e)
        {
            Console.WriteLine(e.Message);
            return e.Message;
        }

        public string Print(string errorMessage)
        {
            Console.WriteLine(errorMessage);
            return errorMessage;
        }

        public void LogToOutput(string fileName, string value)
        {
            Console.WriteLine(value);
        }
    }

    public class FileLogger : LoggerBase, ILogger
    {
        public string OutputLocation { get; set; }


        public string Print(Exception e)
        {
            string errorMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd-hh:mm:ss")}] - {e.Message}";
            return errorMessage;

        }

        public string Print(string errorMessage)
        {
            string err = $"[{DateTime.Now.ToString("yyyy-MM-dd-hh:mm:ss")}] - {errorMessage}";

            return err;
        }

        public void LogToOutput(string fileName, string value)
        {

            var builder = new StringBuilder();

            if (System.IO.Directory.Exists(OutputLocation) == false)
                Directory.CreateDirectory(OutputLocation);

            var path = System.IO.Path.Combine(OutputLocation, fileName);

            if (System.IO.File.Exists(path) == false)
            {

                var stream = System.IO.File.Create(path);
                stream.Close();
                stream.Dispose();


                var assembly = System.Reflection.Assembly.GetCallingAssembly();
                var Version = assembly.GetName().Version;
                var version = $"{Version.Major}.{Version.Minor}.{Version.Revision}-{Version.Build}";

                builder.AppendLine($"Assembly name: {assembly.FullName} - Assembly version: {version}");
                builder.AppendLine($"Process : {System.Diagnostics.Process.GetCurrentProcess().ProcessName} - ID : {System.Diagnostics.Process.GetCurrentProcess().Id}");
                builder.AppendLine($"Started logged at { DateTime.Now.ToString("yyyy-MM-dd-hh:mm:ss")}");
                builder.AppendLine("==============");
                builder.AppendLine($"[{DateTime.Now.ToString("yyyy-MM-dd-hh:mm:ss")}]- {value}");
            }
            else
            {
                builder.AppendLine($"[{DateTime.Now.ToString("yyyy-MM-dd-hh:mm:ss")}]- {value}");
            }
            if (string.IsNullOrWhiteSpace(builder.ToString()) == false)
            System.IO.File.AppendAllText(path, builder.ToString());
        }
    }



}
