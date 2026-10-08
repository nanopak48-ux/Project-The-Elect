using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;

namespace Project_The_Elect.source_code
{
    public class MinigameKeypad : Minigame
    {
        private enum Phase
        {
            StartDelay,
            ShowingColor,
            ColorGap,
            ReadyDelay,
            Answering,
            RoundPause,
            FailurePause,
            FinalSuccessPause
        }

        private enum RoundStatus
        {
            Pending,
            Success,
            Failed
        }

        private const int RoundCount = 3;
        private const int PaletteSize = 6;
        private const float ColorShowDuration = 0.65f;
        private const float ColorGapDuration = 0.25f;
        private const float ReadyDelayDuration = 1f;
        private const float StartDelayDuration = 1f;
        private const float RoundPauseDuration = 0.8f;
        private const float ResultPauseDuration = 0.7f;

        private static readonly float[] AnswerDurations = { 5f, 10f, 15f };
        private static readonly int[] NumbersPerRound = { 3, 4, 5 };
        private static readonly string[] BottleColors = { "red", "green", "blue", "orage", "pink", "yellow" };
        private const int FrameX = 72;
        private const int FrameY = 40;
        private const int FrameWidth = 1776;
        private const int FrameHeight = 850;
        private const int CenterX = GameConfig.ScreenWidth / 2;
        private const int CenterY = 455;
        private const int SmallBottleWidth = 96;
        private const int SmallBottleHeight = 150;
        private const int LargeBottleWidth = 128;
        private const int LargeBottleHeight = 160;
        private const int PaletteSpacing = 180;
        private const int PaletteStartX = CenterX - (PaletteSpacing * (PaletteSize - 1)) / 2;
        private const int PaletteY = 988;
        private const int StatusY = 96;
        private const int StatusStartX = 1450;
        private const int StatusSpacing = 160;
        private const int StatusRadius = 28;
        private const int TimerX = 260;
        private const int TimerY = 805;
        private const int TimerWidth = 1400;
        private const int TimerHeight = 34;

        private readonly SpriteBatch _spriteBatch;
        private readonly ContentManager _content;
        private readonly GameAudioManager _audio;
        private readonly Random _random = new Random();
        private readonly RoundStatus[] _roundStatuses = new RoundStatus[RoundCount];
        private readonly Texture2D[] _smallBottleTextures = new Texture2D[PaletteSize];
        private readonly Texture2D[] _largeBottleTextures = new Texture2D[PaletteSize];

        private Texture2D _scorebarTexture;
        private Texture2D _scorebarCompleteTexture;
        private Texture2D _scorebarFailTexture;
        private Texture2D _timeBarTexture;
        private Texture2D _timeInsideTexture;
        private Texture2D _bottleSelectTexture;
        private BitmapFont _font;
        private List<int> _sequence = new List<int>();
        private int _roundIndex;
        private int _sequenceIndex;
        private int _answerIndex;
        private int _selectedColorIndex;
        private int _shownColorIndex = -1;
        private Phase _phase;
        private float _phaseElapsed;
        private KeyboardState _previousKeyboardState;

        public MinigameKeypad(
            SpriteBatch spriteBatch,
            OrthographicCamera camera,
            ContentManager content,
            GameAudioManager audio)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _audio = audio;
        }

        public override void Initialize()
        {
            ResetResult();
            Array.Fill(_roundStatuses, RoundStatus.Pending);
            _roundIndex = 0;
            _selectedColorIndex = 0;
            _phaseElapsed = 0f;
            _previousKeyboardState = Keyboard.GetState();
            BeginRound(delayBeforeSequence: true);
        }

