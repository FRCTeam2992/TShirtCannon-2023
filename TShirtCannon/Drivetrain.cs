using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using System;

namespace TShirtCannon2023.Subsystems
{
    public class Drivetrain
    {
        private VictorSPX leftLead;
        private VictorSPX leftFollow;
        private VictorSPX rightLead;
        private VictorSPX rightFollow;

        private GamepadAxes forwardAxis;
        private GamepadAxes twistAxis;

        private double leftPrevThrottle = 0.0;
        private double rightPrevThrottle = 0.0;

        public Drivetrain(
            DeviceIDs leftLead, DeviceIDs leftFollow,
            DeviceIDs rightLead, DeviceIDs rightFollow,
            GamepadAxes forwardAxis, GamepadAxes twistAxis)
        {
            this.leftLead = new VictorSPX((int)leftLead);
            this.leftFollow = new VictorSPX((int)leftFollow);
            this.rightLead = new VictorSPX((int)rightLead);
            this.rightFollow = new VictorSPX((int)rightFollow);

            this.forwardAxis = forwardAxis;
            this.twistAxis = twistAxis;

            this.resetSlewRateLimit();
        }

        public void executeCycle(GameController gamepad)
        {
            /* Get gamepad axis inputs */
            float forward = gamepad.GetAxis((uint)forwardAxis);
            float twist = gamepad.GetAxis((uint)twistAxis);

            /* Deadband gamepad axis inputs */
            forward = deadband(forward, DrivetrainConstants.DEADBAND_THRESHOLD);
            twist = deadband(twist, DrivetrainConstants.DEADBAND_THRESHOLD);

            /* Compute throttles */
            float[] throttles = computeThrottle(forward, twist);

            /* Slew rate limit outputs */
            throttles = slewRateLimit(throttles);

            float leftThrot = throttles[0];
            float rightThrot = throttles[1];

            /* Set the motor percent output on each motor
             * based on the computed throttle. */
            leftLead.Set(ControlMode.PercentOutput, leftThrot);
            leftFollow.Set(ControlMode.PercentOutput, leftThrot);
            rightLead.Set(ControlMode.PercentOutput, -rightThrot);
            rightFollow.Set(ControlMode.PercentOutput, -rightThrot);
        }

        /**
         * If value is within threshold of center, clear it.
         * @param value [out] floating point value to deadband.
         */
        private float deadband(float value, float threshold)
        {
            // Force Limit from -1.0 to 1.0
            value = (float)Math.Max(-1, Math.Min(1, value));

            // Check If Value is Inside the Deadzone
            if (value >= 0.0)
            {
                if (Math.Abs(value) >= threshold)
                {
                    return (value - threshold) / (1 - threshold);
                }
                else
                {
                    return (float)0.0;
                }
            }
            else
            {
                if (Math.Abs(value) >= threshold)
                {
                    return (value + threshold) / (1 - threshold);
                }
                else
                {
                    return (float)0.0;
                }
            }
        }

        /* Smooth an input value via mixing linear and cubic gain
         * curves. See the following reference for more detail:
         * https://www.chiefdelphi.com/t/paper-joystick-sensitivity-gain-adjustment/107280
         */
        private float smoothInput(float x)
        {
            float a = DrivetrainConstants.STICK_GAIN_FACTOR;
            float b = DrivetrainConstants.STICK_INV_DEADBAND;

            if (x > 0.0)
            {
                return (float)(b + (1.0 - b) * (a * x * x * x + (1.0 - a) * x));
            }
            if (x < 0.0)
            {
                return (float)(-b + (1.0 - b) * (a * x * x * x + (1.0 - a) * x));
            }
            return (float)0.0;
        }

        /* Compute an array of smoothed throttles for left 
         * and right drive motors */
        private float[] computeThrottle(float forwardAxis, float twistAxis)
        {
            /* Smooth inputs for better control at low-speed drive */
            float forward = smoothInput(forwardAxis);
            float twist = smoothInput(twistAxis);

            float leftThrot = forward - DrivetrainConstants.TWIST_SCALING * twist;
            float rightThrot = forward + DrivetrainConstants.TWIST_SCALING * twist;

            /* Scale by maxOutput to ensure no more than 100% power */
            float maxOutput = (float)Math.Max(
                Math.Abs(leftThrot),
                Math.Abs(rightThrot));

            if (maxOutput > (float)1.0)
            {
                leftThrot /= maxOutput;
                rightThrot /= maxOutput;
            }

            /* Return throttles array */
            float[] throttles = { leftThrot, rightThrot };
            return throttles;
        }

        private float[] slewRateLimit(float[] throttles)
        {
            double leftThrot = throttles[0];
            double rightThrot = throttles[1];

            leftPrevThrottle += 
                Math.Min(
                    Math.Max(
                        leftThrot - leftPrevThrottle,
                        -DrivetrainConstants.MAX_CHANGE_PER_CYCLE
                    ),
                    DrivetrainConstants.MAX_CHANGE_PER_CYCLE
                );
            rightPrevThrottle +=
                Math.Min(
                    Math.Max(
                        rightThrot - rightPrevThrottle,
                        -DrivetrainConstants.MAX_CHANGE_PER_CYCLE
                    ),
                    DrivetrainConstants.MAX_CHANGE_PER_CYCLE
                );

            float[] newThrottles = {
                (float)leftPrevThrottle,
                (float)rightPrevThrottle
            };
            return newThrottles;
        }

        private void resetSlewRateLimit()
        {
            leftPrevThrottle = 0;
            rightPrevThrottle = 0;
        }
    }
}
