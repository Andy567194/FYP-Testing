using Fusion;
using UnityEngine;

public enum InputButton
{
    Jump,
    TSA
}

public struct InputData : INetworkInput
{
    public NetworkButtons JumpButton;
    public Vector2 MoveInput;
    public Angle Pitch;
    public Angle Yaw;
    public NetworkButtons TSAButton;
    public float ScrollInput;
}
