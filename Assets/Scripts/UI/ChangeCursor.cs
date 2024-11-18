using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Texture2D mouseIcon;

    void Start()
    {
        Cursor.SetCursor(mouseIcon, Vector2.zero, CursorMode.Auto);
    }
 
}
