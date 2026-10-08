using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Project_The_Elect.source_code
{
    public class Player
    {
        public enum PlayerState { Ready, Busy }
        public enum PlayerAnimState { Idle, Walk, Interact }
        public enum Direction { Down, Left, Right, Up }

        public Vector2 Position;
        public Vector2 FrameSize = new(16, 32);
        public float Scale = 2.5f;
        public Vector2 Size => FrameSize * Scale;

        public RectangleF HitBox { get; private set; }

        public float Speed = 300f;
        public int Health = 100;

        public PlayerState State { get; private set; }
        public PlayerAnimState AnimState { get; private set; }
        public Direction Facing { get; private set; } = Direction.Down;

        public bool InteractPressed { get; private set; }

        private KeyboardState previousKeyboard;
        private readonly SpriteBatch spriteBatch;
        private SpriteSheet spriteSheet;
        private AnimatedSprite playerSprite;

        public Player(SpriteBatch spriteBatch, Vector2 position, ContentManager content, Texture2D texture)
        {
            this.spriteBatch = spriteBatch;
            Position = position;

            Texture2DAtlas atlas = Texture2DAtlas.Create("Atlas/playerAtlas", texture, (int)FrameSize.X, (int)FrameSize.Y);
            spriteSheet = new SpriteSheet("SpriteSheet/player", atlas);
            DefineAnimation(content);

            State = PlayerState.Ready;
            AnimState = PlayerAnimState.Idle;
            previousKeyboard = Keyboard.GetState();
            UpdateHitBox();
        }

        public void Freeze() => State = PlayerState.Busy;
        public void Unfreeze() => State = PlayerState.Ready;

        public void Update(GameTime gameTime, List<RectangleF> walls)
        {
            KeyboardState keyboard = Keyboard.GetState();

            InteractPressed = State == PlayerState.Ready
                              && keyboard.IsKeyDown(Keys.E)
                              && previousKeyboard.IsKeyUp(Keys.E);

            Vector2 direction = State == PlayerState.Ready
                ? ReadMovementInput(keyboard)
                : Vector2.Zero;

            Move(direction, gameTime, walls);
            UpdateState(direction);
            UpdateAnimation(gameTime);

            previousKeyboard = keyboard;
        }

        public void UpdateWithoutMovement(GameTime gameTime)
        {
            InteractPressed = false;
            UpdateState(Vector2.Zero);
            UpdateAnimation(gameTime);
            previousKeyboard = Keyboard.GetState();
        }

        public void Draw()
        {
            spriteBatch.Draw(playerSprite, Position, 0f, new Vector2(Scale));
        }
        //==========================================================================//
        public void DrawDebug()
        {
            // วาดกรอบ HitBox ของผู้เล่นเป็นสีเขียว หนา 2 พิกเซล
            //spriteBatch.DrawRectangle(HitBox, Color.Green, 2f);
        }
        //==========================================================================//
        public void Audio(GameTime gameTime) { }

        private static Vector2 ReadMovementInput(KeyboardState keyboard)
        {
            Vector2 direction = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
            if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1;
            if (keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
            if (keyboard.IsKeyDown(Keys.D)) direction.X += 1;

            return direction;
        }

        private void Move(Vector2 direction, GameTime gameTime, List<RectangleF> walls)
        {
            if (direction == Vector2.Zero) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            direction.Normalize();

            Vector2 velocity = direction * Speed * deltaTime;

            // เช็คแยกแกน X
            Vector2 testPosX = Position + new Vector2(velocity.X, 0);
            RectangleF testHitBoxX = GetHitBoxAtPosition(testPosX);

            bool collideX = false;
            if (walls != null)
            {
                foreach (var wall in walls)
                {
                    if (testHitBoxX.Intersects(wall))
                    {
                        collideX = true;
                        break;
                    }
                }
            }

            if (!collideX) Position.X = testPosX.X;

            // เช็คแยกแกน Y
            Vector2 testPosY = Position + new Vector2(0, velocity.Y);
            RectangleF testHitBoxY = GetHitBoxAtPosition(testPosY);

            bool collideY = false;
            if (walls != null)
            {
                foreach (var wall in walls)
                {
                    if (testHitBoxY.Intersects(wall))
                    {
                        collideY = true;
                        break;
                    }
                }
            }

            if (!collideY) Position.Y = testPosY.Y;

            UpdateHitBox();
        }

        private RectangleF GetHitBoxAtPosition(Vector2 pos)
        {
            Vector2 topLeft = pos - Size / 2f;
            return new RectangleF(
                topLeft.X + 20f,
                topLeft.Y + Size.Y,
                Size.X,
                Size.Y / 2f
            );
        }

        private void UpdateHitBox()
        {
            HitBox = GetHitBoxAtPosition(Position);
        }

        private void UpdateState(Vector2 direction)
        {
            if (direction == Vector2.Zero)
            {
                AnimState = PlayerAnimState.Idle;
                return;
            }

            AnimState = PlayerAnimState.Walk;

            if (direction.X != 0)
                Facing = direction.X < 0 ? Direction.Left : Direction.Right;
            else
                Facing = direction.Y < 0 ? Direction.Up : Direction.Down;
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            string prefix = AnimState == PlayerAnimState.Walk ? "Walk" : "Idle";
            string animationName = prefix + Facing;

            if (playerSprite.CurrentAnimation != animationName)
                playerSprite.SetAnimation(animationName);

            playerSprite.Update(gameTime);
        }

        private void DefineAnimation(ContentManager content)
        {
            string path = Path.Combine(
                AppContext.BaseDirectory,
                content.RootDirectory,
                "data", "dataPlayer", "player_animation.json");

            if (!File.Exists(path))
                throw new FileNotFoundException($"ไม่พบไฟล์แอนิเมชันของผู้เล่น: {path}");

            string json = File.ReadAllText(path);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            PlayerAnimationData data = JsonSerializer.Deserialize<PlayerAnimationData>(json, options);

            if (data?.Animations == null)
                throw new InvalidDataException("player_animation.json อ่านได้แต่ไม่มีรายการ animations");

            foreach (AnimationData animation in data.Animations)
            {
                spriteSheet.DefineAnimation(
                    animation.Name,
                    builder =>
                    {
                        builder.IsLooping(animation.Loop);
                        TimeSpan frameDuration = TimeSpan.FromSeconds(animation.FrameDuration);

                        foreach (int frame in animation.Frames)
                        {
                            builder.AddFrame(frame, frameDuration);
                        }
                    });
            }

            playerSprite = new AnimatedSprite(spriteSheet, "IdleDown");
        }
    }
}
