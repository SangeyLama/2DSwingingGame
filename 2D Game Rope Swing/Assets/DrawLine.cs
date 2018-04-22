﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public Vector3[] targets; // the objects to draw the line between
                                 // Use this for initialization
    private LineRenderer l;
    void Start()
    {
        l = this.GetComponent<LineRenderer>();
        Vector2 p1 = targets[0];
        l.SetPosition(0, p1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 p1 = targets[0];
        Vector2 p2 = targets[1];
        Vector2 lineVector = p2 - p1;
        l.SetPosition(0, p1);
        l.SetPosition(1, p2);
    }
}
