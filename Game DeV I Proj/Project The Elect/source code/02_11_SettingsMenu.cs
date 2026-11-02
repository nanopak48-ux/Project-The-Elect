using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;

namespace Project_The_Elect.source_code
{
    /// <summary>Shared settings overlay used by Home and Pause menus.</summary>
    public sealed class SettingsMenu
    {
        private const int PanelWidth = 1180;
        private const int PanelHeight = 760;
        private const int RowStartY = 145;
        private const int RowSpacing = 88;
        private const int RowHeight = 62;
        private const int SliderWidth = 360;
        private const float VolumeStep = 0.05f;

        private static readonly string[] Options =
        {
            "MASTER VOLUME", "BACKGROUND MUSIC", "SOUND EFFECTS", "DIALOGUE", "FULLSCREEN", "BACK"
        };

        private readonly SpriteBatch _spriteBatch;
        private readonly BitmapFont _font;
        private readonly GameAudioManager _audio;
        private readonly GameFlowManager _gameFlow;
        private readonly Texture2D _pixel;
        private readonly Texture2D _panelTexture;
        private readonly Texture2D _selectedBarTexture;
        private readonly Texture2D _volumeSliderTexture;
        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private KeyboardState _previousKeyboardState;
        private MouseState _previousMouseState;
        private int _selectedIndex;

        public bool IsOpen { get; private set; }

