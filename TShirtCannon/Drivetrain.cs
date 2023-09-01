using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;

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
        }

        public void executeCycle(GameController gamepad)
        {
            /* Get gamepad axis inputs */
            float forward = gamepad.GetAxis((uint)forwardAxis);
            float twist = gamepad.GetAxis((uint)twistAxis);

            /* Deadband gamepad axis inputs */
            forward = deadband(forward, DrivetrainConstants.DEADBAND_WIDTH);
            twist = deadband(twist, DrivetrainConstants.DEADBAND_WIDTH);

            /* Compute throttle for each side of the robot */
            float leftThrot = forward - twist;
            float rightThrot = forward + twist;

            /* Set the motor percent output on each motor
             * based on the computed throttle. */
            leftLead.Set(ControlMode.PercentOutput, leftThrot);
            leftFollow.Set(ControlMode.PercentOutput, leftThrot);
            rightLead.Set(ControlMode.PercentOutput, -rightThrot);
            rightFollow.Set(ControlMode.PercentOutput, -rightThrot);
        }

        /**
         * If value is within 10% of center, clear it.
         * @param value [out] floating point value to deadband.
         */
        private float deadband(float value, float width)
        {
            if (value < -width)
            {
                /* outside of deadband */
                return value;
            }
            else if (value > +width)
            {
                /* outside of deadband */
                return value;
            }
            else
            {
                /* within the percent width so zero it */
                return (float)0.0;
            }
        }
    }
}
