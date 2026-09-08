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
    public class StateMenu : GameState
    {
        private ContentManager _content;
        private GameStateManager _gameStateManager;
        private SpriteBatch _spriteBatch;
        private GameAudioManager _audioManager;

        private string _returnState;

        private int screenWidth;
        private int screenHeight;

        public StateMenu(string InputState,ContentManager content, GameStateManager gameStateManager, SpriteBatch spriteBatch, GameAudioManager audioManager, int screenWidth, int screenHeight)
        {

            _returnState = InputState;
            _content = content;
            _gameStateManager = gameStateManager;
            _spriteBatch = spriteBatch;
            _audioManager = audioManager;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }
        public void Update(GameTime gameTime)
        {
            InputHandler(gameTime);
        }

        public void Draw(GameTime gameTime)
        {

        }

        public void InputHandler(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyUp(Keys.M))
            {
                _gameStateManager.StateSetTo(new StateDialogue(_content, _gameStateManager, _spriteBatch, _audioManager, screenWidth, screenHeight));
            }
        }

        public void AudioHandler(GameTime gameTime)
        {

        }
    }
}
