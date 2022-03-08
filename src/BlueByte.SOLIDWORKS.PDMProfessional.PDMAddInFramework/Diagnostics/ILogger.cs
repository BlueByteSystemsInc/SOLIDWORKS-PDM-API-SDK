using EPDM.Interop.epdm;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics
{
    /// <summary>
    /// Logger.
    /// </summary>
    public interface ILogger
    {

        /// <summary>
        /// Starts a connection with the SQL server. Call this method after you call <see cref="Init(Identity, IEdmTaskInstance, string)"/>.
        /// </summary>
        void StartConnection();

        /// <summary>
        /// Ends the connection with the SQLServer
        /// </summary>
        void EndConnection();

        /// <summary>
        /// Returns the type of the logger.
        /// </summary>
        /// <returns></returns>
        LoggerType_e GetLoggerType();


        /// <summary>
        /// Logs value to output.
        /// </summary>
        /// <param name="target">Text file path. This is the name of the table when <see cref="GetLoggerType()"/> returns <see cref="LoggerType_e.SQL"/></param>
        /// <param name="value">New value.</param>
        void LogToOutput(string target, string value);
      
        
        /// <summary>
        /// Ouput location. Relative path does not work.
        /// </summary>
        string OutputLocation { get; set; }


        /// <summary>
        /// Initialize the logger.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="identity">The identity struct.</param>
        /// <param name="instance">Task instance.</param>
        void Init(Identity identity, IEdmTaskInstance instance, string connectionString);



    }



}
