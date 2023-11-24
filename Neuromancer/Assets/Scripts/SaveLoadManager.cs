using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Neuromancer;
using UnityEngine.Events;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager saveLoadManager;

    private int activeSaveFileIndex;
    public SaveSlot activeSaveSlot;

    private string saveFilePath;
    private const string ALL_DATA_FILE = "AllData";
    private const string SAVE_SLOT_FILE = "SaveSlot";
    private const string PLAYER_DATA_FILE = "PlayerData";
    private const string LEVEL_DATA_FILE = "LevelData";
    private const string OBJECT_DATA_FILE = "ObjectData";
    private const string FILE_EXTENSION = ".neu";
    private List<GameObject> allAllies;
    public List<ObjectData> allObjects;

    private const string PLAYER_SPAWN_POINTS = "PlayerSpawnPoints";

    public UnityEvent onSaveEvent = new UnityEvent();
    
    private void Awake()
    {
        if(null == saveLoadManager)
        {
            saveLoadManager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        activeSaveFileIndex = 0;
        activeSaveSlot = new SaveSlot();

        saveFilePath = Application.dataPath;
        allAllies = new List<GameObject>();
        allObjects = new List<ObjectData>();
    }

    public void SetActiveSaveFileIndex(int newActiveSaveFileIndex)
    {
        activeSaveFileIndex = newActiveSaveFileIndex;
    }
    
    public bool HasSaveFile()
    {
        return File.Exists(Path.Combine(saveFilePath, $"{ALL_DATA_FILE}{activeSaveFileIndex}") + FILE_EXTENSION);
    }

    public void SaveGame()
    {
        activeSaveSlot.saveDate = DateTime.Now.ToString();
        SaveSaveSlot(activeSaveSlot);
        SavePlayerData(PlayerProgression.playerProgression);
        SaveLevelData(LevelManager.levelManager);
        SaveObjectData();

        FlushSaveFiles();
        onSaveEvent.Invoke();

        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.SAVE_SFX);
    }
    public void LoadGame()
    {
        ParseSaveFiles();

        LoadSaveSlot(activeSaveSlot);

        PlayerProgression.playerProgression.ResetPlayerProgression();
        PlayerInventory.current.Reset();
        PlayerController.player.EnableDash(false);
        allObjects.Clear();
        LoadPlayerData(PlayerProgression.playerProgression);
        PlayerProgression.playerProgression.UpdatePlayerProgression();

        LoadLevelData(LevelManager.levelManager);

        LoadObjectData();
    }
    public void CreateSaveFile(string saveName)
    {
        activeSaveSlot.saveName = saveName;
        activeSaveSlot.saveDate = DateTime.Now.ToString();

        PlayerProgression.playerProgression.ResetPlayerProgression();
        PlayerInventory.current.Reset();
        PlayerController.player.EnableDash(false);
        allObjects.Clear();

        LevelManager.levelManager.lastLevelName=LevelName.CUTSCENE_INTRO;
        LevelManager.levelManager.lastSpawnPointIndex= 0;
        SaveGame();
    }
    public void DeleteSaveData()
    {
        string filePath = Path.Combine(saveFilePath, $"{ALL_DATA_FILE}{activeSaveFileIndex}{FILE_EXTENSION}");
        File.Delete(filePath);
    }

    public void DeleteAllSaveData()
    {
        string[] filesPathes = Directory.GetFiles(saveFilePath, "*" + FILE_EXTENSION, SearchOption.TopDirectoryOnly);
        foreach (string filePath in filesPathes)
        {
            File.Delete(filePath);
        }
    }
    private void FlushSaveFiles()
    {
        string allSaveFilePath = Path.Combine(saveFilePath, $"{ALL_DATA_FILE}{activeSaveFileIndex}");
        allSaveFilePath += FILE_EXTENSION;
        File.Delete(allSaveFilePath);

        string[] saveFiles = Directory.GetFiles(saveFilePath, "*" + FILE_EXTENSION, SearchOption.TopDirectoryOnly);
        string[] persistentSaveFiles = Directory.GetFiles(saveFilePath, $"{ALL_DATA_FILE}*", SearchOption.TopDirectoryOnly);

        using (var writeStream = File.Open(allSaveFilePath, FileMode.Append))
        {
            using (var writer = new BinaryWriter(writeStream, Encoding.UTF8, false))
            {
                foreach (string file in saveFiles)
                {
                    if(persistentSaveFiles.Contains(file)) {
                        continue;
                    }

                    using (var readStream = File.Open(file, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(readStream, Encoding.UTF8, false))
                        {
                            byte[] allData = reader.ReadBytes(65536);
                            writer.Write(file);
                            writer.Write(allData.Length);
                            writer.Write(allData);
                        }
                    }
                    File.Delete(file);
                }
            }
        }
    }
    private void ParseSaveFiles()
    {
        string allSaveFilePath = Path.Combine(saveFilePath, $"{ALL_DATA_FILE}{activeSaveFileIndex}");
        allSaveFilePath += FILE_EXTENSION;
        using (var readStream = File.Open(allSaveFilePath, FileMode.Open))
        {
            using (var reader = new BinaryReader(readStream, Encoding.UTF8, false))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string fileName = reader.ReadString();
                    int dataSize = reader.ReadInt32();
                    byte[] data = reader.ReadBytes(dataSize);

                    using (var writeStream = File.Open(fileName, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(writeStream, Encoding.UTF8, false))
                        {
                            writer.Write(data);
                        }
                    }
                }
            }
        }
    }

    public bool SaveSaveSlot(SaveSlot saveSlot)
    {
        if(null == saveSlot)
        {
            return false;
        }

        string saveSlotDataPath = Path.Combine(saveFilePath, SAVE_SLOT_FILE);
        saveSlotDataPath += FILE_EXTENSION;
        using (var stream = File.Open(saveSlotDataPath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(saveSlot.saveName);
                writer.Write(saveSlot.saveDate);
            }
        }
        return true;
    }
    public bool LoadSaveSlot(SaveSlot saveSlot)
    {
        string saveSlotDataPath = Path.Combine(saveFilePath, SAVE_SLOT_FILE);
        saveSlotDataPath += FILE_EXTENSION;
        if(!File.Exists(saveSlotDataPath))
        {
            return false;
        }

        using (var stream = File.Open(saveSlotDataPath, FileMode.Open))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
                saveSlot.saveName = reader.ReadString();
                saveSlot.saveDate = reader.ReadString();
            }
        }
        return true;
    }

    public void SaveUnitData()
    {
        List<NPCUnit> allUnits = UnitGroupManager.current.GetComponent<UnitGroupManager>().allUnits.units;
        if(null == allUnits)
        {
            return;
        }
        allAllies.Clear();
        foreach (NPCUnit unit in allUnits)
        {
            allAllies.Add(unit.gameObject);
            DontDestroyOnLoad(unit.gameObject);
        }
    }
    public void LoadUnitData(int spawnPointIndex)
    {
        GameObject[] playerSpawnPoints = GameObject.Find(PLAYER_SPAWN_POINTS).GetComponent<PlayerSpawnPoints>().playerSpawnPoints;
        // Failsafe - Default back to the first spawn point if the inputted index is greater than the playerSpawnPoints array or is negative. 
        if(playerSpawnPoints.Length - 1 < spawnPointIndex || spawnPointIndex < 0)
        {
            spawnPointIndex = 0;
        }
        PlayerController.player.transform.position = playerSpawnPoints[spawnPointIndex].transform.position;
        PlayerController.player.transform.rotation = playerSpawnPoints[spawnPointIndex].transform.rotation;
        foreach (GameObject ally in allAllies)
        {
            SceneManager.MoveGameObjectToScene(ally, SceneManager.GetActiveScene());
            float randomRadian = UnityEngine.Random.Range(0f, 2f*Mathf.PI);
            Vector3 randomOffset =  UnityEngine.Random.Range(5f, 8f) * new Vector3(Mathf.Cos(randomRadian), 0f, Mathf.Sin(randomRadian));
            Vector3 target = playerSpawnPoints[spawnPointIndex].transform.position + randomOffset;
            ally.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(target);
            ally.GetComponent<NPCUnit>().IssueCommand(new Command(CommandType.ATTACK_FOLLOW, PlayerController.player.transform));
        }
    }

    public bool LoadPlayerData(PlayerProgression playerProgression)
    {
        string playerDataPath = Path.Combine(saveFilePath, PLAYER_DATA_FILE);
        playerDataPath += FILE_EXTENSION;
        if(!File.Exists(playerDataPath))
        {
            return false;
        }

        string playerSpellData;
        bool dashEnabled;
        using (var stream = File.Open(playerDataPath, FileMode.Open))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
                playerProgression.skillPoint = reader.ReadInt32();
                playerProgression.playerMaxHealthLevel = reader.ReadInt32();
                playerProgression.playerHealthRegenerationLevel = reader.ReadInt32();
                playerProgression.playerMaxManaLevel = reader.ReadInt32();
                playerProgression.playerManaRegenerationLevel = reader.ReadInt32();
                playerProgression.playerMaxAllyCountLevel = reader.ReadInt32();
                dashEnabled = reader.ReadBoolean();
                playerSpellData = reader.ReadString();
            }
        }
        PlayerController.player.EnableDash(dashEnabled);
        PlayerInventory.current.SetData(playerSpellData);

        return true;
    }
    public void SavePlayerData(PlayerProgression playerProgression)
    {
        string playerDataPath = Path.Combine(saveFilePath, PLAYER_DATA_FILE);
        playerDataPath += FILE_EXTENSION;
        string playerSpellData = PlayerInventory.current.GetData();
        bool dashEnabled = PlayerController.player.GetDashEnabled();
        using (var stream = File.Open(playerDataPath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(playerProgression.skillPoint);
                writer.Write(playerProgression.playerMaxHealthLevel);
                writer.Write(playerProgression.playerHealthRegenerationLevel);
                writer.Write(playerProgression.playerMaxManaLevel);
                writer.Write(playerProgression.playerManaRegenerationLevel);
                writer.Write(playerProgression.playerMaxAllyCountLevel);
                writer.Write(dashEnabled);
                writer.Write(playerSpellData);
            }
        }
    }

    public bool LoadLevelData(LevelManager levelManager)
    {
        string levelDataPath = Path.Combine(saveFilePath, LEVEL_DATA_FILE);
        levelDataPath += FILE_EXTENSION;
        if(!File.Exists(levelDataPath))
        {
            return false;
        }

        using (var stream = File.Open(levelDataPath, FileMode.Open))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
                levelManager.lastLevelName = reader.ReadString();
                levelManager.lastSpawnPointIndex = reader.ReadInt32();
            }
        }

        return true;
    }
    public void SaveLevelData(LevelManager levelManager)
    {
        string levelDataPath = Path.Combine(saveFilePath, LEVEL_DATA_FILE);
        levelDataPath += FILE_EXTENSION;

        using (var stream = File.Open(levelDataPath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(levelManager.lastLevelName);
                writer.Write(levelManager.lastSpawnPointIndex);
            }
        }
    }
    
    public ObjectData FindObjectEntry(string id)
    {
        ObjectData od = allObjects.Find(obj => obj.id == id);
        return od;
    }

    public void AddObjectEntry(ObjectData objectData) {
        foreach (ObjectData od in allObjects) {
            if (od.id == objectData.id) {
                return; // don't add duplicates
            }
        }
        allObjects.Add(objectData);
    }

    public void ModObjectEntry(ObjectData objectData) {
        foreach (ObjectData od in allObjects) {
            if (od.id == objectData.id) {
                od.state = objectData.state;
                return;
            }
        }
    }

    class ObjectDataWrapper {
        public List<string> list = new List<string>();
    }

    public void SaveObjectData() {
        ObjectDataWrapper data = new ObjectDataWrapper();
        foreach (ObjectData od in allObjects) { data.list.Add(od.ToJsonString()); }
        string dataStr = JsonUtility.ToJson(data);

        string objectDataPath = Path.Combine(saveFilePath, OBJECT_DATA_FILE);
        objectDataPath += FILE_EXTENSION;
        using (var stream = File.Open(objectDataPath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(dataStr);
            }
        }
    }

    public bool LoadObjectData() {
        string objectDataPath = Path.Combine(saveFilePath, OBJECT_DATA_FILE);
        objectDataPath += FILE_EXTENSION;
        if(!File.Exists(objectDataPath)) { return false; }

        string dataStr;
        using (var stream = File.Open(objectDataPath, FileMode.Open))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
                dataStr = reader.ReadString();
            }
        }

        ObjectDataWrapper d = JsonUtility.FromJson<ObjectDataWrapper>(dataStr);
        allObjects.Clear();
        foreach (string s in d.list) {
            ObjectData od = JsonUtility.FromJson<ObjectData>(s);
            AddObjectEntry(od);
        }

        return true;
    }
}
