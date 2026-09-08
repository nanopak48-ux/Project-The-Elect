## Asset Pipeline Flow

```mermaid
flowchart TD
    subgraph SOURCES["แหล่ง Asset ฟรี"]
        K1[OpenGameArt / itch.io] --> C1[docs/02_Assets/_candidates/]
        K2[Freesound.org] --> C1
        K3[itch.io] --> C1
	K4[dafont.com] --> C1
    end

    subgraph REPO["Git Repository (Staging)"]
        C1 --> R1[docs/02_Assets/_candidates/candidate_spr]
        C1 --> R2[docs/02_Assets/_candidates/candidate_sfx]
        C1 --> R3[docs/02_Assets/_candidates/candidate_bgm]
        C1 --> R4[docs/02_Assets/_candidates/candidate_fnt]
	C1 --> R5[docs/02_Assets/_candidates/candidate_tex]
    end

    subgraph PIPELINE["MonoGame Content Pipeline (Lab ถัดไป)"]
        R1 --> CP[คัดเลือก แล้วย้ายเข้า Content/]
        R2 --> CP
        R3 --> CP
        R4 --> CP
        CP --> P1[Content Builder / MGCB]
        P1 --> XNB[.xnb files]
    end

    XNB --> GAME[Runtime Game]
```
