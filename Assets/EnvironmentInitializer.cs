using UnityEngine;
using System.Collections;
using UnityEditor;

public class EnvironmentInitializer : MonoBehaviour {
    // Human Prefab - set in inspector
    public GameObject humanPrefab;
    // Human Prefab - set in inspector
    public GameObject housePrefab;
    // Tree Prefab - set in inspector
    public GameObject treePrefab;

    // Amount of humans generated
    public int humanCount;
    // Amount of houses generated
    public int houseCount;
    // Amount of trees generated
    public int treeCount;


    private int EnvId;
    private bool DoneGenerating = false;


    void Start()
    {
        EnvId = gameObject.GetInstanceID();
        //Init constant world objects
        InitWorld();
        CreateNavMesh();
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(new Vector3(201.5f, 0.52f, 245.3f), new Vector3(201.5f, 0.52f, 251.3f), 1, path);
        Vector3 tester = gameObject.transform.lossyScale;

        //CreateNavMesh(); // we might have to create the navmesh after generating the humans - probably not (because the navAgents avoid crossing each others ways)
    }

    private void CreateNavMesh()
    {
        //Create a NavMesh

        //Mark all gameObjects as "Navigation static"

        //Set parameters for "baking" the NavMesh

        //"Bake"
        NavMeshBuilder.BuildNavMesh();

    }

    private void InitWorld()
    {
        //1. Add houses randomly spread over the attached gameObject

        for (int i = 0; i < houseCount; i++)
        {
            Instantiate(housePrefab,
                new Vector3(
     /*x*/      gameObject.transform.position.x + UnityEngine.Random.Range(0.0f, gameObject.transform.localScale.x) - gameObject.transform.localScale.x / 2,
     /*y*/      gameObject.transform.position.y + gameObject.transform.localScale.y / 2 + housePrefab.transform.localScale.y / 2,
     /*z*/      gameObject.transform.position.z + UnityEngine.Random.Range(0.0f, gameObject.transform.localScale.z) - gameObject.transform.localScale.z / 2),
     /*rot*/    new Quaternion(0, Random.Range(0.0f, 360.0f), 0, 0)
                );
        }
        //2. Add trees to the remaining space, if there is enough room for a forest - add a forest

        for (int i = 0; i < treeCount; i++)
        {
            Instantiate(treePrefab, 
                new Vector3(
     /*x*/      gameObject.transform.position.x + UnityEngine.Random.Range(0.0f, gameObject.transform.localScale.x) - gameObject.transform.localScale.x / 2,
     /*y*/      gameObject.transform.position.y + gameObject.transform.localScale.y / 2 + treePrefab.transform.localScale.y / 2,
     /*z*/      gameObject.transform.position.z + UnityEngine.Random.Range(0.0f,gameObject.transform.localScale.z) - gameObject.transform.localScale.z / 2), 
     /*rot*/    new Quaternion(0,Random.Range(0.0f, 360.0f),0,0)
                );
        }

        //3. Add food/water-repositories

        //4. Add humans
        for (int i = 0; i < humanCount; i++)
        {
            Instantiate(humanPrefab, 
                new Vector3(
     /*x*/      gameObject.transform.position.x + UnityEngine.Random.Range(0.0f, gameObject.transform.localScale.x) - gameObject.transform.localScale.x / 2,
     /*y*/      gameObject.transform.position.y + gameObject.transform.localScale.y / 2 + humanPrefab.transform.localScale.y / 2,
     /*z*/      gameObject.transform.position.z + UnityEngine.Random.Range(0.0f, gameObject.transform.localScale.z) - gameObject.transform.localScale.z / 2), 
     /*rot*/    new Quaternion(0, Random.Range(0.0f, 360.0f), 0, 0)
                );
    }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
