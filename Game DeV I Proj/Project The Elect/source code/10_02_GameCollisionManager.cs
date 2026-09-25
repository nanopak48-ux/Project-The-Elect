using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Tiled;

namespace Project_The_Elect.source_code
{
    public class CollisionManager
    {
        private Tilemap _map;

        public List<Rectangle> CollisionObjects { get; private set; }

        public CollisionManager(Tilemap map)
        {
            _map = map;
            CollisionObjects = new List<Rectangle>();
            LoadCollision();
        }

        private void LoadCollision()
        {
            foreach (TilemapGroupLayer group in _map.Layers)
            {
                if (group.Name != "GameCollision")
                    continue;

                foreach (var Childlayers in group.ChildLayers)
                {
                    CollisionObjects.Add(new Rectangle((int)Childlayers.Bounds.X, (int)Childlayers.Bounds.Y, (int)Childlayers.Bounds.Width, (int)Childlayers.Bounds.Height));
                }

                /*foreach (var obj in group)
                {
                    CollisionObjects.Add(new Rectangle((int)obj.Bounds.X, (int)obj.Bounds.Y, (int)obj.Bounds.Width, (int)obj.Bounds.Height));
                }

                /* (var tilemapLayer in _map.Layers)
                {
                    foreach (var obj in group)
                    {
                        CollisionObjects.Add(new Rectangle((int)obj.Bounds.X, (int)obj.Bounds.Y, (int)obj.Bounds.Width, (int)obj.Bounds.Height));
                    }
                }*/
            }
        }
        public string Update(Player player,OrthographicCamera camera)
        {
            foreach (Rectangle collision in CollisionObjects)
            {
                if (!CheckCollision(collision, camera.BoundingRectangle.ToRectangle()))
                continue;

                if (CheckCollision(player.HitBox, collision)) return ToString();

            }
            return "none";
        }

        public bool CheckCollision(Rectangle Bound1,Rectangle Bound2)
        {
                if (Bound1.Intersects(Bound2))
                {
                    return true;
                }
      
            return false;
        }

    }
}
