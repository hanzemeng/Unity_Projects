using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using TMPro;

public class GameControl : MonoBehaviour
{
    [SerializeField] private CameraRayCaster cameraRayCaster;

    [SerializeField] private Transform playerSpawnArea;
    [SerializeField] private Transform enemySpawnArea;

    private EntityManager entityManager;
    private Entity playerSpawnPoint;
    [SerializeField] private float playerUnitSpawnRate; // units per still frame
    private Entity enemySpawnPoint;
    [SerializeField] private float enemyUnitSpawnRate; // units per still frame

    [SerializeField] private float debugInformationUpdateInterval;
    private float debugInformationUpdateElapseTime;
    [SerializeField] private TMP_Text debugInformation;

    private void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed, new RefreshRate{denominator=1, numerator=60});

        cameraRayCaster.AddMouseButtonAction(0, OnMouseButton);
        cameraRayCaster.AddMouseButtonDownAction(0, OnMouseButtonDown);
        cameraRayCaster.AddMouseButtonUpAction(0, OnMouseButtonUp);

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        playerSpawnPoint = entityManager.CreateEntity();
        entityManager.AddComponentData(playerSpawnPoint, new PlayerUnitSpawnData{leftMouseButtonDown = false, spawnRate=playerUnitSpawnRate});

        float3 bottomLeft = enemySpawnArea.position - enemySpawnArea.localScale/2f;
        bottomLeft.z = 0f;
        float3 topRight = enemySpawnArea.position + enemySpawnArea.localScale/2f;
        topRight.z = 0f;
        enemySpawnPoint=entityManager.CreateEntity();
        entityManager.AddComponentData(enemySpawnPoint, new EnemyUnitSpawnData{spawnLocationBottomLeftPoint=bottomLeft, spawnLocationTopRightPoint=topRight, spawnRate=enemyUnitSpawnRate});
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            PlayerUnitSpawnData playerTemp = entityManager.GetComponentData<PlayerUnitSpawnData>(playerSpawnPoint);
            playerTemp.spawnRate += 1;
            entityManager.SetComponentData(playerSpawnPoint, playerTemp);

            EnemyUnitSpawnData enemyTemp = entityManager.GetComponentData<EnemyUnitSpawnData>(enemySpawnPoint);
            enemyTemp.spawnRate += 1;
            entityManager.SetComponentData(enemySpawnPoint, enemyTemp);
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            PlayerUnitSpawnData playerTemp = entityManager.GetComponentData<PlayerUnitSpawnData>(playerSpawnPoint);
            playerTemp.spawnRate -= 1;
            entityManager.SetComponentData(playerSpawnPoint, playerTemp);

            EnemyUnitSpawnData enemyTemp = entityManager.GetComponentData<EnemyUnitSpawnData>(enemySpawnPoint);
            enemyTemp.spawnRate -= 1;
            entityManager.SetComponentData(enemySpawnPoint, enemyTemp);
        }

        debugInformationUpdateElapseTime += Time.deltaTime;
        if(debugInformationUpdateElapseTime < debugInformationUpdateInterval)
        {
            return;
        }
        debugInformationUpdateElapseTime = 0f;

        EntityQueryBuilder entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp);
        entityQueryBuilder.WithAll<ArrowData>();
        EntityQuery entityQuery = entityQueryBuilder.Build(entityManager);
        NativeArray<Entity> arrowEntities = entityQuery.ToEntityArray(Allocator.Temp);
        PlayerUnitSpawnData playerUnitSpawnData = entityManager.GetComponentData<PlayerUnitSpawnData>(playerSpawnPoint);

        debugInformation.text = $"Frame Rate: {(int)(1f/Time.deltaTime)} Arrow Count: {arrowEntities.Length} Spawn Rate: {(int)(playerUnitSpawnData.spawnRate / Time.fixedDeltaTime)}";
    }

    public void OnMouseButton(RaycastHit raycastHit)
    {
        if(null == raycastHit.collider || raycastHit.transform != playerSpawnArea)
        {
            return;
        }

        PlayerUnitSpawnData playerUnitSpawnData = entityManager.GetComponentData<PlayerUnitSpawnData>(playerSpawnPoint);
        playerUnitSpawnData.mousePosition = raycastHit.point;
        playerUnitSpawnData.mousePosition.z = 0f;
        entityManager.SetComponentData(playerSpawnPoint, playerUnitSpawnData);
    }
    public void OnMouseButtonDown(RaycastHit raycastHit)
    {
        if(null == raycastHit.collider || raycastHit.transform != playerSpawnArea)
        {
            return;
        }

        PlayerUnitSpawnData playerUnitSpawnData = entityManager.GetComponentData<PlayerUnitSpawnData>(playerSpawnPoint);
        playerUnitSpawnData.leftMouseButtonDown = true;
        entityManager.SetComponentData(playerSpawnPoint, playerUnitSpawnData);
    }
    public void OnMouseButtonUp(RaycastHit raycastHit)
    {
        PlayerUnitSpawnData playerUnitSpawnData = entityManager.GetComponentData<PlayerUnitSpawnData>(playerSpawnPoint);
        playerUnitSpawnData.leftMouseButtonDown = false;
        entityManager.SetComponentData(playerSpawnPoint, playerUnitSpawnData);
    }
}
