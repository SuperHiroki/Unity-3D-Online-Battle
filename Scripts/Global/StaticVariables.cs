//このスクリプトはオブジェクトにアタッチしない。
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Linq;

//################################################################################################
//グローバル変数
public class RoomPlayerInfo
{
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //値を取得
    //##############################################
    //StartScene
    public static Dictionary<string, TMP_InputField> inputFields = new Dictionary<string, TMP_InputField>();
    public static Dictionary<string, TMP_Dropdown> dropdowns = new Dictionary<string, TMP_Dropdown>();
    public static Dictionary<string, (Image keyObj, TMP_Text keyText)> texts = new Dictionary<string, (Image, TMP_Text)>();
    public static Dictionary<string, Button> buttons = new Dictionary<string, Button>();
    //##############################################
    //MainScene
    public static Dictionary<string, (Image keyObj, TMP_Text keyText)> textsMainScene = new Dictionary<string, (Image, TMP_Text)>();
    public static Dictionary<string, (Image keyObj, TMP_Text keyText)> textsMainSceneIsDead = new Dictionary<string, (Image, TMP_Text)>();
    public static Dictionary<string, Button> buttonsMainScene = new Dictionary<string, Button>();
    public static Dictionary<string, Button> buttonsMainSceneIsDead = new Dictionary<string, Button>();
    public static Dictionary<string, Image> imagesMainScene = new Dictionary<string, Image>();
    //##############################################
    //CharaScene
    public static Dictionary<string, (Image keyObj, TMP_Text keyText)> textsCharaScene = new Dictionary<string, (Image, TMP_Text)>();
    public static Dictionary<string, Image> imagesCharaScene = new Dictionary<string, Image>();
    public static Dictionary<string, Button> buttonsCharaScene = new Dictionary<string, Button>();
    //##############################################
    //GetNewCharaScene
    public static Dictionary<string, Button> buttonsGetNewCharaScene = new Dictionary<string, Button>();
    public static Dictionary<string, Image> imagesGetNewCharaScene = new Dictionary<string, Image>();
    public static Dictionary<string, (Image keyObj, TMP_Text keyText)> textsGetNewCharaScene = new Dictionary<string, (Image, TMP_Text)>();
    //##############################################
    //AllScene
    public static Dictionary<string, (Image keyObj, Image crossObj, TMP_Text keyText)> textsAllScene = new Dictionary<string, (Image, Image, TMP_Text)>();
    //##############################################
    //FirstScene
    public static Dictionary<string, Button> buttonsFirstScene = new Dictionary<string, Button>();
    public static Dictionary<string, (Image keyObj, TMP_Text keyText)> textsFirstScene = new Dictionary<string, (Image, TMP_Text)>();
    public static Dictionary<string, Image> imagesFirstScene = new Dictionary<string, Image>();
    //一度だけ最初に呼び出される。
    static RoomPlayerInfo()
    {
        //########################################################################
        //########################################################################
        //########################################################################
        //StartScene
        foreach (var key in GlobalDefine.InputsDefineDict.Keys)
        {
            inputFields[key] = null;
        }
        foreach (var key in GlobalDefine.DropdownsDefineDict.Keys)
        {
            dropdowns[key] = null;
        }
        foreach (var key in GlobalDefine.TextsDefineDict.Keys)
        {
            texts[key] = (null, null);
        }
        foreach (var key in GlobalDefine.ButtonsDefineDict.Keys)
        {
            buttons[key] = null;
        }
        //##############################################
        //##############################################
        //MainScene
        foreach (var key in GlobalDefine.TextsDefineDictMainScene.Keys)
        {
            textsMainScene[key] = (null, null);
        }
        foreach (var key in GlobalDefine.TextsDefineDictMainSceneIsDead.Keys)
        {
            textsMainSceneIsDead[key] = (null, null);
        }
        foreach (var key in GlobalDefine.ButtonsDefineDictMainScene.Keys)
        {
            buttonsMainScene[key] = null;
        }
        foreach (var key in GlobalDefine.ButtonsDefineDictMainSceneIsDead.Keys)
        {
            buttonsMainSceneIsDead[key] = null;
        }
        foreach (var key in GlobalDefine.ImagesDefineDictMainScene.Keys)
        {
            imagesMainScene[key] = null;
        }
        //##############################################
        //##############################################
        //CharaScene
        foreach (var key in GlobalDefine.TextsDefineDictCharaScene.Keys)
        {
            textsCharaScene[key] = (null, null);
        }
        foreach (var key in GlobalDefine.ImagesDefineDictCharaScene.Keys)
        {
            imagesCharaScene[key] = null;
        }
        foreach (var key in GlobalDefine.ButtonsDefineDictCharaScene.Keys)
        {
            buttonsCharaScene[key] = null;
        }
        //##############################################
        //##############################################
        //GetNewCharaScene
        foreach (var key in GlobalDefine.ButtonsDefineDictGetNewCharaScene.Keys)
        {
            buttonsGetNewCharaScene[key] = null;
        }
        foreach (var key in GlobalDefine.ImagesDefineDictGetNewCharaScene.Keys)
        {
            imagesGetNewCharaScene[key] = null;
        }
        foreach (var key in GlobalDefine.TextsDefineDictGetNewCharaScene.Keys)
        {
            textsGetNewCharaScene[key] = (null, null);
        }
        //##############################################
        //##############################################
        //AllScene
        foreach (var key in GlobalDefine.TextsDefineDictAllScene.Keys)
        {
            textsAllScene[key] = (null, null, null);
        }
        //##############################################
        //##############################################
        //FristScene
        foreach (var key in GlobalDefine.ButtonsDefineDictFirstScene.Keys)
        {
            buttonsFirstScene[key] = null;
        }
        foreach (var key in GlobalDefine.TextsDefineDictFirstScene.Keys)
        {
            textsFirstScene[key] = (null, null);
        }
        foreach (var key in GlobalDefine.ImagesDefineDictFirstScene.Keys)
        {
            imagesFirstScene[key] = null;
        }
    }
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
}

