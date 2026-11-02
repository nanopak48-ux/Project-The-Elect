using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project_The_Elect.source_code
{
    /// <summary>Plays the extracted Good End corridor GIF frames once at their original timing.</summary>
    public sealed class GoodEndingCutscene
    {
        private const int FrameCount = 26;
        private static readonly double[] FrameDurationsSeconds =
        {
            0.30, 0.30, 0.30, 0.30,
            0.50, 0.50, 0.50, 0.50, 0.50, 0.50, 0.50, 0.50,
            0.50, 0.50, 0.50, 0.50, 0.50, 0.50, 0.50, 0.50,
            0.30, 0.30, 0.30,
            0.50, 0.50, 0.50
        };

        private readonly Texture2D[] _frames = new Texture2D[FrameCount];
        private int _frameIndex;
        private int _loopStart;
        private int _loopEnd;
        private double _frameTimer;

        public GoodEndingCutscene(ContentManager content)
        {
            for (int i = 0; i < _frames.Length; i++)
                _frames[i] = content.Load<Texture2D>($"texture/02_cutscene/good_end/frame_{i:00}");
        }

        public void Start()
        {
            SetLoopRange(0, _frames.Length - 1);
        }

        public void SetLoopRange(int firstFrame, int lastFrame)
        {
            _loopStart = System.Math.Clamp(firstFrame, 0, _frames.Length - 1);
            _loopEnd = System.Math.Clamp(lastFrame, _loopStart, _frames.Length - 1);
            _frameIndex = _loopStart;
            _frameTimer = 0;
        }

        public void Update(GameTime gameTime)
        {
            _frameTimer += gameTime.ElapsedGameTime.TotalSeconds;
            while (_frameTimer >= FrameDurationsSeconds[_frameIndex])
            {
                _frameTimer -= FrameDurationsSeconds[_frameIndex];
                _frameIndex = _frameIndex >= _loopEnd ? _loopStart : _frameIndex + 1;
            }
        }

        public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            const int screenMargin = 24;
            const int dialoguePanelHeight = 356;
            int dialoguePanelTop = screenHeight * 5 / 7 - dialoguePanelHeight / 4;
            int availableWidth = screenWidth - screenMargin * 2;
            int availableHeight = dialoguePanelTop - screenMargin * 2;

            float scale = System.Math.Min(
                availableWidth / (float)_frames[_frameIndex].Width,
                availableHeight / (float)_frames[_frameIndex].Height);
            int drawWidth = (int)(_frames[_frameIndex].Width * scale);
            int drawHeight = (int)(_frames[_frameIndex].Height * scale);
            Rectangle destination = new Rectangle(
                (screenWidth - drawWidth) / 2,
                screenMargin + (availableHeight - drawHeight) / 2,
                drawWidth,
                drawHeight);

            spriteBatch.Draw(_frames[_frameIndex], destination, Color.White);
        }
    }
}
