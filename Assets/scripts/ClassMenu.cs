using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ClassMenu : MonoBehaviour
{
    public static ClassMenu instance;

    public Texture2D pinTexture;
    public Texture2D rectTexture;

    public float holdDistance = .3f;

    public float photoWidth = 200;
    public float photoHeight = 200;

    public GameObject tutorialText;

    public AudioSource grabSound;
    public AudioSource pinSound;

    bool pinning = false;
    bool mouseOver = false;

    float cursorFadeTimer = 0;

    Photograph grabbedPhoto;

    [HideInInspector]
    public List<Photograph> photosToGrade;

    float eyesTimer = 0;
    bool eyesClosing = false;

    public GameObject eyes;

    public TextMesh scoreText;

    bool lateStart = false;

    bool gradedPhotos = false;
    public ParticleSystem gradeParticles;

    void Start()
    {
        instance = this;
        photosToGrade = new List<Photograph>();
        
    }

    void Update()
    {
        if(!lateStart)
        {
            if (FindObjectOfType<PhotoSpawner>().photoList.Count < 3)
            {
                tutorialText.GetComponent<TextMesh>().text = "Not enough photos.\nTry again.";
            }
            lateStart = true;
        }
        if(eyesClosing)
        {
            eyesTimer += Time.deltaTime;
            if(eyesTimer >= 2)
            {
                //next scene
            }
            return;
        }
        mouseOver = false;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit rayHit = new RaycastHit();
        if (Physics.Raycast(ray, out rayHit))
        {
            if (rayHit.transform.name == "Quit" && Input.GetButtonDown("Fire1"))
            {
                Application.Quit();
            }
            if (rayHit.transform.name == "Retry" && Input.GetButtonDown("Fire1"))
            {
                Destroy(FindObjectOfType<PhotoStorage>().gameObject);
                Destroy(FindObjectOfType<CursorLocker>().gameObject);
                SceneManager.LoadScene("Dream");
            }
            //Debug.DrawLine(rayHit.point, transform.position);
            Photograph photo = rayHit.collider.gameObject.GetComponent<Photograph>();
            if (photo != null)
            {
                mouseOver = true;
                if (Input.GetButtonDown("Fire1"))
                {
                    if(grabbedPhoto != null)
                    {
                        grabbedPhoto.transform.position = photo.transform.position;
                        grabbedPhoto.transform.rotation = photo.transform.rotation;
                        grabbedPhoto.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                        grabbedPhoto.shrink();
                        photosToGrade.Remove(grabbedPhoto);
                    }
                    grabbedPhoto = photo;
                    grabbedPhoto.transform.position = new Vector3(0, -10, 0);
                    grabSound.pitch = Random.Range(0.75f, 2f);
                    grabSound.Play();
                }
                photosToGrade.Remove(grabbedPhoto);
            }  
            else if(grabbedPhoto != null && Input.GetButtonDown("Fire1"))
            {
                grabbedPhoto.transform.position = rayHit.point + rayHit.normal * .05f;
                grabbedPhoto.transform.rotation = Quaternion.LookRotation(-rayHit.normal, Vector3.up);
                if (rayHit.normal.y < .5f)
                {
                    grabbedPhoto.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    grabbedPhoto.grow();
                    if (rayHit.transform.name == "Grading Wall")
                    {
                        photosToGrade.Add(grabbedPhoto);
                        if(photosToGrade.Count == 3 && !gradedPhotos)
                        {
                            gradePhotos();
                        }
                    }
                } else
                {
                    grabbedPhoto.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    grabbedPhoto.shrink();
                    photosToGrade.Remove(grabbedPhoto);
                }
                pinSound.pitch = Random.Range(.75f, 2f);
                pinSound.Play();
                grabbedPhoto = null;
            }
        }
        if(photosToGrade.Count == 3)
        {
            tutorialText.SetActive(false);
        } else
        {
            tutorialText.SetActive(true);
        }
    }

    void OnGUI()
    {
        if (grabbedPhoto != null)
        {
            //draw white border
            GUI.DrawTexture(new Rect((Screen.width - photoWidth - 16) / 2,
            (Screen.height - photoHeight - 16) / 2 + photoHeight / 2 + 32,
            photoWidth + 16, photoHeight + 16),
            Texture2D.whiteTexture);
            //draw photo
            GUI.DrawTexture(new Rect((Screen.width - photoWidth) / 2,
            (Screen.height - photoHeight) / 2 + photoHeight / 2 + 32,
            photoWidth, photoHeight),
            grabbedPhoto.GetComponent<MeshRenderer>().material.mainTexture as Texture2D);
            drawCrosshair(pinTexture, 32, 32);
        }
        else if (mouseOver)
        {
            drawCrosshair(rectTexture, 32, 32);
        } else 
        {
            if(GetComponent<Rigidbody>().velocity.magnitude > 0  || 
                Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                cursorFadeTimer = 3;
            }
            if (cursorFadeTimer > 0) {
                drawCrosshair(rectTexture, 32, 32);
            }
        }
        cursorFadeTimer = Mathf.Max(0, cursorFadeTimer - Time.deltaTime);
    }

    void drawCrosshair(Texture2D crosshairTexture, float width, float height)
    {
        GUI.DrawTexture(new Rect((Screen.width - width) / 2,
            (Screen.height - height) / 2,
            width, height), crosshairTexture);
    }

    static Color32 DEERMASK = new Color32(255, 0, 255, 255);
    static Color32 EYEMASK = new Color32(255, 0, 0, 255);
    static Color32 EFFECTMASK = new Color32(0, 255, 0, 255);

    void gradePhotos()
    {
        gradeParticles.Play();
        gradedPhotos = true;

        int composition = 0;
        int framing = 0;
        int effects = 0;
        int smudges = 0;

        foreach(Photograph p in photosToGrade)
        {
            composition += Mathf.FloorToInt(p.info.dataMaskRatio(DEERMASK) * 500);
            framing += Mathf.Min(100, Mathf.FloorToInt(p.info.dataMaskRatio(EYEMASK) * 20000));
            effects += Mathf.Min(100, Mathf.FloorToInt(p.info.dataMaskRatio(EFFECTMASK) * 500));
            smudges -= Mathf.FloorToInt(p.info.damage * 20);
        }

        scoreText.text = "SCORE:\nComposition: " + composition + "\nFraming: " + framing +
            "\nEffects: " + effects + "\nSmudges: " + smudges +
            "\nTOTAL: " + (composition + framing + effects + smudges);
    }
}
