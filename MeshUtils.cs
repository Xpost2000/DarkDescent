using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DarkDescent {
    /*
     * For simplicity purposes everything here is a triangle list.
     * I don't normalize the vectors. But they should be normalized.
     */
    public static class MeshUtils {
        public static readonly VertexDeclaration vertex_format = 
            VertexPositionColorTexture.VertexDeclaration;

        public static List<VertexPositionColorNormalTexture> GeneratePlane(
            Vector3 position,
            Vector3 normal,
            Vector3 up,
            Color   color,
            float half_width,
            float? half_height=null
        ) {
            List<VertexPositionColorNormalTexture> result = new List<VertexPositionColorNormalTexture>();

            Vector3 right = Vector3.Cross(normal, up);
            right.Normalize();

            float half_height_to_use = half_height.GetValueOrDefault(half_width);

            VertexPositionColorNormalTexture[] quad_vertices = new VertexPositionColorNormalTexture[4] {
                // top left
                new VertexPositionColorNormalTexture(position + right * -half_width + up * -half_height_to_use, color, normal, new Vector2(0.0f, 1.0f)),
                // top right
                new VertexPositionColorNormalTexture(position + right * half_width + up * -half_height_to_use, color, normal, new Vector2(1.0f, 1.0f)),
                // bottom left
                new VertexPositionColorNormalTexture(position + right * -half_width + up * half_height_to_use, color, normal, new Vector2(0.0f, 0.0f)),
                // bottom right
                new VertexPositionColorNormalTexture(position + right * half_width + up * half_height_to_use, color, normal, new Vector2(1.0f, 0.0f)),
            };

            result.Add(quad_vertices[0]);
            result.Add(quad_vertices[1]);
            result.Add(quad_vertices[2]);
            result.Add(quad_vertices[2]);
            result.Add(quad_vertices[1]);
            result.Add(quad_vertices[3]);

            return result;
        }

        public static List<VertexPositionColorNormalTexture> GenerateCube(Vector3 position, Vector3 forward, Vector3 up, Color color, float half_width, float? half_height=null) {
            List<VertexPositionColorNormalTexture> result = new List<VertexPositionColorNormalTexture>();

            Vector3 right = Vector3.Cross(forward, up);
            right.Normalize();
            result.AddRange(GeneratePlane(position + forward*half_width, forward, up, color, half_width, half_height));
            result.AddRange(GeneratePlane(position - forward*half_width, forward, up, color, half_width, half_height));

            result.AddRange(GeneratePlane(position + right*half_width,  right, up, color, half_width, half_height));
            result.AddRange(GeneratePlane(position - right*half_width, -right, up, color, half_width, half_height));

            result.AddRange(GeneratePlane(position + up*half_height.GetValueOrDefault(half_width), up,  forward, color, half_width, half_width));
            result.AddRange(GeneratePlane(position - up*half_height.GetValueOrDefault(half_width), -up, forward, color, half_width, half_width));
            return result;
        }

        // TODO Cylinder or sphere.
    }
}
