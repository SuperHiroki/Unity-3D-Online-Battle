using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class DropdownManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdownPrefab;
    [SerializeField] private Transform parentTransform;

    private CharacterData[] characterDataArrayForShow => UserDataManager.UserData.Characters;
    private int lifeStock => UserDataManager.LifeStock;

    private Dictionary<int, int> dropdownIndexCharacterId = new Dictionary<int, int>();

    private void Start()
    {
        foreach (var key in GlobalDefine.DropdownsDefineDict.Keys)
        {
            //それぞれDropdownで画像が異なるかもしれないのでForループの中に入れる
            Sprite inputBackgroundSprite = Resources.Load<Sprite>("Photo/dropdown_background");
            //inputを生成して基本設定を行う
            TMP_Dropdown instanceDropdown = Instantiate(dropdownPrefab, parentTransform);
            //RectTransformの設定
            RectTransform dropdownRectTransform = instanceDropdown.GetComponent<RectTransform>();
            dropdownRectTransform.anchoredPosition = GlobalDefine.DropdownsDefineDict[key].position;
            //optionの設定
            instanceDropdown.ClearOptions();
            instanceDropdown.AddOptions(new List<string>(GlobalDefine.DropdownsDefineDict[key].options));
            //shadow
            Shadow thisShadow = instanceDropdown.gameObject.AddComponent<Shadow>();
            thisShadow.effectColor = new Color(0, 0, 0, 0.7f);
            thisShadow.effectDistance = new Vector2(-3.5f, -3.5f);
            // Dropdownの背景にSpriteを設定
            Image bgImage = instanceDropdown.GetComponent<Image>();
            bgImage.sprite = inputBackgroundSprite;
            //グローバル変数に格納。
            RoomPlayerInfo.dropdowns[key] = instanceDropdown;
        }

        //################################################################################################################
        //################################################################################################################
        //#######################################################################################
        //RoomPlayerInfo.dropdowns["Character"]
        //キャラの選択肢をDropdownsに入れる
        HavingCharacterNamesFromIdArrayToDropdown();
        //選択が変更したら発火する関数を設定
        TMP_Dropdown characterDropdown = RoomPlayerInfo.dropdowns["Character"];
        characterDropdown.onValueChanged.AddListener((selectedIndex) => {
            UserDataManager.SetCharacterId(dropdownIndexCharacterId[selectedIndex]);
            SelectedCharaAllowedLevelListToDropdown();
        });
        //#######################################################################################
        //RoomPlayerInfo.dropdowns["CharacterLevel"]
        //キャラレベルの選択肢をDropdownsに入れる
        SelectedCharaAllowedLevelListToDropdown();
        //選択が変更したら発火する関数を設定
        TMP_Dropdown characterLevelDropdown = RoomPlayerInfo.dropdowns["CharacterLevel"];
        characterLevelDropdown.onValueChanged.AddListener((selectedIndex) => {
            UserDataManager.SetCharacterLevel(selectedIndex + 1);
        });
        //#######################################################################################
        //RoomPlayerInfo.dropdowns["LifeStock"]
        //LifeStockの選択肢をDropdownsに入れる
        RoomPlayerInfo.dropdowns["LifeStock"].value = 1;
        //選択が変更したら発火する関数を設定
        TMP_Dropdown lifeStockDropdown = RoomPlayerInfo.dropdowns["LifeStock"];
        lifeStockDropdown.onValueChanged.AddListener((selectedIndex) => {
            UserDataManager.SetLifeStock(selectedIndex + 1);
        });
        //################################################################################################################
        //################################################################################################################
        //最初にグローバル変数に格納する。
        UserDataManager.SetCharacterId(RoomPlayerInfo.dropdowns["Character"].value);
        UserDataManager.SetCharacterLevel(RoomPlayerInfo.dropdowns["CharacterLevel"].value + 1);
        UserDataManager.SetLifeStock(RoomPlayerInfo.dropdowns["LifeStock"].value + 1);
    }

    //#######################################################################################
    //RoomPlayerInfo.dropdowns["Character"]
    //キャラの選択肢をDropdownsに入れる
    public void HavingCharacterNamesFromIdArrayToDropdown()
    {
        List<string> HavingCharaNameList = new List<string>();
        int j = 0;
        foreach (CharacterData charaData in characterDataArrayForShow)
        {
            HavingCharaNameList.Add(GlobalDefine.CharaNamesList[charaData.CharacterId]);
            dropdownIndexCharacterId[j] = charaData.CharacterId;
            j++;
        }
        RoomPlayerInfo.dropdowns["Character"].ClearOptions();
        RoomPlayerInfo.dropdowns["Character"].AddOptions(HavingCharaNameList);
    }

    //#######################################################################################
    //RoomPlayerInfo.dropdowns["CharacterLevel"]
    //キャラレベルの選択肢をDropdownsに入れる
    public void SelectedCharaAllowedLevelListToDropdown()
    {
        List<string> SelectedCharaAllowedLevelList = new List<string>();
        int maxLevel = characterDataArrayForShow[RoomPlayerInfo.dropdowns["Character"].value].CharacterLevel;
        for (int i = 0; i < maxLevel; i++)
        {
            SelectedCharaAllowedLevelList.Add((i + 1) + " Level");
        }
        RoomPlayerInfo.dropdowns["CharacterLevel"].ClearOptions();
        RoomPlayerInfo.dropdowns["CharacterLevel"].AddOptions(SelectedCharaAllowedLevelList);
        RoomPlayerInfo.dropdowns["CharacterLevel"].value = maxLevel - 1;
    }
}

