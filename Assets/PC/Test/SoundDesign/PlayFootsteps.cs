using UnityEngine;

public class PlayFootsteps : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] metal,hardwood,dirt;
    public Transform rayStart;
    public float range;
    public LayerMask layerMask;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }

    private void RandomizeAudio(AudioClip clip)
    {
        audioSource.pitch = Random.Range(0.8f, 1f);
        audioSource.volume = Random.Range(0.8f, 1f);
        audioSource.PlayOneShot(clip);
    }

    private void SelectFootStepSound(AudioClip[] clips, string materialName)
    {
        RandomizeAudio(GetRandomClip(clips));
        Debug.Log($"{materialName} Detected!");
    }

    public void AllFootSteps()
    {
        if (Physics.Raycast(rayStart.position, -rayStart.up, out RaycastHit hit, range, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Metal":
                    SelectFootStepSound(metal, "Metal");
                    break;
                case "Hardwood":
                    SelectFootStepSound(hardwood, "Hardwood");
                    break;
                case "Dirt":
                    SelectFootStepSound(dirt, "Dirt");
                    break;
            }
        }
    }
}
