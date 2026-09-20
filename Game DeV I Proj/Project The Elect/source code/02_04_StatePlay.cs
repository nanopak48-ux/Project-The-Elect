using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Input;

namespace Project_The_Elect.source_code
{
    public class StatePlay : IGameState
    {
        private GameStateManager _gameState;
        private GameFlowManager _gameFlow;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private GameAudioManager _audio;
        private GraphicsDeviceManager _graphics;
        private GameWindow _window;
        private MinigameManager _minigame;

        private Player player;
        private GraphicsDevice _graphicDevice;

        private OrthographicCamera camera;
        private Vector2 lookAtPos;
        public StatePlay
            (
            GameStateManager _stateManager,
            GameFlowManager _gameflow,
            SpriteBatch _spritebatch,
            ContentManager _content,
            GameAudioManager _audio,
            GraphicsDeviceManager _graphics,
            GameWindow _window
            )
        {
            _gameState = _stateManager;
            this._gameFlow = _gameflow;
            this._spriteBatch = _spritebatch;
            this._content = _content;
            this._audio = _audio;
            this._graphics = _graphics;
            this._window = _window;
            _graphicDevice = _graphics.GraphicsDevice;

            Initialize();
            LoadContent();
        }
        public void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GameConfig.CameraWidth;
            _graphics.PreferredBackBufferHeight = GameConfig.CameraHeight;
            _graphics.ApplyChanges();

            ViewportAdapter viewportAdapter = new BoxingViewportAdapter(_window, _graphicDevice, GameConfig.CameraWidth, GameConfig.CameraHeight);
            camera = new OrthographicCamera(viewportAdapter);
            viewportAdapter.Reset();

            _minigame = new MinigameManager();
        }

        private Texture2D map;
        public void LoadContent()
        {
            Texture2D texture = _content.Load<Texture2D>("texture/08_player/00_spritesheet_player");  
            player = new Player(_spriteBatch, new Vector2(500, 500), _content, texture);
            map = _content.Load<Texture2D>("texture/04_play/00_map");
        }

        public Vector2 currentCenter;

        public void Update(GameTime gameTime)
        {

            player.Update(gameTime);

            if (_minigame.IsPlaying)
            {
                _minigame.Update(gameTime);
                return;
            }

            Vector2 lookAtPos = player.Position;
            currentCenter = camera.Position + camera.Origin;
            camera.LookAt(Vector2.Lerp(currentCenter, lookAtPos, 0.1f));
        }

        public void InputHandler(GameTime gameTime)
        {
            if (_minigame.IsPlaying)
            {
                _minigame.InputHandler(gameTime);
                return;
            }
        }

        public void Draw(GameTime gameTime)
        {
            Matrix transformMatrix = camera.GetViewMatrix();

            _spriteBatch.Begin(transformMatrix: transformMatrix);

            _spriteBatch.Draw(map, new Vector2(0, 0), Color.White);
            player.Draw();
            if (_minigame.IsPlaying) _minigame.Draw(gameTime);

            _spriteBatch.End();
        }

        public void AudioHandler(GameTime gameTime)
        {
            player.Audio(gameTime);
        }
    }
}
