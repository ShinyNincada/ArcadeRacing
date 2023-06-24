using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOnSpawn : MonoBehaviour
{
    //Component
    SpriteRenderer sprite;

    public Color rootColor;

    private void OnEnable() {
        sprite.color = rootColor;
    }
}
