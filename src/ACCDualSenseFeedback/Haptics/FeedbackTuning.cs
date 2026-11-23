using ACCDualSenseFeedback.Hardware.DualSense;

namespace ACCDualSenseFeedback.Haptics;

internal enum FeedbackPreset
{
    Default,
    Custom
}

internal enum PedalResistanceMode
{
    Progressive,
    ConstantLight
}

internal sealed record FeedbackProfile
{
    public const int NeutralValue = 50;
    public const int DefaultRedlineStartPermille = 962;
    public const int DefaultEngineTexture = 20;
    public const int DefaultCollisionFeedback = 60;

    public const int DefaultAbsPulseTriggerFrequency = 15;
    public const int DefaultAbsPulseGripFrequency = 16;
    public const int DefaultLockPulseTriggerFrequency = 8;
    public const int DefaultLockPulseGripFrequency = 8;
    public const int DefaultShiftKickTriggerFrequency = 28;
    public const int DefaultShiftKickGripFrequency = 32;
    public const int DefaultRedlinePulseFrequency = 18;
    public const int DefaultTcPulseFrequency = 40;
    public const int DefaultWheelspinPulseFrequency = 7;

    // Both frequency sliders are exposed at the raw DualSense 0–255 range.
    public const int MinTriggerFrequency = 0;
    public const int MaxTriggerFrequency = 255;
    public const int MinGripFrequency = 0;
    public const int MaxGripFrequency = 255;

    public static readonly FeedbackProfile Default = new();

    public FeedbackPreset ActivePreset { get; init; } = FeedbackPreset.Default;

    public int BrakeResistance { get; init; } = NeutralValue;
    public int ThrottleResistance { get; init; } = NeutralValue;
    public PedalResistanceMode PedalResistanceMode { get; init; } = PedalResistanceMode.Progressive;

    public int AbsPulse { get; init; } = NeutralValue;
    public int LockPulse { get; init; } = NeutralValue;

    public int ShiftKick { get; init; } = NeutralValue;
    public int RedlinePulse { get; init; } = NeutralValue;
    public int RedlineStartPermille { get; init; } = DefaultRedlineStartPermille;
    public int EngineTexture { get; init; } = DefaultEngineTexture;

    public int TcPulse { get; init; } = NeutralValue;
    public int WheelspinPulse { get; init; } = NeutralValue;

    public int RoadSurface { get; init; } = NeutralValue;
    public int GripLoss { get; init; } = NeutralValue;
    public int NativeAcc { get; init; } = NeutralValue;
    public int CollisionFeedback { get; init; } = DefaultCollisionFeedback;

    public int AbsPulseTriggerFrequency { get; init; } = DefaultAbsPulseTriggerFrequency;
    public int AbsPulseGripFrequency { get; init; } = DefaultAbsPulseGripFrequency;

    public int LockPulseTriggerFrequency { get; init; } = DefaultLockPulseTriggerFrequency;
    public int LockPulseGripFrequency { get; init; } = DefaultLockPulseGripFrequency;

    public int ShiftKickTriggerFrequency { get; init; } = DefaultShiftKickTriggerFrequency;
    public int ShiftKickGripFrequency { get; init; } = DefaultShiftKickGripFrequency;

    public int RedlinePulseFrequency { get; init; } = DefaultRedlinePulseFrequency;
    public int TcPulseFrequency { get; init; } = DefaultTcPulseFrequency;
    public int WheelspinPulseFrequency { get; init; } = DefaultWheelspinPulseFrequency;

