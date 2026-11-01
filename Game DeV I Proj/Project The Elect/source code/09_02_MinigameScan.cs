using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using System;

namespace Project_The_Elect.source_code
{
    /// <summary>
    /// มินิเกมจับจังหวะ: กด Space เมื่อตัวชี้สีแดงซ้อนกับเป้า
    /// โดน = ความคืบหน้าเพิ่ม, พลาด = ลด, ครบ 100 = จบ
    /// </summary>
    public class MinigameScan : Minigame
    {
        //====================== CONFIG: ขนาดและความเร็ว ======================//
        private const int BarWidth = 1200;          // ความยาวรางแนวนอน
        private const int BarHeight = 240; // ความสูงราง
        private const int TargetHeight = 170; // ความสูงภาพเป้าตามขนาดต้นฉบับ
        private const int MinTargetWidth = 70;       // เป้าเล็กสุด (ยังมองเห็นชัด)
        private const int MaxTargetWidth = 140;      // เป้าใหญ่สุด (ไม่กินพื้นที่รางมากเกินไป)

        private const int PointerWidth = 30;        // ตัวชี้สีแดง
        private const int PointerHeight = 136;

        private const float PointerIncrement = 15f; // ความเร็วตัวชี้ (พิกเซลต่อเฟรม)

        private const int BarHitboxPadding = 30;     // ยิ่งมาก เป้ายิ่งกดโดนง่าย
        private const int PointerHitboxPadding = 15; // ยิ่งมาก ตัวชี้ยิ่งกดโดนง่าย

        private const float StartingProgress = 21f;               // หลอดเริ่มต้น ให้มีโอกาสพลาดก่อนแพ้
        private const float BaseProgressPoint = 20f;              // โดน +20
        private const float MissPenalty = BaseProgressPoint * 1f; // พลาด -4
        //=====================================================================//

        //====================== CONFIG: ตำแหน่งบนหน้าจอ ======================//
        // รางแนวนอน: กึ่งกลางจอตามแนวนอน, อยู่ที่ 1/4 ของความสูงจอ
        private static int TrackX => (GameConfig.ScreenWidth - BarWidth) / 2;
        private static int TrackY => GameConfig.ScreenHeight / 2 + 250;
        private static int TargetY => TrackY + (BarHeight - TargetHeight) / 2 + 10;

        // ตัวชี้อยู่กึ่งกลางแนวตั้งของราง (ถ้าอยากชิดขอบบนแบบเดิม ให้เปลี่ยนเป็น => TrackY)
        private static int PointerY => TrackY + (BarHeight - PointerHeight) / 2 +10;
        private const int BackdropMargin = 27;
        //=====================================================================//

        public bool ShowDebugHitboxes = false;

        private readonly SpriteBatch _spriteBatch;
        private readonly ContentManager _content;
        private readonly GameAudioManager _audio;
        private BitmapFont _font;

        private Texture2D _barTexture;
        private Texture2D _pointerTexture;
        private Texture2D _targetTexture;

        private float _pointerX;
        private float _targetX;
        private int _targetWidth;
        private float _pointerSpeed;
        private readonly Random _random = new Random();

        private float _progress;
        private KeyboardState _previousKeyboardState;


        // camera ยังรับเข้ามาเพื่อให้ตัวที่เรียก (StatePlay) ไม่ต้องแก้ แต่มินิเกมนี้ไม่ได้ใช้
        public MinigameScan(
            SpriteBatch spriteBatch,
            OrthographicCamera camera,
            ContentManager content,
            GameAudioManager audio)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _audio = audio;
        }

        // ---------- Hitbox / สี่เหลี่ยมที่ใช้ซ้ำ ----------

        private Rectangle TrackRect => new(TrackX, TrackY, BarWidth, BarHeight);
        private Rectangle TargetRect => new((int)_targetX, TargetY, _targetWidth, TargetHeight);
        private Rectangle PointerRect => new((int)_pointerX, PointerY, PointerWidth, PointerHeight);

        private Rectangle TargetHitbox => new(
            (int)_targetX - BarHitboxPadding,
            TargetY,
            _targetWidth + BarHitboxPadding * 2,
            TargetHeight);

