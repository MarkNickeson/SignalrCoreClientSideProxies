namespace _485f64dcc8cb49e6852679baae3d2f41
{
    public class ProxyImpl : ProxyGenTests.INoParamReturnTask
    {
        ProxyGen.SignalR.IHubBridge _hub;
        public ProxyImpl(ProxyGen.SignalR.IHubBridge hub)
        {
            _hub = hub;
        }
        public async System.Threading.Tasks.Task Foo()
        {
            await _hub.SendCoreAsync("Foo", new object[] { });
        }
    }
}

namespace _485f64dcc8cb49e6852679baae3d2f41
{
    public class FactoryImpl : ProxyGen.SignalR.IFactory<ProxyGenTests.INoParamReturnTask>
    {
        public ProxyGenTests.INoParamReturnTask Create(ProxyGen.SignalR.IHubBridge hub)
        {
            return new ProxyImpl(hub);
        }
    }
}