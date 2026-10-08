using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps;

namespace Project_The_Elect.source_code
{
    public class CollisionManager
    {
        private Tilemap _map;

        public List<RectangleF> CollisionObjects { get; private set; }

        public CollisionManager(Tilemap map)
        {
            _map = map;
            CollisionObjects = new List<RectangleF>();
            LoadCollision();
        }

        private void LoadCollision()
        {
            CollisionObjects.Clear();

            foreach (var layer in _map.Layers)
            {
                if (layer is TilemapObjectLayer objectLayer && objectLayer.Name == "wall_collision")
                {
                    foreach (var obj in objectLayer.Objects)
                    {
                        float width = obj.Bounds.Max.X - obj.Bounds.Min.X;
                        float height = obj.Bounds.Max.Y - obj.Bounds.Min.Y;

                        RectangleF rect = new RectangleF(
                            obj.Bounds.Min.X,
                            obj.Bounds.Min.Y,
                            width,
                            height
                        );

                        CollisionObjects.Add(rect);
                    }
                }
            }
        }
        //====================================================================//
        public void DrawDebug(SpriteBatch spriteBatch)
        {
            foreach (var wall in CollisionObjects)
            {
                //spriteBatch.DrawRectangle(wall, Color.Red, 2f);
            }
        }
        //====================================================================//
    }
}