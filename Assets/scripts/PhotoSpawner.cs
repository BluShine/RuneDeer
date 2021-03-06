using UnityEngine;
using System.Collections.Generic;

public class PhotoSpawner : MonoBehaviour
{
    public GameObject photoPrefab;

    public float spacing = .25f;
    public float randomness = .5f;
    public int tableSize = 6;
    static float STACKING = .2f;

    PhotoStorage photos;

    public List<Photograph> photoList;

    public void Start()
    {
        photoList = new List<Photograph>();

        photos = GameObject.FindObjectOfType<PhotoStorage>();
        photos.transferTextures();

        int count = 0;
        int layer = 0;
        for(int i = 0; i < photos.photos.Count; i++)
        {
            GameObject p = GameObject.Instantiate(photoPrefab);
            p.GetComponent<MeshRenderer>().material.mainTexture = photos.photos[i];
            p.transform.position = transform.position + new Vector3(randomness * Random.Range(-1f, 1f), layer * STACKING, count * spacing);
            p.transform.Rotate(0, 0, Random.Range(0f, 360f));
            count++;
            if(count >= tableSize)
            {
                count = 0;
                layer++;
            }
            Photograph pho = p.GetComponent<Photograph>();
            pho.info = photos.infos[i];
            photoList.Add(pho);
        }
    }
}
