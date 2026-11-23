# ACC DualSense Feedback

> **AI-generated content warning**
>
> This repository and its release notes were produced with AI assistance and may contain mistakes. Review the code, build artifacts and settings before use. Use at your own risk.

## v1.2.1.2 — Pedal resistance mode

Adds a selectable pedal-resistance mode on top of v1.2.1.1.

### Added

- A **Progressive / Constant light** switch to the right of the **Pedal resistance** header in Advanced settings.
- **Progressive** keeps the original `0x21` ten-zone curve. It is the default and is always used by the Default preset.
- **Constant light** uses a single `0x01` continuous resistance with an 8-bit force, so the full trigger travel can stay lighter than the minimum `0x21` zone step.
- The mode applies to both brake (L2) and throttle (R2) resting resistance. Event pulses remain independent.
- **Reset all** restores Progressive. The selected mode is saved in the profile.
- The startup confirmation pulse keeps the original progressive logic and does not follow the selected mode.
- Fixed the Simplified Chinese font subset for 渐/恒; the Constant-light switch track turns blue.

### Verification

- Full test harness: **176/176 checks passed**.
- UI render checks cover the switch, Default preset, Reset all and the blue off state.

### Downloads

- **English:** `ACCDualSenseFeedback-v1.2.1.2-win-x64-portable.zip`
- **简体中文:** `ACCDualSenseFeedback-v1.2.1.2-win-x64-zh-CN-portable.zip`

### SHA-256

```text
2be5fe45ec6357133e35b02324a9980b8abeb26feb43b5a07d125982877bd3f5  ACCDualSenseFeedback-v1.2.1.2-win-x64-portable.zip
276e05481b8f85200fd0945b09a1f236110f17b34319011c4576e687b2635c8f  ACCDualSenseFeedback-v1.2.1.2-win-x64-zh-CN-portable.zip
```

## v1.2.1.1 — Adjustable trigger and grip frequencies

Adds independently adjustable trigger and grip vibration frequencies while preserving the calibrated defaults from v1.2.1.

### Added

- **ABS pulse**, **wheel lock** and **upshift kick** expose separate **trigger vibration frequency** and **grip vibration frequency** sliders.
- **Redline pulse**, **TC pulse** and **wheelspin warning** expose a **trigger vibration frequency** slider.
- Every frequency slider uses the controller's raw **0–255** range. Setting a frequency to **0** disables that vibration layer while retaining the resting trigger resistance.
- Frequency rows support right-click reset to their calibrated defaults.
- The original fixed values remain the defaults: ABS 15/16, wheel lock 8/8, upshift 28/32, redline 18, TC 40 and wheelspin 7.
- Removed the explanatory captions below the frequency sliders.
- Updated the Simplified Chinese font subset so the new labels render at the correct weight.

### Verification

- Full test harness: **173/173 checks passed**.

### Downloads

- **English:** `ACCDualSenseFeedback-v1.2.1.1-win-x64-portable.zip`
- **简体中文:** `ACCDualSenseFeedback-v1.2.1.1-win-x64-zh-CN-portable.zip`

See the [GitHub Releases](https://github.com/CocaColor/ACC-DualSense-Feedback/releases) page for the portable packages and checksums.

## Setup

This is a portable Windows companion app for Assetto Corsa Competizione. Extract the ZIP before running the EXE, keep the bundled DLLs beside it, and use a USB-connected DualSense or DualSense Edge. See the release downloads for the full portable notes.
