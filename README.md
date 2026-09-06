# Resources for Game Development Courses

### ▶ [Read the lecture notes online](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/)

[![Advanced Game Testing and Debugging](https://img.shields.io/badge/Open-Advanced%20Game%20Testing%20and%20Debugging-1f6feb?style=for-the-badge)](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/advanced-game-testing-and-debugging/)
[![Introduction to VR](https://img.shields.io/badge/Open-Introduction%20to%20VR-3b4fd8?style=for-the-badge)](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/introduction-to-vr/)

A collection of teaching resources for game development courses. Each course folder groups
the lecture notes, in-class activities, and tutorials for one subject area.

> **The lecture notes are HTML, and github.com displays HTML as source code.** Open them through
> the links above (or clone a folder and open `index.html` locally) — clicking an `.html` file in
> the repository file browser shows you markup, not the page. Markdown files, including the
> activity briefs, render here normally.

| | What it is |
|---|---|
| 🥽 **[Introduction to VR](#introduction-to-vr)** | Seven **interactive HTML lecture-note topics**, **30 Unity project skeletons** (one per class meeting), and a **Git/GitHub tutorial** for student VR teams. |
| 🧪 **[Advanced Game Testing and Debugging](#advanced-game-testing-and-debugging)** | A **course hub** with a 30-session calendar, **eight interactive lessons** (Unity and Unreal side by side), and **30 activity briefs**. |

> A companion repository, **Resources for AI Courses**, holds the same kind of material for
> machine learning, deep learning, NLP, and reinforcement learning.

---

## Introduction to VR

A 15-week introduction to virtual reality and game development in **Unity 6.5 (6000.5.x)**
targeting **Meta Quest 3**. The whole course is built around one running project —
*Realm of Legends*, a VR adventure quest game — that grows by one milestone per topic, and every
in-class activity is a stepping stone toward the milestone due in that topic.

### 📓 [Lecture Notes](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/introduction-to-vr/)

*Live pages. The [source files](./Introduction%20to%20VR/Lecture%20Notes) are here in the repo.*

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
| [1 · Introduction to Virtual Reality](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/introduction-to-vr/topic-1.html) | 1–2 | 4 |
| [2 · VR Game Design Principles on a Variety of Platforms](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/introduction-to-vr/topic-2.html) | 3–4 | 4 |
| [3 · 3D Modeling and VR Environments](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/introduction-to-vr/topic-3.html) | 5–6 | 4 |
| [4 · VR Interaction and User Interfaces](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/introduction-to-vr/topic-4.html) | 7–8 | 4 |
| [5 · VR Locomotion and Movement](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/introduction-to-vr/topic-5.html) | 9–10 | 4 |
| [6 · VR Audio and Sound Design](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/introduction-to-vr/topic-6.html) | 11–12 | 4 |
| [7 · VR Game Development Project](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/introduction-to-vr/topic-7.html) | 13–15 | 6 |


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

### Requirements

Unity 6.5 (6000.5.x) with Android Build Support · XR Interaction Toolkit 3.5 · OpenXR 1.17 ·
Visual Studio Code · a Meta Quest 3 for porting. Packages resolve automatically on first open of
an activity. The lecture notes need only a browser.

---

## Advanced Game Testing and Debugging

A 15-week course on finding, reproducing, and fixing what ships broken: bug taxonomy and triage,
test design, automated testing, performance profiling, optimization, memory management, ethics and
compliance, and a final applied project on mobile. Engine-specific material is written twice —
a **Unity** pane and an **Unreal** pane — so the course does not assume one engine.

### 📓 [Lecture Notes](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/advanced-game-testing-and-debugging/)

*Live pages. The [source files](./Advanced%20Game%20Testing%20and%20Debugging/Lecture%20Notes) are here in the repo.*

A course hub plus eight interactive lessons. Like the VR notes, every file is **fully
self-contained** — no CDN, no network, no accounts — so a lesson runs from a local download or
projected in a classroom with no connection.

- The hub carries topic cards, the 30-session calendar, due dates, and a “Where am I?” date picker
  that reports which session you are in and what is due next.
- Lessons carry runnable simulators, annotated code walkthroughs where clicking a line explains
  *why* it is written that way, a diagnostic decision tree, and knowledge checks that explain why
  the wrong answers are tempting.

| Topic | Lesson | Dates |
|---|---|---|
| 1 | [Understanding Bugs](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/advanced-game-testing-and-debugging/lessons/lesson-01-understanding-bugs.html) | Sep 8–13 |
| 2 | [Test Design and Diagramming](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/advanced-game-testing-and-debugging/lessons/lesson-02-test-design-diagramming.html) | Sep 14–27 |
| 3 | [Automated Testing Strategies](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/advanced-game-testing-and-debugging/lessons/lesson-03-automated-testing.html) | Sep 28 – Oct 11 |
| 4 | [Advanced Performance Profiling](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/advanced-game-testing-and-debugging/lessons/lesson-04-performance-profiling.html) | Oct 12–25 |
| 5 | [Advanced Optimization Techniques](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/advanced-game-testing-and-debugging/lessons/lesson-05-optimization-techniques.html) | Oct 26 – Nov 8 |
| 6 | [Memory Management in Games](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/advanced-game-testing-and-debugging/lessons/lesson-06-memory-management.html) | Nov 9–22 |
| 7 | [Ethics and Compliance in Testing](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/advanced-game-testing-and-debugging/lessons/lesson-07-ethics-compliance.html) | Nov 23 – Dec 6 |
| 8 | [Applied Testing Project](https://isac-artzi.github.io/Resources-for-Game-Development-Courses/advanced-game-testing-and-debugging/lessons/lesson-08-applied-mobile-project.html) | Dec 7–20 |


### 🧪 [Activities](./Advanced%20Game%20Testing%20and%20Debugging/Activities)

Thirty activity briefs, two per week, one per class meeting. Each is scoped to 35–60 minutes and
produces an artifact that feeds the graded assignment for its topic. They are markdown rather than
web pages because they are meant to be read in VS Code beside the project under test, and none of
them requires a particular engine build — your own project, a provided sample, or any game you can
currently run will do.

[→ Full activity index](./Advanced%20Game%20Testing%20and%20Debugging/Activities/README.md)

### Requirements

Unity 3D · Unreal Engine · Visual Studio Code. The lessons themselves need only a browser.

---

Created by Isac Artzi.
