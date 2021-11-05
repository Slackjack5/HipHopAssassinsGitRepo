using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class XmlManager : MonoBehaviour
{
  public static Consumable[] Consumables { get; private set; }

  private void Awake()
  {
    XmlSerializer serializer = new XmlSerializer(typeof(Consumable[]));
    StreamReader reader = new StreamReader(Application.dataPath + "/Data/consumables.xml");
    Consumables = (Consumable[]) serializer.Deserialize(reader.BaseStream);
    reader.Close();
  }
}
