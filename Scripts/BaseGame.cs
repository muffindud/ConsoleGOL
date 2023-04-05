using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BaseGame : MonoBehaviour
{
    Cell[,] cellGrid;
    Rule baseRule;
    System.Random rand;

    public float speed;
    public float timer;
    public bool randomization;
    public bool isPaused;
    public bool isRandom;
    public bool isZone;
    
    Zone[,] zoneGrid;
    Rule zoneRule;

    int[] zoneX;
    int[] zoneY;

    void Awake()
    {
        cellGrid = new Cell[Globals.gridWidth, Globals.gridHeight];
        baseRule = new Rule();
        rand = new System.Random();
        zoneRule = new Rule();
        speed = 0.1f;
        timer = 0f;
        randomization = false;
        isPaused = true;
        isRandom = false;
        isZone = false;
        zoneX = new int[2];
        zoneY = new int[2];
        zoneX[0] = -1;
        zoneX[1] = -1;
        zoneY[0] = -1;
        zoneY[1] = -1;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("deathRules") || PlayerPrefs.HasKey("birthRules"))
        {
            string deathRules = PlayerPrefs.GetString("deathRules");
            string birthRules = PlayerPrefs.GetString("birthRules");
            int maxAge = PlayerPrefs.GetInt("maxAge");

            var d = new int[0];
            var b = new int[0];

            if (deathRules != " ")
            {
                d = Array.ConvertAll(deathRules.Split(' '), int.Parse);
            }
            else
            {
                d = new int[0];
            }

            if (birthRules != " ")
            {
                b = Array.ConvertAll(birthRules.Split(' '), int.Parse);
            }
            else
            {
                b = new int[0];
            }

            zoneX[0] = PlayerPrefs.GetInt("zoneX1");
            zoneX[1] = PlayerPrefs.GetInt("zoneX2");
            zoneY[0] = PlayerPrefs.GetInt("zoneY1");
            zoneY[1] = PlayerPrefs.GetInt("zoneY2");

            zoneRule.cellDeath = d;
            zoneRule.cellBirth = b;
            zoneRule.cellMaxAge = maxAge;
            zoneRule.cellAge = maxAge > 0;

            isZone = true;
            PlaceZone();
            PlaceCells();
            DrawCells();
        }
        else
        {
            PlaceCells();
            DrawCells();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }

        if (!isPaused)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (speed >= 0.0f)
                {
                    speed -= 0.01f;
                }
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                speed += 0.01f;
            }

            if (timer >= speed)
            {
                timer = 0f;
                CycleCells();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        else 
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                isRandom = !isRandom;
            }

            if (randomization != isRandom && isRandom)
            {
                RandomizeCells();
                randomization = isRandom;
            }
            else if (randomization != isRandom && !isRandom)
            {
                ClearCells();
                randomization = isRandom;
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                ResetZone();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                MouseInput();
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                SelectZone();
            }

            if (isZone && Input.GetKeyDown(KeyCode.M))
            {
                GoToZoneConfig();
            }
        }
    }

    // Internal functions

    void PlaceCells()
    {
        for (int x = 0; x < Globals.gridWidth; x++)
        {
            for (int y = 0; y < Globals.gridHeight; y++)
            {
                Cell cell = Instantiate(Resources.Load("Prefabs/Cell", typeof(Cell)), new Vector2(x, y), Quaternion.identity) as Cell;
                cell.activeRule = baseRule;
                cellGrid[x, y] = cell;
            }
        }
    }

    void RandomizeCells()
    {
        for (int x = 0; x < Globals.gridWidth; x++)
        {
            for (int y = 0; y < Globals.gridHeight; y++)
            {
                if (rand.NextDouble() > 0.8)
                {
                    cellGrid[x, y].isAlive = true;
                    cellGrid[x, y].age = 0;
                    cellGrid[x, y].Draw();
                }
            }
        }
    }

    void ClearCells()
    {
        for (int x = 0; x < Globals.gridWidth; x++)
        {
            for (int y = 0; y < Globals.gridHeight; y++)
            {
                cellGrid[x, y].isAlive = false;
                cellGrid[x, y].age = 0;
                cellGrid[x, y].Draw();
            }
        }
    }

    void MouseInput()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(mousePosition.x);
        int y = Mathf.RoundToInt(mousePosition.y);
        if (x >= 0 && x < Globals.gridWidth && y >= 0 && y < Globals.gridHeight)
        {
            cellGrid[x, y].isAlive = !cellGrid[x, y].isAlive;
            cellGrid[x, y].age = 0;
            cellGrid[x, y].Draw();
        }
    }

    void DrawCells()
    {
        for (int x = 0; x < Globals.gridWidth; x++)
        {
            for (int y = 0; y < Globals.gridHeight; y++)
            {
                cellGrid[x, y].Draw();
            }
        }
    }

    void CycleCells()
    {
        DrawCells();

        for (int x = 0; x < Globals.gridWidth; x++)
        {
            for (int y = 0; y < Globals.gridHeight; y++)
            {
                int neighbors = 0;
                
                if (x > 0 && y > 0 && cellGrid[x - 1, y - 1].isAlive) neighbors++;
                if (x > 0 && cellGrid[x - 1, y].isAlive) neighbors++;
                if (x > 0 && y < Globals.gridHeight - 1 && cellGrid[x - 1, y + 1].isAlive) neighbors++;
                if (y > 0 && cellGrid[x, y - 1].isAlive) neighbors++;
                if (y < Globals.gridHeight - 1 && cellGrid[x, y + 1].isAlive) neighbors++;
                if (x < Globals.gridWidth - 1 && y > 0 && cellGrid[x + 1, y - 1].isAlive) neighbors++;
                if (x < Globals.gridWidth - 1 && cellGrid[x + 1, y].isAlive) neighbors++;
                if (x < Globals.gridWidth - 1 && y < Globals.gridHeight - 1 && cellGrid[x + 1, y + 1].isAlive) neighbors++;

                cellGrid[x, y].neighbors = neighbors;
            }
        }

        for (int x = 0; x < Globals.gridWidth; x++)
        {
            for (int y = 0; y < Globals.gridHeight; y++)
            {
                if (x >= zoneX[0] && x <= zoneX[1] && y >= zoneY[0] && y <= zoneY[1])
                {
                    if (isZone)
                    {
                        cellGrid[x, y].activeRule = zoneRule;
                    }
                }
                else
                {
                    cellGrid[x, y].activeRule = baseRule;
                }

                cellGrid[x, y].UpdateCell();
            }
        }
    }

    void SelectZone()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(mousePosition.x);
        int y = Mathf.RoundToInt(mousePosition.y);

        if (x >= 0 && x < Globals.gridWidth && y >= 0 && y < Globals.gridHeight)
        {
            if (zoneX[0] == -1 || zoneX[1] != -1)
            {
                ResetZone();

                zoneX[0] = x;
                zoneY[0] = y;
            }
            else 
            {
                isZone = true;

                if (x < zoneX[0])
                {
                    zoneX[1] = zoneX[0];
                    zoneX[0] = x;
                }
                else
                {
                    zoneX[1] = x;
                }

                if (y < zoneY[0])
                {
                    zoneY[1] = zoneY[0];
                    zoneY[0] = y;
                }
                else
                {
                    zoneY[1] = y;
                }

                PlaceZone();
            }
        }
    }

    void ResetZone()
    {
        if (isZone)
        {
            ClearZone();
            isZone = false;
        }

        zoneX[0] = -1;
        zoneX[1] = -1;
        zoneY[0] = -1;
        zoneY[1] = -1;
    }

    void PlaceZone()
    {
        zoneGrid = new Zone[zoneX[1] - zoneX[0] + 1, zoneY[1] - zoneY[0] + 1];

        for (int x = zoneX[0]; x <= zoneX[1]; x++)
        {
            for (int y = zoneY[0]; y <= zoneY[1]; y++)
            {
                Zone zoneCell = Instantiate(Resources.Load("Prefabs/Zone", typeof(Zone)), new Vector3(x, y, 1), Quaternion.identity) as Zone;
                zoneGrid[x - zoneX[0], y - zoneY[0]] = zoneCell;
            }
        }

        DrawZone();
    }

    void DrawZone()
    {
       for (int x = zoneX[0]; x <= zoneX[1]; x++)
        {
            for (int y = zoneY[0]; y <= zoneY[1]; y++)
            {
                zoneGrid[x - zoneX[0], y - zoneY[0]].Draw();
            }
        }
    }

    void ClearZone()
    {
        for (int x = zoneX[0]; x <= zoneX[1]; x++)
        {
            for (int y = zoneY[0]; y <= zoneY[1]; y++)
            {
                Destroy(zoneGrid[x - zoneX[0], y - zoneY[0]].gameObject);
                Destroy(zoneGrid[x - zoneX[0], y - zoneY[0]]);
            }
        }
    }

    void GoToZoneConfig()
    {
        PlayerPrefs.SetInt("zoneX1", zoneX[0]);
        PlayerPrefs.SetInt("zoneX2", zoneX[1]);
        PlayerPrefs.SetInt("zoneY1", zoneY[0]);
        PlayerPrefs.SetInt("zoneY2", zoneY[1]);

        SceneManager.LoadScene("BaseMenuZone");
    }

    // External functions (Buttons)
    public void SetStart()
    {
        isPaused = !isPaused;
    }

    public void SetRandom()
    {
        if (isPaused)
        {
            isRandom = !isRandom;
        }
    }
}
