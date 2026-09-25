using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public class StateMenu : IGameState
    {
        private ContentManager _content;
        private GameStateManager _gameStateManager;
        private SpriteBatch _spriteBatch;
        private GameAudioManager _audioManager;

        private string _returnState;

        private int screenWidth;
        private int screenHeight;
        private bool _isLoadedContent = false;

        public StateMenu(ContentManager content, GameStateManager gameStateManager, SpriteBatch spriteBatch, GameAudioManager audioManager)
        {
            _content = content;
            _gameStateManager = gameStateManager;
            _spriteBatch = spriteBatch;
            _audioManager = audioManager;
            this.screenWidth = GameConfig.ScreenWidth;
            this.screenHeight = GameConfig.ScreenHeight;

            LoadContentMenu();
            _audioManager.PlaySFX("menuentry"); 
            _audioManager.LowBGM(true);
        }
        private Texture2D background;
        public void LoadContentMenu()
        {
            background = _content.Load<Texture2D>("texture/05_menu/01_menuBG");
        }

        public void Update(GameTime gameTime)
        {

            InputHandler(gameTime);
        }

        public void Draw(GameTime gameTime)
        {

            _spriteBatch.Begin();
            // Draw menu items
            _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);

            _spriteBatch.End();
        }

        private KeyboardState _previousKeyboardState;
        private MouseState _previousMouseState;
        private bool _isMenuOpen = true;

        public void InputHandler(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            bool keyMPressed = currentKeyboardState.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape);

            if (keyMPressed)
            {
                _isMenuOpen = !_isMenuOpen;
                if(_isMenuOpen)
                {         
                    _audioManager.PlaySFX("menuentry");
                    _audioManager.LowBGM(false);
                    _gameStateManager.StateReturn();
                }
            }

            _previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;
        }

        public void AudioHandler(GameTime gameTime)
        {

        }
    }
}
