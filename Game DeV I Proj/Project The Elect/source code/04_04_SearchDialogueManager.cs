using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Text.Json;

namespace Project_The_Elect.source_code
{
    /// <summary>
    /// Displays search dialogue without a character portrait, with optional speaker names.
    /// The owning game state starts this when the player interacts with a search point.
    /// </summary>
    public class SearchDialogueManager
    {
        private readonly ContentManager _content;
        private readonly SpriteBatch _spriteBatch;
        private readonly GameAudioManager _audio;
        private readonly BitmapFont _dialogueFont;
        private readonly Texture2D _dialogueBackground;
        private readonly int _screenWidth;
        private readonly int _screenHeight;

        private ChapterData _searchData;
        private int _dialogueIndex;
        private int _visibleCharacterCount;
        private double _characterTimer;
        private KeyboardState _previousKeyboardState;

        private const double CharacterIntervalSeconds = 0.02;
        private const float DialogueScale = 1.2f;
        private const float CharacterScale = 2.0f;
        private static readonly Vector2 DialoguePosition = new Vector2(230f, 820f);
        private static readonly Vector2 CharacterPosition = new Vector2(230f, 730f);

        public bool IsActive { get; private set; }

        /// <summary>Raised after the last search dialogue line is dismissed.</summary>
        public event Action DialogueEnded;

        public SearchDialogueManager(
            ContentManager content,
            SpriteBatch spriteBatch,
            GameAudioManager audio,
            int screenWidth,
            int screenHeight)
        {
            _content = content;
            _spriteBatch = spriteBatch;
            _audio = audio;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;

            _dialogueBackground = _content.Load<Texture2D>("texture/03_dialogue/01_dialogueBG3");
            _dialogueFont = _content.Load<BitmapFont>("font/fontGenshin");
        }

        /// <summary>Loads a search JSON file and starts displaying its dialogue.</summary>
        public void Start(string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);
            _searchData = JsonSerializer.Deserialize<ChapterData>(json);

            if (_searchData?.dialogues == null || _searchData.dialogues.Count == 0)
                throw new InvalidDataException($"Search dialogue file has no dialogue lines: {jsonPath}");

            _dialogueIndex = 0;
            _visibleCharacterCount = 0;
            _characterTimer = 0;
            _previousKeyboardState = Keyboard.GetState();
            IsActive = true;

            _audio.LoadDialogueVoicelines(_searchData, _content);
            PlayCurrentVoiceline();
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            DialogueData current = CurrentDialogue;
            if (current == null)
                return;

            if (_visibleCharacterCount >= current.text.Length)
                return;

            _characterTimer += gameTime.ElapsedGameTime.TotalSeconds;
            while (_characterTimer >= CharacterIntervalSeconds && _visibleCharacterCount < current.text.Length)
            {
                _visibleCharacterCount++;
                _characterTimer -= CharacterIntervalSeconds;
            }
        }

        /// <summary>Call from the owning game state while this dialogue is active.</summary>
        public void InputHandler(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            bool spacePressed = keyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space);

            if (IsActive && spacePressed)
            {
                _audio.PlaySFX("skipdia");
                DialogueData current = CurrentDialogue;
                if (current != null && _visibleCharacterCount < current.text.Length)
                {
                    _visibleCharacterCount = current.text.Length;
                    _characterTimer = 0;
                }
                else if (_dialogueIndex < _searchData.dialogues.Count - 1)
                {
                    _dialogueIndex++;
                    _visibleCharacterCount = 0;
                    _characterTimer = 0;
                    PlayCurrentVoiceline();
                }
                else
                {
                    IsActive = false;
                    DialogueEnded?.Invoke();
                }
            }

            _previousKeyboardState = keyboardState;
        }

        /// <summary>Call inside an active SpriteBatch Begin/End pair.</summary>
        public void Draw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            Rectangle backgroundBounds = new Rectangle(
                (_screenWidth - 1840) / 2,
                _screenHeight * 5 / 7 - 356 / 4,
                1840,
                356);

            _spriteBatch.Draw(_dialogueBackground, backgroundBounds, Color.White);

            DialogueData current = CurrentDialogue;
            if (current == null)
                return;

            if (!string.IsNullOrWhiteSpace(current.character))
            {
                _spriteBatch.DrawString(
                    _dialogueFont,
                    current.character.ToUpperInvariant(),
                    CharacterPosition,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    CharacterScale,
                    SpriteEffects.None,
                    0f);
            }

            string visibleText = current.text.Substring(0, _visibleCharacterCount);
            _spriteBatch.DrawString(
                _dialogueFont,
                visibleText,
                DialoguePosition,
                Color.White,
                0f,
                Vector2.Zero,
                DialogueScale,
                SpriteEffects.None,
                0f);
        }

        private DialogueData CurrentDialogue =>
            _searchData?.dialogues != null &&
            _dialogueIndex >= 0 &&
            _dialogueIndex < _searchData.dialogues.Count
                ? _searchData.dialogues[_dialogueIndex]
                : null;

        private void PlayCurrentVoiceline()
        {
            _audio.PlayVoicelines(_dialogueIndex);
        }
    }
}
