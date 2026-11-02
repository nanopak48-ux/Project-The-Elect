using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

namespace Project_The_Elect.source_code
{
    /// <summary>Pause overlay shown over normal gameplay.</summary>
    public class StateMenu : IGameState
    {
        private const int PanelWidth = 900;
        private const int PanelHeight = 610;
        private const int RowWidth = 700;
        private const int RowHeight = 100;
        private const int RowStartY = 145;
        private const int RowSpacing = 140;

        private readonly GameStateManager _gameStateManager;
        private readonly GameFlowManager _gameFlow;
        private readonly SpriteBatch _spriteBatch;
        private readonly GameAudioManager _audio;
        private readonly SettingsMenu _settingsMenu;
        private readonly Texture2D _pixel;
        private readonly BitmapFont _font;
        private readonly string[] _options = { "RESUME", "SETTINGS", "RETURN TO HOME" };
        private KeyboardState _previousKeyboardState;
        private MouseState _previousMouseState;
        private int _selectedIndex;

        public StateMenu(
            ContentManager content,
            GameStateManager gameStateManager,
            GameFlowManager gameFlow,
            SpriteBatch spriteBatch,
            GameAudioManager audioManager)
        {
            _gameStateManager = gameStateManager;
            _gameFlow = gameFlow;
            _spriteBatch = spriteBatch;
            _audio = audioManager;
            _font = content.Load<BitmapFont>("font/fontGenshin");
            _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            _settingsMenu = new SettingsMenu(
                _spriteBatch, _font, _audio, _gameFlow, GameConfig.ScreenWidth, GameConfig.ScreenHeight);
            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();

            _audio.PlaySFX("menuentry");
            _audio.LowBGM(true);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            _gameStateManager.DrawPreviousState(gameTime);
            _spriteBatch.Begin();

            _spriteBatch.Draw(_pixel,
                new Rectangle(0, 0, GameConfig.ScreenWidth, GameConfig.ScreenHeight),
                new Color((byte)0, (byte)0, (byte)0, (byte)155));

            int panelX = (GameConfig.ScreenWidth - PanelWidth) / 2;
            int panelY = (GameConfig.ScreenHeight - PanelHeight) / 2;
            var panel = new Rectangle(panelX, panelY, PanelWidth, PanelHeight);
            _spriteBatch.Draw(_pixel, panel, new Color((byte)5, (byte)18, (byte)25, (byte)245));
            _spriteBatch.Draw(_pixel, new Rectangle(panelX, panelY, PanelWidth, 5), Color.Cyan);
            DrawCenteredText("PAUSED", panelY + 80, 1.45f, Color.Cyan);

            for (int i = 0; i < _options.Length; i++)
                DrawOption(i);

            DrawCenteredText("UP / DOWN: SELECT     SPACE: CONFIRM     ESC: RESUME",
                panelY + PanelHeight - 58, 0.52f, Color.LightGray);

            _settingsMenu.Draw();
            _spriteBatch.End();
        }

        public void InputHandler(GameTime gameTime)
        {
            if (_settingsMenu.IsOpen)
            {
                _settingsMenu.InputHandler();
                _previousKeyboardState = Keyboard.GetState();
                _previousMouseState = Mouse.GetState();
                return;
            }

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            bool mouseMoved = mouse.X != _previousMouseState.X || mouse.Y != _previousMouseState.Y;
            if (WasPressed(keyboard, Keys.Escape))
            {
                ResumeGame();
                return;
            }

            if (WasPressed(keyboard, Keys.Up))
                ChangeSelection(-1);
            else if (WasPressed(keyboard, Keys.Down))
                ChangeSelection(1);

            for (int i = 0; i < _options.Length; i++)
            {
                if (!GetRowBounds(i).Contains(mouse.Position))
                    continue;

                if (mouseMoved && _selectedIndex != i)
                {
                    _selectedIndex = i;
                    _audio.PlaySFX("menuentry");
                }

                bool clicked = mouse.LeftButton == ButtonState.Pressed
                    && _previousMouseState.LeftButton == ButtonState.Released;
                if (clicked)
                    ActivateSelection(i);
                break;
            }

            if (WasPressed(keyboard, Keys.Enter) || WasPressed(keyboard, Keys.Space))
                ActivateSelection(_selectedIndex);

            _previousKeyboardState = keyboard;
            _previousMouseState = mouse;
        }

        public void AudioHandler(GameTime gameTime)
        {
        }

        private void ActivateSelection(int index)
        {
            _audio.PlaySFX("selected");
            switch (index)
            {
                case 0:
                    ResumeGame();
                    break;
                case 1:
                    _settingsMenu.Open();
                    break;
                case 2:
                    _audio.LowBGM(false);
                    _gameFlow.ChangeFlow(GameFlow.Home);
                    break;
            }
        }

        private void ResumeGame()
        {
            _audio.PlaySFX("exit");
            _audio.LowBGM(false);
            _gameStateManager.StateReturn();
        }

        private void ChangeSelection(int direction)
        {
            _selectedIndex = (_selectedIndex + direction + _options.Length) % _options.Length;
            _audio.PlaySFX("menuentry");
        }

        private void DrawOption(int index)
        {
            Rectangle bounds = GetRowBounds(index);
            bool selected = index == _selectedIndex;
            Color fill = selected
                ? new Color((byte)0, (byte)60, (byte)72, (byte)245)
                : new Color((byte)10, (byte)25, (byte)32, (byte)235);
            Color border = selected ? Color.Cyan : Color.DarkSlateGray;

            _spriteBatch.Draw(_pixel, bounds, fill);
            _spriteBatch.Draw(_pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, 4), border);
            _spriteBatch.Draw(_pixel, new Rectangle(bounds.X, bounds.Bottom - 4, bounds.Width, 4), border);
            _spriteBatch.Draw(_pixel, new Rectangle(bounds.X, bounds.Y, 4, bounds.Height), border);
            _spriteBatch.Draw(_pixel, new Rectangle(bounds.Right - 4, bounds.Y, 4, bounds.Height), border);

            string label = (selected ? ">  " : "   ") + _options[index];
            var size = _font.MeasureString(label);
            const float scale = 0.9f;
            var position = new Vector2(
                (GameConfig.ScreenWidth - size.Width * scale) / 2f,
                bounds.Y + (bounds.Height - size.Height * scale) / 2f);
            _spriteBatch.DrawString(_font, label, position, selected ? Color.Cyan : Color.White,
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private Rectangle GetRowBounds(int index)
        {
            int x = (GameConfig.ScreenWidth - RowWidth) / 2;
            int y = (GameConfig.ScreenHeight - PanelHeight) / 2 + 145 + index * RowSpacing;
            return new Rectangle(x, y, RowWidth, RowHeight);
        }

        private void DrawCenteredText(string text, float y, float scale, Color color)
        {
            var size = _font.MeasureString(text);
            var position = new Vector2((GameConfig.ScreenWidth - size.Width * scale) / 2f, y);
            _spriteBatch.DrawString(_font, text, position, color, 0f, Vector2.Zero,
                scale, SpriteEffects.None, 0f);
        }

        private bool WasPressed(KeyboardState current, Keys key)
        {
            return current.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }
    }
}