//################################################################################################
//グローバル定数
public class GlobalDefine
{
    //スタート画面に戻るときにDontDestroyOnLoadの属性を持ったオブジェクトを破壊するための、名前のリスト
    public static readonly string[] ObjectListDontDestroy = new string[] { "EmptyRespawnPrefab", "EmptyNetworkManager" };
    //API
    public const string BaseUrl = "http://myapp6.shiroatohiro.com/";
    //キャラ一覧
    public static readonly string[] CharaNamesList = new string[] { "Robot", "AAA", "BBB", "CCC", "DDD", "EEE"};
    //レベルアップまでの経験値
    public static readonly int[] UserLevelUpExperienceList = new int[] { 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 300, 600, 1000, 5000, 7000, 10000, 11000, 13000, 15000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000, 20000 };
    public static readonly int UserMaxLevel = 20;
    public static readonly int UserMaxMagicStone = 9999;
    public static readonly int[] CharaLevelUpExperienceList = new int[] { 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 500, 1000, 1200, 1500, 1800, 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000, 3100, 3200, 3300, 3400, 3500, 3600, 3700, 3800, 3900, 4000, 4100, 4200, 4300, 4400, 4500, 4600, 4700, 4800, 4900, 5000, 5100, 5200, 5300, 5400, 5500, 5600, 5600, 5600, 5600, 5600, 5600, 5600, 5600, 5600, 5600, 5600, 5600, 5600, 5600, 5600 };
    public static readonly int CharaMaxLevel = 30;
    public static readonly int CharaMaxAwakening = 10;
    public static readonly int CharaMaxReliability = 999;
    //ガチャに必要なMagicStoneの個数
    public static readonly int MagicStoneGetNewChara = 3;
    //#############################################################################
    //#############################################################################
    //StartScene
    //Input情報
    public static readonly Dictionary<string, InputFieldDefine> InputsDefineDict = new Dictionary<string, InputFieldDefine>
    {
        { "EnterRoomId" , new InputFieldDefine("Room ID", new Vector2(-305, 112), new Vector2(164, 29)) },
        { "Username" , new InputFieldDefine("username", new Vector2(315, 185), new Vector2(164, 29)) },
        { "Password" , new InputFieldDefine("password", new Vector2(315, 150), new Vector2(164, 29)) }
    };
    //Dropdown情報
    public static readonly Dictionary<string, DropdownDefine> DropdownsDefineDict = new Dictionary<string, DropdownDefine>
    {
        { "Character" , new DropdownDefine(new string[] { "No Need", "No Need" }, new Vector2(-305, 75)) },
        { "CharacterLevel" , new DropdownDefine(new string[] { "No Need", "No Need", "No Need" }, new Vector2(-305, 40)) },
        { "LifeStock" , new DropdownDefine(new string[] { "1 LifeStock", "2 LifeStock", "3 LifeStock", "4 LifeStock", "5 LifeStock" }, new Vector2(-305, 5)) }
    };
    //Button情報(nullの要素は使わないね)
    public static readonly Dictionary<string, ButtonDefine> ButtonsDefineDict = new Dictionary<string, ButtonDefine>
    {
        { "MakeRoom" , new ButtonDefine("Make a Room", new Vector2(-315, 205), 13, 4.4f, new Vector2(110f, 30f), "Photo/button_background")},
        { "EnterRoom" , new ButtonDefine("Enter a Room", new Vector2(-315, 145), 13, 4.4f, new Vector2(110f, 30f), "Photo/button_background") },
        { "MakeRoomAsServer" , new ButtonDefine("Make a Room as server",  new Vector2(-315, 175), 7, 4.4f, new Vector2(110f, 30f), "Photo/button_background") },
        { "Login" , new ButtonDefine("Login",new Vector2(315, 115), 13, 2.0f, new Vector2(110f, 30f), "Photo/button_background") },
        { "Logout" , new ButtonDefine("Logout", new Vector2(315, 81), 13, 2.0f, new Vector2(110f, 30f), "Photo/button_background") },
        { "Signup" , new ButtonDefine("Signup", new Vector2(315, 47), 13, 2.0f, new Vector2(110f, 30f), "Photo/button_background") },
        { "ShowRooms" , new ButtonDefine("Show Rooms", new Vector2(-315, -30), 13, 2.0f, new Vector2(110f, 30f), "Photo/button_background") },
        { "Save" , new ButtonDefine("Save", new Vector2(315, -165), 13, 2.0f, new Vector2(110f, 30f), "Photo/button_background") },
        { "MoveToGetNewChara" , new ButtonDefine("Move to Get New Character", new Vector2(315, -115), 13, 2.0f, new Vector2(154f, 42f), "Photo/button_background") }
    };
    //Text情報
    public static readonly Dictionary<string, TextDefine> TextsDefineDict = new Dictionary<string, TextDefine>
    {
        { "UserName" , new TextDefine("", Color.black, new Vector2(-65, 202), 25.0f, new Vector2(300, 35), Color.red, 1f) },
        { "UserLevel" , new TextDefine("", Color.black, new Vector2(-65, 152), 25.0f, new Vector2(300, 35), Color.red, 1f) },
        { "Experience" , new TextDefine("Experience...", Color.black, new Vector2(115, 177), 17.0f, new Vector2(200, 32), Color.red, 1f) },
        { "MagicStone" , new TextDefine("MagicStone...", Color.black, new Vector2(115, 138), 17.0f, new Vector2(200, 32), Color.red, 1f) }
    };
    //#############################################################################
    //#############################################################################
    //MainScene
    //Button情報(nullの要素は使わないね)
    public static readonly Dictionary<string, ButtonDefine> ButtonsDefineDictMainScene = new Dictionary<string, ButtonDefine>
    {
        { "BackToHome" , new ButtonDefine("Back To Home", new Vector2(-315, 205), 13, 0.0f, new Vector2(110f, 30f), "Photo/button_background")},
        { "Dash" , new ButtonDefine("Dash", new Vector2(-355, -185), 13, 0.0f, new Vector2(60f, 60f), "Photo/button_background")},
        { "Jump" , new ButtonDefine("Jump", new Vector2(-290, -185), 13, 0.0f, new Vector2(60f, 60f), "Photo/button_background")},
        { "Attack1" , new ButtonDefine("Attack1", new Vector2(-355, -120), 13, 0.0f, new Vector2(60f, 60f), "Photo/button_background")},
        { "Attack2" , new ButtonDefine("Attack2", new Vector2(-355, -55), 13, 0.0f, new Vector2(60f, 60f), "Photo/button_background")},
        { "Attack3" , new ButtonDefine("Attack3", new Vector2(-355, 5), 13, 0.0f, new Vector2(60f, 60f), "Photo/button_background")},
        { "Attack4" , new ButtonDefine("Attack4", new Vector2(-355, 70), 13, 0.0f, new Vector2(60f, 60f), "Photo/button_background")}
    };
    //Button情報(nullの要素は使わないね)。死んだとき
    public static readonly Dictionary<string, ButtonDefine> ButtonsDefineDictMainSceneIsDead = new Dictionary<string, ButtonDefine>
    {
        { "GoBackToHome" , new ButtonDefine("Go Back To Home", new Vector2(0, 10), 13, 3.0f, new Vector2(110f, 30f), "Photo/button_background")},
        { "Watch" , new ButtonDefine("Watch", new Vector2(0, -40), 13, 3.0f, new Vector2(110f, 30f), "Photo/button_background")}
    };
    //Text情報
    public static readonly Dictionary<string, TextDefine> TextsDefineDictMainScene = new Dictionary<string, TextDefine>
    {
        { "RoomId" , new TextDefine("", Color.black, new Vector2(-75, 200), 25.0f, new Vector2(300, 35), Color.red, 1f) },
        { "YourRole" , new TextDefine("", Color.black, new Vector2(233, 200), 25.0f, new Vector2(300, 35), Color.red, 1f) }
    };
    //Text情報。死んだとき
    public static readonly Dictionary<string, TextDefine> TextsDefineDictMainSceneIsDead = new Dictionary<string, TextDefine>
    {
        { "YouAreDead" , new TextDefine("", Color.red, new Vector2(0, 100), 45.0f, new Vector2(500, 65), Color.red, 1f) }
    };
    //Image情報
    public static readonly Dictionary<string, ImageDefine> ImagesDefineDictMainScene = new Dictionary<string, ImageDefine>
    {
        { "DragBackground" , new ImageDefine(Color.white, new Vector2(310, -156), new Vector2(120, 120), "Photo/dragging_background", 1f) },
        { "DragArrow" , new ImageDefine(Color.white, new Vector2(-1200, -1200), new Vector2(192, 108), "Photo/arrow_background", 1f) }
    };
    //#############################################################################
    //#############################################################################
    //CharaScene
    //Text情報
    public static readonly Dictionary<string, TextDefine> TextsDefineDictCharaScene = new Dictionary<string, TextDefine>
    {
        { "CharaName" , new TextDefine("", Color.black, new Vector2(-10, 202), 25.0f, new Vector2(300, 35), Color.red, 1f) },
        { "CharaLevel" , new TextDefine("", Color.black, new Vector2(-10, 152), 25.0f, new Vector2(300, 35), Color.red, 1f) },
        { "CharaAwakening" , new TextDefine("", Color.black, new Vector2(-310, 122), 18.0f, new Vector2(240, 28), Color.red, 1f) },
        { "CharaReliability" , new TextDefine("", Color.black, new Vector2(-310, 82), 18.0f, new Vector2(240, 28), Color.red, 1f) },
        { "CharaExperience" , new TextDefine("", Color.black, new Vector2(-310, 42), 18.0f, new Vector2(240, 28), Color.red, 1f) },
    };
    //Image情報
    public static readonly Dictionary<string, ImageDefine> ImagesDefineDictCharaScene = new Dictionary<string, ImageDefine>
    {
        { "Chara" , new ImageDefine(Color.white, new Vector2(0, 0), new Vector2(800/2, 450/2), "searchEachChara", 1f) }
    };
    //Button情報(nullの要素は使わないね)
    public static readonly Dictionary<string, ButtonDefine> ButtonsDefineDictCharaScene = new Dictionary<string, ButtonDefine>
    {
        { "BackToHome" , new ButtonDefine("Back To Home", new Vector2(-315, 205), 13, 3.0f, new Vector2(110f, 30f), "Photo/button_background")},
    };
    //#############################################################################
    //#############################################################################
    //GetNewCharaScene
    //Button情報(nullの要素は使わないね)
    public static readonly Dictionary<string, ButtonDefine> ButtonsDefineDictGetNewCharaScene = new Dictionary<string, ButtonDefine>
    {
        { "BackToHome" , new ButtonDefine("Back To Home", new Vector2(-315, 205), 13, 3.0f, new Vector2(110f, 30f), "Photo/button_background")},
        { "GetNewChara" , new ButtonDefine("Get New Character", new Vector2(-315, -100), 13, 15.0f, new Vector2(165, 45f), "Photo/button_background")}
    };
    //Image情報
    public static readonly Dictionary<string, ImageDefine> ImagesDefineDictGetNewCharaScene = new Dictionary<string, ImageDefine>
    {
        { "Chara" , new ImageDefine(Color.white, new Vector2(0, 0), new Vector2(800/2, 450/2), "searchEachChara", 1f) }
    };
    //Text情報
    public static readonly Dictionary<string, TextDefine> TextsDefineDictGetNewCharaScene = new Dictionary<string, TextDefine>
    {
        { "CharaName" , new TextDefine("", Color.black, new Vector2(-10, 202), 25.0f, new Vector2(300, 35), Color.red, 1f) },
        { "CharaLevel" , new TextDefine("", Color.black, new Vector2(-10, 152), 25.0f, new Vector2(300, 35), Color.red, 1f) },
        { "CharaAwakening" , new TextDefine("", Color.black, new Vector2(-310, 122), 18.0f, new Vector2(240, 28), Color.red, 1f) },
        { "CharaReliability" , new TextDefine("", Color.black, new Vector2(-310, 82), 18.0f, new Vector2(240, 28), Color.red, 1f) },
        { "CharaExperience" , new TextDefine("", Color.black, new Vector2(-310, 42), 18.0f, new Vector2(240, 28), Color.red, 1f) },
    };
    //#############################################################################
    //#############################################################################
    //AllScene
    //Text情報
    public static readonly Dictionary<string, TextDefineAllScene> TextsDefineDictAllScene = new Dictionary<string, TextDefineAllScene>
    {
        { "Alert" , new TextDefineAllScene("", Color.red, 25.0f, new Vector2(300, 35), 2.0f, new Vector2(25, 25), new Vector2(0, 350), new Vector2(0, 100), 0.34f) },
        { "SomeText" , new TextDefineAllScene("", Color.red, 25.0f, new Vector2(600, 300), 20.0f, new Vector2(60, 60), new Vector2(0, 520), new Vector2(0, 0), 0.34f) }
    };
    //#############################################################################
    //#############################################################################
    //FirstScene
    //Button情報(nullの要素は使わないね)
    public static readonly Dictionary<string, ButtonDefine> ButtonsDefineDictFirstScene = new Dictionary<string, ButtonDefine>
    {
        { "TapToStart" , new ButtonDefine("Press to Start", new Vector2(0, 80), 13, 3.0f, new Vector2(145f, 40f), "Photo/button_background")}
    };
    //Text情報
    public static readonly Dictionary<string, TextDefine> TextsDefineDictFirstScene = new Dictionary<string, TextDefine>
    {
        { "GameTitle" , new TextDefine("3D Online Battle", Color.green, new Vector2(0, 162), 44.0f, new Vector2(455, 66), Color.red, 0.6f) },
        { "GameExplanation" , new TextDefine("You can play multiplayer battle.\nThe maximum number of players is 4.\nYou can use buttons or keyboards below.\nMove: Arrow Keys / Jump: J / Dash: K / Attacks: M, N, B, V, C", 
            Color.black, new Vector2(0, -80), 24.0f, new Vector2(700, 230), Color.red, 0.6f) }
    };
    //Image情報
    public static readonly Dictionary<string, ImageDefine> ImagesDefineDictFirstScene = new Dictionary<string, ImageDefine>
    {
        { "TitleImage" , new ImageDefine(Color.white, new Vector2(0, 0), new Vector2(800*1.0f, 450*1.0f), "Photo/TitleImage", 1f) }
    };
}

