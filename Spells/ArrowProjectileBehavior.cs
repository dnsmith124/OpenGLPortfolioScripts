using UnityEngine;

public class ArrowProjectileBehavior : MonoBehaviour
{

    public float maxTravelTime = 10f;
    public float disappearDelay = 2f;
    public float moveSpeed;

    public GameObject FXHit;
    private int damage;

    ParticleSystem fXProjectileParticles;
    ParticleSystem fXProjectileBaseParticles;
    ParticleSystem fXProjectileTrailParticles;

    AudioSource projectileAudio;

    CapsuleCollider projectileCollider;
    Rigidbody projectileRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        projectileCollider = GetComponent<CapsuleCollider>();
        //projectileAudio = gameObject.GetComponent<AudioSource>();
        projectileRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, maxTravelTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        // if we hit something we shouldn't, return
        if (!col.GetComponent<PlayerStats>())
            return;

        //GameObject hitFX;
        //Vector3 alteredPosition = col.transform.position;
        //alteredPosition.y += 1;
        //hitFX = Instantiate(FXHit, alteredPosition, Quaternion.identity);
        //Destroy(hitFX, 3f);

        //fXProjectileBaseParticles.gameObject.SetActive(false);
        //fXProjectileBaseParticles.Stop();
        //fXProjectileTrailParticles.Stop();
        //fXProjectileParticles.Stop();

        //projectileAudio.Stop();

        moveSpeed = 0f;
        projectileRigidbody.velocity = new Vector3();
        projectileCollider.enabled = false;

        col.GetComponent<PlayerStats>().adjustHealth(-damage);

        Destroy(gameObject, .25f);
    }

    public void Setup(Vector3 projectileDir, int damage)
    {
        GetComponent<Rigidbody>().velocity = projectileDir * moveSpeed;
        this.damage = damage;
    }
}
