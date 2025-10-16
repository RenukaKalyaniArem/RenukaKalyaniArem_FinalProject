using UnityEngine;

public class HomingMissile : MonoBehaviour
{
 
    public Transform target;
    public float speed = 50f;
    public float rotateSpeed = 200f;
    public float lifeTime = 6f;

    void Start()
    {   
        if (target == null)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length > 0)
            {
                target = enemies[Random.Range(0, enemies.Length)].transform;
            }
            Destroy(gameObject, lifeTime);
        }
    }

    void Update()
    {
        if (target == null)
        {
            transform.position += Vector3.right * speed * Time.deltaTime; 
            return;
        }
        Vector3 dir = (target.position - transform.position).normalized;
        float step = rotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, dir, step * Mathf.Deg2Rad, 0f);
        transform.rotation = Quaternion.LookRotation(newDir);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyMover e = other.GetComponent<EnemyMover>();
            if (e) e.DestroyByHit();
            Destroy(gameObject);
        }
    }
}
