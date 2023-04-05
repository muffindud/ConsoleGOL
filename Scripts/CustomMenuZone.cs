using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class CustomMenuZone : MonoBehaviour
{
    bool[] deathRules   = {true,  true,  false, false, true,  true,  true};
    bool[] neutralRules = {false, false, true,  false, false, false, false};
    bool[] birthRules   = {false, false, false, true,  false, false, false};

    int deahthN = 5;
    int neutralN = 1;
    int birthN = 1;

    int[] death;
    int[] neutral;
    int[] birth;

    int maxAge = 0;
    
    public TMP_InputField maxAgeInput;
    public TextMeshProUGUI deathRulesText;
    public TextMeshProUGUI birthRulesText;
    public TextMeshProUGUI ageRulesText;

    void OnEnable()
    {
        SetRules();
    }

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log(maxAge.ToString());
        }
    }


    // Internal functions

    void SetRules()
    {
        death = new int[deahthN];
        neutral = new int[neutralN];
        birth = new int[birthN];

        for (int i = 0, j = 0; i < 7; i++)
        {
            if (deathRules[i])
            {
                death[j] = i;
                j++;
            }
        }

        for (int i = 0, j = 0; i < 7; i++)
        {
            if (neutralRules[i])
            {
                neutral[j] = i;
                j++;
            }
        }

        for (int i = 0, j = 0; i < 7; i++)
        {
            if (birthRules[i])
            {
                birth[j] = i;
                j++;
            }
        }

        deathRulesText.text = "Death rules(neighbours): \n" + string.Join(", ", death);
        birthRulesText.text = "Birth rules(neighbours): \n" + string.Join(", ", birth);
        ageRulesText.text = "Max age[>=0:disabled]: \n" + maxAge.ToString();
    }


    // External functions (Buttons)

    public void SetDeathRules(int n)
    {
        if (!deathRules[n])
        {
            deahthN++;
            if (neutralRules[n])
            {
                neutralN--;
            }
            else if (birthRules[n])
            {
                birthN--;
            }
        }

        deathRules[n] = true;
        neutralRules[n] = false;
        birthRules[n] = false;

        SetRules();
    }

    public void SetNeutralRules(int n)
    {
        if (!neutralRules[n])
        {
            neutralN++;
            if (deathRules[n])
            {
                deahthN--;
            }
            else if (birthRules[n])
            {
                birthN--;
            }
        }

        deathRules[n] = false;
        neutralRules[n] = true;
        birthRules[n] = false;
        SetRules();
    }

    public void SetBirthRules(int n)
    {
        if (!birthRules[n])
        {
            birthN++;
            if (deathRules[n])
            {
                deahthN--;
            }
            else if (neutralRules[n])
            {
                neutralN--;
            }
        }

        deathRules[n] = false;
        neutralRules[n] = false;
        birthRules[n] = true;
        SetRules();
    }

    public void SetMaxAge()
    {
        maxAge = int.Parse(maxAgeInput.text);
        SetRules();
    }

    public void Confirm(string scene)
    {
        SetRules();
        
        string deathRulesString = string.Join(" ", death);
        string birthRulesString = string.Join(" ", birth);
        
        if (deathRulesString == "")
        {
            deathRulesString = " ";
        }
        
        if (birthRulesString == "")
        {
            birthRulesString = " ";
        }
        
        PlayerPrefs.SetString("deathRulesZone", deathRulesString);
        PlayerPrefs.SetString("birthRulesZone", birthRulesString);
        PlayerPrefs.SetInt("maxAgeZone", maxAge);
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}
