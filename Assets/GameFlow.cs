using UnityEngine;
using System.Collections;
using Assets;
using System;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{

    public enum PlacableTiles { Street = 0, Blockade = 1, Checkpoint = 2 };

    public TimeSpan TimeLeft = TimeSpan.FromMinutes(1);

    public int[] Placable = new int[3];

    public int Level = 1;


    public float HealthMin = 0.2f;

    public float HappyMax = 0.5f;
    public float HappyMin = 0.15f;

    private Text InfoScreen;

    string Filename;

    private bool LevelStarted = false;

    private Disease DiseaseProto;

    // Use this for initialization
    void Start()
    {
        InfoScreen = GameObject.FindWithTag("InfoScreen").GetComponent<Text>();
        SetupLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelStarted)
        {
            TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(Time.fixedDeltaTime));
            
            if (TimeLeft.TotalSeconds == 0)
                WinLevel();

            if (TimeLeft.TotalSeconds % 5 == 0)
            {
                FindObjectOfType<HumanAI>().gameObject.AddComponent<Disease>();
            }
            float count = 0, infected = 0, happy = 0;
            foreach (HumanAI human in GameObject.FindObjectsOfType<HumanAI>())
            {
                ++count;
                happy += human.Happiness;
                if (human.IsInfected())
                    ++infected;
            }
            float totalHappy = happy / count / 255;
            float adjHappy = totalHappy;
            if (adjHappy > HappyMax)
                adjHappy = HappyMax;
            if (adjHappy < HappyMin)
                adjHappy = HappyMin;
            GameObject.FindWithTag("HappyScreen").GetComponent<Text>().text =
                string.Format("Happiness:\t{0}%",
                (int)((adjHappy - HappyMin)/(HappyMax-HappyMin)*100));
            if (count - infected / count <= HealthMin ||totalHappy <= HappyMin)
                LoseLevel();
        }
    }

    void SetupLevel()
    {
        switch (Level)
        {
            case 1:
                Placable[(int)PlacableTiles.Street] = 3;
                Placable[(int)PlacableTiles.Blockade] = 8;
                Placable[(int)PlacableTiles.Checkpoint] = 2;
                TimeLeft = TimeSpan.FromSeconds(60);
                Filename = @"Assets\Levels\Level.dat";
                DiseaseProto = new Disease();
                break;
            case 2:
                Placable[(int)PlacableTiles.Street] = 2;
                Placable[(int)PlacableTiles.Blockade] = 6;
                Placable[(int)PlacableTiles.Checkpoint] = 1;
                TimeLeft = TimeSpan.FromSeconds(60);
                Filename = @"Assets\Levels\Level.dat";
                DiseaseProto = new Disease();
                break;
            case 3:
                Placable[(int)PlacableTiles.Street] = 1;
                Placable[(int)PlacableTiles.Blockade] = 4;
                Placable[(int)PlacableTiles.Checkpoint] = 1;
                TimeLeft = TimeSpan.FromSeconds(60);
                Filename = @"Assets\Levels\Level.dat";
                DiseaseProto = new Disease();
                break;
        }

        InfoScreen.text = string.Format(
            "Welcome to Level {0}.\nPrevent the disease from spreading and keep the people happy!" +
            "\nSurvive {1} Seconds to clear this level.\n"+
            "If the amount of uninfected poeple drops below {2}%, you lose this level.\n"+
            "Press START to begin.",
            Level,
            (int)TimeLeft.TotalSeconds,
            (int)(HealthMin * 100));
    }

    public void StartLevel()
    {
        if (LevelStarted)
            return;
        InfoScreen.text = "";
        GameObject map = new GameObject();
        map.name = "map";
        map.AddComponent<WorldMap>();
        FindObjectOfType<WorldMap>().SpawnLevel(Filename);
        LevelStarted = true;
    }

    void StopLevel()
    {
        foreach (HumanAI human in FindObjectsOfType<HumanAI>())
            human.Die();
        GameObject.Destroy(FindObjectOfType<WorldMap>().gameObject);
        LevelStarted = false;
        //
    }

    void LoseLevel()
    {
        StopLevel();
        InfoScreen.text = "Sorry, you lost this level.\nPlease try again.\nPress START to continue.";
        SetupLevel();
    }

    void WinLevel()
    {
        StopLevel();
        InfoScreen.text = "Congratulations!\nYou Kept the epidemic at bay.\n" +
            "Advance to the next level.\nPress START to continue.";
        Level++;
        SetupLevel();
    }

    public void SetTile(PlacableTiles tile)
    {
        WorldMap map = FindObjectOfType<WorldMap>();
        if (Placable[(int)tile] > 0)
        {
            --Placable[(int)tile];
            switch (tile)
            {
                case PlacableTiles.Street:
                    map.SetTile((int)WorldMap.Tile.Street);
                    break;
                case PlacableTiles.Blockade:
                    map.SetTile((int)WorldMap.Tile.Blockade);
                    break;
                case PlacableTiles.Checkpoint:
                    map.SetTile((int)WorldMap.Tile.Checkpoint);
                    break;
            }

        }

    }
}
