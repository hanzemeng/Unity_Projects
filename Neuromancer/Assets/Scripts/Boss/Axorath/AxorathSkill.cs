using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmeraldAI;

public class AxorathSkill : MonoBehaviour
{
    [SerializeField] private GameObject axorathPrefab;
    [SerializeField] private List<Transform> axorathSpawnPoints;
    [SerializeField] private float axorathInLevelInterval;
    [SerializeField] private float axorathOffLevelInterval;
    [SerializeField] private GameObject axorathDisappearEffect;

    private GameObject axorath;
    private bool axorathInLevel;
    private int axorathStartHealth;
    private int axorathHealth;
    [SerializeField] private float axorathHealthThreshold; // from 0 to 1, Axorath become stronger until axorathHealth/axorathStartHealth is lower than axorathHealthThreshold 
    [SerializeField] private float skillIntervalMaxReduceFactor; // Axorath uses skills faster
    private float skillIntervalCurrentReduceFactor;

    public GameObject hellStormProjectile;
    public AudioManager.SoundResource hellStormProjectileImpactSound;
    public GameObject hellStormProjectileImpactExplosion;
    public float hellStormStartThreshold; // from 0 to 1, Axorath begins to use this when axorathHealth/axorathStartHealth < hellStormStartThreshold
    public float hellStormInterval;
    public float hellStormDuration;
    public float hellStormProjectileInterval;
    public float hellStormDetectionRadius;
    public float hellStormProjectileHeight;
    public float hellStormProjectileSpeed;
    public float hellStormLandProjectileVariance;
    public float hellStormSpawnProjectileVariance;
    public int hellStormProjectileDamage;


    [SerializeField] private GameObject pointStrikeProjectile;
    [SerializeField] private AudioManager.SoundResource pointStrikeProjectileImpactSound;
    [SerializeField] private GameObject pointStrikeProjectileImpactExplosion;
    [SerializeField] private Vector3 pointStrikeProjectileImpactExplosionOffset;
    [SerializeField] private GameObject pointStrikeProjectileHint;
    [SerializeField] public float pointStrikeStartThreshold; // from 0 to 1, Axorath begins to use this when axorathHealth/axorathStartHealth < pointStrikeStartThreshold
    [SerializeField] private float pointStrikeInterval;
    [SerializeField] private float pointStrikeProjectileHeight;
    [SerializeField] private float pointStrikeProjectileSpeed;
    [SerializeField] private float pointStrikeProjectileRadius;
    [SerializeField] private int pointStrikeProjectileDamage;


    [SerializeField] private GameObject horizontalStrikeProjectile;
    [SerializeField] private AudioManager.SoundResource horizontalStrikeProjectileImpactSound;
    [SerializeField] private GameObject horizontalStrikeProjectileImpactExplosion;
    [SerializeField] private List<Transform> horizontalStrikePoints; // in counter clockwise order, start with bottom left point
    [SerializeField] public float horizontalStrikeStartThreshold; // from 0 to 1, Axorath begins to use this when axorathHealth/axorathStartHealth < horizontalStrikeStartThreshold
    [SerializeField] private float horizontalStrikeInterval;
    [SerializeField] private float horizontalStrikeProjectileInterval;
    [SerializeField] private int horizontalStrikeProjectileCount;
    [SerializeField] private float horizontalStrikeProjectileSpeed;
    [SerializeField] private int horizontalStrikeProjectileDamage;

    [SerializeField] private GameObject circularStrikeProjectile;
    [SerializeField] private AudioManager.SoundResource circularStrikeProjectileImpactSound;
    [SerializeField] private GameObject circularStrikeProjectileImpactExplosion;
    [SerializeField] public float circularStrikeStartThreshold; // from 0 to 1, Axorath begins to use this when axorathHealth/axorathStartHealth < circularStrikeStartThreshold
    [SerializeField] private float circularStrikeInterval;
    [SerializeField] private float circularStrikeRadius;
    [SerializeField] private float circularStrikeProjectileInterval;
    [SerializeField] private int circularStrikeProjectileCount;
    [SerializeField] private float circularStrikeProjectileSpeed;
    [SerializeField] private int circularStrikeProjectileDamage;


