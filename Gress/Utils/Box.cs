namespace Gress.Utils;

// Like Nullable<T> but for reference types that can themselves be null
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