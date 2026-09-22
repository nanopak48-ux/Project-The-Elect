using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public abstract class Minigame
    {
        public bool IsCompleted { get; protected set; }
        public bool IsClosed { get; protected set; }

        public abstract void Initialize();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        public abstract void InputHandler(GameTime gameTime);

        public virtual void Close()
        {
            IsClosed = true;
        }
    }
}
