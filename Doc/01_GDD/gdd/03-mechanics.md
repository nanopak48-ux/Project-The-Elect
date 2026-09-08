---
type: gdd-mechanics
version: 0.1
date: [14/07/2026]
---
# Mechanic Design — [Movement]

## State Diagram

```mermaid
flowchart LR
	A[Idle] --> B{Interacting}
	B --> |FALSE| C{Hold W/A/S/D} 
	B --> |TRUE| A
	C --> |TRUE| D[Walk]
		C --> |FALSE| A
	G --> |FALSE| E{Hold LShift}
	E --> |TRUE| F[Run]
		E --> |FALSE| C
	D --> G{Release W/A/S/D}
	F --> H{Release LShift}
	H --> |FALSE| C
	G --> |TRUE| A
```

## Rules

| State | เข้าเงื่อนไข                        | ออกเงื่อนไข                     | Note           |
| ----- | ----------------------------------------------- | ------------------------------------------ | -------------- |
| Idle  | เริ่มเกม / หยุดเคลื่อนที่ | กด input ใดๆ                          | Animation loop |
| Move  | กดปุ่มทิศทาง                        | ปล่อยปุ่ม / กดปุ่ม Interact | Speed = 3 px   |
| Run   | กดปุ่มทิศทางพร้อม LShift       | ปล่อยปุ่ม / กดปุ่ม Interact | Speed = 5 px   |

# Mechanic Design — [Interact]

## State Diagram

```mermaid
flowchart LR
  A[Move/Run/Idle] --> B{Press E nearby interactable objects}
  B -->|TRUE| C[Interact]
  C -->A
  B -->|FALSE| A
```

## Rules

| State    | เข้าเงื่อนไข                                               | ออกเงื่อนไข                             | Note           |
| -------- | ---------------------------------------------------------------------- | -------------------------------------------------- | -------------- |
| Idle     | ตัวละครอยู่ในสถานะที่สามารถ interact ได้ | กด input ใดๆ                                  | Animation loop |
| Interact | กด E ใกล้วัตถุที่สามารถ interact ได้            | สิ้นสุด interact ของวัตถุนั้นๆ | Animation loop |
