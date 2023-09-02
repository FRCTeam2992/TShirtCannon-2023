using CTRE.Phoenix;
using CTRE.Phoenix.Controller;

namespace TShirtCannon2023.Subsystems
{
    public class Barrel
    {
        private SafeOutputPort spikeHigh;
        private SafeOutputPort spikeLow;

        private GamepadButtonMappings shootTrigger;
        private GamepadButtonMappings highPressure;
        private GamepadButtonMappings lowPressure;

        private uint debounceCounter;

        public Barrel(
            Microsoft.SPOT.Hardware.Cpu.Pin spikeHighPin,
            Microsoft.SPOT.Hardware.Cpu.Pin spikeLowPin,
            GamepadButtonMappings shootTrigger,
            GamepadButtonMappings highPressure,
            GamepadButtonMappings lowPressure
        )
        {
            this.shootTrigger = shootTrigger;
            this.highPressure = highPressure;
            this.lowPressure = lowPressure;

            spikeHigh = new SafeOutputPort(spikeHighPin, false);
            spikeLow = new SafeOutputPort(spikeLowPin, false);

            debounceCounter = 0;
        }

        public void executeCycle(GameController gamepad)
        {
            if (debounceShot())
            {
                bool shootBarrel = gamepad.GetButton((uint)shootTrigger);
                bool pressureLow = gamepad.GetButton((uint)lowPressure);
                bool pressureHigh = gamepad.GetButton((uint)highPressure);

                if (shootBarrel)
                {
                    if (pressureHigh)
                    {
                        spikeHigh.Write(true);
                    }
                    else
                    {
                        spikeHigh.Write(false);
                    }

                    if (pressureLow)
                    {
                        spikeLow.Write(true);
                    }
                    else
                    {
                        spikeLow.Write(false);
                    }

                    debounceCounter = 0;
                }
                else
                {
                    spikeHigh.Write(false);
                    spikeLow.Write(false);
                }
            }
            else
            {
                spikeHigh.Write(false);
                spikeLow.Write(false);
            }
        }

        private bool debounceShot()
        {
            debounceCounter++;
            return debounceCounter > ShootingConstants.DEBOUNCE_WAIT_CYCLES;
        }
    }
}