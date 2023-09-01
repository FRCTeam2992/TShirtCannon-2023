namespace TShirtCannon2023
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

    public enum GamepadButtonMappings : uint
    {
        LOW_BARREL_SHOOT = GamepadButtons.RIGHT_TRIGGER,
        LOW_BARREL_LOW_PRESSURE = GamepadButtons.BUTTON_2,
        LOW_BARREL_HIGH_PRESSURE = GamepadButtons.BUTTON_1,
        HIGH_BARREL_SHOOT = GamepadButtons.RIGHT_BUMPER,
        HIGH_BARREL_LOW_PRESSURE = GamepadButtons.BUTTON_3,
        HIGH_BARREL_HIGH_PRESSURE = GamepadButtons.BUTTON_4,
        ACTUATOR_UP = GamepadButtons.LEFT_BUMPER,
        ACTUATOR_DOWN = GamepadButtons.LEFT_TRIGGER,
    }

    public enum GamepadAxes : uint
    {
        DRIVESTICK_DO_NOT_USE = 0,
        DRIVE = 1,
        TWIST = 2,
    }
}