    public FeedbackProfile Sanitize()
        => this with
        {
            ActivePreset = Enum.IsDefined(ActivePreset) ? ActivePreset : FeedbackPreset.Default,
            BrakeResistance = ClampStrength(BrakeResistance),
            ThrottleResistance = ClampStrength(ThrottleResistance),
            PedalResistanceMode = Enum.IsDefined(PedalResistanceMode)
                ? PedalResistanceMode
                : PedalResistanceMode.Progressive,
            AbsPulse = ClampStrength(AbsPulse),
            LockPulse = ClampStrength(LockPulse),
            ShiftKick = ClampStrength(ShiftKick),
            RedlinePulse = ClampStrength(RedlinePulse),
            RedlineStartPermille = Math.Clamp(RedlineStartPermille, 940, 990),
            EngineTexture = ClampStrength(EngineTexture),
            TcPulse = ClampStrength(TcPulse),
            WheelspinPulse = ClampStrength(WheelspinPulse),
            RoadSurface = ClampStrength(RoadSurface),
            GripLoss = ClampStrength(GripLoss),
            NativeAcc = ClampStrength(NativeAcc),
            CollisionFeedback = ClampStrength(CollisionFeedback),
            AbsPulseTriggerFrequency = ClampTriggerFrequency(AbsPulseTriggerFrequency),
            AbsPulseGripFrequency = ClampGripFrequency(AbsPulseGripFrequency),
            LockPulseTriggerFrequency = ClampTriggerFrequency(LockPulseTriggerFrequency),
            LockPulseGripFrequency = ClampGripFrequency(LockPulseGripFrequency),
            ShiftKickTriggerFrequency = ClampTriggerFrequency(ShiftKickTriggerFrequency),
            ShiftKickGripFrequency = ClampGripFrequency(ShiftKickGripFrequency),
            RedlinePulseFrequency = ClampTriggerFrequency(RedlinePulseFrequency),
            TcPulseFrequency = ClampTriggerFrequency(TcPulseFrequency),
            WheelspinPulseFrequency = ClampTriggerFrequency(WheelspinPulseFrequency)
        };

    public FeedbackTuning ToTuning()
    {
        if (ActivePreset == FeedbackPreset.Default)
            return FeedbackTuning.Neutral;

        return new FeedbackTuning(
            ScaleIntensity(BrakeResistance, 1.35f),
            ScaleIntensity(ThrottleResistance, 1.35f),
            ScaleIntensity(AbsPulse, 1.55f),
            ScaleIntensity(LockPulse, 1.55f),
            ScaleIntensity(ShiftKick, 1.55f),
            ScaleIntensity(RedlinePulse, 1.50f),
            ScaleIntensity(TcPulse, 1.50f),
            ScaleIntensity(WheelspinPulse, 1.55f),
            ScaleIntensity(RoadSurface, 1.45f),
            ScaleIntensity(GripLoss, 1.45f),
            ScaleIntensity(NativeAcc, 1.35f),
            ScaleCalibrated(EngineTexture, DefaultEngineTexture, 2.00f),
            ScaleCalibrated(CollisionFeedback, DefaultCollisionFeedback, 1.50f),
            RedlineStartPermille / 1000f)
        {
            AbsPulseTriggerFrequency = (byte)ClampTriggerFrequency(AbsPulseTriggerFrequency),
            AbsPulseGripFrequency = (byte)ClampGripFrequency(AbsPulseGripFrequency),
            LockPulseTriggerFrequency = (byte)ClampTriggerFrequency(LockPulseTriggerFrequency),
            LockPulseGripFrequency = (byte)ClampGripFrequency(LockPulseGripFrequency),
            ShiftKickTriggerFrequency = (byte)ClampTriggerFrequency(ShiftKickTriggerFrequency),
            ShiftKickGripFrequency = (byte)ClampGripFrequency(ShiftKickGripFrequency),
            RedlinePulseFrequency = (byte)ClampTriggerFrequency(RedlinePulseFrequency),
            TcPulseFrequency = (byte)ClampTriggerFrequency(TcPulseFrequency),
            WheelspinPulseFrequency = (byte)ClampTriggerFrequency(WheelspinPulseFrequency),
            ConstantPedalResistance = PedalResistanceMode == PedalResistanceMode.ConstantLight,
            BrakeConstantResistance = ConstantLightForce(BrakeResistance),
            ThrottleConstantResistance = ConstantLightForce(ThrottleResistance)
        };
    }

    private static int ClampStrength(int value) => Math.Clamp(value, 0, 100);

    // Constant-light mode uses 0x01 continuous resistance. Its 8-bit force
    // stays light even at 100 so the setting keeps the full-travel feel.
    private static float ConstantLightForce(int value)
    {
        int clamped = ClampStrength(value);
        if (clamped <= 0)
            return 0f;
        return clamped / 100f * 0.40f;
    }

    private static int ClampTriggerFrequency(int value)
        => Math.Clamp(value, MinTriggerFrequency, MaxTriggerFrequency);

    private static int ClampGripFrequency(int value)
        => Math.Clamp(value, MinGripFrequency, MaxGripFrequency);

    private static float ScaleIntensity(int value, float maximum)
    {
        if (value <= 0)
            return 0f;

        int clamped = ClampStrength(value);
        if (clamped <= NeutralValue)
        {
            float normalized = clamped / (float)NeutralValue;
            return MathF.Pow(normalized, 0.82f);
        }

        return Lerp(1f, maximum, (clamped - NeutralValue) / (float)NeutralValue);
    }

