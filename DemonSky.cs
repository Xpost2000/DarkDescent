using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/*
 * As far as I'm concerned this game is always taking
 * place in Hell.
 * 
 * So this is the only sky I need....
 * 
 * This isn't a skybox, it's more like a classic Doom sky...
 * 
 * TODO: make this sky influence the global directional lighting...
 */
namespace DarkDescent {
    /* But maybe I might consider other skies. */
    internal class DemonSky {
        private Effect m_effect;
        private Texture m_texture;
        private VertexBuffer m_sky_vbo;
        public DemonSky(GraphicsDevice device, ContentManager content) {
            m_effect = content.Load<Effect>("HellSkyEffect");
            m_texture = content.Load<Texture2D>("lavasky1");
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
