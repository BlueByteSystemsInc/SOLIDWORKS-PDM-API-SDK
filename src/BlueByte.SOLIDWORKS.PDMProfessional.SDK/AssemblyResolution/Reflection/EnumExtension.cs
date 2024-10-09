//*********************************************************************
//xToolkit
//Copyright(C) 2023 Xarial Pty Limited
//Product URL: https://xtoolkit.xarial.com
//License: https://xtoolkit.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xarial.XToolkit.Reflection
{
    public static class EnumExtension
    {
        public static bool TryGetAttribute<TAtt>(this Enum enumer, Action<TAtt> attProc)
            where TAtt : Attribute
        {
            var enumType = enumer.GetType();
            var enumField = enumType.GetMember(enumer.ToString()).FirstOrDefault();

            var atts = enumField?.GetCustomAttributes(typeof(TAtt), false);

            if (atts != null && atts.Any())
            {
                var att = atts.First() as TAtt;
                attProc.Invoke(att);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
