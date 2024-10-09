//*********************************************************************
//xToolkit
//Copyright(C) 2023 Xarial Pty Limited
//Product URL: https://xtoolkit.xarial.com
//License: https://xtoolkit.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xarial.XToolkit.Helpers;

namespace Xarial.XToolkit.Reflection
{
    /// <summary>
    /// Default assembly name resolver
    /// </summary>
    public abstract class AssemblyNameReferenceResolver : IReferenceResolver
    {
        /// <summary>
        /// Assembly information
        /// </summary>
        public class AssemblyInfo
        {
            /// <summary>
            /// File path to the assembly
            /// </summary>
            public string FilePath { get; }

            /// <summary>
            /// Assembly name
            /// </summary>
            public AssemblyName Name { get; }
            
            internal bool IsLoaded { get; }

            internal AssemblyInfo(AssemblyName name, string filePath, bool isLoaded)
            {
                Name = name;
                FilePath = filePath;
                IsLoaded = isLoaded;
            }
        }

        /// <summary>
        /// Name of the resolver
        /// </summary>
        /// <remarks>Used in the logs</remarks>
        public string Name { get; }

        private readonly string[] m_FilterDirs;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">Name of the resolver</param>
        /// <param name="filterDirs">Only resolve the assembly if requesting assembly is in the specified directories</param>
        public AssemblyNameReferenceResolver(string name, string[] filterDirs = null) 
        {
            if (string.IsNullOrEmpty(name))
            {
                name = this.GetType().FullName;
            }

            Name = name;

            m_FilterDirs = filterDirs;
        }
        
