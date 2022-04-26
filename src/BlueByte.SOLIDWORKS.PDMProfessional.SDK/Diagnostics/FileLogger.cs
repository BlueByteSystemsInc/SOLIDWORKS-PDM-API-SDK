using EPDM.Interop.epdm;
using System;
using System.IO;
using System.Text;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics
{
    internal class FileLogger : LoggerBase, ILogger
    {
        public string OutputLocation { get; set; }
        public IEdmTaskInstance instance { get; private set; }

      

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

                builder.AppendLine($"AddIn: {identity.Name} - Version: {identity.Version} - Task : {instance?.TaskName} - ID : {instance?.InstanceGUID}");
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

        public void StartConnection()
        {
            throw new NotImplementedException($"Not implemented because the logger type chosen is {GetLoggerType()}.");
        }

        public void EndConnection()
        {
            throw new NotImplementedException($"Not implemented because the logger type chosen is {GetLoggerType()}.");
        }

        public void Init(Identity identity, IEdmTaskInstance instance, string connectionString)
        {
            this.identity = identity;
            this.instance = instance;
             
        }
    }



}
