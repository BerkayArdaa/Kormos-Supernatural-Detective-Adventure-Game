using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TPSController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSens;
    [SerializeField] private float aimSens;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform BulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] public int maxCloneCount = 0;
    [SerializeField] private int collectibleCloneIncrease = 2;
    [SerializeField] private Volume slownTimeEffect;
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private float cloneManaCost = 20f;
    [SerializeField] private float timeSlowManaCost = 30f;

    private ThirdPersonController tpsCont;
    private StarterAssetsInputs starterAssetsInputs;
    public int maxAmmo = 16;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI reloadText;
    public float slowMotionFactor = 0.1f;
    public float slowDuration = 5f;
    private bool isSlowed = false;
    public int currentCloneCount = 0;
    public float defaultMoveSpeed = 2.0f;

    private void Awake()
    {
        tpsCont = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        reloadText.gameObject.SetActive(false);
        ammoText.gameObject.SetActive(false);
        slownTimeEffect.enabled = false;
    }

    void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            tpsCont.SetSensitivity(aimSens);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            tpsCont.SetSensitivity(normalSens);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
        CheckAim();
        CheckSlowed();
        checkCloned();
    }

    public void CheckSlowed()
    {
        if (Input.GetKeyDown(KeyCode.G) && !isSlowed)
        {
            float manaCost = timeSlowManaCost;
            if (playerStats.CanUseAbility(manaCost))
            {
                playerStats.AddMana(manaCost);
                SlowDownTime();
            }
            else
            {
                Debug.Log("Not enough mana to slow time!");
            }
        }
    }

    public void SlowDownTime()
    {
        tpsCont.MoveSpeed = tpsCont.MoveSpeed * 60f;
        tpsCont._animator.speed = 10f;
        Time.timeScale = slowMotionFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        isSlowed = true;
        slownTimeEffect.enabled = true;
        StartCoroutine(ResumeTimeAfterDelay());
    }

    private IEnumerator ResumeTimeAfterDelay()
    {
        yield return new WaitForSecondsRealtime(slowDuration);
        slownTimeEffect.enabled = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        tpsCont._animator.speed = 1f;
        tpsCont.MoveSpeed = defaultMoveSpeed;
        isSlowed = false;
    }

    public void CheckAim()
    {
        if (Input.GetButton("Fire2"))
        {
            ammoText.gameObject.SetActive(true);
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                ammoText.gameObject.SetActive(false);
            }
        }
        else
        {
            ammoText.gameObject.SetActive(false);
        }
    }

    public void TryShoot()
    {
        if (isReloading || !Input.GetButton("Fire2"))
        {
            return;
        }
        if (currentAmmo > 0)
        {
            Shoot();
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();
        /* if (starterAssetsInputs.shoot)
        {
            Ray raymond = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            Vector3 aimDir;
            if (Physics.Raycast(raymond, out hit))
            {
                aimDir = (hit.point - spawnBulletPosition.position).normalized;
            }
            else
            {
                aimDir = Camera.main.transform.forward;
            }

            Instantiate(BulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            starterAssetsInputs.shoot = false;
        }*/
        if (starterAssetsInputs.shoot)
        {
            Ray rayForShoot = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(rayForShoot, out hit))
            {
                Transform hitTransform = hit.transform;
                if (hitTransform.GetComponent<BulletTarget>() != null)
                {
                    Instantiate(vfxHitRed, hit.point, Quaternion.identity);
                }
                else
                {
                    Instantiate(vfxHitGreen, hit.point, Quaternion.identity);
                }
            }

            Target target = hit.collider.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(20);
            }

            starterAssetsInputs.shoot = false;
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        reloadText.gameObject.SetActive(true);
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        reloadText.gameObject.SetActive(false);
        UpdateAmmoUI();
        isReloading = false;
    }

    private void UpdateAmmoUI()
    {
        ammoText.text = "Ammo: " + currentAmmo + "/" + maxAmmo;
    }

    public void checkCloned()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (maxCloneCount > 0)
            {
                float manaCost = cloneManaCost;
                if (playerStats.CanUseAbility(manaCost) && currentCloneCount < maxCloneCount)
                {
                    playerStats.AddMana(manaCost);
                    SpawnCloneNearTarget();
                }
                else if (currentCloneCount >= maxCloneCount)
                {
                    Debug.Log("Reached maximum clone count!");
                }
                else
                {
                    Debug.Log("Not enough mana to spawn a clone!");
                }
            }
        }
    }

    public void CollectCollectible()
    {
        maxCloneCount += collectibleCloneIncrease;
        Debug.Log("Clone count increased by " + collectibleCloneIncrease);
    }

    public void SpawnCloneNearTarget()
    {
        Ray rayForClone = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(rayForClone, out hit))
        {
            Transform targetTransform = hit.transform;

            if (targetTransform.GetComponent<BulletTarget>() != null)
            {
                Vector3 spawnPosition = FindSpawnPositionNearTarget(targetTransform.position);
                if (spawnPosition != Vector3.zero)
                {
                    GameObject clone = Instantiate(clonePrefab, spawnPosition, Quaternion.identity);

                    CloneController cloneController = clone.GetComponent<CloneController>();

                    if (cloneController != null)
                    {
                        cloneController.SetTarget(targetTransform);
                        currentCloneCount++;
                    }
                    else
                    {
                        Debug.LogError("CloneController component is missing on clonePrefab.");
                    }
                }
            }
        }
    }

    public void OnCloneDestroyed()
    {
        currentCloneCount = Mathf.Max(currentCloneCount - 1, 0);
    }

    private Vector3 FindSpawnPositionNearTarget(Vector3 targetPosition)
    {
        float spawnRadius = 4f;
        int maxAttempts = 100;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0;
            Vector3 potentialPosition = targetPosition + randomOffset;

            if (!Physics.CheckSphere(potentialPosition, 1f))
            {
                return potentialPosition;
            }
        }

        return Vector3.zero;
    }
}
