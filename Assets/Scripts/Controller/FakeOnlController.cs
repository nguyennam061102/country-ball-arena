using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class FakeOnlController : SingletonMonoBehavior<FakeOnlController>
{
    [SerializeField] private bool needDontDestroy = false;
    [SerializeField] List<string> nameOfAI;
    [SerializeField] List<int> nameInGame;
    [SerializeField] List<Sprite> emoji;
    int randomIndex;

    public List<Sprite> Emoji { get => emoji; set => emoji = value; }

    protected override void Awake()
    {
        base.Awake();
        if (needDontDestroy) DontDestroyOnLoad(gameObject);
        GetRandomeName();
    }
    // Start is called before the first frame update
    public string NameRandom()
    {
        string newName;
        newName = nameOfAI[nameInGame[0]];
        nameInGame.RemoveAt(0);
        return newName;
    }
    
    //[Button]
    //void Test()
    //{
    //    nameOfAI = new List<string>()
    //    {
    //        "John",
    //        "Mary",
    //        "David",
    //        "Jennifer",
    //        "Michael",
    //        "Sarah",
    //        "James",
    //        "Emily",
    //        "Robert",
    //        "Jessica",
    //        "William",
    //        "Ashley",
    //        "Christopher",
    //        "Amanda",
    //        "Matthew",
    //        "Elizabeth",
    //        "Daniel",
    //        "Samantha",
    //        "Joseph",
    //        "Nicole",
    //        "Andrew",
    //        "Lauren",
    //        "Joshua",
    //        "Megan",
    //        "Ryan",
    //        "Rachel",
    //        "Nicholas",
    //        "Tiffany",
    //        "Tyler",
    //        "Michelle",
    //        "Brandon",
    //        "Stephanie",
    //        "Jonathan",
    //        "Heather",
    //        "Justin",
    //        "Melissa",
    //        "Zachary",
    //        "Amber",
    //        "Kevin",
    //        "Brittany",
    //        "Eric",
    //        "Christina",
    //        "Brian",
    //        "Kimberly",
    //        "Alexander",
    //        "Courtney",
    //        "Jacob",
    //        "Danielle",
    //        "Benjamin",
    //        "Victoria"
    //    };
    //}
    public void GetRandomeName()
    {
        for (int i = 0; i < 3; i++)
        {
            do
            {
                randomIndex = Random.Range(0, nameOfAI.Count);
            }
            while (nameInGame.Contains(randomIndex)); 
            nameInGame.Add(randomIndex); 
        }
    }
}
