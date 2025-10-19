using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    public float upDownSpeed = 8f;
    public float forwardSpeed = 20f;
    public float horizontalSpeed = 10f;

     public float xMin = 0f;
    public float xMax = 300f;
    public float yMin = 5f;
    public float yMax = 50f;
    public float zRange = 50f;

    public GameObject missilePrefab;
    public Transform[] missileSpawnPoints;
    public float missileCooldown = 0.25f;

     public AudioSource movementAudioSource;
    public AudioSource firingAudioSource;
    public AudioClip moveUpDownSound;
    public AudioClip forwardSound;
    public AudioClip horizontalSound;
    public AudioClip missileSound;
    public ParticleSystem upDownParticles;
    public ParticleSystem forwardParticles;
    public ParticleSystem horizontalParticles;

    public GameObject shieldVisual;
    public bool multiMissileActive = false;
    public bool shieldActive = false;
    public ParticleSystem shieldActivateVFX;
    public ParticleSystem shieldLoopVFX;

    private Rigidbody rb;
    private float lastMissileTime = 0f;
    private GameManager gameManager;
    private Coroutine shieldCoroutine;
    public bool controlsEnabled = true;


    // Ensures movement and firing audio sources exists and finds GameManager
    // Sets shield active to false
    void Awake() 
    {
        rb = GetComponent<Rigidbody>();

        if (movementAudioSource == null)
            movementAudioSource = gameObject.AddComponent<AudioSource>();

        if (firingAudioSource == null)
            firingAudioSource = gameObject.AddComponent<AudioSource>();

        gameManager = FindAnyObjectByType<GameManager>();

        if (shieldVisual)
            shieldVisual.SetActive(false);
    }

    // Checks for player controls
    void Update()
    {
        if (!controlsEnabled || !gameManager || gameManager.currentLives <= 0)
            return;

        HandleMovement();
        ClampPosition();
        UpdateParticleEffectsAndSoundEffects();

        if (Input.GetKey(KeyCode.Space))
            FireMissile();
    }

    void HandleMovement()
    {
        float moveX = 0f; // Forward/Backward (X-axis)
        float moveY = 0f; // Up/Down (Y-axis)
        float moveZ = 0f; // Horizontal (Z-axis)

        // Forward/Backward (X)
        if (Input.GetKey(KeyCode.W)) moveX = 1f;
        if (Input.GetKey(KeyCode.S)) moveX = -1f;

        // Up/Down (Y)
        if (Input.GetKey(KeyCode.UpArrow)) moveY = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) moveY = -1f;

        // Left/Right (Z)
        if (Input.GetKey(KeyCode.LeftArrow)) moveZ = 1f;
        if (Input.GetKey(KeyCode.RightArrow)) moveZ = -1f;

        Vector3 movement = new Vector3(
            moveX * forwardSpeed,
            moveY * upDownSpeed,
            moveZ * horizontalSpeed
        ) * Time.deltaTime;

        transform.position += movement;
    }

    void UpdateParticleEffectsAndSoundEffects()
    {
        bool movingForwardBackward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S);
        bool movingUpDown = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);
        bool movingHorizontal = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

        // --- Particle Effects ---
        if (forwardParticles)
        {
            if (movingForwardBackward && !forwardParticles.isPlaying) forwardParticles.Play();
            if (!movingForwardBackward && forwardParticles.isPlaying) forwardParticles.Stop();
        }

        if (upDownParticles)
        {
            if (movingUpDown && !upDownParticles.isPlaying) upDownParticles.Play();
            if (!movingUpDown && upDownParticles.isPlaying) upDownParticles.Stop();
        }

        if (horizontalParticles)
        {
            if (movingHorizontal && !horizontalParticles.isPlaying) horizontalParticles.Play();
            if (!movingHorizontal && horizontalParticles.isPlaying) horizontalParticles.Stop();
        }

        // --- Audio Logic ---
        if (movingForwardBackward)
        {
            if (movementAudioSource.clip != forwardSound)
            {
                movementAudioSource.Stop();
                movementAudioSource.clip = forwardSound;
                movementAudioSource.loop = true;
                movementAudioSource.Play();
            }
        }
        else if (movingUpDown)
        {
            if (movementAudioSource.clip != moveUpDownSound)
            {
                movementAudioSource.Stop();
                movementAudioSource.clip = moveUpDownSound;
                movementAudioSource.loop = true;
                movementAudioSource.Play();
            }
        }
        else if (movingHorizontal)
        {
            if (movementAudioSource.clip != horizontalSound)
            {
                movementAudioSource.Stop();
                movementAudioSource.clip = horizontalSound;
                movementAudioSource.loop = true;
                movementAudioSource.Play();
            }
        }
        else
        {
            if (movementAudioSource.isPlaying)
            {
                movementAudioSource.Stop();
                movementAudioSource.clip = null;
            }
        }
    }

    void ClampPosition()
    {
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, xMin, xMax); // forward/backward
        p.y = Mathf.Clamp(p.y, yMin, yMax); // vertical
        p.z = Mathf.Clamp(p.z, -zRange, zRange); // horizontal
        transform.position = p;
    }

    void FireMissile()
    {
        if (missilePrefab == null || missileSpawnPoints.Length == 0) return;
        if (Time.time - lastMissileTime < missileCooldown) return;

        lastMissileTime = Time.time;

        if (multiMissileActive)
        {
            foreach (Transform point in missileSpawnPoints)
                InstantiateMissile(point);
        }
        else
        {
            InstantiateMissile(missileSpawnPoints[0]);
        }

        if (missileSound != null)
            firingAudioSource.PlayOneShot(missileSound);
    }

    void InstantiateMissile(Transform point)
    {
        if (missilePrefab == null) return;

        GameObject m = Instantiate(missilePrefab, point.position, point.rotation);
        Rigidbody rbMissile = m.GetComponent<Rigidbody>();
        if (rbMissile != null)
            rbMissile.linearVelocity = transform.forward * 60f;
    }

    public void ActivateShield(bool on)
    {
        shieldActive = on;
        if (shieldVisual) shieldVisual.SetActive(on);

        if (on)
        {
            if (shieldActivateVFX)
            {
                var vfx = Instantiate(shieldActivateVFX, transform.position, transform.rotation, transform);
                vfx.Play();
                var main = vfx.main;
                Destroy(vfx.gameObject, main.duration + main.startLifetime.constantMax);
            }
            if (shieldLoopVFX) shieldLoopVFX.Play();
        }
        else
        {
            if (shieldLoopVFX) shieldLoopVFX.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyMover e = other.GetComponent<EnemyMover>();
            if (shieldActive)
            {
                if (e) e.DestroyByHit();
            }
            else
            {
                gameManager.ChangeLives(-1);
                if (e) e.DestroyByHit();

                if (gameManager.currentLives < 1)
                    DestroyPlayerSequence();
            }
        }
        else if (other.CompareTag("Powerup"))
        {
            Powerup p = other.GetComponent<Powerup>();
            if (p) p.Collect(this);
        }
        else if (other.CompareTag("LandingPad"))
        {
            AudioAndParticle.Instance.PlayImpactAt(transform.position);
            gameManager.OnShipLanded();
        }
    }

    void DestroyPlayerSequence()
    {
        AudioAndParticle.Instance.PlayExplosionAt(transform.position);
        Destroy(gameObject);
        gameManager.OnPlayerDestroyed();
    }

    public void GrantShield(float duration)
    {
        if (shieldCoroutine != null)
            StopCoroutine(shieldCoroutine);

        shieldCoroutine = StartCoroutine(ShieldRoutine(duration));
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        ActivateShield(true);
        yield return new WaitForSeconds(duration);
        ActivateShield(false);
        shieldCoroutine = null;
    }

    public void FreezeMovement(float duration)
    {
        StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        controlsEnabled = false;
        yield return new WaitForSecondsRealtime(duration);
    }

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }
}
