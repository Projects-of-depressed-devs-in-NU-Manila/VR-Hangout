using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fishgame : MonoBehaviour
{
    public RectTransform greenBar;
    public RectTransform fishIcon;
    public GameObject youWon;
    public GameObject youLost;
    public GameObject parentGO;
    public Slider catchMeterSlider;
    public PlayerMovement player;

    public float fillSpeed = 0.1f;
    public float drainSpeed = 0.2f;

    private float catchProgress = 0.5f;
    private bool gameActive = false; 

    void OnEnable()
    {
        ResetGame();
    }

    void Update()
    {
        if (!gameActive)
            return;

        bool isOverlapping = RectOverlaps(greenBar, fishIcon);

        if (isOverlapping)
            catchProgress += fillSpeed * Time.deltaTime;
        else
            catchProgress -= drainSpeed * Time.deltaTime;

        catchProgress = Mathf.Clamp01(catchProgress);
        catchMeterSlider.value = catchProgress;

        if (catchProgress >= 1f)
        {
            FishGameWon();
        }
        else if (catchProgress <= 0f)
        {
            FishGameLost();
        }
    }

    public void ResetGame()
    {
        catchProgress = 0.3f;
        catchMeterSlider.value = catchProgress;
        gameActive = true;
        youWon.SetActive(false);
        youLost.SetActive(false);
        parentGO.SetActive(true);
    }

    void FishGameWon()
    {
        gameActive = false;
        Debug.Log("Fish Caught!");
        youWon.SetActive(true);
        StartCoroutine(HideWinScreenAfterDelay(2f));
    }

    void FishGameLost()
    {
        gameActive = false;
        Debug.Log("Fish Lost!");
        youLost.SetActive(true);
        StartCoroutine(HideLoseScreenAfterDelay(2f));
    }

    IEnumerator HideWinScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        youWon.SetActive(false);
        parentGO.SetActive(false);
        player.SetFishing(false);
    }

    IEnumerator HideLoseScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        youLost.SetActive(false);
        parentGO.SetActive(false);
        player.SetFishing(false);
    }

    bool RectOverlaps(RectTransform a, RectTransform b)
    {
        return RectTransformToScreenRect(a).Overlaps(RectTransformToScreenRect(b));
    }

    Rect RectTransformToScreenRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector2 bottomLeft = corners[0];
        Vector2 topRight = corners[2];
        return new Rect(bottomLeft, topRight - bottomLeft);
    }
}
