namespace ClientSideProxyHelper.CodeGen
{
    public interface IServerProxyFactory<T> where T : class
    {
        T Create(IHubConnectionBridge hub);
    }
}
