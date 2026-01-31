using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Diagnostics;

public class GameLogic : MonoBehaviour
{
    public GardenLogic gardenLogicObj;
    public LetterCharacterAnimate LetterCharacterObj;
    public RectTransform LetterCharacterTransform;
    public LetterCharacterAnimate AnimateCharacter1Obj;
    public RectTransform AnimateCharacter1Transform;
    public LetterCharacterAnimate AnimateCharacter2Obj;
    public RectTransform AnimateCharacter2Transform;
    public ImageObject AnimateObject;
    public RectTransform AnimateObjectTransform;
    public string gameState;
    public string characterSelection;
    public int levelNumber;
    public GameObject StartScreen;
    public GameObject CharacterScreen;
    public GameObject LevelScreen;
    public GameObject GardenScreen;
    public GameObject PlayScreen;
    public GameObject AnimateScreen;
    public GameObject AnalyticsScreen;
    private List<string> aWordList;
    private List<string> bWordList;
    private List<string> cWordList;
    private List<string> dWordList;
    private List<string> eWordList;
    private List<string> fWordList;
    private List<string> gWordList;
    private List<string> hWordList;
    private List<string> iWordList;
    private List<string> jWordList;
    private List<string> kWordList;
    private List<string> lWordList;
    private List<string> mWordList;
    private List<string> nWordList;
    private List<string> oWordList;
    private List<string> pWordList;
    private List<string> qWordList;
    private List<string> rWordList;
    private List<string> sWordList;
    private List<string> tWordList;
    private List<string> uWordList;
    private List<string> vWordList;
    private List<string> wWordList;
    private List<string> xWordList;
    private List<string> yWordList;
    private List<string> zWordList;
    private List<string>[] wordsListArray;
    private string[] letterList;
    private string[] wordDisplayArray;
    private string currentDisplayLetter;
    private int[] currentLetterIndex;
    private int currentLetterPointer;
    private string[] correctAudioArray;
    private string[] incorrectAudioArray;
    public GridObject[] gridObjects;
    public RectTransform[] gridTransforms;
    public Image DisplayLetterText;
    [SerializeField] private TextMeshProUGUI DisplayLetterTMP;  // new text-based letter
    public RectTransform DisplayGridTransform;
    public RectTransform DisplayLetterTransform;
    public GameObject[] starObjects;
    public int currentGridClick;
    int CorrectLetterCounter;
    int CorrectLetterTotal;
    int letterIndex;
    bool audioPlayIntro;
    public TextMeshProUGUI todayText;
    public TextMeshProUGUI weekText;
    public TextMeshProUGUI monthText;
    public TextMeshProUGUI alltimeText;
    public TextMeshProUGUI[] gardenLetterLabels; // size 26, A–Z in order
    string todayDateString;
    public SaveDataObject saveDataObj;
    public SaveDataObject weekDataObj;
    public SaveDataObject monthDataObj;
    public float startTime;
    public float elapsedTime;
    public TextMeshProUGUI[] LevelsText;
    public TextMeshProUGUI[] PercentText;
    public TextMeshProUGUI[] TimeSpentText;
    public TMPro.TMP_Dropdown LettersMenu;
    public int AnimationBreakNumber;
    public int RandomCharIndex1;
    public int RandomCharIndex2;
    public int RandomLetterIndex;
    public int RandomItemIndex;
    public GameObject MusicOnObj;
    public GameObject MusicOffObj;
    public AudioSource MusicSource;
    public GameObject ResetConfirmPanel;   // optional: confirmation popup (can be null if unused.)
    //PlayScene Letter Box Variable
    // Pop animation base sizes
    private Vector2 baseGridSize;
    private Vector2 baseLetterSize;
    //Audio Logic variables: 
    // Main VO source (same AudioSource you've been using on this GameObject)
    [SerializeField] private AudioSource voSource;
    // Track what we last played so we can avoid spam
    private string currentVOKey = null;
    private float lastVOStartTime = -999f;
    // Tweak these in the Inspector if needed
    [SerializeField] private float sameLineCooldown = 0.5f;   // Same line cannot replay within 0.5s
    [SerializeField] private float minGapBetweenLines = 0.1f; // Tiny gap between any two VO lines
    //New variables for new logic!
    public enum LetterCaseMode { Uppercase, Lowercase, Mixed }
    public LetterCaseMode letterCaseMode = LetterCaseMode.Lowercase;
    // Cache "home" positions for each tile
    private Vector3[] gridBasePositions;

    [Header("Music Playlist")]
    [SerializeField] private List<AudioClip> musicTracks = new List<AudioClip>();
    [SerializeField] private bool shuffle = false;
    [SerializeField] private bool loopPlaylist = true;
    [SerializeField] private bool keepCurrentTrackOnToggle = true;

    private int currentTrackIndex = 0;
    private bool playlistActive = false;   // are we managing auto-advance?
    private bool wasPlayingBeforeMute = false;


    // Start is called before the first frame update
    void Start()
    {
        gameState = "StartGame";
        currentGridClick = -1;
        // Load Previous Data
        todayDateString = CreateFilename(0);
        UnityEngine.Debug.Log(todayDateString);
        saveDataObj = new SaveDataObject();
        saveDataObj.Init();
        saveDataObj.InitSum();
        LoadData(todayDateString, saveDataObj);
        for (int i = 0; i < 26; i++)
        {
            UnityEngine.Debug.Log("LP" + saveDataObj.medlevelPlayed[i]);
        }
        LoadWeekData();
        LoadMonthData();
        playVOAudio("title_new");
        toggleMusicOn();
        //save the box and letter size
        baseGridSize = DisplayGridTransform.sizeDelta;
        baseLetterSize = DisplayLetterTransform.sizeDelta;

    }
    // --------------------- Load/Save Usage Data ---------------------------//

    // A - Z, Datename, Levels, % Correct, Time Spent'

    private string CreateFilename(int _days)
    {
        System.DateTime dt = System.DateTime.Now.AddDays(_days);
        string Day = dt.Day.ToString();
        string Month = dt.Month.ToString();
        string Year = dt.Year.ToString();
        if (Day.Length == 1)
        {
            Day = "0" + Day;
        }
        if (Month.Length == 1)
        {
            Month = "0" + Month;
        }
        string dateString = Year + Month + Day;
        return dateString;
    }

    private void LoadWeekData()
    {
        string loadDateString;
        weekDataObj = new SaveDataObject();
        weekDataObj.Init();
        weekDataObj.InitSum();
        for (int i = 0; i < 7; i++)
        {
            loadDateString = CreateFilename(-i);
            LoadData(loadDateString, weekDataObj);
        }
    }

    private void LoadMonthData()
    {
        string loadDateString;
        monthDataObj = new SaveDataObject();
        monthDataObj.Init();
        monthDataObj.InitSum();
        for (int i = 0; i < 30; i++)
        {
            loadDateString = CreateFilename(-i);
            LoadData(loadDateString, monthDataObj);
        }
    }

