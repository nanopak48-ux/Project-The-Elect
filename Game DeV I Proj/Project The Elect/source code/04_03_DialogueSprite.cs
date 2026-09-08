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
    public class DialogueSprite
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private GameAudioManager _audioManager;

        //----------------------- DIALOGUE PROFILE VARIABLES -----------------------//
        private Texture2D spr_dialogueProfileSheet;
        private Texture2DAtlas spr_dialogueprofile;
        private Sprite spr_profile;
        private int profileWidth = 278;
        private int profileHeight = 320;
        private Vector2 spr_profilePos = new Vector2(108, 560);
        private string[] profileIndex = new string[]
        {
            "Columbina",
            "Furina",
            "Tanjiro",
        };

        //----------------------------------------------------------------------------//
        //----------------------------------------------------------------------------//
        private bool EaseInActive = false;
        private float EaseSpeed = 15f;

        private Texture2D txtr_dialogueBG;
        private int dialogueBGWidth = 1840;
        private int dialogueBGHeight = 356;
        private int screenWidth;
        private int screenHeight;

        public DialogueSprite(ContentManager content, SpriteBatch spriteBatch, GameAudioManager audioManager, int screenWidth, int screenHeight)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _audioManager = audioManager;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            LoadContent();
        }

        public void LoadContent()
        {
            // Load content for dialogue state
            string prefix = "texture/03_dialogue/";
            txtr_dialogueBG = _content.Load<Texture2D>(prefix + "01_dialogueBG");

            spr_dialogueProfileSheet = _content.Load<Texture2D>(prefix + "02_dialogueProfileSheet");

            spr_dialogueprofile = new Texture2DAtlas(spr_dialogueProfileSheet);

            for (int i = 0; i < profileIndex.Length; i++)
            {
                spr_dialogueprofile.CreateRegion(i * profileWidth, 0, profileWidth, profileHeight, profileIndex[i]);
            }
        }
       
        private string previousCharacter = null;

        public void Draw(GameTime gametime, DialogueData dialogue)
        {

            if (spr_profile == null || dialogue.character != spr_profile.TextureRegion.Name || spr_profile.TextureRegion.Name != previousCharacter)
            {

                spr_profile = new Sprite(spr_dialogueprofile.GetRegion(dialogue.character));
                previousCharacter = spr_profile.TextureRegion.Name;
                spr_profilePos = new Vector2(108, 560);
                EaseSpeed = 15f;
                EaseInActive = true;
            }

            _spriteBatch.Draw(txtr_dialogueBG, new Rectangle((screenWidth - dialogueBGWidth) / 2, screenHeight * 5 / 7 - dialogueBGHeight / 4, dialogueBGWidth, dialogueBGHeight), Color.White);
            EaseIn();
            _spriteBatch.Draw(spr_profile, spr_profilePos);
            
        }
        private void EaseIn()
        {
            if (EaseInActive)
            {
                if (spr_profilePos.Y > 460)
                {
                    spr_profilePos.Y -= EaseSpeed;
                    EaseSpeed *= 0.8f;
                }
                else
                {
                    spr_profilePos.Y = 460;
                    EaseInActive = false;
                }
            }
        }
    }
}


