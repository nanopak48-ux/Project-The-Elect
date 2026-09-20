using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using Project_The_Elect;
using Project_The_Elect.source_code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public class Player
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public float Speed = 300f;
        public float JumpForce = 500f;
        public float Gravity = 1200f;

        public int Health = 100;

        public enum PlayerState
        {
            Ready,
            Busy
        }
        public enum PlayerAnimState
        {
            Idle,
            Walk,
            Interact
        }

        public PlayerState State { get; private set; }
        public PlayerAnimState AnimState { get; private set; }


        private bool isGrounded;

        private Texture2D texture;
        private SpriteBatch spriteBatch;

        public Player(SpriteBatch spriteBatch, Vector2 position, ContentManager content)
        {
            this.spriteBatch = spriteBatch;
            Position = position;

            State = PlayerState.Ready;
        }

        public void Update()
        {
            InputHandler();
            UpdateMovement();
            UpdateState();
        }

        private void InputHandler()
        {

        }

        private void UpdateMovement()
        {

        }

        private void UpdateState()
        {

        }


        public void Draw()
        {
            spriteBatch.Draw(
                texture,
                Position,
                Color.White
            );
        }
    }

}
