using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;

namespace Project_The_Elect.source_code
{
    public enum EndingChoiceOption
    {
        TrustAndSleep,
        PretendAndEscape
    }

    /// <summary>Draws and handles the final choice without adding UI code to StatePlay.</summary>
    public sealed class EndingChoiceManager
    {
        private const int PanelWidth = 1320;
        private const int PanelHeight = 570;
        private const int OptionWidth = 1080;
        private const int OptionHeight = 92;
        private const int OptionGap = 24;

        private readonly SpriteBatch _spriteBatch;
        private readonly BitmapFont _font;
        private readonly Texture2D _pixel;
        private readonly Texture2D _panelTexture;
        private readonly Texture2D _optionTexture;
        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private KeyboardState _previousKeyboardState;
        private int _selectedIndex;

        public bool IsActive { get; private set; }
        public event Action<EndingChoiceOption> ChoiceSelected;

        public EndingChoiceManager(ContentManager content, SpriteBatch spriteBatch, BitmapFont font, int screenWidth, int screenHeight)
        {
            _spriteBatch = spriteBatch;
            _font = font;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _panelTexture = content.Load<Texture2D>("texture/03_dialogue/03_final_panel");
            _optionTexture = content.Load<Texture2D>("texture/03_dialogue/04_trust_sleep");
            _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Start()
        {
            _selectedIndex = 0;
            _previousKeyboardState = Keyboard.GetState();
            IsActive = true;
        }

        public void InputHandler()
        {
            if (!IsActive)
                return;

            KeyboardState current = Keyboard.GetState();
            bool upPressed = current.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyUp(Keys.Up);
            bool downPressed = current.IsKeyDown(Keys.Down) && _previousKeyboardState.IsKeyUp(Keys.Down);
            bool confirmPressed = current.IsKeyDown(Keys.Space)
                && _previousKeyboardState.IsKeyUp(Keys.Space);

            if (upPressed || downPressed)
                _selectedIndex = 1 - _selectedIndex;

            _previousKeyboardState = current;

            if (!confirmPressed)
                return;

            IsActive = false;
            ChoiceSelected?.Invoke(_selectedIndex == 0
                ? EndingChoiceOption.TrustAndSleep
                : EndingChoiceOption.PretendAndEscape);
        }

        public void Draw()
        {
            if (!IsActive)
                return;

            int panelX = (_screenWidth - PanelWidth) / 2;
            int panelY = (_screenHeight - PanelHeight) / 2;
            var panel = new Rectangle(panelX, panelY, PanelWidth, PanelHeight);
            _spriteBatch.Draw(
                _pixel,
                new Rectangle(0, 0, _screenWidth, _screenHeight),
                new Color((byte)0, (byte)0, (byte)0, (byte)75));
            _spriteBatch.Draw(_panelTexture, panel, Color.White);

            DrawCentered("FINAL DECISION", panelY + 44, 1.35f, Color.White);
            DrawCentered("What will you do?", panelY + 125, 0.8f, Color.White);

            DrawOption(0, panelY + 220, "Trust them and go to sleep");
            DrawOption(1, panelY + 220 + OptionHeight + OptionGap, "Pretend to sleep and escape");

            DrawCentered("Use UP / DOWN to choose, then press SPACE", panelY + PanelHeight - 95, 0.58f, Color.LightGray);
        }

        private void DrawOption(int index, int y, string label)
        {
            int x = (_screenWidth - OptionWidth) / 2;
            var bounds = new Rectangle(x, y, OptionWidth, OptionHeight);
            _spriteBatch.Draw(_optionTexture, bounds, Color.White);
            if (index == _selectedIndex)
            {
                const int selectionBorder = 4;
                _spriteBatch.Draw(_pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, selectionBorder), Color.White);
                _spriteBatch.Draw(_pixel, new Rectangle(bounds.X, bounds.Bottom - selectionBorder, bounds.Width, selectionBorder), Color.White);
                _spriteBatch.Draw(_pixel, new Rectangle(bounds.X, bounds.Y, selectionBorder, bounds.Height), Color.White);
                _spriteBatch.Draw(_pixel, new Rectangle(bounds.Right - selectionBorder, bounds.Y, selectionBorder, bounds.Height), Color.White);
            }

            string text = (index == _selectedIndex ? ">  " : "   ") + label;
            var size = _font.MeasureString(text);
            float scale = 0.85f;
            var position = new Vector2(
                (_screenWidth - size.Width * scale) / 2f,
                bounds.Y + (bounds.Height - size.Height * scale) / 2f);
            _spriteBatch.DrawString(_font, text, position, Color.White,
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void DrawCentered(string text, float y, float scale, Color color)
        {
            var size = _font.MeasureString(text);
            var position = new Vector2((_screenWidth - size.Width * scale) / 2f, y);
            _spriteBatch.DrawString(_font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
