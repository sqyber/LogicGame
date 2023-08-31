using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LogicGame
{
    public class Player : MonoBehaviour
    {
        public int initialRow, initialCol;
        public Transform target;
        [SerializeField] private Rigidbody2D rb;

        private void Start()
        {
            transform.position = new Vector3(initialCol, initialRow, 0);
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
/*
           //rotate the sprite towards move direction
           Vector3 relativePos = target.position - transform.position;
           Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.back);
           transform.rotation = rotation;
          */

            if (Input.GetKeyDown(KeyCode.D))
                transform.position = new Vector3(transform.position.x + 1, transform.position.y, 0);
            else if (Input.GetKeyDown(KeyCode.A))
                transform.position = new Vector3(transform.position.x - 1, transform.position.y, 0);
            else if (Input.GetKeyDown(KeyCode.W))
                transform.position = new Vector3(transform.position.x, transform.position.y + 1, 0);
            else if (Input.GetKeyDown(KeyCode.S))
                transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
        }
    }
}