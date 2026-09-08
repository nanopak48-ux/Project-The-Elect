using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using System.Reflection.Metadata;


namespace Project_The_Elect.source_code
{
    public class GameAudioManager
    {
        private float volume_bgm = 30f;
        private float volume_sfx = 100f;
        private float volume_fadeStep;
        private float volume_fadeDuration = 2f;

        private string[] _sfxIndex = new string[]
        {
            "proceed",
            "selected"
        };
        
        private string[] _bgmIndex = new string[]
        {
            "home",
            "dialogue"
        };

        private List<SoundEffect> _SFX;
        private SoundEffectInstance _currentVoiceline;

        private List<SoundEffect> _Voicelines;  
        private List<Song> _BGM;

        private string prefix;

        private bool _isvoicelinePlaying = false;

        public void LoadContent(ContentManager content)
        {
            _SFX = new List<SoundEffect>();
            _BGM = new List<Song>();

            for(int i = 0; i < _bgmIndex.Length; i++)
            {
                if (i < 10) prefix = "audio/01_bgm/bgm_0";
                else prefix = "audio/01_bgm/bgm_";

                string bgmPath = prefix + i.ToString() + "_" + _bgmIndex[i];
                Song bgm = content.Load<Song>(bgmPath);
                _BGM.Add(bgm);
            }

            for (int i = 0; i < _sfxIndex.Length; i++)
            {
                if(i < 10) prefix = "audio/02_sfx/sfx_0";
                else prefix = "audio/02_sfx/sfx_";

                string sfxPath = prefix + i.ToString() + "_" + _sfxIndex[i];
                SoundEffect sfx = content.Load<SoundEffect>(sfxPath);
                _SFX.Add(sfx);
            }

        }

        public void LoadDialogueVoicelines(int dialoguelineIndex, ContentManager content)
        {
            prefix = "audio/03_voiceline/dialogue/chap01_v0";
            _Voicelines = new List<SoundEffect>();
            for (int i = 1; i < dialoguelineIndex+1; i++)
            {
                string voicelinePath = prefix + i.ToString();

                SoundEffect voiceline = content.Load<SoundEffect>(voicelinePath);
                _Voicelines.Add(voiceline);

            }
        }
        public void PlayBGM(int bgmIndex)
        {
            if (bgmIndex >= 0 && bgmIndex < _BGM.Count)
            {
                MediaPlayer.Play(_BGM[bgmIndex]);
                MediaPlayer.IsRepeating = true;
            }
        }


        public void PlayVoicelines(int voicelineIndex)
        {
            _currentVoiceline?.Stop();

            _currentVoiceline = _Voicelines[voicelineIndex].CreateInstance();

            _currentVoiceline.Play();
        }

        public void PlaySFX(int sfxIndex)
        {
            _SFX[sfxIndex].Play();
        }

        public void VolumeControl(float bgmVolume, float sfxVolume)
        {
            volume_bgm = bgmVolume;
            volume_sfx = sfxVolume;
            MediaPlayer.Volume = volume_bgm;
        }
    }
}
