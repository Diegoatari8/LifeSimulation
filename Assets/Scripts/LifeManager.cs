using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Component that just manages the amount of entities living,mainly for debugging.
public class LifeManager : MonoBehaviour
{//Prefab of the entity
    public Object LifePrefab;
    //Instance of the Life Manager
    public static LifeManager instance;
    //Static variable of the life prefab for static voids
    public static Object EntityPrefab;
    //Current living entities
    public List<LifeEntity> Entities;
    public int Population;
    
    // Start is called before the first frame update
    void Start()
    { //Set the global instance to this one
        LifeManager.instance = this;
        //Add existing entities into population
        LifeManager.instance.Entities.AddRange(FindObjectsOfType<LifeEntity>());
        //Set the Entity Prefab for static methods;
        EntityPrefab = LifePrefab;
    }

    // Update is called once per frame
    void Update()
    { //Updates the shown count of entities in population
        Population = Entities.Count;
    }
    //Function called for spawning entities. 
    public static void Spawn(Vector3 position, float MHealth, float WHealth,float MAttract,float WAttract, Color MColor, Color WColor, GameObject Mother) 
    { //Creates the newborn object
        GameObject Newborn = Instantiate(LifeManager.EntityPrefab, position,Quaternion.identity) as GameObject;
       //Sets parent to this gameobject
        Newborn.transform.parent = instance.transform;
        //Define the entity component once for not getting the component for each change to the variables
       LifeEntity Entity = Newborn.GetComponent<LifeEntity>();
        //Sets the mother of the new entity
        Entity.Mother = Mother;
        //Sets mental health to the average between mother`s and father`s mental health.
        Entity.mentalHealth = Mathf.Lerp(MHealth, WHealth,0.5f);
        //Sets color to the average between mother`s and father`s color.
        Entity.color = Color.Lerp(MColor, WColor,0.5f);
        //Sets attractiveness to the average between mother`s and father`s attractiveness
        Entity.attractive = Mathf.Lerp(MAttract, WAttract, 0.5f);
        //Add the instance to the living entities list.
        LifeManager.instance.Entities.Add(Entity);

      
    }
}
