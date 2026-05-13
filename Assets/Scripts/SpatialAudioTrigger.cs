using UnityEngine;

/// <summary>
/// Trigger de áudio espacial por proximidade.
/// Toca narração quando o jogador se aproxima.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SpatialAudioTrigger : MonoBehaviour
{
    [Header("Configurações")]
    public AudioClip narrationClip;
    public float triggerRadius = 3f;
    public float fadeSpeed = 2f;

    private AudioSource audioSource;
    private Transform player;
    private bool isPlaying;
    private float targetVolume = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = narrationClip;
        audioSource.spatialBlend = 1f; // 3D
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 0f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= triggerRadius)
        {
            if (!isPlaying && narrationClip != null)
            {
                audioSource.Play();
                isPlaying = true;
            }
            // Fade in
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
        }
        else
        {
            // Fade out
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, fadeSpeed * Time.deltaTime);

            if (audioSource.volume <= 0.01f && isPlaying)
            {
                audioSource.Stop();
                isPlaying = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
