using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class MapTweener : MonoBehaviour
{
    List<TweenPosition> objectListToTween;
    List<Vector3> originalPositions;
    List<Rigidbody2D> physicBlocks;
    List<Rigidbody2D> customPhysicBlocks;
    [SerializeField] float TimeTween = 0.25f;
    public UnityEvent onStartTweenShowEvent, onStartTweenHideEvent;
    public UnityEvent onDoneTweenShowEvent, onDoneTweenHideEvent;

    [Header("===TOOL===")] public List<GameObject> objList;
    public GameObject objToReplace;

    public bool IsThisMapPhysic()
    {
        return physicBlocks.Count != 0 || customPhysicBlocks.Count != 0;
    }

#if UNITY_EDITOR
    [ContextMenu("Skygo Game 2021")]
    void Execute()
    {
        foreach (var item in objList)
        {
            var newGo = Instantiate(objToReplace, item.transform.position, item.transform.rotation, transform);
            newGo.transform.SetParent(item.transform.parent);
            DestroyImmediate(item.gameObject);
            EditorUtility.SetDirty(newGo);
        }

        objList.Clear();
    }
#endif

    Rigidbody2D rb2d;
    private void Awake()
    {
        GameObject[] gameObjectList = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            gameObjectList[i] = transform.GetChild(i).gameObject;
        }

        gameObjectList = gameObjectList.OrderBy(go => go.transform.position.x).ToArray();
        TweenPosition tmp = null;
        objectListToTween = new List<TweenPosition>();
        originalPositions = new List<Vector3>();
        physicBlocks = new List<Rigidbody2D>();
        customPhysicBlocks = new List<Rigidbody2D>();
        for (int i = 0; i < gameObjectList.Length; i++)
        {
            Transform child = gameObjectList[i].transform;
            if (child.GetComponent<Collider2D>() != null)
            {
                child.tag = "Block";
                if (child.GetComponent<Rigidbody2D>() != null && !child.name.Contains("saw") && !child.name.Contains("tamgiac"))
                {
                    rb2d = child.GetComponent<Rigidbody2D>();
                    rb2d.mass = Random.Range(20000, 40000);
                    rb2d.drag = 0.2f;
                    rb2d.angularDrag = 0.2f;
                    physicBlocks.Add(rb2d);
                }
                tmp = child.GetComponent<TweenPosition>();
                if (tmp == null) tmp = child.gameObject.AddComponent<TweenPosition>();
                tmp.from = child.transform.position;
                tmp.to = child.transform.position;
                tmp.duration = TimeTween * 0.75f;
                tmp.enabled = false;
                objectListToTween.Add(tmp);
                originalPositions.Add(child.position);
            }
        }
        listSpawnPositions = new List<Vector3>();
        foreach (SpawnPoint sp in GetComponentsInChildren<SpawnPoint>())
        {
            listSpawnPositions.Add(sp.localStartPos);
        }
        GameUtils.Shuffle(listSpawnPositions);

        foreach (SpriteMask sprMask in GetComponentsInChildren<SpriteMask>()) sprMask.enabled = false;
    }

    private void Start()
    {
        if (gameController.startGame) onDoneTweenShowEvent.AddListener(gameController.StartRound);
        else
        {
            onDoneTweenShowEvent.AddListener(GetPanelSelectCard);
        }
        onDoneTweenHideEvent.AddListener(DestroyMap);
    }

    public void ShowMap()
    {
        onStartTweenShowEvent?.Invoke();
        StartCoroutine(coroShowMap());
    }

    IEnumerator coroShowMap()
    {
        //MapController.Instance.SetupParticles();
        foreach (Rigidbody2D rb in physicBlocks)
        {
            rb.isKinematic = true;
        }
        for (int i = 0; i < objectListToTween.Count; i++)
        {
            TweenPosition tmp = objectListToTween[i];
            Vector3 orgPos = originalPositions[i];
            tmp.from = orgPos + new Vector3(50f, 0, 0);
            tmp.to = orgPos;
            tmp.ResetToBeginning();
        }
        for (int i = 0; i < objectListToTween.Count; i++)
        {
            TweenPosition tmp = objectListToTween[i];
            tmp.PlayForward();
            yield return new WaitForSeconds(TimeTween * 0.1f);
        }
        yield return new WaitForSeconds(TimeTween);
        foreach (Rigidbody2D rb in physicBlocks)
        {
            if (rb.GetComponent<DropPlatform>() == null && rb.GetComponent<ForceField>() == null) rb.isKinematic = false;
        }
        onDoneTweenShowEvent?.Invoke();
    }

    public void HideMap()
    {
        onStartTweenHideEvent?.Invoke();
        StartCoroutine(coroHideMap());
    }

    IEnumerator coroHideMap()
    {
        foreach (Rigidbody2D rb in physicBlocks) rb.isKinematic = true;
        for (int i = 0; i < objectListToTween.Count; i++)
        {
            TweenPosition tmp = objectListToTween[i];
            Vector3 orgPos = originalPositions[i];
            tmp.from = tmp.transform.position;
            tmp.to = tmp.transform.position + new Vector3(-50f, 0, 0);
            tmp.ResetToBeginning();
        }

        for (int i = 0; i < objectListToTween.Count; i++)
        {
            TweenPosition tmp = objectListToTween[i];
            tmp.PlayForward();
            yield return new WaitForSeconds(TimeTween * 0.1f);
        }
        yield return new WaitForSeconds(TimeTween);
        onDoneTweenHideEvent?.Invoke();
    }

    public void AddPhysicBlock(Rigidbody2D rb)
    {
        rb.tag = "Block";
        rb.mass = Random.Range(2000, 4000);
        //rb.drag = 0.2f;
        //rb.angularDrag = 0.2f;
        customPhysicBlocks.Add(rb);
    }

    public void RemoveAllPhysicBlock()
    {
        foreach (Rigidbody2D rb2d in customPhysicBlocks)
        {
            rb2d.gameObject.SetActive(false);
        }
    }

    private GameController gameController => GameController.Instance;
    private PlayerManager playerManager => PlayerManager.Instance;
    private UiController UI => UiController.Instance;

    private void GetPanelSelectCard() //gọi khi chuyển map trong game
    {
        UI.GetPanel(PanelName.PanelSelectCard).GetComponent<PanelSelectCard>().StartShowCard(3, 1);
    }

    private void DestroyMap()
    {
        MapController.Instance.SpawnMap(true);
        DestroyImmediate(gameObject);
    }

    List<Vector3> listSpawnPositions;

    public Vector3 GetRespawnPosition()
    {
        Vector3 result = listSpawnPositions[0];
        listSpawnPositions.RemoveAt(0);
        GameUtils.Shuffle(listSpawnPositions);
        listSpawnPositions.Add(result);
        return result;
    }
}