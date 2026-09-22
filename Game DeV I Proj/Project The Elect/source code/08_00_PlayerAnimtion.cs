using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public class PlayerAnimationData
    {
        [JsonPropertyName("animations")]
        public List<AnimationData> Animations { get; set; }
    }

    public class AnimationData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("loop")]
        public bool Loop { get; set; }

        [JsonPropertyName("frameDuration")]
        public float FrameDuration { get; set; }

        [JsonPropertyName("frames")]
        public List<int> Frames { get; set; }
    }
}
