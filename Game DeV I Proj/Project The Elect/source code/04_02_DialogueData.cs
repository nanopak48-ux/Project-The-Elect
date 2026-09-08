using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public class DialogueData
    {
        public string character { get; set; }
        public string text { get; set; }
        public string voiceline { get; set; }
    }

    public class ChapterData
    {
        public int chapter { get; set; }
        public List<DialogueData> dialogues { get; set; }
    }

}
