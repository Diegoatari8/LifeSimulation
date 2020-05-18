using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Component that just manages the amount of entities living,mainly for debugging.
public class LifeManager : MonoBehaviour
{//Prefab of the entity
    public Object LifePrefab;
    //Instance of the Life Manager
    public static LifeManager instance;
    //Static variable of the life prefab for static voids

    //Current living entities
    public List<LifeEntity> Entities;
    public int Population;
    public int maxPopulation = 1000;
    //Population text for the demo scene
    public Text popText;
    //Simulation speed text for the demo scene
    public Text simSpeedText;
    // Start is called before the first frame update
    void Start()
    { //Set the global instance to this one
        LifeManager.instance = this;
        //Add existing entities into population
        LifeManager.instance.Entities.AddRange(FindObjectsOfType<LifeEntity>());
 
    }

    // Update is called once per frame
    void Update()
    { //Updates the shown count of entities in population
        Population = Entities.Count;
        //Text Updates
        popText.text = "Population: " + Population;
        simSpeedText.text = "Speed: " + Time.timeScale;
    }
    //Function called for spawning entities. 
    public static void Spawn(Vector3 position, float MHealth, float WHealth, float MAttract, float WAttract, Color MColor, Color WColor, GameObject Mother)
    { //Checks if the population has not reached the limit.
        if (LifeManager.instance.Population < LifeManager.instance.maxPopulation)
        {//Creates the newborn object
            GameObject Newborn = Instantiate(LifeManager.instance.LifePrefab, position, Quaternion.identity) as GameObject;
            //Sets parent to this gameobject
            Newborn.transform.parent = instance.transform;
            //Define the entity component once for not getting the component for each change to the variables
            LifeEntity Entity = Newborn.GetComponent<LifeEntity>();
            //Sets the mother of the new entity
            Entity.Mother = Mother;
            //Sets mental health to the average between mother`s and father`s mental health.
            Entity.mentalHealth = Mathf.Lerp(MHealth, WHealth, 0.5f);
            //Sets color to the average between mother`s and father`s color.
            Entity.color = Color.Lerp(MColor, WColor, 0.5f);
            //Sets attractiveness to the average between mother`s and father`s attractiveness
            Entity.attractive = Mathf.Lerp(MAttract, WAttract, 0.5f);
            //Add the instance to the living entities list.
            LifeManager.instance.Entities.Add(Entity);


        }
        else
        {
            Debug.Log("Maximum population reached!");
        }
    }
    public void SetMaxPopulation(string value)
    {
        maxPopulation = int.Parse(value);
    }
    public void ResetSimulation()
    {
        Application.LoadLevel(0);
    }
    public void SimSpeed(float speed)
    {
        Time.timeScale = speed;
    }
}
