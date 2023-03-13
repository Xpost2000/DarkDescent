using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DarkDescent {
    /*
     * Theme set for the game.
     * 
     * Not totally that elegant but it should work fine for quickly theming stuff.
     */
    internal class EnvironmentThemeSet {
        private Texture2D m_floor_texture;
        private Texture2D m_ceiling_texture;
        private Texture2D m_wall_texture;
        private Texture2D m_stone_texture;
        private Sky m_sky;

        public EnvironmentThemeSet(
            ContentManager content,
            GraphicsDevice device,
            string floor_texture,
            string ceiling_texture,
            string wall_texture,
            string stone_texture,
            string sky_effect,
            string sky_texture
            ) {
            m_floor_texture = content.Load<Texture2D>(floor_texture);
            m_ceiling_texture = content.Load<Texture2D>(ceiling_texture);
            m_wall_texture = content.Load<Texture2D>(wall_texture);
            m_stone_texture = content.Load<Texture2D>(stone_texture);
            m_sky = new Sky(device, content, sky_effect, sky_texture);
        }

        public Texture2D Ceiling {
            get {
                return m_ceiling_texture;
            }
        }
        public Texture2D Stone {
            get {
                return m_stone_texture;
            }
        }
        public Texture2D Wall {
            get {
                return m_wall_texture;
            }
        }
        public Texture2D Floor {
            get {
                return m_floor_texture;
            }
        }
        public Sky Sky {
            get {
                return m_sky;
            }
        }
    }
}
