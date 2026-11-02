using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public class GameTextManager
    {
        public SpriteFont MainFont { get; private set; }

        private SpriteBatch _spriteBatch;

        private Vector2 _profilePos;

        private Vector2 _dialoguePos;
        private float _profileScale = 2.0f;

        private string[] _dialogueElements = Array.Empty<string>();
        private int _dialogueCharIndex = 0;

        private readonly StringBuilder _dialoguedisplay = new StringBuilder();

        private bool _isPlayingTextAnim = false;

        private DialogueData _dialogueData;

        private SpriteFont DialogueFont;
        private BitmapFont ProfileFont;
        private readonly ThaiTextRenderer _thaiDialogueRenderer;

        public GameTextManager(ContentManager content, SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            DialogueFont = content.Load<SpriteFont>("font/NotoSerifThaiDialogue");
            ProfileFont = content.Load<BitmapFont>("font/fontGenshin");
            string thaiFontPath = System.IO.Path.Combine(
                AppContext.BaseDirectory,
                "Content", "font", "NotoSerifThai-SemiBold.ttf");
            _thaiDialogueRenderer = ThaiTextRenderer.GetShared(_spriteBatch.GraphicsDevice, thaiFontPath);

            _dialoguePos = new Vector2(270, 820);
        }

        public void Update(GameTime gameTime, DialogueData current, bool isNextDialogue)
        {
            if (isNextDialogue)
            {
                _dialoguePos = new Vector2(230, 820);
                _dialoguedisplay.Clear();
                _dialogueCharIndex = 0;
                _dialogueElements = ThaiTextRenderer.SplitTextElements(current.text);
                _thaiDialogueRenderer.SetText(string.Empty);
                _isPlayingTextAnim = true;
                string profileName = current.character.ToUpper();
                float profileNameWidth = ProfileFont.MeasureString(profileName).Width * _profileScale;
                const float profileImageX = 150f;
                const float profileImageWidth = 278f;
                _profilePos = new Vector2(
                    profileImageX + (profileImageWidth - profileNameWidth) / 2f,
                    730f);
            }

            if (_isPlayingTextAnim)
            {
                if (_dialogueCharIndex < _dialogueElements.Length)
                {
                    _dialoguedisplay.Append(_dialogueElements[_dialogueCharIndex]);
                    _dialogueCharIndex++;
                    _thaiDialogueRenderer.SetText(_dialoguedisplay.ToString());
                }
                else
                {
                    _isPlayingTextAnim = false;
                }
            }
        }

        public void DrawDialogue(GameTime gametime, DialogueData current)
        {
            //DRAW TEXT 
            _thaiDialogueRenderer.Draw(_spriteBatch, _dialoguePos);
            _spriteBatch.DrawString(
                ProfileFont,
                current.character.ToUpperInvariant(),
                _profilePos,
                Color.White,
                0f,
                Vector2.Zero,
                _profileScale,
                SpriteEffects.None,
                0f);

        }

        public void Draw(List<string> TextList, int firstTextOffsetX = 0)
        {
            for (int i = 0; i < TextList.Count; i++)
            {
                int firstItemOffset = i == 0 ? firstTextOffsetX : 0;
                _spriteBatch.DrawString(
                    DialogueFont,
                    TextList[i],
                    new Vector2(1818 - (i * 108) - firstItemOffset, 1038),
                    Color.White);
            }
        }
    }
}
