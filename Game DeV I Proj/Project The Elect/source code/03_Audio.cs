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
        private float volume_bgm = 0.2f;
        private float volume_sfx = 0.8f;
        private float volume_vc = 1f;
        private float volume_fadeStep;
        private float volume_fadeDuration = 2f;

        private string[] _sfxIndex = new string[]
        {
            "proceed",
            "selected",
            "menuentry",
            "exit",
            "scan_start",
            "scan_collision",
            "error_task",
            "speacbar",
            "skipdia"
        };
        
        private string[] _bgmIndex = new string[]
        {
            "home",
            "dialogue",
            "bgscary",
            "scan_start"

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

        public void LoadDialogueVoicelines(ChapterData chapter, ContentManager content)
        {
            _Voicelines = new List<SoundEffect>();
            for (int i = 0; i < chapter.dialogues.Count; i++)
            {
                string voiceline = chapter.dialogues[i].voiceline;
                if (string.IsNullOrWhiteSpace(voiceline) || voiceline == "none")
                {
                    _Voicelines.Add(null);
                    continue;
                }

                string voicelinePath = "audio/03_voiceline/dialogue/" + voiceline;
                try
                {
                    _Voicelines.Add(content.Load<SoundEffect>(voicelinePath));
                }
                catch (ContentLoadException)
                {
                    // Missing optional voice assets should not prevent text dialogue from playing.
                    _Voicelines.Add(null);
                }
            }
        }

        public void PlayBGM(int bgmIndex)
        {
            if (bgmIndex >= 0 && bgmIndex < _BGM.Count)
            {
                MediaPlayer.Volume = volume_bgm;
                MediaPlayer.Play(_BGM[bgmIndex]);
                MediaPlayer.IsRepeating = true;
            }
        }

        public void LowBGM(bool decision)
        {
            if (decision) MediaPlayer.Volume = 0.05f;
            else MediaPlayer.Volume = volume_bgm;

        }

        public void PlayVoicelines(int voicelineIndex)
        {
            _currentVoiceline?.Stop();
            if (voicelineIndex < 0 || voicelineIndex >= _Voicelines.Count || _Voicelines[voicelineIndex] == null) return;

            _currentVoiceline = _Voicelines[voicelineIndex].CreateInstance();
            _currentVoiceline.Volume = volume_vc;
            _currentVoiceline.Play();
        }

        public void PlaySFX(string sfxName)
        {
            int index = Array.IndexOf(_sfxIndex, sfxName);
            if (index != -1)
            {
                SoundEffectInstance sfx = _SFX[index].CreateInstance();
                sfx.Volume = volume_sfx;
                sfx.Play();
            }
        }

        public void VolumeControl(float bgmVolume, float sfxVolume)
        {
            volume_bgm += bgmVolume;
            volume_sfx += sfxVolume;
            volume_vc += sfxVolume;
            MediaPlayer.Volume = volume_bgm;
        }
    }
}
