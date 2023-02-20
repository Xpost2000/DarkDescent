using System;

namespace DarkDescent {
    internal static class FloatingPointEpsilon {
        public static bool Equal(float a, float b, float tolerance=float.Epsilon) {
            return MathF.Abs(a - b) <= tolerance;
        }
    }
}
