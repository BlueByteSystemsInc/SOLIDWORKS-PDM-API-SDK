//*********************************************************************
//xToolkit
//Copyright(C) 2023 Xarial Pty Limited
//Product URL: https://xtoolkit.xarial.com
//License: https://xtoolkit.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Xarial.XToolkit.Reflection
{
    public static class Lambda
    {
        /// <summary>
        /// Invokes generic method by supplying the type arguments
        /// </summary>
        /// <param name="method">Method body to invoke</param>
        /// <param name="typeArguments">Type arguments</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <remarks>Method body within expression must only contain constant values or variables (no expression). Use cast to match the parameter type (actual cast wil lnot be called)</remarks>
        public static object InvokeGenericMethod(Expression<Action> method, params Type[] typeArguments)
        {
            var methodCall = (MethodCallExpression)method.Body;

            if (methodCall.Method.IsGenericMethod)
            {
                var regInitMethod = methodCall.Method.GetGenericMethodDefinition().MakeGenericMethod(typeArguments);

                var target = ExtractInstance(methodCall.Object);

                var args = methodCall.Arguments.Select(a => ExtractInstance(a)).ToArray();

                return regInitMethod.Invoke(target, args);
            }
            else
            {
                throw new NotSupportedException("Non generic method");
            }
        }

        private static object ExtractInstance(Expression exp)
        {
            if (exp is MemberExpression)
            {
                var memberExpr = (MemberExpression)exp;

                var constantExpr = (ConstantExpression)memberExpr.Expression;

                var classInstance = constantExpr.Value;

                var calledClassField = (FieldInfo)memberExpr.Member;

                var inst = calledClassField.GetValue(classInstance);

                return inst;
            }
            else if (exp is UnaryExpression)
            {
                var unaryExp = (UnaryExpression)exp;

                return ExtractInstance(unaryExp.Operand);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
