using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompactorBehavior : MonoBehaviour
{
    [SerializeField] List<AudioClip> audioClips = new List<AudioClip>();
    private AudioSource audioSource;

    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("AudioPlayer").GetComponent<AudioSource>();
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gm = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
            gm.emptyTrash();
            PlayRandomAudioClip();
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Trash")
        {
            Destroy(gameObject);
        }
    }
    void PlayRandomAudioClip()
    {
        // Use Random.Range to generate a random index within the list
        int randomIndex = Random.Range(0, audioClips.Count);

        // Access the randomly chosen AudioClip
        AudioClip randomAudioClip = audioClips[randomIndex];

        // Play the randomly chosen audio clip
        audioSource.clip = randomAudioClip;
        audioSource.Play();
    }
}
