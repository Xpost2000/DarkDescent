using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DarkDescent {
    /*
     * This is the main level format of this game thing
     * I would frankly like to change this format later to be a bit more flexible.
     * 
     * However this is not meant to last very long so I'm sticking with this very bad
     * style.
     * 
     * At least it renders in a few calls.
     */
    internal class DungeonRoom {
        public enum TileId {
            None = 0,
            Wall = 1,
            Floor = 2,
            FloorNoCeiling = 3,

            // UNI DIRECTIONAL TABLE
            StoneTable,

            Count = 4,
        };
        static readonly float TileHalfSize = 0.5f;
        
        // Graphical Resources (per material type)
        private VertexBuffer m_floor_vbo;
        private VertexBuffer m_ceiling_vbo;
        private VertexBuffer m_wall_vbo;
        private VertexBuffer m_stone_vbo;

        // Logical Resources
        private float m_ceiling_height = 1.0f;
        private int m_width;
        private int m_height;
        private bool m_dirty = false;
        private int[] m_dungeon_layout;
        public DungeonRoom(int width, int height) {
            this.m_width = width;
            this.m_height = height;
            this.m_dungeon_layout = new int[this.m_width * this.m_height];
        }

        public int Width {
            get { return m_width;  }
        }
        public int Height {
            get { return m_height; }
        }
        public float CeilingHeight {
            get { return m_ceiling_height; }
            set { m_ceiling_height = value; }
        }

        public void SetTile(int x, int y, int value) {
            this.m_dungeon_layout[y * Width + x] = value;
            this.m_dirty = true;
        }

        public int GetTile(int x, int y) {
            return this.m_dungeon_layout[y * Width + x];
        }

        public static bool IsSolidTile(int id) {
            switch (id) {
                case (int)TileId.Wall:
                case (int)TileId.StoneTable:
                    return true;
            }

            return false;
        }

        public BoundingRectangle? GetTileRectangle(int x, int y) {
            var tile_id = GetTile(x, y);
            
            if (tile_id == (int)TileId.None) {
                return null;
            }

            return new BoundingRectangle(x, y, 1, 1);
        }

        public void RegenerateMeshesForDungeon(GraphicsDevice graphics_device) {
            if (!this.m_dirty) return;
            this.m_dirty = false;

            if (m_floor_vbo != null) m_floor_vbo.Dispose();
            if (m_ceiling_vbo != null) m_ceiling_vbo.Dispose();
            if (m_wall_vbo != null) m_wall_vbo.Dispose();

            var floor_vertices   = new List<VertexPositionColorNormalTexture>();
            var wall_vertices    = new List<VertexPositionColorNormalTexture>();
            var ceiling_vertices = new List<VertexPositionColorNormalTexture>();
            var stone_vertices = new List<VertexPositionColorNormalTexture>();

            // Assume Y is up.
            float tile_size = TileHalfSize * 2;
            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    Vector3 position = new Vector3((x * tile_size)+TileHalfSize, 0.0f, (y * tile_size)+TileHalfSize);
                    System.Diagnostics.Debug.WriteLine(position);
                    switch (m_dungeon_layout[y * Width + x]) {
                        case (int)TileId.None: {
                            // nothing
                            System.Diagnostics.Debug.WriteLine("Nothing");
                        } break;
                        case (int)TileId.Wall: {
                            System.Diagnostics.Debug.WriteLine("Found a wall.");
                            wall_vertices.AddRange(
                                MeshUtils.GenerateCube(
                                    position, 
                                    Vector3.UnitZ, 
                                    Vector3.UnitY, 
                                    Color.White, 
                                    TileHalfSize
                                )
                            );
                        } break;
                        case (int)TileId.Floor: {
                            System.Diagnostics.Debug.WriteLine("Found a floor/ceiling pair.");
                            floor_vertices.AddRange(
                                MeshUtils.GeneratePlane(
                                    position - new Vector3(0.0f, TileHalfSize, 0.0f),
                                    Vector3.UnitY, 
                                    Vector3.UnitZ,
                                    Color.White, 
                                    TileHalfSize
                                )
                            );
                            ceiling_vertices.AddRange(
                                MeshUtils.GeneratePlane(
                                    position + new Vector3(0.0f, TileHalfSize, 0.0f),
                                    Vector3.UnitY,
                                    Vector3.UnitZ,
                                    Color.DarkGray,
                                    TileHalfSize
                                )
                            );
                        } break;
                        case (int)TileId.FloorNoCeiling: {
                            System.Diagnostics.Debug.WriteLine("Found a floor, no ceiling");
                            floor_vertices.AddRange(
                                MeshUtils.GeneratePlane(
                                    position - new Vector3(0.0f, TileHalfSize, 0.0f),
                                    Vector3.UnitY,
                                    Vector3.UnitZ,
                                    Color.White,
                                    TileHalfSize
                                )
                            );
                        }
                        break;
                        case (int)TileId.StoneTable: {
                            // Uni directional table is a flat cuboid with a few longer ones.
                            {
                                stone_vertices.AddRange(
                                    MeshUtils.GenerateCube(
                                        position + new Vector3(0.0f, -TileHalfSize/2, 0.0f),
                                        Vector3.UnitZ,
                                        Vector3.UnitY,
                                        Color.White,
                                        TileHalfSize,
                                        TileHalfSize/11
                                    ));
                               // leg?
                                stone_vertices.AddRange(
                                MeshUtils.GenerateCube(
                                    position + new Vector3(0.0f, -TileHalfSize / 2 - TileHalfSize / 4, 0.0f),
                                    Vector3.UnitZ,
                                    Vector3.UnitY,
                                    Color.White,
                                    TileHalfSize / 8,
                                    TileHalfSize / 8
                                ));
                                // foot/base
                                stone_vertices.AddRange(
                                    MeshUtils.GenerateCube(
                                        position + new Vector3(0.0f, -TileHalfSize, 0.0f),
                                        Vector3.UnitZ,
                                        Vector3.UnitY,
                                        Color.White,
                                        TileHalfSize / 2,
                                        TileHalfSize / 8
                                    ));
                            }
                            ceiling_vertices.AddRange(
                                MeshUtils.GeneratePlane(
                                    position + new Vector3(0.0f, TileHalfSize, 0.0f),
                                    Vector3.UnitY,
                                    Vector3.UnitZ,
                                    Color.White,
                                    TileHalfSize
                                )
                            );
                            floor_vertices.AddRange(
                                MeshUtils.GeneratePlane(
                                    position - new Vector3(0.0f, TileHalfSize, 0.0f),
                                    Vector3.UnitY,
                                    Vector3.UnitZ,
                                    Color.White,
                                    TileHalfSize
                                )
                            );
                        } break;
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("Regenerating meshes...");

            if (ceiling_vertices.Count >= 3) {
                m_ceiling_vbo = new VertexBuffer(graphics_device, VertexPositionColorNormalTexture.VertexDeclaration, ceiling_vertices.Count, BufferUsage.WriteOnly);
                m_ceiling_vbo.SetData(ceiling_vertices.ToArray());
            }
            if (floor_vertices.Count >= 3) {
                m_floor_vbo = new VertexBuffer(graphics_device, VertexPositionColorNormalTexture.VertexDeclaration, floor_vertices.Count, BufferUsage.WriteOnly);
                m_floor_vbo.SetData(floor_vertices.ToArray());
            }
            if (wall_vertices.Count >= 3) {
                m_wall_vbo = new VertexBuffer(graphics_device, VertexPositionColorNormalTexture.VertexDeclaration, wall_vertices.Count, BufferUsage.WriteOnly);
                m_wall_vbo.SetData(wall_vertices.ToArray());
            }
            if (stone_vertices.Count >= 3) {
                m_stone_vbo = new VertexBuffer(graphics_device, VertexPositionColorNormalTexture.VertexDeclaration, stone_vertices.Count, BufferUsage.WriteOnly);
                m_stone_vbo.SetData(stone_vertices.ToArray());
            }
        }

        /* Effect is for the shader I guess */
        public void DrawDungeonLayoutMeshes(GraphicsDevice graphics_device, BasicEffect basic_effect, EnvironmentThemeSet theme) {
            RegenerateMeshesForDungeon(graphics_device);

            // This is basically a cheap material system but okay.
            if (m_floor_vbo != null) {
                basic_effect.CurrentTechnique.Passes[0].Apply();
                basic_effect.Texture = theme.Floor;
                graphics_device.SetVertexBuffer(m_floor_vbo);
                graphics_device.DrawPrimitives(PrimitiveType.TriangleList, 0, m_floor_vbo.VertexCount);
            }

            if (m_ceiling_vbo != null) {
                basic_effect.CurrentTechnique.Passes[0].Apply();
                basic_effect.Texture = theme.Ceiling;
                graphics_device.SetVertexBuffer(m_ceiling_vbo);
                graphics_device.DrawPrimitives(PrimitiveType.TriangleList, 0, m_ceiling_vbo.VertexCount);
            }

            if (m_wall_vbo != null) {
                basic_effect.CurrentTechnique.Passes[0].Apply();
                basic_effect.Texture = theme.Wall;
                graphics_device.SetVertexBuffer(m_wall_vbo);
                graphics_device.DrawPrimitives(PrimitiveType.TriangleList, 0, m_wall_vbo.VertexCount);
            }

            if (m_stone_vbo != null) {
                basic_effect.CurrentTechnique.Passes[0].Apply();
                basic_effect.Texture = theme.Stone;
                graphics_device.SetVertexBuffer(m_stone_vbo);
                graphics_device.DrawPrimitives(PrimitiveType.TriangleList, 0, m_stone_vbo.VertexCount);
            }
        }
    }
}
