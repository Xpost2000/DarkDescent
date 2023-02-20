/*
 * This engine has exceptional bad code quality for my own standards,
 * and compared to what I usually write but that's cause I'm not too versed
 * in C#isms and also I'm having fun with making whatever here.
 * 
 * Very exploratory.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DarkDescent {
    /*
     * All engine units are relative to a tile block.
     */
    public class Main : Game {
        // Should be resolution independent NOTE:
        private static readonly int RenderTarget_Width = 320;
        private static readonly int RenderTarget_Height = 240;

        private GraphicsDeviceManager m_graphics;

        // Obligatory Graphics Resources
        private BasicEffect m_basic_effect;
        private Effect m_postprocess_effect;
        private VertexBuffer m_postprocess_quad_vbo;

        private DemonSky m_sky;

        private RenderTarget2D m_render_target;

        private RasterizerState world_rendering_rasterizer_state;
        private Texture2D m_floor_texture;
        private Texture2D m_ceiling_texture;
        private Texture2D m_wall_texture;
        private Texture2D m_stone_texture;

        private Texture2D m_white_texture;
        private float m_total_elapsed_time = 0.0f;

        private Matrix m_projection;

        private DungeonRoom m_test_dungeon; /*NOTE dungeon rooms should be able to concat with each other*/
        private Player m_player;

        private SpriteBatch sprite_batch;
        private float AspectRatio() {
            return ((float)m_graphics.PreferredBackBufferWidth / m_graphics.PreferredBackBufferHeight);
        }

        public Main() {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void SetProjectionMatrix() {
            m_projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(75),
                AspectRatio(),
                0.1f,
                1000.0f
            );
        }

        protected override void Initialize() {
            m_graphics.PreferredBackBufferWidth = 1024;
            m_graphics.PreferredBackBufferHeight = 768;
            m_graphics.ApplyChanges();
            
            m_basic_effect = new BasicEffect(m_graphics.GraphicsDevice);
            m_sky = new DemonSky(m_graphics.GraphicsDevice, Content);
            /*
             * The nice thing is I don't actually have to do any math...
             */
            SetProjectionMatrix();
            m_basic_effect.Projection = m_projection;
            m_basic_effect.View = Matrix.Identity;
            m_basic_effect.TextureEnabled = true;
            m_basic_effect.VertexColorEnabled = true;
            m_basic_effect.LightingEnabled = true;
            m_basic_effect.AmbientLightColor = new Vector3(0.18f);
            m_basic_effect.PreferPerPixelLighting = true;
            m_basic_effect.DirectionalLight0.Enabled = true;
            m_basic_effect.DirectionalLight0.DiffuseColor = new Vector3(0.315f, 0.300f, 0.366f);
            m_basic_effect.DirectionalLight0.Direction = -Vector3.UnitY - Vector3.UnitX * 0.4f;

            world_rendering_rasterizer_state = new RasterizerState();
            world_rendering_rasterizer_state.CullMode = CullMode.None;

            m_player = new Player(new Vector2(2+0.5f,2+0.5f));
            m_player.LookAngle = MathHelper.Pi/2 + MathHelper.Pi*2;

            IsFixedTimeStep = true;
            TargetElapsedTime = System.TimeSpan.FromSeconds(1.0 / 20.0);

            sprite_batch = new SpriteBatch(GraphicsDevice);

            m_render_target = new RenderTarget2D(
                GraphicsDevice, 
                RenderTarget_Width, 
                RenderTarget_Height, 
                false, 
                GraphicsDevice.PresentationParameters.BackBufferFormat, 
                DepthFormat.Depth24
            );
            base.Initialize();
        }

        protected override void LoadContent() {
            m_floor_texture = Content.Load<Texture2D>("floor");
            m_wall_texture = Content.Load<Texture2D>("wall");
            m_ceiling_texture = Content.Load<Texture2D>("ceiling");
            m_white_texture = Content.Load<Texture2D>("white");
            m_stone_texture = Content.Load<Texture2D>("stone");
            m_postprocess_effect = Content.Load<Effect>("GamePostProcess");
            // Test Dungeon. I should have probably prefixed dungeons
            // brush tiles or something.
            m_test_dungeon = new DungeonRoom(6, 6);
            m_test_dungeon.WallTexture = m_wall_texture;
            m_test_dungeon.CeilingTexture = m_ceiling_texture;
            m_test_dungeon.FloorTexture = m_floor_texture;
            m_test_dungeon.StoneTexture = m_stone_texture;

            for (int y = 0; y < m_test_dungeon.Height; ++y) { 
                for (int x = 0; x < m_test_dungeon.Width; ++x) {
                    m_test_dungeon.SetTile(x, y, (int)DungeonRoom.TileId.Floor);
                }
            }
            for (int i = 0; i < m_test_dungeon.Height; ++i) {
                m_test_dungeon.SetTile(0, i, (int)DungeonRoom.TileId.Wall);
                m_test_dungeon.SetTile(m_test_dungeon.Width-1, i, (int)DungeonRoom.TileId.Wall);
            }
            for (int i = 0; i < m_test_dungeon.Width; ++i) {
                m_test_dungeon.SetTile(i, 0, (int)DungeonRoom.TileId.Wall);
                m_test_dungeon.SetTile(i, m_test_dungeon.Height-1, (int)DungeonRoom.TileId.Wall);
            }
            m_test_dungeon.SetTile(2, 4, (int)DungeonRoom.TileId.Wall);
            m_test_dungeon.SetTile(2, 3, (int)DungeonRoom.TileId.StoneTable);
            m_test_dungeon.SetTile(1, 4, (int)DungeonRoom.TileId.FloorNoCeiling);
            m_test_dungeon.SetTile(0, 4, (int)DungeonRoom.TileId.FloorNoCeiling);
            m_test_dungeon.RegenerateMeshesForDungeon(GraphicsDevice);

            m_postprocess_quad_vbo = new VertexBuffer(
                GraphicsDevice,
                VertexPositionColorNormalTexture.VertexDeclaration,
                6,
                BufferUsage.WriteOnly
            );
            m_postprocess_quad_vbo.SetData(MeshUtils.GeneratePlane(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY, Color.White, 1.0f).ToArray());
        }

        private void GameUpdate(float dt) {
            if (Input.KeyPressedThenReleased(Keys.D1)) {
                Exit();
            }

            m_player.Update(m_test_dungeon, dt);
        }

        protected override void Update(GameTime game_time) {
            float dt = (float)game_time.ElapsedGameTime.TotalSeconds;
            GameUpdate(dt);
            Input.UpdateGamePadState();
            Input.UpdateKeyState();
            m_total_elapsed_time += dt;
            base.Update(game_time);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(m_render_target);
            {
                GraphicsDevice.Clear(Color.DarkBlue);
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
                // Draw the sky
                m_sky.SetGlobalElapsedTime(m_total_elapsed_time);
                m_sky.Draw(GraphicsDevice);

                // Game world pass

                GraphicsDevice.RasterizerState = world_rendering_rasterizer_state;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                m_basic_effect.Projection = m_projection;
                m_basic_effect.View = m_player.GetView();
                m_basic_effect.World = Matrix.Identity;
                m_test_dungeon.DrawDungeonLayoutMeshes(GraphicsDevice, m_basic_effect);

                // 2d map
                sprite_batch.Begin();
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

                sprite_batch.End();
            }
            GraphicsDevice.SetRenderTarget(null);
#if true
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            m_postprocess_effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.SetVertexBuffer(m_postprocess_quad_vbo);
            m_postprocess_effect.Parameters["FramebufferTexture"].SetValue(m_render_target);
            m_postprocess_effect.Parameters["GlobalElapsedTime"].SetValue(m_total_elapsed_time);
            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
#else
            sprite_batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
            SamplerState.LinearClamp, DepthStencilState.Default,
            RasterizerState.CullNone);
            sprite_batch.Draw(m_render_target, new Rectangle(0, 0, 400, 240), Color.Red);
            sprite_batch.End();
#endif
            base.Draw(gameTime);
        }
    }
}
