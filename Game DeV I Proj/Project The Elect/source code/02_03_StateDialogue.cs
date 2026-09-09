using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using Project_The_Elect;
using Project_The_Elect.source_code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect
{
    public class StateDialogue : IGameState
    {
        private SpriteBatch _spriteBatch;
        private DialogueManager _dialogueManager;
        private DialogueSprite _dialoguesprite;

        private int _currentDialogueIndex = 0;
        private int _previousDialogueIndex = 0;

        private bool _isNextDialogue = true;
        private bool _isPlayingBGM;

        private ChapterData chapter;
        private DialogueData current;
        private GameTextManager _fontManager;
        private GameAudioManager _audioManager;
        private GameStateManager _gameStateManager;

        private GameButtonGuide _gameButtonGuide;
        private int screenWidth;
        private int screenHeight;

        private KeyboardState _previousKeyboardState;
        private MouseState _previousMouseState;

        private bool _isPlayedVoiceline = false;
        private ContentManager contentManager;

        public StateDialogue(ContentManager _contentManager, GameStateManager gameStateManager, SpriteBatch spriteBatch, GameAudioManager audioManager,int chapterIndex, int screenWidth, int screenHeight)
        {
            _dialogueManager = new DialogueManager(_contentManager);
            _dialoguesprite = new DialogueSprite(_contentManager, spriteBatch, audioManager, screenWidth, screenHeight);
            _gameButtonGuide = new GameButtonGuide(gameStateManager, _contentManager,spriteBatch);
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            _gameStateManager = gameStateManager;
            _fontManager = new GameTextManager(_contentManager, spriteBatch);
            _spriteBatch = spriteBatch;
            _audioManager = audioManager;
            _gameStateManager = gameStateManager;


            if (chapterIndex < 10) chapter = _dialogueManager.LoadChapter("Content/dialoguedata/chapter0"+ chapterIndex +".json");
            else chapter = _dialogueManager.LoadChapter("Content/dialoguedata/chapter" + chapterIndex + ".json");

            if (chapter?.dialogues != null && chapter.dialogues.Count > 0)
            {
                _dialogueManager._dialogues = chapter.dialogues;
            }
            current = _dialogueManager.GetDialogue(_currentDialogueIndex);
            _audioManager.LoadDialogueVoicelines(chapter, _contentManager);

            _isPlayingBGM = false;
            contentManager = _contentManager;
        }    

        public void Update(GameTime gameTime)
        {
            _gameButtonGuide.Update(gameTime, _gameStateManager);

            current = _dialogueManager.GetDialogue(_currentDialogueIndex);
            _fontManager.Update(gameTime, current, _isNextDialogue);
            if(_isNextDialogue) {_isNextDialogue = false;}
            InputHandler(gameTime);
            AudioHandler(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            //DRAW PROFILE & BG
            _dialoguesprite.Draw(gameTime, current);

            //DRAW TEXT
            _fontManager.DrawDialogue(gameTime, current);

            //DRAW BUTTONS
            _gameButtonGuide.DrawBtnGuide(gameTime, _gameStateManager);

            _spriteBatch.End();

            
        }

        public void InputHandler(GameTime gametime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            bool spacePressed = currentKeyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space);
            bool keyEscapePressed = currentKeyboardState.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape);
            bool mouseClicked = currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
            bool keyDownPresed = currentKeyboardState.IsKeyDown(Keys.Down) && _previousKeyboardState.IsKeyUp(Keys.Down);
            bool keyUpPressed = currentKeyboardState.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyUp(Keys.Up);

            if (spacePressed || mouseClicked)
            {
                if (_currentDialogueIndex < _dialogueManager._dialogues.Count - 1)
                {
                    _currentDialogueIndex++;
                    _isNextDialogue = true;
                    _isPlayedVoiceline = false;
                    _audioManager.PlaySFX("proceed");
                }
            }

            if (keyEscapePressed)
            {
                _gameStateManager.StatePush(new StateMenu(contentManager, _gameStateManager, _spriteBatch, _audioManager, screenWidth, screenHeight));
            }

            if(keyUpPressed)
            {
                _audioManager.VolumeControl(10f, 0);
            }

            if(keyDownPresed)
            {
                _audioManager.VolumeControl(-10f, 0);
            }


            _previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;

        }
        public void AudioHandler(GameTime gameTime)
        {
            if (!_isPlayingBGM)
            {
                _audioManager.PlayBGM(1);
                _isPlayingBGM = true;
            }

            if (_isPlayedVoiceline == false)
            {
                _audioManager.PlayVoicelines(_currentDialogueIndex);
                _isPlayedVoiceline = true;
            }
        }

    }
}



