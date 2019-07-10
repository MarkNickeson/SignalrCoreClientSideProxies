using System;
using System.Text;

namespace CodeGenHelper
{
    public static class GenericTypeUtils
    {
        public static string UnrollGenericTypeToString(Type type)
        {
            if (!type.IsGenericType) throw new ApplicationException("Not a generic type");

            var sb = new StringBuilder();
            PeelGenericLayer(type, sb);
            return sb.ToString();
        }

        // recursively invoked in depth first fashion for total type extract
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
