namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using System;

    internal static class Extensions
    {
        public static T[] Add<T>(this T[] array, T value)
        {
            T[] newArray = new T[array.Length + 1];

            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = value;

            return newArray;
        }

        public static T[] AddRange<T>(this T[] array, params T[] values)
        {
            T[] newArray = new T[array.Length + values.Length];

            Array.Copy(array, newArray, array.Length);
            Array.Copy(values, 0, newArray, array.Length, values.Length);

            return newArray;
        }

        public static bool ApproximatelyGreaterThan(this double thisValue, double otherValue, double delta)
        {
            return thisValue >= otherValue - delta;
        }
    }
}
