using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyGen
{
    public static class GenericTypeUtils
    {
        public static string UnrollGenericType(Type type)
        {
            if (!type.IsGenericType) throw new ApplicationException("type is not generic");

            var sb = new StringBuilder();

            PeelGenericLayer(type, sb);
            return sb.ToString();
        }

        static void PeelGenericLayer(Type type, StringBuilder builder)
        {
            // write the generic suffix portion (eg "IFoo<")
            builder.Append(type.Namespace);
            builder.Append('.');
            builder.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
            builder.Append("<");
            // get the generic parameters
            bool first = true;
            foreach(var argType in type.GetGenericArguments())
            {
                if (!first) builder.Append(",");
                first = false;
                if (argType.IsGenericType)
                {
                    // recursively descend
                    PeelGenericLayer(argType, builder);
                }
                else
                {
                    // write the type 
                    builder.Append(argType.FullName);
                }
            }
            builder.Append(">");
        }
    }
}