    private void LoadData(string _dateName, SaveDataObject _dataObj)
    {
        string path = Application.persistentDataPath + "/" + _dateName + ".save";

        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            // Safety: old files might not have some lists
            int count = (save.lowlevelPlayed != null) ? save.lowlevelPlayed.Count : 0;

            for (int i = 0; i < count; i++)
            {
                // -------- PLAYED --------
                _dataObj.lowlevelPlayed[i] += save.lowlevelPlayed[i];
                _dataObj.medlevelPlayed[i] += save.medlevelPlayed[i];
                _dataObj.highlevelPlayed[i] += save.highlevelPlayed[i];

                // Mixed (new) – only if list exists in this file
                if (save.mixlevelPlayed != null && save.mixlevelPlayed.Count > i)
                {
                    _dataObj.mixlevelPlayed[i] += save.mixlevelPlayed[i];
                }

                // -------- MISSES --------
                _dataObj.lowlevelMiss[i] += save.lowlevelMiss[i];
                _dataObj.medlevelMiss[i] += save.medlevelMiss[i];
                _dataObj.highlevelMiss[i] += save.highlevelMiss[i];

                if (save.mixlevelMiss != null && save.mixlevelMiss.Count > i)
                {
                    _dataObj.mixlevelMiss[i] += save.mixlevelMiss[i];
                }

                // -------- TIME --------
                _dataObj.lowlevelTime[i] += save.lowlevelTime[i];
                _dataObj.medlevelTime[i] += save.medlevelTime[i];
                _dataObj.highlevelTime[i] += save.highlevelTime[i];

                if (save.mixlevelTime != null && save.mixlevelTime.Count > i)
                {
                    _dataObj.mixlevelTime[i] += save.mixlevelTime[i];
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("No Save Data");
        }
    }

    private Save CreateSaveDataObject()
    {
        Save save = new Save();

        for (int i = 0; i < 26; i++)
        {
            // UPPER
            save.lowlevelPlayed.Add(saveDataObj.lowlevelPlayed[i]);
            save.lowlevelMiss.Add(saveDataObj.lowlevelMiss[i]);
            save.lowlevelTime.Add((int)saveDataObj.lowlevelTime[i]);

            // LOWER
            save.medlevelPlayed.Add(saveDataObj.medlevelPlayed[i]);
            save.medlevelMiss.Add(saveDataObj.medlevelMiss[i]);
            save.medlevelTime.Add((int)saveDataObj.medlevelTime[i]);

            // WORDS
            save.highlevelPlayed.Add(saveDataObj.highlevelPlayed[i]);
            save.highlevelMiss.Add(saveDataObj.highlevelMiss[i]);
            save.highlevelTime.Add((int)saveDataObj.highlevelTime[i]);

            // NEW: MIXED
            save.mixlevelPlayed.Add(saveDataObj.mixlevelPlayed[i]);
            save.mixlevelMiss.Add(saveDataObj.mixlevelMiss[i]);
            save.mixlevelTime.Add((int)saveDataObj.mixlevelTime[i]);
        }

        return save;
    }

    public void SaveData(string _dateName)
    {
        Save save = CreateSaveDataObject();
        //
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + _dateName + ".save");
        bf.Serialize(file, save);
        file.Close();
    }

    public void DropDownMenuToggleData()
    {
        ParseDisplayData(LettersMenu.value - 1, saveDataObj);
    }

    void ParseDisplayData(int _letterSelect, SaveDataObject _dataObj)
    {
        if (_letterSelect == -1)
        {
            _dataObj.InitSum();
            _dataObj.TotalSum();

            // PLAYED – per mode
            LevelsText[0].text = _dataObj.lowlevelSum.ToString();    // Upper
            LevelsText[1].text = _dataObj.medlevelSum.ToString();    // Lower
            LevelsText[2].text = _dataObj.mixlevelSum.ToString();    // Mixed
            LevelsText[3].text = _dataObj.highlevelSum.ToString();   // Words
            LevelsText[4].text = _dataObj.levelSum.ToString();       // All modes

            float lowPercent = 0f;
            float medPercent = 0f;
            float mixPercent = 0f;
            float highPercent = 0f;
            float totalPercent = 0f;

            if (_dataObj.lowlevelSum != 0)
            {
                lowPercent = 100f - (100f * (_dataObj.lowmissSum /
                            (_dataObj.lowlevelSum * 3f + _dataObj.lowmissSum)));
            }
            if (_dataObj.medlevelSum != 0)
            {
                medPercent = 100f - (100f * (_dataObj.medmissSum /
                            (_dataObj.medlevelSum * 3f + _dataObj.medmissSum)));
            }
            if (_dataObj.mixlevelSum != 0)
            {
                mixPercent = 100f - (100f * (_dataObj.mixmissSum /
                            (_dataObj.mixlevelSum * 3f + _dataObj.mixmissSum)));
            }
            if (_dataObj.highlevelSum != 0)
            {
                highPercent = 100f - (100f * (_dataObj.highmissSum /
                            (_dataObj.highlevelSum * 3f + _dataObj.highmissSum)));
            }

            if (_dataObj.levelSum != 0)
            {
                totalPercent = 100f - (100f * (_dataObj.missSum /
                              (_dataObj.levelSum * 3f + _dataObj.missSum)));
            }

            PercentText[0].text = lowPercent.ToString("F1");
            PercentText[1].text = medPercent.ToString("F1");
            PercentText[2].text = mixPercent.ToString("F1");
            PercentText[3].text = highPercent.ToString("F1");
            PercentText[4].text = totalPercent.ToString("F1");

            float lowMinutes = _dataObj.lowtimeSum / 60f;
            float medMinutes = _dataObj.medtimeSum / 60f;
            float mixMinutes = _dataObj.mixtimeSum / 60f;
            float highMinutes = _dataObj.hightimeSum / 60f;
            float totalMinutes = _dataObj.timeSum / 60f;

            TimeSpentText[0].text = lowMinutes.ToString("F1");
            TimeSpentText[1].text = medMinutes.ToString("F1");
            TimeSpentText[2].text = mixMinutes.ToString("F1");
            TimeSpentText[3].text = highMinutes.ToString("F1");
            TimeSpentText[4].text = totalMinutes.ToString("F1");
        }
        else
        {
            _dataObj.InitSum();
            _dataObj.LevelSum(_letterSelect);

            // PLAYED – this letter
            LevelsText[0].text = _dataObj.lowlevelPlayed[_letterSelect].ToString();   // Upper
            LevelsText[1].text = _dataObj.medlevelPlayed[_letterSelect].ToString();   // Lower
            LevelsText[2].text = _dataObj.mixlevelPlayed[_letterSelect].ToString();   // Mixed
            LevelsText[3].text = _dataObj.highlevelPlayed[_letterSelect].ToString();  // Words
            LevelsText[4].text = _dataObj.levelSum.ToString();                        // All

            float lowPercent = 0f;
            float medPercent = 0f;
            float mixPercent = 0f;
            float highPercent = 0f;
            float totalPercent = 0f;

            if (_dataObj.lowlevelPlayed[_letterSelect] != 0)
            {
                lowPercent = 100f - (100f *
                    (_dataObj.lowlevelMiss[_letterSelect] /
                    (_dataObj.lowlevelPlayed[_letterSelect] * 3f + _dataObj.lowlevelMiss[_letterSelect])));
            }
            if (_dataObj.medlevelPlayed[_letterSelect] != 0)
            {
                medPercent = 100f - (100f *
                    (_dataObj.medlevelMiss[_letterSelect] /
                    (_dataObj.medlevelPlayed[_letterSelect] * 3f + _dataObj.medlevelMiss[_letterSelect])));
            }
            if (_dataObj.mixlevelPlayed[_letterSelect] != 0)
            {
                mixPercent = 100f - (100f *
                    (_dataObj.mixlevelMiss[_letterSelect] /
                    (_dataObj.mixlevelPlayed[_letterSelect] * 3f + _dataObj.mixlevelMiss[_letterSelect])));
            }
            if (_dataObj.highlevelPlayed[_letterSelect] != 0)
            {
                highPercent = 100f - (100f *
                    (_dataObj.highlevelMiss[_letterSelect] /
                    (_dataObj.highlevelPlayed[_letterSelect] * 3f + _dataObj.highlevelMiss[_letterSelect])));
            }

            if (_dataObj.levelSum != 0)
            {
                totalPercent = 100f - (100f *
                    (_dataObj.missSum / (_dataObj.levelSum * 3f + _dataObj.missSum)));
            }

            PercentText[0].text = lowPercent.ToString("F1");
            PercentText[1].text = medPercent.ToString("F1");
            PercentText[2].text = mixPercent.ToString("F1");
            PercentText[3].text = highPercent.ToString("F1");
            PercentText[4].text = totalPercent.ToString("F1");

            float lowMinutes = _dataObj.lowlevelTime[_letterSelect] / 60f;
            float medMinutes = _dataObj.medlevelTime[_letterSelect] / 60f;
            float mixMinutes = _dataObj.mixlevelTime[_letterSelect] / 60f;
            float highMinutes = _dataObj.highlevelTime[_letterSelect] / 60f;
            float totalMinutes = _dataObj.timeSum / 60f;

            TimeSpentText[0].text = lowMinutes.ToString("F1");
            TimeSpentText[1].text = medMinutes.ToString("F1");
            TimeSpentText[2].text = mixMinutes.ToString("F1");
            TimeSpentText[3].text = highMinutes.ToString("F1");
            TimeSpentText[4].text = totalMinutes.ToString("F1");
        }
    }

    // ---------------------  Stats Helpers ---------------------------------------//

