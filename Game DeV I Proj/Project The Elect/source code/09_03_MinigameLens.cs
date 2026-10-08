using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;

namespace Project_The_Elect.source_code
{
    public class MinigameLens : Minigame
    {
        private const int TrackWidth = 1200;
        private const int TrackHeight = 180;
        private const int TargetWidth = 150;
        private const int PointerWidth = 30;
        private const int PointerHeight = 136;

        private const float PointerMoveSpeed = 520f;
        private const float PointerReturnSpeed = 360f;
        private const float TargetMinSpeed = 140f;
        private const float TargetMaxSpeed = 300f;
        private const float TargetDirectionChangeInterval = 1.5f;
        private const float InitialTargetSpeed = 90f;
        private const float ProgressGainPerSecond = 15f;
        private const float ProgressLossPerSecond = 10f;
        private const float StartingProgress = 30f;
        private const float MinimumWinDuration = 10f;

        private static int TrackX => (GameConfig.ScreenWidth - TrackWidth) / 2;
        private static int TrackY => GameConfig.ScreenHeight / 2 + 250;
        private static int PointerY => TrackY + (TrackHeight - PointerHeight) / 2;

        private const int ProgressX = 47;
        private const int ProgressY = 75;
        private const int ProgressWidth = 120;
        private const int ProgressHeight = 905;
        private const int ProgressBorderThickness = 6;
        private const int ProgressInnerWidth = ProgressWidth - ProgressBorderThickness * 2;
        private const int ProgressInnerHeight = ProgressHeight - ProgressBorderThickness * 2;

        private readonly SpriteBatch _spriteBatch;
        private readonly ContentManager _content;
        private readonly GameAudioManager _audio;
        private readonly Random _random = new Random();

        private Texture2D _barTexture;
        private Texture2D _pointerTexture;
        private Texture2D _targetTexture;
        private Texture2D _barScoreTexture;
        private Texture2D _barInsideTexture;
        private float _pointerX;
        private float _targetX;
        private float _targetDirection;
        private float _targetSpeed;
        private float _targetDirectionTimer;
        private float _progress;
        private float _elapsedPlayTime;

        public MinigameLens(
            SpriteBatch spriteBatch,
            OrthographicCamera camera,
            ContentManager content,
            GameAudioManager audio)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _audio = audio;
        }

        private Rectangle TrackRect => new(TrackX, TrackY, TrackWidth, TrackHeight);
        private Rectangle TargetRect => new((int)_targetX, TrackY, TargetWidth, TrackHeight);
        private Rectangle PointerRect => new((int)_pointerX, PointerY, PointerWidth, PointerHeight);

        public override void Initialize()
        {
            ResetResult();
            _progress = StartingProgress;
            _elapsedPlayTime = 0f;
            _pointerX = TrackX;
            _targetX = TrackX + (TrackWidth - TargetWidth) / 2f;
            _targetDirection = -1f;
            _targetSpeed = InitialTargetSpeed;
            _targetDirectionTimer = 0f;
        }

        public override void LoadContent()
        {
            const string texturePrefix = "texture/09_minigame/01_scan/";
            _barTexture = _content.Load<Texture2D>(texturePrefix + "00_bar");
            _pointerTexture = _content.Load<Texture2D>(texturePrefix + "01_pointer");
            _targetTexture = _content.Load<Texture2D>(texturePrefix + "02_traget");
            _barScoreTexture = _content.Load<Texture2D>(texturePrefix + "03_barscore");
            _barInsideTexture = _content.Load<Texture2D>(texturePrefix + "04_barinside");

            _audio.PlaySFX("scan_start");

        }

        public override void Update(GameTime gameTime)
        {
            if (IsClosed)
                return;

            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _elapsedPlayTime += elapsedSeconds;
            bool spaceHeld = Keyboard.GetState().IsKeyDown(Keys.Space);

            // Hold Space to move right. Release it to let the pointer drift left.
            float pointerSpeed = spaceHeld ? PointerMoveSpeed : PointerReturnSpeed;
            _pointerX += (spaceHeld ? pointerSpeed : -pointerSpeed) * elapsedSeconds;
            _pointerX = MathHelper.Clamp(
                _pointerX,
                TrackX,
                TrackX + TrackWidth - PointerWidth);

            UpdateTarget(elapsedSeconds);

            if (TargetRect.Intersects(PointerRect))
            {
                _progress = MathHelper.Clamp(
                    _progress + ProgressGainPerSecond * elapsedSeconds,
                    0f,
                    100f);
            }
            else
            {
                _progress = MathHelper.Clamp(
                    _progress - ProgressLossPerSecond * elapsedSeconds,
                    0f,
                    100f);
            }

            if (_progress >= 100f && _elapsedPlayTime >= MinimumWinDuration)
            {
                _audio.PlaySFX("selected");
                Complete();
            }
            else if (_progress <= 0f)
            {
                _audio.PlaySFX("error_task");
                Fail();
            }
        }

        public override void InputHandler(GameTime gameTime)
        {
            // The held state is read in Update so pointer and overlap use the same frame.
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Draw(_barTexture, TrackRect, Color.White);
            _spriteBatch.Draw(_targetTexture, TargetRect, Color.White);
            _spriteBatch.Draw(_pointerTexture, PointerRect, Color.White);

            int fillHeight = (int)(_progress / 100f * ProgressInnerHeight);
            int sourceY = ProgressInnerHeight - fillHeight;
            if (fillHeight > 0)
            {
                _spriteBatch.Draw(
                    _barInsideTexture,
                    new Rectangle(
                        ProgressX + ProgressBorderThickness,
                        ProgressY + ProgressBorderThickness + sourceY,
                        ProgressInnerWidth,
                        fillHeight),
                    new Rectangle(0, sourceY, ProgressInnerWidth, fillHeight),
                    Color.White);
            }

            // Draw the transparent score-bar frame over the fill to keep its border clear.
            _spriteBatch.Draw(
                _barScoreTexture,
                new Rectangle(ProgressX, ProgressY, ProgressWidth, ProgressHeight),
                Color.White);
        }

        private void UpdateTarget(float elapsedSeconds)
        {
            _targetX += _targetDirection * _targetSpeed * elapsedSeconds;

            float minX = TrackX;
            float maxX = TrackX + TrackWidth - TargetWidth;

            if (_targetX < minX)
            {
                _targetX = minX;
                _targetDirection = 1f;
            }
            else if (_targetX > maxX)
            {
                _targetX = maxX;
                _targetDirection = -1f;
            }

            _targetDirectionTimer += elapsedSeconds;
            while (_targetDirectionTimer >= TargetDirectionChangeInterval)
            {
                _targetDirectionTimer -= TargetDirectionChangeInterval;
                ChooseTargetDirectionAndSpeed();
            }
        }

        private void ChooseTargetDirectionAndSpeed()
        {
            _targetDirection = _random.Next(0, 2) == 0 ? -1f : 1f;
            _targetSpeed = (float)(_random.NextDouble() * (TargetMaxSpeed - TargetMinSpeed) + TargetMinSpeed);
        }
    }
}
