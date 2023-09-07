using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;

namespace TShirtCannon2023.Subsystems
{
    public class Actuator
    {
        private VictorSPX actuator;

        private GamepadButtonMappings upButton;
        private GamepadButtonMappings downButton;

        public Actuator(DeviceIDs actuatorMotorID, GamepadButtonMappings up, GamepadButtonMappings down)
        {
            actuator = new VictorSPX((int)actuatorMotorID);
            upButton = up;
            downButton = down;
        }

        public void disable()
        {
            actuator.Set(ControlMode.PercentOutput, 0.0);
        }

        public void executeCycle(GameController gamepad)
        {
            bool actuatorUp = gamepad.GetButton((uint)upButton);
            bool actuatorDown = gamepad.GetButton((uint)downButton);

            if (actuatorUp)
            {
                actuator.Set(ControlMode.PercentOutput, ActuatorConstants.ACTUATOR_DRIVE_POWER);
            }
            else if (actuatorDown)
            {
                actuator.Set(ControlMode.PercentOutput, -ActuatorConstants.ACTUATOR_DRIVE_POWER);
            }
            else
            {
                actuator.Set(ControlMode.PercentOutput, 0.0);
            }
        }
    }
}
