using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManage : MonoBehaviour
{
    Texture2D amaing;
    Texture2D original;

    void Start()
    {
        amaing = Resources.Load<Texture2D>("hand");
        original = Resources.Load<Texture2D>("original");
    }

    public void OnMouseOver()
    {
        Cursor.SetCursor(amaing, new Vector2(amaing.width / 3, 0), CursorMode.Auto);
    }

    public void OnMouseExit()
    {
        Cursor.SetCursor(original, new Vector2(0, 0), CursorMode.Auto);
    }
}