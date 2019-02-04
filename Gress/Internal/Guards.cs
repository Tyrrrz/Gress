using System;

namespace Gress.Internal
{
    internal static class Guards
    {
        public static T GuardNotNull<T>(this T o, string argName = null) where T : class
        {
            return o ?? throw new ArgumentNullException(argName);
        }

        public static int GuardNotNegative(this int i, string argName = null)
        {
            return i >= 0
                ? i
                : throw new ArgumentOutOfRangeException(argName, i, "Cannot be negative.");
        }

        public static double GuardNotNegative(this double i, string argName = null)
        {
            return i >= 0
                ? i
                : throw new ArgumentOutOfRangeException(argName, i, "Cannot be negative.");
        }

        public static double GuardRange(this double i, double min, double max, string argName = null)
        {
            return i >= min && i <= max
                ? i
                : throw new ArgumentOutOfRangeException(argName, i,
                    $"Must be greater than or equal to [{min}] and lesser than or equal to [{max}].");
        }
    }
}