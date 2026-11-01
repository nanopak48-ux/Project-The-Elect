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
        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private KeyboardState _previousKeyboardState;
        private int _selectedIndex;

        public bool IsActive { get; private set; }
        public event Action<EndingChoiceOption> ChoiceSelected;

        public EndingChoiceManager(SpriteBatch spriteBatch, BitmapFont font, int screenWidth, int screenHeight)
        {
            _spriteBatch = spriteBatch;
            _font = font;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
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
            _spriteBatch.Draw(_pixel, panel, new Color((byte)5, (byte)18, (byte)25, (byte)175));
            _spriteBatch.Draw(_pixel, new Rectangle(panelX, panelY, PanelWidth, 5), Color.Cyan);

            DrawCentered("FINAL DECISION", panelY + 44, 1.35f, Color.Cyan);
            DrawCentered("What will you do?", panelY + 125, 0.8f, Color.White);

            DrawOption(0, panelY + 220, "Trust them and go to sleep");
            DrawOption(1, panelY + 220 + OptionHeight + OptionGap, "Pretend to sleep and escape");

            DrawCentered("Use UP / DOWN to choose, then press SPACE", panelY + PanelHeight - 54, 0.58f, Color.LightGray);
        }

        private void DrawOption(int index, int y, string label)
        {
            int x = (_screenWidth - OptionWidth) / 2;
            var bounds = new Rectangle(x, y, OptionWidth, OptionHeight);
            Color fill = index == _selectedIndex
                ? new Color((byte)0, (byte)80, (byte)92, (byte)195)
                : new Color((byte)15, (byte)32, (byte)40, (byte)185);
            Color border = index == _selectedIndex ? Color.Cyan : Color.DarkSlateGray;

            _spriteBatch.Draw(_pixel, bounds, fill);
            _spriteBatch.Draw(_pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, 3), border);
            _spriteBatch.Draw(_pixel, new Rectangle(bounds.X, bounds.Bottom - 3, bounds.Width, 3), border);
            _spriteBatch.Draw(_pixel, new Rectangle(bounds.X, bounds.Y, 3, bounds.Height), border);
            _spriteBatch.Draw(_pixel, new Rectangle(bounds.Right - 3, bounds.Y, 3, bounds.Height), border);

            string text = (index == _selectedIndex ? ">  " : "   ") + label;
            var size = _font.MeasureString(text);
            float scale = 0.85f;
            var position = new Vector2(
                (_screenWidth - size.Width * scale) / 2f,
                bounds.Y + (bounds.Height - size.Height * scale) / 2f);
            _spriteBatch.DrawString(_font, text, position, index == _selectedIndex ? Color.Cyan : Color.White,
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