        private Rectangle PointerHitbox => new(
            (int)_pointerX - PointerHitboxPadding,
            PointerY - PointerHitboxPadding,
            PointerWidth + PointerHitboxPadding * 2,
            PointerHeight + PointerHitboxPadding * 2);

        // ---------- วงจรชีวิตของมินิเกม ----------

        public override void Initialize()
        {
            ResetResult();

            _progress = StartingProgress;
            _pointerX = TrackX;
            SpawnTarget(false);
            _pointerSpeed = PointerIncrement;
        }

        public override void LoadContent()
        {
            const string texturePrefix = "texture/09_minigame/04_lens/";
            _font = _content.Load<BitmapFont>("font/fontGenshin");
            _barTexture = _content.Load<Texture2D>(texturePrefix + "00_bar");
            _pointerTexture = _content.Load<Texture2D>(texturePrefix + "01_pointer");
            _targetTexture = _content.Load<Texture2D>(texturePrefix + "02_traget");

            _audio.PlaySFX("startLens");
        }

        public override void Update(GameTime gameTime)
        {
            if (IsClosed) return;

            // ขยับเฉพาะ pointer ส่วนเป้าหมายจะอยู่กับที่จนกว่าจะกดโดน
            _pointerX = Bounce(_pointerX, ref _pointerSpeed, TrackX, TrackX + BarWidth - PointerWidth);

            if (_progress >= 100f)
            {
                Complete();
            }
        }

        public override void InputHandler(GameTime gameTime)
        {
            if (IsClosed) return;

            KeyboardState currentKeyboardState = Keyboard.GetState();
            bool spacePressed = currentKeyboardState.IsKeyDown(Keys.Space)
                               && _previousKeyboardState.IsKeyUp(Keys.Space);
            _previousKeyboardState = currentKeyboardState;
            if (!spacePressed) return;
            if (TargetHitbox.Intersects(PointerHitbox))
            {
                _progress = MathHelper.Clamp(_progress + BaseProgressPoint, 0f, 100f);
                _audio.PlaySFX("hitLens");
                SpawnTarget(true);
            }
            else
            {
                _audio.PlaySFX("errorLens");
                _progress = MathHelper.Clamp(_progress - MissPenalty, 0f, 100f);
                if (_progress <= 0f)
                {
                    Fail();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            MinigameBackdrop.Draw(_spriteBatch, MinigameBackdrop.Bounds);
            _spriteBatch.Draw(_barTexture, TrackRect, Color.White);
            DrawControlHint("[ Space bar ]");
            _spriteBatch.Draw(_targetTexture, TargetRect, Color.White);
            _spriteBatch.Draw(_pointerTexture, PointerRect, Color.White);

            if (ShowDebugHitboxes)
            {
                _spriteBatch.DrawRectangle(TargetHitbox, Color.Purple);
                _spriteBatch.DrawRectangle(PointerHitbox, Color.Orange);
            }
        }

        private void DrawControlHint(string text)
        {
            const float scale = 0.8f;
            var textSize = _font.MeasureString(text);
            var position = new Vector2(
                TrackRect.Center.X - textSize.Width * scale / 2f,
                TrackRect.Top - textSize.Height * scale - 12f);

            _spriteBatch.DrawString(_font, text, position + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(_font, text, position, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        private void SpawnTarget(bool avoidPreviousPosition)
        {
            int previousPosition = (int)_targetX;
            _targetWidth = _random.Next(MinTargetWidth, MaxTargetWidth + 1);
            int maxPosition = TrackX + BarWidth - _targetWidth;
            int position;
            do
            {
                position = _random.Next(TrackX, maxPosition + 1);
            } while (avoidPreviousPosition && position == previousPosition);
            _targetX = position;
        }

        private static float Bounce(float position, ref float speed, float min, float max)
        {
            position += speed;

            if (position < min)
            {
                position = min;
                speed = Math.Abs(speed);
            }
            else if (position > max)
            {
                position = max;
                speed = -Math.Abs(speed);
            }

            return position;
        }
    }
}
