using UnityEngine;

public class ProjectileSpellBehavior : MonoBehaviour
{
    public float maxTravelTime = 10f;
    public float disappearDelay = 2f;
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
        // if we hit something we shouldn't, return
        if(col.GetComponent<PlayerController>() || col.GetComponent<AttackingHitbox>() || col.GetComponent<NPCDialogue>())
        {
            return;
        }

        GameObject hitFX;
        Vector3 alteredPosition = col.transform.position;
        alteredPosition.y += 1;
        hitFX = Instantiate(FXHit, alteredPosition, Quaternion.identity);
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
