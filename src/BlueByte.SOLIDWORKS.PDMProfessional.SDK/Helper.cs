using System;
using System.Collections.Generic;
using System.Reflection;


namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK
{







    internal static class Helper


        
 
    {
        internal static T GetFirstAttribute<T>(object source) where T : Attribute
        {

            var attributes = source.GetType().GetCustomAttributes();
            
                if (attributes != null)
                {
                    foreach (var attribute in attributes)
                    {
                        if (attribute is T)
                            return attribute as T;
                    }
                }
            
            return null;
        }

      


        internal static T[] GetAttributes<T>(object source) where T : Attribute
        {
                var list = new List<T>();
                var attributes = source.GetType().GetCustomAttributes();
                if (attributes != null)
                {
                    foreach (var attribute in attributes)
                    {
                        if (attribute is T)
                            list.Add(attribute as T);
                    }
                }
           
            return list.ToArray();
        }
    }
}
