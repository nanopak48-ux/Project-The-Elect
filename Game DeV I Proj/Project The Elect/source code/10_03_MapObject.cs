using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps;

namespace Project_The_Elect.source_code
{
    public class InteractableObject
    {
        public string Name { get; }

        /// <summary>ขอบจริงของ object ใน Tiled (ใช้วาดดีบั๊ก)</summary>
        public RectangleF Bounds { get; }

        /// <summary>ขอบที่ขยายออกแล้ว ใช้เช็กว่าผู้เล่น "อยู่ใกล้พอ" จะกด E ได้</summary>
        public RectangleF TriggerZone { get; }

        public Vector2 Center { get; }

        public InteractableObject(string name, float x, float y, float width, float height, float range)
        {
            Name = name;
            Bounds = new RectangleF(x, y, width, height);

            TriggerZone = new RectangleF(
                x - range,
                y - range,
                width + (range * 2f),
                height + (range * 2f)
            );

            Center = new Vector2(x + width / 2f, y + height / 2f);
        }
    }

    public class InteractManager
    {
        // ชื่อ Object Layer ใน Tiled (ต้องตรงทุกตัวอักษร)
        private const string LayerName = "Interact_Obj";

        private readonly Tilemap _map;
        private readonly float _interactRange;

        public List<InteractableObject> InteractObjects { get; private set; }
        public InteractableObject CurrentNearbyObject { get; private set; }

        /// <param name="interactRange">
        /// ระยะที่ขยายออกจากขอบ object (พิกเซล) เพราะเครื่องส่วนใหญ่ถูกกำแพงกันไว้
        /// ผู้เล่นจึงทับตัว object ตรงๆ ไม่ได้ ต้องมีระยะเผื่อให้ยืนข้างๆ แล้วกด E ได้
        /// </param>
        public InteractManager(Tilemap map, float interactRange = 13f)
        {
            _map = map;
            _interactRange = interactRange;
            InteractObjects = new List<InteractableObject>();
            LoadInteractObjects();
        }

        private void LoadInteractObjects()
        {
            InteractObjects.Clear();
            bool layerFound = false;

            foreach (var layer in _map.Layers)
            {
                if (layer is not TilemapObjectLayer objectLayer) continue;
                if (!IsInteractLayer(objectLayer.Name)) continue;

                layerFound = true;

                foreach (var obj in objectLayer.Objects)
                {
                    float x = obj.Bounds.Min.X;
                    float y = obj.Bounds.Min.Y;
                    float width = obj.Bounds.Max.X - obj.Bounds.Min.X;
                    float height = obj.Bounds.Max.Y - obj.Bounds.Min.Y;

                    string name = (obj.Name ?? "").Trim();

                    if (name.Length == 0)
                    {
                        Console.WriteLine($"[Interact Warning] Skipped unnamed object at ({x}, {y}). Please set 'Name' in Tiled Properties.");
                        continue;
                    }

                    InteractObjects.Add(new InteractableObject(name, x, y, width, height, _interactRange));
                }
            }

            if (!layerFound)
            {
                Console.WriteLine($"[Interact] ไม่พบ Object Layer ชื่อ '{LayerName}' ในแมพ " +
                                  "(เช็กชื่อ และเช็กว่าไม่ได้อยู่ใน Group layer)");
            }
            else
            {
                string names = string.Join(", ", InteractObjects.ConvertAll(o => o.Name));
                Console.WriteLine($"[Interact] โหลด {InteractObjects.Count} object: {names}");
            }
        }

        private static bool IsInteractLayer(string layerName)
        {
            if (string.IsNullOrEmpty(layerName)) return false;
            return layerName == LayerName || layerName.EndsWith("/" + LayerName);
        }

        /// <summary>เรียกหลัง Player.Update ในแต่ละเฟรม เพื่อใช้ HitBox ล่าสุด</summary>
        public void Update(Player player)
        {
            CurrentNearbyObject = null;

            RectangleF playerBox = player.HitBox;
            float nearest = float.MaxValue;

            foreach (var obj in InteractObjects)
            {
                if (!playerBox.Intersects(obj.TriggerZone)) continue;

                float distance = Vector2.DistanceSquared(player.Position, obj.Center);
                if (distance < nearest)
                {
                    nearest = distance;
                    CurrentNearbyObject = obj;
                }
            }
        }

        public void RemoveObject(InteractableObject interactableObject)
        {
            if (interactableObject == null)
                return;

            InteractObjects.Remove(interactableObject);
            if (CurrentNearbyObject == interactableObject)
                CurrentNearbyObject = null;
        }
        //====================================================================//
        public void DrawDebug(SpriteBatch spriteBatch)
        {
            foreach (var obj in InteractObjects)
            {
                //Color drawColor = (obj == CurrentNearbyObject) ? Color.Lime : Color.Yellow;
                //spriteBatch.DrawRectangle(obj.TriggerZone, Color.Cyan * 0.5f, 1f);
                //spriteBatch.DrawRectangle(obj.Bounds, drawColor, 2f);
            }
        }
        //====================================================================//
    }
}
