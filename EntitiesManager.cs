using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EntityManager : IEnumerable<Entity>
{
    public List<Entity> NPCs = new List<Entity>();
    public List<Player> Players = new List<Player>();

    public IEnumerator<Entity> GetEnumerator()
    {
        foreach (var npc in NPCs) yield return npc;
        foreach (var player in Players) yield return player;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => NPCs.Count + Players.Count;
    
    public Entity GetByIndex(int index)
    {
        if (index < NPCs.Count) return NPCs[index];
        return Players[index - NPCs.Count];
    }

	public void Add(Entity entity){
        if (entity is Player player){
			Players.Add(player);
        }else{
            NPCs.Add(entity);
        }
    }
	
	public Entity this[int index]{
        get{
            if (index < NPCs.Count)
                return NPCs[index];

            int playerIndex = index - NPCs.Count;

            if (playerIndex < Players.Count)
				return Players[playerIndex];

            throw new System.IndexOutOfRangeException("Entity index out of range.");
        }
    }
}
