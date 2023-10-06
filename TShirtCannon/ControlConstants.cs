namespace TShirtCannon2023
{
    public enum BarrelShotTypes
    {
        NOT_SHOOTING,
        LOW_AIR_VOLUME,
        MED_AIR_VOLUME,
        HIGH_AIR_VOLUME
    }
    
    public enum DeviceIDs : int
    {
        DRIVE_LEFT_LEAD = 11,
        DRIVE_LEFT_FOLLOW = 12,
        DRIVE_RIGHT_LEAD = 13,
        DRIVE_RIGHT_FOLLOW = 14,
        ACTUATOR_MOTOR = 15,
    }

    public class DigitalIOPins
    {
        public static Microsoft.SPOT.Hardware.Cpu.Pin HIGH_BARREL_LOW_PRESSURE = CTRE.HERO.IO.Port8.Pin4;
        public static Microsoft.SPOT.Hardware.Cpu.Pin HIGH_BARREL_HIGH_PRESSURE = CTRE.HERO.IO.Port8.Pin5;
        public static Microsoft.SPOT.Hardware.Cpu.Pin LOW_BARREL_LOW_PRESSURE = CTRE.HERO.IO.Port6.Pin4;
        public static Microsoft.SPOT.Hardware.Cpu.Pin LOW_BARREL_HIGH_PRESSURE = CTRE.HERO.IO.Port6.Pin5;
    }

    public class RobotConstants
    {
        public static int robotCycleTimeMs = 10;
        public static double codeExecutionTimeMs = 5.8;
    }

    public class DrivetrainConstants
    {
        public static float DEADBAND_THRESHOLD = (float)0.10;
        public static float STICK_GAIN_FACTOR = (float)0.90;
        public static float STICK_INV_DEADBAND = (float)0.0;
        public static float TWIST_SCALING = (float)1.0;
    }

    public class ShootingConstants
    {
        // n seconds at (robotCycleTimeMs each) per second
        public static int DEBOUNCE_WAIT_CYCLES = (int)(2.0 * (1000.0/RobotConstants.robotCycleTimeMs));
        // Keep air open for this many cycles (robotCycleTimeMs each)
        public static uint SHOT_CYCLES = 3;
    }

    public class ActuatorConstants
    {
        public static double ACTUATOR_DRIVE_POWER = 1.0;
    }
}
