using System.Diagnostics;
using ACCDualSenseFeedback.Haptics;
using ACCDualSenseFeedback.Telemetry;

internal static class FrequencySettingsChecks
{
    private static long At(int ms) => (long)((1 + ms / 1000.0) * Stopwatch.Frequency);

    private static void Assert(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    private static TelemetrySnapshot TcCar() => new()
    {
        PacketId = 101, ObservedTimestamp = At(0), TcIntervention = .02f,
        Gas = 1, Gear = 4, Rpm = 5000, CurrentMaxRpm = 8000, SpeedKmh = 100, IsEngineRunning = 1
    };

    public static void Run(Action<string, Action> check)
    {
        check("Frequency defaults preserve the calibrated values and clamp to slider ranges", () =>
        {
            FeedbackProfile profile = FeedbackProfile.Default;
            Assert(profile.AbsPulseTriggerFrequency == 15 && profile.AbsPulseGripFrequency == 16, "ABS defaults changed");
            Assert(profile.LockPulseTriggerFrequency == 8 && profile.LockPulseGripFrequency == 8, "lock defaults changed");
            Assert(profile.ShiftKickTriggerFrequency == 28 && profile.ShiftKickGripFrequency == 32, "shift defaults changed");
            Assert(profile.RedlinePulseFrequency == 18, "redline default changed");
            Assert(profile.TcPulseFrequency == 40, "TC default changed");
            Assert(profile.WheelspinPulseFrequency == 7, "wheelspin default changed");

            FeedbackTuning neutral = (profile with { ActivePreset = FeedbackPreset.Custom }).ToTuning();
            Assert(neutral.AbsPulseTriggerFrequency == 15 && neutral.AbsPulseGripFrequency == 16, "ABS tuning defaults changed");
            Assert(neutral.LockPulseTriggerFrequency == 8 && neutral.LockPulseGripFrequency == 8, "lock tuning defaults changed");
            Assert(neutral.ShiftKickTriggerFrequency == 28 && neutral.ShiftKickGripFrequency == 32, "shift tuning defaults changed");
            Assert(neutral.RedlinePulseFrequency == 18 && neutral.TcPulseFrequency == 40 &&
                neutral.WheelspinPulseFrequency == 7, "right trigger tuning defaults changed");

            FeedbackProfile clamped = (profile with
            {
                ActivePreset = FeedbackPreset.Custom,
                AbsPulseTriggerFrequency = 0,
                AbsPulseGripFrequency = 999,
                LockPulseTriggerFrequency = 999,
                LockPulseGripFrequency = 0,
                ShiftKickTriggerFrequency = -4,
                ShiftKickGripFrequency = 999,
                RedlinePulseFrequency = 999,
                TcPulseFrequency = 0,
                WheelspinPulseFrequency = 999
            }).Sanitize();
            Assert(clamped.AbsPulseTriggerFrequency == 0 && clamped.AbsPulseGripFrequency == 255,
                "ABS slider range was not enforced");
            Assert(clamped.LockPulseTriggerFrequency == 255 && clamped.LockPulseGripFrequency == 0,
                "lock slider range was not enforced");
            Assert(clamped.ShiftKickTriggerFrequency == 0 && clamped.ShiftKickGripFrequency == 255,
                "shift slider range was not enforced");
            Assert(clamped.RedlinePulseFrequency == 255 && clamped.TcPulseFrequency == 0 &&
                clamped.WheelspinPulseFrequency == 255, "right trigger slider range was not enforced");
        });

        check("Custom frequency profile values reach FeedbackTuning unchanged", () =>
        {
            var profile = FeedbackProfile.Default with
            {
                ActivePreset = FeedbackPreset.Custom,
                AbsPulseTriggerFrequency = 33,
                AbsPulseGripFrequency = 120,
                LockPulseTriggerFrequency = 27,
                LockPulseGripFrequency = 75,
                ShiftKickTriggerFrequency = 39,
                ShiftKickGripFrequency = 120,
                RedlinePulseFrequency = 21,
                TcPulseFrequency = 35,
                WheelspinPulseFrequency = 12
            };
            FeedbackTuning tuning = profile.ToTuning();
            Assert(tuning.AbsPulseTriggerFrequency == 33 && tuning.AbsPulseGripFrequency == 120, "ABS custom frequencies lost");
            Assert(tuning.LockPulseTriggerFrequency == 27 && tuning.LockPulseGripFrequency == 75, "lock custom frequencies lost");
            Assert(tuning.ShiftKickTriggerFrequency == 39 && tuning.ShiftKickGripFrequency == 120, "shift custom frequencies lost");
            Assert(tuning.RedlinePulseFrequency == 21 && tuning.TcPulseFrequency == 35 &&
                tuning.WheelspinPulseFrequency == 12, "right trigger custom frequencies lost");
        });

        check("Custom ABS and TC trigger frequencies change the R2/L2 effect bytes", () =>
        {
            var abs = BrakePedalFeedback.BuildAbs(1f, 1f, 1f, 33);
            Assert(abs.Mode == 0x26 && abs.P8 == 33, "custom ABS trigger frequency was ignored");
            var absZero = BrakePedalFeedback.BuildAbs(1f, 1f, 1f, 0);
            Assert(absZero.Mode == 0x21, "zero ABS frequency removed resting resistance");

            FeedbackTuning tuning = FeedbackTuning.Neutral with
            {
                BrakeResistanceScale = 0, ThrottleResistanceScale = 0,
                AbsPulseScale = 0, LockPulseScale = 0, ShiftKickScale = 0,
                RedlinePulseScale = 0, WheelspinPulseScale = 0,
                RoadSurfaceScale = 0, GripLossScale = 0, NativeAccScale = 0,
                EngineTextureScale = 0, CollisionFeedbackScale = 0,
                TcPulseScale = 1, TcPulseFrequency = 33
            };
            var frame = new CompetitionFeedbackEngine().Compute(
                TcCar(), At(0), 0, 0, tuning, new AccShiftCueState(6000, false, false));
            Assert(frame.RightTrigger.Mode == 0x26 && frame.RightTrigger.P8 == 33,
                "custom TC trigger frequency was ignored");
        });

        check("Custom ABS grip frequency changes the body modulation", () =>
        {
            FeedbackTuning low = FeedbackTuning.Neutral with
            {
                BrakeResistanceScale = 0, ThrottleResistanceScale = 0,
                AbsPulseScale = 1, LockPulseScale = 0, ShiftKickScale = 0,
                RedlinePulseScale = 0, WheelspinPulseScale = 0, TcPulseScale = 0,
                RoadSurfaceScale = 0, GripLossScale = 0, NativeAccScale = 0,
                EngineTextureScale = 0, CollisionFeedbackScale = 0,
                AbsPulseGripFrequency = 16
            };
            FeedbackTuning high = low with { AbsPulseGripFrequency = 60 };
            var lowEngine = new CompetitionFeedbackEngine();
            var highEngine = new CompetitionFeedbackEngine();
            bool differs = false;
            for (int ms = 0; ms <= 240; ms += 4)
            {
                var car = AbsCar(ms);
                var a = lowEngine.Compute(car, At(ms), 0, 0, low);
                var b = highEngine.Compute(car, At(ms), 0, 0, high);
                differs |= a.LeftActuator != b.LeftActuator || a.RightActuator != b.RightActuator;
            }
            Assert(differs, "custom ABS grip frequency did not reach the body output");
        });
    }

    private static TelemetrySnapshot AbsCar(int ms) => new()
    {
        PacketId = 100 + ms, ObservedTimestamp = At(ms), Brake = 1, Gas = 0,
        SpeedKmh = 100, Abs = .5f, AbsLevel = 1,
        Rpm = 4000, CurrentMaxRpm = 8000, IsEngineRunning = 1
    };
}
