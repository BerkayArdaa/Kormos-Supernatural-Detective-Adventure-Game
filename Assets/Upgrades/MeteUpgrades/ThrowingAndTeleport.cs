using System.Collections;
using UnityEngine;

public class ThrowingAndTeleport : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;
    public GameObject knifeInTheHand;
    public Transform player;
    public GameObject playerGameObject;
    public Transform handTransform;
    public ParticleSystem throwParticleEffect;

    [Header("Settings")]
    public float throwCooldown;
    public float disappearTime;
    public float teleportOffset = 1.5f;
    public float throwForce;
    public float throwUpwardForce;

    [Header("Input")]
    public KeyCode throwKey1 = KeyCode.Q;
    public KeyCode throwKey2 = KeyCode.E;

    private bool readyToThrow;

    private void Start()
    {
        knifeInTheHand.SetActive(true);
        readyToThrow = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(throwKey1) && readyToThrow)
        {
            Throw(teleportAction: true);
        }
        else if (Input.GetKeyDown(throwKey2) && readyToThrow)
        {
            Throw(teleportAction: false);
        }
    }

    private void Throw(bool teleportAction)
    {
        readyToThrow = false;
        knifeInTheHand.SetActive(false);
        if (throwParticleEffect != null)
        {
            throwParticleEffect.transform.position = handTransform.position;
            throwParticleEffect.transform.rotation = handTransform.rotation;
            throwParticleEffect.Play();
            Debug.Log("Throw particle effect triggered at handTransform position.");
        }
        else
        {
            Debug.LogError("throwParticleEffect is not assigned.");
        }

        //StartCoroutine(ReappearKnifeInHandAfterDelay(2f));

        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        TeleportTrigger teleportTrigger = projectile.AddComponent<TeleportTrigger>();
        teleportTrigger.Initialize(this, teleportAction);

        StartCoroutine(DestroyAfterTime(projectile));

        Invoke(nameof(ResetThrow), throwCooldown);
    }


    private IEnumerator DestroyAfterTime(GameObject projectile)
    {
        float timer = 0f;
        while (timer < disappearTime)
        {
            if (projectile == null) yield break;
            timer += Time.deltaTime;
            yield return null;
        }
        knifeInTheHand.SetActive(true);
        if (projectile != null) Destroy(projectile);
    }

    private IEnumerator ReappearKnifeInHandAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    public void TeleportPlayerTo(Vector3 targetPosition)
    {
        if (playerGameObject != null) playerGameObject.SetActive(false);

        Vector3 directionToPlayer = (player.position - targetPosition).normalized;
        player.position = targetPosition + directionToPlayer * teleportOffset;

        if (playerGameObject != null) playerGameObject.SetActive(true);
    }

    public void PullTargetToPlayer(Transform target)
    {
        Transform parent = target.parent;
        Transform targetToPull = parent != null ? parent : target;
        Vector3 offsetPosition = player.position + (player.forward * teleportOffset);
        offsetPosition.y = player.position.y;
        targetToPull.position = offsetPosition;
    }
}

public class TeleportTrigger : MonoBehaviour
{
    private ThrowingAndTeleport throwingAndTeleport;
    private bool teleportAction;

    public void Initialize(ThrowingAndTeleport handler, bool teleport)
    {
        throwingAndTeleport = handler;
        teleportAction = teleport;
    }

    public Vector2 positionOffset(Vector3 playerPosition, Vector3 hitObjectPosition)
    {
        float xOffset = 0f;
        float zOffset = 0f;

        xOffset = playerPosition.x - hitObjectPosition.x > 0 ? -1 : +1;
        zOffset = playerPosition.z - hitObjectPosition.z > 0 ? -1 : +1;

        return new Vector2(xOffset, zOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        TeleportProperties properties = other.GetComponent<TeleportProperties>();

        if (properties == null) return;

        if (teleportAction && properties.isItTeleportable)
        {
            Vector3 targetPosition = transform.position;

            if (other.gameObject.CompareTag("platform"))
            {
                Bounds bounds = other.bounds;
                float middleOfX = (bounds.max.x + bounds.min.x) / 2;
                float middleOfZ = (bounds.max.z + bounds.min.z) / 2;

                Vector2 offsets = positionOffset(throwingAndTeleport.player.position, bounds.center);
                targetPosition = new Vector3(middleOfX + offsets.x, bounds.max.y + 2, middleOfZ + offsets.y);
            }
            BeastController enemyController = other.transform.parent?.GetComponent<BeastController>();
            if (enemyController != null)
            {
                enemyController.isStunned = true;
            }

            throwingAndTeleport.TeleportPlayerTo(targetPosition);
        }
        else if (!teleportAction && properties.isItPullable)
        {
            BeastController enemyController = other.transform.parent?.GetComponent<BeastController>();
            if (enemyController != null)
            {
                enemyController.isStunned = true;
            }

            throwingAndTeleport.PullTargetToPlayer(other.transform);

        }

        Destroy(gameObject);
    }
}
