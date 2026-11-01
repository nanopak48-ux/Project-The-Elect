using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.IO;
using System.Text.Json;

namespace Project_The_Elect.source_code
{
    public class MinigameLens : Minigame
    {
        private const int TrackWidth = 1200;
        private const int TrackHeight = 240;
        private const int TargetWidth = 140;
        private const int TargetHeight = 170;
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
        private const int ScanVisualWidth = 912;
        private const int ScanVisualHeight = 432;
        private const int ScanVisualY = 260;

        private static int TrackX => (GameConfig.ScreenWidth - TrackWidth) / 2;
        private static int TrackY => GameConfig.ScreenHeight / 2 + 250;
        private static int TargetY => TrackY + (TrackHeight - TargetHeight) / 2 + 10;
        private static int PointerY => TrackY + (TrackHeight - PointerHeight) / 2 + 10;

        private const int ProgressX = 107;
        private const int ProgressY = 75;
        private const int ProgressWidth = 120;
        private const int ProgressHeight = 905;
        private const int BackdropMargin = 27;
        private const int ProgressBorderThickness = 6;
        private const int ProgressInnerWidth = ProgressWidth - ProgressBorderThickness * 2;
        private const int ProgressInnerHeight = ProgressHeight - ProgressBorderThickness * 2;

