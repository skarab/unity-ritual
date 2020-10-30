using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkTransform : MonoBehaviour
{
    public Transform Source;

    void Start()
    {
        Link();
    }

    void LateUpdate()
    {
        Link();
    }

    void Link()
    {
        transform.position = Source.position;
        transform.rotation = Source.rotation;
    }
}
