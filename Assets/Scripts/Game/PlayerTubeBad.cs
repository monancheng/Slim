﻿using UnityEngine;

public class PlayerTubeBad : MonoBehaviour
{
    private void Update()
    {
        Move();
        if (transform.position.y < -24f)
        {
            Destroy(gameObject);
            return;
        }
        var color = gameObject.GetComponent<MeshRenderer>().material.color;
        if (color.a > 0f)
        {
            color.a -= 0.05f;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
//            gameObject.GetComponent<MeshRenderer>().material.color = color;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    protected void Move()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - TubeManager.CurrentSpeed * Time.deltaTime,
            transform.position.z);
    }
}