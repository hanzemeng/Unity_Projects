using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class ResourceData
{
    public string keyword;
    public string filterWord = "";
    public Object resourceObject;

    public ResourceData(string keyword, string filterWord = "", Object resourceObject = null)
    {
        this.keyword = keyword;
        this.filterWord = filterWord;  
        this.resourceObject = resourceObject;  
    }
}

// Responsible for storing all important data from Resources and letting the user access them based on inputted commands. 
[RequireComponent(typeof(DeveloperConsoleController))]
public class ResourceDatabaseController : MonoBehaviour
{
    // To be used in case we want to abbreviate any prefab names into something shorter when typing in console.
    [System.Serializable]
    public class AbbreviationMapping
    {
        public string fullName;
        public string abbreviation;
    }

    [System.Serializable]
    public class FilenameToKeywordGeneratorBase
    {
        public string fileName;
        public char endKeywordSubstringAtChar  = '\0';
        public List<AbbreviationMapping> abbreviations;
        public List<string> filterWords;
    }

    [SerializeField] private List<FilenameToKeywordGeneratorBase> keywordGenerationSettings;
    public List<FilenameToKeywordGeneratorBase> KeywordGenerationSettings { get {return keywordGenerationSettings;}}

    private Dictionary<string, Object[]> allResourceObjects;
    private Dictionary<string, Dictionary<string, string>> allAbbreviations;
    private Dictionary<string, char> allCharacterDividers;
    private Dictionary<string, List<string>> allFilterWords;

    private Dictionary<string, HashSet<ResourceData>> allResourceData;
    public Dictionary<string, HashSet<ResourceData>> AllResourceData { get {return allResourceData;}}

    public static ResourceDatabaseController instance;

    // Start is called before the first frame update
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this; 

