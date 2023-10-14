using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class ButtonEventSetGetNewCharaScene : MonoBehaviour
{
    //�O���[�o���ϐ�
    private bool isLogin => UserDataManager.IsLogin;
    private CharacterData[] charactersArray => UserDataManager.UserData.Characters;
    private UserData userData => UserDataManager.UserData;

    //�O���N���X�Q��
    [SerializeField] private ImageManagerGetNewCharaCharaScene imageManagerGetNewCharaCharaScene;
    [SerializeField] private TextManagerGetNewCharaScene textManagerGetNewCharaScene;

    private void Start()
    {
        SetButtonDisable();
        SetButtonActions();
    }
    //###################################################################################################################
    //###################################################################################################################
    //###################################################################################################################
    //###################################################################################################################
    //�{�^���̖������ƗL����
    private void SetButtonDisable()
    {
        foreach (var pair in RoomPlayerInfo.buttonsGetNewCharaScene)
        {
            var key = pair.Key;
            var button = pair.Value;
            Button currentButton = button;
            button.onClick.AddListener(() =>
            {
                DisableButton(currentButton);
                StartCoroutine(EnableButtonWithDelay(currentButton, GlobalDefine.ButtonsDefineDictGetNewCharaScene[key].disableTime));
            });
        }
    }

    //�{�^�����ĂїL����
    private IEnumerator EnableButtonWithDelay(Button btn, float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableButton(btn);
    }

    private void EnableButton(Button btn)
    {
        btn.interactable = true;
    }

    //�{�^���𖳌���
    private void DisableButton(Button btn)
    {
        btn.interactable = false;
    }

    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //�{�^���ɃA�N�V������ݒ�
    private void SetButtonActions()
    {
        //�z�[����ʂɖ߂�
        RoomPlayerInfo.buttonsGetNewCharaScene["BackToHome"].onClick.AddListener(() =>
        {
            ShutdownAndLoad();
        });

        //�K�`��������
        RoomPlayerInfo.buttonsGetNewCharaScene["GetNewChara"].onClick.AddListener(() =>
        {
            if (!isLogin)
            {
                TextManagerAllScene.MakeAlertText("You must Login");
                return;
            }
            //MagicStone�̌����m�F
            if (userData.MagicStone < 3)
            {
                TextManagerAllScene.MakeAlertText($"{GlobalDefine.MagicStoneGetNewChara} Magic Stone Needed.");
                return;
            }
            int randomIndex = UnityEngine.Random.Range(0, GlobalDefine.CharaNamesList.Length);
            GetCharacter(randomIndex);
            imageManagerGetNewCharaCharaScene.ShowCharaImage(randomIndex);
            textManagerGetNewCharaScene.ShowCharaStatus(randomIndex);
            //���Z�}����h�����߂ɂ�����API�ɂƂ΂�
            SaveUserInformation.Instance.SaveUserData();
        });
    }

    //#####################################################################################
    //#####################################################################################
    //�z�[����ʂɖ߂�֐�
    private void ShutdownAndLoad()
    {
        /*
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            Destroy(obj);
        }
        */
        SceneManager.LoadScene("StartScene");
    }

    //#####################################################################################
    //#####################################################################################
    //�K�`���̎��ɃL������ǉ�����
    private void GetCharacter(int characterId)
    {
        var tempUserData = userData; // ������ŏI�I�Ɋۂ��ƃR�s�[����
        //MagicStone�������
        tempUserData.MagicStone -= 3;
        //�L������ǉ��������͊o��
        CharacterData charaData = Array.Find(tempUserData.Characters, c => c.CharacterId == characterId);
        if (charaData != null)
        {
            if(charaData.Awakening == GlobalDefine.CharaMaxAwakening)
            {
                TextManagerAllScene.MakeAlertText("Already Max Awakening");
            }
            else
            {
                charaData.Awakening += 1;
            }
        }
        else
        {
            CharacterData newCharacterData = new CharacterData
            {
                CharacterId = characterId,
                CharacterLevel = 1,
                Awakening = 0,
                Reliability = 0,
                Experience = 0
            };
            List<CharacterData> characterDataList = new List<CharacterData>(tempUserData.Characters);
            characterDataList.Add(newCharacterData);
            tempUserData.Characters = characterDataList.ToArray();
        }
        UserDataManager.UpdateAllUserData(tempUserData);
    }
    //#####################################################################################
    //#####################################################################################
}