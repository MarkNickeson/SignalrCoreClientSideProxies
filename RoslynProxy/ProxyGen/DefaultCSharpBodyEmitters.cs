using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ProxyGen
{
    public static class DefaultCSharpBodyEmitters
    {
        public static void Empty(StringBuilder builder, MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(void))
            {
                builder.AppendLine($"return default({methodInfo.ReturnType.FullName});");
            }
        }

        public static void PassThroughTrace(StringBuilder builder, MethodInfo methodInfo)
        {
            builder.AppendLine("Trace.WriteLine(\"Entering: {methodInfo.Name}\");");

            if (methodInfo.ReturnType == typeof(void))
            {
                // have to assume field name for the target instance (essentially the name of the
                // parameter used by the associate factory)
                // emit instance.MethodName(.....parameters.....);
            }
            else
            {
                // emit return instance.MethodName(.....parameters.....);
            }

            builder.AppendLine("Trace.WriteLine(\"Existing: {methodInfo.Name}\");");

            
        }
    }
}
