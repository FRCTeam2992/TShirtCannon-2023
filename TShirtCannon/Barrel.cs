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

        private int debounceCounter;
        private uint shotCounter;

        private BarrelShotTypes currentShot = BarrelShotTypes.NOT_SHOOTING;

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
            shotCounter = 0;

            currentShot = BarrelShotTypes.NOT_SHOOTING;
        }

        public void executeCycle(GameController gamepad)
        {
            if (debounceShot())
            {
                // Debounce has counted all required wait cycles,
                // so we can try to shoot again

                // If this is the first cycle of a potential shot,
                // we need to read the gamepad buttons to set the shot type
                if (shotCounter == 0 && !shooting())
                {
                    bool shootBarrel = gamepad.GetButton((uint)shootTrigger);
                    if (shootBarrel && currentShot == BarrelShotTypes.NOT_SHOOTING)
                    {
                        // Get shot selection
                        if (gamepad.GetButton((uint)lowShot))
                        {
                            currentShot = BarrelShotTypes.LOW_AIR_VOLUME;
                        }
                        else if (gamepad.GetButton((uint)mediumShot))
                        {
                            currentShot = BarrelShotTypes.MED_AIR_VOLUME;
                        }
                        else if (gamepad.GetButton((uint)highShot))
                        {
                            currentShot = BarrelShotTypes.HIGH_AIR_VOLUME;
                        }
                    }
                }

                if (shooting())
                {
                    // Read shot type to set what to tell spikes
                    bool triggerSpikeLow = (
                        currentShot == BarrelShotTypes.LOW_AIR_VOLUME ||
                        currentShot == BarrelShotTypes.HIGH_AIR_VOLUME
                    );
                    bool triggerSpikeHigh = (
                        currentShot == BarrelShotTypes.MED_AIR_VOLUME ||
                        currentShot == BarrelShotTypes.HIGH_AIR_VOLUME
                    );

                    // Write to spikes
                    spikeHigh.Write(triggerSpikeHigh);
                    spikeLow.Write(triggerSpikeLow);

                    // Track shot cycles expired
                    shotCounter++;

                    if (shotCounter >= ShootingConstants.SHOT_CYCLES)
                    {
                        // When max shot cycles expired, reset and start debounce
                        debounceCounter = ShootingConstants.DEBOUNCE_WAIT_CYCLES;
                        currentShot = BarrelShotTypes.NOT_SHOOTING;
                        shotCounter = 0;
                    }
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

        private bool shooting()
        {
            return currentShot != BarrelShotTypes.NOT_SHOOTING;
        }

        private bool debounceShot()
        {
            debounceCounter--;
            return debounceCounter <= 0;
        }
    }
}