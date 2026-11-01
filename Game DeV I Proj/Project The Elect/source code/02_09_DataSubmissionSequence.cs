using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.IO;
using System.Text.Json;

namespace Project_The_Elect.source_code
{
    /// <summary>Runs the begin, loading, and done animations for data submission.</summary>
    public sealed class DataSubmissionSequence
    {
        private enum Phase
        {
            Idle,
            Begin,
            Loading,
            Done
        }

        private const int DoneRepeatCount = 2;

        private readonly SpriteBatch _spriteBatch;
        private readonly GameAudioManager _audio;
        private readonly BitmapFont _font;
        private readonly Rectangle _destination;
        private readonly float _beginFrameDuration;
        private readonly float _loadingFrameDuration;
        private readonly float _doneFrameDuration;
        private readonly Texture2D[][] _animations;

        private Phase _phase;
        private int _frameIndex;
        private float _frameTimer;
        private int _doneRepeatIndex;
        private KeyboardState _previousKeyboardState;


        public bool IsActive => _phase != Phase.Idle;
        public event Action Completed;

        public DataSubmissionSequence(
            ContentManager content,
            SpriteBatch spriteBatch,
            GameAudioManager audio,
            int screenWidth,
            int screenHeight)
        {
            _spriteBatch = spriteBatch;
            _audio = audio;
            _font = content.Load<BitmapFont>("font/fontGenshin");

            string manifestPath = Path.Combine(
                AppContext.BaseDirectory,
                content.RootDirectory,
                "data",
                "dataMinigame",
                "data_submission_animation.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            SubmissionAnimationManifest manifest = JsonSerializer.Deserialize<SubmissionAnimationManifest>(
                File.ReadAllText(manifestPath), options);

            if (manifest?.Begin == null || manifest.Loading == null || manifest.Done == null)
                throw new InvalidDataException("data_submission_animation.json is missing an animation phase.");

            _beginFrameDuration = Math.Max(0.01f, manifest.Begin.FrameDurationSeconds);
            _loadingFrameDuration = Math.Max(0.01f, manifest.Loading.FrameDurationSeconds);
            _doneFrameDuration = Math.Max(0.01f, manifest.Done.FrameDurationSeconds);
            int displayWidth = manifest.DisplayWidth > 0 ? manifest.DisplayWidth : manifest.FrameWidth;
            int displayHeight = manifest.DisplayHeight > 0 ? manifest.DisplayHeight : manifest.FrameHeight;
            _destination = new Rectangle(
                (screenWidth - displayWidth) / 2,
                (screenHeight - displayHeight) / 2,
                displayWidth,
                displayHeight);

            _animations = new[]
            {
                LoadFrames(content, manifest.Begin),
                LoadFrames(content, manifest.Loading),
                LoadFrames(content, manifest.Done)
            };
        }

        public void Start()
        {
            if (IsActive)
                return;

            _phase = Phase.Begin;
            _frameIndex = 0;
            _frameTimer = 0f;
            _doneRepeatIndex = 0;
            _previousKeyboardState = Keyboard.GetState();
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameTimer += elapsed;
            while (_frameTimer >= CurrentFrameDuration && IsActive)
            {
                _frameTimer -= CurrentFrameDuration;
                Texture2D[] frames = CurrentFrames;
                if (_frameIndex < frames.Length - 1)
                {
                    _frameIndex++;
                    continue;
                }

                if (_phase == Phase.Begin)
                {
                    // วนแอนิเมชันหน้าเริ่มไปเรื่อย ๆ ระหว่างรอผู้เล่นกด Enter
                    _frameIndex = 0;
                }
                else if (_phase == Phase.Loading)
                {
                    _phase = Phase.Done;
                    _frameIndex = 0;
                    _frameTimer = 0f;
                    break;
                }
                else if (_phase == Phase.Done)
                {
                    _doneRepeatIndex++;
                    if (_doneRepeatIndex < DoneRepeatCount)
                    {
                        _frameIndex = 0;
                    }
                    else
                    {
                        _phase = Phase.Idle;
                        _frameTimer = 0f;
                        Completed?.Invoke();
                        break;
                    }
                }
            }
        }

        public void InputHandler(GameTime gameTime)
        {
            KeyboardState current = Keyboard.GetState();
            bool spacePressed = current.IsKeyDown(Keys.Space)
                && _previousKeyboardState.IsKeyUp(Keys.Space);

            if (_phase == Phase.Begin && spacePressed)
            {
                _audio.PlaySFX("speacbar");
                _phase = Phase.Loading;
                _frameIndex = 0;
                _frameTimer = 0f;
            }

            _previousKeyboardState = current;
        }

        public void Draw()
        {
            if (!IsActive)
                return;
            _spriteBatch.Draw(CurrentFrames[_frameIndex], _destination, Color.White);

            if (_phase == Phase.Begin)
                DrawControlHint("[ Space bar ]");
        }

        private Texture2D[] CurrentFrames => _phase switch
        {
            Phase.Begin => _animations[0],
            Phase.Loading => _animations[1],
            Phase.Done => _animations[2],
            _ => Array.Empty<Texture2D>()
        };

        private float CurrentFrameDuration => _phase switch
        {
            Phase.Begin => _beginFrameDuration,
            Phase.Loading => _loadingFrameDuration,
            Phase.Done => _doneFrameDuration,
            _ => _beginFrameDuration
        };

        private void DrawControlHint(string text)
        {
            const float scale = 0.8f;
            var textSize = _font.MeasureString(text);
            float textWidth = textSize.Width * scale;
            float textHeight = textSize.Height * scale;
            float x = (_destination.Width - textWidth) / 2f + _destination.X;
            float y = Math.Min(_destination.Bottom + 24f, GameConfig.ScreenHeight - textHeight - 24f);
            var position = new Vector2(x, y);

            _spriteBatch.DrawString(_font, text, position + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(_font, text, position, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        private static Texture2D[] LoadFrames(ContentManager content, AnimationPhaseManifest phase)
        {
            if (string.IsNullOrWhiteSpace(phase.FramePrefix) || phase.FrameCount <= 0)
                throw new InvalidDataException("A data submission animation phase has no frame configuration.");

            var frames = new Texture2D[phase.FrameCount];
            for (int i = 0; i < frames.Length; i++)
                frames[i] = content.Load<Texture2D>(phase.FramePrefix + i.ToString("00"));
            return frames;
        }

        private sealed class SubmissionAnimationManifest
        {
            public int FrameWidth { get; set; }
            public int FrameHeight { get; set; }
            public int DisplayWidth { get; set; }
            public int DisplayHeight { get; set; }
            public AnimationPhaseManifest Begin { get; set; }
            public AnimationPhaseManifest Loading { get; set; }
            public AnimationPhaseManifest Done { get; set; }
        }

        private sealed class AnimationPhaseManifest
        {
            public string FramePrefix { get; set; }
            public int FrameCount { get; set; }
            public float FrameDurationSeconds { get; set; }
        }
    }
}
