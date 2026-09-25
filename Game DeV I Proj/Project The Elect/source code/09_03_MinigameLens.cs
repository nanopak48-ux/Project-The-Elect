using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;

namespace Project_The_Elect.source_code
{
    public class MinigameLens : Minigame
    {
        //====================== MINIGAME CONFIG ======================//

        // Lens
        private Vector2 LensCenter;
        private float LensRadius = 250f;

        // Pointer
        private Vector2 PointerPosition;
        private float PointerRadius = 25f;

        private float PointerX = 0f;
        private float PointerY = 0f;

        private float PointerIncrement = 15f;

        private bool PointerReverse = false;

        // Progress
        private float MinigameProgress = 0f;
        private float BaseProgressPoint = 5f;

        // Progress bar
        private int ProgressBarX = 47;
        private int ProgressBarY = 75;
        private int ProgressBarWidth = 120;
        private int ProgressBarHeight = 905;

        //=============================================================//

        public bool IsContentLoaded = false;
        public bool IsCompleted { get; protected set; }

        private OrthographicCamera _camera;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private GameAudioManager _audio;

        private Texture2D bar;

        public MinigameLens
            (
            SpriteBatch spriteBatch,
            OrthographicCamera camera,
            ContentManager content,
            GameAudioManager audio
            )
        {
            _spriteBatch = spriteBatch;
            _camera = camera;
            _content = content;
            _audio = audio;
        }

        public override void Initialize()
        {
            IsClosed = false;
            MinigameProgress = 0f;

            //========================================
            // Lens อยู่ตรงกลางหน้าจอ
            //========================================

            LensCenter = new Vector2(
                GameConfig.ScreenWidth / 2f,
                GameConfig.ScreenHeight / 2f
            );

            //========================================
            // Pointer เริ่มจากด้านซ้ายของจอ
            //========================================

            PointerX = LensCenter.X - 600f;
            PointerY = LensCenter.Y;

            PointerPosition = new Vector2(
                PointerX,
                PointerY
            );
        }

        public override void LoadContent()
        {
            string prefix = "texture/09_minigame/01_scan/";

            bar = _content.Load<Texture2D>(
                prefix + "00_bar"
            );
        }

        public override void Update(GameTime gameTime)
        {
            Console.WriteLine($"Progress: {MinigameProgress}");

            //========================================
            // Pointer movement
            //========================================

            if (PointerX < LensCenter.X - 600f ||
                PointerX > LensCenter.X + 600f)
            {
                PointerReverse = true;
            }

            if (PointerReverse)
            {
                PointerIncrement *= -1;
                PointerReverse = false;
            }

            PointerX += PointerIncrement;

            PointerPosition.X = PointerX;

            //========================================
            // Close
            //========================================

            if (MinigameProgress >= 100)
            {
                Console.WriteLine(">>> CALLING CLOSE()");
                Close();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //=========================================================
            // Lens
            //=========================================================

            _spriteBatch.DrawCircle(
                LensCenter,
                LensRadius,
                64,
                Color.Green
            );

            //=========================================================
            // Pointer
            //=========================================================

            _spriteBatch.DrawCircle(
                PointerPosition,
                PointerRadius,
                32,
                Color.Red
            );

            //=========================================================
            // Progress Bar Background
            //=========================================================

            _spriteBatch.DrawRectangle(
                new Rectangle(
                    ProgressBarX,
                    ProgressBarY,
                    ProgressBarWidth,
                    ProgressBarHeight
                ),
                Color.Green
            );

            //=========================================================
            // Progress Bar
            //=========================================================

            int bottomY = ProgressBarY + ProgressBarHeight;

            int height =
                (int)(
                    MinigameProgress / 100f
                    * ProgressBarHeight
                );

            int y = bottomY - height;

            _spriteBatch.Draw(
                bar,
                new Rectangle(
                    ProgressBarX,
                    y,
                    ProgressBarWidth,
                    height
                ),
                Color.White
            );
        }

        public override void InputHandler(GameTime gameTime)
        {

            KeyboardStateExtended keyboardState =
                KeyboardExtended.GetState();

            //=========================================================
            // Space
            //=========================================================

            if (keyboardState.WasKeyPressed(Keys.Space))
            {
                // Distance ระหว่าง Pointer กับ Lens
                float distance =
                    Vector2.Distance(
                        PointerPosition,
                        LensCenter
                    );

                //=====================================================
                // Collision แบบ Circle vs Circle
                //=====================================================

                if (distance <= LensRadius + PointerRadius)
                {
                    MinigameProgress += BaseProgressPoint;

                    _audio.PlaySFX("selected");

                    Console.WriteLine("LENS HIT!");
                }
                else if (MinigameProgress > 0)
                {
                    MinigameProgress -= BaseProgressPoint * 0.2f;

                    Console.WriteLine("LENS MISS!");
                }
            }
        }

        public override void Close()
        {
            IsClosed = true;
        }
    }
}