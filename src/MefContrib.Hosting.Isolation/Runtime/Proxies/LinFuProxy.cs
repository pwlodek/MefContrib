
namespace MefContrib.Hosting.Isolation.Runtime.Proxies
{
    using LinFu.DynamicProxy;

    public class LinFuProxy : IInterceptor
    {
        private readonly ObjectReference _objectReference;

        public LinFuProxy(ObjectReference objectReference)
        {
            _objectReference = objectReference;
        }

        public object Intercept(InvocationInfo info)
        {
            return ProxyServices.InvokeMember(
                _objectReference, info.TargetMethod, info.Arguments);
        }
    }
}