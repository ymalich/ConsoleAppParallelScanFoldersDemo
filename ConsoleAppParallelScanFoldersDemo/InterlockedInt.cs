namespace ConsoleAppPathScan1;

internal sealed class InterlockedInt
{
    private int _value;

    public int Value => _value;

    public InterlockedInt()
    {
    }

    public InterlockedInt(int initValue)
    {
        _value = initValue;
    }

    public void Reset()
    {
        _value = 0;
    }

    public int Inc() => Interlocked.Increment(ref _value);

    public int Dec() => Interlocked.Decrement(ref _value);

    public int Add(int val) => Interlocked.Add(ref _value, val);

    public static InterlockedInt operator +(InterlockedInt a, InterlockedInt b)
        => new InterlockedInt(a.Value + b.Value);

    public static InterlockedInt operator -(InterlockedInt a, InterlockedInt b)
        => new InterlockedInt(a.Value - b.Value);

    public static bool operator ==(InterlockedInt a, int b)
        => a._value == b;

    public static bool operator !=(InterlockedInt a, int b)
        => a._value != b;

    public static bool operator <=(InterlockedInt a, int b)
    => a._value <= b;

    public static bool operator >=(InterlockedInt a, int b)
        => a._value >= b;

    public static bool operator <(InterlockedInt a, int b)
        => a._value < b;

    public static bool operator >(InterlockedInt a, int b)
        => a._value > b;

    public override string ToString() => _value.ToString();

    public string ToString(string format) => _value.ToString(format);

    public string ToString(string format, IFormatProvider provider) => _value.ToString(format, provider);

    public override bool Equals(object? obj) => _value.Equals(obj);

    public override int GetHashCode() => _value.GetHashCode();
}