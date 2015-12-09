using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class Controls : MonoBehaviour
    {
        public Vector2 Cursor = new Vector2(0,0);

        void Update()
        {
            WorldMap map = GameObject.FindObjectOfType<WorldMap>();
            Cursor.x += Input.GetAxis("Horizontal") / 8;
            Cursor.y += Input.GetAxis("Vertical") / 8;
            if (Input.GetAxis("Horizontal") == 0)
                Cursor.x = (float)Math.Round((double)Cursor.x);
            if (Input.GetAxis("Vertical") == 0)
                Cursor.y = (float)Math.Round((double)Cursor.y);
            if (Cursor.x >= map.Dimensions.x)
                Cursor.x = map.Dimensions.x - 1;
            if (Cursor.y >= map.Dimensions.y)
                Cursor.y = map.Dimensions.y - 1;
            if (Cursor.x < 0)
                Cursor.x = 0;
            if (Cursor.y < 0)
                Cursor.y = 0;
        }

    }
}
