//*********************************************************************
//xToolkit
//Copyright(C) 2023 Xarial Pty Limited
//Product URL: https://xtoolkit.xarial.com
//License: https://xtoolkit.xarial.com/license/
//*********************************************************************

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Xarial.XToolkit.Reflection
{
    /// <summary>
    /// Resolver for <see cref="Helpers.AssemblyResolver"/> allowing to redirect assembly binding based on the .config files
    /// </summary>
    /// <remarks>This resolver can be useful for the plugin applications (.dlls) when the app.config files will not be considered for binding redirects
    /// It can also be useful when the binding redirects specified in the separate file which is not named after the application name (e.g. custom binding redirect without an automatic option)</remarks>
    public class AppConfigBindingRedirectReferenceResolver : AssemblyNameReferenceResolver
    {
        public AppConfigBindingRedirectReferenceResolver() : this("")
        {
        }

        public AppConfigBindingRedirectReferenceResolver(string name, string[] filterDirs = null) : base(name, filterDirs) 
        {
        }

        protected virtual Assembly[] GetRequestingAssemblies(Assembly requestingAssembly) 
        {
            if (requestingAssembly != null)
            {
                return new Assembly[] { requestingAssembly };
            }
            else 
            {
                return new Assembly[0];
            }
        }

        protected virtual string[] GetAppConfigs(Assembly requestingAssembly) 
            => GetRequestingAssemblies(requestingAssembly)
            .Select(a => a.Location + ".config")
            .Where(f => File.Exists(f)).ToArray();

        protected override AssemblyName GetReplacementAssemblyName(AssemblyName assmName, Assembly requestingAssembly, 
            out string searchDir, out bool recursiveSearch)
        {
            foreach (var appConfigPath in GetAppConfigs(requestingAssembly))
            {
                if (!File.Exists(appConfigPath))
                {
                    throw new FileNotFoundException($"Specified application configuration file is not available: '{appConfigPath}'");
                }

                using (var xmlStream = File.OpenRead(appConfigPath))
                {
                    using (var reader = new XmlTextReader(xmlStream))
                    {
                        var doc = XDocument.Load(reader);

                        var namespaceManager = new XmlNamespaceManager(reader.NameTable);
                        namespaceManager.AddNamespace("b", "urn:schemas-microsoft-com:asm.v1");

                        var name = assmName.Name;
                        var publicKeyToken = GetPublicKeyToken(assmName);
                        var culture = GetCulture(assmName);

                        var bindRedirect = doc.XPathSelectElement(
                            $"//configuration/runtime/b:assemblyBinding/b:dependentAssembly[b:assemblyIdentity[@name = '{name}'][@publicKeyToken = '{publicKeyToken}'][@culture = '{culture}']]/b:bindingRedirect", namespaceManager);

                        if (bindRedirect != null)
                        {
                            var oldVersionVal = bindRedirect.Attribute("oldVersion").Value;
                            var newVersionVal = bindRedirect.Attribute("newVersion").Value;

                            Version oldVersionMin = null;
                            Version oldVersionMax = null;

                            if (oldVersionVal.Contains("-"))
                            {
                                var oldVersRange = oldVersionVal.Split('-');
                                oldVersionMin = Version.Parse(oldVersRange[0]);
                                oldVersionMax = Version.Parse(oldVersRange[1]);
                            }
                            else
                            {
                                oldVersionMin = Version.Parse(oldVersionVal);
                                oldVersionMax = Version.Parse(oldVersionVal);
                            }

                            var newVersion = Version.Parse(newVersionVal);

                            if (assmName.Version >= oldVersionMin && assmName.Version <= oldVersionMax)
                            {
                                var searchAssmName = new AssemblyName($"{name}, Version={newVersion}, Culture={culture}, PublicKeyToken={publicKeyToken}");

                                searchDir = Path.GetDirectoryName(appConfigPath);
                                recursiveSearch = true;
                                return searchAssmName;
                            }
                        }
                    }
                }
            }

            searchDir = "";
            recursiveSearch = false;
            return null;
        }
    }
}