using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HideController : MonoBehaviour
{
    public void HidePlayer(bool state)
    {
        PlayerController.Instance.HidePlayer(state);
    }
}
