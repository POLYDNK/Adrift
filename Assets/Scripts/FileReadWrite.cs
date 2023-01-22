using System.IO;//use when accessing files on os
using System.Runtime.Serialization.Formatters.Binary; //Allows access and use of binary formatter
using UnityEngine;


//used for saving and loading data by first turning specific object into binary and then back
public static class FileReadWrite
{
  public static void WriteToBinaryFile<T>(string filePath, T objectToWrite) //method to write object of any type to chosen path
  {
    using (Stream stream = File.Open(filePath, FileMode.Create)) //if file exists, overwrite, otherwise create new file
    {
      var binaryFormatter = new BinaryFormatter(); //new object of type binaryformatter
      binaryFormatter.Serialize(stream, objectToWrite); //serialize converts file to binary
      //stream.Close() is not needed to be called explicitly, it is done automatically when using is done
    }
  }
  public static T ReadFromBinaryFile<T>(string filePath)
  {
    using (Stream stream = File.Open(filePath, FileMode.Open))
    {
      var binaryFormatter = new BinaryFormatter();
      return (T)binaryFormatter.Deserialize(stream);
    }
  }
}