        private readonly SpriteBatch _spriteBatch;
        private readonly ContentManager _content;
        private readonly GameAudioManager _audio;
        private BitmapFont _font;
        private readonly int _monsterIndex;
        private Texture2D[] _scanFrames;
        private Texture2D _scanSuccessTexture;
        private Texture2D _scanFailTexture;
        private float _scanFrameDuration = 0.1f;
        private float _resultDisplayDuration = 1.5f;
        private float _scanAnimationElapsed;
        private float _resultDisplayRemaining;
        private bool _isShowingResult;
        private bool _resultWasSuccess;
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
            GameAudioManager audio, int monsterIndex = 1)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _audio = audio;
            _monsterIndex = monsterIndex >= 2 ? 2 : 1;
        }

        private Rectangle TrackRect => new(TrackX, TrackY, TrackWidth, TrackHeight);
        private Rectangle TargetRect => new((int)_targetX, TargetY, TargetWidth, TargetHeight);
        private Rectangle PointerRect => new((int)_pointerX, PointerY, PointerWidth, PointerHeight);

        public override void Initialize()
        {
            ResetResult();
            _progress = StartingProgress;
            _elapsedPlayTime = 0f;
            _scanAnimationElapsed = 0f;
            _isShowingResult = false;
            _resultWasSuccess = false;
            _resultDisplayRemaining = 0f;
            _pointerX = TrackX;
            _targetX = TrackX + (TrackWidth - TargetWidth) / 2f;
            _targetDirection = -1f;
            _targetSpeed = InitialTargetSpeed;
            _targetDirectionTimer = 0f;
        }

        public override void LoadContent()
        {
            const string texturePrefix = "texture/09_minigame/01_scan/";
            _font = _content.Load<BitmapFont>("font/fontGenshin");
            _barTexture = _content.Load<Texture2D>(texturePrefix + "00_bar");
            _pointerTexture = _content.Load<Texture2D>(texturePrefix + "01_pointer");
            _targetTexture = _content.Load<Texture2D>(texturePrefix + "02_traget");
            _barScoreTexture = _content.Load<Texture2D>(texturePrefix + "03_barscore");
            _barInsideTexture = _content.Load<Texture2D>(texturePrefix + "04_barinside");
            LoadScanVisuals();

            _audio.PlaySFX("scan_start");

        }

        public override void Update(GameTime gameTime)
        {
            if (IsClosed)
                return;

            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_isShowingResult)
            {
                _resultDisplayRemaining -= elapsedSeconds;
                if (_resultDisplayRemaining <= 0f)
                {
                    if (_resultWasSuccess)
                        Complete();
                    else
                        Fail();
                }
                return;
            }

            _scanAnimationElapsed += elapsedSeconds;
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
                BeginResultDisplay(success: true);
            }
            else if (_progress <= 0f)
            {
                _audio.PlaySFX("error_task");
                BeginResultDisplay(success: false);
            }
        }

        public override void InputHandler(GameTime gameTime)
        {
            // The held state is read in Update so pointer and overlap use the same frame.
        }

        public override void Draw(GameTime gameTime)
        {
            MinigameBackdrop.Draw(_spriteBatch, MinigameBackdrop.Bounds);
            Rectangle visualBounds = new(
                (GameConfig.ScreenWidth - ScanVisualWidth) / 2,
                ScanVisualY,
                ScanVisualWidth,
                ScanVisualHeight);

            if (_isShowingResult)
            {
                Texture2D resultTexture = _resultWasSuccess ? _scanSuccessTexture : _scanFailTexture;
                _spriteBatch.Draw(resultTexture, visualBounds, Color.White);
            }
            else
            {
                int frameIndex = (int)(_scanAnimationElapsed / _scanFrameDuration) % _scanFrames.Length;
                _spriteBatch.Draw(_scanFrames[frameIndex], visualBounds, Color.White);
            }
            _spriteBatch.Draw(_barTexture, TrackRect, Color.White);
            DrawControlHint("Hold [ Space bar ]");
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

        private void LoadScanVisuals()
        {
            string manifestPath = Path.Combine(
                AppContext.BaseDirectory,
                _content.RootDirectory,
                "data",
                "dataMinigame",
                "lens_scan_animation.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            ScanVisualManifest manifest = JsonSerializer.Deserialize<ScanVisualManifest>(File.ReadAllText(manifestPath), options);
            if (manifest == null || manifest.Monsters == null)
                throw new InvalidDataException("lens_scan_animation.json is missing animation data.");

            ScanVisualMonsterAnimation animation = manifest.Monsters.Find(item => item.MonsterIndex == _monsterIndex);
            if (animation == null || animation.FrameCount <= 0)
                throw new InvalidDataException($"No Lens scan animation is configured for Monster {_monsterIndex}.");

            _scanFrameDuration = Math.Max(0.01f, manifest.FrameDurationSeconds);
            _resultDisplayDuration = Math.Max(0.1f, manifest.ResultDisplaySeconds);
            _scanFrames = new Texture2D[animation.FrameCount];
            for (int i = 0; i < animation.FrameCount; i++)
                _scanFrames[i] = _content.Load<Texture2D>(animation.FramePrefix + i.ToString("00"));

            _scanSuccessTexture = _content.Load<Texture2D>(manifest.SuccessTexture);
            _scanFailTexture = _content.Load<Texture2D>(manifest.FailureTexture);
        }

        private void BeginResultDisplay(bool success)
        {
            _isShowingResult = true;
            _resultWasSuccess = success;
            _resultDisplayRemaining = _resultDisplayDuration;
        }

        private sealed class ScanVisualManifest
        {
            public float FrameDurationSeconds { get; set; }
            public float ResultDisplaySeconds { get; set; }
            public string SuccessTexture { get; set; }
            public string FailureTexture { get; set; }
            public System.Collections.Generic.List<ScanVisualMonsterAnimation> Monsters { get; set; }
        }

        private sealed class ScanVisualMonsterAnimation
        {
            public int MonsterIndex { get; set; }
            public string FramePrefix { get; set; }
            public int FrameCount { get; set; }
        }
        private void DrawControlHint(string text)
        {
            const float scale = 0.8f;
            var textSize = _font.MeasureString(text);
            var position = new Vector2(
                TrackRect.Center.X - textSize.Width * scale / 2f,
                TrackRect.Top - textSize.Height * scale - 12f);

            _spriteBatch.DrawString(_font, text, position + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(_font, text, position, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
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
