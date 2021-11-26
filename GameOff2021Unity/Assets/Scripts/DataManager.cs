using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DataManager : MonoBehaviour
{
  [SerializeField] private bool useFakeData;

  public static Consumable[] AllConsumables { get; private set; }
  public static Macro[] AllMacros { get; private set; }
  public static Stance[] AllStances { get; private set; }

  private void Awake()
  {
    if (useFakeData)
    {
      LoadFakeData();
      Serialize(AllConsumables, "/Data/fake_consumables.xml");
      Serialize(AllMacros, "/Data/fake_macros.xml");
      Serialize(AllStances, "/Data/fake_stances.xml");
    }
    else
    {
      AllConsumables = Deserialize<Consumable[]>("/Data/consumables.xml");
      AllMacros = Deserialize<Macro[]>("/Data/macros.xml");
      AllStances = Deserialize<Stance[]>("/Data/stances.xml");
    }
  }

  private static void LoadFakeData()
  {
    var consumables = new List<Consumable>();
    for (var i = 0; i < 10; i++)
    {
      consumables.Add(new Consumable
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
      macros.Add(new Macro
      {
        id = i,
        name = "Macro " + i,
        description = "This is macro " + i + ".",
        patternId = 3
      });
    }

    AllMacros = macros.ToArray();

    var stances = new List<Stance>();
    for (var i = 0; i < 10; i++)
    {
      stances.Add(new Stance
      {
        id = i,
        name = $"Stance {i}",
        description = $"This is stance {i}."
      });
    }

    AllStances = stances.ToArray();
  }

  private static void Serialize(object toSerialize, string path)
  {
    var serializer = new XmlSerializer(toSerialize.GetType());
    using var writer = new StreamWriter(Application.dataPath + path);
    serializer.Serialize(writer.BaseStream, toSerialize);
  }

  private static T Deserialize<T>(string path)
  {
    var serializer = new XmlSerializer(typeof(T));
    using var reader = new StreamReader(Application.dataPath + path);
    return (T) serializer.Deserialize(reader.BaseStream);
  }
}
