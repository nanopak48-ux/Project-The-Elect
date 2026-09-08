---
type: doc-index
version: 0.1
date: [วันที่]
---
# [ชื่อเกม] — Documentation Index

## 📖 เอกสารในโปรเจกต์นี้

| ไฟล์                                | เนื้อหา                   | สถานะ |
| --------------------------------------- | -------------------------------- | ---------- |
| [00-concept.md](00-concept.md)             | Game concept, core loop, scope   | ✅         |
| [01-mechanics.md](01-mechanics.md)         | Mechanic flow (state diagram)    | ✅         |
| [02-asset-list.md](02-asset-list.md)       | Asset list + asset pipeline flow | ✅         |
| [03-class-diagram.md](03-class-diagram.md) | Class diagram เบื้องต้น | ✅         |

## 🏷️ Naming Convention

**Asset:** ดูตารางเต็มใน [00-concept.md](00-concept.md#asset-naming-convention)

| Prefix   | ประเภท     |
| -------- | ---------------- |
| `spr_` | Sprite / Texture |
| `sfx_` | Sound Effect     |
| `bgm_` | Background Music |
| `fnt_` | Font             |
| `dat_` | Data / Config    |

**เอกสาร:** ไฟล์ใน `docs/01_GDD/` เรียงลำดับด้วย prefix ตัวเลข 2 หลัก (`00-`, `01-`, ...) ตามลำดับที่สร้างขึ้นในแต่ละ Lab — ห้ามสลับเลขไฟล์ที่มีอยู่แล้ว เพิ่มไฟล์ใหม่ให้ต่อเลขถัดไป

## Naming Convention

| Prefix | Detail           | Example                |
| :----: | ---------------- | ---------------------- |
|  bgm_  | Background music | bgm_home_normal.mp3    |
|  sfx_  | Sound Effect     | sfx_game_walk.mp3      |
|  spr_  | Sprite Sheet     | spr_mc_idle.png        |
|  fnt_  | Font             | fnt_(font name).(font) |
|  tex_  | Texture          | tex_button.png         |

## Group Task

| Detail           | Prefix | Name      | Folder Staging                          |
| ---------------- | ------ | --------- | --------------------------------------- |
| Background music | bgm_   | 682110152 | 02_Assets\\_candidates\candidate_bgm.md |
| Sound Effect     | sfx_   | 682110147 | 02_Assets\\_candidates\candidate_sfx.md |
| Sprite Sheet     | spr_   | 682110115 | 02_Assets\\_candidates\candidate_spr.md |
| Font             | fnt_   | 682210152 | 02_Assets\\_candidates\candidate_fnt.md |
| Texture          | tex_   | 682110115 | 02_Assets\\_candidates\candidate_tex.md |
