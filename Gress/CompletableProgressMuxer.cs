namespace Gress;

public class CompletableProgressMuxer
{
    private readonly object _lock = new();
    private readonly ProgressMuxer _muxer;

    private int _pendingInputs;

    public CompletableProgressMuxer(ProgressMuxer muxer) =>
        _muxer = muxer;

    private void HandleCompletion()
    {
        lock (_lock)
        {
            if (--_pendingInputs <= 0)
            {
                _muxer.ClearInputs();
            }
        }
    }

    public ICompletableProgress<Percentage> CreateInput(double weight = 1.0)
    {
        lock (_lock)
        {
            _pendingInputs++;

            return new CompletableProgress<Percentage>(
                _muxer.CreateInput(weight).Report,
                HandleCompletion
            );
        }
    }
}

public static class CompletableProgressMuxerExtensions
{
    public static CompletableProgressMuxer CreateCompletableProgressMuxer(this ProgressMuxer muxer) => new(muxer);
}