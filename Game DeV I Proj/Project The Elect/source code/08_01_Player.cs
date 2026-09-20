using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Project_The_Elect.source_code
{
    public class Player
    {
        public Vector2 Position;
        public Vector2 Velocity = new(5,5);

        private int size = 16;

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

        public enum InputState
        {
            W,
            A,
            S,
            D

        }

        private InputState Input { get; set; }
        public PlayerState State { get; private set; }
        public PlayerAnimState AnimState { get; private set; }


        private bool isGrounded;
        private SpriteSheet spriteSheet;
        private SpriteBatch spriteBatch;

        private AnimatedSprite playerSprite;

        public Player(SpriteBatch spriteBatch, Vector2 position, ContentManager content,Texture2D texture)
        {
            this.spriteBatch = spriteBatch;
            Position = position;

            Texture2DAtlas atlas = Texture2DAtlas.Create("Atlas/playerAtlas", texture, size, size);
            spriteSheet = new SpriteSheet("SpriteSheet/player", atlas);
            DefineAnimation(content);

            State = PlayerState.Ready;
        }

        public void Update(GameTime gameTime)
        {

            InputHandler();
            UpdateState();
            UpdateAnim(gameTime);
        }

        public void Draw()
        {
            spriteBatch.Draw(playerSprite,Position);
        }
        private void UpdateState()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W) ||
                keyboardState.IsKeyDown(Keys.A) ||
                keyboardState.IsKeyDown(Keys.S) ||
                keyboardState.IsKeyDown(Keys.D)
                )
            {
                AnimState = PlayerAnimState.Walk;
            }
        }
        public void Audio(GameTime gameTime)
        {
            if (AnimState == PlayerAnimState.Walk)
            {

            }
        }
        private void InputHandler()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            UpdateMovement(keyboardState);
            
        }
        private void UpdateAnim(GameTime gameTime)
        {
            playerSprite.Update(gameTime);
        }
        private void UpdateMovement(KeyboardState keyboardState)
        {
            if (State == PlayerState.Ready)
            {
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    Position.Y -= Velocity.Y;
                    if (playerSprite.CurrentAnimation != "WalkUp") playerSprite.SetAnimation("WalkUp");
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    Position.X -= Velocity.X;
                    if (playerSprite.CurrentAnimation != "WalkLeft") playerSprite.SetAnimation("WalkLeft");
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    Position.Y += Velocity.Y;
                    if (playerSprite.CurrentAnimation != "WalkDown") playerSprite.SetAnimation("WalkDown");
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    Position.X += Velocity.X;
                    if (playerSprite.CurrentAnimation != "WalkRight") playerSprite.SetAnimation("WalkRight");
                }
            }
        }

        private void DefineAnimation(ContentManager content)
        {
            string path = Path.Combine(content.RootDirectory,"playerdata","player_animation.json");

            string json = File.ReadAllText(path);

            PlayerAnimationData? data = JsonSerializer.Deserialize<PlayerAnimationData>(json);

            foreach (AnimationData animation in data.Animations)
            {
                spriteSheet.DefineAnimation(
                    animation.Name,
                    builder =>
                    {
                        builder.IsLooping(animation.Loop);

                        TimeSpan frameDuration =
                            TimeSpan.FromSeconds(animation.FrameDuration);

                        foreach (int frame in animation.Frames)
                        {
                            builder.AddFrame(
                                frame,
                                frameDuration);
                        }
                    });
            }

            playerSprite = new AnimatedSprite(spriteSheet,"WalkDown");
        }

    }

}
