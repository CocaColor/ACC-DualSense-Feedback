using System.Diagnostics;
using ACCDualSenseFeedback.Haptics;
using ACCDualSenseFeedback.Hardware.DualSense;
using ACCDualSenseFeedback.Telemetry;

internal static class PedalResistanceModeChecks
{
    private static long At(int ms) => (long)((1 + ms / 1000.0) * Stopwatch.Frequency);

    private static void Assert(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    public static void Run(Action<string, Action> check)
    {
        check("Progressive pedal resistance is the default and constant mode is explicit", () =>
        {
            Assert(FeedbackProfile.Default.PedalResistanceMode == PedalResistanceMode.Progressive,
                "the default profile is not progressive");

            FeedbackProfile custom = FeedbackProfile.Default with { ActivePreset = FeedbackPreset.Custom };
            FeedbackTuning progressive = custom.ToTuning();
            Assert(!progressive.ConstantPedalResistance, "custom progressive tuning became constant");

            FeedbackTuning constant = (custom with
            {
                BrakeResistance = 50,
                ThrottleResistance = 100,
                PedalResistanceMode = PedalResistanceMode.ConstantLight
            }).ToTuning();
            Assert(constant.ConstantPedalResistance, "constant-light tuning was not selected");
            Assert(constant.BrakeConstantResistance > 0f && constant.ThrottleConstantResistance > 0f,
                "constant-light force was not populated");
            Assert(constant.ThrottleConstantResistance > constant.BrakeConstantResistance,
                "constant-light force did not follow the throttle slider");

            FeedbackTuning defaultPreset = (custom with
            {
                ActivePreset = FeedbackPreset.Default,
                PedalResistanceMode = PedalResistanceMode.ConstantLight
            }).ToTuning();
            Assert(!defaultPreset.ConstantPedalResistance,
                "the Default preset must always use progressive resistance");
        });

        check("Constant-light mode emits 0x01 full-travel resistance", () =>
        {
            TriggerEffect progressive = BrakePedalFeedback.BuildAbs(1f, 0f, 1f, 15);
            Assert(progressive.Mode == 0x21, "progressive resting resistance changed");

            TriggerEffect constant = BrakePedalFeedback.BuildAbs(1f, 0f, 1f, 15, true, 0.20f);
            Assert(constant.Mode == 0x01 && constant.P1 == 51,
                "constant-light ABS fallback did not use 0x01 resistance");

            TriggerEffect zero = BrakePedalFeedback.BuildAbs(1f, 0f, 1f, 15, true, 0f);
            Assert(zero.Mode == 0x05, "zero constant force did not turn resistance off");
        });

        check("Constant-light mode builds both rested pedals without event pulses", () =>
        {
            var car = new TelemetrySnapshot
            {
                PacketId = 1,
                ObservedTimestamp = At(0),
                SpeedKmh = 100,
                Gear = 4,
                Rpm = 3000,
                CurrentMaxRpm = 8000,
                IsEngineRunning = 1,
                Gas = 0,
                Brake = 0
            };
            FeedbackTuning tuning = FeedbackTuning.Neutral with
            {
                ConstantPedalResistance = true,
                BrakeConstantResistance = 0.20f,
                ThrottleConstantResistance = 0.30f,
                BrakeResistanceScale = 1f,
                ThrottleResistanceScale = 1f,
                AbsPulseScale = 0f,
                LockPulseScale = 0f,
                ShiftKickScale = 0f,
                RedlinePulseScale = 0f,
                TcPulseScale = 0f,
                WheelspinPulseScale = 0f,
                RoadSurfaceScale = 0f,
                GripLossScale = 0f,
                NativeAccScale = 0f,
                EngineTextureScale = 0f,
                CollisionFeedbackScale = 0f
            };
            FeedbackFrame frame = new CompetitionFeedbackEngine().Compute(
                car, At(0), 0, 0, tuning);
            Assert(frame.LeftTrigger.Mode == 0x01 && frame.LeftTrigger.P1 == 51,
                "left pedal did not use constant-light resistance");
            Assert(frame.RightTrigger.Mode == 0x01 && frame.RightTrigger.P1 == 76,
                "right pedal did not use constant-light resistance");
        });
    }
}
