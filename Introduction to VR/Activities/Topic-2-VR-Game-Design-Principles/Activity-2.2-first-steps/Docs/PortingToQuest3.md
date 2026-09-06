# Porting an activity to Meta Quest 3

Every activity is built desktop-first with the XR Interaction Simulator. Once it works on your laptop, push it to the headset. The steps are the same for every activity, so this page is included in each project.

## One-time setup (per machine)

1. **Unity Android modules.** In Unity Hub → Installs → your Unity 6.5 install → Add modules → check *Android Build Support*, *OpenJDK*, and *Android SDK & NDK Tools*.
2. **Developer mode on the headset.** In the Meta Horizon mobile app: Menu → Devices → your Quest 3 → Headset Settings → Developer Mode → On. You need a (free) Meta developer account or to be part of a developer organization.
3. **Cable + trust.** Connect the Quest with a USB-C data cable. Put the headset on and accept *Allow USB debugging* and *Allow access to data*. (On Windows you may need the Oculus ADB driver from the Meta developer site.)
4. **Verify adb sees the device.** Terminal / PowerShell:
   `adb devices` → your headset serial should appear with `device` next to it. The `adb` binary lives inside the Unity install under `.../PlaybackEngines/AndroidPlayer/SDK/platform-tools/`.

## Per-project settings (about 5 minutes)

1. **Switch platform.** File → Build Profiles → Android → *Switch Platform*.
2. **XR Plug-in Management.** Edit → Project Settings → XR Plug-in Management → Android tab → check **OpenXR**.
3. **OpenXR features.** Still in Project Settings → XR Plug-in Management → OpenXR → Android tab:
   - Under *OpenXR Feature Groups* enable **Meta Quest Support**.
   - Under *Enabled Interaction Profiles* add **Oculus Touch Controller Profile** (add *Meta Quest Touch Plus Controller Profile* too if it is listed).
   - Render Mode: *Multi-pass* is safest to start; *Single Pass Instanced* is faster once everything works.
4. **Player settings.** Edit → Project Settings → Player → Android tab:
   - Other Settings → *Minimum API Level*: Android 10 (API 29) or higher; *Target API Level*: highest installed.
   - Other Settings → *Scripting Backend*: IL2CPP; *Target Architectures*: ARM64 only.
   - Other Settings → *Graphics APIs*: Vulkan first (remove OpenGLES3 if Vulkan works for you).
   - Other Settings → *Texture compression format*: ASTC.
   - Resolution and Presentation → *Default Orientation*: Landscape Left.
5. **Quality.** Project Settings → Quality: pick a lighter tier for Android (no real-time shadows on many lights, MSAA 4x is fine, anisotropic off).
6. **Remove the simulator from the scene** or disable its GameObject. It is for desktop only.
7. **Build and Run.** File → Build Profiles → Android → *Build And Run*. The first build takes several minutes (IL2CPP). The app installs into *Library → Unknown Sources* on the headset.

## Frame budget reminder

The Quest 3 refreshes at 72 Hz by default (90 and 120 Hz optional). Your whole frame — CPU and GPU — must fit in

- 72 Hz → 13.9 ms
- 90 Hz → 11.1 ms
- 120 Hz → 8.3 ms

If you miss the budget the runtime reprojects the previous frame and the world judders. Check the *Stats* overlay and the Profiler (Window → Analysis → Profiler, attach to the Android player) before and after changes.

## Common problems

| Symptom | Likely cause / fix |
|---|---|
| Build succeeds but headset shows a black screen | OpenXR not enabled on the Android tab, or Meta Quest Support feature not checked |
| Controllers do not track | Oculus Touch Controller Profile not added under Enabled Interaction Profiles |
| "adb: no devices" | Cable is charge-only, USB debugging prompt not accepted, or developer mode off |
| App runs but everything is tiny or huge | Objects were scaled; keep 1 Unity unit = 1 meter and put the XR Origin at floor level with Tracking Origin Mode = Floor |
| Blurry or shimmering | Texture compression not ASTC, or too many transparent overlapping objects |

Created by Isac Artzi
