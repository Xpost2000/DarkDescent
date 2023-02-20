using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/**
 * For now not going to inherit anything since that's
 * kind of a thing I haven't done in a long time because
 * of how much I program in C...
 **/
namespace DarkDescent {
    internal class Player {
        private Vector2 m_position;
        private float m_look_angle;

        public static readonly float TurnVelocity = 1.0f;
        public static readonly float Velocity = 1.00f;
        private static readonly float BobSpeed = 6.0f;
        /*
         * NOTE: due to the relatively small unit scale. The collision
         * routine can crack because these floating point numbers are small
         */
        public static readonly float BoundingHalfSize = (0.5f)/2.0f;

        /*
         * Anim Data
         */
        float m_walk_bob_timer = 0.0f;
        bool m_walking = false;
        /*
         * End of Anim Data
         */

        public Player(Vector2 position, float look_angle = 0.0f) {
            Position = position;
            LookAngle = look_angle;
        }

        public Vector2 Position {
            get { return m_position; }
            set { m_position = value; }
        }

        public Vector3 Position3D {
            get {
                /*
                 * Center the view
                 * 
                 * This offset bias happens to make the collision look fine
                 * because this is working from top left coordinates which is
                 * obviously bad for doing stuff in 3D... Especially since everything
                 * else I'm rendering is actually centered.
                 */
                return new Vector3(m_position.X, 0.0f, m_position.Y);
            }
        }

        public float LookAngle {
            get { return m_look_angle; }
            set { m_look_angle = value; }
        }

        public BoundingRectangle GetRectangle() {
            return new BoundingRectangle(
                m_position.X - BoundingHalfSize, 
                m_position.Y - BoundingHalfSize, 
                // This is misnamed.
                BoundingHalfSize*2,
                BoundingHalfSize*2
            );
        }
        public Vector2 Forward2D {
            get {
                return Vector2Extensions.FromRadians(m_look_angle);
            }
        }
        public Vector3 Forward3D {
            get {
                var in2D = Forward2D;
                return new Vector3(in2D.X, 0.0f, in2D.Y);
            }

        }

        public bool Walking {
            get {
                return m_walking;
            }
            set {
                if (m_walking != true) {
                    if (value == true) {
                        m_walking = true;
                        m_walk_bob_timer = 0.0f;
                    }
                } else {
                    m_walking = value;
                }
            }
        }

        public Matrix GetView() {
            var camera_position = Position3D;
            camera_position.Y = System.MathF.Sin(m_walk_bob_timer * BobSpeed) * 0.035f;
            return Matrix.CreateLookAt(
                camera_position, 
                camera_position + Forward3D, 
                Vector3.Up
            );
        }

        
        public void CheckIntersectionsAndMove(Vector2 velocity, DungeonRoom room, float dt) {
            // X axis
            bool stop_horizontal_movement = false;
            bool stop_vertical_movement = false;

            {
                m_position.X += velocity.X * dt;

                // World Map Collisions
                {
                    for (int y = 0; y < room.Height && !stop_horizontal_movement; ++y) {
                        for (int x = 0; x < room.Width && !stop_horizontal_movement; ++x) {
                            var tile_rectangle = room.GetTileRectangle(x, y);
                            var current_rectangle = GetRectangle();
                            if (DungeonRoom.IsSolidTile(room.GetTile(x, y))) {
                                if (BoundingRectangle.Overlaps(current_rectangle, tile_rectangle.Value)) {
                                    switch (BoundingRectangle.OverlapHorizontal(current_rectangle, tile_rectangle.Value)) {
                                        case BoundingRectangle.IntersectEdge.Left: {
                                            m_position.X = tile_rectangle.Value.x - BoundingHalfSize;
                                            System.Diagnostics.Debug.WriteLine("hit on left edge");
                                        }
                                        break;
                                        case BoundingRectangle.IntersectEdge.Right: {
                                            m_position.X = (tile_rectangle.Value.x + tile_rectangle.Value.w) + BoundingHalfSize;
                                            System.Diagnostics.Debug.WriteLine("hit on right edge");
                                        }
                                        break;
                                    }
                                    stop_horizontal_movement = true;
                                }
                            }
                        }
                    }
                 }
            }

            {
                m_position.Y += velocity.Y * dt;

                for (int y = 0; y < room.Height && !stop_vertical_movement; ++y) {
                    for (int x = 0; x < room.Width && !stop_vertical_movement; ++x) {
                        var tile_rectangle = room.GetTileRectangle(x, y);
                        var current_rectangle = GetRectangle();
                        if (DungeonRoom.IsSolidTile(room.GetTile(x, y))) {
                            if (BoundingRectangle.Overlaps(current_rectangle, tile_rectangle.Value)) {
                                System.Diagnostics.Debug.WriteLine("hit on vertical test.");
                                switch (BoundingRectangle.OverlapVertical(current_rectangle, tile_rectangle.Value)) {
                                    case BoundingRectangle.IntersectEdge.Top: {
                                        m_position.Y = tile_rectangle.Value.y - BoundingHalfSize;
                                    }
                                    break;
                                    case BoundingRectangle.IntersectEdge.Bottom: {
                                        m_position.Y = tile_rectangle.Value.y + tile_rectangle.Value.h + BoundingHalfSize;
                                        System.Diagnostics.Debug.WriteLine("hit on bottom edge");
                                    }
                                    break;
                                }
                                stop_vertical_movement = true;
                            }
                        }
                    }
                }
            }

            if (stop_horizontal_movement) {
                velocity.X = 0;
            }
            if (stop_vertical_movement) {
                velocity.Y = 0;
            }
        }

        private void DoWalkViewBob(Vector2 target_velocity, float dt) {
            if (target_velocity.LengthSquared() != 0.0f) {
                Walking = true;
            } else {
                Walking = false;
            }

            if (Walking) {
                m_walk_bob_timer += dt;
                if (m_walk_bob_timer >= MathHelper.Pi / BobSpeed) {
                    m_walk_bob_timer = 0;
                }
            } else {
                m_walk_bob_timer -= dt;
            }

            if (m_walk_bob_timer < 0.0f) {
                m_walk_bob_timer = 0.0f;
            }
        }
        public void Update(DungeonRoom room, float dt) {
            if (Input.KeyDown(Keys.Q)) {
                LookAngle -= dt * MathHelper.Pi * TurnVelocity;
            } else if (Input.KeyDown(Keys.E)) {
                LookAngle += dt * MathHelper.Pi * TurnVelocity;
            }

            var forward = Forward2D;
            var right = Vector2Extensions.Perpendicular(forward);

            Vector2 target_velocity = Vector2.Zero;

            if (Input.KeyDown(Keys.W)) {
                target_velocity += forward * Velocity;
            } else if (Input.KeyDown(Keys.S)) {
                target_velocity += -forward * Velocity;
            }

            if (Input.KeyDown(Keys.A)) {
                target_velocity += -right * Velocity * 1.5f;
            } else if (Input.KeyDown(Keys.D)) {
                target_velocity += right * Velocity * 1.5f;
            }

            {CheckIntersectionsAndMove(target_velocity, room, dt);}
            // TODO: clean up at some point. Maybe next week.
            {DoWalkViewBob(target_velocity, dt);}
        }
    }
}