//##############################################################################################################################
//##############################################################################################################################
//##############################################################################################################################
//##############################################################################################################################
//グローバルクラス
//UIで使う
[System.Serializable]
public class InputFieldDefine
{
    public string placeholder;
    public Vector2 position;
    public Vector2 sizeDelta;

    public InputFieldDefine(string placeholder, Vector2 position, Vector2 sizeDelta)
    {
        this.placeholder = placeholder;
        this.position = position;
        this.sizeDelta = sizeDelta;
    }
}

[System.Serializable]
public class DropdownDefine
{
    public string[] options;
    public Vector2 position;

    public DropdownDefine(string[] options, Vector2 position)
    {
        this.options = options;
        this.position = position;
    }
}

[System.Serializable]
public class ButtonDefine
{
    public string label;
    public Vector2 position;
    public int textSize;
    public float disableTime;
    public Vector2 size;
    public string photoPath;

    public ButtonDefine(string label, Vector2 position, int textSize, float disableTime, Vector2 size, string photoPath)
    {
        this.label = label;
        this.position = position;
        this.textSize = textSize;
        this.disableTime = disableTime;
        this.size = size;
        this.photoPath = photoPath;
    }
}

[System.Serializable]
public class TextDefine
{
    public string text;
    public Color color;
    public Vector2 position;
    public float fontSize;
    public Vector2 sizeDelta;
    public Color backgroundColor;
    public float alpha;

