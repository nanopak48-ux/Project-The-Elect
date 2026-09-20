using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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

        public StatePlay
            (
            GameStateManager _stateManager,
            GameFlowManager _gameflow,
            SpriteBatch _spritebatch,
            ContentManager _content,
            GameAudioManager _audio
            )
        {
            _gameState = _stateManager;
            this._gameFlow = _gameflow;
            this._spriteBatch = _spritebatch;
            this._content = _content;
            this._audio = _audio;

            LoadContent();
        }

        private Player player;
        public void LoadContent()
        {
            Texture2D texture = _content.Load<Texture2D>("00_spritesheet_player");
            player = new Player(_spriteBatch, new Vector2(500, 500), _content);

        }
        public void Update(GameTime gameTime)
        {
            player.Update();
        }

        public void InputHandler(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            player.Draw();
            _spriteBatch.End();
        }

        public void AudioHandler(GameTime gameTime)
        {

        }
    }
}
