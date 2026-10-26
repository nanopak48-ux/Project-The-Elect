using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public class GameMapManager
    {
        public CollisionManager Collision { get; private set; }
        private ContentManager _content;
        private GraphicsDevice _graphic;
        private SpriteBatch _spriteBatch;
        public Tilemap _tilemap;
        private TilemapSpriteBatchRenderer _renderer;
        public GameMapManager
            (
            ContentManager content,
            GraphicsDevice graphic,
            SpriteBatch spriteBatch
            )
        {
            _content = content;
            _graphic = graphic;
            _spriteBatch = spriteBatch;
        }
        TilemapObject _objects;
        public void LoadContent()
        {
            _tilemap = _content.Load<Tilemap>("texture/10_map/01_tile/OperationCenterVI");

            //_objects = _content.Load<TilemapObject>("texture/10_map/01_tile/OperationCenterVI");

            _renderer = new TilemapSpriteBatchRenderer();

            _renderer.LoadTilemap(_tilemap);

            Collision = new CollisionManager(_tilemap);

            _spriteBatch = new SpriteBatch(_graphic);
        }

        public void Update(GameTime gameTime)
        {
            _renderer.Update(gameTime);
        }

        public void Draw(GameTime gameTime, OrthographicCamera _camera , string layer)
        {
            //_graphic.Clear(Color.Black);

            _renderer.DrawLayers(_spriteBatch, _camera, layer);
            
        }
        public List<RectangleF> GetWallCollisions()
        {
            return Collision != null ? Collision.CollisionObjects : new List<RectangleF>();
        }

        public bool HasLayer(string layerName)
        {
            return _tilemap != null && _tilemap.Layers.Any(layer => layer.Name == layerName);
        }

        public string GetFootstepSurface(Vector2 footPosition)
        {
            if (_tilemap == null)
                return null;

            TilemapTileLayer blackFloor = _tilemap.Layers.GetLayer<TilemapTileLayer>("floor_black");
            if (HasTileAt(blackFloor, footPosition))
                return "black";

            TilemapTileLayer whiteFloor = _tilemap.Layers.GetLayer<TilemapTileLayer>("floor_white");
            if (HasTileAt(whiteFloor, footPosition))
                return "white";

            return null;
        }

        private static bool HasTileAt(TilemapTileLayer layer, Vector2 worldPosition)
        {
            if (layer == null || layer.TileWidth <= 0 || layer.TileHeight <= 0)
                return false;

            int tileX = (int)Math.Floor(worldPosition.X / layer.TileWidth);
            int tileY = (int)Math.Floor(worldPosition.Y / layer.TileHeight);

            return tileX >= 0
                && tileX < layer.Width
                && tileY >= 0
                && tileY < layer.Height
                && layer.GetTile(tileX, tileY).HasValue;
        }
        //===========================================================//
        public void DrawDebug(SpriteBatch spriteBatch)
        {
            //Collision?.DrawDebug(spriteBatch);
        }
        //===========================================================//
    }
}
