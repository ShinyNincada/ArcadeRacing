using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class GhostCarReplay : ISerializationCallbackReceiver
{
    [System.NonSerialized]
    public Vector2 position = Vector2.zero;

    [System.NonSerialized]
    public float rotationZ = 0;

    [System.NonSerialized]
    public float timeSinceLevelLoaded = 0;

    [System.NonSerialized]
    public Vector3 localScale = Vector3.one;
    //To preserve size we round off the values of the floats. This way we can keep the file size down
    [SerializeField]
    int x = 0;

    [SerializeField]
    int y = 0;

    [SerializeField]
    int r = 0;

    [SerializeField]
    int t = 0;
    [SerializeField]
    int s = 0;

    public GhostCarReplay(Vector2 _position, float _rotation, Vector3 _localScale, float _timeSinceLevelLoaded){
        position = _position;
        rotationZ = _rotation;
        timeSinceLevelLoaded = _timeSinceLevelLoaded;
        localScale = _localScale;
    }
    public void OnBeforeSerialize(){
        t = (int)(timeSinceLevelLoaded * 1000.0f);

        x  = (int) (position.x / 1000.0f);
        y  = (int) (position.y / 1000.0f);

        s = (int)(localScale.x * 1000.0f);

        r = Mathf.RoundToInt(rotationZ);
    }

    public void OnAfterDeserialize(){
        timeSinceLevelLoaded = t / 1000.0f;
        position.x = x / 1000.0f;
        position.y = y / 1000.0f;
        localScale = new Vector3(s/1000.0f, s/1000.0f, s/1000.0f);

        //Rotation doesn't need any decimals so we just kêep it as an int
        rotationZ = r;
    }
}
