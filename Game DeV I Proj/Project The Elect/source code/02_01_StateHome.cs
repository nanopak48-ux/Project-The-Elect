using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public class StateHome : IGameState
    {
        private bool IsLoadedContent = false;
        private bool IsPlayingBGM = false;

        private SpriteBatch _spriteBatch;
        public GraphicsDeviceManager _graphics;
        private ContentManager _content;
        private List<Texture2D> background;
        private GameAudioManager _audio;
        private GameStateManager _gameStateManager;

        private GameFlowManager _gameFlow;

        private int screenWidth = GameConfig.ScreenWidth;
        private int screenHeight = GameConfig.ScreenHeight;
        private KeyboardState _previousKeyboardState;


        public StateHome
            (
            GameStateManager _stateManager,
            GameFlowManager _gameflow,
            SpriteBatch _spritebatch,
            ContentManager _content,
            GameAudioManager _audio
            )
        {
            _gameStateManager = _stateManager;
            this._gameFlow = _gameflow;
            this._spriteBatch = _spritebatch;
            this._content = _content;
            this._audio = _audio;
            _previousKeyboardState = Keyboard.GetState();

            // A state can be drawn immediately after it is installed, before Update runs.
            LoadContent();
            IsLoadedContent = true;
        }

        public void LoadContent()
        {
            background = new List<Texture2D>();
            background.Add(_content.Load<Texture2D>("texture/01_home/01_homeBG"));
        }

        public void Update(GameTime gameTime)
        {
            if (!IsLoadedContent)
            {
                LoadContent();
                IsLoadedContent = true;
            }
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(background[0], new Vector2(0, 0), Color.White);

            _spriteBatch.End();
        }

        

        public void InputHandler(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            bool enterPressed = currentKeyboardState.IsKeyDown(Keys.Enter)
                && _previousKeyboardState.IsKeyUp(Keys.Enter);

            if (enterPressed)
            {
                _audio.PlaySFX("speacbar");
                _gameFlow.ChangeFlow(GameFlow.Chapter01);
            }

            _previousKeyboardState = currentKeyboardState;
        }

        public void AudioHandler(GameTime gameTime)
        {
            if(!IsPlayingBGM)
            {
                _audio.PlayBGM(2);
                IsPlayingBGM = true;
            }
            
            
        }
    }
}