    private void Start()
    {
        axorath = Instantiate(axorathPrefab, axorathSpawnPoints[0].position, Quaternion.identity);
        axorathStartHealth = axorath.GetComponent<EmeraldAISystem>().CurrentHealth;
        skillIntervalCurrentReduceFactor = 1f;
        axorathInLevel = true;
        StartCoroutine(AxorathInLevelCoroutine());
        StartCoroutine(HellStormCoroutine());
        StartCoroutine(PointStrikeCoroutine());
        StartCoroutine(HorizontalStrikeCoroutine());
        StartCoroutine(CircularStrikeCoroutine());
    }

    private void FixedUpdate()
    {
        if(null != axorath && axorath.GetComponent<EmeraldAISystem>().CurrentHealth <= 0)
        {
            LevelManager.levelManager.LoadLevel(LevelName.CUTSCENE_HELL_2, 0);
        }
    }

    private IEnumerator AxorathInLevelCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(axorathInLevelInterval);
            Instantiate(axorathDisappearEffect, axorath.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            axorathInLevel = false;
            axorathHealth = axorath.GetComponent<EmeraldAISystem>().CurrentHealth;
            Destroy(axorath);

            float lerpAmount = Mathf.Max(0f, (float) axorathHealth / (float) axorathStartHealth - axorathHealthThreshold) / axorathHealthThreshold;
            skillIntervalCurrentReduceFactor = Mathf.Lerp(skillIntervalMaxReduceFactor, 1f, lerpAmount);

            // yield return new WaitForSeconds(axorathOffLevelInterval * skillIntervalCurrentReduceFactor);
            yield return new WaitForSeconds(axorathOffLevelInterval);
            Transform spawnPoint = axorathSpawnPoints[Random.Range(0, axorathSpawnPoints.Count)];
            Instantiate(axorathDisappearEffect, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            axorathInLevel = true;
            axorath = Instantiate(axorathPrefab, spawnPoint.position, Quaternion.identity);
            axorath.GetComponent<EmeraldAISystem>().CurrentHealth = axorathHealth;
        }
    }

    private IEnumerator HellStormCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(hellStormInterval / skillIntervalCurrentReduceFactor);
            while(axorathInLevel || (float) axorathHealth / (float) axorathStartHealth > hellStormStartThreshold)
            {
                yield return new WaitForSeconds(hellStormInterval / skillIntervalCurrentReduceFactor / 2f);
            }

            int layerMask = 0;
            layerMask |= 1 << LayerMask.NameToLayer("Ally");
            layerMask |= 1 << LayerMask.NameToLayer("Hero");
            Collider[] colliders = Physics.OverlapSphere(transform.position, hellStormDetectionRadius, layerMask);
            if(0 == colliders.Length)
            {
                continue;
            }