    // 0 = Upper, 1 = Lower, 2 = Both, 3 = Words
    private int GetCurrentStatsBucket()
    {
        // Words mode still uses the old "hard/words" bucket
        if (levelNumber == 2)
        {
            return 3; // Words column (highlevel*)
        }

        // Letter modes: route by case
        switch (letterCaseMode)
        {
            case LetterCaseMode.Uppercase:
                return 0; // Upper -> lowlevel*
            case LetterCaseMode.Lowercase:
                return 1; // Lower -> medlevel*
            case LetterCaseMode.Mixed:
                return 2; // Both -> mixlevel*
            default:
                return 0;
        }
    }

    private void AddPlayedStat(int letterIndex)
    {
        int bucket = GetCurrentStatsBucket();

        switch (bucket)
        {
            case 0: // Upper
                saveDataObj.lowlevelPlayed[letterIndex] += 1;
                break;
            case 1: // Lower
                saveDataObj.medlevelPlayed[letterIndex] += 1;
                break;
            case 2: // Both
                saveDataObj.mixlevelPlayed[letterIndex] += 1;
                break;
            case 3: // Words
                saveDataObj.highlevelPlayed[letterIndex] += 1;
                break;
        }
    }

    private void AddMissStat(int letterIndex)
    {
        int bucket = GetCurrentStatsBucket();

        switch (bucket)
        {
            case 0: // Upper
                saveDataObj.lowlevelMiss[letterIndex] += 1;
                break;
            case 1: // Lower
                saveDataObj.medlevelMiss[letterIndex] += 1;
                break;
            case 2: // Both
                saveDataObj.mixlevelMiss[letterIndex] += 1;
                break;
            case 3: // Words
                saveDataObj.highlevelMiss[letterIndex] += 1;
                break;
        }
    }

    private void AddTimeStat(int letterIndex, float seconds)
    {
        int bucket = GetCurrentStatsBucket();
        int s = Mathf.RoundToInt(seconds);

        switch (bucket)
        {
            case 0: // Upper
                saveDataObj.lowlevelTime[letterIndex] += s;
                break;
            case 1: // Lower
                saveDataObj.medlevelTime[letterIndex] += s;
                break;
            case 2: // Both
                saveDataObj.mixlevelTime[letterIndex] += s;
                break;
            case 3: // Words
                saveDataObj.highlevelTime[letterIndex] += s;
                break;
        }
    }

    // ---------------------  Reset Stats (shared tablet)  ---------------------------//

    // Called by the main "Reset Stats" button (shows confirm popup, or resets directly)
    public void OnResetStatsButton()
    {
        // If you don't want a confirmation popup, just call DoResetStats() here.
        if (ResetConfirmPanel != null)
        {
            ResetConfirmPanel.SetActive(true);
        }
        else
        {
            DoResetStats();
        }
    }

    // Called by "Yes" button in confirm popup
    public void OnConfirmResetYes()
    {
        DoResetStats();

        if (ResetConfirmPanel != null)
            ResetConfirmPanel.SetActive(false);
    }

    // Called by "No" / "Cancel" button in confirm popup
    public void OnConfirmResetNo()
    {
        if (ResetConfirmPanel != null)
            ResetConfirmPanel.SetActive(false);
    }

    // Actual logic that wipes stats + files and refreshes UI
    private void DoResetStats()
    {
        // 1) Delete all saved stat files for this app
        string path = Application.persistentDataPath;
        string[] saveFiles = System.IO.Directory.GetFiles(path, "*.save");
        foreach (var f in saveFiles)
        {
            System.IO.File.Delete(f);
        }

        // 2) Re-init in-memory stat objects
        saveDataObj.Init();
        saveDataObj.InitSum();

        weekDataObj.Init();
        weekDataObj.InitSum();

        monthDataObj.Init();
        monthDataObj.InitSum();

        // 3) Reload week/month aggregates (everything is zero now anyway,
        // but this keeps the flow consistent)
        LoadWeekData();
        LoadMonthData();

        // 4) If Analytics screen is open, refresh what’s shown
        ParseDisplayData(-1, saveDataObj);

        UnityEngine.Debug.Log("All stats reset for this device.");
    }

    // --------------------- Grid Click Event -------------------------------//

    public void CheckGridClick(int _gridNumber)
    {
        // NEW: case-insensitive comparison so A == a
        if (string.Equals(
                gridObjects[_gridNumber].gridLetter,
                currentDisplayLetter,
                StringComparison.OrdinalIgnoreCase))
        {
            // ---------------- CORRECT ----------------
            if (!gridObjects[_gridNumber].clickObjectFlag && gridObjects[_gridNumber].gameObject.activeSelf)
            {
                UnityEngine.Debug.Log("CORRECT");
                gridObjects[_gridNumber].clickObjectFlag = true;

                starObjects[_gridNumber].GetComponent<Stars>().Animate();

                // Correct Answer
                if (levelNumber == 2)
                {
                    // Play Word Sound
                    playWordAudio(wordDisplayArray[_gridNumber]);
                }
                else
                {
                    // Play Letter Sound
                    playLetterAudio(wordDisplayArray[_gridNumber].Substring(0, 1));
                }

                // Decoupled from animation. BV 2/8/24
                CorrectLetterCounter++;
                LetterCharacterObj.ShowCharacter(currentLetterPointer);

                if (currentGridClick == -1)
                {
                    currentGridClick = _gridNumber;
                    LetterCharacterTransform.localPosition = new Vector3(
                        LetterCharacterTransform.localPosition.x,
                        gridObjects[currentGridClick].transform.localPosition.y,
                        LetterCharacterTransform.localPosition.z);

                    LeanTween
                        .move(LetterCharacterTransform,
                              gridObjects[currentGridClick].transform.localPosition,
                              0.75f)
                        .setOnComplete(OnAnimateOffScreen);
                }
                else // Person clicked too fast. So remove the previous letter and set the next letter to be removed.
                {
                    gridObjects[currentGridClick].GetComponent<GridObject>().followCharacterFlag = true;
                    currentGridClick = _gridNumber;
                    gridObjects[currentGridClick].GetComponent<GridObject>().followCharacterFlag = true;

                    LeanTween.cancelAll();
                    LeanTween
                        .move(LetterCharacterTransform,
                              gridObjects[currentGridClick].transform.localPosition,
                              0.75f)
                        .setOnComplete(OnAnimateOffScreen);
                }
            }
        }
        else
        {
            // ---------------- INCORRECT ----------------
            UnityEngine.Debug.Log("INCORRECT");

            if (currentGridClick == -1)
            {
                playIncorrectAudio();
                ShakeGrid();

                // Single-letter mode safety
                int statsLetter = (currentLetterIndex != null && currentLetterIndex.Length > 0)
                    ? currentLetterIndex[0]
                    : currentLetterPointer;

                // NEW: route miss into the correct bucket (Upper / Lower / Both / Words)
                AddMissStat(statsLetter);
            }
        }
    }

    // -------------------- Animate Stars FX --------------------------------//
    public void ResetStars()
    {
        for (int i = 0; i < 6; i++)
        {
            starObjects[i].GetComponent<Stars>().Reset();
        }
    }

    // --------------------   Load/Display Images  --------------------------//

    // get list of image filenames and put into wordlist array

    // put images into ArrayList
    public void FillGridImages()
    {
        for (int i = 0; i < gridObjects.Length; i++)
        {
            gridObjects[i].DisplayWordImage(wordDisplayArray[i]);
            gridObjects[i].DisplayWordText(wordDisplayArray[i]);
        }
    }

