namespace MefContrib.Hosting.Isolation.Runtime.Proxies
{
    using System;
    using LinFu.DynamicProxy;

    public class LinFuProxy : IInterceptor
    {
        private readonly ObjectReference _objectReference;

        public LinFuProxy(ObjectReference objectReference)
        {
            if (objectReference == null)
            {
                throw new ArgumentNullException("objectReference");
            }

            _objectReference = objectReference;
        }

        public object Intercept(InvocationInfo info)
        {
            return PartHost.InvokeMember(
                _objectReference, info.TargetMethod, info.Arguments);
        }
    }
}