using System;
using System.Collections;
using System.Collections.Generic;

namespace hashes;

public class ReadonlyBytes : IEnumerable<byte>
{
    private readonly byte[] bytes;
    private readonly int hash;
    public int Length { get; }

    public ReadonlyBytes(params byte[] bytes)
    {
        this.bytes = bytes ?? throw new ArgumentNullException();
        this.bytes = bytes;
        Length = bytes.Length;
        unchecked
        {
            const int prime = 0x01000193;
            var h = 0x811c9dc5;
            foreach (var b in bytes)
            {
                h = h * prime ^ b;
            }

            hash = (int)h;
        }
    }

    public byte this[int i]
    {
        get
        {
            if (i < 0 || i > Length - 1) throw new IndexOutOfRangeException();
            return bytes[i];
        }
    }

    public IEnumerator<byte> GetEnumerator() => ((IEnumerable<byte>)bytes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override bool Equals(object obj)
    {
        var other = obj as ReadonlyBytes;
        if (other == this)
        {
            return true;
        }

        if (other?.Length != Length || other.GetType() != GetType())
        {
            return false;
        }

        for (int i = 0; i < other.Length; i++)
        {
            if (other.bytes[i] != bytes[i])
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode() => hash;

    public override string ToString() => $"[{string.Join(", ", bytes)}]";
}