    private static float Lerp(float from, float to, float amount) => from + (to - from) * amount;

    private static float ScaleCalibrated(int value, int calibratedValue, float maximum)
    {
        int clamped = ClampStrength(value);
        if (clamped <= 0)
            return 0f;
        if (clamped <= calibratedValue)
            return clamped / (float)calibratedValue;
        return Lerp(1f, maximum, (clamped - calibratedValue) / (float)(100 - calibratedValue));
    }
}

internal sealed class FeedbackProfileState
{
    private FeedbackProfile _current;

    public FeedbackProfileState(FeedbackProfile initial) => _current = initial.Sanitize();

    public FeedbackProfile Current => Volatile.Read(ref _current);

    public FeedbackTuning CurrentTuning => Current.ToTuning();

    public void Update(FeedbackProfile profile) => Volatile.Write(ref _current, profile.Sanitize());
}

internal readonly record struct FeedbackTuning(
    float BrakeResistanceScale,
    float ThrottleResistanceScale,
    float AbsPulseScale,
    float LockPulseScale,
    float ShiftKickScale,
    float RedlinePulseScale,
    float TcPulseScale,
    float WheelspinPulseScale,
    float RoadSurfaceScale,
    float GripLossScale,
    float NativeAccScale,
    float EngineTextureScale,
    float CollisionFeedbackScale,
    float RedlineStartRatio)
{
    public byte AbsPulseTriggerFrequency { get; init; } = (byte)FeedbackProfile.DefaultAbsPulseTriggerFrequency;
    public byte AbsPulseGripFrequency { get; init; } = (byte)FeedbackProfile.DefaultAbsPulseGripFrequency;

    public byte LockPulseTriggerFrequency { get; init; } = (byte)FeedbackProfile.DefaultLockPulseTriggerFrequency;
    public byte LockPulseGripFrequency { get; init; } = (byte)FeedbackProfile.DefaultLockPulseGripFrequency;

    public byte ShiftKickTriggerFrequency { get; init; } = (byte)FeedbackProfile.DefaultShiftKickTriggerFrequency;
    public byte ShiftKickGripFrequency { get; init; } = (byte)FeedbackProfile.DefaultShiftKickGripFrequency;

    public byte RedlinePulseFrequency { get; init; } = (byte)FeedbackProfile.DefaultRedlinePulseFrequency;
    public byte TcPulseFrequency { get; init; } = (byte)FeedbackProfile.DefaultTcPulseFrequency;
    public byte WheelspinPulseFrequency { get; init; } = (byte)FeedbackProfile.DefaultWheelspinPulseFrequency;

    public bool ConstantPedalResistance { get; init; }
    public float BrakeConstantResistance { get; init; }
    public float ThrottleConstantResistance { get; init; }

    public static readonly FeedbackTuning Neutral = new(
        1f, 1f,
        1f, 1f,
        1f, 1f,
        1f, 1f,
        1f, 1f, 1f,
        1f, 1f,
        0.962f);

    public static TriggerEffect ScaleTrigger(in TriggerEffect effect, float scale)
    {
        if (effect.Mode == 0x05)
            return effect;
        if (scale <= 0.001f)
            return TriggerEffect.Off;
        if (MathF.Abs(scale - 1f) < 0.0001f)
            return effect;

        if (effect.Mode == 0x01)
        {
            return effect with
            {
                P1 = ScaleByte(effect.P1, scale)
            };
        }

        if (effect.Mode is not (0x21 or 0x26))
            return effect;

        ushort active = (ushort)(effect.P0 | effect.P1 << 8);
        uint packed = (uint)(effect.P2 |
            effect.P3 << 8 |
            effect.P4 << 16 |
            effect.P5 << 24);
        uint scaled = 0;

        for (int index = 0; index < 10; index++)
        {
            if ((active & (1 << index)) == 0)
                continue;

            int strength = (int)((packed >> (index * 3)) & 0x07) + 1;
            int adjusted = Math.Clamp((int)MathF.Round(strength * scale), 1, 8);
            scaled |= (uint)(adjusted - 1) << (index * 3);
        }

        return effect with
        {
            P2 = (byte)scaled,
            P3 = (byte)(scaled >> 8),
            P4 = (byte)(scaled >> 16),
            P5 = (byte)(scaled >> 24)
        };
    }

    private static byte ScaleByte(byte value, float scale)
        => (byte)Math.Clamp((int)MathF.Round(value * scale), 0, byte.MaxValue);
}
