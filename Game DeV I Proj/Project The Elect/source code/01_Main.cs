using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using Project_The_Elect.source_code;
using System;
using System.Collections.Generic;

namespace Project_The_Elect
{

    public static class GameConfig
    {
        public const int ScreenWidth = 1920;
        public const int ScreenHeight = 1080;

        public const int CameraWidth = ScreenWidth/2;
        public const int CameraHeight = ScreenHeight/2;

    }
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public GameStateManager StateManager = new GameStateManager();
        public GameAudioManager _audioManager;
        public GameFlowManager _gameFlow;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.IsFullScreen = true;
            Window.IsBorderless = true;
            _graphics.PreferredBackBufferWidth = GameConfig.ScreenWidth;
            _graphics.PreferredBackBufferHeight = GameConfig.ScreenHeight;
            _graphics.ApplyChanges();
            base.Initialize();  
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _audioManager = new GameAudioManager();
            _audioManager.LoadContent(Content);

            _gameFlow = new GameFlowManager(StateManager,_spriteBatch,Content,_audioManager, _graphics, Window);
            _gameFlow.ChangeFlow(GameFlow.Home);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.P))
            {
                _audioManager.PlaySFX("exit");
                Exit();
            }

            if (Keyboard.GetState().IsKeyUp(Keys.N) && Keyboard.GetState().IsKeyDown(Keys.N))
            {
                _gameFlow.SkipState();
            }

            StateManager.Update(gameTime);

            StateManager.InputHandler(gameTime);

            StateManager.AudioHandler(gameTime);

            base.Update(gameTime);
        }

        private float frames = 0f;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            StateManager.Draw(gameTime);
            frames += 0.5f;

            if (frames % 7 == 0)
            {

                Console.WriteLine(StateManager.CurrentState);
                Console.WriteLine(_gameFlow.CurrentFlow);
            }


            base.Draw(gameTime);
        }

        
    }
}
