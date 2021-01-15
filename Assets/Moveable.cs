using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    private float startPosX;
    private float startPosY;
    private bool isBeingHeld;

    public void OnMouseDown()
    {
        isBeingHeld = true;
    }

    public void OnMouseUp()
    {
        isBeingHeld = false;
    }

    void Update()
    {
        if (isBeingHeld)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);
        }
    }
}
