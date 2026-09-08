---
type: workflow rule
version: 0.1
date: [2026-07-21]
team: [Vier Selec]
---
# Git Workflow Rules — [Vier Selec]

**เป้าหมาย:** ปรับ workflow ให้ "เร็วและปลอดภัยพอ" สำหรับ 48 ชม. — ตกลงกติกาให้ครบก่อนเริ่ม แล้วห้ามเปลี่ยนกลางทาง!

## Branching Model

- **โครงสร้าง Branch:** `main` (Production Ready) + `feature/<module>` (เช่น `feature/auth`, `feature/payment`)
- **อายุของ Feature Branch:** สั้นที่สุด ไม่เกิน **2–3 ชั่วโมง** ต่อกิ่ง (ทำทีละ sub-feature แล้วกด merge)
- **สิทธิ์ Merge เข้า `main`:** [เจ้าของ Module หรือ Tech Lead เท่านั้น]
- **คนดูแล `main` (Spot-checker):** [นพกร] คอยดูว่า `main` ยัง Build/Run ผ่านอยู่เสมอ

## Commit Convention

| Prefix        | ใช้เมื่อ                                                                                 | ตัวอย่าง                               |
| :------------ | :----------------------------------------------------------------------------------------------- | :--------------------------------------------- |
| `feat:`     | เพิ่ม feature/module ใหม่                                                               | `feat(auth): add google login button`        |
| `fix:`      | แก้ไข Bug                                                                                   | `fix(ui): resolve overflow on mobile screen` |
| `wip:`      | งานยังไม่เสร็จ แต่ต้อง Push สำรองไว้กันงานหาย              | `wip(api): draft user profile endpoint`      |
| `docs:`     | แก้ไขเอกสาร เช่น README, API Spec                                                 | `docs: update setup instruction`             |
| `refactor:` | ปรับปรุงโครงสร้างโค้ด (ไม่เพิ่มฟีเจอร์/ไม่แก้บั๊ก) | `refactor: simplify database query logic`    |
| `test:`     | เพิ่มหรือแก้ไข Test                                                                | `test: add unit test for checkout logic`     |

* **กฎการ Push:** Push งานขึ้น Remote อย่างน้อยทุก **1 ชั่วโมง** (หรือก่อนพัก/กินข้าว/นอน) โดยใช้ `wip:` เพื่อกันคอมพิวเตอร์พังแล้วงานหาย
* **กฎการ Pull:** รัน `git pull origin main` ทุกครั้งก่อนเริ่มเขียนโค้ดรอบใหม่ หรือก่อนสร้าง branch ใหม่

## Branch Protection ขั้นต่ำ

- **`main` ต้อง Build Pass และสั่ง Run ได้เสมอ** (เพราะอาจถูกเรียกตรวจหรือต้อง Demo/Submit ได้ตลอดเวลา)
- **ขั้นตอนก่อน Merge เข้า `main`:**
  1. สั่ง `git pull origin main` เข้ามาที่ Feature branch ของตัวเอง
  2. แก้ไข Conflict บน Local (ถ้ามี)
  3. ทดสอบรันแอปบนเครื่องตัวเองว่าไม่แตก/ไม่ crash
  4. สั่ง Merge เข้า `main` (ไม่อนุญาตให้ Merge โค้ดที่ยังมี Commit ขึ้นต้นด้วย `wip:`)

## Merge Conflict — เมื่อเกิดขึ้นกลางดึก

1. คนที่เจอ conflict ให้**แจ้งใน Group Chat ทันที** พร้อมระบุชื่อไฟล์ที่ตีกัน
2. **ถ้าเจ้าของไฟล์ออนไลน์:** ให้เจ้าของไฟล์เป็นตัดสินใจว่าใช้โค้ดชุดไหน
3. **ถ้าเจ้าของไฟล์ไม่อยู่ (หลับ/ไปพัก):** ผู้ตัดสินใจสำรองคือ **[นาย นพกร]**
4. **ห้ามแก้ conflict แบบเดาเองเด็ดขาด** — หากติดปัญหาและติดต่อใครไม่ได้ ให้รอได้ไม่เกิน **15 นาที** หลังจากนั้นให้ผู้ตัดสินใจสำรองสรุปและสั่งลุยต่อ
