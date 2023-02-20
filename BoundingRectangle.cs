using Microsoft.Xna.Framework;

namespace DarkDescent {
    internal struct BoundingRectangle {
        public float x;
        public float y;
        public float w;
        public float h;

        public BoundingRectangle(float x, float y, float width, float height) {
            this.x = x;
            this.y = y;
            this.w = width;
            this.h = height;
        }

        public enum IntersectEdge {
            None,
            Left,
            Right,
            Bottom,
            Top,
        }
        public static IntersectEdge OverlapVertical(BoundingRectangle a, BoundingRectangle b) {
            float a_top_edge = a.y;
            float a_bottom_edge = a.y + a.h;
            float b_top_edge = b.y;
            float b_bottom_edge = b.y + b.h;

            if (Overlaps(a, b)) {
                if (a_bottom_edge > b_top_edge && a_bottom_edge < b_bottom_edge) {
                    return IntersectEdge.Top;
                } else if (a_top_edge < b_bottom_edge && a_bottom_edge > b_bottom_edge) {
                    return IntersectEdge.Bottom;
                }

            }

            return IntersectEdge.None;
        }
        public static IntersectEdge OverlapHorizontal(BoundingRectangle a, BoundingRectangle b) {
            float a_right_edge = a.x + a.w;
            float a_left_edge = a.x;

            float b_right_edge = b.x + b.w;
            float b_left_edge = b.x;

            if (Overlaps(a, b)) {
                if (a_right_edge > b_right_edge) {
                    return IntersectEdge.Right;
                } else if (a_right_edge > b_left_edge) {
                    return IntersectEdge.Left;
                }
            }

            return IntersectEdge.None;
        }

        public Vector2 Center {
            get {
                return new Vector2(x + w / 2, y + h / 2);
            }
        }

        public static bool Overlaps(BoundingRectangle a, BoundingRectangle b) {
            if (a.x < b.x + b.w && a.x + a.w > b.x &&
                a.y < b.y + b.h && a.y + a.h > b.y) {
                return true;
            }

            return false;
        }

        public static bool Overlaps(BoundingRectangle a, BoundingCircle b) {
            return BoundingCircle.Overlaps(b, a);
        }
    }
}
