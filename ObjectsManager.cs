
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectsManager : IEnumerable<Object>
{
    public List<Object> Objects = new List<Object>();
    public EntityManager Entities = new EntityManager();

    public IEnumerator<Object> GetEnumerator()
    {
        foreach (var obj in Objects) yield return obj;
        foreach (var ent in Entities) yield return ent;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Objects.Count + Entities.Count;
    
    public Object GetByIndex(int index){
        if (index < Objects.Count) return Objects[index];
        return Entities[index - Objects.Count];
    }

	public void Add(Object obj){
        if (obj is Entity entity){
			Entities.Add(entity);
        }else{
            Objects.Add(obj);
        }
    }

	public Object this[int index]{
        get{
            if (index < Objects.Count) return Objects[index];

            int entityIndex = index - Objects.Count;

            if (entityIndex < Entities.Count)
				return Entities[entityIndex];
            throw new System.IndexOutOfRangeException("Entity index out of range.");
        }
    }
}
