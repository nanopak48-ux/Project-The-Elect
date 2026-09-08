using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect
{
    public interface GameState
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void InputHandler(GameTime gameTime);
        void AudioHandler(GameTime gameTime);
    }
    public class GameStateManager : GameState
    {
        private SpriteBatch _spriteBatch;
        public GameState CurrentState 
        { 
            get; 
            private set;
        }

        public void StateSetTo(GameState newState)
        {
            CurrentState = newState;
        }

        public void Update(GameTime gameTime)
        {
            CurrentState?.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            CurrentState?.Draw(gameTime);
        }
        
        public void InputHandler(GameTime gameTime)
        {
            CurrentState?.InputHandler(gameTime);
        }

        public void AudioHandler(GameTime gameTime)
        {
            CurrentState?.AudioHandler(gameTime);
        }
    }
}
