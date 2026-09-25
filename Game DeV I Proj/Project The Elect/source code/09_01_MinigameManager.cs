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

        public MinigameManager()
        {
            LoadMinigameContent();
        }

        public void Start(Minigame minigame)
        {
            _currentMinigame = minigame;

            _currentMinigame.Initialize();
            _currentMinigame.LoadContent();
        }

        /*public void Update(GameTime gameTime)
        {
            if (_currentMinigame == null)
                return;

            _currentMinigame.Update(gameTime);

            Console.WriteLine(
                "IsClosed = " + _currentMinigame.IsClosed
            );

            if (_currentMinigame.IsClosed)
            {
                Console.WriteLine("MANAGER CLOSING MINIGAME");

                _currentMinigame = null;
            }
        }*/
        public void Update(GameTime gameTime)
        {
            if (_currentMinigame == null)
                return;

            _currentMinigame.Update(gameTime);

            Console.WriteLine($"Manager IsClosed: {_currentMinigame.IsClosed}");

            if (_currentMinigame.IsClosed)
            {
                Console.WriteLine(">>> MANAGER REMOVING MINIGAME");
                _currentMinigame = null;
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
