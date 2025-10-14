using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Xarial.XToolkit.Helpers
{
    /// <summary>
    /// Service to resolve the missing assembly references use in the <see cref="AssemblyResolver"/>
    /// </summary>
    public interface IReferenceResolver
    {
        /// <summary>
        /// Name of the resolver
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Resolves the missing assembly reference
        /// </summary>
        /// <param name="appDomain">Application domain</param>
        /// <param name="assmName">Assembly name to resolve</param>
        /// <param name="requestingAssembly">Assembly which requests the missing reference</param>
        /// <returns>Replacement assembly</returns>
        Assembly Resolve(AppDomain appDomain, AssemblyName assmName, Assembly requestingAssembly);
    }

    /// <summary>
    /// This is a helper class allowing to specify strategies for resolving the missing dlls
    /// </summary>
    public class AssemblyResolver : IDisposable
    {
        private readonly List<IReferenceResolver> m_AssemblyResolvers;
        private readonly AppDomain m_AppDomain;
        private readonly string m_LogName;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="appDomain">Application domain</param>
        public AssemblyResolver(AppDomain appDomain) : this(appDomain, "Xarial.xToolkit")
        {
        }

        /// <summary>
        /// Constructor with app domain and log name
        /// </summary>
        /// <param name="appDomain">Application domain</param>
        /// <param name="logName">Log name</param>
        public AssemblyResolver(AppDomain appDomain, string logName)
        {
            m_LogName = logName;
            m_AppDomain = appDomain;
            m_AssemblyResolvers = new List<IReferenceResolver>();

            m_AppDomain.AssemblyResolve += OnResolveMissingAssembly;
        }

        /// <summary>
        /// Addes the resolver service
        /// </summary>
        /// <param name="resolver">Resolver service</param>
        public void RegisterAssemblyReferenceResolver(IReferenceResolver resolver)
            => m_AssemblyResolvers.Add(resolver);

        private Assembly OnResolveMissingAssembly(object sender, ResolveEventArgs args)
        {
            var assmName = new AssemblyName(args.Name);

            // we also need to ignore the edrawings dlls
            if (string.IsNullOrEmpty(args.Name) == false && args.Name.Contains("edRemoteWindowProxy")) 
                //if the name contains edRemoteWindowProxy, don’t resolve it and return NULL.
            {
                return null;
            }
       

            if (!assmName.Name.EndsWith(".resources"))
            {
                foreach (var resolver in m_AssemblyResolvers)
                {
                    var assm = resolver.Resolve(m_AppDomain, assmName, args.RequestingAssembly);

                    if (assm != null)
                    {
                        Trace.WriteLine($"Assembly '{args.Name}' is resolved to '{assm.Location}' via '{resolver.Name}' resolver", m_LogName);
                        return assm;
                    }
                    else
                    {
                        Trace.WriteLine($"Assembly '{args.Name}' is not resolved via '{resolver.Name}' resolver", m_LogName);
                    }
                }
            }

            return null;
        }

        public void Dispose()
        {
            m_AppDomain.AssemblyResolve -= OnResolveMissingAssembly;
        }
    }
}
