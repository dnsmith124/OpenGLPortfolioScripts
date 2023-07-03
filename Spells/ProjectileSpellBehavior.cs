using UnityEngine;

public class ProjectileSpellBehavior : MonoBehaviour
{
    public float maxTravelTime = 10f; // Maximum travel distance
    public float disappearDelay = 2f; // Delay before disappearing after collision
    public int damage = 20;
    public float moveSpeed;

    public GameObject FXHit;

    ParticleSystem fXProjectileParticles;
    ParticleSystem fXProjectileBaseParticles;
    ParticleSystem fXProjectileTrailParticles;

    AudioSource projectileAudio;

    SphereCollider projectileCollider;
    public Rigidbody projectileRigidbody;

    private Vector3 startingPosition;

    private void Start()
    {
        // Save the starting position of the spell
        startingPosition = transform.position;

        fXProjectileParticles = gameObject.GetComponent<ParticleSystem>();
        fXProjectileBaseParticles = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        fXProjectileTrailParticles = gameObject.transform.GetChild(1).GetComponent<ParticleSystem>();

        projectileCollider = gameObject.GetComponent<SphereCollider>();
        projectileAudio = gameObject.GetComponent<AudioSource>();
        projectileRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public void Setup(Vector3 projectileDir)
    {
        projectileRigidbody.velocity = projectileDir * moveSpeed;
    }

    private void Update()
    {
        Destroy(gameObject, maxTravelTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        // if we hit the player, return
        if(col.GetComponent<PlayerController>())
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

        // If the collided object is an enemy
        EnemyAI enemy = col.GetComponent<EnemyAI>(); 
        if (enemy != null) 
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject, 3f);
    }

    public void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;  // Calculate the direction to the target
        directionToTarget.y = 0;  // Assuming you want to rotate only on the Y axis (to keep the object upright)

        if (directionToTarget != Vector3.zero)  // Prevents LookRotation from creating errors when given a zero vector
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);  // Calculate the required rotation to face the target
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);  // Gradually rotate towards the target
        }
    }

}
