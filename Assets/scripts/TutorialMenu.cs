using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialMenu : MonoBehaviour {

    public GameObject shutterAnim;
    public AudioSource shutterSound;
    public Text hintText;
    public AudioSource music;

    public LayerMask deerMask;

    bool started = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(!started && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Hover") != 0 || Input.GetButtonDown("Fire1")))
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }

		if(Input.GetButtonDown("Fire1"))
        {
            shutterAnim.SetActive(true);
            shutterAnim.GetComponent<Animator>().SetTrigger("click");
            shutterSound.Play();
            RaycastHit info = new RaycastHit();
            if(Physics.SphereCast(transform.position, 3, transform.forward, out info, 10, deerMask))
            {
                hintText.enabled = true;
            }
        }

        if(hintText.enabled && Input.GetButtonDown("Submit"))
        {
            StartCoroutine(AudioFadeOut.FadeOut(music, 0.3f));
            SceneManager.LoadScene("Dream");
        }
	}
}
