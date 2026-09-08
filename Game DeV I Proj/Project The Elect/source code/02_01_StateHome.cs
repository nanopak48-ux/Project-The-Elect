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
    public class StateHome : GameState
    {
        private bool IsLoadedContent = false;
        private bool IsPlayingBGM = false;

        private SpriteBatch _spriteBatch;
        public GraphicsDeviceManager _graphics;
        private ContentManager _content;
        private List<Texture2D> background;
        private GameAudioManager _audioManager;
        private GameStateManager _gameStateManager;

        private int screenWidth;
        private int screenHeight;


        public StateHome(ContentManager content,GameStateManager gameStateManager,SpriteBatch spriteBatch,GameAudioManager audioManager,int screenWidth,int screenHeight)
        {
            _spriteBatch = spriteBatch;
            _gameStateManager = gameStateManager;
            _content = content;
            _audioManager = audioManager;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
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

            InputHandler(gameTime);
            AudioHandler(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(background[0], new Vector2(0, 0), Color.White);

            _spriteBatch.End();
        }

        public void InputHandler(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.Enter))
            {  
                _audioManager.PlaySFX(1);
                _gameStateManager.StateSetTo(new StateDialogue(_content, _gameStateManager, _spriteBatch, _audioManager, screenWidth, screenHeight));
            }
        }

        public void AudioHandler(GameTime gameTime)
        {
            if(!IsPlayingBGM)
            {
                _audioManager.PlayBGM(0);
                IsPlayingBGM = true;
            }
            
            
        }
    }
}
