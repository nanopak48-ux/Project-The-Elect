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
    public interface IGameState
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void InputHandler(GameTime gameTime);
        void AudioHandler(GameTime gameTime);
    }
    public class GameStateManager : IGameState
    {
        private SpriteBatch _spriteBatch;

        private Stack<IGameState> _stateStack = new Stack<IGameState>();

        public IGameState CurrentState
        {
            get
            {
                return _stateStack.Peek();
            }
        }
        public void StatePush(IGameState newState)
        {
            _stateStack.Push(newState);
        }

        public void StateReturn()
        {
            _stateStack.Pop();
        }

        public void StateSetTo(IGameState newState)
        {
            _stateStack.Clear();
            _stateStack.Push(newState);
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
