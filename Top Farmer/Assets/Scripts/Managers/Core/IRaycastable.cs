using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public interface IRaycastable 
{
   CursorType GetCursorType();
    bool HandleRaycast(PlayerController controller);
}
