//*********************************************************************
//xToolkit
//Copyright(C) 2023 Xarial Pty Limited
//Product URL: https://xtoolkit.xarial.com
//License: https://xtoolkit.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Xarial.XToolkit.Reflection
{
    /// <summary>
    /// Simple resolve for the assembly references
    /// </summary>
    /// <remarks>Resolver will consider the following rules to load missing references
    /// 1 - Loading local references only (if requesting assembly is in the working directory)
    /// 2 - Loading target or newer version
    /// 3 - Loading from work directory and sub-directories
    /// 4 - Loading matched name, public key and culture
    /// 5 - Loading nearest available version</remarks>
    public class AssemblyReferenceResolver : IDisposable
    {
        private class AssemblyNameEqualityComparer : IEqualityComparer<AssemblyName>
        {
            public bool Equals(AssemblyName x, AssemblyName y)
                => string.Equals(x.FullName, y.FullName);

            public int GetHashCode(AssemblyName obj) => 0;
        }

        private readonly AppDomain m_AppDomain;

        private readonly string m_WorkDir;
        private readonly string m_LogName;

        private readonly Dictionary<AssemblyName, Assembly> m_CachedAssemblies;

        private readonly IEqualityComparer<AssemblyName> m_AssmNameEqualityComparer;

        /// <summary>
        /// Starts monitoring and loading missin assemblies
        /// </summary>
        /// <param name="appDomain">Application domain</param>
        /// <param name="workDir">Working directories to look for the assmebly to load</param>
        /// <param name="logName">Name of the trace log</param>
        public AssemblyReferenceResolver(AppDomain appDomain, string workDir, string logName)
        {
            m_WorkDir = workDir;

            m_AssmNameEqualityComparer = new AssemblyNameEqualityComparer();
            m_CachedAssemblies = new Dictionary<AssemblyName, Assembly>(m_AssmNameEqualityComparer);

            m_LogName = logName;

            m_AppDomain = appDomain;

            m_AppDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assmName = new AssemblyName(args.Name);

            var requestingAssm = args.RequestingAssembly;

            if (!assmName.Name.EndsWith(".resources")
                && (requestingAssm == null || IsInDirectory(requestingAssm.Location, m_WorkDir)))
            {
                return Resolve(assmName);
            }
            else
            {
                return null;
            }
        }

        private Assembly Resolve(AssemblyName searchAssmName)
        {
            if (!m_CachedAssemblies.TryGetValue(searchAssmName, out var assm))
            {
                var matchedAssmNames = new List<AssemblyName>();

                foreach (var assmName in EnumerateAssemblyByName(m_WorkDir, true, searchAssmName))
                {
                    if (m_AssmNameEqualityComparer.Equals(assmName, searchAssmName))
                    {
                        Trace($"Loading '{searchAssmName}' from '{assmName.CodeBase}' as exact match");

                        assm = LoadAssembly(assmName);
                        break;
                    }
                    else
                    {
                        if (!matchedAssmNames.Contains(assmName, m_AssmNameEqualityComparer))
                        {
                            matchedAssmNames.Add(assmName);
                        }
                    }
                }

                if (assm == null)
                {
                    var targAssmName = ResolveAmbiguity(matchedAssmNames, searchAssmName);

                    if (targAssmName != null)
                    {
                        assm = LoadAssembly(targAssmName);
                    }
                    else
                    {
                        assm = null;
                    }
                }

                m_CachedAssemblies.Add(searchAssmName, assm);

                return assm;
            }
            else
            {
                return assm;
            }
        }

        private Assembly LoadAssembly(AssemblyName assmName)
        {
            Trace($"Loading assembly '{assmName}'");

            return Assembly.Load(assmName);
        }

        private AssemblyName ResolveAmbiguity(IReadOnlyList<AssemblyName> assmNames, AssemblyName searchAssmName)
        {
            Trace($"Resolving ambiguity for '{searchAssmName}'");

            var targAssmName = assmNames.FirstOrDefault(a => m_AssmNameEqualityComparer.Equals(a, searchAssmName));

            if (targAssmName != null)
            {
                Trace($"Ambiguity for '{searchAssmName}' is resolved by exact match");
            }
            else
            {
                targAssmName = assmNames.OrderBy(a => a.Version).FirstOrDefault();

                if (targAssmName != null)
                {
                    Trace($"Ambiguity for '{searchAssmName}' is resolved by nearest available version of the assembly");
                }
                else
                {
                    Trace($"Ambiguity for '{searchAssmName}' is not resolved");
                }
            }

            return targAssmName;
        }

        private IEnumerable<AssemblyName> EnumerateAssemblyByName(string dir, bool recurse, AssemblyName searchAssmName)
        {
            var probeAssmFilePath = Path.Combine(dir, searchAssmName.Name + ".dll");

            if (File.Exists(probeAssmFilePath))
            {
                var probeAssmName = AssemblyName.GetAssemblyName(probeAssmFilePath);

                if (Matches(probeAssmName, searchAssmName))
                {
                    yield return probeAssmName;
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

        private bool Matches(AssemblyName probeAssmName, AssemblyName searchAssmName)
        {
            return (probeAssmName.Name == searchAssmName.Name)
                    && (GetPublicKeyTokenString(probeAssmName) == GetPublicKeyTokenString(searchAssmName))
                    && (probeAssmName.CultureName == probeAssmName.CultureName)
                    && (probeAssmName.Version >= searchAssmName.Version);
        }

        private string GetPublicKeyTokenString(AssemblyName assmName)
        {
            var token = assmName.GetPublicKeyToken();

            if (token != null)
            {
                return string.Join("", token.Select(t => string.Format("{0:x2}", t)));
            }
            else
            {
                return "";
            }
        }

        private bool IsInDirectory(string thisPath, string parentDir)
        {
            if (!parentDir.EndsWith("\\"))
            {
                parentDir = parentDir + "\\";
            }

            return thisPath.StartsWith(parentDir, StringComparison.CurrentCultureIgnoreCase);
        }

        private void Trace(string message)
            => System.Diagnostics.Trace.WriteLine(message, m_LogName);

        public void Dispose()
        {
            m_AppDomain.AssemblyResolve -= OnAssemblyResolve;
        }
    }
}