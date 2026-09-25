using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public class MinigameScan : Minigame
    {
        //====================== MINIGAME CONFIG ======================//
        private int BarWidth = 1200;
        private int BarHeight = 180;

        private Rectangle Pointer;
        private float PointerX = 0f;
        private int PointerY = 0;
        private int PointerWidth = 30;
        private int PointerHeight = 136;
        private float PointerIncrement = 15f;

        private float BarIncrement = 10f;

        private float GreenBar = 0f;
        private bool PointerReverse = false;
        private bool BarReverse = false;

        private float MinigameProgress = 0f;
        private float BaseProgressPoint = 30f;
        //=============================================================//

        public bool IsContentLoaded = false;
        public bool IsCompleted { get; protected set; }
        private OrthographicCamera _camera;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private Texture2D bar;

        private GameAudioManager _audio;

        public MinigameScan
            (
            SpriteBatch spriteBatch,
            OrthographicCamera camera,
            ContentManager content,
            GameAudioManager audio
            )
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _audio = audio;

        }

        public override void Initialize()
        {
            IsClosed = false;
            PointerX = (GameConfig.ScreenWidth - BarWidth)/ 2;
            PointerY = (GameConfig.ScreenHeight) / 4;
            Pointer = new Rectangle((int)PointerX, PointerY,PointerWidth,PointerHeight);

            GreenBar = (GameConfig.ScreenWidth - BarWidth) / 2;
        }

        public override void LoadContent()
        {
            string prefix = "texture/09_minigame/01_scan/";
            bar = _content.Load<Texture2D>(prefix + "00_bar");

            _audio.PlaySFX("exit");
        }
        public override void Update(GameTime gameTime)
        {
            Console.WriteLine($"Progress: {MinigameProgress}");

            if (PointerX < (GameConfig.ScreenWidth - BarWidth) / 2 ||
                PointerX > ((GameConfig.ScreenWidth - BarWidth) / 2) + BarWidth)
            {
                PointerReverse = true;
            }

            if (PointerReverse)
            {
                PointerIncrement *= -1;
                PointerReverse = false;
            }

            PointerX += PointerIncrement;

            if (GreenBar < (GameConfig.ScreenWidth - BarWidth) / 2 ||
                GreenBar > ((GameConfig.ScreenWidth - BarWidth) / 2) + BarWidth)
            {
                BarReverse = true;
            }

            if (BarReverse)
            {
                BarIncrement *= -1;
                BarReverse = false;
            }

            GreenBar += BarIncrement;

            if (MinigameProgress >= 100)
            {
                Console.WriteLine(">>> CALLING CLOSE()");
                Close();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            int bottomY = 905+75;
            int maxHeight = 905;

            int height = (int)(MinigameProgress / 100f * maxHeight);
            int y = bottomY - height;

            _spriteBatch.DrawRectangle(new Rectangle((GameConfig.ScreenWidth - BarWidth) / 2, (GameConfig.ScreenHeight) / 4, BarWidth, BarHeight), Color.Green);

            _spriteBatch.Draw(bar, new Rectangle((int)GreenBar, (GameConfig.ScreenHeight) / 4, 120, BarHeight), Color.White);

            _spriteBatch.DrawRectangle(new Rectangle((int)PointerX, PointerY, PointerWidth, PointerHeight), Color.Red); 

            _spriteBatch.DrawRectangle(new Rectangle(47, 75, 120, 905),Color.Green);

            _spriteBatch.DrawRectangle(new Rectangle((int)GreenBar - BarHitboxPadding,
                                        GameConfig.ScreenHeight / 4,
                                        120 + BarHitboxPadding * 2,
                                        BarHeight), Color.Purple);
            _spriteBatch.Draw(bar,new Rectangle(47, y, 120, height),Color.White);
        }

        private int BarHitboxPadding = 30;
        private int PointerHitboxPadding = 15;

        public override void InputHandler(GameTime gameTime)
        {
            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();

            Rectangle bar = new Rectangle((int)GreenBar-20, (GameConfig.ScreenHeight) / 4, 60+20, BarHeight);
            Rectangle Pointer = new Rectangle((int)PointerX, PointerY, PointerWidth, PointerHeight);

            Rectangle barHitbox = new Rectangle((int)GreenBar - BarHitboxPadding,
                                        GameConfig.ScreenHeight / 4,
                                        120 + BarHitboxPadding * 2,
                                        BarHeight);

            Rectangle pointerHitbox = new Rectangle(
                (int)PointerX - PointerHitboxPadding,
                PointerY - PointerHitboxPadding,
                PointerWidth + PointerHitboxPadding * 2,
                PointerHeight + PointerHitboxPadding * 2
            );

            if (keyboardState.WasKeyPressed(Keys.Space))
            {
                if (barHitbox.Intersects(pointerHitbox))
                {
                    MinigameProgress += BaseProgressPoint;
                    _audio.PlaySFX("selected");
                }
                else if (MinigameProgress > 0)
                {
                    MinigameProgress -= (int)(BaseProgressPoint * 0.2);
                }
            }
        }

        public override void Close()
        {
            _audio.PlaySFX("proceed");
            IsClosed = true;
        }

    }
}
