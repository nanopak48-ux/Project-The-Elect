using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using Project_The_Elect.source_code;

namespace Project_The_Elect.source_code
{
    public class GameButtonGuide
    {
        private string[] _buttonGuideIndex = new string[]
        {
            "E",
            "P",
            "ESC",
            "ENTER",
            "RMB",
            "LMB"
        };
        private string[] _buttonGuideTo = new string[]
        {
            "INTERACT",
            "EXIT",
            "MENU",
            "PROCEED",
            "NO",
            "YES"
        };

        private List<string> _dfbtnDialogue = new List<string>();
        private Vector2 _buttonSize = new Vector2(90, 90);

        private GameStateManager _gameStateManager;
        private StateCutscene _stateCutscene;
        private StateDialogue _stateDialogue;
        private ContentManager _contentManager;
        private SpriteBatch _spriteBatch;

        private Texture2D _gameButtonGuides;
        private Texture2DAtlas spr_button;
        private List<string> _buttonDisplayList;
        

        public GameButtonGuide(GameStateManager gameStateManager,ContentManager gameContentmanager,SpriteBatch spriteBatch)
        {
            _gameStateManager = gameStateManager;
            _contentManager = gameContentmanager;
            _spriteBatch = spriteBatch;

            _buttonDisplayList = new List<string>();
            _buttonDisplayTxtList = new List<string>();
            _gameTxtManager = new GameTextManager(gameContentmanager, spriteBatch);
            LoadContent();
        }
        public void LoadContent()
        {
            _gameButtonGuides = _contentManager.Load<Texture2D>("texture/07_buttonguide/01_buttonSpriteSheet");
            spr_button = new Texture2DAtlas(_gameButtonGuides);

            for (int i = 0; i < _buttonGuideIndex.Length; i++) 
            { 
                spr_button.CreateRegion(i * 90, 0, 90, 90, _buttonGuideIndex[i]);
            }
        }

        private bool _addedDefaultBtn = false;
        private GameStateManager _previousState;
        private List<string> _buttonDisplayTxtList;
        public void Update(GameTime gametime, GameStateManager gameStateManager)
        {
            //SET DEFAULT FOR STATE CHANGE
                if (!_addedDefaultBtn)
                {
                        _buttonDisplayList.Add("ENTER");
                        _buttonDisplayList.Add("E");
                    _buttonDisplayTxtList.Add("PROCEED");
                    _buttonDisplayTxtList.Add("INTERACT");
                        _addedDefaultBtn = true;
                }
        }
        private GameTextManager _gameTxtManager; 
        public void DrawBtnGuide(GameTime gametime, GameStateManager gameStateManager)
        {
            if (_buttonDisplayList == null) return;
            for (int i = 0; i < _buttonDisplayList.Count; i++)
            {
                Sprite sprite = new Sprite(spr_button.GetRegion(_buttonDisplayList[i]));
               _spriteBatch.Draw(sprite,new Vector2(1800-(108 * i),936));
            }
            _gameTxtManager.Draw(_buttonDisplayTxtList);
            
        }
    }
}
