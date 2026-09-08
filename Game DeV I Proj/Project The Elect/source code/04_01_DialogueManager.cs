using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Text.Json;
using System.IO;

namespace Project_The_Elect.source_code
{
    public class DialogueManager
    {
        public List<DialogueData> _dialogues;
        private ContentManager _content;

        public DialogueManager(ContentManager content)
        {
            _content = content;
            _dialogues = new List<DialogueData>();
        }

        public ChapterData LoadChapter(string path)
        {
            string json = File.ReadAllText(path);

            ChapterData chapter = JsonSerializer.Deserialize<ChapterData>(json);
            _dialogues = chapter.dialogues;

            return chapter;
        }

        public DialogueData GetDialogue(int index)
        {
            if (_dialogues != null && index >= 0 && index < _dialogues.Count)
            {
                return _dialogues[index];
            }

            return null;
        }
    }
}
