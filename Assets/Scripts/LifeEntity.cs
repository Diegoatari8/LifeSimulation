using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//Main script used by entities to behave.
public class LifeEntity : MonoBehaviour
//Definition of Genders and Sexualities
{
    public enum Gender
    {
        Male,
        Female
    }
    public enum Sexuality
    {
        Hetero,
        Homo,
        Bi
    }
    public Gender gender;

    int gend;
    public Sexuality sexuality;
    float sexu;
    public Color color;
    public int age;
    int deathChance;
    float ageTimer;
    //How attractive is the entity to others. Increases couple chance. (0-100%)
    public float attractive;
    //How mental healthy is our entity. Decreases chance of killing the couple or breaking up. (0-100%)
    public float mentalHealth;
    NavMeshAgent agent;
    MeshRenderer renderer;
    Collider collider;
    Vector3 target;
    LifeEntity couple;
    bool isCoupled;
    float babyTimer;
    public GameObject Mother;
    TrailRenderer trail;
    // Start is called before the first frame update
    void Start()
    {
        trail = GetComponentInChildren<TrailRenderer>();
        //If entity does not have parents to get their values averaged as variables,set random ones.
        if (Mother == null)
        {
            mentalHealth = Random.Range(25, 100);
            attractive = Random.Range(25, 100);
            color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)255);

        }
        trail.startColor = color;
        trail.endColor = color;
        deathChance = Random.Range(30, 100);

        //Chance to die on birth
        if (Random.Range(0, 100) < 1)
        {
            Die();
        }
        //Sets a random gender when spawned
        gend = Mathf.RoundToInt(Random.Range(0, 100));
        if (gend > 50)
        {
            gender = Gender.Male;
        }
        else
        {
            gender = Gender.Female;
        }
        //Sets a random sexuality when spawned. (Chances are: 66.66% Hetero,33.33% Homo and 33.33% Bi)
        sexu = Random.Range(0, 3);
        if (sexu < 2f)
        {
            sexuality = Sexuality.Hetero;
        }
        if (sexu >2f && sexu< 2.5f)
        {
            sexuality = Sexuality.Bi;
        }
        if (sexu >= 2.5f)
        {
            sexuality = Sexuality.Homo;
        }

        //Define values;
        agent = GetComponent<NavMeshAgent>();
     

        renderer = GetComponent<MeshRenderer>();
        renderer.material.color = color;
        collider = GetComponent<Collider>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Set entity size according to age
        transform.localScale = Vector3.one * 0.33f + Vector3.one * age/40;
        trail.widthMultiplier = .35f + age / 40;
        //Only can couple after 14 simulation years (6 real life seconds)
        if (age > 14)
        {
            collider.enabled = true;

        }
        else
        {
            collider.enabled = false;
        }
        //Change path if already has path and is not moving. If coupled,the couple stays static.
        if (agent.velocity.magnitude < 0.1f && !isCoupled)
        {
            target = Vector3.Scale(Vector3.one, new Vector3(Random.Range(-33, 33), 0, Random.Range(-33, 33)));
            Debug.DrawLine(transform.position, target, Color.blue, .33f);
            agent.SetDestination(target);
        }
        //When coupled,stay still and start reproducing.
        if (isCoupled)
        {
            agent.SetDestination(transform.position);
            babyTimer += 0.02f;
            if (babyTimer > 3f)
            {
                babyTimer = 0;
                //Reproduces if genders are different
                if (gender != couple.gender)
                {
                    if (gender == Gender.Female)
                    {
                        LifeManager.Spawn(transform.position, mentalHealth, couple.mentalHealth, attractive, couple.attractive, color, couple.color, this.gameObject);
                        //If reproduced successfully there is a 5% chance (if mental health of the couple is 100%) to die after giving birth.
                        if (Random.Range(0, couple.mentalHealth) < 5)
                        {
                            Die();
                        }
                    }

                }
                //There is a 10% chance (if mental health is 100%) of breaking up the couple.
                if (Random.Range(0, mentalHealth) < 10)
                {

                    couple.couple = null;
                    couple.isCoupled = false;
                    couple = null;
                    isCoupled = false;
                }


            }
        }
      

        //Aging
        ageTimer += 0.02f;
        if (ageTimer > 0.5f)
        {
            ageTimer = 0;
            age++;
            if (age > 30)
            { //The higher the age,the higher the chance to die depending on the random deathChance value.
                if (Random.Range(0, age) > deathChance)
                {
                    Die();
                }
            }
        }

    }
    private void OnCollisionEnter(Collision collision)
    { //Try to couple if the other collider is another entity and both are not coupled yet.
        if (collision.gameObject.GetComponent<LifeEntity>() != null && collision.gameObject.GetComponent<LifeEntity>().isCoupled == false && isCoupled == false)
        {
            LifeEntity other = collision.gameObject.GetComponent<LifeEntity>();
         //If genders and sexualities are compatible continue
            if (CheckGenderComp(this, other))
            {//If attraction average is higher than a random number,then couple
                if (Random.Range(0, 50) <= Mathf.Lerp(attractive, collision.gameObject.GetComponent<LifeEntity>().attractive, 0.5f))
                {
                    collision.gameObject.GetComponent<LifeEntity>().couple = this;
                    collision.gameObject.GetComponent<LifeEntity>().isCoupled = true;
                    couple = collision.gameObject.GetComponent<LifeEntity>();
                    isCoupled = true;
                }
            }
        }

    }



    //Sexuality and Gender compatibility check functions. 
    public static bool CheckGenderComp(Gender ent1, Gender ent2, Sexuality sex1, Sexuality sex2)
    { //Hetero & Bi
        if (ent1 != ent2 && sex1 == Sexuality.Hetero && sex2 == Sexuality.Hetero || ent1 != ent2 && sex1 == Sexuality.Hetero && sex2 == Sexuality.Bi || ent1 != ent2 && sex1 == Sexuality.Bi && sex2 == Sexuality.Hetero)
        {
            return true;

        }
        //Homo & Bi
        else if (ent1 == ent2 && sex1 == Sexuality.Homo && sex2 == Sexuality.Homo || ent1 == ent2 && sex1 == Sexuality.Bi && sex2 == Sexuality.Homo || ent1 == ent2 && sex1 == Sexuality.Homo && sex2 == Sexuality.Bi)
        {
            return true;
        }
        //Bi
        else if (sex1 == Sexuality.Bi && sex2 == Sexuality.Bi)
        {
            return true;
        }
        //Not compatible
        else
        {
            return false;
        }
    }
    //Same function but with less inputs,getting the variables from the entities themselves.
    public static bool CheckGenderComp(LifeEntity enti1, LifeEntity enti2)
    {
        Gender ent1 = enti1.gender;
        Gender ent2 = enti2.gender;
        Sexuality sex1 = enti1.sexuality;
        Sexuality sex2 = enti2.sexuality;
        //Hetero & Bi
        if (ent1 != ent2 && sex1 == Sexuality.Hetero && sex2 == Sexuality.Hetero || ent1 != ent2 && sex1 == Sexuality.Hetero && sex2 == Sexuality.Bi || ent1 != ent2 && sex1 == Sexuality.Bi && sex2 == Sexuality.Hetero)
        {
            return true;

        }
        //Homo & Bi
        else if (ent1 == ent2 && sex1 == Sexuality.Homo && sex2 == Sexuality.Homo || ent1 == ent2 && sex1 == Sexuality.Bi && sex2 == Sexuality.Homo || ent1 == ent2 && sex1 == Sexuality.Homo && sex2 == Sexuality.Bi)
        {
            return true;
        }
        //Bi
        else if (sex1 == Sexuality.Bi && sex2 == Sexuality.Bi)
        {
            return true;
        }
        //Not compatible
        else
        {
            return false;
        }
    }
    //Death function
    void Die()
    {//Removes entity from living entities list
        LifeManager.instance.Entities.Remove(this);
        //if has a couple break up 
        if (isCoupled)
        {
            couple.couple = null;
            couple.isCoupled = false;
            couple = null;
            isCoupled = false;
        }
        //Destroy the object.
        Destroy(this.gameObject);
    }

}