    public void SetGridObjectDisplays()
    {
        for (int i = 0; i < 6; i++)
        {
            GridObject go = gridObjects[i];

            // Word for this tile
            go.gridWord = wordDisplayArray[i];

            // Logical letter for checks/stats – always lowercase
            string baseLetter = go.gridWord.Substring(0, 1).ToLower();
            go.gridLetter = baseLetter;

            // How we want the letter to LOOK to the player
            string visualLetter = baseLetter;

            // Letter-case styling only matters in letter modes (not full-word mode)
            if (levelNumber != 2)
            {
                switch (letterCaseMode)
                {
                    case LetterCaseMode.Uppercase:
                        visualLetter = baseLetter.ToUpper();
                        break;

                    case LetterCaseMode.Lowercase:
                        visualLetter = baseLetter.ToLower();
                        break;

                    case LetterCaseMode.Mixed:
                        bool showUpper = UnityEngine.Random.value > 0.5f;
                        visualLetter = showUpper ? baseLetter.ToUpper() : baseLetter.ToLower();
                        break;
                }
            }

            // ----------------- MODE 4: FULL WORDS (levelNumber == 2) -----------------
            if (levelNumber == 2)
            {
                go.ClearLetterText();

                // ✅ WORDS MODE: display word text in all caps
                go.DisplayWordText(go.gridWord.ToUpper());

                // IMPORTANT: keep the ORIGINAL word for image lookup
                go.DisplayWordImage(go.gridWord);

                go.gridImage.color = new Color(1f, 1f, 1f, 1f);
                go.ClearLetterImage();
            }
            else
            {
                // ----------------- MODES 1–3: LETTER MODES -----------------

                // Show letter via TMP so we can do upper / lower / mixed
                go.DisplayLetterText(visualLetter);

                // We don’t need word text in letter modes
                go.ClearWordText();

                // If you still want a “medium” style that shows the picture,
                // keep this branch. If you’re not using level 1 anymore,
                // everything will fall into the "else" case and be letter-only.
                if (levelNumber == 1)
                {
                    // Letter + image
                    go.DisplayWordImage(go.gridWord);
                    go.gridImage.color = new Color(1f, 1f, 1f, 0.6f);
                }
                else
                {
                    // Letter only
                    go.ClearWordImage();
                    go.gridImage.color = new Color(1f, 1f, 1f, 1f);
                }

                // We’re using TMP for the letter, so blank out the letter sprite slot
                go.ClearLetterImage();
            }
        }
    }

    // ---------------------  Alphabet Content Selection ----------------------//

    private void GenerateVocabularyList()
    {
        letterList = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        //
        //TO DO: Generate Word List from json file
        aWordList = new List<string>() { "acorn", "angel", "apricot", "april", "apron" };
        bWordList = new List<string>() { "bag", "ball", "balloon", "banana", "bat", "bee", "bell", "book", "boot", "box", "bread", "broom" };
        cWordList = new List<string>() { "cake", "can", "candle", "cap", "car", "carrot", "clock", "cookie", "cup" };
        dWordList = new List<string>() { "dinosaur", "dish", "doll", "door", "doughnut", "drum", "duck" };
        eWordList = new List<string>() { "ear", "easel", "eel", "eraser" };
        fWordList = new List<string>() { "fan", "feather", "fence", "fire", "fish", "flag", "flower", "fly" };
        gWordList = new List<string>() { "garage", "gate", "gift", "glasses", "glove", "grapes", "guitar" };
        hWordList = new List<string>() { "hamburger", "hammer", "hanger", "hat", "heart", "hook", "horse", "hot dog", "house" };
        iWordList = new List<string>() { "ice cream", "ice cube", "ice skate", "icicle", "iron", "ivy" };
        jWordList = new List<string>() { "jacket", "jar", "jello", "jet", "jewel", "jug", "jumprope" };
        kWordList = new List<string>() { "kettle", "key", "keyboard", "kite", "kitten" };
        lWordList = new List<string>() { "ladder", "lady bug", "lamp", "leaf", "lion", "lollipop" };
        mWordList = new List<string>() { "milk", "mitten", "money", "moon", "mosquito", "mouth", "mug", "mushroom" };
        nWordList = new List<string>() { "nail", "necklace", "needle", "nest", "net", "nose" };
        oWordList = new List<string>() { "oatmeal", "oboe", "ocean", "oval", "overalls" };
        pWordList = new List<string>() { "pancakes", "parrot", "pear", "pen", "pencil", "pie", "pig", "pizza", "plane", "popcorn", "pot", "present", "pumpkin" };
        qWordList = new List<string>() { "quarter", "question mark", "quiet", "quilt" };
        rWordList = new List<string>() { "rabbit", "raspberry", "ring", "rock", "rocket", "rooster" };
        sWordList = new List<string>() { "sandwich", "scissors", "sled", "sock", "spoon", "star", "strawberry", "sun" };
        tWordList = new List<string>() { "table", "tomato", "train", "tree", "truck", "tub" };
        uWordList = new List<string>() { "u turn", "unicycle", "uniform", "united states" };
        vWordList = new List<string>() { "vase", "vegtables", "vest", "violin", "volcano" };
        wWordList = new List<string>() { "wagon", "wallet", "walrus", "water", "watermelon", "wheel", "wood" };
        xWordList = new List<string>() { "x axis", "xray fish", "xray" };
        yWordList = new List<string>() { "yarn", "yellow", "yolk", "yoyo" };
        zWordList = new List<string>() { "zebra", "zipper", "zoom", "zucchini" };
        //
        wordsListArray = new List<string>[] { aWordList, bWordList, cWordList, dWordList, eWordList, fWordList, gWordList, hWordList, iWordList, jWordList, kWordList, lWordList, mWordList, nWordList, oWordList, pWordList, qWordList, rWordList, sWordList, tWordList, uWordList, vWordList, wWordList, xWordList, yWordList, zWordList };
    }

    private void ExcludeFromVocabularyList()
    {
    }

    public List<string> FindCorrentWords(string _word, int _num)
    {
        List<string> randomWords = new List<string>();
        return randomWords;
    }

    public void ClearWordDisplay()
    {
        wordDisplayArray = new string[6];
    }

    public void AddWord(int _pos, string _word)
    {
        wordDisplayArray[_pos] = _word;
    }

    public void RandomWordDisplay()
    {
        RandomizeWordArray(wordDisplayArray);
    }

