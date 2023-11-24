using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using CleverCrow.Fluid.UniqueIds;

public class UniqueIdTests : InputTestFixture
{
    [UnityTest]
    public IEnumerator UniqueIdTest()
    {
        LogAssert.ignoreFailingMessages = true;

        List<string> testLevelsNames = new List<string>();

        testLevelsNames.Add(LevelName.CATACOMB_1);
        testLevelsNames.Add(LevelName.CATACOMB_2);
        testLevelsNames.Add(LevelName.CATACOMB_3);
        testLevelsNames.Add(LevelName.CATACOMB_4);
        testLevelsNames.Add(LevelName.SWAMP_1);
        testLevelsNames.Add(LevelName.SWAMP_2);
        testLevelsNames.Add(LevelName.SWAMP_3);
        testLevelsNames.Add(LevelName.SWAMP_4);
        testLevelsNames.Add(LevelName.VERDANT_CASTLE_1);
        testLevelsNames.Add(LevelName.VERDANT_CASTLE_2);
        testLevelsNames.Add(LevelName.VERDANT_CASTLE_3);
        testLevelsNames.Add(LevelName.VERDANT_CASTLE_4);
        testLevelsNames.Add(LevelName.VERDANT_CASTLE_5);
        testLevelsNames.Add(LevelName.HELL_TRENCH_1);
        testLevelsNames.Add(LevelName.HELL_TRENCH_2);
        testLevelsNames.Add(LevelName.HELL_TRENCH_3);
        testLevelsNames.Add(LevelName.HELL_TRENCH_4);
        testLevelsNames.Add(LevelName.HELL_TRENCH_5);
        testLevelsNames.Add(LevelName.HELL_TRENCH_6);
        testLevelsNames.Add(LevelName.HELL_TRENCH_7);
        testLevelsNames.Add(LevelName.HELL_TRENCH_8);

        Dictionary<string, List<(string, string)>> duplicateObjects = new Dictionary<string, List<(string, string)>>();

        AsyncOperation loadingScene;
        foreach(string sceneName in testLevelsNames)
        {
            loadingScene = SceneManager.LoadSceneAsync(sceneName);
            while (!loadingScene.isDone)
            {
                yield return null;
            }

            UniqueId[] UniqueIdObjects = Object.FindObjectsOfType<UniqueId>();
            FieldInfo _idField = typeof(UniqueId).GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (UniqueId uniqueIdObject in UniqueIdObjects)
            {
                string _id = (string)_idField.GetValue(uniqueIdObject);
                if(null == _id || "" == _id)
                {
                    Debug.Log($"{sceneName}, {uniqueIdObject.transform.name} has no id");
                    Assert.IsTrue(false);
                }

                if(!duplicateObjects.ContainsKey(_id))
                {
                    duplicateObjects[_id] = new List<(string, string)>();
                }
                duplicateObjects[_id].Add((sceneName, uniqueIdObject.transform.name));
            }
        }

        bool shouldPass = true;
        foreach(KeyValuePair<string, List<(string, string)>> keyValuePair in duplicateObjects)
        {
            if(1 == keyValuePair.Value.Count)
            {
                continue;
            }

            shouldPass = false;
            Debug.Log($"Key: {keyValuePair.Key} is found on:");
            foreach((string, string) duplicateObject in keyValuePair.Value)
            {
                Debug.Log($"Scene: {duplicateObject.Item1}, Object: {duplicateObject.Item2}.");
            }
        }

        Assert.IsTrue(shouldPass);
        Debug.Log("Passed the unique id test");
    }
}
