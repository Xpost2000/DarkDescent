using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/*
 */
namespace DarkDescent {
    internal class Sky {
        private Effect m_effect;
        private Texture m_texture;

        /*
         * I should really only have to share one VBO but okay,
         * it's not a big deal.
         * 
         * Just don't want to refactor this right now.
         */
        private VertexBuffer m_sky_vbo;
        public Sky(GraphicsDevice device, ContentManager content, string sky_effect, string sky_texture) {
            m_effect = content.Load<Effect>(sky_effect);
            m_texture = content.Load<Texture2D>(sky_texture);
            m_sky_vbo = new VertexBuffer(
                device, 
                VertexPositionColorNormalTexture.VertexDeclaration, 
                6, 
                BufferUsage.WriteOnly
            );
            m_sky_vbo.SetData(MeshUtils.GeneratePlane(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY, Color.White, 1.0f).ToArray());
        }

        public void SetGlobalElapsedTime(float time) {
            m_effect.Parameters["GlobalElapsedTime"].SetValue(time);
        }

        public void Draw(GraphicsDevice device) {
            var old_stencil_state = device.DepthStencilState;

            device.DepthStencilState = DepthStencilState.None;
            m_effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(m_sky_vbo);
            m_effect.Parameters["SkyTexture"].SetValue(m_texture);
            // This can be a triangle strip but it's a PITA to do a lot of this
            // right now so yeah...
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);

            device.DepthStencilState = old_stencil_state;
        }
    }
}
