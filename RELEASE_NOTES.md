# ACC DualSense Feedback 1.2.1.1
##  Added

- **ABS pulse**, **wheel lock** and **upshift kick** now expose separate **trigger vibration frequency** and **grip vibration frequency** sliders.
- **Redline pulse**, **TC pulse** and **wheelspin warning** expose a **trigger vibration frequency** slider.
- Every frequency slider uses the controller's raw **0-255** range. Setting a frequency to **0** disables that vibration layer while retaining the resting trigger resistance.
- Right-clicking a frequency row restores its calibrated default.
- Updated the Simplified Chinese font subset so the new labels render at the same weight as the rest of the UI.

##  Defaults

| Control | Trigger | Grip |
| --- | ---: | ---: |
| ABS pulse | 15 | 16 |
| Wheel lock | 8 | 8 |
| Upshift kick | 28 | 32 |
| Redline pulse | 18 | - |
| TC pulse | 40 | - |
| Wheelspin warning | 7 | - |

ACC detection, trigger resistance, gyro steering, input mapping and lighting logic are unchanged from v1.2.1.
