using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float upDownSpeed = 8f;
    public float forwardSpeed = 20f;
    public float rotateSpeed = 120f; // degrees per second while tilting
    public float maxTilty = 45f; // degrees

    public Vector2 xRange = new Vector2(0f, 300f);
    public Vector2 yRange = new Vector2(5f, 50f);


    public GameObject missilePrefab;

    public Transform[] missileSpawnPoints;

    public float missileCooldown = 0.25f;


    public AudioClip moveUpDownSound;
    public AudioClip forwardSound;
    public AudioClip rotateSound;
    public AudioClip missileSound;
    public ParticleSystem upDownParticles;
    public ParticleSystem forwardParticles;
    public ParticleSystem rotateParticles;

    public GameObject shieldVisual; // enable/disable

    public bool multiMissileActive = false;

    Rigidbody rb;
    private float lastMissileTime = 0f;


    private bool isRotating = false;
    public bool shieldActive = false;

    public AudioSource audioSource;

    private GameManager gameManager;
     
    private float currentYaw = 0f;   // Relative yaw from initial orientation
    private float initialYaw = 0f;   // Player starting yaw

    public ParticleSystem shieldActivateVFX; // one-shot burst on enable
    public ParticleSystem shieldLoopVFX;     // continuous particle effects attached to shield Visual

    private Coroutine shieldCoroutine;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        gameManager = FindAnyObjectByType<GameManager>();
        if (shieldVisual)
        {
            shieldVisual.SetActive(false);
        }
        initialYaw = transform.eulerAngles.y;

    }

    void Update()
    {
        if (!gameManager || gameManager.currentLives <= 0) return;
        HandleInput();
        ClampPosition();
        UpdateParticleEffectsAndSoundEffects();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireMissile();
        } 
    }

    void HandleInput()
    {
        // For Up/down movement
        float vertical = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) vertical = -1f;
        transform.position += Vector3.up * vertical * upDownSpeed * Time.deltaTime;

        // For Left/right rotation (yaw around Y)
        float turn = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) turn = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) turn = 1f;

        if (Mathf.Abs(turn) > 0.01f)
        {
            isRotating = true;
            currentYaw += turn * rotateSpeed * Time.deltaTime;
            currentYaw = Mathf.Clamp(currentYaw, -maxTilty, maxTilty);
        }
        else
        {
            // Return yaw smoothly to 0
            currentYaw = Mathf.MoveTowards(currentYaw, 0f, rotateSpeed * Time.deltaTime);
            isRotating = Mathf.Abs(currentYaw) > 0.01f;
        }

        // Apply rotation relative to initial orientation
        transform.rotation = Quaternion.Euler(0f, initialYaw + currentYaw, 0f);

        //For Forward movement along +X, only if not rotating
        if (!isRotating && Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.right * forwardSpeed * Time.deltaTime;
        }
    }

    void UpdateParticleEffectsAndSoundEffects()
    {
        bool movingUpDown = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);
        bool movingForward = Input.GetKey(KeyCode.W) && !isRotating;
        bool rotating = isRotating;


        if (upDownParticles)
        {
            if (movingUpDown && !upDownParticles.isPlaying) upDownParticles.Play();
            if (!movingUpDown && upDownParticles.isPlaying) upDownParticles.Stop();
        }
        if (forwardParticles)
        {
            if (movingForward && !forwardParticles.isPlaying) forwardParticles.Play();
            if (!movingForward && forwardParticles.isPlaying) forwardParticles.Stop();
        }
        if (rotateParticles)
        {
            if (rotating && !rotateParticles.isPlaying) rotateParticles.Play();
            if (!rotating && rotateParticles.isPlaying) rotateParticles.Stop();
        }


        if (movingUpDown && moveUpDownSound != null && !audioSource.isPlaying)
            audioSource.PlayOneShot(moveUpDownSound);

        if (movingForward && forwardSound != null && !audioSource.isPlaying)
            audioSource.PlayOneShot(forwardSound);

        if (rotating && rotateSound != null && !audioSource.isPlaying)
            audioSource.PlayOneShot(rotateSound);
    }


    void ClampPosition()
    {
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, xRange.x, xRange.y);
        p.y = Mathf.Clamp(p.y, yRange.x, yRange.y);
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
            {
                InstantiateMissile(point);
            }
        }
        else
        {
            InstantiateMissile(missileSpawnPoints[0]);
        }

        if (missileSound != null)
            audioSource.PlayOneShot(missileSound);
    }

    void InstantiateMissile(Transform point)
    {
        if (missilePrefab == null) return;

        // Instantiate missile at spawn point
        GameObject m = Instantiate(missilePrefab, point.position, point.rotation);


        Rigidbody rbMissile = m.GetComponent<Rigidbody>();
        if (rbMissile != null)
        {
            rbMissile.linearVelocity = transform.forward * 60f; 
        }
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
                {
                    DestroyPlayerSequence();
                }
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

    public void GrantShield(float duration) {
        if (shieldCoroutine != null) {
            StopCoroutine(shieldCoroutine);
        }
        shieldCoroutine = StartCoroutine(ShieldRoutine(duration));
    }

    private IEnumerator ShieldRoutine(float duration) {
        ActivateShield(true);
        yield return new WaitForSeconds(duration);
        ActivateShield(false);
        shieldCoroutine = null;
    }

}

    