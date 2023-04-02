using System;

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

        public MovementData(float forwardSpeed, float jumpVelocity)
        {
            ForwardSpeed = forwardSpeed;
            JumpVelocity = jumpVelocity;
        }
    }
}