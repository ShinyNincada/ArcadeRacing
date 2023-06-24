using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lean.Pool;
using DG.Tweening;
public class GoldManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public TMP_Text goldText;
    [SerializeField] GameObject animationGoldPrefabs;
    [SerializeField] Transform target;

    [Space]
    [Header("Available gold: (golds to pool)")]
    [SerializeField] int maxGold;
    Queue<GameObject> goldsQueue = new Queue<GameObject> ();

    [Space]
    [Header("Animation Settings")]
    [SerializeField] [Range(1.5f, 2f)] float minAnimDuration;
    [SerializeField] [Range(2.5f, 4f)] float maxAnimDuration;

    [SerializeField] Ease easeType;
    Vector3 targetPosition;
    int _g;
    
    //Update gias
    public int Golds{
        get {return _g; }
        set{
            _g = value;
            goldText.text = Golds.ToString();
        }
    }

    void Awake(){
        targetPosition = target.position;
    }
    void Start(){
        Golds = SaveManager.instance.gold;
        goldText.text = Golds.ToString();
    }

    void Update(){
        goldText.text = Golds.ToString();
    }

    public void AddGold(int value){
        Golds += value;
        UpdateText();
    }

    public void SpendGold(int value){
        if(Golds >= value){
            Golds -= value;
            UpdateText();
        }
    }

    public void GoldCollected(Transform collectedPosition, int amount){
        for(int i = 0; i < amount; i++){
            GameObject gold = LeanPool.Spawn(animationGoldPrefabs, collectedPosition);
            
            //Move the gold using tweening
            float duration = Random.Range(minAnimDuration, maxAnimDuration);
            gold.transform.DOMove(targetPosition, duration)
            .SetEase(easeType)
            .OnComplete(() => {
                LeanPool.Despawn(gold, 0.2f);
                AddGold(1);
                GameManager.instance.SaveGold();
            });

        }
    }


    void UpdateText(){
        goldText.text = Golds.ToString();
    }

}
