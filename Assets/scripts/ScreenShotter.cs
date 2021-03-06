using UnityEngine;
using System.Collections;
using System.IO;

//taken from https://github.com/martinysa/Unity/blob/master/Libraries/ScreenShotMethods/shotMethod3.cs
//(MIT liscense)
//--Method 3 -- Using readPixel -- UNITY3D PRO ONLY --//

public class ScreenShotter : MonoBehaviour
{

    int width = Screen.width;
    int height = Screen.height;
    int offsetX = 0;
    int offsetY = 0;
    float horizFov = 60;
    float vertFov = 60;

    public int raycastDetail = 10;

    public float aspectRatio = 1f/1f; //width / height

    public PhotoStorage storage;

    DreamMenu dMenu;

    public GameObject shutterAnim;
    public AudioSource shutterSound;

    public AudioSource filmSound;

    public Camera evaluatorCamera;
    public Shader evaluationShader;
    RenderTexture evaluationRender;

    CameraDamage cDamage;

    [BitStrap.Button]
    public void setShader()
    {
        if (evaluatorCamera != null && evaluationShader != null)
        {
            evaluatorCamera.SetReplacementShader(evaluationShader, "RenderType");
        }
    }
    [ExecuteInEditMode]
    private void Awake()
    {
        setShader();
    }

    public void Start()
    {
        cDamage = FindObjectOfType<CameraDamage>();
        dMenu = FindObjectOfType<DreamMenu>();
        width = Screen.width;
        height = Screen.height;
        if(width > height * aspectRatio)
        {
            //screen is too wide 
            offsetX = Mathf.FloorToInt((width - height * aspectRatio) / 2);
            width = Mathf.FloorToInt(height * aspectRatio);
        } else if(width < height * aspectRatio)
        {
            //screen is too narrow
            offsetY = Mathf.FloorToInt((height - width * (1f / aspectRatio)) / 2);
            height = Mathf.FloorToInt(width * (1f / aspectRatio));
            float vertRadians = Camera.main.fieldOfView * Mathf.Deg2Rad;
            float vertCrop = (float)height / (float)Screen.height;
            //recalculate vertical fov
            vertFov = Mathf.Rad2Deg * 2 * Mathf.Atan(Mathf.Tan(vertRadians / 2) * vertCrop);
        }
        //calculate horizontal fov
        float radHFov = 2 * Mathf.Atan(Mathf.Tan((vertFov * Mathf.Deg2Rad) / 2) * aspectRatio);
        horizFov = Mathf.Rad2Deg * radHFov;
        //Debug.Log("Vert " + vertFov + " Horiz " + horizFov);
        //setup evaluator camera
        setShader();
        evaluationRender = evaluatorCamera.targetTexture;
    }

    void LateUpdate()
    {
        if (Input.GetButtonDown("Fire1") && dMenu.photoReady() && storage.photos.Count < dMenu.filmShots)
        {
            //disable text
            dMenu.hintText.enabled = false;
            dMenu.clickPhoto();
            dMenu.filmImage.enabled = false;

            //play sound
            shutterSound.Play();
            filmSound.PlayDelayed(1);

            //save image
            StartCoroutine(ScreenshotEncode());

            //save data
            PhotoInfo photoInfo = new PhotoInfo(raycastDetail * raycastDetail);
            //cast rays
            for(int i = 0; i < raycastDetail; i++)
            {
                float xAngle = vertFov * (.5f - (i + .5f) / (float) raycastDetail);
                for(int j = 0; j < raycastDetail; j++)
                {
                    float yAngle = horizFov * (.5f - (j + .5f) / (float)raycastDetail);
                    RaycastHit rayHit;
                    Transform mCam = Camera.main.transform;
                    Vector3 angle = mCam.rotation * Quaternion.Euler(xAngle, yAngle, 0) * Vector3.forward;
                    Ray ray = new Ray(mCam.position, angle);
                    if(Physics.Raycast(ray, out rayHit)) {
                        /*PipeInfoHolder pipe = rayHit.transform.GetComponent<PipeInfoHolder>();
                        if (pipe != null)
                        {
                            photoInfo.pipeDetection[i * raycastDetail + j] = pipe.info;
                            //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = rayHit.point;//Debug spheres
                        }*/
                    } 
                }
            }
            photoInfo.damage = cDamage.damage;

            storage.infos.Add(photoInfo);
            //Debug.Log("pipes: " + photoInfo.countPipes() + "" + " density: " + photoInfo.pipeDensity());
        }
    }

    IEnumerator ScreenshotEncode()
    {   // Call a coroutine let me know when all objects
        // have finished being rendered on screen 


        // -- that's not a another thread -- !!

        // wait for graphics to render
        yield return new WaitForEndOfFrame();

        // create a texture to pass to encoding
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);

        // put buffer into texture

        texture.ReadPixels(new Rect(offsetX, offsetY, width, height), 0, 0);
        texture.Apply();
        storage.photos.Add(texture);

        //data texture
        Texture2D dataTex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        //evaluatorCamera.Render();
        RenderTexture.active = evaluationRender;
        evaluatorCamera.Render();
        dataTex.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        dataTex.Apply();
        storage.photoMasks.Add(dataTex);

        // split the process up--ReadPixels() and the GetPixels()
        // call inside of the encoder are both pretty heavy
        yield return 0;

        byte[] bytes = texture.EncodeToPNG();

        // save to HDD
        string timestamp = System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "_" + 
            System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Second;
        if(!Directory.Exists(Application.dataPath + "/../screenshots"))
        {
            Directory.CreateDirectory(Application.dataPath + "/../screenshots");
            yield return 0;
        }
        File.WriteAllBytes(Application.dataPath + "/../screenshots/photo-" + timestamp + ".png", bytes);


        //Release memory 
        //DestroyObject(texture); just kidding

        //play shutter animation
        shutterAnim.SetActive(true);
        shutterAnim.GetComponent<Animator>().SetTrigger("click");
    }
}