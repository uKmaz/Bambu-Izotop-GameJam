using System.Collections;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private float timer;
    public float eventRate;
    public float flashDuration = 0.6f;

    public GameObject meteorAttack;

    public Camera mainCamera;
    private Vector3 originalCamPos;

    public float shakeDuration = 5f;
    public float shakeMagnitude = 10f;
    public float magnitudeDelta = 0.2f;

    public float speedUpeventDuration = 10f;
    public float spedUpRate = 3f;


    public GameObject screenObject;
    private SpriteRenderer objRenderer;

    private void Awake()
    {
        if (screenObject)
            objRenderer = screenObject.GetComponent<SpriteRenderer>();
        if (mainCamera)
            originalCamPos = mainCamera.transform.localPosition;
    }
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= eventRate)
        {
            Event();
            timer = 0f;
        }

        
    }
    private void Event()
    {
        int type = Random.Range(1, 6); 

        switch (type)
        {
            case 1:
                Meteor();
                break;
            case 2:
                StartSpedUp();
                break;
            case 3:
                Exchange();
                break;
            case 4:
                Darkness();
                break;
            case 5:
                ScreenShake();
                break;
        }
    }
    private void Meteor()
    {
        Instantiate(meteorAttack);
    }
    private void StartSpedUp()
    {
        Time.timeScale *= spedUpRate;
        Invoke("EndSpedUp",speedUpeventDuration);

    }
    private void EndSpedUp()
    {
        Time.timeScale = 1f;
    }

    private void Exchange() {
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
            float offsetX = Random.Range(-1f, 1f) * (magnitude-elapsed*magnitudeDelta);
            float offsetY = Random.Range(-1f, 1f) * (magnitude-elapsed*magnitudeDelta);

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

        for(int i = 0; i < 3; i++)
        {
            screenObject.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            screenObject.SetActive(false);
            yield return new WaitForSeconds(flashDuration);

        }

    }

}
