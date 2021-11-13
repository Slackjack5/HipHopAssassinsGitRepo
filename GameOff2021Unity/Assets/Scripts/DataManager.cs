using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DataManager : MonoBehaviour
{
  [SerializeField] private bool useFakeData;

  public static Consumable[] AllConsumables { get; private set; }
  public static Macro[] AllMacros { get; private set; }

  private void Awake()
  {
    if (useFakeData)
    {
      LoadFakeData();
    }
    else
    {
      var serializer = new XmlSerializer(typeof(Consumable[]));
      var reader = new StreamReader(Application.dataPath + "/Data/consumables.xml");
      AllConsumables = (Consumable[])serializer.Deserialize(reader.BaseStream);
      reader.Close();
    }
  }

  private static void LoadFakeData()
  {
    var consumables = new List<Consumable>();
    for (var i = 0; i < 10; i++)
    {
      consumables.Add(new Consumable() 
      { 
        id = i, 
        name = "Consumable " + i, 
        description = "This is consumable " + i + "."
      });
    }
    AllConsumables = consumables.ToArray();

    var macros = new List<Macro>();
    for (var i = 0; i < 10; i++)
    {
      macros.Add(new Macro()
      {
        id = i,
        name = "Macro " + i,
        description = "This is macro " + i + "."
      });
    }
    AllMacros = macros.ToArray();
  }
}
