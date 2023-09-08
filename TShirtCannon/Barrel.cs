using CTRE.Phoenix;
using CTRE.Phoenix.Controller;

namespace TShirtCannon2023.Subsystems
{
    public class Barrel
    {
        private SafeOutputPort spikeHigh;
        private SafeOutputPort spikeLow;

        private GamepadButtonMappings shootTrigger;
        private GamepadButtonMappings highShot;
        private GamepadButtonMappings mediumShot;
        private GamepadButtonMappings lowShot;

        private uint debounceCounter;

        public Barrel(
            Microsoft.SPOT.Hardware.Cpu.Pin spikeHighPin,
            Microsoft.SPOT.Hardware.Cpu.Pin spikeLowPin,
            GamepadButtonMappings shootTrigger,
            GamepadButtonMappings highShot,
            GamepadButtonMappings mediumShot,
            GamepadButtonMappings lowShot
        )
        {
            this.shootTrigger = shootTrigger;
            this.highShot = highShot;
            this.mediumShot = mediumShot;
            this.lowShot = lowShot;

            spikeHigh = new SafeOutputPort(spikeHighPin, false);
            spikeLow = new SafeOutputPort(spikeLowPin, false);

            debounceCounter = 0;
        }

        public void executeCycle(GameController gamepad)
        {
            if (debounceShot())
            {
                bool shootBarrel = gamepad.GetButton((uint)shootTrigger);
                bool triggerSpikeLow = (
                    gamepad.GetButton((uint)lowShot) || gamepad.GetButton((uint)highShot)
                );
                bool triggerSpikeHigh = (
                    gamepad.GetButton((uint)mediumShot) || gamepad.GetButton((uint)highShot)
                );

                if (shootBarrel)
                {
                    if (triggerSpikeHigh)
                    {
                        spikeHigh.Write(true);
                    }
                    else
                    {
                        spikeHigh.Write(false);
                    }

                    if (triggerSpikeLow)
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