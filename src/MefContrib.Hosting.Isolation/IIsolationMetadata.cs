namespace MefContrib.Hosting.Isolation
{
    public interface IIsolationMetadata
    {
        IsolationLevel Isolation { get; }

        bool HostPerInstance { get; }

        string IsolationGroup { get; }
    }
}