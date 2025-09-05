using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    private int typeX = 0;
    [Header("Event References")]

    private float timer;
    public float eventRate;

    [Header("UI References")]
    public TextMeshProUGUI countdownText;

    public Image EventTimerImage;

    private string eventIsComing = "The Next event Will Come in :";
    private string upcomingEventName = "";
    private bool evenentInPlace = false;
    private float nextEventTime = 0f;

    [Header("MeteorPrefab")]

    public GameObject meteorAttack;

    [Header("Camera References(For Shaking)")]
    public Camera mainCamera;
    private Vector3 originalCamPos;

    [Header("Shake References")]
    public float shakeDuration = 5f;
    public float shakeMagnitude = 10f;
    public float magnitudeDelta = 0.2f;

    [Header("SpedUp References")]
    public float speedUpeventDuration = 10f;
    public float spedUpRate = 3f;

    [Header("Darkness References")]

    public GameObject screenObject;
    private SpriteRenderer objRenderer;
    public float flashDuration = 0.6f;


    private void Awake()
    {
        if (screenObject)
            objRenderer = screenObject.GetComponent<SpriteRenderer>();
        if (mainCamera)
            originalCamPos = mainCamera.transform.localPosition;
        EventTimerImage.fillAmount = 0f;


    }
    private void Update()
    {
        if (!evenentInPlace)
        {
            timer += Time.deltaTime;

            countdownText.text = (eventRate - (int)timer).ToString();
            EventTimerImage.fillAmount = (timer / eventRate);

            if (timer >= eventRate)
            {
                //Event();
                typeX = choseEvent();
                evenentInPlace = true;
                timer = 0f;
            }
        }
        else
        {
            timer += Time.deltaTime;

            EventTimerImage.fillAmount = timer / 3;
            countdownText.text = (3 - (int)timer).ToString();

            if (timer >= 3)
            {
                Event(typeX);
                timer = 0f;
            }

        }


    }
    private int choseEvent()
    {
        int type = Random.Range(1, 6);
        switch (type)
        {
            case 1:
                upcomingEventName = "Meteor Strike";

                break;
            case 2:
                upcomingEventName = "StartSpedUp";

                break;
            case 3:
                upcomingEventName = "Exchange";

                break;
            case 4:
                upcomingEventName = "Darkness";
                break;
            case 5:

                upcomingEventName = "ScreenShake";
                break;
        }
        return type;
    }

    private void Event(int type)
    {
        switch (type)
        {
            case 1:
                upcomingEventName = "Meteor Strike";
                evenentInPlace = true;
                Meteor();
                break;
            case 2:
                upcomingEventName = "StartSpedUp";
                evenentInPlace = true;
                StartSpedUp();
                break;
            case 3:
                upcomingEventName = "Exchange";
                evenentInPlace = true;
                Exchange();
                break;
            case 4:
                upcomingEventName = "Darkness";
                evenentInPlace = true;
                Darkness();
                break;
            case 5:

                upcomingEventName = "ScreenShake";
                evenentInPlace = true;
                ScreenShake();
                break;
        }
        evenentInPlace = false;
        countdownText.text = "";
    }

    private void Meteor()
    {
        Instantiate(meteorAttack);
    }
    private void StartSpedUp()
    {
        Time.timeScale *= spedUpRate;
        Invoke("EndSpedUp", speedUpeventDuration);

    }
    private void EndSpedUp()
    {
        Time.timeScale = 1f;
    }


    private void Exchange()
    {
        PlayerSpawner.instance.ExchangePlayers();
        Invoke("deExchange", 2f);
    }
    private void deExchange()
    {
        PlayerSpawner.instance.ExchangePlayers();
    }

    /// <summary>
    /// SHAKE IT SHAKE IT
    /// </summary>
    private void ScreenShake()
    {
        if (mainCamera)
            StartCoroutine(DoShake(shakeDuration, shakeMagnitude));
    }
    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;


            // Random offset
            float offsetX = Random.Range(-1f, 1f) * (magnitude - elapsed * magnitudeDelta);
            float offsetY = Random.Range(-1f, 1f) * (magnitude - elapsed * magnitudeDelta);

            mainCamera.transform.localPosition = originalCamPos + new Vector3(offsetX, offsetY, 0f);


            yield return null;
        }

        // Reset camera
        mainCamera.transform.localPosition = originalCamPos;
    }


    // Call this to flash: black -> white -> black


    /// <summary>
    /// DARKNESS
    /// </summary>
    private void Darkness()
    {
        FlashBlackWhiteBlack();

    }
    public void FlashBlackWhiteBlack()
    {
        if (screenObject)
            StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        objRenderer.color = Color.black;

        for (int i = 0; i < 3; i++)
        {
            screenObject.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            screenObject.SetActive(false);
            yield return new WaitForSeconds(flashDuration);

        }

    }


}
