using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WhiteWorld.utility;

namespace WhiteWorld.engine.interfaces;

public interface ISerializable {
    string Identifier { get; }
    void Save(DataBuffer buffer);
    void Load(DataBuffer? buffer);
}

public class DataBuffer : List<object> {
    public void WriteToDisk(string fileName) {
        var json = JsonConvert.SerializeObject(this);
        File.WriteAllText(@$"{fileName}.json", json);
    }

    public static DataBuffer? ReadFromDisk(string fileName) {
        DataBuffer? buffer;
        try {
            var json = File.ReadAllText(@$"{fileName}.json");
            buffer = JsonConvert.DeserializeObject<DataBuffer>(json);
        }
        catch (FileNotFoundException e) {
            Engine.Error($"File {e.FileName} not found. Returning null.");
            return null;
        }
        catch (Exception) {
            Engine.Error($"File {fileName}.json could not be read");
            return null;
        }
        return buffer;
    }

    public List<T> ReadList<T>() {
        return ((JArray) this.Pop()).ToObject<List<T>>()!;
    }
}