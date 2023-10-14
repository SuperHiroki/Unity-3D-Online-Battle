using System.Diagnostics;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;

public class BulletController : NetworkBehaviour
{
    //プレハブの参照
    public NetworkObject sparksVfxPrefab;

    //グローバル変数
    private int lifeStock = UserDataManager.LifeStock;
    private List<CharacterNetworkObjectInfo> characterNetworkObjectsList => GameManager.CharacterNetworkObjectsList;
    private List<AttackNetworkObjectInfo> attackNetworkObjectsList => GameManager.AttackNetworkObjectsList;

    //外部クラス
    private NetworkObjectStatusShow networkObjectStatusShow;

    private void Start()
    {
        if (IsServer)
        {
            //ステータス(キル数など)を表示させるために
            networkObjectStatusShow = NetworkObjectStatusShow.MyInstance;
            //時間たったら破壊
            StartCoroutine(DestroyThisObjectAfterSeconds(lifeTime));
        }
    }

    //########################################################################################################################################################
    //########################################################################################################################################################
    //########################################################################################################################################################
    //########################################################################################################################################################
    private float speed = 10f;

    //真っ直ぐ飛んでいく
    public void Launch(Vector3 direction, AttackNetworkObjectInfo attackNetworkObjectInfo)
    {
        StartCoroutine(MoveStraight(direction));
    }

