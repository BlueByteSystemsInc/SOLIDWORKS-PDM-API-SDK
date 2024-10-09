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
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Xarial.XToolkit.Helpers;

namespace Xarial.XToolkit.Reflection
{
    public static class AppDomainExtension
    {
        private static readonly Dictionary<int, AssemblyResolver> m_DomainsAssemblyResolvers;
        private static readonly object m_Lock;

        static AppDomainExtension()
        {
            m_DomainsAssemblyResolvers = new Dictionary<int, AssemblyResolver>();
            m_Lock = new object();
        }

        public static void RegisterGlobalAssemblyReferenceResolver(this AppDomain appDomain,
            IReferenceResolver resolver)
        {
            lock (m_Lock)
            {
                if (!m_DomainsAssemblyResolvers.TryGetValue(appDomain.Id, out AssemblyResolver assmResolver))
                {
                    assmResolver = new AssemblyResolver(appDomain);
                    m_DomainsAssemblyResolvers.Add(appDomain.Id, assmResolver);
                }

                assmResolver.RegisterAssemblyReferenceResolver(resolver);
            }
        }
    }
}