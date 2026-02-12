using System;
using System.Collections;
using System.Threading;

namespace Engine
{
  [Serializable]
  public sealed class BitArrayEx : ICollection, IEnumerable, ICloneable
  {
    private int[] m_array;
    private int m_length;
    private int _version;
    [NonSerialized]
    private object _syncRoot;
    private const int _ShrinkThreshold = 256;
    private const int BitsPerInt32 = 32;
    private const int BytesPerInt32 = 4;
    private const int BitsPerByte = 8;

    public BitArrayEx(int length)
      : this(length, false)
    {
    }

    public BitArrayEx(int length, bool defaultValue)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), (object) length, "Non-negative number required.");
      this.m_array = new int[BitArrayEx.GetArrayLength(length, 32)];
      this.m_length = length;
      int num = defaultValue ? -1 : 0;
      for (int index = 0; index < this.m_array.Length; ++index)
        this.m_array[index] = num;
      this._version = 0;
    }

    public BitArrayEx(byte[] bytes)
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      if (bytes.Length > 268435455)
          LogUtils.LogWarning("The input array length must not exceed Int32.MaxValue. Otherwise BitArrayEx.Length would exceed Int32.MaxValue.");
      this.m_array = new int[BitArrayEx.GetArrayLength(bytes.Length, 4)];
      this.m_length = bytes.Length * 8;
      int index1 = 0;
      int index2;
      for (index2 = 0; bytes.Length - index2 >= 4; index2 += 4)
        this.m_array[index1++] = (int) bytes[index2] & (int) byte.MaxValue | ((int) bytes[index2 + 1] & (int) byte.MaxValue) << 8 | ((int) bytes[index2 + 2] & (int) byte.MaxValue) << 16 | ((int) bytes[index2 + 3] & (int) byte.MaxValue) << 24;
      switch (bytes.Length - index2)
      {
        case 1:
          this.m_array[index1] |= (int) bytes[index2] & (int) byte.MaxValue;
          break;
        case 2:
          this.m_array[index1] |= ((int) bytes[index2 + 1] & (int) byte.MaxValue) << 8;
          goto case 1;
        case 3:
          this.m_array[index1] = ((int) bytes[index2 + 2] & (int) byte.MaxValue) << 16;
          goto case 2;
      }
      this._version = 0;
    }

    public BitArrayEx(bool[] values)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      this.m_array = new int[BitArrayEx.GetArrayLength(values.Length, 32)];
      this.m_length = values.Length;
      for (int index = 0; index < values.Length; ++index)
      {
        if (values[index])
          this.m_array[index / 32] |= 1 << index % 32;
      }
      this._version = 0;
    }

    public BitArrayEx(int[] values)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (values.Length > 67108863)
        LogUtils.LogWarning("The input array length must not exceed Int32.MaxValue / {0}. Otherwise BitArrayEx.Length would exceed Int32.MaxValue.");
      this.m_array = new int[values.Length];
      Array.Copy((Array) values, 0, (Array) this.m_array, 0, values.Length);
      this.m_length = values.Length * 32;
      this._version = 0;
    }

    public BitArrayEx(BitArrayEx bits)
    {
      if (bits == null)
        throw new ArgumentNullException(nameof (bits));
      int arrayLength = BitArrayEx.GetArrayLength(bits.m_length, 32);
      this.m_array = new int[arrayLength];
      Array.Copy((Array) bits.m_array, 0, (Array) this.m_array, 0, arrayLength);
      this.m_length = bits.m_length;
      this._version = bits._version;
    }

    public bool this[int index]
    {
      get
      {
        return this.Get(index);
      }
      set
      {
        this.Set(index, value);
      }
    }

    public bool Get(int index)
    {
      if (index < 0 || index >= this.Length)
        throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Index was out of range. Must be non-negative and less than the size of the collection.");
      return (uint) (this.m_array[index / 32] & 1 << index % 32) > 0U;
    }

    public void Set(int index, bool value)
    {
      if (index < 0 || index >= this.Length)
        throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Index was out of range. Must be non-negative and less than the size of the collection.");
      if (value)
        this.m_array[index / 32] |= 1 << index % 32;
      else
        this.m_array[index / 32] &= ~(1 << index % 32);
      ++this._version;
    }

    public void SetAll(bool value)
    {
      int num = value ? -1 : 0;
      int arrayLength = BitArrayEx.GetArrayLength(this.m_length, 32);
      for (int index = 0; index < arrayLength; ++index)
        this.m_array[index] = num;
      ++this._version;
    }

    public BitArrayEx And(BitArrayEx value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (this.Length != value.Length)
        throw new ArgumentException("Array lengths must be the same.");
      int arrayLength = BitArrayEx.GetArrayLength(this.m_length, 32);
      for (int index = 0; index < arrayLength; ++index)
        this.m_array[index] &= value.m_array[index];
      ++this._version;
      return this;
    }

    public BitArrayEx Or(BitArrayEx value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (this.Length != value.Length)
        throw new ArgumentException("Array lengths must be the same.");
      int arrayLength = BitArrayEx.GetArrayLength(this.m_length, 32);
      for (int index = 0; index < arrayLength; ++index)
        this.m_array[index] |= value.m_array[index];
      ++this._version;
      return this;
    }

    public BitArrayEx Xor(BitArrayEx value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (this.Length != value.Length)
        throw new ArgumentException("Array lengths must be the same.");
      int arrayLength = BitArrayEx.GetArrayLength(this.m_length, 32);
      for (int index = 0; index < arrayLength; ++index)
        this.m_array[index] ^= value.m_array[index];
      ++this._version;
      return this;
    }

    public BitArrayEx Not()
    {
      int arrayLength = BitArrayEx.GetArrayLength(this.m_length, 32);
      for (int index = 0; index < arrayLength; ++index)
        this.m_array[index] = ~this.m_array[index];
      ++this._version;
      return this;
    }

    public BitArrayEx RightShift(int count)
    {
      if (count <= 0)
      {
        if (count < 0)
          throw new ArgumentOutOfRangeException(nameof (count), (object) count, "Non-negative number required.");
        ++this._version;
        return this;
      }
      int index = 0;
      int arrayLength = BitArrayEx.GetArrayLength(this.m_length, 32);
      if (count < this.m_length)
      {
        int sourceIndex = count / 32;
        int num1 = count - sourceIndex * 32;
        if (num1 == 0)
        {
          uint num2 = uint.MaxValue >> 32 - this.m_length % 32;
          this.m_array[arrayLength - 1] &= (int) num2;
          Array.Copy((Array) this.m_array, sourceIndex, (Array) this.m_array, 0, arrayLength - sourceIndex);
          index = arrayLength - sourceIndex;
        }
        else
        {
          int num2 = arrayLength - 1;
          while (sourceIndex < num2)
          {
            uint num3 = (uint) this.m_array[sourceIndex] >> num1;
            int num4 = this.m_array[++sourceIndex] << 32 - num1;
            this.m_array[index++] = num4 | (int) num3;
          }
          uint num5 = uint.MaxValue >> 32 - this.m_length % 32 & (uint) this.m_array[sourceIndex];
          this.m_array[index++] = (int) (num5 >> num1);
        }
      }
      Array.Clear((Array) this.m_array, index, arrayLength - index);
      ++this._version;
      return this;
    }

    public BitArrayEx LeftShift(int count)
    {
      if (count <= 0)
      {
        if (count < 0)
          throw new ArgumentOutOfRangeException(nameof (count), (object) count, "Non-negative number required.");
        ++this._version;
        return this;
      }
      int num1;
      if (count < this.m_length)
      {
        int index1 = (this.m_length - 1) / 32;
        num1 = count / 32;
        int num2 = count - num1 * 32;
        if (num2 == 0)
        {
          Array.Copy((Array) this.m_array, 0, (Array) this.m_array, num1, index1 + 1 - num1);
        }
        else
        {
          int index2 = index1 - num1;
          while (index2 > 0)
          {
            int num3 = this.m_array[index2] << num2;
            uint num4 = (uint) this.m_array[--index2] >> 32 - num2;
            this.m_array[index1] = num3 | (int) num4;
            --index1;
          }
          this.m_array[index1] = this.m_array[index2] << num2;
        }
      }
      else
        num1 = BitArrayEx.GetArrayLength(this.m_length, 32);
      Array.Clear((Array) this.m_array, 0, num1);
      ++this._version;
      return this;
    }

    public int Length
    {
      get
      {
        return this.m_length;
      }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException(nameof (value), (object) value, "Non-negative number required.");
        int arrayLength = BitArrayEx.GetArrayLength(value, 32);
        if (arrayLength > this.m_array.Length || arrayLength + 256 < this.m_array.Length)
          Array.Resize<int>(ref this.m_array, arrayLength);
        if (value > this.m_length)
        {
          int index = BitArrayEx.GetArrayLength(this.m_length, 32) - 1;
          int num = this.m_length % 32;
          if (num > 0)
            this.m_array[index] &= (1 << num) - 1;
          Array.Clear((Array) this.m_array, index + 1, arrayLength - index - 1);
        }
        this.m_length = value;
        ++this._version;
      }
    }

    public void CopyTo(Array array, int index)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Non-negative number required.");
      if (array.Rank != 1)
        throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof (array));
      switch (array)
      {
        case int[] numArray2:
          int index1 = BitArrayEx.GetArrayLength(this.m_length, 32) - 1;
          int num1 = this.m_length % 32;
          if (num1 == 0)
          {
            Array.Copy((Array) this.m_array, 0, (Array) numArray2, index, BitArrayEx.GetArrayLength(this.m_length, 32));
            break;
          }
          Array.Copy((Array) this.m_array, 0, (Array) numArray2, index, BitArrayEx.GetArrayLength(this.m_length, 32) - 1);
          numArray2[index + index1] = this.m_array[index1] & (1 << num1) - 1;
          break;
        case byte[] _:
          int num2 = this.m_length % 8;
          int arrayLength = BitArrayEx.GetArrayLength(this.m_length, 8);
          if (array.Length - index < arrayLength)
            throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
          if (num2 > 0)
            --arrayLength;
          byte[] numArray1 = (byte[]) array;
          for (int index2 = 0; index2 < arrayLength; ++index2)
            numArray1[index + index2] = (byte) (this.m_array[index2 / 4] >> index2 % 4 * 8 & (int) byte.MaxValue);
          if (num2 <= 0)
            break;
          int num3 = arrayLength;
          numArray1[index + num3] = (byte) (this.m_array[num3 / 4] >> num3 % 4 * 8 & (1 << num2) - 1);
          break;
        case bool[] _:
          if (array.Length - index < this.m_length)
            throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
          bool[] flagArray = (bool[]) array;
          for (int index2 = 0; index2 < this.m_length; ++index2)
            flagArray[index + index2] = (uint) (this.m_array[index2 / 32] >> index2 % 32 & 1) > 0U;
          break;
        default:
          throw new ArgumentException("Only supported array types for CopyTo on BitArrayExs are Boolean[], Int32[] and Byte[].", nameof (array));
      }
    }

    public int Count
    {
      get
      {
        return this.m_length;
      }
    }

    public object SyncRoot
    {
      get
      {
        if (this._syncRoot == null)
          Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), (object) null);
        return this._syncRoot;
      }
    }

    public bool IsSynchronized
    {
      get
      {
        return false;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public object Clone()
    {
      return (object) new BitArrayEx(this);
    }

    public IEnumerator GetEnumerator()
    {
      return (IEnumerator) new BitArrayEx.BitArrayExEnumeratorSimple(this);
    }

    private static int GetArrayLength(int n, int div)
    {
      return n <= 0 ? 0 : (n - 1) / div + 1;
    }

    [Serializable]
    private class BitArrayExEnumeratorSimple : IEnumerator, ICloneable
    {
      private BitArrayEx BitArrayEx;
      private int index;
      private int version;
      private bool currentElement;

      internal BitArrayExEnumeratorSimple(BitArrayEx BitArrayEx)
      {
        this.BitArrayEx = BitArrayEx;
        this.index = -1;
        this.version = BitArrayEx._version;
      }

      public object Clone()
      {
        return this.MemberwiseClone();
      }

      public virtual bool MoveNext()
      {
        ICollection BitArrayEx = (ICollection) this.BitArrayEx;
        if (this.version != this.BitArrayEx._version)
          throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
        if (this.index < BitArrayEx.Count - 1)
        {
          ++this.index;
          this.currentElement = this.BitArrayEx.Get(this.index);
          return true;
        }
        this.index = BitArrayEx.Count;
        return false;
      }

      public virtual object Current
      {
        get
        {
          if (this.index == -1)
            throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
          if (this.index >= this.BitArrayEx.Count)
            throw new InvalidOperationException("Enumeration already finished.");
          return (object) this.currentElement;
        }
      }

      public void Reset()
      {
        if (this.version != this.BitArrayEx._version)
          throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
        this.index = -1;
      }
    }
  }
}
