using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public enum MinigameResult
    {
        None,
        Success,
        Failed,
        Cancelled
    }

    public abstract class Minigame
    {
        public bool IsCompleted { get; protected set; }
        public bool IsClosed { get; protected set; }
        public MinigameResult Result { get; private set; }

        public abstract void Initialize();

        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        public abstract void InputHandler(GameTime gameTime);

        public virtual void Close()
        {
            if (IsClosed)
                return;

            if (Result == MinigameResult.None)
                Result = MinigameResult.Cancelled;

            IsClosed = true;
        }

        protected void ResetResult()
        {
            IsCompleted = false;
            IsClosed = false;
            Result = MinigameResult.None;
        }

        protected void Complete()
        {
            IsCompleted = true;
            Result = MinigameResult.Success;
            Close();
        }

        protected void Fail()
        {
            Result = MinigameResult.Failed;
            Close();
        }
    }
}
