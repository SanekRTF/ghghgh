using System;
using System.Collections;
using System.Collections.Generic;

namespace hashes;

public class ReadonlyBytes : IEnumerable<byte>
{
    public int Length => byteArray.Length;
    private readonly byte[] byteArray;
    private int cachedHashCode;

    public ReadonlyBytes(params byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        byteArray = new byte[bytes.Length];
        for (var i = 0; i < Length; i++)
            byteArray[i] = bytes[i];
        cachedHashCode = ComputeFNVHashCode();
    }

    public byte this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException();
            return byteArray[index];
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(ReadonlyBytes))
            return false;
        var other = obj as ReadonlyBytes;
        if (other.Length != Length)
            return false;
        for (var i = 0; i < Length; i++)
            if (byteArray[i] != other[i])
                return false;
        return true;
    }

    public override int GetHashCode() => cachedHashCode;

    public override string ToString()
    {
        return "[" + string.Join(", ", byteArray) + "]";
    }

    public IEnumerator<byte> GetEnumerator()
    {
        for (var i = 0; i < Length; i++)
            yield return byteArray[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private int ComputeFNVHashCode()
    {
        var hash = 0;
        var prime = 0x01000193;
        foreach (var byteValue in byteArray)
        {
            hash = unchecked(hash * prime);
            hash ^= byteValue;
        }
        return hash;
    }
}
