using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_The_Elect.source_code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect
{
    public enum GameFlow
    {
        Home,
        Cutscene01,
        Chapter01,
        Tutorial,
        Chapter02,
        Play,
        Chapter03,
        Play02,
        Chapter04,
        Chapter05,
        Play03,
        Cutscene02
    }

    public class GameFlowManager
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public GameAudioManager _audio;
        private GameStateManager _gameStateManager;
        private ContentManager _content;
        public GameFlow CurrentFlow { get; private set; }

        public GameFlowManager(
            GameStateManager _stateManager,
            SpriteBatch _spritebatch,
            ContentManager _content,
            GameAudioManager _audio)
        {
            _gameStateManager = _stateManager;
            _spriteBatch = _spritebatch;
            this._content = _content;
            this._audio = _audio;

            CurrentFlow = GameFlow.Home;
        }

        public void DialogueEnd()
        { 
            ChangeFlow((GameFlow)((int)GameFlow.Chapter01 + 1));
        }

        public void ChangeFlow(GameFlow nextFlow)
        {
            CurrentFlow = nextFlow;

            switch (nextFlow)
            {
                case GameFlow.Home:
                    StartHomeState();
                    break;
                case GameFlow.Cutscene01:
                    StartCutscene("chapter01_cutscene01");
                    break;

                case GameFlow.Chapter01:
                    StartDialogue(1);
                    break;
                case GameFlow.Play:
                    StartPlayState();
                    break;
            }
        }

        private void StartCutscene(string dialogueFile)
        {
            
        }
        private void StartHomeState()
        {
            _gameStateManager.StateSetTo(new StateHome(_gameStateManager,this,_spriteBatch,_content,_audio));
        }

        private void StartPlayState()
        {
            _gameStateManager.StateSetTo(new StatePlay(_gameStateManager, this, _spriteBatch, _content, _audio));
        }

        private void StartDialogue(int _chapterIndex)
        {
            _gameStateManager.StateSetTo(new StateDialogue(_chapterIndex,_content, _gameStateManager, _spriteBatch, _audio, 1, GameConfig.ScreenWidth, GameConfig.ScreenHeight));
        }

    }
}