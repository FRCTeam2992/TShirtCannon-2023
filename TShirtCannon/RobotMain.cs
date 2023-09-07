using CTRE.Phoenix;
using CTRE.Phoenix.Controller;
using System.Threading;
using TShirtCannon2023.Subsystems;

namespace TShirtCannon2023
{
    public class RobotMain
    {
        static Drivetrain drivetrain = new Drivetrain(
            DeviceIDs.DRIVE_LEFT_LEAD,
            DeviceIDs.DRIVE_LEFT_FOLLOW,
            DeviceIDs.DRIVE_RIGHT_LEAD,
            DeviceIDs.DRIVE_RIGHT_FOLLOW,
            GamepadAxes.DRIVE,
            GamepadAxes.TWIST);

        static Actuator actuator = new Actuator(
            DeviceIDs.ACTUATOR_MOTOR,
            GamepadButtonMappings.ACTUATOR_UP,
            GamepadButtonMappings.ACTUATOR_DOWN);

        static Barrel highBarrel = new Barrel(
            DigitalIOPins.HIGH_BARREL_HIGH_PRESSURE,
            DigitalIOPins.HIGH_BARREL_LOW_PRESSURE,
            GamepadButtonMappings.HIGH_BARREL_SHOOT,
            GamepadButtonMappings.HIGH_BARREL_HIGH_PRESSURE,
            GamepadButtonMappings.HIGH_BARREL_LOW_PRESSURE);

        static Barrel lowBarrel = new Barrel(
            DigitalIOPins.LOW_BARREL_HIGH_PRESSURE,
            DigitalIOPins.LOW_BARREL_LOW_PRESSURE,
            GamepadButtonMappings.LOW_BARREL_SHOOT,
            GamepadButtonMappings.LOW_BARREL_HIGH_PRESSURE,
            GamepadButtonMappings.LOW_BARREL_LOW_PRESSURE);

        static CTRE.Phoenix.Controller.GameController _gamepad = null;

        static RobotModes robotMode = RobotModes.DISABLED;
        static bool feedDevices = false;

        public static void Main()
        {
            /* loop forever */
            while (true)
            {
                if (null == _gamepad)
                    _gamepad = new GameController(UsbHostDevice.GetInstance());

                /* enable the robot? */
                robotMode = getRobotModeAndSetFeed(_gamepad, robotMode);

                switch (robotMode)
                {
                    case RobotModes.ENABLED:
                        /* drive robot using gamepad */
                        drivetrain.executeCycle(_gamepad);
                        /* control linear actuator for aim */
                        actuator.executeCycle(_gamepad);
                        /* shoot t-shirts out of the cannon barrels */
                        highBarrel.executeCycle(_gamepad);
                        lowBarrel.executeCycle(_gamepad);
                        break;
                    case RobotModes.DISABLED:
                        /* disable the drivetrain */
                        drivetrain.disable();
                        /* disable the linear actuator */
                        actuator.disable();
                        /* disable the cannon barrels */
                        highBarrel.disable();
                        lowBarrel.disable();
                        break;
                }

                if (feedDevices)
                {
                    /* feed watchdog to keep all devices enabled */
                    CTRE.Phoenix.Watchdog.Feed();
                }

                /* run this task every 20ms */
                Thread.Sleep(20);
            }
        }

        static RobotModes getRobotModeAndSetFeed(GameController gamepad, RobotModes currentMode)
        {
            bool disableRobot;
            if (gamepad == null ||
                gamepad.GetConnectionStatus() == UsbDeviceConnection.NotConnected)
            {
                disableRobot = true;
            }
            else
            {
                disableRobot = gamepad.GetButton(
                    (uint)GamepadButtonMappings.DISABLE_ROBOT);
            }
            bool enableRobot = gamepad.GetButton(
                (uint)GamepadButtonMappings.ENABLE_ROBOT);

            feedDevices = (currentMode == RobotModes.ENABLED);
            if (enableRobot && !disableRobot)
            {
                return RobotModes.ENABLED;
            }
            if (disableRobot)
            {
                /* On a switch to disabled, ensure that we feed
                 * devices for one more cycle to zero values
                 */
                feedDevices = true;
                return RobotModes.DISABLED;
            }
            return currentMode;
        }
    }
}