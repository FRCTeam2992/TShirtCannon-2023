# T-Shirt Cannon 2023

This project is the code for the 2023 build of the T-Shirt Cannon using
[the CTRE HERO control board](https://store.ctr-electronics.com/hero-development-board/),
for which you can [look at the docs here](https://store.ctr-electronics.com/content/user-manual/HERO%20User%27s%20Guide.pdf).

This code base uses C# with the .NET Microframework (.NETMF). To work on
it, you'll need Microsoft Visual Studio 2019 (Community edition is fine).
Note that the current version _will not_ work.

You'll also need to install
[CTRE's Phoenix Framework Software](https://store.ctr-electronics.com/software/)
with the HERO board libraries and tools installed.

## Code Organization

This code is organized similar to the approach taken for our FRC robot
projects, by moving code related to different hardware subsystems into
their own classes in their own code files. Because this project is much
simpler than most of our FRC robots, we do not introduce the Command
construction here, but only stick with robot cycles and subsystems.

### RobotMain

The main program is `RobotMain`, which sets up the infinite loop with a
20ms delay robot cycle of execution.

That main loop calls the `executeCycle` method on each of the subsystems
once per robot cycle.

### Subsystems

The following subsystems are included in the robot.

#### Drivetrain

The robot uses a tank/arcade drive system controlled by two controller
axes, one for forward/backward movement, and one for twist/turn movement.
The controller axes are deadbanded to prevent arbitrary unintentional
movement. The drivetrain is aligned perpendicularly to the shooting
direction, so forward movement is along the long side of the robot.
The drivetrain uses 4 Mini CIM motors, two on each side, with each motor
controlled by a Victor SRX controller connected on the CAN bus provided
by the HERO board.

#### Actuator

The linear actuator is used to move the double-barrel assembly up and down
in order to aim shots. The linear actuator has a motor internal to it which
is controlled by a Victor SRX controller. The extension and retraction movement
of the linear actuator which provides up and down movement respectively is
mapped to two buttons which drive positive and negative percentage output of
the interior motor through the controller.

#### Barrels

Each barrel shot is executed through a complex pressurized air plumbing system
controlled by a spike relay which provides (through a solenoid on each signal
output from the spike) for states mapped to differing shot executions:

|              | Signal 1 OFF | Signal 1 ON  |
|--------------|--------------|--------------|
| Signal 2 OFF |   NO SHOT    |   LOW AIR    |
| Signal 2 ON  |   MED AIR    |    HI AIR    |

In code, this is controlled through two GPIO ports, one for each signal line
to the spike relay. These are mapped to button inputs controlling whether the
signal lines are triggered which tied to low pressure shots and high pressure
shots respectively. Shots are executed by a third button mapped for a shot for
each barrel, and shots are debounced to happen for a triggering happening for
a duration of one robot cycle in a given interval of a few seconds.

### Supporting code files

#### Control Constants

Robot control constants are set in a separate file and used from there in order
to make tuning robot controls easier.

#### Button Mappings

Controller button mappings are also defined in a separate file and used from
there in order to make remapping button controls easier and to make reading
code more easy.
