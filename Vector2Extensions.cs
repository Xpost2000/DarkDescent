using Microsoft.Xna.Framework;

namespace DarkDescent {
    public static class Vector2Extensions {
        static public Vector2 Perpendicular(Vector2 input) {
            return new Vector2(-input.Y, input.X);
        }

        static public Vector2 FromRadians(float radians) {
            return new Vector2(System.MathF.Cos(radians), System.MathF.Sin(radians));
        }
    }
}
