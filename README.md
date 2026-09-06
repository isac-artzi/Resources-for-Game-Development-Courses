# Resources for Game Development Courses

A collection of teaching resources for game development courses. Each course folder groups
the lecture notes, in-class activities, and tutorials for one subject area.

📖 **Read the lecture notes online:** <https://isac-artzi.github.io/Resources-for-Game-Development-Courses/>

| | What it is |
|---|---|
| 🥽 **[Introduction to VR](#introduction-to-vr)** | Seven **interactive HTML lecture-note topics**, **30 Unity project skeletons** (one per class meeting), and a **Git/GitHub tutorial** for student VR teams. |

> A companion repository, **Resources for AI Courses**, holds the same kind of material for
> machine learning, deep learning, NLP, and reinforcement learning.

---

## Introduction to VR

A 15-week introduction to virtual reality and game development in **Unity 6.5 (6000.5.x)**
targeting **Meta Quest 3**. The whole course is built around one running project —
*Realm of Legends*, a VR adventure quest game — that grows by one milestone per topic, and every
in-class activity is a stepping stone toward the milestone due in that topic.

### 📓 [Lecture Notes](./Introduction%20to%20VR/Lecture%20Notes)

Seven interactive topics plus a landing page. Each file is **fully self-contained** — CSS and
JavaScript inlined, no CDN, no fonts, no images — so it works from a local download, from GitHub
Pages, or from any static host, online or offline.

- One tab per class session (Tuesday / Thursday), linkable: `topic-3.html#s2` opens session 2.
- Interactive figures (marked ▶) are Canvas 2D visualizations driven by sliders, with formulas
  that update live as you move the parameters.
- “Check yourself” questions give immediate feedback. Nothing is transmitted or stored.
- Topic 6 optionally uses the browser's built-in Web Audio API.

| Topic | Weeks | Sessions |
|---|---|---|
| [1 · Introduction to Virtual Reality](./Introduction%20to%20VR/Lecture%20Notes/topic-1.html) | 1–2 | 4 |
| [2 · VR Game Design Principles on a Variety of Platforms](./Introduction%20to%20VR/Lecture%20Notes/topic-2.html) | 3–4 | 4 |
| [3 · 3D Modeling and VR Environments](./Introduction%20to%20VR/Lecture%20Notes/topic-3.html) | 5–6 | 4 |
| [4 · VR Interaction and User Interfaces](./Introduction%20to%20VR/Lecture%20Notes/topic-4.html) | 7–8 | 4 |
| [5 · VR Locomotion and Movement](./Introduction%20to%20VR/Lecture%20Notes/topic-5.html) | 9–10 | 4 |
| [6 · VR Audio and Sound Design](./Introduction%20to%20VR/Lecture%20Notes/topic-6.html) | 11–12 | 4 |
| [7 · VR Game Development Project](./Introduction%20to%20VR/Lecture%20Notes/topic-7.html) | 13–15 | 6 |

> **[→ Read these online](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/introduction-to-vr/)** — published from this folder by the
> [Pages workflow](./.github/workflows/pages.yml). HTML files render as source on github.com,
> so to read them from the repository instead, clone or download the folder and open
> `index.html` locally.

### 🎮 [Activities](./Introduction%20to%20VR/Activities)

Thirty small Unity projects, one per class meeting. Each folder is a **complete Unity 6.5 project
skeleton**: open it in Unity Hub, import the two XR Interaction Toolkit samples, run
**Activity > Build Starter Scene**, read the project's `README.md`, and implement the numbered
TODOs in `Assets/Scripts/`.

| Topic | Activities |
|---|---|
| [1 · Introduction to VR](./Introduction%20to%20VR/Activities/Topic-1-Introduction-to-VR) | Hello Elaria · Mock View Studio · Story Beats in Space · Greybox Elaria |
| [2 · VR Game Design Principles](./Introduction%20to%20VR/Activities/Topic-2-VR-Game-Design-Principles) | Grab the Shard · First Steps · Menus in Space · Build to Quest and Measure |
| [3 · 3D Modeling and VR Environments](./Introduction%20to%20VR/Activities/Topic-3-3D-Modeling-and-VR-Environments) | Plant the Forest · Mountain Pass · Make It Fast · Ancient Ruins |
| [4 · VR Interaction and UI](./Introduction%20to%20VR/Activities/Topic-4-VR-Interaction-and-UI) | Inventory · Speak with the Sage · Runestone Puzzle · Usability Lab |
| [5 · VR Locomotion and Movement](./Introduction%20to%20VR/Activities/Topic-5-VR-Locomotion-and-Movement) | Teleport Deep Dive · Smooth and Turn · Dash and Blink · Comfort Settings |
| [6 · VR Audio and Sound Design](./Introduction%20to%20VR/Activities/Topic-6-VR-Audio-and-Sound-Design) | Hear the Forest · Soundscapes · Sound of Touch · Dark Mode |
| [7 · VR Game Development Project](./Introduction%20to%20VR/Activities/Topic-7-VR-Game-Development-Project) | Portals and Scenes · Quests and Saves · Playtest Telemetry · Juice · Performance Pass · Demo Kit |

Keep each activity in its own Unity project — every folder defines its own `ActivitySceneBuilder`
and some script names repeat across activities, so merging two activities' `Assets/` into one
project produces duplicate-class compile errors. Free assets, where an activity needs them, are
listed in that activity's `Docs/FreeAssets.md`; porting notes are in `Docs/PortingToQuest3.md`.

[→ Full activity index with scripts and stepping stones](./Introduction%20to%20VR/Activities/README.md)

### 🧰 [Tutorials](./Introduction%20to%20VR/Tutorials)

**GitHub for Unity VR Teams** — a beginner's guide to the parts of Git and GitHub a small team
needs to share a Unity project: setting up, cloning, committing, pushing, pulling, branching, pull
requests, and fixing the merge conflicts that Unity projects tend to create. Assumes no prior Git
experience; Windows (PowerShell) and macOS (Terminal) commands side by side.

---

## Requirements

Unity 6.5 (6000.5.x) with Android Build Support · XR Interaction Toolkit 3.5 · OpenXR 1.17 ·
Visual Studio Code · a Meta Quest 3 for porting. Packages resolve automatically on first open of
an activity. The lecture notes need only a browser.

Created by Isac Artzi.
