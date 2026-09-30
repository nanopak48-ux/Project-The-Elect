using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        private const int BarHeight = 180;          // ความสูงราง (และความสูงของเป้า)
        private const int TargetWidth = 120;        // ความกว้างเป้า 

        private const int PointerWidth = 30;        // ตัวชี้สีแดง
        private const int PointerHeight = 136;

        private const float PointerIncrement = 15f; // ความเร็วตัวชี้ (พิกเซลต่อเฟรม)
        private const float BarIncrement = 10f;     // ความเร็วเป้า (พิกเซลต่อเฟรม)

        private const int BarHitboxPadding = 30;     // ยิ่งมาก เป้ายิ่งกดโดนง่าย
        private const int PointerHitboxPadding = 15; // ยิ่งมาก ตัวชี้ยิ่งกดโดนง่าย

        private const float BaseProgressPoint = 20f;              // โดน +30
        private const float MissPenalty = BaseProgressPoint * 0.2f; // พลาด -6
        //=====================================================================//

        //====================== CONFIG: ตำแหน่งบนหน้าจอ ======================//
        // รางแนวนอน: กึ่งกลางจอตามแนวนอน, อยู่ที่ 1/4 ของความสูงจอ
        private static int TrackX => (GameConfig.ScreenWidth - BarWidth) / 2;
        private static int TrackY => GameConfig.ScreenHeight / 2 + 250;

        // ตัวชี้อยู่กึ่งกลางแนวตั้งของราง (ถ้าอยากชิดขอบบนแบบเดิม ให้เปลี่ยนเป็น => TrackY)
        private static int PointerY => TrackY + (BarHeight - PointerHeight) / 2;

        // แท่งความคืบหน้าแนวตั้งด้านซ้าย (ออกแบบไว้สำหรับจอสูง 1080)
        private const int ProgressX = 47;
        private const int ProgressY = 75;
        private const int ProgressWidth = 120;
        private const int ProgressHeight = 905;
        //=====================================================================//

        /// <summary>เปิดเพื่อวาดกรอบ hitbox (ม่วง = เป้า, ส้ม = ตัวชี้) ตอนปรับความง่าย</summary>
        public bool ShowDebugHitboxes = false;

        private readonly SpriteBatch _spriteBatch;
        private readonly ContentManager _content;
        private readonly GameAudioManager _audio;

        private Texture2D _barTexture;

        private float _pointerX;
        private float _targetX;
        private float _pointerSpeed;
        private float _targetSpeed;

        private float _progress; 

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
        private Rectangle TargetRect => new((int)_targetX, TrackY, TargetWidth, BarHeight);
        private Rectangle PointerRect => new((int)_pointerX, PointerY, PointerWidth, PointerHeight);

        private Rectangle TargetHitbox => new(
            (int)_targetX - BarHitboxPadding,
            TrackY,
            TargetWidth + BarHitboxPadding * 2,
            BarHeight);

        private Rectangle PointerHitbox => new(
            (int)_pointerX - PointerHitboxPadding,
            PointerY - PointerHitboxPadding,
            PointerWidth + PointerHitboxPadding * 2,
            PointerHeight + PointerHitboxPadding * 2);

        // ---------- วงจรชีวิตของมินิเกม ----------

        public override void Initialize()
        {
            IsClosed = false;
            IsCompleted = false;

            _progress = 0f;
            _pointerX = TrackX;
            _targetX = TrackX;
            _pointerSpeed = PointerIncrement;
            _targetSpeed = BarIncrement;
        }

        public override void LoadContent()
        {
            string prefix = "texture/09_minigame/01_scan/";
            _barTexture = _content.Load<Texture2D>(prefix + "00_bar");

            _audio.PlaySFX("scan_start");
        }

        public override void Update(GameTime gameTime)
        {
            if (IsClosed) return;

            // ตัวชี้และเป้าวิ่งไปกลับภายในรางพอดี (ไม่ยื่นเลยปลายราง)
            _pointerX = Bounce(_pointerX, ref _pointerSpeed, TrackX, TrackX + BarWidth - PointerWidth);
            _targetX = Bounce(_targetX, ref _targetSpeed, TrackX, TrackX + BarWidth - TargetWidth);

            if (_progress >= 100f)
            {
                IsCompleted = true;
                Close();
            }
        }

        public override void InputHandler(GameTime gameTime)
        {
            if (IsClosed) return;

            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();
            if (!keyboardState.WasKeyPressed(Keys.Space)) return;

            if (TargetHitbox.Intersects(PointerHitbox))
            {
                _progress = MathHelper.Clamp(_progress + BaseProgressPoint, 0f, 100f);
                _audio.PlaySFX("scan_collision");
            }
            else
            {
                _progress = MathHelper.Clamp(_progress - MissPenalty, 0f, 100f);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.DrawRectangle(TrackRect, Color.Green);
            _spriteBatch.Draw(_barTexture, TargetRect, Color.White);
            _spriteBatch.DrawRectangle(PointerRect, Color.Red);

            _spriteBatch.DrawRectangle(
                new Rectangle(ProgressX, ProgressY, ProgressWidth, ProgressHeight),
                Color.Green);

            int fillHeight = (int)(_progress / 100f * ProgressHeight);
            int fillY = ProgressY + ProgressHeight - fillHeight;
            _spriteBatch.Draw(
                _barTexture,
                new Rectangle(ProgressX, fillY, ProgressWidth, fillHeight),
                Color.White);

            if (ShowDebugHitboxes)
            {
                _spriteBatch.DrawRectangle(TargetHitbox, Color.Purple);
                _spriteBatch.DrawRectangle(PointerHitbox, Color.Orange);
            }
        }

        public override void Close()
        {
            if (IsClosed) return; 

            _audio.PlaySFX("scan_start");
            base.Close();
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