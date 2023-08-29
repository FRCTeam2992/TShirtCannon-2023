using CTRE.Phoenix;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using Microsoft.SPOT;
using System;
using System.Text;
using System.Threading;

namespace TShirtCannonStarter
{
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

    public class Program
    {
        static VictorSPX leftLead = new VictorSPX(11);
        static VictorSPX leftFollow = new VictorSPX(12);
        static VictorSPX rightLead = new VictorSPX(13);
        static VictorSPX rightFollow = new VictorSPX(14);
        static VictorSPX actuator = new VictorSPX(15);

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
            float x = _gamepad.GetAxis(0);
            float y = -1 * _gamepad.GetAxis(1);
            float twist = _gamepad.GetAxis(2);

            /* Deadband gamepad axis inputs */
            float deadbandWidth = (float)0.10;
            Deadband(ref x, deadbandWidth);
            Deadband(ref y, deadbandWidth);
            Deadband(ref twist, deadbandWidth);

            /* Compute throttle for each side of the robot */
            float leftThrot = y + twist;
            float rightThrot = y - twist;

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
                actuator.Set(ControlMode.PercentOutput, 0.5);
            }
            else if (actuatorDown)
            {
                actuator.Set(ControlMode.PercentOutput, -0.5);
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
                    // actuator.Set(ControlMode.PercentOutput, 1.0);
                }
                else
                {
                    /* For testing only. This should trigger the spike for
                     * low pressure shot from high barrel. */
                    // actuator.Set(ControlMode.PercentOutput, 0.3);
                }
            }
            else
            {
                /* For testing only. This should ensure we don't
                 * trigger the high barrel for some number of cycles
                 * after a shot. */
                // actuator.Set(ControlMode.PercentOutput, 0.0);
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
                    // actuator.Set(ControlMode.PercentOutput, -1.0);
                }
                else
                {
                    /* For testing only. This should trigger the spike for
                     * low pressure shot from low barrel. */
                    // actuator.Set(ControlMode.PercentOutput, -0.3);
                }
            }
            else
            {
                /* For testing only. This should ensure we don't
                 * trigger the low barrel for some number of cycles
                 * after a shot. */
                // actuator.Set(ControlMode.PercentOutput, 0.0);
            }
        }
    }
}