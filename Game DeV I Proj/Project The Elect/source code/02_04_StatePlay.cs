using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_The_Elect.source_code
{
    public class StatePlay : IGameState, IOverlayBackgroundState
    {
        private GameStateManager _gameState;
        private GameFlowManager _gameFlow;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private GameAudioManager _audio;
        private GraphicsDeviceManager _graphics;
        private GameWindow _window;
        private MinigameManager _minigame;
        private QuestProgress _questProgress;
        private MinigameResult _lastMinigameResult = MinigameResult.None;
        public MinigameResult LastMinigameResult => _lastMinigameResult;

        private CollisionManager Collision;

        private Player _player;
        private GraphicsDevice _graphicDevice;
        private GameMapManager _map;
        private InteractManager _interactManager;

        private OrthographicCamera _camera;
        private Vector2 lookAtPos;
        private TimeSpan _timeUntilQuestDialogue = TimeSpan.FromSeconds(2);
        private TimeSpan _timeSincePlayStarted = TimeSpan.Zero;
        private const float MeatCutBlackoutDuration = 3f;
        private const float SleepBlackoutDuration = 2f;
        private const float MinigameResultDialogueDelay = 1f;
        private const float MinigameStartDelay = 1f;
        private bool _isBlackout;
        private float _blackoutRemaining;
        private Action _afterBlackout;
        private float _minigameDialogueDelayRemaining;
        private int _pendingMinigameDialogueChapter;
        private float _minigameStartDelayRemaining;
        private string _pendingMinigameMachineName;
        private Minigame _pendingMinigame;
        private bool _questDialogueTriggered;
        private bool _lensInstructionsShown;
        private bool _lensPraiseShown;
        private string _activeMachineName;
        public StatePlay
            (
            GameStateManager _stateManager,
            GameFlowManager _gameflow,
            SpriteBatch _spritebatch,
            ContentManager _content,
            GameAudioManager _audio,
            GraphicsDeviceManager _graphics,
            GameWindow _window
            )
        {
            _gameState = _stateManager;
            this._gameFlow = _gameflow;
            _questProgress = _gameflow.QuestProgress;
            this._spriteBatch = _spritebatch;
            this._content = _content;
            this._audio = _audio;
            this._graphics = _graphics;
            this._window = _window;
            _graphicDevice = _graphics.GraphicsDevice;

            Initialize();
            LoadContent();
        }
        public void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GameConfig.ScreenWidth;
            _graphics.PreferredBackBufferHeight = GameConfig.ScreenHeight;
            _graphics.ApplyChanges();

            ViewportAdapter viewportAdapter = new BoxingViewportAdapter(_window, _graphicDevice, GameConfig.CameraWidth, GameConfig.CameraHeight);
            _camera = new OrthographicCamera(viewportAdapter);
            viewportAdapter.Reset();

            _minigame = new MinigameManager();
            _minigame.MinigameEnded += OnMinigameEnded;
            _map = new GameMapManager(_content, _graphics.GraphicsDevice, _spriteBatch);
        }

        private Texture2D map;
        public void LoadContent()
        {
            Texture2D texture = _content.Load<Texture2D>("texture/08_player/00_spritesheet_player5");
            _player = new Player(_spriteBatch, new Vector2(44 * 64 + 16, 58 * 64), _content, texture);
            map = _content.Load<Texture2D>("texture/04_play/00_map");

            _map.LoadContent();

            if (_map._tilemap != null)
            {
                _interactManager = new InteractManager(_map._tilemap);
            }
        }

        public Vector2 currentCenter;

        public void Update(GameTime gameTime)
        {
            _questProgress.Update(gameTime);

            if (_isBlackout)
            {
                _blackoutRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                _player.UpdateWithoutMovement(gameTime);

                if (_blackoutRemaining <= 0f)
                {
                    _isBlackout = false;
                    _player.Unfreeze();
                    Action afterBlackout = _afterBlackout;
                    _afterBlackout = null;
                    afterBlackout?.Invoke();
                }

                return;
            }

            if (_pendingMinigameDialogueChapter != 0)
            {
                _minigameDialogueDelayRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_minigameDialogueDelayRemaining <= 0f)
                {
                    int chapter = _pendingMinigameDialogueChapter;
                    _pendingMinigameDialogueChapter = 0;
                    _gameFlow.ShowQuestWarningDialogue(chapter, () => _player.Unfreeze());
                }
            }

            if (_pendingMinigame != null)
            {
                _minigameStartDelayRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_minigameStartDelayRemaining <= 0f)
                {
                    string machineName = _pendingMinigameMachineName;
                    Minigame minigame = _pendingMinigame;
                    _pendingMinigameMachineName = null;
                    _pendingMinigame = null;
                    StartMinigame(machineName, minigame);
                }
            }

            _map.Update(gameTime);
            _interactManager?.Update(_player);

            List<RectangleF> walls = _map.GetWallCollisions();
            if (_questProgress.HasDialogueMessage)
                _player.UpdateWithoutMovement(gameTime);
            else
                _player.Update(gameTime, walls);

            if (_minigame.IsPlaying)
            {
                _minigame.Update(gameTime);

                if (!_minigame.IsPlaying && _pendingMinigameDialogueChapter == 0)
                    _player.Unfreeze();
                return;
            }

            if (!_questDialogueTriggered)
            {
                _timeSincePlayStarted += gameTime.ElapsedGameTime;
                if (_timeSincePlayStarted >= _timeUntilQuestDialogue)
                {
                    _questDialogueTriggered = _gameFlow.TryStartChapter02Dialogue();
                    if (_questDialogueTriggered)
                        return;
                }
            }

            Vector2 lookAtPos = _player.Position;
            currentCenter = _camera.Position + _camera.Origin;
            _camera.LookAt(Vector2.Lerp(currentCenter, lookAtPos, 0.1f));
        }

        public void UpdateWhileOverlay(GameTime gameTime)
        {
            _map.Update(gameTime);
            _interactManager?.Update(_player);
            _player.UpdateWithoutMovement(gameTime);

            Vector2 lookAtPos = _player.Position;
            currentCenter = _camera.Position + _camera.Origin;
            _camera.LookAt(Vector2.Lerp(currentCenter, lookAtPos, 0.1f));
        }

        public void InputHandler(GameTime gameTime)
        {
            KeyboardExtended.Update();

            if (_isBlackout || _pendingMinigameDialogueChapter != 0 || _pendingMinigame != null)
                return;

            CheckEscape();

            if (_minigame.IsPlaying)
            {
                _minigame.InputHandler(gameTime);
                return;
            }

            InputHandlerMinigame();
        }

        public void Draw(GameTime gameTime)
        {
            if (_isBlackout)
            {
                _graphics.GraphicsDevice.Clear(Color.Black);
                return;
            }

            Matrix transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

            // 1. วาดแมพ
            _map.Draw(gameTime, _camera, "floor_white");
            _map.Draw(gameTime, _camera, "floor_black");
            _map.Draw(gameTime, _camera, "background");
            _map.Draw(gameTime, _camera, "middleground");
            _map.Draw(gameTime, _camera, "stuff");
            _map.Draw(gameTime, _camera, "light");
            _map.Draw(gameTime, _camera, "foreground");
            _map.Draw(gameTime, _camera, "wall_side");
            _map.Draw(gameTime, _camera, "wall_upper");
            _map.Draw(gameTime, _camera, "wall_lower");
            string monsterLayer = $"mon{_questProgress.MonsterIndex}";
            if (_map.HasLayer(monsterLayer))
                _map.Draw(gameTime, _camera, monsterLayer);
            if (!_questProgress.HasScissors && _map.HasLayer("scissors"))
                _map.Draw(gameTime, _camera, "scissors");
            if (!_questProgress.HasFlashDrive && _map.HasLayer("flashdrive"))
                _map.Draw(gameTime, _camera, "flashdrive");

            // 2. วาดตัวละคร
            _player.Draw();

            // --------------------------------------------------
            // 3. วาด Debug Collision (เห็นกรอบสีแดงของแมพ และกรอบสีเขียวของตัวละคร)
            _map.DrawDebug(_spriteBatch);
            _player.DrawDebug();
            _interactManager?.DrawDebug(_spriteBatch);
            // --------------------------------------------------

            _spriteBatch.End();

            _spriteBatch.Begin();
            if (!_minigame.IsPlaying)
                _questProgress.Draw(_spriteBatch);
            if (_minigame.IsPlaying) _minigame.Draw(gameTime);
            _spriteBatch.End();
        }

        public void AudioHandler(GameTime gameTime)
        {
            _player.Audio(gameTime);
        }
        /*private void InputHandlerMinigame()
        {
            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();


            if (keyboardState.WasKeyPressed(Keys.U))
            {
                StartMinigame(new MinigameScan(_spriteBatch, _camera, _content, _audio));
            }
            else if (keyboardState.WasKeyPressed(Keys.I))
            {
                StartMinigame(new MinigameLens(_spriteBatch, _camera, _content, _audio));
            }
            else if (keyboardState.WasKeyPressed(Keys.O))
            {

            }
            else if (keyboardState.WasKeyPressed(Keys.P))
            {

            }


        }*/
        private void InputHandlerMinigame()
        {
            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();

            if (_questProgress.HasDialogueMessage)
            {
                if (keyboardState.WasKeyPressed(Keys.E)
                    || keyboardState.WasKeyPressed(Keys.Space)
                    || keyboardState.WasKeyPressed(Keys.Enter))
                {
                    _questProgress.DismissDialogueMessage();
                }

                return;
            }

            if (keyboardState.WasKeyPressed(Keys.N))
            {
                _questProgress.SkipNextObjective();
                return;
            }

            // เช็คว่ามีการกดปุ่ม E หรือไม่
            if (keyboardState.WasKeyPressed(Keys.E))
            {
                if (_interactManager?.CurrentNearbyObject != null)
                {
                    InteractableObject nearbyObject = _interactManager.CurrentNearbyObject;
                    string objectName = nearbyObject.Name;

                    if (_questProgress.IsSanityDepleted && !_questProgress.DataSubmitted
                        && objectName != "Bed"
                        && !(objectName == "DataSubmissionPoint" && _questProgress.CanSubmitData))
                    {
                        _gameFlow.ShowQuestWarningDialogue(9);
                        return;
                    }

                    if (_questProgress.IsStarted
                        && !_questProgress.HasFlashDrive
                        && IsQuestWorkObject(objectName))
                    {
                        _gameFlow.ShowQuestWarningDialogue(12);
                        return;
                    }

                    switch (objectName)
                    {
                        case "FlashDrive":
                            _questProgress.CollectFlashDrive();
                            break;

                        case "Bed":
                            if (_questProgress.DataSubmitted && !_questProgress.IsSanityDepleted)
                            {
                                _gameFlow.ShowQuestWarningDialogue(10, BeginSleepTransition);
                            }
                            else if (_questProgress.IsSanityDepleted)
                            {
                                BeginSleepTransition();
                            }
                            else
                            {
                                _gameFlow.ShowQuestWarningDialogue(8);
                            }
                            break;

                        case "ScissorsItem":
                            if (!_questProgress.LensComplete)
                                _gameFlow.ShowQuestWarningDialogue(3);
                            else if (_questProgress.CollectScissors())
                            {
                                // Keep this object registered so it can be collected after the daily reset.
                            }
                            break;

                        case "MonsterCutPoint":
                            if (!_questProgress.LensComplete)
                                _gameFlow.ShowQuestWarningDialogue(3);
                            else if (!_questProgress.HasScissors)
                                _gameFlow.ShowQuestWarningDialogue(4);
                            else if (!_questProgress.TryCutMeat())
                                ShowCompletedTaskDialogue();
                            else
                            {
                                _audio.PlaySFX("error_task");
                                BeginBlackout(MeatCutBlackoutDuration,
                                    () => _gameFlow.ShowQuestWarningDialogue(15));
                            }
                            break;

                        case "DataSubmissionPoint":
                            if (!_questProgress.TrySubmitData())
                            {
                                if (!_questProgress.DataSubmitted)
                                    _gameFlow.ShowQuestWarningDialogue(5);
                                else
                                    _gameFlow.ShowQuestWarningDialogue(11);
                            }
                            else
                            {
                                _gameFlow.ShowQuestWarningDialogue(11);
                            }
                            break;

                        case "LensMachine":
                        case "ScanMachine":
                        case "KeypadMachine":
                            TryStartMachine(objectName);
                            break;
                    }
                }
            }
        }

        private void ShowCompletedTaskDialogue()
        {
            int chapterIndex = _questProgress.DataSubmitted ? 11 : 7;
            _gameFlow.ShowQuestWarningDialogue(chapterIndex);
        }

        private void BeginSleepTransition()
        {
            BeginBlackout(SleepBlackoutDuration, () =>
            {
                if (_questProgress.TrySleep())
                    _questProgress.ShowNotice($"ตื่นแล้ว: Day {_questProgress.Day} และเริ่มภารกิจใหม่");
            });
        }

        private void BeginBlackout(float duration, Action afterBlackout = null)
        {
            if (_isBlackout)
                return;

            _isBlackout = true;
            _blackoutRemaining = duration;
            _afterBlackout = afterBlackout;
            _player.Freeze();
        }

        private static bool IsQuestWorkObject(string objectName)
        {
            return objectName == "LensMachine"
                || objectName == "ScanMachine"
                || objectName == "KeypadMachine"
                || objectName == "ScissorsItem"
                || objectName == "MonsterCutPoint"
                || objectName == "DataSubmissionPoint";
        }

        private void TryStartMachine(string machineName)
        {
            if (_questProgress.IsSanityDepleted && !_questProgress.DataSubmitted)
            {
                _gameFlow.ShowQuestWarningDialogue(9);
                return;
            }

            if (!_questProgress.IsStarted)
            {
                _questProgress.ShowDialogueMessage(_questProgress.GetMachineBlockReason(machineName));
                return;
            }

            if (machineName != "LensMachine" && !_questProgress.LensComplete)
            {
                _gameFlow.ShowQuestWarningDialogue(3);
                return;
            }

            if ((machineName == "ScanMachine" || machineName == "KeypadMachine")
                && !_questProgress.MeatCollected)
            {
                if (!_questProgress.HasScissors)
                    _gameFlow.ShowQuestWarningDialogue(4);
                else
                    _gameFlow.ShowQuestWarningDialogue(6);
                return;
            }

            if (!_questProgress.CanStartMachine(machineName))
            {
                bool alreadyCompleted = machineName switch
                {
                    "LensMachine" => _questProgress.LensComplete,
                    "ScanMachine" => _questProgress.ScanComplete,
                    "KeypadMachine" => _questProgress.KeypadComplete,
                    _ => false
                };

                if (alreadyCompleted)
                    ShowCompletedTaskDialogue();
                else
                    _questProgress.ShowDialogueMessage(_questProgress.GetMachineBlockReason(machineName));
                return;
            }

            Minigame minigame = machineName switch
            {
                "LensMachine" => new MinigameLens(_spriteBatch, _camera, _content, _audio),
                "ScanMachine" => new MinigameScan(_spriteBatch, _camera, _content, _audio),
                "KeypadMachine" => new MinigameKeypad(_spriteBatch, _camera, _content, _audio),
                _ => null
            };

            if (minigame == null)
                return;

            if (machineName == "LensMachine" && !_lensInstructionsShown)
            {
                _lensInstructionsShown = true;
                _gameFlow.ShowQuestWarningDialogue(13, () => ScheduleMinigameStart(machineName, minigame));
                return;
            }

            if (machineName == "ScanMachine")
            {
                _gameFlow.ShowQuestWarningDialogue(16, () => ScheduleMinigameStart(machineName, minigame));
                return;
            }

            if (machineName == "KeypadMachine")
            {
                _gameFlow.ShowQuestWarningDialogue(17, () => ScheduleMinigameStart(machineName, minigame));
                return;
            }

            StartMinigame(machineName, minigame);
        }

        private void StartMinigame(string machineName, Minigame minigame)
        {
            _activeMachineName = machineName;
            _lastMinigameResult = MinigameResult.None;
            _player.Freeze();
            _minigame.Start(minigame);
        }

        private void ScheduleMinigameStart(string machineName, Minigame minigame)
        {
            _pendingMinigameMachineName = machineName;
            _pendingMinigame = minigame;
            _minigameStartDelayRemaining = MinigameStartDelay;
            _player.Freeze();
        }

        private void OnMinigameEnded(Minigame minigame, MinigameResult result)
        {
            _lastMinigameResult = result;
            if (result == MinigameResult.Success)
            {
                _questProgress.CompleteMachine(_activeMachineName);

                if (_activeMachineName == "LensMachine" && !_lensPraiseShown)
                {
                    _lensPraiseShown = true;
                    ScheduleMinigameDialogue(14);
                }
                else if (_activeMachineName == "ScanMachine" || _activeMachineName == "KeypadMachine")
                {
                    ScheduleMinigameDialogue(18);
                }
            }

            if (result != MinigameResult.None)
                _questProgress.SpendSanity(10);

            _activeMachineName = null;
        }

        private void ScheduleMinigameDialogue(int chapterIndex)
        {
            _pendingMinigameDialogueChapter = chapterIndex;
            _minigameDialogueDelayRemaining = MinigameResultDialogueDelay;
            _player.Freeze();
        }

        private void CheckEscape()
        {
            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyPressed(Keys.Escape))
            {
                _gameState.StatePush(new StateMenu(_content, _gameState, _spriteBatch, _audio));
            }
        }
    }
}
