using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets
{
    class WorldMap : MonoBehaviour
    {
        public enum Tile {
            Nothing     = 0,
            Street      = 1,
            Hospital    = 2,
            Bar         = 3,
            Office      = 4,
            Home        = 5,
            Mall        = 6,
            Blockade    = 7,
            Checkpoint  = 8
        };

        Tile[,] tiles;
        GameObject[,] visualTiles;
        public string Filename;
        const char Sep = ',';
        
        public const int Humans = 100;

        public Vector2 Dimensions;
        public PathMap Distance;
        public PathMap InfectedDistance;

        public class PathMap
        {
            public float[,] Distance;
            public int DimX, DimY;

            public int[,] ShortestPath;

            public PathMap(int dimX, int dimY)
            {
                DimX = dimX;
                DimY = dimY;
                Distance = new float[dimX*dimY, dimX*dimY];
                ShortestPath = new int[dimX * dimY, dimX * dimY];
            }
            
            public float CalcDistance (Vector2 src, Vector2 target)
            {
                return Distance[VecToInt(src), VecToInt(target)];
            }

            int VecToInt(Vector2 v)
            {
                if (v.x < 0)
                    v.x = 0;
                if (v.x > DimX - 1)
                    v.x = DimX - 1;
                if (v.y < 0)
                    v.y = 0;
                if (v.y > DimY - 1)
                    v.y = DimY - 1;
                return (int)Math.Round(v.x) + (int)Math.Round(v.y) * DimX;
            }

            Vector2 IntToVec(int i)
            {
                return new Vector2(i % DimX, i / DimX);
            }

            public Vector2 NextPoint(Vector2 pos, Vector2 target)
            {
                return IntToVec(ShortestPath[VecToInt(pos), VecToInt(target)]);
            }

            public float this[Vector2 a, Vector2 b]
            {
                get
                {
                    return Distance[VecToInt(a), VecToInt(b)];
                }

                set
                {
                    Distance[VecToInt(a), VecToInt(b)] = value;
                }
            }
            // set all values to infinite/ max
            public void Init()
            {
                for (int y = 0; y < DimY*DimX; ++y)
                    for (int x = 0; x < DimY*DimX; ++x)
                    {
                        if (x == y)
                            Distance[x, y] = 1.0f;
                        else
                            Distance[x, y] = float.PositiveInfinity;
                        
                        ShortestPath[x, y] = y;
                    }
            }
        }

        private bool Spawned = false;

        public Tile this[int x, int y]{
            set
            {
                tiles[x, y] = value;
                //UpdateVisual();
            }

            get
            {
                return tiles[x, y];
            }
        }

        protected void Start()
        {
            
        }

        public void SpawnLevel(string filename)
        {
            Filename = filename;
            Load();
            Dimensions = new Vector2(tiles.GetLength(0), tiles.GetLength(1));
            Distance = new PathMap(tiles.GetLength(0), tiles.GetLength(1));
            InfectedDistance = new PathMap(tiles.GetLength(0), tiles.GetLength(1));
            CreatePlanes();
            CreatePlaces();
            UpdateVisual();
            UpdatePaths(Distance);
            UpdatePaths(InfectedDistance, true);
            SpawnHumans();
            Spawned = true;
        }

        private void SpawnHumans()
        {
            for (int i = 0; i < Humans; ++i)
            {
                GameObject human = 
                    HumanFactory.CreateHuman();
                human.transform.SetParent(transform);
                int x = Random.Range(1, 9);
                int y = Random.Range(1, 9);
                while (tiles[x, y] != Tile.Street)
                {
                    x = Random.Range(1, 9);
                    y = Random.Range(1, 9);
                }
                human.transform.localPosition = new Vector3(x, 0.01f, y);
            }
        }

        private void CreatePlaces()
        {
            for (int y = 0; y < tiles.GetLength(1); ++y)
                for (int x = 0; x < tiles.GetLength(0); ++x)
                {
                    GameObject place;
                    switch (tiles[x, y])
                    {
                        case Tile.Hospital:
                            place = PlaceFactory.CreatePlace(Place.Type.Hospital, x, y);
                            place.transform.SetParent(transform);
                            break;
                        case Tile.Home:
                            place = PlaceFactory.CreatePlace(Place.Type.Home, x, y);
                            place.transform.SetParent(transform);
                            break;
                        case Tile.Mall:
                            place = PlaceFactory.CreatePlace(Place.Type.Shop, x, y);
                            place.transform.SetParent(transform);
                            break;
                        case Tile.Bar:
                            place = PlaceFactory.CreatePlace(Place.Type.Bar, x, y);
                            place.transform.SetParent(transform);
                            break;
                        case Tile.Office:
                            place = PlaceFactory.CreatePlace(Place.Type.Office, x, y);
                            place.transform.SetParent(transform);
                            break;
                    }
                }
        }

        private void CreatePlanes()
        {
            visualTiles = new GameObject[tiles.GetLength(0),tiles.GetLength(1)];
            for (int y = 0; y < tiles.GetLength(1); ++y)
                for (int x = 0; x < tiles.GetLength(0); ++x)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    go.transform.localPosition = new Vector3(x, -0.1f, y);
                    go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    go.transform.SetParent(transform);
                    visualTiles[x, y] = go;
                }
                 
               
        }

        protected void Load()
        {
            string[] lines = File.ReadAllLines(Filename);
            tiles = new Tile[lines[0].Split(Sep).Count(), lines.Count()];
            for(int y = 0; y < tiles.GetLength(1); ++y)
            {
                string[] line = lines[y].Split(Sep) ;
                for (int x = 0; x < tiles.GetLength(0); ++x)
                {
                    tiles[x, y] = (Tile)int.Parse(line[x]);
                }
            }
        }

        void Update()
        {
            if (Spawned)
                UpdateVisual();
            
        }

        private void UpdateVisual()
        {
            for (int y = 0; y < tiles.GetLength(1); ++y)
                for (int x = 0; x < tiles.GetLength(0); ++x)
                {
                    var r = visualTiles[x, y].GetComponent<Renderer>();
                    switch (tiles[x, y])
                    {
                        case Tile.Nothing:
                            r.material.color = Color.green;
                            break;
                        case Tile.Street:
                            r.material.color = Color.grey;
                            break;
                        case Tile.Home:
                            r.material = (Material)Resources.Load("Home");
                            break;
                        case Tile.Hospital:
                            r.material = (Material)Resources.Load("Hospital");
                            break;
                        case Tile.Office:
                            r.material = (Material)Resources.Load("Office");
                            break;
                        case Tile.Bar:
                            r.material = (Material)Resources.Load("Bar");
                            break;
                        case Tile.Mall:
                            r.material = (Material)Resources.Load("Shop");
                            break;
                        case Tile.Blockade:
                            r.material = (Material)Resources.Load("Blockade");
                            break;
                        case Tile.Checkpoint:
                            r.material = (Material)Resources.Load("Checkpoint");
                            break;
                    }
                }
            Controls c = GameObject.FindObjectOfType<Controls>();
            var re = visualTiles[(int)Math.Round(c.Cursor.x), (int)Math.Round(c.Cursor.y)].GetComponent<Renderer>();
            re.material = (Material)Resources.Load("default");
            re.material.color = Color.white;
        }

        private void UpdatePaths(PathMap distanceMap, bool infected = false)
        {
            distanceMap.Init();
            for (int y = 0; y < tiles.GetLength(1); ++y)
                for (int x = 0; x < tiles.GetLength(0); ++x)
                {
                    Vector2 pos = new Vector2(x, y);
                    switch (tiles[x, y])
                    {
                        case Tile.Street:
                            distanceMap[pos, new Vector2(pos.x + 1, pos.y)] = 1.0f;
                            distanceMap[pos, new Vector2(pos.x, pos.y - 1)] = 1.0f;
                            distanceMap[pos, new Vector2(pos.x - 1, pos.y)] = 1.0f;
                            distanceMap[pos, new Vector2(pos.x, pos.y + 1)] = 1.0f;
                            break;
                        case Tile.Checkpoint:
                            if (!infected)
                            {
                                distanceMap[pos, new Vector2(pos.x + 1, pos.y)] = 1.0f;
                                distanceMap[pos, new Vector2(pos.x, pos.y - 1)] = 1.0f;
                                distanceMap[pos, new Vector2(pos.x - 1, pos.y)] = 1.0f;
                                distanceMap[pos, new Vector2(pos.x, pos.y + 1)] = 1.0f;
                            }
                            break;
                        case Tile.Hospital:
                        case Tile.Home:
                        case Tile.Office:
                        case Tile.Bar:
                        case Tile.Mall:
                            distanceMap[pos, new Vector2(pos.x, pos.y + 1)] = 1.0f;
                            break;   
                    }                  
                }
            // Floyd
            for (int k = 0; k < distanceMap.DimX * distanceMap.DimY; ++k)
                for (int i = 0; i < distanceMap.DimX * distanceMap.DimY; ++ i)
                    for (int j = 0; j < distanceMap.DimX * distanceMap.DimY; ++j)
                    {
                        if (distanceMap.Distance[i, k] + distanceMap.Distance[k, j] < distanceMap.Distance[i, j])
                        {
                            distanceMap.Distance[i, j] = distanceMap.Distance[i, k] + distanceMap.Distance[k, j];
                            distanceMap.ShortestPath[i, j] = distanceMap.ShortestPath[i, k];
                        }
                    }
        }

        public void SetTile(int type)
        {
            Controls c = GameObject.FindObjectOfType<Controls>();
            Tile[] buildableTiles = new Tile[] { Tile.Nothing, Tile.Street, Tile.Blockade, Tile.Checkpoint };
            int x = (int)Math.Round(c.Cursor.x);
            int y = (int)Math.Round(c.Cursor.y);
            
            if (tiles[x, y] != (Tile)type && buildableTiles.Contains(tiles[x, y]))
            {
                //if(ressources[tiles[x][y]] > 0 { ressources--;
                tiles[x, y] = (Tile)type;
                UpdatePaths(Distance);
                UpdatePaths(InfectedDistance, true);              
            }
        }
    }

}