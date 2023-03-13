using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DarkDescent {
    /*
     * Theme set for the game.
     * 
     * Not totally that elegant but it should work fine for quickly theming stuff.
     */
    internal class EnvironmentThemeSet {
        private readonly Texture2D m_floor_texture;
        private readonly Texture2D m_ceiling_texture;
        private readonly Texture2D m_wall_texture;
        private readonly Texture2D m_stone_texture;
        private readonly Sky m_sky;

        public EnvironmentThemeSet(
            ContentManager content,
            GraphicsDevice device,
            string environment_path
            ) {
            m_floor_texture = content.Load<Texture2D>(environment_path + "\\floor");
            m_ceiling_texture = content.Load<Texture2D>(environment_path + "\\ceiling");
            m_wall_texture = content.Load<Texture2D>(environment_path + "\\wall");
            m_stone_texture = content.Load<Texture2D>(environment_path + "\\stone");
            m_sky = new Sky(device, content, environment_path + "\\SkyEffect", environment_path + "\\sky");
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
