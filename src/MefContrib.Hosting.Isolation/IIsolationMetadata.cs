namespace MefContrib.Hosting.Isolation
{
    public interface IIsolationMetadata
    {
        IsolationLevel Isolation { get; }

        string IsolationGroup { get; }
    }
}