        public override void LoadContent()
        {
            _font = _content.Load<BitmapFont>("font/fontGenshin");

            const string texturePrefix = "texture/09_minigame/02_keypad/";
            for (int i = 0; i < PaletteSize; i++)
            {
                string color = BottleColors[i];
                string index = i.ToString("00");
                _largeBottleTextures[i] = _content.Load<Texture2D>(texturePrefix + index + "_bottle_" + color);
                _smallBottleTextures[i] = _content.Load<Texture2D>(texturePrefix + index + "_bottle_" + color + "_small");
            }

            _scorebarTexture = _content.Load<Texture2D>(texturePrefix + "06_scorebar");
            _scorebarCompleteTexture = _content.Load<Texture2D>(texturePrefix + "07_scorebar_complete");
            _scorebarFailTexture = _content.Load<Texture2D>(texturePrefix + "08_scorebar_fail");
            _timeBarTexture = _content.Load<Texture2D>(texturePrefix + "09_bartime");
            _timeInsideTexture = _content.Load<Texture2D>(texturePrefix + "10_timeinside");
            _bottleSelectTexture = _content.Load<Texture2D>(texturePrefix + "11_bottleselect");
        }

        public override void Update(GameTime gameTime)
        {
            if (IsClosed)
                return;

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _phaseElapsed += elapsed;

            switch (_phase)
            {
                case Phase.StartDelay:
                    if (_phaseElapsed >= StartDelayDuration)
                        ShowNextColor();
                    break;

                case Phase.ShowingColor:
                    if (_phaseElapsed >= ColorShowDuration)
                    {
                        _shownColorIndex = -1;
                        SetPhase(Phase.ColorGap);
                    }
                    break;

                case Phase.ColorGap:
                    if (_phaseElapsed >= ColorGapDuration)
                    {
                        _sequenceIndex++;
                        if (_sequenceIndex < _sequence.Count)
                        {
                            ShowNextColor();
                        }
                        else
                        {
                            SetPhase(Phase.ReadyDelay);
                        }
                    }
                    break;

                case Phase.ReadyDelay:
                    if (_phaseElapsed >= ReadyDelayDuration)
                    {
                        _answerIndex = 0;
                        _selectedColorIndex = 0;
                        SetPhase(Phase.Answering);
                    }
                    break;

                case Phase.Answering:
                    if (_phaseElapsed >= AnswerDurations[_roundIndex])
                        BeginFailure();
                    break;

                case Phase.RoundPause:
                    if (_phaseElapsed >= RoundPauseDuration)
                    {
                        _roundIndex++;
                        BeginRound();
                    }
                    break;

                case Phase.FailurePause:
                    if (_phaseElapsed >= ResultPauseDuration)
                        Fail();
                    break;

                case Phase.FinalSuccessPause:
                    if (_phaseElapsed >= ResultPauseDuration)
                        Complete();
                    break;
            }
        }

        public override void InputHandler(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (_phase == Phase.Answering)
            {
                if (WasPressed(keyboard, Keys.Left))
                    _selectedColorIndex = (_selectedColorIndex + PaletteSize - 1) % PaletteSize;
                else if (WasPressed(keyboard, Keys.Right))
                    _selectedColorIndex = (_selectedColorIndex + 1) % PaletteSize;

                if (WasPressed(keyboard, Keys.Space))
                    ConfirmColor();
            }

            _previousKeyboardState = keyboard;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.DrawRectangle(
                new Rectangle(FrameX, FrameY, FrameWidth, FrameHeight),
                new Color(138, 61, 255),
                4f);

            DrawStatusDots();
            DrawCenterCircle();
            DrawTimer();
            DrawPalette();
        }

        public override void Close()
        {
            if (IsClosed)
                return;

            base.Close();
        }

        private void BeginRound(bool delayBeforeSequence = false)
        {
            int sequenceLength = NumbersPerRound[_roundIndex];
            var availableNumbers = new List<int>(PaletteSize);
            for (int i = 0; i < PaletteSize; i++)
                availableNumbers.Add(i);

            // Shuffle all six numbers, then take this round's required count.ฟ
            for (int i = availableNumbers.Count - 1; i > 0; i--)
            {
                int swapIndex = _random.Next(i + 1);
                (availableNumbers[i], availableNumbers[swapIndex]) =
                    (availableNumbers[swapIndex], availableNumbers[i]);
            }

            _sequence = availableNumbers.GetRange(0, sequenceLength);
            _sequenceIndex = 0;
            _answerIndex = 0;

            if (delayBeforeSequence)
            {
                _shownColorIndex = -1;
                SetPhase(Phase.StartDelay);
            }
            else
            {
                ShowNextColor();
            }
        }

