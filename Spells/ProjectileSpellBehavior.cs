using UnityEngine;

public class ProjectileSpellBehavior : MonoBehaviour
{
    public float maxTravelTime = 10f; // Maximum travel distance
    public float disappearDelay = 2f; // Delay before disappearing after collision
    public float moveSpeed;

    public GameObject FXHit;

    int damage = 20;

    ParticleSystem fXProjectileParticles;
    ParticleSystem fXProjectileBaseParticles;
    ParticleSystem fXProjectileTrailParticles;

    AudioSource projectileAudio;

    SphereCollider projectileCollider;
    public Rigidbody projectileRigidbody;


    private void Start()
    {
        fXProjectileParticles = gameObject.GetComponent<ParticleSystem>();
        fXProjectileBaseParticles = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        fXProjectileTrailParticles = gameObject.transform.GetChild(1).GetComponent<ParticleSystem>();

        projectileCollider = gameObject.GetComponent<SphereCollider>();
        projectileAudio = gameObject.GetComponent<AudioSource>();
        projectileRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public void Setup(Vector3 projectileDir, int damage)
    {
        projectileRigidbody.velocity = projectileDir * moveSpeed;
        this.damage = damage;
    }

    private void Update()
    {
        Destroy(gameObject, maxTravelTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        // if we hit the player or the wrong hitbox return
        if(col.GetComponent<PlayerController>() || col.GetComponent<AttackingHitbox>())
        {
            return;
        }

        GameObject hitFX;
        hitFX = Instantiate(FXHit, col.transform.position, Quaternion.identity);
        Destroy(hitFX, 3f);

        fXProjectileBaseParticles.gameObject.SetActive(false);
        fXProjectileBaseParticles.Stop();
        fXProjectileTrailParticles.Stop();
        fXProjectileParticles.Stop();

        projectileAudio.Stop();

        moveSpeed = 0f;
        projectileCollider.enabled = false;

        if (col.GetComponent<ReceivingHitbox>())
        {
            col.GetComponentInParent<EnemyAI>().TakeDamage(damage);
        }

        Destroy(gameObject, 2f);
    }

}
