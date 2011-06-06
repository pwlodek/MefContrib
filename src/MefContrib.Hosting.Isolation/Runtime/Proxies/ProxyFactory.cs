namespace MefContrib.Hosting.Isolation.Runtime.Proxies
{
    public static class ProxyFactory
    {
        public static IProxyFactory GetFactory()
        {
            return LinFuProxyFactory.Default;
        }
    }
}