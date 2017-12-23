using UnityEngine;
using System.Collections;

public class LockCursorOnClickScript : MonoBehaviour {
    public bool mouseLock = true;

    void Update()   {
        if (!mouseLock)
            Screen.lockCursor = false;
        if (mouseLock && !Screen.lockCursor)
            Screen.lockCursor = true;
    }
}