namespace Gress.Utils;

// Container for a value that may or may not be set.
// Essentially Nullable<T>, but for cases where null is also a valid value.
internal class Box<T>
{
    private bool _isValueSet;
    private T _value = default!;

    public void Store(T value)
    {
        _value = value;
        _isValueSet = true;
    }

    public bool TryOpen(out T value)
    {
        if (_isValueSet)
        {
            value = _value;
            return true;
        }

        value = default!;
        return false;
    }
}