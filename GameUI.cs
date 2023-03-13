using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DarkDescent {
    internal class GameUI {
        private SpriteFont m_font;
        private SpriteBatch m_sprite_batch;
        private Main m_game_state;
        public GameUI(ContentManager content, GraphicsDevice device, Main game_state) {
            m_sprite_batch = new SpriteBatch(device);
            m_font = content.Load<SpriteFont>("UINormal");
            m_game_state = game_state;
        }

        public void UpdateRender() {
            m_sprite_batch.Begin();
            m_sprite_batch.DrawString(m_font, "Hello World", Vector2.Zero, Color.Red);
            m_sprite_batch.End();

#if false
                sprite_batch.Begin();
#if false
                for (int y = 0; y < m_test_dungeon.Height; ++y) {
                    for (int x = 0; x < m_test_dungeon.Width; ++x) {
                        if (DungeonRoom.IsSolidTile(m_test_dungeon.GetTile(x, y))) {
                            sprite_batch.Draw(m_white_texture, new Rectangle(x * 16, y * 16, 16, 16), Color.Red);
                        } else {
                            sprite_batch.Draw(m_white_texture, new Rectangle(x * 16, y * 16, 16, 16), Color.Black);
                        }
                    }
                }
                {
                    var bb = m_player.GetRectangle();
                    sprite_batch.Draw(m_white_texture,
                        new Rectangle((int)(bb.x * 16), (int)(bb.y * 16), (int)(bb.w * 16), (int)(bb.h * 16)), Color.White);
                    sprite_batch.Draw(m_white_texture,
                           new Rectangle((int)(m_player.Position.X * 16 - 2), (int)(m_player.Position.Y * 16 - 2), (int)(4), (int)(4)), Color.Green);
                }
#endif
                sprite_batch.DrawString(m_font, "Hello World", Vector2.Zero, Color.Red);

                sprite_batch.End();
#endif
        }
    }
}
