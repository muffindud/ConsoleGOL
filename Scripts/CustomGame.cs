using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading;

public class CustomGame : MonoBehaviour
{
    Cell[,] cellGrid = new Cell[Globals.gridWidth, Globals.gridHeight];
    Rule ruleSet = new Rule();
    System.Random rand = new System.Random();

    public float speed = 0.1f;
    public float timer = 0f;
    public bool randomization = false;
    public bool isPaused = true;
    public bool isRandom = false;

    void OnEnable()
    {
        string deathRules = PlayerPrefs.GetString("deathRules");
        string birthRules = PlayerPrefs.GetString("birthRules");
        int maxAge = PlayerPrefs.GetInt("maxAge");
        var d = Array.ConvertAll(deathRules.Split(' '), int.Parse);
        var b = Array.ConvertAll(birthRules.Split(' '), int.Parse);

        ruleSet.cellDeath = d;
        ruleSet.cellBirth = b;
        ruleSet.cellMaxAge = maxAge;
        ruleSet.cellAge = maxAge > 0;
    }

    void Start()
    {
        PlaceCells();
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
                // Increase speed
                if (speed >= 0.0f)
                {
                    speed -= 0.01f;
                }
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                // Decrease speed
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

            if (Input.GetMouseButton(0))
            {
                MouseInput();
            }

            DrawCells();
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
                cell.activeRule = ruleSet;
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

        // Count number of neighbors
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
                cellGrid[x, y].UpdateCell();
            }
        }
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
