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
        private Tilemap _tilemap;
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
        
        public void LoadContent()
        {
            _tilemap = _content.Load<Tilemap>("texture/10_map/01_tile/OperationCenterVI");
            _renderer = new TilemapSpriteBatchRenderer();

            _renderer.LoadTilemap(_tilemap);

            //Collision = new CollisionManager(_tilemap);

            _spriteBatch = new SpriteBatch(_graphic);
        }

        public void Update(GameTime gameTime)
        {
            _renderer.Update(gameTime);
        }

        public void Draw(GameTime gameTime, OrthographicCamera _camera)
        {
            _graphic.Clear(Color.Black);

            _renderer.DrawLayers(_spriteBatch, _camera,"background");

        }

    }
}
