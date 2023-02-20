using Microsoft.Xna.Framework;

namespace DarkDescent {
    internal struct BoundingCircle {
        public Vector2 center;
        public float radius;

        public BoundingCircle(Vector2 center, float radius) {
            this.center = center;
            this.radius = radius;
        }

        public static bool Overlaps(BoundingCircle a, BoundingCircle b) {
            var difference_in_position = b.center - a.center;
            float radi_sum = (b.radius + a.radius);
            return difference_in_position.LengthSquared() <= radi_sum*radi_sum;
        }

        public static bool Overlaps(BoundingCircle a, Vector2 point) {
            var difference_in_position = point - a.center;
            return difference_in_position.LengthSquared() <= a.radius*a.radius;
        }

        public static bool Overlaps(BoundingCircle a, BoundingRectangle b) {
            Vector2 closest_point = a.center;
            if (a.center.X <= b.x) {
                closest_point.X = b.x;
            } else if (a.center.X >= b.x + b.w) {
                closest_point.X = b.x + b.w;
            }

            if (a.center.Y <= b.y) {
                closest_point.Y = b.y;
            } else if (a.center.Y >= b.y + b.h) {
                closest_point.Y = b.y;
            }

            return Overlaps(a, closest_point);
        }
    }
}