        public SettingsMenu(
            ContentManager content,
            SpriteBatch spriteBatch,
            BitmapFont font,
            GameAudioManager audio,
            GameFlowManager gameFlow,
            int screenWidth,
            int screenHeight)
        {
            _spriteBatch = spriteBatch;
            _font = font;
            _audio = audio;
            _gameFlow = gameFlow;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _panelTexture = content.Load<Texture2D>("texture/01_home/04_settingsPanel");
            _selectedBarTexture = content.Load<Texture2D>("texture/01_home/05_settingsSelectBar");
            _volumeSliderTexture = content.Load<Texture2D>("texture/01_home/06_settingsVolumeSlider");
            _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Open()
        {
            _selectedIndex = 0;
            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();
            IsOpen = true;
        }

        public void InputHandler()
        {
            if (!IsOpen)
                return;

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            bool mouseMoved = mouse.X != _previousMouseState.X || mouse.Y != _previousMouseState.Y;

            if (WasPressed(keyboard, Keys.Escape))
            {
                Close();
                UpdatePreviousInput(keyboard, mouse);
                return;
            }

            if (WasPressed(keyboard, Keys.Up))
                ChangeSelection(-1);
            else if (WasPressed(keyboard, Keys.Down))
                ChangeSelection(1);

            if (WasPressed(keyboard, Keys.Left))
                AdjustSelectedVolume(-VolumeStep);
            else if (WasPressed(keyboard, Keys.Right))
                AdjustSelectedVolume(VolumeStep);

            bool confirmPressed = WasPressed(keyboard, Keys.Enter) || WasPressed(keyboard, Keys.Space);
            if (confirmPressed)
            {
                if (_selectedIndex == 4)
                    _gameFlow.ToggleFullScreen();
                else if (_selectedIndex == 5)
                    Close();
            }

            bool mouseClicked = mouse.LeftButton == ButtonState.Pressed
                && _previousMouseState.LeftButton == ButtonState.Released;
            for (int i = 0; i < Options.Length; i++)
            {
                Rectangle row = GetRowBounds(i);
                if (!row.Contains(mouse.Position))
                    continue;

                if (mouseMoved)
                    _selectedIndex = i;
                if (mouseClicked)
                {
                    Rectangle slider = GetSliderBounds(i);
                    if (i < 4 && slider.Contains(mouse.Position))
                    {
                        float value = MathHelper.Clamp((mouse.X - slider.X) / (float)slider.Width, 0f, 1f);
                        SetVolume(i, value);
                    }
                    else if (i == 4)
                        _gameFlow.ToggleFullScreen();
                    else if (i == 5)
                        Close();
                }

                break;
            }

            UpdatePreviousInput(keyboard, mouse);
        }

        public void Draw()
        {
            if (!IsOpen)
                return;

            _spriteBatch.Draw(_pixel,
                new Rectangle(0, 0, _screenWidth, _screenHeight),
                new Color((byte)0, (byte)0, (byte)0, (byte)175));

            int panelX = (_screenWidth - PanelWidth) / 2;
            int panelY = (_screenHeight - PanelHeight) / 2;
            var panel = new Rectangle(panelX, panelY, PanelWidth, PanelHeight);
            _spriteBatch.Draw(_panelTexture, panel, Color.White);
            DrawCenteredText("SETTINGS", panelY + 42, 1.3f, Color.White);

            for (int i = 0; i < Options.Length; i++)
                DrawRow(i, panelX, panelY);

            DrawCenteredText("UP / DOWN: SELECT     LEFT / RIGHT: ADJUST     SPACE: CONFIRM",
                panelY + PanelHeight - 52, 0.54f, Color.LightGray);
        }

        private void DrawRow(int index, int panelX, int panelY)
        {
            Rectangle row = GetRowBounds(index);
            bool selected = index == _selectedIndex;
            if (selected)
            {
                _spriteBatch.Draw(_selectedBarTexture, row, Color.White);
            }

            if (index < 4)
            {
                DrawLeftText(Options[index], panelX + 78, row.Y + 17, 0.68f,
                    selected ? Color.Cyan : Color.White);
                Rectangle slider = GetSliderBounds(index);
                _spriteBatch.Draw(_pixel, slider, Color.DarkSlateGray);
                int knobTravel = Math.Max(0, slider.Width - slider.Height);
                int fillWidth = (int)Math.Round(knobTravel * GetVolume(index));
                if (fillWidth > 0)
                {
                    var source = new Rectangle(0, 0, fillWidth, _volumeSliderTexture.Height);
                    var destination = new Rectangle(slider.X, slider.Y, fillWidth, slider.Height);
                    _spriteBatch.Draw(_volumeSliderTexture, destination, source, Color.White);
                }

                int handleSize = slider.Height;
                int handleX = slider.X + fillWidth;
                var handleSource = new Rectangle(
                    _volumeSliderTexture.Width - handleSize, 0, handleSize, _volumeSliderTexture.Height);
                var handleDestination = new Rectangle(handleX, slider.Y, handleSize, slider.Height);
                _spriteBatch.Draw(_volumeSliderTexture, handleDestination, handleSource, Color.White);
                DrawLeftText($"{(int)Math.Round(GetVolume(index) * 100f)}%", slider.Right + 24,
                    row.Y + 17, 0.68f, Color.White);
            }
            else if (index == 4)
            {
                DrawLeftText(Options[index], panelX + 78, row.Y + 17, 0.68f,
                    selected ? Color.Cyan : Color.White);
                DrawLeftText(_gameFlow.IsFullScreen ? "ON" : "OFF", panelX + 670, row.Y + 17,
                    0.68f, _gameFlow.IsFullScreen ? Color.LimeGreen : Color.White);
            }
            else
                DrawCenteredText("BACK", row.Y + 15, 0.75f, selected ? Color.Cyan : Color.White);
        }

        private void ChangeSelection(int direction)
        {
            _selectedIndex = (_selectedIndex + direction + Options.Length) % Options.Length;
            _audio.PlaySFX("menuentry");
        }

        private void AdjustSelectedVolume(float delta)
        {
            if (_selectedIndex >= 4)
                return;

            SetVolume(_selectedIndex, GetVolume(_selectedIndex) + delta);
            _audio.PlaySFX("selected");
        }

        private float GetVolume(int index) => index switch
        {
            0 => _audio.MasterVolume,
            1 => _audio.BackgroundMusicVolume,
            2 => _audio.SoundEffectsVolume,
            3 => _audio.DialogueVolume,
            _ => 0f
        };

        private void SetVolume(int index, float value)
        {
            switch (index)
            {
                case 0: _audio.SetMasterVolume(value); break;
                case 1: _audio.SetBackgroundMusicVolume(value); break;
                case 2: _audio.SetSoundEffectsVolume(value); break;
                case 3: _audio.SetDialogueVolume(value); break;
            }
        }

        private Rectangle GetRowBounds(int index)
        {
            int panelX = (_screenWidth - PanelWidth) / 2;
            int panelY = (_screenHeight - PanelHeight) / 2;
            return new Rectangle(panelX + 60, panelY + RowStartY + index * RowSpacing,
                PanelWidth - 120, RowHeight);
        }

        private Rectangle GetSliderBounds(int index)
        {
            int panelX = (_screenWidth - PanelWidth) / 2;
            int panelY = (_screenHeight - PanelHeight) / 2;
            return new Rectangle(panelX + 585, panelY + RowStartY + index * RowSpacing + 22,
                SliderWidth, 18);
        }

        private void DrawCenteredText(string text, float y, float scale, Color color)
        {
            var size = _font.MeasureString(text);
            var position = new Vector2((_screenWidth - size.Width * scale) / 2f, y);
            _spriteBatch.DrawString(_font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void DrawLeftText(string text, float x, float y, float scale, Color color)
        {
            _spriteBatch.DrawString(_font, text, new Vector2(x, y), color, 0f,
                Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private bool WasPressed(KeyboardState current, Keys key)
        {
            return current.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }

        private void UpdatePreviousInput(KeyboardState keyboard, MouseState mouse)
        {
            _previousKeyboardState = keyboard;
            _previousMouseState = mouse;
        }

        private void Close()
        {
            IsOpen = false;
            _audio.PlaySFX("exit");
        }
    }
}
