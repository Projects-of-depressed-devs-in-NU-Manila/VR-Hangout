using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AvatarSelection : MonoBehaviour
{
    public List<GameObject> characters;
    public GameObject buttonPrefab;
    public Transform buttonPanel;

    private int currentIndex = 0;
    private string characterName;

    void Start()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            int index = i;
            GameObject newButton = Instantiate(buttonPrefab, buttonPanel);
            newButton.GetComponentInChildren<TMP_Text>().text = characters[i].name;
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(index));
        }

        foreach (var character in characters)
            character.SetActive(false);

        if (characters.Count > 0)
        {
            characters[0].SetActive(true);
            currentIndex = 0;
            characterName = characters[0].name;
        }
        Debug.Log("Loaded number of characters: " + characters.Count);
    }

    void SelectCharacter(int index)
    {
        if (index < 0 || index >= characters.Count) return;

        if (currentIndex >= 0)
            characters[currentIndex].SetActive(false);

        characters[index].SetActive(true);
        currentIndex = index;   
        characterName = characters[index].name;
        PlayerContext.Instance.playerCharacter = characterName;
    }

    public string getCharacterName()
    {
        return characterName;
    }

    public int getCharacterIndex()
    {
        return currentIndex;
    }
}
