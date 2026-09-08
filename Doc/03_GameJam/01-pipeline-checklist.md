---
type: pipeline checklist
version: 0.1
date: [2026-07-21]
team: [Vier Selec]
---
# Pipeline Function Checklist — [Project The Elect]

> เกมทุกแนวต้องมี pipeline ขั้นต่ำนี้ก่อนถึงจะเรียกว่า "เล่นได้" — เติมชื่อผู้รับผิดชอบและติ๊กสถานะระหว่างลงมือทำจริง เพิ่มแถวได้ถ้าเกมของทีมต้องการ module อื่น

## Must-Have (ต้องมีก่อน Hour 24 — Playable Build)

| Module (ตาม MonoGame Game Loop)             | หน้าที่                                                            | สถานะ     | ผู้รับผิดชอบ |
| ---------------------------------------------- | ------------------------------------------------------------------------- | -------------- | ------------------------ |
| `GameStateManager`                           | สลับ state (Menu / Playing / Pause / GameOver)                        | 🔲 Not Started | Johnny                   |
| `InputManager`                               | รับ input keyboard/gamepad แบบรวมศูนย์                      | 🔲 Not Started | Johnny                   |
| Core `Update()` logic ของกลไกหลัก | logic ของ core mechanic 1 อย่างที่เป็นหัวใจเกม     | 🔲 Not Started | Johnny                   |
| Collision / interaction พื้นฐาน         | ตรวจชน/ตรวจ trigger ระหว่าง entity                       | 🔲 Not Started | Johnny                   |
| `SpriteBatch` render + camera/viewport       | วาดผ่าน `GraphicsDevice.Viewport` (ห้าม hardcode resolution) | 🔲 Not Started | Johnny                   |
| Win / Lose condition                           | เงื่อนไขจบเกม/จบด่าน                                   | 🔲 Not Started | Johnny                   |
| Content pipeline (MGCB)                        | โหลด asset ผ่าน `Content.Load<T>()` เท่านั้น            | 🔲 Not Started | Johnny                   |

## Nice-to-Have (ทำถ้าเหลือเวลา — Hour 24–34)

| Module                                | หน้าที่                           | สถานะ     | ผู้รับผิดชอบ |
| ------------------------------------- | ---------------------------------------- | -------------- | ------------------------ |
| `AudioManager` (SFX พื้นฐาน) | เสียงตอบสนอง action หลัก | 🔲 Not Started | Pub g                    |
| UI/HUD (score, timer)                 | แสดงสถานะระหว่างเล่น | 🔲 Not Started | Pub g                    |
| Background music                      | เพลงระหว่างในเกม         | 🔲 Not Started | Kaowniew                 |
| Fonts                                 | ฟ้อนตัวอักษรในเกม       | 🔲 Not Started | Kaowniew                 |

## Cut-List (ตัดทิ้งก่อนถ้าเวลาไม่พอ — ห้ามเริ่มก่อน Must-Have เสร็จ)

- ❌ Save/Load
- ❌ Settings menu
- ❌ Leaderboard
- ❌ Physics Mechanic

> เมื่อถึง **Feature Freeze (Hour 34)** ทุก module ในตารางนี้ต้องมีสถานะ ✅ Done หรือถูกย้ายไป Cut-List อย่างชัดเจน — ห้ามค้างเป็น 🔲/🟡
