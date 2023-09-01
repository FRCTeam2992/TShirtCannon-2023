using CTRE.Phoenix;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using System.Threading;

namespace TShirtCannon2023
{
    public enum DeviceIDs : int
    {
        DRIVE_LEFT_LEAD = 11,
        DRIVE_LEFT_FOLLOW = 12,
        DRIVE_RIGHT_LEAD = 13,
        DRIVE_RIGHT_FOLLOW = 14,
        ACTUATOR_MOTOR = 15,
    }

    public enum GamepadButtons : uint
    {
        BUTTON_1 = 1,
        BUTTON_2 = 2,
        BUTTON_3 = 3,
        BUTTON_4 = 4,
        LEFT_BUMPER = 5,
        RIGHT_BUMPER = 6,
        LEFT_TRIGGER = 7,
        RIGHT_TRIGGER = 8
    }

    public enum GamepadAxes : uint
    {
        DRIVE_X = 0,
        DRIVE_Y = 1,
        TWIST = 2,
    }

    public class Program
    {
        static double ACTUATOR_DRIVE_POWER = 0.5;

        static VictorSPX leftLead = new VictorSPX((int)DeviceIDs.DRIVE_LEFT_LEAD);
        static VictorSPX leftFollow = new VictorSPX((int)DeviceIDs.DRIVE_LEFT_FOLLOW);
        static VictorSPX rightLead = new VictorSPX((int)DeviceIDs.DRIVE_RIGHT_LEAD);
        static VictorSPX rightFollow = new VictorSPX((int)DeviceIDs.DRIVE_RIGHT_FOLLOW);
        static VictorSPX actuator = new VictorSPX((int)DeviceIDs.ACTUATOR_MOTOR);

        static CTRE.Phoenix.Controller.GameController _gamepad = null;

        public static void Main()
        {
            /* loop forever */
            while (true)
            {
                if (null == _gamepad)
                    _gamepad = new GameController(UsbHostDevice.GetInstance());

                /* drive robot using gamepad */
                Drive();
                /* control linear actuator for aim */
                Aim();
                /* shoot t-shirts out of the cannon barrels */
                ShootHighBarrel();
                ShootLowBarrel();
                /* feed watchdog to keep Victor's enabled */
                CTRE.Phoenix.Watchdog.Feed();
                /* run this task every 20ms */
                Thread.Sleep(20);
            }
        }
        /**
         * If value is within 10% of center, clear it.
         * @param value [out] floating point value to deadband.
         */
        static void Deadband(ref float value, float width)
        {
            if (value < -width)
            {
                /* outside of deadband */
            }
            else if (value > +width)
            {
                /* outside of deadband */
            }
            else
            {
                /* within the percent width so zero it */
                value = 0;
            }
        }
        static void Drive()
        {
            /* Get gamepad axis inputs */
            float x = _gamepad.GetAxis((uint)GamepadAxes.DRIVE_X);
            float y = _gamepad.GetAxis((uint)GamepadAxes.DRIVE_Y);
            float twist = _gamepad.GetAxis((uint)GamepadAxes.TWIST);

            /* Deadband gamepad axis inputs */
            float deadbandWidth = (float)0.10;
            Deadband(ref x, deadbandWidth);
            Deadband(ref y, deadbandWidth);
            Deadband(ref twist, deadbandWidth);

            /* Compute throttle for each side of the robot */
            float leftThrot = y - twist;
            float rightThrot = y + twist;

            /* Set the motor percent output on each motor
             * based on the computed throttle. */
            leftLead.Set(ControlMode.PercentOutput, leftThrot);
            leftFollow.Set(ControlMode.PercentOutput, leftThrot);
            rightLead.Set(ControlMode.PercentOutput, -rightThrot);
            rightFollow.Set(ControlMode.PercentOutput, -rightThrot);
        }

        static void Aim()
        {
            bool actuatorUp = _gamepad.GetButton((uint)GamepadButtons.LEFT_BUMPER);
            bool actuatorDown = _gamepad.GetButton((uint)GamepadButtons.LEFT_TRIGGER);

            if (actuatorUp)
            {
                actuator.Set(ControlMode.PercentOutput, ACTUATOR_DRIVE_POWER);
            }
            else if (actuatorDown)
            {
                actuator.Set(ControlMode.PercentOutput, -ACTUATOR_DRIVE_POWER);
            }
            else
            {
                actuator.Set(ControlMode.PercentOutput, 0.0);
            }
        }

        static void ShootHighBarrel()
        {
            bool shootBarrel = _gamepad.GetButton((uint)GamepadButtons.RIGHT_BUMPER);
            bool pressureLow = _gamepad.GetButton((uint)GamepadButtons.BUTTON_1);
            bool pressureHigh = _gamepad.GetButton((uint)GamepadButtons.BUTTON_4);

            if (shootBarrel)
            {
                if (pressureHigh)
                {
                    /* For testing only. This should trigger the spike for
                     * high pressure shot from high barrel. */
                }
                else
                {
                    /* For testing only. This should trigger the spike for
                     * low pressure shot from high barrel. */
                }
            }
            else
            {
                /* For testing only. This should ensure we don't
                 * trigger the high barrel for some number of cycles
                 * after a shot. */
            }
        }

        static void ShootLowBarrel()
        {
            bool shootBarrel = _gamepad.GetButton((uint)GamepadButtons.RIGHT_TRIGGER);
            bool pressureLow = _gamepad.GetButton((uint)GamepadButtons.BUTTON_2);
            bool pressureHigh = _gamepad.GetButton((uint)GamepadButtons.BUTTON_3);

            if (shootBarrel)
            {
                if (pressureHigh)
                {
                    /* For testing only. This should trigger the spike for
                     * high pressure shot from low barrel. */
                }
                else
                {
                    /* For testing only. This should trigger the spike for
                     * low pressure shot from low barrel. */
                }
            }
            else
            {
                /* For testing only. This should ensure we don't
                 * trigger the low barrel for some number of cycles
                 * after a shot. */
            }
        }
    }
}