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
    //        "Jack","Oliver","James","Charlie","Harris","Lewis","Leo","Noah","Alfie","Rory","Alexander","Max",
    //        "Logan","Lucas","Harry","Theo","Thomas","Brodie","Archie","Jacob","Finlay","Finn","Daniel","Joshua",
    //        "Oscar","Arthur","Hunter","Ethan","Mason","Harrison","Freddie","Ollie","Adam","William","Jaxon",
    //        "Aaron","Cameron","Liam","George","Jamie","Callum","Matthew","Muhammad","Caleb","Nathan","Tommy",
    //        "Carter","Blake","Andrew","Luke","Jude","Angus","Riley","Luca","Samuel","Joseph","David","Isaac",
    //        "Ryan","Hamish","Sonny","Arlo","Arran","Cole","Robert","Blair","Dylan","Louie","Ruaridh","Connor",
    //        "Benjamin","Kai","Michael","Jackson","Leon","Cooper","Louis","Theodore","Fraser","Owen","Reuben",
    //        "John","Carson","Innes","Elijah","Murray","Grayson","Aiden","Aidan","Cody","Elliot","Ben","Henry",
    //        "Sam","Alex","Ellis","Gabriel","Jax","Callan","Ruairidh","Frankie","Lachlan","Roman","Brody","Josh",
    //        "Sebastian","Struan","Evan","Kyle","Myles","Calum","Lochlan","Jayden","Lyle","Robbie","Calvin",
    //        "Corey","Jaxson","Christopher","Teddy","Eli","Marcus","Parker","Tyler","Euan","Hudson","Joey",
    //        "Austin","Zac","Dominic","Kayden","Zack","Harvey","Rowan","Hugo","Edward","Fergus","Finley","Patrick",
    //        "Cillian","Conor","Olivia","Emily","Isla","Sophie","Ella","Ava","Amelia","Grace","Freya","Charlotte",
    //        "Jessica","Lucy","Ellie","Sophia","Aria","Lily","Harper","Mia","Rosie","Millie","Evie","Eilidh","Ruby",
    //        "Willow","Anna","Maisie","Hannah","Eva","Chloe","Mila","Orla","Isabella","Ivy","Emma","Georgia",
    //        "Poppy","Robyn","Daisy","Zara","Gracie","Holly","Skye","Esme","Sofia","Erin","Hallie","Molly","Ayla",
    //        "Emilia","Layla","Katie","Sienna","Niamh","Alice","Amber","Bonnie","Maya","Zoe","Ada","Hollie","Bella",
    //        "Luna","Thea","Rose","Abigail","Summer","Callie","Hope","Lexi","Iona","Elsie","Leah","Scarlett","Julia",
    //        "Violet","Myla","Harley","Eve","Imogen","Elizabeth","Cora","Florence","Georgie","Lilly","Matilda",
    //        "Mirren","Phoebe","Rowan","Lola","Aurora","Evelyn","Brooke","Clara","Lucie","Sadie","Cara","Darcy",
    //        "Nova","Penelope","Abbie","Aila","Ailsa","Aoife","Lottie","Lyla","Maria","Alba","Arya","Eden","Lucia",
    //        "Penny","Remi","Flora","Heidi","Mollie","Sarah"
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