        /// <inheritdoc/>>
        public virtual Assembly Resolve(AppDomain appDomain, AssemblyName assmName, Assembly requestingAssembly)
        {
            if (ShouldResolve(appDomain, assmName, requestingAssembly))
            {
                var searchAssmName = GetReplacementAssemblyName(assmName, requestingAssembly, out string searchDir, out bool recursiveSearch);

                if (searchAssmName != null)
                {
                    var matchedAssmNames = new List<AssemblyInfo>();

                    var replacementAssms = appDomain.GetAssemblies().Where(
                                            a => Match(a.GetName(), searchAssmName));

                    var exactMatch = replacementAssms.FirstOrDefault(a => CompareAssemblyNames(a.GetName(), searchAssmName));

                    if (exactMatch != null)
                    {
                        Trace.WriteLine($"Assembly '{searchAssmName}' is resolved to '{exactMatch.Location}' as exact match via '{Name}' resolver", Name);

                        return exactMatch;
                    }
                    else
                    {
                        matchedAssmNames.AddRange(replacementAssms.Select(a => new AssemblyInfo(a.GetName(), a.Location, true)));
                    }

                    foreach (var name in EnumerateAssemblyByName(searchDir, recursiveSearch, searchAssmName))
                    {
                        if (CompareAssemblyNames(name.Name, searchAssmName))
                        {
                            Trace.WriteLine($"Loading '{searchAssmName}' from '{name.FilePath}' as exact match via '{Name}' resolver", Name);

                            return LoadAssembly(new AssemblyInfo(AssemblyName.GetAssemblyName(name.FilePath), name.FilePath, false));
                        }
                        else
                        {
                            matchedAssmNames.Add(name);
                        }
                    }

                    var assmInfo = ResolveAmbiguity(matchedAssmNames, searchAssmName);

                    if (assmInfo != null)
                    {
                        return LoadAssembly(assmInfo);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if assembly should be resolved by this resolver
        /// </summary>
        /// <param name="appDomain">App Domain</param>
        /// <param name="assmName">Assembly to resolve</param>
        /// <param name="requestingAssembly">Requesting assembly</param>
        /// <returns></returns>
        protected virtual bool ShouldResolve(AppDomain appDomain, AssemblyName assmName, Assembly requestingAssembly) 
        {
            if (m_FilterDirs?.Any() == true && requestingAssembly != null)
            {
                var reqAssmFilePath = requestingAssembly.Location;

                return m_FilterDirs.Any(f => FileSystemUtils.IsInDirectory(reqAssmFilePath, f));
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Load the assembly
        /// </summary>
        /// <param name="assmInfo">Assembly information</param>
        /// <returns>Loaded assembly</returns>
        protected Assembly LoadAssembly(AssemblyInfo assmInfo) 
        {
            Trace.WriteLine($"Loading '{assmInfo.Name}' from file '{assmInfo.FilePath}' [Loaded={assmInfo.IsLoaded}] via '{Name}' resolver", Name);

            return Assembly.Load(assmInfo.Name);
        }

        /// <summary>
        /// Gets the culture of the assembly name
        /// </summary>
        /// <param name="assmName">Assembly name</param>
        /// <returns>Text of the culture</returns>
        protected string GetCulture(AssemblyName assmName)
        {
            if (string.IsNullOrEmpty(assmName.CultureName))
            {
                return "neutral";
            }
            else
            {
                return assmName.CultureName;
            }
        }

        /// <summary>
        /// Gets the public key token from the assembly name
        /// </summary>
        /// <param name="assmName">Assembly name</param>
        /// <returns>Text version of public key token</returns>
        protected string GetPublicKeyToken(AssemblyName assmName)
        {
            var bytes = assmName.GetPublicKeyToken();

            if (bytes == null || bytes.Length == 0)
            {
                return "null";
            }

            var publicKeyToken = "";

            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                publicKeyToken += string.Format("{0:x2}", bytes[i]);
            }

            return publicKeyToken;
        }

        /// <summary>
        /// Provides the name of the assembly to replace for this missing reference
        /// </summary>
        /// <param name="assmName">Missing assembly name</param>
        /// <param name="requestingAssembly">Assembly that requests this missing reference</param>
        /// <param name="searchDir">Search directory</param>
        /// <param name="recursiveSearch">True to search in sub-directories recursievely</param>
        /// <returns>Assemly to replace</returns>
        protected abstract AssemblyName GetReplacementAssemblyName(AssemblyName assmName, Assembly requestingAssembly,
            out string searchDir, out bool recursiveSearch);

        /// <summary>
        /// Compares two assemblies to see if those match
        /// </summary>
        /// <param name="probeAssmName">Assembly candidate</param>
        /// <param name="searchAssmName">Target assembly</param>
        /// <returns>True if assembly names are matching</returns>
        /// <remarks>Use this method to override logic for matching (e.g. full match or only match by file name, version, public key token etc.)</remarks>
        protected virtual bool Match(AssemblyName probeAssmName, AssemblyName searchAssmName)
            => CompareAssemblyNames(probeAssmName, searchAssmName);

        /// <summary>
        /// Provides the assembly to use if multiple options available
        /// </summary>
        /// <param name="assmNames">Assembly candidates</param>
        /// <param name="searchAssmName">Target assembly name</param>
        /// <returns>Assembly to use</returns>
        protected virtual AssemblyInfo ResolveAmbiguity(
            IReadOnlyList<AssemblyInfo> assmNames, AssemblyName searchAssmName)
        {
            Trace.WriteLine($"Resolving ambiguity for '{searchAssmName}' via '{Name}' resolver", Name);

            var assmInfo = assmNames.FirstOrDefault(a => CompareAssemblyNames(a.Name, searchAssmName));

            if (assmInfo == null)
            {
                Trace.WriteLine($"Ambiguity for '{searchAssmName}' is not resolved via exact match via '{Name}' resolver", Name);

                assmInfo = assmNames.FirstOrDefault(a => a.IsLoaded);

                if (assmInfo == null)
                {
                    assmInfo = assmNames.FirstOrDefault();

                    if (assmInfo != null)
                    {
                        Trace.WriteLine($"Ambiguity for '{searchAssmName}' is resolved by first assembly via '{Name}' resolver", Name);
                    }
                    else 
                    {
                        Trace.WriteLine($"Ambiguity for '{searchAssmName}' is not resolved via '{Name}' resolver", Name);
                    }
                }
                else 
                {
                    Trace.WriteLine($"Ambiguity for '{searchAssmName}' is resolved by first loaded assembly via '{Name}' resolver", Name);
                }
            }

            return assmInfo;
        }

        private bool CompareAssemblyNames(AssemblyName firstAssmName, AssemblyName secondAssmName)
            => string.Equals(firstAssmName.FullName, secondAssmName.FullName);

        private IEnumerable<AssemblyInfo> EnumerateAssemblyByName(string dir, bool recurse, AssemblyName searchAssmName)
        {
            foreach (var probeAssmFilePath in ProvideProbeAssemblyFilePaths(dir, searchAssmName))
            {
                if (File.Exists(probeAssmFilePath))
                {
                    var probeAssmName = AssemblyName.GetAssemblyName(probeAssmFilePath);

                    if (Match(probeAssmName, searchAssmName))
                    {
                        yield return new AssemblyInfo(probeAssmName, probeAssmFilePath, false);
                    }
                }
            }

            if (recurse)
            {
                foreach (var subDir in Directory.EnumerateDirectories(dir, "*.*", SearchOption.TopDirectoryOnly))
                {
                    foreach (var res in EnumerateAssemblyByName(subDir, recurse, searchAssmName))
                    {
                        yield return res;
                    }
                }
            }
        }

        /// <summary>
        /// Returnes probe assemly file paths
        /// </summary>
        /// <param name="dir">Directory to search in</param>
        /// <param name="searchAssmName">Target assembly name</param>
        /// <returns>Possible file paths of the assembly file</returns>
        protected virtual IEnumerable<string> ProvideProbeAssemblyFilePaths(string dir, AssemblyName searchAssmName) 
        {
            yield return Path.Combine(dir, searchAssmName.Name + ".dll");
        }
    }
}