using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private CollisionManager Collision;

        private Player _player;
        private GraphicsDevice _graphicDevice;
        private GameMapManager _map;

        private OrthographicCamera _camera;
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
            _graphics.PreferredBackBufferWidth = GameConfig.ScreenWidth;
            _graphics.PreferredBackBufferHeight = GameConfig.ScreenHeight;
            _graphics.ApplyChanges();

            ViewportAdapter viewportAdapter = new BoxingViewportAdapter(_window, _graphicDevice, GameConfig.CameraWidth*3, GameConfig.CameraHeight*3);
            _camera = new OrthographicCamera(viewportAdapter);
            viewportAdapter.Reset();

            _minigame = new MinigameManager();
            _map = new GameMapManager(_content,_graphics.GraphicsDevice,_spriteBatch);
        }

        private Texture2D map;
        public void LoadContent()
        {
            Texture2D texture = _content.Load<Texture2D>("texture/08_player/00_spritesheet_player");  
            _player = new Player(_spriteBatch, new Vector2(37*64, 22*64), _content, texture);
            map = _content.Load<Texture2D>("texture/04_play/00_map");

            _map.LoadContent();
        }

        public Vector2 currentCenter;

        public void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            _map.Update(gameTime);

            Console.WriteLine(_player.Position);

            //Collision.Update(_player,_camera);

            if (_minigame.IsPlaying)
            {
                _minigame.Update(gameTime);
                return;
            }

            Vector2 lookAtPos = _player.Position;
            currentCenter = _camera.Position + _camera.Origin;
            _camera.LookAt(Vector2.Lerp(currentCenter, lookAtPos, 0.1f));
        }

        public void InputHandler(GameTime gameTime)
        {
            KeyboardExtended.Update();
            CheckEscape();
            if (_minigame.IsPlaying)
            {
                _minigame.InputHandler(gameTime);
                return;
            }
            else InputHandlerMinigame();
        }

        public void Draw(GameTime gameTime)
        {
            Matrix transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(transformMatrix: transformMatrix);


            _map.Draw(gameTime, _camera);
            _player.Draw();

            _spriteBatch.End();

            _spriteBatch.Begin();
            if (_minigame.IsPlaying) _minigame.Draw(gameTime);
            _spriteBatch.End();
        }

        public void AudioHandler(GameTime gameTime)
        {
            _player.Audio(gameTime);
        }
        private void InputHandlerMinigame()
        {
            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();


            if (keyboardState.WasKeyPressed(Keys.U))
            {
                _minigame.Start(new MinigameScan(_spriteBatch,_camera, _content, _audio));
            }
            else if (keyboardState.WasKeyPressed(Keys.I))
            {
                _minigame.Start(new MinigameLens(_spriteBatch, _camera, _content, _audio));
            }
            else if (keyboardState.WasKeyPressed(Keys.O))
            {
                
            }
            else if (keyboardState.WasKeyPressed(Keys.P))
            {
               
            }


        }

        private void CheckEscape()
        {
            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyPressed(Keys.Escape))
            {
                _gameState.StatePush(new StateMenu(_content, _gameState, _spriteBatch, _audio));
            }
        }
    }
}
