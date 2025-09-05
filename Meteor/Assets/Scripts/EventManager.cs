using System.Collections;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private float timer;
    public float eventRate = 2f;
    float Bduration = 15f;
    public float flashDuration = 0.2f; // duration per color

    public GameObject meteorAttack;

    public Camera mainCamera;
    private Vector3 originalCamPos;

    public float shakeDuration = 5f;
    public float shakeMagnitude = 10f;
    public float magnitudeDelta = 0.2f;

    public float speedUpeventDuration = 10f;
    public float spedUpRate = 3f;


    public GameObject screenObject;
    private Renderer objRenderer;

    private void Awake()
    {
        if (screenObject)
            objRenderer = screenObject.GetComponent<Renderer>();
        if (mainCamera)
            originalCamPos = mainCamera.transform.localPosition;
    }
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= eventRate)
        {
            //Event();
            Meteor();
            timer = 0f;
        }

        
    }
    private void Event()
    {
        int type = Random.Range(1, 6); // 1–4

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
        int safekeep = 0;
        int control = 0;
        while (elapsed < duration)
        {   
            elapsed += Time.deltaTime;
            /*safekeep = (int)elapsed;
            if (safekeep > control)
            {
                magnitude-=magnitudeDelta;
                control++;
            }*/


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
        // Ensure object is active
        screenObject.SetActive(true);

        // Black
        objRenderer.material.color = Color.black;
        yield return new WaitForSeconds(flashDuration);

        // White
        objRenderer.material.color = Color.white;
        yield return new WaitForSeconds(flashDuration);

        // Black again
        objRenderer.material.color = Color.black;
        yield return new WaitForSeconds(flashDuration);

        // Hide object
        screenObject.SetActive(false);
    }

}