    public TextDefine(string text, Color color, Vector2 position, float fontSize, Vector2 sizeDelta, Color backgroundColor, float alpha)
    {
        this.text = text;
        this.color = color;
        this.position = position;
        this.fontSize = fontSize; 
        this.sizeDelta = sizeDelta;
        this.backgroundColor = backgroundColor;
        this.alpha = alpha;
    }
}

[System.Serializable]
public class TextDefineAllScene
{
    public string text;
    public Color color;
    public float fontSize;
    public Vector2 sizeDelta;
    public float stayTime;
    public Vector2 crossImageSizeDelta;
    public Vector2 startPos;
    public Vector2 endPos;
    public float moveTime;

    public TextDefineAllScene(string text, Color color, float fontSize, Vector2 sizeDelta, float stayTime, Vector2 crossImageSizeDelta, Vector2 startPos, Vector2 endPos, float moveTime)
    {
        this.text = text;
        this.color = color;
        this.fontSize = fontSize;
        this.sizeDelta = sizeDelta;
        this.stayTime = stayTime;
        this.crossImageSizeDelta = crossImageSizeDelta;
        this.startPos = startPos;
        this.endPos = endPos;
        this.moveTime = moveTime;
    }
}

[System.Serializable]
public class ImageDefine
{
    public Color color;
    public Vector2 position;
    public Vector2 sizeDelta;
    public string photoPath;
    public float alpha;

    public ImageDefine(Color color, Vector2 position, Vector2 sizeDelta, string photoPath, float alpha)
    {
        this.color = color;
        this.position = position;
        this.sizeDelta = sizeDelta;
        this.photoPath = photoPath;
        this.alpha = alpha;
    }
}

//##############################################################################################################################
//##############################################################################################################################
//##############################################################################################################################
//##############################################################################################################################
//グローバルメソッド
//APIからのデータが配列で来た場合。
public class MyJsonHelper
{
    public static T[] MyFromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        MyWrapper<T> myWrapper = JsonUtility.FromJson<MyWrapper<T>>(newJson);
        return myWrapper.array;
    }

    [System.Serializable]
    private class MyWrapper<T>
    {
        public T[] array;
    }
}