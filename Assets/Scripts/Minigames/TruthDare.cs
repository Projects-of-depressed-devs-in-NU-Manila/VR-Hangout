using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TruthDare : MonoBehaviour
{
    public Sprite[] truthCards;
    public Sprite[] dareCards;
    public Image cardImage;
    public Canvas canvas;

    public bool isTruth = true;

    void OnMouseDown()
    {
        RevealRandomCard();
    }

    void RevealRandomCard()
    {
        Sprite chosenCard;
        if (isTruth)
        {
            int index = Random.Range(0, truthCards.Length);
            chosenCard = truthCards[index];
        }
        else
        {
            int index = Random.Range(0, dareCards.Length);
            chosenCard = dareCards[index];
        }

        cardImage.sprite = chosenCard;
        cardImage.gameObject.SetActive(true);

        StartCoroutine(HideCardAfterDelay(8));
    }
    IEnumerator HideCardAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        cardImage.gameObject.SetActive(false);
    }
}
