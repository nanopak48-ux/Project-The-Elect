using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public class MinigameManager
    {
        private Minigame _currentMinigame;

        public bool IsPlaying => _currentMinigame != null;
        public event Action<Minigame, MinigameResult> MinigameEnded;

        public MinigameManager()
        {
            LoadMinigameContent();
        }

        public void Start(Minigame minigame)
        {
            if (minigame == null)
                throw new ArgumentNullException(nameof(minigame));

            if (_currentMinigame != null)
                return;

            _currentMinigame = minigame;

            _currentMinigame.Initialize();
            _currentMinigame.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (_currentMinigame == null)
                return;

            _currentMinigame.Update(gameTime);

            if (_currentMinigame.IsClosed)
            {
                Minigame endedMinigame = _currentMinigame;
                _currentMinigame = null;
                MinigameEnded?.Invoke(endedMinigame, endedMinigame.Result);
            }
        }

        public void InputHandler(GameTime gameTime)
        {
            if (_currentMinigame == null)
                return;

            _currentMinigame.InputHandler(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (_currentMinigame == null)
                return;

            _currentMinigame.Draw(gameTime);
        }

        public void Close()
        {
            _currentMinigame?.Close();
            _currentMinigame = null;
        }

        private void LoadMinigameContent()
        {

        }
    }
}
