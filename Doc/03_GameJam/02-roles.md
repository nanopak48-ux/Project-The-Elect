---
type: jam-roles
version: 0.1
date: [21/07/2026]
team: [Vier Select]
---
# Team Roles & Pipeline Ownership — [Vier Select]

## แบ่งความรับผิดชอบตาม Pipeline (4 คน)

| คน | ชื่อ | Module ที่รับผิดชอบ         | Phase ที่ต้องเสร็จ |
| ---- | -------- | --------------------------------------- | ------------------------------ |
| 1    | Johnny   | `GameStateManager` + `InputManager` | Phase 2 (Hour 6–14)           |
| 2    | Johnny   | Core mechanic `Update()` logic        | Phase 2–3 (Hour 6–24)        |
| 3    | Johnny   | Render/`SpriteBatch` + Collision      | Phase 2–3 (Hour 6–24)        |
| 4    | Plub     | Content pipeline (MGCB) + Audio/UI      | Phase 3–4 (Hour 14–34)       |

> คนละไฟล์/module = ชนกันน้อยที่สุด ปรับ module ตามตาราง [01-pipeline-checklist.md](01-pipeline-checklist.md) ให้ตรงกับเกมจริงของทีม

## File Ownership Rule

> ไฟล์ที่ต้องแตะร่วมกันบ่อย เช่น `Game1.cs`, ไฟล์ scene หลัก — ตกลงล่วงหน้าว่าใครแก้ก่อน-หลัง เพื่อลด merge conflict

| ไฟล์/พื้นที่ที่เสี่ยงชนกัน | เจ้าของหลัก | กติกาการแก้ไข |
| --------------------------------------------------- | ---------------------- | :------------------------: |
| `Game1.cs`                                        | Johnny                 |             -             |

## เมื่อคนใดคนหนึ่งเสร็จงานตัวเองก่อน

- ลำดับความช่วยเหลือถัดไป: [เช่น ไปช่วย QA/playtest → ไปช่วย content → ไปช่วย polish]
- [กติกาเพิ่มเติมของทีม]
