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
        private const float ReadyDelayDuration = 2f;
        private const float RoundPauseDuration = 0.8f;
        private const float ResultPauseDuration = 0.7f;

        private static readonly float[] AnswerDurations = { 5f, 10f, 15f };
        private static readonly int[] NumbersPerRound = { 4, 5, 6 };
        private const int FrameX = 72;
        private const int FrameY = 40;
        private const int FrameWidth = 1776;
        private const int FrameHeight = 850;
        private const int CenterX = GameConfig.ScreenWidth / 2;
        private const int CenterY = 455;
        private const int CenterRadius = 170;
        private const int PaletteRadius = 66;
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

        private Texture2D _pixelTexture;
        private Texture2D _circleTexture;
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
            BeginRound();
        }

        public override void LoadContent()
        {
            _font = _content.Load<BitmapFont>("font/fontGenshin");

            _pixelTexture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            const int textureSize = 128;
            _circleTexture = new Texture2D(_spriteBatch.GraphicsDevice, textureSize, textureSize);
            var pixels = new Color[textureSize * textureSize];
            float center = (textureSize - 1) / 2f;
            float radius = textureSize / 2f - 1f;

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    byte alpha = (byte)(MathHelper.Clamp(radius + 0.5f - distance, 0f, 1f) * 255f);
                    pixels[y * textureSize + x] = new Color((byte)255, (byte)255, (byte)255, alpha);
                }
            }

            _circleTexture.SetData(pixels);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsClosed)
                return;

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _phaseElapsed += elapsed;

            switch (_phase)
            {
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

            _pixelTexture?.Dispose();
            _circleTexture?.Dispose();
            base.Close();
        }

        private void BeginRound()
        {
            int sequenceLength = NumbersPerRound[_roundIndex];
            var availableNumbers = new List<int>(PaletteSize);
            for (int i = 0; i < PaletteSize; i++)
                availableNumbers.Add(i);

            // Shuffle all six numbers, then take this round's required count.
            for (int i = availableNumbers.Count - 1; i > 0; i--)
            {
                int swapIndex = _random.Next(i + 1);
                (availableNumbers[i], availableNumbers[swapIndex]) =
                    (availableNumbers[swapIndex], availableNumbers[i]);
            }

            _sequence = availableNumbers.GetRange(0, sequenceLength);
            _sequenceIndex = 0;
            _answerIndex = 0;
            ShowNextColor();
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
                Color fillColor = _roundStatuses[i] switch
                {
                    RoundStatus.Success => Color.LimeGreen,
                    RoundStatus.Failed => Color.Red,
                    _ => Color.White
                };

                DrawFilledCircle(
                    new Vector2(StatusStartX + i * StatusSpacing, StatusY),
                    StatusRadius + 4,
                    Color.Black);
                DrawFilledCircle(
                    new Vector2(StatusStartX + i * StatusSpacing, StatusY),
                    StatusRadius,
                    fillColor);
            }
        }

        private void DrawCenterCircle()
        {
            int numberToShow = -1;

            if (_phase == Phase.ShowingColor && _shownColorIndex >= 0)
                numberToShow = _shownColorIndex + 1;
            else if (_phase == Phase.Answering)
                numberToShow = _selectedColorIndex + 1;

            DrawFilledCircle(new Vector2(CenterX, CenterY), CenterRadius + 5, Color.Black);
            DrawFilledCircle(new Vector2(CenterX, CenterY), CenterRadius, Color.White);

            if (numberToShow > 0)
                DrawNumber(numberToShow, new Vector2(CenterX, CenterY), 3.5f, 48f, 44f);
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
            _spriteBatch.DrawRectangle(
                new Rectangle(TimerX, TimerY, TimerWidth, TimerHeight),
                Color.Black,
                3f);

            int fillWidth = (int)(TimerWidth * remaining);
            if (fillWidth > 0)
            {
                _spriteBatch.Draw(
                    _pixelTexture,
                    new Rectangle(TimerX + 3, TimerY + 3, Math.Max(0, fillWidth - 6), TimerHeight - 6),
                    new Color(95, 200, 112));
            }
        }

        private void DrawPalette()
        {
            for (int i = 0; i < PaletteSize; i++)
            {
                int centerX = PaletteStartX + i * PaletteSpacing;
                var center = new Vector2(centerX, PaletteY);

                if (_phase == Phase.Answering && i == _selectedColorIndex)
                {
                    DrawFilledCircle(center, PaletteRadius + 9, new Color(138, 61, 255));
                }

                DrawFilledCircle(center, PaletteRadius + 4, Color.Black);
                DrawFilledCircle(center, PaletteRadius, Color.White);
                DrawNumber(i + 1, center, 1.5f, 18f, 22f);
            }
        }

        private void DrawNumber(int number, Vector2 center, float scale, float horizontalOffset, float verticalOffset)
        {
            _spriteBatch.DrawString(
                _font,
                number.ToString(),
                new Vector2(center.X - horizontalOffset, center.Y - verticalOffset),
                Color.Black,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }

        private void DrawFilledCircle(Vector2 center, int radius, Color color)
        {
            var destination = new Rectangle(
                (int)center.X - radius,
                (int)center.Y - radius,
                radius * 2,
                radius * 2);
            _spriteBatch.Draw(_circleTexture, destination, color);
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
