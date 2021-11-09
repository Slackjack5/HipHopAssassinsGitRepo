using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DataManager : MonoBehaviour
{
  [SerializeField] private bool useFakeData = false;

  public static Consumable[] AllConsumables { get; private set; }
  public static Spell[] AllSpells { get; private set; }

  private void Awake()
  {
    if (useFakeData)
    {
      LoadFakeData();
    }
    else
    {
      XmlSerializer serializer = new XmlSerializer(typeof(Consumable[]));
      StreamReader reader = new StreamReader(Application.dataPath + "/Data/consumables.xml");
      AllConsumables = (Consumable[])serializer.Deserialize(reader.BaseStream);
      reader.Close();
    }
  }

  private void LoadFakeData()
  {
    List<Consumable> consumables = new List<Consumable>();
    for (int i = 0; i < 10; i++)
    {
      consumables.Add(new Consumable() { 
        id = i, 
        name = "Consumable " + i, 
        description = "This is consumable " + i + "."
      });
    }
    AllConsumables = consumables.ToArray();

    List<Spell> spells = new List<Spell>();
    for (int i = 0; i < 10; i++)
    {
      spells.Add(new Spell()
      {
        id = i,
        name = "Spell " + i,
        description = "This is spell " + i + "."
      });
    }
    AllSpells = spells.ToArray();
  }
}
