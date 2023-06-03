using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Extensions;

public class CameraSignals : MonoSingleton<CameraSignals>
{
    public UnityAction onSetCameraTarget = delegate { };
}
