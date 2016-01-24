using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class Controls : MonoBehaviour
    {
        public Vector2 Cursor = new Vector2(6,6);

        void Update()
        {
            WorldMap map = GameObject.FindObjectOfType<WorldMap>();
            if (map != null)
            {
                Cursor.x -= Input.GetAxis("Horizontal") / 8;
                Cursor.y -= Input.GetAxis("Vertical") / 8;
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

                if (Input.GetButton("Street"))
                {
                    FindObjectOfType<GameFlow>().SetTile(GameFlow.PlacableTiles.Street);
                }
                if (Input.GetButton("Blockade"))
                {
                    FindObjectOfType<GameFlow>().SetTile(GameFlow.PlacableTiles.Blockade);
                }
                if (Input.GetButton("Checkpoint"))
                {
                    FindObjectOfType<GameFlow>().SetTile(GameFlow.PlacableTiles.Checkpoint);
                }
            }
            if (Input.GetButton("Start"))
                FindObjectOfType<GameFlow>().StartLevel();
        }

    }
}
