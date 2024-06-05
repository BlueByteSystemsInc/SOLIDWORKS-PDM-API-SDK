using BlueByte.SOLIDWORKS.PDMProfessional.SDK.Exceptions;
using EPDM.Interop.epdm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPDMLogger : IDisposable
    {
        /// <summary>
        /// Gets the logs from vault.
        /// </summary>
        /// <returns></returns>
        List<Tuple<string,object[]>> GetAllLogsFromVault();


        /// <summary>
        /// Commits log entry to vault.
        /// </summary>
        /// <param name="logEntryOrTarget">The log entry or target.</param>
        void CommitToVault(string logEntryOrTarget);
        
 
    }

    internal class PDMLogger : LoggerBase, ILogger  , IPDMLogger
    {
         

        public List<Tuple<string, object[]>> GetAllLogsFromVault()
        {
            return PDMLoggerHelper.GetLogs($"{this.PDMLoggerHelper.AddInName}-Logs", LogType);
        }

        public Type LogType { get; set; }

        internal void Verify()
        {
            var type = GetLoggerType();
            switch (type)
            {
                case LoggerType_e.Console:
                case LoggerType_e.File:
                case LoggerType_e.SQL:
                    throw new Exception($"Chosen logger type is {type.ToString()}");
                case LoggerType_e.PDM:
                    break;
                default:
                    break;
            }


        }


        public PDMLogger()
        {
            Logs = new List<object>();

        }


        public List<object> Logs { get; set; }

        /// <summary>
        /// Starts the connection.
        /// </summary>
        public void StartConnection()
        {
            Verify();


            

        }

        /// <summary>
        /// Ends the connection.
        /// </summary>
        /// <returns></returns>
        public void EndConnection()
        {
         
 
        }



        public string OutputLocation { get; set; }

        private IEdmVault5 vault;

        public PDMLoggerHelper PDMLoggerHelper { get; private set; }

        
        public void LogToOutput(string target, object value)
        {
            if (this.PDMLoggerHelper.Initialized == false)
                this.PDMLoggerHelper.Initialize(vault, identity.Name);


            if (value.GetType().Name.Equals(LogType.Name) == false)
                throw new Exceptions.PDMSDKException("You need to make sure value's type match the LogType of the PDM logger", null);

            Logs.Add(value);

        }


        public void CommitToVault(string logEntryOrTarget)
        {
            this.PDMLoggerHelper.SaveLog($"{this.PDMLoggerHelper.AddInName}-Logs", logEntryOrTarget, Logs.ToArray());
        }
      

        public void Init(Identity identity, IEdmTaskInstance instance, string connectionString)
        {
            this.identity = identity;

            this.vault = identity.Vault;

            this.PDMLoggerHelper = new PDMLoggerHelper();

            this.PDMLoggerHelper.Initialize(this.vault, identity.Name);
        }

        public void Dispose()
        {
            var Name = $"{this.PDMLoggerHelper.AddInName}-Logs";
            IEdmDictionary5 dictionary = this.identity.Vault.GetDictionary(Name, true);
            
            dictionary.RemoveDictionary(); 

        }
    }




    internal class PDMLoggerHelper 

    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="PDMLoggerHelper"/> is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        public bool Initialized { get; private set; }

        /// <summary>
        /// Gets the vault.
        /// </summary>
        /// <value>
        /// The vault.
        /// </value>
        public IEdmVault10 Vault { get; private set; }

    

     

        /// <summary>
        /// Initializes a new instance of the <see cref="PDMLoggerHelper"/> class.
        /// </summary>
        public PDMLoggerHelper()
        {

        }
        /// <summary>
        /// Initializes the specified vault.
        /// </summary>
        /// <param name="vault">The vault.</param>
        /// <param name="addInName">AddIn Name</param>
        /// <exception cref="System.NullReferenceException">
        /// name
        /// or
        /// vault
        /// </exception>
        public void Initialize(IEdmVault5 vault, string addInName)
        {

            if (string.IsNullOrWhiteSpace(addInName))
                throw new NullReferenceException(nameof(addInName));

            if (vault == null)
                throw new NullReferenceException(nameof(vault));


            this.AddInName = addInName;

            this.Vault = vault as IEdmVault10;

            Initialized = true;
        }

        /// <summary>
        /// Gets the name of the add in.
        /// </summary>
        /// <value>
        /// The name of the add in.
        /// </value>
        public string AddInName { get; private set; }



        public void SaveLog(string name, string key, object item, JsonSerializerSettings settings = null)
        {
            IEdmDictionary5 dictionary = Vault.GetDictionary(name, true);

            var serialized = Extensions.Serialize(item, settings);

            dictionary.StringSetAt(key, serialized);
        }

     
        public void SaveLog<T>(string name, string key, T item, JsonSerializerSettings settings = null)
        {
            IEdmDictionary5 dictionary = Vault.GetDictionary(name, true);

            var serialized = Extensions.Serialize(item, settings);

            try
            {
                dictionary.StringSetAt(key, serialized);

            }
            catch (OutOfMemoryException e)
            {
                throw new PDMSDKException($"[{name}] [{key}] - This log entry has exceeded the allowed space.", e);
            }
        }

        
         
        public List<Tuple<string, object[]>> GetLogs(string Name, Type logType, JsonSerializerSettings settings = null)
        {
            IEdmDictionary5 dictionary = Vault.GetDictionary(Name, true);

            var logs = new List<Tuple<string, object[]>>();


            var pos = dictionary.StringGetFirstPosition();

            while (pos.IsNull == false)
            {
                string title;

                string logStr;
                
                dictionary.StringGetNextAssoc(pos, out title, out logStr);

                var logObj = Extensions.Deserialize(logStr, logType.MakeArrayType(), settings) as object[];

                logs.Add(new Tuple<string,object[]>(title, logObj));
            }

           
            return logs;
        }










    }

     

    internal static class Extensions
    {
        public static T Deserialize<T>(this string value, JsonSerializerSettings settings = null)
        {

            return JsonConvert.DeserializeObject<T>(value, settings);
        }


        public static object Deserialize(this string value, Type logType, JsonSerializerSettings settings = null)
        {

            return JsonConvert.DeserializeObject(value, logType, settings);
        }

        public static string Serialize<T>(this T value, JsonSerializerSettings settings = null)
        {
            if (value == null)
                return string.Empty;



            return JsonConvert.SerializeObject(value, settings);


        }
    }

}