    IEnumerator MoveStraight(Vector3 direction)
    {
        while (true)
        {
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
    }
    //###################################################################################################
    private float rotateSpeed = 30f; // degrees per second, adjust as needed

    // Right Curve
    public void Launch2(Vector3 direction, AttackNetworkObjectInfo attackNetworkObjectInfo)
    {
        StartCoroutine(MoveToTheRight(direction));
    }

    IEnumerator MoveToTheRight(Vector3 direction)
    {
        while (true)
        {
            direction = Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0) * direction;
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
    }
    //###################################################################################################
    // Left Curve
    public void Launch3(Vector3 direction, AttackNetworkObjectInfo attackNetworkObjectInfo)
    {
        StartCoroutine(MoveToTheLeft(direction));
    }

    IEnumerator MoveToTheLeft(Vector3 direction)
    {
        while (true)
        {
            direction = Quaternion.Euler(0, -rotateSpeed * Time.deltaTime, 0) * direction;
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
    }
    //###################################################################################################
    // With Smooth Noise (Horizontal Only)
    public void Launch4(Vector3 direction, AttackNetworkObjectInfo attackNetworkObjectInfo)
    {
        StartCoroutine(MoveWithSmoothHorizontalNoise(direction));
    }

    IEnumerator MoveWithSmoothHorizontalNoise(Vector3 direction)
    {
        float noiseStrength = 2.0f;  // Adjust the strength of the noise
        float noiseScale = 1.0f; // Adjust the scale of the noise oscillation over time

        while (true)
        {
            float noiseX = Mathf.PerlinNoise(Time.time * noiseScale, 0f) * 2f - 1f;
            float noiseZ = Mathf.PerlinNoise(0f, Time.time * noiseScale) * 2f - 1f;

            Vector3 noise = new Vector3(noiseX, 0f, noiseZ) * noiseStrength;
            transform.position += (direction + noise) * speed * Time.deltaTime;
            yield return null;
        }
    }
    //###################################################################################################
    
    private float noiseAmount = 8f;
    private float noiseChangeSpeed = 0.4f;
    private Vector3 currentNoise = Vector3.zero;
    private float speed2 = 15f;

    // 真っ直ぐだけどノイズありで飛んでいく
    public void LaunchNoise(Vector3 direction, AttackNetworkObjectInfo attackNetworkObjectInfo)
    {
        StartCoroutine(GrowAndMoveCoroutine(direction, attackNetworkObjectInfo));
    }

    IEnumerator GrowAndMoveCoroutine(Vector3 direction, AttackNetworkObjectInfo attackNetworkObjectInfo)
    {
        float growDuration = 3f;
        float elapsed = 0f;
        Vector3 firstPosition = transform.position;
        Vector3 targetedPosition = firstPosition + transform.up * attackNetworkObjectInfo.ShooterCharacterLevel * 0.2f;
        Vector3 firstScale = Vector3.zero;
        Vector3 targetedScale = Vector3.one * (1 + attackNetworkObjectInfo.ShooterCharacterLevel * 0.1f);
        while (elapsed < growDuration)
        {
            transform.position = Vector3.Lerp(firstPosition, targetedPosition, elapsed / growDuration);
            transform.localScale = Vector3.Lerp(firstScale, targetedScale, elapsed / growDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetedPosition;
        transform.localScale = targetedScale;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MoveStraightNoise(direction));
    }

    IEnumerator MoveStraightNoise(Vector3 direction)
    {
        while (true)
        {
            Vector3 targetNoise = new Vector3(UnityEngine.Random.Range(-noiseAmount, noiseAmount), 0, UnityEngine.Random.Range(-noiseAmount, noiseAmount));
            currentNoise = Vector3.Lerp(currentNoise, targetNoise, noiseChangeSpeed * Time.deltaTime);
            Vector3 noisyDirection = direction + currentNoise;
            transform.position += noisyDirection.normalized * speed2 * Time.deltaTime;
            yield return null;
        }
    }
    

    //###################################################################################################


    //########################################################################################################################################################
    //########################################################################################################################################################
    //########################################################################################################################################################
    //########################################################################################################################################################
    private float lifeTime = 10f;
    private Dictionary<int, float> attackIdDamageDict = new Dictionary<int, float> { { 0, 5 }, { 1, 10 }, { 2, 15 }, { 3, 20 }, { 4, 25 }, { 5, 30 } };

    //指定の秒数経ったら破壊する
    IEnumerator DestroyThisObjectAfterSeconds(float seconds)
    {
        if (IsServer)
        {
            //指定の秒数待つ
            yield return new WaitForSeconds(seconds);
            //破壊する
            AttackNetworkObjectInfo bulletInfo = attackNetworkObjectsList.Find(info => info.AttackNetworkObjectID == this.GetComponent<NetworkObject>().NetworkObjectId);
            DestroyAndDeleteFromList(bulletInfo);
        }
    }

    //他のオブジェクトに衝突したら破壊する
    void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            AttackNetworkObjectInfo bulletInfo = attackNetworkObjectsList.Find(info => info.AttackNetworkObjectID == this.GetComponent<NetworkObject>().NetworkObjectId);
            if (collision.gameObject.CompareTag("Character"))
            {
                //Collisionとなったキャラの情報（ChracterNetworkObjectInfo）を取得
                ulong hitCharacterID = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
                CharacterNetworkObjectInfo hitCharacterInfo = characterNetworkObjectsList.Find(info => info.CharacterNetworkObjectID == hitCharacterID);
                hitCharacterInfo.CharacterHP -= Convert.ToInt32(Math.Round(attackIdDamageDict[bulletInfo.AttackID] * (1.0f + bulletInfo.ShooterCharacterLevel * 0.2f)));
                if (hitCharacterInfo.CharacterHP <= 0)
                {
                    //弾丸を撃たれたキャラのHPを復帰して、被キルカウントを増加させる
                    hitCharacterInfo.CharacterKilledCount++;
                    hitCharacterInfo.CharacterHP = 100;
                    //弾丸を撃ったキャラの情報（NetworkObjectInfo）を取得して、キルカウントを増加させる
                    CharacterNetworkObjectInfo shooterInfo = characterNetworkObjectsList.Find(info => info.CharacterNetworkObjectID == bulletInfo.ShooterCharacterNetworkObjectID);
                    if (shooterInfo != null && shooterInfo.CharacterNetworkObjectID != hitCharacterInfo.CharacterNetworkObjectID)
                    {
                        shooterInfo.CharacterKillCount++;
                        GameManager.UpdateCharacterNetworkObjectsList(shooterInfo);
                        networkObjectStatusShow.ChangeCharacterStatus(shooterInfo);
                    }
                }
                //攻撃を受けたら通知する
                GameManager.UpdateCharacterNetworkObjectsList(hitCharacterInfo);
                networkObjectStatusShow.ChangeCharacterStatus(hitCharacterInfo);
                //lifeStockの回数キルされたら通知
                if (hitCharacterInfo.CharacterKilledCount == lifeStock)
                {
                    networkObjectStatusShow.KillCountEqualsLifeStock(hitCharacterInfo);
                }

            }
            DestroyAndDeleteFromList(bulletInfo);
        }
    }
    
    //破壊する関数
    private void DestroyAndDeleteFromList(AttackNetworkObjectInfo bulletInfo)
    {
        if (IsServer)
        {
            //リストから削除する（ネットワークオブジェクトである弾丸が破壊される前に、リストから削除することで、同じAttackNetworkObjectIDを持つ要素がリストに存在することを防ぐ）
            GameManager.RemoveAttackNetworkObject(bulletInfo);
            //破壊の時に生じるエフェクト
            NetworkObject netObj = gameObject.GetComponent<NetworkObject>();
            NetworkObject spawnedVfx = Instantiate(sparksVfxPrefab, netObj.transform.position, Quaternion.identity).GetComponent<NetworkObject>();
            spawnedVfx.Spawn();
            //自身を破壊する
            foreach (Transform child in netObj.transform)
            {
                NetworkObject childNetObj = child.GetComponent<NetworkObject>();
                childNetObj.Despawn(true);
            }
            netObj.Despawn(true);
        }
    }
    //################################################################################################################################
    //################################################################################################################################
}
