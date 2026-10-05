using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Project_The_Elect.source_code
{
    public sealed class QuestProgress
    {
        private readonly BitmapFont _font;
        private readonly Texture2D _dialogueBackground;
        private readonly Texture2D _pixel;
        private float _noticeTime;

        public const int MaxSanity = 100;
        public int Sanity { get; private set; } = MaxSanity;
        public int Day { get; private set; } = 1;
        public int MonsterIndex { get; private set; } = 1;
        public bool IsSanityDepleted => Sanity <= 0;
        public bool CanSubmitData => IsStarted && LensComplete && MeatCollected && ScanComplete && KeypadComplete && !DataSubmitted;
        public bool IsStarted { get; private set; }
        public bool LensComplete { get; private set; }
        public bool HasScissors { get; private set; }
        public bool MeatCollected { get; private set; }
        public bool ScanComplete { get; private set; }
        public bool KeypadComplete { get; private set; }
        public bool DataSubmitted { get; private set; }
        public string Notice { get; private set; }
        public string DialogueMessage { get; private set; }
        public bool HasDialogueMessage => !string.IsNullOrEmpty(DialogueMessage);

        public QuestProgress(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _font = content.Load<BitmapFont>("font/fontGenshin");
            _dialogueBackground = content.Load<Texture2D>("texture/03_dialogue/01_dialogueBG2");
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Start()
        {
            IsStarted = true;
        }

        public void SpendSanity(int amount)
        {
            if (amount <= 0 || DataSubmitted)
                return;

            Sanity = System.Math.Max(0, Sanity - amount);
         
        }

        public bool TrySleep()
        {
            if (!IsStarted || (!DataSubmitted && !IsSanityDepleted))
                return false;

            if (DataSubmitted)
                MonsterIndex++;

            Day++;
            Sanity = MaxSanity;
            LensComplete = false;
            HasScissors = false;
            MeatCollected = false;
            ScanComplete = false;
            KeypadComplete = false;
            DataSubmitted = false;
            Notice = $"Day {Day} started. Quest progress has reset.";
            _noticeTime = 2.5f;
            DialogueMessage = null;
            IsStarted = true;
            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (_noticeTime <= 0f)
                return;

            _noticeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_noticeTime <= 0f)
                Notice = null;
        }

        public void ShowNotice(string text)
        {
            Notice = text;
            _noticeTime = 2.5f;
        }

        public void ShowDialogueMessage(string text)
        {
            DialogueMessage = text;
        }

        public void DismissDialogueMessage()
        {
            DialogueMessage = null;
        }

        public bool CollectScissors()
        {
            if (!IsStarted || !LensComplete || HasScissors || MeatCollected)
                return false;

            HasScissors = true;
            ShowNotice("Scissors collected.");
            return true;
        }

        public bool TryCutMeat()
        {
            if (!IsStarted || !LensComplete || !HasScissors || MeatCollected)
                return false;

            MeatCollected = true;
            ShowNotice("Meat collected");
            return true;
        }

        public bool CanStartMachine(string machineName)
        {
            if (!IsStarted)
                return false;

            if (machineName == "LensMachine")
                return !LensComplete;

            if (!MeatCollected)
                return false;

            if (machineName == "ScanMachine")
                return !ScanComplete;

            if (machineName == "KeypadMachine")
                return !KeypadComplete;

            return false;
        }

        public string GetMachineBlockReason(string machineName)
        {
            if (!IsStarted)
                return "";

            if (machineName == "LensMachine")
                return LensComplete ? "" : null;

            if (machineName == "ScanMachine" || machineName == "KeypadMachine")
            {
                if (!MeatCollected)
                    return "";

                bool alreadyComplete = machineName == "ScanMachine" ? ScanComplete : KeypadComplete;
                return alreadyComplete ? "" : null;
            }

            return null;
        }

        public void CompleteMachine(string machineName)
        {
            if (machineName == "LensMachine" && IsStarted && !LensComplete)
            {
                LensComplete = true;

            }
            else if (machineName == "ScanMachine" && MeatCollected)
            {
                ScanComplete = true;
            }
            else if (machineName == "KeypadMachine" && MeatCollected)
            {
                KeypadComplete = true;
            }
        }

        public bool TrySubmitData()
        {
            if (!CanSubmitData)
                return false;

            DataSubmitted = true;
            ShowNotice("Data submitted. Quest complete.");
            return true;
        }

        public bool SkipNextObjective()
        {
            if (!IsStarted || DataSubmitted)
                return false;

            if (!LensComplete)
            {
                CompleteMachine("LensMachine");
                return LensComplete;
            }

            if (!MeatCollected)
            {
                // The HUD treats finding scissors and cutting the meat as one objective.
                HasScissors = true;
                return TryCutMeat();
            }

            if (!ScanComplete)
            {
                CompleteMachine("ScanMachine");
                return ScanComplete;
            }

            if (!KeypadComplete)
            {
                CompleteMachine("KeypadMachine");
                return KeypadComplete;
            }

            return TrySubmitData();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawSanity(spriteBatch);

            if (!IsStarted)
                return;

            const float x = 1390f;
            float y = 32f;
            DrawText(spriteBatch, "QUEST", x, y, Color.White, 0.9f);
            y += 38f;

            DrawTask(spriteBatch, "SCAN LAB", LensComplete, !LensComplete, x, ref y);
            string meatTask = HasScissors || MeatCollected
                ? "Cut the monster meat"
                : "Find scissors to cut the meat";
            DrawTask(spriteBatch, meatTask, MeatCollected, LensComplete && !MeatCollected, x, ref y);
            DrawTask(spriteBatch, "MICROSCOPE LAB", ScanComplete, MeatCollected && !ScanComplete, x, ref y);
            DrawTask(spriteBatch, "CHEMICAL LAB", KeypadComplete, MeatCollected && !KeypadComplete, x, ref y);
            DrawTask(spriteBatch, "Submit In DATA ROOM", DataSubmitted, ScanComplete && KeypadComplete && !DataSubmitted, x, ref y);

            if (!string.IsNullOrEmpty(Notice))
                DrawText(spriteBatch, Notice, x, y + 4f, Color.Orange, 0.65f);
        }

        private void DrawSanity(SpriteBatch spriteBatch)
        {
            const int x = 32;
            const int y = 36;
            const int width = 360;
            const int height = 26;

            DrawText(spriteBatch, $"SANITY {Sanity}/{MaxSanity}  |  DAY {Day}  |  MONSTER {MonsterIndex}", x, y - 29, Color.White, 0.72f);
            spriteBatch.Draw(_pixel, new Rectangle(x - 2, y - 2, width + 4, height + 4), Color.Black);
            spriteBatch.Draw(_pixel, new Rectangle(x, y, width, height), Color.DarkGray);

            int fillWidth = width * Sanity / MaxSanity;
            Color fillColor = Sanity <= 20 ? Color.Red : Sanity <= 50 ? Color.Orange : Color.LimeGreen;
            if (fillWidth > 0)
                spriteBatch.Draw(_pixel, new Rectangle(x, y, fillWidth, height), fillColor);
        }

        

        private void DrawTask(SpriteBatch spriteBatch, string text, bool complete, bool active, float x, ref float y)
        {
            string prefix = complete ? "[DONE] " : active ? "> " : "[LOCKED] ";
            Color color = complete ? Color.LimeGreen : active ? Color.White : Color.Gray;
            DrawText(spriteBatch, prefix + text, x, y, color, 0.72f);
            y += 31f;
        }

        private void DrawText(SpriteBatch spriteBatch, string text, float x, float y, Color color, float scale)
        {
            Vector2 position = new Vector2(x, y);
            spriteBatch.DrawString(_font, text, position + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