        private void ShowNextColor()
        {
            _shownColorIndex = _sequence[_sequenceIndex];
            SetPhase(Phase.ShowingColor);
        }

        private void ConfirmColor()
        {
            if (_selectedColorIndex != _sequence[_answerIndex])
            {
                BeginFailure();
                return;
            }

            _audio.PlaySFX("selected");
            _answerIndex++;

            if (_answerIndex < _sequence.Count)
                return;

            _roundStatuses[_roundIndex] = RoundStatus.Success;
            if (_roundIndex == RoundCount - 1)
                SetPhase(Phase.FinalSuccessPause);
            else
                SetPhase(Phase.RoundPause);
        }

        private void BeginFailure()
        {
            _roundStatuses[_roundIndex] = RoundStatus.Failed;
            SetPhase(Phase.FailurePause);
        }

        private void DrawStatusDots()
        {
            for (int i = 0; i < RoundCount; i++)
            {
                int centerX = StatusStartX + i * StatusSpacing;
                var frame = new Rectangle(centerX - 32, StatusY - 32, 64, 64);
                _spriteBatch.Draw(_scorebarTexture, frame, Color.White);

                Texture2D stateTexture = _roundStatuses[i] switch
                {
                    RoundStatus.Success => _scorebarCompleteTexture,
                    RoundStatus.Failed => _scorebarFailTexture,
                    _ => null
                };

                if (stateTexture != null)
                {
                    var state = new Rectangle(centerX - 28, StatusY - 28, 64, 64);
                    _spriteBatch.Draw(stateTexture, state, Color.White);
                }
            }
        }

        private void DrawCenterCircle()
        {
            int bottleToShow = -1;

            if (_phase == Phase.ShowingColor && _shownColorIndex >= 0)
                bottleToShow = _shownColorIndex;
            else if (_phase == Phase.Answering)
                bottleToShow = _selectedColorIndex;

            if (bottleToShow >= 0)
            {
                var bottleBounds = new Rectangle(
                    CenterX - LargeBottleWidth / 2,
                    CenterY - LargeBottleHeight / 2,
                    LargeBottleWidth,
                    LargeBottleHeight);
                _spriteBatch.Draw(_largeBottleTextures[bottleToShow], bottleBounds, Color.White);
            }
        }

        private void DrawTimer()
        {
            float remaining = 1f;
            if (_phase == Phase.Answering)
            {
                remaining = MathHelper.Clamp(
                    1f - _phaseElapsed / AnswerDurations[_roundIndex],
                    0f,
                    1f);
            }
            else if (_phase == Phase.FailurePause)
            {
                remaining = 0f;
            }

            _spriteBatch.DrawString(_font, "Time", new Vector2(GameConfig.ScreenWidth / 2f - 24f, 755f), Color.Black);
            int fillWidth = (int)(TimerWidth * remaining);
            if (fillWidth > 0)
            {
                _spriteBatch.Draw(
                    _timeInsideTexture,
                    new Rectangle(TimerX, TimerY, fillWidth, TimerHeight),
                    new Rectangle(0, 0, fillWidth, TimerHeight),
                    Color.White);
            }

            // Draw the frame last so the shrinking fill never covers its border.
            _spriteBatch.Draw(_timeBarTexture, new Rectangle(TimerX, TimerY, TimerWidth, TimerHeight), Color.White);
        }

        private void DrawPalette()
        {
            for (int i = 0; i < PaletteSize; i++)
            {
                int centerX = PaletteStartX + i * PaletteSpacing;

                var bottleBounds = new Rectangle(
                    centerX - SmallBottleWidth / 2,
                    PaletteY - SmallBottleHeight / 2,
                    SmallBottleWidth,
                    SmallBottleHeight);
                _spriteBatch.Draw(_smallBottleTextures[i], bottleBounds, Color.White);

                if (_phase == Phase.Answering && i == _selectedColorIndex)
                    _spriteBatch.Draw(_bottleSelectTexture, bottleBounds, Color.White);
            }
        }

        private void SetPhase(Phase phase)
        {
            _phase = phase;
            _phaseElapsed = 0f;
        }

        private bool WasPressed(KeyboardState current, Keys key)
        {
            return current.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }
    }
}
