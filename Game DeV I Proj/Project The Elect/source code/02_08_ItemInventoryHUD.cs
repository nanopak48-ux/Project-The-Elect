using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project_The_Elect.source_code
{
    /// <summary>Draws the currently held quest items below the Day label.</summary>
    public sealed class ItemInventoryHUD
    {
        private const float IconBoxSize = 72f;
        private const float IconGap = 12f;
        private const float Top = 104f;

        private readonly Texture2D _flashDriveIcon;
        private readonly Texture2D _scissorsIcon;

        public ItemInventoryHUD(ContentManager content)
        {
            _flashDriveIcon = content.Load<Texture2D>("texture/04_play/03_flashdrive");
            _scissorsIcon = content.Load<Texture2D>("texture/04_play/04_tool");
        }

        public void Draw(SpriteBatch spriteBatch, float centerX, bool showFlashDrive, bool showScissors)
        {
            float x = centerX - IconBoxSize / 2f;
            float y = Top;

            if (showFlashDrive)
            {
                DrawFittedIcon(spriteBatch, _flashDriveIcon, x, y);
                y += IconBoxSize + IconGap;
            }

            if (showScissors)
                DrawFittedIcon(spriteBatch, _scissorsIcon, x, y);
        }

        private static void DrawFittedIcon(SpriteBatch spriteBatch, Texture2D texture, float boxX, float boxY)
        {
            float scale = System.Math.Min(IconBoxSize / texture.Width, IconBoxSize / texture.Height);
            int width = (int)(texture.Width * scale);
            int height = (int)(texture.Height * scale);
            var destination = new Rectangle(
                (int)(boxX + (IconBoxSize - width) / 2f),
                (int)(boxY + (IconBoxSize - height) / 2f),
                width,
                height);

            spriteBatch.Draw(texture, destination, Color.White);
        }
    }
}
