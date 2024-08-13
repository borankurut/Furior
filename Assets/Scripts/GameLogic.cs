using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public GameObject RocksObject;

    private const int sceneAmount = 2;
    private string[] scenes = { "KnightScene", "MaulerScene" };
    private int currentSceneIx = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start(){
        RocksObject = GameObject.Find("Rocks");
    }

    void Update()
    {
        if(RocksObject == null){
            RocksObject = GameObject.Find("Rocks");
        }
        
        if (RocksObject.transform.childCount < 10)
        {
            currentSceneIx = nextSceneIx(currentSceneIx);
            SceneManager.LoadScene(scenes[currentSceneIx]);
            RocksObject = GameObject.Find("Rocks");
        }
    }

    int nextSceneIx(int currentSceneIx)
    {
        int next = currentSceneIx + 1;
        next %= sceneAmount;
        return next;
    }
}
