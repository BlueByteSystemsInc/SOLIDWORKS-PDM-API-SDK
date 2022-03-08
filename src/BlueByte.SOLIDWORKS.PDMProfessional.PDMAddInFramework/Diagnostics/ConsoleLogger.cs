using EPDM.Interop.epdm;
using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics
{
    internal class ConsoleLogger  : LoggerBase, ILogger
    {
        public string OutputLocation { get; set; }

        public void EndConnection()
        {
            throw new NotImplementedException($"Not implemented because the logger type chosen is {GetLoggerType()}.");

        }

        public void Init(Identity identity, IEdmTaskInstance instance, string connectionString)
        {
            throw new NotImplementedException($"Not implemented because the logger type chosen is {GetLoggerType()}.");

        }

        public void LogToOutput(string fileName, string value)
        {
            Console.WriteLine(value);
        }

        public void StartConnection()
        {
            throw new NotImplementedException();
        }
    }



}
