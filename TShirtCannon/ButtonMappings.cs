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
        RIGHT_TRIGGER = 8,
        SELECT_BUTTON = 9,
        START_BUTTON = 10,
        EXTRA_BUTTON = 11
    }

    public enum GamepadButtonMappings : uint
    {
        LOW_BARREL_SHOOT = GamepadButtons.RIGHT_TRIGGER,
        HIGH_BARREL_SHOOT = GamepadButtons.RIGHT_BUMPER,
        LOW_SHOT = GamepadButtons.BUTTON_2,
        MEDIUM_SHOT = GamepadButtons.BUTTON_3,
        HIGH_SHOT = GamepadButtons.BUTTON_4,
        ACTUATOR_UP = GamepadButtons.LEFT_BUMPER,
        ACTUATOR_DOWN = GamepadButtons.LEFT_TRIGGER,
        ENABLE_ROBOT = GamepadButtons.START_BUTTON,
        DISABLE_ROBOT = GamepadButtons.SELECT_BUTTON
    }

    public enum GamepadAxes : uint
    {
        DRIVESTICK_DO_NOT_USE = 0,
        DRIVE = 1,
        TWIST = 2,
    }
}
