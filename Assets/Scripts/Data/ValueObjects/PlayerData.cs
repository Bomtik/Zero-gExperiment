using System;
using Enums;

namespace Data.ValueObjects
{
    [Serializable]
    public struct PlayerData
    {
        public MovementData MovementData;
    }

    [Serializable]
    public struct MovementData
    {
        public float ForwardSpeed;
        public float JumpVelocity;
        public PlayerState StatePlayer;

        public MovementData(float forwardSpeed, float jumpVelocity, PlayerState state)
        {
            ForwardSpeed = forwardSpeed;
            JumpVelocity = jumpVelocity;
            StatePlayer = state;
        }
    }
}