    public void RandomizeWordArray(string[] _words)
    {
        for (int i = _words.Length - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i + 1);
            string tmp = _words[i];
            _words[i] = _words[r];
            _words[r] = tmp;
        }
    }

    public void RandomizeWordList(List<string> _words)
    {
        for (int i = _words.Count - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i + 1);
            string tmp = _words[i];
            _words[i] = _words[r];
            _words[r] = tmp;
        }
    }

    private void UpdateGardenLetterVisuals()
    {
        if (gardenLetterLabels == null || gardenLetterLabels.Length == 0)
            return;

        for (int i = 0; i < gardenLetterLabels.Length; i++)
        {
            char baseLetter = (char)('a' + i); // 0 -> 'a', 1 -> 'b', etc.
            string letter = baseLetter.ToString();

            switch (letterCaseMode)
            {
                case LetterCaseMode.Uppercase:
                    gardenLetterLabels[i].text = letter.ToUpper();
                    break;

                case LetterCaseMode.Lowercase:
                    gardenLetterLabels[i].text = letter.ToLower();
                    break;

                case LetterCaseMode.Mixed:
                    gardenLetterLabels[i].text =
                        UnityEngine.Random.value > 0.5f
                            ? letter.ToUpper()
                            : letter.ToLower();
                    break;
            }
        }
    }

    // Task 02: Single-letter mode
    // Previously supported 3-letter clusters with wrap-around (Y/Z → A/B).
    // Now intentionally restricted to one letter per round.
    public void SetLetterIndex(int _num)
    {
        currentLetterIndex = new int[] { _num };
        letterIndex = 0;
    }

    /*public void SetLetterIndex(int _num)
    {
        currentLetterIndex = new int[] { _num, _num + 1, _num + 2 };
        if (_num == 24)
        {
            currentLetterIndex = new int[] { _num, _num + 1, 0 };
        }
        if (_num == 25)
        {
            currentLetterIndex = new int[] { _num, 0, 1 };
        }
    }
    */

    /*
    public void SetCurrentLetter(int _num)
    {
        currentLetterPointer = _num;

        // Canonical logical letter (always lowercase)
        currentDisplayLetter = letterList[_num].ToLower();

        // Sprites are named using the base (lowercase) letter
        string imageFilename = "Letters/" + currentDisplayLetter + "_letter";
        DisplayLetterText.sprite = Resources.Load<Sprite>(imageFilename);

        // If you ever switch to text instead of an image for the big letter,
        // you could apply letterCaseMode here to control its visual case.
        // e.g. DisplayLetterTextTMP.text = ...;
    }
    */

    public void SetCurrentLetter(int _num)
    {
        currentLetterPointer = _num;

        // Canonical logical letter (always lowercase for checks/audio)
        currentDisplayLetter = letterList[_num].ToLower();

        string visual;

        // WORDS MODE OVERRIDE: big letter is always a single uppercase letter
        if (levelNumber == 2)
        {
            visual = currentDisplayLetter.ToUpper();
        }
        else
        {
            // Letter modes only
            switch (letterCaseMode)
            {
                case LetterCaseMode.Uppercase:
                    visual = currentDisplayLetter.ToUpper();
                    break;

                case LetterCaseMode.Lowercase:
                    visual = currentDisplayLetter.ToLower();
                    break;

                case LetterCaseMode.Mixed:
                    // Mixed letter mode: show both side-by-side
                    visual = $"{currentDisplayLetter.ToUpper()}{currentDisplayLetter.ToLower()}";
                    break;

                default:
                    visual = currentDisplayLetter;
                    break;
            }
        }

        if (DisplayLetterTMP != null)
        {
            DisplayLetterTMP.text = visual;

            if (DisplayLetterText != null)
                DisplayLetterText.gameObject.SetActive(false);
        }
    }




    public void GenerateRandomDisplayList()
	{
		// Put all correct words in ArrayList
		List<string> randomCorrectWords = new List<string>();
		for (int i = 0; i < wordsListArray[currentLetterPointer].Count; i++)
		{
			randomCorrectWords.Add(wordsListArray[currentLetterPointer][i]);
		}
		// Randomize correct words ArrayList
		RandomizeWordList(randomCorrectWords);

		// Put all remaining words in ArrayList
		List<string> randomIncorrectWords = new List<string>();
		for (int j = 0; j < 26; j++)
		{
			if (j != currentLetterPointer)
			{
				for (int k = 0; k < wordsListArray[j].Count; k++)
				{
					randomIncorrectWords.Add(wordsListArray[j][k]);
				}
			}
		}
		// Randomize incorrect words ArrayList
		RandomizeWordList(randomIncorrectWords);

		// Number of Correct Letter Tiles
		CorrectLetterCounter = 0;
		CorrectLetterTotal = 3;

		// Create WordDisplayArray
		ClearWordDisplay();
		AddWord(0, randomCorrectWords[0]);
		AddWord(1, randomCorrectWords[1]);
		AddWord(2, randomCorrectWords[2]);
		AddWord(3, randomIncorrectWords[0]);
		AddWord(4, randomIncorrectWords[1]);
		AddWord(5, randomIncorrectWords[2]);

		// Randomize WordDisplayArray
		RandomWordDisplay();
	}

	public void SetupGrid()
	{
		ResetStars();
		GenerateRandomDisplayList();
		SetGridObjectDisplays();
	}

    // --------------------   Audio Playback   ------------------------------//

    private AudioSource GetVOSource()
    {
        if (!voSource)
            voSource = GetComponent<AudioSource>(); // same as your old code

        return voSource;
    }

    // Decide if we're allowed to play a VO line right now
    private bool CanPlayVO(string key)
    {
        var source = GetVOSource();
        if (!source)
            return false;

        float now = Time.time;

        // Don't fire a bunch of lines in the same frame
        if (now - lastVOStartTime < minGapBetweenLines)
            return false;

        // Same line recently? ignore to avoid "try again try again try again..."
        if (!string.IsNullOrEmpty(currentVOKey) &&
            currentVOKey == key &&
            now - lastVOStartTime < sameLineCooldown)
        {
            return false;
        }

        // If something is already playing, just ignore this new request.
        // (This is the main "no audio soup" rule.)
        if (source.isPlaying)
            return false;

        return true;
    }

    // Actually play the clip and record what we did
    private void PlayVOClip(AudioClip clip, string key)
    {
        var source = GetVOSource();
        if (!source || !clip)
            return;

        source.PlayOneShot(clip);

        currentVOKey = key;
        lastVOStartTime = Time.time;
    }

    // Optional helper if you ever want to forcibly stop VO from another place
    public void StopVO()
    {
        var source = GetVOSource();
        if (source && source.isPlaying)
            source.Stop();

        currentVOKey = null;
    }

    // put audio into ArrayList
    private void InitAudioAssets()
	{
		correctAudioArray = new string[] { "ex_00", "ex_01", "ex_02", "ex_03", "ex_04", "ex_05", "ex_06", "ex_07", "ex_08", "ex_09", "ex_10", "ex_11", "ex_12", "ex_13", "ex_14" };
		incorrectAudioArray = new string[] { "try_another_00", "try_another_01", "try_another_02" };
	}

    // play vo audio
    public void playVOAudio(string _filename)
    {
        string key = "VO_" + _filename;
        if (!CanPlayVO(key))
            return;

        string letterFilename = "VO/" + _filename;
        AudioClip clip = Resources.Load<AudioClip>(letterFilename);

        PlayVOClip(clip, key);
    }

    // play alphabet character audio
    public void playCharacterAudio(int _filenum)
    {
        string[] letters = new string[]
        {
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
        "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
        };

        string _filename = letters[_filenum];

        string key = "CHAR_" + _filename;
        if (!CanPlayVO(key))
            return;

        string letterFilename = "Characters/Audio/" + _filename;
        AudioClip clip = Resources.Load<AudioClip>(letterFilename);

        PlayVOClip(clip, key);
    }

    // play alphabet letter audio
    public void playLetterAudio(string _filename)
    {
        string key = "LETTER_" + _filename;
        if (!CanPlayVO(key))
            return;

        string letterFilename = "Objects/Audio/letters/" + _filename;
        AudioClip clip = Resources.Load<AudioClip>(letterFilename);

        PlayVOClip(clip, key);
    }

    // play word audio
    public void playWordAudio(string _filename)
    {
        char letter = _filename[0];

        string key = "WORD_" + _filename;  // treat each word as unique
        if (!CanPlayVO(key))
            return;

        string wordFilename = "Objects/Audio/words/letter" + letter + "/" + _filename;
        AudioClip clip = Resources.Load<AudioClip>(wordFilename);

        PlayVOClip(clip, key);
    }

    public void playCorrectAudio()
    {
        string key = "CORRECT";
        var source = GetVOSource();
        if (!source) return;

        float now = Time.time;

        // Still avoid literal machine-gun of the *same* "Good job!" line
        if (!string.IsNullOrEmpty(currentVOKey) &&
            currentVOKey == key &&
            now - lastVOStartTime < sameLineCooldown)
        {
            // We just played CORRECT very recently; skip this one
            return;
        }

        // IMPORTANT FEEDBACK: interrupt whatever is talking (letter, word, etc.)
        if (source.isPlaying)
        {
            source.Stop();
        }

        int audioIndex = UnityEngine.Random.Range(0, correctAudioArray.Length);
        string audioFilename = "Correct/" + correctAudioArray[audioIndex];
        AudioClip clip = Resources.Load<AudioClip>(audioFilename);

        // Use the same helper so tracking stays consistent
        PlayVOClip(clip, key);
    }

    // play incorrect audio
    public void playIncorrectAudio()
    {
        string key = "INCORRECT";
        if (!CanPlayVO(key))
            return;

        int audioIndex = UnityEngine.Random.Range(0, incorrectAudioArray.Length);
        string audioFilename = "Incorrect/" + incorrectAudioArray[audioIndex];

        AudioClip clip = Resources.Load<AudioClip>(audioFilename);
        PlayVOClip(clip, key);
    }

    public void toggleMusicOn()
    {
        MusicOnObj.SetActive(true);
        MusicOffObj.SetActive(false);

        MusicSource.mute = false;

        // If music was already playing, just unmute and continue.
        // If nothing is playing, start the playlist.
        if (!MusicSource.isPlaying)
        {
            StartPlaylist(keepCurrentTrackOnToggle);
        }
    }

    public void toggleMusicOff()
    {
        MusicOnObj.SetActive(false);
        MusicOffObj.SetActive(true);

        playlistActive = false;
        MusicSource.Stop();
        MusicSource.mute = true;
    }

    private void StartPlaylist(bool keepCurrent)
    {
        if (musicTracks == null || musicTracks.Count == 0)
        {
            UnityEngine.Debug.LogWarning("Music playlist is empty. Assign tracks in Inspector.");
            return;
        }

        playlistActive = true;

        // If we want to resume the current clip and it's valid, keep it.
        if (keepCurrent && MusicSource.clip != null)
        {
            MusicSource.loop = false; // we are looping playlist, not a single clip
            MusicSource.Play();
            return;
        }

        // Otherwise start from current index (or first track)
        currentTrackIndex = Mathf.Clamp(currentTrackIndex, 0, musicTracks.Count - 1);
        PlayTrackAtIndex(currentTrackIndex);
    }

    private void PlayTrackAtIndex(int index)
    {
        if (musicTracks == null || musicTracks.Count == 0) return;

        index = Mathf.Clamp(index, 0, musicTracks.Count - 1);
        currentTrackIndex = index;

        MusicSource.loop = false; // IMPORTANT: loop playlist, not individual track
        MusicSource.clip = musicTracks[currentTrackIndex];
        MusicSource.Play();
    }

    private void PlayNextTrack()
    {
        if (musicTracks == null || musicTracks.Count == 0) return;

        if (shuffle)
        {
            // Avoid repeating the same track if possible
            if (musicTracks.Count == 1)
            {
                currentTrackIndex = 0;
            }
            else
            {
                int next;
                do { next = UnityEngine.Random.Range(0, musicTracks.Count); }
                while (next == currentTrackIndex);
                currentTrackIndex = next;
            }

            PlayTrackAtIndex(currentTrackIndex);
            return;
        }

        int nextIndex = currentTrackIndex + 1;

        if (nextIndex >= musicTracks.Count)
        {
            if (!loopPlaylist)
            {
                playlistActive = false;
                return;
            }
            nextIndex = 0;
        }

        PlayTrackAtIndex(nextIndex);
    }


    // --------------------  Alphabet Game States ---------------------------//
    public void SetGameState(string _state)
	{
		gameState = _state;
	}

	public void StartGame()
	{
		gameState = "CharacterSelect";
	}

	public void Analytics()
	{
		gameState = "AnalyticsSelect";
	}

	public void PlayGame()
	{
		if (!gardenLogicObj.AnimateFlag)
		{

			ResetGrid();
            letterIndex = 0;
            SetCurrentLetter(currentLetterIndex[0]);
            StopGardenAnimate();
			GardenScreen.SetActive(false);
			SetupGrid();
			gameState = "PlayGame";
		}
		else
		{
			gardenLogicObj.AnimateStop = gardenLogicObj.AnimateCounter;
			gardenLogicObj.randomLetterIndex = gardenLogicObj.LetterCounter + 1;
		}
	}

	public void ChooseCharacter(string _name)
	{
		characterSelection = _name;
		if (_name == "boy")
		{
			gardenLogicObj.SelectChars(0);
		}
		if (_name == "girl")
		{
			gardenLogicObj.SelectChars(1);
		}
		if (_name == "dog")
		{
			gardenLogicObj.SelectChars(2);
		}
		if (_name == "cat")
		{
			gardenLogicObj.SelectChars(3);
		}
		//
		gameState = "LevelSelect";
	}

    public void ChooseLevel(int menuChoice)
    {
        // menuChoice comes from the Button's OnClick() parameter

        switch (menuChoice)
        {
            case 0: // Uppercase letters
                letterCaseMode = LetterCaseMode.Uppercase;
                levelNumber = 0;              // letter layout
                break;

            case 1: // Lowercase letters
                letterCaseMode = LetterCaseMode.Lowercase;
                levelNumber = 0;              // letter layout (same layout, different look)
                break;

            case 2: // Mixed upper + lower letters
                letterCaseMode = LetterCaseMode.Mixed;
                levelNumber = 0;              // still the letter layout
                break;

            case 3: // Full words
                letterCaseMode = LetterCaseMode.Uppercase;  // garden letters become A–Z
                levelNumber = 2;              // the existing "hard / word" layout
                break;

            default:
                // Safety fallback
                letterCaseMode = LetterCaseMode.Lowercase;
                levelNumber = 0;
                break;
        }

        UpdateGardenLetterVisuals();
        gameState = "AnimateGarden";
    }

    public void ClearGlows()
	{
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("Glow");
		foreach (GameObject go in gos)
		{
			go.SetActive(false);
		}
	}

	public void UpdateScreenDisplay()
	{
		HideScreens();
		switch (gameState)
		{
			case "StartGame":
				GenerateVocabularyList();
				InitAudioAssets();
				StartScreen.SetActive(true);
				break;
			case "CharacterSelect":
				CharacterScreen.SetActive(true);
				break;
			case "AnalyticsSelect":
				toggleTimeFrame(0);
				AnalyticsScreen.SetActive(true);
				break;
			case "LevelSelect":
				LevelScreen.SetActive(true);
				break;
			case "AnimateGarden":
				GardenScreen.SetActive(true);
				break;
			case "PlayGame":
				PlayScreen.SetActive(true);
				break;
			case "AnimationBreak":
				AnimateScreen.SetActive(true);
				break;
		}
	}

	// --------------------  End Alphabet Game States ---------------------------//

	public void toggleTimeFrame(int _timeScale)
	{
		switch (_timeScale)
		{
			case 0:
                UnityEngine.Debug.Log("DAY");
				ParseDisplayData(-1, saveDataObj);
				//
				todayText.color = new Color(1f, 1f, 0.25f);
				weekText.color = new Color(1f, 1f, 1f);
				monthText.color = new Color(1f, 1f, 1f);
				alltimeText.color = new Color(1f, 1f, 1f);
				break;
			case 1:
                UnityEngine.Debug.Log("Week");
				ParseDisplayData(-1, weekDataObj);
				//
				todayText.color = new Color(1f, 1f, 1f);
				weekText.color = new Color(1f, 1f, 0.25f);
				monthText.color = new Color(1f, 1f, 1f);
				alltimeText.color = new Color(1f, 1f, 1f);
				break;
			case 2:
                UnityEngine.Debug.Log("Month!");
				ParseDisplayData(-1, monthDataObj);
				//
				todayText.color = new Color(1f, 1f, 1f);
				weekText.color = new Color(1f, 1f, 1f);
				monthText.color = new Color(1f, 1f, 0.25f);
				alltimeText.color = new Color(1f, 1f, 1f);
				break;
			case 3:
				todayText.color = new Color(1f, 1f, 1f);
				weekText.color = new Color(1f, 1f, 1f);
				monthText.color = new Color(1f, 1f, 1f);
				alltimeText.color = new Color(1f, 1f, 0.25f);
				break;
		}
	}

	// -------------------------------------------------------------------------//

	void HideScreens()
	{
		ClearGlows();
        StopVO(); // uses the same AudioSource as the VO manager
        StartScreen.SetActive(false);
		CharacterScreen.SetActive(false);
		LevelScreen.SetActive(false);
		GardenScreen.SetActive(false);
		PlayScreen.SetActive(false);
		AnimateScreen.SetActive(false);
		AnalyticsScreen.SetActive(false);
	}

	public void StopLevelSelect()
	{
		gameState = "CharacterSelect";
	}

	public void StopGardenAnimate()
	{
		gardenLogicObj.AnimateFlag = false;
		gardenLogicObj.ResetGarden();
		gameState = "LevelSelect";
	}

	public void StopPlayGame()
	{
		SaveData(todayDateString);
		LeanTween.cancelAll();
        StopVO();
        StopGardenAnimate();
		gameState = "AnimateGarden";
	}

	public void StopAnimationBreak()
	{
		LeanTween.cancelAll();
		gameState = "AnimateGarden";
	}

	public void SelectGridButton(int _buttonNumber)
	{
        UnityEngine.Debug.Log("Num : " + _buttonNumber);
		// Check Answer / Play Sound
		CheckGridClick(_buttonNumber);
	}

	void OnAnimateOffScreen()
	{
		// "Good Job!"
		if (levelNumber < 2)
		{
			playCorrectAudio();
		}
		//
		// We need to decouple this from animation. BV 2/8/24
		// CorrectLetterCounter++;
		// AnimateToOffScreen
		gridObjects[currentGridClick].GetComponent<GridObject>().followCharacterFlag = true;
		Vector3 offScreenPosition = new Vector3(650f, LetterCharacterTransform.localPosition.y, LetterCharacterTransform.localPosition.z);
		LeanTween.move(LetterCharacterTransform, offScreenPosition, 0.75f).setDelay(1f).setOnComplete(OnAnimateReset);
	}

	void OnAnimateReset()
	{
		// ResetAnimateParameters
		LetterCharacterTransform.localPosition = new Vector3(-650f, LetterCharacterTransform.localPosition.y, LetterCharacterTransform.localPosition.z);
		gridObjects[0].GetComponent<GridObject>().followCharacterFlag = false;
		gridObjects[1].GetComponent<GridObject>().followCharacterFlag = false;
		gridObjects[2].GetComponent<GridObject>().followCharacterFlag = false;
		gridObjects[3].GetComponent<GridObject>().followCharacterFlag = false;
		gridObjects[4].GetComponent<GridObject>().followCharacterFlag = false;
		gridObjects[5].GetComponent<GridObject>().followCharacterFlag = false;
		gridObjects[currentGridClick].gameObject.SetActive(false);
		currentGridClick = -1;
		CheckEndPlayScreen();
	}

	void CheckEndPlayScreen()
	{
		if (CorrectLetterCounter >= CorrectLetterTotal)
		{

            //Round is done: prevent late letter VO from playing.
            audioPlayIntro = false;

            currentGridClick = -2;

			// how far (in local‑space units) each tile should drop straight down
			const float dropAmount = 684f;

			for (int i = 0; i < gridTransforms.Length; i++)
			{
				Vector3 startPos = gridTransforms[i].localPosition;
				LTDescr tween = LeanTween.move(
					gridTransforms[i],
					new Vector3(startPos.x, startPos.y - dropAmount, startPos.z),
					0.5f
				).setDelay(0.5f);

				// fire OnResetPlayScreen once the last tween completes
				if (i == gridTransforms.Length - 1)
				{
					tween.setOnComplete(OnResetPlayScreen);
				}
			}
		}
	}

    void ShakeGrid()
    {
        const float shakeMagnitude = 40f;  // same intensity you had before
        const float duration = 0.15f;
        const int repeatCount = 8;    // keep your original "energy"

        for (int i = 0; i < gridTransforms.Length; i++)
        {
            RectTransform tile = gridTransforms[i];

            // Fallback: if base positions aren't set yet, use whatever we have
            Vector3 basePos = (gridBasePositions != null && gridBasePositions.Length > i)
                ? gridBasePositions[i]
                : tile.localPosition;

            // Cancel any existing tweens on this tile to avoid overlapping shakes
            LeanTween.cancel(tile.gameObject);

            // Random horizontal offset around the base position
            float offsetX = UnityEngine.Random.Range(-0.5f, 0.5f) * shakeMagnitude;
            Vector3 targetPos = new Vector3(basePos.x + offsetX, basePos.y, basePos.z);

            LeanTween.moveLocal(tile.gameObject, targetPos, duration)
                .setEaseInOutCirc()
                .setLoopPingPong()
                .setRepeat(repeatCount)
                .setOnComplete(() =>
                {
                    // Hard snap back to the original cell position at the end
                    tile.localPosition = basePos;
                });
        }
    }

    void ResetGrid()
	{
		LetterCharacterTransform.localPosition = new Vector3(-650f, 0f, 0f);
		currentGridClick = -1;
		//
		gridObjects[0].gameObject.SetActive(true);
		gridObjects[1].gameObject.SetActive(true);
		gridObjects[2].gameObject.SetActive(true);
		gridObjects[3].gameObject.SetActive(true);
		gridObjects[4].gameObject.SetActive(true);
		gridObjects[5].gameObject.SetActive(true);
		//
		gridObjects[0].followCharacterFlag = false;
		gridObjects[1].followCharacterFlag = false;
		gridObjects[2].followCharacterFlag = false;
		gridObjects[3].followCharacterFlag = false;
		gridObjects[4].followCharacterFlag = false;
		gridObjects[5].followCharacterFlag = false;
		//
		gridObjects[0].clickObjectFlag = false;
		gridObjects[1].clickObjectFlag = false;
		gridObjects[2].clickObjectFlag = false;
		gridObjects[3].clickObjectFlag = false;
		gridObjects[4].clickObjectFlag = false;
		gridObjects[5].clickObjectFlag = false;
		//
		gridTransforms[0].localPosition = new Vector3(-162f, 42f, 0f);
		gridTransforms[1].localPosition = new Vector3(1f, 42f, 0f);
		gridTransforms[2].localPosition = new Vector3(164f, 42f, 0f);
		gridTransforms[3].localPosition = new Vector3(-162f, -103f, 0f);
		gridTransforms[4].localPosition = new Vector3(1f, -103f, 0f);
		gridTransforms[5].localPosition = new Vector3(164f, -103f, 0f);

        // Update base positions after resetting the grid layout
        if (gridBasePositions == null || gridBasePositions.Length != gridTransforms.Length)
        {
            gridBasePositions = new Vector3[gridTransforms.Length];
        }

        for (int i = 0; i < gridTransforms.Length; i++)
        {
            gridBasePositions[i] = gridTransforms[i].localPosition;
        }
    }

    void OnResetPlayScreen()
    {
        elapsedTime = Time.time - startTime;

        // Single-letter mode safety: always record against the active letter
        int statsLetter = (currentLetterIndex != null && currentLetterIndex.Length > 0)
            ? currentLetterIndex[0]
            : currentLetterPointer; // fallback

        // NEW: record stats using the current bucket (Upper / Lower / Both / Words)
        AddPlayedStat(statsLetter);
        AddTimeStat(statsLetter, elapsedTime);

        // Task 02: Remove multi-letter cluster progression
        letterIndex = 0;

        // Proceed with the existing flow (animation break / stop)
        if (AnimationBreakNumber == 0)
        {
            RandomCharIndex1 = (int)UnityEngine.Random.Range(0, 25);
            RandomCharIndex2 = (int)UnityEngine.Random.Range(0, 25);
            RandomLetterIndex = (int)UnityEngine.Random.Range(0, 25);
            RandomItemIndex = (int)UnityEngine.Random
                .Range(0, wordsListArray[RandomLetterIndex].Count - 1);

            AnimateCharacter1Obj.ShowCharacter(RandomCharIndex1);
            AnimateCharacter2Obj.ShowCharacter(RandomCharIndex2);
            AnimateObject.DisplayWordImage(
                wordsListArray[RandomLetterIndex][RandomItemIndex]);
        }

        if (AnimationBreakNumber < 3)
        {
            SaveData(todayDateString);
            gameState = "AnimationBreak";
        }
        else
        {
            StopPlayGame();
        }
    }


    void StartAnimation(int _animationNumber)
	{
		switch (_animationNumber)
		{
			case 0:
				AnimateObject.followCharacter1Flag = false;
				AnimateObject.followCharacter2Flag = false;
				LeanTween.move(AnimateCharacter1Transform, new Vector3(-650f, AnimateCharacter1Transform.localPosition.y, AnimateCharacter1Transform.localPosition.z), 0f);
				LeanTween.move(AnimateCharacter2Transform, new Vector3(650f, AnimateCharacter2Transform.localPosition.y, AnimateCharacter2Transform.localPosition.z), 0f);
				LeanTween.move(AnimateObjectTransform, new Vector3(-650f, AnimateCharacter2Transform.localPosition.y, AnimateCharacter2Transform.localPosition.z), 0f);
				AnimationBreak1();
				break;
			case 1:
				AnimateObject.followCharacter1Flag = false;
				AnimateObject.followCharacter2Flag = false;
				LeanTween.move(AnimateCharacter1Transform, new Vector3(-650f, AnimateCharacter1Transform.localPosition.y, AnimateCharacter1Transform.localPosition.z), 0f);
				LeanTween.move(AnimateObjectTransform, new Vector3(100f, AnimateCharacter2Transform.localPosition.y, AnimateCharacter2Transform.localPosition.z), 0f);
				AnimationBreak2();
				break;
			case 2:
				AnimateObject.followCharacter1Flag = false;
				AnimateObject.followCharacter2Flag = false;
				LeanTween.move(AnimateCharacter1Transform, new Vector3(-650f, AnimateCharacter1Transform.localPosition.y, AnimateCharacter1Transform.localPosition.z), 0f);
				LeanTween.move(AnimateCharacter2Transform, new Vector3(650f, AnimateCharacter2Transform.localPosition.y, AnimateCharacter2Transform.localPosition.z), 0f);
				LeanTween.move(AnimateObjectTransform, new Vector3(-650f, AnimateCharacter2Transform.localPosition.y, AnimateCharacter2Transform.localPosition.z), 0f);
				AnimationBreak3();
				break;
		}
	}

	void AnimationBreak1()
	{
		// Move On Screen
		Vector3 onScreenPosition = new Vector3(-100f, AnimateCharacter1Transform.localPosition.y, AnimateCharacter1Transform.localPosition.z);
		LeanTween.move(AnimateCharacter1Transform, onScreenPosition, 1.25f).setOnComplete(OnAnimateBreak1Mid);
	}

	void OnAnimateBreak1Mid()
	{
		// Play Audio
		playCharacterAudio(RandomCharIndex1);
		// Bounce Character
		LeanTween.move(AnimateCharacter1Transform, new Vector3(-100f, AnimateCharacter1Transform.localPosition.y + 30f, 0f), 0.4f).setEaseInOutCirc().setRepeat(8).setLoopPingPong().setOnComplete(OnAnimateBreak1MoveOff);
	}

	void OnAnimateBreak1MoveOff()
	{
		// Move Off Screen
		Vector3 offScreenPosition = new Vector3(650f, AnimateCharacter1Transform.localPosition.y, AnimateCharacter1Transform.localPosition.z);
		LeanTween.move(AnimateCharacter1Transform, offScreenPosition, 1.25f).setOnComplete(OnAnimateBreakEnd);
	}

    void OnAnimateBreakEnd()
    {
        AnimationBreakNumber++;
        if (AnimationBreakNumber < 3)
        {
            ResetGrid();

            letterIndex = 0;
            int nextLetter = (currentLetterIndex != null && currentLetterIndex.Length > 0)
                ? currentLetterIndex[0]
                : currentLetterPointer;

            SetCurrentLetter(nextLetter);

            SetupGrid();
            gameState = "PlayGame";
        }
        else
        {
            StopPlayGame();
        }
    }

    void AnimationBreak2()
	{
		// Move On Screen
		Vector3 onScreenPosition = new Vector3(-100f, AnimateCharacter1Transform.localPosition.y, AnimateCharacter1Transform.localPosition.z);
		LeanTween.move(AnimateCharacter1Transform, onScreenPosition, 1.25f).setOnComplete(OnAnimateBreak2Mid);
	}

	void OnAnimateBreak2Mid()
	{
		// Play Audio
		playWordAudio(wordsListArray[RandomLetterIndex][RandomItemIndex]);
		// Bounce Item
		LeanTween.move(AnimateObjectTransform, new Vector3(100f, AnimateObjectTransform.localPosition.y + 30f, 0f), 0.4f).setEaseInOutCirc().setRepeat(8).setLoopPingPong().setOnComplete(OnAnimateBreak2Move2Item);
	}

	void OnAnimateBreak2Move2Item()
	{
		// Move to Item
		Vector3 itemScreenPosition = new Vector3(AnimateObjectTransform.localPosition.x, AnimateCharacter1Transform.localPosition.y, AnimateCharacter1Transform.localPosition.z);
		LeanTween.move(AnimateCharacter1Transform, itemScreenPosition, 1.25f).setOnComplete(OnAnimateBreak2MoveOff);
	}

	void OnAnimateBreak2MoveOff()
	{
		// Move Off Screen
		AnimateObject.followCharacter1Flag = true;
		Vector3 offScreenPosition = new Vector3(650f, AnimateCharacter1Transform.localPosition.y, AnimateCharacter1Transform.localPosition.z);
		LeanTween.move(AnimateCharacter1Transform, offScreenPosition, 1.25f).setOnComplete(OnAnimateBreakEnd);
	}

	void AnimationBreak3()
	{
		// Move On Screen
		Vector3 onScreen1Position = new Vector3(-100f, AnimateCharacter1Transform.localPosition.y, AnimateCharacter1Transform.localPosition.z);
		Vector3 onScreen2Position = new Vector3(100f, AnimateCharacter2Transform.localPosition.y, AnimateCharacter2Transform.localPosition.z);
		LeanTween.move(AnimateCharacter1Transform, onScreen1Position, 1.2f);
		LeanTween.move(AnimateObjectTransform, onScreen1Position, 1.2f);
		LeanTween.move(AnimateCharacter2Transform, onScreen2Position, 1.2f).setOnComplete(OnAnimateBreak3Mid);
	}

	void OnAnimateBreak3Mid()
	{
		// Play Audio
		playCharacterAudio(RandomCharIndex2);
		// Bounce Item
		LeanTween.move(AnimateCharacter2Transform, new Vector3(100f, AnimateCharacter2Transform.localPosition.y + 30f, 0f), 0.4f).setEaseInOutCirc().setRepeat(8).setLoopPingPong().setOnComplete(OnAnimateBreak3Move2Item);
	}

	void OnAnimateBreak3Move2Item()
	{
		// Move to Item
		Vector3 itemScreenPosition = new Vector3(AnimateCharacter2Transform.localPosition.x, AnimateObjectTransform.localPosition.y, AnimateCharacter2Transform.localPosition.z);
		LeanTween.move(AnimateObjectTransform, itemScreenPosition, 1f).setOnComplete(OnAnimateBreak3MoveOff);
	}

	void OnAnimateBreak3MoveOff()
	{
		// Move Off Screen
		AnimateObject.followCharacter2Flag = true;
		Vector3 offScreenPosition = new Vector3(650f, AnimateCharacter2Transform.localPosition.y, AnimateCharacter2Transform.localPosition.z);
		LeanTween.move(AnimateCharacter2Transform, offScreenPosition, 1.25f).setOnComplete(OnAnimateBreakEnd);
	}


	// Update is called once per frame
	void Update()
	{
		switch (gameState)
		{
			case "StartGame":
				UpdateScreenDisplay();
				gameState = "StartGameWait";
				break;
			case "CharacterSelect":
				UpdateScreenDisplay();
				gameState = "CharacterSelectWait";
				break;
			case "AnalyticsSelect":
				UpdateScreenDisplay();
				ParseDisplayData(-1, saveDataObj);
				gameState = "AnalyticsSelectWait";
				break;
			case "LevelSelect":
				UpdateScreenDisplay();
				gameState = "LevelSelectWait";
				break;
			case "AnimateGarden":
				UpdateScreenDisplay();
				AnimationBreakNumber = 0;
				//gardenLogicObj.AnimateFlag = true;
				gameState = "AnimateGardenWait";
				break;
            case "PlayGame":
                startTime = Time.time;
                UpdateScreenDisplay();

                // Ensure we start from the original base sizes each session
                DisplayGridTransform.sizeDelta = baseGridSize;
                DisplayLetterTransform.sizeDelta = baseLetterSize;

                // Pop size of Letter (always relative to the base)
                LeanTween.size(DisplayGridTransform, baseGridSize * 1.1f, 0.75f)
                    .setDelay(0.5f).setEaseInOutCirc().setRepeat(4).setLoopPingPong();

                LeanTween.size(DisplayLetterTransform, baseLetterSize * 1.1f, 0.75f)
                    .setDelay(0.5f).setEaseInOutCirc().setRepeat(4).setLoopPingPong();

                // VO Instructions
                audioPlayIntro = true;
                playVOAudio("touch_and_say_letter");
                gameState = "PlayerSelectLetter";
                break;
            case "PlayerSelectLetter":
				if (audioPlayIntro)
				{
					AudioSource audioIntro = gameObject.GetComponent<AudioSource>();
					if (!audioIntro.isPlaying && CorrectLetterCounter == 0)
					{
                        // Only play the letter VO if no Correct answers yet
						playLetterAudio(currentDisplayLetter);
						audioPlayIntro = false;
					}
				}
				break;
			case "AnimationBreak":
				UpdateScreenDisplay();
				StartAnimation(AnimationBreakNumber);
				gameState = "AnimationBreakWait";
				break;

		}

        if (!playlistActive) return;

        // If muted, we still want the playlist to continue logically? Usually no:
        // If you want it NOT to advance while muted, keep this guard:
        if (MusicSource.mute) return;

        // Track ended: AudioSource is not playing and has a clip assigned
        if (MusicSource.clip != null && !MusicSource.isPlaying)
        {
            PlayNextTrack();
        }

    }
}