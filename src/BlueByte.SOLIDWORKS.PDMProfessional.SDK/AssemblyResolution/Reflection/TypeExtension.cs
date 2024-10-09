//*********************************************************************
//xToolkit
//Copyright(C) 2023 Xarial Pty Limited
//Product URL: https://xtoolkit.xarial.com
//License: https://xtoolkit.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xarial.XToolkit.Reflection
{
    public static class TypeExtension
    {
        public class MissingMethodInfo : MethodInfo
        {
            public override string Name { get; }

            internal MissingMethodInfo(string name)
            {
                Name = name;
            }

            public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotImplementedException();
            public override MethodAttributes Attributes => throw new NotImplementedException();
            public override RuntimeMethodHandle MethodHandle => throw new NotImplementedException();
            public override Type DeclaringType => throw new NotImplementedException();
            public override Type ReflectedType => throw new NotImplementedException();
            public override MethodInfo GetBaseDefinition() => throw new NotImplementedException();
            public override object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException();
            public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotImplementedException();
            public override MethodImplAttributes GetMethodImplementationFlags() => throw new NotImplementedException();
            public override ParameterInfo[] GetParameters() => throw new NotImplementedException();
            public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotImplementedException();
            public override bool IsDefined(Type attributeType, bool inherit) => throw new NotImplementedException();
        }

        public static void InvokeMethod(object obj, MethodInfo method, params object[] args)
            => InvokeMethod<object>(obj, method, args);

        public static TRes InvokeMethod<TRes>(object obj, MethodInfo method, params object[] args)
        {
            if (method is MissingMethodInfo)
            {
                throw new NullReferenceException($"Method '{method.Name}' is not found");
            }

            var res = method.Invoke(obj, args);

            if (res != null)
            {
                if (res is TRes)
                {
                    return (TRes)res;
                }
                else if (res is IConvertible)
                {
                    return (TRes)Convert.ChangeType(res, typeof(TRes));
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
            else
            {
                return default(TRes);
            }
        }

        public static MethodInfo FindMethod(this Type type, string methodName,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = type.GetMethod(methodName, flags);

            if (method == null)
            {
                if (type.BaseType != null)
                {
                    method = FindMethod(type.BaseType, methodName, flags);
                }
            }

            if (method == null)
            {
                method = new MissingMethodInfo(methodName);
            }

            return method;
        }

        public static bool TryGetAttribute<TAtt>(this Type type, out TAtt att, bool searchInParentTypes = false)
            where TAtt : Attribute
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            att = null;

            var custAtts = type.GetCustomAttributes(typeof(TAtt), true);

            if (custAtts != null && custAtts.Any())
            {
                att = custAtts.First() as TAtt;
                return true;
            }
            else
            {
                if (searchInParentTypes)
                {
                    var baseType = type.BaseType;

                    if (baseType != null && baseType != typeof(object))
                    {
                        if (baseType.TryGetAttribute(out att, searchInParentTypes))
                        {
                            return true;
                        }
                    }

                    var interfaces = type.GetInterfaces();

                    foreach (var parent in interfaces)
                    {
                        if (parent.TryGetAttribute(out att, searchInParentTypes))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Checks if this type can be assigned to the generic type
        /// </summary>
        /// <param name="thisType">This type</param>
        /// <param name="genericType">Generic type</param>
        /// <returns>True if this type can be assigned to generic type</returns>
        public static bool IsAssignableToGenericType(this Type thisType, Type genericType)
            => thisType.TryFindGenericType(genericType) != null;

        public static Type[] GetArgumentsOfGenericType(this Type thisType, Type genericType)
        {
            var type = thisType.TryFindGenericType(genericType);

            if (type != null)
            {
                return type.GetGenericArguments();
            }
            else
            {
                return Type.EmptyTypes;
            }
        }

        public static Type TryFindGenericType(this Type thisType, Type genericType)
        {
            var interfaceTypes = thisType.GetInterfaces();

            bool CanCastFunc(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == genericType;

            foreach (var it in interfaceTypes)
            {
                if (CanCastFunc(it))
                {
                    return it;
                }
            }

            if (CanCastFunc(thisType))
            {
                return thisType;
            }

            var baseType = thisType.BaseType;

            if (baseType != null)
            {
                return baseType.TryFindGenericType(genericType);
            }

            return null;
        }

        /// <summary>
        /// Finds the method from the type if 
        /// </summary>
        /// <param name="type">Type to get method from</param>
        /// <param name="name">Name of the method</param>
        /// <param name="paramTypes">Parameter types of the method</param>
        /// <param name="bindingFlags">Binding flags</param>
        /// <returns>Method or null if not found</returns>
        /// <remarks>This method is similar to <see cref="Type.GetMethod(string)"/>, but allowing to specify the generic types definitions</remarks>
        public static MethodInfo GetMethodWithGenericParameters(this Type type, string name, Type[] paramTypes, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            if (paramTypes == null)
            {
                paramTypes = new Type[0];
            }

            var method = type.GetMethods(bindingFlags).Where(m => m.Name == name)
                .FirstOrDefault(m =>
                {
                    var parameters = m.GetParameters() ?? new ParameterInfo[0];

                    if (parameters.Length == paramTypes.Length)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var paramType = parameters[i].ParameterType;

                            if (paramType.IsGenericType)
                            {
                                paramType = paramType.GetGenericTypeDefinition();
                            }

                            if (paramType != paramTypes[i])
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

            return method;
        }

        public static Enum[] GetEnumFlags(this Type enumType)
        {
            if (!enumType.IsEnum) 
            {
                throw new Exception("Only flag enums are supported");
            }

            var flags = new List<Enum>();

            var flag = 0x1;

            foreach (Enum value in Enum.GetValues(enumType))
            {
                var bits = Convert.ToInt32(value);

                if (bits != 0)
                {
                    while (flag < bits)
                    {
                        flag <<= 1;
                    }
                    if (flag == bits)
                    {
                        flags.Add(value);
                    }
                }
            }

            return flags.ToArray();
        }
    }
}