        InitializeResourceContents();
        InitializeKeywordGenerationSetup();
        InitializeAllKeywords();

        
    }
    
    // Searches through the list of prefabs defined in a list assigned to each filename.
    public Object FindObject(string folderName, string keyword, string filter = "")
    {
        if(allResourceData.ContainsKey(folderName))
        {
            HashSet<ResourceData> filenameData = allResourceData[folderName];
            List<ResourceData> filenameDataList = filenameData.ToList();
            ResourceData data = filenameDataList.Find(data => data.keyword.ToLower() == keyword.ToLower() && data.filterWord.ToLower() == filter.ToLower());
            if(data != null)
            {
                Debug.Log($"Resource object named {data.resourceObject.name} was found");
                return data.resourceObject;
            }
        }
        return null;
    }

    public List<string> ReturnValidFilterWords(string folderName)
    {
        FilenameToKeywordGeneratorBase generatorBase = keywordGenerationSettings.Find(generatorBase => generatorBase.fileName.ToLower() == folderName.ToLower());
        // foreach(string filter in generatorBase.filterWords)
        // {
        //     Debug.Log($"We have found the filter word {filter}");
        // }
        return generatorBase.filterWords;
    }

    /* ----- ALL INITIALIZATION METHODS FOR GENERATING MAIN KEYWORDS ----- */
    // Initializes the dictionary containing all the objects for each of the filenames
    private void InitializeResourceContents()
    {
        allResourceObjects = new Dictionary<string, Object[]>();
        // Search through all of Resources to check if it is a valid filename. 
        foreach(FilenameToKeywordGeneratorBase file in keywordGenerationSettings)
        {
            // Debug.Log($"{file.fileName} is the first filename we are looking for in Resources");
            Object[] allFiles = Resources.LoadAll(file.fileName, typeof(GameObject));
            if(!allResourceObjects.ContainsKey(file.fileName))
            {
                // Debug.Log($"{file.fileName} was found in Resources");
                allResourceObjects[file.fileName] = allFiles;
            }
        }
    }

    // Initializes the dictionary for both the abbreviation and character separators
    private void InitializeKeywordGenerationSetup()
    {
        allAbbreviations = new Dictionary<string, Dictionary<string, string>>();
        allCharacterDividers = new Dictionary<string, char>();
        allFilterWords = new Dictionary<string, List<string>>();

        foreach(FilenameToKeywordGeneratorBase keybase in keywordGenerationSettings)
        {
            string currFilename = keybase.fileName;
            Dictionary<string, string> currAbbreviations = new Dictionary<string, string>();

            if(keybase.abbreviations != null || keybase.abbreviations.Count > 0)
            {
                foreach(AbbreviationMapping abbrev in keybase.abbreviations)
                {
                    currAbbreviations[abbrev.fullName.ToLower()] = abbrev.abbreviation.ToLower();
                    // Debug.Log($"{abbrev.fullName} has been added with the abbreviation of {currAbbreviations[abbrev.fullName.ToLower()]}");
                }
            }
            
            if(!allAbbreviations.ContainsKey(currFilename))
            {
                allAbbreviations[currFilename] = currAbbreviations;
            }

            if(!allCharacterDividers.ContainsKey(currFilename))
            {
                allCharacterDividers[currFilename] = keybase.endKeywordSubstringAtChar;
            }

            if(!allFilterWords.ContainsKey(currFilename))
            {
                allFilterWords[currFilename] = keybase.filterWords;
            }


        }
    }

    private void InitializeAllKeywords()
    {
        allResourceData = new Dictionary<string, HashSet<ResourceData>>();
        foreach(var kvp in allResourceObjects)
        {
            string folderName = kvp.Key;
            if(!allResourceData.ContainsKey(folderName))
            {
                InitializeFilenameKeyWords(folderName);
            }
            
        }
    }

    private void InitializeFilenameKeyWords(string folderName)
    {
        char endSubstringAtChar = allCharacterDividers[folderName];
        Dictionary<string, string> abbrevDictionary = allAbbreviations[folderName];
        List<string> filterWords = allFilterWords[folderName];

        HashSet<ResourceData> allPrefabData = new HashSet<ResourceData>();
       
        if(allResourceObjects.ContainsKey(folderName))
        {
            // Loads all prefabs defined in allResourceObjects dictionary
            Object[] allPrefabs = allResourceObjects[folderName];

            // Loop through all of the prefabs assigned to the respective filenames:
            foreach(Object prefab in allPrefabs)
            {
                string targetName = prefab.name;
                string trimmedName = prefab.name;
                string filterWord = "";

                // checks if the substring is doable
                int indexOfSubstringEnder = targetName.IndexOf(endSubstringAtChar);

                if(indexOfSubstringEnder > 0 && endSubstringAtChar != ' ')
                {
                    trimmedName = targetName.Substring(0, indexOfSubstringEnder).Trim();
                    targetName = trimmedName;
                }
                    
                // Also checks if the name is in the abbreviation dicitonary
                if(abbrevDictionary.ContainsKey(targetName.ToLower()))
                {
                    targetName = abbrevDictionary[targetName.ToLower()];
                }

                // If we are using filter words, check if the prefab's name contains any of them:
                if(filterWords.Count > 0)
                {
                    filterWord = GetMatchingFilterWord(prefab.name, filterWords);
                }
                
                Object targetPrefab = Resources.Load($"{folderName}/{prefab.name}");
                ResourceData prefabData = new ResourceData(targetName.ToLower(), filterWord, resourceObject: targetPrefab);

                // HashSets prevent duplicate entries from being added:
                allPrefabData.Add(prefabData);
                // Debug.Log($"{prefab.name} has been added with the keyword {prefabData.keyword}, the filter word {prefabData.filterWord}, and with the loaded object {prefabData.resourceObject.name}.");
            }
            
            allResourceData[folderName] = allPrefabData;
            // fileNamesWithMainKeywords[filename] = validKeywords;
            // Debug.Log($"{filename} features over {allPrefabData.Count} elements");
        }
    }

    // If there doesn't exist a matching filter word, just return an empty string.
    private string GetMatchingFilterWord(string prefabName, List<string> filterWords)
    {
        foreach(string filter in filterWords)
        {
            if(prefabName.ToLower().Contains(filter.ToLower()))
            {
                return filter;
            }
        }

        return "";
    }
    
}
