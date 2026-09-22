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

        public void Start(Minigame minigame)
        {
            _currentMinigame = minigame;
            _currentMinigame.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            if (_currentMinigame == null)
                return;

            _currentMinigame.Update(gameTime);
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
    }
}
