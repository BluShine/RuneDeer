using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class DreamMenu : MonoBehaviour
{
    PhotoStorage storage;

    public Text hintText;
    public RawImage filmImage;
    public Text filmText;

    [HideInInspector]
    public float hintTimer = 0;
    static float HINTWAIT = 10;

    GlobalFog fog;
    float fogDefault;
    bool fogTween = true;
    static float FOGSPEED = 5f;

    public Transform titleScreen;

    float eyesTimer = 0;
    bool eyesClosing = false;

    public GameObject eyes;
    public AudioSource music;

    public float filmCooldown = 5;
    float filmTimer = 0;
    public float rotationSpeed = 360;
    public int filmShots = 5;

    public void Start()
    {
        Weather weather = FindObjectOfType<Weather>();
        if(weather != null)
        {
            //set fog gradient
            GlobalFog f = FindObjectOfType<GlobalFog>();
            f.fogGradient = weather.transform.Find("FogGradient").GetComponent<GradientToTexture>();
            f.applyGradient();
            //set skybox
            RenderSettings.skybox = weather.skybox;
        }
        storage = FindObjectOfType<PhotoStorage>();
        fog = FindObjectOfType<GlobalFog>();
        fogDefault = fog.endDistance;
        fog.endDistance = 0;
    }

    public void Update()
    {
        filmTimer -= Time.deltaTime;
        filmTimer = Mathf.Max(0, filmTimer);
        if (filmTimer > 0)
        {
            filmImage.enabled = true;
            filmImage.transform.rotation = Quaternion.Euler(filmImage.transform.rotation.eulerAngles + 
                new Vector3(0, 0, Time.deltaTime * rotationSpeed));
            filmText.enabled = true;
            filmText.text = storage.photos.Count + " / " + filmShots;
        }
        else
        {
            filmImage.enabled = false;
            filmText.enabled = false;
        }

        if(eyesClosing)
        {
            eyesTimer += Time.deltaTime;
            if (eyesTimer > 2)
            {
                Weather weather = FindObjectOfType<Weather>();
                if (weather != null)
                {
                    Destroy(weather.gameObject);
                }
                SceneManager.LoadScene("Classroom");
            }
            return;
        }
        if(Input.GetButtonDown("Submit"))
        {
            if (storage.photos.Count >= filmShots)
            {
                closeEyes();
            }
        } else if (storage.photos.Count == filmShots)
        {
            hintText.color = new Color(.27f, .27f, .27f);
            hintText.enabled = true;
        } else
        {
            hintText.enabled = false;
        }

        if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Hover") != 0 || Input.GetButtonDown("Fire1"))
        {
            fogTween = true;
        }

        if (fogTween)
        {
            hintTimer += Time.deltaTime;
            if(fog.endDistance != fogDefault)
            {
                fog.endDistance = Mathf.Min(fogDefault, fog.endDistance + Time.deltaTime * FOGSPEED);
                if (titleScreen != null)
                {
                    foreach (TextMesh t in titleScreen.GetComponentsInChildren<TextMesh>())
                    {
                        t.color = new Color(1, 1, 1, 1 - fog.endDistance / fogDefault);
                    }
                    foreach (SpriteRenderer s in titleScreen.GetComponentsInChildren<SpriteRenderer>())
                    {
                        s.color = new Color(1, 1, 1, 1 - fog.endDistance / fogDefault);
                    }
                }
            } else
            {
                if (titleScreen != null)
                {
                    titleScreen.gameObject.SetActive(false);
                }
            }
        }
    }

    public void closeEyes()
    {
        if (!eyesClosing)
        {
            StartCoroutine(AudioFadeOut.FadeOut(music, 2f));
            eyesClosing = true;
            eyes.SetActive(true);
            eyes.GetComponent<Animator>().SetTrigger("close");
        }
    }

    public bool photoReady()
    {
        return filmTimer == 0;
    }

    public void clickPhoto()
    {
        filmTimer = filmCooldown;
    }
}
