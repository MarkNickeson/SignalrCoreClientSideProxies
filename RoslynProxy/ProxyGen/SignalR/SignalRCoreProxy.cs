using CodeGenHelper;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProxyGen.SignalR
{    
    public static class SignalRCoreProxy
    {
        static Type taskType = typeof(Task);

        public static TServer CreateServerProxy<TServer>(IHubBridge hub) 
            where TServer : class
        {
            Validate(typeof(TServer));

            var factory = ProxyGenerator.Create<IFactory<TServer>, TServer>(ServerProxyCodeGen);

            return factory.Create(hub);
        }

        internal static void Validate(Type serverType)
        {
            // expect interface
            if (!serverType.IsInterface)
            {
                throw new ApplicationException("TServer is not an interface");
            }

            // expect methods only
            foreach(var m in serverType.GetMembers())
            {
                if (m.MemberType!= MemberTypes.Method)
                {
                    throw new ApplicationException("TServer must contain methods only");
                }
            }

            // expect all methods to have return type derived from Task

            foreach(var m in serverType.GetMethods())
            {
                if (!taskType.IsAssignableFrom(m.ReturnType))
                {
                    throw new ApplicationException("TServer methods must return Task or Task<TResult> to enable async/await");
                }
            }
        }

        internal static void ServerProxyCodeGen(StringBuilder builder, MethodInfo methodInfo)
        {
            // for each method want to emit code as:
            // public async {Method.ReturnType} {Method.Name}({ params })
            // {
            //     return await hub.InvokeAsyncCore(methodName, returnType, args, ct);
            //     or
            //     await hubConnection.SendAsyncCore(methodName, args, ct);
            // }

            if (methodInfo.ReturnType==taskType)
            {
                // emit SendAsyncCore
                builder.Append($"await _hub.SendCoreAsync(");
                builder.Append($"\"{methodInfo.Name}\"");
                builder.Append(",new object[]{");
                bool first = true;
                foreach(var p in methodInfo.GetParameters())
                {
                    if (!first) builder.Append(",");
                    first = false;
                    builder.Append(p.Name);
                }
                builder.Append("}");// end of object[]
                // ignoring cancellation token
                builder.Append(");"); // end of SendCoreAsync
            }
            else
            {
                // emit InvokeAsyncCore
                // difference here is that InvokeAsyncCore returns Task<object> so
                // must await to capture object then cast and return
                builder.Append($"var temp = await _hub.InvokeCoreAsync(\"");
                builder.Append(methodInfo.Name);
                // Return type is assumed to be Task<T>, but T can still be generic so unroll it
                var taskResultType = methodInfo.ReturnType.GetGenericArguments()[0];

                if (taskResultType.IsGenericType)
                {
                    // Task<T> where T is also generic
                    var genericType = GenericTypeUtils.UnrollGenericTypeToString(taskResultType);
                    builder.Append($"\",typeof({genericType})");
                }
                else
                {
                    // Task<T> where T is not generic
                    builder.Append($"\",typeof({taskResultType.FullName})");
                }
                
                // box the method parameters
                builder.Append(",new object[]{");
                bool first = true;
                foreach (var p in methodInfo.GetParameters())
                {
                    if (!first) builder.Append(",");
                    first = false;
                    builder.Append(p.Name);
                }
                builder.Append("}");// end of object[]
                // ignore cancellation token
                builder.AppendLine(");"); // end of Invoke

                // cast the return value 
                if (taskResultType.IsGenericType)
                {
                    var returnType = GenericTypeUtils.UnrollGenericTypeToString(taskResultType);
                    builder.AppendLine($"return ({returnType})temp;");
                }
                else
                {
                    var returnType = taskResultType.FullName;
                    builder.AppendLine($"return ({returnType})temp;");
                }                                
            }
        }
    }
}
