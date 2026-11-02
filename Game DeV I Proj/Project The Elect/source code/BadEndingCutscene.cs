using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project_The_Elect.source_code
{
    /// <summary>Loops the four extracted frames of the Bad End injection scene.</summary>
    public sealed class BadEndingCutscene
    {
        private const int FrameCount = 4;
        private const double FrameDurationSeconds = 0.3;

        private readonly Texture2D[] _frames = new Texture2D[FrameCount];
        private int _frameIndex;
        private double _frameTimer;

        public BadEndingCutscene(ContentManager content)
        {
            for (int i = 0; i < _frames.Length; i++)
                _frames[i] = content.Load<Texture2D>($"texture/02_cutscene/bad_end/frame_{i:00}");
        }

        public void Start()
        {
            _frameIndex = 0;
            _frameTimer = 0;
        }

        public void Update(GameTime gameTime)
        {
            _frameTimer += gameTime.ElapsedGameTime.TotalSeconds;
            while (_frameTimer >= FrameDurationSeconds)
            {
                _frameTimer -= FrameDurationSeconds;
                _frameIndex = (_frameIndex + 1) % _frames.Length;
            }
        }

        public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            const int ScreenMargin = 24;
            const int DialoguePanelHeight = 356;
            int dialoguePanelTop = screenHeight * 5 / 7 - DialoguePanelHeight / 4;
            int availableWidth = screenWidth - ScreenMargin * 2;
            int availableHeight = dialoguePanelTop - ScreenMargin * 2;

            float scale = System.Math.Min(
                availableWidth / (float)_frames[_frameIndex].Width,
                availableHeight / (float)_frames[_frameIndex].Height);
            int drawWidth = (int)(_frames[_frameIndex].Width * scale);
            int drawHeight = (int)(_frames[_frameIndex].Height * scale);
            Rectangle destination = new Rectangle(
                (screenWidth - drawWidth) / 2,
                ScreenMargin + (availableHeight - drawHeight) / 2,
                drawWidth,
                drawHeight);

            spriteBatch.Draw(
                _frames[_frameIndex],
                destination,
                Color.White);
        }
    }
}