            float elapsedTime = 0f;
            float nextHellStormProjectileTime = hellStormProjectileInterval;
            while(elapsedTime < hellStormDuration)
            {
                elapsedTime += Time.deltaTime;
                if(elapsedTime > nextHellStormProjectileTime)
                {
                    nextHellStormProjectileTime += hellStormProjectileInterval;

                    Vector2 positionVariance = hellStormLandProjectileVariance * Random.insideUnitCircle;
                    Vector3 targetPosition = colliders[Random.Range(0, colliders.Length)].transform.position + new Vector3(positionVariance.x, 0f, positionVariance.y);

                    positionVariance = hellStormSpawnProjectileVariance * Random.insideUnitCircle;
                    PukeStormProjectile newProjectile = Instantiate(hellStormProjectile, new Vector3(targetPosition.x + positionVariance.x, hellStormProjectileHeight, targetPosition.z+positionVariance.y), Quaternion.identity).GetComponent<PukeStormProjectile>();

                    newProjectile.impactSound = hellStormProjectileImpactSound;
                    newProjectile.impactExplosion = hellStormProjectileImpactExplosion;
                    newProjectile.fallingDirection = (targetPosition - newProjectile.transform.position).normalized;
                    newProjectile.transform.rotation = Quaternion.LookRotation(newProjectile.fallingDirection);
                    newProjectile.fallingSpeed = hellStormProjectileSpeed;
                    newProjectile.damage = hellStormProjectileDamage;
                }
                yield return null;
            }
        }
    }

    private IEnumerator PointStrikeCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(pointStrikeInterval / skillIntervalCurrentReduceFactor);
            while(axorathInLevel || (float) axorathHealth / (float) axorathStartHealth > pointStrikeStartThreshold)
            {
                yield return new WaitForSeconds(pointStrikeInterval / skillIntervalCurrentReduceFactor / 2f);
            }

            Instantiate(pointStrikeProjectileHint, PlayerController.player.transform.position+0.5f*Vector3.up, pointStrikeProjectileHint.transform.rotation);

            Vector3 spawnPosition = PlayerController.player.transform.position + pointStrikeProjectileHeight * Vector3.up;
            PointStrikeProjectile newProjectile = Instantiate(pointStrikeProjectile, spawnPosition, Quaternion.identity).GetComponent<PointStrikeProjectile>();
            newProjectile.impactSound = pointStrikeProjectileImpactSound;
            newProjectile.impactExplosion = pointStrikeProjectileImpactExplosion;
            newProjectile.impactExplosionOffset = pointStrikeProjectileImpactExplosionOffset;
            newProjectile.fallingDirection = Vector3.down;
            newProjectile.transform.rotation = Quaternion.LookRotation(newProjectile.fallingDirection);
            newProjectile.fallingSpeed = pointStrikeProjectileSpeed;
            newProjectile.radius = pointStrikeProjectileRadius;
            newProjectile.damage = pointStrikeProjectileDamage;
        }
    }

    private IEnumerator HorizontalStrikeCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(horizontalStrikeInterval / skillIntervalCurrentReduceFactor);
            while(axorathInLevel || (float) axorathHealth / (float) axorathStartHealth > horizontalStrikeStartThreshold)
            {
                yield return new WaitForSeconds(horizontalStrikeInterval / skillIntervalCurrentReduceFactor / 2f);
            }

            int sideIndex = Random.Range(0, 4);

            Vector3 startPosition = horizontalStrikePoints[sideIndex].position;
            Vector3 endPosition = horizontalStrikePoints[(sideIndex+1)%4].position;
            Vector3 flyingDirection = Vector3.zero;
            switch(sideIndex)
            {
                case 0:
                {
                    flyingDirection = Vector3.forward;
                    break;
                }
                case 1:
                {
                    flyingDirection = Vector3.left;
                    break;
                }
                case 2:
                {
                    flyingDirection = Vector3.back;
                    break;
                }
                case 3:
                {
                    flyingDirection = Vector3.right;
                    break;
                }
            }    

            for(int i=0; i<horizontalStrikeProjectileCount; i++)
            {
                Vector3 currentStartPosition = Vector3.Lerp(startPosition, endPosition, (float)i / (float)horizontalStrikeProjectileCount);

                PukeStormProjectile newProjectile = Instantiate(horizontalStrikeProjectile, currentStartPosition, Quaternion.identity).GetComponent<PukeStormProjectile>();
                newProjectile.impactSound = horizontalStrikeProjectileImpactSound;
                newProjectile.impactExplosion = horizontalStrikeProjectileImpactExplosion;
                newProjectile.fallingDirection = flyingDirection;
                newProjectile.transform.rotation = Quaternion.LookRotation(newProjectile.fallingDirection);
                newProjectile.fallingSpeed = horizontalStrikeProjectileSpeed;
                newProjectile.damage = horizontalStrikeProjectileDamage;
                yield return new WaitForSeconds(horizontalStrikeProjectileInterval);
            }
        }
    }

    private IEnumerator CircularStrikeCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(circularStrikeInterval / skillIntervalCurrentReduceFactor);
            while(axorathInLevel || (float) axorathHealth / (float) axorathStartHealth > circularStrikeStartThreshold)
            {
                yield return new WaitForSeconds(circularStrikeInterval / skillIntervalCurrentReduceFactor / 2f);
            }

            for(int i=0; i<circularStrikeProjectileCount; i++)
            {
                Vector2 temp = circularStrikeRadius * Random.insideUnitCircle.normalized;
                Vector3 startPosition = PlayerController.player.transform.position + new Vector3(temp.x, 0.5f, temp.y);
                Vector3 flyingDirection = new Vector3(-temp.x, 0f, -temp.y).normalized;

                PukeStormProjectile newProjectile = Instantiate(circularStrikeProjectile, startPosition, Quaternion.identity).GetComponent<PukeStormProjectile>();
                newProjectile.impactSound = circularStrikeProjectileImpactSound;
                newProjectile.impactExplosion = circularStrikeProjectileImpactExplosion;
                newProjectile.fallingDirection = flyingDirection;
                newProjectile.transform.rotation = Quaternion.LookRotation(newProjectile.fallingDirection);
                newProjectile.fallingSpeed = circularStrikeProjectileSpeed;
                newProjectile.damage = circularStrikeProjectileDamage;
                yield return new WaitForSeconds(circularStrikeProjectileInterval);
            }
        }
    }
}
