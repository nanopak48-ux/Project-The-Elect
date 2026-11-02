using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

namespace Project_The_Elect.source_code
{
    public class StateHome : IGameState
    {
        private const int ButtonWidth = 760;
        private const int ButtonHeight = 88;
        private const int ButtonStartY = 570;
        private const int ButtonSpacing = 100;
        private const float ButtonLeftMarginRatio = 0.03f;

        private readonly SpriteBatch _spriteBatch;
        private readonly GameAudioManager _audio;
        private readonly GameFlowManager _gameFlow;
        private readonly int _screenWidth = GameConfig.ScreenWidth;
        private readonly int _screenHeight = GameConfig.ScreenHeight;
        private readonly SettingsMenu _settingsMenu;
        private readonly string[] _menuOptions = { "PLAY", "SETTINGS", "EXIT" };
        private Texture2D _background;
        private Texture2D _selectionBar;
        private BitmapFont _font;
        private KeyboardState _previousKeyboardState;
        private MouseState _previousMouseState;
        private int _selectedIndex;
        private bool _isPlayingBGM;

        public StateHome(
            GameStateManager stateManager,
            GameFlowManager gameFlow,
            SpriteBatch spriteBatch,
            ContentManager content,
            GameAudioManager audio)
        {
            _gameFlow = gameFlow;
            _spriteBatch = spriteBatch;
            _audio = audio;
            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();

            _background = content.Load<Texture2D>("texture/01_home/01_homeBG");
            _selectionBar = content.Load<Texture2D>("texture/01_home/03_homeSelectBar");
            _font = content.Load<BitmapFont>("font/fontGenshin");
            _settingsMenu = new SettingsMenu(
                content, _spriteBatch, _font, _audio, _gameFlow, _screenWidth, _screenHeight);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_background, new Rectangle(0, 0, _screenWidth, _screenHeight), Color.White);

            for (int i = 0; i < _menuOptions.Length; i++)
                DrawMenuButton(i);

            _settingsMenu.Draw();
            _spriteBatch.End();
        }

        public void InputHandler(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            bool mouseMoved = mouse.X != _previousMouseState.X || mouse.Y != _previousMouseState.Y;

            if (_settingsMenu.IsOpen)
            {
                _settingsMenu.InputHandler();
                _previousKeyboardState = keyboard;
                _previousMouseState = mouse;
                return;
            }

            if (WasPressed(keyboard, Keys.Up))
                ChangeSelection(-1);
            else if (WasPressed(keyboard, Keys.Down))
                ChangeSelection(1);

            for (int i = 0; i < _menuOptions.Length; i++)
            {
                Rectangle bounds = GetButtonBounds(i);
                if (!bounds.Contains(mouse.Position))
                    continue;

                if (mouseMoved && _selectedIndex != i)
                {
                    _selectedIndex = i;
                    _audio.PlaySFX("menuentry");
                }

                bool mouseClicked = mouse.LeftButton == ButtonState.Pressed
                    && _previousMouseState.LeftButton == ButtonState.Released;
                if (mouseClicked)
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
            if (_isPlayingBGM)
                return;

            _audio.PlayBGM(2);
            _isPlayingBGM = true;
        }

        private void ActivateSelection(int index)
        {
            _audio.PlaySFX("selected");
            switch (index)
            {
                case 0:
                    _gameFlow.ChangeFlow(GameFlow.Chapter01);
                    break;
                case 1:
                    _settingsMenu.Open();
                    break;
                case 2:
                    _gameFlow.RequestExit();
                    break;
            }
        }

        private void ChangeSelection(int direction)
        {
            _selectedIndex = (_selectedIndex + direction + _menuOptions.Length) % _menuOptions.Length;
            _audio.PlaySFX("menuentry");
        }

        private void DrawMenuButton(int index)
        {
            Rectangle bounds = GetButtonBounds(index);
            bool selected = index == _selectedIndex;
            if (selected)
                _spriteBatch.Draw(_selectionBar, bounds, Color.White);

            string label = _menuOptions[index];
            var size = _font.MeasureString(label);
            const float scale = 1.7f;
            var position = new Vector2(
                bounds.X + 24,
                bounds.Y + (bounds.Height - size.Height * scale) / 2f);
            Color textColor = selected ? new Color(238, 250, 248) : Color.White;
            _spriteBatch.DrawString(_font, label, position + new Vector2(3, 3), new Color(0, 0, 0, 170),
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(_font, label, position, textColor,
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private Rectangle GetButtonBounds(int index)
        {
            return new Rectangle(
                (int)(_screenWidth * ButtonLeftMarginRatio),
                ButtonStartY + index * ButtonSpacing,
                ButtonWidth,
                ButtonHeight);
        }

        private bool WasPressed(KeyboardState current, Keys key)
        {
            return current.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }
    }
}
