//*********************************************************************
//xToolkit
//Copyright(C) 2023 Xarial Pty Limited
//Product URL: https://xtoolkit.xarial.com
//License: https://xtoolkit.xarial.com/license/
//*********************************************************************

using System;
using System.Linq;
using System.Reflection;

namespace Xarial.XToolkit.Reflection
{
    public static class AssemblyExtension
    {
        public static bool TryGetAttribute<TAtt>(this Assembly assm, out TAtt att)
            where TAtt : Attribute
        {
            TAtt localAtt = null;

            assm.WithAttribute<TAtt>(a => localAtt = a);

            att = localAtt;

            return localAtt != null;
        }

        public static void WithAttribute<TAtt>(this Assembly assm, Action<TAtt> action)
            where TAtt : Attribute
        {
            if (assm == null)
            {
                throw new ArgumentNullException(nameof(assm));
            }
            
            var custAtts = assm.GetCustomAttributes(typeof(TAtt), true);

            if (custAtts != null && custAtts.Any())
            {
                foreach (TAtt att in custAtts)
                {
                    action.Invoke(att);
                }
            }
        }
    }
}
