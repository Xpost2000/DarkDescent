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
    public enum EnvironmentType {
        Earth = 0,
        Hell = 1,
        Cocytus = 2,
        Count = 3,
    }
    public class Main : Game {
        // Should be resolution independent NOTE:
        private static readonly int RenderTarget_Width = 320;
        private static readonly int RenderTarget_Height = 240;

        private GraphicsDeviceManager m_graphics;

        // Obligatory Graphics Resources
        private BasicEffect m_basic_effect;
        private Effect m_postprocess_effect;
        private VertexBuffer m_postprocess_quad_vbo;
        private RenderTarget2D m_render_target;
        private RasterizerState world_rendering_rasterizer_state;
        private EnvironmentThemeSet[] m_theme_sets;
        private EnvironmentType m_current_environment_type = EnvironmentType.Earth;

        private Texture2D m_white_texture;
        private float m_total_elapsed_time = 0.0f;

        private Matrix m_projection;

        /* Game State */
        private DungeonRoom m_test_dungeon; /*NOTE dungeon rooms should be able to concat with each other*/
        private Player m_player;

        private GameUI m_ui;
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

        private void InitializeEnvironmentSets() {
            m_theme_sets = new EnvironmentThemeSet[(int)EnvironmentType.Count];
            string[] themeset_strings = {
                "env_earth", "env_hell", "env_cocytus"
            };
            for (int i = 0; i < m_theme_sets.Length; i++) {
                m_theme_sets[i] = new EnvironmentThemeSet(Content, GraphicsDevice, themeset_strings[i]);
            }
        }

        protected override void LoadContent() {
            m_white_texture = Content.Load<Texture2D>("white");
            m_ui = new GameUI(Content, GraphicsDevice, this);
            InitializeEnvironmentSets();

            m_postprocess_effect = Content.Load<Effect>("GamePostProcess");
            // Test Dungeon. I should have probably prefixed dungeons
            // brush tiles or something.
            m_test_dungeon = new DungeonRoom(6, 6);

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
                m_current_environment_type = EnvironmentType.Hell;
            } else if (Input.KeyPressedThenReleased(Keys.D2)) {
                m_current_environment_type = EnvironmentType.Cocytus;
            } else if (Input.KeyPressedThenReleased(Keys.D3)) {
                m_current_environment_type = EnvironmentType.Earth;
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

        void GameDraw(GameTime game_time) {
            {
                GraphicsDevice.Clear(Color.DarkBlue);
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

                EnvironmentThemeSet current_theme = m_theme_sets[(int)m_current_environment_type];
                // Draw the sky
                {
                    Sky current_sky = current_theme.Sky;
                    current_sky.SetGlobalElapsedTime(m_total_elapsed_time);
                    current_sky.Draw(GraphicsDevice);
                }

                // Game world pass

                GraphicsDevice.RasterizerState = world_rendering_rasterizer_state;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                m_basic_effect.Projection = m_projection;
                m_basic_effect.View = m_player.GetView();
                m_basic_effect.World = Matrix.Identity;
                m_test_dungeon.DrawDungeonLayoutMeshes(GraphicsDevice, m_basic_effect, current_theme);

                // 2d map
                m_ui.UpdateRender();

            }
        }

        protected override void Draw(GameTime game_time) {
            GraphicsDevice.SetRenderTarget(m_render_target);
            GameDraw(game_time);
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            m_postprocess_effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.SetVertexBuffer(m_postprocess_quad_vbo);
            m_postprocess_effect.Parameters["FramebufferTexture"].SetValue(m_render_target);
            m_postprocess_effect.Parameters["GlobalElapsedTime"].SetValue(m_total_elapsed_time);
            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            base.Draw(game_time);
        }
    }
}
